using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xamarin.Forms;

using HodlWallet.UI.Locale;
using HodlWallet.Core.Services;

namespace HodlWallet.UI.Views
{
    public partial class CreateAccountView : ContentPage
    {
        public CreateAccountView()
        {
            InitializeComponent();
            SetLabels();
        }

        void SetLabels()
        {
            Title = LocaleResources.AddAccount_title;
            Header.Text = LocaleResources.AddAccount_header;
            //Subheader.Text = LocaleResources.AddAccount_subheader;
            AddAccountLbl.Text = LocaleResources.AddAccountText_label;
            CreateAcc.Text = LocaleResources.AddAccount_button;
        }
        
    }
}