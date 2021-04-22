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

            int wordKey = 0;
            int indexKey = 0;
            string word;

            for (int rowIndex = 0; rowIndex < words.Count; rowIndex++)
            {
                if (indexKey >= words.Count)
                    break;
                for (int columnIndex = 0; columnIndex < 3; columnIndex++)
                {
                        if (rowIndex % 2 == 0)
                        {
                            word = words[wordKey].Word;
                            wordKey += 1;
                        } 
                        else
                        {
                            word = words[indexKey].WordIndex;
                            indexKey += 1;
                        }

                        var label = new Label
                        {
                            Text = word,
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.Center
                        };
                        gridLayout.Children.Add(label, columnIndex, rowIndex);
                }                
            }
        }
    }
}