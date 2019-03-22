using NetFwTypeLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace S.E.R.V.E.R___Shadow_Of_Chernobyl_1._0006
{
    public partial class ServerBasePlayers : Form
    {
        Settings _base_settings = null;
        public ServerBasePlayers()
        {
            _base_settings = Settings.GetSettings();
            InitializeComponent();
            Initialize_save_form();
            DYNAMIC_SET_COLOR();
            BaseLoadInBuffer();

            ListProxyLoad(); // test
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0084)
            {
                m.Result = (IntPtr)2;
                return;
            }
            base.WndProc(ref m);
        }

        public HashSet<string> PlayersBaseBuffer = new HashSet<string>();
        private void BaseLoadInBuffer()
        {
            try
            {
                PlayersBaseBuffer.Clear();
                foreach (string UpdateData in File.ReadLines(@"PlayersDataBase\Players.xrBase", Encoding.GetEncoding("UTF-8")))
                {
                    PlayersBaseBuffer.Add(UpdateData + Environment.NewLine);
                }
                str_base_value.Text = "Данных в базе: " + PlayersBaseBuffer.Count;
                handler_writer_base = PlayersBaseBuffer.Count;
            }
            catch (Exception)
            {
                ERROR_COUNTER++;
            }
        }

        private void Save(string AddBase)
        {
            try
            {
                StreamWriter writer = new StreamWriter(@"PlayersDataBase\Players.xrBase", false, Encoding.GetEncoding("UTF-8"));
                writer.Write(AddBase);
                writer.Close();
                str_base_value.Text = "Данных в базе: " + PlayersBaseBuffer.Count;
            }
            catch (Exception)
            {
                ERROR_COUNTER++;
            }
        }

        public void LogProc(string AppendLine)
        {
            StreamWriter writer = new StreamWriter(@"Log.txt", true, Encoding.GetEncoding("UTF-8"));
            writer.Write(AppendLine + Environment.NewLine);
            writer.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if (SrvEventsBuffer.Count != 0)
            {
                DialogResult dialogResult = MessageBox.Show("Вы действительно хотите завершить работу базы? В данный момент в ней находятся игровые события которые будут удалены.\nВыйти из базы?", "Завершение работы базы", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (dialogResult == DialogResult.Yes)
                {
                    save_settings();
                    close_form(ActiveForm);
                }
            }
            else
            {
                save_settings();
                close_form(ActiveForm);
            }
        }

        private void close_form(Form memory)
        {
            PlayersBaseBuffer.Clear();
            BaseEvents.Clear();
            memory.Dispose();
            GC.Collect(4, GCCollectionMode.Forced);
            GC.GetTotalMemory(true);
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            StartAutoCheckThread.Checked = false;
        }

        private void get_auto_scan_Click(object sender, EventArgs e)
        {
            if (StartAutoCheckThread.CheckState == CheckState.Unchecked)
            {
                StartAutoCheckThread.Checked = true;
                get_auto_scan.Text = "Выйти из автоматического режима";
            }
            else
            {
                StartAutoCheckThread.Checked = false;
                get_auto_scan.Text = "Автоматический режим";
            }
        }

        private void WindowFormat_Click(object sender, EventArgs e)
        {
            if (WindowFormat.CheckState == CheckState.Checked)
                TopMost = true;
            else
                TopMost = false;
        }

        private void btnMinimized_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void ui_search_exit_Click(object sender, EventArgs e)
        {
            SearchPanel.Visible = false;
        }

        private void GUI_MINI_MENU_Click(object sender, EventArgs e)
        {
            StartAutoCheckThread.Checked = false;
        }

        private void index_search_Click(object sender, EventArgs e)
        {
            int sscan = 0;
            if (!(index_text_search.TextLength > 0))
            {
                MessageBox.Show("Поиск невозможен т.к отсутствует информация для запроса поиска.", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            listBase.Items.Clear();
            try
            {
                foreach (string s in PlayersBaseBuffer)
                {
                    int addr_skip = 0;
                    addr_skip = s.Count(simbolcount => simbolcount == '%');
                    if (addr_skip == 4 || addr_skip == 5)
                    {
                        var id_filter = s.Split('%')[0];
                        var id_name = s.Split('%')[1];
                        var id_hash = s.Split('%')[3];
                        var id_address = s.Split('%')[4];

                        //  По всем стоблцам
                        if (index_select_reason.SelectedIndex == 0)
                        {
                            if (s.Contains(index_text_search.Text))
                            {
                                sscan++;
                                string[] values = s.Split('%');
                                listBase.Items.Add(new ListViewItem(values));
                            }
                        }
                        //  По имени
                        else if (index_select_reason.SelectedIndex == 1)
                        {
                            if (s.Contains(id_name))
                            {
                                var get_search_name = s.Split('%')[1];
                                if (get_search_name.Contains(index_text_search.Text))
                                {
                                    sscan++;
                                    string[] values = s.Split('%');
                                    listBase.Items.Add(new ListViewItem(values));
                                }
                            }
                        }

                        //  По адресу
                        else if (index_select_reason.SelectedIndex == 2)
                        {
                            if (s.Contains(id_address))
                            {
                                var get_search_address = s.Split('%')[4];
                                if (get_search_address.Contains(index_text_search.Text))
                                {
                                    sscan++;
                                    string[] values = s.Split('%');
                                    listBase.Items.Add(new ListViewItem(values));
                                }
                            }
                        }

                        // По хешу
                        else if (index_select_reason.SelectedIndex == 3)
                        {
                            if (s.Contains(id_hash))
                            {
                                var get_search_hash = s.Split('%')[3];
                                if (get_search_hash.Contains(index_text_search.Text))
                                {
                                    sscan++;
                                    string[] values = s.Split('%');
                                    listBase.Items.Add(new ListViewItem(values));
                                }
                            }
                        }

                        // По фильтру нарушителей
                        else if (index_select_reason.SelectedIndex == 4)
                        {
                            if (id_filter == "[V+]")
                            {
                                if (s.Contains(id_filter))
                                {
                                    if (s.Contains(index_text_search.Text))
                                    {
                                        sscan++;
                                        string[] values = s.Split('%');
                                        listBase.Items.Add(new ListViewItem(values));
                                    }
                                }
                            }
                        }
                    }
                }
                if (sscan != 0)
                {
                    SearchPanel.Visible = false;
                    MessageBox.Show("По Вашему запросу найдено: " + sscan + " записей.", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    DialogResult result = MessageBox.Show("Поиск не дал результатов.\nНажмите \"Да\" - чтобы продолжить поиск\nНажмите \"Отмена\" - чтобы закрыть окно поиска", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    if (result == DialogResult.Cancel)
                        SearchPanel.Visible = false;                 
                }
            }
            catch (Exception)
            {
                ERROR_COUNTER++;
            }
        }

        private void BaseMenu1GetChatSearch_Click(object sender, EventArgs e)
        {
            if (ChatSearchPanel.Visible == false)
                ChatSearchPanel.Visible = true;
            else
                ChatSearchPanel.Visible = false;
        }

        private void ChatSearchExit_Click(object sender, EventArgs e)
        {
            ChatSearchPanel.Visible = false;
        }

        private void btnLoadGreenChat_Click(object sender, EventArgs e)
        {
            GetChatFilter(2, false);
        }

        private void btnLoadBlueChat_Click(object sender, EventArgs e)
        {
            GetChatFilter(4, false);
        }

        private void btnLoadAdminChat_Click(object sender, EventArgs e)
        {
            GetChatFilter(8, false);
        }

        private void btnLoadAllChat_Click(object sender, EventArgs e)
        {
            GetChatFilter(0, true);
        }

        private void btnStartChatSearch_Click(object sender, EventArgs e)
        {
            listChatHistory.Items.Clear();
            foreach (string Messages in SrvEventsBuffer)
            {
                if (Messages.Contains("Чат:"))
                {

                    var color_msg = Messages.Split()[0];    // отобразим цвет сообщения
                    var check_msg = Messages.Split()[1];

                    if (color_msg + " " + check_msg == "Чат: ServerAdmin")
                    {
                        var admin = Messages.Replace("Чат: ServerAdmin", "[Администратор]: ");
                        if (CheckResult.SelectedIndex == 0)
                        {
                            if (admin.Contains(ChatSearchText.Text))
                            {
                                listChatHistory.Items.Add(admin).BackColor = Color.Gold;
                            }
                        }
                        else
                        {
                            if (admin.Contains(ChatSearchText.Text))
                            {
                                listChatHistory.Items.Add(admin).BackColor = Color.Violet;
                            }
                            else
                            {
                                listChatHistory.Items.Add(admin).BackColor = Color.Gold;
                            }
                        }
                    }
                    else if (color_msg + " " + check_msg == "- Чат:")
                    {
                        var green = Messages.Replace("- Чат:", "[Свобода]: ");
                        if (CheckResult.SelectedIndex == 0)
                        {
                            if (green.Contains(ChatSearchText.Text))
                            {
                                listChatHistory.Items.Add(green).BackColor = Color.PaleGreen;
                            }
                        }
                        else
                        {
                            if (green.Contains(ChatSearchText.Text))
                            {
                                listChatHistory.Items.Add(green).BackColor = Color.Violet;
                            }
                            else
                            {
                                listChatHistory.Items.Add(green).BackColor = Color.PaleGreen;
                            }
                        }
                    }
                    else if (color_msg + " " + check_msg == "~ Чат:")
                    {
                        var blue = Messages.Replace("~ Чат:", "[Наемники]: ");
                        if (CheckResult.SelectedIndex == 0)
                        {
                            if (blue.Contains(ChatSearchText.Text))
                            {
                                listChatHistory.Items.Add(blue).BackColor = Color.LightBlue;
                            }
                        }
                        else
                        {
                            if (blue.Contains(ChatSearchText.Text))
                            {
                                listChatHistory.Items.Add(blue).BackColor = Color.Violet;
                            }
                            else
                            {
                                listChatHistory.Items.Add(blue).BackColor = Color.LightBlue;
                            }
                        }
                    }
                    else if ((color_msg + " " + check_msg.Length == "Чат: " + check_msg.Length) && (check_msg != "ServerAdmin") && (check_msg != "SERVER"))
                    {
                        var all_msg = Messages.Replace("Чат: ", "[Общий чат]: ");
                        if (CheckResult.SelectedIndex == 0)
                        {
                            if (all_msg.Contains(ChatSearchText.Text))
                            {
                                listChatHistory.Items.Add(all_msg).BackColor = Color.Honeydew;
                            }
                        }
                        else
                        {
                            if (all_msg.Contains(ChatSearchText.Text))
                            {
                                listChatHistory.Items.Add(all_msg).BackColor = Color.Violet;
                            }
                            else
                            {
                                listChatHistory.Items.Add(all_msg).BackColor = Color.Honeydew;
                            }
                        }
                    }
                }
            }
        }

        private void GetChatFilter(int FILTER_MSG, bool AllMsg)
        {
            try
            {
                listChatHistory.Items.Clear();
                foreach (string Messages in SrvEventsBuffer)
                {
                    if (Messages.Contains("Чат:"))
                    {
                        var color_msg = Messages.Split()[0];
                        var check_msg = Messages.Split()[1];

                        if ((color_msg + " " + check_msg == "- Чат:") && (FILTER_MSG == 2 || AllMsg == true))
                        {
                            var green = Messages.Replace("- Чат:", "[Свобода]: ");
                            listChatHistory.Items.Add(green).BackColor = Color.PaleGreen;
                        }
                        if ((color_msg + " " + check_msg == "~ Чат:") && (FILTER_MSG == 4 || AllMsg == true))
                        {
                            var blue = Messages.Replace("~ Чат:", "[Наемники]: ");
                            listChatHistory.Items.Add(blue).BackColor = Color.LightBlue;
                        }
                        if ((color_msg + " " + check_msg == "Чат: ServerAdmin") && (FILTER_MSG == 8 || AllMsg == true))
                        {
                            var admin = Messages.Replace("Чат: ServerAdmin", "[Администратор]: ");
                            listChatHistory.Items.Add(admin).BackColor = Color.Gold;
                        }
                        if ((color_msg + " " + check_msg.Length == "Чат: " + check_msg.Length) && (AllMsg == true) && (check_msg != "ServerAdmin") && (check_msg != "SERVER"))
                        {
                            var all_msg = Messages.Replace("Чат: ", "[Общий чат]: ");
                            listChatHistory.Items.Add(all_msg).BackColor = Color.Honeydew;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                listChatHistory.Items.Add("[ERROR] CHAT FILTER => " + ex.Message).BackColor = Color.Coral;
            }
        }


        private void StartAutoCheckThread_CheckedChanged(object sender, EventArgs e)
        {
            if (StartAutoCheckThread.CheckState == CheckState.Checked)
            {
                FinishReadIndex = 0;             
                START_LEVEL_THREAD = 0;
                Width = 151;
                Height = 133;
                tabControl1.Visible = false;
                GUI_MINI_MENU.Visible = true;
                ScanDataFiles.Start();
                try
                {
                    DateTime DataModification = File.GetLastWriteTime(@"server_settings\logs\xray_" + SystemInformation.UserName + ".log");
                    xrRegFileTime = (DataModification.ToString("dd.MM.yy HH.mm.ss")); // hh - 12 hours format // HH - 24 hours format
                }
                catch (Exception)
                {
                    ERROR_COUNTER++;
                }
                get_auto_scan.Text = "Выйти из автоматического режима";
            }
            else
            {
                Width = 780;
                Height = 487;
                ScanDataFiles.Stop();
                tabControl1.Visible = true;
                GUIEvents.Visible = true;
                GUI_MINI_MENU.Visible = false;
                get_auto_scan.Text = "Автоматический режим";
            }
        }


        int name_skip_space = 0;
        int server_version = 0;
        int server_chat_warning = 0;
        int server_attack_blocked = 0;
        int ERROR_COUNTER = 0;                                                 // counter read/write error proc 
        int SIGNAL_BANNED = 0;                                                 // counter ban players
        int ADDRESS_COUNTER_AUTO = 0;                                          // Количество адресов в списке
        int EventsCount = 0;                                                   // Число завышенных данных пользователем, требуется для автоматической блокировки
        int EventsWeaponsCount = 0;
        int handler_writer_base = 0;                                           // Блокирует запись данных в базу, когда это не нужно
        int START_LEVEL_THREAD = 0;                                            // 0 - AUTO START // 2 - USER START // 4 - USER START & TRANSFER DATA TO BASE
        long finish_log_size = 0;
        int get_show_informer_at_threading_complited = 0;                      // выводим подсказку о успешном переносе в базу
        int statistics_new_players = 0;                                        // Счетчик новых игроков за время работы программы
        int statistics_new_blocked = 0;                                        // Счетчик новых блокировок за время работы программы                   

        int EventsControllerMaxHPLimit = 20;                                   // Максимальное число пуль для параметра Hit Power

        int StartReadIndex = 0;
        int FinishReadIndex = 0;

        HashSet<string> SrvEventsBuffer = new HashSet<string>();               // Events history
        HashSet<string> SrvPlayersBuffer = new HashSet<string>();              // HIM Players Автоматический режим           
        HashSet<string> CHEATERPLAYERSLIST = new HashSet<string>();            // Лист нарушителей которые были обнаружены с завышенными данными
        HashSet<string> ListplayersEvents = new HashSet<string>();             // для просмотра списка игроков.

        public static HashSet<string> BaseEvents = new HashSet<string>();      // События которые относятся к временным событиям

        HashSet<string> CHECK_PLAYER_INPROXYLIST = new HashSet<string>();      // Список всех адресов с сервера
        HashSet<string> CHECK_ADDR = new HashSet<string>();
        private void ListProxyLoad()
        {
            try
            {
                CHECK_ADDR.Clear();
                foreach (string UpdateData in File.ReadLines(@"PlayersDataBase\ProxyAddrBase.txt", Encoding.GetEncoding("UTF-8")))
                {
                    if (UpdateData.Contains("NO%PROXY"))
                        CHECK_ADDR.Add(UpdateData);
                    if (UpdateData.Contains("PROXY%SERVER"))
                        CHECK_ADDR.Add(UpdateData);
                }
            }
            catch (Exception)
            {
                ERROR_COUNTER++;
            }
        }


        private void ThreadListplayers_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                name_skip_space = 0;
                server_chat_warning = 0;
                server_attack_blocked = 0;
                ListplayersEvents.Clear();
                try
                {
                    long Size = new FileInfo(@"server_settings\logs\xray_" + SystemInformation.UserName + ".log").Length / 1024;
                    if (Size < finish_log_size)
                    {
                        CHEATERPLAYERSLIST.Clear();
                        ListCheaterEvents.Items.Clear();
                        PlayersCheaterList.Items.Clear();
                        SIGNAL_BANNED = 0;
                        FinishReadIndex = 0;
                    }
                    finish_log_size = Size;
                }
                catch (Exception)
                {
                    ERROR_COUNTER++;
                }

                foreach (string s in File.ReadLines(@"server_settings\logs\xray_" + SystemInformation.UserName + ".log", Encoding.GetEncoding(1251)))
                {
                    if (s.Contains("] ping["))       // 0 - standart server
                    {
                        server_version = 0;
                        break;
                    }
                    if (s.Contains("session_id:")) // 1 - stalker controller
                    {
                        server_version = 1;
                        break;
                    }
                }

                if (!(START_LEVEL_THREAD == 0 || START_LEVEL_THREAD == 4))
                {
                    FinishReadIndex = 0;
                }             
                else if ((StartReadIndex > 0) && (START_LEVEL_THREAD == 0))
                {
                    SrvPlayersBuffer.Clear();
                    CHEATERPLAYERSLIST.Clear();
                }           
                StartReadIndex = 0;
                foreach (string s in File.ReadLines(@"server_settings\logs\xray_" + SystemInformation.UserName + ".log", Encoding.GetEncoding(1251)))
                {
                   StartReadIndex++;
                   if (FinishReadIndex <= StartReadIndex)
                    {
                        if (server_version == 1)
                        {
                            if (s.Contains("session_id: "))
                            {
                                string WriteToBase = s.Remove(0, s.IndexOf("name:"));
                                WriteToBase = WriteToBase.Substring(0, WriteToBase.LastIndexOf("ping: "));
                                WriteToBase = WriteToBase.Replace(",", "").Replace("(", "").Replace(")", "");

                                name_skip_space = WriteToBase.Count(simbolcount => simbolcount == ' '); // Число пробелов в строке должно быть всегда 8
                                var get_name = WriteToBase.Split()[1];          // Проверяем на наличие ника, если он есть, то добавляем данные, если же строка пустая, то не добавляем ничего.
                                if (get_name.Length > 0 && name_skip_space == 8)
                                {
                                    int a = WriteToBase.IndexOf("session_id:"); // index start
                                    int b = WriteToBase.IndexOf("hash:");       // index end
                                    WriteToBase = WriteToBase.Remove(a, b - a); // remove index   Удаляем все, что находится между session_id и hash + включая эти слова
                                    var pName = WriteToBase.Split()[1];
                                    var pHash = WriteToBase.Split()[3];
                                    var pAddr = WriteToBase.Split()[5];
                                    if (START_LEVEL_THREAD == 0 || START_LEVEL_THREAD == 4)
                                        PlayersBaseBuffer.Add("[A]%" + pName + "%%" + pHash + "%" + pAddr + "%" + Environment.NewLine);
                                    else
                                        ListplayersEvents.Add("[A]%" + pName + "%%" + pHash + "%" + pAddr + "%" + Environment.NewLine);
                                    if (EventCheckProxy.CheckState == CheckState.Checked)
                                        CHECK_PLAYER_INPROXYLIST.Add(pAddr + "%" + pName);
                                }
                                else if (name_skip_space > 8)
                                {
                                    int str_name = WriteToBase.IndexOf("name:"); // Извлекаем имя игрока
                                    int str_name_finished = WriteToBase.IndexOf("session_id:");
                                    var check_name_out = WriteToBase.Substring(str_name, str_name_finished - str_name).Replace("name: ", "").TrimEnd(' ').Replace(" ", "_");

                                    int a = WriteToBase.IndexOf("session_id:"); // index start
                                    int b = WriteToBase.IndexOf("hash:");       // index end
                                    WriteToBase = WriteToBase.Remove(a, b - a); // remove index   Удаляем все, что находится между session_id и hash + включая эти слова

                                    if (!(START_LEVEL_THREAD == 0 || START_LEVEL_THREAD == 4))
                                        ListplayersEvents.Add("[A]%" + check_name_out + "%%[ERROR STRING FORMAT]%" + name_skip_space + "%" + Environment.NewLine);
                                }

                                // test  
                                /*      
                                else if ((get_name.Length == 0) && (START_LEVEL_THREAD == 0))
                                {
                                    fakeAddr = WriteToBase.Split()[7];
                                    if (fakeAddr == fakeAddrCmp)  // null str
                                    {
                                        protect++;
                                        if (protect > 5) // attack
                                        {
                                            foreach (string SKIP_BLOCKED in AddressBuffer)
                                            {
                                                if (SKIP_BLOCKED.Contains(fakeAddr))
                                                {
                                                    protect = 0;
                                                    break;
                                                }
                                            }
                                            if (protect > 0)
                                            {
                                                WriteToCheaterList("FAKE>" + protect, fakeAddr, "[null]", "[null]", 16);
                                                FirewallWriteNewRule(8, "FAKE", " [FAKEPLAYER] IP: ", fakeAddr, "@");
                                                protect = 0;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        fakeAddrCmp = fakeAddr;
                                        protect = 0;
                                    }
                                }*/
                            }
                        }
                        else
                        {
                            if (s.Contains("] ping["))
                            {
                                string WriteToBaseST = s.Remove(0, s.IndexOf(": ") + 1);
                                WriteToBaseST = WriteToBaseST.Substring(0, WriteToBaseST.LastIndexOf(" port["));
                                name_skip_space = WriteToBaseST.Count(simbolcount => simbolcount == ' ');
                                var get_name = WriteToBaseST.Split()[1];

                                if (name_skip_space == 3 && get_name.Length > 0)
                                {
                                    var p2name = WriteToBaseST.Split()[1];
                                    var p2addr = WriteToBaseST.Split()[3];
                                    if (START_LEVEL_THREAD == 0 || START_LEVEL_THREAD == 4)
                                        PlayersBaseBuffer.Add("[A]%" + p2name + "%%%" + p2addr + "%%" + Environment.NewLine);
                                    else
                                        ListplayersEvents.Add("[A]%" + p2name + "%%%" + p2addr + "%%" + Environment.NewLine);
                                }
                            }
                        }

                        if (s.Contains("logged as") || s.Contains("Access denied") || s.Contains("! too large packet size") || s.Contains("Disconnecting and Banning: ") || s.Contains("~ Чат:") || s.Contains("- Чат:") || s.Contains("Чат:"))
                        {
                            SrvEventsBuffer.Add(s);
                        }

                        else if (s.Contains("от Заряд ВОГ-25") || s.Contains("M209"))
                        {
                            var cmp_line = s.Split()[0];
                            if ((server_version == 1) && (cmp_line == "*"))
                                SrvEventsBuffer.Add(s);

                            else if ((server_version == 1) && (cmp_line != "*"))
                                SrvEventsBuffer.Add("* " + s);

                            else if (server_version == 0)
                                SrvEventsBuffer.Add("* " + s);
                        }

                        else if (s.Contains("! blocked") || s.Contains("! ip attack"))
                        {
                            var a = s.Replace("! blocked", "stalkazz_attack%%%").Replace("! ip attack", "stalkazz_attack%%%").Replace(" ", string.Empty);
                            PlayersBaseBuffer.Add("[IP]%" + a + "%" + Environment.NewLine);
                            SrvEventsBuffer.Add("[IP]%" + a + "%");
                        }

                        if (START_LEVEL_THREAD == 0 || START_LEVEL_THREAD == 2)
                        {
                            if (EventsWeaponsBlockedPlayers.CheckState == CheckState.Checked)
                            {
                                if (s.Contains("от Заряд ВОГ-25") || s.Contains("M209"))
                                {
                                    var cmp_line = s.Split()[0];
                                    if ((server_version == 1) && (cmp_line == "*"))
                                        SrvPlayersBuffer.Add("@ Weapons Flags -> " + s);

                                    else if ((server_version == 1) && (cmp_line != "*"))
                                        SrvPlayersBuffer.Add("@ Weapons Flags -> * " + s);

                                    else if (server_version == 0)
                                        SrvPlayersBuffer.Add("@ Weapons Flags -> * " + s);
                                }

                                if (server_version == 0)
                                {
                                    if (s.Contains("] ping["))
                                    {
                                        string WriteToBaseST = s.Remove(0, s.IndexOf(": ") + 1);
                                        WriteToBaseST = WriteToBaseST.Substring(0, WriteToBaseST.LastIndexOf(" port["));
                                        name_skip_space = WriteToBaseST.Count(simbolcount => simbolcount == ' ');
                                        var get_name = WriteToBaseST.Split()[1];
                                        if (name_skip_space == 3 && get_name.Length > 0)
                                        {
                                            var p2name = WriteToBaseST.Split()[1];
                                            var p2addr = WriteToBaseST.Split()[3];
                                            SrvPlayersBuffer.Add("name: " + p2name + " hash: unknown ip: " + p2addr);
                                        }
                                    }
                                }
                            }

                            if (s.Contains("! ") || (s.Contains("- Banning by id -")))
                            {
                                var str = s.Split()[0];
                                if (str == "!")
                                {
                                    SrvPlayersBuffer.Add(s);
                                }
                                else if (str == "-")
                                {
                                    SrvPlayersBuffer.Add(s);
                                    int count_max_space_line = s.Count(line => line == ' ');
                                    if ((count_max_space_line == 11) && (EventsControllerBlockedPlayers.CheckState == CheckState.Checked))
                                    {
                                        var NAME = s.Split('-')[2];
                                        var IPs = s.Split()[11];
                                        BlockedClient(2, NAME.Replace(", id ", "").Replace(" ", ""), " [PATCH] IP: ", IPs, "@");
                                    }
                                }
                            }
                        }

                        if ((EventsControllerAutoCheckPlayers.CheckState == CheckState.Checked) && (server_version == 1))
                        {
                            try
                            {
                                if (s.Contains("- | BT"))
                                {
                                    string print = s.Replace("# srv: ", "");
                                    var STR_NO_CHAT_MSG = print.Split()[1];
                                    if (STR_NO_CHAT_MSG != "Чат:")
                                    {
                                        var str_find_impulse = print.Split()[13];
                                        var str_find_hp = print.Split()[6];
                                        var str_find_weapons = print.Split()[2];

                                        long HITPOWER = long.Parse(str_find_hp.Substring(0, str_find_hp.LastIndexOf("+")));
                                        long IMPULSE = long.Parse(str_find_impulse.Substring(0, str_find_impulse.LastIndexOf(".")));

                                        if ((mp_wpn_knife.CheckState == CheckState.Unchecked) && (IMPULSE > 120 || str_find_hp.Length >= 8 || HITPOWER > EventsControllerMaxHPLimit) && (str_find_weapons == "[mp_wpn_knife]") && (str_find_weapons != "[mp_actor]"))
                                        {
                                            SrvPlayersBuffer.Add(print);
                                        }
                                        else if ((mp_wpn_pm.CheckState == CheckState.Unchecked) && (IMPULSE > 91 || str_find_hp.Length >= 8 || HITPOWER > EventsControllerMaxHPLimit) && (str_find_weapons == "[mp_wpn_pm]") && (str_find_weapons != "[mp_actor]"))
                                        {
                                            SrvPlayersBuffer.Add(print);
                                        }
                                        else if ((mp_wpn_fort.CheckState == CheckState.Unchecked) && (IMPULSE > 100 || str_find_hp.Length >= 8 || HITPOWER > EventsControllerMaxHPLimit) && (str_find_weapons == "[mp_wpn_fort]") && (str_find_weapons != "[mp_actor]"))
                                        {
                                            SrvPlayersBuffer.Add(print);
                                        }
                                        else if ((mp_wpn_walther.CheckState == CheckState.Unchecked) && (IMPULSE > 131 || str_find_hp.Length >= 8 || HITPOWER > EventsControllerMaxHPLimit) && (str_find_weapons == "[mp_wpn_walther]") && (str_find_weapons != "[mp_actor]"))
                                        {
                                            SrvPlayersBuffer.Add(print);
                                        }
                                        else if ((mp_wpn_sig220.CheckState == CheckState.Unchecked) && (IMPULSE > 126 || str_find_hp.Length >= 8 || HITPOWER > EventsControllerMaxHPLimit) && (str_find_weapons == "[mp_wpn_sig220]") && (str_find_weapons != "[mp_actor]"))
                                        {
                                            SrvPlayersBuffer.Add(print);
                                        }
                                        else if ((mp_wpn_usp.CheckState == CheckState.Unchecked) && (IMPULSE > 126 || str_find_hp.Length >= 8 || HITPOWER > EventsControllerMaxHPLimit) && (str_find_weapons == "[mp_wpn_usp]") && (str_find_weapons != "[mp_actor]"))
                                        {
                                            SrvPlayersBuffer.Add(print);
                                        }
                                        else if ((mp_wpn_desert_eagle.CheckState == CheckState.Unchecked) && (IMPULSE > 144 || str_find_hp.Length >= 8 || HITPOWER > EventsControllerMaxHPLimit) && (str_find_weapons == "[mp_wpn_desert_eagle]") && (str_find_weapons != "[mp_actor]"))
                                        {
                                            SrvPlayersBuffer.Add(print);
                                        }
                                        else if ((mp_wpn_pb.CheckState == CheckState.Unchecked) && (IMPULSE > 72 || str_find_hp.Length >= 8 || HITPOWER > EventsControllerMaxHPLimit) && (str_find_weapons == "[mp_wpn_pb]") && (str_find_weapons != "[mp_actor]"))
                                        {
                                            SrvPlayersBuffer.Add(print);
                                        }
                                        else if ((mp_wpn_colt1911.CheckState == CheckState.Unchecked) && (IMPULSE > 126 || str_find_hp.Length >= 8 || HITPOWER > EventsControllerMaxHPLimit) && (str_find_weapons == "[mp_wpn_colt1911]") && (str_find_weapons != "[mp_actor]"))
                                        {
                                            SrvPlayersBuffer.Add(print);
                                        }
                                        else if ((mp_wpn_bm16.CheckState == CheckState.Unchecked) && (IMPULSE > 315 || str_find_hp.Length >= 8) && (str_find_weapons == "[mp_wpn_bm16]") && (str_find_weapons != "[mp_actor]"))
                                        {
                                            SrvPlayersBuffer.Add(print);
                                        }
                                        else if ((mp_wpn_wincheaster1300.CheckState == CheckState.Unchecked) && (IMPULSE > 315 || str_find_hp.Length >= 8) && (str_find_weapons == "[mp_wpn_wincheaster1300]") && (str_find_weapons != "[mp_actor]"))
                                        {
                                            SrvPlayersBuffer.Add(print);
                                        }
                                        else if ((mp_wpn_spas12.CheckState == CheckState.Unchecked) && (IMPULSE > 315 || str_find_hp.Length >= 8) && (str_find_weapons == "[mp_wpn_spas12]") && (str_find_weapons != "[mp_actor]"))
                                        {
                                            SrvPlayersBuffer.Add(print);
                                        }
                                        else if ((mp_wpn_ak74u.CheckState == CheckState.Unchecked) && (IMPULSE > 140 || str_find_hp.Length >= 8 || HITPOWER > EventsControllerMaxHPLimit) && (str_find_weapons == "[mp_wpn_ak74u]") && (str_find_weapons != "[mp_actor]"))
                                        {
                                            SrvPlayersBuffer.Add(print);
                                        }
                                        else if ((mp_wpn_ak74.CheckState == CheckState.Unchecked) && (IMPULSE > 140 || str_find_hp.Length >= 8 || HITPOWER > EventsControllerMaxHPLimit) && (str_find_weapons == "[mp_wpn_ak74]") && (str_find_weapons != "[mp_actor]"))
                                        {
                                            SrvPlayersBuffer.Add(print);
                                        }
                                        else if ((mp_wpn_abakan.CheckState == CheckState.Unchecked) && (IMPULSE > 140 || str_find_hp.Length >= 8 || HITPOWER > EventsControllerMaxHPLimit) && (str_find_weapons == "[mp_wpn_abakan]") && (str_find_weapons != "[mp_actor]"))
                                        {
                                            SrvPlayersBuffer.Add(print);
                                        }
                                        else if ((mp_wpn_groza.CheckState == CheckState.Unchecked) && (IMPULSE > 184 || str_find_hp.Length >= 8 || HITPOWER > EventsControllerMaxHPLimit) && (str_find_weapons == "[mp_wpn_groza]") && (str_find_weapons != "[mp_actor]"))
                                        {
                                            SrvPlayersBuffer.Add(print);
                                        }
                                        else if ((mp_wpn_val.CheckState == CheckState.Unchecked) && (IMPULSE > 114 || str_find_hp.Length >= 8 || HITPOWER > EventsControllerMaxHPLimit) && (str_find_weapons == "[mp_wpn_val]") && (str_find_weapons != "[mp_actor]"))
                                        {
                                            SrvPlayersBuffer.Add(print);
                                        }
                                        else if ((mp_wpn_fn2000.CheckState == CheckState.Unchecked) && (IMPULSE > 105 || str_find_hp.Length >= 8 || HITPOWER > EventsControllerMaxHPLimit) && (str_find_weapons == "[mp_wpn_fn2000]") && (str_find_weapons != "[mp_actor]"))
                                        {
                                            SrvPlayersBuffer.Add(print);
                                        }
                                        else if ((mp_wpn_mp5.CheckState == CheckState.Unchecked) && (IMPULSE > 210 || str_find_hp.Length >= 8 || HITPOWER > EventsControllerMaxHPLimit) && (str_find_weapons == "[mp_wpn_mp5]") && (str_find_weapons != "[mp_actor]"))
                                        {
                                            SrvPlayersBuffer.Add(print);
                                        }
                                        else if ((mp_wpn_l85.CheckState == CheckState.Unchecked) && (IMPULSE > 140 || str_find_hp.Length >= 8 || HITPOWER > EventsControllerMaxHPLimit) && (str_find_weapons == "[mp_wpn_l85]") && (str_find_weapons != "[mp_actor]"))
                                        {
                                            SrvPlayersBuffer.Add(print);
                                        }
                                        else if ((mp_wpn_lr300.CheckState == CheckState.Unchecked) && (IMPULSE > 140 || str_find_hp.Length >= 8 || HITPOWER > EventsControllerMaxHPLimit) && (str_find_weapons == "[mp_wpn_lr300]") && (str_find_weapons != "[mp_actor]"))
                                        {
                                            SrvPlayersBuffer.Add(print);
                                        }
                                        else if ((mp_wpn_sig550.CheckState == CheckState.Unchecked) && (IMPULSE > 140 || str_find_hp.Length >= 8 || HITPOWER > EventsControllerMaxHPLimit) && (str_find_weapons == "[mp_wpn_sig550]") && (str_find_weapons != "[mp_actor]"))
                                        {
                                            SrvPlayersBuffer.Add(print);
                                        }
                                        else if ((mp_wpn_g36.CheckState == CheckState.Unchecked) && (IMPULSE > 105 || str_find_hp.Length >= 8 || HITPOWER > EventsControllerMaxHPLimit) && (str_find_weapons == "[mp_wpn_g36]") && (str_find_weapons != "[mp_actor]"))
                                        {
                                            SrvPlayersBuffer.Add(print);
                                        }
                                        else if ((mp_wpn_vintorez.CheckState == CheckState.Unchecked) && (IMPULSE > 104 || str_find_hp.Length >= 8 || HITPOWER > EventsControllerMaxHPLimit) && (str_find_weapons == "[mp_wpn_vintorez]") && (str_find_weapons != "[mp_actor]"))
                                        {
                                            SrvPlayersBuffer.Add(print);
                                        }
                                        else if ((mp_wpn_svu.CheckState == CheckState.Unchecked) && (IMPULSE > 224 || str_find_hp.Length >= 8 || HITPOWER > EventsControllerMaxHPLimit) && (str_find_weapons == "[mp_wpn_svu]") && (str_find_weapons != "[mp_actor]"))
                                        {
                                            SrvPlayersBuffer.Add(print);
                                        }
                                        else if ((mp_wpn_svd.CheckState == CheckState.Unchecked) && (IMPULSE > 245 || str_find_hp.Length >= 8 || HITPOWER > EventsControllerMaxHPLimit) && (str_find_weapons == "[mp_wpn_svd]") && (str_find_weapons != "[mp_actor]"))
                                        {
                                            SrvPlayersBuffer.Add(print);
                                        }
                                        else if ((mp_wpn_gauss.CheckState == CheckState.Unchecked) && (IMPULSE > 3001 || str_find_hp.Length >= 8 || HITPOWER > EventsControllerMaxHPLimit) && (str_find_weapons == "[mp_wpn_gauss]") && (str_find_weapons != "[mp_actor]"))
                                        {
                                            SrvPlayersBuffer.Add(print);
                                        }
                                        if (HITPOWER > 40)
                                        {
                                            SrvPlayersBuffer.Add(print + " <-> Too many bullets for the value Hit Power");
                                        }
                                    }
                                }

                                if (s.Contains("session_id: "))
                                {
                                    string StrListplayers = s.Remove(0, s.IndexOf("name:"));
                                    StrListplayers = StrListplayers.Substring(0, StrListplayers.LastIndexOf("ping: "));
                                    StrListplayers = StrListplayers.Replace(",", "").Replace("(", "").Replace(")", "");
                                    var get_name = StrListplayers.Split()[1];
                                    if (get_name.Length > 0)
                                    {
                                        int a = StrListplayers.IndexOf("session_id:");
                                        int b = StrListplayers.IndexOf("hash:");
                                        StrListplayers = StrListplayers.Remove(a, b - a);
                                        SrvPlayersBuffer.Add(StrListplayers);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                LogProc(ex.Message + Environment.NewLine);
                                ERROR_COUNTER++;
                                ListCheaterEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy]: ")) + "[AntiCheatThread]: Ошибка выполнения команды => " + ex.Message).BackColor = Color.Lime;
                            }
                        }
                    }
                }
                FinishReadIndex = StartReadIndex;
            }
            catch (Exception ex)
            {
                LogProc(ex.Message + Environment.NewLine);
                ERROR_COUNTER++;
                listEventsSrv.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy]: ")) + "[AutoScanBaseThread]: Ошибка выполнения команды => " + ex.Message).BackColor = Color.Lime;
            } 

            foreach (string Messages in SrvEventsBuffer)
            {
                if (Messages.Contains("stalkazz_attack"))
                    server_attack_blocked++;

                if (Messages.Contains("Чат:"))
                {
                    var color_msg = Messages.Split()[0];
                    var check_msg = Messages.Split()[1];

                    if (color_msg + " " + check_msg == "- Чат:")
                    {
                        if (Messages.Length > 200)
                            server_chat_warning++;
                    }
                    else if (color_msg + " " + check_msg == "~ Чат:")
                    {
                        if (Messages.Length > 200)
                            server_chat_warning++;
                    }
                    else if ((color_msg + " " + check_msg.Length == "Чат: " + check_msg.Length) && (check_msg != "ServerAdmin"))
                    {
                        if (Messages.Length > 200)
                            server_chat_warning++;
                    }
                }
            }


            if ((EventsControllerAutoCheckPlayers.CheckState == CheckState.Checked) && (server_version == 1))
            {
                foreach (string ICHEATER in SrvPlayersBuffer)
                {
                    if (ICHEATER.Contains("- | BT"))
                    {
                        string print = ICHEATER.Remove(0, ICHEATER.IndexOf("#"));
                        var str_find_name = ICHEATER.Split()[1];                                      // NAME
                        var str_find_weapons = ICHEATER.Split()[2];                                   // NAME WEAPONS
                        var str_find_hp = print.Split()[6];                                           // HP
                        var str_find = ICHEATER.Split()[13];                                          // выдергиваем только показатели HIM (целое число)
                        str_find = str_find.Substring(0, str_find.LastIndexOf("."));                  // удаляем все после точки, оставляем целое число
                        {                                                                             // Input # Name [weapon] HT 0 HP 1+900.00 - BT 18 HIM 900 K_AP 0.00
                            if (print.Contains("| HIM " + str_find))                                  // Проверяем, точно ли у нашего игрока завышенный HIM
                            {
                                foreach (string get_ip in SrvPlayersBuffer)                           // Находим адреса нашего игрока
                                {
                                    if (get_ip.Contains("name: " + str_find_name))                    // Находим строку по name: NamePlayer
                                    {
                                        var getname = get_ip.Split()[1];                              // NAME

                                        if (getname == str_find_name)                                 // Проверка нужна - чтобы не добавлять в список ложно
                                        {
                                            var getip = get_ip.Split()[5];                            // IP ADDRESS
                                            var gethash = get_ip.Split()[3];                          // HASH     
                                            CHEATERPLAYERSLIST.Add(getname + " % " + getip + " % " + gethash + " % [CHEATER] ");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (EventsWeaponsBlockedPlayers.CheckState == CheckState.Checked)
            {
                int CheckMaxSpaceInLine = 0;
                foreach (string AddCheaterlistReasonWeaponUsage in SrvEventsBuffer)
                {
                    if (AddCheaterlistReasonWeaponUsage.Contains("от Заряд ВОГ-25") || AddCheaterlistReasonWeaponUsage.Contains("M209"))
                    {
                        var StartLine = AddCheaterlistReasonWeaponUsage.Split()[0];
                        var PNAME = AddCheaterlistReasonWeaponUsage.Split()[1];
                        CheckMaxSpaceInLine = AddCheaterlistReasonWeaponUsage.Count(simbolcount => simbolcount == ' ');
                        if (AddCheaterlistReasonWeaponUsage.Contains(StartLine + " " + PNAME) && (CheckMaxSpaceInLine == 5 || CheckMaxSpaceInLine == 6) && (StartLine == "*"))
                        {
                            var CMP_PNAME = AddCheaterlistReasonWeaponUsage.Split()[1];
                            if (CMP_PNAME == PNAME)
                            {
                                foreach (string s in SrvPlayersBuffer)
                                {
                                    if (s.Contains("name: " + PNAME))
                                    {
                                        var getname = s.Split()[1];
                                        if (getname == PNAME)
                                        {
                                            var getip = s.Split()[5];
                                            var gethash = s.Split()[3];
                                            CHEATERPLAYERSLIST.Add(getname + " % " + getip + " % " + gethash + " % [HWEAPONS] ");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            try
            {
                LoadInCheaterList();

                EventsCount = 0;
                ADDRESS_COUNTER_AUTO = 0;
                foreach (string add_ip in CHEATERPLAYERSLIST)  // Пробегаемся по всему списку построчно
                {
                    EventsCount = 0;
                    EventsWeaponsCount = 0;
                    var CMP_GETNAME = add_ip.Split()[0];
                    var GETHASH = add_ip.Split()[4];
                    var GETIP = add_ip.Split()[2];

                    foreach (string WeaponsUsageFlags in SrvPlayersBuffer)
                    {
                        if (WeaponsUsageFlags.Contains("* " + CMP_GETNAME))
                        {
                            var cmpname = WeaponsUsageFlags.Split()[5];
                            if (cmpname == CMP_GETNAME)
                                EventsWeaponsCount++;
                        }
                    }

                    foreach (string CheaterFlags in SrvPlayersBuffer)        // считываем всю завышенную статистику
                    {
                        if (CheaterFlags.Contains("# " + CMP_GETNAME))       // Далее работаем только с CMP_GETNAME
                        {
                            var GETNAME = CheaterFlags.Split()[1];           // из нее выдергиванием имя нарушителя
                            if (CMP_GETNAME == GETNAME)                      // если адрес и ник совпадает, то продолжаем обработку                            
                                EventsCount++;
                        }
                    }


                    // BLOCKED PLAYER
                    if (START_LEVEL_THREAD == 0)
                    {
                        if ((EventsControllerBlockedPlayers.CheckState == CheckState.Checked) && (EventsCount >= EventsControllerMaxLimit.Value) && (server_version == 1))
                        {
                            ADDRESS_COUNTER_AUTO = 0;
                            foreach (string SV_LISTPLAYERS_COUNT_IN_LIST in CHEATERPLAYERSLIST)
                            {
                                if (SV_LISTPLAYERS_COUNT_IN_LIST.Contains(CMP_GETNAME))
                                    if (SV_LISTPLAYERS_COUNT_IN_LIST.Contains("% [CHEATER] "))
                                        ADDRESS_COUNTER_AUTO++;
                            }
                            if ((ADDRESS_COUNTER_AUTO == 1) && (server_version == 1))
                            {
                                BlockedClient(2, CMP_GETNAME, " Flags:[" + EventsCount + "/" + EventsControllerMaxLimit.Value + "] IP: ", GETIP, GETHASH);
                            }
                        }

                        if (EventsWeaponsCount >= EventsWeaponsMaxLimit.Value)
                        {
                            ADDRESS_COUNTER_AUTO = 0;
                            foreach (string SV_LISTPLAYERS_COUNT_IN_LIST in CHEATERPLAYERSLIST)
                            {
                                if (SV_LISTPLAYERS_COUNT_IN_LIST.Contains(CMP_GETNAME))
                                    if (SV_LISTPLAYERS_COUNT_IN_LIST.Contains("% [HWEAPONS] "))
                                        ADDRESS_COUNTER_AUTO++;
                            }
                            if (ADDRESS_COUNTER_AUTO == 1)
                            {
                                BlockedClient(4, CMP_GETNAME, " Flags:[" + EventsWeaponsCount + "/" + EventsWeaponsMaxLimit.Value + "] IP: ", GETIP, GETHASH);
                            }
                        }

                        if (EventCheckProxy.CheckState == CheckState.Checked)
                        {
                            int write_in_base = 0;
                            foreach (string find in CHECK_PLAYER_INPROXYLIST)
                            {
                                int proxy_skip_block = 0;
                                int block_ready = 0;
                                var pAddress = find.Split('%')[0];
                                var pName = find.Split('%')[1];
                                int result_proxy_exit = 0;
                                if (pAddress.Length > 5)
                                {
                                    foreach (string ip in AddressBuffer)
                                    {
                                        if (ip.Contains(pAddress))
                                        {
                                            proxy_skip_block = 4;
                                        }
                                    }

                                    if (proxy_skip_block != 4)
                                    {
                                        foreach (string search_addr in CHECK_ADDR)
                                        {
                                            // white list           
                                            if (search_addr.Contains("NO%PROXY"))
                                                if (search_addr.Contains(pAddress))
                                                    proxy_skip_block++;
                                            // black list
                                            if (search_addr.Contains("PROXY%SERVER"))
                                                if (search_addr.Contains(pAddress))
                                                    block_ready = 1;
                                        }

                                        // Если мы нашли адрес в списке выше, то пропускаем данную проверку
                                        if ((block_ready == 0) && (proxy_skip_block == 0))
                                        {
                                            string result_check_ip = new WebClient().DownloadString("http://proxy.mind-media.com/block/proxycheck.php?ip=" + pAddress);
                                            if (result_check_ip == "Y")
                                            {
                                                block_ready = 1;
                                                foreach (string search_proxy_addr in CHECK_ADDR)
                                                {
                                                    // находим адрес в списке, если его нет то добавляем
                                                    if (search_proxy_addr.Contains(pAddress))
                                                    {
                                                        result_proxy_exit++;
                                                        break;
                                                    }
                                                }
                                                if (result_proxy_exit == 0)
                                                    CHECK_ADDR.Add(pAddress + " Player: " + pName + " PROXY%SERVER <-------------------------> " + DateTime.Now);
                                            }
                                            else
                                            {
                                                CHECK_ADDR.Add(pAddress + " Player: " + pName + " NO%PROXY");
                                            }
                                            write_in_base++;
                                        }
                                        if ((proxy_skip_block == 0) && (block_ready == 1))
                                        {
                                            WriteToCheaterList(pName, pAddress, "null", "[null]", 8);
                                            BlockedClient(8, pName, " [PROXY] IP: ", pAddress, GETHASH);
                                        }
                                    }
                                }
                            }
                            if (write_in_base > 0)
                            {
                                File.WriteAllLines(@"PlayersDataBase\ProxyAddrBase.txt", CHECK_ADDR);
                            }
                        }
                    }
                }

                if (AddressBuffer.Count < SIGNAL_BANNED)
                {
                    WinFirewallActivate.Checked = false;
                    ListCheaterBlockedEvents.Items.Add("[PROCESS FIREWALL FINISHED WITH ERROR]: Process Deactivate: " + AddressBuffer.Count + " < " + SIGNAL_BANNED).BackColor = Color.Red;
                }

                if (START_LEVEL_THREAD == 0 || START_LEVEL_THREAD == 4)
                {
                    if (handler_writer_base != PlayersBaseBuffer.Count)
                    {
                        Save(string.Concat(PlayersBaseBuffer));
                        int get_state = (PlayersBaseBuffer.Count - handler_writer_base);
                        for (int i = 0; i < get_state; i++)
                            statistics_new_players++;
                        handler_writer_base = PlayersBaseBuffer.Count;
                    }

                    foreach (string WriteEventsInBuffer in SrvPlayersBuffer)
                    {
                        BaseEvents.Add(WriteEventsInBuffer);
                    }

                    if (SrvEventsBuffer.Count > 5000)
                    {
                        SrvEventsBuffer.Clear();
                    }

                    GUI_INFO_3.Text = "Новых игроков: " + statistics_new_players;
                    GUI_INFO_BASE.Text = "Игроков в базе: " + PlayersBaseBuffer.Count;
                    GUI_INFO_2.Text = "Атак на сервер: " + (server_chat_warning + server_attack_blocked);
                    str_base_value.Text = "Данных в базе: " + PlayersBaseBuffer.Count;
                    GUI_INFO_EVENTS.Text = "Игровых событий: " + SrvEventsBuffer.Count;
                    GUI_STATUS.Text = "Обработка не требуется";
                    GUI_STATUS.BackColor = Color.White;
                    if ((EventsControllerAutoCheckPlayers.CheckState == CheckState.Checked) && (server_version == 1))
                    {
                        if (SIGNAL_BANNED > 0)
                        {
                            GUI_INFO_1.Text = "Нарушителей:" + CHEATERPLAYERSLIST.Count + "/" + SIGNAL_BANNED;
                        }
                        else if (SIGNAL_BANNED == 0)
                        {
                            GUI_INFO_1.Text = "Нарушителей: " + CHEATERPLAYERSLIST.Count;
                        }
                    }
                }
                else
                {
                    events_load();
                }
            }
            catch (Exception ex)
            {
                LogProc(ex.Message + Environment.NewLine);
                ERROR_COUNTER++;
                ListCheaterBlockedEvents.Items.Add("[ERROR]: " + ex.Message).BackColor = Color.Red;
            }
        }

        bool SubnetActivate = false;
        private void BlockedClient(int Description, string pName, string Flags, string pAddr, string pHash)
        {
            try
            {

                string desc = null;
                foreach (string FoundAddressInBlocked in AddressBuffer)
                {
                    if (FoundAddressInBlocked.Contains(pAddr))
                    {
                        return;
                    }
                }

                int address_format = pAddr.Count(address_check_symbol => address_check_symbol == '.');
                if ((Description > 0) && (Description == 16 || (pAddr.Length >= 7 && pAddr.Length <= 15 && pAddr != "0.0.0.0" && pAddr != "127.0.0.1" && pAddr != "255.255.255.255") && (address_format == 3)))
                {
                    if (Description == 2)
                    {
                        desc = " [CHEATER] ";
                        SrvPlayersBuffer.Add("~ " + pName + " =============== Banned Address: " + pAddr + " =============== " + Flags); // All Server Events
                        WriteToCheaterList(pName, pAddr, pHash, Flags.Replace("Flags:", "").Replace("IP:", ""), 2);                     // Write info in table
                        PlayersBaseBuffer.Add("[V+]%" + pName + "%[CHEATER]:" + Flags.Replace(" IP:", "").Replace(" Flags:", "Flags: ").Replace(" ", "") + "%" + pHash + "%" + pAddr + Environment.NewLine);
                        Thread.Sleep(1000);
                        if (EventsControllerCheckPlayers.CheckState == CheckState.Checked)
                        {
                            EXPORT_DATA_TO_HTML(pName, pAddr, @"PlayersDataBase\html_cheater_base\" + pName.Replace((char)0x22, (char)0x5F).Replace((char)0x2F, (char)0x5F).Replace((char)0x5C, (char)0x5F).Replace((char)0x3C, (char)0x5F).Replace((char)0x3E, (char)0x5F).Replace((char)0x3A, (char)0x5F).Replace((char)0x7C, (char)0x5F).Replace((char)0x3F, (char)0x5F).Replace((char)0x2A, (char)0x5F) + DateTime.Now.ToString(" [dd.MM.yyyy--HH-mm-ss]") + "[CHEATER].html", @"<hr><br/><font style='color:Green'>Файл сформирован программой S.E.R.V.E.R - Shadow Of Chernobyl в Автоматическом режиме в: " + DateTime.Now + "</font>");
                        }
                        if (EventsControllerTrayMessage.CheckState == CheckState.Checked)
                        {
                            notifyIcon.ShowBalloonTip(1000, "Заблокирован нарушитель", "Name: " + pName + "\nReason: [CHEATER]:" + Flags.Replace(" IP:", "\nIP:") + pAddr, ToolTipIcon.Warning);
                            Thread.Sleep(3000);
                        }
                    }
                    else if (Description == 4)
                    {
                        desc = " [HWEAPONS] ";
                        SrvPlayersBuffer.Add("~ " + pName + " =============== Banned Address: " + pAddr + " =============== " + Flags);
                        WriteToCheaterList(pName, pAddr, pHash, Flags.Replace("Flags:", "").Replace("IP:", ""), 4);
                        Thread.Sleep(1000);
                        if (EventsControllerTrayMessage.CheckState == CheckState.Checked)
                        {
                            notifyIcon.ShowBalloonTip(1000, "Заблокирован нарушитель", "Name: " + pName + "\nReason: [HWEAPONS]:" + Flags.Replace(" IP:", "\nIP:") + pAddr, ToolTipIcon.Warning);
                            Thread.Sleep(3000);
                        }
                    }
                    else if (Description == 8)
                    {
                        desc = " [ADMIN] ";
                    }

                    if (EventsBlockedSubnet.CheckState == CheckState.Checked)
                        SubnetActivate = true;
                    else
                        SubnetActivate = false;

                    IPBlockedServices.FirewallNewRuleCreate(desc + pName.Replace((char)0x22, (char)0x5F).Replace((char)0x2F, (char)0x5F).Replace((char)0x5C, (char)0x5F).Replace((char)0x3C, (char)0x5F).Replace((char)0x3E, (char)0x5F).Replace((char)0x3A, (char)0x5F).Replace((char)0x7C, (char)0x5F).Replace((char)0x3F, (char)0x5F).Replace((char)0x2A, (char)0x5F) + Flags, pAddr, SubnetActivate, AddressByteMask.Text);
                    FirewallAddressBaseLoad();
                    SIGNAL_BANNED++;  // Exit == AddressBuffer.Count < SIGNAL_BANNED 
                }
            }
            catch (Exception)
            {
                ERROR_COUNTER++;
            }
        }

        private void LoadInCheaterList()
        {
            PlayersCheaterList.Items.Clear();
            foreach (string add_ip in CHEATERPLAYERSLIST)
            {
                string[] values = add_ip.Split('%');
                if (add_ip.Contains("% [CHEATER] "))
                {
                    PlayersCheaterList.Items.Add(new ListViewItem(values)).BackColor = Color.Violet;
                }
                else if (add_ip.Contains("% [HWEAPONS] "))
                {
                    PlayersCheaterList.Items.Add(new ListViewItem(values)).BackColor = Color.PaleVioletRed;
                }
            }
        }

        private void WriteToCheaterList(string pName, string pAddr, string pHash, string pFlagsCount, int pReason)
        {
            try
            {
                statistics_new_blocked++;
                ListViewItem Write = new ListViewItem(pName);
                Write.SubItems.Add(pAddr);
                Write.SubItems.Add(pHash);
                if (pReason == 2)
                {
                    Write.SubItems.Add("[CHEATER]: " + pFlagsCount);
                    Write.SubItems.Add(DateTime.Now.ToString("dd.MM.yyyy---HH:mm:ss"));
                    ListCheaterBlockedEvents.Items.Add(Write).BackColor = Color.Violet;
                }
                else if (pReason == 4)
                {
                    Write.SubItems.Add("[HWEAPONS]: " + pFlagsCount);
                    Write.SubItems.Add(DateTime.Now.ToString("dd.MM.yyyy---HH:mm:ss"));
                    ListCheaterBlockedEvents.Items.Add(Write).BackColor = Color.Coral;
                }
                else if (pReason == 8)
                {
                    Write.SubItems.Add("[PROXY]: " + pFlagsCount);
                    Write.SubItems.Add(DateTime.Now.ToString("dd.MM.yyyy---HH:mm:ss"));
                    ListCheaterBlockedEvents.Items.Add(Write).BackColor = Color.Gold;
                }
                else if (pReason == 16)
                {
                    Write.SubItems.Add("[FAKE-PLAYER]: " + pFlagsCount);
                    Write.SubItems.Add(DateTime.Now.ToString("dd.MM.yyyy---HH:mm:ss"));
                    ListCheaterBlockedEvents.Items.Add(Write).BackColor = Color.Lime;
                }
            }
            catch (Exception)
            {
                ERROR_COUNTER++;
            }
        }

        private void ThreadListplayers_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                GUI_INFO_BLOCKED.Text = "Новых блокировок: " + statistics_new_blocked;
                if (START_LEVEL_THREAD == 0)
                {
                    ScanDataFiles.Start();
                    GUI_MINI_MENU.Enabled = true;
                }
                if (get_show_informer_at_threading_complited == 1)
                {
                    get_show_informer_at_threading_complited = 0;
                    notifyIcon.ShowBalloonTip(1000, "Успешное завершение", "Игроки были успешно добавлены в базу.", ToolTipIcon.Info);
                }
                if (START_LEVEL_THREAD == 2)
                {               
                    btnExit.Enabled = true;
                    StartAutoCheckThread.Enabled = true;
                    try
                    {
                        if (EventsChangeWindow.CheckState == CheckState.Checked) // используется старый интерфейс
                        {
                            listBase.Items.Clear();

                            foreach (string s in ListplayersEvents)
                            {
                                string[] values = s.Split('%');
                                listBase.Items.Add(new ListViewItem(values));
                            }
                        }
                        else  // используется новый интерфейс     
                        {
                            ListTableChange1.Items.Clear();
                            foreach (string ipblocked in SrvEventsBuffer)
                            {
                                if (ipblocked.Contains("stalkazz_attack"))
                                {
                                    var IP = ipblocked.Split('%')[4];
                                    var blocked = ipblocked.Replace("stalkazz_attack", "[ATTACK]:" + IP);
                                    string[] values = blocked.Split('%');
                                    ListTableChange1.Items.Add(new ListViewItem(values)).BackColor = Color.DeepPink;
                                }
                            }
                            if (EventsPlayersAutoColor.CheckState == CheckState.Unchecked)
                            {
                                foreach (string s in ListplayersEvents)
                                {
                                    try
                                    {
                                        int color_str = 0;
                                        int warning_chat_events = 0;
                                        var PNAME = s.Split('%')[1];
                                        var PADDR = s.Split('%')[4];
                                        foreach (string ServerEVE in ListplayersEvents)
                                        {
                                            if (ServerEVE.Contains(PNAME))
                                            {
                                                if (ServerEVE.Contains("server") || (ServerEVE.Contains("0.0.0.0")))
                                                {
                                                    color_str = 14;
                                                    break;
                                                }
                                            }
                                        }

                                        foreach (string CheckChatSize in SrvEventsBuffer)
                                        {
                                            if (CheckChatSize.Contains("~ Чат: " + PNAME))
                                            {
                                                if (CheckChatSize.Length >= 150)
                                                {
                                                    var correct_msg = CheckChatSize.Split(':')[1].Replace(" ", "");
                                                    if (correct_msg == PNAME)
                                                    {
                                                        color_str = 8;
                                                        warning_chat_events = 1;
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    var correct_msg = CheckChatSize.Split(':')[1].Replace(" ", "");
                                                    if (correct_msg == PNAME)
                                                    {
                                                        color_str = 10;
                                                        break;
                                                    }
                                                }
                                            }

                                            else if (CheckChatSize.Contains("- Чат: " + PNAME))
                                            {
                                                if (CheckChatSize.Length >= 150)
                                                {
                                                    var correct_msg = CheckChatSize.Split(':')[1].Replace(" ", "");
                                                    if (correct_msg == PNAME)
                                                    {
                                                        color_str = 8;
                                                        warning_chat_events = 1;
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    var correct_msg = CheckChatSize.Split(':')[1].Replace(" ", "");
                                                    if (correct_msg == PNAME)
                                                    {
                                                        color_str = 10;
                                                        break;
                                                    }
                                                }
                                            }

                                            else if (CheckChatSize.Contains("Чат: " + PNAME))
                                            {
                                                if (CheckChatSize.Length >= 150)
                                                {
                                                    var correct_msg = CheckChatSize.Split(':')[1].Replace(" ", "");
                                                    if (correct_msg == PNAME)
                                                    {
                                                        color_str = 8;
                                                        warning_chat_events = 1;
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    var correct_msg = CheckChatSize.Split(':')[1].Replace(" ", "");
                                                    if (correct_msg == PNAME)
                                                    {
                                                        color_str = 10;
                                                        break;
                                                    }
                                                }
                                            }
                                        }

                                        // Check Use Weapons
                                        foreach (string WeaponsEVE in SrvEventsBuffer)
                                        {
                                            if (WeaponsEVE.Contains("от Заряд ВОГ-25") || WeaponsEVE.Contains("M209"))
                                            {
                                                var StartLineScan = WeaponsEVE.Split()[0];
                                                int CheckLineMaxSpace = WeaponsEVE.Count(simbolcount => simbolcount == ' ');
                                                if (server_version == 1)
                                                {
                                                    if (WeaponsEVE.Contains(StartLineScan + " " + PNAME) && (CheckLineMaxSpace == 5 || CheckLineMaxSpace == 6) && (StartLineScan == "*"))
                                                    {
                                                        var CMP_PNAME = WeaponsEVE.Split()[1];
                                                        if (CMP_PNAME == PNAME)
                                                        {
                                                            color_str = 12;
                                                            break;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (WeaponsEVE.Contains(PNAME) && (CheckLineMaxSpace == 4 || CheckLineMaxSpace == 5) && (StartLineScan == PNAME))
                                                    {
                                                        var CMP_PNAME = WeaponsEVE.Split()[0];
                                                        if (CMP_PNAME == PNAME)
                                                        {
                                                            color_str = 12;
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        foreach (string search_player_in_notes in PlayersBaseBuffer)
                                        {
                                            if (search_player_in_notes.Contains("[N+]%"))
                                            {
                                                if (search_player_in_notes.Contains(PADDR))
                                                {
                                                    color_str = 1;
                                                    break;
                                                }
                                            }
                                        }

                                        foreach (string search_player_in_cheaterlist in PlayersBaseBuffer)
                                        {
                                            if (search_player_in_cheaterlist.Contains("[V+]%"))
                                            {
                                                if (search_player_in_cheaterlist.Contains(PADDR))
                                                {
                                                    color_str = 2;
                                                    break;
                                                }
                                            }
                                        }

                                        if (server_version == 1)
                                        {
                                            foreach (string PLAYER_CHEATER_FOUND in SrvPlayersBuffer)
                                            {
                                                if (PLAYER_CHEATER_FOUND.Contains("# " + PNAME + " ["))
                                                {
                                                    var str_cmp_name = PLAYER_CHEATER_FOUND.Split()[1];
                                                    if (str_cmp_name == PNAME)
                                                    {                                                                                           
                                                        color_str = 4;
                                                        break;
                                                    }
                                                }
                                            }
                                        }

                                        if (!(PADDR.Length >= 7 && PADDR.Length <= 15))      // check format address                                   
                                            color_str = 16;
                                       
                                        string result_scan = s;
                                        if (color_str == 4)                                   // cheater found
                                            result_scan = s.Replace("[A]", "[PK]");
                                        else if (color_str == 2 && warning_chat_events == 0)  // chat hack + cheater
                                            result_scan = s.Replace("[A]", "[PJ]");
                                        else if (color_str == 2 && warning_chat_events == 1)
                                            result_scan = s.Replace("[A]", "[PL]");
                                        else if (color_str == 16)
                                            result_scan = s.Replace("[A]", "[PL]").Replace(PNAME, "[ERROR FORMAT]: " + PNAME);

                                        string[] values = result_scan.Split('%');
                                        if (color_str == 1)                                                                     // Notes
                                            ListTableChange1.Items.Add(new ListViewItem(values)).BackColor = Color.LightGreen;
                                        if (color_str == 2 && warning_chat_events == 0)                                         // cheater in base list
                                            ListTableChange1.Items.Add(new ListViewItem(values)).BackColor = Color.Gold;
                                        else if (color_str == 4)                                                                // cheater detect
                                            ListTableChange1.Items.Add(new ListViewItem(values)).BackColor  = Color.Blue;                                      
                                        else if (color_str == 6)                                                                // radmin login
                                            ListTableChange1.Items.Add(new ListViewItem(values)).BackColor = Color.ForestGreen;
                                        else if (color_str == 2 && warning_chat_events == 1)                                    // chat hack + cheater
                                            ListTableChange1.Items.Add(new ListViewItem(values)).BackColor = Color.Red;
                                        else if (color_str == 8 || warning_chat_events == 1)                                    // chat size 
                                            ListTableChange1.Items.Add(new ListViewItem(values)).BackColor = Color.Magenta;
                                        else if (color_str == 10)                                                               // new chat msg
                                            ListTableChange1.Items.Add(new ListViewItem(values)).BackColor = Color.Honeydew;
                                        else if (color_str == 12)                                                               // heavy weapons
                                            ListTableChange1.Items.Add(new ListViewItem(values)).BackColor = Color.Aquamarine;
                                        else if (color_str == 14)                                                               // server
                                            ListTableChange1.Items.Add(new ListViewItem(values)).BackColor = Color.Lime;
                                        else if (color_str == 16)                                                               // bad address format
                                            ListTableChange1.Items.Add(new ListViewItem(values)).BackColor = Color.Coral;
                                        else
                                            ListTableChange1.Items.Add(new ListViewItem(values));
                                    }
                                    catch (Exception)
                                    {
                                        ERROR_COUNTER++;
                                    }
                                }
                            }
                            else
                            {
                                foreach (string s in ListplayersEvents)
                                {
                                    string[] values = s.Split('%');
                                    ListTableChange1.Items.Add(new ListViewItem(values));
                                }
                            }

                            if (ListTableChange1.Items.Count > 0)                // Загружаем список игроков
                            {
                                ListViewItem items = new ListViewItem("[PL]");
                                items.SubItems.Add(Convert.ToString("Найдено игроков: " + (ListTableChange1.Items.Count)));
                                ListTableChange1.Items.Add(items).BackColor = Color.Aqua;
                            }
                            blocked_events_handler = 0;
                            if (server_version == 1)
                            {
                                PlayersCheaterList.Items.Clear();
                                foreach (string s in CHEATERPLAYERSLIST)
                                {
                                    string[] values = s.Split('%');
                                    PlayersCheaterList.Items.Add(new ListViewItem(values)).BackColor = Color.Violet;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ERROR_COUNTER++;
                        listEventsSrv.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy]: ")) + "[BaseListplayersThreadFinished]: Ошибка выполнения команды => " + ex.Message).BackColor = Color.Lime;
                    }
                }
            }

            catch (Exception)
            {
                ERROR_COUNTER++;
            }
        }

        private void btnGetSearchAddr_Click(object sender, EventArgs e)
        {
            try
            {
                int sscan = 0;
                string address = PlayersCheaterList.FocusedItem.SubItems[1].Text;
                listBase.Items.Clear();
                foreach (string s in PlayersBaseBuffer)
                {
                    if (s.Contains(address.Replace(" ", "")))
                    {
                        sscan++;
                        string[] values = s.Split('%');
                        listBase.Items.Add(new ListViewItem(values));
                    }
                }
                if (sscan != 0)
                {
                    tabControl1.SelectedIndex = 0;
                }
                else
                {
                    MessageBox.Show("Поиск не дал результатов.", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Произошла ошибка. Невозможно скопировать пустую строку.", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnGetBanplayer_Click(object sender, EventArgs e)
        {
            try
            {
                string checkName = PlayersCheaterList.FocusedItem.SubItems[0].Text.Replace(" ", string.Empty);
                string checkAddr = PlayersCheaterList.FocusedItem.SubItems[1].Text.Replace(" ", string.Empty);
                BlockedClient(8, checkName, " Flags:[null] IP: ", checkAddr, "@");
                tabControl1.SelectedIndex = 6;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка. Reason:\n" + ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        HashSet<string> html_buffer = new HashSet<string>();
        private void btnExportToHTML_Click(object sender, EventArgs e)
        {
            try
            {          
                SaveFileDialog ExportBase = new SaveFileDialog();
                ExportBase.DefaultExt = "*.html";
                ExportBase.Filter = "Web-HTML|*.html";
                if (ExportBase.ShowDialog() == DialogResult.OK && ExportBase.FileName.Length > 0)
                {
                    var html_name = PlayersCheaterList.FocusedItem.SubItems[0].Text;
                    var html_addr = PlayersCheaterList.FocusedItem.SubItems[1].Text;    
                    EXPORT_DATA_TO_HTML(html_name, html_addr, ExportBase.FileName, @"<hr><font style='color:blue'>Файл сформирован программой S.E.R.V.E.R - Shadow Of Chernobyl в: " + DateTime.Now + "</font>");
                }                      
            }          
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EXPORT_DATA_TO_HTML(string html_name,string address, string src, string description)
        {
            try
            {
                html_buffer.Add("<html><head><meta> <meta http-equiv='content-type' content ='text/html; charset=utf-8'> <title>" + html_name + " - S.E.R.V.E.R - Shadow Of Chernobyl</title></head><body> <center><h1> Информация об игроке: <font style='color:green'>" + html_name + "</font></h1></center><hr>");
                foreach (string print in SrvPlayersBuffer)
                {
                    if (print.Contains("name: " + html_name))
                    {
                        html_buffer.Add(@"<hr align='left' width='500' size='3' color='gold'<br><span style=""background-color:gold"">" + print + "</span><br><hr align='left' width='500' size='3' color='gold'<br>");
                    }

                    if (print.Contains("# " + html_name))
                    {
                        var str_pname = print.Split()[1];
                        var str_find_weapons = print.Split()[2];
                        var str_ht = print.Split()[3];
                        var str_count = print.Split()[4];
                        var str_find_hp = print.Split()[6];
                        var str_btcount = print.Split()[10];
                        var str_find = print.Split()[13];
                        var str_K_APcount = print.Split()[16];
                        str_find = str_find.Substring(0, str_find.LastIndexOf("."));
                        var str_hp_count = str_find_hp.Substring(0, str_find_hp.LastIndexOf("+"));

                        // Игрок / Снаряжение / Параметр хит тип / параметр хит павер / боне тип / хит импульс /тип патронов
                        if ((str_find_weapons == "[mp_wpn_knife]") && (str_find_weapons != "[mp_actor]"))
                        {
                            // Если завышено и то и другое
                            if ((str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit) && (Convert.ToInt64(str_find) > 120))
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 120] </font> | K_AP " + str_K_APcount + "<br>");
                            // Если завышен HIM
                            else if (Convert.ToInt64(str_find) > 120)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " HP " + str_find_hp + " | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 120] </font> | K_AP " + str_K_APcount + "<br>");
                            // Если завышен HP
                            else if (str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | HIM " + str_find + " </font> | K_AP " + str_K_APcount + "<br>");
                        }

                        else if ((str_find_weapons == "[mp_wpn_pm]") && (str_find_weapons != "[mp_actor]"))
                        {
                            if ((str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit) && (Convert.ToInt64(str_find) > 91))
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 91] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (Convert.ToInt64(str_find) > 91)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " HP " + str_find_hp + " | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 91] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | HIM " + str_find + " </font> | K_AP " + str_K_APcount + "<br>");
                        }
                        else if ((str_find_weapons == "[mp_wpn_fort]") && (str_find_weapons != "[mp_actor]"))
                        {
                            if ((str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit) && (Convert.ToInt64(str_find) > 100))
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 100] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (Convert.ToInt64(str_find) > 100)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " HP " + str_find_hp + " | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 100] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | HIM " + str_find + " </font> | K_AP " + str_K_APcount + "<br>");
                        }
                        else if ((str_find_weapons == "[mp_wpn_walther]") && (str_find_weapons != "[mp_actor]"))
                        {
                            if ((str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit) && (Convert.ToInt64(str_find) > 131))
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 131] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (Convert.ToInt64(str_find) > 131)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " HP " + str_find_hp + " | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 131] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | HIM " + str_find + " </font> | K_AP " + str_K_APcount + "<br>");
                        }
                        else if ((str_find_weapons == "[mp_wpn_sig220]") && (str_find_weapons != "[mp_actor]"))
                        {
                            if ((str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit) && (Convert.ToInt64(str_find) > 126))
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 126] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (Convert.ToInt64(str_find) > 126)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " HP " + str_find_hp + " | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 126] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | HIM " + str_find + " </font> | K_AP " + str_K_APcount + "<br>");
                        }
                        else if ((str_find_weapons == "[mp_wpn_usp]") && (str_find_weapons != "[mp_actor]"))
                        {
                            if ((str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit) && (Convert.ToInt64(str_find) > 126))
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 126] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (Convert.ToInt64(str_find) > 126)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " HP " + str_find_hp + " | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 126] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | HIM " + str_find + " </font> | K_AP " + str_K_APcount + "<br>");
                        }
                        else if ((str_find_weapons == "[mp_wpn_desert_eagle]") && (str_find_weapons != "[mp_actor]"))
                        {
                            if ((str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit) && (Convert.ToInt64(str_find) > 144))
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 144] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (Convert.ToInt64(str_find) > 144)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " HP " + str_find_hp + " | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 144] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | HIM " + str_find + " </font> | K_AP " + str_K_APcount + "<br>");
                        }
                        else if ((str_find_weapons == "[mp_wpn_pb]") && (str_find_weapons != "[mp_actor]"))
                        {
                            if ((str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit) && (Convert.ToInt64(str_find) > 72))
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 72] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (Convert.ToInt64(str_find) > 72)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " HP " + str_find_hp + " | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 72] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (str_find_hp.Length >= 8)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | HIM " + str_find + " </font> | K_AP " + str_K_APcount + "<br>");
                        }
                        else if ((str_find_weapons == "[mp_wpn_colt1911]") && (str_find_weapons != "[mp_actor]"))
                        {
                            if ((str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit) && (Convert.ToInt64(str_find) > 126))
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 126] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (Convert.ToInt64(str_find) > 126)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " HP " + str_find_hp + " | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 126] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | HIM " + str_find + " </font> | K_AP " + str_K_APcount + "<br>");
                        }
                        else if ((str_find_weapons == "[mp_wpn_bm16]") && (str_find_weapons != "[mp_actor]"))
                        {
                            if ((str_find_hp.Length >= 8) && (Convert.ToInt64(str_find) > 315))
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 315] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (Convert.ToInt64(str_find) > 315)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " HP " + str_find_hp + " | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 315] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (str_find_hp.Length >= 8)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | HIM " + str_find + " </font> | K_AP " + str_K_APcount + "<br>");
                        }
                        else if ((str_find_weapons == "[mp_wpn_wincheaster1300]") && (str_find_weapons != "[mp_actor]"))
                        {
                            if ((str_find_hp.Length >= 8) && (Convert.ToInt64(str_find) > 315))
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 315] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (Convert.ToInt64(str_find) > 315)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " HP " + str_find_hp + " | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 315] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (str_find_hp.Length >= 8)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | HIM " + str_find + " </font> | K_AP " + str_K_APcount + "<br>");
                        }
                        else if ((str_find_weapons == "[mp_wpn_spas12]") && (str_find_weapons != "[mp_actor]"))
                        {
                            if ((str_find_hp.Length >= 8) && (Convert.ToInt64(str_find) > 315))
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 315] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (Convert.ToInt64(str_find) > 315)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " HP " + str_find_hp + " | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 315] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (str_find_hp.Length >= 8)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | HIM " + str_find + " </font> | K_AP " + str_K_APcount + "<br>");
                        }
                        else if ((str_find_weapons == "[mp_wpn_ak74u]") && (str_find_weapons != "[mp_actor]"))
                        {
                            if ((str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit) && (Convert.ToInt64(str_find) > 140))
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 140] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (Convert.ToInt64(str_find) > 140)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " HP " + str_find_hp + " | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 140] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | HIM " + str_find + " </font> | K_AP " + str_K_APcount + "<br>");
                        }
                        else if ((str_find_weapons == "[mp_wpn_ak74]") && (str_find_weapons != "[mp_actor]"))
                        {
                            if ((str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit) && (Convert.ToInt64(str_find) > 140))
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 140] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (Convert.ToInt64(str_find) > 140)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " HP " + str_find_hp + " | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 140] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | HIM " + str_find + " </font> | K_AP " + str_K_APcount + "<br>");
                        }
                        else if ((str_find_weapons == "[mp_wpn_abakan]") && (str_find_weapons != "[mp_actor]"))
                        {
                            if ((str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit) && (Convert.ToInt64(str_find) > 140))
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 140] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (Convert.ToInt64(str_find) > 140)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " HP " + str_find_hp + " | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 140] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | HIM " + str_find + " </font> | K_AP " + str_K_APcount + "<br>");
                        }
                        else if ((str_find_weapons == "[mp_wpn_groza]") && (str_find_weapons != "[mp_actor]"))
                        {
                            if ((str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit) && (Convert.ToInt64(str_find) > 184))
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 184] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (Convert.ToInt64(str_find) > 184)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " HP " + str_find_hp + " | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 184] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | HIM " + str_find + " </font> | K_AP " + str_K_APcount + "<br>");
                        }
                        else if ((str_find_weapons == "[mp_wpn_val]") && (str_find_weapons != "[mp_actor]"))
                        {
                            if ((str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit) && (Convert.ToInt64(str_find) > 114))
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 114] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (Convert.ToInt64(str_find) > 114)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " HP " + str_find_hp + " | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 114] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | HIM " + str_find + " </font> | K_AP " + str_K_APcount + "<br>");
                        }
                        else if ((str_find_weapons == "[mp_wpn_fn2000]") && (str_find_weapons != "[mp_actor]"))
                        {
                            if ((str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit) && (Convert.ToInt64(str_find) > 105))
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 105] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (Convert.ToInt64(str_find) > 105)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " HP " + str_find_hp + " | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 105] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | HIM " + str_find + " </font> | K_AP " + str_K_APcount + "<br>");
                        }
                        else if ((str_find_weapons == "[mp_wpn_mp5]") && (str_find_weapons != "[mp_actor]"))
                        {
                            if ((str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit) && (Convert.ToInt64(str_find) > 210))
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 210] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (Convert.ToInt64(str_find) > 210)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " HP " + str_find_hp + " | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 210] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | HIM " + str_find + " </font> | K_AP " + str_K_APcount + "<br>");
                        }
                        else if ((str_find_weapons == "[mp_wpn_l85]") && (str_find_weapons != "[mp_actor]"))
                        {
                            if ((str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit) && (Convert.ToInt64(str_find) > 140))
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 140] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (Convert.ToInt64(str_find) > 140)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " HP " + str_find_hp + " | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 140] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | HIM " + str_find + " </font> | K_AP " + str_K_APcount + "<br>");
                        }
                        else if ((str_find_weapons == "[mp_wpn_lr300]") && (str_find_weapons != "[mp_actor]"))
                        {
                            if ((str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit) && (Convert.ToInt64(str_find) > 140))
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 140] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (Convert.ToInt64(str_find) > 140)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " HP " + str_find_hp + " | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 140] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | HIM " + str_find + " </font> | K_AP " + str_K_APcount + "<br>");
                        }
                        else if ((str_find_weapons == "[mp_wpn_sig550]") && (str_find_weapons != "[mp_actor]"))
                        {
                            if ((str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit) && (Convert.ToInt64(str_find) > 140))
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 140] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (Convert.ToInt64(str_find) > 140)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " HP " + str_find_hp + " | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 140] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | HIM " + str_find + " </font> | K_AP " + str_K_APcount + "<br>");
                        }
                        else if ((str_find_weapons == "[mp_wpn_g36]") && (str_find_weapons != "[mp_actor]"))
                        {
                            if ((str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit) && (Convert.ToInt64(str_find) > 105))
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 105] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (Convert.ToInt64(str_find) > 105)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " HP " + str_find_hp + " | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 105] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | HIM " + str_find + " </font> | K_AP " + str_K_APcount + "<br>");
                        }
                        else if ((str_find_weapons == "[mp_wpn_vintorez]") && (str_find_weapons != "[mp_actor]"))
                        {
                            if ((str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit) && (Convert.ToInt64(str_find) > 104))
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 104] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (Convert.ToInt64(str_find) > 104)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " HP " + str_find_hp + " | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 104] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | HIM " + str_find + " </font> | K_AP " + str_K_APcount + "<br>");
                        }
                        else if ((str_find_weapons == "[mp_wpn_svu]") && (str_find_weapons != "[mp_actor]"))
                        {
                            if ((str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit) && (Convert.ToInt64(str_find) > 224))
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 224] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (Convert.ToInt64(str_find) > 224)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " HP " + str_find_hp + " | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 224] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | HIM " + str_find + " </font> | K_AP " + str_K_APcount + "<br>");
                        }
                        else if ((str_find_weapons == "[mp_wpn_svd]") && (str_find_weapons != "[mp_actor]"))
                        {
                            if ((str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit) && (Convert.ToInt64(str_find) > 245))
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 245] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (Convert.ToInt64(str_find) > 245)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " HP " + str_find_hp + " | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 245] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | HIM " + str_find + " </font> | K_AP " + str_K_APcount + "<br>");
                        }
                        else if ((str_find_weapons == "[mp_wpn_gauss]") && (str_find_weapons != "[mp_actor]"))
                        {
                            if ((str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit) && (Convert.ToInt64(str_find) > 3001))
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 3000] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (Convert.ToInt64(str_find) > 3001)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " HP " + str_find_hp + " | BT " + str_btcount + " | <font style='color:red'>HIM " + str_find + " [Max: 3000] </font> | K_AP " + str_K_APcount + "<br>");
                            else if (str_find_hp.Length >= 8 || Convert.ToInt64(str_hp_count) > EventsControllerMaxHPLimit)
                                html_buffer.Add(str_pname + " " + str_find_weapons + " " + str_ht + " " + str_count + " <font style='color:blue'>HP " + str_find_hp + " </font> | BT " + str_btcount + " | HIM " + str_find + " </font> | K_AP " + str_K_APcount + "<br>");
                        }
                    }
                }
                html_buffer.Add(description + "<br><br><font style='color:blue'>Синей подсветкой подсвечены завышенные данные по HP (Hit Power)</font><br><font style='color:red'>Красной подсветкой подсвечены завышенные данные по HIM (Hit Impulse)</font><br><font style='color:orange'>Оранжевый разделитель служит для случаев, если встречаются несколько игроков с 1 именем, но с разным адресом или хешом.<br>В этом случаи следует обратить внимание на того игрока, после которого пошли данные сообщения с завышенными данными</font><br><br><br>");

                File.WriteAllText(src, string.Concat(html_buffer));
                html_buffer.Clear();
            }
            catch (Exception ex)
            {
                ListCheaterEvents.Items.Add("[SYSTEM]: EXPORT_TO_HTML_ERROR: " + ex.Message).BackColor = Color.Orange;
            }
        }
 
        private void BASE_MENU_1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            StartAutoCheckThread.Checked = true;
        }

        private void BASE_MENU_2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenDirHTML();
        }

        private void BASE_MENU_3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                tabControl1.SelectedIndex = 6;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка. Повторите попытку\nReason:\n" + ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BASE_MENU_4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                tabControl1.SelectedIndex = 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка. Reason:\n" + ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BASE_MENU_5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                SaveFileDialog ExportData = new SaveFileDialog();
                ExportData.DefaultExt = "*.log";
                ExportData.Filter = "Текстовый файл|*.log";
                if (ExportData.ShowDialog() == DialogResult.OK && ExportData.FileName.Length > 0)
                    File.WriteAllLines(ExportData.FileName, BaseEvents);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // Проверка игроков
        int badshots = 0;
        int PlayersCount = 0;
        int find_address = 0;
        int update_blocked = 0;
        int weapons_counter = 0;

        private void btnOpenDirHTML_Click(object sender, EventArgs e)
        {
            OpenDirHTML();
        }

        private void btnClearCheaterBuffer_Click(object sender, EventArgs e)
        {
            CHEATERPLAYERSLIST.Clear();
            SrvPlayersBuffer.Clear();
            ListCheaterEvents.Items.Clear();
            PlayersCheaterList.Items.Clear();
            SIGNAL_BANNED = 0;
            ListCheaterEvents.Items.Add("[SYSTEM]: successfully buffer cleared!").BackColor = Color.Lime;
        }

        private void btnSaveCheatTableInFile_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog ExportData = new SaveFileDialog();
                ExportData.DefaultExt = "*.log";
                ExportData.Filter = "Текстовый файл|*.log";
                if (ExportData.ShowDialog() == DialogResult.OK && ExportData.FileName.Length > 0)               
                    File.WriteAllLines(ExportData.FileName, SrvPlayersBuffer);               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdatePlayerState_Click(object sender, EventArgs e)
        {
            ClickUpdate();
        }

        private void ClickUpdate()
        {
            try
            {
                for (int i = 0; i < PlayersCheaterList.Items.Count; i++)
                {
                    PlayersCheaterList.Items[i].Focused = true;
                    PlayersCheaterList.Items[i].Checked = true;
                    if (i > 0)
                    {
                        PlayersCheaterList.Items[i - 1].Focused = false;
                        PlayersCheaterList.Items[i - 1].Checked = false;
                    }
                }
            }
            catch (Exception ex)
            {
                ERROR_COUNTER++;
                ListCheaterEvents.Items.Add("[SYSTEM]: Error in function drawing table => " + ex.Message).BackColor = Color.Lime;
            }
        }

        private void PlayersCheaterList_Click(object sender, EventArgs e)
        {
            update_blocked = 0;
            ListCheaterEvents.Items.Clear();
            ListCheaterScanner(null, null);
        }

        private void PlayersCheaterList_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            update_blocked = 1;
            ListCheaterScanner(null, null);
        }

        private void ListCheaterScanner(string StrNameBuffer, string StrAddressBuffer)
        {
            badshots = 0;
            PlayersCount = 0;
            find_address = 0;
            weapons_counter = 0;
   
            StrNameBuffer = PlayersCheaterList.FocusedItem.SubItems[0].Text;    // NAME   
            StrAddressBuffer = PlayersCheaterList.FocusedItem.SubItems[1].Text; // IP ADDRESS
          
            SelectTable3.Text = "Сведения из буфера событий.";
            foreach (string anticheat in SrvPlayersBuffer)
            {
                // подсчитываем количество клиентов c данным именем
                if (anticheat.Contains("name: " + StrNameBuffer))
                {
                    PlayersCount++;
                    if (update_blocked == 0)
                        ListCheaterEvents.Items.Add(anticheat).BackColor = Color.Gold;
                }
                // подсчитываем количество убойки (статистика нужна для автоблокировки)
                if ((anticheat.Contains("# " + StrNameBuffer)) && (server_version == 1))
                {
                    badshots++;
                    if (update_blocked == 0)
                        ListCheaterEvents.Items.Add(anticheat).BackColor = Color.Violet;
                }
            }
         
            // Подсчитываем число использования подствольного снаряжения
            if ((EventsWeaponsBlockedPlayers.CheckState == CheckState.Checked) && (EventsWeaponsCount > 0))
            {
                foreach (string anticheat in SrvPlayersBuffer)
                {
                    if (anticheat.Contains("* " + StrNameBuffer))
                    {
                        weapons_counter++;
                        if (update_blocked == 0)
                        {
                            ListCheaterEvents.Items.Add(anticheat).BackColor = Color.Violet;
                        }
                    }
                }
            }

            int addr_skip = 0;
            var player_ip = StrAddressBuffer.Replace(" ", string.Empty);
            addr_skip = player_ip.Count(simbolcount => simbolcount == '.');
            if (!(player_ip.Length >= 7 && player_ip.Length <= 15) && (player_ip != "0.0.0.0" && player_ip != "127.0.0.1" && player_ip != "255.255.255.255") && (addr_skip == 3))
            {
                PlayersCheaterList.FocusedItem.BackColor = Color.DeepSkyBlue;
                PlayersCheaterList.FocusedItem.SubItems.Add("Skip. Bad Address Format");
                PlayersCheaterList.FocusedItem.SubItems.Add(badshots + "/" + EventsControllerMaxLimit.Value);
                return;
            }

            if (PlayersCount == 1)
            {
                try
                {
                    foreach (string ip in AddressBuffer)
                    {
                        if (ip.Contains(player_ip))
                        {
                            find_address = 1;
                            PlayersCheaterList.FocusedItem.BackColor = Color.ForestGreen;
                            PlayersCheaterList.FocusedItem.SubItems.Add("This Address is Banned");   
                            if ((badshots > 0) && (weapons_counter == 0))
                            {
                                PlayersCheaterList.FocusedItem.SubItems.Add(badshots + "/" + EventsControllerMaxLimit.Value);
                            }
                            if ((weapons_counter > 0) && (badshots == 0))
                            {
                                PlayersCheaterList.FocusedItem.SubItems.Add(weapons_counter + "/" + EventsWeaponsMaxLimit.Value);
                            }
                            if ((weapons_counter > 0) && (badshots > 0))
                            {
                                PlayersCheaterList.FocusedItem.SubItems.Add(badshots + "/" + EventsControllerMaxLimit.Value + " | " + weapons_counter + "/" + EventsWeaponsMaxLimit.Value);
                            }
                            return;
                        }
                    }
                    // Завышенные данные статистики
                    if ((find_address == 0) && (EventsControllerMaxLimit.Value > badshots) && (weapons_counter == 0))
                    {
                        PlayersCheaterList.FocusedItem.SubItems.Add("Wait Banning [HIM]");
                        PlayersCheaterList.FocusedItem.SubItems.Add(badshots + "/" + EventsControllerMaxLimit.Value);
                    }
                    // Завышенные данные статистики и статистика снаряжения
                    else if ((find_address == 0) && (EventsControllerMaxLimit.Value < badshots) && (EventsWeaponsMaxLimit.Value < weapons_counter))
                    {
                        PlayersCheaterList.FocusedItem.SubItems.Add("Wait Banning [HIM+WS]");
                        PlayersCheaterList.FocusedItem.SubItems.Add(badshots + "/" + EventsControllerMaxLimit.Value + " | " + weapons_counter + "/" + EventsWeaponsMaxLimit.Value);
                    }
                    // Статистика снаряжения
                    else if ((find_address == 0) && (EventsWeaponsMaxLimit.Value > weapons_counter) && (badshots == 0))
                    {
                        PlayersCheaterList.FocusedItem.SubItems.Add("Wait Banning [WS]");
                        PlayersCheaterList.FocusedItem.SubItems.Add(weapons_counter + "/" + EventsWeaponsMaxLimit.Value);
                    }
                    else
                    {
                        PlayersCheaterList.FocusedItem.BackColor = Color.Red;
                        PlayersCheaterList.FocusedItem.SubItems.Add("IP NOT BLOCKED ?");
                        PlayersCheaterList.FocusedItem.SubItems.Add("?/?");
                    }
                }
                catch (Exception ex)
                {
                    ERROR_COUNTER++;
                    ListCheaterEvents.Items.Add("[ThreadListplayers]: list view update error => " + ex.Message).BackColor = Color.Lime;
                }
            }
            else if (PlayersCount > 1)
            {
                foreach (string AdressInBlocked in AddressBuffer)
                {
                    if (AdressInBlocked.Contains(player_ip))
                    {
                        PlayersCheaterList.FocusedItem.BackColor = Color.Lime;
                        PlayersCheaterList.FocusedItem.SubItems.Add("This Address is Banned");
                        PlayersCheaterList.FocusedItem.SubItems.Add("Double Name");
                        return;
                    }
                }
                PlayersCheaterList.FocusedItem.BackColor = Color.Orange;
                PlayersCheaterList.FocusedItem.SubItems.Add("Skip. Double Player Name");
                PlayersCheaterList.FocusedItem.SubItems.Add(badshots + "/" + EventsControllerMaxLimit.Value);
                if (update_blocked == 0)
                {
                    SelectTable3.Text = "Сведения из буфера событий. Внимание! Игроков с данным именем: " + PlayersCount;
                }
            }
        }

        private void save_settings()
        {
            _base_settings.EventsAutoColor = EventsPlayersAutoColor.Checked;
            _base_settings.EventsDirectoryDF = DmpCurrentDirectory.Text;
            _base_settings.EventsChangeWindow = EventsChangeWindow.Checked;
            _base_settings.FirewallActivated = WinFirewallActivate.Checked;
            _base_settings.EventsControllerCheckPlayers = EventsControllerCheckPlayers.Checked;
            _base_settings.EventsControllerAutoCheckPlayers = EventsControllerAutoCheckPlayers.Checked;
            _base_settings.EventsControllerMaxLimit = EventsControllerMaxLimit.Text;
            _base_settings.EventsControllerBlockedPlayers = EventsControllerBlockedPlayers.Checked;
            _base_settings.EventsWeaponsBlockedPlayers = EventsWeaponsBlockedPlayers.Checked;
            _base_settings.EventsControllerAutoSaveHistoryActivate = EventsControllerAutoSaveHistory.Checked;
            _base_settings.EventsWeaponsMaxLimit = EventsWeaponsMaxLimit.Text;
            _base_settings.EventsControllerMaxClickLim = EventsBlockedSubnet.Checked;
            _base_settings.EventsControllerTrayMessage = EventsControllerTrayMessage.Checked;
            _base_settings.SkipScanWeapons1 = mp_wpn_knife.Checked;
            _base_settings.SkipScanWeapons2 = mp_wpn_pm.Checked;
            _base_settings.SkipScanWeapons3 = mp_wpn_fort.Checked;
            _base_settings.SkipScanWeapons4 = mp_wpn_walther.Checked;
            _base_settings.SkipScanWeapons5 = mp_wpn_sig220.Checked;
            _base_settings.SkipScanWeapons6 = mp_wpn_usp.Checked;
            _base_settings.SkipScanWeapons7 = mp_wpn_desert_eagle.Checked;
            _base_settings.SkipScanWeapons8 = mp_wpn_pb.Checked;
            _base_settings.SkipScanWeapons9 = mp_wpn_colt1911.Checked;
            _base_settings.SkipScanWeapons10 = mp_wpn_bm16.Checked;
            _base_settings.SkipScanWeapons11 = mp_wpn_wincheaster1300.Checked;
            _base_settings.SkipScanWeapons12 = mp_wpn_spas12.Checked;
            _base_settings.SkipScanWeapons13 = mp_wpn_ak74u.Checked;
            _base_settings.SkipScanWeapons14 = mp_wpn_ak74.Checked;
            _base_settings.SkipScanWeapons15 = mp_wpn_abakan.Checked;
            _base_settings.SkipScanWeapons16 = mp_wpn_groza.Checked;
            _base_settings.SkipScanWeapons17 = mp_wpn_val.Checked;
            _base_settings.SkipScanWeapons18 = mp_wpn_fn2000.Checked;
            _base_settings.SkipScanWeapons19 = mp_wpn_mp5.Checked;
            _base_settings.SkipScanWeapons20 = mp_wpn_l85.Checked;
            _base_settings.SkipScanWeapons21 = mp_wpn_lr300.Checked;
            _base_settings.SkipScanWeapons22 = mp_wpn_sig550.Checked;
            _base_settings.SkipScanWeapons23 = mp_wpn_g36.Checked;
            _base_settings.SkipScanWeapons24 = mp_wpn_vintorez.Checked;
            _base_settings.SkipScanWeapons25 = mp_wpn_svu.Checked;
            _base_settings.SkipScanWeapons26 = mp_wpn_svd.Checked;
            _base_settings.SkipScanWeapons27 = mp_wpn_gauss.Checked;
       
            _base_settings.Save();
        }

        private void Initialize_save_form()
        {
            EventsPlayersAutoColor.Checked = _base_settings.EventsAutoColor;
            DmpCurrentDirectory.Text = _base_settings.EventsDirectoryDF;
            EventsChangeWindow.Checked = _base_settings.EventsChangeWindow;
            WinFirewallActivate.Checked = _base_settings.FirewallActivated;
            EventsControllerCheckPlayers.Checked = _base_settings.EventsControllerCheckPlayers;
            EventsControllerAutoCheckPlayers.Checked = _base_settings.EventsControllerAutoCheckPlayers;
            EventsControllerMaxLimit.Text = _base_settings.EventsControllerMaxLimit;
            EventsControllerBlockedPlayers.Checked = _base_settings.EventsControllerBlockedPlayers;
            EventsWeaponsBlockedPlayers.Checked = _base_settings.EventsWeaponsBlockedPlayers;
            EventsControllerAutoSaveHistory.Checked = _base_settings.EventsControllerAutoSaveHistoryActivate;
            EventsWeaponsMaxLimit.Text = _base_settings.EventsWeaponsMaxLimit;
            EventsBlockedSubnet.Checked = _base_settings.EventsControllerMaxClickLim;
            EventsControllerTrayMessage.Checked = _base_settings.EventsControllerTrayMessage;
            mp_wpn_knife.Checked = _base_settings.SkipScanWeapons1;
            mp_wpn_pm.Checked = _base_settings.SkipScanWeapons2;
            mp_wpn_fort.Checked = _base_settings.SkipScanWeapons3;
            mp_wpn_walther.Checked = _base_settings.SkipScanWeapons4;
            mp_wpn_sig220.Checked = _base_settings.SkipScanWeapons5;
            mp_wpn_usp.Checked = _base_settings.SkipScanWeapons6;
            mp_wpn_desert_eagle.Checked = _base_settings.SkipScanWeapons7;
            mp_wpn_pb.Checked = _base_settings.SkipScanWeapons8;
            mp_wpn_colt1911.Checked = _base_settings.SkipScanWeapons9;
            mp_wpn_bm16.Checked = _base_settings.SkipScanWeapons10;
            mp_wpn_wincheaster1300.Checked = _base_settings.SkipScanWeapons11;
            mp_wpn_spas12.Checked = _base_settings.SkipScanWeapons12;
            mp_wpn_ak74u.Checked = _base_settings.SkipScanWeapons13;
            mp_wpn_ak74.Checked = _base_settings.SkipScanWeapons14;
            mp_wpn_abakan.Checked = _base_settings.SkipScanWeapons15;
            mp_wpn_groza.Checked = _base_settings.SkipScanWeapons16;
            mp_wpn_val.Checked = _base_settings.SkipScanWeapons17;
            mp_wpn_fn2000.Checked = _base_settings.SkipScanWeapons18;
            mp_wpn_mp5.Checked = _base_settings.SkipScanWeapons19;
            mp_wpn_l85.Checked = _base_settings.SkipScanWeapons20;
            mp_wpn_lr300.Checked = _base_settings.SkipScanWeapons21;
            mp_wpn_sig550.Checked = _base_settings.SkipScanWeapons22;
            mp_wpn_g36.Checked = _base_settings.SkipScanWeapons23;
            mp_wpn_vintorez.Checked = _base_settings.SkipScanWeapons24;
            mp_wpn_svu.Checked = _base_settings.SkipScanWeapons25;
            mp_wpn_svd.Checked = _base_settings.SkipScanWeapons26;
            mp_wpn_gauss.Checked = _base_settings.SkipScanWeapons27;
        }

        // 1 - блокируем отрисовку данных, при автоматической обработке таблицы цветов.
        int blocked_events_handler = 0;
        private void BaseMenu2GetListplayers_Click(object sender, EventArgs e)
        {
            if (ThreadListplayers.IsBusy == false)
            {
                START_LEVEL_THREAD = 2;
                if (EventsChangeWindow.CheckState == CheckState.Checked)
                {
                    BaseMenu1.Visible = true;
                    listBase.Visible = true;
                    ListTableChange1.Visible = false;
                    BaseMenu2.Visible = false;
                    EventsBaseSubnetsSearch.Visible = false;
                }
                else // Переключение интерфейса на новый
                {
                    BaseMenu1.Visible = false;
                    EventsBaseSubnetsSearch.Visible = true;
                    listBase.Visible = false;
                    ListTableChange1.Visible = true;
                    BaseMenu2.Visible = true;
                    SelectedTable1.Visible = true;
                    SelectedTable2.Visible = true;
                    ListTableChange2.Visible = true;
                    ListTableChange3.Visible = true;
                    blocked_events_handler = 1;       // Если открыт интерфейс, и пользователь нажимает кнопку, то всегда == true
                }
             
                if (SearchPanel.Visible == true)
                    SearchPanel.Visible = false;

                ThreadListplayers.RunWorkerAsync();
                ThreadListplayers.WorkerSupportsCancellation = true;
                delFromBase.Enabled = false;
                btnExit.Enabled = false;
                StartAutoCheckThread.Enabled = false;
            }
            else
            {
                MessageBox.Show("Обработка уже выполняется, пожалуйста подождите ее завершения.", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Подробный поиск со всеми разделителями
        private void BaseMenu2GetSearch_Click(object sender, EventArgs e)
        {
            if (SearchPanel.Visible == false)
                SearchPanel.Visible = true;
            else
                SearchPanel.Visible = false;
        }

        private void BaseMenu2StatsEvent_Click(object sender, EventArgs e)
        {
            update_stats();
            MessageBox.Show("Всего данных в базе: " + PlayersBaseBuffer.Count + "\nДанных добавленные пользователем: " + UsersAddInBase + "\nДобавлено в автоматическом режиме: " + AutoAddInBase + "\nЗаблокировано всего атак: " + BaseAttackBlock + "\nПеренесено из дампов статистики: " + BaseDumpPlayers + "\nДобавлено как \"Нарушителей: \"" + CheaterList + "\n\nТекущие сведения\nОбнаружно атак: " + server_attack_blocked + "\nПопыток краша при помощи чата: " + server_chat_warning + "\nСобытий сервера: " + SrvEventsBuffer.Count + "\nЗаблокировано \"Нарушителей\" в Авто режиме: " + SIGNAL_BANNED + "\nОшибок выполнения (Read/Write): " + ERROR_COUNTER, "Сведения по базе", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BaseMenu1OpenEvents_Click(object sender, EventArgs e)
        {
            LoadEvents();
        }

        private void BaseMenu2OpenEvents_Click(object sender, EventArgs e)
        {
            LoadEvents();
        }

        public void LoadEvents()
        {
            try
            {
                if (BaseEvents.Count > 0)
                {
                    foreach (Form ColorEvents in Application.OpenForms)
                    {
                        if (ColorEvents.Name == "ColorEvents")
                        {
                            MessageBox.Show("Окно уже открыто!", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }
                    ColorEvents Events = new ColorEvents();
                    Events.Show();
                }
                else
                {
                    MessageBox.Show("В данный момент в буфере событий нет никаких данных, для того чтобы отобразить таблицу. Для того чтобы отобразить данную информацию, функция 'Сохранять историю важных событий лог-файлов' должна быть активна - в настройках базы.", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }       
            catch (Exception ex)
            {
                MessageBox.Show("Reason: " + ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
             

        private void BaseMenu2StartAutoCheck_Click(object sender, EventArgs e)
        {
            StartAutoCheckThread.Checked = true;
        }

        // Блокируем кнопку, если отключена возможность переключать интерфейс
        private void EventsChangeWindow_CheckedChanged(object sender, EventArgs e)
        {
            if (EventsChangeWindow.CheckState == CheckState.Checked)
            {
                EventsPlayersAutoColor.Checked = false;
                EventsPlayersAutoColor.Enabled = false;
            }
            else
            {
                EventsPlayersAutoColor.Enabled = true;
            }
        }

        private void searchAtName_Click(object sender, EventArgs e)
        {
            try
            {
                string SearchToName = (listBase.FocusedItem.SubItems[1].Text);
                listBase.Items.Clear();
                foreach (string s in PlayersBaseBuffer)
                {
                    if (s.Contains(SearchToName))
                    {
                        string[] values = s.Split('%');
                        listBase.Items.Add(new ListViewItem(values));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            delFromBase.Enabled = false;
            if (GetFuncCreateInFirewall.Enabled == true)         
                GetFuncCreateInFirewall.Enabled = false;            
        }

        private void SearchAtAddress_Click(object sender, EventArgs e)
        {
            try
            {
                string SearchToIP = (listBase.FocusedItem.SubItems[4].Text);
                listBase.Items.Clear();
                foreach (string s in PlayersBaseBuffer)
                {
                    if (s.Contains(SearchToIP))
                    {
                        string[] values = s.Split('%');
                        listBase.Items.Add(new ListViewItem(values));
                    }
                    if (s.Contains("[V+]%"))        // Находим данные только по данному фильтру.
                    {
                        if (s.Contains(SearchToIP)) // Находим IP адрес в данном списке, и если он есть то выводим его. В новом цвете
                        {
                            color_pointer_table++;
                            string[] values = s.Split('%');
                            ListTableChange2.Items.Add(new ListViewItem(values)).BackColor = Color.Gold;
                        }
                    }
                }        
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            delFromBase.Enabled = false;
            if (GetFuncCreateInFirewall.Enabled == true)
                GetFuncCreateInFirewall.Enabled = false;
        }

        private void SearchAtHash_Click(object sender, EventArgs e)
        {
            try
            {
                string SearchToName = (listBase.FocusedItem.SubItems[3].Text);
                listBase.Items.Clear();
                foreach (string s in PlayersBaseBuffer)
                {
                    if (s.Contains(SearchToName))
                    {
                        string[] values = s.Split('%');
                        listBase.Items.Add(new ListViewItem(values));
                    }
                }      
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            delFromBase.Enabled = false;
            if (GetFuncCreateInFirewall.Enabled == true)
                GetFuncCreateInFirewall.Enabled = false;

        }
  
        private void SearchPlayersInBase_Click(object sender, EventArgs e)
        {
            try
            {
                string str = listChatHistory.FocusedItem.Text;
                var str_find = str.Split()[3];
                listBase.Items.Clear();
                foreach (string s in PlayersBaseBuffer)
                {
                    if (s.Contains(str_find))
                    {
                        string[] values = s.Split('%');
                        listBase.Items.Add(new ListViewItem(values)).BackColor = Color.DeepSkyBlue;
                    }
                }
                tabControl1.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void AddInBase_Click(object sender, EventArgs e)
        {
            try
            {
                AddInBaseName.Text = listBase.FocusedItem.SubItems[1].Text;
                AddInBaseHash.Text = listBase.FocusedItem.SubItems[3].Text;
                AddInBaseIPAddress.Text = listBase.FocusedItem.SubItems[4].Text;
                tabControl1.SelectedIndex = 2;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void stringCopy_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText("Name: " + listBase.FocusedItem.SubItems[1].Text + "\r\nIP: " + listBase.FocusedItem.SubItems[4].Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Невозможно получить данные из пустой строки. Повторите попытку", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void delFromBase_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Вы действительно хотите удалить выделенные данные из базы?", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    string BaseStrEditor = ("%" + listBase.FocusedItem.SubItems[1].Text + "%" + listBase.FocusedItem.SubItems[2].Text + "%" + listBase.FocusedItem.SubItems[3].Text + "%" + listBase.FocusedItem.SubItems[4].Text + "%" + listBase.FocusedItem.SubItems[5].Text);

                    foreach (string correct_base in PlayersBaseBuffer.ToArray())
                    {
                        if (correct_base.Contains(BaseStrEditor))
                        {
                            PlayersBaseBuffer.Remove(correct_base);
                            break;
                        }
                    }
                    Save(string.Concat(PlayersBaseBuffer));
                    ListViewItem del_str = listBase.FocusedItem;
                    listBase.Items.Remove(del_str);
                    BaseLoadInBuffer();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка. Повторите попытку\nReason:\n" + ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void RemoveSTR_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Вы действительно хотите удалить выделенные данные из базы?", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    string BaseStrEditor = ("%" + listViewBase.FocusedItem.SubItems[1].Text + "%" + listViewBase.FocusedItem.SubItems[2].Text + "%" + listViewBase.FocusedItem.SubItems[3].Text + "%" + listViewBase.FocusedItem.SubItems[4].Text);
                    foreach (string correct_base in PlayersBaseBuffer.ToArray())
                    {
                        if (correct_base.Contains(BaseStrEditor))
                        {
                            PlayersBaseBuffer.Remove(correct_base);
                            break;
                        }
                    }
                    Save(string.Concat(PlayersBaseBuffer));
                    ListViewItem del_str = listViewBase.FocusedItem;
                    listViewBase.Items.Remove(del_str);
                    Thread.Sleep(100);
                    BaseLoadInBuffer();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка. Повторите попытку\nReason:\n" + ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void BlockADDR_Click(object sender, EventArgs e)
        {
            try
            {
                string pname = listViewBase.FocusedItem.SubItems[1].Text;
                string address = listViewBase.FocusedItem.SubItems[4].Text.Replace(Environment.NewLine, string.Empty);
                if ((address == "0.0.0.0") || (address == "255.255.255.255") || (address == "127.0.0.1"))
                {
                    MessageBox.Show("Нельзя заблокировать локальный адрес!", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    BlockedClient(8, pname, " Flags:[null] IP: ", address, "@");
                    tabControl1.SelectedIndex = 6;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void copyBuffer_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText("sv_banplayer_ip " + listBase.FocusedItem.SubItems[4].Text + " 100000");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка. Повторите попытку\nReason:\n" + ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void OpenProc_Click(object sender, EventArgs e)
        {
            try
            {
                string strOpen = listBase.FocusedItem.SubItems[5].Text;
                Process.Start(strOpen);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка. Повторите попытку\nReason:\n" + ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ==================================================
        // Автоматическая обработка данных
        // ==================================================
        string xrMyFileTime;
        string xrRegFileTime;
        int AutoAddInBase = 0;
        int UsersAddInBase = 0;
        int BaseAttackBlock = 0;
        int BaseDumpPlayers = 0;
        int CheaterList = 0;

        private void ScanDataFiles_Tick(object sender, EventArgs e)
        {
            try
            {
                DateTime DataModification = File.GetLastWriteTime(@"server_settings\logs\xray_" + SystemInformation.UserName + ".log");
                xrMyFileTime = (DataModification.ToString("dd.MM.yy HH.mm.ss"));
                if (xrRegFileTime == xrMyFileTime)
                {
                    GUI_STATUS.Text = "Обработка не требуется";
                    GUI_STATUS.BackColor = Color.White;
                }
                else
                {
                    ThreadListplayers.WorkerSupportsCancellation = true;
                    ThreadListplayers.RunWorkerAsync();
                    GUI_MINI_MENU.Enabled = false;
                    GUI_STATUS.Text = "Выполняется обработка...";
                    GUI_STATUS.BackColor = Color.PaleGreen;
                    ScanDataFiles.Stop();
                    try
                    {
                        DateTime xrRegDataTime = File.GetLastWriteTime(@"server_settings\logs\xray_" + SystemInformation.UserName + ".log");
                        xrMyFileTime = (xrRegDataTime.ToString("dd.MM.yy HH.mm.ss"));
                        xrRegFileTime = xrMyFileTime;
                    }
                    catch (Exception)
                    {
                        GUI_STATUS.Text = "Ошибка чтения.";
                        GUI_STATUS.BackColor = Color.Red;
                    }
                }
            }
            catch (Exception)
            {
                GUI_STATUS.Text = "Нет данных.";
                GUI_STATUS.BackColor = Color.Red;
            }
        }

        private void GUI_CLOSE_Click(object sender, EventArgs e)
        {
            StartAutoCheckThread.Checked = false;
            events_load();
        }

        private void update_stats()
        {
            try
            {
                UsersAddInBase = 0;
                AutoAddInBase = 0;
                BaseAttackBlock = 0;
                BaseDumpPlayers = 0;
                CheaterList = 0;
                foreach (string s in PlayersBaseBuffer)
                {
                    if (s.Contains("[R+]%"))             
                        UsersAddInBase++;                 
                    if (s.Contains("[A]%"))                 
                        AutoAddInBase++;                    
                    if (s.Contains("[A+]%"))                 
                       BaseDumpPlayers++;                   
                    if (s.Contains("[IP]%"))                   
                        BaseAttackBlock++;              
                    if (s.Contains("[V+]%"))                 
                        CheaterList++;             
                }
                info_all_base.Text = "Всего данных в базе: " + PlayersBaseBuffer.Count;
                info_auto_base.Text = "Добавлено в автоматическом режиме: " + AutoAddInBase;
                info_user_base.Text = "Добавлено в базу пользователем: " + UsersAddInBase;
                info_dmp_players.Text = "Добавлено игроков из дампов: " + BaseDumpPlayers;
                info_AttackList_base.Text = "Добавлено в базу список атак: " + BaseAttackBlock;
                info_cheatplayers.Text = "Добавлено игроков как Нарушители: " + CheaterList;
                info_srv_events_counter.Text = "Текущих игровых событий: " + SrvEventsBuffer.Count;
                info_chat_events_counter.Text = "Попыток нарушить работу сервера: " + (server_chat_warning + server_attack_blocked);
                info_block_events_counter.Text = "Заблокировано нарушителей в авто-режиме: " + statistics_new_blocked;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при обработки базы.\nCode error:\n" + ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void BaseMenu3_Events_Click(object sender, EventArgs e)
        {
            listEventsSrv.Items.Clear();
            foreach (string MSG in SrvEventsBuffer)
            {
                if (MSG.Contains("logged as remote administrator"))
                {
                    var reg = MSG.Replace("logged as remote administrator", "- Зарегистрирован как Удаленный Администратор").Replace("# User", "").Replace("# Player", "").Replace("with login", "используя логин"); ;
                    listEventsSrv.Items.Add(reg).BackColor = Color.LightGreen;
                }
                else if (MSG.Contains("tried to login as remote administrator. Access denied"))
                {
                    var reg = MSG.Replace("tried to login as remote administrator. Access denied", "- Доступ запрещен").Replace("# User", "").Replace("# Player", "").Replace("with login", "используя логин");
                    listEventsSrv.Items.Add(reg).BackColor = Color.LightPink;
                }
                else if (MSG.Contains("stalkazz_attack"))
                {
                    string str = MSG.Replace("[IP]", "").Replace("-", "").Replace("%", "").Replace("stalkazz_attack", "[ATTACK]: ");
                    listEventsSrv.Items.Add(str).BackColor = Color.LightCoral;
                }
                else if (MSG.Contains("! too large packet size"))
                {
                    listEventsSrv.Items.Add("[ATTACK]: " + MSG).BackColor = Color.Violet;
                }
                else if (MSG.Contains("Disconnecting and Banning:"))
                {
                    var blocked = MSG.Replace("Disconnecting and Banning:", "Отключен и заблокирован:");
                    listEventsSrv.Items.Add(blocked).BackColor = Color.Gold;
                }
                else if (MSG.Contains("Disconnecting and Banning: 0.0.0.0"))
                {
                    var blocked = MSG.Replace("Disconnecting and Banning:", "Отключен и заблокирован:");
                    listEventsSrv.Items.Add(blocked + " [server ip address]").BackColor = Color.Red;
                }
                else if (MSG.Contains("от Заряд ВОГ-25"))
                {
                    listEventsSrv.Items.Add(MSG.Replace("*", "")).BackColor = Color.LightBlue;
                }
                else if (MSG.Contains("M209"))
                {
                    listEventsSrv.Items.Add(MSG.Replace("*", "")).BackColor = Color.LightCyan;
                }
            }
        }

        private void BaseMenu3_EventsWeapon_Click(object sender, EventArgs e)
        {
            listEventsSrv.Items.Clear();
            foreach (string MSG in SrvEventsBuffer)
            {
                if (MSG.Contains("от Заряд ВОГ-25"))
                {
                    listEventsSrv.Items.Add(MSG.Replace("*","")).BackColor = Color.LightBlue;
                }
                else if (MSG.Contains("M209"))
                {
                    listEventsSrv.Items.Add(MSG.Replace("*", "")).BackColor = Color.LightCyan;
                }
            }
        }

        private void BaseMenu3_EventsBlocked_Click(object sender, EventArgs e)
        {
            listEventsSrv.Items.Clear();
            foreach (string Messages in SrvEventsBuffer)
            {
                if (Messages.Contains("Disconnecting and Banning:"))
                {
                    var blocked = Messages.Replace("Disconnecting and Banning:", "Отключен и заблокирован:");
                    listEventsSrv.Items.Add(blocked).BackColor = Color.Gold;
                }
                else if (Messages.Contains("Disconnecting and Banning: 0.0.0.0"))
                {
                    var blocked = Messages.Replace("Disconnecting and Banning:", "Отключен и заблокирован:");
                    listEventsSrv.Items.Add(blocked + " [server ip address]").BackColor = Color.Red;
                }
            }
        }

        private void BaseMenu3_EventsRadmin_Click(object sender, EventArgs e)
        {
            listEventsSrv.Items.Clear();
            foreach (string Messages in SrvEventsBuffer)
            {
                if (Messages.Contains("logged as remote administrator"))
                {
                    var reg = Messages.Replace("logged as remote administrator", "- Зарегистрирован как Удаленный Администратор").Replace("# User", "").Replace("# Player", "").Replace("with login", "используя логин"); ;
                    listEventsSrv.Items.Add(reg).BackColor = Color.LightGreen;
                }
                else if (Messages.Contains("tried to login as remote administrator. Access denied"))
                {
                    var reg = Messages.Replace("tried to login as remote administrator. Access denied", "- Доступ запрещен").Replace("# User", "").Replace("# Player", "").Replace("with login", "используя логин");
                    listEventsSrv.Items.Add(reg).BackColor = Color.LightPink;
                }
            }
        }

        private void BaseMenu3_EventsAttack_Click(object sender, EventArgs e)
        {
            listEventsSrv.Items.Clear();
            foreach (string MSG in SrvEventsBuffer)
            {
                if (MSG.Contains("stalkazz_attack"))
                {
                    string str = MSG.Replace("[IP]", "").Replace("-", "").Replace("%", "").Replace("stalkazz_attack", "[ATTACK]: ");
                    listEventsSrv.Items.Add(str).BackColor = Color.LightCoral;
                }
                else if (MSG.Contains("! too large packet size"))
                {
                    listEventsSrv.Items.Add("[!ATTACK]:" + MSG).BackColor = Color.Violet;
                }
            }
        }
                  
        private void btnConnectionBase_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ConnectionBase = new OpenFileDialog();
                ConnectionBase.DefaultExt = "*.xrBase";
                ConnectionBase.Filter = "Файл баз данных|*.xrBase";
                if (ConnectionBase.ShowDialog() == DialogResult.OK && ConnectionBase.FileName.Length > 0)
                {
                    int count = 0;
                    foreach (string add in File.ReadLines(ConnectionBase.FileName))
                    {
                        count++;
                    }
                    DialogResult dialogResult = MessageBox.Show("Выбранная Вами база содержит ["+count+"] игроков. Продолжить выполнение?", "Объединение базы", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (dialogResult == DialogResult.Yes)
                    {
                        foreach (string add in File.ReadLines(ConnectionBase.FileName))
                        {
                            PlayersBaseBuffer.Add(add+Environment.NewLine);
                        }
                        Save(string.Concat(PlayersBaseBuffer));
                        BaseLoadInBuffer();
                        update_stats();
                    }              
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connecting Base Error\nReason:\n" + ex.Message, "S.E.R.V.E.R - Shadow of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBaseCopy_Click(object sender, EventArgs e)
        {
            try
            {
                File.Copy(@"PlayersDataBase\Players.xrBase", @"PlayersDataBase\Backup\BackupBase.xrBase", true);
                MessageBox.Show("Копия базы успешно создана!\nPlayersDataBase/Backup/BackupBase.xrBase", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBaseRecovery_Click(object sender, EventArgs e)
        {
            DialogResult Result = MessageBox.Show("Вы уверены что хотите восстановить базу из сделанной ранее копии?\nПри продолжений - База полностью перезапишется.\nЖелаете продолжить?", "Restore Base", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (Result == DialogResult.Yes)
            {
                try
                {
                    File.Copy(@"PlayersDataBase\Backup\BackupBase.xrBase", @"PlayersDataBase\Players.xrBase", true);
                    BaseLoadInBuffer();
                    MessageBox.Show("База успешно восстановлена.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    update_stats();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (Result == DialogResult.No)
            {
                MessageBox.Show("Действие было отменено.", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        HashSet<string> export_base = new HashSet<string>();
        private void btnExportCheaterBase_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog ExportBase = new SaveFileDialog();
                ExportBase.DefaultExt = "*.xrBase";
                ExportBase.Filter = "Файл баз данных|*.xrBase";
                if (ExportBase.ShowDialog() == DialogResult.OK && ExportBase.FileName.Length > 0)
                {
                    foreach (string export in File.ReadLines(@"PlayersDataBase\Players.xrBase"))
                    {
                        if (export.Contains("[V+]"))
                            export_base.Add(export + Environment.NewLine);                   
                    }
                    File.WriteAllText(ExportBase.FileName, string.Concat(export_base));
                    export_base.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ServerBasePlayers_KeyDown(object sender, KeyEventArgs e)
        {
            if (StartAutoCheckThread.CheckState == CheckState.Unchecked)
            {
                if (e.KeyCode == Keys.F1)
                    searchAtName.PerformClick();

                else if (e.KeyCode == Keys.F2)
                    SearchAtAddress.PerformClick();

                else if (e.KeyCode == Keys.F3)
                    SearchAtHash.PerformClick();

                else if (e.KeyCode == Keys.F4)
                    stringCopy.PerformClick();

                else if (e.KeyCode == Keys.F5)
                    copyBuffer.PerformClick();

                else if (e.KeyCode == Keys.F6)
                    OpenProc.PerformClick();

                else if (e.KeyCode == Keys.F7)
                    AddInBase.PerformClick();

                else if (e.KeyCode == Keys.F8)
                    btnInvokeClearEvents.PerformClick();

                else if (e.KeyCode == Keys.F9)
                    btnSaveDialog.PerformClick();

                else if (e.KeyCode == Keys.F10)
                    SearchPlayersInBase.PerformClick();

                else if (e.KeyCode == Keys.F11)
                    NewWindowStrCopy.PerformClick();

                else if (e.KeyCode == Keys.F12)
                    NewWindowBanInfo.PerformClick();

                else if (e.KeyCode == Keys.Delete)
                    delFromBase.PerformClick();

                if (SearchPanel.Visible == true)
                { 
                    if (e.KeyCode == Keys.Enter)
                        index_search.PerformClick();
                }
            }
        }

        private void btnReadingBaseType1_Click(object sender, EventArgs e)
        {
            listViewBase.Items.Clear();
            foreach (string s in PlayersBaseBuffer)
            { 
                if (s.Contains("[R+]%"))
                {
                    string[] values = s.Split('%');
                    listViewBase.Items.Add(new ListViewItem(values)).BackColor = Color.PaleGreen;
                }    
            }
        }

        private void btnReadingBaseType2_Click(object sender, EventArgs e)
        {
            listViewBase.Items.Clear();
            foreach (string s in PlayersBaseBuffer)
            {
                if (s.Contains("[IP]%"))
                {
                    string[] values = s.Split('%');
                    listViewBase.Items.Add(new ListViewItem(values)).BackColor = Color.LightCoral;
                }
            }
        }

        private void btnReadingBaseType3_Click(object sender, EventArgs e)
        {
            listViewBase.Items.Clear();
            foreach (string s in PlayersBaseBuffer)
            {
                if (s.Contains("[CHEATER]"))
                {
                    string[] values = s.Split('%');
                    listViewBase.Items.Add(new ListViewItem(values)).BackColor = Color.Gold;
                }
            }
        }

        private void btnAddToBase_Click(object sender, EventArgs e)
        {
            try
            {
                if (AddInBaseName.TextLength == 0 || AddInBaseReason.Text.Length == 0 || AddInBaseIPAddress.TextLength == 0)
                {
                    MessageBox.Show("Данные заполнены не полностью.\nДобавление в базу невозможно.", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    AddInBaseName.Text = AddInBaseName.Text.Replace("%", "_");
                    AddInBaseHash.Text = AddInBaseHash.Text.Replace("%", "_");
                    AddInBaseIPAddress.Text = AddInBaseIPAddress.Text.Replace("%", "_");
                    AddInBaseLink.Text = AddInBaseLink.Text.Replace("%", "_");
                    AddInBaseReason.Text = AddInBaseReason.Text.Replace("%", "_");

                    if (FilterReasonCheater.CheckState == CheckState.Checked)
                    {
                        PlayersBaseBuffer.Add("[V+]%" + AddInBaseName.Text + "%[CHEATER]: " + AddInBaseReason.Text + "%" + AddInBaseHash.Text + "%" + AddInBaseIPAddress.Text + "%" + AddInBaseLink.Text + "%" + DateTime.Now.ToString("dd.MM.yyyy") + Environment.NewLine);
                        string[] values = ("[V+]%" + AddInBaseName.Text + "%[CHEATER]: " + AddInBaseReason.Text + "%" + AddInBaseHash.Text + "%" + AddInBaseIPAddress.Text + "%" + AddInBaseLink.Text + "%" + DateTime.Now.ToString("dd.MM.yyyy")).Split('%');
                        listViewBase.Items.Add(new ListViewItem(values)).BackColor = Color.Gold;
                    }
                    if (FilterReasonNote.CheckState == CheckState.Checked)
                    {
                        PlayersBaseBuffer.Add("[N+]%" + AddInBaseName.Text + "%[NOTES]: " + AddInBaseReason.Text + "%" + AddInBaseHash.Text + "%" + AddInBaseIPAddress.Text + "%" + AddInBaseLink.Text + "%" + DateTime.Now.ToString("dd.MM.yyyy") + Environment.NewLine);
                        string[] values = ("[N+]%" + AddInBaseName.Text + "%[NOTES]: " + AddInBaseReason.Text + "%" + AddInBaseHash.Text + "%" + AddInBaseIPAddress.Text + "%" + AddInBaseLink.Text + "%" + DateTime.Now.ToString("dd.MM.yyyy")).Split('%');
                        listViewBase.Items.Add(new ListViewItem(values)).BackColor = Color.LightGreen;
                    }
                    if (FilterReasonCheater.Checked == false && FilterReasonNote.Checked == false)
                    {
                        PlayersBaseBuffer.Add("[R+]%" + AddInBaseName.Text + "%" + AddInBaseReason.Text + "%" + AddInBaseHash.Text + "%" + AddInBaseIPAddress.Text + "%" + AddInBaseLink.Text + "%" + DateTime.Now.ToString("dd.MM.yyyy") + Environment.NewLine);
                        string[] values = ("[R+]%" + AddInBaseName.Text + "%" + AddInBaseReason.Text + "%" + AddInBaseHash.Text + "%" + AddInBaseIPAddress.Text + "%" + AddInBaseLink.Text + "%" + DateTime.Now.ToString("dd.MM.yyyy")).Split('%');
                        listViewBase.Items.Add(new ListViewItem(values)).BackColor = Color.Honeydew;
                    }
                    Save(string.Concat(PlayersBaseBuffer));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Повторите попытку. В выделенной строке нет требуемых данных:\n" + ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            foreach (string s in PlayersBaseBuffer)
            {
                string[] values = s.Split('%');
                if (s.Contains("[V+]"))              
                    listViewBase.Items.Add(new ListViewItem(values)).BackColor = Color.Gold;
                else if (s.Contains("[N+]"))
                    listViewBase.Items.Add(new ListViewItem(values)).BackColor = Color.LightGreen;
            }
        }
                

        private void events_load()
        {
            listEventsSrv.Items.Clear();
            listChatHistory.Items.Clear();
            foreach (string Messages in SrvEventsBuffer)
            {
                if (Messages.Contains("logged as remote administrator"))     
                {
                    var reg = Messages.Replace("logged as remote administrator", "- Зарегистрирован как Удаленный Администратор").Replace("# User", "").Replace("# Player", "").Replace("with login", "используя логин"); ;
                    listEventsSrv.Items.Add(reg).BackColor = Color.LightGreen;
                }

                if (Messages.Contains("tried to login as remote administrator. Access denied"))
                {
                    var reg = Messages.Replace("tried to login as remote administrator. Access denied", "- Доступ запрещен").Replace("# User", "").Replace("# Player", "").Replace("with login", "используя логин");
                    listEventsSrv.Items.Add(reg).BackColor = Color.LightPink;    
                }

                if (Messages.Contains("! too large packet size"))
                {
                    listEventsSrv.Items.Add(Messages).BackColor = Color.LightBlue;
                }

                if (Messages.Contains("Disconnecting and Banning:")) 
                {
                    var blocked = Messages.Replace("Disconnecting and Banning:", "Отключен и заблокирован: ");
                    listEventsSrv.Items.Add(blocked).BackColor = Color.Gold;
                }
                else if (Messages.Contains("Disconnecting and Banning: 0.0.0.0"))
                {
                    var blocked = Messages.Replace("Disconnecting and Banning:", "Отключен и заблокирован: ");
                    listEventsSrv.Items.Add(blocked + " [server ip address]").BackColor = Color.Red;
                }

                if (Messages.Contains("stalkazz_attack"))
                {
                    if (Messages.Contains("stalkazz_attack"))
                    {
                        string str = Messages.Replace("[IP]", "").Replace("-", "").Replace("%", "").Replace("stalkazz_attack", "[ATTACK]: ");
                        listEventsSrv.Items.Add(str).BackColor = Color.LightCoral;
                    }
                    if (Messages.Contains("! too large packet size"))
                    {
                        listEventsSrv.Items.Add("[!ATTACK]:" + Messages).BackColor = Color.Violet;
                    }
                }

                try
                {
                    if (Messages.Contains("Чат:"))
                    {
                        var color_msg = Messages.Split()[0];
                        var check_msg = Messages.Split()[1];
                        if (color_msg + " " + check_msg == "Чат: ServerAdmin")
                        {
                            var admin = Messages.Replace("Чат: ServerAdmin", "[Администратор]: ");
                            listChatHistory.Items.Add(admin).BackColor = Color.Gold;
                        }
                        else if (color_msg + " " + check_msg == "- Чат:")
                        {
                            var green = Messages.Replace("- Чат:", "[Свобода]: ");            
                                listChatHistory.Items.Add(green).BackColor = Color.PaleGreen;                     
                        }
                        else if (color_msg + " " + check_msg == "~ Чат:") 
                        {
                            var blue = Messages.Replace("~ Чат:", "[Наемники]: ");                     
                                listChatHistory.Items.Add(blue).BackColor = Color.LightBlue;                  
                        }
                        else if ((color_msg + " " + check_msg.Length == "Чат: " + check_msg.Length) && (check_msg != "ServerAdmin") && (check_msg != "SERVER"))
                        {
                            var all_msg = Messages.Replace("Чат: ", "[Общий чат]: ");                    
                                listChatHistory.Items.Add(all_msg).BackColor = Color.Honeydew;                    
                        }
                        else if ((color_msg + " " + check_msg.Length == "Чат: " + check_msg.Length) && (check_msg == "SERVER"))
                        {
                            var all_msg = Messages.Replace("Чат: ", "[Событие]: ");
                                listChatHistory.Items.Add(all_msg).BackColor = Color.WhiteSmoke;
                        }
                    }
                }
                catch(Exception ex)
                {
                    listChatHistory.Items.Add("[CHAT]: " + ex.Message).BackColor = Color.Blue;
                }
            }
        }

        private void btnInvokeClearEvents_Click(object sender, EventArgs e)
        {
            SrvEventsBuffer.Clear();
            listChatHistory.Items.Clear();
            listEventsSrv.Items.Clear();
        }

        private void btnSaveDialog_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog ChatMSG = new SaveFileDialog();
                ChatMSG.DefaultExt = "*.txt";
                ChatMSG.Filter = "Текстовый файл|*.txt";         // "Текстовый файл (*.txt)|*.txt|Все файлы(*.*)|*.*"
                if (ChatMSG.ShowDialog() == DialogResult.OK && ChatMSG.FileName.Length > 0)
                {
                    File.WriteAllLines(ChatMSG.FileName, SrvEventsBuffer);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void DYNAMIC_SET_COLOR()
        {
            try
            {
                int color = SendMessage.SendMsg;
                var color_rechange = Color.FromArgb(color);
                if (color == 0)
                {
                    GUIEvents.BackColor = Color.FromArgb(192, 192, 255);
                    GUI_MINI_MENU.BackColor = Color.FromArgb(192, 192, 255);
                    btnMinimized.BackColor = Color.FromArgb(192, 192, 255);
                    btnExit.BackColor = Color.FromArgb(192, 192, 255);
                }
                else
                {
                    GUIEvents.BackColor = color_rechange;
                    GUI_MINI_MENU.BackColor = color_rechange;
                    btnMinimized.BackColor = color_rechange;
                    btnExit.BackColor = color_rechange;
                }
            }
            catch (Exception ex)
            {
                GUIEvents.BackColor = Color.FromArgb(192, 192, 255);
                GUI_MINI_MENU.BackColor = Color.FromArgb(192, 192, 255);
                btnMinimized.BackColor = Color.FromArgb(192, 192, 255);
                btnExit.BackColor = Color.FromArgb(192, 192, 255);
                MessageBox.Show("Command init set color error\n"+ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

    
        // Полностью переделать перенос из дампов статистики. Без таймера и в ручной режим
        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!



        // EDIT 2017 
        // ==================================================
        // Добавление данных из дампов статистики                           доработать в нормальный вид
        // ======================================================================================================================================================
        private static void DumpPlayersWrite(string AddPlayersInBase)
        {
            StreamWriter writer = new StreamWriter(@"PlayersDataBase\base_temp.txt", true, Encoding.GetEncoding("UTF-8")); //  windows-1251
            writer.Write(AddPlayersInBase);
            writer.Close();
        }
        /*
        // ==================================================
        int Timer = 3600; // 1 hour
        int TimerProcessFinished = 0;
        int TimerProcessError = 0;
        string pos1;
        string pos2;
        string del1;
        string del2;
        string symbol_1;
        string symbol_2;
        string symbol_3;
        string symbol_4;
        string symbol_5;
        string symbol_6;
        string dump_statistics_buffer;*/

        private void CreateDirectoryDumpFiles_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog DumpFilesDirectory = new FolderBrowserDialog();
            if (DumpFilesDirectory.ShowDialog() == DialogResult.OK)
            {
                DmpCurrentDirectory.Text = DumpFilesDirectory.SelectedPath;
            }
        }
        HashSet<string> dump = new HashSet<string>();

        private void btnConnectAllFiles_Click(object sender, EventArgs e)
        {
           /*
            if (DmpCurrentDirectory.Text != "")
            {
                try
                {
                    btnConnectAllFiles.Enabled = false;
                    PlayersInDump.Text = "Выполняется обработка...";
                    foreach (string ReadAllFiles in Directory.EnumerateFiles(DmpCurrentDirectory.Text, "*.ltx", SearchOption.AllDirectories))
                    {
                        foreach (string cmp in File.ReadLines(ReadAllFiles, Encoding.GetEncoding(1251)))
                        {
                            if (cmp.Contains("player_ip"))
                            {
                                del1 = cmp.Replace(" ", string.Empty);
                                symbol_1 = del1.Replace("=", "");
                                symbol_2 = symbol_1.Replace("-", "");
                                symbol_3 = symbol_2.Replace("%", "");
                                pos1 = symbol_3.Replace("player_ip", "");
                            }
                            if (cmp.Contains("player_name"))
                            {
                                del2 = cmp.Replace(" ", string.Empty);
                                symbol_4 = del2.Replace("=", "");
                                symbol_5 = symbol_4.Replace("-", "");
                                symbol_6 = symbol_5.Replace("%", "");
                                pos2 = symbol_6.Replace("player_name", "");
                                dump_statistics_buffer += ("[A+]%" + pos2 + "%ID:<Unknown>%hash:<Unknown>%" + pos1 + "%%" + Environment.NewLine);
                            }
                        }
                    }

                    File.WriteAllText("1.txt", dump_statistics_buffer);

                    /*
                    using (var READING_DUMP = new FileStream(@"PlayersDataBase\dump_files.txt", FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        foreach (string ReadAllFiles in Directory.EnumerateFiles(DmpCurrentDirectory.Text, "*.ltx", SearchOption.AllDirectories))
                        {
                            using (var WriteAllFiles = File.OpenRead(ReadAllFiles))
                            {
                                WriteAllFiles.CopyTo(READING_DUMP);
                            }
                            READING_DUMP.WriteByte(Convert.ToByte('\r')); // 13 \r https://stackoverflow.com/questions/2915785/how-many-bytes-is-n-r  
                            READING_DUMP.WriteByte(Convert.ToByte('\n')); // 10 \n In ASCII encoding, \n is the Newline character 0x0A (decimal 10), \r is the Carriage Return character 0x0D (decimal 13).        
                        }
                    } 
                }
                catch (Exception)
                {
                    TimerProcessError++;
                }
                ThreadDumpPlayers.WorkerSupportsCancellation = true;
                ThreadDumpPlayers.RunWorkerAsync();
            }
            else
            {
                if (EventsAutoCollectDumpStats.CheckState == CheckState.Checked)
                {
                    TimerDumpFiles.Stop();
                    info_dump_stats.Text = "Внимание! Работа функции остановлена в связи с отсутствием или не назначенной вами директории...";
                    info_dump_stats.ForeColor = Color.Red;
                }
                else
                {
                    MessageBox.Show("Пожалуйста установите директорию нахождения Dump Files!\nКонечная папка Online", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }*/
        }


        private void ThreadDumpPlayers_DoWork(object sender, DoWorkEventArgs e)
        {
            /*
            PlayersInDump.Text = "Обработка данных...\nПожалуйста подождите...";
            try
            {
                string[] readText = File.ReadAllLines(@"PlayersDataBase\dump_files.txt", Encoding.GetEncoding(1251));
                foreach (string s in readText)
                {
                    if (s.Contains("player_ip"))
                    {
                        del1 = s.Replace(" ", string.Empty);
                        symbol_1 = del1.Replace("=", "");
                        symbol_2 = symbol_1.Replace("-", "");
                        symbol_3 = symbol_2.Replace("%", "");
                        pos1 = symbol_3.Replace("player_ip", "");
                    }
                    if (s.Contains("player_name"))
                    {
                        del2 = s.Replace(" ", string.Empty);
                        symbol_4 = del2.Replace("=", "");
                        symbol_5 = symbol_4.Replace("-", "");
                        symbol_6 = symbol_5.Replace("%", "");
                        pos2 = symbol_6.Replace("player_name", "");
                        dump_statistics_buffer += ("[A+]%" + pos2 + "%ID:<Unknown>%hash:<Unknown>%" + pos1 + "%%" + Environment.NewLine);                       
                    }
                }
                DumpPlayersWrite(dump_statistics_buffer);
            }
            catch (Exception)
            {
                TimerProcessError++;
            }
            PlayersInDump.Text = "Формирование таблицы...\nПожалуйста подождите...";
            Thread.Sleep(1000);
            dump_statistics_buffer = null;*/
        }

        private void ThreadDumpPlayers_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            /*
            try
            {
                // =========================================================== BASE PLAYERS FORMAT - НОВАЯ ВЕРСИЯ ОБРАБОТКИ ===========================================================
                // Обработанные новые данные записываются в файл base_temp.txt 
                // Объединение НОВОЙ БАЗЫ и СТАРОЙ БАЗЫ ВМЕСТЕ ВО ВРЕМЕННЫЙ ФАЙЛ
                string BaseOut = string.Empty;
                using (StreamReader BaseLoad = new StreamReader(@"PlayersDataBase\Players.xrBase", Encoding.GetEncoding("UTF-8"))) // 1251
                {
                    BaseOut = BaseLoad.ReadToEnd();
                }
                ResultStatusScanCollect(BaseOut); // Players.xrBase => base_temp.txt  
                // Finish
                // Обработанная данные вместе с базой                   // Входящие данные  (UTF-8)                                          // 1251                                      // Удаляем всю строку, если она начинается с данных слов                                                                               // Удаляем все пробелы
                File.WriteAllLines((@"PlayersDataBase\Players.xrBase"), File.ReadLines(@"PlayersDataBase\base_temp.txt", Encoding.GetEncoding("UTF-8")).Distinct().Where(x => !x.Contains("[A]%<Unknown>")).Where(x => !x.Contains("[A]%%<Unknown>")).Where(x => !x.Contains("[A]%%ID:")).Where(x => !x.Contains("[A]%%%hash:")).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray());
                File.WriteAllText(@"PlayersDataBase\base_temp.txt", string.Empty);
                File.WriteAllText(@"PlayersDataBase\dump_files.txt", string.Empty); // clear  
                // =========================================================== BASE PLAYERS FORMAT - НОВАЯ ВЕРСИЯ ОБРАБОТКИ ===========================================================
            }
            catch (Exception ex)
            {
                if (EventsAutoCollectDumpStats.CheckState == CheckState.Checked)
                {
                    TimerDumpFiles.Stop();
                    PlayersInDump.Text = "Обработка завершена с ошибкой.";
                    info_dump_stats.Text = "Внимание! Работа функции остановлена в связи с ошибкой: " + ex.Message;
                    info_dump_stats.ForeColor = Color.Red;
                }
                else
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            PlayersInDump.Text = "Обработка успешно завершена!";
            btnConnectAllFiles.Enabled = true;
            Thread.Sleep(100);
            BaseLoadInBuffer();             // вызывать всегда и везде, при каких либо действий с базой. */
        }

        private void baseServerLoad_Click(object sender, EventArgs e)
        {
            listBase.Items.Clear();
            foreach (string s in PlayersBaseBuffer)
            {
                if (s.Contains("[A]%"))
                {
                    string[] values = s.Split('%');
                    listBase.Items.Add(new ListViewItem(values));
                }
            }
            if (GetFuncCreateInFirewall.Enabled == true)         
                GetFuncCreateInFirewall.Enabled = false;           
        }

        private void baseRadminLoad_Click(object sender, EventArgs e)
        {
            listBase.Items.Clear();
            foreach (string s in PlayersBaseBuffer)
            {
                if (s.Contains("[R+]%"))
                { 
                    string[] values = s.Split('%');
                    listBase.Items.Add(new ListViewItem(values));
                }
            }
            if (GetFuncCreateInFirewall.Enabled == true)         
                GetFuncCreateInFirewall.Enabled = false;          
        }

        private void baseDumpLoad_Click(object sender, EventArgs e)
        {
            listBase.Items.Clear();
            foreach (string s in PlayersBaseBuffer)
            {
                if (s.Contains("[A+]%"))
                {
                    string[] values = s.Split('%');
                    listBase.Items.Add(new ListViewItem(values));
                }
            }
            if (GetFuncCreateInFirewall.Enabled == true)            
                GetFuncCreateInFirewall.Enabled = false;          
        }

        private void GetCheaterBase_Click(object sender, EventArgs e)
        {
            listBase.Items.Clear();
            foreach (string s in PlayersBaseBuffer)
            {
                if (s.Contains("[V+]%"))
                {
                    string[] values = s.Split('%');
                    listBase.Items.Add(new ListViewItem(values));
                }
            }
            if (WinFirewallActivate.CheckState == CheckState.Checked)          
               GetFuncCreateInFirewall.Enabled = true;           
        }

        private void EventsAutoCollectDumpStats_CheckedChanged(object sender, EventArgs e)
        {
            /*
            if (EventsAutoCollectDumpStats.CheckState == CheckState.Checked)
            {
                TimerDumpFiles.Start();
                StartAutoCheckThread.Enabled = false;
                info_dump_stats.Text = "Пожалуйста подождите...";
                info_dump_stats.Visible = true;
                info_dump_stats.ForeColor = Color.Blue;
                Timer = 3600;
                TimerProcessFinished = 0;
                TimerProcessError = 0;
                btnMinimized.Visible = true;
                EventsAutoCollectDumpStats.BackColor = Color.Lime;
            }
            else
            {
                TimerDumpFiles.Stop();
                StartAutoCheckThread.Enabled = true;
                info_dump_stats.Visible = false;
                btnMinimized.Visible = false;
                EventsAutoCollectDumpStats.BackColor = Color.Orange;
            }*/
        }

        private void TimerDumpFiles_Tick(object sender, EventArgs e)
        {/*
            Timer -= 30;
            if (Timer == 0)
            {
                Timer = 3600;
                btnConnectAllFiles.PerformClick();
                TimerProcessFinished++;
            }
            else
            {
                info_dump_stats.Text = "Обработка выполнится через: [" + TimeSpan.FromSeconds(Timer).ToString(@"hh\:mm\:ss") + "]" + " Выполнено успешно: [" + TimerProcessFinished + "]" + " Ошибок выполнения: [" + TimerProcessError + "]";
            }
            if (TimerProcessError >= 5)
            {
                TimerDumpFiles.Stop();
                info_dump_stats.Text = "Внимание! Работа функции остановлена в связи с превышением лимита ошибок: [" + TimerProcessError + "]";
                info_dump_stats.ForeColor = Color.Violet;
            }*/
        }


        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!


        // обновляем данные на новый интерфейс
        private void ListPage2CheckUpdate_Click(object sender, EventArgs e)
        {
            if (ThreadListplayers.IsBusy == true)
            {
                MessageBox.Show("Обработка уже выполняется, пожалуйста подождите ее завершения.", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                START_LEVEL_THREAD = 2;
                PlayersCheaterList.Items.Clear();
                blocked_events_handler = 1;                                        // Блокировка отрисовки данных - для автоматической подсветки
                ThreadListplayers.RunWorkerAsync();
            }
        }
        // Возращаем все параметры интерфейса назад
        private void ListPage2ExitMenu_Click(object sender, EventArgs e)
        {
            BaseMenu1.Visible = true;
            listBase.Visible = true;
            ListTableChange1.Visible = false;
            BaseMenu2.Visible = false;
            SelectedTable1.Visible = false;
            SelectedTable2.Visible = false;
            ListTableChange2.Visible = false;
            ListTableChange3.Visible = false;
            EventsBaseSubnetsSearch.Visible = false;
        }


        // Обрабатываем выделенную строку в новом интерфейсе
        string Table1NamePlayers;
        string Table1IPAddress;
        string Table1Hash;
        string IndexStr;

        int color_pointer_table;                     // Записан в базе как читер
        int color_pointer_chat_msg;                  // Есть история чата
        int color_pointer_attack_found;              // Найдены атаки
        int color_pointer_server_found;              // server
        int color_pointer_chat_size;                 // Предупреждение о большой длине чата
        int color_pointer_radmin_success;            // Если игрок зашел как радмин, то подсвечиваем его
        int hweapon_using;                           // Если игрок использовал подствол
        // проверка на читерство 
        int color_pointer_create_in_cheaterlist;     // Добавим читера в лист
        int color_pointer_cheat_maxfire_counter;     // Счетчик общего значения количества выстрелов

        private void ListTableChange1_Click(object sender, EventArgs e)
        {
            SCAN_PLAYERS_IN_BASE();
        }

        private void SCAN_PLAYERS_IN_BASE()
        {
            try
            {
                color_pointer_table = 0;             // подсветить указанную строку в ListTableChange1
                color_pointer_chat_msg = 0;          // подсветка игроков, у которых есть история чата
                color_pointer_attack_found = 0;      // Найдены атаки
                color_pointer_server_found = 0;      // server
                color_pointer_chat_size = 0;         // Предупреждение о большой длине чата
                color_pointer_radmin_success = 0;    // Залогинился администратором
                color_pointer_create_in_cheaterlist = 0;
                color_pointer_cheat_maxfire_counter = 0;    // Счетчик завышенных данных
                hweapon_using = 0;

                ListTableChange2.Items.Clear();
                ListTableChange3.Items.Clear();
                ListTableChange1.FocusedItem.ForeColor = Color.Black;
                Table1NamePlayers = ListTableChange1.FocusedItem.SubItems[1].Text;     // Name
                Table1Hash = ListTableChange1.FocusedItem.SubItems[3].Text;            // Hash
                Table1IPAddress = ListTableChange1.FocusedItem.SubItems[4].Text;       // IP

                if (!(Table1IPAddress.Length >= 7 && Table1IPAddress.Length <= 15))    // check format address
                {
                    ListTableChange1.FocusedItem.BackColor = Color.Coral;
                    SelectedTable1.Text = "Player: " + Table1NamePlayers + " Bad Address Format => " + Table1IPAddress;
                    SelectedTable1.BackColor = Color.Coral;
                    return;
                }

                if (blocked_events_handler == 0)                                       // Блокировка отрисовки данных - для автоматической подсветки
                {
                    SelectedTable1.BackColor = Color.Honeydew;
                    SelectedTable1.ForeColor = Color.Black;
                    SelectedTable1.Text = "История игрока (Из внутренней статистики программы)";

                    if (EventsBaseSubnetsSearch.CheckState == CheckState.Checked)      // Искать по 3 блокам
                    {
                        int SelectIndex = Table1IPAddress.IndexOf('.');
                        if (SelectIndex >= 0)
                        {
                            IndexStr = Table1IPAddress.Substring(0, SelectIndex + 7);  // Искать по 3 блокам ip
                        }
                        foreach (string s in PlayersBaseBuffer)
                        {
                            if (s.Contains(IndexStr))
                            {
                                string[] values = s.Split('%');
                                ListTableChange3.Items.Add(new ListViewItem(values));
                            }
                        }
                    }
                    else
                    {
                        foreach (string s in PlayersBaseBuffer)                        // Искать адрес целиком
                        {
                            if (s.Contains("[V+]%"))                                   // Находим данные только по данному фильтру.
                            {
                                if (s.Contains(Table1IPAddress))                       // Находим IP адрес в данном списке, и если он есть то выводим его. В новом цвете
                                {
                                    color_pointer_table = 1;                           // cheater flag
                                    string CreateFilter = s.Replace("[V+]", "Z+");     // Меняем фильтр, для обратной сортировки
                                    string[] values = CreateFilter.Split('%');
                                    ListTableChange3.Items.Add(new ListViewItem(values)).BackColor = Color.Gold;
                                }
                            }
                            else if (s.Contains(Table1IPAddress))
                            {
                                string[] values = s.Split('%');
                                ListTableChange3.Items.Add(new ListViewItem(values));
                            }
                        }
                    }
                }

                // Находим по фильтру, и если он читер то подсвечиваем его.       
                foreach (string s in PlayersBaseBuffer)
                {                
                    if (s.Contains("[V+]%"))                                   // Находим данные только по данному фильтру.
                    {
                        if (s.Contains(Table1IPAddress))                       // Находим IP адрес в данном списке, и если он есть то выводим его. В новом цвете
                        {
                            if (blocked_events_handler == 1)                   // Блокировка отрисовки данных - для автоматической подсветки
                            {
                                color_pointer_table = 1;                       // cheater flag
                                break;
                            }
                        }
                    }
                }


                foreach (string str in SrvEventsBuffer)
                {                
                    // blue msg
                    if (str.Contains("~ Чат: " + Table1NamePlayers))
                    {
                        if (str.Length >= 150)
                        {
                            color_pointer_chat_size = 1;
                            string blue = str.Replace("~ Чат: ", "[Наемники] Size Chat [" + str.Length + "]: ");
                            var correct_msg = blue.Split()[4];    // Считываем 3 слово после пробелов
                            if (correct_msg == Table1NamePlayers) // Проверяем точность имени игрока                            
                                ListTableChange2.Items.Add(blue).BackColor = Color.LightCoral;                            
                        }
                        else
                        {
                            color_pointer_chat_msg = 1;
                            if (blocked_events_handler == 0)
                            {
                                string blue = str.Replace("~ Чат: ", "[Наемники]: ");
                                var correct_msg = blue.Split()[1];
                                if (correct_msg == Table1NamePlayers)                           
                                    ListTableChange2.Items.Add(blue).BackColor = Color.LightBlue;                            
                            }
                        }
                    }

                    // green msg
                    if (str.Contains("- Чат: " + Table1NamePlayers))
                    {
                        if (str.Length >= 150)
                        {
                            color_pointer_chat_size = 1;
                            string green = str.Replace("- Чат: ", "[Свобода] Size Chat [" + str.Length + "]: ");
                            var correct_msg = green.Split()[4];
                            if (correct_msg == Table1NamePlayers)                            
                                ListTableChange2.Items.Add(green).BackColor = Color.LightCoral;                         
                        }
                        else
                        {
                            color_pointer_chat_msg = 1;
                            if (blocked_events_handler == 0)
                            {
                                string green = str.Replace("- Чат: ", "[Свобода]: ");
                                var correct_msg = green.Split()[1];
                                if (correct_msg == Table1NamePlayers)                               
                                    ListTableChange2.Items.Add(green).BackColor = Color.LightGreen;                                
                            }
                        }
                    }

                    // all chat msg
                    if (str.Contains("Чат: " + Table1NamePlayers))
                    {
                        if (str.Length >= 200)
                        {
                            color_pointer_chat_size = 1;
                            string chat = str.Replace("Чат: ", "[Общий чат] Size Chat [" + str.Length + "]: ");
                            var correct_msg = chat.Split()[5];
                            if (correct_msg == Table1NamePlayers)                            
                                ListTableChange2.Items.Add(chat).BackColor = Color.LightCoral;                       
                        }
                        else
                        {
                            color_pointer_chat_msg = 1;
                            if (blocked_events_handler == 0)
                            {
                                string chat = str.Replace("Чат: ", "[Общий чат]: ");
                                var correct_msg = chat.Split()[2];
                                if (correct_msg == Table1NamePlayers)                              
                                    ListTableChange2.Items.Add(chat).BackColor = Color.Honeydew;
                                
                            }
                        }
                    }

                    if (str.Contains("# Player [" + Table1NamePlayers + "] logged"))
                    {
                        color_pointer_radmin_success = 1;
                    }

                    if (str.Contains("от Заряд ВОГ-25") || str.Contains("M209"))
                    {
                        var StartLineScan = str.Split()[0];
                        int MAX_SPACE_LINE = str.Count(simbolcount => simbolcount == ' ');

                        // Выполняем поиск
                        if (server_version == 1)
                        {
                            if (str.Contains(StartLineScan + " " + Table1NamePlayers) && (MAX_SPACE_LINE == 5 || MAX_SPACE_LINE == 6) && (StartLineScan == "*"))
                            {
                                var CMP_PNAME = str.Split()[1];
                                if (CMP_PNAME == Table1NamePlayers)                           
                                    hweapon_using++;                 
                            }
                        }
                        else
                        {
                            if (str.Contains(Table1NamePlayers) && (MAX_SPACE_LINE == 4 || MAX_SPACE_LINE == 5) && (StartLineScan == Table1NamePlayers))
                            {
                                var CMP_PNAME = str.Split()[0];
                                if (CMP_PNAME == Table1NamePlayers)                           
                                    hweapon_using++;                  
                            }
                        }
                    }
                }

                if (server_version == 1)
                {
                    foreach (string s in SrvPlayersBuffer)
                    {
                        if (s.Contains("# " + Table1NamePlayers + " ["))
                        {
                            var str_find = s.Split()[13];                                                 // выдергиваем только показатели HIM
                            var str_find_hp = s.Split()[6];                                               // выдергиваем только показатели HP
                            var str_find_weapons = s.Split()[2];                                          // выдергиваем название снаряжения
                            str_find = str_find.Substring(0, str_find.LastIndexOf("."));                  // удаляем все после точки, оставляем целое число
                            color_pointer_cheat_maxfire_counter++;                                        // число завышенных выстрелов HIM > 350, требуется игнорить gauss и нож
                            color_pointer_create_in_cheaterlist = 1;
                            if (blocked_events_handler == 0)
                            {
                                if (s.Contains("| HIM " + str_find))
                                {
                                    string msg = s.Replace("#", "Player").Replace("|", "");
                                    ListTableChange2.Items.Add(msg).BackColor = Color.Violet;
                                }
                            }
                        }
                    }
                }

                if (color_pointer_create_in_cheaterlist == 1)
                {      
                    var Hash = Table1Hash.Replace("hash:", "");
                    CHEATERPLAYERSLIST.Add(Table1NamePlayers + " % " + Table1IPAddress + " % " + Hash + " % [CHEATER] ");
                }

                if (Table1NamePlayers == "stalkazz_attack%%%")             
                   color_pointer_attack_found = 1;
                
                if (Table1IPAddress == "0.0.0.0" || Table1NamePlayers == "server" || Table1NamePlayers == "Server")
                    color_pointer_server_found = 1;
                
                // Приоритет подсветки сообщений
                // 1) [Фиолетовый] Сетевая атака
                // 2) [Бледно зеленый] Если у игрока есть история чата
                // 3) [Ярко-зеленый] Если игрок использовал подствол
                // 4) [Золотой] Если игрок записан в базе как читер
                // 5) [Розовый] Если игрок вписал строку размером более > 150 символов
                // 6) [Красный] Если игрок записан в базе как читер и Если игрок вписал строку размером более > 150 символов
                // 7) [Синий] Если игрок использует чит программы
           
                // Сетевая атака
                if (color_pointer_attack_found > 0)
                {
                    ListTableChange1.FocusedItem.BackColor = Color.DarkViolet;
                    ListTableChange2.Items.Add("Тип: " + Table1NamePlayers + " IP Address: " + Table1IPAddress).BackColor = Color.Pink;
                }

                // Если нашли 0.0.0.0 или server то данный клиент считается как "сервер" и к нему относятся все его события.
                if (color_pointer_server_found > 0)
                {
                    ListTableChange1.FocusedItem.BackColor = Color.Lime;
                    if (blocked_events_handler == 0)
                    {
                        SelectedTable1.Text = "S.E.R.V.E.R";
                        SelectedTable1.BackColor = Color.Lime;
                        SelectedTable1.ForeColor = Color.Black;
                        foreach (string srvmsg in SrvEventsBuffer)
                        {
                            if (srvmsg.Contains("stalkazz_attack"))
                            {
                                string s = srvmsg.Replace("stalkazz_attack", "[ATTACK]: ").Replace("%", string.Empty);
                                ListTableChange2.Items.Add(s).BackColor = Color.Coral;
                            }
                            if (srvmsg.Contains("Чат: ServerAdmin : "))
                            {
                                string msg = srvmsg.Replace("Чат: ServerAdmin : ", "[Администратор]: ");
                                ListTableChange2.Items.Add(msg).BackColor = Color.Snow;
                            }
                        }
                    }
                }

                // Если у игрока есть сообщение, то подсвечиваем его
                if (color_pointer_chat_msg > 0)
                {
                    ListTableChange1.FocusedItem.BackColor = Color.LightCyan;
                }

                if (hweapon_using > 0)
                {
                    ListTableChange1.FocusedItem.BackColor = Color.Aquamarine;
                    if (blocked_events_handler == 0)
                    {
                        SelectedTable1.BackColor = Color.Aquamarine;
                        SelectedTable1.ForeColor = Color.Black;
                        SelectedTable1.Text = "Игрок использовал тяжелое снаряжение против " + hweapon_using + " игроков";
                        ListTableChange2.Items.Add("[WEAPONS] Игрок использовал тяжелое снаряжение против [" + hweapon_using + "] игроков.").BackColor = Color.Aquamarine;
                    }
                }

                if (color_pointer_table > 0)
                {
                    ListTableChange1.FocusedItem.BackColor = Color.Gold;
                    if (blocked_events_handler == 0)
                    {
                        SelectedTable1.Text = "Игрок записан в базе как \"Нарушитель\"";
                        SelectedTable1.BackColor = Color.Gold;
                        SelectedTable1.ForeColor = Color.Black;
                    }
                }

                if (color_pointer_radmin_success > 0)
                {
                    ListTableChange1.FocusedItem.BackColor = Color.ForestGreen;
                    ListTableChange2.Items.Add("Игрок зарегистрировался как удаленный Администратор. The player has registered as a remote administrator").BackColor = Color.ForestGreen;
                }

                if (color_pointer_chat_size > 0)
                {
                    ListTableChange1.FocusedItem.BackColor = Color.Magenta;
                    if (blocked_events_handler == 0)
                    {
                        SelectedTable1.Text = "Игрок пытался отправить в чат большое число символов";
                        SelectedTable1.BackColor = Color.Magenta;
                        SelectedTable1.ForeColor = Color.White;
                    }
                }

                if (color_pointer_chat_size + color_pointer_table == 2)
                {
                    ListTableChange1.FocusedItem.BackColor = Color.Red;
                    if (blocked_events_handler == 0)
                    {
                        SelectedTable1.Text = "Игрок записан как читер и попытался нарушить работу сервера";
                        SelectedTable1.BackColor = Color.Red;
                        SelectedTable1.ForeColor = Color.White;
                    }
                }

                // Общие данные
                if ((blocked_events_handler == 0) && (color_pointer_cheat_maxfire_counter > 0))
                {
                    SelectedTable1.Text = "Завышенных выстрелов: " + color_pointer_cheat_maxfire_counter;
                    SelectedTable1.BackColor = Color.Lime;
                    SelectedTable1.ForeColor = Color.Black;
                }

                if (color_pointer_cheat_maxfire_counter > 0)
                {
                    ListTableChange1.FocusedItem.BackColor = Color.Blue;
                    ListTableChange1.FocusedItem.ForeColor = Color.White;
                    if (blocked_events_handler == 0)
                    {
                        SelectedTable1.BackColor = Color.Blue;
                        SelectedTable1.ForeColor = Color.White;
                    }
                }
                if ((color_pointer_table == 1 && blocked_events_handler == 0) && (color_pointer_cheat_maxfire_counter > 0))
                {
                    SelectedTable1.Text = "Записан как \"Нарушитель\". Завышенных выстрелов: " + color_pointer_cheat_maxfire_counter;
                    SelectedTable1.BackColor = Color.Coral;
                    SelectedTable1.ForeColor = Color.Black;
                }
                // Вывод информации во 2 блок
                if (blocked_events_handler == 0)
                {
                    SelectedTable2.Text = "Показаны данные из базы по игроку: [ " + Table1NamePlayers + " | " + Table1IPAddress + " ] " + "Найдено: " + ListTableChange3.Items.Count;
                }
                color_pointer_create_in_cheaterlist = 0;
            }
            catch (Exception)
            {
                ERROR_COUNTER++;
            }
        }

        private void Snapshot_Click(object sender, EventArgs e)
        {
            StartAutoCheckThread.Checked = true;
        }

        private void NewWindowStrCopy_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText("Name: " + ListTableChange1.FocusedItem.SubItems[1].Text + "\nIP: " + ListTableChange1.FocusedItem.SubItems[4].Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка. Повторите попытку\nReason:\n" + ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void NewWindowBanInfo_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText("sv_banplayer_ip " + ListTableChange1.FocusedItem.SubItems[4].Text + " 100000");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка. Повторите попытку\nReason:\n" + ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BaseMenu1AddToBase_Click(object sender, EventArgs e)
        {
            try
            {
                string pName = ListTableChange1.FocusedItem.SubItems[1].Text;
                string pHash = ListTableChange1.FocusedItem.SubItems[3].Text;
                string pAddr = ListTableChange1.FocusedItem.SubItems[4].Text;
                AddInBaseName.Text = pName.Replace("%", "_");
                AddInBaseIPAddress.Text = pAddr;
                AddInBaseHash.Text = pHash;
                tabControl1.SelectedIndex = 2;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Принудительно добавить всех в базу
        private void btnStartThread_Click(object sender, EventArgs e)
        {
            if (ThreadListplayers.IsBusy == true)
            {
                MessageBox.Show("Обработка уже выполняется, пожалуйста подождите ее завершения.", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Пожалуйста дождитесь завершения процедуры.\nМы дадим знать, когда данные будут добавлены в базу.", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
                get_show_informer_at_threading_complited = 1;
                START_LEVEL_THREAD = 4;
                ThreadListplayers.WorkerSupportsCancellation = true;
                ThreadListplayers.RunWorkerAsync();
            }
        }

        private void btnStartHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Функция подсветки текста в истории чата: Подсвечивает строки c сообщением, где встречаются введенные вами слова с текстом сообщением пользователя. Каждое новое слово и текст, вписывайте в новый контейнер. Цвет подсветки соответствует цвету поля.", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        // ============================================================================== WINDOWS FIREWALL ==============================================================================      
        HashSet<string> AddressBuffer = new HashSet<string>();  
        int check_new_data = 0;

        int FlagAdmin = 0, FlagCheater = 0, FlagWeapons = 0;

        public void FirewallAddressBaseLoad()
        {
            AddressBuffer.Clear();
            FirewallList.Items.Clear();
            try
            {
                INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
                foreach (INetFwRule WriteToAddress in firewallPolicy.Rules)
                {
                    if (WriteToAddress.Name.Contains("STALKER_SRV "))
                    {
                        var RULE_INFO = WriteToAddress.Name.Split()[0];
                        var PLAYER_INFO = WriteToAddress.Name.Split()[2];
                        var ADDRESS_INFO = WriteToAddress.Name.Split()[6];
                        var REASON_INFO = WriteToAddress.Name.Split()[1];
                        var FLAGS_INFO = WriteToAddress.Name.Split()[3];
                        var DATE_INFO = WriteToAddress.Name.Split()[8];
                        AddressBuffer.Add(PLAYER_INFO + " % " + RULE_INFO + " % " + ADDRESS_INFO + " % " + REASON_INFO + " " + FLAGS_INFO + " % " + DATE_INFO);
                    }
                }
            }
            catch (Exception )
            {
                ERROR_COUNTER++;
            }

            FirewallInfo.Text = "Адресов заблокировано: " + AddressBuffer.Count;
            if (StartAutoCheckThread.CheckState == CheckState.Unchecked)
            {
                check_new_data = AddressBuffer.Count;
                FlagCheater = FlagWeapons = FlagAdmin = 0;
                foreach (string drawing_table in AddressBuffer)
                {
                    if (drawing_table.Contains("STALKER_SRV"))
                    {
                        if (drawing_table.Contains("[CHEATER]"))
                        {
                            string[] values = drawing_table.Split('%');
                            FirewallList.Items.Add(new ListViewItem(values)).BackColor = Color.Violet;
                            FlagCheater++;
                        }
                        else if (drawing_table.Contains("[HWEAPONS]"))
                        {
                            string[] values = drawing_table.Split('%');
                            FirewallList.Items.Add(new ListViewItem(values)).BackColor = Color.Coral;
                            FlagWeapons++;
                        }
                        else if (drawing_table.Contains("[ADMIN]"))
                        {
                            string[] values = drawing_table.Split('%');
                            FirewallList.Items.Add(new ListViewItem(values)).BackColor = Color.ForestGreen;
                            FlagAdmin++;
                        }
                        else if (drawing_table.Contains("[LIST]"))
                        {
                            string[] values = drawing_table.Split('%');
                            FirewallList.Items.Add(new ListViewItem(values)).BackColor = Color.Gold;
                        }
                        else if (drawing_table.Contains("[HIDE]"))
                        {
                            string[] values = drawing_table.Split('%');
                            FirewallList.Items.Add(new ListViewItem(values)).BackColor = Color.DeepSkyBlue;
                        }
                    }
                }
                gui_filter1.Text = "[ADMIN]: " + FlagAdmin;
                gui_filter2.Text = "[CHEATER]: " + FlagCheater;
                gui_filter3.Text = "[WEAPONS]: " + FlagWeapons;
            }
            if ((check_new_data != AddressBuffer.Count) && (StartAutoCheckThread.CheckState == CheckState.Checked))
            {
                FirwallLoadFilterAddress.BackColor = Color.Lime;
                FirwallLoadFilterAddress.Text = "Есть новые данные. Обновить";
            }
        }

        private void WinFirewallActivate_CheckedChanged(object sender, EventArgs e)
        {
            if (WinFirewallActivate.CheckState == CheckState.Checked)
            {
                FirewallAddressBaseLoad();
                FirewallIPBlocked.Enabled = true;
                FirwallSearchInList.Enabled = true;
                FirewallServerHide.Enabled = true;
                ServerFirewallHideRuleDel.Enabled = true;
                FirewallBlock.Enabled = true;
                FirewallAllUnblock.Enabled = true;
                FirwallLoadFilterAddress.Enabled = true;
                GetFuncDelFirewallRule.Enabled = true;
                EventsControllerBlockedPlayers.Enabled = true;
                EventsControllerMaxLimit.Enabled = true;
                btnGetBanplayer.Enabled = true;
                EventsWeaponsBlockedPlayers.Enabled = true;
                EventsControllerTrayMessage.Enabled = true;
                EventsBlockedSubnet.Enabled = true;
                btnStatusFunct.Visible = false;
                BlockADDR.Enabled = true;
            }
            else
            {
                FirewallIPBlocked.Enabled = false;
                FirwallSearchInList.Enabled = false;
                FirewallServerHide.Enabled = false;
                ServerFirewallHideRuleDel.Enabled = false;
                FirewallBlock.Enabled = false;
                FirewallAllUnblock.Enabled = false;
                FirwallLoadFilterAddress.Enabled = false;
                GetFuncDelFirewallRule.Enabled = false;
                EventsControllerBlockedPlayers.Enabled = false;
                EventsControllerMaxLimit.Enabled = false;
                btnGetBanplayer.Enabled = false;
                EventsWeaponsBlockedPlayers.Enabled = false;
                EventsWeaponsBlockedPlayers.Checked = false;
                EventsControllerBlockedPlayers.Checked = false;
                EventsControllerTrayMessage.Enabled = false;
                EventsBlockedSubnet.Enabled = false;
                EventsBlockedSubnet.Checked = false;
                btnStatusFunct.Visible = true;
                BlockADDR.Enabled = false;
            }
        }

        private void FirewallServerHide_Click(object sender, EventArgs e)
        {
            IPBlockedServices.FirewallRuleServerHideCreate(FirewallBlockedPort.Text);
            FirewallAddressBaseLoad();
        }


        private void ServerFirewallHideRuleDel_Click(object sender, EventArgs e)
        {
            int completed_proc = 0;
            int error_proc = 0;
            foreach (string get_state in AddressBuffer)
            {
                try
                {
                    var PLAYER_INFO = get_state.Split()[0];
                    //var RULE_INFO = get_state.Split()[2];
                    var ADDRESS_INFO = get_state.Split()[4];
                    var FLAGS_INFO = get_state.Split()[7];
                    var DATE_INFO = get_state.Split()[9];
                    IPBlockedServices.CleanAllRules("[HIDE] " + PLAYER_INFO + " " + FLAGS_INFO + " IP:  " + ADDRESS_INFO + " Time: " + DATE_INFO);
                    completed_proc++;
                }
                catch (Exception)
                {
                    error_proc++;
                }
            }           
            if ((completed_proc != 0) && (error_proc == 0))          
                MessageBox.Show("Успешное завершение процедуры.\nУдалено правил блокировок: " + completed_proc, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);          
            else if ((completed_proc != 0) && (error_proc > 0))          
                MessageBox.Show("При удалении правил произошли ошибки\nУдалено правил блокировок: " + completed_proc + "\nОшибок: " + error_proc, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);      
            FirewallAddressBaseLoad();
        }

        private void FirewallBlock_Click(object sender, EventArgs e)
        {
            FirewallIPBox.Text = FirewallIPBox.Text.Replace("=", ".").Replace(",", ".");
            if ((FirewallIPBox.Text.Length > 0) && (FirewallTextBox.Text.Length > 0))
            {
                if (FirewallAddressMaskAdd.CheckState == CheckState.Checked)
                    BlockedClient(8, FirewallTextBox.Text.Replace(" ", "_").Replace("%", "_"), " Flags:[null] IP: ", FirewallIPBox.Text + "/" + AddressByteMask.Text, "@");
                else
                    BlockedClient(8, FirewallTextBox.Text.Replace(" ", "_").Replace("%", "_"), " Flags:[null] IP: ", FirewallIPBox.Text, "@");
            }
            else
            {
                MessageBox.Show("Нельзя добавлять данные без явного указанного адреса или имени игрока!", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void FirewallTableGetAddress_Click(object sender, EventArgs e)
        {
            try
            {
                var remove_filters = "";
                var getname = FirewallList.FocusedItem.SubItems[1].Text.Replace(" ", "");
                var address = FirewallList.FocusedItem.SubItems[2].Text.Replace(" ", "");
                var filters = FirewallList.FocusedItem.SubItems[3].Text.Replace(" ", "");
                var timeban = FirewallList.FocusedItem.SubItems[4].Text.Replace(" ", "");
                var strscan = filters;
                filters = filters.Replace("[ADMIN]", "").Replace("[HWEAPONS]", "").Replace("[CHEATER]", "").Replace("[LIST]", "").Replace("[HIDE]", "");
                if (strscan.Contains("[ADMIN]"))
                    remove_filters = "[ADMIN]";
                else if (strscan.Contains("[HWEAPONS]"))
                    remove_filters = "[HWEAPONS]";
                else if (strscan.Contains("[CHEATER]"))
                    remove_filters = "[CHEATER]";
                else if (strscan.Contains("[LIST]"))
                    remove_filters = "[LIST]";
                else if (strscan.Contains("[HIDE]"))
                    remove_filters = "[HIDE]";


               IPBlockedServices.CleanAllRules(remove_filters + " " + FirewallList.FocusedItem.SubItems[0].Text + filters + " IP:  " + address + " Time: " + timeban);

                var index = remove_filters + " " + FirewallList.FocusedItem.SubItems[0].Text + filters + " IP:  " + address + " Time: " + timeban;

                ListViewItem del = FirewallList.FocusedItem;
                FirewallList.Items.Remove(del);
                FirewallAddressBaseLoad();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error sending command. Reason\n" + ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void FirewallSearchAddressInBase_Click(object sender, EventArgs e)
        {        
            try
            {
                int result_search = 0;
                listBase.Items.Clear();
                var getaddr = FirewallList.FocusedItem.SubItems[2].Text.Replace(" ", "");
                foreach (string getip in PlayersBaseBuffer)
                {
                    if (getip.Contains(getaddr))
                    {
                        result_search++;
                        string[] values = getip.Split('%');
                        listBase.Items.Add(new ListViewItem(values));
                    }
                }
                if (result_search != 0)
                {
                    tabControl1.SelectedIndex = 0;
                    MessageBox.Show("По Вашему запросу найдено: " + result_search + " результатов." , "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Поиск не дал результатов.", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void btnCopyAddr_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(FirewallList.FocusedItem.SubItems[2].Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка. Повторите попытку\nReason:\n" + ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void FirwallSearchInList_Click(object sender, EventArgs e)
        {
            int result_search = 0;
            if (FirewallRulesSearch.Text.Length > 0)
            {
                FirewallList.Items.Clear();
                foreach (string search in AddressBuffer)
                {
                    if (search.Contains(FirewallRulesSearch.Text))
                    {
                        result_search++;
                        if (search.Contains("[CHEATER]"))
                        {
                            string[] values = search.Split('%');
                            FirewallList.Items.Add(new ListViewItem(values)).BackColor = Color.Violet;
                        }
                        else if (search.Contains("[HWEAPONS]"))
                        {
                            string[] values = search.Split('%');
                            FirewallList.Items.Add(new ListViewItem(values)).BackColor = Color.Coral;
                        }
                        else if (search.Contains("[ADMIN]"))
                        {
                            string[] values = search.Split('%');
                            FirewallList.Items.Add(new ListViewItem(values)).BackColor = Color.ForestGreen;
                        }
                        else if (search.Contains("[LIST]"))
                        {
                            string[] values = search.Split('%');
                            FirewallList.Items.Add(new ListViewItem(values)).BackColor = Color.Gold;
                        }
                        else if (search.Contains("[HIDE]"))
                        {
                            string[] values = search.Split('%');
                            FirewallList.Items.Add(new ListViewItem(values)).BackColor = Color.DeepSkyBlue;
                        }
                    }
                }
                if (result_search == 0)              
                    MessageBox.Show("Поиск не дал результатов.", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);               
                else               
                    MessageBox.Show("По Вашему запросу найдено: " + result_search + " результатов.", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);              
            }
            else
            {
                MessageBox.Show("Нельзя выполнить поиск! Введите запрос для поиска.", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void FirewallAllUnblock_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Вы действительно хотите удалить все введенные данные из внутренних правил Брандмауэра Windows?\nПродолжить?", "Удаление всех правил блокировки", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (dialogResult == DialogResult.Yes)
            {
                SIGNAL_BANNED = 0;
                int completed_proc = 0;
                int error_proc = 0;
                foreach (string get_state in AddressBuffer)
                {
                    try
                    {
                        var PLAYER_INFO = get_state.Split()[0];
                        var RULE_INFO = get_state.Split()[2];
                        var ADDRESS_INFO = get_state.Split()[4];
                        var REASON_INFO = get_state.Split()[6];
                        var FLAGS_INFO = get_state.Split()[7];
                        var DATE_INFO = get_state.Split()[9];
                        IPBlockedServices.CleanAllRules(REASON_INFO + " " + PLAYER_INFO + " " + FLAGS_INFO + " IP:  " + ADDRESS_INFO + " Time: " + DATE_INFO);
                        completed_proc++;
                    }
                    catch (Exception)
                    {
                        error_proc++;
                    }
                }
                if ((completed_proc != 0) && (error_proc == 0))
                    MessageBox.Show("Успешное завершение процедуры.\nУдалено правил блокировок: " + completed_proc, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else if ((completed_proc != 0) && (error_proc > 0))
                    MessageBox.Show("При удалении правил произошли ошибки\nУдалено правил блокировок: " + completed_proc + "\nОшибок: " + error_proc, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);

                FirewallAddressBaseLoad();
            }
        }

        private void btn_gui_filter1_Click(object sender, EventArgs e)
        {
            GET_REMOVE_STR("[ADMIN]");
        }

        private void btn_gui_filter2_Click(object sender, EventArgs e)
        {
            GET_REMOVE_STR("[CHEATER]");
        }

        private void btn_gui_filter3_Click(object sender, EventArgs e)
        {
            GET_REMOVE_STR("[HWEAPONS]");
        }

        private void GET_REMOVE_STR(string FILTER_REMOVE_DESCRIPTION)
        {
            DialogResult dialogResult = MessageBox.Show("Вы действительно хотите удалить все блокировки из группы " + FILTER_REMOVE_DESCRIPTION +" из внутренних правил Брандмауэра Windows?\nПродолжить?", "Удаление правил блокировки группы: " + FILTER_REMOVE_DESCRIPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (dialogResult == DialogResult.Yes)
            {
                int completed_proc = 0;
                int error_proc = 0;
                foreach (string get_state in AddressBuffer)
                {
                    if (get_state.Contains(FILTER_REMOVE_DESCRIPTION))
                    {
                        try
                        {
                            var PLAYER_INFO = get_state.Split()[0];
                            var ADDRESS_INFO = get_state.Split()[4];
                            var REASON_INFO = get_state.Split()[6];
                            var FLAGS_INFO = get_state.Split()[7];
                            var DATE_INFO = get_state.Split()[9];
                            IPBlockedServices.CleanAllRules(REASON_INFO + " " + PLAYER_INFO + " " + FLAGS_INFO + " IP:  " + ADDRESS_INFO + " Time: " + DATE_INFO);
                            completed_proc++;
                        }
                        catch (Exception)
                        {
                            error_proc++;
                        }
                    }
                }
                SIGNAL_BANNED = (SIGNAL_BANNED - completed_proc);

                if ((completed_proc != 0) && (error_proc == 0))
                    MessageBox.Show("Успешное завершение процедуры.\nУдалено правил блокировок: " + completed_proc, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else if ((completed_proc != 0) && (error_proc > 0))
                    MessageBox.Show("При удалении правил произошли ошибки\nУдалено правил блокировок: " + completed_proc + "\nОшибок: " + error_proc, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FirewallAddressBaseLoad();
            }
        }

        private void FirwallLoadFilterAddress_Click(object sender, EventArgs e)
        {
            check_new_data = AddressBuffer.Count;
            FirwallLoadFilterAddress.BackColor = Color.PaleTurquoise;
            FirwallLoadFilterAddress.Text = "Обновить список блокировок";
            FirewallAddressBaseLoad();
        }

        private void FirewallIPBlocked_Click(object sender, EventArgs e)
        {
            try
            {
                string pname = ListTableChange1.FocusedItem.SubItems[1].Text;
                string address = ListTableChange1.FocusedItem.SubItems[4].Text;
                BlockedClient(8, pname, " Flags:[null] IP: ", address, "@");
                tabControl1.SelectedIndex = 6;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // all players block
        HashSet<string> FirewallAddressContainer = new HashSet<string>();
        private void GetFuncCreateInFirewall_Click(object sender, EventArgs e)
        {
            try
            {
                FirewallAddressContainer.Clear();
                GetFuncDelFirewallRule.PerformClick();
                var addresscount = listBase.Items.Count;
                DialogResult dialogResult = MessageBox.Show("Вы действительно хотите заблокировать " + addresscount + " игроков?\nПродолжить?", "Добавление адресов для блокировки Брандмауэром", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (dialogResult == DialogResult.Yes)
                {
                    for (int i = 0; i < listBase.Items.Count; i++)
                    {
                        listBase.Items[i].Focused = true;
                        listBase.Items[i].Checked = true;
                        if (i > 0)
                        {
                            listBase.Items[i - 1].Focused = false;
                            listBase.Items[i - 1].Checked = false;
                        }
                    }

                    IPBlockedServices.FirewallNewRuleCreate(" [LIST] " + "ExportFromTheBase[" + addresscount + "] Flags:[null] IP: ", string.Concat(FirewallAddressContainer), false, null);
                    FirewallAddressBaseLoad();
                    GetFuncCreateInFirewall.Enabled = false;
                    tabControl1.SelectedIndex = 6;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // check address format
        private void listBase_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            try
            {
                int address_skip = 0;
                string addressblock = listBase.FocusedItem.SubItems[4].Text.Replace(" ", string.Empty); // IP
                if ((addressblock.Length >= 7 && addressblock.Length <= 16) && (addressblock != "0.0.0.0" && addressblock != "127.0.0.1" && addressblock != "255.255.255.255")) // FORMAT STR OK
                {
                    address_skip = addressblock.Count(simbolcount => simbolcount == '.');
                    if (addressblock.Length > 0 && address_skip == 3)
                    {
                        listBase.FocusedItem.BackColor = Color.LightGreen;
                        FirewallAddressContainer.Add(addressblock.Replace(Environment.NewLine, "") + ",");    // OK 
                    }
                    if (address_skip != 3)                             
                    {
                        listBase.FocusedItem.BackColor = Color.Violet;
                    }
                }
                else    
                {
                    listBase.FocusedItem.BackColor = Color.Orange;
                }
            }
            catch (Exception)
            {
                ERROR_COUNTER++;
            }
        }

        private void CheckBaseLineStatus_Click(object sender, EventArgs e)
        {
            int check_base = 0;
            int result_scan = 0;
            listBase.Items.Clear();
            foreach (string check_line in PlayersBaseBuffer)
            {
                check_base = check_line.Count(simbolcount => simbolcount == '%');
                var GetLineAddr = check_line.Split('%')[4];

                if (!(GetLineAddr.Length >= 7 && GetLineAddr.Length <= 16))
                {
                    string[] values = check_line.Split('%');
                    listBase.Items.Add(new ListViewItem(values)).BackColor = Color.Violet;
                }

                if (!(check_base == 4 || check_base == 5 || check_base == 6))
                {
                    result_scan++;
                    string[] values = check_line.Split('%');
                    listBase.Items.Add(new ListViewItem(values));
                }
            }

            if (result_scan > 0)
            {      
                DialogResult OK = MessageBox.Show("Было обнаружено [" + result_scan + "] некорректных введенных данных.\nТребуется удалить их?", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (DialogResult.OK == OK)
                {
                    result_scan = 0;
                    foreach (string check_line in PlayersBaseBuffer.ToArray())
                    {
                        check_base = check_line.Count(simbolcount => simbolcount == '%');
                        if (!(check_base == 4 || check_base == 5 || check_base == 6))
                        {
                            result_scan++;
                            PlayersBaseBuffer.Remove(check_line);
                        }
                    }
                    Save(string.Concat(PlayersBaseBuffer));
                    Thread.Sleep(500);
                    BaseLoadInBuffer();
                    MessageBox.Show("Было успешно удалено [" + result_scan + "] некорректных данных.", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                }
            }
            else          
                MessageBox.Show("Нарушений нет.", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);    
        }

        private void GetFuncDelFirewallRule_Click(object sender, EventArgs e)
        {
            int completed_proc = 0;
            int error_proc = 0;
            foreach (string get_state in AddressBuffer)
            {
                try
                {
                    var PLAYER_INFO = get_state.Split()[0];
                    var RULE_INFO = get_state.Split()[2];
                    var ADDRESS_INFO = get_state.Split()[4];
                    var FLAGS_INFO = get_state.Split()[7];
                    var DATE_INFO = get_state.Split()[9];
                    //FirewallMsgTransferAddress.FirewallAddressInTables = RULE_INFO + " [LIST] " + PLAYER_INFO + " " + FLAGS_INFO + " IP:  " + ADDRESS_INFO + " Time: " + DATE_INFO;
                    //IPBlockedServices.FirewallAllUserRuleDelete();
                    IPBlockedServices.CleanAllRules("[HIDE] " + PLAYER_INFO + " " + FLAGS_INFO + " IP:  " + ADDRESS_INFO + " Time: " + DATE_INFO);
                    completed_proc++;
                }
                catch (Exception)
                {
                    error_proc++;
                }
            }
            if ((completed_proc != 0) && (error_proc == 0))
            {
                MessageBox.Show("Успешное завершение процедуры.\nУдалено правил блокировок: " + completed_proc, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if ((completed_proc != 0) && (error_proc > 0))
            {
                MessageBox.Show("При удалении правил произошли ошибки\nУдалено правил блокировок: " + completed_proc + "\nОшибок: " + error_proc, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            FirewallAddressBaseLoad();
        }


        private void tabControl2_Selected(object sender, TabControlEventArgs e)
        {
            if (tabControl2.SelectedIndex == 3)            
                update_stats();            
        }

        private void OpenDirHTML()
        {
            try
            {
                Process.Start(@"PlayersDataBase\html_cheater_base\");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnStatusFunct_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            tabControl1.SelectedIndex = 5;
        }

        int EnterCulumnTable = -1;
        private void FirewallList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Получим нажатый индекс
            if (e.Column != EnterCulumnTable)
            {
                EnterCulumnTable = e.Column;
                FirewallList.Sorting = SortOrder.Ascending;
            }
            else
            {
                if (FirewallList.Sorting == SortOrder.Ascending)
                    FirewallList.Sorting = SortOrder.Descending;
                else
                    FirewallList.Sorting = SortOrder.Ascending;
            }
            FirewallList.Sort();
            FirewallList.ListViewItemSorter = new ListViewItemComparer(e.Column, FirewallList.Sorting);
        }
    }
}

public class ListViewItemComparer : IComparer
{
    private int idx;
    private SortOrder order;
    public ListViewItemComparer()
    {
        idx = 0;
        order = SortOrder.Ascending;
    }
    public ListViewItemComparer(int column, SortOrder order)
    {
        idx = column;
        this.order = order;
    }
    public int Compare(object x, object y)
    {
        int returnVal = -1;
        returnVal = string.Compare(((ListViewItem)x).SubItems[idx].Text, ((ListViewItem)y).SubItems[idx].Text);
        if (order == SortOrder.Descending)
            returnVal *= -1;
        return returnVal;
    }
}