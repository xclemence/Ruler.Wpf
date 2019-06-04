using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Ruler.Wpf.PositionManagers;

namespace Ruler.Wpf
{
    public class Ruler : RulerBase
    {
        private readonly Subject<int> updateSubject;
        private IDisposable updateSubcription;
        private RulerPositionManager rulerPostionControl;
        
        private Line marker;
        private Canvas firstMajorStepControl;
        private Canvas labelsControl;
        private Rectangle stepRepeaterControl;
        private VisualBrush stepRepeaterBrush;

        public Ruler()
        {

            updateSubject = new Subject<int>();

            UpdateRulerPosition(RulerPosition.Top);

            Loaded += OnRulerLoaded;
        }

        private Line Marker => marker ?? (marker = Template.FindName("marker", this) as Line);
        private Canvas FirstMajorStepControl => firstMajorStepControl ?? (firstMajorStepControl = Template.FindName("firstMajorStepControl", this) as Canvas);
        private Rectangle StepRepeaterControl => stepRepeaterControl ?? (stepRepeaterControl = Template.FindName("stepRepeaterControl", this) as Rectangle);
        private VisualBrush StepRepeaterBrush => stepRepeaterBrush ?? (stepRepeaterBrush = Template.FindName("stepRepeaterBrush", this) as VisualBrush);
        private Canvas LabelsControl => labelsControl ?? (labelsControl = Template.FindName("labelsControl", this) as Canvas);

        private void OnRulerLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnRulerLoaded;

            SizeChanged += OnRulerSizeChanged;
            Unloaded += OnRulerUnloaded;

            updateSubcription = updateSubject.Throttle(TimeSpan.FromMilliseconds(10))
                                             .Subscribe(_ => Application.Current.Dispatcher.BeginInvoke(new Action(() => DrawRuler())));
            RefreshRuler();
        }

        private void OnRulerUnloaded(object sender, RoutedEventArgs e)
        {
            SizeChanged -= OnRulerSizeChanged;
            Unloaded -= OnRulerUnloaded;

            if (MarkerControlReference != null)
                MarkerControlReference.MouseMove -= OnExternalMouseMouve;

            updateSubcription.Dispose();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            var mousePosition = e.GetPosition(this);

            UpdateMarkerPosition(mousePosition);
        }

        private void OnExternalMouseMouve(object sender, MouseEventArgs e)
        {
            var mousePosition = e.GetPosition(this);

            UpdateMarkerPosition(mousePosition);
        }

        protected override void UpdateRulerPosition(RulerPosition position)
        {
            switch (position)
            {
                case RulerPosition.Left:
                    rulerPostionControl = new LeftRulerManager(this);
                    break;
                default:
                    rulerPostionControl = new TopRulerManager(this);
                    break;
            }
        }

        private void UpdateMarkerPosition(Point point)
        {
            var positionUpdated = rulerPostionControl.UpdateMakerPosition(Marker, point);

            Marker.Visibility = positionUpdated ? Visibility.Visible : Visibility.Collapsed;
        }

        private void OnRulerSizeChanged(object sender, SizeChangedEventArgs e) => RefreshRuler();

        public override void RefreshRuler()
        {
            updateSubject.OnNext(1);
            //DrawhRuler();
        }
        
        private bool CanDrawRuler() => SlaveStepProperties != null || (MajorStepValues != null && !double.IsNaN(MaxValue) && MaxValue > 0);

        private void DrawRuler()
        {
            if (!CanDrawRuler()) return;

            var (pixelStep, valueStep) = GetStepProperties();

            var stepNumber = Math.Ceiling(rulerPostionControl.GetSize() / pixelStep);

            var subPixelSize = pixelStep / 10;

            FirstMajorStepControl.Children.Clear();
            LabelsControl.Children.Clear();

            GenerateSubSteps(subPixelSize, 0);

            FirstMajorStepControl.Children.Add(rulerPostionControl.CreateMajorLine(pixelStep));

            rulerPostionControl.UpdateFirstStepControl(FirstMajorStepControl, pixelStep);
            rulerPostionControl.UpdateStepRepeaterControl(StepRepeaterControl, StepRepeaterBrush, pixelStep);

            StepRepeaterBrush.Visual = FirstMajorStepControl;

            double offset;
            for (int i = 0; i < stepNumber; ++i)
            {
                offset = pixelStep * i;

                if (offset + pixelStep <= rulerPostionControl.GetSize())
                    LabelsControl.Children.Add(rulerPostionControl.CreateText(i * valueStep, offset));
            }
        }

        private (double pixelStep, double valueStep) GetStepProperties()
        {
            double pixelStep;
            double valueStep;

            if (SlaveStepProperties == null)
            {
                (pixelStep, valueStep) = GetMajorStep();
                StepProperties = new RulerStepProperties { PixelSize = pixelStep, Value = valueStep };
            }
            else
            {
                (pixelStep, valueStep) = SlaveStepProperties;
            }

            if (ValueStepTransform != null)
                valueStep = ValueStepTransform(valueStep);

            return (pixelStep, valueStep);
        }

        private (double pixelStep, double valueStep) GetMajorStep()
        {

            var normalizeMinSize = MinPixelSize * 10 / rulerPostionControl.GetSize();

            var minStepValue = normalizeMinSize * MaxValue;

            var minStepValueMagnitude = (int) Math.Floor(Math.Log10(minStepValue));

            var normalizeMinStepValue = minStepValue / Math.Pow(10, minStepValueMagnitude);

            var normalizeRealStepValue = MajorStepValues.Union(new int[] { 10 }).First(x => x > normalizeMinStepValue);

            var realStepValue = normalizeRealStepValue * Math.Pow(10, minStepValueMagnitude);

            var pixelStep = rulerPostionControl.GetSize() * realStepValue / MaxValue;

            return (pixelStep, valueStep: realStepValue);
        }

        private void GenerateSubSteps(double subPixelSize, double offset)
        {
            double subOffset;

            for (var y = 1; y < 10; ++y)
            {
                subOffset = offset + y * subPixelSize;

                if (subOffset > rulerPostionControl.GetSize())
                    continue;

                FirstMajorStepControl.Children.Add(rulerPostionControl.CreateMinorLine(subOffset));
            }
        }

        protected override void UpdateMarkerControlReference(UIElement oldControl, UIElement newControl) 
        {
            if (oldControl != null)
                oldControl.MouseMove -= OnExternalMouseMouve;

            if (newControl != null)
                newControl.MouseMove += OnExternalMouseMouve;
        }
    }
}

