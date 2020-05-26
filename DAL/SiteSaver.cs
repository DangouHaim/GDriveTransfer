using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public interface ISiteBehavior
    {
        void PressSavingCombination();
        void PressAddressCombination(string address);
        void PressTypeSelectionCombination();
        void PressEnter();
    }
    public class SiteSaver
    {
        private static ISiteBehavior _behavior;

        public static void SetBehavior(ISiteBehavior behavior)
        {
            _behavior = behavior;
        }

        public void DownloadSite(string url, string path)
        {
            string folder = Directory.GetCurrentDirectory() + "\\" + path;

            Process.Start(new ProcessStartInfo(url));// load site

            Task.Delay(5000).Wait();

            _behavior.PressSavingCombination();
            Task.Delay(1000).Wait();

            _behavior.PressAddressCombination(folder);
            Task.Delay(1000).Wait();

            _behavior.PressTypeSelectionCombination();
            Task.Delay(1000).Wait();

            _behavior.PressEnter();
            Task.Delay(3000).Wait();
        }
    }
}
