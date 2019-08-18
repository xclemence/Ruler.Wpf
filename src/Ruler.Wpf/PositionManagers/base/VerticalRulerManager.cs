//  
// Copyright (c) Xavier CLEMENCE (xavier.clemence@gmail.com). All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information. 
// Ruler Wpf Version 2.0
// 

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Ruler.Wpf.PositionManagers
{
    public abstract class VerticalRulerManager : RulerPositionManager
    {
        public VerticalRulerManager(RulerBase control) : base(control) { }

        public override double GetSize() => Control.ActualHeight;
        public override double GetHeight() => Control.ActualWidth;

        public override void UpdateFirstStepControl(Canvas control, double stepSize)
        {
            control.VerticalAlignment = VerticalAlignment.Top;
            control.Height = stepSize;
        }

        public override void UpdateStepRepeaterControl(Rectangle control, VisualBrush brush, double stepSize)
        {
            brush.Viewport = new Rect(0, 0, GetHeight(), stepSize);
            control.Margin = new Thickness(0, stepSize, 0, 0);
        }

        public override bool UpdateMakerPosition(Line marker, Point position)
        {
            if (position.Y <= 0 || position.Y >= GetSize())
                return false;

            marker.X1 = 0;
            marker.Y1 = position.Y;
            
            marker.X2 = GetHeight();
            marker.Y2 = position.Y;

            return true;
        }
    }
}
