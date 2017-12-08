using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
namespace TULDAct
{
    class Player : Unit
    {
        public static List<Player>   sList = new List<Player>();

        static Bitmap[] sBM = {
            new Bitmap( "player.png" ),
        };
        static Rectangle sRect = new Rectangle( 2, 2, 8, 8 );

        public int  mBtnA;
        public int  mBtnB;
        public int  mItem;

        public string mName = "Player1";

        public int AX{ get{
                if( mAngle == 1 ) return( -1 );
                if( mAngle == 2 ) return(  1 );
                return( 0 );
            } }

        public int AY{ get{
                if( mAngle == 0 ) return(  1 );
                if( mAngle == 3 ) return( -1 );
                return( 0 );
            } }

        public Player() : base( 256 * 3 + 128, ( Map.sMap.GetLength( 0 ) - 4 ) * 256 + 128, 0, 0 )
        {
            mHP = mMHP = 30;
        }

        public void draw( Graphics g )
        {
            sRect.X = ( (int)( ( mX + mY ) * 4 / 256 ) & 1 ) * ( DOT + DOT / 2 ) + 2;
            sRect.Y = mAngle * ( DOT + DOT / 2 ) + 2;
            g.DrawImage( sBM[ 0 ], mX * DOT / 256.0f  - DTH, mY * DOT / 256.0f - DTH, sRect, GraphicsUnit.Pixel );
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
                if( MyForm.sKey[ (int)Keys.Down  ] > 0 ){ mAngle = 0; mDY = 32; }else
                if( MyForm.sKey[ (int)Keys.Left  ] > 0 ){ mAngle = 1; mDX = -32; }else
                if( MyForm.sKey[ (int)Keys.Right ] > 0 ){ mAngle = 2; mDX = 32; }else
                if( MyForm.sKey[ (int)Keys.Up    ] > 0 ){ mAngle = 3; mDY = -32; }
                if( MyForm.sKey[ (int)Keys.Z     ] == 1 ){
                    int  x = mX + AX * 0x100;
                    int  y = mY + AY * 0x100;
                    int  m = Map.Get( x, y );
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
}
