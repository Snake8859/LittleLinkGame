using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

namespace 连连看_佘文庆
{
    public partial class Form2 : Form
    {
        SoundPlayer soundPlayer = new SoundPlayer(); //音效文件对象
        public Form2()
        {
            InitializeComponent();
            axWindowsMediaPlayer1.settings.setMode("loop", true);

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
           
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
           
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.URL = "E:\\KuGou\\宮崎慎二 - ピカチュウ、これなんのカギ# - 铃声版.mp3";
        }
       

        private void Form2_Click(object sender, EventArgs e)
        {
           
        }

        private void axWindowsMediaPlayer1_Enter(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.Visible = false;
        }
    }
}
