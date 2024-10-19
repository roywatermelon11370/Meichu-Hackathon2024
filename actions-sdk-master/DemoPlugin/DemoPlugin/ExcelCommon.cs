namespace Loupedeck.DemoPlugin
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Runtime.InteropServices;
    using System.Threading;

    using Loupedeck;
    using Loupedeck.DemoPlugin;

    public class ExcelCommon : PluginDynamicFolder
    {

        private DemoPlugin DemoPlugin => this.Plugin as DemoPlugin; 
        public ExcelCommon()
        {
            this.DisplayName = "dyna";
            this.GroupName = "dyna";
        }
        protected PluginDynamicFolderNavigation GetNavigationArea() => PluginDynamicFolderNavigation.None; // 設置為 ButtonArea，這將在觸控頁面的左上角自動添加返回按鈕

        public string Action = "select";

        public async void setAction(string msg) {
            this.Action = msg;
        }

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        private const byte VK_CONTROL = 0x11;
        private const byte VK_C = 0x43;
        private const uint KEYEVENTF_KEYUP = 0x0002;

        public static void SimulateCtrlC()
        {
            // 按下 Ctrl
            keybd_event(VK_CONTROL, 0, 0, UIntPtr.Zero);
            // 按下 C
            keybd_event(VK_C, 0, 0, UIntPtr.Zero);
            // 松开 C
            keybd_event(VK_C, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            // 松开 Ctrl
            keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        }

        // public void UpdateAction(string newAction) {
        //     PluginLog.Info($"Excel action updated to {Action}");
        //     GetButtonPressActionNames();
        // }
        
        public event System.EventHandler ActionChanged;

        public Int32 counter = 0;
        public String[] buttons;

        private int i = 0;

        // Define the buttons for the touch page
        public override IEnumerable<String> GetButtonPressActionNames()
        {
            IEnumerable<String> tmp;
            // this.action = this.DemoPlugin.GetMessage();
            switch(this.Action) {
                case "select":
                    this.buttons = new String[]
                        {"rot"     ,"close"
                ,"send"     ,""     ,"change"
                ,"cut"  ,"copy" ,"paste"};
                break;
                // case "select":
                //     this.buttons = new String[]
                //         {"rot"     ,"close"
                // ,"send"     ,""     ,"change"
                // ,"cut"  ,"copy" ,"paste"};
                // break;
                default:
                this.buttons = new String[]
                        {"rot"     ,"close"
                ,"send"     ,""     ,"change"
                ,"1"  ,"2" ,"3"};
                break;

            }
            // if (this.action == "1")//常用
            // {
                
            // }
            // else
            // {
            //     this.buttons = new String[]
            //             {"rot"     ,"close"
            //     ,"send"     ,""     ,"change"
            //     ,"1"    ,"2"    ,"3"};
            // }
            return new[]
                {
                    PluginDynamicFolder.NavigateUpActionName,
                    this.CreateAdjustmentName(this.buttons[0]),
                    this.CreateCommandName(this.buttons[1]),
                    this.CreateCommandName(this.buttons[2]),
                    this.CreateCommandName(this.buttons[3]),
                    this.CreateCommandName(this.buttons[4]),
                    this.CreateCommandName(this.buttons[5]),
                    this.CreateCommandName(this.buttons[6]),
                    this.CreateCommandName(this.buttons[7]),
                };
        }
        public override void RunCommand(String actionParameter)
        {
            switch (actionParameter)
            {
                case "close":
                    PluginLog.Info("close");
                    this.Close();
                    break;
                case "cut":
                    PluginLog.Info("cut");
                    break;
                case "copy":
                    PluginLog.Info("copy");
                    SimulateCtrlC();
                    break;
                case "paste":
                    PluginLog.Info("paste");
                    break;
                case "change":
                    this.Action = "-1";
                    break;
                case "send":
                    this.DemoPlugin.GetServer().SendMessage(i.ToString());
                    i++;
                    break;
                case "rot":
                    //PluginLog.Info();
                    break;
                default:
                    break;
            }
            this.ButtonActionNamesChanged();
        }
    }

}

