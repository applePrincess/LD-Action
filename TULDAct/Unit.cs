using System;
using System.Drawing;

namespace TULDAct
{
    class Unit
    {
        protected static Font       sFont    = new Font( "MS P Gothic", 5 );
        protected static SolidBrush sSBWhite = new SolidBrush( Color.White );

        public static readonly int DOT = 8;
        public static readonly int DTH = DOT / 2;

        public int mX, mY;
        public int mDX, mDY;
        public int mHP;
        public int mMHP;
        public int mLV = 1;
        public int mEXP;
        public int mType;
        public int mAngle;

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
}
