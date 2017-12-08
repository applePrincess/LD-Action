//	#15 見下ろし型アクション８ LD Action8 2017/12/08 T.Umezawa

using System;
using System.Collections.Generic;


class Util
{
	public static int GetAngle4i( double dx, double dy )
	{
		return( (int)( ( GetAngle360( dx, dy ) + 45 ) / 90 ) & 0x03 );
	}

	public static double GetAngle360( double dx, double dy )
	{
		double	r = Math.Atan2( dy, dx ) * 180 / Math.PI;
		if( r < 0 ){
			r += 360;
		}
		return( r );
	}
}


class Map
{
	public static readonly int	WIDTH = 31;
	public static readonly int	HEIGHT = 21;
	public static readonly byte	FLOOR  = 0;
	public static readonly byte	FLAG   = 1;
	public static readonly byte	BLOCK  = 2;
	public static readonly byte	WALL   = 3;
	static System.Drawing.SolidBrush	sSBFloor = new System.Drawing.SolidBrush( System.Drawing.Color.FromArgb( 0x22, 0x00, 0x00 ) );

	static System.Drawing.Bitmap[]	sBM = {
		new System.Drawing.Bitmap( "flag.png" ),
		new System.Drawing.Bitmap( "block.png" ),
		new System.Drawing.Bitmap( "wall.png" ),
	};

	public static byte[,]		sMap = new byte[ HEIGHT, WIDTH ];

	public static int CountNeighbor( int mx, int my, byte v )
	{
		int		r = 0;
		if( sMap[ my - 1, mx ] == v )	r++;
		if( sMap[ my + 1, mx ] == v )	r++;
		if( sMap[ my, mx - 1 ] == v )	r++;
		if( sMap[ my, mx + 1 ] == v )	r++;
		return( r );
	}

	public static void create()
	{
		Array.Clear( sMap, 0, sMap.Length );
		for( int y = 0; y < sMap.GetLength( 0 ); y++ ){
			sMap[ y, 0 ] = WALL;
			sMap[ y, sMap.GetLength( 1 ) - 1 ] = WALL;
		}
		for( int x = 0; x < sMap.GetLength( 1 ); x++ ){
			sMap[ 0, x ] = WALL;
			sMap[ sMap.GetLength( 0 ) - 1, x ] = WALL;
		}

		for( int y = 2; y < sMap.GetLength( 0 ) - 2; y += 2 ){
			for( int x = 2; x < sMap.GetLength( 1 ) - 2; x += 2 ){
				sMap[ y, x ] = BLOCK;
			}
		}

		//	迷路作成
		for( int y = 2; y < sMap.GetLength( 0 ) - 2; y += 2 ){
			for( int x = 2; x < sMap.GetLength( 1 ) - 2; x += 2 ){
				if( isWall( x, y ) ){
					continue;
				}
				int		a = LDAct8.sRnd.Next( 4 );
				if( a == 0 )	sMap[ y - 1, x ] = BLOCK;
				if( a == 1 )	sMap[ y + 1, x ] = BLOCK;
				if( a == 2 )	sMap[ y, x - 1 ] = BLOCK;
				if( a == 3 )	sMap[ y, x + 1 ] = BLOCK;
			}
		}

		sMap[ 3, 27 ] = FLAG;
	}

	public static int Get( int x, int y )
	{
		return( sMap[ (int)( y / 256 ), (int)( x / 256 ) ] );
	}

	public static void Set( int x, int y, byte v )
	{
		sMap[ (int)( y / 256 ), (int)( x / 256 ) ] = v;
	}

	public static bool IsCenter( int x, int y )
	{
		return( ( x & 0xff ) == 0x80 && ( y & 0xff ) == 0x80 );
	}

	static bool isWall( int x, int y )
	{
		return( sMap[ y - 1, x ] != 0 ||
		        sMap[ y + 1, x ] != 0 ||
		        sMap[ y, x - 1 ] != 0 ||
		        sMap[ y, x + 1 ] != 0 );
	}

	public static void draw( System.Drawing.Graphics g )
	{
		for( int y = 0; y < sMap.GetLength( 0 ); y++ ){
			for( int x = 0; x < sMap.GetLength( 1 ); x++ ){
				if( sMap[ y, x ] == 0 ){
					g.FillRectangle( sSBFloor, x * 8, y * 8, 7, 7 );
				}else{
					g.DrawImage( sBM[ sMap[ y, x ] - 1 ], x * 8, y * 8 );
				}
			}
		}
	}

	public static bool IsArea( int x, int y )
	{
		return( x >= 0 && x < sMap.GetLength( 1 ) &&
		        y >= 0 && y < sMap.GetLength( 0 ) );
	}

	public static bool IsBlock( int x, int y )
	{
		x -= 128;
		y -= 128;
		int		x0 = x / 256;
		int		y0 = y / 256;
		int		x1 = ( x + 255 ) / 256;
		int		y1 = ( y + 255 ) / 256;
		return( IsBlockM( x0, y0 ) ||
		        IsBlockM( x1, y0 ) ||
		        IsBlockM( x0, y1 ) ||
		        IsBlockM( x1, y1 ) );
	}

	public static bool IsBlockM( int x, int y )
	{
		return( !IsArea( x, y ) || sMap[ y, x ] == BLOCK || sMap[ y, x ] == WALL );
	}
}


class Unit
{
	protected static System.Drawing.Font			sFont = new System.Drawing.Font( "MS P Gothic", 5 );
	protected static System.Drawing.SolidBrush	sSBWhite = new System.Drawing.SolidBrush( System.Drawing.Color.White );

	public static readonly int		DOT = 8;
	public static readonly int		DTH = DOT / 2;

	public int		mX, mY;
	public int		mDX, mDY;
	public int		mHP;
	public int		mMHP;
	public int		mLV = 1;
	public int		mEXP;
	public int		mType;
	public int		mAngle;

	public int EXP{
		get{ return( mEXP ); }
		set{
			mEXP = value;
			if( mLV * ( mLV + 1 ) * 8 <= mEXP ){
				mLV++;
				mMHP += 5;
			}
		}
	}

	public Unit( int x, int y, int dx, int dy )
	{
		mX = x;
		mY = y;
		mDX = dx;
		mDY = dy;
	}

	public virtual void step()
	{
		if( mHP < mMHP &&
		    ( ( ( LDAct8.sCount & 0x3f ) == 0 ) ||
		      Map.CountNeighbor( mX / 256, mY / 256, 0 ) <= 1 ) ){
			mHP++;
		}

		if( ( mX & 0xff ) == 0x80 && ( mY & 0xff ) == 0x80 &&
			Map.IsBlockM( mX / 256 + Math.Sign( mDX ), 
			              mY / 256 + Math.Sign( mDY ) ) ){
			mDX = 0;
			mDY = 0;
		}

		mX += mDX;
		mY += mDY;
	}

	public bool isCollision( Unit u )
	{
		return( Math.Abs( mX - u.mX ) < 6.0f * 256 / DOT && Math.Abs( mY - u.mY ) < 6.0f * 256 / DOT );
	}
}


class Player : Unit
{
	public static List<Player>			sList = new List<Player>();

	static System.Drawing.Bitmap[]	sBM = {
		new System.Drawing.Bitmap( "player.png" ),
	};
	static System.Drawing.Rectangle	sRect = new System.Drawing.Rectangle( 2, 2, 8, 8 );

	public int		mBtnA;
	public int		mBtnB;
	public int		mItem;

	public string	mName = "Player1";

	public int AX{	get{
		if( mAngle == 1 )	return( -1 );
		if( mAngle == 2 )	return(  1 );
		return( 0 );
	}	}

	public int AY{	get{
		if( mAngle == 0 )	return(  1 );
		if( mAngle == 3 )	return( -1 );
		return( 0 );
	}	}

	public Player() : base( 256 * 3 + 128, ( Map.sMap.GetLength( 0 ) - 4 ) * 256 + 128, 0, 0 )
	{
		mHP = mMHP = 30;
	}

	public void draw( System.Drawing.Graphics g )
	{
		sRect.X = ( (int)( ( mX + mY ) * 4 / 256 ) & 1 ) * ( DOT + DOT / 2 ) + 2;
		sRect.Y = mAngle * ( DOT + DOT / 2 ) + 2;
		g.DrawImage( sBM[ 0 ], mX * DOT / 256.0f  - DTH, mY * DOT / 256.0f - DTH, sRect, System.Drawing.GraphicsUnit.Pixel );
		g.DrawString( "" + mHP, sFont, sSBWhite, mX * DOT / 256.0f + DTH, mY * DOT / 256.0f - DTH );
		g.DrawString( "" + mLV, sFont, sSBWhite, mX * DOT / 256.0f - DTH * 2.5f, mY * DOT / 256.0f - DTH );

		g.DrawString( mName, sFont, sSBWhite, 8, 170 );
		g.DrawString( "Lv " + mLV             , sFont, sSBWhite, 40, 170 );
		g.DrawString( "HP " + mHP + "/" + mMHP, sFont, sSBWhite, 60, 170 );
		g.DrawString( "土:" + mItem           , sFont, sSBWhite, 100, 170 );
	}

	public int getMap()
	{
		return( Map.Get( mX, mY ) );
	}

	public void start()
	{
		mHP = mMHP;
		mX = 256 * 3 + 128;
		mY = ( Map.sMap.GetLength( 0 ) - 4 ) * 256 + 128;
		mDX = 0;
		mDY = 0;
	}

	public override void step()
	{
		if( Map.IsCenter( mX, mY ) ){
			mDX = 0;
			mDY = 0;
		}
		if( mDX == 0 && mDY == 0 ){
			if( MyForm.sKey[ (int)System.Windows.Forms.Keys.Down  ] > 0 ){	mAngle = 0;	mDY = 32;	}else
			if( MyForm.sKey[ (int)System.Windows.Forms.Keys.Left  ] > 0 ){	mAngle = 1;	mDX = -32;	}else
			if( MyForm.sKey[ (int)System.Windows.Forms.Keys.Right ] > 0 ){	mAngle = 2;	mDX = 32;	}else
			if( MyForm.sKey[ (int)System.Windows.Forms.Keys.Up    ] > 0 ){	mAngle = 3;	mDY = -32;	}
			if( MyForm.sKey[ (int)System.Windows.Forms.Keys.Z     ] == 1 ){
				int		x = mX + AX * 0x100;
				int		y = mY + AY * 0x100;
				int		m = Map.Get( x, y );
				if( m == Map.BLOCK ){
					Map.Set( x, y, Map.FLOOR );
					mItem++;
				}
				if( m == Map.FLOOR && mItem > 0 ){
					Map.Set( x, y, Map.BLOCK );
					mItem--;
				}
			}
		}

		base.step();

		if( Map.sMap[ (int)( mY / 256 ), (int)( mX / 256 ) ] == Map.FLAG ){
			EXP += LDAct8.sStage * 10;
			LDAct8.sGameClear = 3 * 30;
		}
	}
}


class Enemy : Unit
{
	public static List<Enemy>			sList;

	static System.Drawing.RectangleF	sRectD = new System.Drawing.RectangleF( 0, 0, 8, 8 );
	static System.Drawing.RectangleF	sRectS = new System.Drawing.RectangleF( 0, 0, 8, 8 );

	static System.Drawing.Bitmap[]	sBM = {
		new System.Drawing.Bitmap( "monster.png" ),
		new System.Drawing.Bitmap( "player2.png" ),
		new System.Drawing.Bitmap( "monster2.png" ),
	};

	public static int[]		sSpeed = { 8, 32, 16 };

	public Enemy( int type, int lv ) : base( LDAct8.sRnd.Next( Map.WIDTH / 2 ) * 512 + 256 + 128, LDAct8.sRnd.Next( 4 ) * 512 + 256 + 128, 0, 0 )
	{
		mType = type;
		mLV = lv;
		mHP = mMHP = ( type + 1 ) * ( mLV + 1 ) * 5;
	}

	public void draw( System.Drawing.Graphics g )
	{
		if( mType == 1 ){	//	ミラーナイト
			sRectS.X = ( (int)( ( mX + mY ) * 4 / 256 ) & 1 ) * ( DOT + DOT / 2 ) + 2;
			sRectS.Y = mAngle * ( DOT + DOT / 2 ) + 2;
			sRectD.X = mX * DOT / 256.0f - DTH;
			sRectD.Width = 8;
		}else{
			sRectS.X = 0;
			sRectS.Y = 0;
			if( mDX > 0 ){
				sRectD.X = mX * DOT / 256.0f + DTH;
				sRectD.Width = -8;
			}else{
				sRectD.X = mX * DOT / 256.0f - DTH;
				sRectD.Width = 8;
			}
			if( mDY != 0 ){
				sRectS.Y = ( 3 - Math.Sign( mDY ) ) / 2 * DOT;
			}else{
				sRectS.Y = 0;
			}
		}

		sRectD.Y = mY * DOT / 256.0f - DTH;

		g.DrawImage( sBM[ mType ], sRectD, sRectS, System.Drawing.GraphicsUnit.Pixel );
		g.DrawString( "" + mHP, sFont, sSBWhite, mX * DOT / 256.0f + DTH, mY * DOT / 256.0f - DTH );
		g.DrawString( "" + mLV, sFont, sSBWhite, mX * DOT / 256.0f - DTH * 2.5f, mY * DOT / 256.0f - DTH );
	}

	public void step( List<Enemy> le )
	{
		step();

		if( ( mDX == 0 && mDY == 0 ) ||
		    ( Map.CountNeighbor( mX / 256, mY / 256, 0 ) > 2 &&
		      Map.IsCenter( mX, mY ) ) ){
			int		s = sSpeed[ mType ];
			mDX = LDAct8.sRnd.Next( 3 ) * s - s;
			if( mDX == 0 ){
				mDY = LDAct8.sRnd.Next( 3 ) * s - s;
			}else{
				mDY = 0;
			}

			if( mDY > 0 )	mAngle = 0;
			if( mDX < 0 )	mAngle = 1;
			if( mDX > 0 )	mAngle = 2;
			if( mDY < 0 )	mAngle = 3;
		}
	}
}


class LDAct8 : MyForm
{
	public static Random		sRnd = new Random();

	System.Drawing.Font			mFont = new System.Drawing.Font( "MS P Gothic", 5 );
	public static int			sCount;
	public static bool			sGameOver;
	public static int			sGameClear;
	public static int			sStage;
	int							mScene;

	protected override void OnLoad( EventArgs e )
	{
		base.OnLoad( e );
		mTimer.Start();
	}

	protected override void onMyPaint( System.Drawing.Graphics g )
	{
		if( mScene == 0 ){
			g.DrawString( "見下ろし型アクション８ LD Action8", mFont, mSBWhite, 60, 30 );
			g.DrawString( "PRESS Z KEY", mFont, mSBWhite, 90, 90 );
			return;
		}

		g.TranslateTransform( -4, 0 );
		Map.draw( g );
		foreach( Player pl in Player.sList ){
			pl.draw( g );
		}
		foreach( Enemy en in Enemy.sList ){
			en.draw( g );
		}
		g.TranslateTransform( 4, 0 );

		g.DrawString( "TIME "  + sCount, mFont, mSBWhite, 200, 170 );
		g.DrawString( "STAGE " + sStage, mFont, mSBWhite, 168, 170 );

		if( sGameClear > 0){
			g.DrawString( "STAGE CLEAR!", mFont, mSBWhite, 90, 90 );
		}

		if( sGameOver ){
			g.DrawString( "GAME OVER", mFont, mSBWhite, 90, 90 );
		}
	}

	protected override void onMyTimer( object sender, System.Timers.ElapsedEventArgs e )
	{
		if( sKey[ (int)System.Windows.Forms.Keys.R ] == 1 )	input( 1, true  );
		if( sKey[ (int)System.Windows.Forms.Keys.Z ] >  0 )	input( 1, false );

		if( Player.sList.Count > 0 ){
			Player.sList[ 0 ].mBtnA = sKey[ (int)System.Windows.Forms.Keys.Z     ];
			Player.sList[ 0 ].mBtnB = sKey[ (int)System.Windows.Forms.Keys.X     ];
		}

		if( sGameClear > 0 ){
			if( --sGameClear == 1 ){
				nextStage();
			}
			return;
		}

		if( sGameOver ){
			return;
		}

		sCount++;

		for( int i = Player.sList.Count - 1; i >= 0; i-- ){
			Player pl = Player.sList[ i ];
			pl.step();
			for( int j = Enemy.sList.Count - 1; j >= 0; j-- ){
				Enemy	en = Enemy.sList[ j ];
				if( pl.isCollision( en ) ){	//	敵に接触
					pl.mHP -= en.mLV;
					en.mHP -= pl.mLV;
					pl.EXP += en.mLV;
					en.EXP += pl.mLV;
					if( pl.mHP <= 0 ){
						sGameOver = true;
					}
					if( en.mHP <= 0 ){
						Enemy.sList.RemoveAt( j );
					}
				}
			}
		}

		for( int i = Enemy.sList.Count - 1; i >= 0; i-- ){
			Enemy	en = Enemy.sList[ i ];
			en.step( Enemy.sList );
		}

		base.onMyTimer( sender, e );

		Invalidate();
	}

	void nextStage()
	{
		sStage++;
		start();
		Player.sList[ 0 ].start();
	}

	void input( int type, bool res )
	{
		if( mScene == 0 ){
			sStage = 1;
			start();
			Player.sList.Add( new Player() );
		}else if( sGameClear == 1 ){
			nextStage();
		}else if( res ){
			sStage = 1;
			start();
			Player.sList.Clear();
			Player.sList.Add( new Player() );
		}
	}

	void start()
	{
		mScene = 1;
		sGameClear = 0;
		sGameOver = false;
		sCount = 0;
		Enemy.sList = new List<Enemy>();
		for( int i = 0; i < sStage; i++ ){
			Enemy.sList.Add( new Enemy( 0, sRnd.Next( sStage ) + 1 ) );
			Enemy.sList.Add( new Enemy( 0, sRnd.Next( sStage ) + 1 ) );
			Enemy.sList.Add( new Enemy( 0, sRnd.Next( sStage ) + 1 ) );
			Enemy.sList.Add( new Enemy( 0, sRnd.Next( sStage ) + 1 ) );
			Enemy.sList.Add( new Enemy( 1, sRnd.Next( sStage ) + 1 ) );
			Enemy.sList.Add( new Enemy( 1, sRnd.Next( sStage ) + 1 ) );
			Enemy.sList.Add( new Enemy( 2, sRnd.Next( sStage ) + 1 ) );
		}

		Map.create();
	}

	[STAThread]
	static void Main()
	{
		System.Windows.Forms.Application.Run( new LDAct8() );
	}
}
