
namespace MySqlDataBuild
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.iptext = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.porttext = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.pwdtext = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.usertext = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.dbcb = new System.Windows.Forms.ComboBox();
            this.nameSpaceText = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.pathCb = new System.Windows.Forms.ComboBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(99, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "数据库名:";
            // 
            // iptext
            // 
            this.iptext.Location = new System.Drawing.Point(164, 78);
            this.iptext.Name = "iptext";
            this.iptext.Size = new System.Drawing.Size(167, 21);
            this.iptext.TabIndex = 3;
            this.iptext.Text = "127.0.0.1";
            this.iptext.TextChanged += new System.EventHandler(this.iptext_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(99, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "IP:";
            // 
            // porttext
            // 
            this.porttext.Location = new System.Drawing.Point(164, 116);
            this.porttext.Name = "porttext";
            this.porttext.Size = new System.Drawing.Size(167, 21);
            this.porttext.TabIndex = 5;
            this.porttext.Text = "3306";
            this.porttext.TextChanged += new System.EventHandler(this.porttext_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(99, 119);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "端口:";
            // 
            // pwdtext
            // 
            this.pwdtext.Location = new System.Drawing.Point(164, 194);
            this.pwdtext.Name = "pwdtext";
            this.pwdtext.Size = new System.Drawing.Size(167, 21);
            this.pwdtext.TabIndex = 9;
            this.pwdtext.Text = "root";
            this.pwdtext.TextChanged += new System.EventHandler(this.pwdtext_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(99, 197);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "密码:";
            // 
            // usertext
            // 
            this.usertext.Location = new System.Drawing.Point(164, 156);
            this.usertext.Name = "usertext";
            this.usertext.Size = new System.Drawing.Size(167, 21);
            this.usertext.TabIndex = 7;
            this.usertext.Text = "root";
            this.usertext.TextChanged += new System.EventHandler(this.usertext_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(99, 159);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 12);
            this.label5.TabIndex = 6;
            this.label5.Text = "用户名:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 310);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 12);
            this.label6.TabIndex = 10;
            this.label6.Text = "生成路径:";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(368, 306);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 20);
            this.button1.TabIndex = 12;
            this.button1.Text = "选择";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(188, 342);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 13;
            this.button2.Text = "生成";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // dbcb
            // 
            this.dbcb.FormattingEnabled = true;
            this.dbcb.Location = new System.Drawing.Point(164, 40);
            this.dbcb.Name = "dbcb";
            this.dbcb.Size = new System.Drawing.Size(167, 20);
            this.dbcb.TabIndex = 14;
            this.dbcb.SelectedIndexChanged += new System.EventHandler(this.dbcb_SelectedIndexChanged);
            this.dbcb.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dbcb_KeyDown);
            this.dbcb.Leave += new System.EventHandler(this.comboBox1_Leave);
            // 
            // nameSpaceText
            // 
            this.nameSpaceText.Location = new System.Drawing.Point(164, 230);
            this.nameSpaceText.Name = "nameSpaceText";
            this.nameSpaceText.Size = new System.Drawing.Size(167, 21);
            this.nameSpaceText.TabIndex = 16;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(99, 233);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 12);
            this.label7.TabIndex = 15;
            this.label7.Text = "命名空间:";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "None",
            "数据库名",
            "自定义"});
            this.comboBox1.Location = new System.Drawing.Point(338, 230);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(106, 20);
            this.comboBox1.TabIndex = 17;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // pathCb
            // 
            this.pathCb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pathCb.FormattingEnabled = true;
            this.pathCb.Location = new System.Drawing.Point(76, 306);
            this.pathCb.Name = "pathCb";
            this.pathCb.Size = new System.Drawing.Size(286, 20);
            this.pathCb.TabIndex = 18;
            this.pathCb.SelectedIndexChanged += new System.EventHandler(this.pathCb_SelectedIndexChanged);
            this.pathCb.KeyDown += new System.Windows.Forms.KeyEventHandler(this.pathCb_KeyDown);
            this.pathCb.Leave += new System.EventHandler(this.pathCb_Leave);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(164, 264);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(167, 21);
            this.textBox1.TabIndex = 20;
            this.textBox1.Text = "utf8";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(99, 267);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 12);
            this.label8.TabIndex = 19;
            this.label8.Text = "charset:";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(98, 346);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(84, 16);
            this.checkBox1.TabIndex = 21;
            this.checkBox1.Text = "清除旧文件";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(280, 346);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(72, 16);
            this.checkBox2.TabIndex = 22;
            this.checkBox2.Text = "兼容类名";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(12, 346);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(84, 16);
            this.checkBox3.TabIndex = 23;
            this.checkBox3.Tag = "";
            this.checkBox3.Text = "新增DB路径";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox3_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(456, 387);
            this.Controls.Add(this.checkBox3);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.pathCb);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.nameSpaceText);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.dbcb);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.pwdtext);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.usertext);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.porttext);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.iptext);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "mysql生成数据表";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox iptext;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox porttext;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox pwdtext;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox usertext;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.ComboBox dbcb;
        private System.Windows.Forms.TextBox nameSpaceText;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ComboBox pathCb;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox3;
    }
}

