using System;
using System.Windows.Input;

namespace HodlWallet2.Core.ViewModels
{
    public class AddAccountViewModel : BaseViewModel
    {
        public string BackupTitle => "Add an Account";

        public string HeaderText =>
            "[Update Image]";

        public string SubheaderText => "Select from a variety of account types.";

        public string ButtonText => "Create an Account";

        public AddAccountViewModel()
        {
        }
    }
}
