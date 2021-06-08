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

        Bitmap Ground = new Bitmap(1200, 800);
        Bitmap bomb, bullet, e_normal, e_speed, e_gun, e_shield;
        Bitmap p_dagger, p_dagger_d, p_gun, p_gun_d, p_rpg, p_rpg_d, p_sword, sword_range;
        Player player;


        [DllImport("User32.dll")]
        private static extern short GetKeyState(int nVirtKey);
        [DllImport("winmm.dll")]
        private static extern long mciSendString(string strCommand, StringBuilder strReturn, int iReturnLength, IntPtr hwndCallback);


        public Game_Form()
        {
            InitializeComponent();
        }
        private void Game_Form_Load(object sender, EventArgs e)
        {
            this.Size = new Size(800, 800);

            InitDatas();

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

            GameData.init();
            timer1.Start();
            Console.WriteLine("Load Data!\n");
        }
        private void Game_Form_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    player.PlayerMoveUp(GameData.getPlayerSpeed());
                    break;
                case Keys.A:
                    player.PlayerMoveLeft(GameData.getPlayerSpeed());
                    break;
                case Keys.S:
                    player.PlayerMoveDown(GameData.getPlayerSpeed());
                    break;
                case Keys.D:
                    player.PlayerMoveRight(GameData.getPlayerSpeed());
                    break;
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
            g.DrawImage(bomb, ClientSize.Width / 2, ClientSize.Height /2);
            Invalidate();

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
