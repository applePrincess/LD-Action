//	Form継承 2017/11/27 T.Umezawa

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
namespace TULDAct
{
    class MyForm : Form
    {
        protected System.Timers.Timer		mTimer = new System.Timers.Timer();
        protected SolidBrush	mSBWhite = new SolidBrush( Color.White );

        public static int[]		sKey = new int[ 0x100 ];
        public static int[]		sMouseB = new int[ 0x20 ];

        public static int MouseLeft{	get{	return( sMouseB[ (int)Math.Log( (int)MouseButtons.Left  , 2 ) ] );	}	}
        public static int MouseMiddle{	get{	return( sMouseB[ (int)Math.Log( (int)MouseButtons.Middle, 2 ) ] );	}	}
        public static int MouseRight{	get{	return( sMouseB[ (int)Math.Log( (int)MouseButtons.Right , 2 ) ] );	}	}

        protected override void OnKeyDown( KeyEventArgs e )
        {
            sKey[ (int)e.KeyCode ] = 1;
            base.OnKeyDown( e );
        }

        protected override void OnKeyUp( KeyEventArgs e )
        {
            sKey[ (int)e.KeyCode ] = 0;
            base.OnKeyUp( e );
        }

        protected override void OnMouseDown( MouseEventArgs e )
        {
            sMouseB[ (int)Math.Log( (int)e.Button, 2 ) ] = 1;
            base.OnMouseDown( e );
        }

        protected override void OnMouseUp( MouseEventArgs e )
        {
            sMouseB[ (int)Math.Log( (int)e.Button, 2 ) ] = 0;
            base.OnMouseUp( e );
        }

        protected override void OnLoad( EventArgs e )
        {
            ClientSize = new Size( 960, 720 );
            #if HAISHIN
            Left = 396; // キャプチャ都合上
            Top  = 20;  // キャプチャ都合上
            #endif

            DoubleBuffered = true;
            //		BackColor = System.Drawing.Color.FromArgb( 0x55, 0x88, 0xff );
            BackColor = Color.Black;

            mTimer.Elapsed += new System.Timers.ElapsedEventHandler( onMyTimer );
            mTimer.Interval = 33;

        }

        protected override void OnPaint( PaintEventArgs e )
        {
            base.OnPaint( e );

            Graphics	g = e.Graphics;
            g.ScaleTransform( 4, 4 );
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode   = PixelOffsetMode.Half;

            onMyPaint( g );
        }

        protected virtual void onMyPaint( Graphics g )
        {
        }

        protected virtual void onMyTimer( object sender, System.Timers.ElapsedEventArgs e )
        {
            for( int i = 0; i < sKey.Length; i++ ){
                if( sKey[ i ] > 0 ){
                    sKey[ i ]++;
                }
            }
            for( int i = 0; i < sMouseB.Length; i++ ){
                if( sMouseB[ i ] > 0 ){
                    sMouseB[ i ]++;
                }
            }
        }
    }
}
