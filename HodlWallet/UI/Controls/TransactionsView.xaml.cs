//
// TransactionsView.xaml.cs
//
// Author:
//       Igor Guerrero <igorgue@protonmail.com>
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
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ReactiveUI;

using HodlWallet.Core.ViewModels;
using HodlWallet.Core.Models;
using HodlWallet.UI.Views;

namespace HodlWallet.UI.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TransactionsView : StackLayout
    {
        const int EMPTY_VIEW_DELAY_MS = 3_000;

        bool openingDetails = false;

        TransactionsViewModel ViewModel => BindingContext as TransactionsViewModel;

        readonly CancellationTokenSource cts = new();

        public TransactionsView()
        {
            InitializeComponent();

            emptyView.IsVisible = false;

            SubscribeToMessages();

            Observable
                .Start(async () => await ShowEmptyAfterTimeout(), RxApp.MainThreadScheduler)
                .Subscribe(cts.Token);
        }

        async Task ShowEmptyAfterTimeout()
        {
            await Task.Delay(EMPTY_VIEW_DELAY_MS);

            emptyView.IsVisible = true;
        }

        void SubscribeToMessages()
        {
            MessagingCenter.Subscribe<TransactionsViewModel>(this, "ScrollToTop", ScrollToTop);
            MessagingCenter.Subscribe<TransactionsViewModel, TransactionModel>(this, "NavigateToTransactionDetail", NavigateToTransactionDetail);
        }

        async void NavigateToTransactionDetail(TransactionsViewModel _, TransactionModel model)
        {
            if (openingDetails) return;

            openingDetails = true;
            await Navigation.PushAsync(new TransactionDetailsView(model));
            openingDetails = false;
        }

        void ScrollToTop(TransactionsViewModel vm)
        {
            if (vm.Transactions.Count <= 0) return;

            // TODO Add this when header is implemented
            //if (headerView.IsVisible) transactionsCollectionView.ScrollTo(1);
            else transactionsCollectionView.ScrollTo(0);
        }

        public void ScrollToTop()
        {
            ScrollToTop(ViewModel);
        }
    }
}