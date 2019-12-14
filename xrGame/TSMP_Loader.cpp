#include "StdAfx.h"
#include "xrserver.h"
#include "TSMP_Loader.h"
#include "sysmsgs.h"
#include "Level.h"
#include "..\xrdownloader\xrdownloader.h"

extern std::string g_sv_mp_loader_ip;
extern std::string TSMP_Loader_Mod_Name;

extern int g_sv_mp_LoaderEnabled;
extern int g_sv_mp_LoaderMap;

bool LoadedDLL = false;
HMODULE dll;

FZSysMsgsSendSysMessage_SOC		ProcSendSysMessage;
FZSysMsgsProcessClientModDll	ProcProcessClientMod;
FZSysMsgsProcessClientMap		ProcProcessClientMap;

static void __stdcall SendCallback(void* msg, unsigned int len, void* userdata)
{
	((SMyUserData*)userdata)->server->IPureServer::SendTo_LL(((SMyUserData*)userdata)->idOfPlayer, msg, len, net_flags(TRUE, TRUE, TRUE, TRUE));
}

void LoadDLL()
{
	if (!LoadedDLL)
	{
		dll = LoadLibrary("sysmsgs.dll");

		if (dll)
		{
			FZSysMsgsInit SysInit;
			SysInit = (FZSysMsgsInit)GetProcAddress(dll, "FZSysMsgsInit");
			(*SysInit)();		

			ProcSendSysMessage = (FZSysMsgsSendSysMessage_SOC)GetProcAddress(dll, "FZSysMsgsSendSysMessage_SOC");
			ProcProcessClientMap = (FZSysMsgsProcessClientMap)GetProcAddress(dll, "FZSysMsgsProcessClientMap");
			ProcProcessClientMod= (FZSysMsgsProcessClientModDll)GetProcAddress(dll, "FZSysMsgsProcessClientModDll");
			
			SetCommonSysmsgsFlags SetFlags;
			SetFlags = (SetCommonSysmsgsFlags)GetProcAddress(dll, "FZSysMsgsSetCommonSysmsgsFlags");
			SetFlags(FZ_SYSMSGS_ENABLE_LOGS | FZ_SYSMSGS_PATCH_UI_PROGRESSBAR);

			LoadedDLL = true;
			Msg("- sysmsgs.dll loaded successfully");
		}
		else
			Msg("! error, cant load dll with sysmsgs api");
	}
}

void UnloadSysmsgs()
{
	if (LoadedDLL)
		FreeLibrary(dll);

	LoadedDLL = false;
}

void DownloadingMod(xrServer* server, ClientID ID);
void DownloadingMap(xrServer* server, ClientID ID);

void TSMP_LoaderExecute(xrServer * server, ClientID ID)
{
	if (g_sv_mp_LoaderEnabled)
	{
		LoadDLL();

		if (!LoadedDLL)
			return;

		Msg("- Sending magic packet to him");

		if (g_sv_mp_LoaderMap)
			DownloadingMap(server, ID);
		else
			DownloadingMod(server, ID);
	}
}

void DownloadingMod(xrServer* server, ClientID ID)
{
	SMyUserData userdata = {};
	userdata.idOfPlayer = ID;
	userdata.server = server;

	FZDllDownloadInfo moddllinfo = {};

	moddllinfo.fileinfo.filename = "";
	moddllinfo.fileinfo.url = "";
	moddllinfo.fileinfo.crc32 = 0x274A4EBD;
	moddllinfo.fileinfo.progress_msg = "In Progress";
	moddllinfo.fileinfo.error_already_has_dl_msg = "Error happens";
	moddllinfo.fileinfo.compression = FZ_COMPRESSION_NO_COMPRESSION;
	moddllinfo.procname = "ModLoad";

	moddllinfo.procarg1 = TSMP_Loader_Mod_Name.data();

	ip_address Address;

	DWORD dwPort = 0;

	Level().GetServerAddress(Address, &dwPort);

	std::string procargs2 = "-srv " + g_sv_mp_loader_ip + " -srvport " + std::to_string(dwPort);
	
	moddllinfo.procarg2 = procargs2.data(); //Аргументы для передачи в процедуру
	moddllinfo.dsign = "";
	moddllinfo.name_lock = "123";  //Цифровая подпись для загруженной DLL - проверяется перед тем, как передать управление в функцию мода
	moddllinfo.reconnect_addr.ip = "127.0.0.1";  //IP-адрес и порт для реконнекта. Если IP нулевой, то параметры реконнекта автоматически берутся игрой из тех, во время которых произошел дисконнект.
	moddllinfo.reconnect_addr.port = 5445; // Порт

	ProcSendSysMessage(ProcProcessClientMod, &moddllinfo, SendCallback, &userdata);
}

void DownloadingMap(xrServer* server, ClientID ID)
{
	SMyUserData userdata = {};
	userdata.idOfPlayer = ID;
	userdata.server = server;

	ip_address Address;
	DWORD dwPort = 0;
	Level().GetServerAddress(Address, &dwPort);

	FZMapInfo mapinfo = {};

	u32 cmpr, crc2;
	std::string Filename, URL,LName;
	LName = Level().name().c_str();

	FillMapParams(Filename,URL
		, cmpr
		, crc2
		, LName
	);

	mapinfo.fileinfo.compression = cmpr;
	mapinfo.fileinfo.crc32 = crc2;
	mapinfo.fileinfo.url = const_cast<char*>(URL.c_str());
	mapinfo.fileinfo.filename = const_cast<char*>(Filename.c_str());
	mapinfo.mapname = const_cast<char*>(LName.c_str());

	//mapinfo.fileinfo.filename = "military_kuznya_1.0.xdb.map";
	//mapinfo.fileinfo.url = "http://82.202.249.152/compressed_maps_shoc/military_kuznya.cab";
	//mapinfo.fileinfo.compression = FZ_COMPRESSION_CAB_COMPRESSION;
	//mapinfo.fileinfo.crc32 = 936722695;
	mapinfo.fileinfo.progress_msg = "Идет загрузка карты. Пожалуйста подождите. Map is downloading. Wait please.";
	mapinfo.fileinfo.error_already_has_dl_msg = "Error happened";


	mapinfo.flags = FZ_MAPLOAD_MANDATORY_RECONNECT;
	mapinfo.mapver = "1.0";
	mapinfo.reconnect_addr.ip = g_sv_mp_loader_ip.data();
	mapinfo.reconnect_addr.port = dwPort;
	mapinfo.xmlname = nullptr;

	ProcSendSysMessage(ProcProcessClientMap, &mapinfo, SendCallback, &userdata);
}

/*

void xrServer::UnloadDll()
{
	if (bIsSysMsgsDllLoaded)
	{
		Msg("unloading dll");

	//	FZSysMsgsFree FreeFZ = (FZSysMsgsFree)GetProcAddress(dll, "FZSysMsgsFree");

	//	FreeFZ();
#pragma todo("tsmp: разобраться почему вылет")
		FreeLibrary(dll);

		Msg("- DLL unloaded");
	}
};



void xrServer::SendCB(void* msg, unsigned int len, void* userdata)  // Для колбека от дллки
{
	((SMyUserData*)userdata)->server->IPureServer::SendTo_LL(((SMyUserData*)userdata)->idOfPlayer, msg, len, net_flags(TRUE, TRUE, TRUE, TRUE));
};

static void	__stdcall				SendCB(void* msg, unsigned int len, void* userdata);

extern std::string g_sv_mp_loader_ip;
extern int g_sv_mp_ModLoaderEnabled;
bool bIsSysMsgsDllLoaded = false;
HMODULE dll;

if (g_sv_mp_ModLoaderEnabled == 1)
		{
			static FZSysMsgsSendSysMessage_SOC		SendSysMessage;
			//static FZSysMsgsProcessClientModDll		writer;
			static FZSysMsgsProcessClientMap map_writer;

			Msg("- Sending magic packet to - %s", CL->name.c_str());

			if (!bIsSysMsgsDllLoaded)
			{
				Msg("- Loading dll");

				dll = LoadLibrary("sysmsgs.dll");

				if (dll == nullptr)
					Msg("! Cant load dll");
				else
				{
					bIsSysMsgsDllLoaded = true;
					Msg("- DLL loaded successfully");
				}

				SendSysMessage = (FZSysMsgsSendSysMessage_SOC)GetProcAddress(dll, "FZSysMsgsSendSysMessage_SOC");

				FZSysMsgsInit SysInit;
				SysInit = (FZSysMsgsInit)GetProcAddress(dll, "FZSysMsgsInit");

				//writer = (FZSysMsgsProcessClientModDll)GetProcAddress(dll, "FZSysMsgsProcessClientModDll");
				map_writer = (FZSysMsgsProcessClientMap)GetProcAddress(dll, "FZSysMsgsProcessClientMap");

				(*SysInit)();

				SetCommonSysmsgsFlags SetFlags;
				SetFlags = (SetCommonSysmsgsFlags)GetProcAddress(dll, "FZSysMsgsSetCommonSysmsgsFlags");

				SetFlags(FZ_SYSMSGS_ENABLE_LOGS | FZ_SYSMSGS_PATCH_UI_PROGRESSBAR);
			}

			SMyUserData userdata = {};
			userdata.idOfPlayer = CL->ID;
			userdata.server = this;

			FZDllDownloadInfo moddllinfo = {};

			FZMapInfo mapinfo = {};

			mapinfo.fileinfo.filename = "military_kuznya_1.0.xdb.map";
			mapinfo.fileinfo.url = "http://82.202.249.152/compressed_maps_shoc/military_kuznya.cab";
			mapinfo.fileinfo.compression = FZ_COMPRESSION_CAB_COMPRESSION;
			mapinfo.fileinfo.crc32 = 936722695;
			mapinfo.fileinfo.progress_msg = "In Progress"; //Сообщение, выводимое пользователю во время закачки
			mapinfo.fileinfo.error_already_has_dl_msg = "Error happens";

			mapinfo.flags = FZ_MAPLOAD_MANDATORY_RECONNECT;
			mapinfo.mapname = "military_kuznya";
			mapinfo.mapver = "1.0";
			mapinfo.reconnect_addr.ip = "192.168.0.22";
			mapinfo.reconnect_addr.port = 5445;
			mapinfo.xmlname = nullptr;


			moddllinfo.fileinfo.filename = "";
			moddllinfo.fileinfo.url = "";
			moddllinfo.fileinfo.crc32 = 0x274A4EBD;
			moddllinfo.fileinfo.progress_msg = "In Progress"; //Сообщение, выводимое пользователю во время закачки
			moddllinfo.fileinfo.error_already_has_dl_msg = "Error happens";  //Сообщение, выводимое пользователю при возникновении ошибки во время закачки
			moddllinfo.fileinfo.compression = FZ_COMPRESSION_NO_COMPRESSION; //Используемый тип компрессии
			moddllinfo.procname = "ModLoad";  //Имя процедуры в dll мода, которая должна быть вызвана; должна иметь тип FZDllModFun

			if (strstr(Core.Params, "-tsmp_debug"))
				moddllinfo.procarg1 = "tsmp_debug"; //Аргументы для передачи в процедуру
			else
				moddllinfo.procarg1 = "tsmp"; //Аргументы для передачи в процедуру

			ip_address Address;

			DWORD dwPort = 0;

			Level().GetServerAddress(Address, &dwPort);

			std::string procargs2 = "-srv " + g_sv_mp_loader_ip + " -srvport " + std::to_string(dwPort);


			char ar[30];
			strcpy(ar, procargs2.c_str());

			moddllinfo.procarg2 = ar; //Аргументы для передачи в процедуру
			moddllinfo.dsign = "";
			moddllinfo.name_lock = "123";  //Цифровая подпись для загруженной DLL - проверяется перед тем, как передать управление в функцию мода
			moddllinfo.reconnect_addr.ip = "127.0.0.1";  //IP-адрес и порт для реконнекта. Если IP нулевой, то параметры реконнекта автоматически берутся игрой из тех, во время которых произошел дисконнект.
			moddllinfo.reconnect_addr.port = 5445; // Порт

			
			-binlist <URL> - ссылка на адрес, по которому берется список файлов движка (для работы требуется запуск клиента с ключлм -fz_custom_bin)
			-gamelist <URL> - ссылка на адрес, по которому берется список файлов мода (геймдатных\патчей)
			-srv <IP> - IP-адрес сервера, к которому необходимо присоединиться после запуска мода
			-srvname <domainname> - доменное имя, по которому располагается сервер. Можно использовать вместо параметра -srv в случае динамического IP сервера
			-port <number> - порт сервера
			-gamespymode - стараться использовать загрузку средствами GameSpy
			-fullinstall - мод представляет собой самостоятельную копию игры, связь с файлами оригинальной не требуется
			-sharedpatches - использовать общую с инсталляцией игры директорию патчей
			-logsev <number> - уровень серьезности логируемых сообщений, по умолчанию FZ_LOG_ERROR
			-configsdir <string> - директория конфигов
			-exename <string> - имя исполняемого файла мода
			

			//SendSysMessage(writer, &moddllinfo, xrServer::SendCB, &userdata);
SendSysMessage(map_writer, &mapinfo, xrServer::SendCB, &userdata);
		}

*/