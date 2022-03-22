﻿//
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
using System;
using System.Diagnostics;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.CommunityToolkit.Extensions;

using HodlWallet.Core.ViewModels;

namespace HodlWallet.UI.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BalanceHeader : ContentView
    {
        BalanceHeaderViewModel ViewModel => BindingContext as BalanceHeaderViewModel;
        bool toggle = true;

        public BalanceHeader()
        {
            InitializeComponent();
        }

        void BalanceHeader_Appearing(object sender, EventArgs e)
        {
            Debug.WriteLine("[BalanceHeader_Appearing] Find out the current display currency");
        }

        void OnBalanceGrid_Tapped(object sender, EventArgs e)
        {
            Debug.WriteLine("[OnBalanceLabels_Tapped] Flipping default currency");

            if (toggle)
            {
                Grid.SetRow(balanceLabel, 1);
                Grid.SetRow(balanceFiatLabel, 0);

                balanceLabel.Margin = new Thickness(0, -7, 0, 0);
                balanceLabel.FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label));

                balanceFiatLabel.Margin = new Thickness(0, 3, 0, 0);
                balanceFiatLabel.FontSize = Device.GetNamedSize(NamedSize.Subtitle, typeof(Label));
            }
            else
            {
                Grid.SetRow(balanceLabel, 0);
                Grid.SetRow(balanceFiatLabel, 1);

                balanceLabel.Margin = new Thickness(0, 3, 0, 0);
                balanceLabel.FontSize = Device.GetNamedSize(NamedSize.Subtitle, typeof(Label));

                balanceFiatLabel.Margin = new Thickness(0, -7, 0, 0);
                balanceFiatLabel.FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label));
            }

            toggle = !toggle;
        }
    }
}