using System;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using NetFwTypeLib; // WindowsFirewallAPI

namespace S.E.R.V.E.R___Shadow_Of_Chernobyl_1._0006
{
    class IPBlockedServices
    {
        // http://www.zonepc.ru/nastrojka-brandmauera-windows-iz-komandnoj-stroki/
        // https://www.osp.ru/winitpro/2012/17/13017914/
        // https://firstvds.ru/blog/windows_rabota_s_fayrvolom_iz_komandnoy_stroki

        public static void FirewallRuleServerHideCreate(string localport)
        {
            try
            {
                string name = "STALKER_SRV [HIDE] " + localport + " Flags:[null] IP:  null Time: " + (DateTime.Now.ToString("dd.MM.yyyy---HH:mm:ss"));
                Process RuleCreate = new Process();
                RuleCreate.StartInfo.FileName = "netsh";
                RuleCreate.StartInfo.Arguments = string.Format(@"advfirewall firewall add rule name=""" + name + "\" protocol=UDP action=BLOCK dir=IN localport=" + localport);
                RuleCreate.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                RuleCreate.StartInfo.CreateNoWindow = true;
                RuleCreate.StartInfo.UseShellExecute = false;
                RuleCreate.StartInfo.RedirectStandardOutput = true;
                RuleCreate.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(866);
                RuleCreate.Start();
                string result_info = RuleCreate.StandardOutput.ReadToEnd();
                RuleCreate.WaitForExit();
                RuleCreate.Close();
             /*   if (result_info.Length > 50)            
                    MessageBox.Show("Данные не были добавлены.", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Warning);               
                else              
                    MessageBox.Show("Данные были успешно добавлены", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);*/
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex, "Ошибка в момент вызова функции FireWall [1]", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
   
       
        public static void FirewallNewRuleCreate(string text, string remoteip, bool SubnetFlag, string SubnetMaskByte)
        {
            try
            {
                INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
                INetFwRule FW_RULE = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
                FW_RULE.Action = NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
                FW_RULE.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;  // Входящее соединение
                FW_RULE.Enabled = true;                                         // устанавливает флаг - включен/отключен
                FW_RULE.InterfaceTypes = "All";                                 // тип сетевого интерфейса
                 
                if ((remoteip == " 0.0.0.0" || remoteip == " 127.0.0.1" || remoteip == " 255.255.255.255"))
                    return;
                else if (remoteip.Length == 0)
                    return;

                if (SubnetFlag == true)
                    remoteip = remoteip + "/" + SubnetMaskByte;

                if (text.Contains("[CHEATER] "))
                {
                    FW_RULE.RemoteAddresses = remoteip;
                    FW_RULE.Description = text + (DateTime.Now) + " IP: " + remoteip;
                    FW_RULE.Name = "STALKER_SRV" + text + " " + remoteip + " Time: " + (DateTime.Now.ToString("dd.MM.yyyy---HH:mm:ss")); 
                }
                else if (text.Contains("[HWEAPONS] "))
                {
                    FW_RULE.RemoteAddresses = remoteip;
                    FW_RULE.Description = text + (DateTime.Now) + " IP: " + remoteip;
                    FW_RULE.Name = "STALKER_SRV" + text + " " + remoteip + " Time: " + (DateTime.Now.ToString("dd.MM.yyyy---HH:mm:ss"));
                }
                else if (text.Contains("[ADMIN] "))
                {
                    FW_RULE.RemoteAddresses = remoteip;
                    FW_RULE.Description = text + " " + (DateTime.Now) + " IP: " + remoteip;
                    FW_RULE.Name = "STALKER_SRV" + text + " " + remoteip + " Time: " + (DateTime.Now.ToString("dd.MM.yyyy---HH:mm:ss"));
                }

                else if (text.Contains("[LIST] "))
                {                 
                    DialogResult addressmask = MessageBox.Show("Если Вы хотите заблокировать подсети игроков - нажмите ДА\nЕсли Вы хотите заблокировать только адреса - нажмите НЕТ\nЕсли Вы хотите отменить блокировку - Нажмите ОТМЕНА", "Выберите вариант блокировки", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                    if (addressmask == DialogResult.Yes)                        // subnet
                    {
                        remoteip = remoteip.Replace(",", "/24,").Replace(" ", string.Empty);
                    }
                    else if (addressmask == DialogResult.No)                    // no mask
                    {
                        remoteip = remoteip.Replace(" ", string.Empty);
                    }
                    else
                    {
                        remoteip = null;
                        MessageBox.Show("Выполнение было отменено.", "S.E.R.V.E.R - Shadow Of Chernobyl", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    if (remoteip.Length > 0)
                        remoteip = remoteip.Substring(0, remoteip.Length - 1);

                    FW_RULE.RemoteAddresses = remoteip;                       
                    FW_RULE.Description = text + " " + (DateTime.Now) + " IP: " + remoteip;
                    FW_RULE.Name = "STALKER_SRV" + text + " null Time: " + (DateTime.Now.ToString("dd.MM.yyyy---HH:mm:ss")); 
                }
                var cmp_address = FW_RULE.Name.Split()[6];
                foreach (INetFwRule rule in firewallPolicy.Rules)
                {
                    if (rule.Name.Contains("STALKER_SRV"))
                    {
                        var get_address = rule.Name.Split()[6];
                        if (get_address == cmp_address)                    
                            return;                       
                    }
                }
                firewallPolicy.Rules.Add(FW_RULE);
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message + ex, "Ошибка в момент вызова функции FireWall [3]", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void CleanAllRules(string FirewallMsg)
        {
            try
            {
                INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
                foreach (INetFwRule rule in firewallPolicy.Rules)
                {
                    if (rule.Name.Contains("STALKER_SRV " + FirewallMsg))
                    {
                        firewallPolicy.Rules.Remove(rule.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex, "ERROR CleanAllRules", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}