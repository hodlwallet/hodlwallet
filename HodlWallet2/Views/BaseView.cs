using Serilog;
using Xamarin.Forms;

namespace HodlWallet2.Views
{
    public partial class BaseView : ContentPage
    {
        protected Wallet _Wallet;
        protected ILogger _Logger;

        public BaseView()
        {
            _Wallet = Wallet.Instance;
            _Logger = _Wallet.Logger;
        }

        /// <summary>
        /// This class opens up the modal that will display the help
        /// </summary>
        /// <param name="sender">View that we clicked the help button from</param>
        /// <param name="e">Event args</param>
        public virtual void HelpButton_Clicked(object sender, System.EventArgs e)
        {
            string viewName = sender.GetType().ToString();

            _Logger.Information("Help Button clicked currently viewing help for: {view}", viewName);
        }
    }
}
