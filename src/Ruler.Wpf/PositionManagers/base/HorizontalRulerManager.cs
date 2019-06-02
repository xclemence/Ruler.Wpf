using System.Windows;
using System.Windows.Shapes;

namespace Ruler.Wpf.PositionManagers
{
    public abstract class HorizontalRulerManager : RulerPositionManager
    {
        public HorizontalRulerManager(RulerBase control) : base(control)
        {
        }

        public override double GetSize() => Control.ActualWidth;
        public override double GetHeight() => Control.ActualHeight;

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