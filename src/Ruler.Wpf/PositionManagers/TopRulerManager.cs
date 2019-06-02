using System.Globalization;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Ruler.Wpf.PositionManagers
{
    public class TopRulerManager : HorizontalRulerManager
    {
        public TopRulerManager(RulerBase control) : base(control)
        {
        }

        public override Line CreateMajorLine(double offset)
        {
            var line = GetBaseLine();

            line.X1 = offset;
            line.Y1 = 0;
            
            line.X2 = offset;
            line.Y2 = GetHeight();
         
            return line;
        }

        public override Line CreateMinorLine(double offset)
        {
            var line = GetBaseLine();

            line.X1 = offset;
            line.Y1 = GetHeight() * 2 / 3;
            
            line.X2 = offset;
            line.Y2 = GetHeight();

            return line;
        }

        public override TextBlock CreateText(double value, double offset)
        {
            var text = GetTextBlock(value.ToString(Control.TextFormat, CultureInfo.CurrentCulture));

            text.SetValue(Canvas.LeftProperty, offset);

            return text;
        }
    }
}
