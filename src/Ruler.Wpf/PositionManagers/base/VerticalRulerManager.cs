using System.Windows;
using System.Windows.Shapes;

namespace Ruler.Wpf.PositionManagers
{
    public abstract class VerticalRulerManager : RulerPositionManager
    {
        public VerticalRulerManager(RulerBase control) : base(control)
        {
        }

        public override double GetSize() => Control.ActualHeight;
        public override double GetHeight() => Control.ActualWidth;

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
