using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinite_War
{
    interface IObject
    {
        o_Point getPositionEdge();
        o_Point getPositionMid();
        o_Size getSize();
        void SetObject(float _x, float _y, int _w, int _h);
        void SetObjectSize(int _w, int _h);
        void SetObjectPosition(float _x, float _y);
        void moveObject();
        void SetDirection(o_Vector v);
    }
    public class Object : IObject
    {
        public o_Point m_position;
        public o_Size m_size;
        public o_Vector m_direction;
        public float speed;
        public float angle;
        public bool exist;
        public Object()
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
        public Object(float _x, float _y, int _w, int _h)
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
        public virtual void SetObject(float _x, float _y, int _w, int _h)
        {
            m_position.x = _x;
            m_position.y = _y;
            m_size.width = _w;
            m_size.height = _h;
        }
        public virtual void SetObjectSize(int _w, int _h)
        {
            m_size.width = _w;
            m_size.height = _h;
        }
        public virtual void SetObjectPosition(float _x, float _y)
        {
            m_position.x = _x;
            m_position.y = _y;
        }
        public virtual void SetObjectPosition(o_Point position)
        {
            m_position.x = position.x;
            m_position.y = position.y;
        }
        public virtual o_Point getPositionEdge()
        {
            return m_position;
        }
        public virtual o_Point getPositionMid()
        {
            o_Point result;
            result.x = m_position.x + m_size.width / 2;
            result.y = m_position.y + m_size.height / 2;
            return result;
        }
        public virtual o_Size getSize()
        {
            return m_size;
        }
        public virtual void moveObject()
        {
            if(GameMath.CheckInside(getPositionEdge()))
            {
                m_direction.normalize();
                m_position.x += m_direction.x * speed * (float)GameMath.dt;
                m_position.y += m_direction.y * speed * (float)GameMath.dt;
            }
            else
            {
                m_position.x = 0.0f;
                m_position.y = 0.0f;
                m_size.width = 0;
                m_size.height = 0;
                m_direction.x = 0.0f;
                m_direction.y = 0.0f;
                //speed = 0.0f;
                angle = 0.0f;
                exist = false;
            }
        }
        public virtual void SetDirection(o_Vector v)
        {
            m_direction.x = v.x;
            m_direction.y = v.y;
        }
        ~Object()
        {  }
    }

    public class Player : Object
    {
        private int HP { get; set; }
        private int MaxHP { get; set; }
        public bool is_can_move { get; set; }
        public bool is_charging { get; set; }
        Weapons cur_weapon;
        public bool[] playerStatus = new bool[4] { false, false, false, false }; //up down left right
        public Player(float _x, float _y, int _w, int _h)
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
        public override o_Point getPositionEdge()
        {
            return m_position;
        }
        public override o_Point getPositionMid()
        {
            o_Point result;
            result.x = m_position.x + GameData.player_offset_x;
            result.y = m_position.y + GameData.player_offset_y;
            return result;
        }
        public void PlayerMoveUp(float speed)
        {
            m_position.y -= speed * (float)GameMath.dt; ; //반대
            m_position.y = Math.Max(GameData.player_height / 2, m_position.y);

        }
        public void PlayerMoveDown(float speed)
        {
            m_position.y += speed * (float)GameMath.dt; ; //반대
            m_position.y = Math.Min(GameData.FormSize_Height - GameData.player_height, m_position.y);
        }
        public void PlayerMoveLeft(float speed)
        {
            m_position.x -= speed * (float)GameMath.dt; ;
            m_position.x = Math.Max(0, m_position.x);
        }
        public void PlayerMoveRight(float speed)
        {
            m_position.x += speed * (float)GameMath.dt; ;
            m_position.x = Math.Min(GameData.FormSize_Width - GameData.player_width, m_position.x);
        }
        public void ChangeWeapon()
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
        public int GetWeaponType()
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
        public void PlayerMove()
        {
            if(is_can_move)
            {
                if (playerStatus[0])
                    PlayerMoveUp(GameData.getPlayerSpeed());
                if (playerStatus[1])
                    PlayerMoveDown(GameData.getPlayerSpeed());
                if (playerStatus[2])
                    PlayerMoveLeft(GameData.getPlayerSpeed());
                if (playerStatus[3])
                    PlayerMoveRight(GameData.getPlayerSpeed());
            }
        }
        
        public void hit()
        {
            HP--;
            if (HP <= 0)
                Death();
        }
        public void Death()
        {
            exist = false;
        }
        public void RecoverHP()
        {
            HP = MaxHP;
        }
        public void AddMaxHP()
        {
            MaxHP++;
        }
        public int GetPlayerHP()
        {
            return HP;
        }
        public int GetPlayerMaxHP()
        {
            return MaxHP;
        }
    }

    class Enemy : Object
    {
        public int HP;
        //public float speed;

        public Enemy()
        {
            HP = GameData.GetStage() * 2;
            m_size.width = GameData.enemy_width;
            m_size.height = GameData.enemy_height;
            speed = GameData.enemy_speed;
        }
        ~Enemy() { }
        public virtual void hit(int damage)
        {
            HP -= damage;
            if(HP <= 0)
            {
                Console.WriteLine("kill!");
                Death();
            }
        }
        public virtual void SetHP()
        {
            HP = GameData.GetStage() * 20;
        }
        public void Death()
        {
            exist = false;
            GameData.AddKill();
            //Score.AddCurScore();
        }
        public void AttackSuccess()
        {
            exist = false;
        }

        public void moveObject(o_Point target)
        {
            m_direction.x = target.x- m_position.x;
            m_direction.y = target.y- m_position.y;
            m_direction.normalize();
            m_position.x += m_direction.x * speed * (float)GameMath.dt;
            m_position.y += m_direction.y * speed * (float)GameMath.dt;
        }

    }

    class Enemy_normal : Enemy
    {
        public Enemy_normal()
        {
        }
    }
    class Enemy_speed : Enemy
    {
        public Enemy_speed()
        {
            speed *= 1.3f;
        }
        public override void SetHP()
        {
            HP = GameData.GetStage() * 15;
        }
    }
    class Enemy_gun : Enemy
    {
        public Enemy_gun()
        {

        }
        public override void SetHP()
        {
            HP = GameData.GetStage() * 15;
        }
    }
    class Enemy_shield : Enemy
    {
        public Enemy_shield()
        {
            speed *= 0.7f;
        }
        public override void SetHP()
        {
            HP = GameData.GetStage() * 40;
        }
    }
    class EnemyBullet : Object
    {
        public o_Vector distance_vector;
        public EnemyBullet()
        {
            SetObjectSize(GameData.bullet_width, GameData.bullet_height);
            speed = 1000.0f;
            distance_vector.x = 0.0f;
            distance_vector.y = 0.0f;
        }
        public override void moveObject()
        {
            if (distance_vector.SizeOfVector() <= GameData.bullet_distance)
            {
                m_direction.normalize();
                m_position.x += m_direction.x * speed * (float)GameMath.dt;
                m_position.y += m_direction.y * speed * (float)GameMath.dt;
                distance_vector.x += m_direction.x * speed * (float)GameMath.dt;
                distance_vector.y += m_direction.y * speed * (float)GameMath.dt;
            }
            else
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
        public void AttackSuccess()
        {
            exist = false;
        }
    }
    class PlayerWeapon : Object
    {
        public int damage;
        public int getDammage()
        {
            return damage;
        }
    }
    class PlayerDagger : PlayerWeapon
    {
        public bool is_can_rotate;
        public bool is_Up;
        public o_Vector distance_vector;

        public PlayerDagger()
        {
            SetObjectSize(GameData.bullet_width, GameData.bullet_height);
            speed = 800.0f;
            is_can_rotate = false;
            is_Up = false;
            distance_vector.x = 0.0f;
            distance_vector.y = 0.0f;
            damage = GameData.init_dagger_dammage;
        }
        public override void moveObject()
        {
            if(is_Up)
            {
                if (distance_vector.SizeOfVector() <= GameData.dagger_distance_up)
                {
                    m_direction.normalize();
                    m_position.x += m_direction.x * speed * (float)GameMath.dt;
                    m_position.y += m_direction.y * speed * (float)GameMath.dt;
                    distance_vector.x += m_direction.x * speed * (float)GameMath.dt;
                    distance_vector.y += m_direction.y * speed * (float)GameMath.dt;
                }
                else
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
            else
            {
                if (distance_vector.SizeOfVector() <= GameData.dagger_distance)
                {
                    m_direction.normalize();
                    m_position.x += m_direction.x * speed * (float)GameMath.dt;
                    m_position.y += m_direction.y * speed * (float)GameMath.dt;
                    distance_vector.x += m_direction.x * speed * (float)GameMath.dt;
                    distance_vector.y += m_direction.y * speed * (float)GameMath.dt;
                }
                else
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
    class PlayerBullet : PlayerWeapon
    {
        public o_Vector distance_vector;
        public PlayerBullet()
        {
            SetObjectSize(GameData.bullet_width, GameData.bullet_height);
            speed = 1000.0f;
            distance_vector.x = 0.0f;
            distance_vector.y = 0.0f;
            damage = GameData.init_bullet_dammage;
        }
        public override void moveObject()
        {
            if (distance_vector.SizeOfVector() <= GameData.bullet_distance)
            {
                m_direction.normalize();
                m_position.x += m_direction.x * speed * (float)GameMath.dt;
                m_position.y += m_direction.y * speed * (float)GameMath.dt;
                distance_vector.x += m_direction.x * speed * (float)GameMath.dt;
                distance_vector.y += m_direction.y * speed * (float)GameMath.dt;
            }
            else
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
    class PlayerRpg : PlayerWeapon
    {
        public PlayerRpg()
        {
            SetObjectSize(GameData.rpg_bullet_width, GameData.rpg_buttet_height);
            speed = 700.0f;
            damage = GameData.init_rpg_dammage;
        }
    }
    class PlayerRpgBomb : PlayerWeapon
    {
        double time_exist;
        int bomb_range;
        public PlayerRpgBomb()
        {
            SetObjectSize(GameData.rpg_bomb_range, GameData.rpg_bomb_range);
            speed = 0.0f;
            damage = GameData.init_bomb_dammage;
            time_exist = 0.0;
            bomb_range = GameData.rpg_bomb_range;
        }

        public void UpgradeRpgBomb()
        {
            SetObjectSize(GameData.rpg_bomb_up_range, GameData.rpg_bomb_up_range);
            speed = 0.0f;
            damage = GameData.init_bomb_dammage;
            time_exist = 0.0;
            bomb_range = GameData.rpg_bomb_up_range;
        }

        public override void moveObject()
        {
            if (time_exist <= GameData.bomb_timer)
            {
                time_exist += GameMath.dt;
                //exist
            }
            else
            {
                m_position.x = 0.0f;
                m_position.y = 0.0f;
                exist = false;
                time_exist = 0.0;
            }
        }
        public int get_bomb_range()
        {
            return bomb_range / 2;
        }
    }
    class PlayerSword : PlayerWeapon
    {
        double timer;
        public double sword_range;
        public PlayerSword()
        {
            timer = 0.0;
            sword_range = 0.0;
            damage = GameData.init_sword_dammage;
        }

        public override void moveObject()
        {
            if (timer <= GameData.sword_timer)
            {
                timer += GameMath.dt;
                //exist
            }
            else
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
        public void Charging(double charged)
        {
            sword_range = charged * 1500.0;
            if (sword_range > GameData.sword_max_range)
                sword_range = GameData.sword_max_range;
        }
        public void ChargingUp(double charged)
        {
            sword_range = charged * 2500.0;
            if (sword_range > GameData.sword_max_range_up)
                sword_range = GameData.sword_max_range_up;
        }
        public double get_sword_range()
        {
            return sword_range;
        }
    }
}
