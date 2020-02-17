//  
// Copyright (c) Xavier CLEMENCE (xavier.clemence@gmail.com). All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information. 
// Ruler Wpf Version 2.0
// 

using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Ruler.Wpf.PositionManagers
{
    public enum RulerPosition
    {
        Top,
        Left
    }

    public abstract class RulerPositionManager
    {
        public RulerPositionManager(RulerBase control) => Control = control;

        public RulerBase Control { get; private set; }

        public abstract Line CreateMajorLine(double offset);
        public abstract Line CreateMinorLine(double offset);
        public abstract TextBlock CreateText(double text, double offset);
        public abstract double GetSize();
        public abstract double GetHeight();

        public abstract bool UpdateMakerPosition(Line marker, Point position);

        public abstract void UpdateFirstStepControl(Canvas control, double stepSize);
        public abstract void UpdateStepRepeaterControl(Rectangle control, VisualBrush brush, double stepSize);

        protected virtual Line GetBaseLine() 
        {
            return new Line
            {
                Stroke = Control.StepColor,
                StrokeThickness = 1,
                Stretch = Stretch.None,
            };
        }

        protected virtual TextBlock GetTextBlock(string text) => new TextBlock { Text = text };

        protected virtual CultureInfo GetTextCulture() => Control.TextCulture ?? CultureInfo.CurrentUICulture;

    }
}
