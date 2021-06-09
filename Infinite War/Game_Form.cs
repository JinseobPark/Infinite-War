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
        Bitmap Ground = new Bitmap(1200, 800);
        Bitmap bomb, bullet, e_normal, e_speed, e_gun, e_shield;
        Bitmap p_dagger, p_dagger_d, p_gun, p_gun_d, p_rpg, p_rpg_d, p_sword, sword_range;
        Player player;

        Bitmap c_dagger;


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
                player.Attack();
                StartTime = EndAttackTime;
            }
            Console.WriteLine("player position : x : " + player.getPosition().x + " y : " + player.getPosition().y);
            //Console.WriteLine("mouse position : x : " + PointToClient(MousePosition).X + " y : " + PointToClient(MousePosition).Y);
            
        }

        public Game_Form()
        {
            InitializeComponent();
            timer1.Tick += new System.EventHandler(this.timer1_Tick);
        }
        private void Game_Form_Load(object sender, EventArgs e)
        {
            this.Size = new Size(GameData.FormSize_Width, GameData.FormSize_Height);

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
            p_dagger = Resource.player_dagger;
            p_dagger_d = Resource.player_dagger_double;
            p_gun = Resource.player_gun;
            p_gun_d = Resource.player_gun_double;
            p_rpg = Resource.player_rpg;
            p_rpg_d = Resource.player_rpg_up;
            p_sword = Resource.player_sword;
            sword_range = Resource.sword_range;
            c_dagger = Resource.player_dagger;
            GameData.init();
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
                Console.WriteLine("current weapon : " + player.GetWeaponType());
            }

            /*  cheat code */
            if (e.KeyCode == Keys.R)
            {
                Console.WriteLine("move up");
                GameData.Upgrade_move_up();
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
            g.Clear(Color.White);
            EndAttackTime = DateTime.Now;
            deltaTime  = (EndAttackTime.Ticks - StartTime.Ticks) / 10000000f;
            attackTime = (EndAttackTime.Ticks - StartTime.Ticks) / 10000000f;
            player.PlayerMove();
            c_dagger = (Bitmap)p_dagger.Clone();

            Rectangle rec_player = new Rectangle((int)player.getPosition().x + GameData.player_offset_x, (int)player.getPosition().y + GameData.player_offset_y, GameData.player_width, GameData.player_height);

            g.DrawImage(bomb, ClientSize.Width / 2, ClientSize.Height / 2, 50, 50);

            player.angle = (float)GameMath.GetAngle(player.getPosition().x + GameData.player_offset_x, player.getPosition().y + GameData.player_offset_y, PointToClient(MousePosition).X, PointToClient(MousePosition).Y);
            c_dagger = RotateImage(c_dagger, player.angle - 90);
            g.DrawImage(c_dagger, player.getPosition().x, player.getPosition().y, GameData.player_width, GameData.player_height);
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
        private void CheckBullet()
        {
            //Todo : check colide bullet with enemy
        }
        private void CheckEnemy()
        {
            //Todo : check colide enemy with bullet or player
        }


    }
}
