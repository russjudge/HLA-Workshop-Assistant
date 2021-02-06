using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace HLA_Workshop_Assistant
{
    public class SortAdorner : Adorner
    {
        private readonly static Geometry _AscGeometry =
              Geometry.Parse("M 0,0 L 10,0 L 5,5 Z");

        private readonly static Geometry _DescGeometry =
            Geometry.Parse("M 0,5 L 10,5 L 5,0 Z");

        public ListSortDirection Direction { get; private set; }

        public SortAdorner(UIElement element, ListSortDirection direction)
            : base(element)
        {
            Initialize(direction, Brushes.LightSteelBlue);
        }
        public SortAdorner(UIElement element, ListSortDirection direction, Brush brushColor)
            : base(element)
        {
            Initialize(direction, brushColor);
        }
        void Initialize(ListSortDirection direction, Brush brushColor)
        {
            Direction = direction;
            BrushColor = brushColor;
        }
        public Brush BrushColor { get; set; }

        protected override void OnRender(DrawingContext drawingContext)
        {

            base.OnRender(drawingContext);

            if (AdornedElement.RenderSize.Width < 20)
                return;
            if (drawingContext != null)
            {
                drawingContext.PushTransform(
                     new TranslateTransform(
                       AdornedElement.RenderSize.Width - 15,
                      (AdornedElement.RenderSize.Height - 5) / 2));

                drawingContext.DrawGeometry(BrushColor, null,
                    Direction == ListSortDirection.Ascending ?
                      _AscGeometry : _DescGeometry);

                drawingContext.Pop();
            }

        }

    }
}
