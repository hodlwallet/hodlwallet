using System;
using System.Windows.Input;
using Serilog;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace HodlWallet2.ViewModels
{
    public class SecureStorageViewModel
    {
        string key;
        string securedValue;
        ILogger _Logger;

        public SecureStorageViewModel()
        {
            _Logger = Wallet.Instance.Logger;

            LoadCommand = new Command(OnLoad);
            SaveCommand = new Command(OnSave);
            RemoveCommand = new Command(OnRemove);
            RemoveAllCommand = new Command(OnRemoveAll);
        }

        public string Key;

        public string SecuredValue;

        public ICommand LoadCommand { get; }

        public ICommand SaveCommand { get; }

        public ICommand RemoveCommand { get; }

        public ICommand RemoveAllCommand { get; }

        async void OnLoad()
        {
            try
            {
                SecuredValue = await SecureStorage.GetAsync(Key) ?? string.Empty;
            }
            catch (Exception ex)
            {
                _Logger.Error(ex.Message);
            }
        }

        async void OnSave()
        {
            try
            {
                await SecureStorage.SetAsync(Key, SecuredValue);
            }
            catch (Exception ex)
            {
                _Logger.Error(ex.Message);
            }
        }

        async void OnRemove()
        {
            try
            {
                SecureStorage.Remove(Key);
            }
            catch (Exception ex)
            {
                _Logger.Error(ex.Message);
            }
        }

        async void OnRemoveAll()
        {
            try
            {
                SecureStorage.RemoveAll();
            }
            catch (Exception ex)
            {
                _Logger.Error(ex.Message);
            }
        }
    }
}