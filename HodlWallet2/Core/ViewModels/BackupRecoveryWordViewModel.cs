using System;
using System.Threading.Tasks;
using System.Windows.Input;
using HodlWallet2.Core.Services;
using HodlWallet2.UI.Views;
using Liviano.Exceptions;
using Xamarin.Forms;

namespace HodlWallet2.Core.ViewModels
{
    public class BackupRecoveryWordViewModel : BaseViewModel
    {
        string[] _Mnemonic;
        int _Position;
        string _Word;

        public string Word
        {
            get => (_Position == 0) ? _Mnemonic[PositionText] : _Word;
            private set => SetProperty(ref _Word, value);
        }

        public int PositionText
        {
            get => _Position;
            private set => SetProperty(ref _Position, value);
        }
        public string NextText => "Next Word";
        public string PreviousText => "Previous";
        public string HeaderText => "Write down each word in order and store it in a safe place.";
        public string TitleText => "Backup Recovery Key";
        public ICommand NextWordCommand { get; }
        public ICommand PreviousWordCommand { get; }

        public BackupRecoveryWordViewModel()
        {
            NextWordCommand = new Command(NextWord);
            PreviousWordCommand = new Command(PreviousWord);

            if (DesignMode.IsDesignModeEnabled) return;

            InitMnemonic();
        }

        private void PreviousWord()
        {
            if (PositionText > 0)
            {
                PositionText--;
                Word = _Mnemonic[PositionText];
            }
        }

        private void NextWord()
        {
            if (PositionText < 11)
            {
                PositionText++;
                Word = _Mnemonic[PositionText];

                return;
            }

            MessagingCenter.Send(this, "NavigateToBackupRecoveryConfirmView", _Mnemonic);
        }

        void InitMnemonic()
        {
            string rawMnemonic = GetMnemonic();

            _WalletService.Logger.Information($"Mnemonic is: {rawMnemonic}");

            _Mnemonic = rawMnemonic.Split(' ');
        }

        private string GetMnemonic()
        {
            if (!SecureStorageService.HasMnemonic())
                throw new WalletException("This wallet doesn't have a mnemonic, we cannot do anything without that one");

            return SecureStorageService.GetMnemonic();
        }
    }
}