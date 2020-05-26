using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using DAL;

namespace BLL
{
    public class DriveService
    {
        private IDriveService _service;
        private const string _commandFileName = "cmd.cmd";
        private const int _maxCommandHandlingIterations = 10000000;
        private BaseSiteSaverBehavior _baseSiteSaverBehavior;

        public enum Commands
        {
            Default,
            OpenSite,
            OpenSiteFull
        }

        public DriveService(ISiteSaverBehavior behavior)
        {
            _baseSiteSaverBehavior = new BaseSiteSaverBehavior(behavior);

            _service = new DAL.DriveService();
        }

        // server side
        public void StartServer()
        {
            List<DAL.File> useless = _service.GetFiles(q: "name = '" + _commandFileName + "'").ToList();
            foreach (var v in useless)
                _service.DeleteFile(v);

            while (true)
            {
                List<DAL.File> commands = _service.GetFiles(q: "name = '" + _commandFileName + "'").ToList();
                
                foreach(var v in commands)
                {
                    ExecuteCommand(v);
                }
            }
        }

        // server side
        private void ExecuteCommand(DAL.File file)
        {
            _service.DownloadFile(file);

            ExecuteCommandByData(System.IO.File.ReadLines(file.Name).ToArray());

            _service.DeleteFile(file);
            System.IO.File.Delete(file.Name);
        }

        private void ExecuteCommandByData(string[] data)
        {
            if(data.Length == 3)
            {
                if (data[0] == Commands.OpenSite.ToString())
                    OpenSite(data);
                if (data[0] == Commands.OpenSiteFull.ToString())
                    OpenSiteFull(data);
            }
        }

        private void OpenSite(string[] data)
        {
            string saveFile = "OpenSite_" + data[2];

            using (WebClient client = new WebClient()) // WebClient class inherits IDisposable
            {
                client.DownloadFile(data[1], saveFile);
            }
            _service.UploadFile(data[2], saveFile, GetContetTypeForCommand(data[0]));
        }

        private void OpenSiteFull(string[] data)
        {
            string saveFile = "OpenSite_" + data[2];
            new SiteSaver().DownloadSite(data[1], saveFile);
            _service.UploadFile(data[2], saveFile, GetContetTypeForCommand(data[0]));
        }

        // Generate a random string with a given size  
        public string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        private string GetExtForCommand(Commands command)
        {
            if (command == Commands.OpenSite)
                return ".html";
            if (command == Commands.OpenSiteFull)
                return ".mhtml";
            return ".txt";
        }

        private string GetContetTypeForCommand(Commands command)
        {
            if (command == Commands.OpenSite)
                return "text/html";
            if (command == Commands.OpenSiteFull)
                return "multipart/related";
            return "text/cmd";
        }

        private string GetContetTypeForCommand(string command)
        {
            if (command == Commands.OpenSite.ToString())
                return "text/html";
            if (command == Commands.OpenSiteFull.ToString())
                return "multipart/related";
            return "text/cmd";
        }

        // client side
        public void RunCommand(Commands command, string target)
        {
            string resultFileName = RandomString(10, false) + GetExtForCommand(command);

            using(FileStream f = new FileStream("run_" + _commandFileName, FileMode.Create, FileAccess.ReadWrite))
            {
                using(StreamWriter sw = new StreamWriter(f))
                {
                    sw.WriteLine(command.ToString());
                    sw.WriteLine(target);
                    sw.WriteLine(resultFileName);
                }
            }

            _service.UploadFile(_commandFileName, "run_" + _commandFileName, GetContetTypeForCommand(Commands.Default));
            HandleCommandResult(command, resultFileName);
        }

        public void HandleCommandResult(Commands command, string file)
        {
            int i = 0;
            DAL.File result = null;
            while((result = _service.GetFiles(q:"name = '" + file + "'").FirstOrDefault()) == null && i < _maxCommandHandlingIterations)
            {
                i++;
            }
            if (result != null)
            {
                _service.DownloadFile(result);
                HandleCommandFile(command, result.Name);
            }
            else
            {
                Console.WriteLine("Command '" + command + "' has been cancelled by server!");
            }
        }

        private void HandleCommandFile(Commands command, string fileName)
        {
            if(command == Commands.OpenSite)
            {
                HandleOpenSite(fileName);
            }
            if(command == Commands.OpenSiteFull)
            {
                HandleOpenSiteFull(fileName);
            }
        }

        private void HandleOpenSite(string fileName)
        {
            Process.Start(new ProcessStartInfo(fileName));
        }

        private void HandleOpenSiteFull(string fileName)
        {
            Process.Start(new ProcessStartInfo(fileName));
        }
    }
}
