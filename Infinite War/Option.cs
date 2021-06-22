using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinite_War
{
    public static class Option //옵션 클래스
    {
        private static bool m_BGM_sound { get; set; } //배경음악
        private static bool m_Effect_sound { get; set; } //효과음

        static Option() //initialize
        {
            m_BGM_sound = true;
            m_Effect_sound = true;
        }

        public static bool GetBgmSound() //현재 배경음악 정보
        {
            return m_BGM_sound;
        }
        public static bool GetEffSound() //현재 효과음 정보
        {
            return m_Effect_sound;
        }

        public static void TurnBgm()    //배경음악 전환
        {
            m_BGM_sound = !m_BGM_sound;
        }

        public static void TurnEff()    //효과음 전환
        {
            m_Effect_sound = !m_Effect_sound;
        }

    }
}
