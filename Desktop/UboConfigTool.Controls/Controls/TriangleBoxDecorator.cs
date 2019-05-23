using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace UboConfigTool.Controls
{
    public class TriangleBoxDecorator : FrameworkElement
    {
        public TriangleBoxDecorator()
        {
            
        }

        public Color BGFillColor 
        { 
            get 
            {
                return (Color)GetValue(BGFillColorProperty);
            }
            set
            {
                SetValue(BGFillColorProperty, value);
            } 
        }

        public static readonly DependencyProperty BGFillColorProperty =
        DependencyProperty.Register("BGFillColor", typeof(Color), typeof(TriangleBoxDecorator), 
            new FrameworkPropertyMetadata(Colors.White, BGFillColorPropertyChangedCallback));

        private static void BGFillColorPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender != null && sender is TriangleBoxDecorator)
            {
                (sender as TriangleBoxDecorator).InvalidateVisual();
            }
        }

        //public static readonly DependencyProperty IsClosedProperty =
        //DependencyProperty.Register("IsClosed", typeof(bool), typeof(ArrowLineDecorator),
        //       new FrameworkPropertyMetadata(false, IsClosedPropertyChangedCallback));

        protected override void OnRender( DrawingContext dc )
        {
            PointCollection pnts = new PointCollection();
            pnts.Add( new Point(0, 0));
            pnts.Add( new Point(RenderSize.Width, 0));
            pnts.Add( new Point(RenderSize.Width, RenderSize.Height));
            pnts.Add( new Point(0, 0 ));


            StreamGeometry geom = new StreamGeometry();
			Rect containerRect = new Rect(RenderSize);
			using (StreamGeometryContext context = geom.Open())
			{
				context.BeginFigure(pnts[0], true, true);
                context.PolyLineTo( pnts, true, true );
			}

            dc.DrawGeometry(new SolidColorBrush(BGFillColor), new Pen(new SolidColorBrush(Colors.Transparent), 1), geom);
        }
    }
}
