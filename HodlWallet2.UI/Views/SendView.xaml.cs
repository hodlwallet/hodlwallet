using System;

using Xamarin.Forms;
using Xamarin.Essentials;

using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using MvvmCross.ViewModels;
using MvvmCross.Base;
using MvvmCross.Binding.BindingContext;

using HodlWallet2.Core.Interactions;
using HodlWallet2.Core.ViewModels;
using HodlWallet2.Locale;
using HodlWallet2.Core.Models;
using HodlWallet2.UI.Renderers;
using NBitcoin;

namespace HodlWallet2.UI.Views
{
    [MvxTabbedPagePresentation(TabbedPosition.Tab)]
    public partial class SendView : MvxContentPage<SendViewModel>
    {
        ZXingScannerPage _ScanPage;

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

        IMvxInteraction<BarcodeScannerPrompt> _BarcodeScannerInteraction = new MvxInteraction<BarcodeScannerPrompt>();
        public IMvxInteraction<BarcodeScannerPrompt> BarcodeScannerInteraction
        {
            get => _BarcodeScannerInteraction;

            set
            {
                if (_BarcodeScannerInteraction != null)
                    _BarcodeScannerInteraction.Requested -= BarcodeScannerInteraction_Requested;

                _BarcodeScannerInteraction = value;
                _BarcodeScannerInteraction.Requested += BarcodeScannerInteraction_Requested;
            }
        }

        IMvxInteraction<SendTransactionQuestion> _SendTransactionQuestionInteraction = new MvxInteraction<SendTransactionQuestion>();
        public IMvxInteraction<SendTransactionQuestion> SendTransactionQuestionInteraction
        {
            get => _SendTransactionQuestionInteraction;

            set
            {
                if (_SendTransactionQuestionInteraction != null)
                    _SendTransactionQuestionInteraction.Requested -= SendTransactionQuestionInteraction_Requested;

                _SendTransactionQuestionInteraction = value;
                _SendTransactionQuestionInteraction.Requested += SendTransactionQuestionInteraction_Requested;
            }
        }

        public SendView()
        {
            IconImageSource = "send_tab.png";

            InitializeComponent();

            SetLabels();
        }


        public async void OnCloseTapped(object sender, EventArgs e)
        {
            //TODO: Replace Modal navigation for MvvmCross.
            await Navigation.PopModalAsync();
        }

        public async void OnFaqTapped(object sender, EventArgs e)
        {
            // TODO:
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            CreateInteractionBindings();
            ViewModel.ProcessAddressOnClipboardToPaste();
        }

        void CreateInteractionBindings()
        {
            var set = this.CreateBindingSet<SendView, SendViewModel>();

            set.Bind(this)
                .For(view => view.QuestionInteraction)
                .To(viewModel => viewModel.QuestionInteraction)
                .OneWay();

            set.Bind(this)
                .For(view => view.DisplayAlertInteraction)
                .To(viewModel => viewModel.DisplayAlertInteraction)
                .OneWay();

            set.Bind(this)
                .For(view => view.BarcodeScannerInteraction)
                .To(viewModel => viewModel.BarcodeScannerInteraction)
                .OneWay();

            set.Bind(this)
                .For(view => view.SendTransactionQuestionInteraction)
                .To(viewModel => viewModel.SendTransactionQuestionInteraction)
                .OneWay();

            set.Apply();
        }

        void SetLabels()
        {
            ToLabel.Text = LocaleResources.Send_to;
            ScanLabel.Text = LocaleResources.Send_scan;
            PasteLabel.Text = LocaleResources.Send_paste;
            AmountLabel.Text = LocaleResources.Send_amount;
            ISOLabel.Text = "USD($)"; // Localize
        }

        async void QuestionInteraction_Requested(object sender, MvxValueEventArgs<YesNoQuestion> e)
        {
            var yesNoQuestion = e.Value;

            string title;
            string content;
            string yes = "Yes";
            string no = "No";

            switch (yesNoQuestion.QuestionKey)
            {
                case "use-address-on-clipboard":
                    // FIXME we had to get the address again here...
                    string address = await Clipboard.GetTextAsync();

                    content = string.Format(
                        LocaleResources.Send_addressDetectedOnClipboardMessage,
                        address
                    );
                    title = LocaleResources.Send_addressDetectedOnClipboardTitle;
                    break;
                default:
                    throw new ArgumentException($"Invalid question sent, value: {yesNoQuestion.QuestionKey}");
            }

            bool answer = await DisplayAlert(
                title, content, yes, cancel: no
            );

            yesNoQuestion.AnswerCallback(answer);
        }

        async void OnDisplayAlertInteractionRequested(object sender, MvxValueEventArgs<DisplayAlertContent> e)
        {
            var displayAlertContent = e.Value;

            await DisplayAlert(
                displayAlertContent.Title, displayAlertContent.Message, displayAlertContent.Buttons[0]
            );
        }

        async void SendTransactionQuestionInteraction_Requested(object sender, MvxValueEventArgs<SendTransactionQuestion> e)
        {
            var sendTransactionQuestion = e.Value;

            var tx = sendTransactionQuestion.TransactionToSend;
            var fees = sendTransactionQuestion.Fees;

            var addressTo = ViewModel.AddressToSendTo;
            // TODO this could be in USD should be a problem...
            var amount = new Money(ViewModel.AmountToSend, MoneyUnit.BTC);

            var total = amount + fees;

            string title = "Send Transaction?";
            // Bug... total is wrong... and fees is wrong wtf...
            string content = $"Do you want to send {total} ({amount} + {fees} in fees) to {addressTo}.\nTx id will be {tx.GetHash().ToString()}";
            string yes = "Yes";
            string no = "No";

            bool answer = await DisplayAlert(
                title, content, yes, cancel: no
            );

            sendTransactionQuestion.AnswerCallback(answer);
        }

        async void BarcodeScannerInteraction_Requested(object sender, MvxValueEventArgs<BarcodeScannerPrompt> e)
        {
            var barcodeScannerPrompt = e.Value;

            // Android
            if (Device.RuntimePlatform == Device.Android)
            {
                MobileBarcodeScanner scanner = new MobileBarcodeScanner();
                ZXing.Result resultAndroid = await scanner.Scan();

                barcodeScannerPrompt.ResultCallback(resultAndroid);

                return;
            }

            // iOS
            // TODO These definitions should be moved to another place...
            //var customOverlay = new StackLayout
            //{
            //    HorizontalOptions = LayoutOptions.FillAndExpand,
            //    VerticalOptions = LayoutOptions.FillAndExpand
            //};
            //var torch = new Button
            //{
            //    Text = "Toggle Torch"
            //};
            //torch.Clicked += delegate {
            //    _ScanPage.ToggleTorch();
            //};
            //customOverlay.Children.Add(torch);

            _ScanPage = new ZXingScannerPage(
                //customOverlay: customOverlay,
                new MobileBarcodeScanningOptions
                {
                    AutoRotate = true,
                    UseNativeScanning = true,
                    DelayBetweenAnalyzingFrames = 5,
                    DelayBetweenContinuousScans = 5
                }
            );

            _ScanPage.OnScanResult += (ZXing.Result resultIOS) =>
            {
                _ScanPage.IsScanning = false;

                Device.BeginInvokeOnMainThread(() =>
                {
                    Navigation.PopAsync();

                    barcodeScannerPrompt.ResultCallback(resultIOS);
                });
            };

            await Navigation.PushAsync(_ScanPage);
        }
    }
}