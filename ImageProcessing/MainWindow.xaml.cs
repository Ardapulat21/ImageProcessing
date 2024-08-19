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
        MainViewModel viewModel;
        MouseState MouseState = MouseState.LeftUp;
        Object Obj = Object.NotFound;
        public ObservableCollection<Rectangle> Rectangles;
        Rectangle Rectangle;
        RectangleElement HighlightedRectangle;
        Point startPoint;
        Point endPoint;
        int indexOfCanvasElement;
        double distanceBetweenX;
        double distanceBetweenY;
        public MainWindow()
        {
            InitializeComponent();
            Rectangles = new ObservableCollection<Rectangle>();

            viewModel = new MainViewModel(this);
            DataContext = viewModel;
            
            startPoint = new Point();
            endPoint = new Point();
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
                    indexOfCanvasElement = i + 1;

                    HighlightedRectangle = (RectangleElement)canvas.Children[indexOfCanvasElement];
                    Rectangle = Rectangles[i];

                    distanceBetweenX = startPoint.X - Rectangle.X;
                    distanceBetweenY = startPoint.Y - Rectangle.Y;

                    return true;
                }
            }
            return false;
        }

        private new void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MouseState = MouseState.LeftDown;
            startPoint = e.GetPosition(this);
            if (IsClickInsideTheObjects(startPoint))
            {
                Obj = Object.Found;
                return;
            }
            Obj = Object.NotFound;
            AddNewElementToCanvas();
        }
        private void AddNewElementToCanvas()
        {
            Rectangle = new Rectangle();
            HighlightedRectangle = new RectangleElement();

            HighlightedRectangle.Stroke = new SolidColorBrush() { Color = Colors.Black };
            
            canvas.Children.Add(HighlightedRectangle);
            Rectangles.Add(Rectangle);
        }
        private new void MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                MouseState = MouseState.LeftUp;
                endPoint = e.GetPosition(this);
                if (Rectangle.Width == 0 ||  Rectangle.Height == 0)
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
                if (MouseState == MouseState.LeftDown)
                {
                    Point Cursor = e.GetPosition(this);
                    Point LeftCorner = new Point();

                    if (Obj == Object.Found)
                    {
                        if(Cursor.X - distanceBetweenX >= 100 && 
                            Cursor.Y - distanceBetweenY >= 0 && 
                            Cursor.X - distanceBetweenX + Rectangle.Width <= 1100 &&
                            Cursor.Y - distanceBetweenY + Rectangle.Height <= 500)
                            {
                            Rectangle.X = (int)(Cursor.X - distanceBetweenX);
                            Rectangle.Y = (int)(Cursor.Y - distanceBetweenY);

                            Canvas.SetLeft(HighlightedRectangle, (int)(Cursor.X - distanceBetweenX));
                            Canvas.SetTop(HighlightedRectangle, (int)(Cursor.Y - distanceBetweenY));

                            Rectangles[indexOfCanvasElement - 1] = Rectangle;

                            canvas.Children[indexOfCanvasElement] = HighlightedRectangle;
                            }
                        return;
                    }
                    if (startPoint.X < Cursor.X && startPoint.Y < Cursor.Y)
                    {
                        LeftCorner = new Point(startPoint.X,startPoint.Y);
                    }
                    else if(startPoint.X < Cursor.X && startPoint.Y > Cursor.Y)
                    {
                        LeftCorner = new Point(startPoint.X, Cursor.Y);
                    }
                    else if(startPoint.X > Cursor.X && startPoint.Y > Cursor.Y)
                    {
                        LeftCorner = new Point(Cursor.X,Cursor.Y);
                    }
                    else if(startPoint.X > Cursor.X && startPoint.Y < Cursor.Y) 
                    {
                        LeftCorner = new Point(Cursor.X, startPoint.Y);
                    }

                    Rectangle.X = (int)LeftCorner.X;
                    Rectangle.Y = (int)LeftCorner.Y;

                    Rectangle.Width = (int)Math.Abs(startPoint.X - Cursor.X);
                    Rectangle.Height = (int)Math.Abs(startPoint.Y - Cursor.Y);

                    Rectangles[Rectangles.Count - 1] = Rectangle;

                    Canvas.SetLeft(HighlightedRectangle, LeftCorner.X);
                    Canvas.SetTop(HighlightedRectangle, LeftCorner.Y);

                    HighlightedRectangle.Width = Math.Abs(startPoint.X - Cursor.X);
                    HighlightedRectangle.Height = Math.Abs(startPoint.Y - Cursor.Y);
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
