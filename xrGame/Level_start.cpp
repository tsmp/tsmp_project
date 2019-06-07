#include "stdafx.h"
#include "level.h"
#include "Level_Bullet_Manager.h"
#include "xrserver.h"
#include "game_cl_base.h"
#include "xrmessages.h"
#include "xrGameSpyServer.h"
#include "../x_ray.h"
#include "../device.h"
#include "../IGame_Persistent.h"
#include "../xr_ioconsole.h"
#include "MainMenu.h"

#include "RegistryFuncs.h"
#include "hudmanager.h"

#include "..\TSMP_BuildConfig.h"

BOOL g_start_total_res = TRUE;
xrServer::EConnect g_connect_server_err = xrServer::ErrConnect;
extern bool bIsDedicatedServer;
std::string LastConnectParams = " ";

BOOL CLevel::net_Start(LPCSTR op_server, LPCSTR op_client)
{
	string128 m_pl_name;
	ReadRegistry_StrValue(REGISTRY_VALUE_USERNAME, m_pl_name);

	if (xr_strlen(m_pl_name) > 17)
		m_pl_name[17] = 0;

	net_start_result_total = TRUE;
	pApp->LoadBegin();

	LPCSTR NameStart = strstr(op_client, "/name="); 	//make Client Name if options don't have it
	
	if (!NameStart)
	{
		string512 tmp;
		strcpy_s(tmp, op_client);
		strcat_s(tmp, "/name=");
		//	strcat_s(tmp, xr_strlen(Core.UserName) ? Core.UserName : Core.CompName);

		strcat_s(tmp, xr_strlen(m_pl_name) ? m_pl_name : (xr_strlen(Core.UserName) ? Core.UserName : Core.CompName));

		m_caClientOptions = tmp;
	}
	else
	{
		string1024 ret = "";
		LPCSTR begin = NameStart + xr_strlen("/name=");
		sscanf(begin, "%[^/]", ret);

		if (!xr_strlen(ret))
		{
			string1024 tmpstr;
			strcpy_s(tmpstr, op_client);
			*(strstr(tmpstr, "name=") + 5) = 0;
			strcat_s(tmpstr, xr_strlen(Core.UserName) ? Core.UserName : Core.CompName);

			const char* ptmp = strstr(strstr(op_client, "name="), "/");

			if (ptmp)
				strcat_s(tmpstr, ptmp);
			m_caClientOptions = tmpstr;
		}
		else
			m_caClientOptions = op_client;		
	}

	m_caServerOptions = op_server;
	m_bDemoPlayMode = FALSE;
	m_aDemoData.clear();
	m_bDemoStarted = FALSE;

	if (strstr(Core.Params, "-tdemo ") || strstr(Core.Params, "-tdemof "))
	{
		string1024 f_name;

		if (strstr(Core.Params, "-tdemo "))
		{
			sscanf(strstr(Core.Params, "-tdemo ") + 7, "%[^ ] ", f_name);
			m_bDemoPlayByFrame = FALSE;

			Demo_Load(f_name);
		}
		else
		{
			sscanf(strstr(Core.Params, "-tdemof ") + 8, "%[^ ] ", f_name);
			m_bDemoPlayByFrame = TRUE;

			m_lDemoOfs = 0;
			Demo_Load_toFrame(f_name, 100, m_lDemoOfs);
		}
	}
	else
	{
		if (m_caServerOptions.size() == 0 || !strstr(*m_caServerOptions, "single"))
			Demo_PrepareToStore();		
	}

	LastConnectParams = m_caClientOptions.c_str();

	Msg("params: %s", LastConnectParams.c_str());

	g_loading_events.push_back(LOADING_EVENT(this, &CLevel::net_start1));
	g_loading_events.push_back(LOADING_EVENT(this, &CLevel::net_start2));
	g_loading_events.push_back(LOADING_EVENT(this, &CLevel::net_start3));
	g_loading_events.push_back(LOADING_EVENT(this, &CLevel::net_start4));
	g_loading_events.push_back(LOADING_EVENT(this, &CLevel::net_start5));
	g_loading_events.push_back(LOADING_EVENT(this, &CLevel::net_start6));

	return net_start_result_total;
}

bool CLevel::net_start1()
{
	// Start client and server if need it

	if (m_caServerOptions.size())
	{
		g_pGamePersistent->LoadTitle("st_server_starting");

		typedef IGame_Persistent::params params;
		params& p = g_pGamePersistent->m_game_params;

		// Connect
		if (!xr_strcmp(p.m_game_type, "single"))
			Server = xr_new<xrServer>();
		else
			Server = xr_new<xrGameSpyServer>();

		if (xr_strcmp(p.m_alife, "alife"))
		{
			string64 l_name = "";
			const char* SOpts = *m_caServerOptions;

			strncpy(l_name, *m_caServerOptions, strchr(SOpts, '/') - SOpts);
			// Activate level
			if (strchr(l_name, '/'))
				* strchr(l_name, '/') = 0;

			m_name = l_name;

			int id = pApp->Level_ID(l_name);

			if (id < 0) 
			{
				pApp->LoadEnd();
				Log("Can't find level: ", l_name);
				net_start_result_total = FALSE;
				return true;
			}

			pApp->Level_Set(id);
		}
	}
	return true;
}

bool CLevel::net_start2()
{
	if (net_start_result_total && m_caServerOptions.size())
	{
		if ((m_connect_server_err = Server->Connect(m_caServerOptions)) != xrServer::ErrNoError)
		{
			net_start_result_total = false;
			Msg("! Failed to start server.");
			//			Console->Execute("main_menu on");
			return true;
		}

		Server->SLS_Default();
		m_name = Server->level_name(m_caServerOptions);
	}
	return true;
}

bool CLevel::net_start3()
{
	if (!net_start_result_total) return true;
	//add server port if don't have one in options
	if (!strstr(m_caClientOptions.c_str(), "port=") && Server)
	{
		string64 PortStr;
		sprintf_s(PortStr, "/port=%d", Server->GetPort());

		string4096 tmp;
		strcpy_s(tmp, m_caClientOptions.c_str());
		strcat_s(tmp, PortStr);

		m_caClientOptions = tmp;
	}

	//add password string to client, if don't have one
	if (m_caServerOptions.size()) 
	{
		if (strstr(m_caServerOptions.c_str(), "psw=") && !strstr(m_caClientOptions.c_str(), "psw="))
		{
			string64 PasswordStr = "";
			const char* PSW = strstr(m_caServerOptions.c_str(), "psw=") + 4;

			if (strchr(PSW, '/'))
				strncpy(PasswordStr, PSW, strchr(PSW, '/') - PSW);
			else
				strcpy_s(PasswordStr, PSW);

			string4096	tmp;
			sprintf_s(tmp, "%s/psw=%s", m_caClientOptions.c_str(), PasswordStr);
			m_caClientOptions = tmp;
		}
	}

	//setting players GameSpy CDKey if it comes from command line
	if (strstr(m_caClientOptions.c_str(), "/cdkey="))
	{
		string64 CDKey;
		const char* start = strstr(m_caClientOptions.c_str(), "/cdkey=") + xr_strlen("/cdkey=");
		sscanf(start, "%[^/]", CDKey);
		string128 cmd;
		sprintf_s(cmd, "cdkey %s", _strupr(CDKey));
		Console->Execute(cmd);
	}
	return true;
}

bool CLevel::net_start4()
{
	if (!net_start_result_total) 
		return true;

	g_loading_events.pop_front();

	g_loading_events.push_front(LOADING_EVENT(this, &CLevel::net_start_client6));
	g_loading_events.push_front(LOADING_EVENT(this, &CLevel::net_start_client5));
	g_loading_events.push_front(LOADING_EVENT(this, &CLevel::net_start_client4));
	g_loading_events.push_front(LOADING_EVENT(this, &CLevel::net_start_client3));
	g_loading_events.push_front(LOADING_EVENT(this, &CLevel::net_start_client2));
	g_loading_events.push_front(LOADING_EVENT(this, &CLevel::net_start_client1));

	return false;
}

bool CLevel::net_start5()
{
	if (net_start_result_total)
	{
		NET_Packet NP;
		NP.w_begin(M_CLIENTREADY);
		Send(NP, net_flags(TRUE, TRUE));

		if (OnClient() && Server)
			Server->SLS_Clear();		
	}

	return true;
}


#include "..\xrDownloader\xrDownloader.h"
#include <thread>

CMainMenu* Men;

bool TSMP_HasNewUpdates()
{
#ifdef TSMP_CLIENT
	bool b = CheckForUpdates();

	if (b)
		Msg("TSMP need to update");
	else
		Msg("TSMP is up to date");

	return b;
#else
	return false;
#endif
}

void TSMP_Update(std::string level="none")
{
	if (!Men)
	{
		Msg("! error tsmp cant get menu");
		return;
	}

	bool DownloadMap = (level == "none") ? false : true;

	Men->SwitchToMultiplayerMenu();

	DownloadFiles* xrdownloader = new DownloadFiles();
	FillDownloadList(xrdownloader, level);

	//std::string DownloadFrom, Arch;
	//DownloadFrom = "http://stalker.stagila.ru:8080/web_drive/shadow_of_chernobyl/mods/military_kuznya.xdb0";
	//Arch="C:\\sdk\\kuznya.xdb0";

	
	Men->OnDownloadMapStart("Загрузка контента (Downloading content)");

	auto ThUI = [](DownloadFiles * xrldr)
	{
		Msg("ThUI started");

		std::this_thread::sleep_for(std::chrono::milliseconds(1000));

		Men->OnMainMenuMessageBox("Загрузка ресурсов. После загрузки игра перезапустится и вы сможете играть на сервере.");

		while (true)
		{
			std::this_thread::sleep_for(std::chrono::milliseconds(500));

			if (!xrldr)
				break;

			int Pr = xrldr->GetProgress();
			Men->OnDownloadPatchProgress(Pr, 100);
			Msg("downloaded %i %%", Pr);

			if (Pr == 100)
				break;
		}

		xrldr->~DownloadFiles();

		Men->OnDownloadMapEnd();
		std::this_thread::sleep_for(std::chrono::milliseconds(1000));

		RunUpdater(LastConnectParams);
		Console->Execute("quit");

		Msg("ThUI end");
	};


	auto ThD = [](DownloadFiles * XRDW)
	{
		Msg("ThD started");
		Msg("ThD: Downloader defined");
		XRDW->StartDownload();
		Msg("ThD downloaded");
		Msg("ThD finished");
		std::this_thread::sleep_for(std::chrono::milliseconds(1000));
	};

	std::thread thread_D(ThD, xrdownloader);
	std::thread thread_UI(ThUI, xrdownloader);

	thread_D.detach();
	thread_UI.detach();
}

struct LevelLoadFinalizer
{
	bool xr_stdcall net_start_finalizer()
	{
		Men = MainMenu();
		bool NoMap = false;
		shared_str ln = Level().name();

		if (g_pGameLevel && !g_start_total_res)
		{
			
			Msg("! Failed to start client. Check the connection or level existance.");
			DEL_INSTANCE(g_pGameLevel);
			Console->Execute("main_menu on");

			switch (g_connect_server_err)
			{

			case IPureServer::ErrConnect:
			{
				if (!psNET_direct_connect && !bIsDedicatedServer)
					MainMenu()->SwitchToMultiplayerMenu();
			}
				break;

			case IPureServer::ErrBELoad:
			{
				Msg("cant load BattlEye/BEServer.dll");
				MainMenu()->OnLoadError("BattlEye/BEServer.dll");
			}
				break;

			case IPureServer::ErrNoLevel:
			{
					MainMenu()->SwitchToMultiplayerMenu();
					Msg("cant find level %s", ln.c_str());
				//	MainMenu()->OnLoadError(ln.c_str());
					
#ifdef TSMP_CLIENT
						TSMP_Update(ln.c_str());
#endif					
			}
				break;
			}


		}

		return true;
	}
};

LevelLoadFinalizer LF;

bool CLevel::net_start6()
{
	g_start_total_res = net_start_result_total;
	g_connect_server_err = m_connect_server_err;
	g_loading_events.pop_front();
	g_loading_events.push_front(LOADING_EVENT(&LF, &LevelLoadFinalizer::net_start_finalizer));

	//init bullet manager
	BulletManager().Clear();
	BulletManager().Load();

	pApp->LoadEnd();

	if (net_start_result_total)
	{
		if (strstr(Core.Params, "-$"))
		{
			string256				buf, cmd, param;
			sscanf(strstr(Core.Params, "-$") + 2, "%[^ ] %[^ ] ", cmd, param);
			strconcat(sizeof(buf), buf, cmd, " ", param);
			Console->Execute(buf);
		}

		if (!bIsDedicatedServer && g_hud)
				HUD().GetUI()->OnConnected();		
	}

	return false;
}

void CLevel::InitializeClientGame(NET_Packet & P)
{
	string256 game_type_name;
	P.r_stringZ(game_type_name);

	if (game && !xr_strcmp(game_type_name, game->type_name()))
		return;

	xr_delete(game);
	Msg("- Game configuring : Started ");
	CLASS_ID clsid = game_GameState::getCLASS_ID(game_type_name, false);
	game = smart_cast<game_cl_GameState*> (NEW_INSTANCE(clsid));
	game->set_type_name(game_type_name);
	game->Init();
	m_bGameConfigStarted = TRUE;
}
