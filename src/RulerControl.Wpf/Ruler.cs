//  
// Copyright (c) Xavier CLEMENCE (xavier.clemence@gmail.com). All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information. 
// Ruler Wpf Version 3.0
// 

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using RulerControl.Wpf.PositionManagers;

namespace RulerControl.Wpf
{
    [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "updateSubject", Justification ="Managed by unload method")]
    [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "updateSubcription", Justification = "Managed by unload method")]
    public class Ruler : RulerBase, IDisposable
    {
        private const int SubStepNumber = 10;

        private readonly TimeSpan RefreshDelay = TimeSpan.FromMilliseconds(10);

        private bool disposedValue;

        private Subject<bool> updateSubject;

        private IDisposable updateSubcription;
        private RulerPositionManager rulerPostionControl;
        
        private Line marker;
        private Canvas firstMajorStepControl;
        private Canvas labelsControl;
        private Rectangle stepRepeaterControl;
        private VisualBrush stepRepeaterBrush;

        private bool isLoadedInternal;

        public Ruler()
        {
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
            
            updateSubject = new Subject<bool>();
            updateSubcription = updateSubject.Throttle(RefreshDelay)
                                             .Subscribe(_ => Application.Current.Dispatcher.BeginInvoke(new Action(() => DrawRuler())));

            isLoadedInternal = true;
            RefreshRuler();
        }

        private void OnRulerUnloaded(object sender, RoutedEventArgs e) => UnloadControl();

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e == null) return;

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
            if(position == RulerPosition.Left)
                rulerPostionControl =  new LeftRulerManager(this);
            else
                rulerPostionControl = new TopRulerManager(this);
        }

        private void UpdateMarkerPosition(Point point)
        {
            if (Marker == null || rulerPostionControl == null)
                return;

            var positionUpdated = rulerPostionControl.UpdateMakerPosition(Marker, point);

            Marker.Visibility = positionUpdated ? Visibility.Visible : Visibility.Collapsed;
        }

        private void OnRulerSizeChanged(object sender, SizeChangedEventArgs e) => RefreshRuler();

        public override void RefreshRuler() => updateSubject?.OnNext(true);
        
        private bool CanDrawRuler() => ValidateSize() && (CanDrawSlaveMode() || CanDrawMasterMode());

        private bool ValidateSize() => ActualWidth > 0 && ActualHeight > 0;

        private bool CanDrawSlaveMode() => SlaveStepProperties != null;
        private bool CanDrawMasterMode() => (MajorStepValues != null && !double.IsNaN(MaxValue) && MaxValue > 0);

        private void DrawRuler()
        {
            if (!CanDrawRuler()) return;

            var (pixelStep, valueStep) = GetStepProperties();

            var stepNumber = Math.Ceiling(rulerPostionControl.GetSize() / pixelStep);

            var subPixelSize = pixelStep / SubStepNumber;

            FirstMajorStepControl.Children.Clear();
            LabelsControl.Children.Clear();
            
            GenerateSubSteps(subPixelSize, 0);

            var majorLinePosition = DisplayZeroLine ? 0 : pixelStep;
            FirstMajorStepControl.Children.Add(rulerPostionControl.CreateMajorLine(majorLinePosition));

            rulerPostionControl.UpdateFirstStepControl(FirstMajorStepControl, pixelStep);
            rulerPostionControl.UpdateStepRepeaterControl(StepRepeaterControl, StepRepeaterBrush, pixelStep);

            StepRepeaterBrush.Visual = FirstMajorStepControl;

            double offset;
            double offsetToCheckDisplay;
            for (int i = 0; i < stepNumber; ++i)
            {
                offset = pixelStep * i;
                offsetToCheckDisplay = TextOverflow == RulerTextOverflow.Hidden ? offset + pixelStep - subPixelSize : offset;

                if (offsetToCheckDisplay <= rulerPostionControl.GetSize())
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
            // find thes minimal position of first major step between 0 and 1
            var normalizeMinSize = MinPixelSize * SubStepNumber / rulerPostionControl.GetSize();

            // calculate the real value of this step (min step value)
            var minStepValue = normalizeMinSize * MaxValue;

            // calculate magnetude of min step value (power of ten)
            var minStepValueMagnitude = (int) Math.Floor(Math.Log10(minStepValue));

            // normalise min step value between 0 and 10 (according to Major step value scale)
            var normalizeMinStepValue = minStepValue / Math.Pow(10, minStepValueMagnitude);

            // select best step according values defined by customer
            var normalizeRealStepValue = MajorStepValues.Union(new int[] { 10 }).First(x => x > normalizeMinStepValue);

            // apply magnitude to return inside  initial value scale
            var realStepValue = normalizeRealStepValue * Math.Pow(10, minStepValueMagnitude);

            // find size of real value (pixel)
            var pixelStep = rulerPostionControl.GetSize() * realStepValue / MaxValue;

            return (pixelStep, valueStep: realStepValue);
        }

        private void GenerateSubSteps(double subPixelSize, double offset)
        {
            double subOffset;

            for (var y = 1; y < SubStepNumber; ++y)
            {
                subOffset = offset + y * subPixelSize;

                if (subOffset > rulerPostionControl.GetSize())
                    continue;

                FirstMajorStepControl.Children.Add(rulerPostionControl.CreateMinorLine(subOffset));
            }
        }

        protected override void UpdateMarkerControlReference(UIElement oldElement, UIElement newElement) 
        {
            if (oldElement != null)
                oldElement.MouseMove -= OnExternalMouseMouve;

            if (newElement != null)
                newElement.MouseMove += OnExternalMouseMouve;
        }

        private void UnloadControl()
        {
            if (isLoadedInternal)
            {
                if (MarkerControlReference != null)
                    MarkerControlReference.MouseMove -= OnExternalMouseMouve;

                updateSubcription?.Dispose();
                updateSubject?.Dispose();
                updateSubject = null;

                isLoadedInternal = false;
            }
        }

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                    UnloadControl();

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}

