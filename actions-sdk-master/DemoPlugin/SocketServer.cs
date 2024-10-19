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

        private readonly Channel<String> _commands;
        private readonly ChannelReader<String> _commandReader;
        private readonly ChannelWriter<String> _commandWriter;

        public SocketServer()
        {
            this._listener = new HttpListener();
            this._listener.Prefixes.Add("http://localhost:8080/");

            this._commands = Channel.CreateUnbounded<String>();
            this._commandReader = this._commands.Reader;
            this._commandWriter = this._commands.Writer;

            Task.Run(async () => await this.StartServer());
        }

        public async void SendReset()
        {
            var msg = JsonSerializer.Serialize(new CommandMsg { Cmd = "reset" });
            PluginLog.Info($"Sending reset command: {msg}");
            await this._commandWriter.WriteAsync(msg);
        }

        public async void SendMoveCursor(Int32 x, Int32 y)
        {
            var cmd = JsonSerializer.Serialize(new CommandMsg { Cmd = "move", X = x, Y = y });
            PluginLog.Info($"Sending move cursor command: {cmd}");
            await this._commandWriter.WriteAsync(cmd);
        }

        protected async Task StartServer()
        {
            this._listener.Start();

            PluginLog.Info($"SketchPlugin.Server is listening on http://localhost:8080");

            while (true)
            {
                HttpListenerContext context = await this._listener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    HttpListenerWebSocketContext webSocketContext = await context.AcceptWebSocketAsync(null);
                    WebSocket webSocket = webSocketContext.WebSocket;

                    await this.HandleWebSocketConnection(webSocket);
                }
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

}