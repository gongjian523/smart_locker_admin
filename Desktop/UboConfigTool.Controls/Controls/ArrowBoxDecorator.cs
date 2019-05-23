using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UboConfigTool.Controls
{
    public class ArrowBoxDecorator : FrameworkElement
    {
        public ArrowBoxDecorator()
        {
            Foreground = new SolidColorBrush( Colors.LightGray );
        }

        private Brush   i_Brush;
        public Brush Foreground
        {
            get
            {
                return i_Brush;
            }
            set
            {
                if( value != null )
                {
                    i_Brush = value;
                }
            }
        }

        protected override void OnRender( DrawingContext dc )
        {
            PointCollection pnts = new PointCollection();
            pnts.Add( new Point( 0, 16 ) );
            pnts.Add( new Point( 60, 16 ) );
            pnts.Add( new Point( 76, 0 ) );
            pnts.Add( new Point( 92, 16 ) );
            pnts.Add( new Point( RenderSize.Width, 16 ) );
            pnts.Add( new Point( RenderSize.Width, RenderSize.Height ) );
            pnts.Add( new Point( 0, RenderSize.Height ) );
            pnts.Add( new Point( 0, 16 ) );

            StreamGeometry geom = new StreamGeometry();
			Rect containerRect = new Rect(RenderSize);
			using (StreamGeometryContext context = geom.Open())
			{
				context.BeginFigure(pnts[0], true, true);
                context.PolyLineTo( pnts, true, true );
			}

            dc.DrawGeometry( new SolidColorBrush( Colors.White ), new Pen( Foreground, 1 ), geom );
        }

    }
}
