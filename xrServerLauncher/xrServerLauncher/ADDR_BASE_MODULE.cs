using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        /*
        [Flags] //AccessRights
        public enum ProcessAccessRights
        {
            PROCESS_VM_READ = (0x0010),
            PROCESS_VM_WRITE = (0x0020),
            PROCESS_VM_OPERATION = (0x0008)
        }

        [DllImport("kernel32.dll")] //OpenProcess function
        public static extern IntPtr OpenProcess(UInt32 dwDesiredAccess, Int32 bInheiritHandle, UInt32 dwProcessId);
        [DllImport("kernel32.dll")] //CloseHandle function
        public static extern Int32 CloseHandle(IntPtr hObject);
        [DllImport("kernel32.dll")] //ReadProcessMemory function
        public static extern Int32 ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, UInt32 nSize, ref UInt32 lpNumberOfBytesRead);
        [DllImport("kernel32.dll")] //WriteProcessMemory function
        public static extern Int32 WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In] byte[] lpBuffer, UInt32 nSize, ref UInt32 lpNumberOfBytesRead);
        [DllImport("kernel32.dll")] //GetLastError function
        public static extern UInt32 GetLastError();
        [DllImport("kernel32.dll")] //SetLastError function
        public static extern void SetLastError(UInt32 dwErrorCode);

        private void button1_Click(object sender, EventArgs e)
        {
            Process[] MyProcess = Process.GetProcessesByName("ProcessName");
            if (MyProcess.Length == 0)
            {
                MessageBox.Show("Please run Conquer online!");
                return;
            }
            IntPtr hprocess = OpenProcess((uint)ProcessAccessRights.PROCESS_VM_WRITE, 1, (uint)MyProcess[0].Id);
            if (hprocess.ToInt32() == 0)
            {
                MessageBox.Show("Handle Failed");
            }
            uint num = 0;
            Int32 y = 128;
            byte[] buffer = BitConverter.GetBytes(y);
            Int32 writeresult = WriteProcessMemory(hprocess, (IntPtr)0xFC1EFE, buffer, 4, ref num);
            if (writeresult == 0)
            {
                UInt32 lastError = GetLastError();
                MessageBox.Show(lastError.ToString());
            }
            CloseHandle(hprocess);
        }*/
    }
}




/*
 * //...и отключаем отправку пакета с запросом
srcInjectionWithConditionalJump.Create(pointer(xrGameSpy+$B26E),@OnAuthSend,8,[F_PUSH_ESI],pointer(xrGameSpy+$B281), JUMP_IF_TRUE, true, false);

//[bug] Если уже убитый игрок до нажатия стрельбы (т.е. до перехода в спектаторы) выберет переход в наблюдатели, то на карте останется неудаляемый труп.
//Исправление - на манер ЧН, в game_sv_mp::OnPlayerSelectSpectator добавим вызовы AllowDeadBodyRemove	и m_CorpseList.push_back.
//Для упрощения жизни  - вызовем напрямую game_sv_mp::RespawnPlayer
tmp:=$9090cb8b; //(mov ecx, ebx; nop; nop
srcKit.CopyBuf(@tmp, pointer(xrGame+$2db327), sizeof(tmp));
srcBaseInjection.Create(pointer(xrGame+$2db329), pointer(xrGame+$2d8b70), 9, [F_PUSH_EBP, F_PUSHCONST+0], false, true);
result:=true;*/
