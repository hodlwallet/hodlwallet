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
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

using Xamarin.Forms;

using HodlWallet.Core.Models;
using HodlWallet.UI.Converters;
using HodlWallet.Core.Utils;

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
            PopulateEmptyOrderedWords();
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

        private void PopulateEmptyOrderedWords()
        {
            for (int i = 0; i < OriginalWordsList.Count; i++)
            {
                OrderedWordsCollection.Add(new BackupWordModel { Word = String.Empty, WordIndex = i.ToString() });
            }
        }

        private void NextWord()
        {
            MessagingCenter.Send(this, "NavigateToRootView");
        }

        void UnorderedTappedWord(object obj)
        {
            BackupWordModel tappedWord = obj as BackupWordModel;
            int nextIndexPositionInOrderedCollection = NextAvailablePosition(OrderedWordsCollection);
            int actualIndexPositionInShuffledCollection = ShuffledWordsCollection.IndexOf(tappedWord);

            // if tap in a free slot do nothing
            if (tappedWord.Word == String.Empty || nextIndexPositionInOrderedCollection == -1)
                return;

            // Add the tapped word to OrderedWordsCollection.
            if (!OrderedWordsCollection.Contains(tappedWord))
            {
                OrderedWordsCollection.RemoveAt(nextIndexPositionInOrderedCollection);
                OrderedWordsCollection.Insert(nextIndexPositionInOrderedCollection, tappedWord);
            }

            // Frees tapped word's slot.
            if (actualIndexPositionInShuffledCollection >= 0)
            {
                ShuffledWordsCollection.RemoveAt(actualIndexPositionInShuffledCollection);
                ShuffledWordsCollection.Insert(actualIndexPositionInShuffledCollection, new BackupWordModel { Word = String.Empty, WordIndex = actualIndexPositionInShuffledCollection.ToString() });
            }

            // Checks again for last word in Collection then verifies if mnemonic is correct
            nextIndexPositionInOrderedCollection = NextAvailablePosition(OrderedWordsCollection);
            if (nextIndexPositionInOrderedCollection == -1)
                CheckWordLists();
        }


        void OrderedTappedWord(object obj)
        {
            BackupWordModel tappedWord = obj as BackupWordModel;
            int nextIndexPositionInShuffledCollection = NextAvailablePosition(ShuffledWordsCollection);
            int actualIndexPositionInOrderedCollection = OrderedWordsCollection.IndexOf(tappedWord);

            // if tap in a free slot do nothing
            if (tappedWord.Word == String.Empty || nextIndexPositionInShuffledCollection == -1)
                return;

            // Add the tapped word to ShuffledWordsCollection.
            if (!ShuffledWordsCollection.Contains(tappedWord))
            {
                ShuffledWordsCollection.RemoveAt(nextIndexPositionInShuffledCollection);
                ShuffledWordsCollection.Insert(nextIndexPositionInShuffledCollection, tappedWord);
            }

            // Frees tapped word's slot.
            if (actualIndexPositionInOrderedCollection >= 0)
            {
                collectionsEqual = false;
                OrderedWordsCollection.RemoveAt(actualIndexPositionInOrderedCollection);
                OrderedWordsCollection.Insert(actualIndexPositionInOrderedCollection, new BackupWordModel { Word = String.Empty, WordIndex = actualIndexPositionInOrderedCollection.ToString() });
                ToggleIncorrectMessage(!collectionsEqual);
                ToggleDoneButton();
            }
        }

        private int NextAvailablePosition(ObservableCollection<BackupWordModel> collection)
        {
            int idx = -1;
            for (int i = 0; i < collection.Count; i++)
            {
                if (collection[i].Word == String.Empty)
                {
                    idx = i;
                    break;
                }
            }
            return idx;
        }

        void CheckWordLists()
        {
            var orderedMnemonicStr = string.Join(' ', OrderedWordsCollection.Select((i) => i.Word));
            var originalMnemonicStr = string.Join(' ', OriginalWordsList.Select((i) => i.Word));

            collectionsEqual = string.Equals(orderedMnemonicStr, originalMnemonicStr);
            if (collectionsEqual)
                ToggleDoneButton();

            ToggleIncorrectMessage(collectionsEqual);
        }

        void ToggleDoneButton()
        {
            MessagingCenter.Send(this, "CollectionsAreEqual", collectionsEqual);
        }

        void ToggleIncorrectMessage(bool equals)
        {
            MessagingCenter.Send(this, "ErrorMessageToggle", equals);
        }
    }
}
