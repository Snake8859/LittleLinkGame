using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace 连连看_佘文庆
{
    class control
    {
       //   Form1 a = new Form1();
        public int mapCol = 10;  //列数
        public int mapRow = 10;  //行数
        public Point z1, z2;  //折点的坐标
        public int[] m_map = new int[10 * 10];//100个图案的ID
        public int BLANK_STATE = -1;
     
  
        public bool X_Link(int x, int y1, int y2)//X直接连通，即垂直方向连通
        {
            if (y1 > y2)//保证y1的值小于y2
            {
                int n = y1;
                y1 = y2;
                y2 = n;
            }
            //直通
            for (int i = y1 + 1; i <= y2; i++)
            {
                if (i == y2)
                    return true;
                if (m_map[i * mapCol + x] != BLANK_STATE)//用来跳到下一个方块,再进1,行判断
                    break;

            }
            return false;
        }
        public bool Y_Link(int x1, int x2, int y) //Y直接连通，即水平方向连通
        {
            if (x1 > x2)
            {
                int x = x1;
                x1 = x2;
                x2 = x;
            }
            //直通
            for (int i = x1 + 1; i <= x2; i++)
            {
                if (i == x2)
                    return true;
                if (m_map[y * mapCol + i] != BLANK_STATE)  // 用来跳到左或者右一块的，再进行判断
                    break;
            }
            return false;
        }
        //一个折点相当于两个方块划出一个矩形，这两个方块是一对对角顶点.分两个方块的位置
        public bool OneCornerLink(int x1, int y1, int x2, int y2)
        {
            if (x1 > x2) //目标点(x1,y1),(x2,y2)两点交换
            {
                int n = x1;
                x1 = x2;
                x2 = n;
                n = y1;
                y1 = y2;
                y2 = n;

            }
            if (y2 < y1)  //(x1,y1)为矩形左下顶点,(x2,y2)为矩形上顶点
            {
                //判断矩形右下角折点(x2,y1)是否空
                if (m_map[y1 * mapCol + x2] == BLANK_STATE)
                {
                    if (Y_Link(x1, x2, y1) && X_Link(x2, y1, y2)) //判断折点与两个目标点是否直通
                    {
                        z1.X = x2;
                        z1.Y = y1;//保存折点坐标到z1
                        return true;
                    }
                }

                if (m_map[y2 * mapCol + x1] == BLANK_STATE)// 判断矩形左上角折点(x1,y2)是否空
                {
                    if (Y_Link(x2, x1, y2) && X_Link(x1, y2, y1)) //判断折点(x1,y2）与两个目标点是否直通
                    {
                        z1.X = x1;
                        z1.Y = y2;
                        //保存折点坐标到z1
                        return true;
                    }
                }
                return false;
            }


            else // (x1,y1)位矩形左上顶点,(x2,y2）为矩形右下顶点
            {
                //判断矩形左下角折点(x1,y2)是否空
                if (m_map[y2 * mapCol + x1] == BLANK_STATE)
                {
                    if (Y_Link(x1, x2, y2) && X_Link(x1, y1, y2))//判断折点(x1,y2)与两个目标点是否直通
                    {
                        z1.X = x1;
                        z1.Y = y2;//保存折点坐标到z1
                        return true;
                    }

                }
                if (m_map[y1 * mapCol + x2] == BLANK_STATE) //判断矩形右上角折点为(x2,y1)是否空
                {
                    if (Y_Link(x1, x2, y1) && X_Link(x2, y1, y2))  //判断折点(x2,y1)与两个目标点是否直通
                    {
                        z1.X = x2;
                        z1.Y = y1;//保存折点坐标到z1
                        return true;
                    }
                }
                return false;
            }
        }
        //两个折点连通，是按p1(x1,y1)点向4个方向探测新的z2点与p2(x2,y2)点是否形成一个折点连通
        public bool TwoCornerLink(int x1, int y1, int x2, int y2)
        {
            if (x1 > x2)  //确保x2大于x1
            {
                int n = x1;
                x1 = x2;
                x2 = n;
                n = y1;
                y1 = y2;
                y2 = n;
            }
            //右
            int x, y;
            for (x = x1 + 1; x <= mapCol; x++)
            {
                if (x == mapCol)

                    if (XThrough(x2 + 1, y2, true))//两个折点在选中方块的右侧,且两个折点在图案区域之间
                    {
                        z1.X = mapCol;
                        z2.X = mapCol;
                        z1.Y = y2;
                        z2.Y = y1;
                   
                       
                        return true;
                    }
                    else
                        break;
                if (m_map[y1 * mapCol + x] != BLANK_STATE)
                    break;
                if (OneCornerLink(x, y1, x2, y2))
                {
                    z2.X = x;
                      z2.Y = y1;  
                
                    return true;
                }

            }
            //左
            for (x = x1 - 1; x >= -1; x--)
            {
                if (x == -1)

                    if (XThrough(x2 - 1, y2, false))  //两个折点在选中方块的左侧，且两个折点在图案区域之外
                    {
                        z2.X = -1; z2.Y = y1; 
                        z1.X = -1; z1.Y = y2; 
                        return true;
                    }

                    else
                        break;
                if (m_map[y1 * mapCol + x] != BLANK_STATE)
                    break;
                if (OneCornerLink(x, y1, x2, y2))
                {
                    z2.X = x;
                    z2.Y = y1;
                    return true;
                }
            }
            //上
            for (y = y1 - 1; y >= -1; y--)
            {
                if (y == -1)

                    if (YThrough(x2, y2 - 1, false))
                    {
                        z2.X = x1;
                        z2.Y = -1;
                        z1.X = x2;
                        z1.Y = -1;
                        return true;
                    }
                    else
                        break;

                if (m_map[y * mapCol + x1] != BLANK_STATE)
                    break;
                if (OneCornerLink(x1, y, x2, y2))
                {
                    z2.X = x1;
                    z2.Y = y;
                    return true;
                }
            }
            //下
            for (y = y1 + 1; y <= mapRow; y++)
            {
                if (y == mapRow)

                    if (YThrough(x2, y2 + 1, true))
                    {
                        z2.X = x1;
                        z2.Y = mapRow;
                        z1.X = x2;
                        z1.Y = mapRow;
                        return true;
                    }

                    else
                        break;
                if (m_map[y * mapCol + x1] != BLANK_STATE)
                    break;
                if (OneCornerLink(x1, y, x2, y2))
                {
                    z2.X = x1;
                    z2.Y = y;
                    return true;
                }
            }
            return false;
        }
        //若bAdd为true ,则从(x,y)点水平向右直到边界，判断是否全部为空块;若为false则相反
        public bool XThrough(int x, int y, bool bAdd)//水平方向判断到边界的连通性
        {
            if (bAdd)//水平向右判断是否连通（是否全为空)
            {
                for (int i = x; i < mapCol; i++)
                {
                    if (m_map[y * mapCol + i] != BLANK_STATE)//向右移动，判断是否为空
                        return false;
                }
            }
            else   //水平向左判断是否连通
            {
                for (int i = 0; i <= x; i++)
                {
                    if (m_map[y * mapCol + i] != BLANK_STATE)
                    {
                        return false;
                    }
                }

            }
            return true;

        }
        public bool YThrough(int x, int y, bool bAdd)//  垂直方向判断到边界的连通性
        {
            if (bAdd)//垂直方向向下判断是否连通
            {
                for (int i = y; i < mapRow; i++)
                {
                    if (m_map[i * mapCol + x] != BLANK_STATE)
                    {
                        return false;
                    }
                }
            }
            else //垂直方向向上判断是否连通
            {
                for (int i = 0; i <= y; i++)
                {
                    if (m_map[i * mapCol + x] != BLANK_STATE)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
      

    }
    
}
    

