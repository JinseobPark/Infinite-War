using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Infinite_War
{
    public partial class Option_form : Form
    {
        public Option_form()
        {
            InitializeComponent();
        }

        private void BGM_button_onoff_Click(object sender, EventArgs e) //BGM 버튼을 눌렀을때
        {
            Option.TurnBgm();       //ONOFF 전환
            if (Option.GetBgmSound())       //텍스트 내용 전환
                BGM_button_onoff.Text = "끄기";
            else
                BGM_button_onoff.Text = "켜기";
            label_BGM_Text();               //label내용도 전환
        }

        private void Effect_button_onoff_Click(object sender, EventArgs e) //effect 버튼을 눌렀을때
        {
            Option.TurnEff();       //ONOFF 전환 
            if (Option.GetEffSound())       //텍스트 내용 전환
                Effect_button_onoff.Text = "끄기";
            else
                Effect_button_onoff.Text = "켜기";
            label_Effect_Text();               //label내용도 전환
        }

        public void label_BGM_Text()        //label 내용
        {
            label_BGM.Text = "배경음";       //배경음의 label에
            if (Option.GetBgmSound())
                label_BGM.Text += " ON";    //현재 상황을 추가
            else
                label_BGM.Text += " OFF";
        }

        public void label_Effect_Text()          //label 내용
        {
            label_Effect.Text = "효과음";         //효과음의 label에
            if (Option.GetEffSound())
                label_Effect.Text += " ON";      //현재 상황을 추가
            else
                label_Effect.Text += " OFF";
        }

        private void Option_form_Load(object sender, EventArgs e)
        {
            label_BGM_Text();           //로드시 내용 세팅
            label_Effect_Text();
        }
    }
}
