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
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Essentials;
using Xamarin.Forms;

using HodlWallet.Core.Services;
using HodlWallet.UI.Locale;
using System.Collections.Generic;
using HodlWallet.Core.Models;
using System.Diagnostics;
using HodlWallet.UI.Converters;
using HodlWallet.Core.Utils;

namespace HodlWallet.Core.ViewModels
{
    public class BackupRecoveryConfirmViewModel : BaseViewModel
    {
        public List<BackupWordModel> ShuffledWordsList { get; set; } = new();

        public ICommand NextCommand { get; }

        //public List<BackupWordModel> Mnemonic
        //{
        //    get => mnemonic;
        //    set
        //    {
        //        SetProperty(ref mnemonic, value);
        //    }
        //}

        public BackupRecoveryConfirmViewModel()
        {
            NextCommand = new Command(NextWord);
          
            Debug.WriteLine($"============>BackupRecoveryConfirmViewModel, ya tenemos la lista revuelta!!! {ShuffledWordsList.Count}");
            MessagingCenter.Subscribe<BackupRecoveryWordViewModel, List<BackupWordModel>>(this, "MnemonicListMessage", GenerateShuffledMnemonics);

            //TestList = new List<BackupWordModel>{Word="Marconi", WordIndex="Uno" };

        }

        public void GenerateShuffledMnemonics(BackupRecoveryWordViewModel _, List<BackupWordModel> Words)
        {
            ShuffledWordsList.AddRange(Words);
            ShuffledWordsList.Shuffle();


            Console.WriteLine("Lista A");
            foreach (var item in Words)
            {
                Console.WriteLine($"{item.WordIndex} --> {item.Word}");
            }
            Console.WriteLine("Lista B");
            foreach (var item in ShuffledWordsList)
            {
                Console.WriteLine($"{item.WordIndex} --> {item.Word}");
            }
        }

        private void NextWord()
        {
            Debug.WriteLine("============> ALL DONE!!!");
        }


        //void ShuffleMnemonic(string[] mnemonic)
        //{
        //    WordList = MnemonicArrayToList.GenerateWordsList(mnemonic);


        //    ShuffledWordsList = WordList.ToList();
        //    ShuffledWordsList.Shuffle<BackupWordModel>();

        //    Console.WriteLine("Lista A");
        //    foreach (var item in WordList)
        //    {
        //        Console.WriteLine($"{item.WordIndex} --> {item.Word}");
        //    }
        //    Console.WriteLine("Lista B");
        //    foreach (var item in ShuffledWordsList)
        //    {
        //        Console.WriteLine($"{item.WordIndex} --> {item.Word}");
        //    }
        //}
    }
}