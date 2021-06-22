using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Infinite_War
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) //메인 폼 로드
        {
            Score.SetRecordScore(0);;       //일단 0점으로 초기화시켜놓음
            Load_record();                  //그리고 기록을 로드
        }

        private void Load_record()          //신기록 로드
        {
            label_record_count.Text = "";

            try
            {
                StreamReader sr = new StreamReader("../../../record.txt");  //record.txt을 가져오기
                label_record_count.Text = sr.ReadToEnd();                   //읽고
                Score.SetRecordScore(int.Parse(label_record_count.Text));   //그 내용을 int로 전달
                sr.Close();                                                 //다읽음
            }
            catch (Exception ex)                                            //실패했다면
            {
                string message = "Exception Type : " + ex.GetType() + "\nMessage : " + ex.Message + "\nStack Trace : " + ex.StackTrace + "\n\n";    //왜 실패했는지
                using (FileStream fs = new FileStream("../../../error.log", FileMode.Append))   //기록으로 남겨
                {
                    string time = "Error Time : " + DateTime.Now.ToString() + "\n";     //시간도 함께
                    BinaryFormatter bf = new BinaryFormatter();                         //바이너리 파일로
                    bf.Serialize(fs, time + message);                                   //기록해
                    fs.Close();                                                         //그리고 닫기
                }
                Save_record();                                                          //그리고 없으니 파일 새로 만들기
            }
        }

        private void Save_record()
        {
            label_record_count.Text = Score.getRecordScore().ToString();        //지금 점수를 텍스트로 전환
            using (StreamWriter sw = new StreamWriter("../../../record.txt"))   //이 파일에 저장할 계획
            {
                sw.Write(label_record_count.Text);                              //쓰자
                sw.Close();
            }
        }

        private void toolStripMenu_Exit_Click(object sender, EventArgs e)       //메뉴 - 종료
        {
            Application.Exit();
        }

        private void toolStripMenu_Option_Click(object sender, EventArgs e)     //메뉴 - 옵션
        {
            if(Application.OpenForms["Option_form"] as Option_form == null)     //옵션의 폼이 없다면 열어
            {
                Option_form option_form = new Option_form();                    //새로운 옵션 폼 오픈
                option_form.Show();
            }
        }

        private void button_option_Click(object sender, EventArgs e)            //옵션 버튼을 눌렀을 때
        {   
            if (Application.OpenForms["Option_form"] as Option_form == null)    //옵션의 폼이 없다면 열어
            {
                Option_form option_form = new Option_form();                    //새로운 옵션 폼 오픈     
                option_form.Show();
            }
        }

        private void button_exit_Click(object sender, EventArgs e)              //종료 버튼을 눌렀을 때
        {
            Application.Exit();
        }

        private void button_gamestart_Click(object sender, EventArgs e)         //게임 시작 버튼을 눌렀을 때
        {
            Game_Form gameform = new Game_Form();                               //게임 폼을 가져옴
            gameform.Location = this.Location;                                  //현재 위치에
            gameform.StartPosition = FormStartPosition.Manual;
            gameform.FormClosing += delegate { this.Show(); };                  //닫어도 이건 안닫힐 예정
            gameform.Show();                                                    //그리고 쇼
            this.Hide();                                                        //게임 하는동안 메인은 숨기기
        }

        private void Main_Activated(object sender, EventArgs e)                 //메인 화면이 활성화 될 때
        {
            Load_record();                                                      //기록은 가져옴
        }

        private void toolStripMenu_GameStart_Click(object sender, EventArgs e)
        {
            Game_Form gameform = new Game_Form();                               //게임 폼을 가져옴
            gameform.Location = this.Location;                                  //현재 위치에
            gameform.StartPosition = FormStartPosition.Manual;
            gameform.FormClosing += delegate { this.Show(); };                  //닫어도 이건 안닫힐 예정
            gameform.Show();                                                    //그리고 쇼
            this.Hide();                                                        //게임 하는동안 메인은 숨기기
        }
    }
}
