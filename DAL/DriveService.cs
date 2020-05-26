using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using GDriveService = Google.Apis.Drive.v3.DriveService;
using GFile = Google.Apis.Drive.v3.Data.File;

namespace DAL
{
    public class DriveService : IDriveService
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/drive-dotnet-quickstart.json
        private string[] _scopes = { GDriveService.Scope.Drive };
        private string _appName = "DriveTransfer";
        private GDriveService _service;

        public DriveService ()
        {
            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    _scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            // Create Drive API service.
            _service = new GDriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = _appName,
            });
        }

        public IEnumerable<File> GetFiles(int pageSize = 10, string fields = "nextPageToken, files(id, name)", string q = "")
        {
            // Define parameters of request.
            FilesResource.ListRequest listRequest = _service.Files.List();
            listRequest.PageSize = pageSize;
            listRequest.Fields = fields;
            listRequest.Q = q;

            // List files.
            return (from x in listRequest.Execute().Files
                    select new File()
                    {
                        Id = x.Id,
                        Name = x.Name
                    });
        }

        public void UploadFile(string filename, string filePath, string contentType)
        {
            var uploadStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            // Get the media upload request object.
            var insert = _service.Files.Create(
                new GFile
                {
                    Name = filename,
                },
                uploadStream,
                contentType);

            insert.Upload();
            uploadStream.Dispose();
        }

        public void DownloadFile(File file)
        {
            // Get the client request object for the bucket and desired object.
            var getRequest = _service.Files.Get(file.Id);
            using (var fileStream = new System.IO.FileStream(
                file.Name,
                System.IO.FileMode.Create,
                System.IO.FileAccess.Write))
            {
                getRequest.Download(fileStream);
            }
        }

        public void DeleteFile(File file)
        {
            _service.Files.Delete(file.Id).Execute();
        }
    }
}
