namespace Scorpio.GUI
{
    partial class MainForm
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
            this.logbox = new System.Windows.Forms.RichTextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ucRoverGamepad1 = new Scorpio.GUI.Controls.ucRoverGamepad();
            this.ucVivotekController1 = new Scorpio.GUI.Controls.ucVivotekController();
            this.ucStreamControl4 = new Scorpio.GUI.Controls.ucStreamControl();
            this.ucStreamControl3 = new Scorpio.GUI.Controls.ucStreamControl();
            this.ucStreamControl2 = new Scorpio.GUI.Controls.ucStreamControl();
            this.ucStreamControl1 = new Scorpio.GUI.Controls.ucStreamControl();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // logbox
            // 
            this.logbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logbox.Location = new System.Drawing.Point(3, 21);
            this.logbox.Name = "logbox";
            this.logbox.ReadOnly = true;
            this.logbox.Size = new System.Drawing.Size(1154, 218);
            this.logbox.TabIndex = 0;
            this.logbox.Text = "";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.logbox);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox1.Font = new System.Drawing.Font("Arial", 9F);
            this.groupBox1.Location = new System.Drawing.Point(0, 535);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1160, 242);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Events";
            // 
            // ucRoverGamepad1
            // 
            this.ucRoverGamepad1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.ucRoverGamepad1.Font = new System.Drawing.Font("Arial", 9F);
            this.ucRoverGamepad1.Location = new System.Drawing.Point(12, 12);
            this.ucRoverGamepad1.Name = "ucRoverGamepad1";
            this.ucRoverGamepad1.Size = new System.Drawing.Size(457, 245);
            this.ucRoverGamepad1.TabIndex = 9;
            // 
            // ucVivotekController1
            // 
            this.ucVivotekController1.Autofac = null;
            this.ucVivotekController1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ucVivotekController1.Font = new System.Drawing.Font("Arial", 9F);
            this.ucVivotekController1.Location = new System.Drawing.Point(475, 12);
            this.ucVivotekController1.Name = "ucVivotekController1";
            this.ucVivotekController1.Size = new System.Drawing.Size(396, 522);
            this.ucVivotekController1.TabIndex = 8;
            this.ucVivotekController1.VivotekId = null;
            this.ucVivotekController1.Load += new System.EventHandler(this.ucVivotekController1_Load);
            // 
            // ucStreamControl4
            // 
            this.ucStreamControl4.Autofac = null;
            this.ucStreamControl4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ucStreamControl4.CameraId = null;
            this.ucStreamControl4.Location = new System.Drawing.Point(877, 408);
            this.ucStreamControl4.Name = "ucStreamControl4";
            this.ucStreamControl4.Size = new System.Drawing.Size(271, 126);
            this.ucStreamControl4.TabIndex = 4;
            this.ucStreamControl4.Load += new System.EventHandler(this.ucStreamControl4_Load);
            // 
            // ucStreamControl3
            // 
            this.ucStreamControl3.Autofac = null;
            this.ucStreamControl3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ucStreamControl3.CameraId = null;
            this.ucStreamControl3.Location = new System.Drawing.Point(877, 276);
            this.ucStreamControl3.Name = "ucStreamControl3";
            this.ucStreamControl3.Size = new System.Drawing.Size(271, 126);
            this.ucStreamControl3.TabIndex = 4;
            // 
            // ucStreamControl2
            // 
            this.ucStreamControl2.Autofac = null;
            this.ucStreamControl2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ucStreamControl2.CameraId = null;
            this.ucStreamControl2.Location = new System.Drawing.Point(877, 144);
            this.ucStreamControl2.Name = "ucStreamControl2";
            this.ucStreamControl2.Size = new System.Drawing.Size(271, 126);
            this.ucStreamControl2.TabIndex = 4;
            // 
            // ucStreamControl1
            // 
            this.ucStreamControl1.Autofac = null;
            this.ucStreamControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ucStreamControl1.CameraId = null;
            this.ucStreamControl1.Location = new System.Drawing.Point(877, 12);
            this.ucStreamControl1.Name = "ucStreamControl1";
            this.ucStreamControl1.Size = new System.Drawing.Size(271, 126);
            this.ucStreamControl1.TabIndex = 4;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1160, 777);
            this.Controls.Add(this.ucRoverGamepad1);
            this.Controls.Add(this.ucVivotekController1);
            this.Controls.Add(this.ucStreamControl4);
            this.Controls.Add(this.ucStreamControl3);
            this.Controls.Add(this.ucStreamControl2);
            this.Controls.Add(this.ucStreamControl1);
            this.Controls.Add(this.groupBox1);
            this.Name = "MainForm";
            this.Text = "Rysiu Player 2.0";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox logbox;
        private System.Windows.Forms.GroupBox groupBox1;
        private Controls.ucStreamControl ucStreamControl1;
        private Controls.ucStreamControl ucStreamControl2;
        private Controls.ucStreamControl ucStreamControl3;
        private Controls.ucStreamControl ucStreamControl4;
        private Controls.ucVivotekController ucVivotekController1;
        private Controls.ucRoverGamepad ucRoverGamepad1;
    }
}

