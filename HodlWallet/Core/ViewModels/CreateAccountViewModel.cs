//
// CreateAccountViewModel.cs
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
using System.Linq;
using System.Windows.Input;

using Xamarin.Forms;

using HodlWallet.Core.Utils;
using HodlWallet.UI.Controls;
using HodlWallet.UI.Converters;
using HodlWallet.UI.Locale;

namespace HodlWallet.Core.ViewModels
{
    class CreateAccountViewModel : BaseViewModel
    {
        Dictionary<string, string> accountTypesList = 
            new() { 
                    { "bip84", LocaleResources.AddAccount_options_bip84 },
                    { "bip141", LocaleResources.AddAccount_options_bip141 }
            };
        public Dictionary<string, string> AccountTypes { get => accountTypesList; }
        public ICommand CreateAccountCommand { get; }

        string accountName;

        public string AccountName
        {
            get => accountName;
            set => SetProperty(ref accountName, value);
        }

        string accountType;

        public string AccountType
        {
            get => accountType;
            set => SetProperty(ref accountType, value);
        }

        string accountColor = ((Color)Application.Current.Resources["ColorPicker10"]).ToHexString();

        public string AccountColor
        {
            get => accountColor;
            set => SetProperty(ref accountColor, value);
        }

        public CreateAccountViewModel()
        {
            CreateAccountCommand = new Command<string>((color) => CreateAccount(color));
        }
        private async void CreateAccount(string colorCode)
        {
            var color = ColorPicker.colorPickerControlList[int.Parse(colorCode)];
            // Get a hexadecimal string of the color to be stored.
            AccountColor = $"{colorCode}{color.ToHexString()}";

            string keyType = GetKeyFromValue();
            DisplayCreatingAccountAlert(
                Constants.LABEL_ALERT_CREATING_ACCOUNT_PROGRESS_MESSAGE);
            (bool Success, string Error) = await WalletService.AddAccount(keyType, AccountName, AccountColor);

            if (!Success && !string.IsNullOrEmpty(Error))
            {
                DisplayCreatingAccountAlert(
                    Constants.LABEL_ERROR_ACCOUNT_CREATION_MESSAGE, Error);
            }
            else
            {
                DisplayCreatingAccountAlert(
                    Constants.LABEL_ALERT_SUCCESS_ACCOUNT_CREATION_MESSAGE);
                await Shell.Current.GoToAsync("..");
            }
        }

        void DisplayCreatingAccountAlert(string labelMessage, string errorMessage = "")
        {
            var message = Constants.DISPLAY_ALERT_CREATING_ACCOUNT_PROGRESS_MESSAGE;
            if (!string.IsNullOrEmpty(errorMessage))
                message = errorMessage;
            else if (labelMessage.Equals(Constants.LABEL_ALERT_SUCCESS_ACCOUNT_CREATION_MESSAGE))
                message = Constants.DISPLAY_ALERT_SUCCESS_ACCOUNT_CREATION_MESSAGE; ;

            MessagingCenter.Send(this, labelMessage, message);
        }

        string GetKeyFromValue()
        {
            string keyFound = accountTypesList.ElementAt(0).Key;
            if (!string.IsNullOrEmpty(AccountType))
                foreach (string keyVar in accountTypesList.Keys)
                {
                    if (accountTypesList[keyVar] == AccountType)
                    {
                        keyFound = keyVar;
                        break;
                    }
                }
            return keyFound;
        }
    }
}
