using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
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
    public enum SkillUpProbability
    {
        HIGH, NORMAL, LOW, RARE
    };
    public struct AbilityBox
    {
        public string type;
        public string ab_name;
        public string ab_value;
        public string ab_add;
        public int ab_code;

        public AbilityBox(string _type, string _ab_name, string _ab_value, string _ab_add, int _ab_code)
        {
            type = _type;
            ab_name = _ab_name;
            ab_value = _ab_value;
            ab_add = _ab_add;
            ab_code = _ab_code;
        }
        public void SetAbilityBox(string _type, string _ab_name, string _ab_value, string _ab_add, int _ab_code)
        {
            type = _type;
            ab_name = _ab_name;
            ab_value = _ab_value;
            ab_add = _ab_add;
            ab_code = _ab_code;
        }
    }
    public static class LevelUpBox
    {
        static public int[] probArray = new int[4] { 40, 30, 20, 5 }; //each high, mid, low, rare
        static public int[] rared_probArray = new int[3] { 40, 30, 20}; //each high, mid, low
        static public List<AbilityBox>[] AbilityList = new List<AbilityBox>[4];

        static public void init()
        {
            AbilityList[0] = new List<AbilityBox>();
            AbilityList[1] = new List<AbilityBox>();
            AbilityList[2] = new List<AbilityBox>();
            AbilityList[3] = new List<AbilityBox>();

            AbilityBox temp = new AbilityBox();

            temp.SetAbilityBox("  *", "Damage Up", "  10", "", 00);
            AbilityList[0].Add(temp); 
            temp.SetAbilityBox("  *", "Dagger Up", "  20", "", 01);
            AbilityList[0].Add(temp);
            temp.SetAbilityBox("  *", "Gun Up", "  20", "", 02);
            AbilityList[0].Add(temp);
            temp.SetAbilityBox("  *", "RPG Up", "  20", "", 03);
            AbilityList[0].Add(temp);
            temp.SetAbilityBox("  *", "Sword Up", "  20", "", 04);
            AbilityList[0].Add(temp);
            temp.SetAbilityBox("  *", "Other probability Up", "", "", 05);
            AbilityList[0].Add(temp);


            temp.SetAbilityBox("  **", "Recover HP", "", "", 10);
            AbilityList[1].Add(temp);
            temp.SetAbilityBox("  **", "Damage Up", "  20", "", 11);
            AbilityList[1].Add(temp);
            temp.SetAbilityBox("  **", "Movement Up", "  10", "", 12);
            AbilityList[1].Add(temp);


            temp.SetAbilityBox(" ***", "Movement Up", "  20", "", 20);
            AbilityList[2].Add(temp);
            temp.SetAbilityBox(" ***", "Damage Up", "  40", "", 21);
            AbilityList[2].Add(temp);
            temp.SetAbilityBox(" ***", "Max HP Up", "  1", "", 22);
            AbilityList[2].Add(temp);
            temp.SetAbilityBox(" ***", "Attack Speed Up", "  10", "", 23);
            AbilityList[2].Add(temp);
            temp.SetAbilityBox(" ***", "Dagger Up", "  40", "", 24);
            AbilityList[2].Add(temp);
            temp.SetAbilityBox(" ***", "Gun Up", "  40", "", 25);
            AbilityList[2].Add(temp);
            temp.SetAbilityBox(" ***", "RPG Up", "  40", "", 26);
            AbilityList[2].Add(temp);
            temp.SetAbilityBox(" ***", "Sword Up", "  40", "", 27);
            AbilityList[2].Add(temp);


            temp.SetAbilityBox(" ****", "Dagger Up", "", "Long Range\n Double Throw", 30);
            AbilityList[3].Add(temp);
            temp.SetAbilityBox(" ****", "Gun Up", "", "Double Shot\n Rapid Fire", 31);
            AbilityList[3].Add(temp);
            temp.SetAbilityBox(" ****", "RPG Up", "", "Range Up", 32);
            AbilityList[3].Add(temp);
            temp.SetAbilityBox(" ****", "Sword Up", "", "Range Up\n More Charge", 33);
            AbilityList[3].Add(temp);

            
        }

        static public int LevelUp_Quality()
        {
            Random random_number = new Random();
            int prob_total = 0;
            int randomPoint;
            if (!GameData.is_Rare()) //not rare pick
            {
                foreach (int prob in probArray)
                    prob_total += prob;
                randomPoint = random_number.Next(prob_total);
                for (int i = 0; i < probArray.Length; i++)
                {
                    if (randomPoint < probArray[i])
                    {
                        return i;
                    }
                    else
                    {
                        randomPoint -= probArray[i];
                    }
                }
                return probArray[0];

            }
            else //already picked rare
            {
                foreach (int prob in rared_probArray)
                    prob_total += prob;
                randomPoint = random_number.Next(prob_total);
                for (int i = 0; i < rared_probArray.Length; i++)
                {
                    if (randomPoint < rared_probArray[i])
                    {
                        return i;
                    }
                    else
                    {
                        randomPoint -= rared_probArray[i];
                    }
                }
                return rared_probArray[0];
            }
        }
        static public AbilityBox ShowLevelUpAbility()
        {
            AbilityBox result = new AbilityBox();
            int Random_rarity = LevelUp_Quality();
            result = AbilityUpList(Random_rarity);
            return result;
        }
        static public AbilityBox HighAbilityUpList()
        {
            Random random = new Random();
            int random_number = random.Next(AbilityList[0].Count());

            return AbilityList[0][random_number];
        }
        static public AbilityBox AbilityUpList(int rarity)
        {
            Random random = new Random();
            int random_number = random.Next(AbilityList[rarity].Count());

            return AbilityList[rarity][random_number];
        }
        static public void MidAbilityUpList()
        {

        }
        static public void LowAbilityUpList()
        {

        }
        static public void RareAbilityUpList()
        {

        }
        static public void ChooseAbility(int code, Player p)
        {
            switch(code)
            {
                case 00:
                    GameData.Upgrade_damage_up(10);
                    break;
                case 01:
                    GameData.Upgrade_dagger_up(20);
                    break;
                case 02:
                    GameData.Upgrade_gun_up(20);
                    break;
                case 03:
                    GameData.Upgrade_rpg_up(20);
                    break;
                case 04:
                    GameData.Upgrade_sword_up(20);
                    break;
                case 05:
                    for(int i = 1; i < probArray.Length; i++)
                    {
                        probArray[i] += 5;
                    }
                    for (int i = 1; i < rared_probArray.Length; i++)
                    {
                        rared_probArray[i] += 5;
                    }
                    break;
                case 10:
                    p.RecoverHP();
                    break;
                case 11:
                    GameData.Upgrade_damage_up(20);
                    break;
                case 12:
                    GameData.Upgrade_move_up(10);
                    break;
                case 20:
                    GameData.Upgrade_move_up(20);
                    break;
                case 21:
                    GameData.Upgrade_damage_up(40);
                    break;
                case 22:
                    p.AddMaxHP();
                    break;
                case 23:
                    GameData.Upgrade_atkSpeed_up();
                    break;
                case 24:
                    GameData.Upgrade_dagger_up(40);
                    break;
                case 25:
                    GameData.Upgrade_gun_up(40);
                    break;
                case 26:
                    GameData.Upgrade_rpg_up(40);
                    break;
                case 27:
                    GameData.Upgrade_sword_up(40);
                    break;
                case 30:
                    GameData.Upgrade_rare_up(1);
                    break;
                case 31:
                    GameData.Upgrade_rare_up(2);
                    break;
                case 32:
                    GameData.Upgrade_rare_up(3);
                    break;
                case 33:
                    GameData.Upgrade_rare_up(4);
                    break;

            }
        }

    }
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
    public static class Score
    {
        static private int record_score;
        static Score()
        {
            record_score = 0;
        }
        static public void CheckNewRecord()
        {
            if (record_score < GameData.GetKill())
                record_score = GameData.GetKill();
        }
        static public void SaveRecord()
        {
            string score = getRecordScore().ToString();
            using (StreamWriter sw = new StreamWriter("../../../record.txt"))
            {
                sw.Write(score);
                sw.Close();
            }
        }
        static public int getRecordScore()
        {
            return record_score;
        }
        static public void SetRecordScore(int score)
        {
            record_score = score;
        }
    }
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
        public const int MAX_BOMB = 10;

        public const int MAX_ENEMY_NORMAL = 30;
        public const int MAX_ENEMY_SPEED  = 20;
        public const int MAX_ENEMY_GUN    = 10;
        public const int MAX_ENEMY_SHIELD = 10;
        public const int MAX_ENEMY_BULLET = 20;

        public const float enemy_speed = 100.0f;
        public const int player_init_HP = 3;

        public const int lv1_stage = 1;
        public const int lv2_stage = 5;
        public const int lv3_stage = 15;
        public const int lv4_stage = 25;
        //Dagger
        public const int dagger_width = 10;
        public const int dagger_height = 20;
        public const float dagger_distance = 150.0f;
        public const float dagger_distance_up = 250.0f;
        public const int init_dagger_dammage = 50;
        static public int curr_dagger_dammage = 50;
        //Bullet
        public const int bullet_width = 10;
        public const int bullet_height = 10;
        public const float bullet_distance = 300.0f;
        public const int init_bullet_dammage = 30;
        static public int curr_bullet_dammage = 30;
        //Rpg
        public const int rpg_bullet_width = 15;
        public const int rpg_buttet_height = 15;
        public const int rpg_bomb_range = 200;
        public const int rpg_bomb_up_range = 300;
        public const int init_rpg_dammage = 1;
        static public int curr_rpg_dammage = 1;
        public const int init_bomb_dammage = 80;
        static public int curr_bomb_dammage = 80;
        public const double bomb_timer = 0.2;
        //sword
        public const int sword_max_range = 300;
        public const int sword_max_range_up = 450;
        public const double sword_timer = 0.2;
        static public double sword_charge = 0.0;
        public const int init_sword_dammage = 100;
        static public int curr_sword_dammage = 100;

        public const int varWeaponType = 4;

        public const int player_offset_x = player_width / 2;
        public const int player_offset_y = player_height / 2;

        static public float[] weapon_cool = new float[varWeaponType] {0.0f, 0.0f, 0.0f, 0.0f };//dagger / gun / rpg / sword
        static public float dagger_cool;
        static public float gun_cool;
        static public float rpg_cool;
        static public float sword_cool;

        static private int stage;
        static public int kill;
        static public Ability ab;

        static private bool is_rpg_up;


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

            weapon_cool[0] = 1.0f;
            weapon_cool[1] = 1.0f;
            weapon_cool[2] = 1.5f;
            weapon_cool[3] = 0.1f;

            curr_dagger_dammage = init_dagger_dammage ;
            curr_bullet_dammage = init_bullet_dammage ; 
            curr_rpg_dammage    = init_rpg_dammage    ;
            curr_bomb_dammage   = init_bomb_dammage   ;
            curr_sword_dammage  = init_sword_dammage  ;

            is_rpg_up = false;
            player_speed = 250.0f;
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
        static public void Upgrade_damage_up(int value)
        {
            ab.damage_up += 1;
            curr_dagger_dammage += (int)(init_dagger_dammage * value * 0.01 );
            curr_bullet_dammage += (int)(init_bullet_dammage * value * 0.01);
            curr_rpg_dammage += (int)(init_rpg_dammage * value * 0.01);
            curr_bomb_dammage += (int)(init_bomb_dammage * value * 0.01);
            curr_sword_dammage += (int)(init_sword_dammage * value * 0.01);
        }
        static public void Upgrade_dagger_up(int value)
        {
            ab.dagger_up += 1;
            curr_dagger_dammage += (int)(init_dagger_dammage * value * 0.01);
        }
        
        static public void Upgrade_gun_up(int value)
        {
            ab.gun_up += 1;
            curr_bullet_dammage += (int)(init_bullet_dammage * value * 0.01);
        }
        static public void Upgrade_rpg_up(int value)
        {
            ab.rpg_up += 1;
            curr_rpg_dammage += (int)(init_rpg_dammage * value * 0.01);
            curr_bomb_dammage += (int)(init_bomb_dammage * value * 0.01);
        }
        static public void Upgrade_sword_up(int value)
        {
            ab.sword_up += 1;
            curr_sword_dammage += (int)(init_sword_dammage * value * 0.01);
        }
        static public void Upgrade_prob_up()
        {
            ab.prob_up += 1;
        }
        static public void Upgrade_move_up(float value)
        {
            ab.move_up++;
            player_speed += player_speed * 0.01f * value;
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
                    weapon_cool[1] *= 0.5f;
                    break;
                case 3:
                    ab.rare_up = WeaponRareUpList.RPG;
                    is_rpg_up = true;
                    break;
                case 4:
                    ab.rare_up = WeaponRareUpList.SWORD;
                    break;
            }
        }
        static public void AddKill()
        {
            kill++;
        }
        static public int GetKill()
        {
            return kill;
        }
        static public bool is_Rare()
        {
            return ab.is_RareSelected;
        }
        static public bool is_RpgUp_once()
        {
            return is_rpg_up;
        }
        static public void UpgradeRpgComplete()
        {
            if(is_rpg_up)
            is_rpg_up = false;
        }

    }
    
}
