﻿using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HodlWallet2.Shared.Controls
{
    public partial class SyncProgressBar : ContentView
    {
        public SyncProgressBar()
        {
            InitializeComponent();
            IsVisible = false;
            SetBinding(IsVisibleProperty, new Binding(nameof(SyncVisible), source: this));
            SBTitle.SetBinding(Label.TextProperty, new Binding(nameof(SyncTitle), source: this));
            SBProgress.SetBinding(ProgressBar.ProgressProperty, new Binding(nameof(SyncProgress), source: this));
            SBDate.SetBinding(Label.TextProperty, new Binding(nameof(SyncDate), source: this));
        }

        public static readonly BindableProperty SyncVisibleProperty =
            BindableProperty.Create(
                "SyncVisible",
                typeof(bool),
                typeof(SyncProgressBar)
            );

        public bool SyncVisible
        {
            get => (bool) GetValue (SyncVisibleProperty);
            set
            {
                SetValue(SyncVisibleProperty, value);
                IsVisible = value;
            }
        }

        public static readonly BindableProperty SyncTitleProperty = 
            BindableProperty.Create(
                "SyncTitle",
                typeof(string),
                typeof(SyncProgressBar)
            );

        public string SyncTitle
        {
            get => (string) GetValue (SyncTitleProperty);
            set
            {
                SetValue(SyncTitleProperty, value);
                SBTitle.Text = value;
            }
        }

        public static readonly BindableProperty SyncProgressProperty =
            BindableProperty.Create(
                "SyncProgress",
                typeof(double),
                typeof(SyncProgressBar)
            );

        public double SyncProgress
        {
            get => (double)GetValue(SyncProgressProperty);
            set
            {
                SetValue(SyncTitleProperty, value);
                SBProgress.Progress = value;
            }
        }

        public static readonly BindableProperty SyncDateProperty =
            BindableProperty.Create(
                "SyncDate",
                typeof(string),
                typeof(SyncProgressBar)
            );

        public string SyncDate
        {
            get => (string)GetValue(SyncDateProperty);
            set
            {
                SetValue(SyncDateProperty, value);
                SBDate.Text = value;
            }
        }
    }
}