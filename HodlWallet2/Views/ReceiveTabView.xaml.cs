using MvvmCross.Base;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using MvvmCross.ViewModels;

using HodlWallet2.Core.Interactions;
using HodlWallet2.Core.ViewModels;
using HodlWallet2.Locale;

namespace HodlWallet2.Views
{
    [MvxTabbedPagePresentation(TabbedPosition.Tab, WrapInNavigationPage = false, Title = "Receive")]
    public partial class ReceiveTabView : MvxContentPage<ReceiveTabViewModel>
    {
        IMvxInteraction<DisplayAlertContent> _CopiedToClipboardInteraction = new MvxInteraction<DisplayAlertContent>();
        public IMvxInteraction<DisplayAlertContent> CopiedToClipboardInteraction
        {
            get => _CopiedToClipboardInteraction;

            set
            {
                if (_CopiedToClipboardInteraction == null)
                    _CopiedToClipboardInteraction.Requested -= CopiedToClipboardInteraction_Requested;

                _CopiedToClipboardInteraction = value;
                _CopiedToClipboardInteraction.Requested += CopiedToClipboardInteraction_Requested;
            }
        }

        public ReceiveTabView()
        {
            IconImageSource = "receive_tab.png";

            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            CreateInteractionBindings();
        }

        void SetLabels()
        {
            //ReceiveTitle.Text = LocaleResources.Receive_title;
            Share.Text = LocaleResources.Receive_share;
            //RequestAmount.Text = LocaleResources.Receive_requestAmount;
        }

        void CreateInteractionBindings()
        {
            var set = this.CreateBindingSet<ReceiveTabView, ReceiveTabViewModel>();

            set.Bind(this)
                .For(view => view.CopiedToClipboardInteraction)
                .To(viewModel => viewModel.CopiedToClipboardInteraction)
                .OneWay();

            set.Apply();
        }

        async void CopiedToClipboardInteraction_Requested(object sender, MvxValueEventArgs<DisplayAlertContent> e)
        {
            var displayAlertContent = e.Value;

            await DisplayAlert(
               displayAlertContent.Title, displayAlertContent.Message, displayAlertContent.Buttons[0]
           );
        }
    }
}