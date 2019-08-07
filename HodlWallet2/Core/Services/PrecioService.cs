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

using Xamarin.Forms;

using HodlWallet2.Core.Interfaces;
using HodlWallet2.Core.Services;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Liviano.Utilities;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices.ComTypes;
using ZXing;

[assembly: Dependency(typeof(PrecioService))]
namespace HodlWallet2.Core.Services
{
    public class PrecioService : IPrecioService, INotifyPropertyChanged
    {
        string _WebSocketServerBaseUrl = "wss://precio.bitstop.co/{0}";
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

        bool _RunningTimers;
        public bool RunningTimers
        {
            get => _RunningTimers;
            set => SetProperty(ref _RunningTimers, value);
        }

        public PrecioService()
        {
            _WebSockets = new Dictionary<string, ClientWebSocket>();
            _RunningTimers = false;
        }

        public void Init()
        {
            Task.Run(WebSocketConnect);
        }

        void InitWebSocketList()
        {
            // Insert each channel with each web socket
            foreach (var channel in _Channels)
                _WebSockets[channel] = new ClientWebSocket();
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
                        _ = Task.Run(async () =>
                        {
                            while (true)
                            {
                                try
                                {
                                    string result = await ReadMessage(entry.Value, cts.Token);

                                    Debug.WriteLine($"[GetConnectionTasks][ReadMessage] Message from {entry.Key}: {result}");
                                }
                                catch (Exception e)
                                {
                                    Debug.WriteLine($"Error {e.ToString()}");
                                    Debug.WriteLine($"Reconnecting to {entry.Key}'s channel");

                                    cts = new CancellationTokenSource();
                                    await entry.Value.ConnectAsync(webSocketUrl, cts.Token);
                                }
                            }
                        });
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

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
