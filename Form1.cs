using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Memory_Policy_Simulator
{
    public partial class Form1 : Form
    {
        Graphics g;
        PictureBox pbPlaceHolder;
        Bitmap bResultImage;

        public Form1()
        {
            InitializeComponent();
            this.pbPlaceHolder = new PictureBox();
            this.bResultImage = new Bitmap(2048, 2048);
            this.pbPlaceHolder.Size = new Size(2048, 2048);
            g = Graphics.FromImage(this.bResultImage);
            pbPlaceHolder.Image = this.bResultImage;
            this.pImage.Controls.Add(this.pbPlaceHolder);
        }

        private void DrawBase(Core core, int windowSize, int dataLength)
        {
            /* parse window */
            var psudoQueue = new Queue<char>();

            g.Clear(Color.Black);

            for ( int i = 0; i < dataLength; i++ ) // length
            {
                int psudoCursor = core.pageHistory[i].loc;
                char data = core.pageHistory[i].data;
                Page.STATUS status = core.pageHistory[i].status;

                switch ( status )
                {
                    case Page.STATUS.PAGEFAULT:
                        psudoQueue.Enqueue(data);
                        break;
                    case Page.STATUS.MIGRATION:
                        psudoQueue.Dequeue();
                        psudoQueue.Enqueue(data);
                        break;
                }

                for ( int j = 0; j <= windowSize; j++) // height - STEP
                {
                    if (j == 0)
                    {
                        DrawGridText(i, j, data);
                    }
                    else
                    {
                        DrawGrid(i, j);
                    }
                }

                DrawGridHighlight(i, psudoCursor, status);
                int depth = 1;

                foreach ( char t in psudoQueue )
                {
                    DrawGridText(i, depth++, t);
                }
            }
        }


        private void DrawBase2(Core core, int windowSize, int dataLength)
        {
            /* parse window */
            var psudoQueue = new List<char>();

            g.Clear(Color.Black);

            for (int i = 0; i < dataLength; i++) // length
            {
                int psudoCursor = core.pageHistory[i].loc;
                char data = core.pageHistory[i].data;
                Page.STATUS status = core.pageHistory[i].status;

                switch (status)
                {
                    case Page.STATUS.PAGEFAULT:
                        psudoQueue.Add(data);
                        break;
                    case Page.STATUS.MIGRATION:
                        psudoQueue.RemoveAt(psudoCursor-1);
                        psudoQueue.Insert(psudoCursor-1,data);
                        break;
                }

                for (int j = 0; j <= windowSize; j++) // height - STEP
                {
                    if (j == 0)
                    {
                        DrawGridText(i, j, data);
                    }
                    else
                    {
                        DrawGrid(i, j);
                    }
                }

                DrawGridHighlight(i, psudoCursor, status);
                int depth = 1;

                foreach (char t in psudoQueue)
                {
                    DrawGridText(i, depth++, t);
                }
            }
        }


        private void DrawBase3(Core core, int windowSize, int dataLength)
        {
            /* parse window */
            var psudoQueue = new List<char>();

            g.Clear(Color.Black);

            for (int i = 0; i < dataLength; i++) // length
            {
                int psudoCursor = core.pageHistory[i].loc;
                char data = core.pageHistory[i].data;
                Page.STATUS status = core.pageHistory[i].status;

                switch (status)
                {
                    case Page.STATUS.PAGEFAULT:
                        psudoQueue.Add(data);
                        break;
                    case Page.STATUS.MIGRATION:
                        psudoQueue.RemoveAt(psudoCursor - 1);
                        psudoQueue.Add(data);
                        break;
                }

                for (int j = 0; j <= windowSize; j++) // height - STEP
                {
                    if (j == 0)
                    {
                        DrawGridText(i, j, data);
                    }
                    else
                    {
                        DrawGrid(i, j);
                    }
                }

                DrawGridHighlight(i, psudoCursor, status);
                int depth = 1;

                foreach (char t in psudoQueue)
                {
                    DrawGridText(i, depth++, t);
                }
            }
        }

        private void DrawGrid(int x, int y)
        {
            int gridSize = 30;
            int gridSpace = 5;
            int gridBaseX = x * gridSize;
            int gridBaseY = y * gridSize;

            g.DrawRectangle(new Pen(Color.White), new Rectangle(
                gridBaseX + (x * gridSpace),
                gridBaseY,
                gridSize,
                gridSize
                ));
        }

        private void DrawGridHighlight(int x, int y, Page.STATUS status)
        {
            int gridSize = 30;
            int gridSpace = 5;
            int gridBaseX = x * gridSize;
            int gridBaseY = y * gridSize;

            SolidBrush highlighter = new SolidBrush(Color.LimeGreen);

            switch (status)
            {
                case Page.STATUS.HIT:
                    break;
                case Page.STATUS.MIGRATION:
                    highlighter.Color = Color.Purple;
                    break;
                case Page.STATUS.PAGEFAULT:
                    highlighter.Color = Color.Red;
                    break;
            }

            g.FillRectangle(highlighter, new Rectangle(
                gridBaseX + (x * gridSpace),
                gridBaseY,
                gridSize,
                gridSize
                ));
        }

        private void DrawGridText(int x, int y, char value)
        {
            int gridSize = 30;
            int gridSpace = 5;
            int gridBaseX = x * gridSize;
            int gridBaseY = y * gridSize;

            g.DrawString(
                value.ToString(), 
                new Font(FontFamily.GenericMonospace, 8), 
                new SolidBrush(Color.White), 
                new PointF(
                    gridBaseX + (x * gridSpace) + gridSize / 3,
                    gridBaseY + gridSize / 4));
        }

        private void btnOperate_Click(object sender, EventArgs e)
        {
            this.tbConsole.Clear();
            
            if (this.tbQueryString.Text != "" || this.tbWindowSize.Text != "")
            {
                string data = this.tbQueryString.Text;
                int windowSize = int.Parse(this.tbWindowSize.Text);

                /* initalize */
                var window = new Core(windowSize);

                if (comboBox1.Text == "MFU")
                {
                    foreach (char element in data)
                    {
                        var status = window.OperateMFU(element);
                        this.tbConsole.Text += "DATA " + element + " is " +
                            ((status == Page.STATUS.PAGEFAULT) ? "Page Fault" : status == Page.STATUS.MIGRATION ? "Migrated" : "Hit")
                            + "\r\n";
                    }
                    DrawBase3(window, windowSize, data.Length);
                    this.pbPlaceHolder.Refresh();
                }

                if (comboBox1.Text == "LFU")
                {
                    foreach (char element in data)
                    {
                        var status = window.OperateLFU(element);
                        this.tbConsole.Text += "DATA " + element + " is " +
                            ((status == Page.STATUS.PAGEFAULT) ? "Page Fault" : status == Page.STATUS.MIGRATION ? "Migrated" : "Hit")
                            + "\r\n";
                    }
                    DrawBase3(window, windowSize, data.Length);
                    this.pbPlaceHolder.Refresh();
                }

                if (comboBox1.Text == "Second Chance")
                {
                    foreach (char element in data)
                    {
                        var status = window.OperateSecondChance(element);
                        this.tbConsole.Text += "DATA " + element + " is " +
                            ((status == Page.STATUS.PAGEFAULT) ? "Page Fault" : status == Page.STATUS.MIGRATION ? "Migrated" : "Hit")
                            + "\r\n";
                    }
                    DrawBase2(window, windowSize, data.Length);
                    this.pbPlaceHolder.Refresh();
                }

                if (comboBox1.Text == "Add-Reference Bits")
                {
                    foreach (char element in data)
                    {
                        var status = window.OperateAdditionalReferenceBits(element);
                        this.tbConsole.Text += "DATA " + element + " is " +
                            ((status == Page.STATUS.PAGEFAULT) ? "Page Fault" : status == Page.STATUS.MIGRATION ? "Migrated" : "Hit")
                            + "\r\n";
                    }
                    DrawBase3(window, windowSize, data.Length);
                    this.pbPlaceHolder.Refresh();
                }


                if (comboBox1.Text == "LRU")
                {
                    foreach (char element in data)
                    {
                        var status = window.OperateLRU(element);
                        this.tbConsole.Text += "DATA " + element + " is " +
                            ((status == Page.STATUS.PAGEFAULT) ? "Page Fault" : status == Page.STATUS.MIGRATION ? "Migrated" : "Hit")
                            + "\r\n";
                    }
                    DrawBase2(window, windowSize, data.Length);
                    this.pbPlaceHolder.Refresh();
                }


                if (comboBox1.Text == "Optimal")
                {
                    int k = 0;
                    foreach (char element in data)
                    {
                        var status = window.OperateOptimal(element, data, k);
                        this.tbConsole.Text += "DATA " + element + " is " +
                            ((status == Page.STATUS.PAGEFAULT) ? "Page Fault" : status == Page.STATUS.MIGRATION ? "Migrated" : "Hit")
                            + "\r\n";
                        k++;
                    }
                    DrawBase2(window, windowSize, data.Length);
                    this.pbPlaceHolder.Refresh();
                }
                if (comboBox1.Text == "FIFO")
                {
                    int k = 0;
                    foreach (char element in data)
                    {
                        var status = window.OperateFIFO(element);
                        this.tbConsole.Text += "DATA " + element + " is " +
                            ((status == Page.STATUS.PAGEFAULT) ? "Page Fault" : status == Page.STATUS.MIGRATION ? "Migrated" : "Hit")
                            + "\r\n";
                        k++;
                    }
                    DrawBase(window, windowSize, data.Length);
                    this.pbPlaceHolder.Refresh();
                }

                label5.Text = "Hit " + window.hit;
                label6.Text = "Page Fault: " + (window.fault);

                /* 차트 생성 */
                chart1.Series.Clear();
                Series resultChartContent = chart1.Series.Add("Statics");
                resultChartContent.ChartType = SeriesChartType.Pie;
                resultChartContent.IsVisibleInLegend = true;
                resultChartContent.Points.AddXY("Hit", window.hit);
                resultChartContent.Points.AddXY("Page Fault", window.fault-window.migration);
                resultChartContent.Points.AddXY("Migrated", window.migration);
                resultChartContent.Points[0].IsValueShownAsLabel = true;
                resultChartContent.Points[1].IsValueShownAsLabel = true;
                resultChartContent.Points[2].IsValueShownAsLabel = true;

                this.lbPageFaultRatio.Text = Math.Round(((float)window.fault / (window.fault + window.hit)), 2) * 100 + "%";
            }
            else
            {
            }

        }

        private void pbPlaceHolder_Paint(object sender, PaintEventArgs e)
        {
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void tbWindowSize_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void tbWindowSize_KeyPress(object sender, KeyPressEventArgs e)
        {
                if (!(Char.IsDigit(e.KeyChar)) && e.KeyChar != 8)
                {
                    e.Handled = true;
                }
        }

        private void btnRand_Click(object sender, EventArgs e)
        {
            Random rd = new Random();

            int count = rd.Next(5, 50);
            StringBuilder sb = new StringBuilder();


            for ( int i = 0; i < count; i++ )
            {
                sb.Append((char)rd.Next(65, 90));
            }

            this.tbQueryString.Text = sb.ToString();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            bResultImage.Save("./result.jpg");
        }
    }
}
