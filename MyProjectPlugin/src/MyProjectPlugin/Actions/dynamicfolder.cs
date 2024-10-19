namespace Loupedeck.MyProjectPlugin
{
    using System;
    using System.Collections.Generic;

    public class TaskSwitcherDynamicFolder : PluginDynamicFolder
    {
        public TaskSwitcherDynamicFolder()
        {
            this.DisplayName = "dyna";
            this.GroupName = "dyna";
        }
        protected PluginDynamicFolderNavigation GetNavigationArea() => PluginDynamicFolderNavigation.ButtonArea; // 設置為 ButtonArea，這將在觸控頁面的左上角自動添加返回按鈕
        

        // Define the buttons for the touch page
        [Obsolete("GetButtonPressActionNames is obsolete, consider using the new method if available.")]
        public override IEnumerable<String> GetButtonPressActionNames()
        {
            return new[]
            {
                PluginDynamicFolder.NavigateUpActionName, // 返回按鈕
                this.CreateCommandName("1"), // 自定義按鈕 1
                this.CreateCommandName("2")  // 自定義按鈕 2
            };
        }
    }
}
