using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinite_War
{
    public static class Option
    {
        private static bool m_BGM_sound { get; set; }
        private static bool m_Effect_sound { get; set; }

        static Option()
        {
            m_BGM_sound = true;
            m_Effect_sound = true;
        }

        public static bool GetBgmSound()
        {
            return m_BGM_sound;
        }
        public static bool GetEffSound()
        {
            return m_Effect_sound;
        }

        public static void TurnBgm()
        {
            m_BGM_sound = !m_BGM_sound;
        }

        public static void TurnEff()
        {
            m_Effect_sound = !m_Effect_sound;
        }

    }
}
