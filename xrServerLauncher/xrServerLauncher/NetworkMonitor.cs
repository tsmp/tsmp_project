using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace S.E.R.V.E.R___Shadow_Of_Chernobyl_1._0006
{
    public partial class NetworkMonitor : Form
    {
        Settings _update_settings = null;
        public NetworkMonitor()
        {
            InitializeComponent();
            _update_settings = Settings.GetSettings();
            DYNAMIC_SET_COLOR();
            InitGetAddress();
            init_settings();
        }

        const int WM_NCHITTEST = 0x0084;
        const int HTCAPTION = 2;
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_NCHITTEST)
            {
                m.Result = (IntPtr)HTCAPTION;
                return;
            }
            base.WndProc(ref m);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if (NetworkDeleteFilterAtCloseForm.CheckState == CheckState.Checked)
            {
                IPSEC_COMMAND_HANDLER("delete all");
            }
            save_setting();
            Close();
        }

        public void DYNAMIC_SET_COLOR()
        {
            try
            {
                int color = SendMessage.SendMsg;
                var color_rechange = Color.FromArgb(color);
                if (color == 0)
                {
                    GUI.BackColor = Color.FromArgb(192, 192, 255);
                    btnMinimized.BackColor = Color.FromArgb(192, 192, 255);
                    btnExit.BackColor = Color.FromArgb(192, 192, 255);
                }
                else
                {
                    GUI.BackColor = color_rechange;
                    btnMinimized.BackColor = color_rechange;
                    btnExit.BackColor = color_rechange;
                }
            }
            catch (Exception ex)
            {
                GUI.BackColor = Color.FromArgb(192, 192, 255);
                btnMinimized.BackColor = Color.FromArgb(192, 192, 255);
                btnExit.BackColor = Color.FromArgb(192, 192, 255);
                MessageBox.Show("Command init set color error\n" + ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // http://winintro.ru/netsh_technicalreference.en/html/3cf22f34-e520-4d34-9728-3f37ae0e0612.htm
        // ========================================================================   
        int address_counter = 0;                                                     // Подсчет количества адресов
        int skip_address = 0;
        int blocked_address = 0;
        int white_list = 0;                                                          // Список разрешенных адресов                       
   
        IPGlobalProperties GetIP = IPGlobalProperties.GetIPGlobalProperties();
        HashSet<string> IPADDRBUFFER = new HashSet<string>();                        // AddressBuffer
        List<string> IPADDRCOUNTERBUFFER = new List<string>();                       // Список всех текущих соединений
        HashSet<string> IPBLOCKEDLIST = new HashSet<string>();
        // ========================================================================

        private void tray_ico()
        {
            notifyIcon.Visible = true;
        }

        private void Update_Tick(object sender, EventArgs e)
        {
            if (ThreadUpdate.IsBusy == false)
                ThreadUpdate.RunWorkerAsync();
        }

        private void ThreadUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            Update.Stop();
            GET_CONNECTIONS_INFORMATION();
        }

        private void ThreadUpdate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Update.Start();
        }

        //public static Socket sock;
        private void GET_CONNECTIONS_INFORMATION()
        {
            IPADDRBUFFER.Clear();           // При каждом вызове обнулим данные
            IPADDRCOUNTERBUFFER.Clear();
            address_counter = 0;

            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            // Получим список активных TCP соединений и добавим их в буфер
            foreach (TcpConnectionInformation GetTcpAddress in ipProperties.GetActiveTcpConnections())
            {  
                IPAddress address = GetTcpAddress.RemoteEndPoint.Address;       // получим удаленный адрес
               // int port = GetTcpAddress.LocalEndPoint.Port;                    // получим порт с данного адреса       
                   
                if ((address.ToString().Length >= 7 && address.ToString().Length <= 15) && (address.ToString() != "0.0.0.0" && address.ToString() != "127.0.0.1" && address.ToString() != "255.255.255.255")) // FORMAT STR OK
                {
                    IPADDRCOUNTERBUFFER.Add(address.ToString()); // Добавим все текущие соединения 
                    IPADDRBUFFER.Add(address.ToString());        // Добавим все текущие соединения без повторов
                }
            }

            // https://metanit.com/sharp/net/3.3.php


            // В списке адресов пройдемся по каждому адресу и подсчитаем количество данных адресов в общем списке
            foreach (string ip in IPADDRBUFFER)
            {
                address_counter = 0;
                skip_address = 0;
                white_list = 0;
                foreach (string counter in IPADDRCOUNTERBUFFER)
                {
                    if (counter.Contains(ip))
                    {
                        address_counter++;
                    }
                }

                // Находим адрес в буфере, и если он есть, то мы пропускаем его блокировку
                foreach (string scan in IPBLOCKEDLIST)
                {
                    if (scan.Contains(ip))
                    {
                        skip_address = 1;
                        break;
                    }
                }

                foreach (string address_ok in WhiteIP.Lines)
                {
                    if (address_ok.Contains(ip))
                    {
                        white_list++;
                    }
                }

                // Блокировка адреса
                if ((NetworkProtectionBlockSrvcs.CheckState == CheckState.Checked) && (skip_address == 0) && (white_list == 0) && (address_counter > MaxCurrentConnections.Value))
                {                  
                    skip_address = 1;
                    if (NetworkBlockedActivate.CheckState == CheckState.Unchecked)
                    {
                        blocked_address++;
                        IPBLOCKEDLIST.Add(ip);
                        IPSEC_COMMAND_HANDLER("add filter filterlist=DDoSDefense protocol=any srcaddr=" + ip + " dstaddr=Me");
                        IPSecRuleAdd();
                    }
                    else if ((NetworkBlockedActivate.CheckState == CheckState.Checked) && (IPBLOCKEDLIST.Count <= MaxCurrentBlockIP.Value))
                    {
                        blocked_address++;
                        IPBLOCKEDLIST.Add(ip);
                        IPSEC_COMMAND_HANDLER("add filter filterlist=DDoSDefense protocol=any srcaddr=" + ip + " dstaddr=Me");
                        IPSecRuleAdd();
                    }
                }
            }
        }

        // update list
        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (tabControl1.SelectedIndex == 2)
            {
                InitGetAddress();
            }
        }

        // Отобразить все в формате таблицы
        private void UpdateBlockList()
        {
            IPSEC_COMMAND_HANDLER("show filterlist DDoSDefense level=verbose format=table wide=no");
        }

        // Добавим фильтр
        private void IPSecFilterListAdd()
        {
            IPSEC_COMMAND_HANDLER("add filterlist name=DDoSDefense");
        }

        // Добавим действие
        private void IPSecActionAdd()
        {
            IPSEC_COMMAND_HANDLER("add filteraction name=DDoSDefense action=block");
        }

        // Добавим политику   
        private void IPSecPolicyAdd(int interval = 30)
        {
            IPSEC_COMMAND_HANDLER("add policy name=DDoSDefense assign=yes pollinginterval=" + interval); // interval время в минутах до изменения политики
        }

        // Добавим правило
        private void IPSecRuleAdd() 
        {
            IPSEC_COMMAND_HANDLER("add rule name=DDoSDefense policy=DDoSDefense filterlist=DDoSDefense filteraction=DDoSDefense");
        }


        private void btnDelFilter_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            IPSecRemoveFilter();
        }

        private void btnDelFilterInBlockList_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            IPSecRemoveFilter();
            tabControl1.SelectedIndex = 2;
        }

        private void NetworkProtectionBlockSrvcs_CheckedChanged(object sender, EventArgs e)
        {
            if (NetworkProtectionBlockSrvcs.CheckState == CheckState.Checked)
            {
                // проверим и добавим лист фильтров
                string get_filterlist = IPSEC_COMMAND_HANDLER("show filterlist all format=table");
                if (get_filterlist.Contains("[05067]"))
                {
                    IPSecFilterListAdd();
                }

                // проверим и добавим лист правил
                string get_action = IPSEC_COMMAND_HANDLER("show filteraction all format=table");
                if (get_action.Contains("[05068]"))
                {
                    IPSecActionAdd();
                }

                // проверим и добавим лист политики
                string get_policy = IPSEC_COMMAND_HANDLER("show policy all format=table");
                if (get_policy.Contains("[05072]"))
                {
                    IPSecPolicyAdd();
                }
                IPSecRuleAdd();
            }
        }

        private void InitGetAddress()
        {
            try
            {
                IPSecBlockList.Items.Clear();
                string result = IPSEC_COMMAND_HANDLER("show filterlist name=DDoSDefense level=verbose format=table wide=no");  // Выводим наше правило все в формате таблицы       
                result = result.Substring(result.IndexOf("-------\r\n") + "-------\r\n".Length);
                string[] get_addr = result.Replace("\r", "").Split('\n');

                for (int i = 0; i < get_addr.Length - 2; i += 2)
                {
                    string get = get_addr[i];
                    string[] str = get.Split('\t');
                    if (str.Length >= 2 && str.Length <= 15)
                    {
                        var ADDR = IPAddress.Parse(str[1].Trim());
                        IPBLOCKEDLIST.Add(ADDR.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                IPSecBlockList.Items.Add("[SYSTEM] => " + ex.Message).BackColor = Color.Orange;
            }

            foreach (string s in IPBLOCKEDLIST)
            {
                IPSecBlockList.Items.Add(s).BackColor = Color.LightCoral;
            }

            UI_IPSEC_INFO.Text = "Information Address in BlockList: " + IPBLOCKEDLIST.Count;
        }

        private void MonitoringActivate_CheckedChanged(object sender, EventArgs e)
        {
            if (MonitoringActivate.CheckState == CheckState.Checked)
            {
                DialogResult result = MessageBox.Show("Подтвердите Ваше согласие, что приняли данное условие. В случаи отмены - окно будет закрыто.", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    NetworkProtectionBlockSrvcs.Enabled = true;
                    MaxCurrentConnections.Enabled = true;
                    NetworkBlockedActivate.Enabled = true;
                    MaxCurrentBlockIP.Enabled = true;
                    btnBlockAllTraffic.Enabled = true;
                    btnUnblockAllTraffic.Enabled = true;
                    btnDelFilters.Enabled = true;
                    MonitoringActivate.Visible = false;
                }
                if (result == DialogResult.No)
                {
                    Close();
                }
            }
            else
            {
                NetworkProtectionBlockSrvcs.Enabled = false;
                MaxCurrentConnections.Enabled = false;
                NetworkBlockedActivate.Enabled = false;
                MaxCurrentBlockIP.Enabled = false;
                btnBlockAllTraffic.Enabled = false;
                btnUnblockAllTraffic.Enabled = false;
                btnDelFilters.Enabled = false;
                NetworkProtectionBlockSrvcs.Checked = false;
                NetworkBlockedActivate.Checked = false;
            }
        }


        private void IPSecRemoveFilter()
        {
            IPSEC_COMMAND_HANDLER("delete policy name=DDoSDefense");
            blocked_address = 0;
            IPBLOCKEDLIST.Clear();
            IPSecBlockList.Items.Clear();
            HistoryBlocked.Items.Clear();
            UI_VALUE_COUNT.Text = "Статус: Нет блокировок";
        }

        private string IPSEC_COMMAND_HANDLER(string command)
        {
            Process RemoveIPSecFilter = new Process();
            RemoveIPSecFilter.StartInfo.FileName = "netsh";
            RemoveIPSecFilter.StartInfo.Arguments = "ipsec static " + command;                       // CMD
            RemoveIPSecFilter.StartInfo.CreateNoWindow = true;
            RemoveIPSecFilter.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;                     // Выполнить в скрытом режиме
            RemoveIPSecFilter.StartInfo.UseShellExecute = false;
            RemoveIPSecFilter.StartInfo.Verb = "runas";                                              // Выполнять от учетной записи админа
            RemoveIPSecFilter.StartInfo.RedirectStandardOutput = true;
            RemoveIPSecFilter.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(866);          // Задаем кодировку вывода для правильного отображения
            RemoveIPSecFilter.Start();
            string msg = RemoveIPSecFilter.StandardOutput.ReadToEnd();                               // Вывод всей информации
            RemoveIPSecFilter.WaitForExit();
            RemoveIPSecFilter.Close();
            // TestListView.Items.Add(msg).BackColor = Color.Red;
            // SIGNATURES
            // IPsec[05067] - Списки фильтров отсутствуют в хранилище политик
            // IPSec[05049] - Список фильтров "NAME" отсутствует
            // IPSec[05014] - действие фильтра уже существует
            // IPSec[05005] - действие политики уже существует
            // IPSec[05020] - Правило в политике уже существует
            // IPSec[05010] - Список фильтров уже существует
            return msg;
        }

        private void WhiteIP_TextChanged(object sender, EventArgs e)
        {
            int address_count = WhiteIP.Lines.Count();
            ui_Info_list.Text = "Список разрешенных адресов которые не будут блокироваться. Каждый новый адрес должен начинаться с новой строки. Добавлено адресов: " + address_count;
        }


        private void btnBlockAllTraffic_Click(object sender, EventArgs e)
        {
            string result_for_check = IPSEC_COMMAND_HANDLER("show filterlist name=DDoSDefenseBlockedTraffic level=verbose format=table wide=no");
            if (result_for_check.Contains("IP-"))
            {
                MessageBox.Show("ВХОДЯЩИЙ/ИСХОДЯЩИЙ трафик уже заблокирован.\nБлокировка не требуется.", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Вы действильно хотите заблокировать ВХОДЯЩИЙ/ИСХОДЯЩИЙ трафик?", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                IPSEC_COMMAND_HANDLER("add filterlist name=DDoSDefenseBlockedTraffic");
                IPSEC_COMMAND_HANDLER("add filter filterlist=DDoSDefenseBlockedTraffic protocol=any srcaddr=any dstaddr=Me");
                IPSEC_COMMAND_HANDLER("add filteraction name=DDoSDefenseBlockedTraffic action=block");
                IPSEC_COMMAND_HANDLER("add policy name=DDoSDefenseBlockedTraffic assign=yes");
                IPSEC_COMMAND_HANDLER("add rule name=DDoSDefenseBlockedTraffic policy=DDoSDefenseBlockedTraffic filterlist=DDoSDefenseBlockedTraffic filteraction=DDoSDefenseBlockedTraffic");       
            }
        }

        private void btnUnblockAllTraffic_Click(object sender, EventArgs e)
        {
            IPSEC_COMMAND_HANDLER("delete policy name=DDoSDefenseBlockedTraffic");
        }

        private void btnDelFilters_Click(object sender, EventArgs e)
        {
            IPSEC_COMMAND_HANDLER("delete all");
        }


        private void init_settings()
        {
            NetworkDeleteFilterAtCloseForm.Checked = _update_settings.Network_NetworkDeleteFilterAtCloseForm;
            MaxCurrentBlockIP.Text = _update_settings.Network_MaxCurrentBlockIP;
            MaxCurrentConnections.Text = _update_settings.Network_MaxCurrentConnections;
            WhiteIP.Text = _update_settings.Network_WhiteIP;
        }

        private void save_setting()
        {
            _update_settings.Network_NetworkDeleteFilterAtCloseForm = NetworkDeleteFilterAtCloseForm.Checked;
            _update_settings.Network_MaxCurrentBlockIP = MaxCurrentBlockIP.Text;
            _update_settings.Network_MaxCurrentConnections = MaxCurrentConnections.Text;
            _update_settings.Network_WhiteIP = WhiteIP.Text;
            _update_settings.Save();
        }

        private void btnMinimized_Click(object sender, EventArgs e)
        {    
            WindowState = FormWindowState.Minimized;
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Normal;
        }
    }
}