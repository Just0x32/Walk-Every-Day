using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Walk_Every_Day.GraphTypes
{
    public class GraphDrawingImage
    {
        #region [ Properties ]
        public Image CoordinateLines { get; private set; }

        public Image GridLines { get; private set; }

        public Image ValueLines { get; private set; }

        public Image MaxValuePoint { get; private set; }

        public Image MinValuePoint { get; private set; }

        public int LeftX { get; private set; }

        public int RightX { get; private set; }

        public int StepX { get; private set; }

        public int BottomY { get; private set; }

        public int TopY { get; private set; }

        public int StepY { get; private set; }

        public IntegerPoint MaxValue { get; private set; }

        public IntegerPoint MinValue { get; private set; }

        public bool AreMinMaxYValuesChecked { get; private set; }

        public Brush CoordinateLinesColor { get; private set; }

        public Brush GridLinesColor { get; private set; }

        public Brush ValuesLinesColor { get; private set; }

        public double ValueLineThikness { get; private set; }

        #endregion

        private readonly int graphSize = 1000;
        private readonly double xOffset;
        private readonly double yOffset;
        private readonly double xScale;
        private readonly double yScale;

        private GeometryGroup coordinatesLinesGroup;
        private GeometryGroup gridLinesGroup;
        private GeometryGroup valueLinesGroup;
        private GeometryGroup maxValuePointsGroup;
        private GeometryGroup minValuePointGroup;

        DrawingImage drawingImage;

        private readonly List<IntegerPoint> points;

        public GraphDrawingImage(
            int leftX, int rightX, int stepX,
            int bottomY, int topY, int stepY,
            List<IntegerPoint> points,
            Brush coordinateLinesColor, Brush gridLinesColor,
            Brush valueLinesColor,
            double valueLineThikness
            )
        {
            CoordinateLinesColor = coordinateLinesColor;
            GridLinesColor = gridLinesColor;
            ValuesLinesColor = valueLinesColor;
            ValueLineThikness = valueLineThikness;

            SetProperties(leftX, rightX, stepX, bottomY, topY, stepY);

            xScale = DimensionScale(LeftX, RightX);
            yScale = DimensionScale(BottomY, TopY);

            xOffset = DimensionOffset(StepX, xScale);
            yOffset = DimensionOffset(StepY, yScale);

            this.points = points;

            MakeCoordinateLines();
            CoordinateLines = ReturnImage(coordinatesLinesGroup, CoordinateLinesColor, ValueLineThikness);

            MakeGridLines();
            GridLines = ReturnImage(gridLinesGroup, GridLinesColor, ValueLineThikness / 4);

            MakeValueLine(points);
            ValueLines = ReturnImage(valueLinesGroup, ValuesLinesColor, ValueLineThikness);

            MakeMaxPoint();
            MakeMinPoint();
            MaxValuePoint = ReturnImage(maxValuePointsGroup, Brushes.Green, ValueLineThikness * 2);
            MinValuePoint = ReturnImage(minValuePointGroup, Brushes.Red, ValueLineThikness * 2);
        }

        private void SetProperties(int startX, int endX, int incrementX, int startY, int endY, int incrementY)
        {
            int[] validXArguments = ReturnValidStartEndIncrement(startX, endX, incrementX);
            int[] validYArguments = ReturnValidStartEndIncrement(startY, endY, incrementY);

            LeftX = validXArguments[0];
            RightX = validXArguments[1];
            StepX = validXArguments[2];

            BottomY = validYArguments[0];
            TopY = validYArguments[1];
            StepY = validYArguments[2];

            int[] ReturnValidStartEndIncrement(int start, int end, int increment)
            {
                int[] validValues = new int[3];

                if (increment == 0 && start == end)
                {
                    validValues[0] = start;
                    validValues[2] = start / Math.Abs(start);
                    validValues[1] = start + increment;
                }
                else if (increment == 0 || Math.Sign(end - start) != Math.Sign(increment))
                {
                    validValues[0] = start;
                    validValues[1] = end;
                    validValues[2] = end - start;
                }
                else if ((end - start) % increment != 0)
                {
                    validValues[0] = start;
                    validValues[1] = (((end - start) / increment) + 1) * increment + start;
                    validValues[2] = increment;
                }
                else
                {
                    validValues[0] = start;
                    validValues[1] = end;
                    validValues[2] = increment;
                }

                return validValues;
            }
        }

        private double DimensionScale(int start, int end) => (double)graphSize / Math.Abs(end - start);

        private double DimensionOffset(int step, double scale) => (double)step / 2 * scale;

        private void MakeCoordinateLines()
        {
            coordinatesLinesGroup = new GeometryGroup();

            coordinatesLinesGroup.Children.Add(Line(LeftX, RightX, BottomY, BottomY));
            coordinatesLinesGroup.Children.Add(Line(LeftX, LeftX, BottomY, TopY));

            AddBorderPoints(coordinatesLinesGroup);
        }

        private void MakeGridLines()
        {
            gridLinesGroup = new GeometryGroup();

            MakeVerticalLines();
            MakeHorizontalLines();

            AddBorderPoints(gridLinesGroup);

            void MakeVerticalLines()
            {
                int quantity = (RightX - LeftX) / StepX;
                int distance = LeftX;

                for (int i = 0; i < quantity; i++)
                {
                    distance += StepX;
                    gridLinesGroup.Children.Add(Line(distance, distance, BottomY, TopY));
                }
            }

            void MakeHorizontalLines()
            {
                int quantity = (TopY - BottomY) / StepY;
                int distance = BottomY;

                for (int i = 0; i < quantity; i++)
                {
                    distance += StepY;
                    gridLinesGroup.Children.Add(Line(LeftX, RightX, distance, distance));
                }
            }
        }

        private void MakeValueLine(List<IntegerPoint> points)
        {
            valueLinesGroup = new GeometryGroup();

            if (points.Count > 1)
            {
                MakePolyline();
            }

            AddBorderPoints(valueLinesGroup);

            void MakePolyline()
            {
                IntegerPoint maxPoint = new IntegerPoint(points[0].X, points[0].Y);
                IntegerPoint minPoint = new IntegerPoint(points[0].X, points[0].Y);

                for (int i = 1; i < points.Count; i++)
                {
                    if (points[i].Y > maxPoint.Y)
                    {
                        maxPoint = new IntegerPoint(points[i].X, points[i].Y);
                    }
                    else if (points[i].Y < minPoint.Y)
                    {
                        minPoint = new IntegerPoint(points[i].X, points[i].Y);
                    }

                    valueLinesGroup.Children.Add(Line(points[i - 1].X, points[i].X, points[i - 1].Y, points[i].Y));
                }

                MaxValue = maxPoint;
                MinValue = minPoint;
            }
        }

        private void MakeMaxPoint()
        {
            maxValuePointsGroup = new GeometryGroup();

            maxValuePointsGroup.Children.Add(Ellipse(MaxValue.X, MaxValue.Y));

            AddBorderPoints(maxValuePointsGroup);
        }

        private void MakeMinPoint()
        {
            minValuePointGroup = new GeometryGroup();

            minValuePointGroup.Children.Add(Ellipse(MinValue.X, MinValue.Y));

            AddBorderPoints(minValuePointGroup);
        }

        private void AddBorderPoints(GeometryGroup group)
        {
            group.Children.Add(Line(0, 0, graphSize + 2 * yOffset, graphSize + 2 * yOffset, true));
            group.Children.Add(Line(graphSize + 2 * xOffset, graphSize + 2 * xOffset, 0, 0, true));
        }

        private LineGeometry Line(double startX, double endX, double startY, double endY, bool isAbsoluteValues = false)
        {
            double scaledStartX;
            double scaledEndX;
            double scaledStartY;
            double scaledEndY;

            if (!isAbsoluteValues)
            {
                scaledStartX = xScale * (startX - LeftX) + xOffset;
                scaledEndX = xScale * (endX - LeftX) + xOffset;
                scaledStartY = yScale * (startY - BottomY) + yOffset;
                scaledEndY = yScale * (endY - BottomY) + yOffset;
            }
            else
            {
                scaledStartX = startX - LeftX;
                scaledEndX = endX - LeftX;
                scaledStartY = startY - BottomY;
                scaledEndY = endY - BottomY;
            }

            return new LineGeometry(new Point(scaledStartX, scaledStartY), new Point(scaledEndX, scaledEndY));
        }

        private EllipseGeometry Ellipse(double centerX, double centerY)
        {
            double scaledStartX = xScale * (centerX - LeftX) + xOffset;
            double scaledStartY = yScale * (centerY - BottomY) + yOffset;

            return new EllipseGeometry(new Point(scaledStartX, scaledStartY), 20, 20);
        }

        private Image ReturnImage(GeometryGroup geometryGroup, Brush color, double thickness)
        {
            GeometryDrawing drawing = new GeometryDrawing();
            drawing.Geometry = geometryGroup;

            drawing.Pen = new Pen(color, thickness);

            drawingImage = new DrawingImage(drawing);

            drawingImage.Freeze();

            Image image = new Image();
            image.Source = drawingImage;
            image.Stretch = Stretch.Fill;
            image.LayoutTransform = new ScaleTransform(1, -1, 0.5, 0.5);

            return image;
        }
    }
}
