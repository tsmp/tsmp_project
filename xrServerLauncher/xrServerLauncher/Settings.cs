using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace S.E.R.V.E.R___Shadow_Of_Chernobyl_1._0006
{
    public class Settings
    {
        public static Settings GetSettings()
        {
            Settings settings = null;
            string filename = SaveEvent.main_form;
            if (File.Exists(filename))
            {
                try
                {
                    using (FileStream fs = new FileStream(filename, FileMode.Open))
                    {
                        XmlSerializer xser = new XmlSerializer(typeof(Settings));
                        settings = (Settings)xser.Deserialize(fs);
                        fs.Close();
                    }
                }
                catch (Exception ex)
                {               
                    MessageBox.Show("Ошибка инициализации параметров\nПожалуйста сохраните настройки программы повторно.\n" + ex.Message, "Critical Error initializing Form settings in Config.xml", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }            
            }
            else settings = new Settings();
            return settings;
        }

        public void Save()
        {
            string filename = SaveEvent.main_form;
            if (File.Exists(filename)) File.Delete(filename);
            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                XmlSerializer xser = new XmlSerializer(typeof(Settings));
                xser.Serialize(fs, this);
                fs.Close();
            }
        }

        public bool EventsAutoColor { get; set; }
        public bool EventsAutoDelete { get; set; }
        public bool EventsChangeWindow { get; set; }
        public bool FirewallActivated { get; set; }
        public bool EventsControllerCheckPlayers { get; set; }
        public bool EventsControllerAutoFullCheckPlayers { get; set; }
        public bool EventsControllerAutoCheckPlayers { get; set; }
        public bool EventsControllerBlockedPlayers { get; set; }
        public bool EventsControllerMaxClickLim { get; set; }
        public bool EventsWeaponsBlockedPlayers { get; set; }
        public bool EventsControllerAutoSaveHistoryActivate { get; set; }
        public bool EventsControllerTrayMessage { get; set; }
        public bool AutoSaveParams { get; set; }
        public string EventsDirectoryDF { get; set; }
        public string EventsControllerMaxLimit { get; set; }
        public string EventsWeaponsMaxLimit { get; set; }
        public string versionPath { get; set; }
        public string Network_MaxCurrentConnections { get; set; }
        public string Network_MaxCurrentBlockIP { get; set; }
        public string Network_WhiteIP { get; set; }
        public bool Network_NetworkDeleteFilterAtCloseForm { get; set; }
        public bool checkEnableEditor { get; set; }
        public string ArtefactType { get; set; }
        public string ShopGreenTeam { get; set; }
        public string ShopBlueTeam { get; set; }
        public string my_shop1 { get; set; }
        public string my_shop2 { get; set; }
        public string my_shop3 { get; set; }
        public string my_shop4 { get; set; }
        public string my_shop5 { get; set; }
        public string my_shop6 { get; set; }
        public string my_shop7 { get; set; }
        public string my_shop8 { get; set; }
        public string my_shop9 { get; set; }
        public string my_shop10 { get; set; }
        public string my_shop11 { get; set; }
        public string my_shop12 { get; set; }
        public string my_shop13 { get; set; }
        public string my_shop14 { get; set; }
        public string my_shop15 { get; set; }
        public string my_shop16 { get; set; }
        // DeathmatchEditor
        public string my_shop17 { get; set; }
        public string my_shop18 { get; set; }
        public string my_shop19 { get; set; }
        public string my_shop20 { get; set; }
        public string my_shop21 { get; set; }
        public string my_shop22 { get; set; }
        public string my_shop23 { get; set; }
        public string my_shop24 { get; set; }
        public string my_shop25 { get; set; }
        public string my_shop26 { get; set; }
        public string my_shop27 { get; set; }
        public string my_shop28 { get; set; }
        public string my_shop29 { get; set; }
        public string my_shop30 { get; set; }
        public string my_shop31 { get; set; }
        public string my_shop32 { get; set; }
        public string my_shop33 { get; set; }
        public string my_shop34 { get; set; }
        public string my_shop35 { get; set; }
        public string my_shop36 { get; set; }
        public string my_shop37 { get; set; }
        public string my_shop38 { get; set; }
        public string my_shop39 { get; set; }
        public string my_shop40 { get; set; }
        public string my_shop41 { get; set; }
        public string my_shop42 { get; set; }
        public string my_shop43 { get; set; }
        public string my_shop44 { get; set; }
        public string my_shop45 { get; set; }
        public string my_shop46 { get; set; }
        public string my_shop47 { get; set; }
        public string my_shop48 { get; set; }
        public string my_shop49 { get; set; }
        public string my_shop50 { get; set; }
        // ArtefactSpawn Editor
        public string my_shop51 { get; set; }
        // cmd
        public string cmd_sv1 { get; set; }
        public string cmd_sv2 { get; set; }
        public string cmd_sv3 { get; set; }
        public string cmd_sv4 { get; set; }
        public string cmd_sv5 { get; set; }
        public string cmd_sv6 { get; set; }
        public string cmd_sv7 { get; set; }
        public string cmd_sv8 { get; set; }
        public string cmd_sv9 { get; set; }
        public string cmd_sv10 { get; set; }
        public string cmd_sv11 { get; set; }
        public string cmd_sv12 { get; set; }
        public string cmd_sv13 { get; set; }
        public string cmd_sv14 { get; set; }
        public string cmd_sv15 { get; set; }
        public string cmd_sv16 { get; set; }
        public string cmd_sv17 { get; set; }
        public string cmd_sv18 { get; set; }
        public string cmd_sv19 { get; set; }
        public string cmd_sv20 { get; set; }
        public string cmd_sv21 { get; set; }
        public string cmd_sv22 { get; set; }
        public string cmd_sv23 { get; set; }
        public string cmd_sv24 { get; set; }
        public string cmd_sv25 { get; set; }
        public string cmd_sv26 { get; set; }
        public string cmd_sv27 { get; set; }
        public string cmd_sv28 { get; set; }
        public string cmd_sv29 { get; set; }
        public string cmd_sv30 { get; set; }
        public string cmd_sv31 { get; set; }
        public string cmd_sv32 { get; set; }
        public string cmd_sv33 { get; set; }
        public string cmd_sv34 { get; set; }
        public string cmd_sv35 { get; set; }
        public string cmd_sv36 { get; set; }
        public string cmd_sv37 { get; set; }
        public string cmd_sv38 { get; set; }
        public string cmd_sv39 { get; set; }
        public string cmd_sv40 { get; set; }
        public string cmd_sv41 { get; set; }
        public string cmd_sv42 { get; set; }
        // radmins
        public string RL1 { get; set; }
        public string RP1 { get; set; }
        public string RL2 { get; set; }
        public string RP2 { get; set; }
        public string RL3 { get; set; }
        public string RP3 { get; set; }
        public string RL4 { get; set; }
        public string RP4 { get; set; }
        public string RL5 { get; set; }
        public string RP5 { get; set; }
        public string RL6 { get; set; }
        public string RP6 { get; set; }
        public string RL7 { get; set; }
        public string RP7 { get; set; }
        public string RL8 { get; set; }
        public string RP8 { get; set; }
        public string RL9 { get; set; }
        public string RP9 { get; set; }
        public string RL10 { get; set; }
        public string RP10 { get; set; }
        public string RL11 { get; set; }
        public string RP11 { get; set; }
        public string RL12 { get; set; }
        public string RP12 { get; set; }
        public string RL13 { get; set; }
        public string RP13 { get; set; }
        public string RL14 { get; set; }
        public string RP14 { get; set; }
        public string RL15 { get; set; }
        public string RP15 { get; set; }
        public string RL16 { get; set; }
        public string RP16 { get; set; }
        public string RL17 { get; set; }
        public string RP17 { get; set; }
        public string RL18 { get; set; }
        public string RP18 { get; set; }
        public string RL19 { get; set; }
        public string RP19 { get; set; }
        public string RL20 { get; set; }
        public string RP20 { get; set; }
        // radmin check
        public bool RS1 { get; set; }
        public bool RS2 { get; set; }
        public bool RS3 { get; set; }
        public bool RS4 { get; set; }
        public bool RS5 { get; set; }
        public bool RS6 { get; set; }
        public bool RS7 { get; set; }
        public bool RS8 { get; set; }
        public bool RS9 { get; set; }
        public bool RS10 { get; set; }
        public bool RS11 { get; set; }
        public bool RS12 { get; set; }
        public bool RS13 { get; set; }
        public bool RS14 { get; set; }
        public bool RS15 { get; set; }
        public bool RS16 { get; set; }
        public bool RS17 { get; set; }
        public bool RS18 { get; set; }
        public bool RS19 { get; set; }
        public bool RS20 { get; set; }

        // asm settings 
        public bool settings_asm1 { get; set; }
        public bool settings_asm2 { get; set; }
        public bool settings_asm3 { get; set; }
        public bool settings_asm4 { get; set; }
        public bool settings_asm5 { get; set; }
        public bool settings_asm6 { get; set; }
        public bool settings_asm7 { get; set; }
        public bool settings_asm8 { get; set; }
        public bool settings_asm9 { get; set; }
        public bool settings_asm10 { get; set; }
        public bool settings_asm11 { get; set; }
        public bool settings_asm12 { get; set; }
        public bool settings_asm13 { get; set; }
        public bool settings_asm14 { get; set; }
        public bool settings_asm15 { get; set; }
        public bool settings_asm16 { get; set; }
        public bool settings_asm17 { get; set; }
        public bool settings_asm18 { get; set; }
        public bool settings_asm19 { get; set; }
        public bool settings_asm20 { get; set; }
        public bool settings_asm21 { get; set; }
        public bool settings_asm22 { get; set; }
        public bool settings_asm23 { get; set; }
        public bool settings_asm24 { get; set; }
        public bool settings_asm25 { get; set; }
        public bool settings_asm26 { get; set; }
        public bool settings_asm27 { get; set; }
        public bool settings_asm28 { get; set; }
        public bool settings_asm29 { get; set; }
        public bool settings_asm30 { get; set; }
        public bool settings_asm31 { get; set; }
        public bool settings_asm32 { get; set; }
        public bool settings_asm33 { get; set; }
        public bool settings_asm34 { get; set; }
        public bool settings_asm35 { get; set; }
        public bool settings_asm36 { get; set; }
        public bool settings_asm37 { get; set; }
        public bool settings_asm38 { get; set; }
        public bool settings_asm39 { get; set; }
        public bool settings_asm40 { get; set; }
        public bool settings_asm41 { get; set; }
        public bool settings_asm42 { get; set; }
        public bool settings_asm43 { get; set; }
        public bool settings_asm44 { get; set; }
        public bool settings_asm45 { get; set; }
        public bool settings_asm46 { get; set; }
        public bool settings_asm47 { get; set; }
        public bool settings_asm48 { get; set; }
        public string settings_rules { get; set; }

        //Vote
        public bool VoteByte2 { get; set; }
        public bool VoteByte4 { get; set; }
        public bool VoteByte6 { get; set; }
        public bool VoteByte8 { get; set; }
        public bool VoteByte16 { get; set; }
        public bool VoteByte32 { get; set; }
        public bool VoteByte64 { get; set; }
        public bool VoteByte128 { get; set; }
        public bool VoteOK { get; set; }
        public string VoteQuota { get; set; }
        public string VoteTime { get; set; }
        // TSMP
        public bool RadioCreateCmd { get; set; }
        public string RadioBlockTime { get; set; }
        public string RadioBlockMSGLimit { get; set; }

        // Auto ChangeLevel
        public bool Level1 { get; set; }
        public bool Level2 { get; set; }
        public bool Level3 { get; set; }
        public bool Level4 { get; set; }
        public bool Level5 { get; set; }
        public bool Level6 { get; set; }
        public bool Level7 { get; set; }
        public bool Level8 { get; set; }
        public bool Level9 { get; set; }
        public bool Level10 { get; set; }
        public bool Level11 { get; set; }
        public bool Level12 { get; set; }
        public bool Level13 { get; set; }
        public bool Level14 { get; set; }
        public bool Level15 { get; set; }
        public bool Level16 { get; set; }
        public bool Level17 { get; set; }
        public bool Level18 { get; set; }
        public bool Level19 { get; set; }
        public bool Level20 { get; set; }
        public bool Level21 { get; set; }
        public bool Level22 { get; set; }
        public bool Level23 { get; set; }
        public bool Level24 { get; set; }
        public bool Level25 { get; set; }
        public bool Level26 { get; set; }
        public bool Level27 { get; set; }
        public bool Level28 { get; set; }
        public bool Level29 { get; set; }
        // disable scan weapons
        public bool SkipScanWeapons1 { get; set; }
        public bool SkipScanWeapons2 { get; set; }
        public bool SkipScanWeapons3 { get; set; }
        public bool SkipScanWeapons4 { get; set; }
        public bool SkipScanWeapons5 { get; set; }
        public bool SkipScanWeapons6 { get; set; }
        public bool SkipScanWeapons7 { get; set; }
        public bool SkipScanWeapons8 { get; set; }
        public bool SkipScanWeapons9 { get; set; }
        public bool SkipScanWeapons10 { get; set; }
        public bool SkipScanWeapons11 { get; set; }
        public bool SkipScanWeapons12 { get; set; }
        public bool SkipScanWeapons13 { get; set; }
        public bool SkipScanWeapons14 { get; set; }
        public bool SkipScanWeapons15 { get; set; }
        public bool SkipScanWeapons16 { get; set; }
        public bool SkipScanWeapons17 { get; set; }
        public bool SkipScanWeapons18 { get; set; }
        public bool SkipScanWeapons19 { get; set; }
        public bool SkipScanWeapons20 { get; set; }
        public bool SkipScanWeapons21 { get; set; }
        public bool SkipScanWeapons22 { get; set; }
        public bool SkipScanWeapons23 { get; set; }
        public bool SkipScanWeapons24 { get; set; }
        public bool SkipScanWeapons25 { get; set; }
        public bool SkipScanWeapons26 { get; set; }
        public bool SkipScanWeapons27 { get; set; }
    }



    // ==================================================================================
    public class SettingsMain
    { 
        public static SettingsMain GetSettingsMain()
        {
            SettingsMain settings = null;
            string filename = SaveEventGlobal.main_form;
            if (File.Exists(filename))
            {
                try
                {
                    using (FileStream fs = new FileStream(filename, FileMode.Open))
                    {
                        XmlSerializer xser = new XmlSerializer(typeof(SettingsMain));
                        settings = (SettingsMain)xser.Deserialize(fs);
                        fs.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка инициализации параметров\nПожалуйста сохраните настройки программы повторно.\n" + ex.Message, "Critical Error initializing Form settings in SrvConfig.xml", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else settings = new SettingsMain();
            return settings;
        }

        public void SaveMain()
        {
            string filename = SaveEventGlobal.main_form;
            if (File.Exists(filename))
                File.Delete(filename);
            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                XmlSerializer xser = new XmlSerializer(typeof(SettingsMain));
                xser.Serialize(fs, this);
                fs.Close();
            }
        }

        // S.E.R.V.E.R - Main Menu
        public string SrvName1 { get; set; }
        public string SrvName2 { get; set; }
        public string SrvName3 { get; set; }
        public string SrvName4 { get; set; }
        public string SrvName5 { get; set; }
        public string SrvMaps1 { get; set; }
        public string SrvMaps2 { get; set; }
        public string SrvMaps3 { get; set; }
        public string SrvMaps4 { get; set; }
        public string SrvMaps5 { get; set; }
        public string SrvGameType1 { get; set; }
        public string SrvGameType2 { get; set; }
        public string SrvGameType3 { get; set; }
        public string SrvGameType4 { get; set; }
        public string SrvGameType5 { get; set; }
        public string svPing { get; set; }
        public string svPlayers { get; set; }
        public string svIco { get; set; }
        public string svTimeLim { get; set; }
        public string svWeatherTime { get; set; }
        public string Artefacts { get; set; }
        public string svTimeArtefact { get; set; }
        public string svFraglim { get; set; }
        public string svFriendlyFire { get; set; }
        public string svWurmUp { get; set; }
        public string PortCL1 { get; set; }
        public string PortSV1 { get; set; }
        public string PortGS1 { get; set; }
        public string PortCL2 { get; set; }
        public string PortSV2 { get; set; }
        public string PortGS2 { get; set; }
        public string PortCL3 { get; set; }
        public string PortSV3 { get; set; }
        public string PortGS3 { get; set; }
        public string PortCL4 { get; set; }
        public string PortSV4 { get; set; }
        public string PortGS4 { get; set; }
        public string PortCL5 { get; set; }
        public string PortSV5 { get; set; }
        public string PortGS5 { get; set; }
        public bool SrvMainFormLanguage { get; set; }
        public string myKeyCreated { get; set; }
        // ui
        public bool SRV_START_LINE { get; set; }
        public bool SRV_COPY_ERROR { get; set; }
        public int ARGB_SET_COLOR { get; set; }
        // priority class
        public bool SRV1_0x0001 { get; set; }
        public bool SRV1_0x0002 { get; set; }
        public bool SRV1_0x0004 { get; set; }
        public bool SRV1_0x0008 { get; set; }
        public bool SRV1_0x00010 { get; set; }
        public bool SRV1_0x00020 { get; set; }
        public bool SRV1_0x00040 { get; set; }
        public bool SRV1_0x00080 { get; set; }
        public bool SRV2_0x0001 { get; set; }
        public bool SRV2_0x0002 { get; set; }
        public bool SRV2_0x0004 { get; set; }
        public bool SRV2_0x0008 { get; set; }
        public bool SRV2_0x00010 { get; set; }
        public bool SRV2_0x00020 { get; set; }
        public bool SRV2_0x00040 { get; set; }
        public bool SRV2_0x00080 { get; set; }
        public bool SRV3_0x0001 { get; set; }
        public bool SRV3_0x0002 { get; set; }
        public bool SRV3_0x0004 { get; set; }
        public bool SRV3_0x0008 { get; set; }
        public bool SRV3_0x00010 { get; set; }
        public bool SRV3_0x00020 { get; set; }
        public bool SRV3_0x00040 { get; set; }
        public bool SRV3_0x00080 { get; set; }
        public bool SRV4_0x0001 { get; set; }
        public bool SRV4_0x0002 { get; set; }
        public bool SRV4_0x0004 { get; set; }
        public bool SRV4_0x0008 { get; set; }
        public bool SRV4_0x00010 { get; set; }
        public bool SRV4_0x00020 { get; set; }
        public bool SRV4_0x00040 { get; set; }
        public bool SRV4_0x00080 { get; set; }
    }
}