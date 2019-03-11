// ------------------------------------------------------------------------------------------------------
// LightningChart® example code: 3D Point Lines with mouse tracking.
//
// If you need any assistance, or notice error in this example code, please contact support@arction.com. 
//
// Permission to use this code in your application comes with LightningChart® license. 
//
// http://arction.com/ | support@arction.com | sales@arction.com
//
// © Arction Ltd 2009-2019. All rights reserved.  
// ------------------------------------------------------------------------------------------------------
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

// Arction namespaces.
using Arction.Wpf.SemibindableCharting;             // LightningChartUltimate and general types.
using Arction.Wpf.SemibindableCharting.Series3D;    // Series for 3D chart.

namespace _3DPointLines_WFP_SB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Random number generator for creating example series data.
        /// </summary>
        private Random random = new Random();

        public MainWindow()
        {
            InitializeComponent();

            // Create chart.
            // This is done using XAML.

            // Disable rendering before updating chart properties to improve performance
            // and to prevent unnecessary chart redrawing while changing multiple properties.
            chart.BeginUpdate();

            // Set Z-axis range.
            chart.View3D.ZAxisPrimary3D.SetRange(0, 80);

            // Add 3D pointlines to the view with different colors.
            CreatePointLine(0, Colors.Red);
            CreatePointLine(1, Colors.Orange);
            CreatePointLine(2, Colors.Yellow);
            CreatePointLine(3, Colors.Green);
            CreatePointLine(4, Colors.Blue);
            CreatePointLine(5, Colors.Indigo);
            CreatePointLine(6, Colors.Violet);

            // Set chart's title and default camera rotation.
            chart.Title.Text = "3D Point Lines";

            // Set default lights scheme for 3D view as this is required by the bindable charts.
            chart.View3D.SetPredefinedLightingScheme(LightingScheme.Default);

            #region Hidden polishing
            CustomizeChart(chart);
            #endregion

            // Call EndUpdate to enable rendering again.
            chart.EndUpdate();

            // Safe disposal of LC components when the window is closed.
            Closed += new EventHandler(Window_Closed);
        }

        /// <summary>
        /// Create a PointLineSeries3D with default random data and add it to the chart.
        /// </summary>
        /// <param name="i">Index of the series.</param>
        /// <param name="color">Series' points & line color.</param>
        private void CreatePointLine(int i, Color color)
        {
            // Create a new PointLineSeries3D for displaying data and set axis bindings to primary axes.
            var series = new PointLineSeries3D(chart.View3D, Axis3DBinding.Primary, Axis3DBinding.Primary, Axis3DBinding.Primary)
            {
                // Set this to true to set a color for individual points.
                IndividualPointColors = true,
                // Set this to true in order for mouse tracking to work.
                MouseInteraction = true
            };

            // Apply styling to the series.

            // Set a title to the series.
            series.Title.Text = "Series " + (i + 1);

            // Set point shape to a sphere.
            series.PointStyle.Shape3D = PointShape3D.Sphere;

            // Set individual point size.
            series.PointStyle.Size3D.SetValues(3, 3, 3);

            // Set the width of the line between points.
            series.LineStyle.Width = 0.4f;

            // Set the line color.
            series.LineStyle.Color = color;

            // Draw the line between points with the same color as the points.
            series.LineStyle.LineOptimization = LineOptimization.NormalWithShading;

            // Create a SeriesPoint3D array for data points.
            SeriesPoint3D[] points = new SeriesPoint3D[10];

            // Generate sample data to the array.
            for (int j = 0; j < 10; j++)
            {
                points[j].X = 5 + j * 10;
                points[j].Y = 30 + random.NextDouble() * 25.0;
                points[j].Z = 10 + i * 10;
                points[j].Color = color;
            }

            // Set series points as the newly created array.
            series.Points = points;

            // Add the series to chart's View3D.
            chart.View3D.PointLineSeries3D.Add(series);
        }

        private void Chart_MouseMove(object sender, MouseEventArgs e)
        {
            // Call BeginUpdate for chart to disable rendering while mouse is moving 
            // over the chart to improve performance.
            chart.BeginUpdate();

            // Set label visible when not hovered over by mouse.
            mouseAnnotation.Visible = false;

            // Check if any object has been found under the mouse.
            object obj = chart.GetActiveMouseOverObject();
            if (obj != null)
            {
                // Check if the active mouse over object is a PointLineSeries object.
                if (obj is PointLineSeries3D)
                {
                    PointLineSeries3D pointLineSeries3D = obj as PointLineSeries3D;

                    // Get the point last hit by mouse.
                    int pointIndex = pointLineSeries3D.LastMouseHitTestIndex;
                    SeriesPoint3D point = pointLineSeries3D.Points[pointIndex];

                    // Set annotation position to the moused over point.
                    mouseAnnotation.TargetAxisValues.SetValues(point.X, point.Y, point.Z);

                    // Set annotation text to display information about the moused over point.
                    mouseAnnotation.Text = "Series index: " + chart.View3D.PointLineSeries3D.IndexOf(pointLineSeries3D).ToString()
                        + "\nPoint index: " + pointIndex.ToString()
                        + "\nX=" + point.X.ToString("0.0") + " ; Y=" + point.Y.ToString("0.0") + " ; Z=" + point.Z.ToString("0.0");

                    // Set the annotation visible while mouse is hovering over the point.
                    mouseAnnotation.Visible = true;
                }
            }

            // Call EndUpdate to enable rendering again after handling mouse move event.
            chart.EndUpdate();
        }

        // Safe disposal of LightningChart components when the window is closed.
        private void Window_Closed(Object sender, EventArgs e)
        {
            // Dispose Chart.
            chart.Dispose();
            chart = null;
        }

        #region Hidden Polishing
        private void CustomizeChart(LightningChartUltimate chart)
        {
            chart.ChartBackground.Color = Color.FromArgb(255, 30, 30, 30);
            chart.ChartBackground.GradientFill = GradientFill.Solid;
            chart.Title.Color = Color.FromArgb(255, 249, 202, 3);
            chart.Title.MouseHighlight = MouseOverHighlight.None;
        }
        #endregion
    }
}
