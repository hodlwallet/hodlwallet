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
        }

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
