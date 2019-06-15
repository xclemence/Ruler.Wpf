//  
// Copyright (c) Xavier CLEMENCE (xavier.clemence@gmail.com). All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information. 
// Ruler Wpf Version 2.0
// 

using System;
using System.ComponentModel;
using System.Windows;

namespace Ruler.Test
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

        public int[] MajorStep { get; } = { 1, 2, 5 };

        public Func<double, double> ValueStepTransform { get; }

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
