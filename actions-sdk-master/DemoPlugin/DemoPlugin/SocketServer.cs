namespace Loupedeck.DemoPlugin
{

    using System;
    using System.Net;
    using System.Net.WebSockets;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using System.Xml.XPath;


    public class CommandMsg {
        public String Cmd { get; set; }
        public Int32 X { get; set; }
        public Int32 Y { get; set; }
    }

    public class TransferRecv : PluginDynamicFolder
    {

        //private DemoPlugin DemoPlugin => this.Plugin as DemoPlugin; 

        public TransferRecv()
        {
            this.DisplayName = "qwq";
            this.GroupName = "qwq";
        }
        protected static PluginDynamicFolderNavigation GetNavigationArea() => PluginDynamicFolderNavigation.None; 

        // public void SendCurrentRecv(String msg)
        // {
        //     PluginLog.Info($"trans in");
        //     ExcelCommon.SetAction(msg);
        //     PluginLog.Info($"trans out");
        // }
    }
    public class SocketServer
    {
        //private DemoPlugin DemoPlugin => this.Plugin as DemoPlugin; 
        private Action<string> setAction;

        private readonly HttpListener _listener;
        // private DemoPlugin DemoPlugin => this.Plugin as DemoPlugin; 

        private readonly Channel<String> _commands;
        private readonly ChannelReader<String> _commandReader;
        private readonly ChannelWriter<String> _commandWriter;

        private readonly TransferRecv _trans;

        // private int SetAction(int set,string act){
        //     set(act);
        //     return 0;
        // }

        public SocketServer(Action<string> setAct)
        {
            Task.Run(async() => await this.StartServer());
            // Above gpt
            this._listener = new HttpListener();
            this._listener.Prefixes.Add("http://localhost:8880/");

            this._commands = Channel.CreateUnbounded<String>();
            this._commandReader = this._commands.Reader;
            this._commandWriter = this._commands.Writer;
            // this._trans = new TransferRecv();
            this.setAction = setAct;
            Task.Run(async () => await this.StartServer());
        }

        public async void SendMessage(String msg)
        {
            // var msg = JsonSerializer.Serialize(new CommandMsg { Cmd = "reset" });
            PluginLog.Info($"Sending reset command: {msg}");
            await this._commandWriter.WriteAsync(msg);
        }

        protected async Task StartServer()
        {
            this._listener.Start();

            PluginLog.Info($"SketchPlugin.Server is listening on http://localhost:8880");

            while (true)
            {
                HttpListenerContext context = await this._listener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    HttpListenerWebSocketContext webSocketContext = await context.AcceptWebSocketAsync(null);
                    WebSocket webSocket = webSocketContext.WebSocket;
                    PluginLog.Info($"Server started");
                    // await this.HandleWebSocketConnection(webSocket);
                    await Task.WhenAll(HandleMessages(webSocket), this.HandleWebSocketConnection(webSocket));
                }
            }
        }

        public async Task HandleMessages(WebSocket webSocket) {
            //var buffer = new byte[1024 * 4];
        try {
            using (var ms = new MemoryStream()) {
                while (webSocket.State == WebSocketState.Open) {
                    WebSocketReceiveResult result;
                    do {
                        var messageBuffer = WebSocket.CreateClientBuffer(1024, 16);
                        result = await webSocket.ReceiveAsync(messageBuffer, CancellationToken.None);
       
                        ms.Write(messageBuffer.Array, messageBuffer.Offset, result.Count);
                    }
                    while (!result.EndOfMessage);
                    
                    if (result.MessageType == WebSocketMessageType.Text) {
                        var msgString = Encoding.UTF8.GetString(ms.ToArray());
                        //this._trans.SendCurrentRecv(msgString);
                        PluginLog.Info($"Message received: {msgString}");
                        this.setAction(msgString);
                    }
                    ms.Seek(0, SeekOrigin.Begin);
                    ms.Position = 0;
                }
            }
        } catch (InvalidOperationException) {
            PluginLog.Info("[WS] Tried to receive message while already reading one.");
        }
        
        }

        protected async Task HandleWebSocketConnection(WebSocket webSocket)
        {
            // WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            PluginLog.Info("WebSocket connection established");

            while (await this._commandReader.WaitToReadAsync())
            {
                while (this._commandReader.TryRead(out var cmd))
                {
                    var msg = Encoding.UTF8.GetBytes(cmd);
                    PluginLog.Info(msg.ToString());
                    await webSocket.SendAsync(new ArraySegment<Byte>(msg), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
}






