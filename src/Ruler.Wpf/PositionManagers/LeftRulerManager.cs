﻿//  
// Copyright (c) Xavier CLEMENCE (xavier.clemence@gmail.com). All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information. 
// Ruler Wpf Version 2.0
// 

using System;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Ruler.Wpf.PositionManagers
{
    public class LeftRulerManager : VerticalRulerManager
    {
        public LeftRulerManager(RulerBase control) : base(control)
        {
        }

        public override Line CreateMajorLine(double offset)
        {
            var line = GetBaseLine();

            line.X1 = 0;
            line.Y1 = offset;
            
            line.X2 = GetHeight();
            line.Y2 = offset;

            return line;
        }

        public override Line CreateMinorLine(double offset)
        {
            var height = GetHeight();
            var line = GetBaseLine();

            line.X1 = height * (1 - Control.MinorStepRatio);
            line.Y1 = offset;
         
            line.X2 = height;
            line.Y2 = offset;

            return line;
        }

        public override TextBlock CreateText(double value, double offset)
        {
            var text = value.ToString(Control.TextFormat, CultureInfo.CurrentCulture)
                            .Select(x => x.ToString())
                            .Where(x => !string.IsNullOrWhiteSpace(x))
                            .Aggregate((x, y) => $"{x}{Environment.NewLine}{y}");


            var textBlock = GetTextBlock(text);

            textBlock.SetValue(Canvas.TopProperty, offset);

            return textBlock;
        }
    }

}
