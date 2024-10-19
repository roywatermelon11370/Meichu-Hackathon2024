namespace Loupedeck.MyProjectPlugin
{
    using System;
    using System.Collections.Generic;
    using System.Windows;

    using Loupedeck;
    using Loupedeck.DemoPlugin;

    public class ExcelCommon : PluginDynamicFolder
    {
        public ExcelCommon()
        {
            this.DisplayName = "dyna";
            this.GroupName = "dyna";
        }
        protected PluginDynamicFolderNavigation GetNavigationArea() => PluginDynamicFolderNavigation.None; // 設置為 ButtonArea，這將在觸控頁面的左上角自動添加返回按鈕

        public Int32 action = 1;
        public Int32 counter = 0;
        public String[] buttons;

        // Define the buttons for the touch page
        public override IEnumerable<String> GetButtonPressActionNames()
        {
            IEnumerable<String> tmp;
            if (this.action == 1)//常用
            {
                this.buttons = new String[]
                        {"rot"     ,"close"
                ,""     ,""     ,"change"
                ,"cut"  ,"copy" ,"paste"};
            }
            else
            {
                this.buttons = new String[]
                        {"rot"     ,"close"
                ,""     ,""     ,"change"
                ,"1"    ,"2"    ,"3"};
            }
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
                    break;
                case "paste":
                    PluginLog.Info("paste");
                    break;
                case "change":
                    this.action *= -1;
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

