using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinite_War
{
    public static class Score
    {
        static public int record_count;
        static public int current_score;
        static Score()
        {
            record_count = 0;
            current_score = 0;
        }
    }
    class Object
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
        public virtual o_Point getPosition()
        {
            return m_position;
        }
        public virtual o_Size getSize()
        {
            return m_size;
        }
        public virtual void moveObject()
        {
            if(GameMath.CheckInside(getPosition()))
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


    class Player : Object
    {
        private int HP { get; set; }
        public bool is_can_move { get; set; }
        public bool is_charging { get; set; }
        //delegate void CharacterMove(float speed);
        Weapons cur_weapon;
        WeaponRareUpList cur_weapon_up;
        public bool[] playerStatus = new bool[4] { false, false, false, false }; //up down left right
        public Player(float _x, float _y, int _w, int _h)
        {
            SetObject(_x, _y, _w, _h);
            cur_weapon = Weapons.DAGGER;
            cur_weapon_up = WeaponRareUpList.NONE;
            HP = GameData.player_init_HP;
            exist = true;
            speed = GameData.getPlayerSpeed();
            is_can_move = true;
            is_charging = false;
        }
        public override o_Point getPosition()
        {
            o_Point result;
            result.x = m_position.x - GameData.player_offset_x;
            result.y = m_position.y - GameData.player_offset_y;
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
        public void Attack()
        {
            switch (cur_weapon)
            {
                case Weapons.DAGGER:
                    //hello
                    break;
                case Weapons.GUN:
                    //cur_weapon = Weapons.RPG;
                    break;
                case Weapons.RPG:
                    //cur_weapon = Weapons.SWORD;
                    break;
                case Weapons.SWORD:
                    //cur_weapon = Weapons.DAGGER;
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
            HP = GameData.GetStage() * 3;
        }
        public virtual void Death()
        {
            exist = false;
            Score.record_count++;
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

        public virtual void CreateEnemy()
        {

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
            HP = GameData.GetStage() * 2;
        }
    }
    class Enemy_gun : Enemy
    {
        public Enemy_gun()
        {

        }
        public override void SetHP()
        {
            HP = GameData.GetStage() * 2;
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
            HP = GameData.GetStage() * 5;
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
        public o_Vector distance_vector;

        public PlayerDagger()
        {
            SetObjectSize(GameData.bullet_width, GameData.bullet_height);
            speed = 800.0f;
            is_can_rotate = false;
            distance_vector.x = 0.0f;
            distance_vector.y = 0.0f;
            damage = GameData.init_dagger_dammage;
        }
        public override void moveObject()
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
                is_can_rotate = false;
                distance_vector.x = 0.0f;
                distance_vector.y = 0.0f;
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
        //public PlayerRpg(float _x, float _y)
        //{
        //    SetObject(_x, _y, GameData.bullet_width, GameData.bullet_height);
        //}
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
            //SetObjectSize(GameData.bullet_width, GameData.bullet_height);
            //speed = 700.0f;
        }

        public override void moveObject()
        {
            timer += GameMath.dt;
            if (timer <= GameData.sword_timer)
            {
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
            sword_range = charged * 1000.0;
            if (sword_range > GameData.sword_max_range)
                sword_range = GameData.sword_max_range;
        }
        public double get_sword_range()
        {
            return sword_range;
        }
    }


}
