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
    public abstract class HorizontalRulerManager : RulerPositionManager
    {
        public HorizontalRulerManager(RulerBase control) : base(control)  { }

        public override double GetSize() => Control.ActualWidth;
        public override double GetHeight() => Control.ActualHeight;

        public override void UpdateFirstStepControl(Canvas control, double stepSize)
        {
            control.HorizontalAlignment = HorizontalAlignment.Left;
            control.Width = stepSize;
        }

        public override void UpdateStepRepeaterControl(Rectangle control, VisualBrush brush, double stepSize)
        {
            brush.Viewport = new Rect(0, 0, stepSize, GetHeight());
            control.Margin = new Thickness(stepSize, 0, 0, 0);
        }

        public override bool UpdateMakerPosition(Line marker, Point position)
        {
            if (position.X <= 0 || position.X >= GetSize())
                return false;

            marker.X1 = position.X;
            marker.Y1 = 0;
            
            marker.X2 = position.X;
            marker.Y2 = GetHeight();

            return true;
        }
    }
}