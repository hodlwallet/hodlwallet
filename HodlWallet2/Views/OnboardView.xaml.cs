using System;

namespace HodlWallet2.Views
{
    public partial class OnboardView : BaseView
    {
        public OnboardView()
        {
            InitializeComponent();
        }

        private void CreateButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SetPinView());
        }

        private void RecoverButton_Clicked(object sender, EventArgs e)
        {
            _Logger.Information("Recover button clicked.");
        }
    }
}
