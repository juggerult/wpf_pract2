using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace wpf_pract2;

public partial class MainWindow : Window
{
    private readonly Image butterfly;
    private double xPosition = 100;
    private double yPosition = 100;
    private double xSpeed = 2;
    private double ySpeed = 1;
    private int wingState = 0;
    private readonly DispatcherTimer animationTimer;

    public MainWindow()
    {
        InitializeComponent();

        // Додавання метелика
        butterfly = new Image
        {
            Width = 80,
            Height = 60,
            RenderTransformOrigin = new Point(0.5, 0.5)
        };

        UpdateButterflyImage();
        MainCanvas.Children.Add(butterfly);

        animationTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(50)
        };
        animationTimer.Tick += AnimationTick;
        animationTimer.Start();
    }

    private void AnimationTick(object sender, EventArgs e)
    {
        xPosition += xSpeed;
        yPosition += ySpeed;

        if (xPosition <= 0 || xPosition >= MainCanvas.ActualWidth - butterfly.Width)
        {
            xSpeed = -xSpeed;
        }

        if (yPosition <= 0 || yPosition >= MainCanvas.ActualHeight - butterfly.Height)
        {
            ySpeed = -ySpeed;
        }

        Canvas.SetLeft(butterfly, xPosition);
        Canvas.SetTop(butterfly, yPosition);

        wingState = (wingState + 1) % 4;
        UpdateButterflyImage();

        if (new Random().Next(30) == 0)
        {
            xSpeed += (new Random().NextDouble() - 0.5) * 2;
            ySpeed += (new Random().NextDouble() - 0.5) * 2;

            double speed = Math.Sqrt(xSpeed * xSpeed + ySpeed * ySpeed);
            if (speed > 5)
            {
                xSpeed = xSpeed / speed * 5;
                ySpeed = ySpeed / speed * 5;
            }
        }
    }

    private void UpdateButterflyImage()
    {
        DrawingVisual visual = new DrawingVisual();
        using (DrawingContext dc = visual.RenderOpen())
        {
            dc.DrawEllipse(Brushes.Black, null, new Point(40, 30), 5, 15);

            switch (wingState)
            {
                case 0:
                    dc.DrawGeometry(Brushes.Purple, null, CreateWingGeometry(40, 30, 30, 10, 10, 40));
                    dc.DrawGeometry(Brushes.Purple, null, CreateWingGeometry(40, 30, 50, 10, 70, 40));
                    break;
                case 1:
                    dc.DrawGeometry(Brushes.Purple, null, CreateWingGeometry(40, 30, 25, 15, 5, 35));
                    dc.DrawGeometry(Brushes.Purple, null, CreateWingGeometry(40, 30, 55, 15, 75, 35));
                    break;
                case 2:
                    dc.DrawGeometry(Brushes.Purple, null, CreateWingGeometry(40, 30, 30, 20, 10, 30));
                    dc.DrawGeometry(Brushes.Purple, null, CreateWingGeometry(40, 30, 50, 20, 70, 30));
                    break;
                case 3:
                    dc.DrawGeometry(Brushes.Purple, null, CreateWingGeometry(40, 30, 25, 15, 5, 35));
                    dc.DrawGeometry(Brushes.Purple, null, CreateWingGeometry(40, 30, 55, 15, 75, 35));
                    break;
            }
        }

        RenderTargetBitmap rtb = new RenderTargetBitmap(
            (int)butterfly.Width,
            (int)butterfly.Height,
            96,
            96,
            PixelFormats.Pbgra32);
        rtb.Render(visual);

        butterfly.Source = rtb;
    }

    private Geometry CreateWingGeometry(double x0, double y0, double x1, double y1, double x2, double y2)
    {
        PathFigure figure = new PathFigure
        {
            StartPoint = new Point(x0, y0),
            IsClosed = true
        };

        figure.Segments.Add(new LineSegment(new Point(x1, y1), true));
        figure.Segments.Add(new LineSegment(new Point(x2, y2), true));

        PathGeometry geometry = new PathGeometry();
        geometry.Figures.Add(figure);
        return geometry;
    }
}