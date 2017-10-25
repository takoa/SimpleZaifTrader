using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Threading;

namespace ZaifNet.Public
{
    public class StreamingApiUtility
    {
        private const string BaseUri = "wss://ws.zaif.jp:8888/ws?currency_pair=";

        public ClientWebSocket Client { get; private set; } = new ClientWebSocket();

        public StreamingApiUtility()
        {
        }

        public async Task StartReceiving(string currencyPair, Action<CallbackArgs, object> callback, object callbackState, CancellationToken cancellationToken)
        {
            if (this.Client.State != WebSocketState.Open)
            {
                CancellationToken ct = new CancellationToken();
                CallbackArgs args = new CallbackArgs() { CurrencyPair = currencyPair };

                await this.Client.ConnectAsync(new Uri(StreamingApiUtility.BaseUri + currencyPair), ct);
                args.Status = CallbackStatus.Connected;

                callback(args, callbackState);

                while (this.Client.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
                {
                    ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[10240]);
                    WebSocketReceiveResult receiveResult = await this.Client.ReceiveAsync(buffer, ct);

                    if (0 < receiveResult.Count)
                    {
                        try
                        {
                            args.StreamingData = JsonConvert.DeserializeObject<StreamingData>(Encoding.UTF8.GetString(buffer.Take(receiveResult.Count).ToArray()).Replace(@"\\", @"\"));
                            args.Status = CallbackStatus.DataReceived;

                            callback(args, callbackState);
                        }
                        catch (JsonSerializationException)
                        {
                            await this.Reconnect(currencyPair, ct);

                            continue;
                        }
                        catch (JsonReaderException)
                        {
                            await this.Reconnect(currencyPair, ct);

                            continue;
                        }
                    }
                }

                await this.Client.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, ct);
                this.Client.Dispose();

                args.Status = CallbackStatus.Disconnected;

                callback(args, callbackState);
            }
        }

        private async Task Reconnect(string currencyPair, CancellationToken ct)
        {
            this.Client.Dispose();
            this.Client = new ClientWebSocket();
            await this.Client.ConnectAsync(new Uri(StreamingApiUtility.BaseUri + currencyPair), ct);
        }

        public class CallbackArgs
        {
            public CallbackStatus Status { get; set; }
            public string CurrencyPair { get; set; }
            public StreamingData StreamingData { get; set; }
        }

        public enum CallbackStatus
        {
            Connected,
            Disconnected,
            DataReceived
        }
    }
}
