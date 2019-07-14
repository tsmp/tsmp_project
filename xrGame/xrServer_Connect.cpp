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
#include "TSMP_Loader.h"

extern bool bIsDedicatedServer;

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

	bool bAllSpaces = true;

	static std::map<char, char> Symbols =
	{
		{'Q','q'}, {'W','w'}, {'E','e'}, {'R','r'}, {'T','t'}, {'Y','y'}, 
		{'U','u'}, {'I','i'}, {'O','o'}, {'P','p'}, {'A','a'}, {'S','s'},
		{'D','d'}, {'F','f'}, {'G','g'}, {'H','h'}, {'J','j'}, {'K','k'}, 
		{'L','l'}, {'Z','z'}, {'X','x'}, {'C','c'}, {'V','v'}, {'B','b'},
		{'N','n'}, {'M','m'}, {'Ё','ё'}, {'Й','й'}, {'Ц','ц'}, {'У','у'}, 
		{'К','к'}, {'Е','е'}, {'Н','н'}, {'Г','г'}, {'Ш','ш'}, {'Щ','щ'},
		{'З','з'}, {'Х','х'}, {'Ъ','ъ'}, {'Ф','ф'}, {'Ы','ы'}, {'В','в'}, 
		{'А','а'}, {'П','п'}, {'Р','р'}, {'О','о'}, {'Л','л'}, {'Д','д'},
		{'Ж','ж'}, {'Э','э'}, {'Я','я'}, {'Ч','ч'}, {'С','с'}, {'М','м'}, 
		{'И','и'}, {'Т','т'}, {'Ь','ь'}, {'Б','б'}, {'Ю','ю'}
	};

	for (int i = 0; i < iLen; ++i)
	{
		auto It = Symbols.find(sName[i]);

		if (It != Symbols.end())
			sName[i] = It->second;
		
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

		if (sName[i] != '_')
			bAllSpaces = false;
	}

	if ((iLen < 2)||(bAllSpaces)) 
		strcpy(sName, "игрок");
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

void xrServer::AttachNewClient(IClient* CL)
{
	if (bIsDedicatedServer)
		TSMP_LoaderExecute(this, CL->ID);	

	MSYS_CONFIG	msgConfig;
	msgConfig.sign1 = 0x12071980;
	msgConfig.sign2 = 0x26111975;
	msgConfig.is_battleye = 0;

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

	CL->m_guid[0] = 0;
}
