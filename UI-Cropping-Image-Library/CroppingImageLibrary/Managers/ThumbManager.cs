using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using CroppingImageLibrary.Thumbs;

namespace CroppingImageLibrary.Managers
{
    /// <summary>
    /// Class that represent for displaying/redraw thumbs
    /// </summary>
    internal class ThumbManager
    {
        private readonly ThumbCrop _topLeft, _topRight, _bottomLeft, _bottomRight;
        private readonly Canvas _canvas;
        private readonly PolygonManager _polygonManager;
        private readonly double _thumbSize;

        public ThumbManager(Canvas canvas, PolygonManager rectangleManager)
        {
            //  initizalize
            _canvas = canvas;
            _polygonManager = rectangleManager;
            _thumbSize = 10;

            //  create thumbs with factory
            _topLeft = ThumbFactory.CreateThumb(ThumbFactory.ThumbPosition.TopLeft, _canvas, _thumbSize);
            _topRight = ThumbFactory.CreateThumb(ThumbFactory.ThumbPosition.TopRight, _canvas, _thumbSize);
            _bottomLeft = ThumbFactory.CreateThumb(ThumbFactory.ThumbPosition.BottomLeft, _canvas, _thumbSize);
            _bottomRight = ThumbFactory.CreateThumb(ThumbFactory.ThumbPosition.BottomRight, _canvas, _thumbSize);

            //  subsctibe to mouse events
            _topLeft.DragDelta += new DragDeltaEventHandler(TopLeftDragDeltaEventHandler);
            _topLeft.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(PreviewMouseLeftButtonDownGenericHandler);
            _topLeft.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(PreviewMouseLeftButtonUpGenericHandler);

            _topRight.DragDelta += new DragDeltaEventHandler(TopRighttDragDeltaEventHandler);
            _topRight.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(PreviewMouseLeftButtonDownGenericHandler);
            _topRight.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(PreviewMouseLeftButtonUpGenericHandler);

            _bottomLeft.DragDelta += new DragDeltaEventHandler(BottomLeftDragDeltaEventHandler);
            _bottomLeft.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(PreviewMouseLeftButtonDownGenericHandler);
            _bottomLeft.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(PreviewMouseLeftButtonUpGenericHandler);

            _bottomRight.DragDelta += new DragDeltaEventHandler(BottomRightDragDeltaEventHandler);
            _bottomRight.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(PreviewMouseLeftButtonDownGenericHandler);
            _bottomRight.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(PreviewMouseLeftButtonUpGenericHandler);
        }
      
        private void BottomRightDragDeltaEventHandler(object sender, DragDeltaEventArgs e)
        {
            Point draggedPoint = new Point(Canvas.GetLeft(_bottomRight) + _thumbSize, Canvas.GetTop(_bottomRight) + _thumbSize);
            UpdatePolygonSize(UpdateRectangleSize(null, null, e.VerticalChange, e.HorizontalChange, draggedPoint));
        }

        private void BottomLeftDragDeltaEventHandler(object sender, DragDeltaEventArgs e)
        {
            Point draggedPoint = new Point(Canvas.GetLeft(_bottomLeft), Canvas.GetTop(_bottomLeft) + _thumbSize);
            UpdatePolygonSize(UpdateRectangleSize(null, null, e.VerticalChange, e.HorizontalChange, draggedPoint));
        }

        private void TopRighttDragDeltaEventHandler(object sender, DragDeltaEventArgs e)
        {
            Point draggedPoint = new Point(Canvas.GetLeft(_topRight) + _thumbSize, Canvas.GetTop(_topRight));
            UpdatePolygonSize(UpdateRectangleSize(null, null, e.VerticalChange, e.HorizontalChange, draggedPoint));
        }

        private void TopLeftDragDeltaEventHandler(object sender, DragDeltaEventArgs e)
        {
            Point draggedPoint = new Point(Canvas.GetLeft(_topLeft), Canvas.GetTop(_topLeft));
            UpdatePolygonSize(UpdateRectangleSize(null, null, e.VerticalChange, e.HorizontalChange, draggedPoint));
        }

        /// <summary>
        /// Update (redraw) thumbs positions
        /// </summary>
        public void UpdateThumbsPosition()
        {
            if (_polygonManager.Points.Count > 0)
            {
                var tl = _polygonManager.Points[0];
                var tr = _polygonManager.Points[1];
                var bl = _polygonManager.Points[2];
                var br = _polygonManager.Points[3];

                _topLeft.SetPosition(tl.X, tl.Y);
                _topRight.SetPosition(tr.X, tr.Y);
                _bottomLeft.SetPosition(bl.X, bl.Y);
                _bottomRight.SetPosition(br.X, br.Y);
            }
        }

        /// <summary>
        /// Manage thumbs visibility
        /// </summary>
        /// <param name="isVisble">Set current visibility</param>
        public void ShowThumbs(bool isVisible)
        {
            if (isVisible && _polygonManager.Points.Count > 0)
            {
                // Determine the conditions under which you want to show the thumbs for the polygon.
                // For example, you might base it on the number of points or other criteria related to the polygon's properties.

                // Implement your custom logic for showing thumbs based on the polygon properties.
                // Example:
                if (_polygonManager.Points.Count >= 3) // Show thumbs when the polygon has at least 3 points.
                {
                    _topLeft.Visibility = Visibility.Visible;
                    _topRight.Visibility = Visibility.Visible;
                    _bottomLeft.Visibility = Visibility.Visible;
                    _bottomRight.Visibility = Visibility.Visible;
                }
            }
            else
            {
                _topLeft.Visibility = Visibility.Hidden;
                _topRight.Visibility = Visibility.Hidden;
                _bottomLeft.Visibility = Visibility.Hidden;
                _bottomRight.Visibility = Visibility.Hidden;
            }
        }


        private PointCollection UpdateRectangleSize(double? left, double? top, double? height, double? width, Point draggedPoint)
        {
            double resultLeft = _polygonManager.Points.Min(p => p.X);
            double resultTop = _polygonManager.Points.Min(p => p.Y);

            if (left != null)
                resultLeft = (double)left;
            if (top != null)
                resultTop = (double)top;

            // Find the closest point to the dragged point
            double minDistance = double.MaxValue;
            int closestIndex = -1;
            for (int i = 0; i < _polygonManager.Points.Count; i++)
            {
                double distance = Math.Sqrt(Math.Pow(_polygonManager.Points[i].X - draggedPoint.X, 2) +
                                            Math.Pow(_polygonManager.Points[i].Y - draggedPoint.Y, 2));
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestIndex = i;
                }
            }

            var updatedPoints = new PointCollection(_polygonManager.Points);
            if (closestIndex != -1)
            {
                updatedPoints[closestIndex] = new Point(draggedPoint.X + width ?? 0, draggedPoint.Y + height ?? 0);
            }

            return updatedPoints;
        }

        private void UpdatePolygonSize(PointCollection points)
        {
            _polygonManager.Points = points;
            UpdateThumbsPosition();
        }


        private void PreviewMouseLeftButtonDownGenericHandler(object sender, MouseButtonEventArgs e)
        {
            ThumbCrop thumb = sender as ThumbCrop;
            thumb.CaptureMouse();
        }

        private void PreviewMouseLeftButtonUpGenericHandler(object sender, MouseButtonEventArgs e)
        {
            ThumbCrop thumb = sender as ThumbCrop;
            thumb.ReleaseMouseCapture();
        }
    }
}
