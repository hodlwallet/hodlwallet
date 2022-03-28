//
// RecoverAccountTypeView.xaml.cs
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
using System.Diagnostics;

using Xamarin.Forms;

using HodlWallet.Core.ViewModels;
using HodlWallet.UI.Extensions;

namespace HodlWallet.UI.Views
{
    public partial class RecoverAccountTypeView : ContentPage
    {
        bool initialLoad = true;

        RecoverAccountTypeViewModel ViewModel => BindingContext as RecoverAccountTypeViewModel;

        Color SELECTED_COLOR => (Color)Application.Current.Resources["FgSuccess"];
        Color UNSELECTED_COLOR => (Color)Application.Current.Resources["Bg2"];

        Color UNSELECTED_TEXT_COLOR => (Color)Application.Current.Resources["Fg"];
        Color SELECTED_TEXT_COLOR => (Color)Application.Current.Resources["Bg"];

        public RecoverAccountTypeView()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<RecoverAccountTypeViewModel>(this, "AnimateSelected", AnimateSelected);
            MessagingCenter.Subscribe<RecoverAccountTypeViewModel>(this, "HideEmptyState", HideEmptyState);
            MessagingCenter.Subscribe<RecoverAccountTypeViewModel>(this, "InitAppShell", InitAppShell);
        }

        void InitAppShell(RecoverAccountTypeViewModel vm)
        {
            Application.Current.MainPage = new AppShell();
        }

        void HideEmptyState(RecoverAccountTypeViewModel vm)
        {
            PleaseSelectAccountTypeLabel.FadeTo(0.00, 500);
        }

        void AnimateSelected(RecoverAccountTypeViewModel vm)
        {
            switch (vm.AccountType)
            {
                case "standard":
                    if (!initialLoad)
                        LegacyFrame.ColorTo(LegacyFrame.BackgroundColor, UNSELECTED_COLOR, c => LegacyFrame.BackgroundColor = c, 100);

                    StandardFrame.ColorTo(StandardFrame.BackgroundColor, SELECTED_COLOR, c => StandardFrame.BackgroundColor = c, 250);

                    StandardTitleLabel.TextColor = SELECTED_TEXT_COLOR;
                    StandardDescriptionLabel.TextColor = SELECTED_TEXT_COLOR;
                    LegacyTitleLabel.TextColor = UNSELECTED_TEXT_COLOR;
                    LegacyDescriptionLabel.TextColor = UNSELECTED_TEXT_COLOR;

                    break;
                case "legacy":
                    if (!initialLoad)
                        StandardFrame.ColorTo(StandardFrame.BackgroundColor, UNSELECTED_COLOR, c => StandardFrame.BackgroundColor = c, 100);

                    LegacyFrame.ColorTo(LegacyFrame.BackgroundColor, SELECTED_COLOR, c => LegacyFrame.BackgroundColor = c, 250);

                    StandardTitleLabel.TextColor = UNSELECTED_TEXT_COLOR;
                    StandardDescriptionLabel.TextColor = UNSELECTED_TEXT_COLOR;
                    LegacyTitleLabel.TextColor = SELECTED_TEXT_COLOR;
                    LegacyDescriptionLabel.TextColor = SELECTED_TEXT_COLOR;

                    break;
                default:
                    break;
            }

            initialLoad = false;
            DoneButton.IsVisible = true;
        }

        void DoneButton_Clicked(object sender, EventArgs e)
        {
            Debug.WriteLine($"[DoneButton_Clicked] Account type selected: {ViewModel.AccountType}");

            ViewModel.InitializeWallet(ViewModel.AccountType);
        }
    }
}
