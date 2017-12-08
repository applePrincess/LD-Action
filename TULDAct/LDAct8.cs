//	#15 見下ろし型アクション８ LD Action8 2017/12/08 T.Umezawa

using System;
using System.Collections.Generic;

namespace TULDAct
{
    class LDAct8 : MyForm
    {
        public static Random  sRnd = new Random();

        System.Drawing.Font mFont = new System.Drawing.Font( "MS P Gothic", 5 );
        public static int  sCount;
        public static bool sGameOver;
        public static int  sGameClear;
        public static int  sStage;
        int                mScene;

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
            if( sKey[ (int)System.Windows.Forms.Keys.R ] == 1 ) input( 1, true  );
            if( sKey[ (int)System.Windows.Forms.Keys.Z ] >  0 ) input( 1, false );

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
                    Enemy en = Enemy.sList[ j ];
                    if( pl.isCollision( en ) ){ //	敵に接触
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
                Enemy en = Enemy.sList[ i ];
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
}
