//
// MnemonicArrayToList.cs
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
using System.Collections.Generic;
using System.Text;

using HodlWallet.Core.Models;
using HodlWallet.Core.Services;
using Liviano.Exceptions;

namespace HodlWallet.UI.Converters
{
    public class MnemonicStringToList
    {
        public static List<BackupWordModel> GenerateWordsList()
        {
            int index = 0;
            string rawMnemonic = GetMnemonic();

            string[] mnemonic = rawMnemonic.Split(' ');

            List<BackupWordModel> words = new();
            foreach (var word in mnemonic)
            {
                index++;
                words.Add(new BackupWordModel() { Word = word, WordIndex = index.ToString() });
            }
            return words;

            //Temp code to print 12 extra-mnemonics
            //for (int i = 0; i < mnemonic.Length; i++)
            //{
            //    index++;
            //    Words.Add(new BackupWordModel() { Word = $"{mnemonic[i]}{index}" , WordIndex = index.ToString() });
            //}
        }

        static string GetMnemonic()
        {
            if (!SecureStorageService.HasMnemonic())
                throw new WalletException("This wallet doesn't have a mnemonic, we cannot do anything without that one");

            return SecureStorageService.GetMnemonic();
        }
    }
}
