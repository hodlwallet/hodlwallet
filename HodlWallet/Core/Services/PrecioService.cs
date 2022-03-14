//
// PrecioService.cs
//
// Author:
//       Igor Guerrero <igorgue@protonmail.com>
//
// Copyright (c) 2019 HODL Wallet
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

using HodlWallet.Core.Interfaces;
using HodlWallet.Core.Services;
using HodlWallet.Core.Models;
using Newtonsoft.Json;
using Refit;
using HodlWallet.Core.Utils;
using System.Net.Http;
using System.Collections.ObjectModel;

[assembly: Dependency(typeof(PrecioService))]
namespace HodlWallet.Core.Services
{
    public class PrecioService : IPrecioService, INotifyPropertyChanged
    {
        public bool IsStarted { get; private set; }

        public IPrecioHttpService PrecioHttpService => CustomRefitSettings.RestClient();

        readonly string webSocketServerBaseUrl = Constants.PRECIO_WS_HOST_URL + "/{0}";
        string[] Channels => new string[]
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

        readonly Dictionary<string, ClientWebSocket> webSockets;

        readonly int btcPriceDelay = 2_500; // 2.5 seconds, time of the animation as well
        readonly int httpRequestsDelay = 30_000; // 30 seconds

        BtcPriceEntity btcPrice;
        public BtcPriceEntity BtcPrice
        {
            get => btcPrice;
            set => SetProperty(ref btcPrice, value, nameof(BtcPrice));
        }

        List<List<object>> exchangesLeaderboard;
        public List<List<object>> ExchangesLeaderboard
        {
            get => exchangesLeaderboard;
            set => SetProperty(ref exchangesLeaderboard, value, nameof(ExchangesLeaderboard));
        }

        MarketCapEntity marketCap;
        public MarketCapEntity MarketCap
        {
            get => marketCap;
            set => SetProperty(ref marketCap, value, nameof(MarketCap));
        }

        BtcPriceChangeEntity btc1hChange;
        public BtcPriceChangeEntity Btc1hChange
        {
            get => btc1hChange;
            set => SetProperty(ref btc1hChange, value, nameof(Btc1hChange));
        }

        BtcPriceChangeEntity btc1dChange;
        public BtcPriceChangeEntity Btc1dChange
        {
            get => btc1dChange;
            set => SetProperty(ref btc1dChange, value, nameof(Btc1dChange));
        }

        BtcPriceChangeEntity btc1wChange;
        public BtcPriceChangeEntity Btc1wChange
        {
            get => btc1wChange;
            set => SetProperty(ref btc1wChange, value, nameof(Btc1wChange));
        }

        BtcPriceChangeEntity btc1mChange;
        public BtcPriceChangeEntity Btc1mChange
        {
            get => btc1mChange;
            set => SetProperty(ref btc1mChange, value, nameof(Btc1mChange));
        }

        BtcPriceChangeEntity btc1yChange;
        public BtcPriceChangeEntity Btc1yChange
        {
            get => btc1yChange;
            set => SetProperty(ref btc1yChange, value, nameof(Btc1yChange));
        }

        BtcPriceChangeEntity btcAllChange;
        public BtcPriceChangeEntity BtcAllChange
        {
            get => btcAllChange;
            set => SetProperty(ref btcAllChange, value, nameof(BtcAllChange));
        }

        PricesEntity prices1h;
        public PricesEntity Prices1h
        {
            get => prices1h;
            set => SetProperty(ref prices1h, value, nameof(Prices1h));
        }

        PricesEntity prices1d;
        public PricesEntity Prices1d
        {
            get => prices1d;
            set => SetProperty(ref prices1d, value, nameof(Prices1d));
        }

        PricesEntity prices1w;
        public PricesEntity Prices1w
        {
            get => prices1w;
            set => SetProperty(ref prices1w, value, nameof(Prices1w));
        }

        PricesEntity prices1m;
        public PricesEntity Prices1m
        {
            get => prices1m;
            set => SetProperty(ref prices1m, value, nameof(Prices1m));
        }

        PricesEntity prices1y;
        public PricesEntity Prices1y
        {
            get => prices1y;
            set => SetProperty(ref prices1y, value, nameof(Prices1y));
        }

        PricesEntity pricesAll;
        public PricesEntity PricesAll
        {
            get => pricesAll;
            set => SetProperty(ref pricesAll, value, nameof(PricesAll));
        }

        CurrencyEntity rate;
        public CurrencyEntity Rate
        {
            get => rate;
            set => SetProperty(ref rate, value, nameof(Rate));
        }

        public List<CurrencyEntity> CurrencyEntities { get; set; } = new();

        public PrecioService()
        {
            PropertyChanged += delegate { };

            webSockets = new Dictionary<string, ClientWebSocket>();
            IsStarted = false;
        }

        public void Init()
        {
            Task.Run(WebSocketConnect);
            Task.Run(HttpDataFetchConnect);

            IsStarted = true;
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
            foreach (var channel in Channels)
                webSockets[channel] = new ClientWebSocket();
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

                    await Task.Delay(httpRequestsDelay);
                }
            }, TaskCreationOptions.LongRunning, CancellationToken.None);

            /*Task.Factory.StartNew(async (options) =>
            {
                while (true)
                {
                    await FetchRate();

                    await Task.Delay(btcPriceDelay);
                }
            }, TaskCreationOptions.LongRunning, CancellationToken.None);*/

            Task.Factory.StartNew(async (options) =>
            {
                while (true)
                {
                    await FetchCurrencies();

                    await Task.Delay(btcPriceDelay);
                }
            }, TaskCreationOptions.LongRunning, CancellationToken.None);
        }

        void FetchPricesForAllPeriods()
        {
            //foreach (var period in new string[] { "1h", "1d", "1w", "1m", "1y", "All" })
            foreach (var period in new string[] { "1d", "1w", "1m", "1y" })
            {
                var prices = PrecioHttpService.GetPrecioByPeriod(period).Result;

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
                        case "All":
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
            Rate = (await PrecioHttpService.GetRates()).SingleOrDefault(r => r.Code == "USD");
        }

        async Task FetchCurrencies()
        {
            Debug.WriteLine("[DEBUGIN Currencies] Into FetchCurrencies");
            var CurrencyEntitiesTemp = await PrecioHttpService.GetRates();
            await Task.Delay(1_000);
            if (CurrencyEntitiesTemp.Count > 0)
            {
                CurrencyEntities.Clear();
                CurrencyEntities.AddRange(CurrencyEntitiesTemp);
            }

            GetRateCurrency();
        }

        public void GetRateCurrency()
        {
            string currencyCode = SecureStorageService.GetCurrencyCode();
            Rate = CurrencyEntities.Where(p => p.Code == currencyCode).FirstOrDefault();
        }

        async Task WebSocketConnect()
        {
            if (webSockets.Count == 0) InitWebSocketList();

            // Connect to each websocket
            List<Task> connectionTasks = GetConnectionTasks();
            await Task.WhenAll(connectionTasks);

            PrintStatuses();
        }

        void PrintStatuses()
        {
            foreach (KeyValuePair<string, ClientWebSocket> entry in webSockets)
                Debug.WriteLine($"[WebSocketConnect] Channel ({entry.Key}) status {entry.Value.State}");
        }

        List<Task> GetConnectionTasks()
        {
            // Connect to each websocket
            List<Task> connectionTasks = new() { };
            foreach (KeyValuePair<string, ClientWebSocket> entry in webSockets)
            {
                var webSocketUrl = new Uri(string.Format(webSocketServerBaseUrl, entry.Key));

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
                                    Debug.WriteLine($"Error {e}");
                                    Debug.WriteLine($"Reconnecting to {entry.Key}'s channel");

                                    cts = new CancellationTokenSource();
                                    await entry.Value.ConnectAsync(webSocketUrl, cts.Token);
                                }

                                int amountToWaitInMilliseconds = btcPriceDelay; // 2.5 Seconds for btc-price
                                //if (entry.Key != "btc-price")
                                //    amountToWaitInMilliseconds = _WebSocketMessageDelay; // 5 Minutes for everything else

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
