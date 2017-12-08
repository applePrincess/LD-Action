using System;
using System.Collections.Generic;
using System.Drawing;

namespace TULDAct
{
    class Enemy : Unit
    {
        public static List<Enemy>			sList;

        static RectangleF	sRectD = new RectangleF( 0, 0, 8, 8 );
        static RectangleF	sRectS = new RectangleF( 0, 0, 8, 8 );

        static Bitmap[]	sBM = {
            new Bitmap( "monster.png" ),
            new Bitmap( "player2.png" ),
            new Bitmap( "monster2.png" ),
        };

        public static int[]		sSpeed = { 8, 32, 16 };

        public Enemy( int type, int lv ) : base( LDAct8.sRnd.Next( Map.WIDTH / 2 ) * 512 + 256 + 128, LDAct8.sRnd.Next( 4 ) * 512 + 256 + 128, 0, 0 )
        {
            mType = type;
            mLV = lv;
            mHP = mMHP = ( type + 1 ) * ( mLV + 1 ) * 5;
        }

        public void draw( Graphics g )
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

            g.DrawImage( sBM[ mType ], sRectD, sRectS, GraphicsUnit.Pixel );
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
}
