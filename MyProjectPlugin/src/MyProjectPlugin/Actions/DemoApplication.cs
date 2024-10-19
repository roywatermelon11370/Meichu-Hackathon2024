namespace Loupedeck.DemoPlugin
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Timers;

    public class DemoApplication : ClientApplication
    {
        // 導入 user32.dll 中的 GetForegroundWindow 函數
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        // 導入 user32.dll 中的 GetWindowThreadProcessId 函數
        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        private static uint lastProcessId = 0; // 用來記錄上一次的前台進程ID
        private static Timer? timer; // 標記為允許為 Null

        //public static String CurrentProcess = "Default";

        public DemoApplication()
        {
            // 初始化計時器，設置每秒觸發一次
            timer = new Timer(100); // 每 100 毫秒觸發一次
            timer.Elapsed += CheckForegroundApp; // 綁定定時觸發的事件
            timer.AutoReset = true; // 設置自動重置，定時觸發
            timer.Enabled = true; // 啟用定時器
        }

        // 改為靜態方法
        //public static String GetAppName() => CurrentProcess;

        // macOS 的應用名稱，可根據需要修改
        protected override String GetBundleName() => "";

        // 定時器回調方法，用於檢查前台應用是否發生變化
        private static void CheckForegroundApp(object? source, ElapsedEventArgs e)
        {
            // 獲取當前前台窗口的句柄
            IntPtr hwnd = GetForegroundWindow();

            // 獲取當前前台窗口的進程ID
            uint processID;
            GetWindowThreadProcessId(hwnd, out processID);

            // 如果當前的進程ID和上一次不同，則表示前台應用發生了變化
            if (processID != lastProcessId)
            {
                lastProcessId = processID; // 更新上次記錄的進程ID

                // 獲取該進程的詳細資訊
                Process foregroundProcess = Process.GetProcessById((int)processID);
                //CurrentProcess = foregroundProcess.ProcessName;
            }
        }
    }
}
