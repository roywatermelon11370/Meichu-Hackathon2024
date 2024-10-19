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

    public class SocketServer
    {
        private readonly HttpListener _listener;
        // private DemoPlugin DemoPlugin => this.Plugin as DemoPlugin; 

        private readonly Channel<String> _commands;
        private readonly ChannelReader<String> _commandReader;
        private readonly ChannelWriter<String> _commandWriter;

        public SocketServer()
        {
            this._listener = new HttpListener();
            this._listener.Prefixes.Add("http://localhost:8880/");

            this._commands = Channel.CreateUnbounded<String>();
            this._commandReader = this._commands.Reader;
            this._commandWriter = this._commands.Writer;

            Task.Run(async () => await this.StartServer());
        }

        public async void SendMessage(string msg)
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
                    await Task.WhenAll(this.HandleMessages(webSocket), this.HandleWebSocketConnection(webSocket));
                }
            }
        }

        public async Task HandleMessages(WebSocket webSocket) {
        try {
            using (var ms = new MemoryStream()) {
                while (webSocket.State == WebSocketState.Open) {
                    WebSocketReceiveResult result;
                    do {
                        var messageBuffer = WebSocket.CreateClientBuffer(1024, 16);
                        result = await webSocket.ReceiveAsync(messageBuffer, CancellationToken.None);
                        PluginLog.Info("Fuck");
                        ms.Write(messageBuffer.Array, messageBuffer.Offset, result.Count);
                    }
                    while (!result.EndOfMessage);

                    if (result.MessageType == WebSocketMessageType.Text) {
                        var msgString = Encoding.UTF8.GetString(ms.ToArray());
                        PluginLog.Info($"Message received: {msgString}");
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

            //await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
        // protected async Task HandleWebSocketConnection(WebSocket webSocket)
        // {
        //     // using (var ms = new MemoryStream()) {
        //     // var messageBuffer = WebSocket.CreateClientBuffer(8192, 8192);
        //     // WebSocketReceiveResult result;

        //     // do {
        //     //     result = await webSocket.ReceiveAsync(messageBuffer, CancellationToken.None);
        //     //     ms.Write(messageBuffer.Array, messageBuffer.Offset, result.Count);
        //     // }
        //     // while (!result.EndOfMessage);
            
        //     // PluginLog.Info("WebSocket connection established");

        //     // if (result.MessageType == WebSocketMessageType.Text) {
        //     //     var msgString = Encoding.UTF8.GetString(ms.ToArray());
        //     //     PluginLog.Info($"Received Message: {msgString}");
        //     //     // this.DemoPlugin.GetExcelCommon().setAction(msgString);
        //     //     // ReceiveMessage(msgString);
        //     // }
        //     try {
        //         using (var ms = new MemoryStream()) {
        //             while (webSocket.State == WebSocketState.Open) {
        //                 WebSocketReceiveResult result;
        //                 do {
        //                     var messageBuffer = WebSocket.CreateClientBuffer(1024, 16);
        //                     result = await webSocket.ReceiveAsync(messageBuffer, CancellationToken.None);
        //                     ms.Write(messageBuffer.Array, messageBuffer.Offset, result.Count);
        //                 }
        //                 while (!result.EndOfMessage);

        //                 if (result.MessageType == WebSocketMessageType.Text) {
        //                     var msgString = Encoding.UTF8.GetString(ms.ToArray());
        //                     PluginLog.Info($"Message Reeceived: {msgString}");
                            
        //                 }
        //                 ms.Seek(0, SeekOrigin.Begin);
        //                 ms.Position = 0;
        //                 while (await this._commandReader.WaitToReadAsync())
        //                 {
        //                     PluginLog.Info("WebSocket Read Sync Triggered");
        //                     while (this._commandReader.TryRead(out var cmd))
        //                     {
        //                         var msg = Encoding.UTF8.GetBytes(cmd);
        //                         await webSocket.SendAsync(new ArraySegment<Byte>(msg), WebSocketMessageType.Text, true, CancellationToken.None);
        //                     }
        //                 }
        //             }

        //         }
        //     } catch (InvalidOperationException) {
        //         PluginLog.Info("[WS] Tried to receive message while already reading one.");
        //     }


        //     //await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        // }
        // }
    }






