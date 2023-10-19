using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CroppingImageLibrary.Managers
{
    internal class PolygonManager
    {
        private enum TouchPoint
        {
            OutsidePolygon,
            InsidePolygon,
        }

        public event EventHandler PolygonSizeChanged;
        public event EventHandler OnPolygonDoubleClickEvent;

        private readonly Polygon _polygon;
        private readonly Polygon _dashedPolygon;
        private readonly Canvas _canvas;
        private bool _isDrawing;
        private bool _isDragging;

        private PointCollection _points = new PointCollection();
        private Point _mouseStartPoint;
        private Point _mouseLastPoint;


        public PointCollection Points
        {
            get { return _points; }
            set
            {
                _points = value;
                _polygon.Points = value;
                _dashedPolygon.Points = value;
            }
        }

        public PolygonManager(Canvas canvasOverlay)
        {
            _canvas = canvasOverlay;

            _polygon = new Polygon()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 1.5
            };

            _dashedPolygon = new Polygon()
            {
                Stroke = Brushes.White,
                StrokeDashArray = new DoubleCollection(new double[] { 4, 4 })
            };

            _canvas.Children.Add(_polygon);
            _canvas.Children.Add(_dashedPolygon);

            _polygon.Points = _points;

            _polygon.SizeChanged += (sender, args) =>
            {
                if (PolygonSizeChanged != null)
                    PolygonSizeChanged(sender, args);
            };
        }

        public void MouseLeftButtonDownEventHandler(MouseButtonEventArgs e)
        {
            _canvas.CaptureMouse();
            Point mouseClickPoint = e.GetPosition(_canvas);

            TouchPoint touch = GetTouchPoint(mouseClickPoint);

            if (_points.Count > 0 && touch == TouchPoint.OutsidePolygon)
            {
                _points.Clear();
                Points = _points;
                _isDrawing = true;
                _mouseStartPoint = mouseClickPoint;
            }

            if (_points.Count == 0)
            {
                _mouseStartPoint = mouseClickPoint;
                _isDrawing = true;
            }

            if (_points.Count > 0 && touch != TouchPoint.OutsidePolygon)
            {
                if (e.ClickCount == 2)
                {
                    OnPolygonDoubleClickEvent(this, EventArgs.Empty);
                    return;
                }
                _isDragging = true;
                _mouseLastPoint = mouseClickPoint;
            }
        }

        public void MouseMoveEventHandler(MouseEventArgs e)
        {
            Point mouseClickPoint = e.GetPosition(_canvas);

            if (_isDrawing)
            {
                _points.Clear();
                _points.Add(_mouseStartPoint);
                _points.Add(new Point(mouseClickPoint.X, _mouseStartPoint.Y));
                _points.Add(mouseClickPoint);
                _points.Add(new Point(_mouseStartPoint.X, mouseClickPoint.Y));
                Points = _points;
                return;
            }
            if (_isDragging)
            {
                double offsetX = mouseClickPoint.X - _mouseLastPoint.X;
                double offsetY = mouseClickPoint.Y - _mouseLastPoint.Y;

                for (int i = 0; i < _points.Count; i++)
                {
                    _points[i] = new Point(_points[i].X + offsetX, _points[i].Y + offsetY);
                }
                Points = _points;
                _mouseLastPoint = mouseClickPoint;
            }
        }
        public void MouseLeftButtonUpEventHandler()
        {
            _isDrawing = false;
            _isDragging = false;
            _canvas.ReleaseMouseCapture();
        }

        private TouchPoint GetTouchPoint(Point mousePoint)
        {
            if (_polygon.Points.Count < 3)
                return TouchPoint.OutsidePolygon;

            var polygon = new System.Windows.Shapes.Polygon();
            foreach (var point in _polygon.Points)
            {
                polygon.Points.Add(point);
            }

            if (polygon.RenderedGeometry.FillContains(mousePoint))
                return TouchPoint.InsidePolygon;

            return TouchPoint.OutsidePolygon;
        }
    }
}
