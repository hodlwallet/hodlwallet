using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HodlWallet.UI.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SecuritySettingsView : ContentPage
    {
        public SecuritySettingsView()
        {
            InitializeComponent();
        }

        private void CloseToolbarItem_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
        
        void BackupMnemonic_Clicked(object sender, EventArgs e)
        {
            var view = new BackupView(action: "close");
            var nav = new NavigationPage(view);

            Navigation.PushModalAsync(nav);
        }

        void PinButton_Clicked(object sender, EventArgs e)
        {
            var view = new PinSettingsView();
            var nav = new NavigationPage(view);

            Navigation.PushModalAsync(nav);
        }

    }
}