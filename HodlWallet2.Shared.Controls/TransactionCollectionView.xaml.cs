using System;
using System.Collections;

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

        public static readonly BindableProperty SelectedItemProperty =
            BindableProperty.Create(
                "SelectedItem",
                typeof(object),
                typeof(TransactionCollectionView),
                null,
                BindingMode.OneWayToSource
            );

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
        }
    }
}
