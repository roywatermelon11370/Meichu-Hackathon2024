namespace Loupedeck.MyProjectPlugin
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Timers;

    // This class contains the plugin-level logic of the Loupedeck plugin.
    public class MyProjectPlugin : Plugin
    {
        // 导入 user32.dll 中的 GetForegroundWindow 函数
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        // 导入 user32.dll 中的 GetWindowThreadProcessId 函数
        [DllImport("user32.dll")]
        static extern UInt32 GetWindowThreadProcessId(IntPtr hWnd, out UInt32 processId);

        // 定义用于记录上一次前台进程ID
        private UInt32 lastProcessId = 0;

        // 定义定时器
        private readonly Timer timer;

        // Gets a value indicating whether this is an API-only plugin.
        public override Boolean UsesApplicationApiOnly => true;

        // Gets a value indicating whether this is a Universal plugin or an Application plugin.
        public override Boolean HasNoApplication => true;

        // Initializes a new instance of the plugin class.
        public MyProjectPlugin()
        {
            // Initialize the plugin log.
            PluginLog.Init(this.Log);

            // Initialize the plugin resources.
            PluginResources.Init(this.Assembly);

            // 初始化定时器
            this.timer = new Timer(100); // 每 100 毫秒触发一次
            this.timer.Elapsed += this.CheckForegroundApp; // 绑定定时触发的事件
            this.timer.AutoReset = true; // 设置自动重置，定时触发
            //this.timer.Enabled = true; // 启用定时器
        }

        // 用于获取当前前台窗口的进程
        private void CheckForegroundApp(Object? source, ElapsedEventArgs e)
        {
            // 获取当前前台窗口的句柄
            var hwnd = GetForegroundWindow();


            // 获取当前前台窗口的进程ID
            GetWindowThreadProcessId(hwnd, out var processID);

            // 如果当前的进程ID和上一次不同，则表示前台应用发生了变化
            if (processID != this.lastProcessId)
            {
                this.lastProcessId = processID; // 更新上次记录的进程ID

                try
                {
                    // 获取该进程的详细信息
                    Process foregroundProcess = Process.GetProcessById((Int32)processID);
                    PluginLog.Info($"Foreground process name: {foregroundProcess.ProcessName}");
                }
                catch (Exception ex)
                {
                    PluginLog.Error($"Error retrieving process information: {ex.Message}");
                }
            }
        }

        // This method is called when the plugin is loaded.
        public override void Load() =>
            // 定时器已在构造函数中初始化，只需确保启用
            this.timer.Start();

        // This method is called when the plugin is unloaded.
        public override void Unload()
        {
            // 停止并销毁定时器
            this.timer.Stop();
            this.timer.Dispose();
        }
    }
}
