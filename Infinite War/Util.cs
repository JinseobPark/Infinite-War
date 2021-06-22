using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Infinite_War
{
    public static class Untility    //제네릭을 위한 유틸.
    {
        public static T[] Init_array<T>(int length) where T : class, new()  //class를 new로 제네릭을 통해 초기화.
        {
            T[] array = new T[length];                                      //array로 접근하기 위해 new를 생성하여 전달
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new T();
            }

            return array;
        }
    }
}
