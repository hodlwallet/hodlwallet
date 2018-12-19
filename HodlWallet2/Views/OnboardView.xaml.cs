using Xamarin.Forms;

namespace HodlWallet2.Views
{
    public partial class OnboardView : BaseView
    {
        public OnboardView()
        {
            InitializeComponent();
        }

        public override void HelpButton_Clicked(object sender, System.EventArgs e)
        {
            base.HelpButton_Clicked(this, e);
        }

        private void CreateButton_Clicked(object sender, System.EventArgs e)
        {
            _Logger.Information("Create button clicked.");
        }

        private void RecoverButton_Clicked(object sender, System.EventArgs e)
        {
            _Logger.Information("Recover button clicked.");
        }
    }
}
