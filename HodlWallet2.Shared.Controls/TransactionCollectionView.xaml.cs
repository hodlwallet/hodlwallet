using System;
using System.Collections;
using System.Windows.Input;

using Xamarin.Forms;

namespace HodlWallet2.Shared.Controls
{
    public partial class TransactionCollectionView : ContentView
    {
        public TransactionCollectionView()
        {
            InitializeComponent();
            // FIXME this is where the code breaks, setting this binding
            Collection.SetBinding(ItemsView.ItemsSourceProperty, new Binding(nameof(TransactionsItemsSource), source: this));
        }

        public static readonly BindableProperty TransactionItemsSourceProperty =
            BindableProperty.Create(
                "TransactionsItemsSource",
                typeof(IEnumerable),
                typeof(TransactionCollectionView)
            );

        public IEnumerable TransactionsItemsSource
        {
            get => (IEnumerable) GetValue (TransactionItemsSourceProperty);
            set
            {
                SetValue(TransactionItemsSourceProperty, value);
                Collection.ItemsSource = value;
            }
        }

        public static readonly BindableProperty SelectedItemProperty =
            BindableProperty.Create(
                nameof(SelectedItem),
                typeof(object),
                typeof(TransactionCollectionView),
                null,
                BindingMode.OneWayToSource
            );

        public object SelectedItem
        {
            get => Collection.SelectedItem;
        }

        public static readonly BindableProperty SelectionChangedCommandProperty = 
            BindableProperty.Create(
                nameof(SelectionChangedCommand),
                typeof(ICommand),
                typeof(TransactionCollectionView),
                null
            );
            
        public ICommand SelectionChangedCommand
        {
            get => (ICommand) GetValue (SelectionChangedCommandProperty);
            set
            {
                SetValue(SelectionChangedCommandProperty, value);
                Collection.SelectionChangedCommand = value;
            }
        }
    }
}
