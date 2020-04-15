//  
// Copyright (c) Xavier CLEMENCE (xavier.clemence@gmail.com). All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information. 
// Ruler Wpf Version 3.0
// 

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using RulerControl.Wpf.PositionManagers;

namespace RulerControl.Wpf
{
    public abstract class RulerBase : Control
    {
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(nameof(MaxValue), typeof(double), typeof(RulerBase), new FrameworkPropertyMetadata(double.NaN, OnChangedRulerUpdate));
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(nameof(Position), typeof(RulerPosition), typeof(RulerBase), new FrameworkPropertyMetadata(RulerPosition.Top, OnRulerPositionChanged));
        public static readonly DependencyProperty MajorStepValuesProperty = DependencyProperty.Register(nameof(MajorStepValues), typeof(IEnumerable<int>), typeof(RulerBase), new FrameworkPropertyMetadata(new int[] { 1, 2, 5 }, OnChangedRulerUpdate));
        public static readonly DependencyProperty MinPixelSizeProperty = DependencyProperty.Register(nameof(MinPixelSize), typeof(int), typeof(RulerBase), new FrameworkPropertyMetadata(4, OnChangedRulerUpdate));
        public static readonly DependencyProperty ValueStepTransformProperty = DependencyProperty.Register(nameof(ValueStepTransform), typeof(Func<double, double>), typeof(RulerBase), new FrameworkPropertyMetadata(null, OnChangedRulerUpdate));
        public static readonly DependencyProperty MarkerControlReferenceProperty = DependencyProperty.Register(nameof(MarkerControlReference), typeof(UIElement), typeof(RulerBase), new FrameworkPropertyMetadata(null, OnMarkerControlReferenceChanged));

        public static readonly DependencyProperty StepColorProperty = DependencyProperty.Register(nameof(StepColor), typeof(Brush), typeof(RulerBase), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Black), OnChangedRulerUpdate));
        public static readonly DependencyProperty MinorStepRatioProperty = DependencyProperty.Register(nameof(MinorStepRatio), typeof(double), typeof(RulerBase), new FrameworkPropertyMetadata(0.33, OnChangedRulerUpdate));
        public static readonly DependencyProperty DisplayZeroLineProperty = DependencyProperty.Register(nameof(DisplayZeroLine), typeof(bool), typeof(RulerBase), new FrameworkPropertyMetadata(false, OnChangedRulerUpdate));

        public static readonly DependencyProperty StepPropertiesProperty = DependencyProperty.Register(nameof(StepProperties), typeof(RulerStepProperties), typeof(RulerBase), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty SlaveStepPropertiesProperty = DependencyProperty.Register(nameof(SlaveStepProperties), typeof(RulerStepProperties), typeof(RulerBase), new FrameworkPropertyMetadata(null, OnChangedRulerUpdate));

        public static readonly DependencyProperty TextFormatProperty = DependencyProperty.Register(nameof(TextFormat), typeof(string), typeof(RulerBase), new FrameworkPropertyMetadata("N0", OnChangedRulerUpdate));
        public static readonly DependencyProperty TextCultureProperty = DependencyProperty.Register(nameof(TextCulture), typeof(CultureInfo), typeof(RulerBase), new FrameworkPropertyMetadata(null, OnChangedRulerUpdate));
        public static readonly DependencyProperty TextOverflowProperty = DependencyProperty.Register(nameof(TextOverflow), typeof(RulerTextOverflow), typeof(RulerBase), new FrameworkPropertyMetadata(RulerTextOverflow.Visible, OnChangedRulerUpdate));

        public double MaxValue
        {
            get => (double)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }

        public RulerPosition Position
        {
            get => (RulerPosition)GetValue(PositionProperty); 
            set => SetValue(PositionProperty, value);
        }

        public IEnumerable<int> MajorStepValues
        {
            get => (IEnumerable<int>)GetValue(MajorStepValuesProperty);
            set => SetValue(MajorStepValuesProperty, value);
        }

        public int MinPixelSize
        {
            get => (int)GetValue(MinPixelSizeProperty);
            set => SetValue(MinPixelSizeProperty, value);
        }

        public Func<double, double> ValueStepTransform
        {
            get => (Func<double, double>)GetValue(ValueStepTransformProperty);
            set => SetValue(ValueStepTransformProperty, value);
        }

        public UIElement MarkerControlReference
        {
            get => (UIElement)GetValue(MarkerControlReferenceProperty);
            set => SetValue(MarkerControlReferenceProperty, value);
        }

        public Brush StepColor
        {
            get => (Brush)GetValue(StepColorProperty);
            set => SetValue(StepColorProperty, value);
        }

        public double MinorStepRatio
        {
            get => (double)GetValue(MinorStepRatioProperty);
            set => SetValue(MinorStepRatioProperty, value);
        }

        public bool DisplayZeroLine
        {
            get => (bool)GetValue(DisplayZeroLineProperty);
            set => SetValue(DisplayZeroLineProperty, value);
        }

        public RulerStepProperties StepProperties
        {
            get => (RulerStepProperties)GetValue(StepPropertiesProperty);
            set => SetValue(StepPropertiesProperty, value);
        }

        public RulerStepProperties SlaveStepProperties
        {
            get => (RulerStepProperties)GetValue(SlaveStepPropertiesProperty);
            set => SetValue(SlaveStepPropertiesProperty, value);
        }

        public string TextFormat
        {
            get => (string)GetValue(TextFormatProperty);
            set => SetValue(TextFormatProperty, value);
        }

        public CultureInfo TextCulture
        {
            get => (CultureInfo)GetValue(TextCultureProperty);
            set => SetValue(TextCultureProperty, value);
        }

        public RulerTextOverflow TextOverflow
        {
            get => (RulerTextOverflow)GetValue(TextOverflowProperty);
            set => SetValue(TextOverflowProperty, value);
        }

        private static void OnRulerPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is RulerBase control) || !(e.NewValue is RulerPosition position)) return;

            control.UpdateRulerPosition(position);
        }

        private static void OnMarkerControlReferenceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is RulerBase control)) return;

            control.UpdateMarkerControlReference(e.OldValue as UIElement, e.NewValue as UIElement);
        }

        private static void OnChangedRulerUpdate(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is RulerBase control)) return;

            control.RefreshRuler();
        }

        public abstract void RefreshRuler();
        protected abstract void UpdateMarkerControlReference(UIElement oldElement, UIElement newElement);
        protected abstract void UpdateRulerPosition(RulerPosition position);
    }
}
