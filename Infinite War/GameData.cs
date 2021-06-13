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
        public const int FormSize_Width = 1200;
        public const int FormSize_Height = 800;

        public const int player_width = 64;
        public const int player_height = 64;

        public const int enemy_width = 64;
        public const int enemy_height = 64;

        public const int MAX_DAGGER = 30;
        public const int MAX_BULLET = 30;
        public const int MAX_RPG = 20;
        public const int MAX_SWORD = 10;

        public const int MAX_ENEMY_NORMAL = 30;
        public const int MAX_ENEMY_SPEED  = 20;
        public const int MAX_ENEMY_GUN    = 10;
        public const int MAX_ENEMY_SHIELD = 10;

        public const float enemy_speed = 100.0f;
        public const int player_init_HP = 3;

        public const int lv1_stage = 1;
        public const int lv2_stage = 5;
        public const int lv3_stage = 10;
        public const int lv4_stage = 15;
        //Dagger
        public const int dagger_width = 10;
        public const int dagger_height = 20;
        public const float dagger_distance = 150.0f;
        public const float dagger_distance_up = 250.0f;
        static public int init_dagger_dammage = 5;
        //Bullet
        public const int bullet_width = 5;
        public const int bullet_height = 5;
        public const float bullet_distance = 300.0f;
        static public int init_bullet_dammage = 3;
        //Rpg
        public const int rpg_bullet_width = 10;
        public const int rpg_buttet_height = 10;
        public const int rpg_bomb_width = 50;
        public const int rpg_bomb_height = 50;
        static public int init_rpg_dammage = 10;
        //sword
        public const int sword_max_range = 300;
        public const double sword_timer = 0.2;
        static public double sword_charge = 0.0;
        static public int init_sword_dammage = 1;

        public const int varWeaponType = 4;

        public const int player_offset_x = player_width / 2;
        public const int player_offset_y = player_height / 2;

        static public float[] weapon_cool = new float[varWeaponType] {2.0f, 2.0f, 2.0f, 1.0f };//none / dagger / gun / rpg / sword
        static public float dagger_cool;
        static public float gun_cool;
        static public float rpg_cool;
        static public float sword_cool;

        static private int stage;
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

            weapon_cool[0] = 1.5f;
            weapon_cool[1] = 1.5f;
            weapon_cool[2] = 2.0f;
            weapon_cool[3] = 0.3f;

            player_speed = 150.0f;
        }
        static public void StageUp()
        {
            stage++;
        }
        static public int GetStage()
        {
            return stage;
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

            for (int i = 0; i < varWeaponType; i++)
            {
                weapon_cool[i] *= 0.9f;
            }
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
