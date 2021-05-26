﻿using HodlWallet.UI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HodlWallet.UI.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FlyoutFooter : ContentView
    {
        public FlyoutFooter()
        {
            InitializeComponent();
        }

        async void CreateAcc_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(CreateAccountView));
        }
    }
}