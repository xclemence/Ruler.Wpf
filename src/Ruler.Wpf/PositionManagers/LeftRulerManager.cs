using System;
using System.Globalization;
using System.Linq;
using System.Windows;
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

            line.X1 = height * 2 / 3;
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
