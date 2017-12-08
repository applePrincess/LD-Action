//	Form継承 2017/11/27 T.Umezawa

using System;
using System.Collections.Generic;

class MyForm : System.Windows.Forms.Form
{
	protected System.Timers.Timer		mTimer = new System.Timers.Timer();
	protected System.Drawing.SolidBrush	mSBWhite = new System.Drawing.SolidBrush( System.Drawing.Color.White );

	public static int[]		sKey = new int[ 0x100 ];
	public static int[]		sMouseB = new int[ 0x20 ];

	public static int MouseLeft{	get{	return( sMouseB[ (int)Math.Log( (int)System.Windows.Forms.MouseButtons.Left  , 2 ) ] );	}	}
	public static int MouseMiddle{	get{	return( sMouseB[ (int)Math.Log( (int)System.Windows.Forms.MouseButtons.Middle, 2 ) ] );	}	}
	public static int MouseRight{	get{	return( sMouseB[ (int)Math.Log( (int)System.Windows.Forms.MouseButtons.Right , 2 ) ] );	}	}

	protected override void OnKeyDown( System.Windows.Forms.KeyEventArgs e )
	{
		sKey[ (int)e.KeyCode ] = 1;
		base.OnKeyDown( e );
	}

	protected override void OnKeyUp( System.Windows.Forms.KeyEventArgs e )
	{
		sKey[ (int)e.KeyCode ] = 0;
		base.OnKeyUp( e );
	}

	protected override void OnMouseDown( System.Windows.Forms.MouseEventArgs e )
	{
		sMouseB[ (int)Math.Log( (int)e.Button, 2 ) ] = 1;
		base.OnMouseDown( e );
	}

	protected override void OnMouseUp( System.Windows.Forms.MouseEventArgs e )
	{
		sMouseB[ (int)Math.Log( (int)e.Button, 2 ) ] = 0;
		base.OnMouseUp( e );
	}

	protected override void OnLoad( EventArgs e )
	{
		ClientSize = new System.Drawing.Size( 960, 720 );
#if HAISHIN
		Left = 396;	//	キャプチャ都合上
		Top = 20;	//	キャプチャ都合上
#endif

		DoubleBuffered = true;
//		BackColor = System.Drawing.Color.FromArgb( 0x55, 0x88, 0xff );
		BackColor = System.Drawing.Color.Black;

		mTimer.Elapsed += new System.Timers.ElapsedEventHandler( onMyTimer );
		mTimer.Interval = 33;

	}

	protected override void OnPaint( System.Windows.Forms.PaintEventArgs e )
	{
		base.OnPaint( e );

		System.Drawing.Graphics	g = e.Graphics;
		g.ScaleTransform( 4, 4 );
		g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
		g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

		onMyPaint( g );
	}

	protected virtual void onMyPaint( System.Drawing.Graphics g )
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
