//
// BackupRecoveryConfirmViewModel.cs
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
using System.Windows.Input;

using Xamarin.Forms;

using HodlWallet.Core.Models;
using HodlWallet.UI.Converters;
using HodlWallet.Core.Utils;
using System.Collections.ObjectModel;
using System.Linq;

namespace HodlWallet.Core.ViewModels
{
    public class BackupRecoveryConfirmViewModel : BaseViewModel
    {
        bool collectionsEqual = false;
        public ObservableCollection<BackupWordModel> ShuffledWordsCollection { get; set; }
        public ObservableCollection<BackupWordModel> OrderedWordsCollection { get; set; } = new();
        public List<BackupWordModel> OriginalWordsList { get; set; } = new();
        public ICommand NextCommand { get; }
        public ICommand TapUnorderedCommand { get; }
        public ICommand TapOrderedCommand { get; }



        public BackupRecoveryConfirmViewModel()
        {
            NextCommand = new Command(NextWord);
            TapUnorderedCommand = new Command(UnorderedTappedWord);
            TapOrderedCommand = new Command(OrderedTappedWord);
            GenerateShuffledMnemonics();
        }

        void GenerateShuffledMnemonics()
        {
            List<BackupWordModel> shuffledWordsList = new();
            //Bring the Mnemonic list from SecureStorage as a List and copy it to a new list
            OriginalWordsList = MnemonicStringToList.GenerateWordsList();
            shuffledWordsList.AddRange(OriginalWordsList);
            //Suffle the copy of the original list.
            shuffledWordsList.Shuffle();
            ShuffledWordsCollection = new ObservableCollection<BackupWordModel>(shuffledWordsList);

        }

        private void NextWord()
        {
            //MessagingCenter.Send(this, "NavigateToExtraBackupView", shuffledWordsList);
        }

        void UnorderedTappedWord(object obj)
        {
            var TappedWord = obj as BackupWordModel;
            OrderedWordsCollection.Add(TappedWord);
            ShuffledWordsCollection.Remove(TappedWord);
            if (OrderedWordsCollection.Count() == OriginalWordsList.Count())
                CheckWordLists();
        }

        void OrderedTappedWord(object obj)
        {
            var TappedWord = obj as BackupWordModel;
            ShuffledWordsCollection.Add(TappedWord);
            OrderedWordsCollection.Remove(TappedWord);
            collectionsEqual = false;
            if (OrderedWordsCollection.Count() == OriginalWordsList.Count() - 1)
                sendStatusNotification();
        }

        void CheckWordLists()
        {
            collectionsEqual = OrderedWordsCollection.SequenceEqual(OriginalWordsList);
            if (collectionsEqual)
                sendStatusNotification();
        }

        void sendStatusNotification()
        {
            MessagingCenter.Send(this, "CollectionsAreEqual", collectionsEqual);
        }
    }
}