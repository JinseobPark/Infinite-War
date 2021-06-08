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

        private void BGM_button_onoff_Click(object sender, EventArgs e)
        {
            Option.TurnBgm();
            if (Option.GetBgmSound())
                BGM_button_onoff.Text = "끄기";
            else
                BGM_button_onoff.Text = "켜기";
            label_BGM_Text();
        }

        private void Effect_button_onoff_Click(object sender, EventArgs e)
        {
            Option.TurnEff();
            if (Option.GetEffSound())
                Effect_button_onoff.Text = "끄기";
            else
                Effect_button_onoff.Text = "켜기";
            label_Effect_Text();
        }

        public void label_BGM_Text()
        {
            label_BGM.Text = "배경음";
            if (Option.GetBgmSound())
                label_BGM.Text += " ON";
            else
                label_BGM.Text += " OFF";
        }

        public void label_Effect_Text()
        {
            label_Effect.Text = "효과음";
            if (Option.GetEffSound())
                label_Effect.Text += " ON";
            else
                label_Effect.Text += " OFF";
        }

        private void Option_form_Load(object sender, EventArgs e)
        {
            label_BGM_Text();
            label_Effect_Text();
        }
    }
}
