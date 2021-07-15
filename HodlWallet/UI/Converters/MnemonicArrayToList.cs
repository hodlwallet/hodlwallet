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

namespace HodlWallet.UI.Converters
{
    class MnemonicArrayToList
    {
        public static List<BackupWordModel> GenerateWordsList(string[] mnemonics)
        {
            int index = 0;
            List<BackupWordModel> words = new List<BackupWordModel>();
            foreach (var word in mnemonics)
            {
                index++;
                words.Add(new BackupWordModel() { Word = word, WordIndex = index.ToString() });
            }
            return words;

            //Temp code to print 12 extra-mnemonics
            //for (int i = 0; i < _Mnemonic.Length; i++)
            //{
            //    index++;
            //    Words.Add(new BackupWordModel() { Word = $"{_Mnemonic[i]}{index}" , WordIndex = index.ToString() });
            //}
        }
    }
}
