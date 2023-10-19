using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CroppingImageLibrary.Managers
{
    /// <summary>
    /// Class that response for adding shadow area outside of cropping rectangle)
    /// </summary>
    internal class OverlayManager
    {
        private readonly Canvas _canvas;
        private readonly PolygonManager _polygonManager;

        private readonly Path _pathOverlay;
        private GeometryGroup _geometryGroup;

        public OverlayManager(Canvas canvas, PolygonManager rectangleManager)
        {
            _canvas = canvas;
            _polygonManager = rectangleManager;

            _pathOverlay = new Path
            {
                Fill = Brushes.Orange,
                Opacity = 0.5
            };

            _canvas.Children.Add(_pathOverlay);
        }

        /// <summary>
        /// Update (redraw) overlay
        /// </summary>
        public void UpdateOverlay()
        {
            _geometryGroup = new GeometryGroup();
            RectangleGeometry geometry1 =
                new RectangleGeometry(new Rect(new Size(_canvas.ActualWidth, _canvas.ActualHeight)));
            PointCollection points = new PointCollection();
            foreach (var point in _polygonManager.Points)
            {
                points.Add(new Point(point.X, point.Y));
            }
            StreamGeometry geometry = new StreamGeometry();
            if (points.Count > 0)
                using (StreamGeometryContext ctx = geometry.Open())
                {
                    ctx.BeginFigure(points[0], true, true);
                    ctx.PolyLineTo(points, true, true);
                }

            _geometryGroup.Children.Add(geometry1);
            _geometryGroup.Children.Add(geometry);
            _pathOverlay.Data = _geometryGroup;
        }
    }
}
