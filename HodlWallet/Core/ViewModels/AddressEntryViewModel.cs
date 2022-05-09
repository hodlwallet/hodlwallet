//
// AddressEntryViewModel.cs
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
using CommunityToolkit.Mvvm.ComponentModel;
using NBitcoin;
using Xamarin.Forms;

namespace HodlWallet.Core.ViewModels;

public partial class AddressEntryViewModel : LightBaseViewModel
{
    [ObservableProperty]
    [AlsoNotifyChangeFor(nameof(IsValid))]
    [AlsoNotifyChangeFor(nameof(FgColor))]
    string address;

    public BitcoinAddress BitcoinAddress => WalletService.GetNetwork().Parse<BitcoinAddress>(Address);

    public bool IsValid => BitcoinAddress is not null;

    Color Fg => (Color)Application.Current.Resources["Fg"];
    
    Color FgError => (Color)Application.Current.Resources["FgError"];

    public Color FgColor => IsValid ? Fg : FgError;
}