//
// PrecioService.cs
//
// Author:
//       Igor Guerrero <igorgue@protonmail.com>
//
// Copyright (c) 2019 
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Xamarin.Forms;

using HodlWallet2.Core.Interfaces;
using HodlWallet2.Core.Services;
using HodlWallet2.Core.Models;
using Newtonsoft.Json;
using Refit;
using HodlWallet2.Core.Utils;

[assembly: Dependency(typeof(PrecioService))]
namespace HodlWallet2.Core.Services
{
    public class PrecioService : IPrecioService, INotifyPropertyChanged
    {
        public IPrecioHttpService _PrecioHttpService => RestService.For<IPrecioHttpService>(Constants.PRECIO_HOST_URL);
        string _WebSocketServerBaseUrl = Constants.PRECIO_WS_HOST_URL + "/{0}";
        string[] _Channels => new string[]
        {
            "btc-price",
            "exchanges-leaderboard",
            "btc-marketcap",
            "btc-1h-change",
            "btc-1d-change",
            "btc-1w-change",
            "btc-1m-change",
            "btc-1y-change",
            "btc-all-change"
        };
        Dictionary<string, ClientWebSocket> _WebSockets;

        int _BtcPriceDelay = 2_500; // 2.5 seconds, time of the animation as well
        int _WebSocketMessageDelay = 300_000; // 5 minutes
        int _HttpRequestsDelay = 300_000; // 5 minutes

        BtcPriceEntity _BtcPrice;
        public BtcPriceEntity BtcPrice
        {
            get => _BtcPrice;
            set => SetProperty(ref _BtcPrice, value, nameof(BtcPrice));
        }

        List<List<object>> _ExchangesLeaderboard;
        public List<List<object>> ExchangesLeaderboard
        {
            get => _ExchangesLeaderboard;
            set => SetProperty(ref _ExchangesLeaderboard, value, nameof(ExchangesLeaderboard));
        }

        MarketCapEntity _MarketCap;
        public MarketCapEntity MarketCap
        {
            get => _MarketCap;
            set => SetProperty(ref _MarketCap, value, nameof(MarketCap));
        }

        BtcPriceChangeEntity _Btc1hChange;
        public BtcPriceChangeEntity Btc1hChange
        {
            get => _Btc1hChange;
            set => SetProperty(ref _Btc1hChange, value, nameof(Btc1hChange));
        }

        BtcPriceChangeEntity _Btc1dChange;
        public BtcPriceChangeEntity Btc1dChange
        {
            get => _Btc1dChange;
            set => SetProperty(ref _Btc1dChange, value, nameof(Btc1dChange));
        }

        BtcPriceChangeEntity _Btc1wChange;
        public BtcPriceChangeEntity Btc1wChange
        {
            get => _Btc1wChange;
            set => SetProperty(ref _Btc1wChange, value, nameof(Btc1wChange));
        }

        BtcPriceChangeEntity _Btc1mChange;
        public BtcPriceChangeEntity Btc1mChange
        {
            get => _Btc1mChange;
            set => SetProperty(ref _Btc1mChange, value, nameof(Btc1mChange));
        }

        BtcPriceChangeEntity _Btc1yChange;
        public BtcPriceChangeEntity Btc1yChange
        {
            get => _Btc1yChange;
            set => SetProperty(ref _Btc1yChange, value, nameof(Btc1yChange));
        }

        BtcPriceChangeEntity _BtcAllChange;
        public BtcPriceChangeEntity BtcAllChange
        {
            get => _BtcAllChange;
            set => SetProperty(ref _BtcAllChange, value, nameof(BtcAllChange));
        }

        PricesEntity _Prices1h;
        public PricesEntity Prices1h
        {
            get => _Prices1h;
            set => SetProperty(ref _Prices1h, value, nameof(Prices1h));
        }

        PricesEntity _Prices1d;
        public PricesEntity Prices1d
        {
            get => _Prices1d;
            set => SetProperty(ref _Prices1d, value, nameof(Prices1d));
        }

        PricesEntity _Prices1w;
        public PricesEntity Prices1w
        {
            get => _Prices1w;
            set => SetProperty(ref _Prices1w, value, nameof(Prices1w));
        }

        PricesEntity _Prices1m;
        public PricesEntity Prices1m
        {
            get => _Prices1m;
            set => SetProperty(ref _Prices1m, value, nameof(Prices1m));
        }

        PricesEntity _Prices1y;
        public PricesEntity Prices1y
        {
            get => _Prices1y;
            set => SetProperty(ref _Prices1y, value, nameof(Prices1y));
        }

        PricesEntity _PricesAll;
        public PricesEntity PricesAll
        {
            get => _PricesAll;
            set => SetProperty(ref _PricesAll, value, nameof(PricesAll));
        }

        CurrencyEntity _Rate;
        public CurrencyEntity Rate
        {
            get => _Rate;
            set => SetProperty(ref _Rate, value, nameof(Rate));
        }

        public PrecioService()
        {
            PropertyChanged += delegate { };

            _WebSockets = new Dictionary<string, ClientWebSocket>();
        }

        public void Init()
        {
            Task.Run(WebSocketConnect);
            Task.Run(HttpDataFetchConnect);
        }

        protected bool SetProperty<T>(ref T backingStore, T value, string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            SendMessageOnPropertyUpdate(propertyName);

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        void SendMessageOnPropertyUpdate(string propertyName)
        {
            var val = GetType().GetProperty(propertyName);

            if (val != null)
            {
                var msg = $"{propertyName}Changed";
                MessagingCenter.Send(this, msg);
            }
        }

        void InitWebSocketList()
        {
            // Insert each channel with each web socket
            foreach (var channel in _Channels)
                _WebSockets[channel] = new ClientWebSocket();
        }

        void HttpDataFetchConnect()
        {
            StartHttpTimers();
        }

        public void StartHttpTimers()
        {
            Debug.WriteLine("[StartHttpTimers] Started");

            Task.Factory.StartNew(async (options) =>
            {
                while (true)
                {
                    FetchPricesForAllPeriods();

                    await Task.Delay(_HttpRequestsDelay);
                }
            }, TaskCreationOptions.LongRunning, CancellationToken.None);

            Task.Factory.StartNew(async (options) =>
            {
                while (true)
                {
                    await FetchRate();

                    await Task.Delay(_BtcPriceDelay);
                }
            }, TaskCreationOptions.LongRunning, CancellationToken.None);
        }

        void FetchPricesForAllPeriods()
        {
            foreach (var period in new string[] { "1h", "1d", "1m", "1y", "all" })
            {
                var prices = _PrecioHttpService.GetPrecioByPeriod(period).Result;

                if (prices != null)
                {
                    switch (period)
                    {
                        case "1h":
                            Prices1h = prices;
                            break;
                        case "1d":
                            Prices1d = prices;
                            break;
                        case "1w":
                            Prices1w = prices;
                            break;
                        case "1m":
                            Prices1m = prices;
                            break;
                        case "1y":
                            Prices1y = prices;
                            break;
                        case "all":
                            PricesAll = prices;
                            break;
                    }
                }
                else
                {
                    Debug.WriteLine("[UpdatePrice] Unable to get prices");
                }
            }
        }

        async Task FetchRate()
        {
            Rate = (await _PrecioHttpService.GetRates()).SingleOrDefault(r => r.Code == "USD");

            Debug.WriteLine($"[FetchRate] Got rate {Rate.Rate.ToString("C")}");
        }

        async Task WebSocketConnect()
        {
            if (_WebSockets.Count == 0) InitWebSocketList();

            // Connect to each websocket
            List<Task> connectionTasks = GetConnectionTasks();
            await Task.WhenAll(connectionTasks);

            PrintStatuses();
        }

        void PrintStatuses()
        {
            foreach (KeyValuePair<string, ClientWebSocket> entry in _WebSockets)
                Debug.WriteLine($"[WebSocketConnect] Channel ({entry.Key}) status {entry.Value.State.ToString()}");
        }

        List<Task> GetConnectionTasks()
        {
            // Connect to each websocket
            List<Task> connectionTasks = new List<Task> { };
            foreach (KeyValuePair<string, ClientWebSocket> entry in _WebSockets)
            {
                var webSocketUrl = new Uri(string.Format(_WebSocketServerBaseUrl, entry.Key));

                connectionTasks.Add(
                    Task.Run(async () =>
                    {
                        var cts = new CancellationTokenSource();

                        await entry.Value.ConnectAsync(webSocketUrl, cts.Token);
                        _ = Task.Factory.StartNew(async (options) =>
                        {
                            while (true)
                            {
                                try
                                {
                                    string result = await ReadMessage(entry.Value, cts.Token);

                                    if (!string.IsNullOrEmpty(result))
                                        UpdateDataFrom(entry.Key, result);
                                }
                                catch (Exception e)
                                {
                                    Debug.WriteLine($"Error {e.ToString()}");
                                    Debug.WriteLine($"Reconnecting to {entry.Key}'s channel");

                                    cts = new CancellationTokenSource();
                                    await entry.Value.ConnectAsync(webSocketUrl, cts.Token);
                                }

                                int amountToWaitInMilliseconds = _BtcPriceDelay; // 2.5 Seconds for btc-price
                                if (entry.Key != "btc-price")
                                    amountToWaitInMilliseconds = _WebSocketMessageDelay; // 5 Minutes for everything else

                                await Task.Delay(amountToWaitInMilliseconds);
                            }
                        }, TaskCreationOptions.LongRunning, CancellationToken.None);
                    })
                );
            }

            return connectionTasks;
        }

        async Task<string> ReadMessage(ClientWebSocket socket, CancellationToken token)
        {
            var bytes = new byte[4096];
            var buffer = new ArraySegment<byte>(bytes);

            WebSocketReceiveResult result;
            string msgString;
            do
            {
                result = await socket.ReceiveAsync(buffer, token);

                byte[] msgBytes = buffer.Skip(buffer.Offset).Take(result.Count).ToArray();
                msgString = Encoding.UTF8.GetString(msgBytes);
            }
            while (!result.EndOfMessage);

            return msgString;
        }

        void UpdateDataFrom(string key, string data)
        {
            Debug.WriteLine($"[UpdateDataFrom] {key} with: {data}");

            switch (key)
            {
                case "btc-price":
                    BtcPrice = JsonConvert.DeserializeObject<BtcPriceEntity>(data);
                    break;
                case "exchanges-leaderboard":
                    ExchangesLeaderboard = JsonConvert.DeserializeObject<List<List<object>>>(data);
                    break;
                case "btc-marketcap":
                    MarketCap = JsonConvert.DeserializeObject<MarketCapEntity>(data);
                    break;
                case "btc-1h-change":
                    Btc1hChange = JsonConvert.DeserializeObject<BtcPriceChangeEntity>(data);
                    break;
                case "btc-1d-change":
                    Btc1dChange = JsonConvert.DeserializeObject<BtcPriceChangeEntity>(data);
                    break;
                case "btc-1w-change":
                    Btc1wChange = JsonConvert.DeserializeObject<BtcPriceChangeEntity>(data);
                    break;
                case "btc-1m-change":
                    Btc1mChange = JsonConvert.DeserializeObject<BtcPriceChangeEntity>(data);
                    break;
                case "btc-1y-change":
                    Btc1yChange = JsonConvert.DeserializeObject<BtcPriceChangeEntity>(data);
                    break;
                case "btc-all-change":
                    BtcAllChange = JsonConvert.DeserializeObject<BtcPriceChangeEntity>(data);
                    break;
                default:
                    var err = $"Cannot parse data from {key}";

                    Debug.WriteLine(err);
                    throw new ArgumentException(err);
        }
        }
    }
}
