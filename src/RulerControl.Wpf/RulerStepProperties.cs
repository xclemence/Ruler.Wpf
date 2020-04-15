//  
// Copyright (c) Xavier CLEMENCE (xavier.clemence@gmail.com). All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information. 
// Ruler Wpf Version 3.0
// 

namespace RulerControl.Wpf
{
    public class RulerStepProperties
    {
        public double PixelSize { get; set; }
        public double Value { get; set; }

        public void Deconstruct(out double pixelSize, out double value)
        {
            pixelSize = PixelSize;
            value = Value;
        }
    }
}
