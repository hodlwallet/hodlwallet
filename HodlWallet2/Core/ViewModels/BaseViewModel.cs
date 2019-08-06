using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Xamarin.Forms;

using Refit;

using HodlWallet2.Core.Interfaces;

using HodlWallet2.Core.Utils;

namespace HodlWallet2.Core.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public IWalletService _WalletService => DependencyService.Get<IWalletService>();
        public IShareIntent _ShareIntent => DependencyService.Get<IShareIntent>();
        public IPrecioHttpService _PrecioHttpService => RestService.For<IPrecioHttpService>(Constants.PRECIO_HOST_URL);
        public IPrecioService _PrecioService => DependencyService.Get<IPrecioService>();
        public IPermissions _PermissionsService => DependencyService.Get<IPermissions>();

        bool _IsLoading;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set { SetProperty(ref _IsLoading, value); }
        }

        string _Title = string.Empty;
        public string Title
        {
            get { return _Title; }
            set { SetProperty(ref _Title, value); }
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName]string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
