namespace Ruler.Wpf
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
