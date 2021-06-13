using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Infinite_War
{
    public partial class Game_Form : Form
    {
        DateTime StartTime = DateTime.Now, EndAttackTime = DateTime.Now;
        float attackTime;
        bool is_TimeElapsed;
        bool is_pause, must_select;
        Stopwatch stoper = new Stopwatch();
        Bitmap Ground = new Bitmap(1200, 800);
        Bitmap bomb, bullet, dagger, e_normal, e_speed, e_gun, e_shield;
        Bitmap p_player, p_dagger, p_dagger_d, p_gun, p_gun_d, p_rpg, p_rpg_d, p_sword, sword_range;
        Player player;

        Enemy_normal[] enemy_normal;
        Enemy_speed[]  enemy_speed;
        Enemy_gun[]    enemy_gun;
        Enemy_shield[] enemy_shield;

        PlayerDagger[] player_dagger;
        PlayerBullet[] player_bullet;
        PlayerRpg[]    player_rpg;
        PlayerSword[]  player_sword;
        PlayerRpgBomb[] player_rpg_bomb;

        Bitmap clone_player;
        Bitmap[] clone_dagger;
        Bitmap[] clone_e_normal, clone_e_speed, clone_e_gun, clone_e_shield;

        Random random;
        Graphics g_ground;

        Rectangle playerR, bulletR, bombR, enemyR, interR, interBombR, enemyBombR;

        public delegate void del_drawingImages(Graphics graphic);
        public delegate void del_CheckColide();

        [DllImport("User32.dll")]
        private static extern short GetKeyState(int nVirtKey);
        [DllImport("winmm.dll")]
        private static extern long mciSendString(string strCommand, StringBuilder strReturn, int iReturnLength, IntPtr hwndCallback);

        private void Game_Form_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
                player.playerStatus[0] = false;
            if (e.KeyCode == Keys.S)
                player.playerStatus[1] = false;
            if (e.KeyCode == Keys.A)
                player.playerStatus[2] = false;
            if (e.KeyCode == Keys.D)
                player.playerStatus[3] = false;
        }

        private void Game_Form_MouseDown(object sender, MouseEventArgs e)
        {
            if(attackTime > GameData.weapon_cool[player.GetWeaponType()])
            {
                Console.WriteLine("Attack");
                playerAttack();
                StartTime = EndAttackTime;
            }

            //Console.WriteLine("player position : x : " + player.getPosition().x + " y : " + player.getPosition().y);
            //Console.WriteLine("mouse position : x : " + PointToClient(MousePosition).X + " y : " + PointToClient(MousePosition).Y);
            //Console.WriteLine("dt : " + GameMath.dt);
            //Console.WriteLine("player speed : " + GameData.getPlayerSpeed());
        }

        public Game_Form()
        {
            InitializeComponent();
            timer1.Tick += new System.EventHandler(this.timer1_Tick);
        }
        private void Game_Form_Load(object sender, EventArgs e)
        {
            Size = new Size(GameData.FormSize_Width, GameData.FormSize_Height);

            InitDatas();
            player = new Player(GameData.FormSize_Width / 2, GameData.FormSize_Height / 2, GameData.player_width, GameData.player_height);

        }
        private void InitDatas()
        {
            bomb = Resource.bomb;
            bullet = Resource.bullet;
            dagger = Resource.dagger;
            e_normal = Resource.enemy_normal;
            e_speed = Resource.enemy_speed;
            e_gun = Resource.enemy_gun;
            e_shield = Resource.enemy_shield;
            p_player = Resource.player_dagger;
            p_dagger = Resource.player_dagger;
            p_dagger_d = Resource.player_dagger_double;
            p_gun = Resource.player_gun;
            p_gun_d = Resource.player_gun_double;
            p_rpg = Resource.player_rpg;
            p_rpg_d = Resource.player_rpg_up;
            p_sword = Resource.player_sword;
            sword_range = Resource.sword_range;
            clone_player = Resource.player_dagger;

            enemy_normal  = Untility.Init_array<Enemy_normal>(GameData.MAX_ENEMY_NORMAL);
            enemy_speed   = Untility.Init_array<Enemy_speed>(GameData.MAX_ENEMY_SPEED);
            enemy_gun     = Untility.Init_array<Enemy_gun>(GameData.MAX_ENEMY_GUN);
            enemy_shield  = Untility.Init_array<Enemy_shield>(GameData.MAX_ENEMY_SHIELD); 
            player_dagger = Untility.Init_array<PlayerDagger>(GameData.MAX_DAGGER);
            player_bullet = Untility.Init_array<PlayerBullet>(GameData.MAX_BULLET);
            player_rpg    = Untility.Init_array<PlayerRpg>(GameData.MAX_RPG);
            player_sword  = Untility.Init_array<PlayerSword>(GameData.MAX_SWORD);
            player_rpg_bomb = Untility.Init_array<PlayerRpgBomb>(GameData.MAX_BOMB);
            clone_dagger  = new Bitmap[GameData.MAX_DAGGER];
            clone_e_normal = new Bitmap[GameData.MAX_ENEMY_NORMAL];
            clone_e_speed = new Bitmap[GameData.MAX_ENEMY_SPEED];
            clone_e_gun = new Bitmap[GameData.MAX_ENEMY_GUN];
            clone_e_shield = new Bitmap[GameData.MAX_ENEMY_SHIELD];

            random = new Random();
            GameData.init();

            InitWeapons();
            is_TimeElapsed = true;
            is_pause = false;
            must_select = false;
            timer1.Start();

            Console.WriteLine("Load Data!\n");
        }
        private void Game_Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
                player.playerStatus[0] = true;
            if (e.KeyCode == Keys.S)
                player.playerStatus[1] = true;
            if (e.KeyCode == Keys.A)
                player.playerStatus[2] = true;
            if (e.KeyCode == Keys.D)
                player.playerStatus[3] = true;
            if (e.KeyCode == Keys.Space)
            {
                player.ChangeWeapon();
                ChangeImagePlayerWeapon(player.GetWeaponType());
                Console.WriteLine("current weapon : " + player.GetWeaponType());
            }
            if(e.KeyCode == Keys.Escape)
            {
                if (is_TimeElapsed)
                {
                    //timer1.Stop();
                    is_TimeElapsed = false;
                }
                else
                {
                    //timer1.Enabled = true;
                    is_TimeElapsed = true;
                }
            }

            if(must_select)
            {
                if (e.KeyCode == Keys.D1)
                {
                    Selecttype(out must_select, out is_pause);
                }
            }
            /*  cheat code */
            if (e.KeyCode == Keys.R)
            {
                Console.WriteLine("move up");
                GameData.Upgrade_move_up();
            }
            if (e.KeyCode == Keys.T)
            {
                Console.WriteLine("atk speed up");
                GameData.Upgrade_atkSpeed_up();
            }
            if (e.KeyCode == Keys.G)
            {
                GameData.StageUp();
                Console.WriteLine("Stage Up. current stage : " + GameData.GetStage());
            }
        }


        private void toolStripMenuExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void toolStripMenuOption_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms["Option_form"] as Option_form == null)
            {
                Option_form option_form = new Option_form();
                option_form.Show();
            }
        }

        private void Game_Form_MouseUp(object sender, MouseEventArgs e)
        {
            switch (player.GetWeaponType())
            {
                case 0:
                case 1:
                case 2:
                    break;
                case 3:
                    SwordAttack();
                    break;
            }
            player.is_can_move = true;
            player.is_charging = false;
            Console.WriteLine("charged : " + GameData.sword_charge);
            GameData.sword_charge = 0.0;
        }

        private void toolStripMenuMain_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Game_Form_Paint(object sender, PaintEventArgs e)
        {
            if (Ground != null)
            {
                e.Graphics.DrawImage(Ground, 0, 0);
            }
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {

        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            del_drawingImages drawing = (grap) =>
            {
                DrawWeapons(grap);
                DrawEnemy(grap);
                DrawScore(grap);
                DrawPlayer(grap);
            };
            del_CheckColide checkColide = () =>
            {
                CheckPlayerWithEnemy();
                CheckBullet();
            };

            if (is_pause == false)
            {
                g_ground = Graphics.FromImage(Ground);
                g_ground.Clear(Color.White);
                EndAttackTime = DateTime.Now;
                attackTime = (EndAttackTime.Ticks - StartTime.Ticks) / 10000000f;
                CreateEnemy();

                player.PlayerMove();
                UpdateWeapons();
                WeaponCharge();
                MoveEnemy();
                
                Score.CheckNewRecord();

                checkColide();
                //Draw
                drawing(g_ground);

                LevelUp();

            }
            else
            {

            }
            Invalidate();
        }
        private void LevelUp()
        {
            if (GameData.GetKill() > 3 && GameData.GetStage() == 1)
            {
                GameData.StageUp();
                ShowType(out must_select, out is_pause);
            }
        }
        private void ShowType(out bool select, out bool pause)
        {
            select = true;
            pause = true;
        }
        private void Selecttype(out bool select, out bool pause)
        {

            select = false;
            pause = false;
        }
        private void DrawScore(Graphics graphics)
        {
            Font _font = new System.Drawing.Font(new FontFamily("휴먼둥근헤드라인"), 14, FontStyle.Bold);
            graphics.DrawString("New Record : " + Score.getRecordScore().ToString(), _font, Brushes.Black, new PointF(10, 30));
            graphics.DrawString("Record : " + GameData.GetKill().ToString(), _font, Brushes.Black, new PointF(500, 30));
        }
        private void DrawPlayer(Graphics graphics)
        {
            player.angle = (float)GameMath.GetAngle(player.getPositionMid().x, player.getPositionMid().y, PointToClient(MousePosition).X, PointToClient(MousePosition).Y);
            clone_player = (Bitmap)p_player.Clone();
            RotateImage(clone_player, player.angle - 100, out clone_player);
            graphics.DrawImage(clone_player, player.getPositionEdge().x, player.getPositionEdge().y, GameData.player_width, GameData.player_height);
        }
        private void RotateImage(Bitmap bmp, float angle, out Bitmap result)
        {
            Bitmap rotatedImage = new Bitmap(bmp.Width, bmp.Height);

            using (Graphics g = Graphics.FromImage(rotatedImage))
            {
                g.TranslateTransform(bmp.Width / 2, bmp.Height / 2);
                g.RotateTransform(angle);
                g.TranslateTransform(-bmp.Width / 2, -bmp.Height / 2);
                g.DrawImage(bmp, 0, 0, bmp.Width, bmp.Height);
            }
            result = rotatedImage;
        }

        private void WeaponCharge()
        {
            if(player.is_charging)
            {
                GameData.sword_charge += GameMath.dt;
            }
        }

        private void ChangeImagePlayerWeapon(int weaponType)
        {
            if(GameData.ab.is_RareSelected)
            {
                switch (weaponType)
                {
                    case 0:
                        if(GameData.ab.rare_up == WeaponRareUpList.DAGGER)
                            p_player = (Bitmap)p_dagger_d.Clone();
                        else
                            p_player = (Bitmap)p_dagger.Clone();
                        break;
                    case 1:
                        if (GameData.ab.rare_up == WeaponRareUpList.GUN)
                            p_player = (Bitmap)p_gun_d.Clone();
                        else
                            p_player = (Bitmap)p_gun.Clone();
                        break;
                    case 2:
                        if (GameData.ab.rare_up == WeaponRareUpList.RPG)
                            p_player = (Bitmap)p_rpg_d.Clone();
                        else
                            p_player = (Bitmap)p_rpg.Clone();
                        break;
                    case 3:
                        if (GameData.ab.rare_up == WeaponRareUpList.SWORD)
                            p_player = (Bitmap)p_sword.Clone();
                        else
                            p_player = (Bitmap)p_sword.Clone();
                        break;
                }
            }
            else
            {
                switch (weaponType)
                {
                    case 0:
                        p_player = (Bitmap)p_dagger.Clone();
                        break;
                    case 1:
                        p_player = (Bitmap)p_gun.Clone();
                        break;
                    case 2:
                        p_player = (Bitmap)p_rpg.Clone();
                        break;
                    case 3:
                        p_player = (Bitmap)p_sword.Clone();
                        break;
                }
            }
        }
        private void CheckBullet()
        {
            //Todo : check colide bullet with enemy
            //dagger
            foreach (PlayerDagger weapon in player_dagger)
            {
                if (!weapon.exist) continue;
                bulletR = new Rectangle((int)weapon.getPositionEdge().x, (int)weapon.getPositionEdge().y, weapon.getSize().width, weapon.getSize().height);

                foreach (Enemy_normal enemy in enemy_normal)
                {
                    if (!enemy.exist) continue;

                    enemyR = new Rectangle((int)enemy.getPositionEdge().x, (int)enemy.getPositionEdge().y, enemy.getSize().width, enemy.getSize().height);

                    interR = Rectangle.Intersect(bulletR, enemyR);
                    if (!interR.IsEmpty)
                    {
                        enemy.hit(weapon.getDammage());
                        weapon.exist = false;
                    }
                }
                foreach (Enemy_speed enemy in enemy_speed)
                {
                    if (!enemy.exist) continue;

                    enemyR = new Rectangle((int)enemy.getPositionEdge().x, (int)enemy.getPositionEdge().y, enemy.getSize().width, enemy.getSize().height);

                    interR = Rectangle.Intersect(bulletR, enemyR);
                    if (!interR.IsEmpty)
                    {
                        enemy.hit(weapon.getDammage());
                        weapon.exist = false;
                    }
                }
                foreach (Enemy_gun enemy in enemy_gun)
                {
                    if (!enemy.exist) continue;

                    enemyR = new Rectangle((int)enemy.getPositionEdge().x, (int)enemy.getPositionEdge().y, enemy.getSize().width, enemy.getSize().height);

                    interR = Rectangle.Intersect(bulletR, enemyR);
                    if (!interR.IsEmpty)
                    {
                        enemy.hit(weapon.getDammage());
                        weapon.exist = false;
                    }
                }
                foreach (Enemy_shield enemy in enemy_shield)
                {
                    if (!enemy.exist) continue;

                    enemyR = new Rectangle((int)enemy.getPositionEdge().x, (int)enemy.getPositionEdge().y, enemy.getSize().width, enemy.getSize().height);

                    interR = Rectangle.Intersect(bulletR, enemyR);
                    if (!interR.IsEmpty)
                    {
                        enemy.hit(weapon.getDammage());
                        weapon.exist = false;
                    }
                }
            }
            //bullet
            foreach (PlayerBullet weapon in player_bullet)
            {
                if (!weapon.exist) continue;
                bulletR = new Rectangle((int)weapon.getPositionEdge().x, (int)weapon.getPositionEdge().y, weapon.getSize().width, weapon.getSize().height);

                foreach (Enemy_normal enemy in enemy_normal)
                {
                    if (!enemy.exist) continue;

                    enemyR = new Rectangle((int)enemy.getPositionEdge().x, (int)enemy.getPositionEdge().y, enemy.getSize().width, enemy.getSize().height);

                    interR = Rectangle.Intersect(bulletR, enemyR);
                    if (!interR.IsEmpty)
                    {
                        enemy.hit(weapon.getDammage());
                        weapon.exist = false;
                    }
                }
                foreach (Enemy_speed enemy in enemy_speed)
                {
                    if (!enemy.exist) continue;

                    enemyR = new Rectangle((int)enemy.getPositionEdge().x, (int)enemy.getPositionEdge().y, enemy.getSize().width, enemy.getSize().height);

                    interR = Rectangle.Intersect(bulletR, enemyR);
                    if (!interR.IsEmpty)
                    {
                        enemy.hit(weapon.getDammage());
                        weapon.exist = false;
                    }
                }
                foreach (Enemy_gun enemy in enemy_gun)
                {
                    if (!enemy.exist) continue;

                    enemyR = new Rectangle((int)enemy.getPositionEdge().x, (int)enemy.getPositionEdge().y, enemy.getSize().width, enemy.getSize().height);

                    interR = Rectangle.Intersect(bulletR, enemyR);
                    if (!interR.IsEmpty)
                    {
                        enemy.hit(weapon.getDammage());
                        weapon.exist = false;
                    }
                }
                foreach (Enemy_shield enemy in enemy_shield)
                {
                    if (!enemy.exist) continue;

                    enemyR = new Rectangle((int)enemy.getPositionEdge().x, (int)enemy.getPositionEdge().y, enemy.getSize().width, enemy.getSize().height);

                    interR = Rectangle.Intersect(bulletR, enemyR);
                    if (!interR.IsEmpty)
                    {
                        enemy.hit(weapon.getDammage());
                        weapon.exist = false;
                    }
                }
            }
            //rpg
            foreach (PlayerRpg weapon in player_rpg)
            {
                if (!weapon.exist) continue;
                bulletR = new Rectangle((int)weapon.getPositionEdge().x, (int)weapon.getPositionEdge().y, weapon.getSize().width, weapon.getSize().height);

                foreach (Enemy_normal enemy in enemy_normal)
                {
                    if (!enemy.exist) continue;

                    enemyR = new Rectangle((int)enemy.getPositionEdge().x, (int)enemy.getPositionEdge().y, enemy.getSize().width, enemy.getSize().height);

                    interR = Rectangle.Intersect(bulletR, enemyR);
                    if (!interR.IsEmpty)
                    {
                        RpgBombCreate(weapon.getPositionMid());
                        enemy.hit(weapon.getDammage());
                        CheckBomb();
                        weapon.exist = false;
                    }
                }
                foreach (Enemy_speed enemy in enemy_speed)
                {
                    if (!enemy.exist) continue;

                    enemyR = new Rectangle((int)enemy.getPositionEdge().x, (int)enemy.getPositionEdge().y, enemy.getSize().width, enemy.getSize().height);

                    interR = Rectangle.Intersect(bulletR, enemyR);
                    if (!interR.IsEmpty)
                    {
                        RpgBombCreate(weapon.getPositionMid());
                        enemy.hit(weapon.getDammage());
                        CheckBomb();
                        weapon.exist = false;
                    }
                }
                foreach (Enemy_gun enemy in enemy_gun)
                {
                    if (!enemy.exist) continue;

                    enemyR = new Rectangle((int)enemy.getPositionEdge().x, (int)enemy.getPositionEdge().y, enemy.getSize().width, enemy.getSize().height);

                    interR = Rectangle.Intersect(bulletR, enemyR);
                    if (!interR.IsEmpty)
                    {
                        RpgBombCreate(weapon.getPositionMid());
                        enemy.hit(weapon.getDammage());
                        CheckBomb();
                        weapon.exist = false;
                    }
                }
                foreach (Enemy_shield enemy in enemy_shield)
                {
                    if (!enemy.exist) continue;

                    enemyR = new Rectangle((int)enemy.getPositionEdge().x, (int)enemy.getPositionEdge().y, enemy.getSize().width, enemy.getSize().height);

                    interR = Rectangle.Intersect(bulletR, enemyR);
                    if (!interR.IsEmpty)
                    {
                        RpgBombCreate(weapon.getPositionMid());
                        enemy.hit(weapon.getDammage());
                        CheckBomb();
                        weapon.exist = false;
                    }
                }
            }
            //sword
            foreach (PlayerSword weapon in player_sword)
            {
                if (!weapon.exist) continue;
                
                foreach (Enemy_normal enemy in enemy_normal)
                {
                    if (!enemy.exist) continue;

                    if (GameMath.getDistance(player.getPositionMid(), enemy.getPositionMid()) < weapon.get_sword_range() / 2)
                    {
                        enemy.hit(weapon.getDammage());
                    }
                }
                foreach (Enemy_speed enemy in enemy_speed)
                {
                    if (!enemy.exist) continue;

                    if (GameMath.getDistance(player.getPositionMid(), enemy.getPositionMid()) < weapon.get_sword_range() / 2)
                    {
                        enemy.hit(weapon.getDammage());
                    }
                }
                foreach (Enemy_gun enemy in enemy_gun)
                {
                    if (!enemy.exist) continue;

                    enemyR = new Rectangle((int)enemy.getPositionEdge().x, (int)enemy.getPositionEdge().y, enemy.getSize().width, enemy.getSize().height);

                    if (GameMath.getDistance(player.getPositionMid(), enemy.getPositionMid()) < weapon.get_sword_range() / 2)
                    {
                        enemy.hit(weapon.getDammage());
                    }
                }
                foreach (Enemy_shield enemy in enemy_shield)
                {
                    if (!enemy.exist) continue;

                    enemyR = new Rectangle((int)enemy.getPositionEdge().x, (int)enemy.getPositionEdge().y, enemy.getSize().width, enemy.getSize().height);

                    if (GameMath.getDistance(player.getPositionMid(), enemy.getPositionMid()) < weapon.get_sword_range() / 2)
                    {
                        enemy.hit(weapon.getDammage());
                    }
                }
            }

        }
        private void CheckBomb()
        {
            foreach(PlayerRpgBomb weapon in player_rpg_bomb)
            {
                if (!weapon.exist) continue;
                bombR = new Rectangle((int)weapon.getPositionEdge().x, (int)weapon.getPositionEdge().y, weapon.getSize().width, weapon.getSize().height);

                foreach (Enemy_normal enemy in enemy_normal)
                {
                    if (!enemy.exist) continue;

                    //enemyBombR = new Rectangle((int)enemy.getPosition().x, (int)enemy.getPosition().y, enemy.getSize().width, enemy.getSize().height);

                    //interBombR = Rectangle.Intersect(bombR, enemyBombR);
                    //if (!interBombR.IsEmpty)
                    //{
                    //    enemy.hit(weapon.getDammage());
                    //}
                    if (GameMath.getDistance(weapon.getPositionMid(), enemy.getPositionMid()) < weapon.get_bomb_range())
                    {
                        enemy.hit(weapon.getDammage());
                    }
                }
                foreach (Enemy_speed enemy in enemy_speed)
                {
                    if (!enemy.exist) continue;

                    enemyBombR = new Rectangle((int)enemy.getPositionEdge().x, (int)enemy.getPositionEdge().y, enemy.getSize().width, enemy.getSize().height);

                    interBombR = Rectangle.Intersect(bombR, enemyBombR);
                    if (!interBombR.IsEmpty)
                    {
                        enemy.hit(weapon.getDammage());
                    }
                }
                foreach (Enemy_gun enemy in enemy_gun)
                {
                    if (!enemy.exist) continue;

                    enemyBombR = new Rectangle((int)enemy.getPositionEdge().x, (int)enemy.getPositionEdge().y, enemy.getSize().width, enemy.getSize().height);

                    interBombR = Rectangle.Intersect(bombR, enemyBombR);
                    if (!interBombR.IsEmpty)
                    {
                        enemy.hit(weapon.getDammage());
                    }
                }
                foreach (Enemy_shield enemy in enemy_shield)
                {
                    if (!enemy.exist) continue;

                    enemyBombR = new Rectangle((int)enemy.getPositionEdge().x, (int)enemy.getPositionEdge().y, enemy.getSize().width, enemy.getSize().height);

                    interBombR = Rectangle.Intersect(bombR, enemyBombR);
                    if (!interBombR.IsEmpty)
                    {
                        enemy.hit(weapon.getDammage());
                    }
                }
            }
        }
        private void CheckPlayerWithEnemy()
        {
            playerR = new Rectangle((int)player.getPositionEdge().x, (int)player.getPositionEdge().y, GameData.player_width, GameData.player_height);

            foreach(Enemy_normal enemy in enemy_normal)
            {
                if (!enemy.exist) continue;
                
                enemyR = new Rectangle((int)enemy.getPositionEdge().x, (int)enemy.getPositionEdge().y, enemy.getSize().width, enemy.getSize().height);

                interR = Rectangle.Intersect(playerR, enemyR);
                if(!interR.IsEmpty)
                {
                    enemy.AttackSuccess();
                    player.hit();
                }
            }
            foreach (Enemy_speed enemy in enemy_speed)
            {
                if (!enemy.exist) continue;

                enemyR = new Rectangle((int)enemy.getPositionEdge().x, (int)enemy.getPositionEdge().y, enemy.getSize().width, enemy.getSize().height);

                interR = Rectangle.Intersect(playerR, enemyR);
                if (!interR.IsEmpty)
                {
                    enemy.AttackSuccess();
                    player.hit();
                }
            }
            foreach (Enemy_gun enemy in enemy_gun)
            {
                if (!enemy.exist) continue;

                enemyR = new Rectangle((int)enemy.getPositionEdge().x, (int)enemy.getPositionEdge().y, enemy.getSize().width, enemy.getSize().height);

                interR = Rectangle.Intersect(playerR, enemyR);
                if (!interR.IsEmpty)
                {
                    enemy.AttackSuccess();
                    player.hit();
                }
            }
            foreach (Enemy_shield enemy in enemy_shield)
            {
                if (!enemy.exist) continue;

                enemyR = new Rectangle((int)enemy.getPositionEdge().x, (int)enemy.getPositionEdge().y, enemy.getSize().width, enemy.getSize().height);

                interR = Rectangle.Intersect(playerR, enemyR);
                if (!interR.IsEmpty)
                {
                    enemy.AttackSuccess();
                    player.hit();
                }
            }
            //Todo : check colide enemy with bullet or player
        }

        private void playerAttack()
        {
            switch (player.GetWeaponType())
            {
                case 0:
                    DaggerAttack();
                    break;
                case 1:
                    GunAttack();
                    break;
                case 2:
                    RpgAttack();
                    break;
                case 3:
                    SwordAttackCharge();
                    break;
            }
        }

        private void InitWeapons()
        {
            //int i;
            //for (i = 0; i < GameData.MAX_BULLET; i++)
            //    player_bullet[i].exist = false;
            //for (i = 0; i < GameData.MAX_DAGGER; i++)
            //    player_dagger[i].exist = false;
            //for (i = 0; i < GameData.MAX_RPG; i++)
            //    player_rpg[i].exist = false;
            //for (i = 0; i < GameData.MAX_SWORD; i++)
            //    player_sword[i].exist = false;

            //player_bullet.Initialize();
            //player_dagger.Initialize();
            //player_rpg.Initialize();
            //player_sword.Initialize();
        }
        private void UpdateWeapons()
        {
            foreach(PlayerDagger weapon in player_dagger)
            {
                if (weapon.exist == false) continue;
                weapon.moveObject();
            }
            foreach (PlayerBullet weapon in player_bullet)
            {
                if (weapon.exist == false) continue;
                weapon.moveObject();
            }
            foreach (PlayerRpg weapon in player_rpg)
            {
                if (weapon.exist == false) continue;
                weapon.moveObject();
            }
            foreach (PlayerRpgBomb weapon in player_rpg_bomb)
            {
                if (weapon.exist == false) continue;
                weapon.moveObject();
            }
            foreach (PlayerSword weapon in player_sword)
            {
                if (weapon.exist == false) continue;
                weapon.moveObject();
            }

            //for (i = 0; i < GameData.MAX_DAGGER; i++)
            //{
            //    if (player_dagger[i].exist == false) continue;

            //    player_dagger[i].moveObject();
            //}
        }

        private void DaggerAttack()
        {
            //Todo : Dagger Attack (throw)
            int i;
            for (i = 0; i < GameData.MAX_DAGGER; i++)
            {
                if (player_dagger[i].exist == false)
                    break;
            }
            if (i != GameData.MAX_DAGGER)
            {
                o_Vector direction;
                direction.x = PointToClient(MousePosition).X - (player.getPositionMid().x);
                direction.y = PointToClient(MousePosition).Y - (player.getPositionMid().y);
                player_dagger[i].exist = true;
                player_dagger[i].SetObjectPosition(player.getPositionMid().x, player.getPositionMid().y);
                player_dagger[i].SetDirection(direction);
                player_dagger[i].is_can_rotate = true;
            }
        }
        private void GunAttack()
        {
            int i;
            for (i = 0; i < GameData.MAX_BULLET; i++)
            {
                if (player_bullet[i].exist == false)
                    break;
            }

            if (i != GameData.MAX_BULLET)
            {
                o_Vector direction;
                direction.x = PointToClient(MousePosition).X - (player.getPositionMid().x);
                direction.y = PointToClient(MousePosition).Y - (player.getPositionMid().y);
                player_bullet[i].exist = true;
                player_bullet[i].SetObjectPosition(player.getPositionMid().x, player.getPositionMid().y);
                player_bullet[i].SetDirection(direction);
            }
        }
        private void RpgAttack()
        {
            int i;
            for (i = 0; i < GameData.MAX_RPG; i++)
            {
                if (player_rpg[i].exist == false)
                    break;
            }

            if (i != GameData.MAX_RPG)
            {
                o_Vector direction;
                direction.x = PointToClient(MousePosition).X - player.getPositionMid().x;
                direction.y = PointToClient(MousePosition).Y - player.getPositionMid().y;
                player_rpg[i].exist = true;
                player_rpg[i].SetObjectPosition(player.getPositionMid().x, player.getPositionMid().y);
                player_rpg[i].SetDirection(direction);
            }
        }
        private void RpgBombCreate(o_Point create_potision)
        {
            int i;
            for (i = 0; i < GameData.MAX_BOMB; i++)
            {
                if (player_rpg_bomb[i].exist == false)
                    break;
            }

            if (i != GameData.MAX_BOMB)
            {
                player_rpg_bomb[i].exist = true;
                player_rpg_bomb[i].SetObjectPosition(create_potision.x - player_rpg_bomb[i].m_size.width / 2 ,
                                                     create_potision.y - player_rpg_bomb[i].m_size.height / 2);
            }
        }
        private void SwordAttack()
        {
            int i;
            for (i = 0; i < GameData.MAX_SWORD; i++)
            {
                if (player_sword[i].exist == false)
                    break;
            }

            if (i != GameData.MAX_SWORD)
            {
                player_sword[i].exist = true;
                player_sword[i].SetObjectPosition(player.getPositionMid().x, player.getPositionMid().y);
                player_sword[i].Charging(GameData.sword_charge);
                Console.WriteLine("sword lange : " + player_sword[i].sword_range);
            }
        }
        private void SwordAttackCharge()
        {
            player.is_can_move = false;
            player.is_charging = true;
        }

        private void DrawWeapons(Graphics graphics)
        {
            int i;
            var bullet_list = from weapons in player_bullet
                              where weapons.exist == true
                              select weapons;
            var rpg_list = from weapons in player_rpg
                           where weapons.exist == true
                              select weapons;
            var bomb_list = from weapons in player_rpg_bomb
                            where weapons.exist == true
                              select weapons;
            var sword_list = from weapons in player_sword
                             where weapons.exist == true
                              select weapons;
            //draw
            for (i = 0; i < GameData.MAX_DAGGER; i++)
            {
                if (player_dagger[i].exist == false) continue;

                if (player_dagger[i].is_can_rotate == true)
                {
                    player_dagger[i].angle = (float)GameMath.GetAngle(player.getPositionMid().x, player.getPositionMid().y, PointToClient(MousePosition).X, PointToClient(MousePosition).Y);
                    clone_dagger[i] = (Bitmap)dagger.Clone();
                    RotateImage(clone_dagger[i], player_dagger[i].angle - 90, out clone_dagger[i]);
                    player_dagger[i].is_can_rotate = false;
                }
                graphics.DrawImage(clone_dagger[i], player_dagger[i].getPositionEdge().x, player_dagger[i].getPositionEdge().y, GameData.dagger_width, GameData.dagger_height);
            }
            foreach (PlayerBullet weapons in bullet_list)
            {
                graphics.DrawImage(bullet, weapons.getPositionEdge().x, weapons.getPositionEdge().y, GameData.bullet_width, GameData.bullet_height);
            }
            foreach (PlayerRpg weapons in rpg_list)
            {
                graphics.DrawImage(bullet, weapons.getPositionEdge().x, weapons.getPositionEdge().y, GameData.rpg_bullet_width, GameData.rpg_buttet_height);
            }
            foreach(PlayerRpgBomb weapons in bomb_list)
            {
                graphics.DrawImage(bomb, weapons.getPositionEdge().x, weapons.getPositionEdge().y, GameData.rpg_bomb_width, GameData.rpg_bomb_height);
            }
            foreach (PlayerSword weapons in sword_list)
            {
                graphics.DrawImage(sword_range, weapons.getPositionEdge().x - (int)weapons.sword_range / 2, weapons.getPositionEdge().y - (int)weapons.sword_range / 2, (int)weapons.sword_range, (int)weapons.sword_range);
            }
        }

        private void CreateEnemy()
        {
            if (GameData.GetStage() >= GameData.lv1_stage) CreateEnemy_normal();
            if (GameData.GetStage() >= GameData.lv2_stage) CreateEnemy_speed();
            if (GameData.GetStage() >= GameData.lv3_stage) CreateEnemy_gun();
            if (GameData.GetStage() >= GameData.lv4_stage) CreateEnemy_shield();
        }
        private void CreateEnemy_normal()
        {
            int i;
            if (random.Next(20) == 0)
            {
                for (i = 0; i < GameData.MAX_ENEMY_NORMAL && enemy_normal[i].exist == true; i++)
                {
                }

                if (i != GameData.MAX_ENEMY_NORMAL)
                {
                    int random_position = random.Next(4);
                    o_Point create_position = new o_Point();
                    switch (random_position)
                    {
                        //left
                        case 0:
                            create_position.x = 0;
                            create_position.y = random.Next(GameData.FormSize_Height);
                            break;
                        //right
                        case 1:
                            create_position.x = GameData.FormSize_Width - GameData.enemy_width;
                            create_position.y = random.Next(GameData.FormSize_Height);
                            break;
                        //up
                        case 2:
                            create_position.x = random.Next(GameData.FormSize_Width);
                            create_position.y = 0;
                            break;
                        //down
                        case 3:
                            create_position.x = random.Next(GameData.FormSize_Width);
                            create_position.y = GameData.FormSize_Height - GameData.enemy_height;
                            break;
                    }
                    enemy_normal[i].m_position = create_position;
                    enemy_normal[i].SetHP();
                    enemy_normal[i].exist = true;
                }
            }
        }
        private void CreateEnemy_speed()
        {
            int i;
            if (random.Next(20) == 0)
            {
                for (i = 0; i < GameData.MAX_ENEMY_SPEED && enemy_speed[i].exist == true; i++)
                {
                }

                if (i != GameData.MAX_ENEMY_SPEED)
                {
                    int random_position = random.Next(4);
                    o_Point create_position = new o_Point();
                    switch (random_position)
                    {
                        //left
                        case 0:
                            create_position.x = 0;
                            create_position.y = random.Next(GameData.FormSize_Height);
                            break;
                        //right
                        case 1:
                            create_position.x = GameData.FormSize_Width - GameData.enemy_width;
                            create_position.y = random.Next(GameData.FormSize_Height);
                            break;
                        //up
                        case 2:
                            create_position.x = random.Next(GameData.FormSize_Width);
                            create_position.y = 0;
                            break;
                        //down
                        case 3:
                            create_position.x = random.Next(GameData.FormSize_Width);
                            create_position.y = GameData.FormSize_Height - GameData.enemy_height;
                            break;
                    }
                    enemy_speed[i].m_position = create_position;
                    enemy_speed[i].SetHP();
                    enemy_speed[i].exist = true;
                }
            }
        }
        private void CreateEnemy_gun()
        {
            int i;
            if (random.Next(20) == 0)
            {
                for (i = 0; i < GameData.MAX_ENEMY_GUN && enemy_gun[i].exist == true; i++)
                {
                }

                if (i != GameData.MAX_ENEMY_GUN)
                {
                    int random_position = random.Next(4);
                    o_Point create_position = new o_Point();
                    switch (random_position)
                    {
                        //left
                        case 0:
                            create_position.x = 0;
                            create_position.y = random.Next(GameData.FormSize_Height);
                            break;
                        //right
                        case 1:
                            create_position.x = GameData.FormSize_Width - GameData.enemy_width;
                            create_position.y = random.Next(GameData.FormSize_Height);
                            break;
                        //up
                        case 2:
                            create_position.x = random.Next(GameData.FormSize_Width);
                            create_position.y = 0;
                            break;
                        //down
                        case 3:
                            create_position.x = random.Next(GameData.FormSize_Width);
                            create_position.y = GameData.FormSize_Height - GameData.enemy_height;
                            break;
                    }
                    enemy_gun[i].m_position = create_position;
                    enemy_gun[i].SetHP();
                    enemy_gun[i].exist = true;
                }
            }
        }
        private void CreateEnemy_shield()
        {
            int i;
            if (random.Next(20) == 0)
            {
                for (i = 0; i < GameData.MAX_ENEMY_SHIELD && enemy_shield[i].exist == true; i++)
                {
                }

                if (i != GameData.MAX_ENEMY_SHIELD)
                {
                    int random_position = random.Next(4);
                    o_Point create_position = new o_Point();
                    switch (random_position)
                    {
                        //left
                        case 0:
                            create_position.x = 0;
                            create_position.y = random.Next(GameData.FormSize_Height);
                            break;
                        //right
                        case 1:
                            create_position.x = GameData.FormSize_Width - GameData.enemy_width;
                            create_position.y = random.Next(GameData.FormSize_Height);
                            break;
                        //up
                        case 2:
                            create_position.x = random.Next(GameData.FormSize_Width);
                            create_position.y = 0;
                            break;
                        //down
                        case 3:
                            create_position.x = random.Next(GameData.FormSize_Width);
                            create_position.y = GameData.FormSize_Height - GameData.enemy_height;
                            break;
                    }
                    enemy_shield[i].m_position = create_position;
                    enemy_shield[i].SetHP();
                    enemy_shield[i].exist = true;
                }
            }
        }

        private void MoveEnemy()
        {
            var enemy_normal_list = from enemyies in enemy_normal
                                      where enemyies.exist == true
                                      select enemyies;
            var enemy_speed_list =  from enemyies in enemy_speed
                                    where enemyies.exist == true
                                      select enemyies;
            var enemy_gun_list = from enemyies in enemy_gun
                                 where enemyies.exist == true
                                   select enemyies;
            var enemy_shield_list = from enemyies in enemy_shield
                                    where enemyies.exist == true
                                   select enemyies;
            if (GameData.GetStage() >= GameData.lv1_stage)
                foreach (Enemy_normal enemy in enemy_normal_list)
                {
                    enemy.moveObject(player.getPositionMid());
                }
            if (GameData.GetStage() >= GameData.lv2_stage)
                foreach (Enemy_speed enemy in enemy_speed_list)
                {
                    enemy.moveObject(player.getPositionMid());
                }
            if (GameData.GetStage() >= GameData.lv3_stage)
                foreach (Enemy_gun enemy in enemy_gun_list)
                {
                    enemy.moveObject(player.getPositionMid());
                }
            if (GameData.GetStage() >= GameData.lv4_stage)
                foreach (Enemy_shield enemy in enemy_shield_list)
                {
                    enemy.moveObject(player.getPositionMid());
                }
        }
        private void DrawEnemy(Graphics graphics)
        {
            int i;
            if(GameData.GetStage() >= GameData.lv1_stage)
                for(i = 0; i < GameData.MAX_ENEMY_NORMAL; i++)
                {
                    if (enemy_normal[i].exist == false) continue;
                    enemy_normal[i].angle = (float)GameMath.GetAngle(enemy_normal[i].getPositionMid().x, enemy_normal[i].getPositionMid().y, player.getPositionMid().x, player.getPositionMid().y);
                    clone_e_normal[i] = (Bitmap)e_normal.Clone();
                    RotateImage(clone_e_normal[i], enemy_normal[i].angle - 90, out clone_e_normal[i]);
                    graphics.DrawImage(clone_e_normal[i], enemy_normal[i].getPositionEdge().x, enemy_normal[i].getPositionEdge().y, enemy_normal[i].m_size.width, enemy_normal[i].m_size.height);
                }
            if (GameData.GetStage() >= GameData.lv2_stage)
                for (i = 0; i < GameData.MAX_ENEMY_SPEED; i++)
                {
                    if (enemy_speed[i].exist == false) continue;
                    enemy_speed[i].angle = (float)GameMath.GetAngle(enemy_speed[i].getPositionMid().x, enemy_speed[i].getPositionMid().y, player.getPositionMid().x, player.getPositionMid().y);
                    clone_e_speed[i] = (Bitmap)e_speed.Clone();
                    RotateImage(clone_e_speed[i], enemy_speed[i].angle - 90, out clone_e_speed[i]);
                    graphics.DrawImage(clone_e_speed[i], enemy_speed[i].getPositionEdge().x, enemy_speed[i].getPositionEdge().y, enemy_speed[i].m_size.width, enemy_speed[i].m_size.height);
                }

            if (GameData.GetStage() >= GameData.lv3_stage)
                for (i = 0; i < GameData.MAX_ENEMY_GUN; i++)
                {
                    if (enemy_gun[i].exist == false) continue;
                    enemy_gun[i].angle = (float)GameMath.GetAngle(enemy_gun[i].getPositionMid().x, enemy_gun[i].getPositionMid().y, player.getPositionMid().x, player.getPositionMid().y);
                    clone_e_gun[i] = (Bitmap)e_gun.Clone();
                    RotateImage(clone_e_gun[i], enemy_gun[i].angle - 90, out clone_e_gun[i]);
                    graphics.DrawImage(clone_e_gun[i], enemy_gun[i].getPositionEdge().x, enemy_gun[i].getPositionEdge().y, enemy_gun[i].m_size.width, enemy_gun[i].m_size.height);
                }
            if (GameData.GetStage() >= GameData.lv4_stage)
                for (i = 0; i < GameData.MAX_ENEMY_SHIELD; i++)
                {
                    if (enemy_shield[i].exist == false) continue;
                    enemy_shield[i].angle = (float)GameMath.GetAngle(enemy_shield[i].getPositionMid().x, enemy_shield[i].getPositionMid().y, player.getPositionMid().x, player.getPositionMid().y);
                    clone_e_shield[i] = (Bitmap)e_shield.Clone();
                    RotateImage(clone_e_shield[i], enemy_shield[i].angle - 90, out clone_e_shield[i]);
                    graphics.DrawImage(clone_e_shield[i], enemy_shield[i].getPositionEdge().x, enemy_shield[i].getPositionEdge().y, enemy_shield[i].m_size.width, enemy_shield[i].m_size.height);
                }
        }

    }
}
