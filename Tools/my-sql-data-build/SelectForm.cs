using MySqlDataBuild.Properties;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace MySqlDataBuild
{
    public partial class SelectForm : Form
    {
        public SelectForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(comboBox1.SelectedIndex == -1){
                MessageBox.Show("请选择MySql版本!");
                return;
            }
            byte[] buffer = null;
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    buffer = Resources.MySql_6_x_x;
                    break;
                case 1:
                    buffer = Resources.MySql_8_x_x;
                    break;
            }
            if (comboBox1.SelectedIndex == 2)
            {
                var form = new Form2();
                Hide();
                form.ShowDialog();
            }
            else 
            {
                App.dllBytes = buffer;
                var form = new Form1();
                Hide();
                form.ShowDialog();
            }
            Process.GetCurrentProcess().Kill();
        }

        private void SelectForm_Load(object sender, EventArgs e)
        {
            int.TryParse(Persist.I.version, out var version);
            comboBox1.SelectedIndex = version;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Persist.I.version = comboBox1.SelectedIndex.ToString();
            Persist.SaveData();
        }
    }
}
