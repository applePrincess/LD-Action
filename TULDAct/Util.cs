using System;
namespace TULDAct
{
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
}
