using CrevisModbusLibrary;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Crevis_IO_Control
{
    public partial class Form1 : Form
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public static string ConfigurePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\Configure\"));

        private const int iCH_MAX = 32;
        private int iNetworkAdapterCnt;
        private string sNetworkAdapterIPInit;
        private ushort iDI_ReadCnt;
        private ushort iDO_ReadCnt;
        private ModbusManager _crevisManager;
        private Label[] status_Conn;
        private Label[] status_DI1;
        private Label[] status_DI2;
        private Button[] setting_DO1;
        private Button[] setting_DO2;

        public Form1()
        {
            InitializeComponent();

            Crevis_Module_Info_Load();

            // 생성 시 자동 연결 & 모니터링 시작
            _crevisManager = new ModbusManager(iNetworkAdapterCnt, sNetworkAdapterIPInit);

            status_Conn = new Label[2] {
                labelConn1, labelConn2
            };

            status_DI1 = new Label[iCH_MAX] {
                label0, label1, label2, label3, label4, label5, label6, label7,
                label8, label9, label10, label11, label12, label13, label14, label15,
                label16, label17, label18, label19, label20, label21, label22, label23,
                label24, label25, label26, label27, label28, label29, label30, label31 
            };

            status_DI2 = new Label[iCH_MAX] {
                label2_0, label2_1, label2_2, label2_3, label2_4, label2_5, label2_6, label2_7,
                label2_8, label2_9, label2_10, label2_11, label2_12, label2_13, label2_14, label2_15,
                label2_16, label2_17, label2_18, label2_19, label2_20, label2_21, label2_22, label2_23,
                label2_24, label2_25, label2_26, label2_27, label2_28, label2_29, label2_30, label2_31 
            };

            setting_DO1 = new Button[iCH_MAX] {
                button0, button1, button2, button3, button4, button5, button6, button7,
                button8, button9, button10, button11, button12, button13, button14, button15,
                button16, button17, button18, button19, button20, button21, button22, button23,
                button24, button25, button26, button27, button28, button29, button30, button31
            };

            setting_DO2 = new Button[iCH_MAX] {
                button2_0, button2_1, button2_2, button2_3, button2_4, button2_5, button2_6, button2_7,
                button2_8, button2_9, button2_10, button2_11, button2_12, button2_13, button2_14, button2_15,
                button2_16, button2_17, button2_18, button2_19, button2_20, button2_21, button2_22, button2_23,
                button2_24, button2_25, button2_26, button2_27, button2_28, button2_29, button2_30, button2_31
            };

            Digital_Output_Init();

            timer1.Enabled = true;
        }

        private void Crevis_Module_Info_Load()
        {
            try
            {
                // Ini file read
                StringBuilder sbValue = new StringBuilder();
                // Network adapter 갯수
                GetPrivateProfileString("NetworkAdapter_Count", "Count", "", sbValue, sbValue.Capacity, string.Format("{0}{1}", ConfigurePath, "Crevis_IO_Init.ini"));
                int iResult = 0;
                if (int.TryParse(sbValue.ToString().Trim(), out iResult))
                    iNetworkAdapterCnt = iResult;

                // Network adapter IP주소 설정
                GetPrivateProfileString("NetworkAdapter_Init", "IP", "", sbValue, sbValue.Capacity, string.Format("{0}{1}", ConfigurePath, "Crevis_IO_Init.ini"));
                sNetworkAdapterIPInit = sbValue.ToString().Trim();

                // Module - DI read count
                GetPrivateProfileString("InputRead_Count", "Count", "", sbValue, sbValue.Capacity, string.Format("{0}{1}", ConfigurePath, "Crevis_IO_Init.ini"));
                iDI_ReadCnt = Convert.ToUInt16(sbValue.ToString().Trim());

                // Module - DO read count
                GetPrivateProfileString("OutputRead_Count", "Count", "", sbValue, sbValue.Capacity, string.Format("{0}{1}", ConfigurePath, "Crevis_IO_Init.ini"));
                iDO_ReadCnt = Convert.ToUInt16(sbValue.ToString().Trim());


                labelNA1.Text = string.Format("{0} [{1}{2}]", labelNA1.Text, sNetworkAdapterIPInit, labelNA1.Tag);
                labelNA2.Text = string.Format("{0} [{1}{2}]", labelNA2.Text, sNetworkAdapterIPInit, labelNA2.Tag);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Digital_Output_Init()
        {
            try
            {
                // Network Adapter1에 연결된 0번 Module DO 32ch setting read
                bool[] doValues1 = _crevisManager.ReadDO(string.Format("{0}{1}", sNetworkAdapterIPInit, "0"), 0, iDO_ReadCnt);

                // UI 표시
                for (int i = 0; i < iCH_MAX; i++)
                {
                    if (doValues1[i])
                    {
                        setting_DO1[i].Tag = "1";
                        setting_DO1[i].BackColor = Color.Cyan;
                    }                        
                    else
                    {
                        setting_DO1[i].Tag = "0";
                        setting_DO1[i].BackColor = Color.Silver;
                    }                        
                }

                // Network Adapter2에 연결된 0번 Module DO 32ch setting read
                bool[] doValues2 = _crevisManager.ReadDO(string.Format("{0}{1}", sNetworkAdapterIPInit, "1"), 0, iDO_ReadCnt);

                // UI 표시
                for (int i = 0; i < iCH_MAX; i++)
                {
                    if (doValues2[i])
                    {
                        setting_DO2[i].Tag = "1";
                        setting_DO2[i].BackColor = Color.Cyan;
                    }
                    else
                    {
                        setting_DO2[i].Tag = "0";
                        setting_DO2[i].BackColor = Color.Silver;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                // 연결 상태
                for (int i = 0; i <= 1; i++)
                {
                    if (_crevisManager._connectionStates[string.Format("{0}{1}", sNetworkAdapterIPInit, i.ToString())] == ConnectionState.Connected)
                    {
                        if (status_Conn[i].BackColor != Color.Lime)
                            status_Conn[i].BackColor = Color.Lime;
                        else
                            status_Conn[i].BackColor = Color.Silver;
                    }
                    else
                    {
                        if (status_Conn[i].BackColor != Color.Red)
                            status_Conn[i].BackColor = Color.Red;
                    }
                }
                
                // Network Adapter1에 연결된 0번 Module DI 32ch read
                bool[] diValues1 = _crevisManager.ReadDI(string.Format("{0}{1}", sNetworkAdapterIPInit, "0"), 0, iDI_ReadCnt);

                // UI 표시
                for (int i = 0; i < iCH_MAX; i++)
                {
                    if (diValues1[i])
                        status_DI1[i].BackColor = Color.Lime;
                    else
                        status_DI1[i].BackColor = Color.Silver;
                }
                
                // Network Adapter2에 연결된 0번 Module DI 32ch read
                bool[] diValues2 = _crevisManager.ReadDI(string.Format("{0}{1}", sNetworkAdapterIPInit, "1"), 0, iDI_ReadCnt);

                // UI 표시
                for (int i = 0; i < iCH_MAX; i++)
                {
                    if (diValues2[i])
                        status_DI2[i].BackColor = Color.Lime;
                    else
                        status_DI2[i].BackColor = Color.Silver;
                }                
            }
            catch (Exception ex)
            {                
                MessageBox.Show($"{ex.Message}", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                EXIT();
            }
        }

        private void btnWriteDO_Click(object sender, EventArgs e)
        {            
            try
            {
                Button btn = (Button)sender;                
                string sTag = btn.Tag as string;
                int iCh = Convert.ToInt16(btn.Text.ToString());

                // Network Adapter1에 연결된 0번 모듈, 채널 ON/OFF
                if (sTag == "0")
                {
                    _crevisManager.WriteDO(string.Format("{0}{1}", sNetworkAdapterIPInit, "0"), 0, iCh, true);    // ON
                    btn.BackColor = Color.Cyan;
                    btn.Tag = "1";
                }
                else
                {
                    _crevisManager.WriteDO(string.Format("{0}{1}", sNetworkAdapterIPInit, "0"), 0, iCh, false);   // OFF
                    btn.BackColor = Color.Silver;
                    btn.Tag = "0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnWriteDO2_Click(object sender, EventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                string sTag = btn.Tag as string;
                int iCh = Convert.ToInt16(btn.Text.ToString());

                // Network Adapter2에 연결된 0번 모듈, 채널 ON/OFF
                if (sTag == "0")
                {
                    _crevisManager.WriteDO(string.Format("{0}{1}", sNetworkAdapterIPInit, "1"), 0, iCh, true);    // ON
                    btn.BackColor = Color.Cyan;
                    btn.Tag = "1";
                }
                else
                {
                    _crevisManager.WriteDO(string.Format("{0}{1}", sNetworkAdapterIPInit, "1"), 0, iCh, false);   // OFF
                    btn.BackColor = Color.Silver;
                    btn.Tag = "0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            EXIT();
            base.OnFormClosed(e);
        }        

        private void EXIT()
        {
            timer1.Enabled = false;
            _crevisManager.Dispose();
        }        
    }
}
