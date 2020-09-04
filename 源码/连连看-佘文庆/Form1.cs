using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;//ArrayList命名空间
using System.Drawing;
using System.Media;//音频头文件

namespace 连连看_佘文庆
{
   
    public partial class Form1 : Form
    {
        public Bitmap Source; //所有动物图案的图片
        public int W = 50;   //动物方块图案的宽度
        public int GameSize = 10; //布局的大小，即行数和列数
        public bool Select_first = false; //是否已经选中了第一块
        public int x1, y1;   //被选中第一块的地图坐标
        public int x2, y2;   //被选中第二块的地图坐标
        SoundPlayer soundPlayer = new SoundPlayer(); //音效文件对象
        public float s, h, m;
           
        public float score;
        public enum LinkType { LineType, OneCornerType, TwoCornerType }//枚举类，表示三种连通方式
        LinkType LType;//连通方式
        control c = new control(); //实例化一个控制对象
        public Form1()
        {
            
            InitializeComponent();
          
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
           
        }
        private void button2_Click(object sender, EventArgs e) //开始的按钮
        {
            Source = (Bitmap)Image.FromFile("C:\\Users\\Administrator\\Desktop\\C#\\animal.bmp");
            this.pictureBox1.Height = W * (c.mapRow + 2);
            this.pictureBox1.Width = W * (c.mapCol + 2);
            
            this.pictureBox1.Top = 0;
            this.pictureBox1.Left = 0;

            StartNewGame();
            Init_Graphic();
            timer1.Enabled = true;
            this.button2.Enabled = false;
        }

        public void StartNewGame()
        {
            //初始化地图,将地图中所有方块区域位置置为空方块状态
            for (int iNum = 0; iNum < (c.mapCol) * (c.mapRow); iNum++)  //int iNum = 0; iNum < ((mapCol+2) * (mapRow+2)
            {
                c.m_map[iNum] = c.BLANK_STATE;
            }
            Random r = new Random();
            //生成随机地图
            //将所有的动物物种放进一个临时的地图tmpMap中
            ArrayList tmpMap = new ArrayList();
            for (int i = 0; i < (c.mapCol * c.mapRow) / 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    tmpMap.Add(i);
                }
            }
            //每次从上面的临时地图tmpMap中取走(获取后并在临时地图删除)
            //一个动物放到地图的空方块上
            for (int i = 0; i < c.mapCol * c.mapRow; i++)
            {
                //随机挑选一个位置
                int nIndex = r.Next() % tmpMap.Count;
                //获取该选定物件放到地图的空方块
                c.m_map[i] = (int)tmpMap[nIndex];

                //在临时地图tmpMap除去该动物
                tmpMap.RemoveAt(nIndex);
            }
        }
        public void Init_Graphic()
        {

            Graphics g = get_Graphic();              //生成Graphics对象

            for (int i = 0; i < 10 * 10; i++)
            {
                g.DrawImage(create_image(c.m_map[i]), W * (i % GameSize) + W,
                  W * (i / GameSize) + W, W, W);
            }
        }
        //create_image（）方法实现按标号n从所有动物图案的图片中截图。
        public Bitmap create_image(int n)  //按标号n截图 
        {
            Bitmap bit = new Bitmap(W, W);
            Graphics g = Graphics.FromImage(bit);   //生成Graphics对象 
            Rectangle a = new Rectangle(0, 0, W, W);
            Rectangle b = new Rectangle(0, n * 39, 39, 39);
            //截取原图中b矩形区域的图形
            g.DrawImage(Source, a, b, GraphicsUnit.Pixel);
            return bit;
        }
        public Graphics get_Graphic()
        {
            if (pictureBox1.Image == null)
            {
                Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                pictureBox1.Image = bmp;
            }
            Graphics g = Graphics.FromImage(pictureBox1.Image);
            return g;
        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            Cursor.Current = Cursors.Hand;
            Graphics g = this.pictureBox1.CreateGraphics();//在picturebox上面建一个画布
            Pen redpen = new Pen(Color.Red, 3);
            int x, y;
            if (e.Button == MouseButtons.Left)//如果点击鼠标左键
            {
                //获取光标位置坐标，转化为左上角的坐标
                x = (e.X - W) / W;
                y = (e.Y - W) / W;
                if (c.m_map[y * c.mapCol + x] == c.BLANK_STATE)//如果该区域没有方块
                    return;  //直接返回
                if (Select_first == false)  // 第一块选中
                {
                    x1 = x;
                    y1 = y;
                    DrawSelectedBlock(x1, y1, redpen, g); //画(x1,y1)红色方框
                    Select_first = true;
                }
                else
                {
                    x2 = x;
                    y2 = y;
                    if ((x1 == x2) && (y1 == y2)) //判断第二次点击的方块是否已经被第一次点击选取，是则返回
                        return;
                    DrawSelectedBlock(x2, y2, redpen, g); //画(x2,y2)红色方框
                    if (IsSame(x1, y1, x2, y2) && isLinke(x1, y1, x2, y2)) //判断是否连通和两个方块是否相同
                    {
                        DrawLinkLine(x1, y1, x2, y2, LType); //画选中方块之间 的连接线
                        System.Threading.Thread.Sleep(500); //延迟0.5s ？？线程知识?
                        ClearSelectedBlock(x1, y1, g);  //清除第一个方块
                        ClearSelectedBlock(x2, y2, g);  //消除第二个方块
                                                        //清空方块的值,使其ID变为-1，处于消除状态
                        c.m_map[y1 * c.mapCol + x1] = c.BLANK_STATE;
                        c.m_map[y2 * c.mapCol + x2] = c.BLANK_STATE;
                        Select_first = false;
                        unDrawLinkLine(x1, y1, x2, y2, LType);//消除选中方块之间的连接线
                        score = score + 5;//消除成功分数加5
                        label2.Text = "分数   " + score.ToString();  //
                        soundPlay("music\\elim.wav");//消除成功音乐
                        

                    }
                    else //重新选定第一个方块
                    {
                        //重画(x1,y1)处动物图案来达到取消原选定(x1,y1)处的框线
                        int i = y1 * c.mapCol + x1;
                        g.DrawImage(create_image(c.m_map[i]), W * (i % GameSize) + W, W * (i / GameSize) + W, W, W);
                        //设置重新选定第一块方块的坐标
                        x1 = x;
                        y1 = y;
                        Select_first = true;
                        soundPlay("music\\notElim.wav");  //消除失败音效
                    }

                }

            }
            if (IsWin())
            {
                MessageBox.Show("恭喜您胜利闯关,即将开始新局");
                //StartNewGame();
            }
        }

       
      /*  public Graphics GetGraphicsObject(ref PictureBox pic)
        {
            System.Drawing.Graphics g;
            Bitmap bmp = new Bitmap(pic.Width, pic.Height);
            pic.Image = bmp;
            g = Graphics.FromImage(bmp);
            return g;
        }
        */
        public void DrawLinkLine(int x1, int y1, int x2, int y2, LinkType LType)  //画选中方块的线
        {
            Graphics g = this.pictureBox1.CreateGraphics(); //生成Graphics对象
            Pen p = new Pen(Color.Red, 3);
            Point p1 = new Point(x1 * W + W / 2 + W, y1 * W + W / 2 + W);
            Point p2 = new Point(x2 * W + W / 2 + W, y2 * W + W / 2 + W);
            if (LType == LinkType.LineType) //如果是直连的
            {
                g.DrawLine(p, p1, p2);
            }
            if (LType == LinkType.OneCornerType)  //如果是一个折点的
            {
                Point pixel_z1 = new Point(c.z1.X * W + W / 2 + W, c.z1.Y * W + W / 2 + W);
                g.DrawLine(p, p1, pixel_z1);
                g.DrawLine(p, pixel_z1, p2);
            }
            if (LType == LinkType.TwoCornerType)  //如果是两个折点的
            {
                Point pixel_z1 = new Point(c.z1.X * W + W / 2 + W, c.z1.Y * W + W / 2 + W);
                Point pixel_z2 = new Point(c.z2.X * W + W / 2 + W, c.z2.Y * W + W / 2 + W);
                if (!(p1.X == pixel_z2.X || p1.Y == pixel_z2.Y))  //p1与pixel_z2不在一直线上，则pixel_z1和pixel_z2交换??
                {
                    Point c;
                    c = pixel_z1;
                    pixel_z1 = pixel_z2;
                    pixel_z2 = c;
                }

                g.DrawLine(p, p1, pixel_z2);
                g.DrawLine(p, pixel_z2, pixel_z1);
                g.DrawLine(p, pixel_z1, p2);
            }
        }
        public void unDrawLinkLine(int x1, int y1, int x2, int y2, LinkType LType)  //用来消除选中方块之间的连接线
        {
            Graphics g = this.pictureBox1.CreateGraphics(); //生成Graphics对象
            Pen p = new Pen(this.BackColor, 3);
            Point p1 = new Point(x1 * W + W / 2 + W, y1 * W + W / 2 + W);
            Point p2 = new Point(x2 * W + W / 2 + W, y2 * W + W / 2 + W);
            if (LType == LinkType.LineType) //如果是直连的
            {
                g.DrawLine(p, p1, p2);
            }
            if (LType == LinkType.OneCornerType)  //如果是一个折点的
            {
                Point pixel_z1 = new Point(c.z1.X * W + W / 2 + W, c.z1.Y * W + W / 2 + W);
                g.DrawLine(p, p1, pixel_z1);
                g.DrawLine(p, pixel_z1, p2);
            }
            if (LType == LinkType.TwoCornerType)  //如果是两个折点的
            {
                Point pixel_z1 = new Point(c.z1.X * W + W / 2 + W, c.z1.Y * W + W / 2 + W);
                Point pixel_z2 = new Point(c.z2.X * W + W / 2 + W, c.z2.Y * W + W / 2 + W);
                if (!(p1.X == pixel_z2.X || p1.Y == pixel_z2.Y))  //p1与pixel_z2不在一直线上，则pixel_z1和pixel_z2交换??
                {
                    Point c;
                    c = pixel_z1;
                    pixel_z1 = pixel_z2;
                    pixel_z2 = c;
                }

                g.DrawLine(p, p1, pixel_z2);
                g.DrawLine(p, pixel_z2, pixel_z1);
                g.DrawLine(p, pixel_z1, p2);
            }

        }
        
      
        public void DrawSelectedBlock(int x, int y, Pen redPan, Graphics g) //用来画方框的
        {
            Rectangle b1 = new Rectangle(x * W + 1 + W, y * W + 1 + W, W - 3, W - 3);
            g.DrawRectangle(redPan, b1);
        }
        public void ClearSelectedBlock(int x, int y, Graphics g)  //用来清除方框的
        {
            SolidBrush myBursh = new SolidBrush(this.BackColor);//定义背景色画刷
            Rectangle b1 = new Rectangle(x * W + W, y * W + W, W, W);
            g.FillRectangle(myBursh, b1);
        }

        private void button1_Click(object sender, EventArgs e)  //智能匹配的按钮
        {
            if (!Find2Block())
            {
                score = score + 5;
                label2.Text = "分数   " + score.ToString();
                MessageBox.Show("没有连通的方块了！！");
            }
            if (IsWin())
            {
                timer1.Enabled = false;
                MessageBox.Show("恭喜您胜利闯关,即将开始新局");
                //StartNewGame();
            }
        }

        //考虑连通算法，分三种，直连，一个折点，两个折点
        public bool isLinke(int x1, int y1, int x2, int y2)  //判断选中的两个方块是否可以消除
        {
            if (x1 == x2) //X直连方式，即垂直方向连通
            {
                if (c.X_Link(x1, y1, y2))
                {
                    LType = LinkType.LineType;
                    return true;
                }
            }
            else if (y1 == y2) //Y直连方式，即水平方向连通
            {
                if (c.Y_Link(x1, x2, y1))
                {
                    LType = LinkType.LineType;
                    return true;
                }
            }
            if (c.OneCornerLink(x1, y1, x2, y2)) //一个转弯（折点）的连通方式
            {
                LType = LinkType.OneCornerType;
                return true;
            }
            else if (c.TwoCornerLink(x1, y1, x2, y2)) //两个转弯（折点）的连通方式
            {
                LType = LinkType.TwoCornerType;
                return true;
            }
            return false;
        }
        public bool IsSame(int x1, int y1, int x2, int y2)  //用来判断(x1,y1)与(x2,y2)方块图案是否相同
        {
            if (c.m_map[y1 * c.mapCol + x1] == c.m_map[y2 * c.mapCol + x2])
                return true;
            else
                return false;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

       

        public bool Find2Block()
        {
            bool bFound = false;
            //第一个方块从地图的0位置开始
            for (int i = 0; i < c.mapRow * c.mapCol; i++)
            {
                //找到则跳出循环
                if (bFound)
                    break;
                //无动物的空格跳过
                if (c.m_map[i] == c.BLANK_STATE)
                    continue;
                //第二个方块从前一个方块的后面开始
                for (int j = i + 1; j < c.mapRow * c.mapCol; j++)
                {

                    //第二个方块不为空 且与第一个方块的动物相同
                    if (c.m_map[j] != c.BLANK_STATE && c.m_map[i] == c.m_map[j])
                    {
                        //算出对应的虚拟行列位置
                        x1 = i % c.mapCol;
                        y1 = i / c.mapCol;
                        x2 = j % c.mapCol;
                        y2 = j / c.mapCol;

                        //判断是否可以连通
                        if (isLinke(x1, y1, x2, y2))
                        {

                            bFound = true;
                            break;
                        }
                    }
                }
            }
            if (bFound)
            {
                //（x1,y1）与（x2,y2）连通
                Graphics g = this.pictureBox1.CreateGraphics();     //生成Graphics对象
             
                //Graphics g = get_Graphic();              //生成Graphics对象
                
                Pen myPen = new Pen(Color.Blue, 3);
                Rectangle b1 = new Rectangle(x1 * W + 1 + W, y1 * W + 1 + W, W - 3, W - 3);
                g.DrawRectangle(myPen, b1);
                Rectangle b2 = new Rectangle(x2 * W + 1 + W, y2 * W + 1 + W, W - 3, W - 3);
                g.DrawRectangle(myPen, b2);
            }
            return bFound;

        }

        private void button3_Click(object sender, EventArgs e)   //重新开始的按钮
        {
            pictureBox1.Visible = false;
            pictureBox1.Visible = true;
            score = 0;
            label2.Text = "分数   " + score.ToString();
            s = 0;m = 0;h = 0;

            StartNewGame();
            Init_Graphic();


            //如何写重新开始
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();

        }

        private void label3_Click(object sender, EventArgs e)
        {
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            s++;
            if (s == 60)
            {
                m++;
                s = 0;
            }
            if (m == 60)
            {
                h++;
                m = 0;
            }
            label3.Text = "Time   "+h.ToString("00") + ":" + m.ToString("00") + ":" + s.ToString("00");//系统默认的一个格式
            if (m>= 5)
            {
                timer1.Enabled = false;
                MessageBox.Show("你已经超时了,请重新开始");
               
                                           //需要加一句把窗体关掉或者重新开始的语句
            }
            
        }

        public bool IsWin()
        {
            //检测所有是否尚有非未被消除的方块
            // (非BLANK_STATE状态)
            for (int i = 0; i < c.mapRow * c.mapCol; i++)
            {
                if (c.m_map[i] != c.BLANK_STATE)
                {
                    return false;
                }
            }
            return true;
        }
        public void soundPlay(string s)//写一个函数来调用播放音乐
        { 
            soundPlayer.SoundLocation = s;
            soundPlayer.Load();
            soundPlayer.Play();

        }

        //可加限时，分数，换图块，加消除时候的背景音乐，自动查找
    }
}
