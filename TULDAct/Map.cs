using System;
using System.Drawing;

namespace TULDAct
{
    class Map
    {
        public static readonly int  WIDTH    = 31;
        public static readonly int  HEIGHT   = 21;
        public static readonly byte FLOOR    = 0;
        public static readonly byte FLAG     = 1;
        public static readonly byte BLOCK    = 2;
        public static readonly byte WALL     = 3;
        static SolidBrush           sSBFloor = new SolidBrush( Color.FromArgb( 0x22, 0x00, 0x00 ) );

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
            for( int y = 0; y < HEIGHT; y++ ){
                sMap[ y, 0 ] = WALL;
                sMap[ y, WIDTH - 1 ] = WALL;
            }
            for( int x = 0; x < WIDTH; x++ ){
                sMap[ 0, x ] = WALL;
                sMap[ HEIGHT - 1, x ] = WALL;
            }

            // 内側はブロックで埋め尽す。
            for( int y = 2; y < HEIGHT - 2; y += 2 ){
                for( int x = 2; x < WIDTH - 2; x += 2 ){
                    sMap[ y, x ] = BLOCK;
                }
            }

            //	迷路作成
            for( int y = 2; y < HEIGHT - 2; y += 2 ){
                for( int x = 2; x < WIDTH - 2; x += 2 ){
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
            for( int y = 0; y < HEIGHT; y++ ){
                for( int x = 0; x < WIDTH; x++ ){
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
            return( x >= 0 && x < WIDTH && y >= 0 && y < HEIGHT );
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
}
