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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Infinite_War
{
    public partial class Game_Form : Form
    {
        //공격 쿨타임을 위해 시간을 이용
        DateTime StartTime = DateTime.Now, EndAttackTime = DateTime.Now;
        float attackTime;
        //게임의 현재 상태를 나타냄.
        bool is_pause, must_select, is_gaming;
        //그림 리소스
        Bitmap Ground = new Bitmap(GameData.FormSize_Width, GameData.FormSize_Height);
        Bitmap bomb, bullet, dagger, e_normal, e_speed, e_gun, e_shield, e_bullet;
        Bitmap p_player, p_dagger, p_dagger_d, p_gun, p_gun_d, p_rpg, p_rpg_d, p_sword, sword_range;
        //플레이어
        Player player;
        //적 오브젝트
        Enemy_normal[] enemy_normal;
        Enemy_speed[]  enemy_speed;
        Enemy_gun[]    enemy_gun;
        EnemyBullet[]  enemy_bullet;
        Enemy_shield[] enemy_shield;
        //무기 오브젝트
        PlayerDagger[] player_dagger;
        PlayerBullet[] player_bullet;
        PlayerRpg[]    player_rpg;
        PlayerSword[]  player_sword;
        PlayerRpgBomb[] player_rpg_bomb;
        //회전을 위한 클론 그림들
        Bitmap clone_player;
        Bitmap[] clone_dagger;
        Bitmap[] clone_e_normal, clone_e_speed, clone_e_gun, clone_e_shield;

        Random random;
        Graphics g_ground;
        //사운드 리소스
        SoundPlayer s_dagger, s_dagger_hit, s_gun, s_gun_hit, s_rpg, s_rpg_hit, s_sword, s_sword_hit;
        SoundPlayer s_player_hit, s_switch, s_select;
        //충돌체크 오브젝트
        Rectangle playerR, bulletR, enemyR, interR, enemy_bulletR;

        //델리게이터
        public delegate void del_drawingImages(Graphics graphic);
        public delegate void del_CheckColide();
        public delegate void del_moves();

        //능력 선택을 위한 도구
        string ab_star_1, ab_star_2, ab_star_3;
        string ab_name_1, ab_name_2, ab_name_3;
        string ab_value_1, ab_value_2 , ab_value_3;
        string ab_add_1, ab_add_2, ab_add_3;
        int ab_code_1 = 0, ab_code_2 = 0, ab_code_3 = 0;

        //사운드 체커
        int volume_bgm;
        bool is_bgm_sound, is_eff_sound;

        //dll사용
        [DllImport("User32.dll")]
        private static extern short GetKeyState(int nVirtKey);
        [DllImport("winmm.dll")]
        private static extern long mciSendString(string strCommand, StringBuilder strReturn, int iReturnLength, IntPtr hwndCallback);


        public Game_Form()
        {
            InitializeComponent();
        }
        private void Game_Form_Load(object sender, EventArgs e)//Form Load
        {
            Size = new Size(GameData.FormSize_Width, GameData.FormSize_Height); //Form size 설정

            InitDatas(); //데이터 초기화
        }
        /*************************************
         * Init Data
         * ***********************************/
        private void InitDatas() //초기 데이터 세팅
        {
            //플레이어
            player = new Player(GameData.FormSize_Width / 2, GameData.FormSize_Height / 2, GameData.player_width, GameData.player_height);
            //이미지 리소스
            bomb = Resource.bomb;
            bullet = Resource.bullet;
            dagger = Resource.dagger;
            e_normal = Resource.enemy_normal;
            e_speed = Resource.enemy_speed;
            e_gun = Resource.enemy_gun;
            e_shield = Resource.enemy_shield;
            e_bullet = Resource.enemy_bullet;
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
            //오브젝트들 (제네릭 이용)
            enemy_normal  = Untility.Init_array<Enemy_normal>(GameData.MAX_ENEMY_NORMAL);
            enemy_speed   = Untility.Init_array<Enemy_speed>(GameData.MAX_ENEMY_SPEED);
            enemy_gun     = Untility.Init_array<Enemy_gun>(GameData.MAX_ENEMY_GUN);
            enemy_shield  = Untility.Init_array<Enemy_shield>(GameData.MAX_ENEMY_SHIELD);
            enemy_bullet  = Untility.Init_array<EnemyBullet>(GameData.MAX_ENEMY_BULLET);
            player_dagger = Untility.Init_array<PlayerDagger>(GameData.MAX_DAGGER);
            player_bullet = Untility.Init_array<PlayerBullet>(GameData.MAX_BULLET);
            player_rpg    = Untility.Init_array<PlayerRpg>(GameData.MAX_RPG);
            player_sword  = Untility.Init_array<PlayerSword>(GameData.MAX_SWORD);
            player_rpg_bomb = Untility.Init_array<PlayerRpgBomb>(GameData.MAX_BOMB);
            //이미지 회전을 위한 클론 
            clone_dagger  = new Bitmap[GameData.MAX_DAGGER];
            clone_e_normal = new Bitmap[GameData.MAX_ENEMY_NORMAL];
            clone_e_speed = new Bitmap[GameData.MAX_ENEMY_SPEED];
            clone_e_gun = new Bitmap[GameData.MAX_ENEMY_GUN];
            clone_e_shield = new Bitmap[GameData.MAX_ENEMY_SHIELD];
            //사운드 리소스
            s_dagger = new SoundPlayer(Resource.s_dagger);
            s_dagger_hit = new SoundPlayer(Resource.s_dagger_hit);
            s_gun = new SoundPlayer(Resource.s_gun);
            s_gun_hit = new SoundPlayer(Resource.s_gun_hit);
            s_rpg = new SoundPlayer(Resource.s_rpg);
            s_rpg_hit = new SoundPlayer(Resource.s_rpg_hit);
            s_sword = new SoundPlayer(Resource.s_sword);
            s_sword_hit = new SoundPlayer(Resource.s_sword_hit);
            s_player_hit = new SoundPlayer(Resource.s_player_hit);
            s_switch = new SoundPlayer(Resource.s_switch_weapon);
            s_select = new SoundPlayer(Resource.s_select);

            //옵션에서 사운드 상태 가져오기
            is_bgm_sound = Option.GetBgmSound();
            is_eff_sound = Option.GetEffSound();
            SetBgmSound(is_bgm_sound, out volume_bgm);

            random = new Random(); //랜덤요소 이용
            GameData.init(); //게임 내 수치 초기화
            InitBgmSound(volume_bgm); //BGM 시작
            InitAbilities(); //능력선택 텍스트 초기화
            LevelUpBox.init(); //능력 수치 추가
            //게임 상태 초기화
            is_pause = false;
            must_select = false;
            is_gaming = true;

            timer1.Start();

            Console.WriteLine("Load Data!\n");
        }
        private void InitBgmSound(int volume)//bgm 초기 세팅
        {
            try //mp3파일 가져오기 시도
            {
                mciSendString("open \"" + "../../../Resources/InfiniteWar_BGM.mp3" + "\" type mpegvideo alias MediaFile", null, 0, IntPtr.Zero);
                mciSendString("play MediaFile REPEAT", null, 0, IntPtr.Zero);
                mciSendString("setaudio MediaFile volume to " + volume.ToString(), null, 0, IntPtr.Zero);
            }
            catch (Exception ex) //실패시 catch 
            {
                string message = "Exception Type : " + ex.GetType() + "\nMessage : " + ex.Message + "\nStack Trace : " + ex.StackTrace + "\n\n";
                using (FileStream fs = new FileStream("../../../error.log", FileMode.Append)) //log저장
                {
                    string time = "Error Time : " + DateTime.Now.ToString() + "\n";
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, time + message);
                    fs.Close();
                }
                throw;
            }
        }
        private void InitAbilities() //능력 선택카드 초기화
        {
            ab_star_1 = ""; ab_star_2 = ""; ab_star_3 = "";
            ab_name_1 = ""; ab_name_2 = ""; ab_name_3 = "";
            ab_value_1 = ""; ab_value_2 = ""; ab_value_3 = "";
            ab_add_1 = ""; ab_add_2 = ""; ab_add_3 = "9";
            ab_code_1 = 0; ab_code_2 = 0; ab_code_3 = 0;
        }

        /*************************************
         * Player Control
         * ***********************************/
        private void Game_Form_KeyUp(object sender, KeyEventArgs e) //플레이어 이동 키업 (키를 떼면 이동을 멈춤)
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
        private void Game_Form_KeyDown(object sender, KeyEventArgs e) //플레이어 이동 키다운 (키 누르면 이동)
        {
            if (e.KeyCode == Keys.W)
                player.playerStatus[0] = true;
            if (e.KeyCode == Keys.S)
                player.playerStatus[1] = true;
            if (e.KeyCode == Keys.A)
                player.playerStatus[2] = true;
            if (e.KeyCode == Keys.D)
                player.playerStatus[3] = true;
            if (e.KeyCode == Keys.Space) //space키는 무기 변경
            {
                player.ChangeWeapon(); //무기변경
                ChangeImagePlayerWeapon(player.GetWeaponType()); //바뀐 무기에 따라 플레이어 이미지도 변경
                if (is_eff_sound) s_switch.Play(); //무기 전환 사운드
            }
            if(e.KeyCode == Keys.Escape) // ESC키는 Pause, exit
            {
                if(!is_gaming) //플레이어가 죽었을 시, 창 닫기
                {
                    this.Close();
                }
                else
                {
                    if (!is_pause && !must_select) //게임 중일때, 퍼즈 가능
                    {
                        is_pause = true;
                        must_select = false;
                    }
                    else if (is_pause && !must_select) //퍼즈 상태일 때, 게임으로 전환
                    {
                        is_pause = false;
                        must_select = false;
                    }
                }
            }

            if(must_select)  //능력 선택해야 할 때
            {
                if (e.KeyCode == Keys.D1) //1번 능력 선택
                {
                    LevelUpBox.ChooseAbility(ab_code_1, player);
                    InitAbilities(); //선택했다면 선택 초기화
                    Selecttype(out must_select, out is_pause); //게임 진행
                }
                if (e.KeyCode == Keys.D2) //2번 능력 선택
                {
                    LevelUpBox.ChooseAbility(ab_code_2, player);
                    InitAbilities(); //선택했다면 선택 초기화
                    Selecttype(out must_select, out is_pause); //게임 진행
                }
                if (e.KeyCode == Keys.D3) //3번 능력 선택
                {
                    LevelUpBox.ChooseAbility(ab_code_3, player);
                    InitAbilities(); //선택했다면 선택 초기화
                    Selecttype(out must_select, out is_pause); //게임 진행
                }
            }
            /*  cheat code */
            if (e.KeyCode == Keys.R) //R키는 이동속도 향상
            {
                Console.WriteLine("move up");
                GameData.Upgrade_move_up(10);
            }
            if (e.KeyCode == Keys.T) //T키는 공격속도 향상
            {
                Console.WriteLine("atk speed up");
                GameData.Upgrade_atkSpeed_up();
            }
            if (e.KeyCode == Keys.G) // G키는 다음 스테이지
            {
                GameData.StageUp();
                Console.WriteLine("Stage Up. current stage : " + GameData.GetStage());
            }
        }
        private void Game_Form_MouseUp(object sender, MouseEventArgs e) //플레이어 마우스 업
        {
            switch (player.GetWeaponType())
            {
                case 0:
                case 1:
                case 2:
                    break;
                case 3: //소드 공격을 위한 장치
                    if (!(GameData.ab.rare_up == WeaponRareUpList.SWORD)) //특수능력이 검일때, 아닐때
                        SwordAttack();
                    else
                        SwordAttackUP();
                    break;
            }
            player.is_can_move = true; // 무빙 봉인 해제
            player.is_charging = false;
            GameData.sword_charge = 0.0; //차징 수치 초기화
        }
        private void Game_Form_MouseDown(object sender, MouseEventArgs e) //플레이어 마우스 클릭(공격)
        {
            if (attackTime > GameData.weapon_cool[player.GetWeaponType()] && !is_pause) //시간이 각 무기타입별로 시간 충족시 공격
            {
                playerAttack();
                StartTime = EndAttackTime;
            }

        }


        /*************************************
         * Menu / Form
         * ***********************************/
        private void toolStripMenuExit_Click(object sender, EventArgs e)//메뉴 - 종료
        {
            Score.CheckNewRecord();
            Application.Exit();
        }
        private void toolStripMenuOption_Click(object sender, EventArgs e)//메뉴 - 옵션
        {
            if (Application.OpenForms["Option_form"] as Option_form == null)
            {
                Option_form option_form = new Option_form();
                option_form.Show();
            }
        }
        private void toolStripMenuMain_Click(object sender, EventArgs e) //메뉴 - 메인으로 돌아가기
        {
            Score.CheckNewRecord();
            this.Close();
        }

        private void Game_Form_FormClosing(object sender, FormClosingEventArgs e) //게임 창이 닫힐때 배경음도 끄기
        {
            mciSendString("stop MediaFile", null, 0, IntPtr.Zero);
        }

        private void Game_Form_Activated(object sender, EventArgs e)  //옵션 창 갔다 올때 활성화. 사운드 변경 유무를 위한 함수
        {
            is_bgm_sound = Option.GetBgmSound();
            is_eff_sound = Option.GetEffSound();
            SetBgmSound(is_bgm_sound, out volume_bgm);
        }

        /*************************************
         * Resource
         * ***********************************/
        public void SetBgmSound(bool onoff, out int volume) //배경음악 설정. off일때는 소리를 0으로.
        {
            if (onoff) volume = 50;
            else volume = 0;
            mciSendString("setaudio MediaFile volume to " + volume.ToString(), null, 0, IntPtr.Zero);
        }

        protected override void OnPaintBackground(PaintEventArgs e) //깜빡임 방지
        {

        }
        private void Game_Form_Paint(object sender, PaintEventArgs e) //밑바탕 그리기
        {
            if (Ground != null)
            {
                e.Graphics.DrawImage(Ground, 0, 0);
            }
        }
        private void timer1_Tick(object sender, EventArgs e) //게임 update()
        {
            del_drawingImages drawing = (grap) => //draw 델리게이트
            {
                DrawWeapons(grap);
                DrawEnemy(grap);
                DrawScore(grap);
                DrawPlayer(grap);
                DrawAttackGauge(grap);
                DrawInfo(grap);
            };
            del_CheckColide checkColide = () => //충돌 델리게이트
            {
                CheckPlayerWithEnemy();
                CheckBullet();
                PlayerLiveCheck();
            };
            del_moves objectMoves = () => //오브젝트 델리게이트
            {
                player.PlayerMove();
                UpdateWeapons();
                MoveEnemy();
                WeaponCharge();
            };

            if (is_gaming) //게임이 진행중일때
            {
                if (!is_pause) //퍼즈가 아니면
                {
                    g_ground = Graphics.FromImage(Ground);
                    g_ground.Clear(Color.White);
                    EndAttackTime = DateTime.Now; //공격 시간을 갖기위한 장치
                    attackTime = (EndAttackTime.Ticks - StartTime.Ticks) / 10000000f;

                    CreateEnemy();      //틱당 확률에 따라 적 생성

                    objectMoves();      //오브젝트 델리게이트
                    checkColide();      //충돌 델리게이트
                    drawing(g_ground);  //draw 델리게이트

                    Score.CheckNewRecord(); //신기록 체커
                    LevelUp();      //킬수에 따라 스테이지 업
                }
                else
                {
                    if (must_select) //능력 선택시간
                    {
                        DrawLevelUp(g_ground); //선택할 능력 드로우
                    }
                    else //선택이 아니라 단순 퍼즈
                    {
                        DrawPause(g_ground);
                    }
                }
            }
            else //플레이어가 죽은 경우
            {
                DrawPlayerDie(g_ground);
            }
            Invalidate();
        }

        /*************************************
         * Stage / Ability
         * ***********************************/

        private void LevelUp()
        {
            if (GameData.GetKill() >= GameData.GetStage() * 10 ) //각 스테이지별 10킬당 렙업
            {
                GameData.StageUp();
                ShowType(out must_select, out is_pause);
            }
        }
        private void RandomAbility(out string ab_star_, out string ab_name_, out string ab_value_, out string ab_add_, out int ab_code_)
        {
            AbilityBox ability = LevelUpBox.ShowLevelUpAbility();
            ab_star_ = ability.type;
            ab_name_ = ability.ab_name;
            ab_value_ = ability.ab_value;
            ab_add_ = ability.ab_add;
            ab_code_ = ability.ab_code;
        }
        private void ShowType(out bool select, out bool pause)
        {
            RandomAbility(out ab_star_1, out ab_name_1, out ab_value_1, out ab_add_1, out ab_code_1);
            RandomAbility(out ab_star_2, out ab_name_2, out ab_value_2, out ab_add_2, out ab_code_2);
            RandomAbility(out ab_star_3, out ab_name_3, out ab_value_3, out ab_add_3, out ab_code_3);
            select = true;
            pause = true;
        }
        private void Selecttype(out bool select, out bool pause)
        {
            if (is_eff_sound) s_select.Play();
            select = false;
            pause = false;
        }

        /*************************************
         * Draw
         * ***********************************/
        private void DrawInfo(Graphics graphics)
        {
            Font _font = new System.Drawing.Font(new FontFamily("Arial"), 10, FontStyle.Bold);
            graphics.DrawString("Dagger : " + GameData.curr_dagger_dammage.ToString() +
                "\nGun : " + GameData.curr_bullet_dammage.ToString() +
                "\nRpg : " + GameData.curr_bomb_dammage.ToString() +
                "\nSword : " + GameData.curr_sword_dammage.ToString() +
                "\nHp : " + player.GetPlayerHP().ToString() + " / " + player.GetPlayerMaxHP().ToString()
                , _font, Brushes.Black, new PointF(1000, 30));
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
        private void DrawLevelUp(Graphics graphics)
        {
            using(Brush brush = new SolidBrush(Color.Black))
            {
                graphics.FillRectangle(brush, 125, 200, 200, 400);
                graphics.FillRectangle(brush, 475, 200, 200, 400);
                graphics.FillRectangle(brush, 825, 200, 200, 400);
                Font _font = new System.Drawing.Font(new FontFamily("Arial"), 10, FontStyle.Bold);

                graphics.DrawString("  NUM 1   \n\n " + ab_star_1 + "\n\n " + ab_name_1 + "\n\n  " + ab_value_1 + "\n\n" + ab_add_1 + "\n", _font, Brushes.White, new PointF(150, 250));
                graphics.DrawString("  NUM 2   \n\n " + ab_star_2 + "\n\n " + ab_name_2 + "\n\n  " + ab_value_2 + "\n\n" + ab_add_2 + "\n", _font, Brushes.White, new PointF(500, 250));
                graphics.DrawString("  NUM 3   \n\n " + ab_star_3 + "\n\n " + ab_name_3 + "\n\n  " + ab_value_3 + "\n\n" + ab_add_3 + "\n", _font, Brushes.White, new PointF(850, 250));
                graphics.DrawString("Choose Ability with keyboard numbers (1~3)", _font, Brushes.Black, new PointF(500, 100));
            }
        }
        private void DrawPause(Graphics graphics)
        {
            using (Brush brush = new SolidBrush(Color.Black))
            {
                graphics.FillRectangle(brush, 200, 100, 800, 600);
                Font _font = new System.Drawing.Font(new FontFamily("Arial"), 60, FontStyle.Bold);
                graphics.DrawString("Pause", _font, Brushes.White, new PointF(500, 400));
            }
        }
        private void DrawPlayerDie(Graphics graphics)
        {
            using (Brush brush = new SolidBrush(Color.Black))
            {
                graphics.FillRectangle(brush, 0, 0, 1200, 800);
                Font _font = new System.Drawing.Font(new FontFamily("Arial"), 60, FontStyle.Bold);
                graphics.DrawString("Die!", _font, Brushes.White, new PointF(500, 300));
                Font _font2 = new System.Drawing.Font(new FontFamily("Arial"), 30, FontStyle.Bold);
                graphics.DrawString("Esc to menu.", _font2, Brushes.White, new PointF(300, 550));
            }
        }
        private void DrawAttackGauge(Graphics graphics)
        {
            using (Brush brush = new SolidBrush(Color.Red))
            {
                graphics.FillRectangle(brush, 0, 50, 25, 100);
            }
            using (Brush brush = new SolidBrush(Color.Green))
            {

                //Console.WriteLine("charged : " + GameData.sword_charge);
                float persent;
                float gauge_height;
                switch (player.GetWeaponType())
                {
                    case 0:
                    case 1:
                    case 2:
                        persent = attackTime / GameData.weapon_cool[player.GetWeaponType()];
                        if (persent > 1.0f) persent = 1.0f;
                        gauge_height = 100 * persent;
                        graphics.FillRectangle(brush, 0, 50, 25, (int)gauge_height);
                        break;
                    case 3:
                        if (!(GameData.ab.rare_up == WeaponRareUpList.SWORD))
                        {
                            double charged = GameData.sword_charge * 1500.0;
                            persent = (float)charged / (float)GameData.sword_max_range;
                            if (persent > 1.0f) persent = 1.0f;
                            gauge_height = 100 * persent;
                            graphics.FillRectangle(brush, 0, 50, 25, (int)gauge_height);
                        }
                        else
                        {
                            double charged = GameData.sword_charge * 2500.0;
                            persent = (float)charged / (float)GameData.sword_max_range_up;
                            if (persent > 1.0f) persent = 1.0f;
                            gauge_height = 100 * persent;
                            graphics.FillRectangle(brush, 0, 50, 25, (int)gauge_height);
                        }
                        break;
                }
            }
        }
        private void DrawEnemy(Graphics graphics)
        {
            int i;
            if (GameData.GetStage() >= GameData.lv1_stage)
                for (i = 0; i < GameData.MAX_ENEMY_NORMAL; i++)
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
            {
                for (i = 0; i < GameData.MAX_ENEMY_GUN; i++)
                {
                    if (enemy_gun[i].exist == false) continue;
                    enemy_gun[i].angle = (float)GameMath.GetAngle(enemy_gun[i].getPositionMid().x, enemy_gun[i].getPositionMid().y, player.getPositionMid().x, player.getPositionMid().y);
                    clone_e_gun[i] = (Bitmap)e_gun.Clone();
                    RotateImage(clone_e_gun[i], enemy_gun[i].angle - 90, out clone_e_gun[i]);
                    graphics.DrawImage(clone_e_gun[i], enemy_gun[i].getPositionEdge().x, enemy_gun[i].getPositionEdge().y, enemy_gun[i].m_size.width, enemy_gun[i].m_size.height);
                }
                var enemy_bullet_list = from bullet in enemy_bullet
                                        where bullet.exist == true
                                        select bullet;
                foreach (EnemyBullet bullets in enemy_bullet)
                {
                    graphics.DrawImage(e_bullet, bullets.getPositionEdge().x, bullets.getPositionEdge().y, GameData.bullet_width, GameData.bullet_height);
                }
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
            foreach (PlayerRpgBomb weapons in bomb_list)
            {
                graphics.DrawImage(bomb, weapons.getPositionEdge().x, weapons.getPositionEdge().y, weapons.getSize().width, weapons.getSize().height);
            }
            foreach (PlayerSword weapons in sword_list)
            {
                graphics.DrawImage(sword_range, weapons.getPositionEdge().x - (int)weapons.sword_range / 2, weapons.getPositionEdge().y - (int)weapons.sword_range / 2, (int)weapons.sword_range, (int)weapons.sword_range);
            }
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

        /*************************************
         * Colide
         * ***********************************/
        private void CheckBullet()
        {
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
                        if (is_eff_sound) s_dagger_hit.Play();
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
                        if (is_eff_sound) s_dagger_hit.Play();
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
                        if (is_eff_sound) s_dagger_hit.Play();
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
                        if (is_eff_sound) s_dagger_hit.Play();
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
                        if (is_eff_sound) s_gun_hit.Play();
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
                        if (is_eff_sound) s_gun_hit.Play();
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
                        if (is_eff_sound) s_gun_hit.Play();
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
                        if (is_eff_sound) s_gun_hit.Play();
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
                        if (is_eff_sound) s_rpg_hit.Play();
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
                        if (is_eff_sound) s_rpg_hit.Play();
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
                        if (is_eff_sound) s_rpg_hit.Play();
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
                        if (is_eff_sound) s_rpg_hit.Play();
                        CheckBomb();
                        weapon.exist = false;
                    }
                }
            }
        }
        private void CheckBomb()
        {
            foreach(PlayerRpgBomb weapon in player_rpg_bomb)
            {
                if (!weapon.exist) continue;
                //bombR = new Rectangle((int)weapon.getPositionEdge().x, (int)weapon.getPositionEdge().y, weapon.getSize().width, weapon.getSize().height);

                foreach (Enemy_normal enemy in enemy_normal)
                {
                    if (!enemy.exist) continue;

                    if (GameMath.getDistance(weapon.getPositionMid(), enemy.getPositionMid()) < weapon.get_bomb_range())
                    {
                        enemy.hit(weapon.getDammage());
                    }
                }
                foreach (Enemy_speed enemy in enemy_speed)
                {
                    if (!enemy.exist) continue;

                    if (GameMath.getDistance(weapon.getPositionMid(), enemy.getPositionMid()) < weapon.get_bomb_range())
                    {
                        enemy.hit(weapon.getDammage());
                    }
                }
                foreach (Enemy_gun enemy in enemy_gun)
                {
                    if (!enemy.exist) continue;

                    if (GameMath.getDistance(weapon.getPositionMid(), enemy.getPositionMid()) < weapon.get_bomb_range())
                    {
                        enemy.hit(weapon.getDammage());
                    }
                }
                foreach (Enemy_shield enemy in enemy_shield)
                {
                    if (!enemy.exist) continue;

                    if (GameMath.getDistance(weapon.getPositionMid(), enemy.getPositionMid()) < weapon.get_bomb_range())
                    {
                        enemy.hit(weapon.getDammage());
                    }
                }
            }
        }
        private void CheckPlayerWithEnemy()
        {
            playerR = new Rectangle((int)player.getPositionEdge().x, (int)player.getPositionEdge().y, GameData.player_width, GameData.player_height);

            var enemy_normal_list = from enemyies in enemy_normal
                                    where enemyies.exist == true
                                    select enemyies;
            var enemy_speed_list = from enemyies in enemy_speed
                                   where enemyies.exist == true
                                   select enemyies;
            var enemy_gun_list = from enemyies in enemy_gun
                                 where enemyies.exist == true
                                 select enemyies;
            var enemy_shield_list = from enemyies in enemy_shield
                                    where enemyies.exist == true
                                    select enemyies;
            var enemy_bullet_list = from bullets in enemy_bullet
                                    where bullets.exist == true
                                    select bullets;
            foreach (Enemy_normal enemy in enemy_normal_list)
            {
                enemyR = new Rectangle((int)enemy.getPositionEdge().x, (int)enemy.getPositionEdge().y, enemy.getSize().width, enemy.getSize().height);

                interR = Rectangle.Intersect(playerR, enemyR);
                if(!interR.IsEmpty)
                {
                    enemy.AttackSuccess();
                    if (is_eff_sound) s_player_hit.Play();
                    player.hit();
                }
            }
            foreach (Enemy_speed enemy in enemy_speed_list)
            {
                enemyR = new Rectangle((int)enemy.getPositionEdge().x, (int)enemy.getPositionEdge().y, enemy.getSize().width, enemy.getSize().height);

                interR = Rectangle.Intersect(playerR, enemyR);
                if (!interR.IsEmpty)
                {
                    enemy.AttackSuccess();
                    if (is_eff_sound) s_player_hit.Play();
                    player.hit();
                }
            }
            foreach (Enemy_gun enemy in enemy_gun_list)
            {
                enemyR = new Rectangle((int)enemy.getPositionEdge().x, (int)enemy.getPositionEdge().y, enemy.getSize().width, enemy.getSize().height);

                interR = Rectangle.Intersect(playerR, enemyR);
                if (!interR.IsEmpty)
                {
                    enemy.AttackSuccess();
                    if (is_eff_sound) s_player_hit.Play();
                    player.hit();
                }
            }
            foreach (Enemy_shield enemy in enemy_shield_list)
            {
                enemyR = new Rectangle((int)enemy.getPositionEdge().x, (int)enemy.getPositionEdge().y, enemy.getSize().width, enemy.getSize().height);

                interR = Rectangle.Intersect(playerR, enemyR);
                if (!interR.IsEmpty)
                {
                    enemy.AttackSuccess();
                    if (is_eff_sound) s_player_hit.Play();
                    player.hit();
                }
            }
            foreach (EnemyBullet bullet in enemy_bullet_list)
            {
                enemy_bulletR = new Rectangle((int)bullet.getPositionEdge().x, (int)bullet.getPositionEdge().y, bullet.getSize().width, bullet.getSize().height);

                interR = Rectangle.Intersect(playerR, enemy_bulletR);
                if (!interR.IsEmpty)
                {
                    bullet.AttackSuccess();
                    if (is_eff_sound) s_player_hit.Play();
                    player.hit();
                }
            }
        }

        /*************************************
         * Player
         * ***********************************/

        private void playerAttack()
        {
            if (GameData.ab.is_RareSelected)
            {
                switch (player.GetWeaponType())
                {
                    case 0:
                        if (GameData.ab.rare_up == WeaponRareUpList.DAGGER)
                            DaggerAttackUP();
                        else
                            DaggerAttack();
                        break;
                    case 1:
                        if (GameData.ab.rare_up == WeaponRareUpList.GUN)
                            GunAttackUP();
                        else
                            GunAttack();
                        break;
                    case 2:
                        if (GameData.ab.rare_up == WeaponRareUpList.RPG)
                            RpgAttack();
                        else
                            RpgAttack();
                        break;
                    case 3:
                        if (GameData.ab.rare_up == WeaponRareUpList.SWORD)
                            SwordAttackCharge();
                        else
                            SwordAttackCharge();
                        break;
                }
            }
            else
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
        }
        private void PlayerLiveCheck()
        {
            if (!player.exist)
            {
                Score.CheckNewRecord();
                Score.SaveRecord();
                is_pause = true;
                is_gaming = false;
            }
        }

        /*************************************
         * Weapon Attack
         * ***********************************/

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
                player_dagger[i].angle = (float)GameMath.GetAngle(player.getPositionMid().x, player.getPositionMid().y, PointToClient(MousePosition).X, PointToClient(MousePosition).Y);
                player_dagger[i].is_can_rotate = true;
                if (is_eff_sound) s_dagger.Play();
            }
        }
        private void DaggerAttackUP()
        {
            int i;
            for (i = 0; i < GameData.MAX_DAGGER; i++)
            {
                if (player_dagger[i].exist == false)
                    break;
            }
            if (i != GameData.MAX_DAGGER)
            {
                o_Vector direction;
                o_Point position_offset = new o_Point(16.0f * (float)Math.Cos(player.angle), 16.0f * (float)Math.Sin(player.angle));
                direction.x = PointToClient(MousePosition).X - (player.getPositionMid().x + position_offset.x);
                direction.y = PointToClient(MousePosition).Y - (player.getPositionMid().y + position_offset.y);
                player_dagger[i].exist = true;
                player_dagger[i].SetObjectPosition(player.getPositionMid().x + position_offset.x, player.getPositionMid().y + position_offset.y);
                player_dagger[i].SetDirection(direction);
                player_dagger[i].is_Up = true;
                player_dagger[i].angle = (float)GameMath.GetAngle(player.getPositionMid().x + position_offset.x, player.getPositionMid().y + position_offset.y, PointToClient(MousePosition).X, PointToClient(MousePosition).Y);
                player_dagger[i].is_can_rotate = true;
                if (is_eff_sound) s_dagger.Play();
            }
            for (i = 0; i < GameData.MAX_DAGGER; i++)
            {
                if (player_dagger[i].exist == false)
                    break;
            }
            if (i != GameData.MAX_DAGGER)
            {
                o_Vector direction;
                o_Point position_offset = new o_Point(-16.0f * (float)Math.Cos(player.angle), -16.0f * (float)Math.Sin(player.angle));
                direction.x = PointToClient(MousePosition).X - (player.getPositionMid().x + position_offset.x);
                direction.y = PointToClient(MousePosition).Y - (player.getPositionMid().y + position_offset.y);
                player_dagger[i].exist = true;
                player_dagger[i].SetObjectPosition(player.getPositionMid().x + position_offset.x, player.getPositionMid().y + position_offset.y);
                player_dagger[i].SetDirection(direction);
                player_dagger[i].is_Up = true;
                player_dagger[i].angle = (float)GameMath.GetAngle(player.getPositionMid().x + position_offset.x, player.getPositionMid().y + position_offset.y, PointToClient(MousePosition).X, PointToClient(MousePosition).Y);
                player_dagger[i].is_can_rotate = true;
                if (is_eff_sound) s_dagger.Play();
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
                if (is_eff_sound) s_gun.Play();
            }
        }
        private void GunAttackUP()
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
                o_Point position_offset = new o_Point(16.0f * (float)Math.Cos(player.angle), 16.0f * (float)Math.Sin(player.angle));
                direction.x = PointToClient(MousePosition).X - (player.getPositionMid().x + position_offset.x);
                direction.y = PointToClient(MousePosition).Y - (player.getPositionMid().y + position_offset.y);
                player_bullet[i].exist = true;
                player_bullet[i].SetObjectPosition(player.getPositionMid().x + position_offset.x, player.getPositionMid().y + position_offset.y);
                player_bullet[i].SetDirection(direction);
                if (is_eff_sound) s_gun.Play();
            }
            for (i = 0; i < GameData.MAX_BULLET; i++)
            {
                if (player_bullet[i].exist == false)
                    break;
            }
            if (i != GameData.MAX_BULLET)
            {
                o_Vector direction;
                o_Point position_offset = new o_Point(-16.0f * (float)Math.Cos(player.angle), -16.0f * (float)Math.Sin(player.angle));
                direction.x = PointToClient(MousePosition).X - (player.getPositionMid().x + position_offset.x);
                direction.y = PointToClient(MousePosition).Y - (player.getPositionMid().y + position_offset.y);
                player_bullet[i].exist = true;
                player_bullet[i].SetObjectPosition(player.getPositionMid().x + position_offset.x, player.getPositionMid().y + position_offset.y);
                player_bullet[i].SetDirection(direction);
                if (is_eff_sound) s_gun.Play();
            }
        }
        private void RpgAttack()
        {
            int i;
            if (GameData.is_RpgUp_once())
            {
                RpgUp();
                GameData.UpgradeRpgComplete();
            }
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
                if (is_eff_sound) s_rpg.Play();
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
        public void RpgUp()
        {
            foreach (PlayerRpgBomb bombs in player_rpg_bomb)
            {
                bombs.UpgradeRpgBomb();
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
                if (is_eff_sound) s_sword.Play();
                //Check Colide
                foreach (Enemy_normal enemy in enemy_normal)
                {
                    if (!enemy.exist) continue;

                    if (GameMath.getDistance(player.getPositionMid(), enemy.getPositionMid()) < player_sword[i].get_sword_range() / 2)
                    {
                        enemy.hit(player_sword[i].getDammage());
                        if (is_eff_sound) s_sword_hit.Play();
                    }
                }
                foreach (Enemy_speed enemy in enemy_speed)
                {
                    if (!enemy.exist) continue;

                    if (GameMath.getDistance(player.getPositionMid(), enemy.getPositionMid()) < player_sword[i].get_sword_range() / 2)
                    {
                        enemy.hit(player_sword[i].getDammage());
                        if (is_eff_sound) s_sword_hit.Play();
                    }
                }
                foreach (Enemy_gun enemy in enemy_gun)
                {
                    if (!enemy.exist) continue;

                    if (GameMath.getDistance(player.getPositionMid(), enemy.getPositionMid()) < player_sword[i].get_sword_range() / 2)
                    {
                        enemy.hit(player_sword[i].getDammage());
                        if (is_eff_sound) s_sword_hit.Play();
                    }
                }
                foreach (Enemy_shield enemy in enemy_shield)
                {
                    if (!enemy.exist) continue;

                    if (GameMath.getDistance(player.getPositionMid(), enemy.getPositionMid()) < player_sword[i].get_sword_range() / 2)
                    {
                        enemy.hit(player_sword[i].getDammage());
                        if (is_eff_sound) s_sword_hit.Play();
                    }
                }
            }
        }
        private void SwordAttackUP()
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
                player_sword[i].ChargingUp(GameData.sword_charge);
                Console.WriteLine("sword lange : " + player_sword[i].sword_range);
                if (is_eff_sound) s_sword.Play();
            }
            //Check Colide
            foreach (Enemy_normal enemy in enemy_normal)
            {
                if (!enemy.exist) continue;

                if (GameMath.getDistance(player.getPositionMid(), enemy.getPositionMid()) < player_sword[i].get_sword_range() / 2)
                {
                    enemy.hit(player_sword[i].getDammage());
                    if (is_eff_sound) s_sword_hit.Play();
                }
            }
            foreach (Enemy_speed enemy in enemy_speed)
            {
                if (!enemy.exist) continue;

                if (GameMath.getDistance(player.getPositionMid(), enemy.getPositionMid()) < player_sword[i].get_sword_range() / 2)
                {
                    enemy.hit(player_sword[i].getDammage());
                    if (is_eff_sound) s_sword_hit.Play();
                }
            }
            foreach (Enemy_gun enemy in enemy_gun)
            {
                if (!enemy.exist) continue;

                if (GameMath.getDistance(player.getPositionMid(), enemy.getPositionMid()) < player_sword[i].get_sword_range() / 2)
                {
                    enemy.hit(player_sword[i].getDammage());
                    if (is_eff_sound) s_sword_hit.Play();
                }
            }
            foreach (Enemy_shield enemy in enemy_shield)
            {
                if (!enemy.exist) continue;

                if (GameMath.getDistance(player.getPositionMid(), enemy.getPositionMid()) < player_sword[i].get_sword_range() / 2)
                {
                    enemy.hit(player_sword[i].getDammage());
                    if (is_eff_sound) s_sword_hit.Play();
                }
            }
        }
        private void SwordAttackCharge()
        {
            player.is_can_move = false;
            player.is_charging = true;
        }


        /*************************************
         * Enemy
         * ***********************************/

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
            if (random.Next(40) == 0)
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
            if (random.Next(70) == 0)
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
            if (random.Next(100) == 0)
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
            if (random.Next(120) == 0)
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

            var enemy_bullet_list = from bullets in enemy_bullet
                                    where bullets.exist == true
                                    select bullets;
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
            {
                foreach (Enemy_gun enemy in enemy_gun_list)
                {
                    if(random.Next(50) == 0)
                    ShotEnemyFunc(enemy);
                    enemy.moveObject(player.getPositionMid());
                }
                foreach (EnemyBullet bullet in enemy_bullet_list)
                {
                    bullet.moveObject();
                }
            }
            if (GameData.GetStage() >= GameData.lv4_stage)
                foreach (Enemy_shield enemy in enemy_shield_list)
                {
                    enemy.moveObject(player.getPositionMid());
                }
        }
        private void ShotEnemyFunc(Enemy_gun enemy)
        {
            int i;
            for (i = 0; i < GameData.MAX_BULLET; i++)
            {
                if (enemy_bullet[i].exist == false)
                    break;
            }
            if (i != GameData.MAX_ENEMY_BULLET)
            {
                o_Vector direction;
                direction.x = (player.getPositionMid().x) - (enemy.getPositionMid().x);
                direction.y = (player.getPositionMid().y) - (enemy.getPositionMid().y);
                enemy_bullet[i].exist = true;
                enemy_bullet[i].SetObjectPosition(enemy.getPositionMid().x, enemy.getPositionMid().y);
                enemy_bullet[i].SetDirection(direction);
            }
        }
    }
}
