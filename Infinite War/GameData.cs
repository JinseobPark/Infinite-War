using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Infinite_War
{
    enum Weapons //무기 타입
    {
        DAGGER, GUN, RPG, SWORD
    };
    public enum WeaponRareUpList //업그레이드 리스트
    {
        NONE, DAGGER, GUN, RPG, SWORD
    };
    public struct AbilityBox    //능력 박스
    {
        public string type;     //레어도
        public string ab_name;  //이름
        public string ab_value; //올라가는 능력치
        public string ab_add;   //추가사항
        public int ab_code;     //능력 코드

        public AbilityBox(string _type, string _ab_name, string _ab_value, string _ab_add, int _ab_code) //initialize
        {
            type = _type;
            ab_name = _ab_name;
            ab_value = _ab_value;
            ab_add = _ab_add;
            ab_code = _ab_code;
        }
        public void SetAbilityBox(string _type, string _ab_name, string _ab_value, string _ab_add, int _ab_code)    //set
        {
            type = _type;
            ab_name = _ab_name;
            ab_value = _ab_value;
            ab_add = _ab_add;
            ab_code = _ab_code;
        }
    }
    public static class LevelUpBox  //레벨업시
    {
        static public int[] probArray = new int[4] { 40, 30, 20, 3 }; //each high, mid, low, rare 각각 확률
        static public int[] rared_probArray = new int[3] { 40, 30, 20}; //each high, mid, low 각각 확률
        static public List<AbilityBox>[] AbilityList = new List<AbilityBox>[4]; //각 희귀도에 따라 리스트를 가짐

        static public void init()   //초기설정
        {
            AbilityList[0] = new List<AbilityBox>();    //각 희귀도의 리스트 
            AbilityList[1] = new List<AbilityBox>();
            AbilityList[2] = new List<AbilityBox>();
            AbilityList[3] = new List<AbilityBox>();

            AbilityBox temp = new AbilityBox();         //저장할 능력의 temp
            //흔한 능력치. 
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

            //일반 능력치
            temp.SetAbilityBox("  **", "Recover HP", "", "", 10);
            AbilityList[1].Add(temp);
            temp.SetAbilityBox("  **", "Damage Up", "  20", "", 11);
            AbilityList[1].Add(temp);
            temp.SetAbilityBox("  **", "Movement Up", "  10", "", 12);
            AbilityList[1].Add(temp);
            temp.SetAbilityBox(" ***", "Max HP Up", "  1", "", 13);
            AbilityList[1].Add(temp);

            //고급 능력치
            temp.SetAbilityBox(" ***", "Movement Up", "  20", "", 20);
            AbilityList[2].Add(temp);
            temp.SetAbilityBox(" ***", "Damage Up", "  30", "", 21);
            AbilityList[2].Add(temp);
            temp.SetAbilityBox(" ***", "Attack Speed Up", "  10", "", 22);
            AbilityList[2].Add(temp);
            temp.SetAbilityBox(" ***", "Dagger Up", "  50", "", 23);
            AbilityList[2].Add(temp);
            temp.SetAbilityBox(" ***", "Gun Up", "  50", "", 24);
            AbilityList[2].Add(temp);
            temp.SetAbilityBox(" ***", "RPG Up", "  50", "", 25);
            AbilityList[2].Add(temp);
            temp.SetAbilityBox(" ***", "Sword Up", "  50", "", 26);
            AbilityList[2].Add(temp);

            //한번만 선택 가능한 능력치.
            temp.SetAbilityBox(" ****", "Dagger Up", "", "Long Range\n Double Throw", 30);
            AbilityList[3].Add(temp);
            temp.SetAbilityBox(" ****", "Gun Up", "", "Double Shot\n Rapid Fire", 31);
            AbilityList[3].Add(temp);
            temp.SetAbilityBox(" ****", "RPG Up", "", "Range Up", 32);
            AbilityList[3].Add(temp);
            temp.SetAbilityBox(" ****", "Sword Up", "", "Range Up\n More Charge", 33);
            AbilityList[3].Add(temp);

            
        }

        static public int LevelUp_Quality() //희귀도 설정
        {
            Random random_number = new Random(); //랜덤을 사용
            int prob_total = 0;                  //총 확률의 합
            int randomPoint;                     //확률 위치
            if (!GameData.is_Rare())             //레어를 선택 안했을때
            {
                foreach (int prob in probArray)  //각 능력의 확률을 더함
                    prob_total += prob;
                randomPoint = random_number.Next(prob_total);
                for (int i = 0; i < probArray.Length; i++) //전체의 확률에서 내려가면서 위치를 설정함.
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
            else                                  //레어를 선택 했을때
            {
                foreach (int prob in rared_probArray)   //위의 알고리즘와 같음
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
        static public AbilityBox ShowLevelUpAbility()   //희귀도에 따른 능력을 가지는 함수
        {
            AbilityBox result = new AbilityBox();
            int Random_rarity = LevelUp_Quality();
            result = AbilityUpList(Random_rarity);
            return result;
        }
        static public AbilityBox AbilityUpList(int rarity)  //희귀도를 정했으면 그 희귀도 내에서 랜덤으로 능력을 정함
        {
            Random random = new Random();
            int random_number = random.Next(AbilityList[rarity].Count());

            return AbilityList[rarity][random_number];
        }
        static public void ChooseAbility(int code, Player p) //능력치 선택하는 함수
        {
            switch(code)    //코드별로 능력치 상승
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
                        probArray[i] += GameData.probUpPer;
                    }
                    for (int i = 1; i < rared_probArray.Length; i++)
                    {
                        rared_probArray[i] += GameData.probUpPer;
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
                case 13:
                    p.AddMaxHP();
                    break;
                case 20:
                    GameData.Upgrade_move_up(20);
                    break;
                case 21:
                    GameData.Upgrade_damage_up(30);
                    break;
                case 22:
                    GameData.Upgrade_atkSpeed_up();
                    break;
                case 23:
                    GameData.Upgrade_dagger_up(50);
                    break;
                case 24:
                    GameData.Upgrade_gun_up(50);
                    break;
                case 25:
                    GameData.Upgrade_rpg_up(50);
                    break;
                case 26:
                    GameData.Upgrade_sword_up(50);
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
    public static class Score   //점수 클래스
    {
        static private int record_score;    //신기록
        static Score()              //initialize
        {
            record_score = 0;
        }
        static public void CheckNewRecord() //신기록 체커
        {
            if (record_score < GameData.GetKill())  //신기록보다 크면 지금이 신기록
            {
                record_score = GameData.GetKill();
                SaveRecord();
            }

        }
        static public void SaveRecord()     //신기록 저장
        {
            string score = getRecordScore().ToString();
            using (StreamWriter sw = new StreamWriter("../../../record.txt"))
            {
                sw.Write(score);
                sw.Close();
            }
        }
        static public int getRecordScore()  //신기록 겟
        {
            return record_score;
        }
        static public void SetRecordScore(int score)    //이 기록이 신기록이다
        {
            record_score = score;
        }
    }
    public struct Ability   //능력 구조체
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
    public static class GameData    //게임 데이터 (매우 중요)
    {
        //창 사이즈
        public const int FormSize_Width = 1200; //폼 너비
        public const int FormSize_Height = 800; //폼 높이
        //플레이어 사이즈
        public const int player_width = 64; //플레이어 너비
        public const int player_height = 64;    //플레이어 높이
        //적 사이즈
        public const int enemy_width = 64;  //적 너비
        public const int enemy_height = 64; //적 높이
        //각 최대 오브젝트 갯수
        public const int MAX_DAGGER = 30;   //최대 단검 갯수
        public const int MAX_BULLET = 30;   //최대 총알 갯수
        public const int MAX_RPG = 20;      //최대 포탄 갯수
        public const int MAX_SWORD = 10;    //최대 검 갯수
        public const int MAX_BOMB = 10;     //최대 폭파 갯수

        public const int MAX_ENEMY_NORMAL = 30; //최대 일반 적
        public const int MAX_ENEMY_SPEED  = 20; //최대 빠른 적
        public const int MAX_ENEMY_GUN    = 10; //최대 총든 적
        public const int MAX_ENEMY_SHIELD = 10; //최대 방패 적
        public const int MAX_ENEMY_BULLET = 20; //최대 적 총알 수
        //적 이동속도
        public const float enemy_speed = 100.0f;
        //초기 플레이어 체력
        public const int player_init_HP = 3;
        //스테이지별 레벨 조건.
        public const int lv1_stage = 1;
        public const int lv2_stage = 5;
        public const int lv3_stage = 10;
        public const int lv4_stage = 15;
        //적 생성 확률. (1 / number) per tick
        public const int ENEMY_NORMAL_Create_Cool = 30;
        public const int ENEMY_SPEED_Create_Cool = 70;
        public const int ENEMY_GUN_Create_Cool = 100;
        public const int ENEMY_SHIELD_Create_Cool = 120;
        public const int ENEMY_BULLET_Create_Cool = 100;

        //Dagger
        public const int dagger_width = 10;             //단검 너비
        public const int dagger_height = 20;             //단검 높이
        public const float dagger_distance = 150.0f;    //일반 단검 사거리
        public const float dagger_distance_up = 250.0f; //강화 단검 사거리
        public const int init_dagger_dammage = 50;      //초기 단검 공격력
        static public int curr_dagger_dammage = 50;     //현재 단검 공격력
        //Bullet
        public const int bullet_width = 10;             //총알 너비
        public const int bullet_height = 10;            //총알 높이
        public const float bullet_distance = 300.0f;    //총알 사거리
        public const int init_bullet_dammage = 30;      //초기 총알 공격력
        static public int curr_bullet_dammage = 30;     //현재 총알 공격력
        //Rpg
        public const int rpg_bullet_width = 15;         //포탄 너비
        public const int rpg_buttet_height = 15;        //포탄 높이
        public const int rpg_bomb_range = 200;          //일반 폭파 범위
        public const int rpg_bomb_up_range = 300;       //강화 폭파 범위
        public const int init_rpg_dammage = 1;          //초기 포탄 공격력
        static public int curr_rpg_dammage = 1;         //현재 포탄 공격력
        public const int init_bomb_dammage = 80;        //초기 폭파 공격력
        static public int curr_bomb_dammage = 80;       //현재 폭파 공격력
        public const double bomb_timer = 0.2;           //폭파 잔상 시간
        //sword
        public const int sword_max_range = 300;         //일반 최대 칼날 범위
        public const int sword_max_range_up = 450;      //강화 최대 칼날 범위
        public const double sword_timer = 0.2;          //칼날 잔상 시간
        static public double sword_charge = 0.0;        //현재 검 차징 시간
        public const int init_sword_dammage = 100;      //초기 검 공격력
        static public int curr_sword_dammage = 100;     //현재 검 공격력
        static public double sword_normal_range_pertick = 1500.0;       //시간에 따른 범위 변환율-일반
        static public double sword_upgrade_range_pertick = 2500.0;      //시간에 따른 범위 변환율-강화
        //무기 타입은 4가지
        public const int varWeaponType = 4;
        //플레이어 offset
        public const int player_offset_x = player_width / 2;
        public const int player_offset_y = player_height / 2;
        //각 무기별 쿨타임
        static public float[] weapon_cool = new float[varWeaponType] {0.0f, 0.0f, 0.0f, 0.0f };//dagger / gun / rpg / sword
        //현재 스테이지
        static private int stage;
        //현재 킬
        static public int kill;
        //능력 구조체
        static public Ability ab;
        //rpg업글 유무
        static private bool is_rpg_up;
        //능력 중 다른 확률 업의 가치. (다른 확률이 n%만큼 증가)
        public const int probUpPer = 5;
        //플레이어 이동속도
        static private float player_speed;

        static public void init()   //초기값 설정
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
            weapon_cool[1] = 0.8f;
            weapon_cool[2] = 2.0f;
            weapon_cool[3] = 0.1f;

            curr_dagger_dammage = init_dagger_dammage ;
            curr_bullet_dammage = init_bullet_dammage ; 
            curr_rpg_dammage    = init_rpg_dammage    ;
            curr_bomb_dammage   = init_bomb_dammage   ;
            curr_sword_dammage  = init_sword_dammage  ;

            is_rpg_up = false;
            player_speed = 350.0f;
        }
        static public void StageUp()    //스테이지 올라가유
        {
            stage++;
        }
        static public int GetStage()    //스테이지 정보
        {
            return stage;
        }
        static public float getPlayerSpeed()    //플레이어 이동속도 겟
        {
            return player_speed;
        }
        static public void Upgrade_damage_up(int value) //전체 데미지 n% 증가
        {
            ab.damage_up += 1;
            curr_dagger_dammage += (int)(init_dagger_dammage * value * 0.01 );
            curr_bullet_dammage += (int)(init_bullet_dammage * value * 0.01);
            curr_rpg_dammage += (int)(init_rpg_dammage * value * 0.01);
            curr_bomb_dammage += (int)(init_bomb_dammage * value * 0.01);
            curr_sword_dammage += (int)(init_sword_dammage * value * 0.01);
        }
        static public void Upgrade_dagger_up(int value) //대거 데미지 n% 증가
        {
            ab.dagger_up += 1;
            curr_dagger_dammage += (int)(init_dagger_dammage * value * 0.01);
        }
        
        static public void Upgrade_gun_up(int value)    //총알 데미지 n% 증가
        {
            ab.gun_up += 1;
            curr_bullet_dammage += (int)(init_bullet_dammage * value * 0.01);
        }
        static public void Upgrade_rpg_up(int value)    //rpg데미지 n% 증가
        {
            ab.rpg_up += 1;
            curr_rpg_dammage += (int)(init_rpg_dammage * value * 0.01);
            curr_bomb_dammage += (int)(init_bomb_dammage * value * 0.01);
        }
        static public void Upgrade_sword_up(int value)  //검 데미지 n% 증가
        {
            ab.sword_up += 1;
            curr_sword_dammage += (int)(init_sword_dammage * value * 0.01);
        }
        static public void Upgrade_move_up(float value) //이동속도 n%만큼 증가
        {
            ab.move_up++;
            player_speed += player_speed * 0.01f * value;
        }
        static public void Upgrade_atkSpeed_up()        //공격속도 업그레이드마다 10% 감소
        {
            ab.atkSpeed_up += 1;
            for (int i = 0; i < varWeaponType; i++)
            {
                weapon_cool[i] *= 0.9f;
            }
        }
        static public void Upgrade_rare_up(int what)    //레어 능력 선택
        {
            ab.is_RareSelected = true;
            switch(what)
            {
                case 1:
                    ab.rare_up = WeaponRareUpList.DAGGER;
                    break;
                case 2:
                    ab.rare_up = WeaponRareUpList.GUN;
                    weapon_cool[1] *= 0.6f;
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
        static public void AddKill()    //난.. 죽였다..악당을
        {
            kill++;
        }
        static public int GetKill()     //kill수 겟
        {
            return kill;
        }
        static public bool is_Rare()    //레어 선택 했나유
        {
            return ab.is_RareSelected;
        }
        static public bool is_RpgUp_once()  //rpg업그레이드 유무
        {
            return is_rpg_up;
        }
        static public void UpgradeRpgComplete() //rpg 업글 했음!
        {
            if(is_rpg_up)
            is_rpg_up = false;
        }

    }
    
}
