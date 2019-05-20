using System;
using System.Collections.Generic;
using HodlWallet2.Core.Services;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace HodlWallet2.Core.ViewModels
{
    public class BackupRecoveryWordViewModel : BaseViewModel<string[]>
    {
        private string[] _mnemonic;
        private int _position;
        private string _word;

        public string Word
        {
            get => (_position == 0) ? _mnemonic[PositionText] : _word;
            private set => SetProperty(ref _word, value);
        }
        
        //TODO: Localize strings
        public int PositionText
        {
            get => _position;
            private set => SetProperty(ref _position, value);
        }
        public string NextText => "Next word";
        public string PreviousText => "Previous";
        public string HeaderText => "Write down each word in order and store it in a safe place.";
        public string TitleText => "Backup Recovery Key";
        public IMvxCommand NextWordCommand { get; private set; }
        public IMvxCommand PreviousWordCommand { get; private set; }
        
        public BackupRecoveryWordViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService) : base(logProvider, navigationService)
        {
            NextWordCommand = new MvxCommand(NextWord_Action);
            PreviousWordCommand = new MvxCommand(PreviousWord_Action);
        }

        private void PreviousWord_Action()
        {
            if (PositionText > 0)
            {
                PositionText--;
                Word = _mnemonic[PositionText];
                //TODO: Log activity here.
            }
        }

        private void NextWord_Action()
        {
            
            if (PositionText < 11)
            {
                PositionText++;
                Word = _mnemonic[PositionText];
                //TODO: Log activity here.
            }
            else
            {
                //TODO: Log activity here.
                NavigationService.Navigate<BackupRecoveryConfirmViewModel, string[]>(_mnemonic);
            }
        }

        public override void Prepare(string[] parameter)
        {
            //TODO: Generate mnemonic in WalletService and pass through view models.
            _mnemonic = parameter;
            
        }

        public override void Prepare()
        {
            //TODO: This is temporal, this view model should always receive a mnemonic to show.
            _mnemonic = "erase fog enforce rice coil start few hold grocery lock youth service".Split(' ');
        }
    }
}