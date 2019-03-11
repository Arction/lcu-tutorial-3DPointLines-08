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
using System.Drawing;
using System.Windows.Forms;

//Arction namespaces.
using Arction.WinForms.Charting;             // LightningChartUltimate and general types.
using Arction.WinForms.Charting.Series3D;    // Series for 3D chart.
using Arction.WinForms.Charting.Annotations; // Annotations for LightningChart.

namespace _3DPointLines_WF
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// LightningChart component.
        /// </summary>
        private LightningChartUltimate chart = null;

        /// <summary>
        /// Random number generator for creating example series data.
        /// </summary>
        private Random random = new Random();

        /// <summary>
        /// Annotation which shows target values when hovering over a point with the mouse.
        /// </summary>
        private Annotation3D mouseAnnotation;

        public Form1()
        {
            InitializeComponent();

            // Create chart.
            chart = new LightningChartUltimate();

            // Set chart control into the parent container.
            chart.Parent = this;
            chart.Dock = DockStyle.Fill;

            // Disable rendering before updating chart properties to improve performance
            // and to prevent unnecessary chart redrawing while changing multiple properties.
            chart.BeginUpdate();

            // 1. Set View3D as active view and set Z-axis range.
            chart.ActiveView = ActiveView.View3D;
            chart.View3D.ZAxisPrimary3D.SetRange(0, 80);

            // 2. Add 3D pointlines to the view with different colors.
            CreatePointLine(0, Color.Red);
            CreatePointLine(1, Color.Orange);
            CreatePointLine(2, Color.Yellow);
            CreatePointLine(3, Color.Green);
            CreatePointLine(4, Color.Blue);
            CreatePointLine(5, Color.Indigo);
            CreatePointLine(6, Color.Violet);

            // 5. Create a new annotation to display target values when hovering over a point with the mouse.
            mouseAnnotation = new Annotation3D(chart.View3D, Axis3DBinding.Primary, Axis3DBinding.Primary, Axis3DBinding.Primary)
            {
                Visible = false,
                TargetCoordinateSystem = AnnotationTargetCoordinates.AxisValues,
                LocationCoordinateSystem = CoordinateSystem.RelativeCoordinatesToTarget,
                MouseInteraction = false
            };

            // Set offset to annotation.
            mouseAnnotation.LocationRelativeOffset.SetValues(40, -70);

            // Add annotation to View3D.
            chart.View3D.Annotations.Add(mouseAnnotation);

            // 6. Add mouse move event handler to chart to enable tracking points with the mouse.
            chart.MouseMove += _chart_MouseMove;

            // Set chart's title and default camera rotation.
            chart.Title.Text = "3D Point Lines";
            chart.View3D.Camera.RotationX = 30;
            chart.View3D.Camera.RotationY = -50;

            // Call EndUpdate to enable rendering again.
            chart.EndUpdate();

            // Safe disposal of LC components when the form is closed.
            FormClosed += new FormClosedEventHandler(Form_Closed);

            #region Hidden polishing
            // Customize chart.
            CustomizeChart(chart);
            #endregion
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

            // 3. Apply styling to the series.

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

            // 4. Create a SeriesPoint3D array for data points.
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

        // 7. Create a function for mouse move event handler.
        private void _chart_MouseMove(object sender, MouseEventArgs e)
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


        // Safe disposal of LightningChart components when the form is closed.
        private void Form_Closed(Object sender, FormClosedEventArgs e)
        {
            // Dispose Chart.
            chart.Dispose();
            chart = null;
        }

        #region Hidden polishing
        void CustomizeChart(LightningChartUltimate chart)
        {
            chart.Background.Color = Color.FromArgb(255, 30, 30, 30);
            chart.Background.GradientFill = GradientFill.Solid;
            chart.Title.Color = Color.FromArgb(255, 249, 202, 3);
            chart.Title.MouseHighlight = MouseOverHighlight.None;
        }
        #endregion
    }
}
