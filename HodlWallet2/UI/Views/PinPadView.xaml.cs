using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HodlWallet2.Core.ViewModels;
using HodlWallet2.UI.Controls;
using Xamarin.Forms;

namespace HodlWallet2.UI.Views
{
    public partial class PinPadView : ContentPage
    {
        public PinPadView()
        {
            InitializeComponent();

            SetPinPadControlBindings();
            SubscribeToMessages();
        }

        void SetPinPadControlBindings()
        {
            var vm = (PinPadViewModel)BindingContext;

            PinPadControl.SetBinding(PinPad.TitleProperty, "PinPadTitle");
            PinPadControl.SetBinding(PinPad.HeaderProperty, "PinPadHeader");
            PinPadControl.SetBinding(PinPad.WarningProperty, "PinPadWarning");

            PinPadControl.BindingContext = vm;
        }

        void SubscribeToMessages()
        {
            MessagingCenter.Subscribe<PinPadViewModel>(this, "NavigateToBackupViewModel", async (vm) => await NavigateToBackupViewModel(vm));
        }

        async Task NavigateToBackupViewModel(PinPadViewModel _)
        {
            await Navigation.PushAsync(new BackupView(action: "skip"));
        }
    }
}
