using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinite_War
{
    enum Weapons
    {
        DAGGER, GUN, RPG, SWORD
    };
    public enum WeaponRareUpList
    {
        NONE, DAGGER, GUN, RPG, SWORD
    };
    public struct Ability
    {
        public int damage_up;
        public int dagger_up;
        public int gun_up;
        public int rpg_up;
        public int sword_up;
        public int prob_up;
        public int move_up;
        public int atkSpeed_up;
        public bool is_RareSelected;
        public WeaponRareUpList rare_up;
    };
    public static class GameData
    {
        public const int player_width = 64;
        public const int player_height = 64;

        public const int enemy_width = 64;
        public const int enemy_height = 64;

        public const int MAX_BULLET = 10000;
        public const int MAX_DAGGER = 10000;
        public const int MAX_RPG = 10000;

        public const float enemy_speed = 5.0f;
        public const int player_init_HP = 3;

        public const int bullet_width = 3;
        public const int bullet_height = 5;


        static public float dagger_cool;
        static public float gun_cool;
        static public float rpg_cool;
        static public float sword_cool;

        static public int stage;
        static public int kill;
        static public Ability ab;

        static private float player_speed;

        static public void init()
        {
            stage = 1;
            kill = 0;
            ab.damage_up = 0;
            ab.dagger_up = 0;
            ab.gun_up = 0;
            ab.rpg_up = 0;
            ab.sword_up = 0;
            ab.prob_up = 0;
            ab.move_up = 0;
            ab.atkSpeed_up = 0;
            ab.is_RareSelected = false;
            ab.rare_up = WeaponRareUpList.NONE;

            dagger_cool = 2.0f;
            gun_cool = 2.0f;
            rpg_cool = 2.0f;
            sword_cool = 1.0f;

            player_speed = 3.0f;
        }

        static public float getPlayerSpeed()
        {
            return player_speed;
        }
        static public void Upgrade_damage_up()
        {
            ab.damage_up += 1;
        }
        static public void Upgrade_dagger_up()
        {
            ab.dagger_up += 1;
        }
        static public void Upgrade_gun_up()
        {
            ab.gun_up += 1;
        }
        static public void Upgrade_rpg_up()
        {
            ab.rpg_up += 1;
        }
        static public void Upgrade_sword_up()
        {
            ab.sword_up += 1;
        }
        static public void Upgrade_prob_up()
        {
            ab.prob_up += 1;
        }
        static public void Upgrade_move_up()
        {
            player_speed *= 1.2f;
        }
        static public void Upgrade_atkSpeed_up()
        {
            ab.atkSpeed_up += 1;

            dagger_cool *= 0.9f;
            gun_cool *= 0.9f;
            rpg_cool *= 0.9f;
            sword_cool *= 0.9f;
        }
        static public void Upgrade_rare_up(int what)
        {
            ab.is_RareSelected = true;
            switch(what)
            {
                case 1:
                    ab.rare_up = WeaponRareUpList.DAGGER;
                    break;
                case 2:
                    ab.rare_up = WeaponRareUpList.GUN;
                    break;
                case 3:
                    ab.rare_up = WeaponRareUpList.RPG;
                    break;
                case 4:
                    ab.rare_up = WeaponRareUpList.SWORD;
                    break;
            }
        }




    }
    
}
