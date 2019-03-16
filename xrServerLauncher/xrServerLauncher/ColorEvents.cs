using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace S.E.R.V.E.R___Shadow_Of_Chernobyl_1._0006
{
    public partial class ColorEvents : Form
    {
        int arg = 0;
        int blocked_flag = 0;
        public ColorEvents()
        {
            InitializeComponent();
            DYNAMIC_SET_COLOR();
            ThreadEvents.RunWorkerAsync();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
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

        private void Thread_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (arg == 0)
                EventsLoad(0);
            else
                EventsLoad(1);
        }
        
        private void EventsLoad(int flag)
        {
            Thread.Sleep(500);
            SrvColorEvents.Items.Add("Please wait...").BackColor = Color.Lime;
            Thread.Sleep(1000);
            SrvColorEvents.Visible = false;
            SrvColorEvents.Items.Clear();

            if (flag == 0)
            {
                try
                {
                    int count = 0, events = 0, cheater_found = 0, cheater_hack = 0, chat_hack = 0, blocked_players = 0, using_weapons = 0;
                    foreach (string s in ServerBasePlayers.BaseEvents)
                    {
                        count++;
                        var color = s.Split()[0];
                        if (color == "!")
                        {
                            events++;
                            SrvColorEvents.Items.Add("[!] " + s.Replace("!", "")).ForeColor = Color.Red;
                        }
                        else if (color == "~")
                        {
                            blocked_players++;
                            SrvColorEvents.Items.Add("[BLOCKED] " + s.Replace("~", "")).ForeColor = Color.Lime;
                        }
                        else if (color == "#")
                        {
                            cheater_found++;
                            SrvColorEvents.Items.Add("[CHEATER] => " + s.Replace("#", "")).ForeColor = Color.Gold;
                        }
                        else if (color == "@")
                        {
                            using_weapons++;
                            SrvColorEvents.Items.Add("[HWEAPONS] " + s.Replace("@", "")).ForeColor = Color.Blue;
                        }
                        else if (color == "+")
                        {
                            cheater_hack++;
                            SrvColorEvents.Items.Add("[CHEATER + BAD HIT POWER && BULLETS] => " + s.Replace("+", "")).ForeColor = Color.Violet;
                        }
                        else if (color == "%")
                        {
                            chat_hack++;
                            SrvColorEvents.Items.Add("[CHAT HACK] => " + s.Replace("%", "")).BackColor = Color.Violet;
                        }
                    }
                    /*
                    if (count == 0)
                    {
                        SrvColorEvents.Items.Add("В настоящий момент событий сервера обнаружено не было.").BackColor = Color.Aqua;
                    }
                    else
                    {
                        SrvColorEvents.Items.Add("Итого:").BackColor = Color.Honeydew;
                    }
                    if (cheater_found > 0)
                    {
                        SrvColorEvents.Items.Add("[CHEATER] Выявлено несоответствующих параметров оружия [HIM, HP]: " + cheater_found).ForeColor = Color.Gold;
                    }
                    if (cheater_hack > 0)
                    {
                        SrvColorEvents.Items.Add("[CHEATER + BAD HIT POWER && BULLETS] Выявлено несоответствующих параметров выстрелов [HP]: " + cheater_hack).ForeColor = Color.Gold;
                    }
                    if (blocked_players > 0)
                    {
                        SrvColorEvents.Items.Add("[BLOCKED] Заблокировано нарушителей: " + blocked_players).ForeColor = Color.Lime;
                    }
                    if (using_weapons > 0)
                    {
                        SrvColorEvents.Items.Add("[HWEAPONS] Использовано тяжелого вооружения: " + using_weapons).ForeColor = Color.Blue;
                    }
                    if (chat_hack > 0)
                    {
                        SrvColorEvents.Items.Add("[CHAT HACK] Выявлено попыток отправить в чат сообщение длиной более > 200").BackColor = Color.Violet;
                    }
                    if (events > 0)
                    {
                        SrvColorEvents.Items.Add("[!] Других событий: " + events).ForeColor = Color.Red;
                    }*/
                }
                catch (Exception ex)
                {
                    SrvColorEvents.Items.Add("[ERROR] " + ex.Message).BackColor = Color.Red;
                }
            }
            else
            {
                SrvColorEvents.Items.Clear();
                foreach (string s in ServerBasePlayers.BaseEvents)
                {
                    blocked_flag = 0;
                    if (s.Contains("name: "))
                    {
                        var pName = s.Split()[1];
                        var pAddr = s.Split()[5];
                        foreach (string blocked_info in ServerBasePlayers.BaseEvents)
                        {
                            if (blocked_info.Contains("Banned Address: " + pAddr))
                            {
                                blocked_flag = 2;
                            }
                            else if (blocked_info.Contains("# " + pName + " ["))
                            {
                                blocked_flag = 4;
                            }
                            else if (blocked_info.Contains("@ Weapons Flags -> * " + pName))
                            {
                                blocked_flag = 8;
                            }
                        }

                        if (blocked_flag == 2)
                        {
                            SrvColorEvents.Items.Add("[BLOCKED] " + s.Replace("~", "")).BackColor = Color.Lime;
                        }
                        else if (blocked_flag == 4)
                        {
                            SrvColorEvents.Items.Add("[CHEATER] => " + s.Replace("#", "")).BackColor = Color.Violet;
                        }
                        else if (blocked_flag == 8)
                        {
                            SrvColorEvents.Items.Add("[HWEAPONS] => " + s.Replace("#", "")).BackColor = Color.DarkOrange;
                        }
                        else
                        {
                            SrvColorEvents.Items.Add(s.Replace("#", "")).BackColor = Color.Wheat;
                        }
                    }
                    else if (s.Contains("Banning by id "))
                    {
                        SrvColorEvents.Items.Add("[PATCH] " + s).BackColor = Color.Lime;
                    }
                }
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

        private void ThreadEvents_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            SrvColorEvents.Visible = true;
        }

        private void GetPlayersHistory_Click(object sender, EventArgs e)
        {
            if (ThreadEvents.IsBusy == false)
            {
                arg = 1;
                ThreadEvents.RunWorkerAsync();
            }
            else
            {
                MessageBox.Show("Обработка уже выполняется, пожалуйста подождите ее завершения.", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void GetEventsHistory_Click(object sender, EventArgs e)
        {
            if (ThreadEvents.IsBusy == false)
            {
                arg = 0;
                ThreadEvents.RunWorkerAsync();
            }
            else
            {
                MessageBox.Show("Обработка уже выполняется, пожалуйста подождите ее завершения.", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
