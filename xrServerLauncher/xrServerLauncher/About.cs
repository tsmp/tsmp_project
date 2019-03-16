using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace S.E.R.V.E.R___Shadow_Of_Chernobyl_1._0006
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
            DYNAMIC_SET_COLOR();
            text.Text = "S.E.R.V.E.R - Shadow Of Chernobyl v " + Version.get_version +"\nСоздание и мониторинг работы серверов.\nАвтоматическое создание баз данных и Авто-проверка игроков.";
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
        private void link_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start("http://team-stalker.ucoz.com/");
            }
            catch (Exception) { }
        }

        private void linkExit_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Close();
        }

        private void ui_events_Click(object sender, EventArgs e)
        {
            ui_informer.Text = "Статистика событий сервера";
        }

        private void ui_error_Click(object sender, EventArgs e)
        {
            ui_informer.Text = "Журнал ошибок и событий сервера";
        }

        private void ui_update_Click(object sender, EventArgs e)
        {
            ui_informer.Text = "Центр обновления S.E.R.V.E.R";
        }

        private void ui_save_Click(object sender, EventArgs e)
        {
            ui_informer.Text = "Сохранить настройки программы";
        }

        private void ui_base_Click(object sender, EventArgs e)
        {
            ui_informer.Text = "Автоматическая база данных и события сервера";
        }

        private void ui_settings_Click(object sender, EventArgs e)
        {
            ui_informer.Text = "Настройки сервера";
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
                    linkExit.BackColor = Color.FromArgb(192, 192, 255);
                }
                else
                {
                    GUI.BackColor = color_rechange;
                    linkExit.BackColor = color_rechange;
                }
            }
            catch (Exception ex)
            {
                GUI.BackColor = Color.FromArgb(192, 192, 255);
                linkExit.BackColor = Color.FromArgb(192, 192, 255);
                MessageBox.Show("Command init set color error\n" + ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btn_check_update_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            get_version();
        }

        public void get_version()
        {
            try
            {
                string pText = new WebClient().DownloadString("http://team-stalker.ucoz.com/update/update.txt");             
                if (pText == Version.get_version)
                {
                    MessageBox.Show("Обновление не требуется\nУстановлена самая новая версия программы", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    DialogResult ok = MessageBox.Show("Обнаружена новая версия программы: " + pText + "\nНажмите Да чтобы обновить версию.\nПрограмма обновится в автоматическом режиме.\nВсе настройки будут сохранены.", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (DialogResult.OK == ok)
                    {
                        WebClient webClient = new WebClient();
                        webClient.DownloadFile("http://team-stalker.ucoz.com/update/srv_update.tx", "update.txt");
                        File.Move(@"S.E.R.V.E.R - Shadow Of Chernobyl 1.0006.exe", @"S.E.R.V.E.R - Shadow Of Chernobyl 1.0006.exeOLD");
                        File.Move(@"update.txt", @"S.E.R.V.E.R - Shadow Of Chernobyl 1.0006.exe");
                        Thread.Sleep(500);
                        Application.Restart();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}