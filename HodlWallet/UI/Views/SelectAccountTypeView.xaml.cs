﻿//
// SelectAccountTypeView.xaml.cs
//
// Author:
//       Igor Guerrero <igorgue@protonmail.com>
//
// Copyright (c) 2021 HODL Wallet
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

using HodlWallet.Core.ViewModels;
using HodlWallet.UI.Extensions;
using System.Diagnostics;

namespace HodlWallet.UI.Views
{
    public partial class SelectAccountTypeView : ContentPage
    {
        bool initialLoad = true;

        SelectAccountTypeViewModel ViewModel => BindingContext as SelectAccountTypeViewModel;
        Color SELECTED_COLOR => (Color)Application.Current.Resources["TextSuccess"];

        public SelectAccountTypeView()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<SelectAccountTypeViewModel>(this, "AnimateSelected", AnimateSelected);
            MessagingCenter.Subscribe<SelectAccountTypeViewModel>(this, "HideEmptyState", HideEmptyState);
        }

        void HideEmptyState(SelectAccountTypeViewModel vm)
        {
            PleaseSelectAccountTypeLabel.FadeTo(0.00);
        }

        void AnimateSelected(SelectAccountTypeViewModel vm)
        {
            switch (vm.AccountType)
            {
                case "standard":
                    StandardFrame.ColorTo(StandardFrame.BackgroundColor, SELECTED_COLOR, c => StandardFrame.BackgroundColor = c, 250);

                    if (!initialLoad)
                        LegacyFrame.ColorTo(LegacyFrame.BackgroundColor, Color.Default, c => LegacyFrame.BackgroundColor = c, 100);

                    break;
                case "legacy":
                    if (!initialLoad)
                        StandardFrame.ColorTo(StandardFrame.BackgroundColor, Color.Default, c => StandardFrame.BackgroundColor = c, 100);

                    LegacyFrame.ColorTo(LegacyFrame.BackgroundColor, SELECTED_COLOR, c => LegacyFrame.BackgroundColor = c, 250);

                    break;
                default:
                    break;
            }

            initialLoad = false;
        }

        void DoneButton_Clicked(object sender, EventArgs e)
        {
            Debug.WriteLine($"[DoneButton_Clicked] Account type selected: {ViewModel.AccountType}");
        }
    }
}