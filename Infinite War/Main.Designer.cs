
namespace Infinite_War
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenu_StartGame = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenu_Option = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenu_Exit = new System.Windows.Forms.ToolStripMenuItem();
            this.label_record = new System.Windows.Forms.Label();
            this.label_record_count = new System.Windows.Forms.Label();
            this.button_gamestart = new System.Windows.Forms.Button();
            this.button_option = new System.Windows.Forms.Button();
            this.button_exit = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenu});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(332, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenu
            // 
            this.toolStripMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenu_StartGame,
            this.toolStripMenu_Option,
            this.toolStripMenu_Exit});
            this.toolStripMenu.Name = "toolStripMenu";
            this.toolStripMenu.Size = new System.Drawing.Size(43, 20);
            this.toolStripMenu.Text = "메뉴";
            // 
            // toolStripMenu_StartGame
            // 
            this.toolStripMenu_StartGame.Name = "toolStripMenu_StartGame";
            this.toolStripMenu_StartGame.Size = new System.Drawing.Size(122, 22);
            this.toolStripMenu_StartGame.Text = "게임시작";
            this.toolStripMenu_StartGame.Click += new System.EventHandler(this.toolStripMenu_GoMain_Click);
            // 
            // toolStripMenu_Option
            // 
            this.toolStripMenu_Option.Name = "toolStripMenu_Option";
            this.toolStripMenu_Option.Size = new System.Drawing.Size(122, 22);
            this.toolStripMenu_Option.Text = "옵션";
            this.toolStripMenu_Option.Click += new System.EventHandler(this.toolStripMenu_Option_Click);
            // 
            // toolStripMenu_Exit
            // 
            this.toolStripMenu_Exit.Name = "toolStripMenu_Exit";
            this.toolStripMenu_Exit.Size = new System.Drawing.Size(122, 22);
            this.toolStripMenu_Exit.Text = "종료";
            this.toolStripMenu_Exit.Click += new System.EventHandler(this.toolStripMenu_Exit_Click);
            // 
            // label_record
            // 
            this.label_record.AutoSize = true;
            this.label_record.Location = new System.Drawing.Point(46, 58);
            this.label_record.Name = "label_record";
            this.label_record.Size = new System.Drawing.Size(42, 15);
            this.label_record.TabIndex = 1;
            this.label_record.Text = "기록 : ";
            // 
            // label_record_count
            // 
            this.label_record_count.AutoSize = true;
            this.label_record_count.Location = new System.Drawing.Point(117, 58);
            this.label_record_count.Name = "label_record_count";
            this.label_record_count.Size = new System.Drawing.Size(14, 15);
            this.label_record_count.TabIndex = 2;
            this.label_record_count.Text = "0";
            // 
            // button_gamestart
            // 
            this.button_gamestart.Location = new System.Drawing.Point(46, 111);
            this.button_gamestart.Name = "button_gamestart";
            this.button_gamestart.Size = new System.Drawing.Size(75, 23);
            this.button_gamestart.TabIndex = 3;
            this.button_gamestart.Text = "게임시작";
            this.button_gamestart.UseVisualStyleBackColor = true;
            this.button_gamestart.Click += new System.EventHandler(this.button_gamestart_Click);
            // 
            // button_option
            // 
            this.button_option.Location = new System.Drawing.Point(46, 151);
            this.button_option.Name = "button_option";
            this.button_option.Size = new System.Drawing.Size(75, 23);
            this.button_option.TabIndex = 4;
            this.button_option.Text = "옵션";
            this.button_option.UseVisualStyleBackColor = true;
            this.button_option.Click += new System.EventHandler(this.button_option_Click);
            // 
            // button_exit
            // 
            this.button_exit.Location = new System.Drawing.Point(46, 194);
            this.button_exit.Name = "button_exit";
            this.button_exit.Size = new System.Drawing.Size(75, 23);
            this.button_exit.TabIndex = 5;
            this.button_exit.Text = "종료";
            this.button_exit.UseVisualStyleBackColor = true;
            this.button_exit.Click += new System.EventHandler(this.button_exit_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(332, 262);
            this.Controls.Add(this.button_exit);
            this.Controls.Add(this.button_option);
            this.Controls.Add(this.button_gamestart);
            this.Controls.Add(this.label_record_count);
            this.Controls.Add(this.label_record);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Main";
            this.Text = "Infinite War";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenu;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenu_StartGame;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenu_Option;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenu_Exit;
        private System.Windows.Forms.Label label_record;
        private System.Windows.Forms.Label label_record_count;
        private System.Windows.Forms.Button button_gamestart;
        private System.Windows.Forms.Button button_option;
        private System.Windows.Forms.Button button_exit;
    }
}

