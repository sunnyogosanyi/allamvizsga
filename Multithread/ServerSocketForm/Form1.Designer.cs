using System.Windows.Forms;

namespace ServerSocketForm
{
    partial class Form1
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
            this.serverStartButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.serverMessagetextBox = new System.Windows.Forms.TextBox();
            this.inputtextBox = new System.Windows.Forms.TextBox();
            this.clientOutputtextBox = new System.Windows.Forms.TextBox();
            this.sendMessageButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.serverIPtextBox = new System.Windows.Forms.TextBox();
            this.serverPorttextBox = new System.Windows.Forms.TextBox();
            this.clientIPtextBox = new System.Windows.Forms.TextBox();
            this.clientPorttextBox = new System.Windows.Forms.TextBox();
            this.clearButton = new System.Windows.Forms.Button();
            this.leftButton = new System.Windows.Forms.Button();
            this.rightButton = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.initButton = new System.Windows.Forms.Button();
            this.startButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.batteryButton = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.button1 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // serverStartButton
            // 
            this.serverStartButton.Location = new System.Drawing.Point(249, 8);
            this.serverStartButton.Name = "serverStartButton";
            this.serverStartButton.Size = new System.Drawing.Size(75, 23);
            this.serverStartButton.TabIndex = 0;
            this.serverStartButton.Text = "Start Server";
            this.serverStartButton.UseVisualStyleBackColor = true;
            this.serverStartButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Server IP:";
            // 
            // serverMessagetextBox
            // 
            this.serverMessagetextBox.Location = new System.Drawing.Point(6, 10);
            this.serverMessagetextBox.Name = "serverMessagetextBox";
            this.serverMessagetextBox.Size = new System.Drawing.Size(219, 20);
            this.serverMessagetextBox.TabIndex = 2;
            // 
            // inputtextBox
            // 
            this.inputtextBox.Location = new System.Drawing.Point(6, 48);
            this.inputtextBox.Name = "inputtextBox";
            this.inputtextBox.Size = new System.Drawing.Size(219, 20);
            this.inputtextBox.TabIndex = 3;
            // 
            // clientOutputtextBox
            // 
            this.clientOutputtextBox.Location = new System.Drawing.Point(3, 12);
            this.clientOutputtextBox.MaxLength = 65535;
            this.clientOutputtextBox.Multiline = true;
            this.clientOutputtextBox.Name = "clientOutputtextBox";
            this.clientOutputtextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.clientOutputtextBox.Size = new System.Drawing.Size(288, 139);
            this.clientOutputtextBox.TabIndex = 4;
            // 
            // sendMessageButton
            // 
            this.sendMessageButton.Location = new System.Drawing.Point(249, 46);
            this.sendMessageButton.Name = "sendMessageButton";
            this.sendMessageButton.Size = new System.Drawing.Size(75, 23);
            this.sendMessageButton.TabIndex = 5;
            this.sendMessageButton.Text = "Send";
            this.sendMessageButton.UseVisualStyleBackColor = true;
            this.sendMessageButton.Click += new System.EventHandler(this.button2_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Client IP:";
            // 
            // serverIPtextBox
            // 
            this.serverIPtextBox.Location = new System.Drawing.Point(63, 6);
            this.serverIPtextBox.Name = "serverIPtextBox";
            this.serverIPtextBox.Size = new System.Drawing.Size(100, 20);
            this.serverIPtextBox.TabIndex = 7;
            this.serverIPtextBox.Text = "192.168.88.14";
            // 
            // serverPorttextBox
            // 
            this.serverPorttextBox.Location = new System.Drawing.Point(169, 6);
            this.serverPorttextBox.Name = "serverPorttextBox";
            this.serverPorttextBox.Size = new System.Drawing.Size(100, 20);
            this.serverPorttextBox.TabIndex = 8;
            this.serverPorttextBox.Text = "5100";
            // 
            // clientIPtextBox
            // 
            this.clientIPtextBox.Location = new System.Drawing.Point(63, 36);
            this.clientIPtextBox.Name = "clientIPtextBox";
            this.clientIPtextBox.Size = new System.Drawing.Size(100, 20);
            this.clientIPtextBox.TabIndex = 9;
            this.clientIPtextBox.Text = "192.168.88.210";
            // 
            // clientPorttextBox
            // 
            this.clientPorttextBox.Location = new System.Drawing.Point(169, 36);
            this.clientPorttextBox.Name = "clientPorttextBox";
            this.clientPorttextBox.Size = new System.Drawing.Size(100, 20);
            this.clientPorttextBox.TabIndex = 10;
            this.clientPorttextBox.Text = "5200";
            // 
            // clearButton
            // 
            this.clearButton.Location = new System.Drawing.Point(249, 114);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(75, 23);
            this.clearButton.TabIndex = 11;
            this.clearButton.Text = "Clear";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.button3_Click);
            // 
            // leftButton
            // 
            this.leftButton.Location = new System.Drawing.Point(6, 67);
            this.leftButton.Name = "leftButton";
            this.leftButton.Size = new System.Drawing.Size(75, 23);
            this.leftButton.TabIndex = 12;
            this.leftButton.Text = "Left";
            this.leftButton.UseVisualStyleBackColor = true;
            this.leftButton.Click += new System.EventHandler(this.button4_Click);
            // 
            // rightButton
            // 
            this.rightButton.Location = new System.Drawing.Point(189, 67);
            this.rightButton.Name = "rightButton";
            this.rightButton.Size = new System.Drawing.Size(80, 28);
            this.rightButton.TabIndex = 13;
            this.rightButton.Text = "Right";
            this.rightButton.UseVisualStyleBackColor = true;
            this.rightButton.Click += new System.EventHandler(this.button5_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(18, 14);
            this.pictureBox1.MaximumSize = new System.Drawing.Size(5000, 5000);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(3000, 3000);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 15;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // initButton
            // 
            this.initButton.Location = new System.Drawing.Point(6, 85);
            this.initButton.Name = "initButton";
            this.initButton.Size = new System.Drawing.Size(75, 23);
            this.initButton.TabIndex = 16;
            this.initButton.Text = "Init";
            this.initButton.UseVisualStyleBackColor = true;
            this.initButton.Click += new System.EventHandler(this.button6_Click);
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(87, 85);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 17;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.button7_Click);
            // 
            // stopButton
            // 
            this.stopButton.Location = new System.Drawing.Point(168, 85);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(75, 23);
            this.stopButton.TabIndex = 18;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.button8_Click);
            // 
            // batteryButton
            // 
            this.batteryButton.Location = new System.Drawing.Point(249, 85);
            this.batteryButton.Name = "batteryButton";
            this.batteryButton.Size = new System.Drawing.Size(75, 23);
            this.batteryButton.TabIndex = 19;
            this.batteryButton.Text = "Battery";
            this.batteryButton.UseVisualStyleBackColor = true;
            this.batteryButton.Click += new System.EventHandler(this.button9_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 109);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 20;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // panel1
            // 
            this.panel1.AllowDrop = true;
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1313, 835);
            this.panel1.TabIndex = 21;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.clientIPtextBox);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.rightButton);
            this.panel2.Controls.Add(this.serverIPtextBox);
            this.panel2.Controls.Add(this.leftButton);
            this.panel2.Controls.Add(this.serverPorttextBox);
            this.panel2.Controls.Add(this.clientPorttextBox);
            this.panel2.Location = new System.Drawing.Point(15, 18);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(295, 151);
            this.panel2.TabIndex = 23;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.serverMessagetextBox);
            this.panel3.Controls.Add(this.inputtextBox);
            this.panel3.Controls.Add(this.serverStartButton);
            this.panel3.Controls.Add(this.clearButton);
            this.panel3.Controls.Add(this.button1);
            this.panel3.Controls.Add(this.sendMessageButton);
            this.panel3.Controls.Add(this.batteryButton);
            this.panel3.Controls.Add(this.initButton);
            this.panel3.Controls.Add(this.stopButton);
            this.panel3.Controls.Add(this.startButton);
            this.panel3.Location = new System.Drawing.Point(334, 18);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(360, 151);
            this.panel3.TabIndex = 24;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.clientOutputtextBox);
            this.panel4.Location = new System.Drawing.Point(727, 18);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(294, 154);
            this.panel4.TabIndex = 25;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1313, 835);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.Form4_MouseWheel);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button serverStartButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox serverMessagetextBox;
        private System.Windows.Forms.TextBox inputtextBox;
        private System.Windows.Forms.TextBox clientOutputtextBox;
        private System.Windows.Forms.Button sendMessageButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox serverIPtextBox;
        private System.Windows.Forms.TextBox serverPorttextBox;
        private System.Windows.Forms.TextBox clientIPtextBox;
        private System.Windows.Forms.TextBox clientPorttextBox;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.Button leftButton;
        private System.Windows.Forms.Button rightButton;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button initButton;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button batteryButton;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private Panel panel4;
    }
}

