#include "stdafx.h"
#include "level.h"
#include "xrServer.h"
#include "xrMessages.h"
#include "xrGameSpyServer.h"
#include "../igame_persistent.h"
#include "GameSpy/GameSpy_Base_Defs.h"
#include "GameSpy/GameSpy_Available.h"
#include "..\TSMP_BuildConfig.h"

extern bool bIsDedicatedServer;

xrGameSpyServer::xrGameSpyServer()
{
	m_iReportToMasterServer = 0;
	m_bQR2_Initialized = FALSE;
	m_bCDKey_Initialized = FALSE;
	m_bCheckCDKey = false;

	if (0 != strstr(Core.Params, "-check_cd_key"))
		m_bCheckCDKey = true;

	ServerFlags.set(server_flag_all, 0);
	iGameSpyBasePort = 0;
}

xrGameSpyServer::~xrGameSpyServer()
{
	CDKey_ShutDown();
	QR2_ShutDown();
}

bool xrGameSpyServer::HasPassword() { return !!ServerFlags.test(server_flag_password); }
bool xrGameSpyServer::IsProtectedServer() { return !!ServerFlags.test(server_flag_protected); }

//----------- xrGameSpyClientData -----------------------
IClient *xrGameSpyServer::client_Create()
{
	return xr_new<xrGameSpyClientData>();
}

xrGameSpyClientData::xrGameSpyClientData() :xrClientData()
{
	m_bCDKeyAuth = false;
	m_iCDKeyReauthHint = 0;
}

void xrGameSpyClientData::Clear()
{
	inherited::Clear();

	m_pChallengeString[0] = 0;
	m_bCDKeyAuth = false;
	m_iCDKeyReauthHint = 0;
};

xrGameSpyClientData::~xrGameSpyClientData()
{
	m_pChallengeString[0] = 0;
	m_bCDKeyAuth = false;
	m_iCDKeyReauthHint = 0;
}

xrGameSpyServer::EConnect xrGameSpyServer::Connect(shared_str &session_name)
{
	EConnect res = inherited::Connect(session_name);

	if (res != ErrNoError)
		return res;

	if (0 == *(game->get_option_s(*session_name, "hname", nullptr)))
	{
		string1024	CompName;
		DWORD		CompNameSize = 1024;

		if (0 != GetComputerName(CompName, &CompNameSize))
			HostName._set(CompName);
	}
	else
		HostName._set(game->get_option_s(*session_name, "hname", nullptr));

	if (0!=*(game->get_option_s(*session_name, "psw", nullptr)))
		Password._set(game->get_option_s(*session_name, "psw", nullptr));
	
	string4096	tMapName = "";
	const char* SName = *session_name;
	strncpy(tMapName, *session_name, strchr(SName, '/') - SName);

	MapName._set(tMapName);
	MapNameRus._set(MapName);
	
	string_path cfg_full_name, cfg_name = "level.name";
	FS.update_path(cfg_full_name, "$level$", cfg_name);

	if (!FS.exist(cfg_full_name))
		strcpy_s(cfg_full_name, cfg_name);

	IReader* F = FS.r_open(cfg_full_name);

	string1024 str;

	if (F)
	{
		F->r_string(str, sizeof(str));

		if (sizeof(str))
			MapNameRus._set(str);

		FS.r_close(F);
		Msg("[%s] successfully loaded.", cfg_full_name);
	}
	else
		Msg("! Cannot open script file [%s]", cfg_full_name);

	m_iReportToMasterServer = game->get_option_i(*session_name, "public", 0);
	m_iMaxPlayers = game->get_option_i(*session_name, "maxplayers", 64);

	if (0 != strstr(Core.Params, "-check_cd_key"))
		m_bCheckCDKey = true;
	else
		m_bCheckCDKey = false;

	if (game->Type() != GAME_SINGLE)
	{
		//----- Check for Backend Services ---
		CGameSpy_Available GSA;
		shared_str result_string;

		if (!GSA.CheckAvailableServices(result_string))
			Msg(*result_string);

		//------ Init of QR2 SDK -------------
		iGameSpyBasePort = game->get_option_i(*session_name, "portgs", -1);
		QR2_Init(iGameSpyBasePort);

		if (m_bCheckCDKey)
			CDKey_Init();
	};

	return res;
}

void			xrGameSpyServer::Update()
{
	inherited::Update();

	if (m_bQR2_Initialized)
		m_QR2.Think(nullptr);	

	if (m_bCDKey_Initialized)
		m_GCDServer.Think();
	
	static u32 next_send_time = Device.dwTimeGlobal + 10000;

	if (Device.dwTimeGlobal >= next_send_time)
	{
		next_send_time = Device.dwTimeGlobal + 5000;
		NET_Packet						Packet;
		Packet.w_begin(M_MAP_SYNC);
		Packet.w_stringZ(MapName);
		SendBroadcast(BroadcastCID, Packet, net_flags(TRUE, TRUE));
	}
}

int	xrGameSpyServer::GetPlayersCount()
{
	int NumPlayers = client_Count();

	if (!bIsDedicatedServer || NumPlayers < 1)
		return NumPlayers;

	if (0 != strstr(Core.Params, "-unknown_low"))
		return NumPlayers + 2;

	if (0 != strstr(Core.Params, "-unknown_high"))
		return NumPlayers + 60;

	return NumPlayers - 1; // dedicated server no need to show hidden player
};

bool xrGameSpyServer::NeedToCheckClient_GameSpy_CDKey(IClient* CL)
{
	if (0 != strstr(Core.Params, "-check_cd_key"))
	{
		if (!m_bCDKey_Initialized || (CL == GetServerClient() &&
			bIsDedicatedServer))
			return false;
		
		SendChallengeString_2_Client(CL);

		return true;
	}
	else 
		return false;
};

void xrGameSpyServer::OnCL_Disconnected(IClient* _CL)
{
	inherited::OnCL_Disconnected(_CL);

	csPlayers.Enter();

	if (m_bCDKey_Initialized)
		m_GCDServer.DisconnectUser(int(_CL->ID.value()));	

	csPlayers.Leave();
}

u32 xrGameSpyServer::OnMessage(NET_Packet& P, ClientID sender)	// Non-Zero means broadcasting with "flags" as returned
{
	u16	type;
	P.r_begin(type);

	xrGameSpyClientData *CL = (xrGameSpyClientData*)ID_to_client(sender);

	switch (type)
	{
	case M_GAMESPY_CDKEY_VALIDATION_CHALLENGE_RESPOND:
	{
		string128 ResponseStr = "";
		u32 bytesRemain = P.r_elapsed();

		if (bytesRemain == 0 || bytesRemain > sizeof(ResponseStr))
		{
			Msg("! WARNING: Validation challenge respond from client is %s. DoS attack?"
				, bytesRemain == 0 ? "empty" : "too long");

			return 0;
		}

		P.r_stringZ(ResponseStr);

		if (!CL->m_bCDKeyAuth)
		{
			Msg("xrGS::CDKey::Server : Respond accepted, Authenticate client.");
			m_GCDServer.AuthUser(int(CL->ID.value()), CL->m_cAddress.m_data.data, CL->m_pChallengeString, ResponseStr, this);
			strcpy_s(CL->m_guid, 128, this->GCD_Server()->GetKeyHash(CL->ID.value()));
		}
		else
		{
			Msg("xrGS::CDKey::Server : Respond accepted, ReAuthenticate client.");
			m_GCDServer.ReAuthUser(int(CL->ID.value()), CL->m_iCDKeyReauthHint, ResponseStr);
		}

		return 0;
	}break;
	}

	return	inherited::OnMessage(P, sender);
};

bool xrGameSpyServer::Check_ServerAccess(IClient *CL, string512 &reason)
{
	if (!IsProtectedServer())
	{
		strcpy_s(reason, "Access successful by server. ");
		return true;
	}

	string_path		fn;
	FS.update_path(fn, "$app_data_root$", "server_users.ltx");

	if (!FS.exist(fn))
	{
		strcpy_s(reason, "Access denied by server. ");
		return false;
	}

	CInifile inif(fn);

	if (!inif.section_exist("users"))
	{
		strcpy_s(reason, "Access denied by server. ");
		return false;
	}

	if (!inif.line_count("users"))
	{
		strcpy_s(reason, "Access denied by server. ");
		return false;
	}

	if (CL && inif.line_exist("users", CL->name))
	{
		if (game->NewPlayerName_Exists(CL, CL->name.c_str()))
		{
			strcpy_s(reason, "! Access denied by server. Login \"");
			strcat_s(reason, CL->name.c_str());
			strcat_s(reason, "\" exist already. ");

			return false;
		}

		shared_str pass1 = inif.r_string_wb("users", CL->name.c_str());

		if (xr_strcmp(pass1, CL->pass) == 0)
		{
			strcpy_s(reason, "- User \"");
			strcat_s(reason, CL->name.c_str());
			strcat_s(reason, "\" access successful by server. ");
			return true;
		}
	}

	strcpy_s(reason, "! Access denied by server. Wrong login/password. ");
	return false;
}

void xrGameSpyServer::Assign_ServerType(string512& res)
{
	string_path		fn;
	FS.update_path(fn, "$app_data_root$", "server_users.ltx");

	if (FS.exist(fn))
	{
		CInifile inif(fn);

		if (inif.section_exist("users"))
		{
			if (inif.line_count("users") != 0)
			{
				ServerFlags.set(server_flag_protected, 1);
				strcpy_s(res, "# Server started as protected, using users list.");
				Msg(res);
				return;
			}
			else 
				strcpy_s(res, "Users count in list is null.");			
		}
		else 
			strcpy_s(res, "Section [users] not found.");		
	}
	else 
		strcpy_s(res, "File <server_users.ltx> not found in folder <$app_data_root$>.");
	
	Msg(res);

	if (0 != strstr(Core.Params, "-tourn_icon"))
		ServerFlags.set(server_flag_protected, 1);
	else
		ServerFlags.set(server_flag_protected, 0);
	
	strcpy_s(res, "# Server started without users list.");
	Msg(res);
}

void xrGameSpyServer::GetServerInfo(CServerInfo* si)
{
	string32 tmp, tmp2;

#pragma todo("tsmp: переписать")

	std::string rus = MapNameRus.c_str();
	std::string eng = MapName.c_str();
	std::string inf, host, port, p2, p3;

	host = HostName.c_str();

	if (rus != eng) 
		inf = rus + " (" + eng + ") ";
	else 
		inf = rus;

	inf = inf + " ; Имя сервера = " + host;

	si->AddItem("Карта", inf.c_str(), RGB(255, 0, 128));


	strcpy_s(tmp, itoa(GetPlayersCount(), tmp2, 10));
	strcat_s(tmp, " / ");
	strcat_s(tmp, itoa(m_iMaxPlayers, tmp2, 10));
	si->AddItem("Игроки", tmp, RGB(255, 128, 255));

	string256 res;
	si->AddItem("Версия сервера", TSMP_VERSION, RGB(0, 158, 255));

	p2 = std::to_string(iGameSpyBasePort);
	p3 = std::to_string(GetPort());
	port = "GameSpy:" + p2 + " Сервера:" + p3;

	si->AddItem("Порты", port.c_str(), RGB(200, 5, 155));
	inherited::GetServerInfo(si);
}
