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

namespace Infinite_War
{
    public partial class Game_Form : Form
    {
        DateTime StartTime = DateTime.Now, EndAttackTime = DateTime.Now;
        float deltaTime, attackTime;
        bool is_TimeElapsed;
        Bitmap Ground = new Bitmap(1200, 800);
        Bitmap bomb, bullet, e_normal, e_speed, e_gun, e_shield;
        Bitmap p_player, p_dagger, p_dagger_d, p_gun, p_gun_d, p_rpg, p_rpg_d, p_sword, sword_range;
        Player player;

        Enemy_normal[] enemy_normal = new Enemy_normal[GameData.MAX_ENEMY_NORMAL];
        Enemy_speed[]  enemy_speed  = new Enemy_speed[GameData.MAX_ENEMY_SPEED];
        Enemy_gun[]    enemy_gun    = new Enemy_gun[GameData.MAX_ENEMY_GUN];
        Enemy_shield[] enemy_shield = new Enemy_shield[GameData.MAX_ENEMY_SHIELD];

        PlayerDagger[] player_dagger = new PlayerDagger[GameData.MAX_DAGGER];
        PlayerBullet[] player_bullet = Untility.InitializeArray<PlayerBullet>(GameData.MAX_BULLET);//Enumerable.Repeat(0, GameData.MAX_BULLET).Select(b => new PlayerBullet()).ToArray();//new PlayerBullet[GameData.MAX_BULLET];
        PlayerRpg[]    player_rpg    = Untility.InitializeArray<PlayerRpg>(GameData.MAX_RPG);//new PlayerRpg[GameData.MAX_RPG];

        private delegate void AttackDelegate();

        Bitmap clone_player;


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
                //player.Attack();
                StartTime = EndAttackTime;
            }

            Console.WriteLine("player position : x : " + player.getPosition().x + " y : " + player.getPosition().y);
            //Console.WriteLine("mouse position : x : " + PointToClient(MousePosition).X + " y : " + PointToClient(MousePosition).Y);
            //Console.WriteLine("dt : " + GameMath.dt);
            Console.WriteLine("player speed : " + player.speed);
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
            GameData.init();


            InitWeapons();
            is_TimeElapsed = true;
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
            Graphics g = Graphics.FromImage(Ground);
            g.Clear(Color.Black);
            EndAttackTime = DateTime.Now;
            deltaTime  = (EndAttackTime.Ticks - StartTime.Ticks) / 10000000f;
            attackTime = (EndAttackTime.Ticks - StartTime.Ticks) / 10000000f;
            player.PlayerMove();

            UpdateWeapons();


            Rectangle rec_player = new Rectangle((int)player.getPosition().x, (int)player.getPosition().y, GameData.player_width, GameData.player_height);


            //Draw
            g.DrawImage(bomb, ClientSize.Width / 2, ClientSize.Height / 2, 50, 50);

            player.angle = (float)GameMath.GetAngle(player.getPosition().x + GameData.player_offset_x, player.getPosition().y + GameData.player_offset_y, PointToClient(MousePosition).X, PointToClient(MousePosition).Y);
            clone_player = (Bitmap)p_player.Clone();
            clone_player = RotateImage(clone_player, player.angle - 90);
            g.DrawImage(clone_player, player.getPosition().x, player.getPosition().y, GameData.player_width, GameData.player_height);
            DrawWeapons(g);
            //Todo : Draw images to delegate.!

            //pause
            //if (!is_TimeElapsed)
            //{
            //    Font _font = new System.Drawing.Font(new FontFamily("휴먼둥근헤드라인"), 20, FontStyle.Bold);
            //    g.DrawString("PAUSE", _font, Brushes.DarkBlue, new PointF(600, 200));
            //    if(ModifierKeys == Keys.Escape)
            //    {
            //        timer1.Enabled = true;
            //        is_TimeElapsed = true;
            //    }
            //}



            Invalidate();
        }


        private Bitmap RotateImage(Bitmap bmp, float angle)
        {
            Bitmap rotatedImage = new Bitmap(bmp.Width, bmp.Height);
            rotatedImage.SetResolution(GameData.player_width, GameData.player_height);

            using (Graphics g = Graphics.FromImage(rotatedImage))
            {
                g.TranslateTransform(bmp.Width / 2, bmp.Height / 2);
                g.RotateTransform(angle);
                g.TranslateTransform(-bmp.Width / 2, -bmp.Height / 2);
                g.DrawImage(bmp, 0, 0, bmp.Width, bmp.Height);
            }
            return rotatedImage;
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
        }
        private void CheckEnemy()
        {
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
                    SwordAttack();
                    break;
            }
        }

        private void InitWeapons()
        {
            for (int i = 0; i < GameData.MAX_BULLET; i++)
                player_bullet[i].exist = false;
            player_bullet.Initialize();
        }
        private void UpdateWeapons()
        {
            int i;
            for (i = 0; i < GameData.MAX_BULLET; i++)
            {
                if (player_bullet[i].exist == false) continue;

                player_bullet[i].moveObject();
            }
        }

        private void DaggerAttack()
        {
            //Todo : Dagger Attack (throw)
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
                direction.x = PointToClient(MousePosition).X - (player.getPosition().x + GameData.player_offset_x);
                direction.y = PointToClient(MousePosition).Y - (player.getPosition().y + GameData.player_offset_y);
                player_bullet[i].exist = true;
                player_bullet[i].SetObject(player.getPosition().x + GameData.player_offset_x, player.getPosition().y + GameData.player_offset_y);
                player_bullet[i].SetDirection(direction);
            }
        }
        private void RpgAttack()
        {
            //Todo : RpgAttack
        }
        private void SwordAttack()
        {
            //Todo : Sword Attack
        }

        private void DrawWeapons(Graphics graphics)
        {
            int i;
            //draw
            for (i = 0; i < GameData.MAX_BULLET; i++)
            {
                if (player_bullet[i].exist == false) continue;

                //player_bullet[i].moveObject();
                graphics.DrawImage(bullet, player_bullet[i].getPosition().x, player_bullet[i].getPosition().y, GameData.bullet_width, GameData.bullet_height);
            }
        }


    }
}
