using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Infinite_War
{
    public struct o_Point
    {
        public float x;
        public float y;
        public o_Point(float _x, float _y)
        {
            x = _x;
            y = _y;
        }
    }
    public struct o_Size
    {
        public int width;
        public int height;
    }
    public struct o_Vector
    {
        public float x;
        public float y;

        public o_Vector(float _x, float _y)
        {
            x = _x;
            y = _y;
        }
        public float SizeOfVector()
        {
            float result;
            result = (float)Math.Sqrt(x * x + y * y);
            return result;
        }
        public void normalize()
        {
            float vectorSize = SizeOfVector();
            x = x / vectorSize;
            y = y / vectorSize;
        }
    }
    public interface GameMath
    {
        //static public TimeSpan deltatime = TimeSpan.FromSeconds(1.0 / 60.0);
        //static public double dt = deltatime.TotalSeconds;
        static public double dt = TimeSpan.FromSeconds(1.0 / 60.0).TotalSeconds;
        public static double dotProduct(float player_x, float player_y, float mouse_x, float mouse_y)
        {
            o_Point player;
            o_Point mouse;
            o_Vector vector1;
            o_Vector vector2;
            double cos;
            player.x = player_x;
            player.y = player_y;
            mouse.x = mouse_x;
            mouse.y = mouse_y;

            vector1.x = 0;
            vector1.y = 1;
            vector2.x = mouse_x - player_x;
            vector2.y = mouse_y - player_y;

            cos = vector2.y / vector2.SizeOfVector();

            double result = Math.Acos(cos);
            Console.WriteLine("angle : " + result);
            return result;
        }
        public static double GetAngle(float from_x, float from_y, float to_x, float to_y)
        {
            double result;
            result = Math.Atan2(from_y - to_y, from_x - to_x) * ( 180.0 / Math.PI);
            //Console.WriteLine("angle : " + result);
            return result;
        }
        public static bool CheckInside(o_Point position)
        {
            if (position.x < 0 || position.x > GameData.FormSize_Width)
                return false;
            if (position.y < 0 || position.y > GameData.FormSize_Height)
                return false;
            return true;
        }
        public static double getDistance(o_Point from, o_Point to)
        {
            double result;
            result = (double)Math.Sqrt((Math.Abs((from.x - to.x) * (from.x - to.x)) + (Math.Abs((from.y - to.y) * (from.y - to.y)))));
            return result;
        }

    }
}
