//
// BalanceHeader.xaml.cs
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
using System.ComponentModel;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using HodlWallet.Core.ViewModels;
using HodlWallet.Core.Services;

namespace HodlWallet.UI.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BalanceHeader : ContentView
    {
        BalanceHeaderViewModel ViewModel => BindingContext as BalanceHeaderViewModel;

        public BalanceHeader()
        {
            InitializeComponent();

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;

            _ = ToggleTo(ViewModel.CurrencyType, animate: false);
        }

        async void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!e.PropertyName.Equals(nameof(ViewModel.CurrencyType))) return;

            await ToggleTo(ViewModel.CurrencyType);
        }

        async Task ToggleTo(DisplayCurrencyType displayCurrencyType, bool animate = true)
        {
            if (animate) await Task.WhenAll(balanceLabel.FadeTo(0.00), balanceFiatLabel.FadeTo(0.00));

            if (displayCurrencyType == DisplayCurrencyType.Bitcoin)
            {
                Grid.SetRow(balanceLabel, 0);
                Grid.SetRow(balanceFiatLabel, 1);

                balanceLabel.Margin = new Thickness(0, 3, 0, 0);
                balanceLabel.FontSize = Device.GetNamedSize(NamedSize.Subtitle, typeof(Label));

                balanceFiatLabel.Margin = new Thickness(0, -4, 0, 0);
                balanceFiatLabel.FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label));
            }
            else
            {
                Grid.SetRow(balanceLabel, 1);
                Grid.SetRow(balanceFiatLabel, 0);

                balanceLabel.Margin = new Thickness(0, -4, 0, 0);
                balanceLabel.FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label));

                balanceFiatLabel.Margin = new Thickness(0, 3, 0, 0);
                balanceFiatLabel.FontSize = Device.GetNamedSize(NamedSize.Subtitle, typeof(Label));
            }

            if (animate) await Task.WhenAll(balanceLabel.FadeTo(1.00), balanceFiatLabel.FadeTo(1.00));
        }
    }
}