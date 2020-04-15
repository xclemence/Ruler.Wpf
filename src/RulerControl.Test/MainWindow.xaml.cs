//  
// Copyright (c) Xavier CLEMENCE (xavier.clemence@gmail.com). All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information. 
// Ruler Wpf Version 3.0
// 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;

namespace RulerControl.Test
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string unit;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();

            ValueStepTransform = UpdateStepValues;
            DataContext = this;
        }

        public IEnumerable<int> MajorStep { get; } = new [] { 1, 2, 5 };

        public Func<double, double> ValueStepTransform { get; }

        public CultureInfo CultureUs { get; } = new CultureInfo("en-us");
        public CultureInfo CultureFr { get; } = new CultureInfo("fr-fr");
        public CultureInfo CultureCustom { get; } = CreateCustomCulture();

        public string Unit
        {
            get => unit;
            set
            {
                if (unit == value) return;

                unit = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Unit)));
            }
        }

        static private CultureInfo CreateCustomCulture()
        {
            var culture = CultureInfo.InvariantCulture.Clone() as CultureInfo;
            culture.NumberFormat.NumberGroupSeparator = "X";

            return culture;
        }

        private double UpdateStepValues(double stepValue)
        {
            if (stepValue < 1)
            {
                Unit = "* 1000";
                return stepValue * 1000;
            }

            Unit = "* 1";

            return stepValue;
        }
    }
}
