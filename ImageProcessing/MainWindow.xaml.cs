using ImageProcessing.Enum;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RectangleElement = System.Windows.Shapes.Rectangle;
using Rectangle = System.Drawing.Rectangle;
using Point = System.Windows.Point;
using Object = ImageProcessing.Enum.Object;
using System.Collections.ObjectModel;

namespace ImageProcessing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainViewModel _viewModel;
        MouseState _mouseState = MouseState.LeftUp;
        Object _obj = Object.NotFound;
        public ObservableCollection<Rectangle> Rectangles;
        Rectangle _rectangle;
        RectangleElement _highlightedRectangle;
        Point _startPoint;
        int _indexOfCanvasElement;
        double _distanceBetweenX;
        double _distanceBetweenY;
        public MainWindow()
        {
            InitializeComponent();
            Rectangles = new ObservableCollection<Rectangle>();

            _viewModel = new MainViewModel(this);
            DataContext = _viewModel;
            
            _startPoint = new Point();
        }
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
        }
        private bool IsClickInsideTheObjects(Point p)
        {
            for (int i = 0;i < Rectangles.Count;i++)
            {
                if (Rectangles[i].Contains(new System.Drawing.Point((int)p.X,(int)p.Y)))
                {
                    _indexOfCanvasElement = i + 1;

                    _highlightedRectangle = (RectangleElement)canvas.Children[_indexOfCanvasElement];
                    _rectangle = Rectangles[i];

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
            Rectangles.Add(_rectangle);
        }
        private new void MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                _mouseState = MouseState.LeftUp;
                if (_rectangle.Width == 0 ||  _rectangle.Height == 0)
                {
                    canvas.Children.RemoveAt(canvas.Children.Count - 1);
                    Rectangles.RemoveAt(Rectangles.Count - 1);
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
                        if(Cursor.X - _distanceBetweenX >= 100 && 
                            Cursor.Y - _distanceBetweenY >= 0 && 
                            Cursor.X - _distanceBetweenX + _rectangle.Width <= 1100 &&
                            Cursor.Y - _distanceBetweenY + _rectangle.Height <= 500)
                            {
                            _rectangle.X = (int)(Cursor.X - _distanceBetweenX);
                            _rectangle.Y = (int)(Cursor.Y - _distanceBetweenY);

                            Canvas.SetLeft(_highlightedRectangle, (int)(Cursor.X - _distanceBetweenX));
                            Canvas.SetTop(_highlightedRectangle, (int)(Cursor.Y - _distanceBetweenY));

                            Rectangles[_indexOfCanvasElement - 1] = _rectangle;

                            canvas.Children[_indexOfCanvasElement] = _highlightedRectangle;
                            }
                        return;
                    }
                    if (_startPoint.X < Cursor.X && _startPoint.Y < Cursor.Y)
                    {
                        LeftCorner = new Point(_startPoint.X,_startPoint.Y);
                    }
                    else if(_startPoint.X < Cursor.X && _startPoint.Y > Cursor.Y)
                    {
                        LeftCorner = new Point(_startPoint.X, Cursor.Y);
                    }
                    else if(_startPoint.X > Cursor.X && _startPoint.Y > Cursor.Y)
                    {
                        LeftCorner = new Point(Cursor.X,Cursor.Y);
                    }
                    else if(_startPoint.X > Cursor.X && _startPoint.Y < Cursor.Y) 
                    {
                        LeftCorner = new Point(Cursor.X, _startPoint.Y);
                    }

                    _rectangle.X = (int)LeftCorner.X;
                    _rectangle.Y = (int)LeftCorner.Y;

                    _rectangle.Width = (int)Math.Abs(_startPoint.X - Cursor.X);
                    _rectangle.Height = (int)Math.Abs(_startPoint.Y - Cursor.Y);

                    Rectangles[Rectangles.Count - 1] = _rectangle;

                    Canvas.SetLeft(_highlightedRectangle, LeftCorner.X);
                    Canvas.SetTop(_highlightedRectangle, LeftCorner.Y);

                    _highlightedRectangle.Width = Math.Abs(_startPoint.X - Cursor.X);
                    _highlightedRectangle.Height = Math.Abs(_startPoint.Y - Cursor.Y);
                }
            }
            catch { }
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
    }
}
