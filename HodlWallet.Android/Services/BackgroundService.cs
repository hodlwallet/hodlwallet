//
// BackgroundService.cs
//
// Copyright (c) 2022 HODL Wallet
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
using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Xamarin.Forms;
using ReactiveUI;

using HodlWallet.Core.Interfaces;
using HodlWallet.Droid.Services;

[assembly: Dependency(typeof(BackgroundService))]
namespace HodlWallet.Droid.Services
{
    [Service]
    public class BackgroundService : Service, IBackgroundService
    {
        static ContextWrapper context;
        static readonly ConcurrentDictionary<string, Func<Task>> store = new ConcurrentDictionary<string, Func<Task>>();

        public static void Init(ContextWrapper context)
        {
            BackgroundService.context = context;
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public Task Start(string name, Func<Task> func)
        {
            Debug_WriteLine("[BackgroundService] [Start] Starting {0} service", name);

            store[name] = func;

            var intent = new Intent(name, null, context, typeof(BackgroundService));

            context.StartService(intent);

            return Task.CompletedTask;
        }

        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            Debug_WriteLine($"[BackgroundService] [OnStartCommand] {intent.Action} service");

            Observable.Start(async () =>
            {
                await store[intent.Action].Invoke();

                context.StopService(intent);
            }, RxApp.TaskpoolScheduler);

            return StartCommandResult.Sticky;
        }

        void Debug_WriteLine(params string[] args)
        {
            System.Diagnostics.Debug.WriteLine(args);
        }
    }
}