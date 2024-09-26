using ImageProcessing.Enum;
using ImageProcessing.Helper;
using ImageProcessing.Models;
using ImageProcessing.Services;
using ImageProcessing.Services.Buffers;
using ImageProcessing.Services.IO;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Object = ImageProcessing.Enum.Object;
using Point = System.Windows.Point;
using Rectangle = System.Drawing.Rectangle;
using RectangleElement = System.Windows.Shapes.Rectangle;
namespace ImageProcessing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region CanvasVariables
        private MouseState _mouseState = MouseState.LeftUp;
        private Object _obj = Object.NotFound;
        private ObservableCollection<Rectangle> _rectangles;
        private Rectangle _rectangle;
        private RectangleElement _highlightedRectangle;
        private Point _startPoint;
        private int _indexOfCanvasElement;
        private double _distanceBetweenX;
        private double _distanceBetweenY;
        #endregion
        #region SliderVariables
        public int DragStartedAt;
        public int DragEndedAt;
        #endregion
        #region Dependencies
        Decoder _decoder;
        #endregion
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            _rectangles = new ObservableCollection<Rectangle>();
            _startPoint = new Point();
            _decoder = Decoder.GetInstance();
        }
        #region SliderEvents
        private async void Slider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            DragEndedAt = State.SliderValue;
            int distance = Math.Abs(DragEndedAt - DragStartedAt);
            if (distance >= 100)
            {
                State.ProcessedFrameIndex = State.SliderValue;
                _decoder.Reset();
                SplashScreenHelper.Display();
            }
        }
        private void Slider_DragStarted(object sender, DragStartedEventArgs e)
        {
            DragStartedAt = State.SliderValue;
        }

        private void Slider_ValueChanged(object sender,RoutedPropertyChangedEventArgs<double> e)
        {
        }
        #endregion
        #region CanvasElements
        private bool IsClickInsideTheObjects(Point p)
        {
            for (int i = 0; i < _rectangles.Count; i++)
            {
                if (_rectangles[i].Contains(new System.Drawing.Point((int)p.X, (int)p.Y)))
                {
                    _indexOfCanvasElement = i + 1;

                    _highlightedRectangle = (RectangleElement)canvas.Children[_indexOfCanvasElement];
                    _rectangle = _rectangles[i];

                    _distanceBetweenX = _startPoint.X - _rectangle.X;
                    _distanceBetweenY = _startPoint.Y - _rectangle.Y;

                    return true;
                }
            }
            return false;
        }
        private new void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _mouseState = MouseState.LeftDown;
            _startPoint = e.GetPosition(this);
            if (IsClickInsideTheObjects(_startPoint))
            {
                _obj = Object.Found;
                return;
            }
            _obj = Object.NotFound;
            AddNewElementToCanvas();
        }
        private void AddNewElementToCanvas()
        {
            _rectangle = new Rectangle();
            _highlightedRectangle = new RectangleElement();

            _highlightedRectangle.Stroke = new SolidColorBrush() { Color = Colors.Black };

            canvas.Children.Add(_highlightedRectangle);
            _rectangles.Add(_rectangle);
        }
        private new void MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                _mouseState = MouseState.LeftUp;
                if (_rectangle.Width == 0 || _rectangle.Height == 0)
                {
                    canvas.Children.RemoveAt(canvas.Children.Count - 1);
                    _rectangles.RemoveAt(_rectangles.Count - 1);
                }
            }
            catch { }
        }
        private new void MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (_mouseState == MouseState.LeftDown)
                {
                    Point Cursor = e.GetPosition(this);
                    Point LeftCorner = new Point();

                    if (_obj == Object.Found)
                    {
                        if (Cursor.X - _distanceBetweenX >= 100 &&
                            Cursor.Y - _distanceBetweenY >= 0 &&
                            Cursor.X - _distanceBetweenX + _rectangle.Width <= 1100 &&
                            Cursor.Y - _distanceBetweenY + _rectangle.Height <= 500)
                        {
                            _rectangle.X = (int)(Cursor.X - _distanceBetweenX);
                            _rectangle.Y = (int)(Cursor.Y - _distanceBetweenY);

                            Canvas.SetLeft(_highlightedRectangle, (int)(Cursor.X - _distanceBetweenX));
                            Canvas.SetTop(_highlightedRectangle, (int)(Cursor.Y - _distanceBetweenY));

                            _rectangles[_indexOfCanvasElement - 1] = _rectangle;

                            canvas.Children[_indexOfCanvasElement] = _highlightedRectangle;
                        }
                        return;
                    }
                    if (_startPoint.X < Cursor.X && _startPoint.Y < Cursor.Y)
                    {
                        LeftCorner = new Point(_startPoint.X, _startPoint.Y);
                    }
                    else if (_startPoint.X < Cursor.X && _startPoint.Y > Cursor.Y)
                    {
                        LeftCorner = new Point(_startPoint.X, Cursor.Y);
                    }
                    else if (_startPoint.X > Cursor.X && _startPoint.Y > Cursor.Y)
                    {
                        LeftCorner = new Point(Cursor.X, Cursor.Y);
                    }
                    else if (_startPoint.X > Cursor.X && _startPoint.Y < Cursor.Y)
                    {
                        LeftCorner = new Point(Cursor.X, _startPoint.Y);
                    }

                    _rectangle.X = (int)LeftCorner.X;
                    _rectangle.Y = (int)LeftCorner.Y;

                    _rectangle.Width = (int)Math.Abs(_startPoint.X - Cursor.X);
                    _rectangle.Height = (int)Math.Abs(_startPoint.Y - Cursor.Y);

                    _rectangles[_rectangles.Count - 1] = _rectangle;

                    Canvas.SetLeft(_highlightedRectangle, LeftCorner.X);
                    Canvas.SetTop(_highlightedRectangle, LeftCorner.Y);

                    _highlightedRectangle.Width = Math.Abs(_startPoint.X - Cursor.X);
                    _highlightedRectangle.Height = Math.Abs(_startPoint.Y - Cursor.Y);
                }
            }
            catch { }
        }
        #endregion
        #region WindowButtons
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        #endregion
    }
}
