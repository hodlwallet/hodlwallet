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
            Collection.SetBinding(ItemsView.ItemsSourceProperty, new Binding(nameof(ItemsSource), source: this));
        }

        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(
                "ItemsSource",
                typeof(IEnumerable),
                typeof(TransactionCollectionView)
            );

        public IEnumerable ItemsSource
        {
            get => (IEnumerable) GetValue (ItemsSourceProperty);
            set
            {
                SetValue(ItemsSourceProperty, value);
                Collection.ItemsSource = value;
            }
        }

        public static readonly BindableProperty SelectionChangedCommandProperty = 
            BindableProperty.Create(
                "SelectionChangedCommand",
                typeof(ICommand),
                typeof(TransactionCollectionView),
                null,
                BindingMode.OneWay
            );
            
        public ICommand SelectionChangedCommand
        {
            get => (ICommand) GetValue (CollectionView.SelectionChangedCommandProperty);
            set
            {
                SetValue(CollectionView.SelectionChangedCommandProperty, value);
                Collection.SelectionChangedCommand = value;
            }
        }
    }
}
