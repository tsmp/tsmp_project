using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace S.E.R.V.E.R___Shadow_Of_Chernobyl_1._0006
{
    public partial class ServerEvents : Form
    {
        public ServerEvents()
        {
            InitializeComponent();
            DYNAMIC_SET_COLOR();
            ThreadEventsType.WorkerSupportsCancellation = true;
            ThreadEventsType.RunWorkerAsync();
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
        int artefact_activated = 0;
        int ip_attack = 0;
        int medic_error = 0;
        int error_stack = 0;
        int connected = 0;
        int unknown_cmd = 0;
        int player_found = 0;
        int kicked_by_server = 0;
        int logged_as = 0;
        int access_denied = 0;
        int Invalid_cdkey = 0;
        int fatal_error = 0;
        int reg_load = 0;
        int ahtung = 0;
        int artefact = 0;
        int sv_destroy_error = 0;
        int send_size = 0;
        int m_change = 0;
        int weapon = 0;
        int packet_size = 0;
        int chat_bad_symbol = 0;
        int server_null_ip = 0;
        int bad_players = 0;
        int chat_size_warning = 0;
        int ERROR = 0;
        int ERROR_2 = 0;
        int V_ERROR = 0;
        int VOTE_START_BLOCK = 0;
        int ERROR_3_PLAYER_UPDATE_ERROR = 0;

        private void ThreadEventsType_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
            listEvents.Items.Add("Пожалуйста подождите, выполняется обработка данных...").BackColor = Color.Gold;
            foreach (string s in File.ReadLines(@"server_settings\logs\xray_" + SystemInformation.UserName + ".log", Encoding.GetEncoding(1251)))
            {
                if (s.Contains("! ip attack"))
                {
                    ip_attack += 1;
                    string ip = s.Replace("! ip attack", "Сетевая атака [1]");
                    listEvents2.Items.Add(ip).BackColor = Color.DarkOrange;
                }
                else if (s.Contains("! blocked"))
                {
                    ip_attack += 1;
                    string ip = s.Replace("! blocked", "Сетевая атака [2]");
                    listEvents2.Items.Add(ip).BackColor = Color.DarkOrange;
                }
                else if (s.Contains("! stalkazz attack blocked , ip -"))
                {
                    ip_attack += 1;
                    string ip = s.Replace("! stalkazz attack blocked , ip -", "Сетевая атака [3]");
                    listEvents2.Items.Add(ip).BackColor = Color.DarkOrange;
                }
                else if (s.Contains("! too large packet size"))
                {
                    packet_size += 1;
                }
                else if (s.Contains("! Error_Stack Trace:"))
                {
                    error_stack += 1;
                }
                else if (s.Contains("Player not found. New player created."))
                {
                    connected += 1;
                }
                else if (s.Contains("bad symbol in chat"))
                {
                    chat_bad_symbol += 1;
                }
                else if (s.Contains("! Unknown command:"))
                {
                    unknown_cmd += 1;
                }
                else if (s.Contains("select artefact RPoint"))
                {
                    artefact += 1;
                }
                else if (s.Contains("Player found"))
                {
                    player_found += 1;
                }
                else if (s.Contains("Kicked By Server"))
                {
                    kicked_by_server += 1;
                }
                else if (s.Contains("kicked_by_server"))
                {
                    kicked_by_server += 1;
                }
                else if (s.Contains("Medic_1"))
                {
                    medic_error += 1;
                }
                else if (s.Contains("Medic_2"))
                {
                    medic_error += 1;
                }
                else if (s.Contains("Medic_3"))
                {
                    medic_error += 1;
                }
                else if (s.Contains("Medic_Error_1"))
                {
                    medic_error += 1;
                }
                else if (s.Contains("Medic_Error_2"))
                {
                    medic_error += 1;
                }
                else if (s.Contains("Server_Error_Resrat mp_grenade_f1"))
                {
                    medic_error += 1;
                }
                else if (s.Contains("logged as"))
                {
                    logged_as += 1;
                    string ls = s.Replace("logged as remote administrator", "- Зарегистрирован как Удаленный Администратор");
                    string rs = ls.Replace("# User", "");
                    listAdminEvents.Items.Add(rs).BackColor = Color.PaleGreen;

                }
                else if (s.Contains("Access denied"))
                {
                    access_denied += 1;
                    string ls = s.Replace("tried to login as remote administrator. Access denied", "- Доступ запрещен");
                    string rs = ls.Replace("# User", "");
                    listAdminEvents.Items.Add(rs).BackColor = Color.LightCoral;
                }
                else if (s.Contains("<Invalid CD-Key>"))
                {
                    Invalid_cdkey += 1;
                }
                else if (s.Contains("FATAL ERROR"))
                {
                    fatal_error += 1;
                }
                else if (s.Contains("REG_LOAD"))
                {
                    reg_load += 1;
                }
                else if (s.Contains("ahtung !!!"))
                {
                    ahtung += 1;
                }
                else if (s.Contains("!SV:ge_destroy:"))
                {
                    sv_destroy_error += 1;
                }
                else if (s.Contains("M_CHANGE"))
                {
                    m_change += 1;
                }
                else if (s.Contains("Send size"))
                {
                    send_size += 1;
                }
                else if (s.Contains("spawned a zone"))
                {
                    artefact_activated += 1;
                }

                else if (s.Contains("Чат:"))
                {
                    var color_msg = s.Split()[0];
                    var check_msg = s.Split()[1];

                    if (color_msg + " " + check_msg == "- Чат:")
                    {
                        string green = s.Replace("- Чат:", "[Свобода]: ");
                        listChatHistory.Items.Add(green).BackColor = Color.PaleGreen;
                    }
                    else if (color_msg + " " + check_msg == "~ Чат:")
                    {
                        string blue = s.Replace("~ Чат:", "[Наемники]: ");
                        listChatHistory.Items.Add(blue).BackColor = Color.LightBlue;
                    }
                    else if ((color_msg + " " + check_msg.Length == "Чат: " + check_msg.Length) && (check_msg != "ServerAdmin"))
                    {
                        string all_msg = s.Replace("Чат:", "[Общий чат]: ");
                        listChatHistory.Items.Add(all_msg).BackColor = Color.Honeydew;
                    }
                    else if ((color_msg + " " + check_msg == "Чат: ServerAdmin") && (check_msg == "ServerAdmin"))
                    {
                        string all_msg = s.Replace("Чат: ServerAdmin : ", "[Администратор]: ");
                        listChatHistory.Items.Add(all_msg).BackColor = Color.Gold;
                    }
                }

                // patch
                else if (s.Contains("used too many symbols in chat"))
                {
                    chat_size_warning += 1;
                }
                else if (s.Contains("attempt to ban server ip"))
                {
                    server_null_ip += 1;
                }
                else if (s.Contains("! Data verification failed. Cheater? Someone tried to connect with this data"))
                {
                    bad_players += 1;
                }
                else if (s.Contains("! ERROR: incorrect destroy sequence for object"))
                {
                    ERROR += 1;
                }
                else if (s.Contains("! error Idx<"))
                {
                    ERROR_2 += 1;
                }
                else if (s.Contains("! bad symbol in started vote player name found - _"))
                {
                    V_ERROR += 1;
                }
                else if (s.Contains("! too many symbols in started vote player name found"))
                {
                    V_ERROR += 1;
                }
                else if (s.Contains("! bad symbol in vote found - _"))
                {
                    V_ERROR += 1;
                }
                else if (s.Contains("! too many symbols in vote found"))
                {
                    V_ERROR += 1;
                }
                else if (s.Contains("! bad symbol in vote found - percent"))
                {
                    V_ERROR += 1;
                }
                else if (s.Contains("in the list.Vote started player"))
                {
                    VOTE_START_BLOCK += 1;
                }
                else if (s.Contains("! error Process_update client not found"))
                {
                    ERROR_3_PLAYER_UPDATE_ERROR += 1;
                }

                // Weapons usage statistics
                else if (s.Contains("от Заряд ВОГ-25"))
                {
                    weapon += 1;
                    listWeaponEvents.Items.Add(s).BackColor = Color.LightPink;
                }
                else if (s.Contains("M209"))
                {
                    weapon += 1;
                    listWeaponEvents.Items.Add(s).BackColor = Color.LightCoral;
                }
            }
        }

        private void ThreadEventsType_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                listEvents.Items.Clear();
                if (artefact > 0)
                {
                    ListViewItem lvi = new ListViewItem("Всего респавнов артефактов");
                    lvi.SubItems.Add(Convert.ToString(artefact));
                    listEvents.Items.Add(lvi).BackColor = Color.Gold;
                }
                if (connected > 0)
                {
                    ListViewItem lvi1 = new ListViewItem("Зарегистрировано всего игроков");
                    lvi1.SubItems.Add(Convert.ToString(connected));
                    listEvents.Items.Add(lvi1).BackColor = Color.LimeGreen;
                }
                // ListViewItem lvi2 = new ListViewItem("Всего игроков, которые вышли с сервера");
                // lvi2.SubItems.Add(Convert.ToString());
                // listEvents.Items.Add(lvi2);

                if (player_found > 0)
                {
                    ListViewItem lvi3 = new ListViewItem("Всего игроков, которые повторно зашли на сервер");
                    lvi3.SubItems.Add(Convert.ToString(player_found));
                    listEvents.Items.Add(lvi3).BackColor = Color.LightGreen;
                }
                if (kicked_by_server > 0)
                {
                    ListViewItem lvi4 = new ListViewItem("Всего игроков, которые автоматический исключены сервером");
                    lvi4.SubItems.Add(Convert.ToString(kicked_by_server));
                    listEvents.Items.Add(lvi4).BackColor = Color.LightCoral;
                }
                if (medic_error > 0)
                {
                    ListViewItem lvi5 = new ListViewItem("Обнаружено попыток обвала сервера аптечками");
                    lvi5.SubItems.Add(Convert.ToString(medic_error));
                    listEvents.Items.Add(lvi5).BackColor = Color.DarkOrange;
                }
                if (access_denied > 0)
                {
                    ListViewItem lvi6 = new ListViewItem("Попыток ввода неверного пароля и/или логина от доступа к серверу [Radmin]");
                    lvi6.SubItems.Add(Convert.ToString(access_denied));
                    listEvents.Items.Add(lvi6).BackColor = Color.Orange;
                }
                if (logged_as > 0)
                {
                    ListViewItem lvi7 = new ListViewItem("Успешного ввода пароля и логина от доступа к серверу [Radmin]");
                    lvi7.SubItems.Add(Convert.ToString(logged_as));
                    listEvents.Items.Add(lvi7).BackColor = Color.Lime;
                }
                if (ip_attack > 0)
                {
                    ListViewItem lvi8 = new ListViewItem("Заблокировано сетевых атак");
                    lvi8.SubItems.Add(Convert.ToString(ip_attack));
                    listEvents.Items.Add(lvi8).BackColor = Color.Red;
                }
                if (Invalid_cdkey > 0)
                {
                    ListViewItem lvi9 = new ListViewItem("Попыток зайти на сервер с занятым CDKEY ключом");
                    lvi9.SubItems.Add(Convert.ToString(Invalid_cdkey));
                    listEvents.Items.Add(lvi9).BackColor = Color.LightBlue;
                }
                if (fatal_error > 0)
                {
                    ListViewItem lvi10 = new ListViewItem("Критических ошибок сервера [fatal error]");
                    lvi10.SubItems.Add(Convert.ToString(fatal_error));
                    listEvents.Items.Add(lvi10).BackColor = Color.LightCoral;
                }
                if (reg_load > 0)
                {
                    ListViewItem lvi11 = new ListViewItem("Критических ошибок сервера [rload]");
                    lvi11.SubItems.Add(Convert.ToString(reg_load));
                    listEvents.Items.Add(lvi11).BackColor = Color.LightCoral;
                }
                if (ahtung > 0)
                {
                    ListViewItem lvi12 = new ListViewItem("Критических ошибок сервера [ahtung]");
                    lvi12.SubItems.Add(Convert.ToString(ahtung));
                    listEvents.Items.Add(lvi12).BackColor = Color.LightCoral;
                }
                if (sv_destroy_error > 0)
                {
                    ListViewItem lvi13 = new ListViewItem("Попыток удалить сервером несуществующий объект");
                    lvi13.SubItems.Add(Convert.ToString(sv_destroy_error));
                    listEvents.Items.Add(lvi13).BackColor = Color.DarkOrange;
                }
                if (m_change > 0)
                {
                    ListViewItem lvi14 = new ListViewItem("Общее количество смен карт и режимов игры");
                    lvi14.SubItems.Add(Convert.ToString(m_change));
                    listEvents.Items.Add(lvi14).BackColor = Color.LightBlue;
                }
                if (send_size > 0)
                {
                    ListViewItem lvi15 = new ListViewItem("Превышен лимит входящих данных[3]");
                    lvi15.SubItems.Add(Convert.ToString(send_size));
                    listEvents.Items.Add(lvi15).BackColor = Color.LightCoral;
                }
                if (unknown_cmd > 0)
                {
                    ListViewItem lvi16 = new ListViewItem("Выполнено недопустимых команд");
                    lvi16.SubItems.Add(Convert.ToString(unknown_cmd));
                    listEvents.Items.Add(lvi16).BackColor = Color.Orange;
                }
                if (packet_size > 0)
                {
                    ListViewItem lvi17 = new ListViewItem("Превышен лимит входящих данных[4]");
                    lvi17.SubItems.Add(Convert.ToString(packet_size));
                    listEvents.Items.Add(lvi17).BackColor = Color.LightCoral;
                }
                if (chat_bad_symbol > 0)
                {
                    ListViewItem lvi18 = new ListViewItem("Введено запрещенных символов");
                    lvi18.SubItems.Add(Convert.ToString(chat_bad_symbol));
                    listEvents.Items.Add(lvi18).BackColor = Color.LightCoral;
                }
                if (server_null_ip > 0)
                {
                    ListViewItem lvi19 = new ListViewItem("Попытка блокировки сервера");
                    lvi19.SubItems.Add(Convert.ToString(server_null_ip));
                    listEvents.Items.Add(lvi19).BackColor = Color.LightCoral;
                }
                if (bad_players > 0)
                {
                    ListViewItem lvi20 = new ListViewItem("Отключено игроков");
                    lvi20.SubItems.Add(Convert.ToString(bad_players));
                    listEvents.Items.Add(lvi20).BackColor = Color.Orange;
                }
                if (ERROR > 0)
                {
                    ListViewItem lvi21 = new ListViewItem("Удалено объектов с ошибками");
                    lvi21.SubItems.Add(Convert.ToString(ERROR));
                    listEvents.Items.Add(lvi21).BackColor = Color.Violet;
                }
                if (chat_size_warning > 0)
                {
                    ListViewItem lvi22 = new ListViewItem("Попытка вызвать внутреннего нарушения в работе сервера длиной чата");
                    lvi22.SubItems.Add(Convert.ToString(chat_size_warning));
                    listEvents.Items.Add(lvi22).BackColor = Color.DeepPink;
                }
                if (ERROR_2 > 0)
                {
                    ListViewItem lvi23 = new ListViewItem("Попытка вызвать внутреннего нарушения в работе сервера с помощью снаряжения");
                    lvi23.SubItems.Add(Convert.ToString(ERROR_2));
                    listEvents.Items.Add(lvi23).BackColor = Color.DeepPink;
                }
                if (V_ERROR > 0)
                {
                    ListViewItem lvi24 = new ListViewItem("Попытка начать голосование использовав в нем запрещенные символы, или его длина превышает безопасное значение");
                    lvi24.SubItems.Add(Convert.ToString(V_ERROR));
                    listEvents.Items.Add(lvi24).BackColor = Color.DeepPink;
                }
                if (VOTE_START_BLOCK > 0)
                {
                    ListViewItem lvi25 = new ListViewItem("Попытка начать голосование использовав в нем запрещенные технические названия с целью нарушить работу сервера");
                    lvi25.SubItems.Add(Convert.ToString(VOTE_START_BLOCK));
                    listEvents.Items.Add(lvi25).BackColor = Color.DeepPink;
                }
                if (ERROR_3_PLAYER_UPDATE_ERROR > 0)
                {
                    ListViewItem lvi26 = new ListViewItem("Неизвестная ошибка или попытка краша: Попытка обновить сервером несуществующего клиента или он не был найден");
                    lvi26.SubItems.Add(Convert.ToString(ERROR_3_PLAYER_UPDATE_ERROR));
                    listEvents.Items.Add(lvi26).BackColor = Color.DodgerBlue;
                }
                if (artefact_activated > 0)
                {
                    ListViewItem lvi27 = new ListViewItem("Артефактов Активировано");
                    lvi27.SubItems.Add(Convert.ToString(artefact_activated));
                    listEvents.Items.Add(lvi27).BackColor = Color.Gold;
                }

                if (artefact + connected + player_found + kicked_by_server + medic_error + access_denied + logged_as + ip_attack + Invalid_cdkey + fatal_error + reg_load + ahtung + sv_destroy_error + m_change + send_size + unknown_cmd + packet_size + chat_bad_symbol + server_null_ip + bad_players + ERROR + chat_size_warning + ERROR_2 + V_ERROR + VOTE_START_BLOCK + ERROR_3_PLAYER_UPDATE_ERROR + artefact_activated == 0)
                {
                    listEvents.Items.Add("В настоящий момент событий сервера обнаружено не было.").BackColor = Color.Aqua;
                }
            }
            catch (Exception ex)
            {
                listEvents.Items.Add("Incorrect Index: " + ex.Message).BackColor = Color.Coral;
            }
        }
            

        private void btnExit_Click(object sender, EventArgs e)
        {
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
                    GUIEvents.BackColor = Color.FromArgb(192, 192, 255);
                    btnExit.BackColor = Color.FromArgb(192, 192, 255);
                }
                else
                {
                    GUIEvents.BackColor = color_rechange;
                    btnExit.BackColor = color_rechange;
                }
            }
            catch (Exception ex)
            {
                GUIEvents.BackColor = Color.FromArgb(192, 192, 255);
                btnExit.BackColor = Color.FromArgb(192, 192, 255);
                MessageBox.Show("Command init set color error\n" + ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void GUIEvents_Click(object sender, EventArgs e)
        {
            // Refresh();
        }
    }
}
