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

        private void Form1_Load(object sender, EventArgs e)
        {
            Score.SetRecordScore(0);;
            Load_record();
        }

        private void Load_record()
        {
            label_record_count.Text = "";

            try
            {
                StreamReader sr = new StreamReader("../../../record.txt");
                label_record_count.Text = sr.ReadToEnd();
                Score.SetRecordScore(int.Parse(label_record_count.Text));
                sr.Close();
            }
            catch (Exception ex)
            {
                string message = "Exception Type : " + ex.GetType() + "\nMessage : " + ex.Message + "\nStack Trace : " + ex.StackTrace + "\n\n";
                using (FileStream fs = new FileStream("../../../error.log", FileMode.Append))
                {
                    string time = "Error Time : " + DateTime.Now.ToString() + "\n";
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, time + message);
                    fs.Close();
                }
                Save_record();
            }
        }

        private void Save_record()
        {
            label_record_count.Text = Score.getRecordScore().ToString();
            using (StreamWriter sw = new StreamWriter("../../../record.txt"))
            {
                sw.Write(label_record_count.Text);
                sw.Close();
            }
        }

        private void toolStripMenu_GoMain_Click(object sender, EventArgs e)
        {
        }
        private void toolStripMenu_Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void toolStripMenu_Option_Click(object sender, EventArgs e)
        {
            if(Application.OpenForms["Option_form"] as Option_form == null)
            {
                Option_form option_form = new Option_form();
                option_form.Show();
            }
        }

        private void button_option_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms["Option_form"] as Option_form == null)
            {
                Option_form option_form = new Option_form();
                option_form.Show();
            }
        }

        private void button_exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button_gamestart_Click(object sender, EventArgs e)
        {
            Game_Form gameform = new Game_Form();
            gameform.Location = this.Location;
            gameform.StartPosition = FormStartPosition.Manual;
            gameform.FormClosing += delegate { this.Show(); };
            gameform.Show();
            this.Hide();
        }
    }
}
