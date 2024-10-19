namespace Loupedeck.DemoPlugin
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Timers;
    using Loupedeck.MyProjectPlugin;

    public class DemoApplication : ClientApplication
    {
        // 導入 user32.dll 中的 GetForegroundWindow 函數
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        // 導入 user32.dll 中的 GetWindowThreadProcessId 函數
        [DllImport("user32.dll")]
        private static extern UInt32 GetWindowThreadProcessId(IntPtr hWnd, out UInt32 processId);

        private static UInt32 lastProcessId = 0; // 用來記錄上一次的前台進程ID
        private static Timer? timer; // 標記為允許為 Null

        //ExcelCommon now;

        //protected override String GetProcessName() => "EXCEL";

        public DemoApplication()
        {
            // 初始化計時器，設置每秒觸發一次
            timer = new Timer(100); // 每 1000 毫秒觸發一次
            timer.Elapsed += this.CheckForegroundApp; // 綁定定時觸發的事件
            timer.AutoReset = true; // 設置自動重置，定時觸發
            timer.Enabled = true; // 啟用定時器
            //now = new ExcelCommon();
        }

        // 這個方法用來獲取 Windows 的前台進程名稱
        // protected override String GetProcessName()
        // {
        //     return "default"; // 這裡可以根據需要返回某個默認進程名
        // }

        private String CurrentAppName { get; set; }

        public String GetAppName() => this.CurrentAppName;

        // macOS 的應用名稱，可根據需要修改
        protected override String GetBundleName() => "";



        // 定時器回調方法，用於檢查前台應用是否發生變化
        private
         void CheckForegroundApp(Object? source, ElapsedEventArgs e) // 允許 source 為 null
        {
            // 獲取當前前台窗口的句柄
            IntPtr hwnd = GetForegroundWindow();

            // 獲取當前前台窗口的進程ID
            GetWindowThreadProcessId(hwnd, out var processID);

            // 如果當前的進程ID和上一次不同，則表示前台應用發生了變化
            if (processID != lastProcessId)
            {
                lastProcessId = processID; // 更新上次記錄的進程ID

                // 獲取該進程的詳細資訊
                Process foregroundProcess = Process.GetProcessById((Int32)processID);
                PluginLog.Info($"Foreground window handle: {hwnd}");
                PluginLog.Info($"Foreground process ID: {processID}");
                PluginLog.Info($"Foreground process name: {foregroundProcess.ProcessName}");
                PluginLog.Info("--------------------------------------------");
                this.CurrentAppName = foregroundProcess.ProcessName;

                if (this.CurrentAppName == "EXCEL")
                {
                    // Change Dynamic Folder to [excel dedault]
                    //
                    //PluginLog.Info("1");
                    
                }
                else{
                    //now.RunCommand("Close");
                }

            }
        }
    }
}
