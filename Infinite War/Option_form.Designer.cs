
namespace Infinite_War
{
    partial class Option_form
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
            this.label_BGM = new System.Windows.Forms.Label();
            this.BGM_button_onoff = new System.Windows.Forms.Button();
            this.label_Effect = new System.Windows.Forms.Label();
            this.Effect_button_onoff = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label_BGM
            // 
            this.label_BGM.AutoSize = true;
            this.label_BGM.Location = new System.Drawing.Point(22, 26);
            this.label_BGM.Name = "label_BGM";
            this.label_BGM.Size = new System.Drawing.Size(43, 15);
            this.label_BGM.TabIndex = 0;
            this.label_BGM.Text = "배경음";
            // 
            // BGM_button_onoff
            // 
            this.BGM_button_onoff.Location = new System.Drawing.Point(101, 26);
            this.BGM_button_onoff.Name = "BGM_button_onoff";
            this.BGM_button_onoff.Size = new System.Drawing.Size(75, 23);
            this.BGM_button_onoff.TabIndex = 1;
            this.BGM_button_onoff.Text = "끄기";
            this.BGM_button_onoff.UseVisualStyleBackColor = true;
            this.BGM_button_onoff.Click += new System.EventHandler(this.BGM_button_onoff_Click);
            // 
            // label_Effect
            // 
            this.label_Effect.AutoSize = true;
            this.label_Effect.Location = new System.Drawing.Point(22, 97);
            this.label_Effect.Name = "label_Effect";
            this.label_Effect.Size = new System.Drawing.Size(43, 15);
            this.label_Effect.TabIndex = 2;
            this.label_Effect.Text = "효과음";
            // 
            // Effect_button_onoff
            // 
            this.Effect_button_onoff.Location = new System.Drawing.Point(101, 97);
            this.Effect_button_onoff.Name = "Effect_button_onoff";
            this.Effect_button_onoff.Size = new System.Drawing.Size(75, 23);
            this.Effect_button_onoff.TabIndex = 3;
            this.Effect_button_onoff.Text = "끄기";
            this.Effect_button_onoff.UseVisualStyleBackColor = true;
            this.Effect_button_onoff.Click += new System.EventHandler(this.Effect_button_onoff_Click);
            // 
            // Option_form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(212, 153);
            this.Controls.Add(this.Effect_button_onoff);
            this.Controls.Add(this.label_Effect);
            this.Controls.Add(this.BGM_button_onoff);
            this.Controls.Add(this.label_BGM);
            this.Name = "Option_form";
            this.Text = "Option";
            this.Load += new System.EventHandler(this.Option_form_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_BGM;
        private System.Windows.Forms.Button BGM_button_onoff;
        private System.Windows.Forms.Label label_Effect;
        private System.Windows.Forms.Button Effect_button_onoff;
    }
}