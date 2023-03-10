using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace Game
{
    public partial class Form1 : Form
    {
        private static int numQueen = 4 ;//Số lượng hậu
        private ArrayList  listBtn = new ArrayList();
        private ArrayList listLabel = new ArrayList();
        private int[,] currentState = null;
        private Button currentBtnActive = null;
        private bool isActive = false;
        private ArrayList btnAvailble = new ArrayList();
        private static ArrayList listResult = new ArrayList();//Kết quả tìm thấy
        private bool isWinGame = false; // Đã win game chưa
        private static int luotdi = 0; //Số lượt đi
        private static int[] tmp = new int[20];//Mảng dùng trâu cày

        private static int pointplayer = 0; //Điểm của player

        private static int pointofAI = 0; // Điểm của AI
        private static bool isAIplayed = false; //Lượt của AI
        private static int speedAI = 300; //Tốc độ của AI

        

        //Thuật toán trâu cày tìm nước đi đúng 
        private static bool check(int[] a, int x2, int y2)
        {
            for (int i = 1; i < x2; i++)
                if (a[i] == y2 || Math.Abs(i-x2) == Math.Abs(a[i] - y2))
                    return false;
            return true;
        }
        private static void tryGetResult(int i, int row)
        {
            for (int j = 1; j<=row; j++)
            {
                // th? d?t quân h?u vào các c?t t? 1 d?n n
                if (check(tmp, i, j))
                {
                    tmp[i] = j;
                    if (i==row)
                    {
                        int[] res = new int[row];
                        for (int k = 1; k<=numQueen; ++k)
                        {
                            res[k-1] = tmp[k];
                        }
                        listResult.Add(res);
                    }
                    tryGetResult(i+1, row);
                }
            }
        }


        //Check nước đi người chơi có đúng không
        private static bool checkRow(int[,] virState, int row, int column, int rowGrowth, int columnGrowth)
        {
            try
            {
                while (true)
                {
                    row += rowGrowth;
                    column += columnGrowth;
                    if (virState[row, column] == 1)
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            {

            }
            return true;
        }

        private bool checkLegit(int[,] virState)
        {
            bool canContinue = true;
            for (int i = 0; i<numQueen; ++i)
            {
                for (int j = 0; j<numQueen; ++j)
                {
                    if (virState[i, j] == 1)
                    {
                        bool ctp = checkRow(virState, i, j, -1, 1);
                        bool ctt = checkRow(virState, i, j, -1, -1);
                        bool cdp = checkRow(virState, i, j, 1, 1);
                        bool cdt = checkRow(virState, i, j, 1, -1);
                        bool trucXright = checkRow(virState, i, j, 0, 1);
                        bool trucXleft = checkRow(virState, i, j, 0, -1);
                        bool trucYtop = checkRow(virState, i, j, -1, 0);
                        bool trucYbottom = checkRow(virState, i, j, 1, 0);
                        bool axis = trucXleft && trucXright && trucYbottom && trucYtop;
                        canContinue = ctp && ctt && cdp && cdt && axis;
                    }
                    if (!canContinue)
                    {
                        return false;
                    }
                }

            }
            int pointGet = Math.Abs(1000 - luotdi * 10)%1000 > 0 ? Math.Abs(1000 - luotdi * 10)%1000 : 100;
            if (!isAIplayed)
            {
                pointplayer += 300 * numQueen +  pointGet;
                pointPlayer.Text = pointplayer.ToString();
            }
            else
            {
                pointofAI += 150 * numQueen +  pointGet;
                //pointofAI += 1;
                pointAI.Text = pointofAI.ToString();
            }
            return true;
        }

        private void analytic()
        {
            string msg = "";
            for (int i = 0; i<numQueen; i++)
            {
                for (int j = 0; j<numQueen; j++)
                {
                    msg += currentState[i, j].ToString() +"     ";
                }
                msg+= Environment.NewLine;
            }
            msg += $"Nút đang chọn : [{getLocationArray(currentBtnActive).X},{getLocationArray(currentBtnActive).Y}] - {getLocationArray(currentBtnActive).X * numQueen + getLocationArray(currentBtnActive).Y}";
            analytics.Text = msg;
        }
        public Point getLocationArray(Button btn)
        {
            Point res = new Point();
            int idx = 0;
            foreach(Button button in listBtn)
            {
                if(btn == button)
                {
                    res.X = idx/numQueen;
                    res.Y = idx%numQueen;
                    break;
                }
                idx++;
            }
            return res;
        }
        public int getLocationList(Button btn)
        {
            Point x = getLocationArray(btn);
            return x.X * numQueen + x.Y;
        }

        public Point convertIntToArraynumQueen(int x)
        {
            return new Point(x/numQueen,x%numQueen);
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void clearColor()
        {
            foreach (Button btn in listBtn)
            {
                btn.BackColor = Color.White;
            }
        }
        
        private int getRandom(int endRange = 0)
        {
            if(endRange == 0)
            return new Random().Next(0, numQueen * numQueen);
            return new Random().Next(0, endRange);
        }
        private void btnClick(object sender,EventArgs e)
        {
            
            //Khi nút đang null
            if(currentBtnActive == null || currentBtnActive.BackgroundImage == null)
            {
                btnAvailble.Clear();
                currentBtnActive = (sender as Button);
                if(currentBtnActive.BackgroundImage != null)
                {
                    currentBtnActive.BackColor = Color.Blue;
                    isActive = true;
                }
                //logs.Text = Environment.NewLine + "Khi nút đang null";
            }
            //Khi nút có image 
            else
            {
                if((sender as Button) == currentBtnActive) // Bấm vào chính nó
                {
                    currentBtnActive = null;
                    //Xoá color của chính nó 
                    (sender as Button).BackColor = Color.White;
                    testbox.Focus();
                    //logs.Text = Environment.NewLine + "Bấm vào chính nút đó!";
                    //Disable active
                    isActive = false;

                    //Xoá hết các nút khả dụng
                    btnAvailble.Clear();
                }    
                else if((sender as Button).BackgroundImage == null) // Nước đi của hậu
                {
                    if (isWinGame)
                    {
                        isWinGame = false;
                        
                        winLabel.Visible = false;
                        loadGame();
                        return;
                    }
                    if (btnAvailble.Contains(sender) == false)
                    {
                        logs.Text = "Nước đi không hợp lệ !";
                        return;
                    }
                    //Clear màu cũ 
                    clearColor();
                    winLabel.Visible = false;

                    // Dịch chuyển vị trí trong state
                    Point current = getLocationArray(currentBtnActive);
                    Point next = getLocationArray((sender as Button));
                    int tmp = currentState[current.X, current.Y];
                    currentState[current.X, current.Y] = currentState[next.X, next.Y];
                    currentState[next.X, next.Y] = tmp;

                    //Dịch chuyển vị trí trên GUI
                    currentBtnActive.BackColor = Color.White;
                    currentBtnActive.BackgroundImage = null;
                    currentBtnActive = (sender as Button);
                    currentBtnActive.BackgroundImage = Properties.Resources.chess64;
                    currentBtnActive.BackColor = Color.Blue;
                    if (checkLegit(currentState)) // Win game
                    {
                        winLabel.Visible = true;
                        currentBtnActive = null;
                        btnAvailble.Clear();
                        isActive = false;
                        //testAIwin();
                        isWinGame = true;
                        if (isAIplayed)
                        {
                            winLabel.Text = "AI đã giải xong !";
                        }
                        else
                        {
                            winLabel.Text = "Chúc mừng bạn đã thắng!";
                        }
                    }
                    else
                    {
                        //Enable active
                        isActive = true;
                    }
                    luotdi += 1;
                    turnLabel.Text = luotdi.ToString();
                    //logs.Text = Environment.NewLine + "Nước đi của hậu!";
                }
                else if((sender as Button).BackgroundImage != null) //Chọn hậu mới
                {
                    //Dịch chuyển vị trí trên GUI
                    currentBtnActive.BackColor= Color.White;
                    clearColor();

                    //Dịch chuyển btnActive
                    currentBtnActive = (sender as Button);
                    currentBtnActive.BackColor = Color.Blue;

                    //Enable Active
                    isActive =true;
                    btnAvailble.Clear();
                    //logs.Text = Environment.NewLine + "Chọn hậu mới";

                }
            }

            analytic();
            //Vẽ nước đi khả dụng

            // 1 quân cờ 
            // chéo trên trái : - (numQueen + 1) * số hàng
            // chéo trên phải : - (numQueen - 1) * số hàng

            // chéo dưới trái : + (numQueen - 1) * số hàng
            // chéo dưới phải : + (numQueen + 1) * số hàng
            if (isActive == true)
            {
                btnAvailble.Clear();
                //Vẽ chéo trên phải
                int count = 1;
                while (getLocationList(currentBtnActive) - (numQueen - 1)*count >= 0)
                {
                    Point LocationCurrent = convertIntToArraynumQueen(getLocationList(currentBtnActive));
                    Point LocationNext = convertIntToArraynumQueen(getLocationList(currentBtnActive) - (numQueen - 1)*count);
                    bool isValid =  LocationCurrent.Y + count == LocationNext.Y ;
                    if (isValid)
                    {
                        if ((listBtn[LocationNext.X * numQueen + LocationNext.Y] as Button).BackgroundImage == null)
                        {
                            (listBtn[getLocationList(currentBtnActive) - (numQueen - 1)*count] as Button).BackColor = Color.Yellow;
                            btnAvailble.Add(listBtn[getLocationList(currentBtnActive) - (numQueen - 1)*count]);
                        }
                        else
                        {
                            (listBtn[getLocationList(currentBtnActive) - (numQueen - 1)*count] as Button).BackColor = Color.Red;
                            break;
                        }
                    }
                    count++;
                }

                //Vẽ chéo trên trái 
                count = 1;
                while (getLocationList(currentBtnActive) - (numQueen + 1)*count >= 0)
                {
                    Point LocationCurrent = convertIntToArraynumQueen(getLocationList(currentBtnActive));
                    Point LocationNext = convertIntToArraynumQueen(getLocationList(currentBtnActive) - (numQueen + 1)*count);
                    bool isValid = LocationCurrent.Y - count == LocationNext.Y;
                    if (isValid)
                    {
                        if ((listBtn[LocationNext.X * numQueen + LocationNext.Y] as Button).BackgroundImage == null)
                        {
                            (listBtn[getLocationList(currentBtnActive) - (numQueen + 1)*count] as Button).BackColor = Color.Yellow;
                            btnAvailble.Add(listBtn[getLocationList(currentBtnActive) - (numQueen + 1)*count]);
                        }
                        else
                        {
                            (listBtn[getLocationList(currentBtnActive) - (numQueen + 1)*count] as Button).BackColor = Color.Red;
                            break;
                        }
                    }
                    count++;
                }

                //Vẽ chéo dưới trái 
                count = 1;
                while (getLocationList(currentBtnActive) + (numQueen - 1)*count <= numQueen * (numQueen) - 1)
                {
                    Point LocationCurrent = convertIntToArraynumQueen(getLocationList(currentBtnActive));
                    Point LocationNext = convertIntToArraynumQueen(getLocationList(currentBtnActive) + (numQueen - 1)*count);
                    bool isValid =  LocationCurrent.Y - count == LocationNext.Y;
                    if (isValid)
                    {
                        if ((listBtn[LocationNext.X * numQueen + LocationNext.Y] as Button).BackgroundImage == null)
                        {
                            (listBtn[getLocationList(currentBtnActive) + (numQueen - 1)*count] as Button).BackColor = Color.Yellow;
                            btnAvailble.Add(listBtn[getLocationList(currentBtnActive) + (numQueen - 1)*count]);
                        }
                        else
                        {
                            (listBtn[getLocationList(currentBtnActive) + (numQueen - 1)*count] as Button).BackColor = Color.Red;
                            break;
                        }
                    }
                    count++;
                }

                //Vẽ chéo dưới phải 
                count = 1;
                while (getLocationList(currentBtnActive) + (numQueen + 1)*count <= numQueen * (numQueen ) - 1)
                {
                    Point LocationCurrent = convertIntToArraynumQueen(getLocationList(currentBtnActive));
                    Point LocationNext = convertIntToArraynumQueen(getLocationList(currentBtnActive) + (numQueen + 1)*count);
                    bool isValid =  LocationCurrent.Y + count == LocationNext.Y;
                    if (isValid)
                    {
                        if ((listBtn[LocationNext.X * numQueen + LocationNext.Y] as Button).BackgroundImage == null)
                        {
                            (listBtn[getLocationList(currentBtnActive) + (numQueen + 1)*count] as Button).BackColor = Color.Yellow;
                            btnAvailble.Add(listBtn[getLocationList(currentBtnActive) + (numQueen + 1)*count]);
                        }
                        else
                        {
                            (listBtn[getLocationList(currentBtnActive) + (numQueen + 1)*count] as Button).BackColor = Color.Red;
                            break;
                        }
                    }
                    count++;
                }

                int currentRow = getLocationArray(currentBtnActive).X;
                int currentCol = getLocationArray(currentBtnActive).Y;

                // Vẽ trục Y trên
                for(int i = currentRow -1; i>=0; --i)
                {
                    if(convertIntToArraynumQueen(i * numQueen + currentCol).Y == currentCol)
                    {
                        //Quân cờ này không có background Image ( không phải hậu )
                        if ((listBtn[i * numQueen + currentCol] as Button).BackgroundImage == null)
                        {
                            //Thì tô nước đi nó màu vàng
                            (listBtn[i * numQueen + currentCol] as Button).BackColor= Color.Yellow;
                            btnAvailble.Add(listBtn[i * numQueen + currentCol]);
                        }
                        else
                        {
                            (listBtn[i * numQueen + currentCol] as Button).BackColor= Color.Red;
                            break;
                        }
                    }
                }

                // Vẽ trục Y dưới
                for (int i = currentRow + 1; i < numQueen; ++i)
                {
                    if (convertIntToArraynumQueen(i * numQueen + currentCol).Y == currentCol)
                    {
                        //Quân cờ này không có background Image ( không phải hậu )
                        if ((listBtn[i * numQueen + currentCol] as Button).BackgroundImage == null)
                        {
                            //Thì tô nước đi nó màu vàng
                            (listBtn[i * numQueen + currentCol] as Button).BackColor= Color.Yellow;
                            btnAvailble.Add(listBtn[i * numQueen + currentCol]);
                        }
                        else
                        {
                            (listBtn[i * numQueen + currentCol] as Button).BackColor= Color.Red;
                            break;
                        }
                    }
                }

                // Vẽ trục X trái
                for (int i = currentCol - 1; i >= 0; --i)
                {
                    if (convertIntToArraynumQueen(currentRow * numQueen + i).X == currentRow)
                    {
                        if (convertIntToArraynumQueen(currentRow * numQueen + i).X == currentRow)
                        {
                            //Quân cờ này không có background Image ( không phải hậu )
                            if ((listBtn[currentRow * numQueen + i] as Button).BackgroundImage == null)
                            {
                                //Thì tô nước đi nó màu vàng
                                ((listBtn[currentRow * numQueen + i] as Button)).BackColor= Color.Yellow;
                                btnAvailble.Add(listBtn[currentRow * numQueen + i]);
                            }
                            else
                            {
                                ((listBtn[currentRow * numQueen + i] as Button)).BackColor= Color.Red;
                                break;
                            }
                        }
                    }
                }

                // Vẽ trục X phải
                for (int i = currentCol + 1; i < numQueen; ++i)
                {
                    if (convertIntToArraynumQueen(currentRow * numQueen + i).X == currentRow)
                    {
                        //Quân cờ này không có background Image ( không phải hậu )
                        if ((listBtn[currentRow * numQueen + i] as Button).BackgroundImage == null)
                        {
                            //Thì tô nước đi nó màu vàng
                            ((listBtn[currentRow * numQueen + i] as Button)).BackColor= Color.Yellow;
                            btnAvailble.Add(listBtn[currentRow * numQueen + i]);
                        }
                        else
                        {
                            ((listBtn[currentRow * numQueen + i] as Button)).BackColor= Color.Red;
                            break;
                        }
                    }
                }
            }
            else
            {
                clearColor();
            }
        }
        private bool validate(int[] a, int x)
        {
            for(int i = 0; i < a.Length; i++)
            {
                if(x == a[i])
                {
                    return false;
                }
            }
            return true;
        }
        private void random()
        {
            int num = numQueen;
            int[] res = new int[num];
            for(int i = 0; i<num; ++i)
            {
                res[i] = -1;
            }
            int index = 0;
            for(int i=0; i<num; i++)
            {
                int rd = getRandom();
                while (!validate(res,rd))
                {
                    rd = getRandom();
                }
                res[index++] = rd;
                currentState[int.Parse((rd/numQueen).ToString()),rd%numQueen] = 1;
                ((Button)listBtn[rd]).BackgroundImage = Properties.Resources.chess64;
            }    
        }
        private void loadGame()
        {
            isWinGame = false;
            luotdi = 0;
            turnLabel.Text = "0";
            foreach (Button btn in listBtn)
            {
                this.Controls.Remove(btn);
            }
            foreach (Label lb in listLabel)
            {
                this.Controls.Remove(lb);
            }
            listBtn = new ArrayList();
            currentBtnActive = null;
            isActive = false;
            btnAvailble = new ArrayList();
            currentState = new int[numQueen, numQueen];
            int stepX = 65;
            int stepY = 65;
            for (int i = 0; i < numQueen; i++)
            {
                for (int j = 0; j < numQueen; j++)
                {
                    Button btn = new Button();
                    this.Controls.Add(btn);
                    btn.Location = new Point(50 + 160 - numQueen*20 + stepX * j, 50 + +160 - numQueen*20  + stepY * i);
                    btn.Size = new Size(64, 64);
                    btn.Text = "";
                    btn.BackColor = Color.White;
                    btn.Click += new EventHandler(this.btnClick);
                    this.listBtn.Add(btn);
                }
            }
            for(int i=0;i<numQueen; i++)
            {
                Label lb = new Label();
                this.Controls.Add(lb);
                lb.Location = new Point(65 + 160 - numQueen*20 + stepX * i, 10 +160 - numQueen*20);
                lb.Size = new Size(20, 20);
                lb.Text = i.ToString();
                Font big = new Font("Arial", 14);
                lb.Font = big;
                listLabel.Add(lb);
            }
            for (int i = 0; i<numQueen; i++)
            {
                Label lb = new Label();
                this.Controls.Add(lb);
                lb.Location = new Point(10 + 160 - numQueen*20, 65 + +160 - numQueen*20  + stepY * i);
                lb.Size = new Size(20, 20);
                lb.Text = i.ToString();
                Font big = new Font("Arial", 14);
                lb.Font = big;
                listLabel.Add(lb);
            }
            random();
            testbox.Focus();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            loadGame();
            isWinGame = false;
            levelSelect.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {
            
        }

        private void levelSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            string level = levelSelect.SelectedItem.ToString();
            if(level.ToLower().Equals("dễ"))
            {
                numQueen = 4;
            }
            else if(level.ToLower().Equals("trung bình"))
            {
                numQueen = 6;
            }
            else
            {
                numQueen = 8;
            }
            loadGame();
        }
        private void calledButton(int i)
        {
            Task.Delay(speedAI).Wait();
            (listBtn[i] as Button).PerformClick();
            Application.DoEvents();

        }
        private void calledButtonObject(Button btn)
        {
            Task.Delay(speedAI).Wait();
            btn.PerformClick();
            Application.DoEvents();
        }
        private void testAIwin()
        {
            logs.Text = "";
            for (int i = 0; i<numQueen*numQueen; i++)
            {
                if ((listBtn[i] as Button).BackgroundImage != null)
                {
                    calledButton((i));
                    logs.Text += $"Click nút [{convertIntToArraynumQueen(i).X +1},{convertIntToArraynumQueen(i).Y +1}]" +Environment.NewLine;
                    
                }
            }
        }
        private void up(ref int currentX, ref int currentY, int x, int y)
        {
            logs.Text = "Up";
            //Check xem đi thẳng được mấy nút
            while (currentX != x &&(listBtn[(currentX - 1)*numQueen + currentY] as Button).BackgroundImage == null)
            {
                currentX--;
            }
            //Bấm vào nút max mà nó có thể tới
            calledButton(currentX*numQueen + currentY);
        }
        private void left(ref int currentX, ref int currentY, int x, int y)
        {
            logs.Text = "Left";
            //Check xem đi trái được mấy nút
            while (y <= currentY && (listBtn[(currentX)*numQueen + currentY - 1] as Button).BackgroundImage == null)
            {
                currentY--;
                if (y==currentY) { break; }

            }
            //Bấm vào nút max mà nó có thể tới
            calledButton(currentX*numQueen + currentY);
        }
        private void right(ref int currentX, ref int currentY, int x, int y)
        {
            logs.Text = "Right";
            //Check xem đi phải được mấy nút
            while (y >= currentY  && (listBtn[(currentX)*numQueen + currentY + 1] as Button).BackgroundImage == null)
            {
                currentY++;
                if (y==currentY) { break; }
            }
            //Bấm vào nút max mà nó có thể tới
            calledButton(currentX*numQueen + currentY);
        }
        private void down(ref int currentX, ref int currentY, int x, int y)
        {
            logs.Text = "Down";
            //Check xem đi xuống được mấy nút
            while (currentX != x &&(listBtn[(currentX + 1)*numQueen + currentY] as Button).BackgroundImage == null)
            {
                currentX++;
            }
            //Bấm vào nút max mà nó có thể tới
            calledButton(currentX*numQueen + currentY);
        }
        private void leftDown(ref int currentX,ref int currentY, int x, int y)
        {
            logs.Text = "LeftDown";
            
            calledButton(currentX*numQueen + currentY);
            //Check xem đi chéo trái được mấy nút
            try
            {
                while (currentX != x && currentY != y &&(listBtn[(currentX + 1)*numQueen + currentY -1] as Button).BackgroundImage == null)
                {
                    currentX++;
                    currentY--;
                }
            }
            catch (Exception) { }
            //Bấm vào nút max mà nó có thể tới
            calledButton(currentX*numQueen + currentY);
        }
        private void rightDown(ref int currentX,ref int currentY, int x, int y)
        {
            logs.Text = "RightDown";
            calledButton(currentX*numQueen + currentY);
            
            //Check xem đi chéo trái được mấy nút
            try
            {
                while (currentX != x && currentY != y &&(listBtn[(currentX + 1)*numQueen + currentY +1] as Button).BackgroundImage == null)
                {
                    currentX++;
                    currentY++;
                }
            }
            catch (Exception) { }
            //Bấm vào nút max mà nó có thể tới
            calledButton(currentX*numQueen + currentY);
        }
        private void GoTo(int currentX,int currentY, int x, int y, bool normal = true)
        {
            calledButton(currentX*numQueen + currentY);
            int count = 0;
            //Khi mà chưa cùng hàng
            while (currentX != x)
            {
                count+=1;
                if (count==1000)
                {
                    //winLabel.Text = "Can't Solve";
                    //winLabel.Visible = true;
                    return;
                }
                //Có thể đi thẳng
                if ( normal &&currentX > x && (listBtn[(currentX - 1)*numQueen + currentY] as Button).BackgroundImage == null)
                {
                    up(ref currentX, ref currentY, x, y);   
                }
                //Có thể rẽ trái 
                else if (normal && y <= currentY && (listBtn[(currentX)*numQueen + currentY - 1] as Button).BackgroundImage == null)
                {
                    left(ref currentX, ref currentY, x, y);
                }
                //Có thể rẽ phải
                else if(normal && y >= currentY && (listBtn[(currentX)*numQueen + currentY + 1] as Button).BackgroundImage == null)
                {
                   right(ref currentX, ref currentY, x, y);
                }
                //Fix lỗi chưa xếp đúng 
                else if (!normal && currentX < numQueen && (listBtn[(currentX + 1)*numQueen + currentY] as Button).BackgroundImage == null)
                {
                    down(ref currentX, ref currentY, x, y);
                    if(y <= currentY && (listBtn[(currentX)*numQueen + currentY - 1] as Button).BackgroundImage == null)
                    {
                        left(ref currentX, ref currentY, x, y);
                    }
                    else if(y >= currentY && (listBtn[(currentX)*numQueen + currentY + 1] as Button).BackgroundImage == null)
                    {
                        right(ref currentX, ref currentY, x, y);
                    }
                }
            }
            calledButton(currentX*numQueen + y);
            /*winLabel.Text = $"Tới [{x},{y}] ! Xong";
            winLabel.Visible = true;*/
            Console.ReadLine();
        }
        private void reCheck(ArrayList test,int idx, bool isTry = false)
        {
            //Di chuyển các nút cùng hàng xuống đáy
            for (int i = 0; i<numQueen; ++i)
            {
                int QueenperRow = 0;
                for (int j = 0; j<numQueen; ++j)
                {
                    if (currentState[i, j] == 1)
                    {
                        QueenperRow+= 1;
                        if (QueenperRow > 1)
                        {
                            try
                            {
                                GoTo(i, j, (test[numQueen - QueenperRow + 1] as int[])[0], (test[numQueen - QueenperRow + 1] as int[])[1], isTry);
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
            }

            //Check lại lần cuối xem đúng chưa
            idx = 0;
            for (int i = 0; i<numQueen; ++i)
            {
                for (int j = 0; j<numQueen; ++j)
                {
                    if (currentState[i, j] == 1)
                    {
                        try
                        {
                            if (i>=(test[idx] as int[])[0])
                            {
                                GoTo(i, j, (test[idx] as int[])[0], (test[idx] as int[])[1]);
                                idx++;
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
            }
        }
        //private bool isAIalways = false;
        private void button1_Click_1(object sender, EventArgs e)
        {

            /*isAIalways = !isAIalways;
            while (isAIalways)
            {
                startAI();
            }*/
            startAI();
        }

        private ArrayList getSolutionArrayList()
        {
            listResult.Clear();
            tryGetResult(1, numQueen);
            ArrayList solution = new ArrayList();
            int row = 0;
            int[] res_rd = (listResult[getRandom(listResult.Count)] as int[]);
            foreach (int pos in res_rd)
            {
                solution.Add(new int[] { row++, pos-1 });
            }
            return solution;
        }
        private void reFix(ArrayList test)
        {
            //Di chuyển 1 ô xuống dưới nếu có 2 ô cùng hàng
                    for (int i = 0; i<numQueen; ++i)
                    {
                        int count = 0;
                        for (int j = 0; j<numQueen; ++j)
                        {
                            if (currentState[i,j] == 1)
                            {
                                count+=1;
                                if(count > 1)
                                {
                                    try
                                    {
                                        int curX = i;
                                        int curY = j;
                                        leftDown(ref curX,ref curY, 100,100);
                                        rightDown( ref curX, ref curY, 100, 100);
                                    }
                                    catch (Exception) 
                                    {
                                        MessageBox.Show("Không thể solve trái xuống!");
                                    }
                                }
                            }
                        }
                    }
                    reCheck(test, 0);
        }
        private void startAI()
        {
            /*if(numQueen == 4 || numQueen == 6)
            {
                MessageBox.Show("AI không bật ở chế độ dễ !");
                return;
            }*/
            isAIplayed = true;
            //Running AI
            {
                int idx = 0;
                //Cùng 1 hàng
                //loadGame();
                Application.DoEvents();
                Task.Delay(speedAI * 3);
                ArrayList test = getSolutionArrayList();
                string msg = "";
                foreach (int[] item in test)
                {
                    msg+=  (item[1]+1).ToString() + " ";
                }
                Clipboard.SetText(msg);
                int oldPoint = pointofAI;
                //Test lần 1
                idx = 0;
                for (int i = 0; i<numQueen; ++i)
                {
                    for (int j = 0; j<numQueen; ++j)
                    {
                        if (currentState[i, j] == 1)
                        {
                            try
                            {
                                if (i>=(test[idx] as int[])[0])
                                {
                                    GoTo(i, j, (test[idx] as int[])[0], (test[idx] as int[])[1]);
                                    idx++;
                                }
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }
                }
                Task.Delay(500);
                Application.DoEvents();
                if (oldPoint == pointofAI)
                    reCheck(test, 0);
                Task.Delay(500);
                Application.DoEvents();
                if (oldPoint == pointofAI)
                    reCheck(test, 0);
                bool isTestTrue = false;
                if (oldPoint == pointofAI)
                {
                    reFix(test);
                    reFix(test);
                }
                if (oldPoint == pointofAI)
                {
                    if (isTestTrue)
                    {
                        MessageBox.Show("Can't Solve");
                        isTestTrue = false;
                    }
                }
                if (isTestTrue)
                {
                    //MessageBox.Show("Solved");
                }
            }

            isAIplayed = false;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            pointofAI = 0;
            pointplayer = 0;
            pointPlayer.Text = pointAI.Text = "0";
        }
    }
}