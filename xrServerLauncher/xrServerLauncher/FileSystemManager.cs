using System;
using System.IO;
using System.Windows.Forms;

namespace S.E.R.V.E.R___Shadow_Of_Chernobyl_1._0006
{
    class FileSystemManager
    {
        public static void CheckedDirectoryFiles() // ServerBasePlayers
        {
            try
            {
                if (!Directory.Exists(@"PlayersDataBase\Backup\"))
                {
                    Directory.CreateDirectory(@"PlayersDataBase\Backup\");
                }
                if (!File.Exists(@"PlayersDataBase\Players.xrBase"))
                {
                    var FileCreate = File.Create(@"PlayersDataBase\Players.xrBase");
                    FileCreate.Close();
                }
                if (!Directory.Exists(@"PlayersDataBase\html_cheater_base\"))
                {
                    Directory.CreateDirectory(@"PlayersDataBase\html_cheater_base\");
                }
                // get logs 
                if (!Directory.Exists(@"server_settings\server_logs\"))
                {
                    Directory.CreateDirectory(@"server_settings\server_logs\");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при созданий директории.\nРабота базы будет невозможной.\nReason:\n" + ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public static void GetLoadReNameMaps()     // Находим и переименовываем файл, если таковы есть
        {
            if (File.Exists(@"mods\server_christmas_maps.xdb0"))
            {
                try
                {
                    File.Move(@"mods\server_christmas_maps.xdb0", @"mods\server_christmas_maps.xrmaps");
                }
                catch (Exception)
                {

                }
            }
            if (File.Exists(@"mods\server_standart_maps.xdb0"))
            {
                try
                {
                    File.Move(@"mods\server_standart_maps.xdb0", @"mods\server_standart_maps.xrmaps");
                }
                catch (Exception)
                {

                }
            }
            if (File.Exists(@"mods\server_singles_2.6.xdb0"))
            {
                try
                {
                    File.Move(@"mods\server_singles_2.6.xdb0", @"mods\server_singles_2.6.xrmaps");
                }
                catch (Exception)
                {

                }
            }

            // DEACTIVATE SURVIVAL MODE
            if (File.Exists("gamedata.dbe"))
            {
                try
                {
                    long SizeOrigin = new FileInfo("gamedata.dbe").Length / 1024;
                    if (!(SizeOrigin == 7437))
                    {
                        File.Move("gamedata.dbe", "gamedata.xr2");                  
                        File.Move("gamedata.xr1", "gamedata.dbe");  // <!----- 7437         
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Произошла ошибка при проверке директории файлов.\nПожалуйста остановите все сервера с данной сборки и повторите попытку.", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public static void GetCheckMods()
        {
            if (File.Exists(@"gamedata\config\mp\deathmatch_game.ltx"))
            {
                DialogResult dialogResult = MessageBox.Show("Обнаружена серверная модификация режима Deathmatch.\nОна несовместима с режимом Artefacthunt, если Вы захотите запустить данный режим.\nХотите удалить модификацию Deathmatch прямо сейчас?\nВы в любой момент сможете применить ее, т.к настройки сохранены!", "S.E.R.V.E.R - Shadow Of Chernobyl - Deathmatch Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (dialogResult == DialogResult.Yes)
                {
                    try
                    {
                        File.Delete(@"gamedata\config\mp\deathmatch_game.ltx");
                        MessageBox.Show("Обнаружена и удалена модификация для режима Deathmatch.", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Произошла ошибка.\nПричина: " + ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    if (File.Exists(@"gamedata\config\mp\teamdeathmatch_game.ltx"))
                    {
                        try
                        {
                            File.Delete(@"gamedata\config\mp\teamdeathmatch_game.ltx");
                            MessageBox.Show("Обнаружена и удалена модификация для режима TeamDeathmatch.", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Произошла ошибка.\nПричина: " + ex.Message, "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
        }
    }
}      