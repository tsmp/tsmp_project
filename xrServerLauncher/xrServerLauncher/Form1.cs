/* Stalker Shadow Of Chernobyl Server Monitoring: 2017, 2018, 2019
 * denisufa12345@gmail.com
 * Сайт программы: http://team-stalker.ucoz.com/
 * 
 *
 * SendKeys https://msdn.microsoft.com/ru-ru/library/system.windows.forms.sendkeys.aspx
 * https://habrahabr.ru/post/131971/
 * IPSec http://winintro.ru/netsh_technicalreference.en/html/3cf22f34-e520-4d34-9728-3f37ae0e0612.htm
 */
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Management;
using System.Net.Mail;
using System.Net;

namespace S.E.R.V.E.R___Shadow_Of_Chernobyl_1._0006
{
    public partial class FormMenu1 : Form
    {
        int CPU_INFO = Environment.ProcessorCount;
        string current_directory = Environment.CurrentDirectory;
        SettingsMain _settings = null;
        public FormMenu1()
        {
            InitializeComponent();
            ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss] ")) + "Server Version " + Version.get_version).ForeColor = Color.White;
            _settings = SettingsMain.GetSettingsMain();
            ProcessorCount();
            Initialize_save_form();   
            FileSystemManager.GetLoadReNameMaps();
            FileSystemManager.CheckedDirectoryFiles();
            FileSystemManager.GetCheckMods();
            InitializeScanMaps();

            //CheckBannedList();                    

            UI_TIME.Text = DateTime.Now.ToString("HH:mm");
            ServerEvents.Items.Add("[Помощь]: Нажмите F3 для загрузки AdminAssistant.").ForeColor = Color.White;
            ServerEvents.Items.Add("[Помощь]: Нажмите F4 для открытия текущего log-файла").ForeColor = Color.White;
            ServerEvents.Items.Add("[Помощь]: Нажмите F5 для открытия директории log-файлов").ForeColor = Color.White;

            // Check Version Update
            if (File.Exists(@"S.E.R.V.E.R - Shadow Of Chernobyl 1.0006.exeOLD"))
            {
                try
                {
                    File.Delete("S.E.R.V.E.R - Shadow Of Chernobyl 1.0006.exeOLD");
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss] ")) + "The program has been successfully updated!").ForeColor = Color.Lime;
                    MessageBox.Show("Программа успешно обновлена!\nСписок изменений:\n" + Version.get_description, "Обновление выполнено успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        // 29 slots
        string[] buffer_listmaps = { "l01_escape", "l02_garbage", "l03_agroprom", "l03u_agr_underground", "l04_darkvalley", "l04u_labx18", "l05_bar", "l06_rostok", "l07_military", "l08_yantar", "l08u_brainlab", "l10_radar", "l10u_bunker", "l11_pripyat", "l12_stancia", "l12_stancia_2", "l12u_sarcofag", "l12u_control_monolith", "testers_mp_atp", "testers_mp_military_1", "testers_mp_agroprom", "testers_mp_factory", "testers_mp_rostok", "testers_mp_darkvalley", "testers_mp_workshop", "testers_mp_lost_village", "testers_mp_railroad", "testers_mp_bath", "testers_mp_pool" };
        private void InitializeScanMaps()
        {
            try
            {
                int maps_collection = 0;
                string formatfile;
                string[] find_maps = Directory.GetFiles("mods", "*.xdb0", SearchOption.AllDirectories); // Находим нужные нам файлы по маске *.xdb0
                ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss] ")) + "[FS]: Find new maps...").ForeColor = Color.LightSeaGreen;
                foreach (var Collection in find_maps)
                {
                    formatfile = Path.GetExtension(Collection);                              // Получим расширение файлов
                    string new_maps = Collection.Replace("mods\\", "").Replace(".xdb0", ""); // Убираем все ненужное, оставляем только название карты
                    if (formatfile == ".xdb0")                                               // Добавим в список карт наши карты
                    {
                        maps_collection++;
                        /*
                        foreach (string FS_SCANMAPS in buffer_listmaps)
                        {
                            if (FS_SCANMAPS.Contains(new_maps))
                            {
                                SrvMaps1.Items.Add(new_maps);
                                SrvMaps2.Items.Add(new_maps);
                                SrvMaps3.Items.Add(new_maps);
                                SrvMaps4.Items.Add(new_maps);
                                SrvMaps5.Items.Add(new_maps);
                                ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss] ")) + "[FS]: New maps created to list: " + FS_SCANMAPS).ForeColor = Color.Red;
                            }
                        }*/
                        SrvMaps1.Items.Add(new_maps);
                        SrvMaps2.Items.Add(new_maps);
                        SrvMaps3.Items.Add(new_maps);
                        SrvMaps4.Items.Add(new_maps);
                        SrvMaps5.Items.Add(new_maps);
                        ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss] ")) + "[FS]: New maps: " + new_maps).ForeColor = Color.LightSeaGreen;
                    }
                }
                if (maps_collection > 0)
                {
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss] ")) + "[FS]: New maps found and added maps collection: " + maps_collection).ForeColor = Color.LightSeaGreen;
                }
                else
                {
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss] ")) + "[FS]: New maps not found").ForeColor = Color.LightSeaGreen;
                }
            }
            catch (Exception ex)
            {
                ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss] ")) + "[FS]: Error: " + ex.Message).ForeColor = Color.DarkOrange;
            }
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

        private void UI_TIME_UPDATE_Tick(object sender, EventArgs e)
        {
            UI_TIME.Text = DateTime.Now.ToString("HH:mm");
            if (SrvWeatherTime.CheckState == CheckState.Checked)
            {
                svWeatherTime.Text = UI_TIME.Text;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show(SystemInformation.UserName + "! Вы действительно хотите завершить работу программы\nи остановить обслуживание серверов?\nСохранить настройки программы?", "Завершение работы программы", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
            if (dialogResult == DialogResult.Yes)
            {
                save_settings();
                Application.Exit();
            }
            else if (dialogResult == DialogResult.No)
            {
                Application.Exit();
            }
        }
        private void btnMinimized_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void btnStartStop1_Click(object sender, EventArgs e)
        {
            if (btnStartStop1.Text == "Старт")
            {
                btnStartStop1.Text = "Стоп";
                LoadingServer1();
                Srv1ProcessScan.Start();
                Srv1Time.Start();
                ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss] ")) + "Server [1] started by user").ForeColor = Color.Lime;
            }
            else
            {
                ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss] ")) + "Server [1] Terminated by user").ForeColor = Color.LightCoral;
                info_sv_time1.Text = "Server is stopped";
                btnStartStop1.Text = "Старт";
                info_sv_reconnect1.Text = "";
                srv1_reconnection_counter = 0;
                Srv1ProcessScan.Stop();
                Srv1Time.Stop();
                try
                {
                    Process Id = Process.GetProcessById(PID_SRV1);
                    Id.Kill();
                }
                catch (Exception)
                {
                    MessageBox.Show("Запрашиваемый сервер не отвечает! " + PID_SRV1, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void btnStartStop2_Click(object sender, EventArgs e)
        {
            if (btnStartStop2.Text == "Старт")
            {
                btnStartStop2.Text = "Стоп";
                LoadingServer2();
                Srv2ProcessScan.Start();
                Srv2Time.Start();
                ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss] ")) + "Server [2] started by user").ForeColor = Color.Lime;
            }
            else
            {
                ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss] ")) + "Server [2] Terminated by user").ForeColor = Color.LightCoral;
                info_sv_time2.Text = "Server is stopped";
                btnStartStop2.Text = "Старт";
                info_sv_reconnect2.Text = "";
                srv2_reconnection_counter = 0;
                Srv2ProcessScan.Stop();
                Srv2Time.Stop();
                try
                {
                    Process Id = Process.GetProcessById(PID_SRV2);
                    Id.Kill();
                }
                catch (Exception)
                {
                    MessageBox.Show("Запрашиваемый сервер не отвечает! " + PID_SRV2, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void btnStartStop3_Click(object sender, EventArgs e)
        {
            if (btnStartStop3.Text == "Старт")
            {
                btnStartStop3.Text = "Стоп";
                LoadingServer3();
                Srv3ProcessScan.Start();
                Srv3Time.Start();
                ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss] ")) + "Server [3] started by user").ForeColor = Color.Lime;
            }
            else
            {
                ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss] ")) + "Server [3] Terminated by user").ForeColor = Color.LightCoral;
                info_sv_time3.Text = "Server is stopped";
                btnStartStop3.Text = "Старт";
                info_sv_reconnect3.Text = "";
                srv3_reconnection_counter = 0;
                Srv3ProcessScan.Stop();
                Srv3Time.Stop();
                try
                {
                    Process Id = Process.GetProcessById(PID_SRV3);
                    Id.Kill();
                }
                catch (Exception)
                {
                    MessageBox.Show("Запрашиваемый сервер не отвечает! " + PID_SRV3, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void btnStartStop4_Click(object sender, EventArgs e)
        {
            if (btnStartStop4.Text == "Старт")
            {
                btnStartStop4.Text = "Стоп";
                LoadingServer4();
                Srv4ProcessScan.Start();
                Srv4Time.Start();
                ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss] ")) + "Server [4] started by user").ForeColor = Color.Lime;
            }
            else
            {
                ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss] ")) + "Server [4] Terminated by user").ForeColor = Color.LightCoral;
                info_sv_time4.Text = "Server is stopped";
                btnStartStop4.Text = "Старт";
                info_sv_reconnect4.Text = "";
                srv4_reconnection_counter = 0;
                Srv4ProcessScan.Stop();
                Srv4Time.Stop();
                try
                {
                    Process Id = Process.GetProcessById(PID4);
                    Id.Kill();
                }
                catch (Exception)
                {
                    MessageBox.Show("Запрашиваемый сервер не отвечает! " + PID4, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void btnStartStop5_Click(object sender, EventArgs e)
        {
            if (btnStartStop5.Text == "Старт")
            {
                btnStartStop5.Text = "Стоп";
                LoadingServer5();
                Srv5ProcessScan.Start();
                Srv5Time.Start();
                ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss] ")) + "Server [5] started by user").ForeColor = Color.Lime;
            }
            else
            {
                ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss] ")) + "Server [5] Terminated by user").ForeColor = Color.LightCoral;
                info_sv_time5.Text = "Server is stopped";
                btnStartStop5.Text = "Старт";
                info_sv_reconnect5.Text = "";
                srv5_reconnection_counter = 0;
                Srv5ProcessScan.Stop();
                Srv5Time.Stop();
                try
                {
                    Process Id = Process.GetProcessById(PID5);
                    Id.Kill();
                }
                catch (Exception)
                {
                    MessageBox.Show("Запрашиваемый сервер не отвечает!", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        // ==================================START SERVER 1==================================
        int PID_SRV1;
        int PID_SRV2;
        int PID_SRV3;
        int srv1_running_time;          
        int srv2_running_time;
        int srv3_running_time;
        int srv4_running_time;
        int srv5_running_time;
        int srv1_reconnection_counter;
        int srv2_reconnection_counter;
        int srv3_reconnection_counter;
        int srv4_reconnection_counter;
        int srv5_reconnection_counter;
        string status = " -$sv_status";  
        string KeySV1;                     
        string ServerOnline = "1";       
        string NAT_IP_ATTACH = "localhost";


        // Патч от Maks0 
        // ==================================
        string RUS_CHAT1;     // -rus_test      - Русский чат 
        string RUS_CHAT2;
        string RUS_CHAT3;
        string NO_SPEECH1;    // -nospeech      - Отключает рацию
        string NO_SPEECH2;
        string NO_SPEECH3;
        string NO_KEY1;       // -check_cd_key  - Включить проверку на наличие ключа
        string NO_KEY2;
        string NO_KEY3;
        string DEBUG_MODE1;   // -debug         - Дебаг сообщения
        string DEBUG_MODE2;   // -noalwaysflush - Отключает подробную запись логи и stack trace
        string DEBUG_MODE3;
        string check;         // -noalwaysflush - Записывает данные в файл в реальном времени
        string server_new_engine;          // Переключение на новую версию движка
        // ==================================
        string StartKeyArgument;

        private void LoadingServer1()
        {
            srv1_running_time = 0;
            CloseErrorDialog();
            SrvName1.Text = SrvName1.Text.Replace("_", " ").Replace("%", " ");
            if (SrvPsw1.Text == "")
                KeySV1 = "";
            else
                KeySV1 = "/psw=" + SrvPsw1.Text;

            if (SrvStartArgument.CheckState == CheckState.Checked && myKeyCreated.TextLength > 0)
            {
                StartKeyArgument = myKeyCreated.Text;
                ParseStartArgument();
            }

            try
            {
                btnStartStop1.Text = "Стоп";
                info_sv_time1.Text = "Loading...";
                if (checkLevel4.CheckState == CheckState.Unchecked)
                {
                    using (Process process = Process.Start(@"bin" + server_new_engine + "\\xr_3da.exe", "-i -nosound -noprefetch " + StartKeyArgument + check + RUS_CHAT1 + NO_KEY1 + NO_SPEECH1 + DEBUG_MODE1 + status + " -start server(" + SrvMaps1.Text + "/" + SrvGameType1.Text + KeySV1 + "/hname=" + SrvName1.Text + "/maxplayers=" + svPlayers.Text + "/public=" + ServerOnline + "/battleye=1/maxping=" + svPing.Text + "/spectr=20/spectrmds=31/vote=1/dmgblock=0/fraglimit=" + svFraglim.Text + "/timelimit=" + svTimeLim.Text + "/ffire=" + svFriendlyFire.Text + "/fn=" + svIco.Text + "/fi=" + svIco.Text + "/ans=0/warmup=" + svWurmUp.Text + "/etimef=0.0/estime=" + svWeatherTime.Text + "/portgs=" + PortGS1.Text + "/portsv=" + PortSV1.Text + "portcl=" + PortCL1.Text + "/anum=" + Artefacts.Text + "/astime=" + svTimeArtefact.Text + ") client(" + NAT_IP_ATTACH + "/name=server)"))
                    {
                        PID_SRV1 = process.Id;
                    }
                    /*
                    Process proc = new Process();
                    proc.StartInfo.FileName = current_directory + @"\bin" + server_new_engine + "\\dedicated\\XR_3DA.exe";
                    proc.StartInfo.WorkingDirectory = current_directory + @"\bin" + server_new_engine + "\\";
                    proc.StartInfo.Arguments = "-i -nosound -noprefetch " + StartKeyArgument + check + RUS_CHAT1 + NO_KEY1 + NO_SPEECH1 + DEBUG_MODE1 + status + " -start server(" + SrvMaps1.Text + "/" + SrvGameType1.Text + KeySV1 + "/hname=" + SrvName1.Text + "/maxplayers=" + svPlayers.Text + "/public=" + ServerOnline + "/battleye=1/maxping=" + svPing.Text + "/spectr=20/spectrmds=31/vote=1/dmgblock=0/fraglimit=" + svFraglim.Text + "/timelimit=" + svTimeLim.Text + "/ffire=" + svFriendlyFire.Text + "/fn=" + svIco.Text + "/fi=" + svIco.Text + "/ans=0/warmup=" + svWurmUp.Text + "/etimef=0.0/estime=" + svWeatherTime.Text + "/portgs=" + PortGS1.Text + "/portsv=" + PortSV1.Text + "portcl = " + PortCL1.Text + "/anum=" + Artefacts.Text + "/astime=" + svTimeArtefact.Text + ") client(localhost/name=server";
                    proc.Start();
                    PID_SRV1 = proc.Id;*/
                }
                else // SURVIVAL MODE
                {
                    using (Process process = Process.Start(@"bin" + server_new_engine + "\\xr_3da.exe", "-i -nosound -nolog" + StartKeyArgument + RUS_CHAT1 + NO_KEY1 + NO_SPEECH1 + DEBUG_MODE1 + " -start server(" + SrvMaps1.Text + "/" + SrvGameType1.Text + KeySV1 + "/hname=" + SrvName1.Text + "[survival]/maxplayers=" + svPlayers.Text + "/public=" + ServerOnline + "/battleye=1/maxping=" + svPing.Text + "/spectr=20/spectrmds=31/vote=1/dmgblock=0/fraglimit=50/timelimit=60/frcrspwn=1/ans=1/warmup=60/etimef=0.0/estime=21:00/portgs=" + PortGS1.Text + "/portsv=" + PortSV1.Text + "portcl=" + PortCL1.Text + ") -$sv_no_auth_check 1 -client(" + NAT_IP_ATTACH + "/name=server)"))
                    {
                        PID_SRV1 = process.Id;
                        ProcessStartAsm();
                    }
                }
                /*
                Process proc = new Process();
                proc.StartInfo.FileName = current_directory + @"\bin" + server_new_engine + "\\dedicated\\XR_3DA.exe";
                proc.StartInfo.WorkingDirectory = current_directory + @"\bin" + server_new_engine + "\\";
                proc.StartInfo.Arguments = "-i -nosound -noprefetch " + StartKeyArgument + check + RUS_CHAT1 + NO_KEY1 + NO_SPEECH1 + DEBUG_MODE1 + status + " -start server(" + SrvMaps1.Text + "/" + SrvGameType1.Text + KeySV1 + "/hname=" + SrvName1.Text + srv1_crash_informer + "/maxplayers=" + svPlayers.Text + "/public=" + ServerOnline + "/battleye=1/maxping=" + svPing.Text + "/spectr=20/spectrmds=31/vote=1/dmgblock=0/fraglimit=" + svFraglim.Text + "/timelimit=" + svTimeLim.Text + "/ffire=" + svFriendlyFire.Text + "/fn=" + svIco.Text + "/fi=" + svIco.Text + "/ans=0/warmup=" + svWurmUp.Text + "/etimef=0.0/estime=" + svWeatherTime.Text + "/portgs=" + PortGS1.Text + "/portsv=" + PortSV1.Text + "portcl = " + PortCL1.Text + "/anum=" + Artefacts.Text + "/astime=" + svTimeArtefact.Text + ") client(localhost/name=server";
                proc.Start();
                PID_SRV1 = proc.Id;*/

                if (SRV_PROTECT1.CheckState == CheckState.Checked)
                    ProcessStartAsm();

                FunctionServer1UsingCore();
                FuncPriorityClassChangeServer1();
            }

            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки исполняемых файлов сервера.\nКод ошибки:\n" + ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ==================================START SERVER 2==================================
        string KeySV2;
        private void LoadingServer2()
        {
            srv2_running_time = 0;
            CloseErrorDialog();
            SrvName2.Text = SrvName2.Text.Replace("_", " ").Replace("%", " ");
            if (SrvPsw2.Text == "")
                KeySV2 = "";
            else
                KeySV2 = "/psw=" + SrvPsw2.Text;

            if (SrvStartArgument.CheckState == CheckState.Checked && myKeyCreated.TextLength > 0)
            {
                StartKeyArgument = myKeyCreated.Text;
                ParseStartArgument();
            }

            try
            {
                btnStartStop2.Text = "Стоп";
                info_sv_time2.Text = "Loading...";
                /*
                Process proc = new Process();
                proc.StartInfo.FileName = current_directory + @"\bin" + server_new_engine + "\\dedicated\\XR_3DA.exe";
                proc.StartInfo.WorkingDirectory = current_directory + @"\bin" + server_new_engine + "\\";
                proc.StartInfo.Arguments = ("-i -nosound -noprefetch " + StartKeyArgument + check + RUS_CHAT2 + NO_KEY2 + NO_SPEECH2 + DEBUG_MODE2 + status + " -start server(" + SrvMaps2.Text + "/" + SrvGameType2.Text + KeySV2 + "/hname=" + SrvName2.Text + srv2_crash_informer + "/maxplayers=" + svPlayers.Text + "/public=" + ServerOnline + "/battleye=1/maxping=" + svPing.Text + "/spectr=20/spectrmds=31/vote=1/dmgblock=0/fraglimit=" + svFraglim.Text + "/timelimit=" + svTimeLim.Text + "/ffire=" + svFriendlyFire.Text + "/fn=" + svIco.Text + "/fi=" + svIco.Text + "/ans=0/warmup=" + svWurmUp.Text + "/etimef=0.0/estime=" + svWeatherTime.Text + "/portgs=" + PortGS2.Text + "/portsv=" + PortSV2.Text + "portcl = " + PortCL2.Text + "/anum=" + Artefacts.Text + "/astime=" + svTimeArtefact.Text + ") client(localhost/name=server");             
                proc.Start();
                PID_SRV2 = proc.Id;*/

                using (Process process = Process.Start(@"bin" + server_new_engine + "\\xr_3da.exe", "-i -nosound -noprefetch " + StartKeyArgument + check + RUS_CHAT2 + NO_KEY2 + NO_SPEECH2 + DEBUG_MODE2 + status + " -start server(" + SrvMaps2.Text + "/" + SrvGameType2.Text + KeySV2 + "/hname=" + SrvName2.Text + "/maxplayers=" + svPlayers.Text + "/public=" + ServerOnline + "/battleye=1/maxping=" + svPing.Text + "/spectr=20/spectrmds=31/vote=1/dmgblock=0/fraglimit=" + svFraglim.Text + "/timelimit=" + svTimeLim.Text + "/ffire=" + svFriendlyFire.Text + "/fn=" + svIco.Text + "/fi=" + svIco.Text + "/ans=0/warmup=" + svWurmUp.Text + "/etimef=0.0/estime=" + svWeatherTime.Text + "/portgs=" + PortGS2.Text + "/portsv=" + PortSV2.Text + "portcl=" + PortCL2.Text + "/anum=" + Artefacts.Text + "/astime=" + svTimeArtefact.Text + ") client(" + NAT_IP_ATTACH + "/name=server)"))
                {
                    PID_SRV2 = process.Id;
                }

                if (SRV_PROTECT2.CheckState == CheckState.Checked)
                    ProcessStartAsm();


                Thread.Sleep(100);
                FunctionServer2UsingCore();
                FuncPriorityClassChangeServer2();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки исполняемых файлов сервера.\nКод ошибки:\n" + ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ==================================START SERVER 3==================================
        string KeySV3;
        private void LoadingServer3()
        {
            srv3_running_time = 0;
            CloseErrorDialog();
            SrvName3.Text = SrvName3.Text.Replace("_", " ").Replace("%", " ");
            if (SrvPsw3.Text == "")
                KeySV3 = "";
            else
                KeySV3 = "/psw=" + SrvPsw3.Text;

            if (SrvStartArgument.CheckState == CheckState.Checked && myKeyCreated.TextLength > 0)
            {
                StartKeyArgument = myKeyCreated.Text;
                ParseStartArgument();
            }

            try
            {
                btnStartStop3.Text = "Стоп";
                info_sv_time3.Text = "Loading...";
                /*
                Process proc = new Process();
                proc.StartInfo.FileName = current_directory + @"\bin" + server_new_engine + "\\dedicated\\XR_3DA.exe";
                proc.StartInfo.WorkingDirectory = current_directory + @"\bin" + server_new_engine + "\\";
                proc.StartInfo.Arguments = ("-i -nosound -noprefetch " + StartKeyArgument + check + RUS_CHAT3 + NO_KEY3 + NO_SPEECH3 + DEBUG_MODE3 + status + " -start server(" + SrvMaps3.Text + "/" + SrvGameType3.Text + KeySV3 + "/hname=" + SrvName3.Text + srv3_crash_informer + "/maxplayers=" + svPlayers.Text + "/public=" + ServerOnline + "/battleye=1/maxping=" + svPing.Text + "/spectr=20/spectrmds=31/vote=1/dmgblock=0/fraglimit=" + svFraglim.Text + "/timelimit=" + svTimeLim.Text + "/ffire=" + svFriendlyFire.Text + "/fn=" + svIco.Text + "/fi=" + svIco.Text + "/ans=0/warmup=" + svWurmUp.Text + "/etimef=0.0/estime=" + svWeatherTime.Text + "/portgs=" + PortGS3.Text + "/portsv=" + PortSV3.Text + "portcl=" + PortCL3.Text + "/anum=" + Artefacts.Text + "/astime=" + svTimeArtefact.Text + ") client(localhost/name=server");
                proc.Start();
                PID_SRV3 = proc.Id;*/

                using (Process process = Process.Start(@"bin" + server_new_engine + "\\xr_3da.exe", "-i -nosound -noprefetch " + StartKeyArgument + check + RUS_CHAT3 + NO_KEY3 + NO_SPEECH3 + DEBUG_MODE3 + status + " -start server(" + SrvMaps3.Text + "/" + SrvGameType3.Text + KeySV3 + "/hname=" + SrvName3.Text + "/maxplayers=" + svPlayers.Text + "/public=" + ServerOnline + "/battleye=1/maxping=" + svPing.Text + "/spectr=20/spectrmds=31/vote=1/dmgblock=0/fraglimit=" + svFraglim.Text + "/timelimit=" + svTimeLim.Text + "/ffire=" + svFriendlyFire.Text + "/fn=" + svIco.Text + "/fi=" + svIco.Text + "/ans=0/warmup=" + svWurmUp.Text + "/etimef=0.0/estime=" + svWeatherTime.Text + "/portgs=" + PortGS3.Text + "/portsv=" + PortSV3.Text + "portcl=" + PortCL3.Text + "/anum=" + Artefacts.Text + "/astime=" + svTimeArtefact.Text + ") client(" + NAT_IP_ATTACH + "/name=server)"))
                {
                    PID_SRV3 = process.Id;
                }
                if (SRV_PROTECT3.CheckState == CheckState.Checked)
                    ProcessStartAsm();

                Thread.Sleep(100);
                FunctionServer3UsingCore();
                FuncPriorityClassChangeServer3();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки исполняемых файлов сервера.\nКод ошибки:\n" + ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ==================================START SERVER 4================================== https://docs.microsoft.com/en-us/windows/desktop/cimwin32prov/win32-process
        string KeySV4;
        int PID4, GetPIDSV4;               // Записываем PID процесса, если он действительно есть
        private void LoadingServer4()
        {
            srv4_running_time = 0;
            CloseErrorDialog();
            SrvName4.Text = SrvName4.Text.Replace("_", " ").Replace("%", " ");
            if (SrvPsw4.Text == "")
                KeySV4 = "";
            else
                KeySV4 = "/psw=" + SrvPsw4.Text;

            if (SrvStartArgument.CheckState == CheckState.Checked && myKeyCreated.TextLength > 0)
            {
                StartKeyArgument = myKeyCreated.Text;
                ParseStartArgument();
            }

            try
            {
                btnStartStop4.Text = "Стоп";
                info_sv_time4.Text = "Loading...";

                // Call Of Pripyat
                /*
                Process load = new Process();
                load.StartInfo.FileName = "soLauncher.exe";
                //proc.StartInfo.WorkingDirectory = current_directory + @"\soProject\";
                load.StartInfo.Arguments = ("-dedicated -perfhud_hack -i -nosound -silent_error_mode " + StartKeyArgument +" -start server("+SrvMaps4.Text+"/tdm"+ KeySV4+"/ver=1.0/hname=" +SrvName4.Text+"/portsv="+PortSV4.Text+"/portgs="+PortGS4.Text+"/maxplayers="+svPlayers.Text+ "/public=" + ServerOnline + "estime=9:00/etimef=1.0/ans=1/anslen=3/pdahunt=0/warmup=0/timelimit=0/dmgblock=0/dmbi=0/fraglimit=0/spectrmds=31/vote=26/frcrspwn=0/abalance=0/aswap=0/fi=0/fn=0/ffire=1.0) client(" + NAT_IP_ATTACH+"/portcl="+PortCL4.Text);
                load.Start();*/

                ProcessStartInfo CONTROLLER = new ProcessStartInfo(current_directory + @"\bin\stalker_csoc.exe", @"bin\dedicated\xr_3da.exe -i -nosound -noprefetch " + StartKeyArgument + " -$sv_status -start server(" + SrvMaps4.Text + "/" + SrvGameType4.Text + KeySV4 + "/hname=" + SrvName4.Text + "/maxplayers=" + svPlayers.Text + "/public=" + ServerOnline + "/battleye=1/maxping=" + svPing.Text + "/spectr=20/spectrmds=31/vote=1/dmgblock=0/fraglimit=" + svFraglim.Text + "/timelimit=" + svTimeLim.Text + "/ffire=" + svFriendlyFire.Text + "/fn=" + svIco.Text + "/fi=" + svIco.Text + "/ans=0/warmup=" + svWurmUp.Text + "/etimef=0.0/estime=" + svWeatherTime.Text + "/portgs=" + PortGS4.Text + "/portsv=" + PortSV4.Text + "portcl=" + PortCL4.Text + "/anum=" + Artefacts.Text + "/astime=" + svTimeArtefact.Text + ") client(" + NAT_IP_ATTACH + "/name=server)");
                CONTROLLER.WorkingDirectory = Path.GetDirectoryName(CONTROLLER.FileName);
                Process load = Process.Start(CONTROLLER);
                // ========================= GET CHILD PID ========================= 
                Thread.Sleep(500);

                var search = new ManagementObjectSearcher("root\\CIMV2", "SELECT ProcessId FROM Win32_Process WHERE ParentProcessId = " + load.Id);
                foreach (var PIDResult in search.Get())
                {
                    var a = (uint)PIDResult["ProcessID"];
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss]")) + "[SYSTEM]: SERVER[4] PID PROCESS => " + a).ForeColor = Color.Coral;
                    GetPIDSV4 = Convert.ToInt32(a);
                }
                Thread.Sleep(500);
                if (GetPIDSV4 != 0)                         // Процесс не должен быть равен нулю, иначе мы его останавливаем. 
                {
                    try                                     // Проверяем точно ли существует наш процесс, и если он существует, то продолжаем работу
                    {
                        Process.GetProcessById(GetPIDSV4);
                        PID4 = GetPIDSV4;                   // если процесс действительно существует, то берем его под контроль. 
                        FunctionServer4UsingCore();
                        FuncPriorityClassChangeServer4();
                    }
                    catch (Exception ex)                    // Потеряли pid сервера... останавливаем контроль.
                    {
                        if (btnStartStop4.Text == "Стоп")
                        {
                            btnStartStop4.PerformClick();
                            if (btnStartStop4.Text == "Старт")
                            {
                                info_sv_time4.Text = "[stop] Lost pid process";
                                ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss]")) + "[STOP]: SERVER[4] LOST PID PROCESS => Reason: " + ex.Message).ForeColor = Color.Red;
                            }
                            else
                            {
                                Srv4ProcessScan.Stop();
                                Srv4Time.Stop();
                                ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss]")) + "[STOP]: SERVER[4] LOST PID PROCESS => INCORRECT EXITS FUNCT !!!").ForeColor = Color.DarkViolet;
                            }
                        }
                    }
                }
                else
                {
                    if (btnStartStop4.Text == "Стоп")
                    {
                        btnStartStop4.PerformClick();
                        info_sv_time4.Text = "[stop] Pid process is null";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки исполняемых файлов сервера.\nКод ошибки:\n" + ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ==================================START SERVER 5==================================
        string KeySV5;
        int PID5, GetPIDSV5;
        private void LoadingServer5()
        {
            srv5_running_time = 0;
            CloseErrorDialog();
            ProcessStartAsm();
            SrvName5.Text = SrvName5.Text.Replace("_", " ").Replace("%", " ");

            if (SrvPsw5.Text == "")
                KeySV5 = "";
            else
                KeySV5 = "/psw=" + SrvPsw5.Text;

            if (SrvStartArgument.CheckState == CheckState.Checked && myKeyCreated.TextLength > 0)
            {
                StartKeyArgument = myKeyCreated.Text;
                ParseStartArgument();
            }

            try
            {
                btnStartStop5.Text = "Стоп";
                info_sv_time5.Text = "Loading...";

                // Call Of Pripyat

                /* Process load = new Process();
                 load.StartInfo.FileName = "soLauncher.exe";
                 load.StartInfo.Arguments = ("-dedicated -perfhud_hack -i -nosound -silent_error_mode "+StartKeyArgument + " -start server(" + SrvMaps5.Text + "/tdm"+ KeySV5+"/ver=1.0/hname=" + SrvName5.Text + "/portsv=" + PortSV5.Text + "/portgs=" + PortGS5.Text + "/maxplayers=" + svPlayers.Text + "/estime=9:00/etimef=1.0/ans=1/anslen=3/pdahunt=0/warmup=0/timelimit=0/dmgblock=0/dmbi=0/fraglimit=0/spectrmds=31/vote=26/frcrspwn=0/abalance=0/aswap=0/fi=0/fn=0/ffire=1.0) client("+NAT_IP_ATTACH+"/portcl=" + PortCL5.Text);
                 load.Start();*/

                ProcessStartInfo CONTROLLER = new ProcessStartInfo(current_directory + @"\bin\stalker_csoc.exe", @"bin\dedicated\xr_3da.exe -i -nosound -noprefetch " + StartKeyArgument + " -$sv_status -start server(" + SrvMaps5.Text + "/" + SrvGameType5.Text + KeySV5 + "/hname=" + SrvName5.Text + "/maxplayers=" + svPlayers.Text + "/public=" + ServerOnline + "/battleye=1/maxping=" + svPing.Text + "/spectr=20/spectrmds=31/vote=1/dmgblock=0/fraglimit=" + svFraglim.Text + "/timelimit=" + svTimeLim.Text + "/ffire=" + svFriendlyFire.Text + "/fn=" + svIco.Text + "/fi=" + svIco.Text + "/ans=0/warmup=" + svWurmUp.Text + "/etimef=0.0/estime=" + svWeatherTime.Text + "/portgs=" + PortGS5.Text + "/portsv=" + PortSV5.Text + "portcl=" + PortCL5.Text + "/anum=" + Artefacts.Text + "/astime=" + svTimeArtefact.Text + ") client(" + NAT_IP_ATTACH + "/name=server)");
                CONTROLLER.WorkingDirectory = Path.GetDirectoryName(CONTROLLER.FileName);
                Process load = Process.Start(CONTROLLER);
      
                Thread.Sleep(500);
                var search = new ManagementObjectSearcher("root\\CIMV2", "SELECT ProcessId FROM Win32_Process WHERE ParentProcessId = " + load.Id);
                foreach (var PIDResult in search.Get())
                {
                    var a = (uint)PIDResult["ProcessID"];
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss]")) + "[SYSTEM]: SERVER[5] PID PROCESS => " + a).ForeColor = Color.Coral;
                    GetPIDSV5 = Convert.ToInt32(a);
                }
                Thread.Sleep(500);
                if (GetPIDSV5 != 0)
                {
                    try
                    {
                        Process.GetProcessById(GetPIDSV5);
                        PID5 = GetPIDSV5;
                        FunctionServer4UsingCore();
                        FuncPriorityClassChangeServer4();
                        AsmLoading.Start();
                    }
                    catch (Exception ex)
                    {
                        if (btnStartStop5.Text == "Стоп")
                        {
                            btnStartStop5.PerformClick();
                            if (btnStartStop5.Text == "Старт")
                            {
                                info_sv_time5.Text = "[stop] Lost pid process";
                                ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss]")) + "[STOP]: SERVER[5] LOST PID PROCESS => Reason: " + ex.Message).ForeColor = Color.Red;
                            }
                            else
                            {
                                Srv5ProcessScan.Stop();
                                Srv5Time.Stop();
                                ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss]")) + "[STOP]: SERVER[5] LOST PID PROCESS => INCORRECT EXITS FUNCT !!!").ForeColor = Color.DarkViolet;
                            }
                        }
                    }
                }
                else
                {
                    if (btnStartStop5.Text == "Стоп")
                    {
                        btnStartStop5.PerformClick();
                        info_sv_time5.Text = "[stop] Pid process is null";
                    }
                }
            }
            catch (Exception ex)
            {
                btnStartStop5.Text = "Старт";
                MessageBox.Show("Ошибка загрузки исполняемых файлов сервера.\nКод ошибки:\n" + ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        // =========================================================================
        // SERVER PROCESS SCAN
        // =========================================================================
        private void Srv1ProcessScan_Tick(object sender, EventArgs e)
        {
            try
            {
                // new Thread :?
                /*
                Process.GetProcessById(PID_SRV1);
                Process ProcessSrvID1 = Process.GetProcessById(PID_SRV1);                                        
                if (ProcessSrvID1.Responding == false)
                {
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss] ")) + "[SYSTEM]: SERVER [1] NOT RESPONDING => [" + ProcessSrvID1.Id + "] id. Exit process").BackColor = Color.Blue;
                     ProcessSrvID1.Kill(); 
                }*/
                Process.GetProcessById(PID_SRV1);
                Process Proc = new Process();
                Proc.StartInfo.FileName = "taskkill.exe";
                Proc.StartInfo.Arguments = "/F /PID " + PID_SRV1 + " /FI \"status eq not responding";
                Proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                Proc.Start();
                Proc.WaitForExit();
                Proc.Close();
            }
            catch (Exception) // ArgumentException т.к проверка может кинуть 2 вида исключения, выбираем 1 действие. 
            {
                srv1_reconnection_counter++;
                info_sv_reconnect1.Text = "Restarts: [" + srv1_reconnection_counter + "]";
                if (checkAutoSrvStop.CheckState == CheckState.Checked)
                {
                    if (numericSrvResetLim.Value <= srv1_reconnection_counter)
                    {
                        Srv1ProcessScan.Stop();
                        Srv1Time.Stop();
                        ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss] ")) + "Server [1] Terminated by auto. Restarting limited").ForeColor = Color.LightPink;
                        info_sv_time1.Text = "Server stopped is auto";
                    }
                    else
                    {
                        Srv1Time.Start();
                        ChangeServer1();
                        ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss] ")) + "Server [1] Restarting server...").ForeColor = Color.LightGreen;
                        CheckBannedList();
                        LoadingServer1();
                    }
                }
                else
                {
                    Srv1Time.Start();
                    ChangeServer1();
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss] ")) + "Server [1] Restarting server...").ForeColor = Color.LightGreen;
                    CheckBannedList();
                    LoadingServer1();
                }
            }
        }

        private void Srv2ProcessScan_Tick(object sender, EventArgs e)
        {
            try
            {
                Process.GetProcessById(PID_SRV2);
                Process Proc = new Process();
                Proc.StartInfo.FileName = "taskkill.exe";
                Proc.StartInfo.Arguments = "/F /PID " + PID_SRV2 + " /FI \"status eq not responding";
                Proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                Proc.Start();
                Proc.WaitForExit();
                Proc.Close();
            }
            catch (Exception)
            {
                srv2_reconnection_counter++;
                info_sv_reconnect2.Text = "Restarts: [" + srv2_reconnection_counter + "]";
                if (checkAutoSrvStop.CheckState == CheckState.Checked)
                {
                    if (numericSrvResetLim.Value <= srv2_reconnection_counter)
                    {
                        Srv2ProcessScan.Stop();
                        Srv2Time.Stop();
                        ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss] ")) + "Server [2] Terminated by auto. Restarting limited").ForeColor = Color.LightPink;
                        info_sv_time2.Text = "Server stopped is auto";
                    }
                    else
                    {
                        Srv2Time.Start();
                        ChangeServer2();
                        ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss] ")) + "Server [2] Restarting server...").ForeColor = Color.LightGreen;
                        CheckBannedList();
                        LoadingServer2();
                    }
                }
                else
                {
                    Srv2Time.Start();
                    ChangeServer2();
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss] ")) + "Server [2] Restarting server...").ForeColor = Color.LightGreen;
                    CheckBannedList();
                    LoadingServer2();
                }
            }
        }

        private void Srv3ProcessScan_Tick(object sender, EventArgs e)
        {
            try
            {
                Process.GetProcessById(PID_SRV3);
                Process Proc = new Process();
                Proc.StartInfo.FileName = "taskkill.exe";
                Proc.StartInfo.Arguments = "/F /PID " + PID_SRV3 + " /FI \"status eq not responding";
                Proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                Proc.Start();
                Proc.WaitForExit();
                Proc.Close();
            }
            catch (Exception)
            {
                srv3_reconnection_counter++;
                info_sv_reconnect3.Text = "Restarts: [" + srv3_reconnection_counter + "]";
                if (checkAutoSrvStop.CheckState == CheckState.Checked)
                {
                    if (numericSrvResetLim.Value <= srv3_reconnection_counter)
                    {
                        Srv3ProcessScan.Stop();
                        Srv3Time.Stop();
                        ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss] ")) + "Server [3] Terminated by auto. Restarting limited").ForeColor = Color.LightPink;
                        info_sv_time3.Text = "Server stopped is auto";
                    }
                    else
                    {
                        Srv3Time.Start();
                        ChangeServer3();
                        ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss]")) + "Server [3] Restarting server...").ForeColor = Color.LightGreen;
                        CheckBannedList();
                        LoadingServer3();
                    }
                }
                else
                {
                    Srv3Time.Start();
                    ChangeServer3();
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss] ")) + "Server [3] Restarting server...").ForeColor = Color.LightGreen;
                    CheckBannedList();
                    LoadingServer3();
                }
            }
        }

        private void Srv4ProcessScan_Tick(object sender, EventArgs e)
        {
            try
            {
                Process ProcessSrvID1 = Process.GetProcessById(PID4);
                Process Proc = new Process();
                Proc.StartInfo.FileName = "taskkill.exe";
                Proc.StartInfo.Arguments = "/F /PID " + PID4 + " /FI \"status eq not responding";
                Proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                Proc.Start();
                Proc.WaitForExit();
                Proc.Close();
            }
            catch (Exception)
            {
                srv4_reconnection_counter++;
                info_sv_reconnect4.Text = "Restarts: [" + srv4_reconnection_counter + "]";
                if (checkAutoSrvStop.CheckState == CheckState.Checked)
                {
                    if (numericSrvResetLim.Value <= srv4_reconnection_counter)
                    {
                        Srv4ProcessScan.Stop();
                        Srv4Time.Stop();
                        ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss] ")) + "Server [4] Terminated by auto. Restarting limited").ForeColor = Color.LightPink;
                        info_sv_time4.Text = "Server stopped is auto";
                    }
                    else
                    {
                        Srv4Time.Start();
                        ChangeServer4();
                        ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss] ")) + "Server [4] Restarting server...").ForeColor = Color.LightGreen;
                        CheckBannedList();
                        LoadingServer4();
                    }
                }
                else
                {
                    Srv4Time.Start();
                    ChangeServer4();
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss] ")) + "Server [4] Restarting server...").ForeColor = Color.LightGreen;
                    CheckBannedList();
                    LoadingServer4();
                }
            }
        }

        private void Srv5ProcessScan_Tick(object sender, EventArgs e)
        {
            try
            {
                Process.GetProcessById(PID5);
                Process Proc = new Process();
                Proc.StartInfo.FileName = "taskkill.exe";
                Proc.StartInfo.Arguments = "/F /PID " + PID5 + " /FI \"status eq not responding";
                Proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                Proc.Start();
                Proc.WaitForExit();
                Proc.Close();
            }
            catch (Exception)
            {
                srv5_reconnection_counter++;
                info_sv_reconnect5.Text = "Restarts: [" + srv5_reconnection_counter + "]";
                if (checkAutoSrvStop.CheckState == CheckState.Checked)
                {
                    if (numericSrvResetLim.Value <= srv5_reconnection_counter)
                    {
                        Srv5ProcessScan.Stop();
                        Srv5Time.Stop();
                        ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss] ")) + "Server [5] Terminated by auto. Restarting limited").ForeColor = Color.LightPink;
                        info_sv_time5.Text = "Server stopped is auto";
                    }
                    else
                    {
                        Srv5Time.Start();
                        ChangeServer5();
                        ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss] ")) + "Server [5] Restarting server...").ForeColor = Color.LightGreen;
                        CheckBannedList();
                        LoadingServer5();
                    }
                }
                else
                {
                    Srv5Time.Start();
                    ChangeServer5();
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss] ")) + "Server [5] Restarting server...").ForeColor = Color.LightGreen;
                    CheckBannedList();
                    LoadingServer5();
                }
            }
        }
        private void AsmLoading_Tick(object sender, EventArgs e)
        {
            try
            {
                ProcessStartAsm();
                Thread.Sleep(350);
                ProcessStartAsm();
            }
            catch (Exception ex)
            {
                ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss]")) + "[SYSTEM]: Asm Loaded Error => " + ex.Message).ForeColor = Color.Coral;
                AsmLoading.Stop();
            }
            AsmLoading.Stop();
        }


        // Server process time && process not found
        private void Srv1Time_Tick(object sender, EventArgs e)
        {
            try
            {
                Process.GetProcessById(PID_SRV1);
                btnStartStop1.Text = "Стоп";
                srv1_running_time += 30;
                info_sv_time1.Text = "Running: [" + TimeSpan.FromSeconds(srv1_running_time).ToString(@"dd\:hh\:mm\:ss") + "]";
            }
            catch (Exception)
            {
                btnStartStop1.Text = "Старт";
                info_sv_time1.Text = "Server restarting wait...";
                ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss] ")) + "Server [1] Crashed... Restarting wait...").ForeColor = Color.Orange;
                srv1_running_time = 0;
                ServerBuffer();
                Srv1Time.Stop();

#if (DEBUG)
                SendMail(); // TEST
#endif
            }
        }

        private void Srv2Time_Tick(object sender, EventArgs e)
        {
            try
            {
                Process.GetProcessById(PID_SRV2);
                btnStartStop2.Text = "Стоп";
                srv2_running_time += 30;
                info_sv_time2.Text = "Running: [" + TimeSpan.FromSeconds(srv2_running_time).ToString(@"dd\:hh\:mm\:ss") + "]";
            }
            catch (Exception)
            {
                btnStartStop2.Text = "Старт";
                info_sv_time2.Text = "Server restarting wait...";
                ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss] ")) + "Server [2] Crashed... Restarting wait...").ForeColor = Color.Orange;
                srv2_running_time = 0;
                ServerBuffer();
                Srv2Time.Stop();
#if (DEBUG)
                SendMail(); // TEST
#endif
            }
        }

        private void Srv3Time_Tick(object sender, EventArgs e)
        {
            try
            {
                Process.GetProcessById(PID_SRV3);
                btnStartStop3.Text = "Стоп";
                srv3_running_time += 30;
                info_sv_time3.Text = "Running: [" + TimeSpan.FromSeconds(srv3_running_time).ToString(@"dd\:hh\:mm\:ss") + "]";
            }
            catch (Exception)
            {
                btnStartStop3.Text = "Старт";
                info_sv_time3.Text = "Server restarting wait...";
                ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss] ")) + "Server [3] Crashed... Restarting wait...").ForeColor = Color.Orange;
                srv3_running_time = 0;
                ServerBuffer();
                Srv3Time.Stop();
#if (DEBUG)
                SendMail(); // TEST
#endif
            }
        }

        private void Srv4Time_Tick(object sender, EventArgs e)
        {
            try
            {
                Process.GetProcessById(PID4);
                btnStartStop4.Text = "Стоп";
                srv4_running_time += 30;
                info_sv_time4.Text = "Running: [" + TimeSpan.FromSeconds(srv4_running_time).ToString(@"dd\:hh\:mm\:ss") + "]";
            }
            catch (Exception)
            {
                btnStartStop4.Text = "Старт";
                info_sv_time4.Text = "Server restarting wait...";
                ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss] ")) + "Server [4] Crashed... Restarting wait...").ForeColor = Color.Orange;
                srv4_running_time = 0;
                ServerBuffer();
                Srv4Time.Stop();
            }
        }
        private void Srv5Time_Tick(object sender, EventArgs e)
        {
            try
            {
                Process.GetProcessById(PID5);
                btnStartStop5.Text = "Стоп";
                srv5_running_time += 30;
                info_sv_time5.Text = "Running: [" + TimeSpan.FromSeconds(srv5_running_time).ToString(@"dd\:hh\:mm\:ss") + "]";
            }
            catch (Exception)
            {
                btnStartStop5.Text = "Старт";
                info_sv_time5.Text = "Server restarting wait...";
                ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss] ")) + "Server [5] Crashed... Restarting wait...").ForeColor = Color.Orange;
                srv5_running_time = 0;
                ServerBuffer();
                Srv5Time.Stop();
            }
        }


        private void XR_3DA_ERROR_CLOSE_Tick(object sender, EventArgs e)
        {
            string ProcessScan = "XR_3DA";
            Process[] local_procs = Process.GetProcesses();
            try
            {
                Process target_proc = local_procs.First(p => p.ProcessName == ProcessScan);
                Process Proc = new Process();
                Proc.StartInfo.FileName = "taskkill.exe";
                Proc.StartInfo.Arguments = "/F /FI \"WINDOWTITLE eq XR_3DA.exe";
                Proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                Proc.Start();
                Proc.WaitForExit();
                Proc.Close();

                if (ServerEvents.Items.Count >= 300)
                {
                    ServerEvents.Items.Clear();
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss]")) + "[SYSTEM] Server events were automatically deleted").ForeColor = Color.Gold;
                }
            }
            catch (InvalidOperationException)
            {
                ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss]")) + "[SYSTEM]: Active Servers Not Found => Stop Control Functions").ForeColor = Color.LightSkyBlue;
                XR_3DA_ERROR_CLOSE.Stop();
            }
        }

        private void CloseErrorDialog()
        {
            if (XR_3DA_ERROR_CLOSE.Enabled != Enabled)
            {
                XR_3DA_ERROR_CLOSE.Start();
            }
        }

        string reg_time;
        string reg_cmp_time;

        private void CheckBannedList()
        {
            try
            {
                DateTime DataModification = File.GetLastWriteTime(@"server_settings\banned_list.ltx");
                reg_cmp_time = (DataModification.ToString("dd.MM.yy HH.mm.ss"));

                if (reg_cmp_time != reg_time)
                {
                    File.WriteAllLines((@"server_settings\banned_list.ltx"), File.ReadAllLines(@"server_settings\banned_list.ltx").Distinct().Where(x => !x.Contains("[0.0.0.0]")));
                    reg_time = (DataModification.ToString("dd.MM.yy HH.mm.ss"));
                }

                /*
               int find_line = 0;

               StreamWriter LINES = new StreamWriter(@"server_settings\banned_list1.ltx"); // открываем поток записи
               string[] test = File.ReadAllLines(@"server_settings\banned_list.ltx");   // считали весь файл  
                foreach (string rt in test)
                {

                    find_line++;
                    if (rt.Contains("[0.0.0.0]"))
                    {
                        for (int i = 0; i < test.Length; i++)
                        {
                            if (i != find_line)            // номер строки которую нам надо пропустить
                            {
                                LINES.WriteLine(test[i]);

                                MessageBox.Show(i + " " + test.ToString());
                            }
                            else
                            {
                                ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss]")) + "[CHECK_BAN_LIST] found bad str format [0.0.0.0] index => " + find_line + " && " + (find_line+1) + " => result remove").BackColor = Color.OrangeRed;
                            }
                        }
                    }
                }*/
                //sw.Close();
            }
            catch (Exception error)
            {
                ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss]")) + "[CHECK_BAN_LIST] Can't file open error " + error.Message).ForeColor = Color.OrangeRed;
            }
        }


        private void ProcessStartAsm()
        {
            try
            {
                if (checkLevel4.CheckState == CheckState.Unchecked)
                    Process.Start(@"bin\server.exe");
                else
                    Process.Start(@"bin\server_sv.exe");
            }
            catch (Exception ex)
            {
                ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss]")) + "[SYSTEM]: Process Loading ASM ERROR => " + ex.Message).ForeColor = Color.Red;
            }
        }

        private void ParseStartArgument()
        {
            if (StartKeyArgument.Contains("IP="))
            {
                try
                {
                    var get_ip = myKeyCreated.Text.Split('=')[1];
                    if ((get_ip.Length >= 6 && get_ip.Length <= 15))
                    {
                        NAT_IP_ATTACH = get_ip;
                        ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss]")) + "[SYSTEM]: Found Argument IP= " + get_ip).ForeColor = Color.Blue;
                    }
                    else
                    {
                        NAT_IP_ATTACH = "localhost";
                    }
                }
                catch (Exception ex)
                {
                    NAT_IP_ATTACH = "localhost";
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss]")) + "[SYSTEM]: Parse argument IP= ERROR, Incorrect value " + ex.Message).ForeColor = Color.Coral;
                }
            }
            else
            {
                NAT_IP_ATTACH = "localhost";
            }
        }


        private void ServerBuffer()
        {
            if (xrDebugCollectMode.CheckState == CheckState.Checked)
            {
                THREAD_LOG_EVENT.WorkerSupportsCancellation = true;
                THREAD_LOG_EVENT.RunWorkerAsync();
            }
        }

        private void THREAD_LOG_EVENT_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss]")) + "[SERVER] Stack Trace Collect Start").ForeColor = Color.SkyBlue;
                File.Copy(@"server_settings\logs\xray_" + SystemInformation.UserName + ".log", @"server_settings\server_logs\server_crashed_" + DateTime.Now.ToString("dd.MM.yyyy---HH-mm-ss") + ".txt", true);
            }
            catch (Exception TLE)
            {
                ServerEvents.Items.Add("[SERVER] Log Move Error => " + TLE.Message).BackColor = Color.Violet;
            }
        }
        private void THREAD_LOG_EVENT_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss]")) + "[SERVER] Stack Trace Collect Finished. File Move in .../server_settings/server_logs").ForeColor = Color.SkyBlue;
        }


        Random RandomStart = new Random();
        private void ChangeServer1()
        {
            if (checkAutoChangePort.CheckState == CheckState.Checked)
            {
                try
                {
                    PortSV1.SelectedIndex = RandomStart.Next(0, PortSV1.Items.Count);
                    PortGS1.SelectedIndex = RandomStart.Next(0, PortGS1.Items.Count);
                    PortCL1.SelectedIndex = RandomStart.Next(0, PortCL1.Items.Count);
                }
                catch (Exception ex)
                {
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy]")) + "[SERVER_1]: AutoChange Ports Select Index Error => " + ex.Message).ForeColor = Color.Red;
                }
                ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy]")) + "[SERVER_1]: AutoChange Ports => [SV: " + PortSV1.Text + " | GS: " + PortGS1.Text + " | CL: " + PortCL1.Text + "]").ForeColor = Color.DeepSkyBlue;
                if (PortCL1.Text == PortSV1.Text || PortCL1.Text == PortGS1.Text || PortGS1.Text == PortSV1.Text)
                {
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy]")) + "[SERVER_1]: AutoChange Ports Incorrect. Returns values").ForeColor = Color.PaleVioletRed;
                    ChangeServer1();
                    return;
                }
            }
            if (checkAutoChangeMaps.CheckState == CheckState.Checked)
            {
                try
                {
                    SrvMaps1.SelectedIndex = RandomStart.Next(0, SrvMaps1.Items.Count);
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy]")) + "[SERVER_1]: AutoChange maps => [" + SrvMaps1.Text + "]").ForeColor = Color.DeepSkyBlue;
                }
                catch (Exception ex)
                {
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy]")) + "[SERVER_1]: AutoChange maps Select Index Error => " + ex.Message).ForeColor = Color.Red;
                }
            }
            if (checkAutoChangeGametype.CheckState == CheckState.Checked)
            {
                try
                {
                    SrvGameType1.SelectedIndex = RandomStart.Next(0, SrvGameType1.Items.Count);
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy]")) + "[SERVER_1]: AutoChange gametype => [" + SrvGameType1.Text + "]").ForeColor = Color.DeepSkyBlue;
                }
                catch (Exception ex)
                {
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy]")) + "[SERVER_1]: AutoChange gametype Select Index Error => " + ex.Message).ForeColor = Color.Red;
                }
            }
        }

        private void ChangeServer2()
        {
            if (checkAutoChangePort.CheckState == CheckState.Checked)
            {
                try
                {
                    PortSV2.SelectedIndex = RandomStart.Next(0, PortSV2.Items.Count);
                    PortGS2.SelectedIndex = RandomStart.Next(0, PortGS2.Items.Count);
                    PortCL2.SelectedIndex = RandomStart.Next(0, PortCL2.Items.Count);
                }
                catch (Exception ex)
                {
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy]")) + "[SERVER_2]: AutoChange Ports Select Index Error => " + ex.Message).ForeColor = Color.Red;
                }
                ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy]")) + "[SERVER_2]: AutoChange Ports => [SV: " + PortSV2.Text + " | GS: " + PortGS2.Text + " | CL: " + PortCL2.Text + "]").ForeColor = Color.DeepSkyBlue;
                if (PortCL2.Text == PortSV2.Text || PortCL2.Text == PortGS2.Text || PortGS2.Text == PortSV2.Text)
                {
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy]")) + "[SERVER_2]: AutoChange Ports Incorrect. Returns values").ForeColor = Color.PaleVioletRed;
                    ChangeServer2();
                    return;
                }
            }
            if (checkAutoChangeMaps.CheckState == CheckState.Checked)
            {
                try
                {
                    SrvMaps2.SelectedIndex = RandomStart.Next(0, SrvMaps2.Items.Count);
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy]")) + "[SERVER_2]: AutoChange maps => [" + SrvMaps2.Text + "]").ForeColor = Color.DeepSkyBlue;
                }
                catch (Exception ex)
                {
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy]")) + "[SERVER_2]: AutoChange maps Select Index Error => " + ex.Message).ForeColor = Color.Red;
                }
            }
            if (checkAutoChangeGametype.CheckState == CheckState.Checked)
            {
                try
                {
                    SrvGameType2.SelectedIndex = RandomStart.Next(0, SrvGameType2.Items.Count);
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy]")) + "[SERVER_2]: AutoChange gametype => [" + SrvGameType2.Text + "]").ForeColor = Color.DeepSkyBlue;
                }
                catch (Exception ex)
                {
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy]")) + "[SERVER_2]: AutoChange gametype Select Index Error => " + ex.Message).ForeColor = Color.Red;
                }
            }
        }

        private void ChangeServer3()
        {
            if (checkAutoChangePort.CheckState == CheckState.Checked)
            {
                try
                {
                    PortSV3.SelectedIndex = RandomStart.Next(0, PortSV3.Items.Count);
                    PortGS3.SelectedIndex = RandomStart.Next(0, PortGS3.Items.Count);
                    PortCL3.SelectedIndex = RandomStart.Next(0, PortCL3.Items.Count);
                }
                catch (Exception ex)
                {
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy]")) + "[SERVER_3]: AutoChange Ports Select Index Error => " + ex.Message).ForeColor = Color.Red;
                }

                ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy]")) + "[SERVER_3]: AutoChange Ports => [SV: " + PortSV3.Text + " | GS: " + PortGS3.Text + " | CL: " + PortCL3.Text + "]").ForeColor = Color.DeepSkyBlue;
                if (PortCL3.Text == PortSV3.Text || PortCL3.Text == PortGS3.Text || PortGS3.Text == PortSV3.Text)
                {
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy]")) + "[SERVER_3]: AutoChange Ports Incorrect. Returns values").ForeColor = Color.PaleVioletRed;
                    ChangeServer3();
                    return;
                }
            }
            if (checkAutoChangeMaps.CheckState == CheckState.Checked)
            {
                try
                {
                    SrvMaps3.SelectedIndex = RandomStart.Next(0, SrvMaps3.Items.Count);
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy]")) + "[SERVER_3]: AutoChange maps => [" + SrvMaps3.Text + "]").ForeColor = Color.DeepSkyBlue;
                }
                catch (Exception ex)
                {
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy]")) + "[SERVER_3]: AutoChange maps Select Index Error => " + ex.Message).ForeColor = Color.Red;
                }
            }
            if (checkAutoChangeGametype.CheckState == CheckState.Checked)
            {
                try
                {
                    SrvGameType3.SelectedIndex = RandomStart.Next(0, SrvGameType3.Items.Count);
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy]")) + "[SERVER_3]: AutoChange gametype => [" + SrvGameType3.Text + "]").ForeColor = Color.DeepSkyBlue;
                }
                catch (Exception ex)
                {
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy]")) + "[SERVER_3]: AutoChange gametype Select Index Error => " + ex.Message).ForeColor = Color.Red;
                }
            }
        }

        private void ChangeServer4()
        {
            if (checkAutoChangePort.CheckState == CheckState.Checked)
            {
                try
                {
                    PortSV4.SelectedIndex = RandomStart.Next(0, PortSV4.Items.Count);
                    PortGS4.SelectedIndex = RandomStart.Next(0, PortGS4.Items.Count);
                    PortCL4.SelectedIndex = RandomStart.Next(0, PortCL4.Items.Count);
                }
                catch (Exception ex)
                {
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy]")) + "[SERVER_4]: AutoChange Ports Select Index Error => " + ex.Message).ForeColor = Color.Red;
                }
                ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy]")) + "[SERVER_4]: AutoChange Ports => [SV: " + PortSV4.Text + " | GS: " + PortGS4.Text + " | CL: " + PortCL4.Text + "]").ForeColor = Color.DeepSkyBlue;

                if (PortCL4.Text == PortSV4.Text || PortCL4.Text == PortGS4.Text || PortGS4.Text == PortSV4.Text)
                {
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy]")) + "[SERVER_4]: AutoChange Ports Incorrect. Returns values").ForeColor = Color.PaleVioletRed;
                    ChangeServer4();
                    return;
                }
            }
            if (checkAutoChangeMaps.CheckState == CheckState.Checked)
            {
                try
                {
                    SrvMaps4.SelectedIndex = RandomStart.Next(0, SrvMaps4.Items.Count);
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy]")) + "[SERVER_4]: AutoChange maps => [" + SrvMaps4.Text + "]").ForeColor = Color.DeepSkyBlue;
                }
                catch (Exception ex)
                {
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy]")) + "[SERVER_4]: AutoChange maps Select Index Error => " + ex.Message).ForeColor = Color.Red;
                }
            }
            if (checkAutoChangeGametype.CheckState == CheckState.Checked)
            {
                try
                {
                    SrvGameType4.SelectedIndex = RandomStart.Next(0, SrvGameType4.Items.Count);
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy]")) + "[SERVER_4]: AutoChange gametype => [" + SrvGameType4.Text + "]").ForeColor = Color.DeepSkyBlue;
                }
                catch (Exception ex)
                {
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy]")) + "[SERVER_4]: AutoChange gametype Select Index Error => " + ex.Message).ForeColor = Color.Red;
                }
            }
        }
        private void ChangeServer5()
        {
            if (checkAutoChangePort.CheckState == CheckState.Checked)
            {
                try
                {
                    PortSV5.SelectedIndex = RandomStart.Next(0, PortSV5.Items.Count);
                    PortGS5.SelectedIndex = RandomStart.Next(0, PortGS5.Items.Count);
                    PortCL5.SelectedIndex = RandomStart.Next(0, PortCL5.Items.Count);
                }
                catch (Exception ex)
                {
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy]")) + "[SERVER_5]: AutoChange Ports Select Index Error => " + ex.Message).ForeColor = Color.Red;
                }
                ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy]")) + "[SERVER_5]: AutoChange Ports => [SV: " + PortSV5.Text + " | GS: " + PortGS5.Text + " | CL: " + PortCL5.Text + "]").ForeColor = Color.DeepSkyBlue;
                if (PortCL5.Text == PortSV5.Text || PortCL5.Text == PortGS5.Text || PortGS5.Text == PortSV5.Text)
                {
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy]")) + "[SERVER_5]: AutoChange Ports Incorrect. Returns values").ForeColor = Color.PaleVioletRed;
                    ChangeServer5();
                    return;
                }
            }
            if (checkAutoChangeMaps.CheckState == CheckState.Checked)
            {
                try
                {
                    SrvMaps5.SelectedIndex = RandomStart.Next(0, SrvMaps5.Items.Count);
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy]")) + "[SERVER_5]: AutoChange maps => [" + SrvMaps5.Text + "]").ForeColor = Color.DeepSkyBlue;
                }
                catch (Exception ex)
                {
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy]")) + "[SERVER_5]: AutoChange maps Select Index Error => " + ex.Message).ForeColor = Color.Red;
                }
            }
            if (checkAutoChangeGametype.CheckState == CheckState.Checked)
            {
                try
                {
                    SrvGameType5.SelectedIndex = RandomStart.Next(0, SrvGameType5.Items.Count);
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy]")) + "[SERVER_5]: AutoChange gametype => [" + SrvGameType5.Text + "]").ForeColor = Color.DeepSkyBlue;
                }
                catch (Exception ex)
                {
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy]")) + "[SERVER_5]: AutoChange gametype Select Index Error => " + ex.Message).ForeColor = Color.Red;
                }
            }
        }
        // =========================================================================
        // SERVER AUTO CHANGE PORTS/GAMETYPE/MAPS
        // =========================================================================
        private void checkAutoChangePort_CheckedChanged(object sender, EventArgs e)
        {
            if (checkAutoChangePort.CheckState == CheckState.Checked)
            {
                checkAutoChangePort.BackColor = Color.PaleGreen;
            }
            else
            {
                checkAutoChangePort.BackColor = Color.White;
            }
        }

        private void checkAutoChangeMaps_CheckedChanged(object sender, EventArgs e)
        {
            if (checkAutoChangeMaps.CheckState == CheckState.Checked)
            {
                checkAutoChangeMaps.BackColor = Color.PaleGreen;
            }
            else
            {
                checkAutoChangeMaps.BackColor = Color.White;
            }
        }

        private void checkAutoChangeGametype_CheckedChanged(object sender, EventArgs e)
        {
            if (checkAutoChangeGametype.CheckState == CheckState.Checked)
            {
                checkAutoChangeGametype.BackColor = Color.PaleGreen;
            }
            else
            {
                checkAutoChangeGametype.BackColor = Color.White;
            }
        }

        private void checkAutoSrvStop_CheckedChanged(object sender, EventArgs e)
        {
            if (checkAutoSrvStop.CheckState == CheckState.Checked)
            {
                checkAutoSrvStop.BackColor = Color.PaleGreen;
                checkValueDisabled.BackColor = Color.PaleGreen;
                numericSrvResetLim.BackColor = Color.PaleGreen;
                numericSrvResetLim.Enabled = true;
            }
            else
            {
                checkAutoSrvStop.BackColor = Color.White;
                checkValueDisabled.BackColor = Color.White;
                numericSrvResetLim.BackColor = Color.White;
                numericSrvResetLim.Enabled = false;
            }
        }

        // =========================================================================
        // SERVER LEVEL PACK SET
        // =========================================================================
        private void checkActivateLevel_CheckedChanged(object sender, EventArgs e)
        {
            if (checkActivateLevel.CheckState == CheckState.Checked)
            {
                checkLevel1.Enabled = true;
                checkLevel2.Enabled = true;
                checkLevel3.Enabled = true;
                checkLevel4.Enabled = true;
                checkActivateLevel.BackColor = Color.PaleGreen;
            }
            else
            {
                checkLevel1.Enabled = false;
                checkLevel2.Enabled = false;
                checkLevel3.Enabled = false;
                checkLevel4.Enabled = false;
                checkLevel1.Checked = false;
                checkLevel2.Checked = false;
                checkLevel3.Checked = false;
                checkLevel4.Checked = false;
                checkActivateLevel.BackColor = Color.White;
            }
        }

        bool change = false;
        private void checkLevel1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkLevel1.CheckState == CheckState.Checked)
                {
                    if (!(checkLevel2.Checked == true || checkLevel3.Checked == true || checkLevel4.Checked == true))
                    {
                        change = false;
                        File.Move(@"mods\server_standart_maps.xrmaps", @"mods\server_standart_maps.xdb0");
                        checkLevel1.BackColor = Color.PaleGreen;
                    }
                    else
                    {
                        change = true;
                        checkLevel1.Checked = false;
                        MessageBox.Show("Одновременное включение различных версии карт\nи модов может привести к некорректной работе сервера.\nПереключение будет возможным только после отключения предыдущей версии карт или мода.", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    if (change == false)
                        File.Move(@"mods\server_standart_maps.xdb0", @"mods\server_standart_maps.xrmaps");
                    checkLevel1.BackColor = Color.White;
                }
            }
            catch (Exception ex)
            {
                checkLevel1.BackColor = Color.LightCoral;
                checkLevel1.Checked = false;
                ServerEvents.Items.Add("[FS->ERROR]: File Move =>" + ex.Message).BackColor = Color.OrangeRed;
            }
        }

        private void checkLevel2_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkLevel2.CheckState == CheckState.Checked)
                {
                    if (!(checkLevel1.Checked == true || checkLevel3.Checked == true || checkLevel4.Checked == true))
                    {
                        change = false;
                        File.Move(@"mods\server_christmas_maps.xrmaps", @"mods\server_christmas_maps.xdb0");
                        checkLevel2.BackColor = Color.PaleGreen;
                    }
                    else
                    {
                        change = true;
                        checkLevel2.Checked = false;
                        MessageBox.Show("Одновременное включение различных версии карт\nи модов может привести к некорректной работе сервера.\nПереключение будет возможным только после отключения предыдущей версии карт или мода.", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    if (change == false)
                        File.Move(@"mods\server_christmas_maps.xdb0", @"mods\server_christmas_maps.xrmaps");
                    checkLevel2.BackColor = Color.White;
                }
            }
            catch (Exception ex)
            {
                checkLevel2.BackColor = Color.LightCoral;
                checkLevel2.Checked = false;
                ServerEvents.Items.Add("[FS->ERROR]: File Move =>" + ex.Message).BackColor = Color.OrangeRed;
            }
        }

        private void checkLevel3_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkLevel3.CheckState == CheckState.Checked)
                {
                    if (!(checkLevel1.Checked == true || checkLevel2.Checked == true || checkLevel4.Checked == true))
                    {
                        change = false;
                        File.Move(@"mods\server_singles_2.6.xrmaps", @"mods\server_singles_2.6.xdb0");
                        checkLevel3.BackColor = Color.PaleGreen;
                    }
                    else
                    {
                        change = true;
                        checkLevel3.Checked = false;
                        MessageBox.Show("Одновременное включение различных версии карт\nи модов может привести к некорректной работе сервера.\nПереключение будет возможным только после отключения предыдущей версии карт или мода.", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    if (change == false)
                        File.Move(@"mods\server_singles_2.6.xdb0", @"mods\server_singles_2.6.xrmaps");
                    checkLevel3.BackColor = Color.White;

                }
            }
            catch (Exception ex)
            {
                checkLevel3.BackColor = Color.LightCoral;
                checkLevel3.Checked = false;
                ServerEvents.Items.Add("[FS->ERROR]: File Move =>" + ex.Message).BackColor = Color.OrangeRed;
            }
        }


        private void checkLevel4_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (btnStartStop1.Text == "Старт" && btnStartStop2.Text == "Старт" && btnStartStop3.Text == "Старт" && btnStartStop4.Text == "Старт" && btnStartStop5.Text == "Старт")
                {
                    int skip_apply_mod = 0;
                    if (checkLevel4.CheckState == CheckState.Checked)
                    {
                        DialogResult ok = MessageBox.Show("При включений данного режима игры, будет возможность запустить максимум 1 сервер с данной сборки.\nПродолжить?", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        {
                            // Перед загрузкой проверить размер архива
                            // Если dbe имеет другой размер то переименовываем его в xr2
                            // а оригинальный файл заменяем на dbe
                            long SizeOrigin = new FileInfo("gamedata.dbe").Length / 1024; // standart dbe 7437
                            if (SizeOrigin == 7437)
                            {
                                if (File.Exists("gamedata.dbe"))
                                    skip_apply_mod++;
                                if (File.Exists("gamedata.xr2"))
                                    skip_apply_mod++;
                            }
                            if (skip_apply_mod == 2)
                            {
                                File.Move("gamedata.dbe", "gamedata.xr1");  // original
                                File.Move("gamedata.xr2", "gamedata.dbe");  // mod
                            }
                            else
                            {
                                checkLevel4.Checked = false;
                                //MessageBox.Show("Что-то пошло не так. Не найдены исполняемые файлы для данного режима.\nВозможно Вы используете устаревшую сборку.", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            checkLevel4.BackColor = Color.Gold;
                            SrvGameType1.SelectedIndex = 2;
                            SrvGameType1.Enabled = false;
                            btnStartStop2.Enabled = false;
                            btnStartStop3.Enabled = false;
                            btnStartStop4.Enabled = false;
                            btnStartStop5.Enabled = false;
                        }
                    }
                    else
                    {
                        skip_apply_mod = 0;
                        if (File.Exists("gamedata.dbe"))
                            skip_apply_mod++;
                        if (File.Exists("gamedata.xr1"))
                            skip_apply_mod++;

                        if (skip_apply_mod == 2)
                        {
                            File.Move("gamedata.dbe", "gamedata.xr2");  // mod                      
                            File.Move("gamedata.xr1", "gamedata.dbe");  // original
                        }

                        SrvGameType1.Enabled = true;
                        btnStartStop2.Enabled = true;
                        btnStartStop3.Enabled = true;
                        btnStartStop4.Enabled = true;
                        btnStartStop5.Enabled = true;
                        checkLevel4.BackColor = Color.White;
                    }
                }
                else
                {
                    checkLevel4.Checked = false;
                    checkLevel4.BackColor = Color.LightCoral;
                    MessageBox.Show("Переключение режима игры будет возможно\nтолько при всех остановленных серверов.", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception)
            {
                checkLevel4.Checked = false;
                checkLevel4.BackColor = Color.LightCoral;
            }
        }


        private void ServerEventsClear_Click(object sender, EventArgs e)
        {
            ServerEvents.Items.Clear();
            ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss]")) + " Events were cleared by the user.").ForeColor = Color.Gold;
        }

        // ===== SRV 1 =====
        private void WriteSelectItemsMaps_Click(object sender, EventArgs e) // Возращяем список карт, таким каким он был изначально
        {
            SrvMaps1.Items.Clear();
            foreach (string sv in buffer_listmaps)
            {
                SrvMaps1.Items.Add(sv);
            }
        }

        private void ClearSelectItemsMaps_Click(object sender, EventArgs e) // Удаляем выбранную строку из всего списка карт
        {
            var namemaps = SrvMaps1.SelectedItem.ToString();
            if (SrvMaps1.Text == namemaps)
            {
                SrvMaps1.Items.Remove(SrvMaps1.SelectedItem);
                SrvMaps1.SelectedIndex++;
            }
        }
        // ===== SRV 2 =====
        private void WriteSelectItemsMaps2_Click(object sender, EventArgs e)
        {
            SrvMaps2.Items.Clear();
            foreach (string sv in buffer_listmaps)
            {
                SrvMaps2.Items.Add(sv);
            }
        }

        private void ClearSelectItemsMaps2_Click(object sender, EventArgs e)
        {
            var namemaps = SrvMaps2.SelectedItem.ToString();
            if (SrvMaps2.Text == namemaps)
            {
                SrvMaps2.Items.Remove(SrvMaps2.SelectedItem);
                SrvMaps2.SelectedIndex++;
            }
        }
        // ===== SRV 3 =====
        private void WriteSelectItemsMaps3_Click(object sender, EventArgs e)
        {
            SrvMaps3.Items.Clear();
            foreach (string sv in buffer_listmaps)
            {
                SrvMaps3.Items.Add(sv);
            }
        }

        private void ClearSelectItemsMaps3_Click(object sender, EventArgs e)
        {
            var namemaps = SrvMaps3.SelectedItem.ToString();
            if (SrvMaps3.Text == namemaps)
            {
                SrvMaps3.Items.Remove(SrvMaps3.SelectedItem);
                SrvMaps3.SelectedIndex++;
            }
        }
        // ===== SRV 4 =====
        private void WriteSelectItemsMaps4_Click(object sender, EventArgs e)
        {
            SrvMaps4.Items.Clear();
            foreach (string sv in buffer_listmaps)
            {
                SrvMaps4.Items.Add(sv);
            }
        }

        private void ClearSelectItemsMaps4_Click(object sender, EventArgs e)
        {
            var namemaps = SrvMaps4.SelectedItem.ToString();
            if (SrvMaps4.Text == namemaps)
            {
                SrvMaps4.Items.Remove(SrvMaps4.SelectedItem);
                SrvMaps4.SelectedIndex++;
            }
        }
        // ===== SRV 5 =====
        private void WriteSelectItemsMaps5_Click(object sender, EventArgs e)
        {
            SrvMaps5.Items.Clear();
            foreach (string sv in buffer_listmaps)
            {
                SrvMaps5.Items.Add(sv);
            }
        }

        private void ClearSelectItemsMaps5_Click(object sender, EventArgs e)
        {
            var namemaps = SrvMaps5.SelectedItem.ToString();
            if (SrvMaps5.Text == namemaps)
            {
                SrvMaps5.Items.Remove(SrvMaps5.SelectedItem);
                SrvMaps5.SelectedIndex++;
            }
        }

        private void HelpMenuEnter_Click(object sender, EventArgs e)
        {
            ServerEvents.Items.Clear();
            ServerEvents.Items.Add("[Помощь]: Справка по клавишам. [Главное Меню]").BackColor = Color.LimeGreen;
            ServerEvents.Items.Add("[Помощь]: F1 - Включить контроль выделяемой памяти используемым сервером").BackColor = Color.White;
            ServerEvents.Items.Add("[Помощь]: F2 - Отключить контроль выделяемой памяти используемым сервером").BackColor = Color.White;
            ServerEvents.Items.Add("[Помощь]: F3 - Открыть AdminAssistant").BackColor = Color.White;
            ServerEvents.Items.Add("[Помощь]: F4 - Открыть текущий Лог-файл").BackColor = Color.White;
            ServerEvents.Items.Add("[Помощь]: F5 - Открыть папку расположения лог файлов").BackColor = Color.White;
            ServerEvents.Items.Add("[Помощь]: Справка по клавишам. [База данных и события сервера]").BackColor = Color.LimeGreen;
            ServerEvents.Items.Add("[Помощь]: F1 - Поиск выделенного игрока по Name").BackColor = Color.White;
            ServerEvents.Items.Add("[Помощь]: F2 - Поиск выделенного игрока по IPAddress").BackColor = Color.White;
            ServerEvents.Items.Add("[Помощь]: F3 - Поиск выделенного игрока по HASH").BackColor = Color.White;
            ServerEvents.Items.Add("[Помощь]: F4 - Скопировать выделенную строку в буфер обмена").BackColor = Color.White;
            ServerEvents.Items.Add("[Помощь]: F5 - Скопировать выделенные данные для быстрой блокировки").BackColor = Color.White;
            ServerEvents.Items.Add("[Помощь]: F6 - Открыть выбранные данные (Файл, ссылка на сайт и тд).").BackColor = Color.White;
            ServerEvents.Items.Add("[Помощь]: F7 - Добавить данные в базу").BackColor = Color.White;
            ServerEvents.Items.Add("[Помощь]: F8 - Удалить автоматические обработанные события сервера").BackColor = Color.White;
            ServerEvents.Items.Add("[Помощь]: F9 - Сохранить автоматические обработанные события сервера в текстовый файл").BackColor = Color.White;
            ServerEvents.Items.Add("[Помощь]: F10 - Поиск выделенного игрока по Name из истории чата").BackColor = Color.White;
            ServerEvents.Items.Add("[Помощь]: F11 - [НОВОЕ ОКНО СО СПИСКОМ ИГРОКОВ] Скопировать выделенные данные").BackColor = Color.White;
            ServerEvents.Items.Add("[Помощь]: F12 - [НОВОЕ ОКНО СО СПИСКОМ ИГРОКОВ] Скопировать выделенные данные для быстрой блокировки").BackColor = Color.White;
            ServerEvents.Items.Add("[Помощь]: DEL - Удалить выделенные данные").BackColor = Color.White;
            ServerEvents.Items.Add("[Помощь]: ENTER - Выполнить поиск по базе").BackColor = Color.White;
        }

        int f_lock = 0;
        private void btnEvents_Click(object sender, EventArgs e)
        {
            ServerEvents.Visible = true;
            btnMinimized.Visible = false;
            btnExit.Visible = false;
            btnCloseEvents.Visible = true;
            if (using_new_engine.CheckState == CheckState.Checked)
            {
                using_new_engine.Checked = false;
                f_lock += 1;
            }
        }
        private void btnCloseEvents_Click(object sender, EventArgs e)
        {
            ServerEvents.Visible = false;
            btnCloseEvents.Visible = false;
            btnMinimized.Visible = true;
            btnExit.Visible = true;
            if (f_lock == 1)
            {
                f_lock = 0;
                using_new_engine.Checked = true;
            }
        }

        private void SrvStartArgument_CheckedChanged(object sender, EventArgs e)
        {
            if (SrvStartArgument.CheckState == CheckState.Checked)
            {
                myKeyCreated.Enabled = true;
            }
            else
            {
                myKeyCreated.Enabled = false;
            }
        }

        private void using_new_engine_CheckedChanged(object sender, EventArgs e)
        {
            if (using_new_engine.CheckState == CheckState.Checked)
            {
                SendMessageVoteTime.SendMsgVoteTime = Convert.ToString(128);
                server_new_engine = "s";
                status = " -dedicated";
                SRV_PROTECT1.Checked = false;
                SRV_PROTECT2.Checked = false;
                SRV_PROTECT3.Checked = false;
                SRV_PROTECT1.Visible = false;
                SRV_PROTECT2.Visible = false;
                SRV_PROTECT3.Visible = false;
                SRV_RUS_CHAT1.Visible = true;
                SRV_NO_SPEECH1.Visible = true;
                SRV_NO_KEY1.Visible = true;
                SRV_DEBUG_MODE1.Visible = true;
                SRV_RUS_CHAT2.Visible = true;
                SRV_NO_SPEECH2.Visible = true;
                SRV_NO_KEY2.Visible = true;
                SRV_DEBUG_MODE2.Visible = true;
                SRV_RUS_CHAT3.Visible = true;
                SRV_NO_SPEECH3.Visible = true;
                SRV_NO_KEY3.Visible = true;
                SRV_DEBUG_MODE3.Visible = true;
                check_1.Visible = true;
                if (SrvMainFormLanguage.CheckState == CheckState.Checked)
                {
                    GUI_PROTECT.Text = "Function";
                }
                else
                {
                    GUI_PROTECT.Text = "Функции";
                }
            }
            else
            {
                SendMessageVoteTime.SendMsgVoteTime = Convert.ToString(0);
                server_new_engine = "";
                status = " -$sv_status";
                SRV_PROTECT1.Visible = true;
                SRV_PROTECT2.Visible = true;
                SRV_PROTECT3.Visible = true;
                SRV_RUS_CHAT1.Visible = false;
                SRV_NO_SPEECH1.Visible = false;
                SRV_NO_KEY1.Visible = false;
                SRV_DEBUG_MODE1.Visible = false;
                SRV_RUS_CHAT2.Visible = false;
                SRV_NO_SPEECH2.Visible = false;
                SRV_NO_KEY2.Visible = false;
                SRV_DEBUG_MODE2.Visible = false;
                SRV_RUS_CHAT3.Visible = false;
                SRV_NO_SPEECH3.Visible = false;
                SRV_NO_KEY3.Visible = false;
                SRV_DEBUG_MODE3.Visible = false;
                check_1.Visible = false;
                if (SrvMainFormLanguage.CheckState == CheckState.Checked)
                {
                    GUI_PROTECT.Text = "Protect";
                }
                else
                {
                    GUI_PROTECT.Text = "Защита";
                }
            }
        }


        private void SRV_RUS_CHAT1_CheckedChanged(object sender, EventArgs e)
        {
            if (SRV_RUS_CHAT1.CheckState == CheckState.Checked)
            {
                RUS_CHAT1 = " -rus_test";
            }
            else
            {
                RUS_CHAT1 = "";
            }
        }

        private void SRV_NO_SPEECH1_CheckedChanged(object sender, EventArgs e)
        {
            if (SRV_NO_SPEECH1.CheckState == CheckState.Checked)
            {
                NO_SPEECH1 = " -nospeech";
            }
            else
            {
                NO_SPEECH1 = "";
            }
        }

        private void SRV_NO_KEY1_CheckedChanged(object sender, EventArgs e)
        {
            if (SRV_NO_KEY1.CheckState == CheckState.Checked)
            {
                NO_KEY1 = " -check_cd_key";
            }
            else
            {
                NO_KEY1 = "";
            }
        }
        private void SRV_DEBUG_MODE1_CheckedChanged(object sender, EventArgs e)
        {
            if (SRV_DEBUG_MODE1.CheckState == CheckState.Checked)
            {
                DEBUG_MODE1 = " -enable_name_change"; //" -debug";
            }
            else
            {
                DEBUG_MODE1 = "";
            }
        }
        // sv2
        private void SRV_RUS_CHAT2_CheckedChanged(object sender, EventArgs e)
        {
            if (SRV_RUS_CHAT2.CheckState == CheckState.Checked)
            {
                RUS_CHAT2 = " -rus_test";
            }
            else
            {
                RUS_CHAT2 = "";
            }
        }

        private void SRV_NO_SPEECH2_CheckedChanged(object sender, EventArgs e)
        {
            if (SRV_NO_SPEECH2.CheckState == CheckState.Checked)
            {
                NO_SPEECH2 = " -nospeech";
            }
            else
            {
                NO_SPEECH2 = "";
            }
        }

        private void SRV_NO_KEY2_CheckedChanged(object sender, EventArgs e)
        {
            if (SRV_NO_KEY2.CheckState == CheckState.Checked)
            {
                NO_KEY2 = " -check_cd_key";
            }
            else
            {
                NO_KEY2 = "";
            }
        }

        private void SRV_DEBUG_MODE2_CheckedChanged(object sender, EventArgs e)
        {
            DEBUG_MODE2 = " -enable_name_change"; //" -debug";
        }

        private void SRV_RUS_CHAT3_CheckedChanged(object sender, EventArgs e)
        {
            if (SRV_RUS_CHAT3.CheckState == CheckState.Checked)
            {
                RUS_CHAT3 = " -rus_test";
            }
            else
            {
                RUS_CHAT3 = "";
            }
        }

        private void SRV_NO_SPEECH3_CheckedChanged(object sender, EventArgs e)
        {
            if (SRV_NO_SPEECH3.CheckState == CheckState.Checked)
            {
                NO_SPEECH3 = " -nospeech ";

            }
            else
            {
                NO_SPEECH3 = "";
            }
        }

        private void SRV_NO_KEY3_CheckedChanged(object sender, EventArgs e)
        {
            if (SRV_NO_KEY3.CheckState == CheckState.Checked)
            {
                NO_KEY3 = " -check_cd_key ";
            }
            else
            {
                NO_KEY3 = "";
            }
        }

        private void SRV_DEBUG_MODE3_CheckedChanged(object sender, EventArgs e)
        {
            if (SRV_DEBUG_MODE3.CheckState == CheckState.Checked)
            {
                DEBUG_MODE3 = " -enable_name_change "; //" -debug";
            }
            else
            {
                DEBUG_MODE3 = "";
            }
        }

        private void check_1_CheckedChanged(object sender, EventArgs e)
        {
            if (check_1.CheckState == CheckState.Checked)
            {
                check = " -alwaysflush";
            }
            else
            {
                check = "";
            }
        }


        private void btnStats_Click(object sender, EventArgs e)
        {
            try
            {
                if (ServerBasePlayers.BaseEvents.Count > 0)
                {
                    DialogResult result = MessageBox.Show("Выберите вариант событий:\nНажмите Да - чтобы открыть текущие события\nНажмите Нет - чтобы просмотреть всю историю событий.", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (DialogResult.Yes == result)
                    {
                        EvELoad();
                    }
                    else if (DialogResult.No == result)
                    {
                        ServerBasePlayers set = new ServerBasePlayers();
                        set.LoadEvents();
                    }
                }
                else
                {
                    EvELoad();
                }                         
            }
            catch (Exception ex)
            {
                MessageBox.Show("Reason: " + ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EvELoad()
        {
            foreach (Form FormLoadCancel in Application.OpenForms)
            {
                if (FormLoadCancel.Name == "ServerEvents")
                {
                    MessageBox.Show("Окно уже открыто!", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            ServerEvents ServerEventsLoad = new ServerEvents();
            ServerEventsLoad.Show();
        }

        private void btnBase_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (Form FormLoadCancel in Application.OpenForms)
                {
                    if (FormLoadCancel.Name == "ServerBasePlayers")
                    {
                        MessageBox.Show("Окно уже открыто!", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                ServerBasePlayers ServerEventsLoad = new ServerBasePlayers();
                ServerEventsLoad.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Reason: " + ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (Form FormLoadCancel in Application.OpenForms)
                {
                    if (FormLoadCancel.Name == "About")
                    {
                        return;
                    }
                }
                About ServerEventsLoad = new About();
                ServerEventsLoad.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Reason: " + ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnSettings_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (Form FormLoadCancel in Application.OpenForms)
                {
                    if (FormLoadCancel.Name == "ServerSettings")
                    {
                        MessageBox.Show("Окно уже открыто!", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                ServerSettings ServerEventsLoad = new ServerSettings();
                ServerEventsLoad.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Reason: " + ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnInitProtectionForm_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (Form FormLoadCancel in Application.OpenForms)
                {
                    if (FormLoadCancel.Name == "NetworkMonitor")
                    {
                        MessageBox.Show("Окно уже открыто!", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                NetworkMonitor LogReader = new NetworkMonitor();
                LogReader.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Reason: " + ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            save_settings();
        }

        private void btnUI_Enable_Click(object sender, EventArgs e)
        {
            btnUI_Enable.Visible = false;
            btnUI_Disable.Visible = true;
            SrvPanel.Visible = true;
            if (SrvStartArgument.CheckState == CheckState.Checked)
            {
                myKeyCreated.Visible = true;
            }
        }

        private void btnUI_Disable_Click(object sender, EventArgs e)
        {
            btnUI_Disable.Visible = false;
            btnUI_Enable.Visible = true;
            SrvPanel.Visible = false;
            if (SrvStartArgument.CheckState == CheckState.Checked)
            {
                myKeyCreated.Visible = false;
            }
        }


        private void btnHidePanel_Click(object sender, EventArgs e)
        {
            btnUI_Disable.Visible = false;
            btnUI_Enable.Visible = true;
            SrvPanel.Visible = false;
        }

        private void xrDebugCollectMode_CheckedChanged(object sender, EventArgs e)
        {
            if (xrDebugCollectMode.CheckState == CheckState.Checked)
            {
                int getfiles = 0, delfiles = 0;
                foreach (var file in Directory.GetFiles(@"server_settings\server_logs\", "server_crashed_*.txt"))
                {
                    getfiles++;
                }
                if (getfiles > 0)
                {
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss]")) + " Logs files found: " + getfiles).ForeColor = Color.LightSeaGreen;
                    DialogResult DResult = MessageBox.Show("Обнаружены старые записи журнала ошибок сервера.\nКоличество файлов: " + getfiles + "\nХотите удалить все старые файлы?", "S.E.R.V.E.R - Журнал ошибок сервера", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (DResult == DialogResult.Yes)
                    {
                        foreach (var file in Directory.GetFiles(@"server_settings\server_logs\", "server_crashed_*.txt"))
                        {
                            try
                            {
                                delfiles++;
                                File.Delete(file);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Не удалось выполнить запрошенную Вами процедуру:\n" + ex.Message, "S.E.R.V.E.R - Ошибка выполнения", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                break;
                            }
                        }
                        ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss]")) + " Logs files deleted: " + delfiles).ForeColor = Color.LightSeaGreen;
                        MessageBox.Show("Удалено файлов: " + delfiles, "S.E.R.V.E.R - Журнал ошибок сервера", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void SrvWeatherTime_CheckedChanged(object sender, EventArgs e)
        {
            if (SrvWeatherTime.CheckState == CheckState.Checked)
            {
                svWeatherTime.Text = UI_TIME.Text;
            }
            else
            {
                svWeatherTime.Text = "21:00";
            }
        }

        private void SrvPublic_CheckedChanged(object sender, EventArgs e)
        {
            if (SrvPublic.CheckState == CheckState.Checked)
            {
                ServerOnline = "0";
            }
            else
            {
                ServerOnline = "1";
            }
        }

        private void SrvMainFormLanguage_CheckedChanged(object sender, EventArgs e)
        {
            if (SrvMainFormLanguage.CheckState == CheckState.Checked)
            {
                initializeSrvMainFormLanguageEN();
            }
            else
            {
                initializeSrvMainFormLanguageRUS();
            }
        }

        public void initializeSrvMainFormLanguageEN()
        {
            SrvMainFormLanguage.Text = "Русская версия";
            GUI_SRV_NAME.Text = "Server Name";
            GUI_SRV_MAPS.Text = "Maps";
            GUI_SRV_GT.Text = "Game Type";
            GUI_PCL.Text = "Port CL";
            GUI_PSV.Text = "Port SV";
            GUI_PGS.Text = "Port GS";
            GUI_PSW.Text = "PswKey";
            GUI_START_STOP.Text = "Start/Stop";
            UI_PING.Text = "Ping";
            UI_MAXPLAYERS.Text = "Players";
            UI_PLAYERS_ICO.Text = "Ico";
            UI_TIME_LIM.Text = "Time Limit";
            UI_WEATHER_TIME.Text = "Weather Time";
            UI_ARTEFACTS.Text = "Artefacts";
            UI_TIME_ARTEFACT.Text = "Time Artefact";
            UI_FRAGLIM.Text = "Fraglimit";
            UI_FRIEND_ATTACK.Text = "FriendFire";
            UI_SRV_WARMUP.Text = "WurmUpTime";
            UI_SRV1.Text = "Information srv No.1";
            UI_SRV2.Text = "Information srv No.2";
            UI_SRV3.Text = "Information srv No.3";
            UI_SRV4.Text = "Information srv No.4";
            UI_SRV5.Text = "Information srv No.5";
            UI_SRV_PARAMS.Text = "Additional startup parameters of the servers";
            UI_SRV_MAPS.Text = "Additional versions of the maps";
            UI_PRIORITY.Text = "Productivity and performance";
            checkAutoChangePort.Text = "Auto change SRV Ports";
            checkAutoChangeGametype.Text = "Auto change GameType";
            checkAutoChangeMaps.Text = "Auto change Maps";
            checkAutoSrvStop.Text = "Limit the number of restarts";
            checkActivateLevel.Text = "To include addition";
            checkLevel1.Text = "Standart mp-maps";
            checkLevel2.Text = "Christmas mp-maps";
            if (using_new_engine.CheckState == CheckState.Checked)
            {
                GUI_PROTECT.Text = "Function";
            }
            else
            {
                GUI_PROTECT.Text = "Protect";
            }
        }
        public void initializeSrvMainFormLanguageRUS()
        {
            SrvMainFormLanguage.Text = "English version";
            GUI_SRV_NAME.Text = "Наименование сервера";
            GUI_SRV_MAPS.Text = "Карта";
            GUI_SRV_GT.Text = "Режим игры";
            GUI_PCL.Text = "Порт CL";
            GUI_PSV.Text = "Порт SV";
            GUI_PGS.Text = "Порт GS";
            GUI_PSW.Text = "Пароль";
            GUI_START_STOP.Text = "Старт/Стоп";
            UI_PING.Text = "Пинг";
            UI_MAXPLAYERS.Text = "Игроков";
            UI_PLAYERS_ICO.Text = "Значки";
            UI_TIME_LIM.Text = "Лим.Времени";
            UI_WEATHER_TIME.Text = "Время Суток";
            UI_ARTEFACTS.Text = "Артефактов";
            UI_TIME_ARTEFACT.Text = "Время Арт.";
            UI_FRAGLIM.Text = "Лимит Фрагов";
            UI_FRIEND_ATTACK.Text = "Урон по своим";
            UI_SRV_WARMUP.Text = "Разминка";
            UI_SRV1.Text = "Статистика Сервера №1";
            UI_SRV2.Text = "Статистика Сервера №2";
            UI_SRV3.Text = "Статистика Сервера №3";
            UI_SRV4.Text = "Статистика Сервера №4";
            UI_SRV5.Text = "Статистика Сервера №5";
            UI_SRV_PARAMS.Text = "Дополнительные параметры автозагрузки сервера";
            UI_SRV_MAPS.Text = "Дополнительные версии карт";
            UI_PRIORITY.Text = "Производительность и быстродействие";
            checkAutoChangePort.Text = "Автосмена портов";
            checkAutoChangeGametype.Text = "Автосмена режимов";
            checkAutoChangeMaps.Text = "Автосмена карт";
            checkAutoSrvStop.Text = "Лимит перезапусков";
            checkActivateLevel.Text = "Включить дополнение";
            checkLevel1.Text = "Стандартные mp-карты";
            checkLevel2.Text = "Новогодние mp-карты";
            if (using_new_engine.CheckState == CheckState.Checked)
            {
                GUI_PROTECT.Text = "Функции";
            }
            else
            {
                GUI_PROTECT.Text = "Защита";
            }
        }
        // =========================================================================
        // Смена цвета
        // =========================================================================
        private void WriteNewColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog newCollor = new ColorDialog())
            {
                if (newCollor.ShowDialog() == DialogResult.OK)
                {
                    int a = newCollor.Color.ToArgb();
                    DYNAMIC_SET_COLOR(a);
                }
                else
                {
                    var a = Color.FromArgb(192, 192, 255);
                    DYNAMIC_SET_COLOR(a.ToArgb());
                }
            }
        }

        private void DYNAMIC_SET_COLOR(int pColorClass)
        {
            try
            {
                if (pColorClass == 0)
                {
                    GUI.BackColor = Color.FromArgb(192, 192, 255);
                    btnMinimized.BackColor = Color.FromArgb(192, 192, 255);
                    btnExit.BackColor = Color.FromArgb(192, 192, 255);
                    btnCloseEvents.BackColor = Color.FromArgb(192, 192, 255);
                    btnStartStop1.BackColor = Color.FromArgb(192, 192, 255);
                    btnStartStop2.BackColor = Color.FromArgb(192, 192, 255);
                    btnStartStop3.BackColor = Color.FromArgb(192, 192, 255);
                    btnStartStop4.BackColor = Color.FromArgb(192, 192, 255);
                    btnStartStop5.BackColor = Color.FromArgb(192, 192, 255);
                }
                else
                {
                    var color_rechange = Color.FromArgb(pColorClass);
                    GUI.BackColor = color_rechange;
                    btnMinimized.BackColor = color_rechange;
                    btnExit.BackColor = color_rechange;
                    btnCloseEvents.BackColor = color_rechange;
                    btnStartStop1.BackColor = color_rechange;
                    btnStartStop2.BackColor = color_rechange;
                    btnStartStop3.BackColor = color_rechange;
                    btnStartStop4.BackColor = color_rechange;
                    btnStartStop5.BackColor = color_rechange;
                    if (pColorClass <= 10)
                    {
                        _settings.ARGB_SET_COLOR = pColorClass;      // Сохраняем значение цвета в массив ARGB_SET_COLOR для будущего сохранения в файл
                        SendMessage.SendMsg = pColorClass;           // Transfer values
                        ServerEvents.Items.Add(pColorClass + " = " + (pColorClass.ToString().Length) + " OK").ForeColor = Color.DarkViolet;

                    }
                    else
                    {
                        SendMessage.SendMsg = 0;
                        ServerEvents.Items.Add(pColorClass + " = " + (pColorClass.ToString().Length) + " BAD").ForeColor = Color.DarkViolet;
                    }
                }
            }
            catch (Exception ex)
            {
                GUI.BackColor = Color.FromArgb(192, 192, 255);
                btnMinimized.BackColor = Color.FromArgb(192, 192, 255);
                btnExit.BackColor = Color.FromArgb(192, 192, 255);
                btnCloseEvents.BackColor = Color.FromArgb(192, 192, 255);
                btnStartStop1.BackColor = Color.FromArgb(192, 192, 255);
                btnStartStop2.BackColor = Color.FromArgb(192, 192, 255);
                btnStartStop3.BackColor = Color.FromArgb(192, 192, 255);
                btnStartStop4.BackColor = Color.FromArgb(192, 192, 255);
                btnStartStop5.BackColor = Color.FromArgb(192, 192, 255);
                MessageBox.Show("Command init set color error\n" + ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        // =========================================================================
        // Сохраним настройки главного меню
        // =========================================================================
        private void save_settings()
        {
            try
            {
                _settings.SrvName1 = SrvName1.Text;
                _settings.SrvName2 = SrvName2.Text;
                _settings.SrvName3 = SrvName3.Text;
                _settings.SrvName4 = SrvName4.Text;
                _settings.SrvName5 = SrvName5.Text;
                _settings.SrvMaps1 = SrvMaps1.Text;
                _settings.SrvMaps2 = SrvMaps2.Text;
                _settings.SrvMaps3 = SrvMaps3.Text;
                _settings.SrvMaps4 = SrvMaps4.Text;
                _settings.SrvMaps5 = SrvMaps5.Text;
                _settings.SrvGameType1 = SrvGameType1.Text;
                _settings.SrvGameType2 = SrvGameType2.Text;
                _settings.SrvGameType3 = SrvGameType3.Text;
                _settings.SrvGameType4 = SrvGameType4.Text;
                _settings.SrvGameType5 = SrvGameType5.Text;
                _settings.svPing = svPing.Text;
                _settings.svPlayers = svPlayers.Text;
                _settings.svIco = svIco.Text;
                _settings.svTimeLim = svTimeLim.Text;
                _settings.svWeatherTime = svWeatherTime.Text;
                _settings.Artefacts = Artefacts.Text;
                _settings.svTimeArtefact = svTimeArtefact.Text;
                _settings.svFraglim = svFraglim.Text;
                _settings.svFriendlyFire = svFriendlyFire.Text;
                _settings.svWurmUp = svWurmUp.Text;
                _settings.PortCL1 = PortCL1.Text;
                _settings.PortSV1 = PortSV1.Text;
                _settings.PortGS1 = PortGS1.Text;
                _settings.PortCL2 = PortCL2.Text;
                _settings.PortSV2 = PortSV2.Text;
                _settings.PortGS2 = PortGS2.Text;
                _settings.PortCL3 = PortCL3.Text;
                _settings.PortSV3 = PortSV3.Text;
                _settings.PortGS3 = PortGS3.Text;
                _settings.PortCL4 = PortCL4.Text;
                _settings.PortSV4 = PortSV4.Text;
                _settings.PortGS4 = PortGS4.Text;
                _settings.PortCL5 = PortCL5.Text;
                _settings.PortSV5 = PortSV5.Text;
                _settings.PortGS5 = PortGS5.Text;
                _settings.SrvMainFormLanguage = SrvMainFormLanguage.Checked;
                _settings.myKeyCreated = myKeyCreated.Text;
                _settings.SRV_COPY_ERROR = xrDebugCollectMode.Checked;
                _settings.SRV_START_LINE = SrvStartArgument.Checked;
                // affinity mask
                _settings.SRV1_0x0001 = CoreUsing1.Checked;
                _settings.SRV1_0x0002 = CoreUsing2.Checked;
                _settings.SRV1_0x0004 = CoreUsing3.Checked;
                _settings.SRV1_0x0008 = CoreUsing4.Checked;
                _settings.SRV1_0x00010 = CoreUsing5.Checked;
                _settings.SRV1_0x00020 = CoreUsing6.Checked;
                _settings.SRV1_0x00040 = CoreUsing7.Checked;
                _settings.SRV1_0x00080 = CoreUsing8.Checked;
                _settings.SRV2_0x0001 = CoreUsing1srv2.Checked;
                _settings.SRV2_0x0002 = CoreUsing2srv2.Checked;
                _settings.SRV2_0x0004 = CoreUsing3srv2.Checked;
                _settings.SRV2_0x0008 = CoreUsing4srv2.Checked;
                _settings.SRV2_0x00010 = CoreUsing5srv2.Checked;
                _settings.SRV2_0x00020 = CoreUsing6srv2.Checked;
                _settings.SRV2_0x00040 = CoreUsing7srv2.Checked;
                _settings.SRV2_0x00080 = CoreUsing8srv2.Checked;
                _settings.SRV3_0x0001 = CoreUsing1srv3.Checked;
                _settings.SRV3_0x0002 = CoreUsing2srv3.Checked;
                _settings.SRV3_0x0004 = CoreUsing3srv3.Checked;
                _settings.SRV3_0x0008 = CoreUsing4srv3.Checked;
                _settings.SRV3_0x00010 = CoreUsing5srv3.Checked;
                _settings.SRV3_0x00020 = CoreUsing6srv3.Checked;
                _settings.SRV3_0x00040 = CoreUsing7srv3.Checked;
                _settings.SRV3_0x00080 = CoreUsing8srv3.Checked;
                _settings.SRV4_0x0001 = CoreUsing1srv4.Checked;
                _settings.SRV4_0x0002 = CoreUsing2srv4.Checked;
                _settings.SRV4_0x0004 = CoreUsing3srv4.Checked;
                _settings.SRV4_0x0008 = CoreUsing4srv4.Checked;
                _settings.SRV4_0x00010 = CoreUsing5srv4.Checked;
                _settings.SRV4_0x00020 = CoreUsing6srv4.Checked;
                _settings.SRV4_0x00040 = CoreUsing7srv4.Checked;
                _settings.SRV4_0x00080 = CoreUsing8srv4.Checked;

                _settings.SaveMain();
                MessageBox.Show("Настройки программы сохранены успешно.", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Initializing settings form ERROR\n" + ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Initialize_save_form()
        {
            try
            {
                SrvName1.Text = _settings.SrvName1;
                SrvName2.Text = _settings.SrvName2;
                SrvName3.Text = _settings.SrvName3;
                SrvName4.Text = _settings.SrvName4;
                SrvName5.Text = _settings.SrvName5;
                SrvMaps1.Text = _settings.SrvMaps1;
                SrvMaps2.Text = _settings.SrvMaps2;
                SrvMaps3.Text = _settings.SrvMaps3;
                SrvMaps4.Text = _settings.SrvMaps4;
                SrvMaps5.Text = _settings.SrvMaps5;
                SrvGameType1.Text = _settings.SrvGameType1;
                SrvGameType2.Text = _settings.SrvGameType2;
                SrvGameType3.Text = _settings.SrvGameType3;
                SrvGameType4.Text = _settings.SrvGameType4;
                SrvGameType5.Text = _settings.SrvGameType5;
                svPing.Text = _settings.svPing;
                svPlayers.Text = _settings.svPlayers;
                svIco.Text = _settings.svIco;
                svTimeLim.Text = _settings.svTimeLim;
                svWeatherTime.Text = _settings.svWeatherTime;
                Artefacts.Text = _settings.Artefacts;
                svTimeArtefact.Text = _settings.svTimeArtefact;
                svFraglim.Text = _settings.svFraglim;
                svFriendlyFire.Text = _settings.svFriendlyFire;
                svWurmUp.Text = _settings.svWurmUp;
                PortCL1.Text = _settings.PortCL1;
                PortSV1.Text = _settings.PortSV1;
                PortGS1.Text = _settings.PortGS1;
                PortCL2.Text = _settings.PortCL2;
                PortSV2.Text = _settings.PortSV2;
                PortGS2.Text = _settings.PortGS2;
                PortCL3.Text = _settings.PortCL3;
                PortSV3.Text = _settings.PortSV3;
                PortGS3.Text = _settings.PortGS3;
                PortCL4.Text = _settings.PortCL4;
                PortSV4.Text = _settings.PortSV4;
                PortGS4.Text = _settings.PortGS4;
                PortCL5.Text = _settings.PortCL5;
                PortSV5.Text = _settings.PortSV5;
                PortGS5.Text = _settings.PortGS5;
                SrvMainFormLanguage.Checked = _settings.SrvMainFormLanguage;
                myKeyCreated.Text = _settings.myKeyCreated;
                xrDebugCollectMode.Checked = _settings.SRV_COPY_ERROR;
                SrvStartArgument.Checked = _settings.SRV_START_LINE;
                // affinity mask
                CoreUsing1.Checked = _settings.SRV1_0x0001;
                CoreUsing2.Checked = _settings.SRV1_0x0002;
                CoreUsing3.Checked = _settings.SRV1_0x0004;
                CoreUsing4.Checked = _settings.SRV1_0x0008;
                CoreUsing5.Checked = _settings.SRV1_0x00010;
                CoreUsing6.Checked = _settings.SRV1_0x00020;
                CoreUsing7.Checked = _settings.SRV1_0x00040;
                CoreUsing8.Checked = _settings.SRV1_0x00080;
                CoreUsing1srv2.Checked = _settings.SRV2_0x0001;
                CoreUsing2srv2.Checked = _settings.SRV2_0x0002;
                CoreUsing3srv2.Checked = _settings.SRV2_0x0004;
                CoreUsing4srv2.Checked = _settings.SRV2_0x0008;
                CoreUsing5srv2.Checked = _settings.SRV2_0x00010;
                CoreUsing6srv2.Checked = _settings.SRV2_0x00020;
                CoreUsing7srv2.Checked = _settings.SRV2_0x00040;
                CoreUsing8srv2.Checked = _settings.SRV2_0x00080;
                CoreUsing1srv3.Checked = _settings.SRV3_0x0001;
                CoreUsing2srv3.Checked = _settings.SRV3_0x0002;
                CoreUsing3srv3.Checked = _settings.SRV3_0x0004;
                CoreUsing4srv3.Checked = _settings.SRV3_0x0008;
                CoreUsing5srv3.Checked = _settings.SRV3_0x00010;
                CoreUsing6srv3.Checked = _settings.SRV3_0x00020;
                CoreUsing7srv3.Checked = _settings.SRV3_0x00040;
                CoreUsing8srv3.Checked = _settings.SRV3_0x00080;
                CoreUsing1srv4.Checked = _settings.SRV4_0x0001;
                CoreUsing2srv4.Checked = _settings.SRV4_0x0002;
                CoreUsing3srv4.Checked = _settings.SRV4_0x0004;
                CoreUsing4srv4.Checked = _settings.SRV4_0x0008;
                CoreUsing5srv4.Checked = _settings.SRV4_0x00010;
                CoreUsing6srv4.Checked = _settings.SRV4_0x00020;
                CoreUsing7srv4.Checked = _settings.SRV4_0x00040;
                CoreUsing8srv4.Checked = _settings.SRV4_0x00080;
                var a = Color.FromArgb(_settings.ARGB_SET_COLOR);
                DYNAMIC_SET_COLOR(a.ToArgb());
            }
            catch (Exception ex)
            {
                DYNAMIC_SET_COLOR(0);
                MessageBox.Show("Save settings form ERROR\n" + ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================================================================
        // Разблокируем и установим доступные нам ядра процессора
        // =========================================================================
        public void ProcessorCount()
        {
            if (CPU_INFO == 1)
            {
                ServerEvents.Items.Add("[SYSTEM]: Available core for selections: 1").ForeColor = Color.DeepPink;
                CoreUsing1.Enabled = true;
                CoreUsing1.Checked = true;
            }
            else if (CPU_INFO == 2)
            {
                ServerEvents.Items.Add("[SYSTEM]: Available core for selections: 2").ForeColor = Color.DeepPink;
                CoreUsing1.Enabled = true;
                CoreUsing2.Enabled = true;
                CoreUsing1srv2.Enabled = true;
                CoreUsing2srv2.Enabled = true;
                CoreUsing1srv3.Enabled = true;
                CoreUsing2srv3.Enabled = true;
                CoreUsing1srv4.Enabled = true;
                CoreUsing2srv4.Enabled = true;

                CoreUsing1.Checked = true;
                CoreUsing2.Checked = true;
                CoreUsing1srv2.Checked = true;
                CoreUsing2srv2.Checked = true;
                CoreUsing1srv3.Checked = true;
                CoreUsing2srv3.Checked = true;
                CoreUsing1srv4.Checked = true;
                CoreUsing2srv4.Checked = true;
            }
            else if (CPU_INFO == 3)
            {
                ServerEvents.Items.Add("[SYSTEM]: Available core for selections: 3").ForeColor = Color.DeepPink;
                CoreUsing1.Enabled = true;
                CoreUsing2.Enabled = true;
                CoreUsing3.Enabled = true;
                CoreUsing1srv2.Enabled = true;
                CoreUsing2srv2.Enabled = true;
                CoreUsing3srv2.Enabled = true;
                CoreUsing1srv3.Enabled = true;
                CoreUsing2srv3.Enabled = true;
                CoreUsing3srv3.Enabled = true;
                CoreUsing1srv4.Enabled = true;
                CoreUsing2srv4.Enabled = true;
                CoreUsing3srv4.Enabled = true;

                CoreUsing1.Checked = true;
                CoreUsing2.Checked = true;
                CoreUsing3.Checked = true;
                CoreUsing1srv2.Checked = true;
                CoreUsing2srv2.Checked = true;
                CoreUsing3srv2.Checked = true;
                CoreUsing1srv3.Checked = true;
                CoreUsing2srv3.Checked = true;
                CoreUsing3srv3.Checked = true;
                CoreUsing1srv4.Checked = true;
                CoreUsing2srv4.Checked = true;
                CoreUsing3srv4.Checked = true;

            }
            else if (CPU_INFO == 4)
            {
                ServerEvents.Items.Add("[SYSTEM]: Available core for selections: 4").ForeColor = Color.DeepPink;
                CoreUsing1.Enabled = true;
                CoreUsing2.Enabled = true;
                CoreUsing3.Enabled = true;
                CoreUsing4.Enabled = true;
                CoreUsing1srv2.Enabled = true;
                CoreUsing2srv2.Enabled = true;
                CoreUsing3srv2.Enabled = true;
                CoreUsing4srv2.Enabled = true;
                CoreUsing1srv3.Enabled = true;
                CoreUsing2srv3.Enabled = true;
                CoreUsing3srv3.Enabled = true;
                CoreUsing4srv3.Enabled = true;
                CoreUsing1srv4.Enabled = true;
                CoreUsing2srv4.Enabled = true;
                CoreUsing3srv4.Enabled = true;
                CoreUsing4srv4.Enabled = true;

                CoreUsing1.Checked = true;
                CoreUsing2.Checked = true;
                CoreUsing3.Checked = true;
                CoreUsing4.Checked = true;
                CoreUsing1srv2.Checked = true;
                CoreUsing2srv2.Checked = true;
                CoreUsing3srv2.Checked = true;
                CoreUsing4srv2.Checked = true;
                CoreUsing1srv3.Checked = true;
                CoreUsing2srv3.Checked = true;
                CoreUsing3srv3.Checked = true;
                CoreUsing4srv3.Checked = true;
                CoreUsing1srv4.Checked = true;
                CoreUsing2srv4.Checked = true;
                CoreUsing3srv4.Checked = true;
                CoreUsing4srv4.Checked = true;
            }
            else if (CPU_INFO == 5)
            {
                ServerEvents.Items.Add("[SYSTEM]: Available core for selections: 5").ForeColor = Color.DeepPink;
                CoreUsing1.Enabled = true;
                CoreUsing2.Enabled = true;
                CoreUsing3.Enabled = true;
                CoreUsing4.Enabled = true;
                CoreUsing5.Enabled = true;
                CoreUsing1srv2.Enabled = true;
                CoreUsing2srv2.Enabled = true;
                CoreUsing3srv2.Enabled = true;
                CoreUsing4srv2.Enabled = true;
                CoreUsing5srv2.Enabled = true;
                CoreUsing1srv3.Enabled = true;
                CoreUsing2srv3.Enabled = true;
                CoreUsing3srv3.Enabled = true;
                CoreUsing4srv3.Enabled = true;
                CoreUsing5srv3.Enabled = true;
                CoreUsing1srv4.Enabled = true;
                CoreUsing2srv4.Enabled = true;
                CoreUsing3srv4.Enabled = true;
                CoreUsing4srv4.Enabled = true;
                CoreUsing5srv4.Enabled = true;

                CoreUsing1.Checked = true;
                CoreUsing2.Checked = true;
                CoreUsing3.Checked = true;
                CoreUsing4.Checked = true;
                CoreUsing5.Checked = true;
                CoreUsing1srv2.Checked = true;
                CoreUsing2srv2.Checked = true;
                CoreUsing3srv2.Checked = true;
                CoreUsing4srv2.Checked = true;
                CoreUsing5srv2.Checked = true;
                CoreUsing1srv3.Checked = true;
                CoreUsing2srv3.Checked = true;
                CoreUsing3srv3.Checked = true;
                CoreUsing4srv3.Checked = true;
                CoreUsing5srv3.Checked = true;
                CoreUsing1srv4.Checked = true;
                CoreUsing2srv4.Checked = true;
                CoreUsing3srv4.Checked = true;
                CoreUsing4srv4.Checked = true;
                CoreUsing5srv4.Checked = true;

            }
            else if (CPU_INFO == 6)
            {
                ServerEvents.Items.Add("[SYSTEM]: Available core for selections: 6").ForeColor = Color.DeepPink;
                CoreUsing1.Enabled = true;
                CoreUsing2.Enabled = true;
                CoreUsing3.Enabled = true;
                CoreUsing4.Enabled = true;
                CoreUsing5.Enabled = true;
                CoreUsing6.Enabled = true;
                CoreUsing1srv2.Enabled = true;
                CoreUsing2srv2.Enabled = true;
                CoreUsing3srv2.Enabled = true;
                CoreUsing4srv2.Enabled = true;
                CoreUsing5srv2.Enabled = true;
                CoreUsing6srv2.Enabled = true;
                CoreUsing1srv3.Enabled = true;
                CoreUsing2srv3.Enabled = true;
                CoreUsing3srv3.Enabled = true;
                CoreUsing4srv3.Enabled = true;
                CoreUsing5srv3.Enabled = true;
                CoreUsing6srv3.Enabled = true;
                CoreUsing1srv4.Enabled = true;
                CoreUsing2srv4.Enabled = true;
                CoreUsing3srv4.Enabled = true;
                CoreUsing4srv4.Enabled = true;
                CoreUsing5srv4.Enabled = true;
                CoreUsing6srv4.Enabled = true;

                CoreUsing1.Checked = true;
                CoreUsing2.Checked = true;
                CoreUsing3.Checked = true;
                CoreUsing4.Checked = true;
                CoreUsing5.Checked = true;
                CoreUsing6.Checked = true;
                CoreUsing1srv2.Checked = true;
                CoreUsing2srv2.Checked = true;
                CoreUsing3srv2.Checked = true;
                CoreUsing4srv2.Checked = true;
                CoreUsing5srv2.Checked = true;
                CoreUsing6srv2.Checked = true;
                CoreUsing1srv3.Checked = true;
                CoreUsing2srv3.Checked = true;
                CoreUsing3srv3.Checked = true;
                CoreUsing4srv3.Checked = true;
                CoreUsing5srv3.Checked = true;
                CoreUsing6srv3.Checked = true;
                CoreUsing1srv4.Checked = true;
                CoreUsing2srv4.Checked = true;
                CoreUsing3srv4.Checked = true;
                CoreUsing4srv4.Checked = true;
                CoreUsing5srv4.Checked = true;
                CoreUsing6srv4.Checked = true;
            }
            else if (CPU_INFO == 7)
            {
                ServerEvents.Items.Add("[SYSTEM]: Available core for selections: 7").ForeColor = Color.DeepPink;
                CoreUsing1.Enabled = true;
                CoreUsing2.Enabled = true;
                CoreUsing3.Enabled = true;
                CoreUsing4.Enabled = true;
                CoreUsing5.Enabled = true;
                CoreUsing6.Enabled = true;
                CoreUsing7.Enabled = true;
                CoreUsing1srv2.Enabled = true;
                CoreUsing2srv2.Enabled = true;
                CoreUsing3srv2.Enabled = true;
                CoreUsing4srv2.Enabled = true;
                CoreUsing5srv2.Enabled = true;
                CoreUsing6srv2.Enabled = true;
                CoreUsing7srv2.Enabled = true;
                CoreUsing1srv3.Enabled = true;
                CoreUsing2srv3.Enabled = true;
                CoreUsing3srv3.Enabled = true;
                CoreUsing4srv3.Enabled = true;
                CoreUsing5srv3.Enabled = true;
                CoreUsing6srv3.Enabled = true;
                CoreUsing7srv3.Enabled = true;
                CoreUsing1srv4.Enabled = true;
                CoreUsing2srv4.Enabled = true;
                CoreUsing3srv4.Enabled = true;
                CoreUsing4srv4.Enabled = true;
                CoreUsing5srv4.Enabled = true;
                CoreUsing6srv4.Enabled = true;
                CoreUsing7srv4.Enabled = true;

                CoreUsing1.Checked = true;
                CoreUsing2.Checked = true;
                CoreUsing3.Checked = true;
                CoreUsing4.Checked = true;
                CoreUsing5.Checked = true;
                CoreUsing6.Checked = true;
                CoreUsing7.Checked = true;
                CoreUsing1srv2.Checked = true;
                CoreUsing2srv2.Checked = true;
                CoreUsing3srv2.Checked = true;
                CoreUsing4srv2.Checked = true;
                CoreUsing5srv2.Checked = true;
                CoreUsing6srv2.Checked = true;
                CoreUsing7srv2.Checked = true;
                CoreUsing1srv3.Checked = true;
                CoreUsing2srv3.Checked = true;
                CoreUsing3srv3.Checked = true;
                CoreUsing4srv3.Checked = true;
                CoreUsing5srv3.Checked = true;
                CoreUsing6srv3.Checked = true;
                CoreUsing7srv3.Checked = true;
                CoreUsing1srv4.Checked = true;
                CoreUsing2srv4.Checked = true;
                CoreUsing3srv4.Checked = true;
                CoreUsing4srv4.Checked = true;
                CoreUsing5srv4.Checked = true;
                CoreUsing6srv4.Checked = true;
                CoreUsing7srv4.Checked = true;
            }
            else if (CPU_INFO >= 8)
            {
                ServerEvents.Items.Add("[SYSTEM]: Available core for selections: " + CPU_INFO).ForeColor = Color.DeepPink;
                CoreUsing1.Enabled = true;
                CoreUsing2.Enabled = true;
                CoreUsing3.Enabled = true;
                CoreUsing4.Enabled = true;
                CoreUsing5.Enabled = true;
                CoreUsing6.Enabled = true;
                CoreUsing7.Enabled = true;
                CoreUsing8.Enabled = true;
                CoreUsing1srv2.Enabled = true;
                CoreUsing2srv2.Enabled = true;
                CoreUsing3srv2.Enabled = true;
                CoreUsing4srv2.Enabled = true;
                CoreUsing5srv2.Enabled = true;
                CoreUsing6srv2.Enabled = true;
                CoreUsing7srv2.Enabled = true;
                CoreUsing8srv2.Enabled = true;
                CoreUsing1srv3.Enabled = true;
                CoreUsing2srv3.Enabled = true;
                CoreUsing3srv3.Enabled = true;
                CoreUsing4srv3.Enabled = true;
                CoreUsing5srv3.Enabled = true;
                CoreUsing6srv3.Enabled = true;
                CoreUsing7srv3.Enabled = true;
                CoreUsing8srv3.Enabled = true;
                CoreUsing1srv4.Enabled = true;
                CoreUsing2srv4.Enabled = true;
                CoreUsing3srv4.Enabled = true;
                CoreUsing4srv4.Enabled = true;
                CoreUsing5srv4.Enabled = true;
                CoreUsing6srv4.Enabled = true;
                CoreUsing7srv4.Enabled = true;
                CoreUsing8srv4.Enabled = true;

                CoreUsing1.Checked = true;
                CoreUsing2.Checked = true;
                CoreUsing3.Checked = true;
                CoreUsing4.Checked = true;
                CoreUsing5.Checked = true;
                CoreUsing6.Checked = true;
                CoreUsing7.Checked = true;
                CoreUsing8.Checked = true;
                CoreUsing1srv2.Checked = true;
                CoreUsing2srv2.Checked = true;
                CoreUsing3srv2.Checked = true;
                CoreUsing4srv2.Checked = true;
                CoreUsing5srv2.Checked = true;
                CoreUsing6srv2.Checked = true;
                CoreUsing7srv2.Checked = true;
                CoreUsing8srv2.Checked = true;
                CoreUsing1srv3.Checked = true;
                CoreUsing2srv3.Checked = true;
                CoreUsing3srv3.Checked = true;
                CoreUsing4srv3.Checked = true;
                CoreUsing5srv3.Checked = true;
                CoreUsing6srv3.Checked = true;
                CoreUsing7srv3.Checked = true;
                CoreUsing8srv3.Checked = true;
                CoreUsing1srv4.Checked = true;
                CoreUsing2srv4.Checked = true;
                CoreUsing3srv4.Checked = true;
                CoreUsing4srv4.Checked = true;
                CoreUsing5srv4.Checked = true;
                CoreUsing6srv4.Checked = true;
                CoreUsing7srv4.Checked = true;
                CoreUsing8srv4.Checked = true;
            }
        }


        int ProcessPriorityClassResult;
        int core1, core2, core3, core4, core5, core6, core7, core8, Result;
        int core1sv2, core2sv2, core3sv2, core4sv2, core5sv2, core6sv2, core7sv2, core8sv2, Resultsv2;
        int core1sv3, core2sv3, core3sv3, core4sv3, core5sv3, core6sv3, core7sv3, core8sv3, Resultsv3;
        int core1sv4, core2sv4, core3sv4, core4sv4, core5sv4, core6sv4, core7sv4, core8sv4, Resultsv4;

        private void CoreUsing1_CheckedChanged(object sender, EventArgs e)
        {
            if (CoreUsing1.CheckState == CheckState.Checked)
            {
                core1 = 0x0001; // HEX const
            }
            else
            {
                core1 = 0;
            }
            Result = core1 + core2 + core3 + core4 + core5 + core6 + core7 + core8;
        }

        private void CoreUsing2_CheckedChanged(object sender, EventArgs e)
        {
            if (CoreUsing2.CheckState == CheckState.Checked)
            {
                core2 = 0x0002;
            }
            else
            {
                core2 = 0;
            }
            Result = core1 + core2 + core3 + core4 + core5 + core6 + core7 + core8;
        }

        private void CoreUsing3_CheckedChanged(object sender, EventArgs e)
        {
            if (CoreUsing3.CheckState == CheckState.Checked)
            {
                core3 = 0x0004;
            }
            else
            {
                core3 = 0;
            }
            Result = core1 + core2 + core3 + core4 + core5 + core6 + core7 + core8;
        }

        private void CoreUsing4_CheckedChanged(object sender, EventArgs e)
        {
            if (CoreUsing4.CheckState == CheckState.Checked)
            {
                core4 = 0x0008;
            }
            else
            {
                core4 = 0;
            }
            Result = core1 + core2 + core3 + core4 + core5 + core6 + core7 + core8;
        }

        private void CoreUsing5_CheckedChanged(object sender, EventArgs e)
        {
            if (CoreUsing5.CheckState == CheckState.Checked)
            {
                core5 = 0x0010;
            }
            else
            {
                core5 = 0;
            }
            Result = core1 + core2 + core3 + core4 + core5 + core6 + core7 + core8;
        }

        private void CoreUsing6_CheckedChanged(object sender, EventArgs e)
        {
            if (CoreUsing6.CheckState == CheckState.Checked)
            {
                core6 = 0x0020;
            }
            else
            {
                core6 = 0;
            }
            Result = core1 + core2 + core3 + core4 + core5 + core6 + core7 + core8;
        }

        private void CoreUsing7_CheckedChanged(object sender, EventArgs e)
        {
            if (CoreUsing7.CheckState == CheckState.Checked)
            {
                core7 = 0x0040;
            }
            else
            {
                core7 = 0;
            }
            Result = core1 + core2 + core3 + core4 + core5 + core6 + core7 + core8;
        }

        private void CoreUsing8_CheckedChanged(object sender, EventArgs e)
        {
            if (CoreUsing8.CheckState == CheckState.Checked)
            {
                core8 = 0x0080;
            }
            else
            {
                core8 = 0;
            }
            Result = core1 + core2 + core3 + core4 + core5 + core6 + core7 + core8;
        }
        // server 2
        private void CoreUsing1srv2_CheckedChanged(object sender, EventArgs e)
        {
            if (CoreUsing1srv2.CheckState == CheckState.Checked)
            {
                core1sv2 = 0x0001;
            }
            else
            {
                core1sv2 = 0;
            }
            Resultsv2 = core1sv2 + core2sv2 + core3sv2 + core4sv2 + core5sv2 + core6sv2 + core7sv2 + core8sv2;
        }

        private void CoreUsing2srv2_CheckedChanged(object sender, EventArgs e)
        {
            if (CoreUsing2srv2.CheckState == CheckState.Checked)
            {
                core2sv2 = 0x0002;
            }
            else
            {
                core2sv2 = 0;
            }
            Resultsv2 = core1sv2 + core2sv2 + core3sv2 + core4sv2 + core5sv2 + core6sv2 + core7sv2 + core8sv2;
        }

        private void CoreUsing3srv2_CheckedChanged(object sender, EventArgs e)
        {
            if (CoreUsing3srv2.CheckState == CheckState.Checked)
            {
                core3sv2 = 0x0004;
            }
            else
            {
                core3sv2 = 0;
            }
            Resultsv2 = core1sv2 + core2sv2 + core3sv2 + core4sv2 + core5sv2 + core6sv2 + core7sv2 + core8sv2;
        }

        private void CoreUsing4srv2_CheckedChanged(object sender, EventArgs e)
        {
            if (CoreUsing4srv2.CheckState == CheckState.Checked)
            {
                core4sv2 = 0x0008;
            }
            else
            {
                core4sv2 = 0;
            }
            Resultsv2 = core1sv2 + core2sv2 + core3sv2 + core4sv2 + core5sv2 + core6sv2 + core7sv2 + core8sv2;
        }

        private void CoreUsing5srv2_CheckedChanged(object sender, EventArgs e)
        {
            if (CoreUsing5srv2.CheckState == CheckState.Checked)
            {
                core5sv2 = 0x0010;
            }
            else
            {
                core5sv2 = 0;
            }
            Resultsv2 = core1sv2 + core2sv2 + core3sv2 + core4sv2 + core5sv2 + core6sv2 + core7sv2 + core8sv2;
        }

        private void CoreUsing6srv2_CheckedChanged(object sender, EventArgs e)
        {
            if (CoreUsing6srv2.CheckState == CheckState.Checked)
            {
                core6sv2 = 0x0020;
            }
            else
            {
                core6sv2 = 0;
            }
            Resultsv2 = core1sv2 + core2sv2 + core3sv2 + core4sv2 + core5sv2 + core6sv2 + core7sv2 + core8sv2;
        }

        private void CoreUsing7srv2_CheckedChanged(object sender, EventArgs e)
        {
            if (CoreUsing7srv2.CheckState == CheckState.Checked)
            {
                core7sv2 = 0x0040;
            }
            else
            {
                core7sv2 = 0;
            }
            Resultsv2 = core1sv2 + core2sv2 + core3sv2 + core4sv2 + core5sv2 + core6sv2 + core7sv2 + core8sv2;
        }

        private void CoreUsing8srv2_CheckedChanged(object sender, EventArgs e)
        {
            if (CoreUsing8srv2.CheckState == CheckState.Checked)
            {
                core8sv2 = 0x0080;
            }
            else
            {
                core8sv2 = 0;
            }
            Resultsv2 = core1sv2 + core2sv2 + core3sv2 + core4sv2 + core5sv2 + core6sv2 + core7sv2 + core8sv2;
        }
        // server 3
        private void CoreUsing1srv3_CheckedChanged(object sender, EventArgs e)
        {
            if (CoreUsing1srv3.CheckState == CheckState.Checked)
            {
                core1sv3 = 0x0001;
            }
            else
            {
                core1sv3 = 0;
            }
            Resultsv3 = core1sv3 + core2sv3 + core3sv3 + core4sv3 + core5sv3 + core6sv3 + core7sv3 + core8sv3;
        }

        private void CoreUsing2srv3_CheckedChanged(object sender, EventArgs e)
        {
            if (CoreUsing2srv3.CheckState == CheckState.Checked)
            {
                core2sv3 = 0x0002;
            }
            else
            {
                core2sv3 = 0;
            }
            Resultsv3 = core1sv3 + core2sv3 + core3sv3 + core4sv3 + core5sv3 + core6sv3 + core7sv3 + core8sv3;
        }

        private void CoreUsing3srv3_CheckedChanged(object sender, EventArgs e)
        {
            if (CoreUsing3srv3.CheckState == CheckState.Checked)
            {
                core3sv3 = 0x0004;
            }
            else
            {
                core3sv3 = 0;
            }
            Resultsv3 = core1sv3 + core2sv3 + core3sv3 + core4sv3 + core5sv3 + core6sv3 + core7sv3 + core8sv3;
        }

        private void CoreUsing4srv3_CheckedChanged(object sender, EventArgs e)
        {
            if (CoreUsing4srv3.CheckState == CheckState.Checked)
            {
                core4sv3 = 0x0008;
            }
            else
            {
                core4sv3 = 0;
            }
            Resultsv3 = core1sv3 + core2sv3 + core3sv3 + core4sv3 + core5sv3 + core6sv3 + core7sv3 + core8sv3;
        }

        private void CoreUsing5srv3_CheckedChanged(object sender, EventArgs e)
        {
            if (CoreUsing5srv3.CheckState == CheckState.Checked)
            {
                core5sv3 = 0x0010;
            }
            else
            {
                core5sv3 = 0;
            }
            Resultsv3 = core1sv3 + core2sv3 + core3sv3 + core4sv3 + core5sv3 + core6sv3 + core7sv3 + core8sv3;
        }

        private void CoreUsing6srv3_CheckedChanged(object sender, EventArgs e)
        {
            if (CoreUsing6srv3.CheckState == CheckState.Checked)
            {
                core6sv3 = 0x0020;
            }
            else
            {
                core6sv3 = 0;
            }
            Resultsv3 = core1sv3 + core2sv3 + core3sv3 + core4sv3 + core5sv3 + core6sv3 + core7sv3 + core8sv3;
        }

        private void CoreUsing7srv3_CheckedChanged(object sender, EventArgs e)
        {
            if (CoreUsing7srv3.CheckState == CheckState.Checked)
            {
                core7sv3 = 0x0040;
            }
            else
            {
                core7sv3 = 0;
            }
            Resultsv3 = core1sv3 + core2sv3 + core3sv3 + core4sv3 + core5sv3 + core6sv3 + core7sv3 + core8sv3;
        }

        private void CoreUsing8srv3_CheckedChanged(object sender, EventArgs e)
        {
            if (CoreUsing8srv3.CheckState == CheckState.Checked)
            {
                core8sv3 = 0x0080;
            }
            else
            {
                core8sv3 = 0;
            }
            Resultsv3 = core1sv3 + core2sv3 + core3sv3 + core4sv3 + core5sv3 + core6sv3 + core7sv3 + core8sv3;
        }
        // server 4 and 5
        private void CoreUsing1srv4_CheckedChanged(object sender, EventArgs e)
        {
            if (CoreUsing1srv4.CheckState == CheckState.Checked)
            {
                core1sv4 = 0x0001;
            }
            else
            {
                core1sv4 = 0;
            }
            Resultsv4 = core1sv4 + core2sv4 + core3sv4 + core4sv4 + core5sv4 + core6sv4 + core7sv4 + core8sv4;
        }

        private void CoreUsing2srv4_CheckedChanged(object sender, EventArgs e)
        {
            if (CoreUsing2srv4.CheckState == CheckState.Checked)
            {
                core2sv4 = 0x0002;
            }
            else
            {
                core2sv4 = 0;
            }
            Resultsv4 = core1sv4 + core2sv4 + core3sv4 + core4sv4 + core5sv4 + core6sv4 + core7sv4 + core8sv4;
        }

        private void CoreUsing3srv4_CheckedChanged(object sender, EventArgs e)
        {
            if (CoreUsing3srv4.CheckState == CheckState.Checked)
            {
                core3sv4 = 0x0004;
            }
            else
            {
                core3sv4 = 0;
            }
            Resultsv4 = core1sv4 + core2sv4 + core3sv4 + core4sv4 + core5sv4 + core6sv4 + core7sv4 + core8sv4;
        }

        private void CoreUsing4srv4_CheckedChanged(object sender, EventArgs e)
        {
            if (CoreUsing4srv4.CheckState == CheckState.Checked)
            {
                core4sv4 = 0x0008;
            }
            else
            {
                core4sv4 = 0;
            }
            Resultsv4 = core1sv4 + core2sv4 + core3sv4 + core4sv4 + core5sv4 + core6sv4 + core7sv4 + core8sv4;
        }

        private void CoreUsing5srv4_CheckedChanged(object sender, EventArgs e)
        {
            if (CoreUsing5srv4.CheckState == CheckState.Checked)
            {
                core5sv4 = 0x0010;
            }
            else
            {
                core5sv4 = 0;
            }
            Resultsv4 = core1sv4 + core2sv4 + core3sv4 + core4sv4 + core5sv4 + core6sv4 + core7sv4 + core8sv4;
        }

        private void CoreUsing6srv4_CheckedChanged(object sender, EventArgs e)
        {
            if (CoreUsing6srv4.CheckState == CheckState.Checked)
            {
                core6sv4 = 0x0020;
            }
            else
            {
                core6sv4 = 0;
            }
            Resultsv4 = core1sv4 + core2sv4 + core3sv4 + core4sv4 + core5sv4 + core6sv4 + core7sv4 + core8sv4;
        }

        private void CoreUsing7srv4_CheckedChanged(object sender, EventArgs e)
        {
            if (CoreUsing7srv4.CheckState == CheckState.Checked)
            {
                core7sv4 = 0x0040;
            }
            else
            {
                core7sv4 = 0;
            }
            Resultsv4 = core1sv4 + core2sv4 + core3sv4 + core4sv4 + core5sv4 + core6sv4 + core7sv4 + core8sv4;
        }

        private void CoreUsing8srv4_CheckedChanged(object sender, EventArgs e)
        {
            if (CoreUsing8srv4.CheckState == CheckState.Checked)
            {
                core8sv4 = 0x0080;
            }
            else
            {
                core8sv4 = 0;
            }
            Resultsv4 = core1sv4 + core2sv4 + core3sv4 + core4sv4 + core5sv4 + core6sv4 + core7sv4 + core8sv4;
        }


        private void sv1update_CheckedChanged(object sender, EventArgs e)
        {
            FunctionServer1UsingCore();
        }

        private void sv2update_CheckedChanged(object sender, EventArgs e)
        {
            FunctionServer2UsingCore();
        }

        private void sv3update_CheckedChanged(object sender, EventArgs e)
        {
            FunctionServer3UsingCore();
        }

        private void sv4update_CheckedChanged(object sender, EventArgs e)
        {
            FunctionServer4UsingCore();
        }

        private void FunctionServer1UsingCore()
        {
            try
            {
                Process.GetProcessById(PID_SRV1).ProcessorAffinity = (IntPtr)Result;
                ServerEvents.Items.Add("[SYSTEM] Using from server [1] values: " + Result).ForeColor = Color.ForestGreen;
            }
            catch (Exception)
            {
                CoreUsing1.Checked = false;
                CoreUsing2.Checked = false;
                CoreUsing3.Checked = false;
                CoreUsing4.Checked = false;
                CoreUsing5.Checked = false;
                CoreUsing6.Checked = false;
                CoreUsing7.Checked = false;
                CoreUsing8.Checked = false;
                ProcessorCount();
                ServerEvents.Items.Add("[SYSTEM] Affinity process srv1 encountered an incorrect value. The default value is set").ForeColor = Color.Violet;
            }
        }

        private void FunctionServer2UsingCore()
        {
            try
            {
                Process.GetProcessById(PID_SRV2).ProcessorAffinity = (IntPtr)Resultsv2;
                ServerEvents.Items.Add("[SYSTEM] Using from server [2] values: " + Resultsv2).ForeColor = Color.ForestGreen;
            }
            catch (Exception)
            {
                CoreUsing1srv2.Checked = false;
                CoreUsing2srv2.Checked = false;
                CoreUsing3srv2.Checked = false;
                CoreUsing4srv2.Checked = false;
                CoreUsing5srv2.Checked = false;
                CoreUsing6srv2.Checked = false;
                CoreUsing7srv2.Checked = false;
                CoreUsing8srv2.Checked = false;
                ProcessorCount();
                ServerEvents.Items.Add("[SYSTEM] Affinity process srv2 encountered an incorrect value. The default value is set").ForeColor = Color.Violet;
            }
        }
        private void FunctionServer3UsingCore()
        {
            try
            {
                Process.GetProcessById(PID_SRV3).ProcessorAffinity = (IntPtr)Resultsv3;
                ServerEvents.Items.Add("[SYSTEM] Using from server [3] values: " + Resultsv3).BackColor = Color.ForestGreen;
            }
            catch (Exception)
            {
                CoreUsing1srv3.Checked = false;
                CoreUsing2srv3.Checked = false;
                CoreUsing3srv3.Checked = false;
                CoreUsing4srv3.Checked = false;
                CoreUsing5srv3.Checked = false;
                CoreUsing6srv3.Checked = false;
                CoreUsing7srv3.Checked = false;
                CoreUsing8srv3.Checked = false;
                ProcessorCount();
                ServerEvents.Items.Add("[SYSTEM] Affinity process srv3 encountered an incorrect value. The default value is set").ForeColor = Color.Violet;
            }
        }

        private void FunctionServer4UsingCore()
        {
            sv4update.Enabled = false;
            new Thread(() =>
            {
                Thread.Sleep(3500);     // после создания сервера, создадим новый поток и ожидаем пока контроллер не перезапустит процесс сервера.
                if (btnStartStop4.Text == "Стоп")
                {
                    try
                    {
                        Process.GetProcessById(PID4).ProcessorAffinity = (IntPtr)Resultsv4;
                        ServerEvents.Items.Add("[SYSTEM] Using from server [4] values: " + Resultsv4).ForeColor = Color.ForestGreen;
                    }
                    catch (Exception)
                    {
                        CoreUsing1srv4.Checked = false;
                        CoreUsing2srv4.Checked = false;
                        CoreUsing3srv4.Checked = false;
                        CoreUsing4srv4.Checked = false;
                        CoreUsing5srv4.Checked = false;
                        CoreUsing6srv4.Checked = false;
                        CoreUsing7srv4.Checked = false;
                        CoreUsing8srv4.Checked = false;
                        ProcessorCount();
                        ServerEvents.Items.Add("[SYSTEM] Affinity process srv4,5 encountered an incorrect value. The default value is set").ForeColor = Color.Violet;
                    }
                }
                // server 2
                if (btnStartStop5.Text == "Стоп")
                {
                    try
                    {
                        Process.GetProcessById(PID5).ProcessorAffinity = (IntPtr)Resultsv4;
                        ServerEvents.Items.Add("[SYSTEM] Using from server [5] values: " + Resultsv4).ForeColor = Color.ForestGreen;
                    }
                    catch (Exception)
                    {
                        CoreUsing1srv4.Checked = true;
                        sv4update.Enabled = true;
                    }
                }
                sv4update.Enabled = true;
            }).Start();
        }

        private void trackMenuPriority_Scroll(object sender, EventArgs e)
        {
            if (trackMenuPriority.Value == 0)
            {
                ProcessPriorityClassResult = 0x00000020; // standart
            }
            if (trackMenuPriority.Value == 1)
            {
                ProcessPriorityClassResult = 0x00000040; // low
            }
            if (trackMenuPriority.Value == 2)
            {
                ProcessPriorityClassResult = 0x00004000; // below average
            }
            if (trackMenuPriority.Value == 3)
            {
                ProcessPriorityClassResult = 0x00000020; // average
            }
            if (trackMenuPriority.Value == 4)
            {
                ProcessPriorityClassResult = 0x00008000; // above average
            }
            if (trackMenuPriority.Value == 5)
            {
                ProcessPriorityClassResult = 0x00000080; // high
            }
        }


        [DllImport("Kernel32.dll")]
        static extern bool SetPriorityClass(IntPtr hProcess, int dwPriorityClass);
        private void FuncPriorityClassChangeServer1()
        {

            new Thread(() =>
            {
                try
                {
                    Thread.Sleep(3500);
                    Process.GetProcessById(PID_SRV1);
                    SetPriorityClass(Process.GetProcessById(PID_SRV1).Handle, ProcessPriorityClassResult);
                }
                catch (Exception ex)
                {
                    ServerEvents.Items.Add("[SYSTEM]: Set Priority Class, Function terminates with error. " + ex.Message).ForeColor = Color.Red;
                }
            }).Start();

        }
        private void FuncPriorityClassChangeServer2()
        {

            new Thread(() =>
            {
                try
                {
                    Thread.Sleep(3500);
                    Process.GetProcessById(PID_SRV2);
                    SetPriorityClass(Process.GetProcessById(PID_SRV2).Handle, ProcessPriorityClassResult);
                }
                catch (Exception ex)
                {
                    ServerEvents.Items.Add("[SYSTEM]: Set Priority Class, Function terminates with error. " + ex.Message).ForeColor = Color.Red;
                }
            }).Start();

        }
        private void FuncPriorityClassChangeServer3()
        {

            new Thread(() =>
            {
                try
                {
                    Thread.Sleep(3500);
                    Process.GetProcessById(PID_SRV3);
                    SetPriorityClass(Process.GetProcessById(PID_SRV3).Handle, ProcessPriorityClassResult);
                }
                catch (Exception ex)
                {
                    ServerEvents.Items.Add("[SYSTEM]: Set Priority Class, Function terminates with error. " + ex.Message).ForeColor = Color.Red;
                }

            }).Start();
        }
        private void FuncPriorityClassChangeServer4()
        {
            new Thread(() =>
            {
                Thread.Sleep(3500);
                if (btnStartStop4.Text == "Стоп")
                {
                    try
                    {
                        Process.GetProcessById(PID4);
                        SetPriorityClass(Process.GetProcessById(PID4).Handle, ProcessPriorityClassResult);
                    }
                    catch (Exception)
                    {
                        //ServerEventList.AppendText("[SYSTEM]: ServerNotFound => server 3 " + Environment.NewLine);
                    }
                }
                if (btnStartStop5.Text == "Стоп")
                {
                    try
                    {
                        Process.GetProcessById(PID5);
                        SetPriorityClass(Process.GetProcessById(PID5).Handle, ProcessPriorityClassResult);
                    }
                    catch (Exception)
                    {
                        //ServerEventList.AppendText("[SYSTEM]: ServerNotFound => server 3 " + Environment.NewLine);
                    }
                }
            }).Start();
        }

        private void FormMenu1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F3)
            {
                try
                {
                    Process.Start(@"AdminAssistant\AdminAssistant.exe");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "AdminAssistant Loading Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            if (e.KeyCode == Keys.F4)
            {
                try
                {
                    Process.Start(@"server_settings\logs\xray_" + SystemInformation.UserName + ".log");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Open File Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            if (e.KeyCode == Keys.F5)
            {
                try
                {
                    Process.Start(@"server_settings\logs\");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Open Directory Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void ProcessMemoryController_CheckedChanged(object sender, EventArgs e)
        {
            if (ProcessMemoryController.CheckState == CheckState.Checked)
                MemoryController.Start();
            else
                MemoryController.Stop();
        }


        private void NetworkInformation_Tick(object sender, EventArgs e)
        {
            if (btnStartStop1.Text == "Стоп")
            {
                try
                {
                    var ProcMemoryValue = (Process.GetProcessById(PID_SRV1).WorkingSet64);
                    if (ProcMemoryValue / 1024 / 1024 > Convert.ToUInt32(MaxMemoryValue.Text))
                    {
                        ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss]")) + "[MEMORY] Использование памяти процессом сервера превышает заданный лимит: " + ProcMemoryValue / 1024 / 1024 + " [Server: 1] ID: " + PID_SRV1).ForeColor = Color.Aqua;
                        try
                        {
                            Process Id = Process.GetProcessById(PID_SRV1);
                            Id.Kill();
                            Thread.Sleep(500);
                            LoadingServer1(); // calling func start server
                            ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss]")) + "[SYSTEM] Server: 1 Restarted").ForeColor = Color.Aqua;

                        }
                        catch (Exception)
                        {
                            ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss]")) + "[SYSTEM] Server: 1 Not Found").ForeColor = Color.Aqua;
                        }
                    }
                }
                catch (Exception)
                {
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss]")) + "[MEMORY] Server Not Found").ForeColor = Color.Aqua;
                }
            }
            if (btnStartStop2.Text == "Стоп")
            {
                try
                {
                    var ProcMemoryValue = (Process.GetProcessById(PID_SRV2).WorkingSet64);
                    if (ProcMemoryValue / 1024 / 1024 > Convert.ToUInt32(MaxMemoryValue.Text))
                    {
                        ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss]")) + "[MEMORY] Использование памяти процессом сервера превышает заданный лимит: " + ProcMemoryValue / 1024 / 1024 + " [Server: 2] ID: " + PID_SRV2).ForeColor = Color.Aqua;
                        try
                        {
                            Process Id = Process.GetProcessById(PID_SRV2);
                            Id.Kill();
                            Thread.Sleep(500);
                            LoadingServer2();
                            ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss]")) + "[SYSTEM] Server: 2 Restarted").ForeColor = Color.Aqua;
                        }
                        catch (Exception)
                        {
                            ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss]")) + "[SYSTEM] Server: 2 Not Found").ForeColor = Color.Aqua;
                        }
                    }
                }
                catch (Exception)
                {
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss]")) + "[MEMORY] Server Not Found").ForeColor = Color.Aqua;
                }
            }
            if (btnStartStop3.Text == "Стоп")
            {
                try
                {
                    var ProcMemoryValue = (Process.GetProcessById(PID_SRV3).WorkingSet64);
                    if (ProcMemoryValue / 1024 / 1024 > Convert.ToUInt32(MaxMemoryValue.Text))
                    {
                        ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss]")) + "[MEMORY] Использование памяти процессом сервера превышает заданный лимит: " + ProcMemoryValue / 1024 / 1024 + " [Server: 3] ID: " + PID_SRV3).ForeColor = Color.Aqua;
                        try
                        {
                            Process Id = Process.GetProcessById(PID_SRV3);
                            Id.Kill();
                            Thread.Sleep(500);
                            LoadingServer3();
                            ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss]")) + "[SYSTEM] Server: 3 Restarted").ForeColor = Color.Aqua;
                        }
                        catch (Exception)
                        {
                            ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss]")) + "[SYSTEM] Server: 3 Not Found").ForeColor = Color.Aqua;
                        }
                    }
                }
                catch (Exception)
                {
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss]")) + "[MEMORY] Server Not Found").ForeColor = Color.Aqua;
                }
            }
            if (btnStartStop4.Text == "Стоп")
            {
                try
                {
                    var ProcMemoryValue = (Process.GetProcessById(PID4).WorkingSet64);
                    if (ProcMemoryValue / 1024 / 1024 > Convert.ToUInt32(MaxMemoryValue.Text))
                    {
                        ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss]")) + "[MEMORY] Использование памяти процессом сервера превышает заданный лимит: " + ProcMemoryValue / 1024 / 1024 + " [Server: 4] ID: " + PID4).ForeColor = Color.Aqua;
                        try
                        {
                            Process Id = Process.GetProcessById(PID4);
                            Id.Kill();
                            Thread.Sleep(500);
                            LoadingServer4();
                            ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss]")) + "[SYSTEM] Server: 4 Restarted").ForeColor = Color.Aqua;
                        }
                        catch (Exception)
                        {
                            ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss]")) + "[SYSTEM] Server: 4 Not Found").ForeColor = Color.Aqua;
                        }
                    }
                }
                catch (Exception)
                {
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss]")) + "[MEMORY] Server Not Found").ForeColor = Color.Aqua;
                }
            }
            if (btnStartStop5.Text == "Стоп")
            {
                try
                {
                    var ProcMemoryValue = (Process.GetProcessById(PID5).WorkingSet64);
                    if (ProcMemoryValue / 1024 / 1024 > Convert.ToUInt32(MaxMemoryValue.Text))
                    {
                        ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss]")) + "[MEMORY] Использование памяти процессом сервера превышает заданный лимит: " + ProcMemoryValue / 1024 / 1024 + " [Server: 5] ID: " + PID5).ForeColor = Color.Aqua;
                        try
                        {
                            Process Id = Process.GetProcessById(PID5);
                            Id.Kill();
                            Thread.Sleep(500);
                            LoadingServer5();
                            ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss]")) + "[SYSTEM] Server: 5 Restarted").ForeColor = Color.Aqua;
                        }
                        catch (Exception)
                        {
                            ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss]")) + "[SYSTEM] Server: 5 Not Found").ForeColor = Color.Aqua;
                        }
                    }
                }
                catch (Exception)
                {
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss]")) + "[MEMORY] Server Not Found").ForeColor = Color.Aqua;
                }
            }
        }


#if (DEBUG)
        private void SendMail()
        {
            if (SendToMail.CheckState == CheckState.Checked)
            {
                try
                {
                    new Thread(() =>
                    {
                        MailMessage mail = new MailMessage();
                        mail.From = new MailAddress("tsmp.project@yandex.ru");                    // Адрес отправителя
                        mail.To.Add(new MailAddress("msol0@outlook.com"));                        // Адрес получателя
                        mail.Subject = "server crashed ";                                         // заголовок сообщения
                                                                                                  // наше сообщение 
                        mail.Body = "server log file" + Environment.NewLine + "Сообщение получено: " + DateTime.Now + Environment.NewLine + 
                        SrvName1.Text + Environment.NewLine +
                        SrvName2.Text + Environment.NewLine + 
                        SrvName3.Text + Environment.NewLine + 
                        SrvName4.Text + Environment.NewLine + 
                        SrvName5.Text;    
                        mail.Attachments.Add(new Attachment(@"server_settings\logs\xray_" + SystemInformation.UserName + ".log"));
                        try
                        {
                            string date = DateTime.Now.ToString("MM-dd-yy");
                            foreach (var file in Directory.GetFiles(@"server_settings\logs\", "xray_" + SystemInformation.UserName + "_" + date + "_*.mdmp"))
                            {
                                mail.Attachments.Add(new Attachment(file));
                                ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss] ")) + "SendMail: Write File: " + file).BackColor = Color.Gold;
                            }
                            foreach (var file in Directory.GetFiles(@"server_settings\logs\", "xray_" + SystemInformation.UserName + "_" + date + "_*.mdmp.log"))
                            {
                                mail.Attachments.Add(new Attachment(file));
                                ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss] ")) + "SendMail: Write File: " + file).BackColor = Color.Gold;
                            }
                        }
                        catch (Exception ex)
                        {
                            ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss]")) + "Dump Files Not Found " + ex.Message).BackColor = Color.Blue;
                        }
                        SmtpClient client = new SmtpClient();
                        client.Host = "smtp.yandex.ru";
                        client.Port = 587;
                        client.EnableSsl = true;                                                          // защищенное соединение
                        client.Credentials = new NetworkCredential("tsmp.project@yandex.ru", "654321tT"); // login & password отправителя
                        client.Send(mail);
                        client.Timeout = 15000;
                        client.Dispose();                                                                 // правильно завершим отправку
                        ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss] ")) + "SendMail: Sending Mail Finished").BackColor = Color.Lime;
                    }).Start();
                }
                catch (Exception ex)
                {
                    ServerEvents.Items.Add((DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss]")) + "SendMail: Message hasn't been sent : " + ex.Message).BackColor = Color.Red;
                }
            }
        }
#endif     
    }
}