using System;
using System.Collections.Generic;
using System.Text;

namespace DAL
{
    public interface IDriveService
    {
        IEnumerable<File> GetFiles(int pageSize = 10, string fields = "nextPageToken, files(id, name)", string q = "");
        void UploadFile(string filename, string filePath, string contentType);
        void DownloadFile(File file);
        void DeleteFile(File file);
    }
}
