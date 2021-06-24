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
                mciSendString("open \"" + "Resources/InfiniteWar_BGM.mp3" + "\" type mpegvideo alias MediaFile", null, 0, IntPtr.Zero); //여기 파일 열어주세요
                mciSendString("play MediaFile REPEAT", null, 0, IntPtr.Zero);                               //무한으로 즐겨요
                mciSendString("setaudio MediaFile volume to " + volume.ToString(), null, 0, IntPtr.Zero);   //불륨조전
            }
            catch (Exception ex) //실패시 catch 
            {
                string message = "Exception Type : " + ex.GetType() + "\nMessage : " + ex.Message + "\nStack Trace : " + ex.StackTrace + "\n\n"; //메세지내용
                using (FileStream fs = new FileStream("error.log", FileMode.Append)) //log저장
                {
                    string time = "Error Time : " + DateTime.Now.ToString() + "\n"; //시간도 같이 저장해요
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
                player.ChangeWeapon();                           //무기변경
                ChangeImagePlayerWeapon(player.GetWeaponType()); //바뀐 무기에 따라 플레이어 이미지도 변경
                if (is_eff_sound) s_switch.Play();               //무기 전환 사운드
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
            del_moves objectMoves = () =>   //오브젝트 델리게이트
            {
                player.PlayerMove();    //플레이어 업데이트
                UpdateWeapons();        //무기(발사체) 업데이트
                MoveEnemy();            //적 업데이트
                WeaponCharge();         //무기 차지(검)
            };
            del_CheckColide checkColide = () => //충돌 델리게이트
            {
                CheckPlayerWithEnemy();     //플레이어 충돌 체크
                CheckBullet();              //적과 무기 충돌 체크
                PlayerLiveCheck();          //플레이어 live 체크
            };
            del_drawingImages drawing = (grap) => //draw 델리게이트
            {
                DrawWeapons(grap);          //무기 드로우
                DrawEnemy(grap);            //적 드로우
                DrawPlayer(grap);           //플레이어 드로우
                DrawAttackGauge(grap);      //무기 쿨타임 드로우
                DrawScore(grap);            //점수 드로우
                DrawInfo(grap);             //무기 공격력, 현재 체력 정보 드로우
            };

            if (is_gaming)                  //게임이 진행중일때
            {
                if (!is_pause)              //퍼즈가 아니면
                {
                    g_ground = Graphics.FromImage(Ground);
                    g_ground.Clear(Color.White);            //흰색바탕
                    EndAttackTime = DateTime.Now;           //공격 시간을 갖기위한 장치
                    attackTime = (EndAttackTime.Ticks - StartTime.Ticks) / 10000000f;       //틱당 차이로 공격시간을 가짐 (dt와 다르게 시험)

                    CreateEnemy();              //틱당 확률에 따라 적 생성

                    objectMoves();              //오브젝트 델리게이트
                    checkColide();              //충돌 델리게이트
                    drawing(g_ground);          //draw 델리게이트

                    LevelUp();                  //킬수에 따라 스테이지 업
                }
                else
                {
                    if (must_select)            //능력 선택시간
                    {
                        DrawLevelUp(g_ground);  //선택할 능력 드로우
                    }
                    else                        //선택이 아니라 단순 퍼즈일때
                    {
                        DrawPause(g_ground);
                    }
                }
            }
            else //플레이어가 죽은 경우
            {
                DrawPlayerDie(g_ground);        //DIE 드로우
            }
            Invalidate();
        }

        /*************************************
         * Stage / Ability
         * ***********************************/

        private void LevelUp()                  //레벨업 조건
        {
            if (GameData.GetKill() >= GameData.GetStage() * 10 ) //각 스테이지별 10킬당 렙업
            {
                GameData.StageUp();                             //게임 데이터 stage 업
                ShowType(out must_select, out is_pause);        //선택의 시간
            }
        }
        private void RandomAbility(out string ab_star_, out string ab_name_, out string ab_value_, out string ab_add_, out int ab_code_)    //능력 선택 정보는 out으로 필수로 표시
        {
            AbilityBox ability = LevelUpBox.ShowLevelUpAbility();   //각 능력은 랜덤으로 배정받음
            ab_star_ = ability.type;                                //랜덤의 요소를 가져옴
            ab_name_ = ability.ab_name;
            ab_value_ = ability.ab_value;
            ab_add_ = ability.ab_add;
            ab_code_ = ability.ab_code;
        }
        private void ShowType(out bool select, out bool pause)      //선택의 시간. 게임 상태를 필수적으로 표시
        {
            RandomAbility(out ab_star_1, out ab_name_1, out ab_value_1, out ab_add_1, out ab_code_1);   //각 랜덤능력의 정보를 가져옴
            RandomAbility(out ab_star_2, out ab_name_2, out ab_value_2, out ab_add_2, out ab_code_2);
            RandomAbility(out ab_star_3, out ab_name_3, out ab_value_3, out ab_add_3, out ab_code_3);
            select = true;
            pause = true;
        }
        private void Selecttype(out bool select, out bool pause)    //선택을 했을때, 게임 그대로 진행
        {
            if (is_eff_sound) s_select.Play();
            select = false;
            pause = false;
        }

        /*************************************
         * Draw
         * ***********************************/
        private void DrawInfo(Graphics graphics)                    //무기 공격력, 현재 체력 정보 드로우
        {
            Font _font = new System.Drawing.Font(new FontFamily("Arial"), 10, FontStyle.Bold);      //폰트변수 설정
            graphics.DrawString("Dagger : " + GameData.curr_dagger_dammage.ToString() +             //정보를 가져옴과 동시에 드로우
                "\nGun : " + GameData.curr_bullet_dammage.ToString() +
                "\nRpg : " + GameData.curr_bomb_dammage.ToString() +
                "\nSword : " + GameData.curr_sword_dammage.ToString() +
                "\nHp : " + player.GetPlayerHP().ToString() + " / " + player.GetPlayerMaxHP().ToString()
                , _font, Brushes.Black, new PointF(1000, 30));
        }
        private void DrawScore(Graphics graphics)                   //점수 드로우
        {
            Font _font = new System.Drawing.Font(new FontFamily("휴먼둥근헤드라인"), 14, FontStyle.Bold);       //폰트변수 설정
            graphics.DrawString("New Record : " + Score.getRecordScore().ToString(), _font, Brushes.Black, new PointF(10, 30));
            graphics.DrawString("Record : " + GameData.GetKill().ToString(), _font, Brushes.Black, new PointF(500, 30));
        }
        private void DrawPlayer(Graphics graphics)                  //플레이어 드로우
        {
            player.angle = (float)GameMath.GetAngle(player.getPositionMid().x, player.getPositionMid().y, PointToClient(MousePosition).X, PointToClient(MousePosition).Y); //마우스를 바라보는 방향으로 플레이어 각도를 가져옴
            clone_player = (Bitmap)p_player.Clone();  //클론 그림에 현제 플레이어의 그림을 가져옴
            RotateImage(clone_player, player.angle - 100, out clone_player);    //플레이어 이미지(클론) 회전
            graphics.DrawImage(clone_player, player.getPositionEdge().x, player.getPositionEdge().y, GameData.player_width, GameData.player_height); //회전한 그림 드로우
        }
        private void DrawLevelUp(Graphics graphics)                 //레벨업시 선택할 창 드로우
        {
            using(Brush brush = new SolidBrush(Color.Black))        //브러쉬 설정
            {
                graphics.FillRectangle(brush, 125, 200, 200, 400);  //각 능력선택 창의 바탕화면
                graphics.FillRectangle(brush, 475, 200, 200, 400);
                graphics.FillRectangle(brush, 825, 200, 200, 400);
                Font _font = new System.Drawing.Font(new FontFamily("Arial"), 10, FontStyle.Bold);  //능력 정보 폰트 설정

                graphics.DrawString("  NUM 1   \n\n " + ab_star_1 + "\n\n " + ab_name_1 + "\n\n  " + ab_value_1 + "\n\n" + ab_add_1 + "\n", _font, Brushes.White, new PointF(150, 250)); //각 능력 정보 드로우
                graphics.DrawString("  NUM 2   \n\n " + ab_star_2 + "\n\n " + ab_name_2 + "\n\n  " + ab_value_2 + "\n\n" + ab_add_2 + "\n", _font, Brushes.White, new PointF(500, 250));
                graphics.DrawString("  NUM 3   \n\n " + ab_star_3 + "\n\n " + ab_name_3 + "\n\n  " + ab_value_3 + "\n\n" + ab_add_3 + "\n", _font, Brushes.White, new PointF(850, 250));
                graphics.DrawString("Choose Ability with keyboard numbers (1~3)", _font, Brushes.Black, new PointF(500, 100));  //키보드로 능력 선택 1~3
            }
        }
        private void DrawPause(Graphics graphics)                   //일시정지 창 드로우
        {
            using (Brush brush = new SolidBrush(Color.Black))       //브러쉬 설정
            {
                graphics.FillRectangle(brush, 200, 100, 800, 600);  //일시정지 창 배경화면
                Font _font = new System.Drawing.Font(new FontFamily("Arial"), 60, FontStyle.Bold);
                graphics.DrawString("Pause", _font, Brushes.White, new PointF(500, 400));
            }
        }
        private void DrawPlayerDie(Graphics graphics)               //플레이어 죽음 드로우
        {
            using (Brush brush = new SolidBrush(Color.Black))       //브러쉬 설정
            {
                graphics.FillRectangle(brush, 0, 0, 1200, 800);     //알림 드로우
                Font _font = new System.Drawing.Font(new FontFamily("Arial"), 60, FontStyle.Bold);
                graphics.DrawString("Die!", _font, Brushes.White, new PointF(500, 300));
                Font _font2 = new System.Drawing.Font(new FontFamily("Arial"), 30, FontStyle.Bold);
                graphics.DrawString("Esc to menu.", _font2, Brushes.White, new PointF(300, 550));
            }
        }
        private void DrawAttackGauge(Graphics graphics)             //무기 쿨타임 게이지 드로우
        {
            using (Brush brush = new SolidBrush(Color.Red))         //빨간 바탕에
            {
                graphics.FillRectangle(brush, 0, 50, 25, 100);
            }
            using (Brush brush = new SolidBrush(Color.Green))       //초록색 바 그리기
            {
                float persent;                                      
                float gauge_height;
                switch (player.GetWeaponType())                     //현재 무기 타입 정보 겟
                {
                    case 0:
                    case 1:
                    case 2:
                        persent = attackTime / GameData.weapon_cool[player.GetWeaponType()];    //검을 제외한 무기는 '현재 시간 / 공격 쿨타임'으로 게이지 보여줌
                        if (persent > 1.0f) persent = 1.0f;
                        gauge_height = 100 * persent;
                        graphics.FillRectangle(brush, 0, 50, 25, (int)gauge_height);
                        break;
                    case 3:
                        if (!(GameData.ab.rare_up == WeaponRareUpList.SWORD))                   //검은 '현재 범위 / 최대 범위'로 게이지 보여줌
                        {
                            double charged = GameData.sword_charge * GameData.sword_normal_range_pertick;
                            persent = (float)charged / (float)GameData.sword_max_range;
                            if (persent > 1.0f) persent = 1.0f;
                            gauge_height = 100 * persent;
                            graphics.FillRectangle(brush, 0, 50, 25, (int)gauge_height);
                        }
                        else
                        {
                            double charged = GameData.sword_charge * GameData.sword_upgrade_range_pertick;
                            persent = (float)charged / (float)GameData.sword_max_range_up;
                            if (persent > 1.0f) persent = 1.0f;
                            gauge_height = 100 * persent;
                            graphics.FillRectangle(brush, 0, 50, 25, (int)gauge_height);
                        }
                        break;
                }
            }
        }
        private void DrawHPBar(Graphics graphics, o_Point position, int HP, int MaxHP)  //적 HP바 드로우
        {
            float HPpersent = 64 * HP / MaxHP;
            using (Brush brush = new SolidBrush(Color.Red))         //빨간 체력바
            {
                graphics.FillRectangle(brush, position.x, position.y - 5, HPpersent, 5);    //적의 위쪽에 빨간 체력바 드로우
            }
        }
        private void DrawEnemy(Graphics graphics)                   //적 드로우
        {
            int i;
            if (GameData.GetStage() >= GameData.lv1_stage)          //1스테이지 이상이면 일반적 드로우
                for (i = 0; i < GameData.MAX_ENEMY_NORMAL; i++)     //모든 일반 적에 대하여
                {
                    if (enemy_normal[i].exist == false) continue;   //존재하지 않으면 패스
                    enemy_normal[i].angle = (float)GameMath.GetAngle(enemy_normal[i].getPositionMid().x, enemy_normal[i].getPositionMid().y, player.getPositionMid().x, player.getPositionMid().y); //플레이어를 바라보는 각도 가져옴
                    clone_e_normal[i] = (Bitmap)e_normal.Clone();   //지금 적을 클론으로 가져옴
                    RotateImage(clone_e_normal[i], enemy_normal[i].angle - 90, out clone_e_normal[i]);  //플레이어를 바라보도록 회전
                    graphics.DrawImage(clone_e_normal[i], enemy_normal[i].getPositionEdge().x, enemy_normal[i].getPositionEdge().y, enemy_normal[i].m_size.width, enemy_normal[i].m_size.height);
                    DrawHPBar(graphics, enemy_normal[i].getPositionEdge(), enemy_normal[i].GetCurHP(), enemy_normal[i].GetMaxHP()); //HP바 생성, 그리기
                }
            if (GameData.GetStage() >= GameData.lv2_stage)          //밑으로는 각 타입별로 각 스테이지별 드로우
                for (i = 0; i < GameData.MAX_ENEMY_SPEED; i++)
                {
                    if (enemy_speed[i].exist == false) continue;
                    enemy_speed[i].angle = (float)GameMath.GetAngle(enemy_speed[i].getPositionMid().x, enemy_speed[i].getPositionMid().y, player.getPositionMid().x, player.getPositionMid().y);
                    clone_e_speed[i] = (Bitmap)e_speed.Clone();
                    RotateImage(clone_e_speed[i], enemy_speed[i].angle - 90, out clone_e_speed[i]);
                    graphics.DrawImage(clone_e_speed[i], enemy_speed[i].getPositionEdge().x, enemy_speed[i].getPositionEdge().y, enemy_speed[i].m_size.width, enemy_speed[i].m_size.height);
                    DrawHPBar(graphics, enemy_speed[i].getPositionEdge(), enemy_speed[i].GetCurHP(), enemy_speed[i].GetMaxHP()); //HP바 생성, 그리기
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
                    DrawHPBar(graphics, enemy_gun[i].getPositionEdge(), enemy_gun[i].GetCurHP(), enemy_gun[i].GetMaxHP()); //HP바 생성, 그리기
                }
                var enemy_bullet_list = from bullets in enemy_bullet     //존재하는 불릿을 linq로 가져옴
                                        where bullets.exist == true
                                        select bullets;
                foreach (EnemyBullet bullets in enemy_bullet_list)           //가져온 linq를 모두 드로우
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
                    DrawHPBar(graphics, enemy_shield[i].getPositionEdge(), enemy_shield[i].GetCurHP(), enemy_shield[i].GetMaxHP()); //HP바 생성, 그리기
                }
        }
        private void DrawWeapons(Graphics graphics)                 //무기 드로우
        {
            int i;
            var bullet_list = from weapons in player_bullet         //각 타입별로 존재하는 무기 리스트를 가져옴.
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
            for (i = 0; i < GameData.MAX_DAGGER; i++)               //검은 회전을 위해서 linq를 사용못함.
            {
                if (player_dagger[i].exist == false) continue;      //존재 하지 않으면 패스

                if (player_dagger[i].is_can_rotate == true)         //한번만 회전해야하기 때문에 bool을 걸었음.
                {
                    clone_dagger[i] = (Bitmap)dagger.Clone();       //회전.
                    RotateImage(clone_dagger[i], player_dagger[i].angle - 90, out clone_dagger[i]);
                    player_dagger[i].is_can_rotate = false;
                }
                graphics.DrawImage(clone_dagger[i], player_dagger[i].getPositionEdge().x, player_dagger[i].getPositionEdge().y, GameData.dagger_width, GameData.dagger_height);
                Console.WriteLine(i + " Dagger : "+ player_dagger[i].distance_vector.SizeOfVector());
            }
            foreach (PlayerBullet weapons in bullet_list)           //각 존재하는 리스트들을 드로우
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
        private void RotateImage(Bitmap bmp, float angle, out Bitmap result)    //이미지 회전 함수. (회전할 이미지, 회전할 각도, out 회전한 결과)
        {
            Bitmap rotatedImage = new Bitmap(bmp.Width, bmp.Height);            //회전할 이미지를 새로 설정. 도화지 사이즈 설정 (도화지 자체를 회전하는 방식)

            using (Graphics g = Graphics.FromImage(rotatedImage))               
            {
                g.TranslateTransform(bmp.Width / 2, bmp.Height / 2);            //이미지를 중간으로 이동
                g.RotateTransform(angle);                                       //중앙에서 자체회전
                g.TranslateTransform(-bmp.Width / 2, -bmp.Height / 2);          //중간으로 이동한 만큼 되돌아감
                g.DrawImage(bmp, 0, 0, bmp.Width, bmp.Height);                  //그곳에서 드로우.
            }
            result = rotatedImage;                                              //드로우 한 결과를 return.
        }
        private void ChangeImagePlayerWeapon(int weaponType)        //무기 스위칭하는 함수 (현재 무기 정보를 가져옴)
        {
            if(GameData.ab.is_RareSelected)                         //레어 능력이 선택되었다면
            {
                switch (weaponType)
                {
                    case 0:
                        if(GameData.ab.rare_up == WeaponRareUpList.DAGGER)  //레어 능력이 대거라면
                            p_player = (Bitmap)p_dagger_d.Clone();          //강화된 대거
                        else                                                //아니면
                            p_player = (Bitmap)p_dagger.Clone();            //일반 대거
                        break;
                    case 1:
                        if (GameData.ab.rare_up == WeaponRareUpList.GUN)    //이하 같은 알고리즘
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
            else                                                    //레어 능력이 선택 안되었다면
            {
                switch (weaponType)
                {
                    case 0:
                        p_player = (Bitmap)p_dagger.Clone();        //강화된 무기를 고려할 필요가 없음
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
        private void CheckBullet()                                  //각 무기와 적의 충돌을 체크
        {
            //dagger
            foreach (PlayerDagger weapon in player_dagger)          //대거별
            {
                if (!weapon.exist) continue;                        //존재 하지 않으면 패스.
                bulletR = new Rectangle((int)weapon.getPositionEdge().x, (int)weapon.getPositionEdge().y, weapon.getSize().width, weapon.getSize().height); //대거의 위치와 크기 정보를 rectangle로 씌움

                foreach (Enemy_normal enemy in enemy_normal)        //각 적에 충돌 체크
                {
                    if (!enemy.exist) continue;

                    enemyR = new Rectangle((int)enemy.getPositionEdge().x, (int)enemy.getPositionEdge().y, enemy.getSize().width, enemy.getSize().height);

                    interR = Rectangle.Intersect(bulletR, enemyR);  //겹치는 범위.
                    if (!interR.IsEmpty)                            //겹치는 범위가 있다면
                    {   
                        enemy.hit(weapon.getDammage());             //적에게 일정한 데미지를 입히고
                        if (is_eff_sound) s_dagger_hit.Play();      //소리가 켜져있다면 대거 히트 소리    
                        weapon.AttackSuccess();                     //공격 성공
                    }
                }
                foreach (Enemy_speed enemy in enemy_speed)          //이하 같은 알고리즘
                {
                    if (!enemy.exist) continue;

                    enemyR = new Rectangle((int)enemy.getPositionEdge().x, (int)enemy.getPositionEdge().y, enemy.getSize().width, enemy.getSize().height);

                    interR = Rectangle.Intersect(bulletR, enemyR);
                    if (!interR.IsEmpty)
                    {
                        enemy.hit(weapon.getDammage());
                        if (is_eff_sound) s_dagger_hit.Play();
                        weapon.AttackSuccess();                     //공격 성공
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
                        weapon.AttackSuccess();                     //공격 성공
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
                        weapon.AttackSuccess();                     //공격 성공
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
                        weapon.AttackSuccess();                     //공격 성공
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
                        weapon.AttackSuccess();                     //공격 성공
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
                        weapon.AttackSuccess();                     //공격 성공
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
                        weapon.AttackSuccess();                     //공격 성공
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
                    if (!interR.IsEmpty)                                    //rpg의 경우, 충돌시 폭파함
                    {
                        RpgBombCreate(weapon.getPositionMid());             //폭파 위치는 당시 총알의 위치.
                        enemy.hit(weapon.getDammage());                     //총알데미지 입힘(약함)
                        if (is_eff_sound) s_rpg_hit.Play();                 //소리 재생
                        CheckBomb();                                        //폭파 범위에 닿는지 체크 (원)
                        weapon.AttackSuccess();                             //공격 성공
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
                        weapon.AttackSuccess();                     //공격 성공
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
                        weapon.AttackSuccess();                     //공격 성공
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
                        weapon.AttackSuccess();                     //공격 성공
                    }
                }
            }
        }
        private void CheckBomb()                                    //rpg의 폭파 충돌 함수
        {   
            foreach(PlayerRpgBomb weapon in player_rpg_bomb)        //폭파무기 리스트
            {       
                if (!weapon.exist) continue;                        //없다면 패스

                foreach (Enemy_normal enemy in enemy_normal)        //각 적타입별로
                {
                    if (!enemy.exist) continue;

                    if (GameMath.getDistance(weapon.getPositionMid(), enemy.getPositionMid()) < weapon.get_bomb_range()) //폭파 중앙 기준으로 폭파 범위 내에 있다면
                    {
                        enemy.hit(weapon.getDammage());             //폭파 데미지를 입음
                    }
                }   
                foreach (Enemy_speed enemy in enemy_speed)          //이하 같은 알고리즘
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
        private void CheckPlayerWithEnemy()                         //적과 플레이어 충돌 체크
        {
            playerR = new Rectangle((int)player.getPositionEdge().x, (int)player.getPositionEdge().y, GameData.player_width, GameData.player_height); //플레이어 위치와 크기 복제

            var enemy_normal_list = from enemyies in enemy_normal  //각 타입별 존재하는 적 리스트 뽑아옴
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
            foreach (Enemy_normal enemy in enemy_normal_list)       //각 타입 존재하는 적에 대해
            {
                enemyR = new Rectangle((int)enemy.getPositionEdge().x, (int)enemy.getPositionEdge().y, enemy.getSize().width, enemy.getSize().height);

                interR = Rectangle.Intersect(playerR, enemyR);
                if(!interR.IsEmpty)                                 //충돌한다면
                {
                    enemy.AttackSuccess();                          //적 : 임무성공
                    if (is_eff_sound) s_player_hit.Play();          //플레이어 맞는 소리
                    player.hit();                                   //플레이어 아파용 (체력 -1)
                }
            }
            foreach (Enemy_speed enemy in enemy_speed_list)         //이하 같은 알고리즘
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
            foreach (EnemyBullet bullet in enemy_bullet_list)       //총알도 적과 같은 알고리즘
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

        private void playerAttack()         //플레이어 공격 함수
        {
            if (GameData.ab.is_RareSelected)                        //레어 능력이 선택되었다면
            {
                switch (player.GetWeaponType())                     //현재 무기 정보 겟
                {
                    case 0:                                         //각 타입별로 무기를 공격함
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
            else                                                      //아니면 무조건 일반공격
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
        private void PlayerLiveCheck()      //플레이어가 살아있는지 체크
        {
            if (!player.exist)              //플레이어가 죽었다면
            {
                Score.CheckNewRecord();     //지금이 신기록인가?
                is_pause = true;            //일단 죽었으니 멈춰!
                is_gaming = false;          //게임도 끝이야!
            }
        }

        /*************************************
         * Weapon Attack
         * ***********************************/

        private void UpdateWeapons()                          //무기 업데이트
        {
            foreach(PlayerDagger weapon in player_dagger)           //각 타입별 무기에 대해
            {
                if (weapon.exist == false) continue;                //존재한다면
                weapon.moveObject();                                //그 무기 업데이트
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
        private void DaggerAttack()                           //단검 공격시
        {
            int i;  //인덱스
            for (i = 0; i < GameData.MAX_DAGGER; i++)                                           //빈자리가 있나용
            {
                if (player_dagger[i].exist == false)
                    break;
            }
            if (i != GameData.MAX_DAGGER)                                                       //있으면 공격 할게용
            {
                o_Vector direction;                                                               //어느 방향으로 공격하는지
                direction.x = PointToClient(MousePosition).X - (player.getPositionMid().x);
                direction.y = PointToClient(MousePosition).Y - (player.getPositionMid().y);
                player_dagger[i].exist = true;                                                   //이제 존재하는 무기
                player_dagger[i].SetObjectPosition(player.getPositionMid().x, player.getPositionMid().y); //첫 위치는 플레이어 위치 (중앙)
                player_dagger[i].SetDirection(direction);                                                   //방향 설정
                player_dagger[i].angle = (float)GameMath.GetAngle(player.getPositionMid().x, player.getPositionMid().y, PointToClient(MousePosition).X, PointToClient(MousePosition).Y);    //각도 설정 (드로우를 위해)
                player_dagger[i].is_can_rotate = true;                                          //처음 무기 회전을 위해(드로우)
                if (is_eff_sound) s_dagger.Play();                                              //사운드 재생
            }
        }
        private void DaggerAttackUP()                         //강화된 단검 공격
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
                o_Point position_offset = new o_Point(16.0f * (float)Math.Cos(player.angle), 16.0f * (float)Math.Sin(player.angle));    //두개를 던지기위해 양손으로 이동 (플레이어 기준 오른손)
                direction.x = PointToClient(MousePosition).X - (player.getPositionMid().x + position_offset.x);
                direction.y = PointToClient(MousePosition).Y - (player.getPositionMid().y + position_offset.y);
                player_dagger[i].exist = true;
                player_dagger[i].SetObjectPosition(player.getPositionMid().x + position_offset.x, player.getPositionMid().y + position_offset.y);
                player_dagger[i].SetDirection(direction);
                player_dagger[i].is_Up = true;                                                                                          //강화된 공격은 더 먼 사거리를 이동
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
                o_Point position_offset = new o_Point(-16.0f * (float)Math.Cos(player.angle), -16.0f * (float)Math.Sin(player.angle));  //두개를 던지기 위해 양손으로 이동 (얘는 왼손)
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
        private void GunAttack()                              //총 공격시
        {
            int i;                                              //인덱스   
            for (i = 0; i < GameData.MAX_BULLET; i++)           //빈자리 체크
            {
                if (player_bullet[i].exist == false)
                    break;
            }

            if (i != GameData.MAX_BULLET)                       //자리 비었으면 공격 가능
            {
                o_Vector direction;                             //플레이어가 마우스로 보는 방향
                direction.x = PointToClient(MousePosition).X - (player.getPositionMid().x);
                direction.y = PointToClient(MousePosition).Y - (player.getPositionMid().y);
                player_bullet[i].exist = true;
                player_bullet[i].SetObjectPosition(player.getPositionMid());
                player_bullet[i].SetDirection(direction);
                if (is_eff_sound) s_gun.Play();
            }
        }
        private void GunAttackUP()                            //강화된 총 공격
        {
            int i;                                            //강화된 단검과 알고리즘 같음
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
        private void RpgAttack()                              //Rpg 공격시
        {   
            int i;                                            //인덱스
            if (GameData.is_RpgUp_once())                     //rpg공격 업그레이드했는지 안했는지
            {
                RpgUp();                                      //업그레이드! (범위만 늘어남)
                GameData.UpgradeRpgComplete();                //한번만 강해집니다
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
        private void RpgBombCreate(o_Point create_potision)   //Rpg의 폭파
        {
            int i;
            for (i = 0; i < GameData.MAX_BOMB; i++)
            {
                if (player_rpg_bomb[i].exist == false)
                    break;
            }

            if (i != GameData.MAX_BOMB)                       //폭파 위치는 주어진 위치에서 rpg의 반만큼 offset
            {
                player_rpg_bomb[i].exist = true;
                player_rpg_bomb[i].SetObjectPosition(create_potision.x - player_rpg_bomb[i].m_size.width / 2 ,
                                                     create_potision.y - player_rpg_bomb[i].m_size.height / 2);
            }
        }
        public void RpgUp()                                   //Rpg 업그레이드
        {
            foreach (PlayerRpgBomb bombs in player_rpg_bomb)  //모든 rpg를 업그레이드 합니다
            {
                bombs.UpgradeRpgBomb();                       //업그레이드중.
            }
        }
        private void SwordAttack()                            //검 공격시
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
                player_sword[i].SetObjectPosition(player.getPositionMid().x, player.getPositionMid().y); //플레이어를 중심으로 공격
                player_sword[i].Charging(GameData.sword_charge);                                         //차징했던 시간을 범위로 전환.
                if (is_eff_sound) s_sword.Play();                                                        //'스윽' 베는 소리
                //Check Colide
                foreach (Enemy_normal enemy in enemy_normal)                                             //즉발 공격이기에 바로 충돌 체크
                {
                    if (!enemy.exist) continue;
                    if (GameMath.getDistance(player.getPositionMid(), enemy.getPositionMid()) < player_sword[i].get_sword_range() / 2) //검 공격의 범위의 반 이내에 있는지 체크
                    {
                        enemy.hit(player_sword[i].getDammage());                                                                        //안에 있다면 맞아야지
                        if (is_eff_sound) s_sword_hit.Play();                                                                           //'스으으윽' 베이는 소리
                    }
                }
                foreach (Enemy_speed enemy in enemy_speed) //이하 같은 알고리즘
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
        private void SwordAttackUP()                          //강화된 검 공격시
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
                player_sword[i].SetObjectPosition(player.getPositionMid());
                player_sword[i].ChargingUp(GameData.sword_charge);  //강화된 범위를 가져옴
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
        private void SwordAttackCharge()                      //검 차징 중 함수
        {
            player.is_can_move = false;                       //차징중엔 집중 해야합니다. 움직일수 없어요
            player.is_charging = true;
        }
        private void WeaponCharge()                           //무기가 검일 때, 차징.
        {
            if (player.is_charging)                                  //차징중이면, 시간을 누적시킴
            {
                GameData.sword_charge += GameMath.dt;
            }
        }


        /*************************************
         * Enemy
         * ***********************************/

        private void CreateEnemy()                          //적 생성 함수
        {
            if (GameData.GetStage() >= GameData.lv1_stage) CreateEnemy_normal();    //각 스테이지별로 누적하며 적 생성
            if (GameData.GetStage() >= GameData.lv2_stage) CreateEnemy_speed();
            if (GameData.GetStage() >= GameData.lv3_stage) CreateEnemy_gun();
            if (GameData.GetStage() >= GameData.lv4_stage) CreateEnemy_shield();
        }
        private void CreateEnemy_normal()                   //일반 적 생성
        {
            int i;
            if (random.Next(GameData.ENEMY_NORMAL_Create_Cool) == 0) //게임 데이터별로 확률에 따라 각 타입의 적을 생성
            {
                for (i = 0; i < GameData.MAX_ENEMY_NORMAL; i++)     //빈자리 있나용
                {
                    if (enemy_normal[i].exist == false)
                        break;
                }

                if (i != GameData.MAX_ENEMY_NORMAL)             //있다면
                {
                    int random_position = random.Next(4);       //동서남북중 정해
                    o_Point create_position = new o_Point();    //생성될 위치
                    switch (random_position)                    //각 위치별 타입
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
                    enemy_normal[i].m_position = create_position;   //적 위치 설정
                    enemy_normal[i].SetHP();                        //적 hp설정 (스테이지별로 증가)
                    enemy_normal[i].exist = true;                   //전 이제 존재합니다
                }
            }
        }
        private void CreateEnemy_speed()                    //발 빠른 적 생성
        {
            int i;
            if (random.Next(GameData.ENEMY_SPEED_Create_Cool) == 0)
            {
                for (i = 0; i < GameData.MAX_ENEMY_SPEED; i++)
                {
                    if (enemy_speed[i].exist == false) break;
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
        private void CreateEnemy_gun()                      //총 든 적 생성
        {
            int i;
            if (random.Next(GameData.ENEMY_GUN_Create_Cool) == 0)
            {
                for (i = 0; i < GameData.MAX_ENEMY_GUN; i++)
                {
                    if (enemy_gun[i].exist == false) break;
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
        private void CreateEnemy_shield()                   //방패 든 적 생성
        {
            int i;
            if (random.Next(GameData.ENEMY_SHIELD_Create_Cool) == 0)
            {
                for (i = 0; i < GameData.MAX_ENEMY_SHIELD; i++)
                {
                    if (enemy_shield[i].exist == false) break;
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
        private void MoveEnemy()                            //적의 이동
        {
            var enemy_normal_list = from enemyies in enemy_normal //각 존재하는 적들 리스트 뽑음
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

            var enemy_bullet_list = from bullets in enemy_bullet    //존재하는 총알도 뽑음
                                    where bullets.exist == true
                                    select bullets;
            if (GameData.GetStage() >= GameData.lv1_stage)          //스테이지별로
                foreach (Enemy_normal enemy in enemy_normal_list)   //모든 적들은
                {
                    enemy.moveObject(player.getPositionMid());      //플레이어를 향해서 이동하도록!
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
                    if(random.Next(GameData.ENEMY_BULLET_Create_Cool) == 0) //총든 친구는 확률로 발사
                    ShotEnemyFunc(enemy);   //총알 생성
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
        private void ShotEnemyFunc(Enemy_gun enemy)         //총알 생성
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
                enemy_bullet[i].SetObjectPosition(enemy.getPositionMid());
                enemy_bullet[i].SetDirection(direction);
            }
        }
    }
}
