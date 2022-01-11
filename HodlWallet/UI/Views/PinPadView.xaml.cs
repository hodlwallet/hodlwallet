//
// PinPadView.xaml.cs
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
using System.Linq;
using System.Threading.Tasks;
using HodlWallet.Core.ViewModels;
using HodlWallet.UI.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HodlWallet.UI.Views
{
    public partial class PinPadView : ContentPage
    {
        string nextView;
        public PinPadView(string nextView)
        {
            InitializeComponent();
            SetPinPadControlBindings();
            SubscribeToMessages();

            this.nextView = nextView;
            if (this.nextView == "PinPadChangeView")
            {
                CloseToolbarItem.IsEnabled = true;
            }
            else
            {
                CloseToolbarItem.IsEnabled = false;
            }
        }

        void CloseToolbarItem_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        void SetPinPadControlBindings()
        {
            var vm = (PinPadViewModel)BindingContext;

            PinPadControl.SetBinding(PinPad.TitleProperty, "PinPadTitle");
            PinPadControl.SetBinding(PinPad.HeaderProperty, "PinPadHeader");
            PinPadControl.SetBinding(PinPad.WarningProperty, "PinPadWarning");

            PinPadControl.BindingContext = vm;
        }

        void SubscribeToMessages()
        {
            MessagingCenter.Subscribe<PinPadViewModel>(this, "NavigateToNextView", NavigateToNextView);
        }

        async void NavigateToNextView(PinPadViewModel _)
        {
            switch(nextView)
            {
                case "NewWalletInfoView":
                    await Navigation.PushAsync(new NewWalletInfoView());
                    break;

                case "PinPadChangeView":
                    await Navigation.PushAsync(new PinPadChangeView());
                    break;

                case "RecoverInfoView":
                    await Navigation.PushAsync(new RecoverInfoView());
                    break;
            }

            // No page should go back to the pinpad view
            if (Navigation.NavigationStack.Contains(this)) Navigation.RemovePage(this);
        }
    }
}
