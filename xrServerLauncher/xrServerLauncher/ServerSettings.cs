using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;

namespace S.E.R.V.E.R___Shadow_Of_Chernobyl_1._0006
{
    public partial class ServerSettings : Form
    {
        Settings server_settings = null;
        public ServerSettings()
        {
            server_settings = Settings.GetSettings();
            InitializeComponent();
            change_interface();
            DYNAMIC_SET_COLOR();
            Initialize_server_settings();
#if (!DEBUG)
            tabControl1.TabPages[6].Parent = null; // скрываем вкладу tab control
            tabControl1.TabPages[5].Parent = null; // скрываем вкладу tab control
            tabControl1.TabPages[4].Parent = null; // скрываем вкладу tab control
            checkEnableEditor.Enabled = false;
            checkEnableDeathmatchEditor.Enabled = false;
#endif
            //tabControl1.TabPages[6].Parent = null; // скрываем вкладу tab control, нельзя в общий доступ :)
            //tabControl1.TabPages[5].Parent = null; // скрываем вкладу tab control
            //tabControl1.TabPages[4].Parent = null; // скрываем вкладу tab control
        }
        int v_value = 0;
        private void change_interface()
        {
            v_value = Convert.ToInt32(SendMessageVoteTime.SendMsgVoteTime);
            if (v_value == 128)
            {
                settings_asm45.Text = "Активировать запрет снаряжения";
                tabPage2.Text = "Параметры TSMP";
                UI_CHANGE.Text = "Набор снаряжения";
                btnLoad.Visible = false;
                VoteTime.Minimum = Convert.ToDecimal(30);
                VoteTime.DecimalPlaces = 0;
                VoteTime.Maximum = Convert.ToDecimal(180);
                VoteTime.Increment = Convert.ToDecimal(30);
                asm_message.Visible = false;
                settings_asm46.Visible = true;
                settings_asm47.Visible = true;
                settings_asm48.Visible = true;
                UI_TSMP.Visible = true;
                UI_TSMP1.Visible = true;
                UI_TSMP2.Visible = true;
                RadioBlockMSGLimit.Visible = true;
                RadioBlockTime.Visible = true;
                RadioCreateCmd.Visible = true;
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

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Сохранить все заданные настройки сервера перед выходом?", "Сохранение", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
            if (dialogResult == DialogResult.Yes)
            {
                commands_create();
                radmins_create();
                Weapons_table_create();
                Save_server_settings();
                Close();
            }
            else if (dialogResult == DialogResult.No)
            {
                Close();
            }
        }

        private void checkUpdateRadmins_Click(object sender, EventArgs e)
        {
            radmins_create();
            MessageBox.Show("Данные Radmin успешно обновлены!", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void btnLoad_Click(object sender, EventArgs e)
        {
            btnLoad.Enabled = false;
            Weapons_table_create();
            Thread.Sleep(100);
            try
            {
                Process.Start(@"bin\server.exe");
            }
            catch (Exception ex)
            {
                btnLoad.Text = "Произошла ошибка: " + ex.Message;
            }
            btnLoad.Enabled = true;
        }
        // =============================================== WRITE FILE ===============================================
        // server_settings.ltx
        private static void Save(string save_settings)
        {
            StreamWriter save = new StreamWriter(@"server_settings\all_server_settings.ltx", true, Encoding.GetEncoding("windows-1251"));
            save.Write(save_settings);
            save.Close();
        }
        // radmins.ltx
        private static void Radmins(string radmins)
        {
            StreamWriter writer = new StreamWriter(@"server_settings\radmins.ltx", true, Encoding.GetEncoding("windows-1251"));
            writer.Write(radmins);
            writer.Close();
        }
        // asm weapons.txt
        private static void Weapons_close(string asm)
        {
            StreamWriter weapon_disable = new StreamWriter(@"weapons.txt", true, Encoding.GetEncoding("windows-1251"));
            weapon_disable.Write(asm);
            weapon_disable.Close();
        }
        // Auto Change Maps
        private static void MapsRotation(string changelevel)
        {
            StreamWriter maps = new StreamWriter(@"server_settings\maprot_list.ltx", true, Encoding.GetEncoding("windows-1251")); // windows-1251
            maps.Write(changelevel);
            maps.Close();
        }
        // ===============================================
        // Artefacthunt Editor
        private static void ArtefacthuntEditor(string GameEditorART)
        {
            StreamWriter editor_check = new StreamWriter(@"gamedata\config\mp\artefacthunt_game.ltx", false, Encoding.GetEncoding("windows-1251"));
            editor_check.Write(GameEditorART);
            editor_check.Close();
        }
        // Deathmatch Editor
        private static void DeathmatchEditor(string GameEditorDM)
        {
            StreamWriter editor_check = new StreamWriter(@"gamedata\config\mp\deathmatch_game.ltx", false, Encoding.GetEncoding("windows-1251"));
            editor_check.Write(GameEditorDM);
            editor_check.Close();
        }
        // teamdeathmatch_game
        private static void TeamDeathmatchEditor(string GameEditorTDM)
        {
            StreamWriter editor_check = new StreamWriter(@"gamedata\config\mp\teamdeathmatch_game.ltx", false, Encoding.GetEncoding("windows-1251"));
            editor_check.Write(GameEditorTDM);
            editor_check.Close();
        }
        // Artefact Spawn Anomaly Editor
        private static void ArtefactAnomalyEditor(string GameEditor)
        {
            StreamWriter editor_check = new StreamWriter(@"gamedata\config\misc\artefacts.ltx", false, Encoding.GetEncoding("windows-1251"));
            editor_check.Write(GameEditor);
            editor_check.Close();
        }
        // =============================================== WRITE FILE ===============================================
        string V_CHANGE;
        string RadioMSG;
        private void commands_create()
        {
            try
            {
                try
               {
                    File.WriteAllText(@"server_settings\all_server_settings.ltx", string.Empty); // clear file
               }
               catch (Exception) { }
                // ===============================================  
                // TSMP 
                string vote_format = VoteTime.Text.Replace(",", ".");
                if (v_value == 128)
                {
                    V_CHANGE = "tsmp_vote_time " + vote_format;
                    if (RadioCreateCmd.CheckState == CheckState.Checked)
                    {
                        RadioMSG = "tsmp_radio_antispam 1" + Environment.NewLine + "tsmp_radio_max_msgs " + RadioBlockMSGLimit.Value + Environment.NewLine + "tsmp_radio_mute_interval " + RadioBlockTime.Value + Environment.NewLine;
                    }
                }
                else
                {
                    V_CHANGE = "sv_vote_time " + vote_format;
                }
                // ===============================================   
                Save("clear_log" + Environment.NewLine + "net_sv_update_rate 20" + Environment.NewLine + "net_sv_pending_lim 10" + Environment.NewLine + "sv_remove_corpse 0" + Environment.NewLine + "g_corpsenum 3 // Value corpse on the maps" + Environment.NewLine + "mm_net_weather_rateofchange 0.000" + Environment.NewLine + "sv_vote_enabled " + ByteResult + Environment.NewLine + "sv_vote_quota 0." + VoteQuota.Value + Environment.NewLine + V_CHANGE + Environment.NewLine + RadioMSG + Environment.NewLine);
                if (v_value == 128)
                {
                    if (settings_asm45.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disabler_enabled 1" + Environment.NewLine);
                    }
                }
                else
                {
                    if (settings_asm45.CheckState == CheckState.Checked)
                    {
                        Save("disable_rename 1" + Environment.NewLine);
                    }
                }
                if (VoteOK.CheckState == CheckState.Checked)
                {
                    Save("sv_vote_participants 1" + Environment.NewLine + Environment.NewLine);
                }
                else
                {
                    Save("sv_vote_participants 0" + Environment.NewLine + Environment.NewLine);
                }

                if (cmd_sv1.Text != "")
                {
                    Save(cmd_sv1.Text + Environment.NewLine);
                }
                if (cmd_sv2.Text != "")
                {
                    Save(cmd_sv2.Text + Environment.NewLine);
                }
                if (cmd_sv3.Text != "")
                {
                    Save(cmd_sv3.Text + Environment.NewLine);
                }
                if (cmd_sv4.Text != "")
                {
                    Save(cmd_sv4.Text + Environment.NewLine);
                }
                if (cmd_sv5.Text != "")
                {
                    Save(cmd_sv5.Text + Environment.NewLine);
                }
                if (cmd_sv6.Text != "")
                {
                    Save(cmd_sv6.Text + Environment.NewLine);
                }
                if (cmd_sv7.Text != "")
                {
                    Save(cmd_sv7.Text + Environment.NewLine);
                }
                if (cmd_sv8.Text != "")
                {
                    Save(cmd_sv8.Text + Environment.NewLine);
                }
                if (cmd_sv9.Text != "")
                {
                    Save(cmd_sv9.Text + Environment.NewLine);
                }
                if (cmd_sv10.Text != "")
                {
                    Save(cmd_sv10.Text + Environment.NewLine);
                }
                if (cmd_sv11.Text != "")
                {
                    Save(cmd_sv11.Text + Environment.NewLine);
                }
                if (cmd_sv12.Text != "")
                {
                    Save(cmd_sv12.Text + Environment.NewLine);
                }
                if (cmd_sv13.Text != "")
                {
                    Save(cmd_sv13.Text + Environment.NewLine);
                }
                if (cmd_sv14.Text != "")
                {
                    Save(cmd_sv14.Text + Environment.NewLine);
                }
                if (cmd_sv15.Text != "")
                {
                    Save(cmd_sv15.Text + Environment.NewLine);
                }
                if (cmd_sv16.Text != "")
                {
                    Save(cmd_sv16.Text + Environment.NewLine);
                }
                if (cmd_sv17.Text != "")
                {
                    Save(cmd_sv17.Text + Environment.NewLine);
                }
                if (cmd_sv18.Text != "")
                {
                    Save(cmd_sv18.Text + Environment.NewLine);
                }
                if (cmd_sv19.Text != "")
                {
                    Save(cmd_sv19.Text + Environment.NewLine);
                }
                if (cmd_sv20.Text != "")
                {
                    Save(cmd_sv20.Text + Environment.NewLine);
                }
                if (cmd_sv21.Text != "")
                {
                    Save(cmd_sv21.Text + Environment.NewLine);
                }
                if (cmd_sv22.Text != "")
                {
                    Save(cmd_sv22.Text + Environment.NewLine);
                }
                if (cmd_sv23.Text != "")
                {
                    Save(cmd_sv23.Text + Environment.NewLine);
                }
                if (cmd_sv24.Text != "")
                {
                    Save(cmd_sv24.Text + Environment.NewLine);
                }
                if (cmd_sv25.Text != "")
                {
                    Save(cmd_sv25.Text + Environment.NewLine);
                }
                if (cmd_sv26.Text != "")
                {
                    Save(cmd_sv26.Text + Environment.NewLine);
                }
                if (cmd_sv27.Text != "")
                {
                    Save(cmd_sv27.Text + Environment.NewLine);
                }
                if (cmd_sv28.Text != "")
                {
                    Save(cmd_sv28.Text + Environment.NewLine);
                }
                if (cmd_sv29.Text != "")
                {
                    Save(cmd_sv29.Text + Environment.NewLine);
                }
                if (cmd_sv30.Text != "")
                {
                    Save(cmd_sv30.Text + Environment.NewLine);
                }
                if (cmd_sv31.Text != "")
                {
                    Save(cmd_sv31.Text + Environment.NewLine);
                }
                if (cmd_sv32.Text != "")
                {
                    Save(cmd_sv32.Text + Environment.NewLine);
                }
                if (cmd_sv33.Text != "")
                {
                    Save(cmd_sv33.Text + Environment.NewLine);
                }
                if (cmd_sv34.Text != "")
                {
                    Save(cmd_sv34.Text + Environment.NewLine);
                }
                if (cmd_sv35.Text != "")
                {
                    Save(cmd_sv35.Text + Environment.NewLine);
                }
                if (cmd_sv36.Text != "")
                {
                    Save(cmd_sv36.Text + Environment.NewLine);
                }
                if (cmd_sv37.Text != "")
                {
                    Save(cmd_sv37.Text + Environment.NewLine);
                }
                if (cmd_sv38.Text != "")
                {
                    Save(cmd_sv38.Text + Environment.NewLine);
                }
                if (cmd_sv39.Text != "")
                {
                    Save(cmd_sv39.Text + Environment.NewLine);
                }
                if (cmd_sv40.Text != "")
                {
                    Save(cmd_sv40.Text + Environment.NewLine);
                }
                if (cmd_sv41.Text != "")
                {
                    Save(cmd_sv41.Text + Environment.NewLine);
                }
                if (cmd_sv42.Text != "")
                {
                    Save(cmd_sv42.Text + Environment.NewLine);
                }
                if (checkEnableDeathmatchEditor.CheckState == CheckState.Checked)
                {
                    Save(Environment.NewLine + "// ===== MODS GAMETYPE - DO NOT TOUCH !!! =====" + Environment.NewLine + "sv_forcerespawn 1" + Environment.NewLine + "sv_anomalies_enabled 1" + Environment.NewLine + "sv_anomalies_length 5" + Environment.NewLine + "net_sv_update_rate 15" + Environment.NewLine +"sv_fraglimit 0" + Environment.NewLine + "sv_timelimit 0" + Environment.NewLine + "sv_remove_weapon 1" + Environment.NewLine);
                    if (v_value == 0)
                    {
                        Save("sv_no_auth_check 1" + Environment.NewLine);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Параметры не удалось сохранить.\nКод ошибки:\n" + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void radmins_create()
        {
            try
            {
                try
                {
                    File.WriteAllText(@"server_settings\radmins.ltx", string.Empty);
                }
                catch (Exception) { }

                Radmins("[radmins]" + Environment.NewLine);
                if (RS1.CheckState == CheckState.Checked)
                {
                    File.AppendAllText(@"server_settings\radmins.ltx", RL1.Text + "=" + RP1.Text + Environment.NewLine);               
                }
                if (RS2.CheckState == CheckState.Checked)
                {
                    File.AppendAllText(@"server_settings\radmins.ltx", RL2.Text + "=" + RP2.Text + Environment.NewLine);
                }
                if (RS3.CheckState == CheckState.Checked)
                {
                    File.AppendAllText(@"server_settings\radmins.ltx", RL3.Text + "=" + RP3.Text + Environment.NewLine);
                }
                if (RS4.CheckState == CheckState.Checked)
                {
                    File.AppendAllText(@"server_settings\radmins.ltx", RL4.Text + "=" + RP4.Text + Environment.NewLine);
                }
                if (RS5.CheckState == CheckState.Checked)
                {
                    File.AppendAllText(@"server_settings\radmins.ltx", RL5.Text + "=" + RP5.Text + Environment.NewLine);
                }
                if (RS6.CheckState == CheckState.Checked)
                {
                    File.AppendAllText(@"server_settings\radmins.ltx", RL6.Text + "=" + RP6.Text + Environment.NewLine);
                }
                if (RS7.CheckState == CheckState.Checked)
                {
                    File.AppendAllText(@"server_settings\radmins.ltx", RL7.Text + "=" + RP7.Text + Environment.NewLine);
                }
                if (RS8.CheckState == CheckState.Checked)
                {
                    File.AppendAllText(@"server_settings\radmins.ltx", RL8.Text + "=" + RP8.Text + Environment.NewLine);
                }
                if (RS9.CheckState == CheckState.Checked)
                {
                    File.AppendAllText(@"server_settings\radmins.ltx", RL9.Text + "=" + RP9.Text + Environment.NewLine);
                }
                if (RS10.CheckState == CheckState.Checked)
                {
                    File.AppendAllText(@"server_settings\radmins.ltx", RL10.Text + "=" + RP10.Text + Environment.NewLine);
                }
                if (RS11.CheckState == CheckState.Checked)
                {
                    File.AppendAllText(@"server_settings\radmins.ltx", RL11.Text + "=" + RP11.Text + Environment.NewLine);
                }
                if (RS12.CheckState == CheckState.Checked)
                {
                    File.AppendAllText(@"server_settings\radmins.ltx", RL12.Text + "=" + RP12.Text + Environment.NewLine);
                }
                if (RS13.CheckState == CheckState.Checked)
                {
                    File.AppendAllText(@"server_settings\radmins.ltx", RL13.Text + "=" + RP13.Text + Environment.NewLine);
                }
                if (RS14.CheckState == CheckState.Checked)
                {
                    File.AppendAllText(@"server_settings\radmins.ltx", RL14.Text + "=" + RP14.Text + Environment.NewLine);
                }
                if (RS15.CheckState == CheckState.Checked)
                {
                    File.AppendAllText(@"server_settings\radmins.ltx", RL15.Text + "=" + RP15.Text + Environment.NewLine);
                }
                if (RS16.CheckState == CheckState.Checked)
                {
                    File.AppendAllText(@"server_settings\radmins.ltx", RL16.Text + "=" + RP16.Text + Environment.NewLine);
                }
                if (RS17.CheckState == CheckState.Checked)
                {
                    File.AppendAllText(@"server_settings\radmins.ltx", RL17.Text + "=" + RP17.Text + Environment.NewLine);
                }
                if (RS18.CheckState == CheckState.Checked)
                {
                    File.AppendAllText(@"server_settings\radmins.ltx", RL18.Text + "=" + RP18.Text + Environment.NewLine);
                }
                if (RS19.CheckState == CheckState.Checked)
                {
                    File.AppendAllText(@"server_settings\radmins.ltx", RL19.Text + "=" + RP19.Text + Environment.NewLine);
                }
                if (RS20.CheckState == CheckState.Checked)
                {
                    File.AppendAllText(@"server_settings\radmins.ltx", RL20.Text + "=" + RP20.Text + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Параметры Radmins не удалось сохранить.\nКод ошибки:\n" + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // v_value
        // 0 - Standart 
        // 128 - TSMP
        private void Weapons_table_create()
        {
            try
            {
                if (v_value == 0)
                {
                    File.WriteAllText(@"weapons.txt", string.Empty);
                    Weapons_close("[weapons]" + Environment.NewLine);
                    if (settings_asm1.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_wpn_pm" + Environment.NewLine);
                    }
                    if (settings_asm2.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_wpn_pb" + Environment.NewLine);
                    }
                    if (settings_asm3.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_wpn_fort" + Environment.NewLine);
                    }
                    if (settings_asm4.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_wpn_walther" + Environment.NewLine);
                    }
                    if (settings_asm5.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_wpn_colt1911" + Environment.NewLine);
                    }
                    if (settings_asm6.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_wpn_sig220" + Environment.NewLine);
                    }
                    if (settings_asm7.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_wpn_usp" + Environment.NewLine);
                    }
                    if (settings_asm8.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_wpn_desert_eagle" + Environment.NewLine);
                    }
                    if (settings_asm9.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_wpn_knife" + Environment.NewLine);
                    }
                    if (settings_asm10.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_wpn_mp5" + Environment.NewLine);
                    }
                    if (settings_asm11.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_wpn_ak74" + Environment.NewLine);
                    }
                    if (settings_asm12.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_wpn_ak74u" + Environment.NewLine);
                    }
                    if (settings_asm13.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_wpn_abakan" + Environment.NewLine);
                    }
                    if (settings_asm14.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_wpn_l85" + Environment.NewLine);
                    }
                    if (settings_asm15.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_wpn_lr300" + Environment.NewLine);
                    }
                    if (settings_asm16.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_wpn_sig550" + Environment.NewLine);
                    }
                    if (settings_asm17.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_wpn_g36" + Environment.NewLine);
                    }
                    if (settings_asm18.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_wpn_val" + Environment.NewLine);
                    }
                    if (settings_asm19.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_wpn_groza" + Environment.NewLine);
                    }
                    if (settings_asm20.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_wpn_fn2000" + Environment.NewLine);
                    }
                    if (settings_asm21.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_wpn_bm16" + Environment.NewLine);
                    }
                    if (settings_asm22.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_wpn_spas12" + Environment.NewLine);
                    }
                    if (settings_asm23.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_wpn_wincheaster1300" + Environment.NewLine);
                    }
                    if (settings_asm24.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_wpn_vintorez" + Environment.NewLine);
                    }
                    if (settings_asm25.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_wpn_svd" + Environment.NewLine);
                    }
                    if (settings_asm26.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_wpn_svu" + Environment.NewLine);
                    }
                    if (settings_asm27.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_wpn_rpg7" + Environment.NewLine);
                    }
                    if (settings_asm28.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_wpn_rg-6" + Environment.NewLine);
                    }
                    if (settings_asm29.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_wpn_addon_grenade_launcher" + Environment.NewLine);
                    }
                    if (settings_asm30.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_exo_outfit" + Environment.NewLine);
                    }
                    if (settings_asm31.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_scientific_outfit" + Environment.NewLine);
                    }
                    if (settings_asm32.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_military_stalker_outfit" + Environment.NewLine);
                    }
                    if (settings_asm33.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_grenade_f1" + Environment.NewLine);
                    }
                    if (settings_asm34.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_grenade_rgd5" + Environment.NewLine);
                    }
                    if (settings_asm35.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_grenade_gd-05" + Environment.NewLine);
                    }
                    if (settings_asm36.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_ammo_og-7b" + Environment.NewLine);
                    }
                    if (settings_asm37.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_ammo_m209" + Environment.NewLine);
                    }
                    if (settings_asm38.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_ammo_vog-25" + Environment.NewLine);
                    }
                    if (settings_asm39.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_medkit" + Environment.NewLine);
                    }
                    if (settings_asm40.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_antirad" + Environment.NewLine);
                    }
                    if (settings_asm41.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_wpn_binoc" + Environment.NewLine);
                    }
                    if (settings_asm42.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_wpn_gauss" + Environment.NewLine);
                    }
                    if (settings_asm43.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_players_rukzak" + Environment.NewLine);
                    }
                    if (settings_asm44.CheckState == CheckState.Checked)
                    {
                        Weapons_close("mp_detector_advances" + Environment.NewLine);
                    }
                    Weapons_close(Environment.NewLine);
                    Weapons_close("[ServerSend]" + Environment.NewLine);
                    if (asm_message.Text != "")
                    {
                        Weapons_close(asm_message.Text + Environment.NewLine + Environment.NewLine);
                    }
                    else
                    {
                        Weapons_close(Environment.NewLine);
                    }
                    Weapons_close("[PlayerTop10]" + Environment.NewLine);
                    Weapons_close("%c[0,0,255,255]" + asm_message.Text);
                }
                else 
                {
                    Save(Environment.NewLine);

                    if (settings_asm1.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_wpn_pm 1" + Environment.NewLine);
                    }

                    if (settings_asm2.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_wpn_pb 1" + Environment.NewLine);
                    }

                    if (settings_asm3.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_wpn_fort 1" + Environment.NewLine);
                    }

                    if (settings_asm4.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_wpn_walther 1" + Environment.NewLine);
                    }

                    if (settings_asm5.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_wpn_colt1911 1" + Environment.NewLine);
                    }

                    if (settings_asm6.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_wpn_sig220 1" + Environment.NewLine);
                    }

                    if (settings_asm7.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_wpn_usp 1" + Environment.NewLine);
                    }

                    if (settings_asm8.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_wpn_desert_eagle 1" + Environment.NewLine);
                    }

                    if (settings_asm9.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_wpn_knife 1" + Environment.NewLine);
                    }

                    if (settings_asm10.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_wpn_mp5 1" + Environment.NewLine);
                    }

                    if (settings_asm11.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_wpn_ak74 1" + Environment.NewLine);
                    }

                    if (settings_asm12.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_wpn_ak74u 1" + Environment.NewLine);
                    }

                    if (settings_asm13.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_wpn_abakan 1" + Environment.NewLine);
                    }

                    if (settings_asm14.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_wpn_l85 1" + Environment.NewLine);
                    }

                    if (settings_asm15.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_wpn_lr300 1" + Environment.NewLine);
                    }

                    if (settings_asm16.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_wpn_sig550 1" + Environment.NewLine);
                    }

                    if (settings_asm17.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_wpn_g36 1" + Environment.NewLine);
                    }

                    if (settings_asm18.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_wpn_val 1" + Environment.NewLine);
                    }

                    if (settings_asm19.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_wpn_groza 1" + Environment.NewLine);
                    }

                    if (settings_asm20.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_wpn_fn2000 1" + Environment.NewLine);
                    }

                    if (settings_asm21.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_wpn_bm16 1" + Environment.NewLine);
                    }

                    if (settings_asm22.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_wpn_spas12 1" + Environment.NewLine);
                    }

                    if (settings_asm23.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_wpn_wincheaster1300 1" + Environment.NewLine);
                    }

                    if (settings_asm24.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_wpn_vintorez 1" + Environment.NewLine);
                    }

                    if (settings_asm25.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_wpn_svd 1" + Environment.NewLine);
                    }

                    if (settings_asm26.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_wpn_svu 1" + Environment.NewLine);
                    }

                    if (settings_asm27.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_wpn_rpg7 1" + Environment.NewLine);
                    }

                    if (settings_asm28.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_wpn_rg-6 1" + Environment.NewLine);
                    }

                    if (settings_asm29.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_wpn_addon_grenade_launcher 1" + Environment.NewLine);
                    }

                    if (settings_asm30.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_exo_outfit 1" + Environment.NewLine);
                    }

                    if (settings_asm31.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_scientific_outfit 1" + Environment.NewLine);
                    }

                    if (settings_asm32.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_military_stalker_outfit 1" + Environment.NewLine);
                    }

                    if (settings_asm33.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_grenade_f1 1" + Environment.NewLine);
                    }

                    if (settings_asm34.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_grenade_rgd5 1" + Environment.NewLine);
                    }

                    if (settings_asm35.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_grenade_gd-05 1" + Environment.NewLine);
                    }

                    if (settings_asm36.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_ammo_og-7b 1" + Environment.NewLine);
                    }

                    if (settings_asm37.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_ammo_m209 1" + Environment.NewLine);
                    }

                    if (settings_asm38.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_ammo_vog-25 1" + Environment.NewLine);
                    }

                    if (settings_asm39.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_medkit 1" + Environment.NewLine);
                    }

                    if (settings_asm40.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_antirad 1" + Environment.NewLine);
                    }

                    if (settings_asm41.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_wpn_binoc 1" + Environment.NewLine);
                    }

                    if (settings_asm42.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_wpn_gauss 1" + Environment.NewLine);
                    }

                    if (settings_asm43.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_players_rukzak 1" + Environment.NewLine);
                    }

                    if (settings_asm44.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable mp_detector_advances 1" + Environment.NewLine);
                    }
                    // group
                    if (settings_asm46.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable granati 1" + Environment.NewLine);
                    }
                    if (settings_asm47.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable podstvol 1" + Environment.NewLine);
                    }
                    if (settings_asm48.CheckState == CheckState.Checked)
                    {
                        Save("tsmp_weapon_disable hard_weapon 1" + Environment.NewLine);
                    }
                }
            }
            catch (Exception) { }
        }

        private void RS1_CheckedChanged(object sender, EventArgs e)
        {
            if (RS1.CheckState == CheckState.Checked)
            {
                RS1.Text = "Включена";
                RS1.ForeColor = Color.LimeGreen;
                RP1.UseSystemPasswordChar = true;
            }
            else
            {
                RS1.Text = "Отключена";
                RS1.ForeColor = Color.Fuchsia;
                RP1.UseSystemPasswordChar = false;
            }
        }

        private void RS2_CheckedChanged(object sender, EventArgs e)
        {
            if (RS2.CheckState == CheckState.Checked)
            {
                RS2.Text = "Включена";
                RS2.ForeColor = Color.LimeGreen;
                RP2.UseSystemPasswordChar = true;
            }
            else
            {
                RS2.Text = "Отключена";
                RS2.ForeColor = Color.Fuchsia;
                RP2.UseSystemPasswordChar = false;
            }
        }

        private void RS3_CheckedChanged(object sender, EventArgs e)
        {
            if (RS3.CheckState == CheckState.Checked)
            {
                RS3.Text = "Включена";
                RS3.ForeColor = Color.LimeGreen;
                RP3.UseSystemPasswordChar = true;
            }
            else
            {
                RS3.Text = "Отключена";
                RS3.ForeColor = Color.Fuchsia;
                RP3.UseSystemPasswordChar = false;
            }
        }

        private void RS4_CheckedChanged(object sender, EventArgs e)
        {
            if (RS4.CheckState == CheckState.Checked)
            {
                RS4.Text = "Включена";
                RS4.ForeColor = Color.LimeGreen;
                RP4.UseSystemPasswordChar = true;
            }
            else
            {
                RS4.Text = "Отключена";
                RS4.ForeColor = Color.Fuchsia;
                RP4.UseSystemPasswordChar = false;
            }
        }

        private void RS5_CheckedChanged(object sender, EventArgs e)
        {
            if (RS5.CheckState == CheckState.Checked)
            {
                RS5.Text = "Включена";
                RS5.ForeColor = Color.LimeGreen;
                RP5.UseSystemPasswordChar = true;
            }
            else
            {
                RS5.Text = "Отключена";
                RS5.ForeColor = Color.Fuchsia;
                RP5.UseSystemPasswordChar = false;
            }
        }

        private void RS6_CheckedChanged(object sender, EventArgs e)
        {
            if (RS6.CheckState == CheckState.Checked)
            {
                RS6.Text = "Включена";
                RS6.ForeColor = Color.LimeGreen;
                RP6.UseSystemPasswordChar = true;
            }
            else
            {
                RS6.Text = "Отключена";
                RS6.ForeColor = Color.Fuchsia;
                RP6.UseSystemPasswordChar = false;
            }
        }

        private void RS7_CheckedChanged(object sender, EventArgs e)
        {
            if (RS7.CheckState == CheckState.Checked)
            {
                RS7.Text = "Включена";
                RS7.ForeColor = Color.LimeGreen;
                RP7.UseSystemPasswordChar = true;
            }
            else
            {
                RS7.Text = "Отключена";
                RS7.ForeColor = Color.Fuchsia;
                RP7.UseSystemPasswordChar = false;
            }
        }

        private void RS8_CheckedChanged(object sender, EventArgs e)
        {
            if (RS8.CheckState == CheckState.Checked)
            {
                RS8.Text = "Включена";
                RS8.ForeColor = Color.LimeGreen;
                RP8.UseSystemPasswordChar = true;
            }
            else
            {
                RS8.Text = "Отключена";
                RS8.ForeColor = Color.Fuchsia;
                RP8.UseSystemPasswordChar = false;
            }
        }

        private void RS9_CheckedChanged(object sender, EventArgs e)
        {
            if (RS9.CheckState == CheckState.Checked)
            {
                RS9.Text = "Включена";
                RS9.ForeColor = Color.LimeGreen;
                RP9.UseSystemPasswordChar = true;
            }
            else
            {
                RS9.Text = "Отключена";
                RS9.ForeColor = Color.Fuchsia;
                RP9.UseSystemPasswordChar = false;
            }
        }

        private void RS10_CheckedChanged(object sender, EventArgs e)
        {
            if (RS10.CheckState == CheckState.Checked)
            {
                RS10.Text = "Включена";
                RS10.ForeColor = Color.LimeGreen;
                RP10.UseSystemPasswordChar = true;
            }
            else
            {
                RS10.Text = "Отключена";
                RS10.ForeColor = Color.Fuchsia;
                RP10.UseSystemPasswordChar = false;
            }
        }

        private void RS11_CheckedChanged(object sender, EventArgs e)
        {
            if (RS11.CheckState == CheckState.Checked)
            {
                RS11.Text = "Включена";
                RS11.ForeColor = Color.LimeGreen;
                RP11.UseSystemPasswordChar = true;
            }
            else
            {
                RS11.Text = "Отключена";
                RS11.ForeColor = Color.Fuchsia;
                RP11.UseSystemPasswordChar = false;
            }
        }

        private void RS12_CheckedChanged(object sender, EventArgs e)
        {
            if (RS12.CheckState == CheckState.Checked)
            {
                RS12.Text = "Включена";
                RS12.ForeColor = Color.LimeGreen;
                RP12.UseSystemPasswordChar = true;
            }
            else
            {
                RS12.Text = "Отключена";
                RS12.ForeColor = Color.Fuchsia;
                RP12.UseSystemPasswordChar = false;
            }
        }

        private void RS13_CheckedChanged(object sender, EventArgs e)
        {
            if (RS13.CheckState == CheckState.Checked)
            {
                RS13.Text = "Включена";
                RS13.ForeColor = Color.LimeGreen;
                RP13.UseSystemPasswordChar = true;
            }
            else
            {
                RS13.Text = "Отключена";
                RS13.ForeColor = Color.Fuchsia;
                RP13.UseSystemPasswordChar = false;
            }
        }

        private void RS14_CheckedChanged(object sender, EventArgs e)
        {
            if (RS14.CheckState == CheckState.Checked)
            {
                RS14.Text = "Включена";
                RS14.ForeColor = Color.LimeGreen;
                RP14.UseSystemPasswordChar = true;
            }
            else
            {
                RS14.Text = "Отключена";
                RS14.ForeColor = Color.Fuchsia;
                RP14.UseSystemPasswordChar = false;
            }
        }

        private void RS15_CheckedChanged(object sender, EventArgs e)
        {
            if (RS15.CheckState == CheckState.Checked)
            {
                RS15.Text = "Включена";
                RS15.ForeColor = Color.LimeGreen;
                RP15.UseSystemPasswordChar = true;
            }
            else
            {
                RS15.Text = "Отключена";
                RS15.ForeColor = Color.Fuchsia;
                RP15.UseSystemPasswordChar = false;
            }
        }

        private void RS16_CheckedChanged(object sender, EventArgs e)
        {
            if (RS16.CheckState == CheckState.Checked)
            {
                RS16.Text = "Включена";
                RS16.ForeColor = Color.LimeGreen;
                RP16.UseSystemPasswordChar = true;
            }
            else
            {
                RS16.Text = "Отключена";
                RS16.ForeColor = Color.Fuchsia;
                RP16.UseSystemPasswordChar = false;
            }
        }

        private void RS17_CheckedChanged(object sender, EventArgs e)
        {
            if (RS17.CheckState == CheckState.Checked)
            {
                RS17.Text = "Включена";
                RS17.ForeColor = Color.LimeGreen;
                RP17.UseSystemPasswordChar = true;
            }
            else
            {
                RS17.Text = "Отключена";
                RS17.ForeColor = Color.Fuchsia;
                RP17.UseSystemPasswordChar = false;
            }
        }

        private void RS18_CheckedChanged(object sender, EventArgs e)
        {
            if (RS18.CheckState == CheckState.Checked)
            {
                RS18.Text = "Включена";
                RS18.ForeColor = Color.LimeGreen;
                RP18.UseSystemPasswordChar = true;
            }
            else
            {
                RS18.Text = "Отключена";
                RS18.ForeColor = Color.Fuchsia;
                RP18.UseSystemPasswordChar = false;
            }
        }

        private void RS19_CheckedChanged(object sender, EventArgs e)
        {
            if (RS19.CheckState == CheckState.Checked)
            {
                RS19.Text = "Включена";
                RS19.ForeColor = Color.LimeGreen;
                RP19.UseSystemPasswordChar = true;
            }
            else
            {
                RS19.Text = "Отключена";
                RS19.ForeColor = Color.Fuchsia;
                RP19.UseSystemPasswordChar = false;
            }
        }

        private void RS20_CheckedChanged(object sender, EventArgs e)
        {
            if (RS20.CheckState == CheckState.Checked)
            {
                RS20.Text = "Включена";
                RS20.ForeColor = Color.LimeGreen;
                RP20.UseSystemPasswordChar = true;
            }
            else
            {
                RS20.Text = "Отключена";
                RS20.ForeColor = Color.Fuchsia;
                RP20.UseSystemPasswordChar = false;
            }
        }

        // Vote

        int Byte_2 = 0;
        int Byte_4 = 0;
        int Byte_8 = 0;
        int Byte_16 = 0;
        int Byte_32 = 0;
        int Byte_64 = 0;
        int Byte_128 = 0;
        int ByteResult = 0;

        private void VoteByte2_CheckedChanged(object sender, EventArgs e)
        {
            if (VoteByte2.CheckState == CheckState.Checked)
            {
                Byte_2 = 2;
                VoteByte2.BackColor = Color.Green;
            }
            else
            {
                Byte_2 = 0;
                VoteByte2.BackColor = Color.Orange;
            }
            ByteResult = Byte_2 + Byte_4 + Byte_8 + Byte_16 + Byte_32 + Byte_64 + Byte_128;
        }

        private void VoteByte4_CheckedChanged(object sender, EventArgs e)
        {
            if (VoteByte4.CheckState == CheckState.Checked)
            {
                Byte_4 = 4;
                VoteByte4.BackColor = Color.Green;
            }
            else
            {
                Byte_4 = 0;
                VoteByte4.BackColor = Color.Orange;
            }
            ByteResult = Byte_2 + Byte_4 + Byte_8 + Byte_16 + Byte_32 + Byte_64 + Byte_128;
        }

        private void VoteByte8_CheckedChanged(object sender, EventArgs e)
        {
            if (VoteByte8.CheckState == CheckState.Checked)
            {
                Byte_8 = 8;
                VoteByte8.BackColor = Color.Green;
            }
            else
            {
                Byte_8 = 0;
                VoteByte8.BackColor = Color.Orange;
            }
            ByteResult = Byte_2 + Byte_4 + Byte_8 + Byte_16 + Byte_32 + Byte_64 + Byte_128;
        }

        private void VoteByte16_CheckedChanged(object sender, EventArgs e)
        {
            if (VoteByte16.CheckState == CheckState.Checked)
            {
                Byte_16 = 16;
                VoteByte16.BackColor = Color.Green;
            }
            else
            {
                Byte_16 = 0;
                VoteByte16.BackColor = Color.Orange;
            }
            ByteResult = Byte_2 + Byte_4 + Byte_8 + Byte_16 + Byte_32 + Byte_64 + Byte_128;
        }

        private void VoteByte32_CheckedChanged(object sender, EventArgs e)
        {
            if (VoteByte32.CheckState == CheckState.Checked)
            {
                Byte_32 = 32;
                VoteByte32.BackColor = Color.Green;
            }
            else
            {
                Byte_32 = 0;
                VoteByte32.BackColor = Color.Orange;
            }
            ByteResult = Byte_2 + Byte_4 + Byte_8 + Byte_16 + Byte_32 + Byte_64 + Byte_128;
        }

        private void VoteByte64_CheckedChanged(object sender, EventArgs e)
        {
            if (VoteByte64.CheckState == CheckState.Checked)
            {
                Byte_64 = 64;
                VoteByte64.BackColor = Color.Green;
            }
            else
            {
                Byte_64 = 0;
                VoteByte64.BackColor = Color.Orange;
            }
            ByteResult = Byte_2 + Byte_4 + Byte_8 + Byte_16 + Byte_32 + Byte_64 + Byte_128;
        }

        private void VoteByte128_CheckedChanged(object sender, EventArgs e)
        {
            if (VoteByte128.CheckState == CheckState.Checked)
            {
                Byte_128 = 128;
                VoteByte128.BackColor = Color.Green;
            }
            else
            {
                Byte_128 = 0;
                VoteByte128.BackColor = Color.Orange;
            }
            ByteResult = Byte_2 + Byte_4 + Byte_8 + Byte_16 + Byte_32 + Byte_64 + Byte_128;
        }
        private void VoteByte0_CheckedChanged(object sender, EventArgs e)
        {
            if (VoteByte0.CheckState == CheckState.Checked)
            {
                VoteByte2.Checked = false;
                VoteByte4.Checked = false;
                VoteByte8.Checked = false;
                VoteByte16.Checked = false;
                VoteByte32.Checked = false;
                VoteByte64.Checked = false;
                VoteByte128.Checked = false;
                VoteByte2.Enabled = false;
                VoteByte4.Enabled = false;
                VoteByte8.Enabled = false;
                VoteByte16.Enabled = false;
                VoteByte32.Enabled = false;
                VoteByte64.Enabled = false;
                VoteByte128.Enabled = false;
                VoteByte0.BackColor = Color.Green;
            }
            else
            {
                VoteByte2.Enabled = true;
                VoteByte4.Enabled = true;
                VoteByte8.Enabled = true;
                VoteByte16.Enabled = true;
                VoteByte32.Enabled = true;
                VoteByte64.Enabled = true;
                VoteByte128.Enabled = true;
                VoteByte0.BackColor = Color.DarkOrange;
            }
        }

        private void VoteOK_CheckedChanged(object sender, EventArgs e)
        {
            if (VoteOK.CheckState == CheckState.Checked)
            {
                VoteOK.BackColor = Color.Green;
            }
            else
            {
                VoteOK.BackColor = Color.DarkOrange;
            }
        }


        private void Initialize_server_settings()
        {
            cmd_sv1.Text = server_settings.cmd_sv1;
            cmd_sv2.Text = server_settings.cmd_sv2;
            cmd_sv3.Text = server_settings.cmd_sv3;
            cmd_sv4.Text = server_settings.cmd_sv4;
            cmd_sv5.Text = server_settings.cmd_sv5;
            cmd_sv6.Text = server_settings.cmd_sv6;
            cmd_sv7.Text = server_settings.cmd_sv7;
            cmd_sv8.Text = server_settings.cmd_sv8;
            cmd_sv9.Text = server_settings.cmd_sv9;
            cmd_sv10.Text = server_settings.cmd_sv10;
            cmd_sv11.Text = server_settings.cmd_sv11;
            cmd_sv12.Text = server_settings.cmd_sv12;
            cmd_sv13.Text = server_settings.cmd_sv13;
            cmd_sv14.Text = server_settings.cmd_sv14;
            cmd_sv15.Text = server_settings.cmd_sv15;
            cmd_sv16.Text = server_settings.cmd_sv16;
            cmd_sv17.Text = server_settings.cmd_sv17;
            cmd_sv18.Text = server_settings.cmd_sv18;
            cmd_sv19.Text = server_settings.cmd_sv19;
            cmd_sv20.Text = server_settings.cmd_sv20;
            cmd_sv21.Text = server_settings.cmd_sv21;
            cmd_sv22.Text = server_settings.cmd_sv22;
            cmd_sv23.Text = server_settings.cmd_sv23;
            cmd_sv24.Text = server_settings.cmd_sv24;
            cmd_sv25.Text = server_settings.cmd_sv25;
            cmd_sv26.Text = server_settings.cmd_sv26;
            cmd_sv27.Text = server_settings.cmd_sv27;
            cmd_sv28.Text = server_settings.cmd_sv28;
            cmd_sv29.Text = server_settings.cmd_sv29;
            cmd_sv30.Text = server_settings.cmd_sv30;
            cmd_sv31.Text = server_settings.cmd_sv31;
            cmd_sv32.Text = server_settings.cmd_sv32;
            cmd_sv33.Text = server_settings.cmd_sv33;
            cmd_sv34.Text = server_settings.cmd_sv34;
            cmd_sv35.Text = server_settings.cmd_sv35;
            cmd_sv36.Text = server_settings.cmd_sv36;
            cmd_sv37.Text = server_settings.cmd_sv37;
            cmd_sv38.Text = server_settings.cmd_sv38;
            cmd_sv39.Text = server_settings.cmd_sv39;
            cmd_sv40.Text = server_settings.cmd_sv40;
            cmd_sv41.Text = server_settings.cmd_sv41;
            cmd_sv42.Text = server_settings.cmd_sv42;
            // radmin
            RL1.Text = server_settings.RL1;
            RP1.Text = server_settings.RP1;
            RS1.Checked = server_settings.RS1;
            RL2.Text = server_settings.RL2;
            RP2.Text = server_settings.RP2;
            RS2.Checked = server_settings.RS2;
            RL3.Text = server_settings.RL3;
            RP3.Text = server_settings.RP3;
            RS3.Checked = server_settings.RS3;
            RL4.Text = server_settings.RL4;
            RP4.Text = server_settings.RP4;
            RS4.Checked = server_settings.RS4;
            RL5.Text = server_settings.RL5;
            RP5.Text = server_settings.RP5;
            RS5.Checked = server_settings.RS5;
            RL6.Text = server_settings.RL6;
            RP6.Text = server_settings.RP6;
            RS6.Checked = server_settings.RS6;
            RL7.Text = server_settings.RL7;
            RP7.Text = server_settings.RP7;
            RS7.Checked = server_settings.RS7;
            RL8.Text = server_settings.RL8;
            RP8.Text = server_settings.RP8;
            RS8.Checked = server_settings.RS8;
            RL9.Text = server_settings.RL9;
            RP9.Text = server_settings.RP9;
            RS9.Checked = server_settings.RS9;
            RL10.Text = server_settings.RL10;
            RP10.Text = server_settings.RP10;
            RS10.Checked = server_settings.RS10;
            RL11.Text = server_settings.RL11;
            RP11.Text = server_settings.RP11;
            RS11.Checked = server_settings.RS11;
            RL12.Text = server_settings.RL12;
            RP12.Text = server_settings.RP12;
            RS12.Checked = server_settings.RS12;
            RL13.Text = server_settings.RL13;
            RP13.Text = server_settings.RP13;
            RS13.Checked = server_settings.RS13;
            RL14.Text = server_settings.RL14;
            RP14.Text = server_settings.RP14;
            RS14.Checked = server_settings.RS14;
            RL15.Text = server_settings.RL15;
            RP15.Text = server_settings.RP15;
            RS15.Checked = server_settings.RS15;
            RL16.Text = server_settings.RL16;
            RP16.Text = server_settings.RP16;
            RS16.Checked = server_settings.RS16;
            RL17.Text = server_settings.RL17;
            RP17.Text = server_settings.RP17;
            RS17.Checked = server_settings.RS17;
            RL18.Text = server_settings.RL18;
            RP18.Text = server_settings.RP18;
            RS18.Checked = server_settings.RS18;
            RL19.Text = server_settings.RL19;
            RP19.Text = server_settings.RP19;
            RS19.Checked = server_settings.RS19;
            RL20.Text = server_settings.RL20;
            RP20.Text = server_settings.RP20;
            RS20.Checked = server_settings.RS20;
            // weapons
            settings_asm1.Checked = server_settings.settings_asm1;
            settings_asm2.Checked = server_settings.settings_asm2;
            settings_asm3.Checked = server_settings.settings_asm3;
            settings_asm4.Checked = server_settings.settings_asm4;
            settings_asm5.Checked = server_settings.settings_asm5;
            settings_asm6.Checked = server_settings.settings_asm6;
            settings_asm7.Checked = server_settings.settings_asm7;
            settings_asm8.Checked = server_settings.settings_asm8;
            settings_asm9.Checked = server_settings.settings_asm9;
            settings_asm10.Checked = server_settings.settings_asm10;
            settings_asm11.Checked = server_settings.settings_asm11;
            settings_asm12.Checked = server_settings.settings_asm12;
            settings_asm13.Checked = server_settings.settings_asm13;
            settings_asm14.Checked = server_settings.settings_asm14;
            settings_asm15.Checked = server_settings.settings_asm15;
            settings_asm16.Checked = server_settings.settings_asm16;
            settings_asm17.Checked = server_settings.settings_asm17;
            settings_asm18.Checked = server_settings.settings_asm18;
            settings_asm19.Checked = server_settings.settings_asm19;
            settings_asm20.Checked = server_settings.settings_asm20;
            settings_asm21.Checked = server_settings.settings_asm21;
            settings_asm22.Checked = server_settings.settings_asm22;
            settings_asm23.Checked = server_settings.settings_asm23;
            settings_asm24.Checked = server_settings.settings_asm24;
            settings_asm25.Checked = server_settings.settings_asm25;
            settings_asm26.Checked = server_settings.settings_asm26;
            settings_asm27.Checked = server_settings.settings_asm27;
            settings_asm28.Checked = server_settings.settings_asm28;
            settings_asm29.Checked = server_settings.settings_asm29;
            settings_asm30.Checked = server_settings.settings_asm30;
            settings_asm31.Checked = server_settings.settings_asm31;
            settings_asm32.Checked = server_settings.settings_asm32;
            settings_asm33.Checked = server_settings.settings_asm33;
            settings_asm34.Checked = server_settings.settings_asm34;
            settings_asm35.Checked = server_settings.settings_asm35;
            settings_asm36.Checked = server_settings.settings_asm36;
            settings_asm37.Checked = server_settings.settings_asm37;
            settings_asm38.Checked = server_settings.settings_asm38;
            settings_asm39.Checked = server_settings.settings_asm39;
            settings_asm40.Checked = server_settings.settings_asm40;
            settings_asm41.Checked = server_settings.settings_asm41;
            settings_asm42.Checked = server_settings.settings_asm42;
            settings_asm43.Checked = server_settings.settings_asm43;
            settings_asm44.Checked = server_settings.settings_asm44;
            settings_asm45.Checked = server_settings.settings_asm45;
            settings_asm46.Checked = server_settings.settings_asm46;
            settings_asm47.Checked = server_settings.settings_asm47;
            settings_asm48.Checked = server_settings.settings_asm48;
            // rules asm
            asm_message.Text = server_settings.settings_rules; 
            // Vote
            VoteByte2.Checked = server_settings.VoteByte2;
            VoteByte4.Checked = server_settings.VoteByte4;
            VoteByte8.Checked = server_settings.VoteByte8;
            VoteByte16.Checked = server_settings.VoteByte16;
            VoteByte32.Checked = server_settings.VoteByte32;
            VoteByte64.Checked = server_settings.VoteByte64;
            VoteByte128.Checked = server_settings.VoteByte128;
            VoteOK.Checked = server_settings.VoteOK;
            VoteQuota.Text = server_settings.VoteQuota;
            VoteTime.Text = server_settings.VoteTime;
            // AutoChangeLevel
            svmaps1.Checked = server_settings.Level1;
            svmaps2.Checked = server_settings.Level2;
            svmaps3.Checked = server_settings.Level3;
            svmaps4.Checked = server_settings.Level4;
            svmaps5.Checked = server_settings.Level5;
            svmaps6.Checked = server_settings.Level6;
            svmaps7.Checked = server_settings.Level7;
            svmaps8.Checked = server_settings.Level8;
            svmaps9.Checked = server_settings.Level9;
            svmaps10.Checked = server_settings.Level10;
            svmaps11.Checked = server_settings.Level11;
            svmaps12.Checked = server_settings.Level12;
            svmaps13.Checked = server_settings.Level13;
            svmaps14.Checked = server_settings.Level14;
            svmaps15.Checked = server_settings.Level15;
            svmaps16.Checked = server_settings.Level16;
            svmaps17.Checked = server_settings.Level17;
            svmaps18.Checked = server_settings.Level18;
            svmaps19.Checked = server_settings.Level19;
            svmaps20.Checked = server_settings.Level20;
            svmaps21.Checked = server_settings.Level21;
            svmaps22.Checked = server_settings.Level22;
            svmaps23.Checked = server_settings.Level23;
            svmaps24.Checked = server_settings.Level24;
            svmaps25.Checked = server_settings.Level25;
            svmaps26.Checked = server_settings.Level26;
            svmaps27.Checked = server_settings.Level27;
            svmaps28.Checked = server_settings.Level28;
            svmaps29.Checked = server_settings.Level29;
            // TSMP
            RadioCreateCmd.Checked = server_settings.RadioCreateCmd;
            RadioBlockTime.Text = server_settings.RadioBlockTime;
            RadioBlockMSGLimit.Text = server_settings.RadioBlockMSGLimit;
            // Editor
            ArtefactType.Text = server_settings.ArtefactType;
            ShopBlueTeam.Text = server_settings.ShopBlueTeam;
            ShopGreenTeam.Text = server_settings.ShopGreenTeam;
            my_shop1.Text = server_settings.my_shop1;
            my_shop2.Text = server_settings.my_shop2;
            my_shop3.Text = server_settings.my_shop3;
            my_shop4.Text = server_settings.my_shop4;
            my_shop5.Text = server_settings.my_shop5;
            my_shop6.Text = server_settings.my_shop6;
            my_shop7.Text = server_settings.my_shop7;
            my_shop8.Text = server_settings.my_shop8;
            my_shop9.Text = server_settings.my_shop9;
            my_shop10.Text = server_settings.my_shop10;
            my_shop11.Text = server_settings.my_shop11;
            my_shop12.Text = server_settings.my_shop12;
            my_shop13.Text = server_settings.my_shop13;
            my_shop14.Text = server_settings.my_shop14;
            my_shop15.Text = server_settings.my_shop15;
            my_shop16.Text = server_settings.my_shop16;
            checkEnableEditor.Checked = server_settings.checkEnableEditor;
            // Editor DM
            dm1.Text = server_settings.my_shop17;
            dm2.Text = server_settings.my_shop18;
            dm3.Text = server_settings.my_shop19;
            dm4.Text = server_settings.my_shop20;
            dm5.Text = server_settings.my_shop21;
            dm6.Text = server_settings.my_shop22;
            dm7.Text = server_settings.my_shop23;
            dm8.Text = server_settings.my_shop24;
            dm9.Text = server_settings.my_shop25;
            // Menu 2
            wn1.Text = server_settings.my_shop26;
            wn2.Text = server_settings.my_shop27;
            wn3.Text = server_settings.my_shop28;
            wn4.Text = server_settings.my_shop29;
            wn5.Text = server_settings.my_shop30;
            wn6.Text = server_settings.my_shop31;
            wn7.Text = server_settings.my_shop32;
            wn8.Text = server_settings.my_shop33;
            wn9.Text = server_settings.my_shop34;
            wn10.Text = server_settings.my_shop35;
            wn11.Text = server_settings.my_shop36;
            wn12.Text = server_settings.my_shop37;
            wn13.Text = server_settings.my_shop38;
            wn14.Text = server_settings.my_shop39;
            wn15.Text = server_settings.my_shop40;
            wn16.Text = server_settings.my_shop41;
            wn17.Text = server_settings.my_shop42;
            wn18.Text = server_settings.my_shop43;
            wn19.Text = server_settings.my_shop44;
            wn20.Text = server_settings.my_shop45;
            wn21.Text = server_settings.my_shop46;
            wn22.Text = server_settings.my_shop47;
            wn23.Text = server_settings.my_shop48;
            wn24.Text = server_settings.my_shop49;
            wn25.Text = server_settings.my_shop50;
            // Spawn Artefact
            ArtSpawnAnomalySet.Text = server_settings.my_shop51;
        }
        private void Save_server_settings()
        {
            server_settings.cmd_sv1 = cmd_sv1.Text;
            server_settings.cmd_sv2 = cmd_sv2.Text;
            server_settings.cmd_sv3 = cmd_sv3.Text;
            server_settings.cmd_sv4 = cmd_sv4.Text;
            server_settings.cmd_sv5 = cmd_sv5.Text;
            server_settings.cmd_sv6 = cmd_sv6.Text;
            server_settings.cmd_sv7 = cmd_sv7.Text;
            server_settings.cmd_sv8 = cmd_sv8.Text;
            server_settings.cmd_sv9 = cmd_sv9.Text;
            server_settings.cmd_sv10 = cmd_sv10.Text;
            server_settings.cmd_sv11 = cmd_sv11.Text;
            server_settings.cmd_sv12 = cmd_sv12.Text;
            server_settings.cmd_sv13 = cmd_sv13.Text;
            server_settings.cmd_sv14 = cmd_sv14.Text;
            server_settings.cmd_sv15 = cmd_sv15.Text;
            server_settings.cmd_sv16 = cmd_sv16.Text;
            server_settings.cmd_sv17 = cmd_sv17.Text;
            server_settings.cmd_sv18 = cmd_sv18.Text;
            server_settings.cmd_sv19 = cmd_sv19.Text;
            server_settings.cmd_sv20 = cmd_sv20.Text;
            server_settings.cmd_sv21 = cmd_sv21.Text;
            server_settings.cmd_sv22 = cmd_sv22.Text;
            server_settings.cmd_sv23 = cmd_sv23.Text;
            server_settings.cmd_sv24 = cmd_sv24.Text;
            server_settings.cmd_sv25 = cmd_sv25.Text;
            server_settings.cmd_sv26 = cmd_sv26.Text;
            server_settings.cmd_sv27 = cmd_sv27.Text;
            server_settings.cmd_sv28 = cmd_sv28.Text;
            server_settings.cmd_sv29 = cmd_sv29.Text;
            server_settings.cmd_sv30 = cmd_sv30.Text;
            server_settings.cmd_sv31 = cmd_sv31.Text;
            server_settings.cmd_sv32 = cmd_sv32.Text;
            server_settings.cmd_sv33 = cmd_sv33.Text;
            server_settings.cmd_sv34 = cmd_sv34.Text;
            server_settings.cmd_sv35 = cmd_sv35.Text;
            server_settings.cmd_sv36 = cmd_sv36.Text;
            server_settings.cmd_sv37 = cmd_sv37.Text;
            server_settings.cmd_sv38 = cmd_sv38.Text;
            server_settings.cmd_sv39 = cmd_sv39.Text;
            server_settings.cmd_sv40 = cmd_sv40.Text;
            server_settings.cmd_sv41 = cmd_sv41.Text;
            server_settings.cmd_sv42 = cmd_sv42.Text;
            // radmin
            server_settings.RL1 = RL1.Text;
            server_settings.RP1 = RP1.Text;
            server_settings.RS1 = RS1.Checked;
            server_settings.RL2 = RL2.Text;
            server_settings.RP2 = RP2.Text;
            server_settings.RS2 = RS2.Checked;
            server_settings.RL3 = RL3.Text;
            server_settings.RP3 = RP3.Text;
            server_settings.RS3 = RS3.Checked;
            server_settings.RL4 = RL4.Text;
            server_settings.RP4 = RP4.Text;
            server_settings.RS4 = RS4.Checked;
            server_settings.RL5 = RL5.Text;
            server_settings.RP5 = RP5.Text;
            server_settings.RS5 = RS5.Checked;
            server_settings.RL6 = RL6.Text;
            server_settings.RP6 = RP6.Text;
            server_settings.RS6 = RS6.Checked;
            server_settings.RL7 = RL7.Text;
            server_settings.RP7 = RP7.Text;
            server_settings.RS7 = RS7.Checked;
            server_settings.RL8 = RL8.Text;
            server_settings.RP8 = RP8.Text;
            server_settings.RS8 = RS8.Checked;
            server_settings.RL9 = RL9.Text;
            server_settings.RP9 = RP9.Text;
            server_settings.RS9 = RS9.Checked;
            server_settings.RL10 = RL10.Text;
            server_settings.RP10 = RP10.Text;
            server_settings.RS10 = RS10.Checked;
            server_settings.RL11 = RL11.Text;
            server_settings.RP11 = RP11.Text;
            server_settings.RS11 = RS11.Checked;
            server_settings.RL12 = RL12.Text;
            server_settings.RP12 = RP12.Text;
            server_settings.RS12 = RS12.Checked;
            server_settings.RL13 = RL13.Text;
            server_settings.RP13 = RP13.Text;
            server_settings.RS13 = RS13.Checked;
            server_settings.RL14 = RL14.Text;
            server_settings.RP14 = RP14.Text;
            server_settings.RS14 = RS14.Checked;
            server_settings.RL15 = RL15.Text;
            server_settings.RP15 = RP15.Text;
            server_settings.RS15 = RS15.Checked;
            server_settings.RL16 = RL16.Text;
            server_settings.RP16 = RP16.Text;
            server_settings.RS16 = RS16.Checked;
            server_settings.RL17 = RL17.Text;
            server_settings.RP17 = RP17.Text;
            server_settings.RS17 = RS17.Checked;
            server_settings.RL18 = RL18.Text;
            server_settings.RP18 = RP18.Text;
            server_settings.RS18 = RS18.Checked;
            server_settings.RL19 = RL19.Text;
            server_settings.RP19 = RP19.Text;
            server_settings.RS19 = RS19.Checked;
            server_settings.RL20 = RL20.Text;
            server_settings.RP20 = RP20.Text;
            server_settings.RS20 = RS20.Checked;
            // weapons
            server_settings.settings_asm1 = settings_asm1.Checked;
            server_settings.settings_asm2 = settings_asm2.Checked;
            server_settings.settings_asm3 = settings_asm3.Checked;
            server_settings.settings_asm4 = settings_asm4.Checked;
            server_settings.settings_asm5 = settings_asm5.Checked;
            server_settings.settings_asm6 = settings_asm6.Checked;
            server_settings.settings_asm7 = settings_asm7.Checked;
            server_settings.settings_asm8 = settings_asm8.Checked;
            server_settings.settings_asm9 = settings_asm9.Checked;
            server_settings.settings_asm10 = settings_asm10.Checked;
            server_settings.settings_asm11 = settings_asm11.Checked;
            server_settings.settings_asm12 = settings_asm12.Checked;
            server_settings.settings_asm13 = settings_asm13.Checked;
            server_settings.settings_asm14 = settings_asm14.Checked;
            server_settings.settings_asm15 = settings_asm15.Checked;
            server_settings.settings_asm16 = settings_asm16.Checked;
            server_settings.settings_asm17 = settings_asm17.Checked;
            server_settings.settings_asm18 = settings_asm18.Checked;
            server_settings.settings_asm19 = settings_asm19.Checked;
            server_settings.settings_asm20 = settings_asm20.Checked;
            server_settings.settings_asm21 = settings_asm21.Checked;
            server_settings.settings_asm22 = settings_asm22.Checked;
            server_settings.settings_asm23 = settings_asm23.Checked;
            server_settings.settings_asm24 = settings_asm24.Checked;
            server_settings.settings_asm25 = settings_asm25.Checked;
            server_settings.settings_asm26 = settings_asm26.Checked;
            server_settings.settings_asm27 = settings_asm27.Checked;
            server_settings.settings_asm28 = settings_asm28.Checked;
            server_settings.settings_asm29 = settings_asm29.Checked;
            server_settings.settings_asm30 = settings_asm30.Checked;
            server_settings.settings_asm31 = settings_asm31.Checked;
            server_settings.settings_asm32 = settings_asm32.Checked;
            server_settings.settings_asm33 = settings_asm33.Checked;
            server_settings.settings_asm34 = settings_asm34.Checked;
            server_settings.settings_asm35 = settings_asm35.Checked;
            server_settings.settings_asm36 = settings_asm36.Checked;
            server_settings.settings_asm37 = settings_asm37.Checked;
            server_settings.settings_asm38 = settings_asm38.Checked;
            server_settings.settings_asm39 = settings_asm39.Checked;
            server_settings.settings_asm40 = settings_asm40.Checked;
            server_settings.settings_asm41 = settings_asm41.Checked;
            server_settings.settings_asm42 = settings_asm42.Checked;
            server_settings.settings_asm43 = settings_asm43.Checked;
            server_settings.settings_asm44 = settings_asm44.Checked;
            server_settings.settings_asm45 = settings_asm45.Checked;
            // group weapons
            server_settings.settings_asm46 = settings_asm46.Checked;
            server_settings.settings_asm47 = settings_asm47.Checked;
            server_settings.settings_asm48 = settings_asm48.Checked;
            // rules
            server_settings.settings_rules = asm_message.Text; 
            // Vote
            server_settings.VoteByte2 = VoteByte2.Checked;
            server_settings.VoteByte4 = VoteByte4.Checked;
            server_settings.VoteByte8 = VoteByte8.Checked;
            server_settings.VoteByte16 = VoteByte16.Checked;
            server_settings.VoteByte32 = VoteByte32.Checked;
            server_settings.VoteByte64 = VoteByte64.Checked;
            server_settings.VoteByte128 = VoteByte128.Checked;
            server_settings.VoteOK = VoteOK.Checked;
            server_settings.VoteQuota = VoteQuota.Text;
            server_settings.VoteTime = VoteTime.Text;
            // AutoChangeLevel
            server_settings.Level1 = svmaps1.Checked;
            server_settings.Level2 = svmaps2.Checked;
            server_settings.Level3 = svmaps3.Checked;
            server_settings.Level4 = svmaps4.Checked;
            server_settings.Level5 = svmaps5.Checked;
            server_settings.Level6 = svmaps6.Checked;
            server_settings.Level7 = svmaps7.Checked;
            server_settings.Level8 = svmaps8.Checked;
            server_settings.Level9 = svmaps9.Checked;
            server_settings.Level10 = svmaps10.Checked;
            server_settings.Level11 = svmaps11.Checked;
            server_settings.Level12 = svmaps12.Checked;
            server_settings.Level13 = svmaps13.Checked;
            server_settings.Level14 = svmaps14.Checked;
            server_settings.Level15 = svmaps15.Checked;
            server_settings.Level16 = svmaps16.Checked;
            server_settings.Level17 = svmaps17.Checked;
            server_settings.Level18 = svmaps18.Checked;
            server_settings.Level19 = svmaps19.Checked;
            server_settings.Level20 = svmaps20.Checked;
            server_settings.Level21 = svmaps21.Checked;
            server_settings.Level22 = svmaps22.Checked;
            server_settings.Level23 = svmaps23.Checked;
            server_settings.Level24 = svmaps24.Checked;
            server_settings.Level25 = svmaps25.Checked;
            server_settings.Level26 = svmaps26.Checked;
            server_settings.Level27 = svmaps27.Checked;
            server_settings.Level28 = svmaps28.Checked;
            server_settings.Level29 = svmaps29.Checked;
            // TSMP
            server_settings.RadioCreateCmd = RadioCreateCmd.Checked;
            server_settings.RadioBlockTime = RadioBlockTime.Text;
            server_settings.RadioBlockMSGLimit = RadioBlockMSGLimit.Text;
            // Editor art
            server_settings.ArtefactType = ArtefactType.Text;
            server_settings.ShopBlueTeam = ShopBlueTeam.Text;
            server_settings.ShopGreenTeam = ShopGreenTeam.Text;
            server_settings.my_shop1 = my_shop1.Text;
            server_settings.my_shop2 = my_shop2.Text;
            server_settings.my_shop3 = my_shop3.Text;
            server_settings.my_shop4 = my_shop4.Text;
            server_settings.my_shop5 = my_shop5.Text;
            server_settings.my_shop6 = my_shop6.Text;
            server_settings.my_shop7 = my_shop7.Text;
            server_settings.my_shop8 = my_shop8.Text;
            server_settings.my_shop9 = my_shop9.Text;
            server_settings.my_shop10 = my_shop10.Text;
            server_settings.my_shop11 = my_shop11.Text;
            server_settings.my_shop12 = my_shop12.Text;
            server_settings.my_shop13 = my_shop13.Text;
            server_settings.my_shop14 = my_shop14.Text;
            server_settings.my_shop15 = my_shop15.Text;
            server_settings.my_shop16 = my_shop16.Text;
            server_settings.checkEnableEditor = checkEnableEditor.Checked;
            // Editor DM
            server_settings.my_shop17 = dm1.Text;
            server_settings.my_shop18 = dm2.Text;
            server_settings.my_shop19 = dm3.Text;
            server_settings.my_shop20 = dm4.Text;
            server_settings.my_shop21 = dm5.Text;
            server_settings.my_shop22 = dm6.Text;
            server_settings.my_shop23 = dm7.Text;
            server_settings.my_shop24 = dm8.Text;
            server_settings.my_shop25 = dm9.Text;
            // Menu 2
            server_settings.my_shop26 = wn1.Text;
            server_settings.my_shop27 = wn2.Text;
            server_settings.my_shop28 = wn3.Text;
            server_settings.my_shop29 = wn4.Text;
            server_settings.my_shop30 = wn5.Text;
            server_settings.my_shop31 = wn6.Text;
            server_settings.my_shop32 = wn7.Text;
            server_settings.my_shop33 = wn8.Text;
            server_settings.my_shop34 = wn9.Text;
            server_settings.my_shop35 = wn10.Text;
            server_settings.my_shop36 = wn11.Text;
            server_settings.my_shop37 = wn12.Text;
            server_settings.my_shop38 = wn13.Text;
            server_settings.my_shop39 = wn14.Text;
            server_settings.my_shop40 = wn15.Text;
            server_settings.my_shop41 = wn16.Text;
            server_settings.my_shop42 = wn17.Text;
            server_settings.my_shop43 = wn18.Text;
            server_settings.my_shop44 = wn19.Text;
            server_settings.my_shop45 = wn20.Text;
            server_settings.my_shop46 = wn21.Text;
            server_settings.my_shop47 = wn22.Text;
            server_settings.my_shop48 = wn23.Text;
            server_settings.my_shop49 = wn24.Text;
            server_settings.my_shop50 = wn25.Text;
            // Spawn Artefact Editor
            server_settings.my_shop51 = ArtSpawnAnomalySet.Text;
            server_settings.Save();
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
                    UI3.BackColor = Color.FromArgb(192, 192, 255);
                    btnLoad.BackColor = Color.FromArgb(192, 192, 255);
                    btnLoad.ForeColor = Color.FromArgb(255, 255, 255);
                }
                else
                {
                    GUIEvents.BackColor = color_rechange;
                    btnExit.BackColor = color_rechange;
                    UI3.BackColor = color_rechange;
                    btnLoad.BackColor = color_rechange;
                    btnLoad.ForeColor = color_rechange;
                }
            }
            catch (Exception ex)
            {
                GUIEvents.BackColor = Color.FromArgb(192, 192, 255);
                btnExit.BackColor = Color.FromArgb(192, 192, 255);
                UI3.BackColor = Color.FromArgb(192, 192, 255);
                btnLoad.BackColor = Color.FromArgb(192, 192, 255);
                btnLoad.ForeColor = Color.FromArgb(255, 255, 255);
                MessageBox.Show("Command init set color error\n" + ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /*
        int color = 0;
        private void func_color_ui_server_settings()
        {
            color = Convert.ToInt32(SendMessage.SendMsg);
            if (color == 0)
            {
                GUIEvents.BackColor = Color.FromArgb(192, 192, 255);
                btnExit.BackColor = Color.FromArgb(192, 192, 255);
                UI3.BackColor = Color.FromArgb(192, 192, 255);
                btnLoad.BackColor = Color.FromArgb(192, 192, 255);
                btnLoad.ForeColor = Color.FromArgb(255, 255, 255);
            }
            if (color == 2)
            {
                GUIEvents.BackColor = Color.Coral;
                btnExit.BackColor = Color.Coral;
                UI3.BackColor = Color.Coral;
                btnLoad.BackColor = Color.Coral;
                btnLoad.ForeColor = Color.White;
            }
            if (color == 4)
            {
                GUIEvents.BackColor = Color.LimeGreen;
                btnExit.BackColor = Color.LimeGreen;
                UI3.BackColor = Color.PaleGreen;
                btnLoad.BackColor = Color.PaleGreen;
                btnLoad.ForeColor = Color.Green;
            }
            if (color == 6)
            {
                GUIEvents.BackColor = Color.Gold;
                btnExit.BackColor = Color.Gold;
                UI3.BackColor = Color.Gold;
                btnLoad.BackColor = Color.Gold;
                btnLoad.ForeColor = Color.Black;
            }
            if (color == 8)
            {
                GUIEvents.BackColor = Color.DodgerBlue;
                btnExit.BackColor = Color.DodgerBlue;
                UI3.BackColor = Color.DodgerBlue;
                btnLoad.BackColor = Color.DodgerBlue;
                btnLoad.ForeColor = Color.White;
            }
            if (color == 10)
            {
                GUIEvents.BackColor = Color.Violet;
                btnExit.BackColor = Color.Violet;
                UI3.BackColor = Color.Violet;
                btnLoad.BackColor = Color.Violet;
                btnLoad.ForeColor = Color.White;
            }
            if (color == 12)
            {
                GUIEvents.BackColor = Color.PaleTurquoise;
                btnExit.BackColor = Color.PaleTurquoise;
                UI3.BackColor = Color.PaleTurquoise;
                btnLoad.BackColor = Color.PaleTurquoise;
                btnLoad.ForeColor = Color.Gray;
            }
            if (color == 14)
            {
                GUIEvents.BackColor = Color.White;
                btnExit.BackColor = Color.White;
                UI3.BackColor = Color.White;
                btnLoad.BackColor = Color.White;
                btnLoad.ForeColor = Color.Black;
            }
        }
        */
        private void RadioCreateCmd_CheckedChanged(object sender, EventArgs e)
        {
            if (RadioCreateCmd.CheckState == CheckState.Checked)
            {
                RadioBlockMSGLimit.Enabled = true;
                RadioBlockTime.Enabled = true;
            }
            else
            {
                RadioBlockMSGLimit.Enabled = false;
                RadioBlockTime.Enabled = false;
            }
        }

        private void btnCreateListRotationMaps_Click(object sender, EventArgs e)
        {
            if (File.Exists(@"server_settings\maprot_list.ltx"))
            {
                try
                {
                    File.WriteAllText(@"server_settings\maprot_list.ltx", string.Empty);
                }
                catch (Exception)
                {
                   // MessageBox.Show("Произошла ошибка:\n" + ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Error);
                   // return;
                }
            }
            if (svmaps1.CheckState == CheckState.Checked)
            {
                MapsRotation("sv_addmap l01_escape" + Environment.NewLine);
            }          
            if (svmaps2.CheckState == CheckState.Checked)
            {
                MapsRotation("sv_addmap l02_escape" + Environment.NewLine);
            }
            if (svmaps3.CheckState == CheckState.Checked)
            {
                MapsRotation("sv_addmap l03_agroprom" + Environment.NewLine);
            }
            if (svmaps4.CheckState == CheckState.Checked)
            {
                MapsRotation("sv_addmap l03u_agr_underground" + Environment.NewLine);
            }
            if (svmaps5.CheckState == CheckState.Checked)
            {
                MapsRotation("sv_addmap l04_darkvalley" + Environment.NewLine);
            }
            if (svmaps6.CheckState == CheckState.Checked)
            {
                MapsRotation("sv_addmap l04u_labx18" + Environment.NewLine);
            }
            if (svmaps7.CheckState == CheckState.Checked)
            {
                MapsRotation("sv_addmap l05_bar" + Environment.NewLine);
            }
            if (svmaps8.CheckState == CheckState.Checked)
            {
                MapsRotation("sv_addmap l06_rostok" + Environment.NewLine);
            }
            if (svmaps9.CheckState == CheckState.Checked)
            {
                MapsRotation("sv_addmap l07_military" + Environment.NewLine);
            }
            if (svmaps10.CheckState == CheckState.Checked)
            {
                MapsRotation("sv_addmap l08_yantar" + Environment.NewLine);
            }
            if (svmaps11.CheckState == CheckState.Checked)
            {
                MapsRotation("sv_addmap l08u_brainlab" + Environment.NewLine);
            }
            if (svmaps12.CheckState == CheckState.Checked)
            {
                MapsRotation("sv_addmap l10_radar" + Environment.NewLine);
            }
            if (svmaps13.CheckState == CheckState.Checked)
            {
                MapsRotation("sv_addmap l10u_bunker" + Environment.NewLine);
            }
            if (svmaps14.CheckState == CheckState.Checked)
            {
                MapsRotation("sv_addmap l11_pripyat" + Environment.NewLine);
            }
            if (svmaps15.CheckState == CheckState.Checked)
            {
                MapsRotation("sv_addmap l12_stancia" + Environment.NewLine);
            }
            if (svmaps16.CheckState == CheckState.Checked)
            {
                MapsRotation("sv_addmap l12_stancia_2" + Environment.NewLine);
            }
            if (svmaps17.CheckState == CheckState.Checked)
            {
                MapsRotation("sv_addmap l12u_control_monolith" + Environment.NewLine);
            }
            if (svmaps18.CheckState == CheckState.Checked)
            {
                MapsRotation("sv_addmap l12u_sarcofag" + Environment.NewLine);
            }
            // mp
            if (svmaps19.CheckState == CheckState.Checked)
            {
                MapsRotation("sv_addmap testers_mp_atp" + Environment.NewLine);
            }
            if (svmaps20.CheckState == CheckState.Checked)
            {
                MapsRotation("sv_addmap testers_mp_agroprom" + Environment.NewLine);
            }
            if (svmaps21.CheckState == CheckState.Checked)
            {
                MapsRotation("sv_addmap testers_mp_rostok" + Environment.NewLine);
            }
            if (svmaps22.CheckState == CheckState.Checked)
            {
                MapsRotation("sv_addmap testers_mp_factory" + Environment.NewLine);
            }
            if (svmaps23.CheckState == CheckState.Checked)
            {
                MapsRotation("sv_addmap testers_mp_military_1" + Environment.NewLine);
            }
            if (svmaps24.CheckState == CheckState.Checked)
            {
                MapsRotation("sv_addmap testers_mp_darkvalley" + Environment.NewLine);
            }
            if (svmaps25.CheckState == CheckState.Checked)
            {
                MapsRotation("sv_addmap testers_mp_railroad" + Environment.NewLine);
            }
            if (svmaps26.CheckState == CheckState.Checked)
            {
                MapsRotation("sv_addmap testers_mp_lost_village" + Environment.NewLine);
            }
            if (svmaps27.CheckState == CheckState.Checked)
            {
                MapsRotation("sv_addmap testers_mp_workshop" + Environment.NewLine);
            }
            if (svmaps28.CheckState == CheckState.Checked)
            {
                MapsRotation("sv_addmap testers_mp_pool" + Environment.NewLine);
            }
            if (svmaps29.CheckState == CheckState.Checked)
            {
                MapsRotation("sv_addmap testers_mp_bath" + Environment.NewLine);
            }
            MessageBox.Show("Настройки автосмены карт - были успешно добавлены!", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnDelListRotationMaps_Click(object sender, EventArgs e)
        {
            if (File.Exists(@"server_settings\maprot_list.ltx"))
            {
                try
                {
                    File.Delete(@"server_settings\maprot_list.ltx");
                    MessageBox.Show("Настройки автосмены карт - были успешно удалены!", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка:\n" + ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Настройки автосмены карт - найдены не были.", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ===============================================================================================================================
        //  ARTEFACTHUNT GAMEDATA EDITOR
        // ===============================================================================================================================

        private void checkEnableEditor_CheckedChanged(object sender, EventArgs e)
        {
            if (checkEnableEditor.CheckState == CheckState.Checked)
            {
                btnSaveEditor.Enabled = true;
            }
            else
            {
                btnSaveEditor.Enabled = false;
            }
        }

        private void btnSaveEditor_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(@"gamedata\config\mp\"))
            {
                FileSystemManager.GetCheckMods();
                PrivateEditorLoad();
            }
            else
            {
                Directory.CreateDirectory(@"gamedata\config\mp\");
            }
        }

        private void btnDelSettings_Click(object sender, EventArgs e)
        {
            if (File.Exists(@"gamedata\config\mp\artefacthunt_game.ltx"))
            {
                try
                {
                    File.Delete(@"gamedata\config\mp\artefacthunt_game.ltx");
                    MessageBox.Show("Настройки режима Artefacthunt - были успешно удалены!", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка:\n"+ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Настройки режима Artefacthunt - найдены не были.", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        string Artefact;
        string green_team_shop, green_my_shop1, green_my_shop2, green_my_shop3, green_my_shop4, green_my_shop5, green_my_shop6, green_my_shop7, green_my_shop8;   
        string blue_team_shop, blue_my_shop1, blue_my_shop2, blue_my_shop3, blue_my_shop4, blue_my_shop5, blue_my_shop6, blue_my_shop7, blue_my_shop8;

        private void PrivateEditorLoad()
        {
          /*  if (File.Exists(@"gamedata\config\mp\artefacthunt_game.ltx"))
            {
                try
                {
                    File.WriteAllText(@"gamedata\config\mp\artefacthunt_game.ltx", string.Empty);
                }
                catch (Exception)
                {
                   // MessageBox.Show("Произошла ошибка:\n" + ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Error);
                   // return;
                }
            }*/
            // ===========================================================================================================================
            // MyShopIndex1 TEAM GREEN
            // ===========================================================================================================================
            if (my_shop1.SelectedIndex == 0)
            {
                green_my_shop1 = "";
            }
            if (my_shop1.SelectedIndex == 1)
            {
                green_my_shop1 = "mp_wpn_pb, ";
            }
            if (my_shop1.SelectedIndex == 2)
            {
                green_my_shop1 = "mp_wpn_fort, ";
            }
            if (my_shop1.SelectedIndex == 3)
            {
                green_my_shop1 = "mp_wpn_sig220, ";
            }
            if (my_shop1.SelectedIndex == 4)
            {
                green_my_shop1 = "mp_wpn_usp, ";
            }
            if (my_shop1.SelectedIndex == 5)
            {
                green_my_shop1 = "mp_wpn_desert_eagle, ";
            }
            // MyShopIndex2
            // =============================================================
            if (my_shop2.SelectedIndex == 0)
            {
                green_my_shop2 = "";
            }
            if (my_shop2.SelectedIndex == 1)
            {
                green_my_shop2 = "mp_wpn_bm16, ";
            }
            if (my_shop2.SelectedIndex == 2)
            {
                green_my_shop2 = "mp_wpn_wincheaster1300, ";
            }
            if (my_shop2.SelectedIndex == 3)
            {
                green_my_shop2 = "mp_wpn_spas12, ";
            }

            // MyShopIndex3
            // =============================================================
            if (my_shop3.SelectedIndex == 0)
            {
                green_my_shop3 = "";
            }
            if (my_shop3.SelectedIndex == 1)
            {
                green_my_shop3 = "mp_wpn_ak74u, ";
            }
            if (my_shop3.SelectedIndex == 2)
            {
                green_my_shop3 = "mp_wpn_ak74, ";
            }
            if (my_shop3.SelectedIndex == 3)
            {
                green_my_shop3 = "mp_wpn_abakan, ";
            }
            if (my_shop3.SelectedIndex == 4)
            {
                green_my_shop3 = "mp_wpn_groza, ";
            }
            if (my_shop3.SelectedIndex == 5)
            {
                green_my_shop3 = "mp_wpn_val, ";
            }
            if (my_shop3.SelectedIndex == 6)
            {
                green_my_shop3 = "mp_wpn_fn2000, ";
            }
            // MyShopIndex4
            // =============================================================
            if (my_shop4.SelectedIndex == 0)
            {
                green_my_shop4 = "";
            }
            if (my_shop4.SelectedIndex == 1)
            {
                green_my_shop4 = "mp_wpn_vintorez, ";
            }
            if (my_shop4.SelectedIndex == 2)
            {
                green_my_shop4 = "mp_wpn_svu, ";
            }
            if (my_shop4.SelectedIndex == 3)
            {
                green_my_shop4 = "mp_wpn_gauss, ";
            }
            // MyShopIndex5
            // =============================================================
            if (my_shop5.SelectedIndex == 0)
            {
                green_my_shop5 = "";
            }
            if (my_shop5.SelectedIndex == 1)
            {
                green_my_shop5 = "mp_wpn_rg-6, ";
            }
            if (my_shop5.SelectedIndex == 2)
            {
                green_my_shop5 = "mp_wpn_rpg7, ";
            }
            // MyShopIndex6
            // =============================================================
            if (my_shop6.SelectedIndex == 0)
            {
                green_my_shop6 = "";
            }
            if (my_shop6.SelectedIndex == 1)
            {
                green_my_shop6 = "mp_scientific_outfit, ";
            }
            if (my_shop6.SelectedIndex == 2)
            {
                green_my_shop6 = "mp_military_stalker_outfit, ";
            }
            if (my_shop6.SelectedIndex == 3)
            {
                green_my_shop6 = "mp_exo_outfit, ";
            }
            // MyShopIndex7
            // =============================================================
            if (my_shop7.SelectedIndex == 0)
            {
                green_my_shop7 = "";
            }
            if (my_shop7.SelectedIndex == 1)
            {
                green_my_shop7 = "mp_grenade_rgd5, mp_grenade_rgd5, mp_grenade_rgd5, ";
            }
            if (my_shop7.SelectedIndex == 2)
            {
                green_my_shop7 = "mp_grenade_f1, mp_grenade_f1, mp_grenade_f1, ";
            }
            if (my_shop7.SelectedIndex == 3)
            {
                green_my_shop7 = "mp_grenade_gd-05, mp_grenade_gd-05, mp_grenade_gd-05, ";
            }
            if (my_shop7.SelectedIndex == 4)
            {
                green_my_shop7 = "mp_grenade_rgd5, mp_grenade_f1, mp_grenade_gd-05, ";
            }
            if (my_shop7.SelectedIndex == 5)
            {
                green_my_shop7 = "mp_ammo_vog-25, mp_ammo_vog-25, mp_ammo_vog-25, ";
            }
            if (my_shop7.SelectedIndex == 6)
            {
                green_my_shop7 = "mp_ammo_m209, mp_ammo_m209, mp_ammo_m209, ";
            }
            if (my_shop7.SelectedIndex == 7)
            {
                green_my_shop7 = "mp_ammo_og-7b, mp_ammo_og-7b, mp_ammo_og-7b, ";
            }
            // MyShopIndex8
            // =============================================================
            if (my_shop8.SelectedIndex == 0)
            {
                green_my_shop8 = "";
            }
            if (my_shop8.SelectedIndex == 1)
            {
                green_my_shop8 = "mp_medkit, mp_medkit, mp_medkit";
            }
            if (my_shop8.SelectedIndex == 2)
            {
                green_my_shop8 = "mp_antirad, mp_antirad, mp_antirad";
            }
            if (my_shop8.SelectedIndex == 3)
            {
                green_my_shop8 = "mp_medkit, mp_antirad";
            }
            if (my_shop8.SelectedIndex == 4)
            {
                green_my_shop8 = "mp_device_torch";
            }
            if (my_shop8.SelectedIndex == 5)
            {
                green_my_shop8 = "mp_wpn_binoc";
            }
            if (my_shop8.SelectedIndex == 6)
            {
                green_my_shop8 = "mp_detector_advances";
            }
            if (my_shop8.SelectedIndex == 7)
            {
                green_my_shop8 = "mp_device_torch, mp_detector_advances, mp_wpn_binoc";
            }
            if (my_shop8.SelectedIndex == 8)
            {
                green_my_shop8 = "mp_medkit, mp_antirad, mp_device_torch, mp_detector_advances, mp_wpn_binoc";
            }
            // ===========================================================================================================================
            // MyShopIndex1 TEAM BLUE
            // ===========================================================================================================================
            if (my_shop9.SelectedIndex == 0)
            {
                blue_my_shop1 = "";
            }
            if (my_shop9.SelectedIndex == 1)
            {
                blue_my_shop1 = "mp_wpn_pm, ";
            }
            if (my_shop9.SelectedIndex == 2)
            {
                blue_my_shop1 = "mp_wpn_walther, ";
            }
            if (my_shop9.SelectedIndex == 3)
            {
                blue_my_shop1 = "mp_wpn_colt1911, ";
            }
            if (my_shop9.SelectedIndex == 4)
            {
                blue_my_shop1 = "mp_wpn_usp, ";
            }
            if (my_shop9.SelectedIndex == 5)
            {
                blue_my_shop1 = "mp_wpn_desert_eagle, ";
            }
            // MyShopIndex2
            // =============================================================
            if (my_shop10.SelectedIndex == 0)
            {
                blue_my_shop2 = "";
            }
            if (my_shop10.SelectedIndex == 1)
            {
                blue_my_shop2 = "mp_wpn_bm16, ";
            }
            if (my_shop10.SelectedIndex == 2)
            {
                blue_my_shop2 = "mp_wpn_wincheaster1300, ";
            }
            if (my_shop10.SelectedIndex == 3)
            {
                blue_my_shop2 = "mp_wpn_spas12, ";
            }
            // MyShopIndex3
            // =============================================================
            if (my_shop11.SelectedIndex == 0)
            {
                blue_my_shop3 = "";
            }
            if (my_shop11.SelectedIndex == 1)
            {
                blue_my_shop3 = "mp_wpn_mp5, ";
            }
            if (my_shop11.SelectedIndex == 2)
            {
                blue_my_shop3 = "mp_wpn_l85, ";
            }
            if (my_shop11.SelectedIndex == 3)
            {
                blue_my_shop3 = "mp_wpn_lr300, ";
            }
            if (my_shop11.SelectedIndex == 4)
            {
                blue_my_shop3 = "mp_wpn_sig550, ";
            }
            if (my_shop11.SelectedIndex == 5)
            {
                blue_my_shop3 = "mp_wpn_g36, ";
            }
            if (my_shop11.SelectedIndex == 6)
            {
                blue_my_shop3 = "mp_wpn_fn2000 ,";
            }
            // MyShopIndex4
            // =============================================================
            if (my_shop12.SelectedIndex == 0)
            {
                blue_my_shop4 = "";
            }
            if (my_shop12.SelectedIndex == 1)
            {
                blue_my_shop4 = "mp_grenade_rgd5, mp_grenade_rgd5, mp_grenade_rgd5, ";
            }
            if (my_shop12.SelectedIndex == 2)
            {
                blue_my_shop4 = "mp_grenade_f1, mp_grenade_f1, mp_grenade_f1, ";
            }
            if (my_shop12.SelectedIndex == 3)
            {
                blue_my_shop4 = "mp_grenade_gd-05, mp_grenade_gd-05, mp_grenade_gd-05, ";
            }
            if (my_shop12.SelectedIndex == 4)
            {
                blue_my_shop4 = "mp_grenade_rgd5, mp_grenade_f1, mp_grenade_gd-05, ";
            }
            if (my_shop12.SelectedIndex == 5)
            {
                blue_my_shop4 = "mp_ammo_vog-25, mp_ammo_vog-25, mp_ammo_vog-25, ";
            }
            if (my_shop12.SelectedIndex == 6)
            {
                blue_my_shop4 = "mp_ammo_m209, mp_ammo_m209, mp_ammo_m209, ";
            }
            if (my_shop12.SelectedIndex == 7)
            {
                blue_my_shop4 = "mp_ammo_og-7b, mp_ammo_og-7b, mp_ammo_og-7b, ";
            }
            // MyShopIndex5
            // =============================================================
            if (my_shop13.SelectedIndex == 0)
            {
                blue_my_shop5 = "";
            }
            if (my_shop13.SelectedIndex == 1)
            {
                blue_my_shop5 = "mp_wpn_svd, ";
            }
            if (my_shop13.SelectedIndex == 2)
            {
                blue_my_shop5 = "mp_wpn_gauss, ";
            }
            // MyShopIndex6
            // =============================================================
            if (my_shop14.SelectedIndex == 0)
            {
                blue_my_shop6 = "";
            }
            if (my_shop14.SelectedIndex == 1)
            {
                blue_my_shop6 = "mp_wpn_rg-6, ";
            }
            if (my_shop14.SelectedIndex == 2)
            {
                blue_my_shop6 = "mp_wpn_rpg7, ";
            }
            // MyShopIndex7
            // =============================================================
            if (my_shop15.SelectedIndex == 0)
            {
                blue_my_shop7 = "";
            }
            if (my_shop15.SelectedIndex == 1)
            {
                blue_my_shop7 = "mp_scientific_outfit, ";
            }
            if (my_shop15.SelectedIndex == 2)
            {
                blue_my_shop7 = "mp_military_stalker_outfit, ";
            }
            if (my_shop15.SelectedIndex == 3)
            {
                blue_my_shop7 = "mp_exo_outfit, ";
            }
            // MyShopIndex8
            // =============================================================
            if (my_shop16.SelectedIndex == 0)
            {
                blue_my_shop8 = "";
            }
            if (my_shop16.SelectedIndex == 1)
            {
                blue_my_shop8 = "mp_medkit, mp_medkit, mp_medkit";
            }
            if (my_shop16.SelectedIndex == 2)
            {
                blue_my_shop8 = "mp_antirad, mp_antirad, mp_antirad";
            }
            if (my_shop16.SelectedIndex == 3)
            {
                blue_my_shop8 = "mp_medkit, mp_antirad";
            }
            if (my_shop16.SelectedIndex == 4)
            {
                blue_my_shop8 = "mp_device_torch";
            }
            if (my_shop16.SelectedIndex == 5)
            {
                blue_my_shop8 = "mp_wpn_binoc";
            }
            if (my_shop16.SelectedIndex == 6)
            {
                blue_my_shop8 = "mp_detector_advances";
            }
            if (my_shop16.SelectedIndex == 7)
            {
                blue_my_shop8 = "mp_device_torch, mp_detector_advances, mp_wpn_binoc";
            }
            if (my_shop16.SelectedIndex == 8)
            {
                blue_my_shop8 = "mp_medkit, mp_antirad, mp_device_torch, mp_detector_advances, mp_wpn_binoc";
            }
            // =============================================================
            // ShopGreenTeamSelectIndex                    Green Team Export
            // =============================================================
            if (ShopGreenTeam.SelectedIndex == 0)
            {
                green_team_shop = "mp_wpn_knife, mp_wpn_pb";
            }
            if (ShopGreenTeam.SelectedIndex == 1)
            {
                green_team_shop = "mp_wpn_knife, mp_wpn_pb, mp_device_torch";
            }
            if (ShopGreenTeam.SelectedIndex == 2)
            {
                green_team_shop = "mp_wpn_knife, mp_wpn_pb, mp_device_torch, mp_medkit";
            }
            if (ShopGreenTeam.SelectedIndex == 3)
            {
                green_team_shop = "mp_wpn_knife, mp_wpn_pb, mp_device_torch, mp_medkit, mp_antirad";
            }
            if (ShopGreenTeam.SelectedIndex == 4)
            {
                green_team_shop = "mp_wpn_knife, mp_wpn_pb, mp_device_torch, mp_medkit, mp_antirad, mp_detector_advances, mp_grenade_rgd5, mp_grenade_f1, mp_grenade_gd-05";
            }
            if (ShopGreenTeam.SelectedIndex == 5)
            {
                green_team_shop = green_my_shop1 + green_my_shop2 + green_my_shop3 + green_my_shop4 + green_my_shop5 + green_my_shop6 + green_my_shop7 + green_my_shop8;
            }
            // =============================================================
            // ShopBlueTeamSelectIndex                      Blue Team Export
            // =============================================================
            if (ShopBlueTeam.SelectedIndex == 0)
            {
                blue_team_shop = "mp_wpn_knife, mp_wpn_pb";
            }
            if (ShopBlueTeam.SelectedIndex == 1)
            {
                blue_team_shop = "mp_wpn_knife, mp_wpn_pb, mp_device_torch";
            }
            if (ShopBlueTeam.SelectedIndex == 2)
            {
                blue_team_shop = "mp_wpn_knife, mp_wpn_pb, mp_device_torch, mp_medkit";
            }
            if (ShopBlueTeam.SelectedIndex == 3)
            {
                blue_team_shop = "mp_wpn_knife, mp_wpn_pb, mp_device_torch, mp_medkit, mp_antirad";
            }
            if (ShopBlueTeam.SelectedIndex == 4)
            {
                blue_team_shop = "mp_wpn_knife, mp_wpn_pb, mp_device_torch, mp_medkit, mp_antirad, mp_detector_advances, mp_grenade_rgd5, mp_grenade_f1, mp_grenade_gd-05";
            }
            if (ShopBlueTeam.SelectedIndex == 5)
            {
                blue_team_shop = blue_my_shop1 + blue_my_shop2 + blue_my_shop3 + blue_my_shop4 + blue_my_shop5 + blue_my_shop6 + blue_my_shop7 + blue_my_shop8;
            }
            // =============================================================
            // Type Artefact
            // =============================================================
            if (ArtefactType.SelectedIndex == 0)
            {
                Artefact = "mp_af_electra_flash";
            }
            if (ArtefactType.SelectedIndex == 1)
            {
                Artefact = "af_fireball";
            }
            if (ArtefactType.SelectedIndex == 2)
            {
                Artefact = "af_fuzz_kolobok";
            }
            if (ArtefactType.SelectedIndex == 3)
            {
                Artefact = "af_ameba_slug";
            }
            if (ArtefactType.SelectedIndex == 4)
            {
                Artefact = "af_ameba_mica";
            }
            if (ArtefactType.SelectedIndex == 5)
            {
                Artefact = "af_dummy_dummy";
            }
            if (ArtefactType.SelectedIndex == 6)
            {
                Artefact = "af_cristall";
            }
            if (ArtefactType.SelectedIndex == 7)
            {
                Artefact = "af_cristall_flower";
            }
            if (ArtefactType.SelectedIndex == 8)
            {
                Artefact = "af_electra_moonlight";
            }
            if (ArtefactType.SelectedIndex == 9)
            {
                Artefact = "af_electra_sparkler";
            }
            // ===========================================================================================================================
            // Artefacthunt_game.ltx Format 
            // ===========================================================================================================================
            ArtefacthuntEditor("[artefacthunt_gamedata]" + Environment.NewLine +
            "artefact = " + Artefact + Environment.NewLine + Environment.NewLine +
            "artefact_spawn_effect = anomaly2\\electra2_blast;" + Environment.NewLine +
            "artefact_disappear_effect = anomaly2\\electra2_blast;" + Environment.NewLine + Environment.NewLine +
            "teambase_particle_0 = static\\group_items\\net_base_red" + Environment.NewLine +
            "teambase_particle_1 = static\\group_items\\net_base_green" + Environment.NewLine +
            "teambase_particle_2 = static\\group_items\\net_base_blue" + Environment.NewLine + Environment.NewLine +
            "spawn_cost = -1500;-2500;-10000" + Environment.NewLine + Environment.NewLine +
            "[artefacthunt_base_cost]:deathmatch_base_cost" + Environment.NewLine + Environment.NewLine +
            "[artefacthunt_team1]" + Environment.NewLine +
            "team_idx = 0" + Environment.NewLine +
            "pistols = mp_wpn_pb,mp_wpn_fort, mp_wpn_sig220, mp_wpn_usp,mp_wpn_desert_eagle" + Environment.NewLine + Environment.NewLine +
            "shotgun = mp_wpn_bm16, mp_wpn_wincheaster1300, mp_wpn_spas12" + Environment.NewLine +
            "assault = mp_wpn_ak74u, mp_wpn_ak74, mp_wpn_abakan, mp_wpn_groza, mp_wpn_val, mp_wpn_fn2000" + Environment.NewLine +
            "sniper_rifles = mp_wpn_vintorez, mp_wpn_svu, mp_wpn_gauss" + Environment.NewLine +
            "heavy_weapons = mp_wpn_rg-6, mp_wpn_rpg7" + Environment.NewLine + Environment.NewLine +
            "granades = mp_grenade_rgd5, mp_grenade_f1, mp_grenade_gd-05, mp_ammo_vog-25, mp_ammo_m209, mp_ammo_og-7b, mp_ammo_9x18_fmj, mp_ammo_9x19_fmj,mp_ammo_9x19_pbp,mp_ammo_11.43x23_fmj,mp_ammo_11.43x23_hydro,mp_ammo_12x70_buck,mp_ammo_12x76_zhekan,mp_ammo_12x76_dart,mp_ammo_5.45x39_fmj,mp_ammo_5.45x39_ap,mp_ammo_5.56x45_ss190,mp_ammo_5.56x45_ap,mp_ammo_9x39_pab9,mp_ammo_9x39_ap,mp_ammo_7.62x54_7h1,mp_ammo_7.62x54_ap,mp_ammo_gauss" + Environment.NewLine +
            "outfits = mp_scientific_outfit, mp_military_stalker_outfit, mp_exo_outfit" + Environment.NewLine + Environment.NewLine +
            "equipment = mp_device_torch, mp_wpn_addon_silencer, mp_wpn_addon_scope, mp_wpn_addon_grenade_launcher, mp_medkit, mp_antirad, mp_detector_advances, mp_wpn_binoc" + Environment.NewLine +
            "skins = stalker_sv_balon_10, stalker_sv_hood_9,stalker_sv_rukzak_3,stalker_sv_rukzak_2,stalker_killer_mask_fr,stalker_killer_mask_uk" + Environment.NewLine + Environment.NewLine + Environment.NewLine +
            "default_items = " + green_team_shop + Environment.NewLine + Environment.NewLine + // TEAM GREEN INDEX
            "mp_exo_outfit = stalker_sv_exoskeleton" + Environment.NewLine +
            "mp_scientific_outfit = stalker_sci_svoboda" + Environment.NewLine +
            "mp_military_stalker_outfit = stalker_sv_military" + Environment.NewLine + Environment.NewLine +
            "money_start = 500" + Environment.NewLine +
            "money_min = 0" + Environment.NewLine + Environment.NewLine +
            "kill_rival = 400;350" + Environment.NewLine +
            "kill_self = -100" + Environment.NewLine +
            "kill_team = -250" + Environment.NewLine + Environment.NewLine +
            "target_rival = 750" + Environment.NewLine +
            "target_team = -500" + Environment.NewLine +
            "target_succeed = 1000;850;750;1000" + Environment.NewLine +
            "target_succeed_all = 750;500;" + Environment.NewLine +
            "target_failed = 300;250;100;" + Environment.NewLine + Environment.NewLine +
            "round_win = 200" + Environment.NewLine +
            "round_loose = 100" + Environment.NewLine +
            "round_draw = 0" + Environment.NewLine + Environment.NewLine +
            "round_loose_minor = 100" + Environment.NewLine +
            "round_win_minor = 150" + Environment.NewLine +
            "clear_run_bonus = 200;150" + Environment.NewLine + Environment.NewLine +
            "kill_while_invincible = 0.5" + Environment.NewLine + Environment.NewLine +
            "mp_wpn_pb_cost	= 0" + Environment.NewLine + Environment.NewLine +
            "indicator_r1 = 0.2" + Environment.NewLine +
            "indicator_r2 = 0.2" + Environment.NewLine + Environment.NewLine +
            "indicator_x = 0.0" + Environment.NewLine +
            "indicator_y = 0.4" + Environment.NewLine +
            "indicator_z = 0.0" + Environment.NewLine + Environment.NewLine +
            "indicator_shader = friendly_indicator" + Environment.NewLine +
            "indicator_texture = ui\\ui_greenteam" + Environment.NewLine + Environment.NewLine +
            "invincible_shader = friendly_indicator" + Environment.NewLine +
            "invincible_texture = ui\\ui_skull" + Environment.NewLine + Environment.NewLine + Environment.NewLine +
            // TEAM BLUE
            "[artefacthunt_team2]" + Environment.NewLine +
            "team_idx = 1" + Environment.NewLine + Environment.NewLine +
            "pistols = mp_wpn_pm, mp_wpn_walther, mp_wpn_colt1911, mp_wpn_usp, mp_wpn_desert_eagle;" + Environment.NewLine + Environment.NewLine +
            "shotgun = mp_wpn_bm16, mp_wpn_wincheaster1300, mp_wpn_spas12;" + Environment.NewLine +
            "assault = mp_wpn_mp5, mp_wpn_l85,mp_wpn_lr300,mp_wpn_sig550,mp_wpn_g36,mp_wpn_fn2000" + Environment.NewLine +
            "sniper_rifles	= mp_wpn_svd, mp_wpn_gauss" + Environment.NewLine +
            "heavy_weapons	= mp_wpn_rpg7, mp_wpn_rg-6" + Environment.NewLine + Environment.NewLine +
            "granades = mp_grenade_rgd5,mp_grenade_f1, mp_grenade_gd-05, mp_ammo_vog-25, mp_ammo_m209, mp_ammo_og-7b, mp_ammo_9x18_fmj,mp_ammo_9x18_pmm, mp_ammo_9x19_fmj,mp_ammo_9x19_pbp,mp_ammo_5.45x39_fmj,mp_ammo_5.45x39_ap,mp_ammo_5.56x45_ss190,mp_ammo_5.56x45_ap,mp_ammo_7.62x54_7h1,mp_ammo_7.62x54_ap,mp_ammo_9x39_pab9,mp_ammo_9x39_ap,mp_ammo_11.43x23_fmj,mp_ammo_11.43x23_hydro,mp_ammo_12x70_buck,mp_ammo_12x76_zhekan,mp_ammo_12x76_dart,mp_ammo_gauss" + Environment.NewLine +
            "outfits = mp_scientific_outfit, mp_military_stalker_outfit,mp_exo_outfit " + Environment.NewLine +
            "equipment = mp_device_torch, mp_wpn_addon_silencer,  mp_wpn_addon_grenade_launcher_m203, mp_medkit, mp_antirad, mp_detector_advances, mp_wpn_binoc, mp_wpn_addon_scope_susat" + Environment.NewLine + Environment.NewLine + Environment.NewLine +
            "skins = stalker_killer_head_1,stalker_killer_antigas,stalker_killer_head_3,stalker_killer_mask,stalker_killer_mask_de,stalker_killer_mask_us" + Environment.NewLine +
            "default_items = " + blue_team_shop + Environment.NewLine + Environment.NewLine + // TEAM BLUE INDEX
            "mp_exo_outfit = stalker_killer_exoskeleton" + Environment.NewLine +
            "mp_military_stalker_outfit = stalker_killer_military" + Environment.NewLine +
            "mp_scientific_outfit = stalker_sci_killer" + Environment.NewLine + Environment.NewLine +
            "money_start = 500" + Environment.NewLine +
            "money_min = 0" + Environment.NewLine + Environment.NewLine +
            "kill_rival = 400;350" + Environment.NewLine +
            "kill_self = -100" + Environment.NewLine +
            "kill_team = -250" + Environment.NewLine + Environment.NewLine +
            "target_rival = 750" + Environment.NewLine +
            "target_team = -500" + Environment.NewLine +
            "target_succeed	= 1000;850;750;1000" + Environment.NewLine +
            "target_succeed_all	= 750;500;" + Environment.NewLine +
            "target_failed = 300;250;100;" + Environment.NewLine + Environment.NewLine +
            "round_win = 200" + Environment.NewLine +
            "round_loose = 100" + Environment.NewLine +
            "round_draw = 0" + Environment.NewLine + Environment.NewLine +
            "round_loose_minor = 100" + Environment.NewLine +
            "round_win_minor = 150" + Environment.NewLine +
            "clear_run_bonus = 200;150" + Environment.NewLine + Environment.NewLine +
            "kill_while_invincible = 0.5" + Environment.NewLine + Environment.NewLine +
            "mp_wpn_pm_cost	= 0" + Environment.NewLine + Environment.NewLine +
            "indicator_r1 = 0.2" + Environment.NewLine +
            "indicator_r2 = 0.2" + Environment.NewLine + Environment.NewLine +
            "indicator_x = 0.0" + Environment.NewLine +
            "indicator_y = 0.5" + Environment.NewLine +
            "indicator_z = 0.0" + Environment.NewLine + Environment.NewLine +
            "indicator_shader = friendly_indicator" + Environment.NewLine +
            "indicator_texture = ui\\ui_blueteam" + Environment.NewLine + Environment.NewLine +
            "invincible_shader = friendly_indicator" + Environment.NewLine +
            "invincible_texture = ui\\ui_skull" + Environment.NewLine + Environment.NewLine +
            "[ahunt_messages_menu]:tdm_messages_menu" + Environment.NewLine +
            "menu_0 = ahunt_menu_0" + Environment.NewLine +
            "menu_1 = ahunt_menu_1" + Environment.NewLine + Environment.NewLine +
            "[ahunt_menu_0]:tdm_menu_0" + Environment.NewLine +
            "phrase_5 = speech_guardartifact, keep_" + Environment.NewLine +
            "phrase_6 = speech_takeartifact, take_" + Environment.NewLine + Environment.NewLine +
            "[ahunt_menu_1]:tdm_menu_1" + Environment.NewLine +
            "phrase_8 = speech_icoverartifact, cover_" + Environment.NewLine +
            "phrase_9 = speech_covermeartifact, artefact_" + Environment.NewLine);
            MessageBox.Show("Настройки успешно добавлены", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ===============================================================================================================================
        // Format artefacthunt_game.ltx
        // ===============================================================================================================================

        private void ShopGreenTeam_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ShopGreenTeam.SelectedIndex == 5)
            {
                my_shop1.Enabled = true;
                my_shop2.Enabled = true;
                my_shop3.Enabled = true;
                my_shop4.Enabled = true;
                my_shop5.Enabled = true;
                my_shop6.Enabled = true;
                my_shop7.Enabled = true;
                my_shop8.Enabled = true;
            }
            else
            {
                my_shop1.Enabled = false;
                my_shop2.Enabled = false;
                my_shop3.Enabled = false;
                my_shop4.Enabled = false;
                my_shop5.Enabled = false;
                my_shop6.Enabled = false;
                my_shop7.Enabled = false;
                my_shop8.Enabled = false;
            }
        }

        private void ShopBlueTeam_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ShopBlueTeam.SelectedIndex == 5)
            {
                my_shop9.Enabled = true;
                my_shop10.Enabled = true;
                my_shop11.Enabled = true;
                my_shop12.Enabled = true;
                my_shop13.Enabled = true;
                my_shop14.Enabled = true;
                my_shop15.Enabled = true;
                my_shop16.Enabled = true;
            }
            else
            {
                my_shop9.Enabled = false;
                my_shop10.Enabled = false;
                my_shop11.Enabled = false;
                my_shop12.Enabled = false;
                my_shop13.Enabled = false;
                my_shop14.Enabled = false;
                my_shop15.Enabled = false;
                my_shop16.Enabled = false;
            }
        }

        // ===============================================================================================================================
        //  DEATHMATCH GAMEDATA EDITOR
        // ===============================================================================================================================
        string Result;
        string sh1, sh2, sh3, sh4, sh5, sh6, sh7, sh8;
        string s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s11, s12, s13, s14, s15, s16, s17, s18, s19, s20, s21, s22, s23, s24, s25; // single weapon block
        string create_remote_weapon = "";

        // Прописываем команду, для принудительного респавна игроков. Иначе у нас ничего не получится.
        private void checkEnableDeathmatchEditor_CheckedChanged(object sender, EventArgs e) 
        {
            if (checkEnableDeathmatchEditor.CheckState == CheckState.Checked)
            {
                btnDeathmatchSelectedMyShopStart.Enabled = true;
                btnTeamDeathmatchSelectedMyShopStart.Enabled = true;
            }
            else
            {
                btnDeathmatchSelectedMyShopStart.Enabled = false;
                btnTeamDeathmatchSelectedMyShopStart.Enabled = false;
            }
        }

        private void btnDeathmatchSelectedMyShopDeleteStart_Click(object sender, EventArgs e)
        {
            if (File.Exists(@"gamedata\config\mp\deathmatch_game.ltx"))
            {
                try
                {
                    File.Delete(@"gamedata\config\mp\deathmatch_game.ltx");
                    MessageBox.Show("Настройки успешно удалены", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка.\nПричина: " + ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Настройки не были найдены.", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            if (File.Exists(@"gamedata\config\mp\teamdeathmatch_game.ltx"))
            {
                try
                {
                    File.Delete(@"gamedata\config\mp\teamdeathmatch_game.ltx");
                    //MessageBox.Show("Настройки успешно удалены", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка.\nПричина: " + ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        // update all_server_settings.ltx
        private void btnDeathmatchSelectedMyShopStart_Click(object sender, EventArgs e) 
        {
            if (File.Exists(@"gamedata\config\mp\artefacthunt_game.ltx"))
            {
                try
                {
                    checkEnableEditor.AutoCheck = false;
                    File.Delete(@"gamedata\config\mp\artefacthunt_game.ltx");
                    MessageBox.Show("Настройки для режима Artefachunt были отключены, из-за несовместимости некоторых параметров.", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка при удалений настроек Artefacthunt.\nПричина: " + ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            commands_create();
            createmod();
            WeaponBaseIndex();
            MessageBox.Show("Настройки модификаций для режима Deathmatch - были успешно созданы!", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ===============================================================================================================================
        //  DEATHMATCH & TEAM DEATHMATCH WEAPONS BASE
        // ===============================================================================================================================
        private void WeaponBaseIndex()
        {
            // Записываем все содержимое в файл. 
            DeathmatchEditor("#include \"mp_ranks.ltx\"" + Environment.NewLine +
            "#include \"mp_bonuses.ltx\"" + Environment.NewLine + Environment.NewLine +
            "[buy_menu_items_place]" + Environment.NewLine + Environment.NewLine +
            "lst_pistol	= mp_wpn_pm, mp_wpn_pb, mp_wpn_fort, mp_wpn_walther, mp_wpn_colt1911, mp_wpn_usp, mp_wpn_sig220, mp_wpn_desert_eagle;" + Environment.NewLine +
            "lst_pistol_ammo = mp_ammo_9x18_fmj, mp_ammo_9x18_pmm, mp_ammo_11.43x23_fmj, mp_ammo_11.43x23_hydro" + Environment.NewLine + Environment.NewLine +
            "lst_rifle = mp_wpn_bm16, mp_wpn_wincheaster1300, mp_wpn_spas12, mp_wpn_ak74u, mp_wpn_ak74,  mp_wpn_mp5, mp_wpn_l85, mp_wpn_lr300, mp_wpn_abakan, mp_wpn_sig550, mp_wpn_groza, mp_wpn_g36, mp_wpn_fn2000, mp_wpn_val, mp_wpn_vintorez, mp_wpn_svd, mp_wpn_svu,mp_wpn_gauss, mp_wpn_rpg7, mp_wpn_rg-6;" + Environment.NewLine +
            "lst_rifle_ammo	= mp_ammo_vog-25, mp_ammo_m209, mp_ammo_og-7b, mp_ammo_gauss, mp_ammo_9x19_fmj, mp_ammo_9x19_pbp, mp_ammo_5.45x39_fmj, mp_ammo_5.45x39_ap, mp_ammo_5.56x45_ss190, mp_ammo_5.56x45_ap, mp_ammo_7.62x54_7h1, mp_ammo_7.62x54_ap, mp_ammo_9x39_pab9, mp_ammo_9x39_ap, mp_ammo_12x70_buck, mp_ammo_12x76_zhekan, mp_ammo_12x76_dart;" + Environment.NewLine + Environment.NewLine +
            "lst_outfit = mp_exo_outfit, mp_scientific_outfit, mp_military_stalker_outfit" + Environment.NewLine +
            "lst_medkit	= mp_medkit" + Environment.NewLine +
            "lst_granade = mp_grenade_f1, mp_grenade_rgd5, mp_grenade_gd-05" + Environment.NewLine +
            "lst_others	= mp_antirad, mp_detector_advances, mp_device_torch, mp_wpn_addon_scope, mp_wpn_addon_scope_susat, mp_wpn_addon_silencer, mp_wpn_addon_grenade_launcher, mp_wpn_addon_grenade_launcher_m203, mp_wpn_binoc" + Environment.NewLine +
            "lst_shop = " + Environment.NewLine + // скорее всего то что закупили игроки.
            "lst_player_bag	= mp_wpn_knife" + Environment.NewLine + Environment.NewLine + Environment.NewLine +
            "[deathmatch_gamedata]" + Environment.NewLine +
            ";actor_spawn_effect = anomaly2\\electra2_blast" + Environment.NewLine + Environment.NewLine +
            "[deathmatch_base_cost]" + Environment.NewLine +
            "mp_wpn_knife = 0" + Environment.NewLine +
            "mp_wpn_pm = 0" + Environment.NewLine +
            "mp_wpn_pb = 0" + Environment.NewLine +
            "mp_wpn_fort = 50,0,0,0,0" + Environment.NewLine +
            "mp_wpn_walther	= 100,0,0,0,0;125" + Environment.NewLine +
            "mp_wpn_colt1911 = 150,150,0,0,0;175" + Environment.NewLine +
            "mp_wpn_sig220 = 175,175,0,0,0;225  " + Environment.NewLine +
            "mp_wpn_usp	= 175,175,175,0,0;200" + Environment.NewLine +
            "mp_wpn_desert_eagle = 300;350 " + Environment.NewLine +
            "mp_wpn_bm16 = 275" + Environment.NewLine +
            "mp_wpn_wincheaster1300	= 500;350;400" + Environment.NewLine +
            "mp_wpn_spas12 = 550;450" + Environment.NewLine + Environment.NewLine +
            "mp_wpn_ak74u = 375;400;375   " + Environment.NewLine +
            "mp_wpn_mp5 = 400;425;400" + Environment.NewLine +
            "mp_wpn_ak74 = 525;575;425" + Environment.NewLine +
            "mp_wpn_l85	= 600;650;600" + Environment.NewLine +
            "mp_wpn_abakan = 850" + Environment.NewLine +
            "mp_wpn_lr300 = 850;800" + Environment.NewLine +
            "mp_wpn_groza = 1150;900" + Environment.NewLine +
            "mp_wpn_sig550 = 1000" + Environment.NewLine +
            "mp_wpn_val	= 1300" + Environment.NewLine +
            "mp_wpn_g36 = 1450;1350" + Environment.NewLine +
            "mp_wpn_fn2000	= 1800;1750;1500;1200" + Environment.NewLine +
            "mp_wpn_vintorez  = 850;925;850" + Environment.NewLine + Environment.NewLine +
            "mp_wpn_svd	= 1700;1400;1250;1700" + Environment.NewLine +
            "mp_wpn_svu	= 1800;1500;1700;1300;1800" + Environment.NewLine +
            "mp_wpn_gauss = 2000" + Environment.NewLine +
            "mp_wpn_rpg7 = 2500" + Environment.NewLine +
            "mp_wpn_rg-6 = 2250" + Environment.NewLine +
            "mp_grenade_f1 = 250;100" + Environment.NewLine +
            "mp_grenade_rgd5 = 100;75" + Environment.NewLine +
            "mp_grenade_gd-05 = 25;100" + Environment.NewLine +
            "mp_ammo_vog-25	= 500;150" + Environment.NewLine +
            "mp_ammo_m209 = 500;150" + Environment.NewLine +
            "mp_ammo_og-7b = 1200" + Environment.NewLine +
            "mp_ammo_9x18_fmj = 0, 0, 0, 0, 0" + Environment.NewLine +
            "mp_ammo_9x18_pmm = 10;25" + Environment.NewLine +
            "mp_ammo_9x19_fmj = 25, 0, 0, 0, 0;50" + Environment.NewLine +
            "mp_ammo_9x19_pbp = 50;75" + Environment.NewLine +
            "mp_ammo_11.43x23_fmj = 50, 50, 0, 0, 0" + Environment.NewLine +
            "mp_ammo_11.43x23_hydro	= 75" + Environment.NewLine +
            "mp_ammo_5.45x39_fmj = 35;75" + Environment.NewLine +
            "mp_ammo_5.45x39_ap	= 60;100" + Environment.NewLine +
            "mp_ammo_5.56x45_ss190	= 60;100" + Environment.NewLine +
            "mp_ammo_5.56x45_ap	= 85;150" + Environment.NewLine +
            "mp_ammo_7.62x54_7h1 = 125;175" + Environment.NewLine +
            "mp_ammo_7.62x54_ap	= 165;200" + Environment.NewLine +
            "mp_ammo_9x39_pab9 = 100;125" + Environment.NewLine +
            "mp_ammo_9x39_ap = 125;175" + Environment.NewLine +
            "mp_ammo_12x70_buck	= 50;100" + Environment.NewLine +
            "mp_ammo_12x76_zhekan = 100;150" + Environment.NewLine +
            "mp_ammo_12x76_dart	= 125;200" + Environment.NewLine +
            "mp_ammo_gauss = 250; 500" + Environment.NewLine +
            "mp_exo_outfit = 1500;1250;1500" + Environment.NewLine +
            "mp_scientific_outfit = 250;300;350" + Environment.NewLine +
            "mp_military_stalker_outfit	= 750;850" + Environment.NewLine +
            "mp_medkit = 250;100" + Environment.NewLine +
            "mp_antirad	= 50" + Environment.NewLine +
            "mp_detector_advances = 50" + Environment.NewLine +
            "mp_device_torch = 50" + Environment.NewLine +
            "mp_wpn_addon_scope	= 250;150" + Environment.NewLine +
            "mp_wpn_addon_scope_susat = 250;150" + Environment.NewLine +
            "mp_wpn_addon_silencer = 150;100" + Environment.NewLine +
            "mp_wpn_addon_grenade_launcher = 350;300" + Environment.NewLine +
            "mp_wpn_addon_grenade_launcher_m203 = 350;300" + Environment.NewLine +
            "mp_wpn_binoc = 200" + Environment.NewLine +
            // ================================================================================================================
            create_remote_weapon +     // Список вырезанного снаряжения, а также модифицированное снаряжение из одиночной игры.
            // ================================================================================================================
            Environment.NewLine + Environment.NewLine +
            "[deathmatch_team0]" + Environment.NewLine +
            "team_idx = 1" + Environment.NewLine + Environment.NewLine +
            "pistols = mp_wpn_pm, mp_wpn_pb, mp_wpn_fort, mp_wpn_walther, mp_wpn_colt1911, mp_wpn_sig220, mp_wpn_usp, mp_wpn_desert_eagle; mp_wpn_hpsa, mp_wpn_beretta;" + Environment.NewLine +
            "shotgun = mp_wpn_bm16, mp_wpn_wincheaster1300, mp_wpn_spas12;" + Environment.NewLine +
            "assault = mp_wpn_ak74u, mp_wpn_mp5, mp_wpn_ak74, mp_wpn_l85, mp_wpn_abakan, mp_wpn_lr300, mp_wpn_groza, mp_wpn_sig550, mp_wpn_val, mp_wpn_g36, mp_wpn_fn2000;" + Environment.NewLine +
            "sniper_rifles = mp_wpn_vintorez, mp_wpn_svd, mp_wpn_svu, mp_wpn_gauss;" + Environment.NewLine +
            "heavy_weapons = mp_wpn_rpg7, mp_wpn_rg-6;" + Environment.NewLine + Environment.NewLine +
            "granades = mp_grenade_rgd5, mp_grenade_f1,mp_grenade_gd-05,mp_ammo_vog-25, mp_ammo_m209, mp_ammo_og-7b;, mp_ammo_9x18_fmj, mp_ammo_9x19_fmj,mp_ammo_9x19_pbp,mp_ammo_11.43x23_fmj,mp_ammo_11.43x23_hydro,mp_ammo_12x70_buck,mp_ammo_12x76_zhekan,mp_ammo_12x76_dart,mp_ammo_5.45x39_fmj,mp_ammo_5.45x39_ap,mp_ammo_5.56x45_ss190,mp_ammo_5.56x45_ap,mp_ammo_9x39_pab9,mp_ammo_9x39_ap,mp_ammo_7.62x54_7h1,mp_ammo_7.62x54_ap,mp_ammo_gauss ;mp_ammo_9x39_sp5, mp_ammo_7.62x54_7h14" + Environment.NewLine +
            "outfits = mp_scientific_outfit, mp_military_stalker_outfit,mp_exo_outfit" + Environment.NewLine +
            "equipment = mp_device_torch, mp_wpn_addon_silencer,mp_wpn_addon_scope,mp_wpn_addon_scope_susat, mp_wpn_addon_grenade_launcher,mp_wpn_addon_grenade_launcher_m203, mp_medkit, mp_antirad, mp_detector_advances, mp_wpn_binoc" + Environment.NewLine + Environment.NewLine + Environment.NewLine +
            "skins = stalker_killer_head_1,stalker_killer_antigas,stalker_killer_head_3,stalker_killer_mask,stalker_sv_balon_10, stalker_sv_hood_9,stalker_sv_rukzak_3,stalker_sv_rukzak_2,stalker_killer_mask_de,stalker_killer_mask_us,stalker_killer_mask_fr,stalker_killer_mask_uk" + Environment.NewLine + Environment.NewLine + Environment.NewLine +
            "mp_military_stalker_outfit	= stalker_killer_military" + Environment.NewLine +
            "mp_scientific_outfit = stalker_sci_killer" + Environment.NewLine +
            "mp_exo_outfit = stalker_killer_exoskeleton" + Environment.NewLine + Environment.NewLine +
            "money_start = 5000" + Environment.NewLine +
            "money_min = 0" + Environment.NewLine +
            "kill_rival = 400" + Environment.NewLine +
            "kill_self = 0" + Environment.NewLine +
            "kill_team = 0" + Environment.NewLine +
            "target_rival = 0" + Environment.NewLine +
            "target_team = 0" + Environment.NewLine +
            "target_succeed	= 0" + Environment.NewLine +
            "target_succeed_all	= 0" + Environment.NewLine +
            "round_win = 100" + Environment.NewLine +
            "round_loose = 50" + Environment.NewLine +
            "round_draw = 50" + Environment.NewLine +
            "round_loose_minor = 0" + Environment.NewLine +
            "round_win_minor = 0" + Environment.NewLine +
            "clear_run_bonus = 200" + Environment.NewLine +
            "mp_wpn_pm_cost	= 0" + Environment.NewLine +
            // ===================================================================
            "default_items = " + Result + Environment.NewLine +     // IMPORT LIST
            // ===================================================================
            "indicator_r1 = 0.2" + Environment.NewLine +
            "indicator_r2 = 0.2" + Environment.NewLine +
            "indicator_x = 0.0" + Environment.NewLine +
            "indicator_y = 0.5" + Environment.NewLine +
            "indicator_z = 0.0" + Environment.NewLine +
            "indicator_shader = friendly_indicator" + Environment.NewLine +
            "indicator_texture = ui\\ui_blueteam" + Environment.NewLine +
            "invincible_shader = friendly_indicator" + Environment.NewLine +
            "invincible_texture = ui\\ui_skull" + Environment.NewLine);
        }

        private void createmod()
        {
            if (!Directory.Exists(@"gamedata\config\mp\"))
            {
                Directory.CreateDirectory(@"gamedata\config\mp\");
            }
            /*
            if (File.Exists(@"gamedata\config\mp\deathmatch_game.ltx"))
            {
                try
                {
                    File.WriteAllText(@"gamedata\config\mp\deathmatch_game.ltx", string.Empty);
                }
                catch (Exception)
                {
                    // error
                }
            }*/
            // ===========================================================================================================================
            // MyShopIndex1 ALL TEAM
            // ===========================================================================================================================
            if (dm2.SelectedIndex == 0)
            {
                sh1 = "";
            }
            if (dm2.SelectedIndex == 1)
            {
                sh1 = "mp_wpn_pb, ";
            }
            if (dm2.SelectedIndex == 2)
            {
                sh1 = "mp_wpn_fort, ";
            }
            if (dm2.SelectedIndex == 3)
            {
                sh1 = "mp_wpn_sig220, ";
            }
            if (dm2.SelectedIndex == 4)
            {
                sh1 = "mp_wpn_usp, ";
            }
            if (dm2.SelectedIndex == 5)
            {
                sh1 = "mp_wpn_desert_eagle, ";
            }
            if (dm2.SelectedIndex == 6)
            {
                sh1 = "mp_wpn_pb,mp_wpn_fort,mp_wpn_sig220,mp_wpn_usp,mp_wpn_desert_eagle, ";
            }
            // MyShopIndex2
            // =============================================================
            if (dm3.SelectedIndex == 0)
            {
                sh2 = "";
            }
            if (dm3.SelectedIndex == 1)
            {
                sh2 = "mp_wpn_bm16, ";
            }
            if (dm3.SelectedIndex == 2)
            {
                sh2 = "mp_wpn_wincheaster1300, ";
            }
            if (dm3.SelectedIndex == 3)
            {
                sh2 = "mp_wpn_spas12, ";
            }
            if (dm3.SelectedIndex == 4)
            {
                sh2 = "mp_wpn_bm16, mp_wpn_wincheaster1300, mp_wpn_spas12, ";
            }
            // MyShopIndex3
            // =============================================================
            if (dm4.SelectedIndex == 0)
            {
                sh3 = "";
            }
            if (dm4.SelectedIndex == 1)
            {
                sh3 = "mp_wpn_ak74u, ";
            }
            if (dm4.SelectedIndex == 2)
            {
                sh3 = "mp_wpn_ak74, ";
            }
            if (dm4.SelectedIndex == 3)
            {
                sh3 = "mp_wpn_abakan, ";
            }
            if (dm4.SelectedIndex == 4)
            {
                sh3 = "mp_wpn_groza, ";
            }
            if (dm4.SelectedIndex == 5)
            {
                sh3 = "mp_wpn_val, ";
            }
            if (dm4.SelectedIndex == 6)
            {
                sh3 = "mp_wpn_fn2000, ";
            }
            if (dm4.SelectedIndex == 7)
            {
                sh3 = "mp_wpn_ak74u, mp_wpn_ak74, mp_wpn_abakan, mp_wpn_groza, mp_wpn_val, mp_wpn_fn2000, ";
            }
            // MyShopIndex4
            // =============================================================
            if (dm5.SelectedIndex == 0)
            {
                sh7 = "";
            }
            if (dm5.SelectedIndex == 1)
            {
                sh7 = "mp_grenade_rgd5, mp_grenade_rgd5, mp_grenade_rgd5, ";
            }
            if (dm5.SelectedIndex == 2)
            {
                sh7 = "mp_grenade_f1, mp_grenade_f1, mp_grenade_f1, ";
            }
            if (dm5.SelectedIndex == 3)
            {
                sh7 = "mp_grenade_gd-05, mp_grenade_gd-05, mp_grenade_gd-05, ";
            }
            if (dm5.SelectedIndex == 4)
            {
                sh7 = "mp_grenade_rgd5, mp_grenade_f1, mp_grenade_gd-05, ";
            }
            if (dm5.SelectedIndex == 5)
            {
                sh7 = "mp_ammo_vog-25, mp_ammo_vog-25, mp_ammo_vog-25, ";
            }
            if (dm5.SelectedIndex == 6)
            {
                sh7 = "mp_ammo_m209, mp_ammo_m209, mp_ammo_m209, ";
            }
            if (dm5.SelectedIndex == 7)
            {
                sh7 = "mp_ammo_og-7b, mp_ammo_og-7b, mp_ammo_og-7b, ";
            }
            // MyShopIndex5
            // =============================================================
            if (dm6.SelectedIndex == 0)
            {
                sh4 = "";
            }
            if (dm6.SelectedIndex == 1)
            {
                sh4 = "mp_wpn_vintorez, ";
            }
            if (dm6.SelectedIndex == 2)
            {
                sh4 = "mp_wpn_svu, ";
            }
            if (dm6.SelectedIndex == 3)
            {
                sh4 = "mp_wpn_gauss, ";
            }
            if (dm6.SelectedIndex == 4)
            {
                sh4 = "mp_wpn_vintorez, mp_wpn_svu, mp_wpn_gauss, ";
            }
            // MyShopIndex6
            // =============================================================
            if (dm7.SelectedIndex == 0)
            {
                sh5 = "";
            }
            if (dm7.SelectedIndex == 1)
            {
                sh5 = "mp_wpn_rg-6, ";
            }
            if (dm7.SelectedIndex == 2)
            {
                sh5 = "mp_wpn_rpg7, ";
            }
            if (dm7.SelectedIndex == 3)
            {
                sh5 = "mp_wpn_rg-6, mp_wpn_rpg7, ";
            }
            // MyShopIndex7
            // =============================================================
            if (dm8.SelectedIndex == 0)
            {
                sh6 = "";
            }
            if (dm8.SelectedIndex == 1)
            {
                sh6 = "mp_scientific_outfit, ";
            }
            if (dm8.SelectedIndex == 2)
            {
                sh6 = "mp_military_stalker_outfit, ";
            }
            if (dm8.SelectedIndex == 3)
            {
                sh6 = "mp_exo_outfit, ";
            }
            if (dm8.SelectedIndex == 4)
            {
                sh6 = "mp_scientific_outfit, mp_military_stalker_outfit, mp_exo_outfit, ";
            }

            // MyShopIndex8
            // =============================================================
            if (dm9.SelectedIndex == 0)
            {
                sh8 = "";
            }
            if (dm9.SelectedIndex == 1)
            {
                sh8 = "mp_medkit, mp_medkit, mp_medkit";
            }
            if (dm9.SelectedIndex == 2)
            {
                sh8 = "mp_antirad, mp_antirad, mp_antirad";
            }
            if (dm9.SelectedIndex == 3)
            {
                sh8 = "mp_medkit, mp_antirad";
            }
            if (dm9.SelectedIndex == 4)
            {
                sh8 = "mp_device_torch";
            }
            if (dm9.SelectedIndex == 5)
            {
                sh8 = "mp_wpn_binoc";
            }
            if (dm9.SelectedIndex == 6)
            {
                sh8 = "mp_detector_advances";
            }
            if (dm9.SelectedIndex == 7)
            {
                sh8 = "mp_device_torch, mp_detector_advances, mp_wpn_binoc";
            }
            if (dm9.SelectedIndex == 8)
            {
                sh8 = "mp_medkit, mp_antirad, mp_device_torch, mp_detector_advances, mp_wpn_binoc";
            }
            // ==========================================================================================================================
            // Singles Weapon Block 1
            // ==========================================================================================================================
            if (wn1.SelectedIndex == 0)    // Не использовать        
            {
                s1 = "";
            }
            if (wn1.SelectedIndex == 1)    // АС - 96 / 2    
            {
                s1 = "wpn_abakan_m1, ";
            }
            if (wn1.SelectedIndex == 2)    // АКМ - 74 / 2
            {
                s1 = "wpn_ak74_m1, ";
            }
            if (wn1.SelectedIndex == 3)    // AKM - 74 / 2U
            {
                s1 = "wpn_ak74u_m1, ";
            }
            if (wn1.SelectedIndex == 4)    // Гром С-14
            {
                s1 = "wpn_groza_m1, ";
            }
            if (wn1.SelectedIndex == 5)    // ИЛ86
            {
                s1 = "wpn_l85_m2, ";
            }
            if (wn1.SelectedIndex == 6)    // ТРс301
            {
                s1 = "wpn_lr300_m1, ";
            }
            if (wn1.SelectedIndex == 7)    // Сиг
            {
                s1 = "wpn_sig_m1, ";
            }
            if (wn1.SelectedIndex == 8)    // ВЛА
            {
                s1 = "wpn_val_m1, ";
            }
            if (wn1.SelectedIndex == 9)    // Гадюка
            {
                s1 = "wpn_mp5_m2, ";
            }
            // =============================================================
            // Singles Weapon Block 2
            // =============================================================
            if (wn9.SelectedIndex == 0)    // Не использовать        
            {
                s2 = "";
            }
            if (wn9.SelectedIndex == 1)    // АС - 96 / 2    
            {
                s2 = "wpn_abakan_m1, ";
            }
            if (wn9.SelectedIndex == 2)    // АКМ - 74 / 2
            {
                s2 = "wpn_ak74_m1, ";
            }
            if (wn9.SelectedIndex == 3)    // AKM - 74 / 2U
            {
                s2 = "wpn_ak74u_m1, ";
            }
            if (wn9.SelectedIndex == 4)    // Гром С-14
            {
                s2 = "wpn_groza_m1, ";
            }
            if (wn9.SelectedIndex == 5)    // ИЛ86
            {
                s2 = "wpn_l85_m2, ";
            }
            if (wn9.SelectedIndex == 6)    // ТРс301
            {
                s2 = "wpn_lr300_m1, ";
            }
            if (wn9.SelectedIndex == 7)    // Сиг
            {
                s2 = "wpn_sig_m1, ";
            }
            if (wn9.SelectedIndex == 8)    // ВЛА
            {
                s2 = "wpn_val_m1, ";
            }
            if (wn9.SelectedIndex == 9)    // Гадюка
            {
                s2 = "wpn_mp5_m2, ";
            }
            // =============================================================
            // Singles Weapon Block 3
            // =============================================================
            if (wn14.SelectedIndex == 0)    // Не использовать        
            {
                s3 = "";
            }
            if (wn14.SelectedIndex == 1)    // АС - 96 / 2    
            {
                s3 = "wpn_abakan_m1, ";
            }
            if (wn14.SelectedIndex == 2)    // АКМ - 74 / 2
            {
                s3 = "wpn_ak74_m1, ";
            }
            if (wn14.SelectedIndex == 3)    // AKM - 74 / 2U
            {
                s3 = "wpn_ak74u_m1, ";
            }
            if (wn14.SelectedIndex == 4)    // Гром С-14
            {
                s3 = "wpn_groza_m1, ";
            }
            if (wn14.SelectedIndex == 5)    // ИЛ86
            {
                s3 = "wpn_l85_m2, ";
            }
            if (wn14.SelectedIndex == 6)    // ТРс301
            {
                s3 = "wpn_lr300_m1, ";
            }
            if (wn14.SelectedIndex == 7)    // Сиг
            {
                s3 = "wpn_sig_m1, ";
            }
            if (wn14.SelectedIndex == 8)    // ВЛА
            {
                s3 = "wpn_val_m1, ";
            }
            if (wn14.SelectedIndex == 9)    // Гадюка
            {
                s3 = "wpn_mp5_m2, ";
            }
            // =============================================================
            // Singles Weapon Block 4
            // =============================================================
            if (wn18.SelectedIndex == 0)    // Не использовать        
            {
                s4 = "";
            }
            if (wn18.SelectedIndex == 1)    // АС - 96 / 2    
            {
                s4 = "wpn_abakan_m1, ";
            }
            if (wn18.SelectedIndex == 2)    // АКМ - 74 / 2
            {
                s4 = "wpn_ak74_m1, ";
            }
            if (wn18.SelectedIndex == 3)    // AKM - 74 / 2U
            {
                s4 = "wpn_ak74u_m1, ";
            }
            if (wn18.SelectedIndex == 4)    // Гром С-14
            {
                s4 = "wpn_groza_m1, ";
            }
            if (wn18.SelectedIndex == 5)    // ИЛ86
            {
                s4 = "wpn_l85_m2, ";
            }
            if (wn18.SelectedIndex == 6)    // ТРс301
            {
                s4 = "wpn_lr300_m1, ";
            }
            if (wn18.SelectedIndex == 7)    // Сиг
            {
                s4 = "wpn_sig_m1, ";
            }
            if (wn18.SelectedIndex == 8)    // ВЛА
            {
                s4 = "wpn_val_m1, ";
            }
            if (wn18.SelectedIndex == 9)    // Гадюка
            {
                s4 = "wpn_mp5_m2, ";
            }
            // =============================================================
            // Singles Weapon Block 5
            // =============================================================
            if (wn22.SelectedIndex == 0)    // Не использовать        
            {
                s5 = "";
            }
            if (wn22.SelectedIndex == 1)    // АС - 96 / 2    
            {
                s5 = "wpn_abakan_m1, ";
            }
            if (wn22.SelectedIndex == 2)    // АКМ - 74 / 2
            {
                s5 = "wpn_ak74_m1, ";
            }
            if (wn22.SelectedIndex == 3)    // AKM - 74 / 2U
            {
                s5 = "wpn_ak74u_m1, ";
            }
            if (wn22.SelectedIndex == 4)    // Гром С-14
            {
                s5 = "wpn_groza_m1, ";
            }
            if (wn22.SelectedIndex == 5)    // ИЛ86
            {
                s5 = "wpn_l85_m2, ";
            }
            if (wn22.SelectedIndex == 6)    // ТРс301
            {
                s5 = "wpn_lr300_m1, ";
            }
            if (wn22.SelectedIndex == 7)    // Сиг
            {
                s5 = "wpn_sig_m1, ";
            }
            if (wn22.SelectedIndex == 8)    // ВЛА
            {
                s5 = "wpn_val_m1, ";
            }
            if (wn22.SelectedIndex == 9)    // Гадюка
            {
                s5 = "wpn_mp5_m2, ";
            }
            if (wn22.SelectedIndex == 10)   // Весь набор
            {
                s5 = "wpn_abakan_m1, wpn_ak74_m1, wpn_ak74u_m1, wpn_groza_m1, wpn_l85_m2, wpn_lr300_m1, wpn_sig_m1, wpn_val_m1, wpn_mp5_m2, ";
            }
            // ==========================================================================================================================
            // Singles Weapon Block Table 2
            // ==========================================================================================================================
            if (wn2.SelectedIndex == 0)    // Не использовать        
            {
                s6 = "";
            }
            if (wn2.SelectedIndex == 1)    // Кольт 
            {
                s6 = "wpn_colt_m1, ";
            }
            if (wn2.SelectedIndex == 2)    // Черный ястреб     
            {
                s6 = "wpn_eagle_m1, ";
            }
            if (wn2.SelectedIndex == 3)    // Фора      
            {
                s6 = "wpn_fort_m1, ";
            }
            if (wn2.SelectedIndex == 4)    // Волкер-П9м    
            {
                s6 = "wpn_walther_m1, ";
            }
            if (wn2.SelectedIndex == 5)    // ХПСС-1м (Уникальное)    
            {
                s6 = "mp_wpn_hpsa, ";
            }
            if (wn2.SelectedIndex == 6)    // Марта (Уникальное)  
            {
                s6 = "mp_wpn_beretta, ";
            }
            // =============================================================
            // Singles Weapon Block 2, 2
            // =============================================================

            if (wn10.SelectedIndex == 0)    // Не использовать        
            {
                s7 = "";
            }
            if (wn10.SelectedIndex == 1)    // Кольт 
            {
                s7 = "wpn_colt_m1, ";
            }
            if (wn10.SelectedIndex == 2)    // Черный ястреб     
            {
                s7 = "wpn_eagle_m1, ";
            }
            if (wn10.SelectedIndex == 3)    // Фора      
            {
                s7 = "wpn_fort_m1, ";
            }
            if (wn10.SelectedIndex == 4)    // Волкер-П9м    
            {
                s7 = "wpn_walther_m1, ";
            }
            if (wn10.SelectedIndex == 5)    // ХПСС-1м (Уникальное)    
            {
                s7 = "mp_wpn_hpsa, ";
            }
            if (wn10.SelectedIndex == 6)    // Марта (Уникальное)  
            {
                s7 = "mp_wpn_beretta, ";
            }
            // =============================================================
            // Singles Weapon Block 2, 3
            // =============================================================

            if (wn15.SelectedIndex == 0)    // Не использовать        
            {
                s8 = "";
            }
            if (wn15.SelectedIndex == 1)    // Кольт 
            {
                s8 = "wpn_colt_m1, ";
            }
            if (wn15.SelectedIndex == 2)    // Черный ястреб     
            {
                s8 = "wpn_eagle_m1, ";
            }
            if (wn15.SelectedIndex == 3)    // Фора      
            {
                s8 = "wpn_fort_m1, ";
            }
            if (wn15.SelectedIndex == 4)    // Волкер-П9м    
            {
                s8 = "wpn_walther_m1, ";
            }
            if (wn15.SelectedIndex == 5)    // ХПСС-1м (Уникальное)    
            {
                s8 = "mp_wpn_hpsa, ";
            }
            if (wn15.SelectedIndex == 6)    // Марта (Уникальное)  
            {
                s8 = "mp_wpn_beretta, ";
            }
            // =============================================================
            // Singles Weapon Block 2, 4
            // =============================================================

            if (wn19.SelectedIndex == 0)    // Не использовать        
            {
                s9 = "";
            }
            if (wn19.SelectedIndex == 1)    // Кольт 
            {
                s9 = "wpn_colt_m1, ";
            }
            if (wn19.SelectedIndex == 2)    // Черный ястреб     
            {
                s9 = "wpn_eagle_m1, ";
            }
            if (wn19.SelectedIndex == 3)    // Фора      
            {
                s9 = "wpn_fort_m1, ";
            }
            if (wn19.SelectedIndex == 4)    // Волкер-П9м    
            {
                s9 = "wpn_walther_m1, ";
            }
            if (wn19.SelectedIndex == 5)    // ХПСС-1м (Уникальное)    
            {
                s9 = "mp_wpn_hpsa, ";
            }
            if (wn19.SelectedIndex == 6)    // Марта (Уникальное)  
            {
                s9 = "mp_wpn_beretta, ";
            }
            // =============================================================
            // Singles Weapon Block 2, 5
            // =============================================================

            if (wn23.SelectedIndex == 0)    // Не использовать        
            {
                s10 = "";
            }
            if (wn23.SelectedIndex == 1)    // Кольт 
            {
                s10 = "wpn_colt_m1, ";
            }
            if (wn23.SelectedIndex == 2)    // Черный ястреб     
            {
                s10 = "wpn_eagle_m1, ";
            }
            if (wn23.SelectedIndex == 3)    // Фора      
            {
                s10 = "wpn_fort_m1, ";
            }
            if (wn23.SelectedIndex == 4)    // Волкер-П9м    
            {
                s10 = "wpn_walther_m1, ";
            }
            if (wn23.SelectedIndex == 5)    // ХПСС-1м (Уникальное)    
            {
                s10 = "mp_wpn_hpsa, ";
            }
            if (wn23.SelectedIndex == 6)    // Марта (Уникальное)  
            {
                s10 = "mp_wpn_beretta, ";
            }
            if (wn23.SelectedIndex == 7)    // Весь набор
            {
                s10 = "wpn_colt_m1, wpn_eagle_m1, wpn_fort_m1,wpn_walther_m1,mp_wpn_hpsa, mp_wpn_beretta, ";
            }
            // ==========================================================================================================================
            // Singles Weapon Block Table 3
            // ==========================================================================================================================
            if (wn3.SelectedIndex == 0)    // Не использовать        
            {
                s11 = "";
            }
            if (wn3.SelectedIndex == 1)    // Аптечка (Красная)  
            {
                s11 = "mp_medkit, ";
            }
            if (wn3.SelectedIndex == 2)    // Армейская Аптечка (Синяя)         
            {
                s11 = "medkit_army, ";
            }
            if (wn3.SelectedIndex == 3)    // Научная Аптечка (Желтая)               
            {
                s11 = "medkit_scientic, ";
            }
            if (wn3.SelectedIndex == 4)    // 3 Аптечка разных видов      
            {
                s11 = "mp_medkit, medkit_army, medkit_scientic, ";
            }
            if (wn3.SelectedIndex == 5)    // Бинт   
            {
                s11 = "bandage, ";
            }
            if (wn3.SelectedIndex == 6)    // Противорадиационный препарат      
            {
                s11 = "mp_antirad, ";
            }
            if (wn3.SelectedIndex == 7)    // Водка (Антирад -20%)   
            {
                s11 = "vodka, ";
            }
            // =============================================================
            // Singles Weapon Block 3, 2
            // =============================================================
            if (wn11.SelectedIndex == 0)    // Не использовать        
            {
                s12 = "";
            }
            if (wn11.SelectedIndex == 1)    // Аптечка (Красная)  
            {
                s12 = "mp_medkit, ";
            }
            if (wn11.SelectedIndex == 2)    // Армейская Аптечка (Синяя)         
            {
                s12 = "medkit_army, ";
            }
            if (wn11.SelectedIndex == 3)    // Научная Аптечка (Желтая)               
            {
                s12 = "medkit_scientic, ";
            }
            if (wn11.SelectedIndex == 4)    // 3 Аптечка разных видов      
            {
                s12 = "mp_medkit, medkit_army, medkit_scientic, ";
            }
            if (wn11.SelectedIndex == 5)    // Бинт   
            {
                s12 = "bandage, ";
            }
            if (wn11.SelectedIndex == 6)    // Противорадиационный препарат      
            {
                s12 = "mp_antirad, ";
            }
            if (wn11.SelectedIndex == 7)    // Водка (Антирад -20%)   
            {
                s12 = "vodka, ";
            }
            // =============================================================
            // Singles Weapon Block 3, 3
            // =============================================================
            if (wn16.SelectedIndex == 0)    // Не использовать        
            {
                s13 = "";
            }
            if (wn16.SelectedIndex == 1)    // Аптечка (Красная)  
            {
                s13 = "mp_medkit, ";
            }
            if (wn16.SelectedIndex == 2)    // Армейская Аптечка (Синяя)         
            {
                s13 = "medkit_army, ";
            }
            if (wn16.SelectedIndex == 3)    // Научная Аптечка (Желтая)               
            {
                s13 = "medkit_scientic, ";
            }
            if (wn16.SelectedIndex == 4)    // 3 Аптечка разных видов      
            {
                s13 = "mp_medkit, medkit_army, medkit_scientic, ";
            }
            if (wn16.SelectedIndex == 5)    // Бинт   
            {
                s13 = "bandage, ";
            }
            if (wn16.SelectedIndex == 6)    // Противорадиационный препарат      
            {
                s13 = "mp_antirad, ";
            }
            if (wn16.SelectedIndex == 7)    // Водка (Антирад -20%)   
            {
                s13 = "vodka, ";
            }
            // =============================================================
            // Singles Weapon Block 3, 4
            // =============================================================
            if (wn20.SelectedIndex == 0)    // Не использовать        
            {
                s14 = "";
            }
            if (wn20.SelectedIndex == 1)    // Аптечка (Красная)  
            {
                s14 = "mp_medkit, ";
            }
            if (wn20.SelectedIndex == 2)    // Армейская Аптечка (Синяя)         
            {
                s14 = "medkit_army, ";
            }
            if (wn20.SelectedIndex == 3)    // Научная Аптечка (Желтая)               
            {
                s14 = "medkit_scientic, ";
            }
            if (wn20.SelectedIndex == 4)    // 3 Аптечка разных видов      
            {
                s14 = "mp_medkit, medkit_army, medkit_scientic, ";
            }
            if (wn20.SelectedIndex == 5)    // Бинт   
            {
                s14 = "bandage, ";
            }
            if (wn20.SelectedIndex == 6)    // Противорадиационный препарат      
            {
                s14 = "mp_antirad, ";
            }
            if (wn20.SelectedIndex == 7)    // Водка (Антирад -20%)   
            {
                s14 = "vodka, ";
            }
            // =============================================================
            // Singles Weapon Block 3, 5
            // =============================================================
            if (wn24.SelectedIndex == 0)    // Не использовать        
            {
                s15 = "";
            }
            if (wn24.SelectedIndex == 1)    // Аптечка (Красная)  
            {
                s15 = "mp_medkit, ";
            }
            if (wn24.SelectedIndex == 2)    // Армейская Аптечка (Синяя)         
            {
                s15 = "medkit_army, ";
            }
            if (wn24.SelectedIndex == 3)    // Научная Аптечка (Желтая)               
            {
                s15 = "medkit_scientic, ";
            }
            if (wn24.SelectedIndex == 4)    // 3 Аптечка разных видов      
            {
                s15 = "mp_medkit, medkit_army, medkit_scientic, ";
            }
            if (wn24.SelectedIndex == 5)    // Бинт   
            {
                s15 = "bandage, ";
            }
            if (wn24.SelectedIndex == 6)    // Противорадиационный препарат      
            {
                s15 = "mp_antirad, ";
            }
            if (wn24.SelectedIndex == 7)    // Водка (Антирад -20%)   
            {
                s15 = "vodka, ";
            }
            if (wn24.SelectedIndex == 8)    // Весь набор      
            {
                s15 = "mp_medkit,medkit_army,medkit_scientic,bandage,mp_antirad, vodka, ";
            }
            // ==========================================================================================================================
            // Singles Weapon Block Table 4, 1
            // ==========================================================================================================================
            if (wn4.SelectedIndex == 0)    // Не использовать        
            {
                s16 = "";
            }
            if (wn4.SelectedIndex == 1)    // Хлеб (Лечение 10%)      
            {
                s16 = "bread, ";
            }
            if (wn4.SelectedIndex == 2)    // Колбаса (Лечение 10%)      
            {
                s16 = "kolbasa, ";
            }
            if (wn4.SelectedIndex == 3)    // Тушенка (Лечение 10%)     
            {
                s16 = "conserva, ";
            }
            if (wn4.SelectedIndex == 4)    // Энергетик      
            {
                s16 = "energy_drink, ";
            }
            if (wn4.SelectedIndex == 5)    // Фонарик        
            {
                s16 = "mp_device_torch, ";
            }
            if (wn4.SelectedIndex == 6)    // Бинокль       
            {
                s16 = "mp_wpn_binoc, ";
            }
            if (wn4.SelectedIndex == 7)    // Детектор аномалий       
            {
                s16 = "detector_simple, ";
            }
            if (wn4.SelectedIndex == 8)    // Прицел Susat    
            {
                s16 = "wpn_addon_scope_susat, ";
            }
            if (wn4.SelectedIndex == 9)    // Прицел ПСО     
            {
                s16 = "wpn_addon_scope, ";
            }
            if (wn4.SelectedIndex == 10)    // Прицел Susat + Прицел ПСО   
            {
                s16 = "wpn_addon_scope_susat, wpn_addon_scope, ";
            }
            // =============================================================
            // Singles Weapon Block 4, 2
            // =============================================================
            if (wn12.SelectedIndex == 0)    // Не использовать        
            {
                s17 = "";
            }
            if (wn12.SelectedIndex == 1)    // Хлеб (Лечение 10%)      
            {
                s17 = "bread, ";
            }
            if (wn12.SelectedIndex == 2)    // Колбаса (Лечение 10%)      
            {
                s17 = "kolbasa, ";
            }
            if (wn12.SelectedIndex == 3)    // Тушенка (Лечение 10%)     
            {
                s17 = "conserva, ";
            }
            if (wn12.SelectedIndex == 4)    // Энергетик      
            {
                s17 = "energy_drink, ";
            }
            if (wn12.SelectedIndex == 5)    // Фонарик        
            {
                s17 = "mp_device_torch, ";
            }
            if (wn12.SelectedIndex == 6)    // Бинокль       
            {
                s17 = "mp_wpn_binoc, ";
            }
            if (wn12.SelectedIndex == 7)    // Детектор аномалий       
            {
                s17 = "detector_simple, ";
            }
            if (wn12.SelectedIndex == 8)    // Прицел Susat    
            {
                s17 = "wpn_addon_scope_susat, ";
            }
            if (wn12.SelectedIndex == 9)    // Прицел ПСО     
            {
                s17 = "wpn_addon_scope, ";
            }
            if (wn12.SelectedIndex == 10)    // Прицел Susat + Прицел ПСО   
            {
                s17 = "wpn_addon_scope_susat, wpn_addon_scope, ";
            }
            // =============================================================
            // Singles Weapon Block 4, 3
            // =============================================================
            if (wn17.SelectedIndex == 0)    // Не использовать        
            {
                s18 = "";
            }
            if (wn17.SelectedIndex == 1)    // Хлеб (Лечение 10%)      
            {
                s18 = "bread, ";
            }
            if (wn17.SelectedIndex == 2)    // Колбаса (Лечение 10%)      
            {
                s18 = "kolbasa, ";
            }
            if (wn17.SelectedIndex == 3)    // Тушенка (Лечение 10%)     
            {
                s18 = "conserva, ";
            }
            if (wn17.SelectedIndex == 4)    // Энергетик      
            {
                s18 = "energy_drink, ";
            }
            if (wn17.SelectedIndex == 5)    // Фонарик  
            {       
                s18 = "mp_device_torch, ";
            }
            if (wn17.SelectedIndex == 6)    // Бинокль       
            {
                s18 = "mp_wpn_binoc, ";
            }
            if (wn17.SelectedIndex == 7)    // Детектор аномалий       
            {
                s18 = "detector_simple, ";
            }
            if (wn17.SelectedIndex == 8)    // Прицел Susat    
            {
                s18 = "wpn_addon_scope_susat, ";
            }
            if (wn17.SelectedIndex == 9)    // Прицел ПСО     
            {
                s18 = "wpn_addon_scope, ";
            }
            if (wn17.SelectedIndex == 10)    // Прицел Susat + Прицел ПСО   
            {
                s18 = "wpn_addon_scope_susat, wpn_addon_scope, ";
            }
            // =============================================================
            // Singles Weapon Block 4, 4
            // =============================================================
            if (wn21.SelectedIndex == 0)    // Не использовать        
            {
               s19 = "";
            }
            if (wn21.SelectedIndex == 1)    // Хлеб (Лечение 10%)      
            {
                s19 = "bread, ";
            }
            if (wn21.SelectedIndex == 2)    // Колбаса (Лечение 10%)      
            {
                s19 = "kolbasa, ";
            }
            if (wn21.SelectedIndex == 3)    // Тушенка (Лечение 10%)     
            {
                s19 = "conserva, ";
            }
            if (wn21.SelectedIndex == 4)    // Энергетик      
            {
                s19 = "energy_drink, ";
            }
            if (wn21.SelectedIndex == 5)    // Фонарик  
            {
                s19 = "mp_device_torch, ";
            }
            if (wn21.SelectedIndex == 6)    // Бинокль       
            {
                s19 = "mp_wpn_binoc, ";
            }
            if (wn21.SelectedIndex == 7)    // Детектор аномалий   mp_detector_advances   
            {
                s19 = "detector_simple, ";
            }
            if (wn21.SelectedIndex == 8)    // Прицел Susat    
            {
                s19 = "wpn_addon_scope_susat, ";
            }
            if (wn21.SelectedIndex == 9)    // Прицел ПСО     
            {
                s19 = "wpn_addon_scope, ";
            }
            if (wn21.SelectedIndex == 10)    // Прицел Susat + Прицел ПСО   
            {
                s19 = "wpn_addon_scope_susat, wpn_addon_scope, ";
            }
            // =============================================================
            // Singles Weapon Block 4, 5
            // =============================================================
            if (wn25.SelectedIndex == 0)    // Не использовать        
            {
                s20 = "";
            }
            if (wn25.SelectedIndex == 1)    // Хлеб (Лечение 10%)      
            {
                s20 = "bread, ";
            }
            if (wn25.SelectedIndex == 2)    // Колбаса (Лечение 10%)      
            {
                s20 = "kolbasa, ";
            }
            if (wn25.SelectedIndex == 3)    // Тушенка (Лечение 10%)     
            {
                s20 = "conserva, ";
            }
            if (wn25.SelectedIndex == 4)    // Энергетик      
            {
                s20 = "energy_drink, ";
            }
            if (wn25.SelectedIndex == 5)    // Фонарик 
            {  
                s20 = "mp_device_torch, ";
            }
            if (wn25.SelectedIndex == 6)    // Бинокль       
            {
                s20 = "mp_wpn_binoc, ";
            }
            if (wn25.SelectedIndex == 7)    // Детектор аномалий       
            {
                s20 = "detector_simple, ";
            }
            if (wn25.SelectedIndex == 8)    // Прицел Susat    
            {
                s20 = "wpn_addon_scope_susat, ";
            }
            if (wn25.SelectedIndex == 9)    // Прицел ПСО     
            {
                s20 = "wpn_addon_scope, ";
            }
            if (wn25.SelectedIndex == 10)   // Прицел Susat + Прицел ПСО   
            {
                s20 = "wpn_addon_scope_susat, wpn_addon_scope, ";
            }
            if (wn25.SelectedIndex == 11)   // Весь набор
            {
                s20 = "bread,kolbasa,conserva,energy_drink,mp_device_torch,mp_wpn_binoc,detector_simple,wpn_addon_scope_susat,wpn_addon_scope, ";
            }
            // ==========================================================================================================================
            // Singles Weapon 
            // ==========================================================================================================================
            if (wn5.SelectedIndex == 0)    // Не использовать        
            {
                s21 = "";
            }
            if (wn5.SelectedIndex == 1)    // СВД Модифицированный       
            {
                s21 = "wpn_svd_m1, ";
            }
            // =============================================================
            if (wn6.SelectedIndex == 0)    // Не использовать        
            {
                s22 = "";
            }
            if (wn6.SelectedIndex == 1)    // ТОЗ-34 (Был вырезан из игры)     
            {
                s22 = "mp_wpn_toz34, ";
            }
            if (wn6.SelectedIndex == 2)    // Чейзер
            {
                s22 = "wpn_winchester_m1, ";
            }
            if (wn6.SelectedIndex == 3)    // СПСА-12
            {
                s22 = "wpn_spas12_m1, ";
            }
            if (wn6.SelectedIndex == 4)    // Весь набор
            {
                s22 = "mp_wpn_toz34, wpn_winchester_m1, wpn_spas12_m1, ";
            }
            // =============================================================
            if (wn7.SelectedIndex == 0)    // Не использовать        
            {
                s23 = "";
            }
            if (wn7.SelectedIndex == 1)    // РГ6 (Модифированный)     
            {
                s23 = "wpn_rg6_m1, ";
            }
            // =============================================================
            if (wn13.SelectedIndex == 0)    // Не использовать        
            {
                s24 = "";
            }
            if (wn13.SelectedIndex == 1)    // Огненный шар (светится в темноте)
            {
                s24 = "af_fireball, ";
            }
            if (wn13.SelectedIndex == 2)    // Колобок (Оставляет красивые следы за игроками)
            {
                s24 = "af_fuzz_kolobok, ";
            }
            if (wn13.SelectedIndex == 3)    // Слизь     
            {
                s24 = "af_ameba_slime, ";
            }
            if (wn13.SelectedIndex == 4)    // Слизняк     
            {
                s24 = "af_ameba_slug, ";
            }
            if (wn13.SelectedIndex == 5)    // Слюда   
            {
                s24 = "af_ameba_mica, ";
            }
            if (wn13.SelectedIndex == 6)    // Батарейка     
            {
                s24 = "af_dummy_battery, ";
            }
            if (wn13.SelectedIndex == 7)    // Лунный Свет
            {
                s24 = "af_electra_moonlight, ";
            }
            if (wn13.SelectedIndex == 8)    // Пружинка
            {
                s24 = "af_dummy_dummy, ";
            }
            if (wn13.SelectedIndex == 9)    // Пружинка
            {
                s24 = "af_electra_flash, ";
            }
            if (wn13.SelectedIndex == 10)    // Весь набор    
            {
                s24 = "af_fireball, af_fuzz_kolobok, af_ameba_slime,af_ameba_slug, af_ameba_mica, af_dummy_battery, af_electra_moonlight, af_dummy_dummy, ";
            }
            // =============================================================
            if (wn8.SelectedIndex == 0)    // Не использовать        
            {
                s25 = "";
            }
            if (wn8.SelectedIndex == 1)    // РГ6 (Модифированный)     
            {
                s25 = "mp_af_electra_flash, ";
            }
            // =============================================================
            // ShopTeamSelectIndex                               Export List
            // =============================================================
            if (dm1.SelectedIndex == 0)
            {
                Result = "mp_wpn_knife, mp_wpn_pb;";
            }
            if (dm1.SelectedIndex == 1)
            {
                Result = "mp_wpn_knife, mp_wpn_pb, mp_device_torch;";
            }
            if (dm1.SelectedIndex == 2)
            {
                Result = "mp_wpn_knife, mp_wpn_pb, mp_device_torch, mp_medkit;";
            }
            if (dm1.SelectedIndex == 3)
            {
                Result = "mp_wpn_knife, mp_wpn_pb, mp_device_torch, mp_medkit, mp_antirad;";
            }
            if (dm1.SelectedIndex == 4)
            {
                Result = "mp_wpn_knife, mp_wpn_pb, mp_device_torch, mp_medkit, mp_antirad, mp_detector_advances, mp_grenade_rgd5, mp_grenade_f1, mp_grenade_gd-05;";
            }

            if (dm1.SelectedIndex == 5) // стандартный набор
            {
                Result = sh1 + sh2 + sh3 + sh4 + sh5 + sh6 + sh7 + sh8 + ";";
            }
            if (dm1.SelectedIndex == 6) // только новый набор
            {
                Result = s1 + s2 + s3 + s4 + s5 + s6 + s7 + s8 + s9 + s10 + s11 + s12 + s13 + s14 + s15 + s16 + s17 + s18 + s19 + s20 + s21 + s22 + s23 + s24 + s25 + ";";
            }
            if (dm1.SelectedIndex == 7) // Стандартный + новый набор
            {
                Result = s1 + s2 + s3 + s4 + s5 + s6 + s7 + s8 + s9 + s10 + s11 + s12 + s13 + s14 + s15 + s16 + s17 + s18 + s19 + s20 + s21 + s22 + s23 + s24 + s25 + sh1 + sh2 + sh3 + sh4 + sh5 + sh6 + sh7 + sh8 + ";";
            }
        }

        /*
        id
        5 Свой набор из стандартного снаряжения
        6 Использовать снаряжение из одиночной игры
        7 Использовать снаряжение из одиночной игры и стандартное из сетевой игры.*/

        private void dm1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dm1.SelectedIndex == 5 || dm1.SelectedIndex == 7)
            {
                dm2.Enabled = true;
                dm3.Enabled = true;
                dm4.Enabled = true;
                dm5.Enabled = true;
                dm6.Enabled = true;
                dm7.Enabled = true;
                dm8.Enabled = true;
                dm9.Enabled = true;
            }
            else
            {
                dm2.Enabled = false;
                dm3.Enabled = false;
                dm4.Enabled = false;
                dm5.Enabled = false;
                dm6.Enabled = false;
                dm7.Enabled = false;
                dm8.Enabled = false;
                dm9.Enabled = false;
            }

            if (dm1.SelectedIndex == 6 || dm1.SelectedIndex == 7)
            {
                wn1.Enabled = true;
                wn2.Enabled = true;
                wn3.Enabled = true;
                wn4.Enabled = true;
                wn5.Enabled = true;
                wn6.Enabled = true;
                wn7.Enabled = true;
                wn8.Enabled = true;
                wn9.Enabled = true;
                wn10.Enabled = true;
                wn11.Enabled = true;
                wn12.Enabled = true;
                wn13.Enabled = true;
                wn14.Enabled = true;
                wn15.Enabled = true;
                wn16.Enabled = true;
                wn17.Enabled = true;
                wn18.Enabled = true;
                wn19.Enabled = true;
                wn20.Enabled = true;
                wn21.Enabled = true;
                wn22.Enabled = true;
                wn23.Enabled = true;
                wn24.Enabled = true;
                wn25.Enabled = true;
            }
            else
            {
                wn1.Enabled = false;
                wn2.Enabled = false;
                wn3.Enabled = false;
                wn4.Enabled = false;
                wn5.Enabled = false;
                wn6.Enabled = false;
                wn7.Enabled = false;
                wn8.Enabled = false;
                wn9.Enabled = false;
                wn10.Enabled = false;
                wn11.Enabled = false;
                wn12.Enabled = false;
                wn13.Enabled = false;
                wn14.Enabled = false;
                wn15.Enabled = false;
                wn16.Enabled = false;
                wn17.Enabled = false;
                wn18.Enabled = false;
                wn19.Enabled = false;
                wn20.Enabled = false;
                wn21.Enabled = false;
                wn22.Enabled = false;
                wn23.Enabled = false;
                wn24.Enabled = false;
                wn25.Enabled = false;
            }
            
            // =================================================================
            // Список уникального снаряжения                         IMPORT LIST
            // =================================================================
            if (dm1.SelectedIndex == 6 || dm1.SelectedIndex == 7)
            {
                create_remote_weapon =
                // Снаряжение
                "wpn_abakan_m1 = 0;" + Environment.NewLine +        // АС-96/2
                "wpn_ak74_m1 = 0;" + Environment.NewLine +          // АКМ-74/2
                "wpn_ak74u_m1 = 0;" + Environment.NewLine +         // AKM-74/2U
                "wpn_colt_m1 = 0;" + Environment.NewLine +          // Кольт пистолет
                "wpn_eagle_m1 = 0;" + Environment.NewLine +         // Черный ястреб
                "wpn_fort_m1 = 0;" + Environment.NewLine +          // Фора
                "wpn_groza_m1 = 0;" + Environment.NewLine +         // Гром С14
                "wpn_l85_m2 = 0;" + Environment.NewLine +           // Ил86
                "wpn_lr300_m1 = 0;" + Environment.NewLine +         // ТРС301
                "wpn_mp5_m2 = 0;" + Environment.NewLine +           // Гадюка mp5_m2
                "wpn_sig_m1 = 0;" + Environment.NewLine +           // Сиг
                "wpn_spas12_m1 = 0;" + Environment.NewLine +        // СПСА-14 Дробовик
                "wpn_svd_m1 = 0;" + Environment.NewLine +           // СВД 
                "wpn_val_m1 = 0;" + Environment.NewLine +           // ВЛА Автомат
                "wpn_walther_m1 = 0;" + Environment.NewLine +       // Волкер-П9м Пистолет
                "wpn_winchester_m1 = 0;" + Environment.NewLine +    // Чейзер-13 Дробовик
                "mp_wpn_toz34 = 0;" + Environment.NewLine +         // ТОЗ-34 Был вырезан
                "wpn_rg6_m1 = 0;" + Environment.NewLine +           // РГ6 Подствольный гранатомет
                "mp_wpn_hpsa = 0;" + Environment.NewLine +          // ХПСС-1м Был вырезан
                "mp_wpn_beretta = 0;" + Environment.NewLine +       // Марта Был вырезан
                // Разное
                "medkit = 0;" + Environment.NewLine +               // Аптечка обычная (Красная)
                "medkit_army= 0;" + Environment.NewLine +           // Армейская аптечка (Синяя)
                "medkit_scientic = 0;" + Environment.NewLine +      // Научная аптечка (Желтая)
                "antirad = 0;" + Environment.NewLine +              // Антирад
                "bandage = 0;" + Environment.NewLine +              // Бинт
                "bread = 0;" + Environment.NewLine +                // Хлеб
                "energy_drink = 0;" + Environment.NewLine +         // Энергетик
                "vodka = 0;" + Environment.NewLine +                // Водка
                "conserva = 0;" + Environment.NewLine +             // Консерва
                "kolbasa = 0;" + Environment.NewLine +              // Колбаса
                "wpn_addon_scope = 0;" + Environment.NewLine +      // Прицел длинный
                "wpn_addon_scope_susat = 0;" + Environment.NewLine +// Прицел короткий 
                "mp_players_rukzak = 0;" + Environment.NewLine +    // Рюкзак
                "detector_simple = 0;" + Environment.NewLine +      // Детектор
                // Артефакты
                "af_ameba_slime = 0;" + Environment.NewLine +
                "af_ameba_slug = 0;" + Environment.NewLine +
                "af_ameba_mica = 0;" + Environment.NewLine +
                "af_dummy_battery = 0;" + Environment.NewLine +
                "af_dummy_dummy = 0;" + Environment.NewLine +
                "af_dummy_glassbeads = 0;" + Environment.NewLine +
                "af_dummy_pellicle = 0;" + Environment.NewLine +
                "af_dummy_spring = 0;" + Environment.NewLine +
                "mp_af_electra_flash = 0;" + Environment.NewLine +  // Можно активировать
                "af_electra_moonlight = 0;" + Environment.NewLine +
                "af_electra_sparkler = 0;" + Environment.NewLine +
                "af_electra_flash = 0;" + Environment.NewLine +     // Аналог артефакта mp_af_electra_flash, но только без активации.
                "af_gold_fish = 0;" + Environment.NewLine +
                "af_gravi = 0;" + Environment.NewLine +
                "af_vyvert = 0;" + Environment.NewLine +
                "af_blood = 0;" + Environment.NewLine +
                "af_mincer_meat = 0;" + Environment.NewLine +
                "af_soul = 0;" + Environment.NewLine +
                "af_cristall_flower = 0;" + Environment.NewLine +
                "af_medusa = 0;" + Environment.NewLine +
                "af_night_star = 0;" + Environment.NewLine +
                "af_rusty_kristall = 0;" + Environment.NewLine +
                "af_rusty_sea-urchin = 0;" + Environment.NewLine +
                "af_rusty_thorn = 0;" + Environment.NewLine +
                "af_cristall = 0;" + Environment.NewLine +
                "af_drops = 0;" + Environment.NewLine +
                "af_fireball = 0;" + Environment.NewLine +
                "af_fuzz_kolobok = 0;" + Environment.NewLine +          
                // Патроны
                "ammo_5.45x39_fmj = 0;" + Environment.NewLine +                  
                "ammo_11.43x23_fmj = 0;" + Environment.NewLine +
                "ammo_9x39_sp5 = 0;" + Environment.NewLine +
                "ammo_9x18_fmj = 0;" + Environment.NewLine +
                "ammo_5.56x45_ss190 = 0;" + Environment.NewLine +
                "ammo_9x19_fmj = 0;" + Environment.NewLine +
                "ammo_5.56x45_ss190 = 0;" + Environment.NewLine +
                "ammo_12x70_buck = 0;" + Environment.NewLine +
                "ammo_7.62x54_7h1 = 0;" + Environment.NewLine +
                "ammo_9x39_pab9 = 0;" + Environment.NewLine+
                "ammo_m209 = 0;" + Environment.NewLine;
            }
            else
            {
                create_remote_weapon = "";
            }
        }
        // ===============================================================================================================================
        // SPAWN ANOMALY GAMEDATA EDITOR
        // ===============================================================================================================================
        string TypeAnomalySet, AnomalyRadius, AnomalyPower;

        private void btnEditorVisualDelete_Click(object sender, EventArgs e)
        {
            if (File.Exists(@"gamedata\config\misc\artefacts.ltx"))
            {
                try
                {
                    File.Delete(@"gamedata\config\misc\artefacts.ltx");
                    MessageBox.Show("Настройки успешно удалены", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка.\nПричина: " + ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Настройки не были найдены.", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void ArtSpawnAnomalySet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ArtSpawnAnomalySet.SelectedIndex == 0) // Электра
            {
                gui_description.Text = "Описание: При активации артефакта образует стандартную аномалию \"Электра\"\nРадиус действия: 3 метра.\nВремя действия: 1 минута.";
            }
            else if (ArtSpawnAnomalySet.SelectedIndex == 1) // Холодец
            {
                gui_description.Text = "Описание: При активации артефакта образует кислотную аномалию \"Холодец\". Обжигает всех кто находится в ее зоне.\nРадиус действия: 3 метра.\nВремя действия: 1 минута.";
            }
            else if (ArtSpawnAnomalySet.SelectedIndex == 2) // Жарка
            {
                gui_description.Text = "Описание: При активации артефакта образует огненную струю \"Жарка\". Уничтожает всех кто попал в ее зону.\nРадиус действия: 3 метра.\nВремя действия: 1 минута.";
            }
            else if (ArtSpawnAnomalySet.SelectedIndex == 3) // Мясорубка
            {
                gui_description.Text = "Описание: При активации артефакта образует затягивающую в себя аномалию \"Мясорубка\". Наносит большой урон тем кто не смог выбраться из нее.\nРадиус действия: 5 метров.\nВремя действия: 1 минута.";
            }
            else if (ArtSpawnAnomalySet.SelectedIndex == 4) // Карусель
            {
                gui_description.Text = "Описание: При активации артефакта образует карусель \"Карусель\". Наносит урон тем кто прошел сквозь нее\nРадиус действия: 5 метров.\nВремя действия: 1 минута.";
            }
            else if (ArtSpawnAnomalySet.SelectedIndex == 5) // Костер
            {
                gui_description.Text = "Описание: При активации артефакта образует \"Костер\". Обжигает всех кто зашел в него.\nРадиус действия: 1 метр.\nВремя действия: 1 минута.";
            }
            else if (ArtSpawnAnomalySet.SelectedIndex == 6) // Пламя
            {
                gui_description.Text = "Описание: При активации артефакта образует \"Пламя\". Обжигает всех кто зашел в него.\nРадиус действия: 2 метра.\nВремя действия: 1 минута.";
            }
            else if (ArtSpawnAnomalySet.SelectedIndex == 7) // Пузырь
            {
                gui_description.Text = "Описание: При активации артефакта образует шаровую аномалию \"Пузырь\". Наносит большой урон тем кто прошел сквозь нее.\nРадиус действия: 5 метров.\nВремя действия: 1 минута.";
            }
            else if (ArtSpawnAnomalySet.SelectedIndex == 8) // Телепорт
            {
                gui_description.Text = "Описание: При активации артефакта образует \"Телепорт\". Не наносит урон - является только декоративным элементом.\nРадиус действия: 3 метра.\nВремя действия: 1 минута.";
            }
            else if (ArtSpawnAnomalySet.SelectedIndex == 9) // Радиация
            {
                gui_description.Text = "Описание: При активации артефакта образует аномалию \"Радиация\". Заражает быстро радиацией всех кто попал в эту территорию.\nРадиус действия: 10 метров.\nВремя действия: 1 минута.";
            }
            else if (ArtSpawnAnomalySet.SelectedIndex == 10) // Монолит
            {
                gui_description.Text = "Описание: При активации артефакта образует радиоактивное облако \"Монолит\". Заражает радиацией всех кто находился в этом периметре образуя яркое облако.\nРадиус действия: 10 метров.\nВремя действия: 1 минута.";
            }
            else if (ArtSpawnAnomalySet.SelectedIndex == 11) // Мясорубка
            {
                gui_description.Text = "Описание: При активации артефакта образует затягивающую в себя аномалию \"Мясорубка\". Наносит большой урон тем кто не смог выбраться из нее.\nРадиус действия: 5 метров.\nВремя действия: 1 минута.";
            }
            else if (ArtSpawnAnomalySet.SelectedIndex == 12) // Мина
            {
                gui_description.Text = "Описание: При активации артефакта образует объект \"Мина\". Создает на карте мину игрокам находящихся в этот момент детонации моментально наносится очень сильный урон а добытые фраги прибавляются активировавшему артефакт\nРадиус действия взрыва: 5 метров.\nКоличество: 1 шт | Время действия: 1 минута.";
            }
            else if (ArtSpawnAnomalySet.SelectedIndex == 13) // Электрический шторм
            {
                gui_description.Text = "Описание: При активации артефакта образует накопительную аномалию \"ЭМИ (Электромагнитный импульс)\". Проходящие игроки вызывают накопление заряда после которого вызывается мощных электро-хлопок нанося урон всем в ее зоне\nРадиус действия: 15 метров.\nВремя действия: 1 минута.";
            }
        }

        private void btnEditorVisualWrite_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(@"gamedata\config\misc"))
            {
                Directory.CreateDirectory(@"gamedata\config\misc\");
            }
            // =============================================
            if (ArtSpawnAnomalySet.SelectedIndex == 0) // Электра
            {
                AnomalyRadius = "3";
                AnomalyPower = "100.0";
                TypeAnomalySet = "zone_witches_galantine";
            }
            else if (ArtSpawnAnomalySet.SelectedIndex == 1) // Холодец
            {
                AnomalyRadius = "3";
                AnomalyPower = "100.0";
                TypeAnomalySet = "zone_buzz";
            }
            else if (ArtSpawnAnomalySet.SelectedIndex == 2) // Жарка
            {
                AnomalyRadius = "3";
                AnomalyPower = "100.0";
                TypeAnomalySet = "zone_zharka_static";
            }
            else if (ArtSpawnAnomalySet.SelectedIndex == 3) // Мясорубка
            {
                AnomalyRadius = "5";
                AnomalyPower = "100.0";
                TypeAnomalySet = "zone_gravi_zone";
            }
            else if (ArtSpawnAnomalySet.SelectedIndex == 4) // Карусель
            {
                AnomalyRadius = "5";
                AnomalyPower = "100.0";
                TypeAnomalySet = "zone_mosquito_bald";
            }
            else if (ArtSpawnAnomalySet.SelectedIndex == 5) // Костер
            {
                AnomalyRadius = "1";
                AnomalyPower = "50.0";
                TypeAnomalySet = "zone_flame_small";
            }
            else if (ArtSpawnAnomalySet.SelectedIndex == 6) // Пламя
            {
                AnomalyRadius = "2";
                AnomalyPower = "100.0";
                TypeAnomalySet = "zone_flame";
            }
            else if (ArtSpawnAnomalySet.SelectedIndex == 7) // Пузырь
            {
                AnomalyRadius = "5";
                AnomalyPower = "100.0";
                TypeAnomalySet = "zone_sarcofag";
            }
            else if (ArtSpawnAnomalySet.SelectedIndex == 8) // Телепорт
            {
                AnomalyRadius = "3";
                AnomalyPower = "100.0";
                TypeAnomalySet = "zone_teleport";
            }
            else if (ArtSpawnAnomalySet.SelectedIndex == 9) // Радиация
            {
                AnomalyRadius = "10";
                AnomalyPower = "100.0";
                TypeAnomalySet = "zone_radioactive";
            }
            else if (ArtSpawnAnomalySet.SelectedIndex == 10) // Монолит
            {
                AnomalyRadius = "15";
                AnomalyPower = "100.0";
                TypeAnomalySet = "zone_monolith";
            }
            else if (ArtSpawnAnomalySet.SelectedIndex == 11) // Мясорубка
            {
                AnomalyRadius = "5";
                AnomalyPower = "100.0";
                TypeAnomalySet = "zone_mincer";
            }
            else if (ArtSpawnAnomalySet.SelectedIndex == 12) // Мина
            {
                AnomalyRadius = "5";
                AnomalyPower = "100.0";
                TypeAnomalySet = "zone_mine_field"; // zone_mine_field zone_zhar
            }
            else if (ArtSpawnAnomalySet.SelectedIndex == 13) // Электрический шторм (ЭМИ)
            {
                AnomalyRadius = "15";
                AnomalyPower = "100.0";
                TypeAnomalySet = "zone_emi";
            }

            // =============================================
            // EXPORT CONFIG    ..\config\misc\artefacts.ltx
            // =============================================
            ArtefactAnomalyEditor("[artefact_spawn_zones]" + Environment.NewLine +
            // имя артефакта / Аномалия при активаций / радиус / сила аномалии
            "af_medusa = zone_mosquito_bald_weak, 2.0, 30.0" + Environment.NewLine +
            "af_cristall_flower = zone_mosquito_bald_weak, 3.0, 50.0" + Environment.NewLine +
            "af_night_star = zone_mosquito_bald_weak, 5.0, 100.0" + Environment.NewLine +
            "af_vyvert = zone_gravi_zone_weak, 2.0, 30.0" + Environment.NewLine +
            "af_gravi = zone_gravi_zone_weak, 3.0, 50.0" + Environment.NewLine +
            "af_gold_fish = zone_gravi_zone_weak, 5.0, 100.0" + Environment.NewLine +
            "af_cristall = torrid_zone, 3.0, 50.0" + Environment.NewLine +
            "af_blood = zone_mincer_weak, 2, 30.0" + Environment.NewLine +
            "af_mincer_meat = zone_mincer_weak, 3, 50.0" + Environment.NewLine +
            "af_soul = zone_mincer_weak, 5, 100.0" + Environment.NewLine +
            "af_electra_sparkler = zone_witches_galantine, 2, 30.0" + Environment.NewLine +
            "af_electra_flash = zone_witches_galantine, 3, 50.0" + Environment.NewLine +
            "af_electra_moonlight = zone_witches_galantine, 5, 100.0" + Environment.NewLine +
            // =============================================
            "mp_af_electra_flash = " + TypeAnomalySet + ", " + AnomalyRadius + ", " + AnomalyPower + Environment.NewLine + Environment.NewLine +     // наш артефакт
            // =============================================
            // Здесь в секциях должны быть двойные явные скобки !!! Иначе словим вылет
            "[af_activation_gravi]" + Environment.NewLine +
            "starting = 5.0, anomaly\\gravi_idle00, 0.5, 1.0, 1.5, 5.0, \"anomaly2\\artefact_gravi_blast_start\"" + "," + "\"idle\"" + Environment.NewLine +
            "flying = 2.0, anomaly\\gravi_idle01, 0.8, 1.0, 1.2, 3.0, \"anomaly2\\artefact\\artefact_gravi\"" + "," + "\"idle\"" + Environment.NewLine +
            "idle_before_spawning = 5.0, anomaly\\gravi_blowout5, 1.0, 1.0, 1, 0.0, \"anomaly2\\artefact_gravi_blast_finished\"" + "," + "\"idle\"" + Environment.NewLine +
            "spawning = 1, anomaly\\gravity_entrance, 0.9, 1.1, 1.01, 0.0, \"\"" + "," + "\"idle\"" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_activation_bold]" + Environment.NewLine +
            "starting = 5.0, anomaly\\gravi_idle00, 0.8, 1.5, 0.9, 5.0, \"anomaly2\\artefact_bold_blast_start\"" + "," + "\"idle\"" + Environment.NewLine +
            "flying	= 2.0, anomaly\\gravi_idle01, 0.8, 0.9, 0.8, 2.0, \"anomaly2\\artefact\\artefact_zharka\"" + "," + "\"idle\"" + Environment.NewLine +
            "idle_before_spawning =	2.0, anomaly\\gravity_entrance,1.0, 1.1, 1.0, 0.0, \"anomaly2\\artefact_bold_blast_finished\"" + "," + "\"idle\"" + Environment.NewLine +
            "spawning = 0.5, anomaly\\pux_blast, 1.0, 1.0, 0.5, 5.0, \"anomaly2\\gravi_anomaly_00\"" + "," + "\"idle\"" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_activation_mincer]" + Environment.NewLine +
            "starting = 5.0, anomaly\\gravi_idle00,	0.5, 1.0, 1.5, 5.0, \"anomaly2\\artefact_gravi_blast_start\"" + "," + "\"idle\"" + Environment.NewLine +
            "flying	= 2.0, anomaly\\gravi_idle01, 0.8, 1.0, 1.2, 3.0, \"anomaly2\\artefact\\artefact_mincer\"" + "," + "\"idle\"" + Environment.NewLine +
            "idle_before_spawning = 5.0, anomaly\\gravi_blowout2, 1.0, 1.0, 1.0, 0.0, \"anomaly2\\artefact_gravi_blast_finished\"" + "," + "\"idle\"" + Environment.NewLine +
            "spawning = 0.0, \"\"" + "," + " 1.0, 1.0, 0.5, 0.0, \"\"" + "," + "\"idle\"" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_activation_electra]" + Environment.NewLine +
            "starting = 5.0, anomaly\\bfuzz_blowout, 0.8, 1.0, 1.5, 5.0, \"anomaly2\\artefact_electra_blast_start\"" + "," + "\"idle\"" + Environment.NewLine +
            "flying	 = 2.0, anomaly\\electra_idle1, 1.0, 1.0, 1.0, 5.0, \"anomaly2\\artefact\\artefact_electra\"" + "," + "\"idle\"" + Environment.NewLine +
            "idle_before_spawning = 0.0, \"\"" + "," + "1.0, 1.0, 2.5, 0, \"\"" + "," + "\"idle\"" + Environment.NewLine +
            "spawning = 2, \"anomaly\\pux_blast\"" + "," + "1.0, 1.0, 1.5, 5.0, \"anomaly2\\artefact_electra_blast_finished\"" + "," + "\"idle\"" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_base]:identity_immunities" + Environment.NewLine +
            "hud = artefact_hud" + Environment.NewLine +
            "animation_slot	= 11" + Environment.NewLine +
            "allow_inertion	= true" + Environment.NewLine +
            ";slot = 10" + Environment.NewLine +
            "attach_angle_offset = 0,0,0" + Environment.NewLine +
            "attach_position_offset = -0.1,-0.1,0" + Environment.NewLine +
            "attach_bone_name = bip01_root" + Environment.NewLine +
            "inv_grid_width = 1" + Environment.NewLine +
            "inv_grid_height = 1" + Environment.NewLine +
            "belt = true" + Environment.NewLine +
            "af_actor_properties = on" + Environment.NewLine +
            "actor_properties = on" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[artefact_hud]" + Environment.NewLine +
            "orientation = 0, 0, 0" + Environment.NewLine +
            "position = 0, 0, 0" + Environment.NewLine +
            "visual = weapons\\artefact\\wpn_artefact_hud.ogf" + Environment.NewLine +
            "anim_idle = idle" + Environment.NewLine +
            "anim_idle_sprint = idle_sprint" + Environment.NewLine +
            "anim_hide = holster" + Environment.NewLine +
            "anim_show = draw" + Environment.NewLine +
            "anim_activate = activate" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_medusa]:af_base" + Environment.NewLine +
            "GroupControlSection = spawn_group" + Environment.NewLine +
            "$spawn	= \"artifacts\\moscito medusa\"" + Environment.NewLine +
            "$prefetch = 64" + Environment.NewLine +
            "cform = skeleton" + Environment.NewLine +
            "class = ARTEFACT" + Environment.NewLine +
            "visual = physics\\anomaly\\artefact_blackdrip.ogf" + Environment.NewLine +
            "description = enc_zone_artifact_af-medusa" + Environment.NewLine +
            "inv_name = af-medusa" + Environment.NewLine +
            "inv_name_short =" + Environment.NewLine +
            "inv_weight = 0.5" + Environment.NewLine +
            "inv_grid_x = 9" + Environment.NewLine +
            "inv_grid_y = 4" + Environment.NewLine +
            "cost = 1000" + Environment.NewLine +
            "jump_height = .5" + Environment.NewLine +
            "particles = anomaly2\\artefact\\artefact_gravi" + Environment.NewLine +
            "lights_enabled = false" + Environment.NewLine +
            "health_restore_speed = 0.0" + Environment.NewLine +
            "radiation_restore_speed = 0.0005" + Environment.NewLine +
            "satiety_restore_speed = 0.0" + Environment.NewLine +
            "power_restore_speed = 0.0" + Environment.NewLine +
            "bleeding_restore_speed = 0.0" + Environment.NewLine +
            "hit_absorbation_sect = af_medusa_absorbation" + Environment.NewLine +
            "artefact_activation_seq = af_activation_bold" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_medusa_absorbation]" + Environment.NewLine +
            "burn_immunity = 1.0" + Environment.NewLine +
            "strike_immunity = 1.0" + Environment.NewLine +
            "shock_immunity = 1.0" + Environment.NewLine +
            "wound_immunity = 1.0	" + Environment.NewLine +
            "radiation_immunity = 1.0" + Environment.NewLine +
            "telepatic_immunity = 1.0" + Environment.NewLine +
            "chemical_burn_immunity = 1.0" + Environment.NewLine +
            "explosion_immunity = 1.0" + Environment.NewLine +
            "fire_wound_immunity  = 0.98" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_cristall_flower]:af_base" + Environment.NewLine +
            "GroupControlSection = spawn_group" + Environment.NewLine +
            "$spawn = \"artifacts\\moscito flower\"" + Environment.NewLine +
            "$prefetch = 64" + Environment.NewLine +
            "cform	= skeleton" + Environment.NewLine +
            "class = ARTEFACT" + Environment.NewLine +
            "visual = physics\\anomaly\\artefact_blackdrip_2.ogf" + Environment.NewLine +
            "description = enc_zone_artifact_af-cristall-flower" + Environment.NewLine +
            "inv_name = af-cristall-flower" + Environment.NewLine +
            "inv_name_short =" + Environment.NewLine +
            "inv_weight = 0.5" + Environment.NewLine +
            "inv_grid_x = 11" + Environment.NewLine +
            "inv_grid_y = 4" + Environment.NewLine +
            "cost = 2500" + Environment.NewLine +
            "jump_height = .4" + Environment.NewLine +
            "particles = anomaly2\\artefact\\artefact_gravi" + Environment.NewLine +
            "lights_enabled = true;" + Environment.NewLine +
            "trail_light_color = 0.6,0.7,0.5" + Environment.NewLine +
            "trail_light_range = 0.5" + Environment.NewLine +
            "artefact_activation_seq = af_activation_bold" + Environment.NewLine +
            ";скорости увеличения(уменьшения)" + Environment.NewLine +
            "health_restore_speed = 0.0" + Environment.NewLine +
            "radiation_restore_speed = 0.0005" + Environment.NewLine +
            "satiety_restore_speed = 0.0" + Environment.NewLine +
            "power_restore_speed = 0.0" + Environment.NewLine +
            "bleeding_restore_speed = 0.0" + Environment.NewLine +
            "hit_absorbation_sect = af_cristall_flower_absorbation" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_cristall_flower_absorbation]" + Environment.NewLine +
            "burn_immunity = 1.0; коэффициенты иммунитета" + Environment.NewLine +
            "strike_immunity = 1.0" + Environment.NewLine +
            "shock_immunity = 1.0" + Environment.NewLine +
            "wound_immunity = 1.0" + Environment.NewLine +
            "radiation_immunity = 1.0" + Environment.NewLine +
            "telepatic_immunity = 1.0" + Environment.NewLine +
            "chemical_burn_immunity = 1.0" + Environment.NewLine +
            "explosion_immunity = 1.0" + Environment.NewLine +
            "fire_wound_immunity  = 0.97" + Environment.NewLine +
            // =============================================
            "[af_night_star]:af_base" + Environment.NewLine +
            "GroupControlSection = spawn_group" + Environment.NewLine +
            "$spawn = \"artifacts\\moscito star\"" + Environment.NewLine +
            "$prefetch = 64" + Environment.NewLine +
            "cform = skeleton" + Environment.NewLine +
            "class = ARTEFACT" + Environment.NewLine +
            "visual = physics\\anomaly\\artefact_blackdrip_1.ogf" + Environment.NewLine +
            "description = enc_zone_artifact_af-night-star" + Environment.NewLine +
            "inv_name = af-night-star" + Environment.NewLine +
            "inv_name_short =" + Environment.NewLine +
            "inv_weight = 0.5" + Environment.NewLine +
            "inv_grid_x = 10" + Environment.NewLine +
            "inv_grid_y = 4" + Environment.NewLine +
            "cost = 5000" + Environment.NewLine +
            "jump_height = .2" + Environment.NewLine +
            "particles = anomaly2\\artefact\\artefact_gravi" + Environment.NewLine +
            "lights_enabled = true;" + Environment.NewLine +
            "trail_light_color = 0.6,0.7,0.5" + Environment.NewLine +
            "trail_light_range = 3.0" + Environment.NewLine +
            "artefact_activation_seq = af_activation_bold" + Environment.NewLine +
            "health_restore_speed = 0.0" + Environment.NewLine +
            "radiation_restore_speed = 0.0005" + Environment.NewLine +
            "satiety_restore_speed = 0.0" + Environment.NewLine +
            "power_restore_speed = 0.0" + Environment.NewLine +
            "bleeding_restore_speed = 0.0" + Environment.NewLine +
            "hit_absorbation_sect = af_night_star_absorbation" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_night_star_absorbation]" + Environment.NewLine +
            "burn_immunity = 1.0; коэффициенты иммунитета" + Environment.NewLine +
            "strike_immunity = 1.0" + Environment.NewLine +
            "shock_immunity = 1.0" + Environment.NewLine +
            "wound_immunity = 1.0" + Environment.NewLine +
            "radiation_immunity = 1.0" + Environment.NewLine +
            "telepatic_immunity = 1.0" + Environment.NewLine +
            "chemical_burn_immunity = 1.0" + Environment.NewLine +
            "explosion_immunity = 1.0" + Environment.NewLine +
            "fire_wound_immunity  = 0.95" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_vyvert]:af_base" + Environment.NewLine +
            "GroupControlSection = spawn_group" + Environment.NewLine +
            "$spawn	= \"artifacts\\gravy vyvert\"" + Environment.NewLine +
            "$prefetch = 64" + Environment.NewLine +
            "cform = skeleton" + Environment.NewLine +
            "class = ARTEFACT" + Environment.NewLine +
            "visual = physics\\anomaly\\artefact_gravy1.ogf" + Environment.NewLine +
            "description = enc_zone_artifact_af-vyvert" + Environment.NewLine +
            "inv_name = af-vyvert" + Environment.NewLine +
            "inv_name_short =" + Environment.NewLine +
            "inv_weight = 0.5" + Environment.NewLine +
            "inv_grid_x = 14" + Environment.NewLine +
            "inv_grid_y	= 0" + Environment.NewLine +
            "cost = 1000" + Environment.NewLine +
            "jump_height = .3" + Environment.NewLine +
            "particles = anomaly2\\artefact\\artefact_gravi" + Environment.NewLine +
            "lights_enabled = false;" + Environment.NewLine +
            "artefact_activation_seq = af_activation_gravi" + Environment.NewLine +
            "health_restore_speed = 0.0" + Environment.NewLine +
            "radiation_restore_speed = 0.0005" + Environment.NewLine +
            "satiety_restore_speed = 0.0" + Environment.NewLine +
            "power_restore_speed = 0.0" + Environment.NewLine +
            "bleeding_restore_speed = 0.0" + Environment.NewLine +
            "hit_absorbation_sect = af_vyvert_absorbation" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_vyvert_absorbation]" + Environment.NewLine +
            "burn_immunity = 1.0; коэффициенты иммунитета" + Environment.NewLine +
            "strike_immunity = 1.0" + Environment.NewLine +
            "shock_immunity = 1.0" + Environment.NewLine +
            "wound_immunity = 0.98" + Environment.NewLine +
            "radiation_immunity = 1.0" + Environment.NewLine +
            "telepatic_immunity = 1.0" + Environment.NewLine +
            "chemical_burn_immunity = 1.0" + Environment.NewLine +
            "explosion_immunity = 1.0" + Environment.NewLine +
            "fire_wound_immunity  = 1.0" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_gravi]:af_base" + Environment.NewLine +
            "GroupControlSection = spawn_group" + Environment.NewLine +
            "$spawn	= \"artifacts\\gravy gravi\"" + Environment.NewLine +
            "$prefetch = 64" + Environment.NewLine +
            "cform = skeleton" + Environment.NewLine +
            "class = ARTEFACT" + Environment.NewLine +
            "visual = physics\\anomaly\\artefact_gravy2.ogf" + Environment.NewLine +
            "description = enc_zone_artifact_af-gravi" + Environment.NewLine +
            "inv_name = af-gravi" + Environment.NewLine +
            "inv_name_short =" + Environment.NewLine +
            "inv_weight = 0.5" + Environment.NewLine +
            "inv_grid_x = 15" + Environment.NewLine +
            "inv_grid_y	= 0" + Environment.NewLine +
            "cost = 2500" + Environment.NewLine +
            "jump_height = .3" + Environment.NewLine +
            "particles = anomaly2\\artefact\\artefact_gravi" + Environment.NewLine +
            "lights_enabled = true;" + Environment.NewLine +
            "trail_light_color = 0.6,0.7,0.5" + Environment.NewLine +
            "trail_light_range = 0.5" + Environment.NewLine +
            "artefact_activation_seq = af_activation_gravi" + Environment.NewLine +
            "health_restore_speed = 0.0" + Environment.NewLine +
            "radiation_restore_speed = 0.0005" + Environment.NewLine +
            "satiety_restore_speed = 0.0" + Environment.NewLine +
            "power_restore_speed = 0.0" + Environment.NewLine +
            "bleeding_restore_speed = 0.0" + Environment.NewLine +
            "hit_absorbation_sect = af_gravi_absorbation" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_gravi_absorbation]" + Environment.NewLine +
            "burn_immunity = 1.0; коэффициенты иммунитета" + Environment.NewLine +
            "strike_immunity = 1.0" + Environment.NewLine +
            "shock_immunity = 1.0" + Environment.NewLine +
            "wound_immunity = 0.97" + Environment.NewLine +
            "radiation_immunity = 1.0" + Environment.NewLine +
            "telepatic_immunity = 1.0" + Environment.NewLine +
            "chemical_burn_immunity = 1.0" + Environment.NewLine +
            "explosion_immunity = 1.0" + Environment.NewLine +
            "fire_wound_immunity  = 1.0" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_gold_fish]:af_base" + Environment.NewLine +
            "GroupControlSection = spawn_group" + Environment.NewLine +
            "$spawn	= \"artifacts\\gravy fish\"" + Environment.NewLine +
            "$prefetch = 64" + Environment.NewLine +
            "cform = skeleton" + Environment.NewLine +
            "class = ARTEFACT" + Environment.NewLine +
            "visual = physics\\anomaly\\artefact_gravy3.ogf" + Environment.NewLine +
            "description = enc_zone_artifact_af-gold-fish" + Environment.NewLine +
            "inv_name = af-gold-fish" + Environment.NewLine +
            "inv_name_short =" + Environment.NewLine +
            "inv_weight = 0.5" + Environment.NewLine +
            "inv_grid_x = 16" + Environment.NewLine +
            "inv_grid_y = 0" + Environment.NewLine +
            "cost = 5000" + Environment.NewLine +
            "jump_height = .2" + Environment.NewLine +
            "particles = anomaly2\\artefact\\artefact_gravi" + Environment.NewLine +
            "lights_enabled = true;" + Environment.NewLine +
            "trail_light_color = 0.6,0.7,0.5" + Environment.NewLine +
            "trail_light_range = 2.0" + Environment.NewLine +
            "artefact_activation_seq = af_activation_gravi" + Environment.NewLine +
            "health_restore_speed = 0.0	" + Environment.NewLine +
            "radiation_restore_speed = 0.0005" + Environment.NewLine +
            "satiety_restore_speed = 0.0" + Environment.NewLine +
            "power_restore_speed = 0.0" + Environment.NewLine +
            "bleeding_restore_speed = 0.0" + Environment.NewLine +
            "hit_absorbation_sect = af_gold_fish_absorbation" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_gold_fish_absorbation]" + Environment.NewLine +
            "burn_immunity = 1.0; коэффициенты иммунитета" + Environment.NewLine +
            "strike_immunity = 1.0" + Environment.NewLine +
            "shock_immunity = 1.0" + Environment.NewLine +
            "wound_immunity = 0.95" + Environment.NewLine +
            "radiation_immunity = 1.0" + Environment.NewLine +
            "telepatic_immunity = 1.0" + Environment.NewLine +
            "chemical_burn_immunity = 1.0" + Environment.NewLine +
            "explosion_immunity = 1.0" + Environment.NewLine +
            "fire_wound_immunity  = 1.0" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_drops]:af_base" + Environment.NewLine +
            "GroupControlSection = spawn_group" + Environment.NewLine +
            "$spawn = \"artifacts\\zharka drops\"" + Environment.NewLine +
            "$npc = on" + Environment.NewLine +
            ";$prefetch = 3" + Environment.NewLine +
            "cform = skeleton" + Environment.NewLine +
            "class = ARTEFACT" + Environment.NewLine +
            "visual = physics\\anomaly\\artefact_kaply.ogf" + Environment.NewLine +
            "description = enc_zone_artifact_af-drops" + Environment.NewLine +
            "inv_name = af-drops" + Environment.NewLine +
            "inv_name_short =" + Environment.NewLine +
            "inv_weight = 0.5" + Environment.NewLine +
            "inv_grid_x	= 12" + Environment.NewLine +
            "inv_grid_y	= 4" + Environment.NewLine +
            "cost = 1000" + Environment.NewLine +
            "jump_height = .1" + Environment.NewLine +
            "particles = anomaly2\\artefact\\artefact_zharka" + Environment.NewLine +
            "lights_enabled = true" + Environment.NewLine +
            "trail_light_color = 0.6,0.7,0.5" + Environment.NewLine +
            "trail_light_range = 0.5" + Environment.NewLine +
            "health_restore_speed = 0.0" + Environment.NewLine +
            "radiation_restore_speed = -0.001" + Environment.NewLine +
            "satiety_restore_speed = 0.0" + Environment.NewLine +
            "power_restore_speed = -0.001" + Environment.NewLine +
            "bleeding_restore_speed = 0.0" + Environment.NewLine +
            "hit_absorbation_sect = af_drops_absorbation" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_drops_absorbation]" + Environment.NewLine +
            "burn_immunity = 1.0; коэффициенты иммунитета" + Environment.NewLine +
            "strike_immunity = 1.0" + Environment.NewLine +
            "shock_immunity = 1.0" + Environment.NewLine +
            "wound_immunity = 1.0" + Environment.NewLine +
            "radiation_immunity = 1.0" + Environment.NewLine +
            "telepatic_immunity = 1.0" + Environment.NewLine +
            "chemical_burn_immunity = 1.0" + Environment.NewLine +
            "explosion_immunity = 1.0" + Environment.NewLine +
            "fire_wound_immunity  = 1.0" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_fireball]:af_base" + Environment.NewLine +
            "GroupControlSection = spawn_group" + Environment.NewLine +
            "$spawn = \"artifacts\\zharka fireball\"" + Environment.NewLine +
            "$npc = on;" + Environment.NewLine +
            ";$prefetch = 3" + Environment.NewLine +
            "cform = skeleton" + Environment.NewLine +
            "class = ARTEFACT" + Environment.NewLine +
            "visual = physics\\anomaly\\artefact_fire.ogf" + Environment.NewLine +
            "description = enc_zone_artifact_af-fireball" + Environment.NewLine +
            "inv_name = af-fireball" + Environment.NewLine +
            "inv_name_short =" + Environment.NewLine +
            "inv_weight = 0.5" + Environment.NewLine +
            "inv_grid_x	= 13" + Environment.NewLine +
            "inv_grid_y = 4" + Environment.NewLine +
            "cost = 2500" + Environment.NewLine +
            "impulse_threshold = 10" + Environment.NewLine +
            "radius	= 10" + Environment.NewLine +
            "strike_impulse	= 20" + Environment.NewLine +
            "particles = anomaly2\\artefact\\artefact_zharka" + Environment.NewLine +
            "lights_enabled = true;" + Environment.NewLine +
            "lights_enabled = true;" + Environment.NewLine +
            "trail_light_color = 0.9,0.5,0.5" + Environment.NewLine +
            "trail_light_range = 3.0" + Environment.NewLine +
            "health_restore_speed = 0.0" + Environment.NewLine +
            "radiation_restore_speed = -0.002" + Environment.NewLine +
            "satiety_restore_speed = 0.0" + Environment.NewLine +
            "power_restore_speed = -0.001" + Environment.NewLine +
            "bleeding_restore_speed = 0.0" + Environment.NewLine +
            "hit_absorbation_sect = af_fireball_absorbation" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_fireball_absorbation]" + Environment.NewLine +
            "burn_immunity = 1.0; коэффициенты иммунитета" + Environment.NewLine +
            "strike_immunity = 1.0" + Environment.NewLine +
            "shock_immunity = 1.0" + Environment.NewLine +
            "wound_immunity = 1.0" + Environment.NewLine +
            "radiation_immunity = 1.0" + Environment.NewLine +
            "telepatic_immunity = 1.0" + Environment.NewLine +
            "chemical_burn_immunity = 1.0" + Environment.NewLine +
            "explosion_immunity = 1.0" + Environment.NewLine +
            "fire_wound_immunity  = 1.0" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_cristall]:af_base" + Environment.NewLine +
            "GroupControlSection = spawn_group" + Environment.NewLine +
            "$spawn = \"artifacts\\zharka cristall\"" + Environment.NewLine +
            "$npc = on;" + Environment.NewLine +
            ";$prefetch = 3" + Environment.NewLine +
            "cform = skeleton" + Environment.NewLine +
            "class = ARTEFACT" + Environment.NewLine +
            "visual = physics\\anomaly\\artefact_cristall.ogf" + Environment.NewLine +
            "description = enc_zone_artifact_af-cristall" + Environment.NewLine +
            "inv_name = af-cristall" + Environment.NewLine +
            "inv_name_short =" + Environment.NewLine +
            "inv_weight = 0.5" + Environment.NewLine +
            "inv_grid_x	= 14" + Environment.NewLine +
            "inv_grid_y	= 4" + Environment.NewLine +
            "cost = 5000" + Environment.NewLine +
            "jump_height = .1" + Environment.NewLine +
            "particles = anomaly2\\artefact\\artefact_zharka" + Environment.NewLine +
            "lights_enabled = true;" + Environment.NewLine +
            "trail_light_color = 0.9,0.5,0.5" + Environment.NewLine +
            "trail_light_range = 3.0" + Environment.NewLine +
            "artefact_activation_seq = af_activation_mincer" + Environment.NewLine +
            "health_restore_speed = 0.0" + Environment.NewLine +
            "radiation_restore_speed = -0.003" + Environment.NewLine +
            "satiety_restore_speed = 0.0" + Environment.NewLine +
            "power_restore_speed = -0.001" + Environment.NewLine +
            "bleeding_restore_speed = 0.0" + Environment.NewLine +
            "hit_absorbation_sect = af_cristall_absorbation" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_cristall_absorbation]" + Environment.NewLine +
            "burn_immunity = 1.0;" + Environment.NewLine +
            "strike_immunity = 1.0" + Environment.NewLine +
            "shock_immunity = 1.0" + Environment.NewLine +
            "wound_immunity = 1.0" + Environment.NewLine +
            "radiation_immunity = 1.0" + Environment.NewLine +
            "telepatic_immunity = 1.0" + Environment.NewLine +
            "chemical_burn_immunity = 1.0" + Environment.NewLine +
            "explosion_immunity = 1.0" + Environment.NewLine +
            "fire_wound_immunity  = 1.0" + Environment.NewLine +
            // =============================================
            "[af_blood]:af_base" + Environment.NewLine +
            "GroupControlSection = spawn_group" + Environment.NewLine +
            "$spawn	= \"artifacts\\mincer blood\"" + Environment.NewLine +
            "$prefetch = 64" + Environment.NewLine +
            "cform = skeleton" + Environment.NewLine +
            "class = ARTEFACT" + Environment.NewLine +
            "visual = physics\\anomaly\\artefact_myasorubka1.ogf" + Environment.NewLine +
            "description = enc_zone_artifact_af-blood" + Environment.NewLine +
            "inv_name = af-blood" + Environment.NewLine +
            "inv_name_short =" + Environment.NewLine +
            "inv_weight = 0.5" + Environment.NewLine +
            "inv_grid_x	= 17" + Environment.NewLine +
            "inv_grid_y	= 0" + Environment.NewLine +
            "cost = 1000" + Environment.NewLine +
            "jump_height = .01" + Environment.NewLine +
            "particles = anomaly2\\artefact\\artefact_mincer" + Environment.NewLine +
            "lights_enabled = false;" + Environment.NewLine +
            "artefact_activation_seq = af_activation_mincer" + Environment.NewLine +
            "health_restore_speed = 0.0002" + Environment.NewLine +
            "radiation_restore_speed = 0.0" + Environment.NewLine +
            "satiety_restore_speed = 0.0" + Environment.NewLine +
            "power_restore_speed = 0.0" + Environment.NewLine +
            "bleeding_restore_speed = 0.0" + Environment.NewLine +
            "hit_absorbation_sect = af_blood_absorbation" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_blood_absorbation]" + Environment.NewLine +
            "burn_immunity = 1.0;" + Environment.NewLine +
            "strike_immunity = 1.1" + Environment.NewLine +
            "shock_immunity = 1.0" + Environment.NewLine +
            "wound_immunity = 1.1" + Environment.NewLine +
            "radiation_immunity = 1.0" + Environment.NewLine +
            "telepatic_immunity = 1.0" + Environment.NewLine +
            "chemical_burn_immunity = 1.0" + Environment.NewLine +
            "explosion_immunity = 1.0" + Environment.NewLine +
            "fire_wound_immunity  = 1.1" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_mincer_meat]:af_base" + Environment.NewLine +
            "GroupControlSection = spawn_group" + Environment.NewLine +
            "$spawn	= \"artifacts\\mincer meatloaf\"" + Environment.NewLine +
            "$prefetch = 64" + Environment.NewLine +
            "cform = skeleton" + Environment.NewLine +
            "class = ARTEFACT" + Environment.NewLine +
            "visual = physics\\anomaly\\artefact_myasorubka2.ogf" + Environment.NewLine +
            "description = enc_zone_artifact_af-mincer-meat" + Environment.NewLine +
            "inv_name = af-mincer-meat" + Environment.NewLine +
            "inv_name_short =" + Environment.NewLine +
            "inv_weight = 0.5" + Environment.NewLine +
            "inv_grid_x	= 18" + Environment.NewLine +
            "inv_grid_y	= 16" + Environment.NewLine +
            "cost = 2500" + Environment.NewLine +
            "jump_height = .1" + Environment.NewLine +
            "particles = anomaly2\\artefact\\artefact_mincer" + Environment.NewLine +
            "lights_enabled = false;" + Environment.NewLine +
            "artefact_activation_seq = af_activation_mincer" + Environment.NewLine +
            "health_restore_speed = 0.0004" + Environment.NewLine +
            "radiation_restore_speed = 0.0" + Environment.NewLine +
            "satiety_restore_speed = 0.0" + Environment.NewLine +
            "power_restore_speed = 0.0" + Environment.NewLine +
            "bleeding_restore_speed = 0.0" + Environment.NewLine +
            "hit_absorbation_sect = af_mincer_meat_absorbation" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_mincer_meat_absorbation]" + Environment.NewLine +
            "burn_immunity = 1.0;" + Environment.NewLine +
            "strike_immunity = 1.1" + Environment.NewLine +
            "shock_immunity = 1.0" + Environment.NewLine +
            "wound_immunity = 1.1" + Environment.NewLine +
            "radiation_immunity = 1.0" + Environment.NewLine +
            "telepatic_immunity = 1.0" + Environment.NewLine +
            "chemical_burn_immunity = 1.0" + Environment.NewLine +
            "explosion_immunity = 1.0" + Environment.NewLine +
            "fire_wound_immunity  = 1.1" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_soul]:af_base" + Environment.NewLine +
            "GroupControlSection = spawn_group" + Environment.NewLine +
            "$spawn	= \"artifacts\\mincer soul\"" + Environment.NewLine +
            "$prefetch = 64" + Environment.NewLine +
            "cform = skeleton" + Environment.NewLine +
            "class = ARTEFACT" + Environment.NewLine +
            "visual = physics\\anomaly\\artefact_myasorubka3.ogf" + Environment.NewLine +
            "description = enc_zone_artifact_af-soul" + Environment.NewLine +
            "inv_name = af-soul" + Environment.NewLine +
            "inv_name_short =" + Environment.NewLine +
            "inv_weight = 0.5" + Environment.NewLine +
            "inv_grid_x	= 19" + Environment.NewLine +
            "inv_grid_y	= 16" + Environment.NewLine +
            "cost = 5000" + Environment.NewLine +
            "jump_height = .1" + Environment.NewLine +
            "particles = anomaly2\\artefact\\artefact_mincer" + Environment.NewLine +
            "lights_enabled = false" + Environment.NewLine +
            "artefact_activation_seq = af_activation_mincer" + Environment.NewLine +
            "health_restore_speed = 0.0006" + Environment.NewLine +
            "radiation_restore_speed = 0.0" + Environment.NewLine +
            "satiety_restore_speed = 0.0" + Environment.NewLine +
            "power_restore_speed = 0.0" + Environment.NewLine +
            "bleeding_restore_speed = 0.0" + Environment.NewLine +
            "hit_absorbation_sect = af_soul_absorbation" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_soul_absorbation]" + Environment.NewLine +
            "burn_immunity = 1.0;" + Environment.NewLine +
            "strike_immunity = 1.1" + Environment.NewLine +
            "shock_immunity = 1.0" + Environment.NewLine +
            "wound_immunity = 1.1" + Environment.NewLine +
            "radiation_immunity = 1.0" + Environment.NewLine +
            "telepatic_immunity = 1.0" + Environment.NewLine +
            "chemical_burn_immunity = 1.0" + Environment.NewLine +
            "explosion_immunity = 1.0" + Environment.NewLine +
            "fire_wound_immunity  = 1.1" + Environment.NewLine + Environment.NewLine +
            // =============================================                                    
            "[af_electra_sparkler]:af_base" + Environment.NewLine +
            "GroupControlSection = spawn_group" + Environment.NewLine +
            "$spawn	= \"artifacts\\electra sparkler\"" + Environment.NewLine +
            "$prefetch = 64" + Environment.NewLine +
            "cform = skeleton" + Environment.NewLine +
            "class = SCRPTART" + Environment.NewLine +
            "visual = physics\\anomaly\\artefact_electra4.ogf" + Environment.NewLine +
            "description = enc_zone_artifact_af-electra-sparkler" + Environment.NewLine +
            "inv_name = af-electra-sparkler" + Environment.NewLine +
            "inv_name_short =" + Environment.NewLine +
            "inv_weight = 0.5" + Environment.NewLine +
            "inv_grid_x	= 8" + Environment.NewLine +
            "inv_grid_y	= 11" + Environment.NewLine +
            "cost = 1000" + Environment.NewLine +
            "jump_height = .5" + Environment.NewLine +
            "particles = anomaly2\\artefact\\artefact_electra" + Environment.NewLine +
            "lights_enabled = true;" + Environment.NewLine +
            "trail_light_color = 0.5,0.5,0.9" + Environment.NewLine +
            "trail_light_range = 0.50" + Environment.NewLine +
            "artefact_activation_seq = af_activation_electra" + Environment.NewLine +
            "health_restore_speed = 0.0" + Environment.NewLine +
            "radiation_restore_speed = 0.0" + Environment.NewLine +
            "satiety_restore_speed = 0.0" + Environment.NewLine +
            "power_restore_speed = 0.002" + Environment.NewLine +
            "bleeding_restore_speed = 0.0" + Environment.NewLine +
            "hit_absorbation_sect = af_electra_sparkler_absorbation" + Environment.NewLine + Environment.NewLine +
            // =============================================                                 
            "[af_electra_sparkler_absorbation]" + Environment.NewLine +
            "burn_immunity = 1.0;" + Environment.NewLine +
            "strike_immunity = 1.0" + Environment.NewLine +
            "shock_immunity = 1.1" + Environment.NewLine +
            "wound_immunity = 1.0" + Environment.NewLine +
            "radiation_immunity = 1.0" + Environment.NewLine +
            "telepatic_immunity = 1.0" + Environment.NewLine +
            "chemical_burn_immunity = 1.0" + Environment.NewLine +
            "explosion_immunity = 1.0" + Environment.NewLine +
            "fire_wound_immunity  = 1.0" + Environment.NewLine + Environment.NewLine +
            // =============================================                                   // ELECTRA ARTEFACT
            "[af_electra_flash]:af_base" + Environment.NewLine +
            "GroupControlSection = spawn_group" + Environment.NewLine +
            "$spawn	= \"artifacts\\electra flash\"" + Environment.NewLine +
            "$prefetch = 64" + Environment.NewLine +
            "cform = skeleton" + Environment.NewLine +
            "class = SCRPTART" + Environment.NewLine +
            "visual = physics\\anomaly\\artefact_electra3.ogf" + Environment.NewLine +         // Вид артефакта physics\\anomaly\\artefact_electra3.ogf
            "description = enc_zone_artifact_af-electra-flash" + Environment.NewLine +         
            "inv_name = af-electra-flash" + Environment.NewLine +
            "inv_name_short =" + Environment.NewLine +
            "inv_weight = 0.5" + Environment.NewLine +
            "inv_grid_x	= 4" + Environment.NewLine +
            "inv_grid_y	= 8" + Environment.NewLine +
            "cost = 2500" + Environment.NewLine +
            "jump_height = .1" + Environment.NewLine +
            "particles = anomaly2\\artefact\\artefact_electra" + Environment.NewLine +         // Партиклы проигрывания             // anomaly2\\artefact\\artefact_electra
            "lights_enabled = true" + Environment.NewLine +                                    // Разрешить издавать свет
            "trail_light_color = 0.6,0.7,0.9" + Environment.NewLine +                          // RGB LIGHT COLOR 0.6,0.7,0.9
            "trail_light_range = 2.5" + Environment.NewLine +                                  // LIGHT RANGE 2.5
            "artefact_activation_seq = af_activation_electra" + Environment.NewLine +
            "health_restore_speed = 0.0" + Environment.NewLine +
            "radiation_restore_speed = 0.0" + Environment.NewLine +
            "satiety_restore_speed = 0.0" + Environment.NewLine +
            "power_restore_speed = 0.004" + Environment.NewLine +
            "bleeding_restore_speed = 0.0" + Environment.NewLine +
            "hit_absorbation_sect = af_electra_flash_absorbation" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_electra_flash_absorbation]" + Environment.NewLine +
            "burn_immunity = 1.0" + Environment.NewLine +
            "strike_immunity = 1.0" + Environment.NewLine +
            "shock_immunity = 1.1" + Environment.NewLine +
            "wound_immunity = 1.0" + Environment.NewLine +
            "radiation_immunity = 1.0" + Environment.NewLine +
            "telepatic_immunity = 1.0" + Environment.NewLine +
            "chemical_burn_immunity = 1.0" + Environment.NewLine +
            "explosion_immunity = 1.0" + Environment.NewLine +
            "fire_wound_immunity  = 1.0" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_electra_moonlight]:af_base" + Environment.NewLine +
            "GroupControlSection = spawn_group" + Environment.NewLine +
            "$spawn	= \"artifacts\\electra moonlight\"" + Environment.NewLine +
            "$prefetch = 64" + Environment.NewLine +
            "cform = skeleton" + Environment.NewLine +
            "class = SCRPTART" + Environment.NewLine +
            "visual	= physics\\anomaly\\artefact_electra1.ogf " + Environment.NewLine +
            "description = enc_zone_artifact_af-electra-moonlight" + Environment.NewLine +
            "inv_name = af-electra-moonlight" + Environment.NewLine +
            "inv_name_short	= " + Environment.NewLine +
            "inv_weight	= 0.5" + Environment.NewLine +
            "inv_grid_x	= 10" + Environment.NewLine +
            "inv_grid_y	= 10" + Environment.NewLine +
            "cost = 5000" + Environment.NewLine +
            "jump_height = .1" + Environment.NewLine +
            "particles = anomaly2\\artefact\\artefact_electra" + Environment.NewLine +
            "lights_enabled = true" + Environment.NewLine +
            "trail_light_color = 0.6,0.7,0.9" + Environment.NewLine +
            "trail_light_range = 1.0" + Environment.NewLine +
            "artefact_activation_seq = af_activation_electra" + Environment.NewLine +
            "health_restore_speed = 0.0" + Environment.NewLine +
            "radiation_restore_speed = 0.0" + Environment.NewLine +
            "satiety_restore_speed = 0.0" + Environment.NewLine +
            "power_restore_speed = 0.006" + Environment.NewLine +
            "bleeding_restore_speed = 0.0" + Environment.NewLine +
            "hit_absorbation_sect = af_electra_moonlight_absorbation" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_electra_moonlight_absorbation]" + Environment.NewLine +
            "burn_immunity = 1.0 ;коэффициенты иммунитета " + Environment.NewLine +
            "strike_immunity = 1.0" + Environment.NewLine +
            "shock_immunity = 1.1" + Environment.NewLine +
            "wound_immunity = 1.0" + Environment.NewLine +
            "radiation_immunity = 1.0" + Environment.NewLine +
            "telepatic_immunity = 1.0" + Environment.NewLine +
            "chemical_burn_immunity = 1.0" + Environment.NewLine +
            "explosion_immunity = 1.0" + Environment.NewLine +
            "fire_wound_immunity  = 1.0" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_ameba_slime]:af_base" + Environment.NewLine +
            "GroupControlSection = spawn_group" + Environment.NewLine +
            "$spawn	= \"artifacts\\ameba slime\"" + Environment.NewLine +
            "$prefetch = 64" + Environment.NewLine +
            "cform = skeleton" + Environment.NewLine +
            "class = ARTEFACT" + Environment.NewLine +
            "visual	= physics\\anomaly\\artefact_ameba3.ogf" + Environment.NewLine +
            "description = enc_zone_artifact_af-ameba-slime" + Environment.NewLine +
            "inv_name = af-ameba-slime" + Environment.NewLine +
            "inv_name_short =" + Environment.NewLine +
            "inv_weight	= 0.5" + Environment.NewLine +
            "inv_grid_x	= 7" + Environment.NewLine +
            "inv_grid_y	= 4" + Environment.NewLine +
            "cost = 1000" + Environment.NewLine +
            "jump_height = .0" + Environment.NewLine +
            "particles = anomaly2\\artefact\\artefact_ameba" + Environment.NewLine +
            "lights_enabled = false" + Environment.NewLine +
            "health_restore_speed = 0.0" + Environment.NewLine +
            "radiation_restore_speed = 0.0" + Environment.NewLine +
            "satiety_restore_speed = 0.0" + Environment.NewLine +
            "power_restore_speed = 0.0" + Environment.NewLine +
            "bleeding_restore_speed = 0.004" + Environment.NewLine +
            "hit_absorbation_sect = af_ameba_slime_absorbation" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_ameba_slime_absorbation]" + Environment.NewLine +
            "burn_immunity = 1.1" + Environment.NewLine +
            "strike_immunity = 1.0" + Environment.NewLine +
            "shock_immunity = 1.0" + Environment.NewLine +
            "wound_immunity = 1.0" + Environment.NewLine +
            "radiation_immunity = 1.0" + Environment.NewLine +
            "telepatic_immunity = 1.0" + Environment.NewLine +
            "chemical_burn_immunity = 1.1" + Environment.NewLine +
            "explosion_immunity = 1.0" + Environment.NewLine +
            "fire_wound_immunity  = 1.0" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_ameba_slug]:af_base" + Environment.NewLine +
            "GroupControlSection	= spawn_group" + Environment.NewLine +
            "$spawn	= \"artifacts\\ameba slug\"" + Environment.NewLine +
            "$prefetch = 64" + Environment.NewLine +
            "cform = skeleton" + Environment.NewLine +
            "class = ARTEFACT" + Environment.NewLine +
            "visual	= physics\\anomaly\\artefact_ameba2.ogf" + Environment.NewLine +
            "description = enc_zone_artifact_af-ameba-slug" + Environment.NewLine +
            "inv_name = af-ameba-slug" + Environment.NewLine +
            "inv_name_short	=" + Environment.NewLine +
            "inv_weight	= 0.5" + Environment.NewLine +
            "inv_grid_x	= 6" + Environment.NewLine +
            "inv_grid_y	= 4" + Environment.NewLine +
            "cost = 2500" + Environment.NewLine +
            "jump_height = .1" + Environment.NewLine +
            "particles = anomaly2\\artefact\\artefact_ameba" + Environment.NewLine +
            "lights_enabled = true" + Environment.NewLine +
            "trail_light_color = 0.6,0.6,0.5" + Environment.NewLine +
            "trail_light_range = 1.0" + Environment.NewLine +
            "health_restore_speed = 0.0	" + Environment.NewLine +
            "radiation_restore_speed = 0.0" + Environment.NewLine +
            "satiety_restore_speed = 0.0" + Environment.NewLine +
            "power_restore_speed = 0.0" + Environment.NewLine +
            "bleeding_restore_speed = 0.008" + Environment.NewLine +
            "hit_absorbation_sect = af_ameba_absorbation" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_ameba_absorbation]" + Environment.NewLine +
            "burn_immunity = 1.1" + Environment.NewLine +
            "strike_immunity = 1.0" + Environment.NewLine +
            "shock_immunity = 1.0" + Environment.NewLine +
            "wound_immunity = 1.0" + Environment.NewLine +
            "radiation_immunity = 1.0" + Environment.NewLine +
            "telepatic_immunity = 1.0" + Environment.NewLine +
            "chemical_burn_immunity = 1.1" + Environment.NewLine +
            "explosion_immunity = 1.0" + Environment.NewLine +
            "fire_wound_immunity  = 1.0" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_ameba_mica]:af_base" + Environment.NewLine +
            "GroupControlSection = spawn_group" + Environment.NewLine +
            "$spawn	= \"artifacts\\ameba mica\"" + Environment.NewLine +
            "$prefetch = 64" + Environment.NewLine +
            "cform = skeleton" + Environment.NewLine +
            "class = ARTEFACT" + Environment.NewLine +
            "visual	= physics\\anomaly\\artefact_ameba1.ogf" + Environment.NewLine +
            "description = enc_zone_artifact_af-ameba-mica" + Environment.NewLine +
            "inv_name = af-ameba-mica" + Environment.NewLine +
            "inv_name_short	=" + Environment.NewLine +
            "inv_weight	= 0.5" + Environment.NewLine +
            "inv_grid_x	= 5" + Environment.NewLine +
            "inv_grid_y	= 4" + Environment.NewLine +
            "cost = 5000" + Environment.NewLine +
            "jump_height = .1" + Environment.NewLine +
            "particles = anomaly2\\artefact\\artefact_ameba" + Environment.NewLine +
            "lights_enabled = true" + Environment.NewLine +
            "trail_light_color = 0.6,0.7,0.6" + Environment.NewLine +
            "trail_light_range = 1" + Environment.NewLine +
            "health_restore_speed = 0.0	" + Environment.NewLine +
            "radiation_restore_speed = 0.0" + Environment.NewLine +
            "satiety_restore_speed = 0.0" + Environment.NewLine +
            "power_restore_speed = 0.0" + Environment.NewLine +
            "bleeding_restore_speed = 0.012" + Environment.NewLine +
            "hit_absorbation_sect = af_ameba_mica_absorbation" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_ameba_mica_absorbation]" + Environment.NewLine +
            "burn_immunity = 1.1" + Environment.NewLine +
            "strike_immunity = 1.0" + Environment.NewLine +
            "shock_immunity = 1.0" + Environment.NewLine +
            "wound_immunity = 1.0" + Environment.NewLine +
            "radiation_immunity = 1.0" + Environment.NewLine +
            "telepatic_immunity = 1.0" + Environment.NewLine +
            "chemical_burn_immunity = 1.1" + Environment.NewLine +
            "explosion_immunity = 1.0" + Environment.NewLine +
            "fire_wound_immunity  = 1.0" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_dummy_spring]:af_base" + Environment.NewLine +
            "GroupControlSection = spawn_group" + Environment.NewLine +
            "$spawn = \"artifacts\\dummy spring\"" + Environment.NewLine +
            "$npc = on" + Environment.NewLine +
            ";$prefetch = 3" + Environment.NewLine +
            "cform = skeleton" + Environment.NewLine +
            "class = ARTEFACT" + Environment.NewLine +
            "visual = physics\\anomaly\\artefact_pustishka.ogf" + Environment.NewLine +
            "description = enc_zone_artifact_af-dummy-spring" + Environment.NewLine +
            "inv_name = af-dummy-spring" + Environment.NewLine +
            "inv_name_short	= " + Environment.NewLine +
            "inv_weight	= 0.5" + Environment.NewLine +
            "inv_grid_x	= 14" + Environment.NewLine +
            "inv_grid_y	= 1" + Environment.NewLine +
            "cost = 5000" + Environment.NewLine +
            "jump_height = .2" + Environment.NewLine +
            "lights_enabled = false" + Environment.NewLine +
            "health_restore_speed = 0.0" + Environment.NewLine +
            "radiation_restore_speed = 0.0" + Environment.NewLine +
            "satiety_restore_speed = 0.0" + Environment.NewLine +
            "power_restore_speed = 0.0" + Environment.NewLine +
            "bleeding_restore_speed = 0.0" + Environment.NewLine +
            "hit_absorbation_sect = af_dummy_spring_absorbation" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_dummy_spring_absorbation]" + Environment.NewLine +
            "burn_immunity = 1.0" + Environment.NewLine +
            "strike_immunity = 0.7" + Environment.NewLine +
            "shock_immunity = 1.0" + Environment.NewLine +
            "wound_immunity = 1.0" + Environment.NewLine +
            "radiation_immunity = 1.0" + Environment.NewLine +
            "telepatic_immunity = 1.0" + Environment.NewLine +
            "chemical_burn_immunity = 1.0" + Environment.NewLine +
            "explosion_immunity = 1.0" + Environment.NewLine +
            "fire_wound_immunity  = 1.0" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_dummy_dummy]:af_base" + Environment.NewLine +
            "GroupControlSection = spawn_group" + Environment.NewLine +
            "$spawn = \"artifacts\\dummy dummy\"" + Environment.NewLine +
            "$npc = on" + Environment.NewLine +
            ";$prefetch = 3" + Environment.NewLine +
            "cform = skeleton" + Environment.NewLine +
            "class = ARTEFACT" + Environment.NewLine +
            "visual	= physics\\anomaly\\artefact_pustishka1.ogf" + Environment.NewLine +
            "description = enc_zone_artifact_af-dummy-dummy" + Environment.NewLine +
            "inv_name = af-dummy-battery" + Environment.NewLine +
            "inv_name_short	=" + Environment.NewLine +
            "inv_weight	= 0.5" + Environment.NewLine +
            "inv_grid_x	= 19" + Environment.NewLine +
            "inv_grid_y	= 6" + Environment.NewLine +
            "cost = 5000" + Environment.NewLine +
            "jump_height = .0" + Environment.NewLine +
            "lights_enabled = false" + Environment.NewLine +
            "health_restore_speed = 0.0" + Environment.NewLine +
            "radiation_restore_speed = 0.0" + Environment.NewLine +
            "satiety_restore_speed = 0.0" + Environment.NewLine +
            "power_restore_speed = 0.0" + Environment.NewLine +
            "bleeding_restore_speed = 0.0" + Environment.NewLine +
            "hit_absorbation_sect = af_dummy_dummy_absorbation" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_dummy_dummy_absorbation]" + Environment.NewLine +
            "burn_immunity = 0.7" + Environment.NewLine +
            "strike_immunity = 1.0" + Environment.NewLine +
            "shock_immunity = 1.0" + Environment.NewLine +
            "wound_immunity = 1.0" + Environment.NewLine +
            "radiation_immunity = 1.0" + Environment.NewLine +
            "telepatic_immunity = 1.0" + Environment.NewLine +
            "chemical_burn_immunity = 1.0" + Environment.NewLine +
            "explosion_immunity = 1.0" + Environment.NewLine +
            "fire_wound_immunity  = 1.0" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_dummy_battery]:af_base" + Environment.NewLine +
            "GroupControlSection = spawn_group" + Environment.NewLine +
            "$spawn = \"artifacts\\dummy battery\"" + Environment.NewLine +
            "$npc = on" + Environment.NewLine +
            ";$prefetch = 3" + Environment.NewLine +
            "cform = skeleton" + Environment.NewLine +
            "class = ARTEFACT" + Environment.NewLine +
            "visual = physics\\anomaly\\artefact_battery.ogf" + Environment.NewLine +
            "description = enc_zone_artifact_af-dummy-battery" + Environment.NewLine +
            "inv_name = af-dummy-battery" + Environment.NewLine +
            "inv_name_short	=" + Environment.NewLine +
            "inv_weight	= 0.5" + Environment.NewLine +
            "inv_grid_x	= 8" + Environment.NewLine +
            "inv_grid_y	= 4" + Environment.NewLine +
            "jump_height = .2" + Environment.NewLine +
            "cost = 5000	;200" + Environment.NewLine +
            "particles = anomaly2\\artefact\\artefact_gravi" + Environment.NewLine +
            "lights_enabled = false" + Environment.NewLine +
            "health_restore_speed = 0.0" + Environment.NewLine +
            "radiation_restore_speed = 0.0" + Environment.NewLine +
            "satiety_restore_speed = 0.0" + Environment.NewLine +
            "power_restore_speed = 0.0" + Environment.NewLine +
            "bleeding_restore_speed = 0.0" + Environment.NewLine +
            "hit_absorbation_sect = af_dummy_battery_absorbation" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_dummy_battery_absorbation]" + Environment.NewLine +
            "burn_immunity = 1.0 ; коэффициенты иммунитета " + Environment.NewLine +
            "strike_immunity = 1.0" + Environment.NewLine +
            "shock_immunity = 0.7" + Environment.NewLine +
            "wound_immunity = 1.0" + Environment.NewLine +
            "radiation_immunity = 1.0" + Environment.NewLine +
            "telepatic_immunity = 1.0" + Environment.NewLine +
            "chemical_burn_immunity = 1.0" + Environment.NewLine +
            "explosion_immunity = 1.0" + Environment.NewLine +
            "fire_wound_immunity  = 1.0" + Environment.NewLine + Environment.NewLine +
            // =============================================                                         // PLENKA ARTEFACT
            "[af_dummy_pellicle]:af_base" + Environment.NewLine +
            "GroupControlSection = spawn_group" + Environment.NewLine +
            "$spawn = \"artifacts\\dummy pellicle\"" + Environment.NewLine +
            "$npc = on" + Environment.NewLine +
            ";$prefetch = 3" + Environment.NewLine +
            "cform = skeleton" + Environment.NewLine +
            "class = ARTEFACT" + Environment.NewLine +
            "visual	= physics\\box\\box_lab_01.ogf" + Environment.NewLine +                 // ВИД ОБЪЕКТА physics\\anomaly\\artefact_plenka.ogf
            "description = enc_zone_artifact_af-dummy-pellicle" + Environment.NewLine +
            "inv_name = af-dummy-pellicle" + Environment.NewLine +
            "inv_name_short	= " + Environment.NewLine +
            "inv_weight	= 0.5" + Environment.NewLine +
            "inv_grid_x	= 4" + Environment.NewLine +
            "inv_grid_y	= 9" + Environment.NewLine +
            "jump_height = .2" + Environment.NewLine +
            "cost = 5000; 200" + Environment.NewLine +
            "particles = anomaly2\\artefact\\artefact_ameba" + Environment.NewLine +
            "lights_enabled = false;" + Environment.NewLine +
            "health_restore_speed = 0.0" + Environment.NewLine +
            "radiation_restore_speed = 0.0" + Environment.NewLine +
            "satiety_restore_speed = 0.0" + Environment.NewLine +
            "power_restore_speed = 0.0" + Environment.NewLine +
            "bleeding_restore_speed = 0.0" + Environment.NewLine +
            "hit_absorbation_sect = af_dummy_pellicle_absorbation" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_dummy_pellicle_absorbation]" + Environment.NewLine +
            "burn_immunity = 1.0" + Environment.NewLine +
            "strike_immunity = 1.0" + Environment.NewLine +
            "shock_immunity = 1.0" + Environment.NewLine +
            "wound_immunity = 1.0" + Environment.NewLine +
            "radiation_immunity = 1.0" + Environment.NewLine +
            "telepatic_immunity = 1.0" + Environment.NewLine +
            "chemical_burn_immunity = 0.7" + Environment.NewLine +
            "explosion_immunity = 1.0" + Environment.NewLine +
            "fire_wound_immunity  = 1.0" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_dummy_glassbeads]:af_base" + Environment.NewLine +
            "GroupControlSection = spawn_group" + Environment.NewLine +
            "$spawn = \"artifacts\\dummy glassbeads\"" + Environment.NewLine +
            "$npc = on" + Environment.NewLine +
            ";$prefetch = 3" + Environment.NewLine +
            "cform = skeleton" + Environment.NewLine +
            "class = ARTEFACT" + Environment.NewLine +
            "visual = physics\\anomaly\\artefact_spiral.ogf" + Environment.NewLine +
            "description = enc_zone_artifact_af-dummy-glassbeads" + Environment.NewLine +
            "inv_name = af-dummy-glassbeads" + Environment.NewLine +
            "inv_name_short	=" + Environment.NewLine +
            "inv_weight	= 0.5" + Environment.NewLine +
            "inv_grid_x	= 16" + Environment.NewLine +
            "inv_grid_y	= 1" + Environment.NewLine +
            "cost = 5000 ;200" + Environment.NewLine +
            "jump_height = .2" + Environment.NewLine +
            "particles = anomaly2\\artefact\\artefact_mincer" + Environment.NewLine +
            "lights_enabled = false" + Environment.NewLine +
            "health_restore_speed = 0.0" + Environment.NewLine +
            "radiation_restore_speed = 0.0" + Environment.NewLine +
            "satiety_restore_speed = 0.0" + Environment.NewLine +
            "power_restore_speed = 0.0" + Environment.NewLine +
            "bleeding_restore_speed = 0.0" + Environment.NewLine +
            "hit_absorbation_sect = af_dummy_glassbeads_absorbation" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_dummy_glassbeads_absorbation]" + Environment.NewLine +
            "burn_immunity = 1.0" + Environment.NewLine +
            "strike_immunity = 1.0" + Environment.NewLine +
            "shock_immunity = 1.0" + Environment.NewLine +
            "wound_immunity = 1.0" + Environment.NewLine +
            "radiation_immunity = 1.0" + Environment.NewLine +
            "telepatic_immunity = 1.0" + Environment.NewLine +
            "chemical_burn_immunity = 1.0" + Environment.NewLine +
            "explosion_immunity = 1.0" + Environment.NewLine +
            "fire_wound_immunity  = 0.95" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_rusty_thorn]:af_base" + Environment.NewLine +
            "GroupControlSection = spawn_group" + Environment.NewLine +
            "$spawn = \"artifacts\\rusty thorn\"" + Environment.NewLine +
            "$npc = on" + Environment.NewLine +
            ";$prefetch = 3" + Environment.NewLine +
            "cform = skeleton" + Environment.NewLine +
            "class = ARTEFACT" + Environment.NewLine +
            "visual	= physics\\anomaly\\artefact_needles1.ogf" + Environment.NewLine +
            "visual	= physics\\anomaly\\artefact_needles1.ogf" + Environment.NewLine +
            "description = enc_zone_artifact_af-rusty-thorn" + Environment.NewLine +
            "inv_name = af-rusty-thorn" + Environment.NewLine +
            "inv_name_short	=" + Environment.NewLine +
            "inv_weight	= 0.5" + Environment.NewLine +
            "inv_grid_x	= 9" + Environment.NewLine +
            "inv_grid_y	= 11" + Environment.NewLine +
            "cost = 1000" + Environment.NewLine +
            "jump_height = .3" + Environment.NewLine +
            "lights_enabled = false" + Environment.NewLine +
            "health_restore_speed = 0.0" + Environment.NewLine +
            "radiation_restore_speed = -0.001" + Environment.NewLine +
            "satiety_restore_speed = 0.0" + Environment.NewLine +
            "power_restore_speed = 0.0" + Environment.NewLine +
            "bleeding_restore_speed = -0.003" + Environment.NewLine +
            "hit_absorbation_sect = af_rusty_thorn_absorbation" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_rusty_thorn_absorbation]" + Environment.NewLine +
            "burn_immunity = 1.0" + Environment.NewLine +
            "strike_immunity = 1.0" + Environment.NewLine +
            "shock_immunity = 1.0" + Environment.NewLine +
            "wound_immunity = 1.0" + Environment.NewLine +
            "radiation_immunity = 1.0" + Environment.NewLine +
            "telepatic_immunity = 1.0" + Environment.NewLine +
            "chemical_burn_immunity = 1.0" + Environment.NewLine +
            "explosion_immunity = 1.0" + Environment.NewLine +
            "fire_wound_immunity  = 1.0" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_rusty_kristall]:af_base" + Environment.NewLine +
            "GroupControlSection = spawn_group" + Environment.NewLine +
            "$spawn = \"artifacts\\rusty cristall\"" + Environment.NewLine +
            "$npc = on" + Environment.NewLine +
            ";$prefetch = 3" + Environment.NewLine +
            "cform = skeleton" + Environment.NewLine +
            "class = ARTEFACT" + Environment.NewLine +
            "visual = physics\\anomaly\\artefact_needles2.ogf" + Environment.NewLine +
            "description = enc_zone_artifact_af-rusty-kristall" + Environment.NewLine +
            "inv_name = af-rusty-kristall" + Environment.NewLine +
            "inv_name_short	= " + Environment.NewLine +
            "inv_weight	= 0.5" + Environment.NewLine +
            "inv_grid_x	= 9" + Environment.NewLine +
            "inv_grid_y = 10" + Environment.NewLine +
            "cost = 2500" + Environment.NewLine +
            "jump_height = .4" + Environment.NewLine +
            "lights_enabled = false" + Environment.NewLine +
            "health_restore_speed = 0.0" + Environment.NewLine +
            "radiation_restore_speed = -0.002" + Environment.NewLine +
            "satiety_restore_speed = 0.0" + Environment.NewLine +
            "power_restore_speed = 0.0" + Environment.NewLine +
            "bleeding_restore_speed = -0.003" + Environment.NewLine +
            "hit_absorbation_sect = af_rusty_kristall_absorbation" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_rusty_kristall_absorbation]" + Environment.NewLine +
            "burn_immunity = 1.0" + Environment.NewLine +
            "strike_immunity = 1.0" + Environment.NewLine +
            "shock_immunity = 1.0" + Environment.NewLine +
            "wound_immunity = 1.0" + Environment.NewLine +
            "radiation_immunity = 1.0" + Environment.NewLine +
            "telepatic_immunity = 1.0" + Environment.NewLine +
            "chemical_burn_immunity = 1.0" + Environment.NewLine +
            "explosion_immunity = 1.0" + Environment.NewLine +
            "fire_wound_immunity  = 1.0" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_rusty_sea-urchin]:af_base" + Environment.NewLine +
            "GroupControlSection = spawn_group" + Environment.NewLine +
            "$spawn = \"artifacts\\rusty sea-urchin\"" + Environment.NewLine +
            "$npc = on" + Environment.NewLine +
            ";$prefetch = 3" + Environment.NewLine +
            "cform = skeleton" + Environment.NewLine +
            "class = ARTEFACT" + Environment.NewLine +
            "visual = physics\\anomaly\\artefact_rusty_hairs.ogf" + Environment.NewLine +
            "description = enc_zone_artifact_af-rusty-sea-urchin" + Environment.NewLine +
            "inv_name = af-rusty-sea-urchin" + Environment.NewLine +
            "inv_name_short	= " + Environment.NewLine +
            "inv_weight	= 0.5" + Environment.NewLine +
            "inv_grid_x	= 15" + Environment.NewLine +
            "inv_grid_y	= 1" + Environment.NewLine +
            "cost = 5000" + Environment.NewLine +
            "jump_height = .5" + Environment.NewLine +
            "lights_enabled = false" + Environment.NewLine +
            "health_restore_speed = 0.0" + Environment.NewLine +
            "radiation_restore_speed = -0.003" + Environment.NewLine +
            "satiety_restore_speed = 0.0" + Environment.NewLine +
            "power_restore_speed = 0.0" + Environment.NewLine +
            "bleeding_restore_speed = -0.003" + Environment.NewLine +
            "hit_absorbation_sect = af_rusty_sea-urchin_absorbation" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_rusty_sea-urchin_absorbation]" + Environment.NewLine +
            "burn_immunity = 1.0" + Environment.NewLine +
            "strike_immunity = 1.0" + Environment.NewLine +
            "shock_immunity = 1.0" + Environment.NewLine +
            "wound_immunity = 1.0" + Environment.NewLine +
            "radiation_immunity = 1.0" + Environment.NewLine +
            "telepatic_immunity = 1.0" + Environment.NewLine +
            "chemical_burn_immunity = 1.0" + Environment.NewLine +
            "explosion_immunity = 1.0" + Environment.NewLine +
            "fire_wound_immunity  = 1.0" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_fuzz_kolobok]:af_base" + Environment.NewLine +
            "GroupControlSection = spawn_group" + Environment.NewLine +
            "$spawn = \"artifacts\\fuzz kolobok\"" + Environment.NewLine +
            "$npc = on" + Environment.NewLine +
            ";$prefetch = 3" + Environment.NewLine +
            "cform = skeleton" + Environment.NewLine +
            "class = ARTEFACT" + Environment.NewLine +
            "visual	= physics\\anomaly\\artefact_rusty_hairs.ogf" + Environment.NewLine +
            "description = enc_zone_artifact_af-fuzz-kolobok" + Environment.NewLine +
            "inv_name = af-fuzz-kolobok" + Environment.NewLine +
            "inv_name_short	=" + Environment.NewLine +
            "inv_weight	= 0.5" + Environment.NewLine +
            "inv_grid_x	= 17" + Environment.NewLine +
            "inv_grid_y	= 1" + Environment.NewLine +
            "cost = 5000" + Environment.NewLine +
            "jump_height = .5" + Environment.NewLine +
            "particles = anomaly2\\artefact\\artefact_puxx" + Environment.NewLine +
            "lights_enabled = false" + Environment.NewLine +
            "health_restore_speed = 0.0" + Environment.NewLine +
            "radiation_restore_speed = 0.0" + Environment.NewLine +
            "satiety_restore_speed = 0.0" + Environment.NewLine +
            "power_restore_speed = 0.0" + Environment.NewLine +
            "bleeding_restore_speed = 0.0" + Environment.NewLine +
            "hit_absorbation_sect = af_fuzz_kolobok_absorbation" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[af_fuzz_kolobok_absorbation]" + Environment.NewLine +
            "burn_immunity = 1.0" + Environment.NewLine +
            "strike_immunity = 1.0" + Environment.NewLine +
            "shock_immunity = 1.0" + Environment.NewLine +
            "wound_immunity = 0.95" + Environment.NewLine +
            "radiation_immunity = 1.0" + Environment.NewLine +
            "telepatic_immunity = 1.0" + Environment.NewLine +
            "chemical_burn_immunity = 1.0" + Environment.NewLine +
            "explosion_immunity = 1.0" + Environment.NewLine +
            "fire_wound_immunity = 1.0" + Environment.NewLine);
        }
        // ===============================================================================================================================
        // TeamDeathmatch Editor
        // ===============================================================================================================================
        private void btnTeamDeathmatchSelectedMyShopStart_Click(object sender, EventArgs e)
        {
            // =============================================
            btnDeathmatchSelectedMyShopStart.PerformClick();
            // =============================================             
            TeamDeathmatchEditor("[teamdeathmatch_base_cost]:deathmatch_base_cost" + Environment.NewLine + Environment.NewLine +
            "[teamdeathmatch_team1]" + Environment.NewLine +
            "team_idx = 0" + Environment.NewLine +
            "pistols = mp_wpn_pb, mp_wpn_fort, mp_wpn_sig220, mp_wpn_usp, mp_wpn_desert_eagle;" + Environment.NewLine +
            "shotgun = mp_wpn_bm16, mp_wpn_wincheaster1300, mp_wpn_spas12;" + Environment.NewLine +
            "assault = mp_wpn_ak74u, mp_wpn_ak74, mp_wpn_abakan, mp_wpn_groza, mp_wpn_val, mp_wpn_fn2000;" + Environment.NewLine +
            "sniper_rifles = mp_wpn_vintorez, mp_wpn_svu, mp_wpn_gauss;" + Environment.NewLine +
            "heavy_weapons = mp_wpn_rg-6, mp_wpn_rpg7;" + Environment.NewLine +
            "granades = mp_grenade_rgd5, mp_grenade_f1,mp_grenade_gd-05,mp_ammo_vog-25, mp_ammo_m209, mp_ammo_og-7b;" + Environment.NewLine +
            "outfits = mp_scientific_outfit, mp_military_stalker_outfit,mp_exo_outfit;" + Environment.NewLine +
            "equipment = mp_device_torch, mp_wpn_addon_silencer,mp_wpn_addon_scope, mp_wpn_addon_grenade_launcher, mp_medkit, mp_antirad, mp_detector_advances, mp_wpn_binoc;" + Environment.NewLine +
            "skins = stalker_sv_balon_10, stalker_sv_hood_9,stalker_sv_rukzak_3,stalker_sv_rukzak_2,stalker_killer_mask_fr,stalker_killer_mask_uk;" + Environment.NewLine +
            // =============================================
            "default_items	= " + Result + Environment.NewLine +  // get items TEAM 1
            // =============================================
            "mp_exo_outfit	= stalker_sv_exoskeleton;" + Environment.NewLine +
            "mp_scientific_outfit = stalker_sci_svoboda;" + Environment.NewLine +
            "mp_military_stalker_outfit	= stalker_sv_military;" + Environment.NewLine +
            "money_start = 5000" + Environment.NewLine +
            "money_min = 0" + Environment.NewLine +
            "kill_rival = 400" + Environment.NewLine +
            "kill_self = 0" + Environment.NewLine +
            "kill_team = -250" + Environment.NewLine +
            "target_rival = 0" + Environment.NewLine +
            "target_team = 0" + Environment.NewLine +
            "target_succeed	= 0" + Environment.NewLine +
            "target_succeed_all	= 0" + Environment.NewLine +
            "round_win = 100" + Environment.NewLine +
            "round_loose = 50" + Environment.NewLine +
            "round_draw = 50" + Environment.NewLine +
            "round_loose_minor = 0" + Environment.NewLine +
            "round_win_minor = 0" + Environment.NewLine +
            "clear_run_bonus = 200" + Environment.NewLine +
            "kill_while_invincible = 0.5" + Environment.NewLine +
            "mp_wpn_pb_cost	= 0" + Environment.NewLine +
            "indicator_r1 = 0.2" + Environment.NewLine +
            "indicator_r2 = 0.2" + Environment.NewLine +
            "indicator_x = 0.0" + Environment.NewLine +
            "indicator_y = 0.5" + Environment.NewLine +
            "indicator_z = 0.0" + Environment.NewLine +
            "indicator_shader = friendly_indicator" + Environment.NewLine +
            "indicator_texture = ui\\ui_greenteam" + Environment.NewLine +
            "invincible_shader = friendly_indicator" + Environment.NewLine +
            "invincible_texture = ui\\ui_skull" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[teamdeathmatch_team2]" + Environment.NewLine +
            "team_idx = 1" + Environment.NewLine +
            "pistols = mp_wpn_pm, mp_wpn_walther, mp_wpn_colt1911, mp_wpn_usp, mp_wpn_desert_eagle;" + Environment.NewLine +
            "shotgun = mp_wpn_bm16, mp_wpn_wincheaster1300, mp_wpn_spas12;" + Environment.NewLine +
            "assault = mp_wpn_mp5, mp_wpn_l85,mp_wpn_lr300,mp_wpn_sig550,mp_wpn_g36,mp_wpn_fn2000" + Environment.NewLine +
            "sniper_rifles = mp_wpn_svd, mp_wpn_gauss" + Environment.NewLine +
            "heavy_weapons = mp_wpn_rpg7, mp_wpn_rg-6" + Environment.NewLine +
            "granades = mp_grenade_rgd5,mp_grenade_f1, mp_grenade_gd-05, mp_ammo_vog-25, mp_ammo_m209, mp_ammo_og-7b;" + Environment.NewLine +
            "outfits = mp_scientific_outfit, mp_military_stalker_outfit,mp_exo_outfit " + Environment.NewLine +
            "equipment = mp_device_torch, mp_wpn_addon_silencer,  mp_wpn_addon_grenade_launcher_m203, mp_medkit, mp_antirad, mp_detector_advances, mp_wpn_binoc, mp_wpn_addon_scope_susat" + Environment.NewLine +
            "skins = stalker_killer_head_1, stalker_killer_antigas, stalker_killer_head_3, stalker_killer_mask, stalker_killer_mask_de, stalker_killer_mask_us" + Environment.NewLine +
            // =============================================
            "default_items = " + Result + Environment.NewLine +  // get items TEAM 2
            // =============================================
            "mp_exo_outfit = stalker_killer_exoskeleton" + Environment.NewLine +
            "mp_military_stalker_outfit = stalker_killer_military" + Environment.NewLine +
            "mp_scientific_outfit = stalker_sci_killer" + Environment.NewLine +
            "money_start = 5000" + Environment.NewLine +
            "money_min = 0" + Environment.NewLine +
            "kill_rival = 400" + Environment.NewLine +
            "kill_self = -100" + Environment.NewLine +
            "kill_team = -250" + Environment.NewLine +
            "target_rival = 0" + Environment.NewLine +
            "target_team = 0" + Environment.NewLine +
            "target_succeed = 0" + Environment.NewLine +
            "target_succeed_all = 0" + Environment.NewLine +
            "round_win = 100" + Environment.NewLine +
            "round_loose = 50" + Environment.NewLine +
            "round_draw = 50" + Environment.NewLine +
            "round_loose_minor = 0" + Environment.NewLine +
            "round_win_minor = 0" + Environment.NewLine +
            "clear_run_bonus = 200" + Environment.NewLine +
            "kill_while_invincible = 0.5" + Environment.NewLine +
            "mp_wpn_pm_cost = 0" + Environment.NewLine +
            "indicator_r1 = 0.2" + Environment.NewLine +
            "indicator_r2 = 0.2" + Environment.NewLine +
            "indicator_x = 0.0" + Environment.NewLine +
            "indicator_y = 0.5" + Environment.NewLine +
            "indicator_z = 0.0" + Environment.NewLine +
            "indicator_shader = friendly_indicator" + Environment.NewLine +
            "indicator_texture = ui\\ui_blueteam" + Environment.NewLine +
            "invincible_shader = friendly_indicator" + Environment.NewLine +
            "invincible_texture = ui\\ui_skull" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[tdm_messages_menu]" + Environment.NewLine +
            "menu_0 = tdm_menu_0" + Environment.NewLine +
            "menu_1 = tdm_menu_1" + Environment.NewLine +
            "sounds_path = characters_voice\\multiplayer\\" + Environment.NewLine +
            "team_prefix = team_" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[tdm_menu_0]" + Environment.NewLine +
            "phrase_0 = speech_attack, attack_" + Environment.NewLine +
            "phrase_1 = speech_retreat, retreat_" + Environment.NewLine +
            "phrase_2 = speech_holdpos, hold_" + Environment.NewLine +
            "phrase_3 = speech_report, report_" + Environment.NewLine +
            "phrase_4 = speech_silence, silence_" + Environment.NewLine + Environment.NewLine +
            // =============================================
            "[tdm_menu_1]" + Environment.NewLine +
            "phrase_0 = speech_roger, roger_" + Environment.NewLine +
            "phrase_1 = speech_no, no_" + Environment.NewLine +
            "phrase_2 = speech_needhelp, help_" + Environment.NewLine +
            "phrase_3 = speech_noenemy, clear_" + Environment.NewLine +
            "phrase_4 = speech_holdingpos, camp_" + Environment.NewLine +
            "phrase_5 = speech_sticktogether, together_ " + Environment.NewLine +
            "phrase_6 = speech_followme, follow_" + Environment.NewLine +
            "phrase_7 = speech_needmoney, money_");
            MessageBox.Show("Настройки модификаций для режима TeamDeathmatch - были успешно созданы!", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
 }
