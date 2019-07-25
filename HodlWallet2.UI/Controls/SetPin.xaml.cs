using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HodlWallet2.UI.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SetPin : PopupPage
    {
        public SetPin()
        {
            InitializeComponent();
        }

        protected override async void OnAppearingAnimationEnd()
        {
            base.OnAppearingAnimationEnd();
            await Task.Delay(2000);
            await Navigation.PopPopupAsync();
        }
    }
}
