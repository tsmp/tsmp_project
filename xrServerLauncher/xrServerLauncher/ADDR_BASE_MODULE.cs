using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace S.E.R.V.E.R___Shadow_Of_Chernobyl_1._0006
{
    class ADDR_BASE_MODULE
    {
        /*
        // ---------------------- Call of Pripyat 1.6.02 ---------------------
        // 438F7A -> nop (0x90) 2 byte == 2 nop
        // 438F92
        // xrgame.dll+438f7a // open 04DF9009
        // int Addr = 0x043F87A;    // Адрес функции je   CALL IF PRIPYAT 1.6.02

        // ---------------------- Shadow Of Chernobyl ---------------------


        // xrGameSpy+$B26E to xrGameSpy+$B281
        const int PROCESS_WM_READ = 0x0010;

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        public static void Main(int get_proc_address, string get_proc_name)
        {

            Process process = Process.GetProcessesByName("notepad")[0];
            IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, process.Id);

            int bytesRead = 0;
            byte[] buffer = new byte[24]; //'Hello World!' takes 12*2 bytes because of Unicode 

            // 0x0046A3B8 is the address where I found the string, replace it with what you found
            ReadProcessMemory((int)processHandle, 0x0046A3B8, buffer, buffer.Length, ref bytesRead);

            Console.WriteLine(Encoding.Unicode.GetString(buffer) + " (" + bytesRead.ToString() + "bytes)");
            Console.ReadLine();
        }

        // Write Process Memory

        const int PROCESS_ALL_ACCESS = 0x1F0FFF;  
        [DllImport("kernel32.dll")]
        public static extern IntPtr ROpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);

        public static void WriteProcessMemory(int get_proc_address, string get_proc_name)
        {

            Process process = Process.GetProcessesByName("notepad")[0];
            IntPtr processHandle = ROpenProcess(PROCESS_ALL_ACCESS, false, process.Id);
            int bytesWritten = 0;
            byte[] buffer = Encoding.Unicode.GetBytes("It works!\0"); // '\0' marks the end of string

            // replace 0x0046A3B8 with your address
            WriteProcessMemory((int)processHandle, 0x0046A3B8, buffer, buffer.Length, ref bytesWritten);
            Console.ReadLine();
        }

        */






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
    }
}
