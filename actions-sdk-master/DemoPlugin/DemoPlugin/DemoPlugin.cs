namespace Loupedeck.DemoPlugin
{
    using System;
    using System.Timers;

    using Loupedeck.SimpleWebSocket;

    public class DemoPlugin : Plugin
    {
        public SocketServer _server;

        public SocketServer GetServer() => this._server;

        public ExcelCommon _excelcommon;

        public ExcelCommon GetExcelCommon() => this._excelcommon;

        // Gets a value indicating whether this is an API-only plugin.
        public override Boolean UsesApplicationApiOnly => true;

        // Gets a value indicating whether this is a Universal plugin or an Application plugin.
        public override Boolean HasNoApplication => true;

        // Initializes a new instance of the plugin class.

        

        // public string GetMessage() => this._server.msgString;

        // public event Action<string> MsgChanged;

        // public void OnMsgChanged(string msg) {
        //     MsgChanged?.Invoke(msg);
        // }

        public DemoPlugin()
        {
            // Initialize the plugin log.
            PluginLog.Init(this.Log);

            // Initialize the plugin resources.
            PluginResources.Init(this.Assembly);

            this._server = new SocketServer();
            this._excelcommon = new ExcelCommon();
        }

        // This method is called when the plugin is loaded.
        public override void Load()
        {
        }



        // This method is called when the plugin is unloaded.
        public override void Unload()
        {
        }
    }
}
