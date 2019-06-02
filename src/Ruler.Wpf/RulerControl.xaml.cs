//using System;
//using System.Linq;
//using System.Reactive.Linq;
//using System.Reactive.Subjects;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Input;
//using System.Windows.Media;

//namespace POC_Ruler
//{

//    /// <summary>
//    /// Interaction logic for Ruler.xaml
//    /// </summary>
//    public partial class RulerControl : UserControl
//    {
//        #region DependencyProperty
//        public static readonly DependencyProperty PositionProperty =
//             DependencyProperty.Register(nameof(Position), typeof(RulerPosition),
//             typeof(RulerControl), new FrameworkPropertyMetadata(RulerPosition.Top, OnRulerPositionChanged));

//        public RulerPosition Position
//        {
//            get { return (RulerPosition)GetValue(PositionProperty); }
//            set { SetValue(PositionProperty, value); }
//        }

//        private static void OnRulerPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
//        {
//            if (!(d is RulerControl control) || !(e.NewValue is RulerPosition position))
//                return;
//            control.UpdateRulerPosition(position);
//        }

//        /// <summary>
//        /// /////////////////////////////////////////////////////////////////////////////////:
//        /// </summary>
//        public static readonly DependencyProperty MarkerControlReferenceProperty =
//             DependencyProperty.Register(nameof(MarkerControlReference), typeof(UIElement),
//             typeof(RulerControl), new FrameworkPropertyMetadata(null, OnMarkerControlReferenceChanged));

//        public UIElement MarkerControlReference
//        {
//            get { return (UIElement)GetValue(MarkerControlReferenceProperty); }
//            set { SetValue(MarkerControlReferenceProperty, value); }
//        }

//        private static void OnMarkerControlReferenceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
//        {
//            if (!(d is RulerControl control))
//                return;

//            if (e.OldValue is UIElement oldControl)
//                oldControl.MouseMove -= control.OnExternalMouseMouve;

//            if (e.NewValue is UIElement newControl)
//                newControl.MouseMove += control.OnExternalMouseMouve;
//        }

//        /// <summary>
//        /// /////////////////////////////////////////////////////////////////////////////////:
//        /// </summary>
//        public static readonly DependencyProperty MaxValueProperty =
//             DependencyProperty.Register(nameof(MaxValue), typeof(double),
//             typeof(RulerControl), new FrameworkPropertyMetadata(100.0, OnChangedRulerUpdate));

//        public double MaxValue
//        {
//            get { return (double)GetValue(MaxValueProperty); }
//            set { SetValue(MaxValueProperty, value); }
//        }

//        private static void OnChangedRulerUpdate(DependencyObject d, DependencyPropertyChangedEventArgs e)
//        {
//            if (!(d is RulerControl control))
//                return;

//            control.RefreshRuler();
//        }

//        /// <summary>
//        /// /////////////////////////////////////////////////////////////////////////////////:
//        /// </summary>
//        public static readonly DependencyProperty MinPixelSizeProperty =
//             DependencyProperty.Register(nameof(MinPixelSize), typeof(int),
//             typeof(RulerControl), new FrameworkPropertyMetadata(4, OnChangedRulerUpdate));

//        public int MinPixelSize
//        {
//            get { return (int)GetValue(MinPixelSizeProperty); }
//            set { SetValue(MinPixelSizeProperty, value); }
//        }

//        /// <summary>
//        /// /////////////////////////////////////////////////////////////////////////////////:
//        /// </summary>
//        public static readonly DependencyProperty StepPropertiesProperty =
//             DependencyProperty.Register(nameof(StepProperties), typeof(RulerStepProperties),
//             typeof(RulerControl), new FrameworkPropertyMetadata(null));

//        public RulerStepProperties StepProperties
//        {
//            get { return (RulerStepProperties)GetValue(StepPropertiesProperty); }
//            set { SetValue(StepPropertiesProperty, value); }
//        }

//        /// <summary>
//        /// /////////////////////////////////////////////////////////////////////////////////:
//        /// </summary>
//        public static readonly DependencyProperty SlaveStepPropertiesProperty =
//             DependencyProperty.Register(nameof(SlaveStepProperties), typeof(RulerStepProperties),
//             typeof(RulerControl), new FrameworkPropertyMetadata(null, OnChangedRulerUpdate));

//        public RulerStepProperties SlaveStepProperties
//        {
//            get { return (RulerStepProperties)GetValue(SlaveStepPropertiesProperty); }
//            set { SetValue(SlaveStepPropertiesProperty, value); }
//        }


//        /// <summary>
//        /// /////////////////////////////////////////////////////////////////////////////////:
//        /// </summary>
//        public static readonly DependencyProperty StepColorProperty =
//             DependencyProperty.Register(nameof(StepColor), typeof(Brush),
//             typeof(RulerControl), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Black), OnChangedRulerUpdate));

//        public Brush StepColor
//        {
//            get { return (Brush)GetValue(StepColorProperty); }
//            set { SetValue(StepColorProperty, value); }
//        }

//        public static readonly DependencyProperty TextMarginProperty =
//             DependencyProperty.Register(nameof(TextMargin), typeof(Thickness),
//             typeof(RulerControl), new FrameworkPropertyMetadata(new Thickness(2), OnChangedRulerUpdate));

//        public Thickness TextMargin
//        {
//            get { return (Thickness)GetValue(TextMarginProperty); }
//            set { SetValue(TextMarginProperty, value); }
//        }


//        public static readonly DependencyProperty TextFormatProperty =
//             DependencyProperty.Register(nameof(TextFormat), typeof(string),
//             typeof(RulerControl), new FrameworkPropertyMetadata("F2", OnChangedRulerUpdate));

//        public string TextFormat
//        {
//            get { return (string)GetValue(TextFormatProperty); }
//            set { SetValue(TextFormatProperty, value); }
//        }

//        public static readonly DependencyProperty TextLineHeightProperty =
//             DependencyProperty.Register(nameof(TextLineHeight), typeof(double),
//             typeof(RulerControl), new FrameworkPropertyMetadata(12.0, OnChangedRulerUpdate));

//        public double TextLineHeight
//        {
//            get { return (double)GetValue(TextLineHeightProperty); }
//            set { SetValue(TextLineHeightProperty, value); }
//        }


//        /// //////////////////////////////////////////////////////////////////////////////////
//        #endregion DependencyProperty

//        private Subject<int> updateSubject;
//        private IDisposable updateSubcription;
//        private RulerPositionController rulerPostionControl;

//        public RulerControl()
//        {
//            InitializeComponent();

//            SizeChanged += OnRulerSizeChanged;

//            updateSubject = new Subject<int>();

//            UpdateRulerPosition(RulerPosition.Top);

//            updateSubcription = updateSubject.Throttle(TimeSpan.FromMilliseconds(5))
//                                            .Subscribe(events =>
//                                            {
//                                                Application.Current.Dispatcher.BeginInvoke(new Action(() => DrawRuler()));
//                                            });


//            //Setter by style
//            ClipToBounds = true;

//        }

//        protected override void OnMouseMove(MouseEventArgs e)
//        {
//            base.OnMouseMove(e);

//            var mousePosition = e.GetPosition(this);

//            UpdateMarkerPosition(mousePosition);
//        }

//        private void OnExternalMouseMouve(object sender, MouseEventArgs e)
//        {
//            var mousePosition = e.GetPosition(this);

//            UpdateMarkerPosition(mousePosition);
//        }

//        private void UpdateRulerPosition(RulerPosition position)
//        {
//            //switch (position)
//            //{
//            //    case RulerPosition.Left:
//            //        rulerPostionControl = new LeftRulerControl(this);
//            //        break;
//            //    default:
//            //        rulerPostionControl = new TopRulerControl(this);
//            //        break;
//            //}
//        }

//        private void UpdateMarkerPosition(Point point)
//        {
//            var positionUpdated = rulerPostionControl.UpdateMakerPosition(marker, point);

//            marker.Visibility = positionUpdated ? Visibility.Visible : Visibility.Collapsed;
//        }

//        private void OnRulerSizeChanged(object sender, SizeChangedEventArgs e)
//        {
//            RefreshRuler();
//        }

//        public int[] Steps { get; set; } = { 1, 2, 5 };

//        private (double pixelStep, double valueStep ) GetMajorStep()
//        {

//            var normaliserMinSize = MinPixelSize * 10 /  rulerPostionControl.GetSize();
            
//            var minStepValue = normaliserMinSize * MaxValue;
            
//            var mixValueMagnitude = Math.Floor(Math.Log10(minStepValue));
            
//            var normalizeminStepValue = minStepValue / Math.Pow(10, mixValueMagnitude);
            
//            var normalizeRealStep = Steps.Union(new int[] { 10 }).First(x => x > normalizeminStepValue);
            
//            var realMinStepValue = normalizeRealStep * Math.Pow(10, mixValueMagnitude);
            
//            var pixelStep = rulerPostionControl.GetSize() * realMinStepValue / MaxValue;

//            return (pixelStep, valueStep: realMinStepValue);
//        }
        
//        public void RefreshRuler()
//        {
//            updateSubject.OnNext(1);
//            //DrawhRuler();
//        }

//        private void DrawRuler()
//        {
//            double pixelStep;
//            double valueStep;
            
//            if (SlaveStepProperties == null)
//            {
//                (pixelStep, valueStep) = GetMajorStep();
//                StepProperties = new RulerStepProperties { PixelSize = pixelStep, Value = valueStep };
//            }
//            else 
//            {
//                (pixelStep, valueStep) = SlaveStepProperties;
//            }

//            var stepNumber = Math.Ceiling(rulerPostionControl.GetSize() / pixelStep);

//            var subPixelSize = pixelStep / 10;

//            rulerTicks.Children.Clear();

//            double offset;
//            for (int i = 0; i < stepNumber; ++i)
//            {
//                offset = pixelStep * i;

//                if (offset > rulerPostionControl.GetSize())
//                    continue;

//                rulerTicks.Children.Add(rulerPostionControl.CreateMajorLine(offset));

//                if (offset + pixelStep <= rulerPostionControl.GetSize())
//                    rulerTicks.Children.Add(rulerPostionControl.CreateText(i * valueStep, offset));
                
//                GenerateSubSteps(subPixelSize, offset);
//            }
//        }

//        private void GenerateSubSteps(double subPixelSize, double offset)
//        {
//            double subOffset;

//            for (var y = 1; y < 10; ++y)
//            {
//                subOffset = offset + y * subPixelSize;

//                if (subOffset > rulerPostionControl.GetSize())
//                    continue;

//                rulerTicks.Children.Add(rulerPostionControl.CreateMinorLine(subOffset));
//            }
//        }
//    }

   
//}
