using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinite_War
{
    interface IObject   //오브젝트 인터페이스
    {
        o_Point getPositionEdge();  //오브젝트의 꼭지점 위치 겟 (왼쪽 위)
        o_Point getPositionMid();   //오브젝트의 중심 위치 겟
        o_Size getSize();           //오브젝트 사이즈 겟
        void SetObject(float _x, float _y, int _w, int _h); //오브젝트 세팅
        void SetObjectSize(int _w, int _h);                 //오브젝트 사이즈 세팅
        void SetObjectPosition(float _x, float _y);         //오브젝트 위치 세팅
        void SetObjectPosition(o_Point position);           //오브젝트 위치 세팅2
        void SetDirection(o_Vector v);                      //오브젝트 방향 세팅
        void moveObject();                                  //오브젝트 행동
    }
    public class Object : IObject                           //오브젝트 클래스
    {
        public o_Point m_position;      //위치
        public o_Size m_size;           //크기
        public o_Vector m_direction;    //방향
        public float speed;             //속도
        public float angle;             //각도
        public bool exist;              //존재
        public Object()                 //초기값 설정
        {
            m_position.x = 0.0f;
            m_position.y = 0.0f;
            m_size.width = 0;
            m_size.height = 0;
            m_direction.x = 0.0f;
            m_direction.y = 0.0f;
            speed = 0.0f;
            angle = 0.0f;
            exist = false;
        }
        public Object(float _x, float _y, int _w, int _h)   //초기값 설정2
        {
            m_position.x = _x;
            m_position.y = _y;
            m_size.width = _w;
            m_size.height = _h;
            m_direction.x = 0.0f;
            m_direction.y = 0.0f;
            speed = 0.0f;
            angle = 0.0f;
            exist = false;
        }
        public virtual void SetObject(float _x, float _y, int _w, int _h)   //오브젝트 설정
        {
            m_position.x = _x;
            m_position.y = _y;
            m_size.width = _w;
            m_size.height = _h;
        }
        public virtual void SetObjectSize(int _w, int _h)   //크기 설정
        {
            m_size.width = _w;
            m_size.height = _h;
        }
        public virtual void SetObjectPosition(float _x, float _y) //위치 설정
        {
            m_position.x = _x;
            m_position.y = _y;
        }
        public virtual void SetObjectPosition(o_Point position) //위치 설정 2
        {
            m_position.x = position.x;
            m_position.y = position.y;
        }
        public virtual o_Point getPositionEdge()    //끝위치 값 겟
        {
            return m_position;
        }
        public virtual o_Point getPositionMid()     //중앙 위치 값 겟
        {
            o_Point result;
            result.x = m_position.x + m_size.width / 2;
            result.y = m_position.y + m_size.height / 2;
            return result;
        }
        public virtual o_Size getSize()             //사이즈 겟
        {
            return m_size;
        }
        public virtual void moveObject()            //오브젝트 무브무브
        {
            if(GameMath.CheckInside(getPositionMid())) //오브젝트 위치가 화면 내에 있다면
            {
                m_direction.normalize();
                m_position.x += m_direction.x * speed * (float)GameMath.dt;
                m_position.y += m_direction.y * speed * (float)GameMath.dt;
            }
            else                                      //없다면 이거슨 죽은 물체간주, 초기화
            {
                m_position.x = 0.0f;
                m_position.y = 0.0f;
                m_size.width = 0;
                m_size.height = 0;
                m_direction.x = 0.0f;
                m_direction.y = 0.0f;
                angle = 0.0f;
                exist = false;
            }
        }
        public virtual void SetDirection(o_Vector v)    //방향 설정
        {
            m_direction.x = v.x;
            m_direction.y = v.y;
        }
        ~Object()   //잘가요.. 오브젝트
        {  }
    }

    public class Player : Object
    {
        private int HP { get; set; }        //플레이어 체력
        private int MaxHP { get; set; }     //플레이어 최대 체력
        public bool is_can_move { get; set; }   //플레이어 움직임 유무(검 차징중 불가)
        public bool is_charging { get; set; }   //플레이어 차징중? (검 차징)
        Weapons cur_weapon;                     //플레이어 현재 무기
        public bool[] playerStatus = new bool[4] { false, false, false, false }; //up down left right 이동의 유무
        public Player(float _x, float _y, int _w, int _h)   //플레이어 초기화
        {
            SetObject(_x, _y, _w, _h);
            cur_weapon = Weapons.DAGGER;
            HP = GameData.player_init_HP;
            MaxHP = GameData.player_init_HP;
            exist = true;
            speed = GameData.getPlayerSpeed();
            is_can_move = true;
            is_charging = false;
        }
        public override o_Point getPositionEdge()   //플레이어 끝위치 겟
        {
            return m_position;
        }
        public override o_Point getPositionMid()    //플레이어 중앙위치 겟
        {
            o_Point result;
            result.x = m_position.x + GameData.player_offset_x;
            result.y = m_position.y + GameData.player_offset_y;
            return result;
        }
        public void PlayerMoveUp(float speed)       //플레이어 위로 움직임
        {
            m_position.y -= speed * (float)GameMath.dt; ; //반대
            m_position.y = Math.Max(GameData.player_height / 2, m_position.y);  //맵 이탈 ㄴㄴ

        }
        public void PlayerMoveDown(float speed)     //플레이어 아래 움직임
        {
            m_position.y += speed * (float)GameMath.dt; //반대
            m_position.y = Math.Min(GameData.FormSize_Height - GameData.player_height, m_position.y);  //맵 이탈 ㄴㄴ
        }
        public void PlayerMoveLeft(float speed)     //플레이어 왼쪽 움직임
        {
            m_position.x -= speed * (float)GameMath.dt;
            m_position.x = Math.Max(0, m_position.x);  //맵 이탈 ㄴㄴ
        }
        public void PlayerMoveRight(float speed)    //플레이어 오른쪽 움직임
        {
            m_position.x += speed * (float)GameMath.dt;
            m_position.x = Math.Min(GameData.FormSize_Width - GameData.player_width, m_position.x);  //맵 이탈 ㄴㄴ
        }
        public void ChangeWeapon()                  //플레이어 무기 변경 (지정된 다음 무기)
        {
            switch(cur_weapon)
            {
                case Weapons.DAGGER:
                    cur_weapon = Weapons.GUN;
                    break;
                case Weapons.GUN:
                    cur_weapon = Weapons.RPG;
                    break;
                case Weapons.RPG:
                    cur_weapon = Weapons.SWORD;
                    break;
                case Weapons.SWORD:
                    cur_weapon = Weapons.DAGGER;
                    break;
            }
        }
        public int GetWeaponType()          //현재 무기타입 겟
        {
            switch (cur_weapon)
            {
                case Weapons.DAGGER:
                    return 0;
                case Weapons.GUN:
                    return 1;
                case Weapons.RPG:
                    return 2;
                case Weapons.SWORD:
                    return 3;
            }
            return 0;
        }
        public void PlayerMove()            //플레이어 무브무브
        {
            if(is_can_move)                 //움직일 수 있을때
            {
                if (playerStatus[0])        //각 위치로 움직임
                    PlayerMoveUp(GameData.getPlayerSpeed());
                if (playerStatus[1])
                    PlayerMoveDown(GameData.getPlayerSpeed());
                if (playerStatus[2])
                    PlayerMoveLeft(GameData.getPlayerSpeed());
                if (playerStatus[3])
                    PlayerMoveRight(GameData.getPlayerSpeed());
            }
        }
        
        public void hit()                   //플레이어 맞을때
        {
            HP--;                           //아파용
            if (HP <= 0)
                Death();                    //죽어용
        }
        public void Death()                 //플레이어 죽었을 때
        {
            exist = false;                  //너는 죽었다.
        }
        public void RecoverHP()             //피 회복!
        {
            HP = MaxHP;                     //나는야 만피
        }
        public void AddMaxHP()              //최대 체력 증가!
        {
            MaxHP++;                        //사실 고작 1증가
        }
        public int GetPlayerHP()            //현재 체력 겟
        {
            return HP;
        }
        public int GetPlayerMaxHP()         //최대 체력 겟
        {
            return MaxHP;
        }
    }

    class Enemy : Object
    {
        public int HP;  //적 체력

        public Enemy()  //초기화
        {
            HP = GameData.GetStage() * 20;
            m_size.width = GameData.enemy_width;
            m_size.height = GameData.enemy_height;
            speed = GameData.enemy_speed;
        }
        ~Enemy() { }    //죽음화
        public virtual void hit(int damage) //데미지 입었을때
        {
            HP -= damage;   //데미지 입음
            if(HP <= 0)         
            {
                Death();    //듀금
            }
        }
        public virtual void SetHP() //체력 설정
        {
            HP = GameData.GetStage() * 20;
        }
        public void Death()         //듀금
        {
            exist = false;          //존재의 가치가 없음
            GameData.AddKill();     //주님 한명 더 갑니다
        }
        public void AttackSuccess() //적 : 임무 성공
        {
            exist = false;          //임무 성공하여 자결
        }

        public void moveObject(o_Point target)  //타겟을 향하여 이동
        {
            m_direction.x = target.x- m_position.x;
            m_direction.y = target.y- m_position.y;
            m_direction.normalize();
            m_position.x += m_direction.x * speed * (float)GameMath.dt;
            m_position.y += m_direction.y * speed * (float)GameMath.dt;
        }

    }

    class Enemy_normal : Enemy          //일반 적
    {
        public Enemy_normal()
        {
        }
    }
    class Enemy_speed : Enemy           //빠른 적
    {
        public Enemy_speed()          //저는 빠릅니다
        {
            speed *= 1.4f;
        }
        public override void SetHP()    //하지만 약해요
        {
            HP = GameData.GetStage() * 15;
        }
    }
    class Enemy_gun : Enemy             //총 든적
    {
        public Enemy_gun()
        {

        }
        public override void SetHP()    //하지만 약해요
        {
            HP = GameData.GetStage() * 15;
        }
    }
    class Enemy_shield : Enemy          //방패 든 적
    {
        public Enemy_shield()            //저는 느립니다.
        {
            speed *= 0.7f;
        }
        public override void SetHP()    //대신 잘 버텨요
        {
            HP = GameData.GetStage() * 40;
        }
    }
    class EnemyBullet : Object          //적의 총알
    {
        public o_Vector distance_vector;    //총알의 사거리
        public EnemyBullet()             //초기화
        {
            SetObjectSize(GameData.bullet_width, GameData.bullet_height);
            speed = 1000.0f;
            distance_vector.x = 0.0f;
            distance_vector.y = 0.0f;
        }
        public override void moveObject()   //총알의 여행
        {
            if (distance_vector.SizeOfVector() <= GameData.bullet_distance) //거리만큼 이동
            {
                m_direction.normalize();
                m_position.x += m_direction.x * speed * (float)GameMath.dt;
                m_position.y += m_direction.y * speed * (float)GameMath.dt;
                distance_vector.x += m_direction.x * speed * (float)GameMath.dt;
                distance_vector.y += m_direction.y * speed * (float)GameMath.dt;
            }
            else                            //여행 끝. 임무 실패
            {
                m_position.x = 0.0f;
                m_position.y = 0.0f;
                m_size.width = 0;
                m_size.height = 0;
                m_direction.x = 0.0f;
                m_direction.y = 0.0f;
                angle = 0.0f;
                exist = false;
                distance_vector.x = 0.0f;
                distance_vector.y = 0.0f;
            }
        }
        public void AttackSuccess()         //여행 성공! 임무 성공!
        {
            exist = false;                  
        }
    }
    class PlayerWeapon : Object         //플레이어 무기
    {
        public int damage;              //무기 데미지
        public int getDammage()         //데미지 겟
        {
            return damage;
        }
    }
    class PlayerDagger : PlayerWeapon        //대거 무기
    {
        public bool is_can_rotate;           //회전은 한번
        public bool is_Up;                   //업글 했는가?
        public o_Vector distance_vector;     //사거리

        public PlayerDagger()                //초기화
        {
            SetObjectSize(GameData.bullet_width, GameData.bullet_height);
            speed = 800.0f;
            is_can_rotate = false;
            is_Up = false;
            distance_vector.x = 0.0f;
            distance_vector.y = 0.0f;
            damage = GameData.init_dagger_dammage;
        }
        public override void moveObject()    //대검의 여행
        {
            if(is_Up)                        //업글이라면
            {
                if (distance_vector.SizeOfVector() <= GameData.dagger_distance_up)  //더 멀리가유
                {
                    m_direction.normalize();
                    m_position.x += m_direction.x * speed * (float)GameMath.dt;
                    m_position.y += m_direction.y * speed * (float)GameMath.dt;
                    distance_vector.x += m_direction.x * speed * (float)GameMath.dt;
                    distance_vector.y += m_direction.y * speed * (float)GameMath.dt;
                }
                else                        //여행 끝
                {
                    m_position.x = 0.0f;
                    m_position.y = 0.0f;
                    m_size.width = 0;
                    m_size.height = 0;
                    m_direction.x = 0.0f;
                    m_direction.y = 0.0f;
                    angle = 0.0f;
                    exist = false;
                    is_Up = false;
                    is_can_rotate = false;
                    distance_vector.x = 0.0f;
                    distance_vector.y = 0.0f;
                }
            }
            else                                //업글이 아니라면
            {
                if (distance_vector.SizeOfVector() <= GameData.dagger_distance)  //멀리 몬가유
                {
                    m_direction.normalize();
                    m_position.x += m_direction.x * speed * (float)GameMath.dt;
                    m_position.y += m_direction.y * speed * (float)GameMath.dt;
                    distance_vector.x += m_direction.x * speed * (float)GameMath.dt;
                    distance_vector.y += m_direction.y * speed * (float)GameMath.dt;
                }
                else                            //여행 끝
                {
                    m_position.x = 0.0f;
                    m_position.y = 0.0f;
                    m_size.width = 0;
                    m_size.height = 0;
                    m_direction.x = 0.0f;
                    m_direction.y = 0.0f;
                    angle = 0.0f;
                    exist = false;
                    is_Up = false;
                    is_can_rotate = false;
                    distance_vector.x = 0.0f;
                    distance_vector.y = 0.0f;
                }
            }
        }
    }
    class PlayerBullet : PlayerWeapon       //총알 무기
    {
        public o_Vector distance_vector;    //사거리
        public PlayerBullet()               //초기화
        {
            SetObjectSize(GameData.bullet_width, GameData.bullet_height);
            speed = 1000.0f;
            distance_vector.x = 0.0f;
            distance_vector.y = 0.0f;
            damage = GameData.init_bullet_dammage;
        }
        public override void moveObject()   //총알의 여행
        {
            if (distance_vector.SizeOfVector() <= GameData.bullet_distance) //여행떠나자
            {
                m_direction.normalize();
                m_position.x += m_direction.x * speed * (float)GameMath.dt;
                m_position.y += m_direction.y * speed * (float)GameMath.dt;
                distance_vector.x += m_direction.x * speed * (float)GameMath.dt;
                distance_vector.y += m_direction.y * speed * (float)GameMath.dt;
            }
            else //여행 끝
            {
                m_position.x = 0.0f;
                m_position.y = 0.0f;
                m_size.width = 0;
                m_size.height = 0;
                m_direction.x = 0.0f;
                m_direction.y = 0.0f;
                angle = 0.0f;
                exist = false;
                distance_vector.x = 0.0f;
                distance_vector.y = 0.0f;
            }
        }

    }
    class PlayerRpg : PlayerWeapon  //Rpg 무기
    {
        public PlayerRpg()  //초기화
        {
            SetObjectSize(GameData.rpg_bullet_width, GameData.rpg_buttet_height);
            speed = 700.0f;
            damage = GameData.init_rpg_dammage;
        }
    }
    class PlayerRpgBomb : PlayerWeapon  //Rpg의 폭파 무기
    {
        double time_exist;              //폭파가 보여지는 시간
        int bomb_range;                 //폭파 범위
        public PlayerRpgBomb()          //초기화
        {
            SetObjectSize(GameData.rpg_bomb_range, GameData.rpg_bomb_range);
            speed = 0.0f;
            damage = GameData.init_bomb_dammage;
            time_exist = 0.0;
            bomb_range = GameData.rpg_bomb_range;
        }

        public void UpgradeRpgBomb()    //Rpg 업그레이드
        {
            SetObjectSize(GameData.rpg_bomb_up_range, GameData.rpg_bomb_up_range);
            speed = 0.0f;
            damage = GameData.init_bomb_dammage;
            time_exist = 0.0;
            bomb_range = GameData.rpg_bomb_up_range;
        }

        public override void moveObject()   //폭파의 여행
        {
            if (time_exist <= GameData.bomb_timer)  //폭파 잔상 시간
            {
                time_exist += GameMath.dt;
                //exist
            }
            else                                    //연기가 되어.. 흐려지다
            {
                m_position.x = 0.0f;
                m_position.y = 0.0f;
                exist = false;
                time_exist = 0.0;
            }
        }
        public int get_bomb_range()         //폭파의 범위
        {
            return bomb_range / 2;          //크기의 반
        }
    }
    class PlayerSword : PlayerWeapon    //검 무기
    {
        double timer;                   //범위 잔상 시간
        public double sword_range;      //범위
        public PlayerSword()            //초기화
        {
            timer = 0.0;
            sword_range = 0.0;
            damage = GameData.init_sword_dammage;
        }

        public override void moveObject()   //검의 여행
        {
            if (timer <= GameData.sword_timer)  //잔상 시간
            {
                timer += GameMath.dt;
                //exist
            }
            else                                //바람의 소리였다
            {
                m_position.x = 0.0f;
                m_position.y = 0.0f;
                m_size.width = 0;
                m_size.height = 0;
                m_direction.x = 0.0f;
                m_direction.y = 0.0f;
                angle = 0.0f;
                exist = false;
                timer = 0.0;
                sword_range = 0.0;
            }
        }
        public void Charging(double charged) //차징했다 (차징 시간)
        {
            sword_range = charged * GameData.sword_normal_range_pertick;
            if (sword_range > GameData.sword_max_range)
                sword_range = GameData.sword_max_range;
        }
        public void ChargingUp(double charged)  //고오급 차징 (차징시간)
        {
            sword_range = charged * GameData.sword_upgrade_range_pertick;
            if (sword_range > GameData.sword_max_range_up)
                sword_range = GameData.sword_max_range_up;
        }
        public double get_sword_range()         //차징 범위
        {
            return sword_range;
        }
    }
}
