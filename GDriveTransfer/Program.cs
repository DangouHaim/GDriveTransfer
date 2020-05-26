using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using BLL;
using WindowsInput;
using WindowsInput.Native;

namespace GDriveTransfer
{
    class SiteSaver : BLL.ISiteSaverBehavior
    {
        InputSimulator _service;

        public SiteSaver()
        {
            _service = new InputSimulator();
        }

        public void PressAddressCombination(string address)
        {
            Clipboard.SetText(address);
            _service.Keyboard.ModifiedKeyStroke(VirtualKeyCode.LCONTROL, VirtualKeyCode.VK_V);
        }

        public void PressEnter()
        {
            _service.Keyboard.KeyPress(VirtualKeyCode.RETURN);
            Task.Delay(1000).Wait();
            _service.Keyboard.ModifiedKeyStroke(VirtualKeyCode.LCONTROL, VirtualKeyCode.F4);
        }

        public void PressSavingCombination()
        {
            _service.Keyboard.ModifiedKeyStroke(VirtualKeyCode.LCONTROL, VirtualKeyCode.VK_S);
        }

        public void PressTypeSelectionCombination()
        {
            _service.Keyboard.KeyPress(VirtualKeyCode.TAB);
            Task.Delay(300).Wait();
            _service.Keyboard.KeyPress(VirtualKeyCode.DOWN);
            Task.Delay(300).Wait();
            _service.Keyboard.KeyPress(VirtualKeyCode.DOWN);
            Task.Delay(300).Wait();
            _service.Keyboard.KeyPress(VirtualKeyCode.DOWN);
            Task.Delay(300).Wait();
            _service.Keyboard.KeyPress(VirtualKeyCode.UP);
            Task.Delay(300).Wait();
            _service.Keyboard.KeyPress(VirtualKeyCode.TAB);
        }
    }

    class Program
    {
        private const bool _serverDebug = false;
        private const bool _clientFullDebug = true;

        [STAThread]
        static void Main(string[] args)
        {
            Start(args);
        }

        private static void Start(string[] args)
        {
            if (args.Length == 0 && !_serverDebug && !_clientFullDebug)
                return;

            var s = new DriveService(new SiteSaver());

            if (_serverDebug || args.Length > 0 && args[0] == "-s")
            {
                s.StartServer();
                Console.ReadLine();
            }

            if (_clientFullDebug || args[0] == "-c" && args[1] == "-f")
            {
                while (true)
                {
                    s.RunCommand(DriveService.Commands.OpenSiteFull, Console.ReadLine());
                }
            }

            if (args[0] == "-c")
            {
                while (true)
                {
                    s.RunCommand(DriveService.Commands.OpenSite, Console.ReadLine());
                }
            }
        }
    }
}
