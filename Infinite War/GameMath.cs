using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Infinite_War
{
    public struct o_Point   //위치 구조체
    {
        public float x;
        public float y;
        public o_Point(float _x, float _y)  //초기화
        {
            x = _x;
            y = _y;
        }
    }
    public struct o_Size    //크기 구조체
    {
        public int width;
        public int height;
    }
    public struct o_Vector  //방향 구조체
    {
        public float x;
        public float y;

        public o_Vector(float _x, float _y) //초기화
        {
            x = _x;
            y = _y;
        }
        public float SizeOfVector()         //벡터의 크기
        {
            float result;
            result = (float)Math.Sqrt(x * x + y * y);
            return result;
        }
        public void normalize()             //벡터의 정규화
        {
            float vectorSize = SizeOfVector();
            x /= vectorSize;
            y /= vectorSize;
        }
    }
    public interface GameMath               //게임에서 쓰는 수학들
    {
        static public double dt = TimeSpan.FromSeconds(1.0 / 60.0).TotalSeconds;    //delta time
        public static double dotProduct(float player_x, float player_y, float mouse_x, float mouse_y)   //dot product
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
            return result;
        }
        public static double GetAngle(float from_x, float from_y, float to_x, float to_y)   //from, to에 대한 각도 구하는 함수
        {
            double result;
            result = Math.Atan2(from_y - to_y, from_x - to_x) * ( 180.0 / Math.PI);
            return result;
        }
        public static bool CheckInside(o_Point position)                            //범위 내에 있는지 체크하는 함수
        {
            if (position.x < 0 || position.x > GameData.FormSize_Width)
                return false;
            if (position.y < 0 || position.y > GameData.FormSize_Height)
                return false;
            return true;
        }
        public static double getDistance(o_Point from, o_Point to)                  //from에서 to까지 거리를 구하는 함수
        {
            double result;
            result = (double)Math.Sqrt((Math.Abs((from.x - to.x) * (from.x - to.x)) + (Math.Abs((from.y - to.y) * (from.y - to.y)))));
            return result;
        }

    }
}
