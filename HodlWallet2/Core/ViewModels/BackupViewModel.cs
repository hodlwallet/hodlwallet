using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace HodlWallet2.Core.ViewModels
{
    public class BackupViewModel : BaseViewModel
    {
        //TODO: Add localization strings
        public string BackupTitle => "Backup Recovery Key";

        public string HeaderText =>
            "Your backup recovery key is the only way to restore your wallet if your phone is lost, stolen, broken or upgraded.";

        public string SubheaderText =>
            "We will show you a list of words to write down on a piece of paper and keep safe.";
        public string ButtonText => "Write Down Backup Recovery Key";

        public ICommand WriteDownWordsCommand { get; }
    }
}