//
// BackupRecoveryWordView.xaml.cs
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
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using HodlWallet.Core.ViewModels;
using HodlWallet.UI.Locale;
using HodlWallet.Core.Models;

namespace HodlWallet.UI.Views
{
    public partial class BackupRecoveryWordView : ContentPage
    {
        List<BackupWordModel> words = new List<BackupWordModel>();
        public BackupRecoveryWordView()
        {
            InitializeComponent();
            SubscribeToMessages();
            GenerateGridWithWords();
            SetLabels();
        }

        void SetLabels()
        {
            Title = LocaleResources.BackupWord_title;
            Header.Text = LocaleResources.BackupWord_header;
            Next.Text = LocaleResources.BackupWord_next;
        }

        void SubscribeToMessages()
        {
            MessagingCenter.Subscribe<BackupRecoveryWordViewModel, string[]>(this, "NavigateToBackupRecoveryConfirmView", NavigateToBackupRecoveryConfirmView);
        }

        async void NavigateToBackupRecoveryConfirmView(BackupRecoveryWordViewModel _, string[] mnemonic)
        {
            Debug.WriteLine($"[NavigateToBackupRecoveryConfirmView] About to write mnemonic: {string.Join(" ", mnemonic)}");

            await Navigation.PushAsync(new BackupRecoveryConfirmView(mnemonic));
        }

        void GenerateGridWithWords()
        {
            var vm = BindingContext as BackupRecoveryWordViewModel;
            words = (List<BackupWordModel>)vm.words;

            var wordKey = 0;
            var indexKey = 0;

            for (int rowIndex = 0; rowIndex < words.Count; rowIndex++)
            {
                if (rowIndex % 2 == 0)
                {
                    for (int columnIndex = 0; columnIndex < 3; columnIndex++)
                    {
                        if (wordKey >= words.Count)
                            break;
                        var label = new Label
                        {
                            Text = words[wordKey].Word,
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.Center
                        };
                        wordKey += 1;
                        gridLayout.Children.Add(label, columnIndex, rowIndex);
                    }
                }
                else
                {
                    for (int columnIndex = 0; columnIndex < 3; columnIndex++)
                    {
                        if (indexKey >= words.Count)
                            break;
                        var labelWordIndex = new Label
                        {
                            Text = words[indexKey].WordIndex,
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.Center
                        };
                        indexKey += 1;
                        gridLayout.Children.Add(labelWordIndex, columnIndex, rowIndex);
                    }
                }
            }
        }
    }
}