using System;

using MvvmCross.Base;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using MvvmCross.ViewModels;

using HodlWallet2.Core.ViewModels;
using HodlWallet2.Core.Interactions;
using HodlWallet2.Locale;
using HodlWallet2.Core.Utils;

namespace HodlWallet2.Views
{
    [MvxModalPresentation]
    public partial class MenuView : MvxContentPage<MenuViewModel>
    {
        IMvxInteraction<YesNoQuestion> _QuestionInteraction = new MvxInteraction<YesNoQuestion>();
        public IMvxInteraction<YesNoQuestion> QuestionInteraction
        {
            get => _QuestionInteraction;

            set
            {
                if (_QuestionInteraction != null)
                    _QuestionInteraction.Requested -= QuestionInteraction_Requested;

                _QuestionInteraction = value;
                _QuestionInteraction.Requested += QuestionInteraction_Requested;
            }
        }

        IMvxInteraction<DisplayAlertContent> _DisplayAlertInteraction;
        public IMvxInteraction<DisplayAlertContent> DisplayAlertInteraction
        {
            get => _DisplayAlertInteraction;
            set
            {
                if (_DisplayAlertInteraction != null)
                    _DisplayAlertInteraction.Requested -= OnDisplayAlertInteractionRequested;

                _DisplayAlertInteraction = value;
                _DisplayAlertInteraction.Requested += OnDisplayAlertInteractionRequested;
            }
        }

        public MenuView()
        {
            InitializeComponent();
            SetLabels();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            CreateInteractionsBindings();
        }

        void CreateInteractionsBindings()
        {
            var set = this.CreateBindingSet<MenuView, MenuViewModel>();

            set.Bind(this)
                .For(view => view.QuestionInteraction)
                .To(viewModel => viewModel.QuestionInteraction)
                .OneWay();

            set.Bind(this)
                .For(view => view.DisplayAlertInteraction)
                .To(viewModel => viewModel.DisplayAlertInteraction)
                .OneWay();

            set.Apply();
        }

        async void QuestionInteraction_Requested(object sender, MvxValueEventArgs<YesNoQuestion> e)
        {
            var yesNoQuestion = e.Value;

            // Dialog data
            string title;
            string content;
            string yes = "Yes";
            string no = "No";

            // Show dialog depending on the type
            switch (yesNoQuestion.QuestionKey)
            {
                case "wipe-wallet":
                    title = LocaleResources.Menu_wipeWallet;
                    content = "This action is irreversible, are you sure?";
                    break;
                case "resync-wallet":
                    title = LocaleResources.Menu_resyncWallet;
                    content = "This action is irreversible, are you sure?";
                    break;
                case "restore-wallet":
                    title = LocaleResources.Menu_restoreWallet;
                    content = "This action is irreversible, are you sure?";
                    break;
                default:
                    throw new ArgumentException($"Invalid question sent, value: {yesNoQuestion.QuestionKey}");
            }

            bool answer = await DisplayAlert(
                title, content, yes, cancel: no
            );

            yesNoQuestion.AnswerCallback(answer);
        }

        void SetLabels()
        {
            MenuTitle.Text = LocaleResources.Menu_title;

            ResyncWallet.Text = LocaleResources.Menu_resyncWallet;
            RestoreWallet.Text = LocaleResources.Menu_restoreWallet;
            WipeWallet.Text = LocaleResources.Menu_wipeWallet;
            BackupMnemonic.Text = LocaleResources.Backup_title;

#if DEBUG
            BuildDate.Text = $"Built on: {BuildInfo.BuildDateText}";
#endif

            // TODO Add these later once we got all the settings implemented.
            //Security.Text = LocaleResources.Menu_security;
            //Knowledge.Text = LocaleResources.Menu_knowledge;
            //Settings.Text = LocaleResources.Menu_settings;
            //LockWallet.Text = LocaleResources.Menu_lock;
        }

        async void OnDisplayAlertInteractionRequested(object sender, MvxValueEventArgs<DisplayAlertContent> e)
        {
            var displayAlertContent = e.Value;

            await DisplayAlert(
               displayAlertContent.Title, displayAlertContent.Message, displayAlertContent.Buttons[0]
           );
        }
    }
}
