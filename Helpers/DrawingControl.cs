using System;
using System.Windows;
using System.Windows.Media;

namespace Helpers
{
    public class DrawingControl : FrameworkElement
    {
        private readonly DrawingVisual _visual;
        private readonly VisualCollection _visuals;

        public DrawingControl()
        {
            _visual = new DrawingVisual();
            _visuals = new VisualCollection(this) {_visual};
        }

        protected override int VisualChildrenCount => _visuals.Count;

        public DrawingContext GetContext()
        {
            return _visual.RenderOpen();
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= _visuals.Count)
                throw new ArgumentOutOfRangeException();
            return _visuals[index];
        }
    }
}
