
namespace Infinite_War
{
    partial class Game_Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuMain = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuOption = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenu});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(711, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenu
            // 
            this.toolStripMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuMain,
            this.toolStripMenuOption,
            this.toolStripMenuExit});
            this.toolStripMenu.Name = "toolStripMenu";
            this.toolStripMenu.Size = new System.Drawing.Size(43, 20);
            this.toolStripMenu.Text = "메뉴";
            // 
            // toolStripMenuMain
            // 
            this.toolStripMenuMain.Name = "toolStripMenuMain";
            this.toolStripMenuMain.Size = new System.Drawing.Size(122, 22);
            this.toolStripMenuMain.Text = "메인화면";
            this.toolStripMenuMain.Click += new System.EventHandler(this.toolStripMenuMain_Click);
            // 
            // toolStripMenuOption
            // 
            this.toolStripMenuOption.Name = "toolStripMenuOption";
            this.toolStripMenuOption.Size = new System.Drawing.Size(122, 22);
            this.toolStripMenuOption.Text = "옵션";
            this.toolStripMenuOption.Click += new System.EventHandler(this.toolStripMenuOption_Click);
            // 
            // toolStripMenuExit
            // 
            this.toolStripMenuExit.Name = "toolStripMenuExit";
            this.toolStripMenuExit.Size = new System.Drawing.Size(122, 22);
            this.toolStripMenuExit.Text = "종료";
            this.toolStripMenuExit.Click += new System.EventHandler(this.toolStripMenuExit_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Game_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(711, 502);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Game_Form";
            this.Text = "Game";
            this.Load += new System.EventHandler(this.Game_Form_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Game_Form_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Game_Form_KeyDown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenu;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuMain;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuOption;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuExit;
        private System.Windows.Forms.Timer timer1;
    }
}