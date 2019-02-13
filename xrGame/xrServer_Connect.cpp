#include "stdafx.h"
#include "xrServer.h"
#include "game_sv_single.h"
#include "game_sv_deathmatch.h"
#include "game_sv_teamdeathmatch.h"
#include "game_sv_artefacthunt.h"
#include "xrMessages.h"
#include "game_cl_artefacthunt.h"
#include "game_cl_single.h"
#include "MainMenu.h"

#include "sysmsgs.h"

extern bool bIsDedicatedServer;
extern std::string g_sv_mp_loader_ip;
extern int g_sv_mp_ModLoaderEnabled;
bool bIsSysMsgsDllLoaded = false;
HMODULE dll;

#pragma warning(push)
#pragma warning(disable:4995)
#include <malloc.h>
#pragma warning(pop)

xrServer::EConnect xrServer::Connect(shared_str &session_name)
{
#ifdef DEBUG
	Msg("* sv_Connect: %s",	*session_name);
#endif

	// Parse options and create game
	if (0==strchr(*session_name,'/'))
		return				ErrConnect;

	string1024				options;
	R_ASSERT2(xr_strlen(session_name) <= sizeof(options), "session_name too BIIIGGG!!!");
	strcpy					(options,strchr(*session_name,'/')+1);
	
	// Parse game type
	string1024				type;
	R_ASSERT2(xr_strlen(options) <= sizeof(type), "session_name too BIIIGGG!!!");
	strcpy					(type,options);
	if (strchr(type,'/'))	*strchr(type,'/') = 0;
	game					= NULL;

	CLASS_ID clsid			= game_GameState::getCLASS_ID(type,true);
	game					= smart_cast<game_sv_GameState*> (NEW_INSTANCE(clsid));

	// Options
	if (0==game)			return ErrConnect;
	csPlayers.Enter			();
//	game->type				= type_id;
#ifdef DEBUG
	Msg("* Created server_game %s",game->type_name());
#endif

	game->Create			(session_name);
	csPlayers.Leave			();
	
#ifdef BATTLEYE
	if ( game->get_option_i( *session_name, "battleye", 1) != 0 ) // default => battleye enable (always)
	{
		// if level exist & if server in internet
		if ( g_pGameLevel && (game->get_option_i( *session_name, "public", 0) != 0)  )
		{
			if ( Level().battleye_system.server )
			{
				Msg( "Warning: BattlEye already loaded!" );
			}
			else
			{
				if ( !Level().battleye_system.LoadServer( this ) )
				{
					return ErrBELoad;
				}
			}
		}//g_pGameLevel
	}
#endif // BATTLEYE
	
	return IPureServer::Connect(*session_name);
}

void xrServer::CheckPlayerName(string1024 &sName)
{
	if (strlen(sName) > 70)
	{
		string128 sTemp;
		strncpy(sTemp, sName, 70);
		strcpy(sName, sTemp);

		Msg("! Player tried to everride max nickname length");
	}

	int iLen = strlen(sName);

	if (iLen < 2) strcpy(sName, "�����");

	for (int i = 0; i < iLen; ++i)
	{
		switch (sName[i])
		{
		case '%':
		case ' ':
		case '#':
		case '!':
		case '~':
		case '*':
		{
			sName[i] = '_';
		}
		}
	}
}


IClient* xrServer::new_client(SClientConnectData* cl_data)
{
	IClient* CL = client_Find_Get(cl_data->clientID);
	VERIFY(CL);
	
	// copy entity
	CL->ID			= cl_data->clientID;
	CL->process_id	= cl_data->process_id;
	
	string1024 sNewClientName;
	strcpy_s(sNewClientName, cl_data->name);

	if(cl_data->new_code[0])
		Msg("- Player connecting with TSMP!");

	Msg("- Connecting player - %s", sNewClientName);

	CheckPlayerName(sNewClientName);

	if (game->NewPlayerName_Exists(CL, sNewClientName))
	{
		game->NewPlayerName_Generate(CL, sNewClientName);
		game->NewPlayerName_Replace(CL, sNewClientName);
	}

	CL->name._set(sNewClientName);
	CL->pass._set(cl_data->pass);

	NET_Packet P;
	P.B.count	= 0;
	P.r_pos		= 0;
	
	game->AddDelayedEvent(P, GAME_EVENT_CREATE_CLIENT, 0, CL->ID);

	if (client_Count() == 1)
	{
		Update();
	}
	return CL;
}

void xrServer::SendCB(void* msg, unsigned int len, void* userdata)  // ��� ������� �� �����
{
	((SMyUserData*)userdata)->server->IPureServer::SendTo_LL(((SMyUserData*)userdata)->idOfPlayer, msg, len, net_flags(TRUE, TRUE, TRUE, TRUE));
};

void xrServer::UnloadDll()
{	
	if (bIsSysMsgsDllLoaded)
	{		
		FZSysMsgsFree FreeFZ = (FZSysMsgsFree)GetProcAddress(dll, "FZSysMsgsFree");
		FreeFZ();
		FreeLibrary(dll);

		Msg("- DLL unloaded");
	}
};

void xrServer::AttachNewClient(IClient* CL)
{
	if (bIsDedicatedServer)
	{
		if (g_sv_mp_ModLoaderEnabled == 1)
		{
			static FZSysMsgsSendSysMessage_SOC		SendSysMessage;
			static FZSysMsgsProcessClientModDll		writer;

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

				writer = (FZSysMsgsProcessClientModDll)GetProcAddress(dll, "FZSysMsgsProcessClientModDll");

				(*SysInit)();

				SetCommonSysmsgsFlags SetFlags;
				SetFlags = (SetCommonSysmsgsFlags)GetProcAddress(dll, "FZSysMsgsSetCommonSysmsgsFlags");

				SetFlags(FZ_SYSMSGS_ENABLE_LOGS | FZ_SYSMSGS_PATCH_UI_PROGRESSBAR);
			}

			SMyUserData userdata = {};
			userdata.idOfPlayer = CL->ID;
			userdata.server = this;

			FZDllDownloadInfo moddllinfo = {};

			moddllinfo.fileinfo.filename = "";
			moddllinfo.fileinfo.url = "";
			moddllinfo.fileinfo.crc32 = 0x274A4EBD;
			moddllinfo.fileinfo.progress_msg = "In Progress"; //���������, ��������� ������������ �� ����� �������
			moddllinfo.fileinfo.error_already_has_dl_msg = "Error happens";  //���������, ��������� ������������ ��� ������������� ������ �� ����� �������
			moddllinfo.fileinfo.compression = FZ_COMPRESSION_NO_COMPRESSION; //������������ ��� ����������
			moddllinfo.procname = "ModLoad";  //��� ��������� � dll ����, ������� ������ ���� �������; ������ ����� ��� FZDllModFun
			moddllinfo.procarg1 = "tsmp"; //��������� ��� �������� � ���������

			ip_address Address;
			DWORD dwPort = 0;

			Level().GetServerAddress(Address, &dwPort);

			std::string procargs2 = "-srv " + g_sv_mp_loader_ip + " -srvport " + std::to_string(dwPort);


			char ar[30];
			strcpy(ar, procargs2.c_str());

			moddllinfo.procarg2 = ar; //��������� ��� �������� � ���������
			moddllinfo.dsign = "";
			moddllinfo.name_lock = "123";  //�������� ������� ��� ����������� DLL - ����������� ����� ���, ��� �������� ���������� � ������� ����
			moddllinfo.reconnect_addr.ip = "127.0.0.1";  //IP-����� � ���� ��� ����������. ���� IP �������, �� ��������� ���������� ������������� ������� ����� �� ���, �� ����� ������� ��������� ����������.
			moddllinfo.reconnect_addr.port = 5445; // ����

			/*
			-binlist <URL> - ������ �� �����, �� �������� ������� ������ ������ ������ (��� ������ ��������� ������ ������� � ������ -fz_custom_bin)
			-gamelist <URL> - ������ �� �����, �� �������� ������� ������ ������ ���� (����������\������)
			-srv <IP> - IP-����� �������, � �������� ���������� �������������� ����� ������� ����
			-srvname <domainname> - �������� ���, �� �������� ������������� ������. ����� ������������ ������ ��������� -srv � ������ ������������� IP �������
			-port <number> - ���� �������
			-gamespymode - ��������� ������������ �������� ���������� GameSpy
			-fullinstall - ��� ������������ ����� ��������������� ����� ����, ����� � ������� ������������ �� ���������
			-sharedpatches - ������������ ����� � ������������ ���� ���������� ������
			-logsev <number> - ������� ����������� ���������� ���������, �� ��������� FZ_LOG_ERROR
			-configsdir <string> - ���������� ��������
			-exename <string> - ��� ������������ ����� ����
			*/

			SendSysMessage(writer, &moddllinfo, xrServer::SendCB, &userdata);
		}
	}

	MSYS_CONFIG	msgConfig;
	msgConfig.sign1 = 0x12071980;
	msgConfig.sign2 = 0x26111975;
	msgConfig.is_battleye = 0;

#ifdef BATTLEYE
		msgConfig.is_battleye = (g_pGameLevel && Level().battleye_system.server != 0)? 1 : 0;
#endif // BATTLEYE
	
			if (psNET_direct_connect)  //single_game
			{
				SV_Client = CL;
				CL->flags.bLocal = 1;
				SendTo_LL(SV_Client->ID, &msgConfig, sizeof(msgConfig), net_flags(TRUE, TRUE, TRUE, TRUE));
			}
			else
			{
				SendTo_LL(CL->ID, &msgConfig, sizeof(msgConfig), net_flags(TRUE, TRUE, TRUE, TRUE));
				Server_Client_Check(CL);				
			}		

	// gen message
	if (!NeedToCheckClient_GameSpy_CDKey(CL))
		Check_GameSpy_CDKey_Success(CL);	

	CL->m_guid[0]=0;
}
