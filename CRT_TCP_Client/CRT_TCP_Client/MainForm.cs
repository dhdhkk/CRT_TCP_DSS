using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.IO;

// 10월 5일
namespace CRT_TCP_Client
{
    public partial class MainForm : Form
    {
        TcpClient tc;

        public MainForm()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                tc = new TcpClient(tbIp.Text.ToString(), Convert.ToInt32(tbPort.Text));
                if (tc.Connected)
                {
                    LogTextBox.AppendText("접속 - IP :" + tbIp.Text.ToString() + " / Port : " + Convert.ToInt32(tbPort.Text) + "\r\n");
                }
            }
            catch
            {
                MessageBox.Show("접속 오류");
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            tc.Close();
            LogTextBox.AppendText("연결 해제 " + "\r\n");
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            byte[] arrBYTE = new byte[10];

            // 요청 날짜를 맞추기.
            // start time (YY-MM-DD hh:mm)
            arrBYTE[0] = (byte)18;
            arrBYTE[1] = (byte)6;
            arrBYTE[2] = (byte)5;
            arrBYTE[3] = (byte)14;
            arrBYTE[4] = (byte)00;

            // end time (YY-MM-DD hh:mm)
            arrBYTE[5] = (byte)18;
            arrBYTE[6] = (byte)6;
            arrBYTE[7] = (byte)6;
            arrBYTE[8] = (byte)13;
            arrBYTE[9] = (byte)59;

            NetworkStream stream = tc.GetStream();
            stream.Write(arrBYTE, 0, arrBYTE.Length);
            stream.Flush();
           
            int length;
            byte[] outbuf = new byte[1024];
           
            string fileName = @"C:\WHELMON\CMDB\out.txt";

            FileStream fis = new FileStream(fileName, FileMode.Create);

            bool isFirst = true;
            while ((length = stream.Read(outbuf, 0, outbuf.Length)) > 0)
            {
                if(isFirst)
                {
                    fis.Write(outbuf, 6, length - 6);
                    isFirst = false;
                }
                else
                {
                    fis.Write(outbuf, 0, length);
                }
                
            }

            fis.Close();

            int nbytes = stream.Read(outbuf, 0, outbuf.Length);
            string output = Encoding.ASCII.GetString(outbuf, 0, nbytes);

            stream.Close();
            tc.Close();

            LogTextBox.AppendText($"{nbytes} bytes: {output}");
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Test용 초기 세팅값 지정
            tbIp.Text = "192.168.10.199";
            tbPort.Text = "8017";
        }
    }
}
