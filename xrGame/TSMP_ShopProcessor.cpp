#include "stdafx.h"
#include "game_base.h"
#include "game_sv_deathmatch.h"
#include "ui/UIBuyWndShared.h"

enum class Types
{
	mk,					// аптечки
	mka,				// антирад
	expl,				// гранаты
	smk,				// дымовые гранаты
	glgr,				// гранаты для гранатометов
	g_a,				// патроны для гаусса
	standart_ammo_p,	// патроны для пистолетов
	advanced_ammo_p,	// бронебойные патроны для пистолетов
	standart_ammo,		// патроны для всего остального
	advanced_ammo,		// бронебойные патроны для всего остального
	best_ammo,			// дротики для дробовика
	wpn_auto,			// все оружие кроме пистолетов
	wpn_pist,			// пистолеты 
	outfit,				// костюмы
	scp,				// прицелы
	sil,				// глушители
	gl,					// подствольные гранатометы
	torch,				// фонарик
	det,				// детекторы
	binoc,				// бинокль
	kf					// нож
};

static const std::map<std::string, Types> Itms =
{
	{"mp_medkit",							Types::mk				},
	{"mp_antirad",							Types::mka				},
	{"mp_grenade_rgd5",						Types::expl				},
	{"mp_grenade_f1",						Types::expl				},
	{"mp_grenade_gd-05",					Types::smk				},
	{"mp_ammo_vog-25",						Types::glgr				},
	{"mp_ammo_m209",						Types::glgr				},
	{"mp_ammo_og-7b",						Types::glgr				},
	{"mp_ammo_gauss",						Types::g_a				},
	{"mp_ammo_9x18_fmj",					Types::standart_ammo_p	},
	{"mp_ammo_11.43x23_fmj",				Types::standart_ammo_p	},
	{"mp_ammo_9x18_pmm",					Types::advanced_ammo_p	},
	{"mp_ammo_11.43x23_hydro",				Types::advanced_ammo_p	},
	{"mp_ammo_5.45x39_fmj",					Types::standart_ammo	},
	{"mp_ammo_9x19_fm",						Types::standart_ammo	},
	{"mp_ammo_7.62x54_7h1",					Types::standart_ammo	},
	{"mp_ammo_5.56x45_ss190",				Types::standart_ammo	},
	{"mp_ammo_9x39_pab9",					Types::standart_ammo	},
	{"mp_ammo_12x70_buck",					Types::standart_ammo	},
	{"mp_ammo_5.45x39_ap",					Types::advanced_ammo	},
	{"mp_ammo_9x19_pbp",					Types::advanced_ammo	},
	{"mp_ammo_7.62x54_ap",					Types::advanced_ammo	},
	{"mp_ammo_5.56x45_ap",					Types::advanced_ammo	},
	{"mp_ammo_9x39_ap",						Types::advanced_ammo	},
	{"mp_ammo_12x76_zhekan" ,				Types::advanced_ammo	},
	{"mp_ammo_12x76_dart",					Types::best_ammo		},
	{"mp_wpn_bm16",							Types::wpn_auto			},
	{"mp_wpn_wincheaster1300",				Types::wpn_auto			},
	{"mp_wpn_spas12",						Types::wpn_auto			},
	{"mp_wpn_ak74u",						Types::wpn_auto			},
	{"mp_wpn_ak74",							Types::wpn_auto			},
	{"mp_wpn_mp5",							Types::wpn_auto			},
	{"mp_wpn_l85",							Types::wpn_auto			},
	{"mp_wpn_lr300",						Types::wpn_auto			},
	{"mp_wpn_abakan",						Types::wpn_auto			},
	{"mp_wpn_sig550",						Types::wpn_auto			},
	{"mp_wpn_groza",						Types::wpn_auto			},
	{"mp_wpn_g36",							Types::wpn_auto			},
	{"mp_wpn_fn2000",						Types::wpn_auto			},
	{"mp_wpn_val",							Types::wpn_auto			},
	{"mp_wpn_vintorez",						Types::wpn_auto			},
	{"mp_wpn_svd",							Types::wpn_auto			},
	{"mp_wpn_svu",							Types::wpn_auto			},
	{"mp_wpn_gauss",						Types::wpn_auto			},
	{"mp_wpn_rpg7",							Types::wpn_auto			},
	{"mp_wpn_rg - 6",						Types::wpn_auto			},
	{"mp_wpn_pm",							Types::wpn_pist			},
	{"mp_wpn_pb",							Types::wpn_pist			},
	{"mp_wpn_fort",							Types::wpn_pist			},
	{"mp_wpn_walther",						Types::wpn_pist			},
	{"mp_wpn_colt1911",						Types::wpn_pist			},
	{"mp_wpn_usp",							Types::wpn_pist			},
	{"mp_wpn_sig220",						Types::wpn_pist			},
	{"mp_wpn_desert_eagle",					Types::wpn_pist			},
	{"mp_scientific_outfit",				Types::outfit			},
	{"mp_military_stalker_outfit",			Types::outfit			},
	{"mp_exo_outfit",						Types::outfit			},
	{"mp_wpn_addon_scope_susat",			Types::scp				},
	{"mp_wpn_addon_scope",					Types::scp				},
	{"mp_wpn_addon_silencer",				Types::sil				},
	{"mp_wpn_addon_grenade_launcher",		Types::gl				},
	{"mp_wpn_addon_grenade_launcher_m203",	Types::gl				},
	{"mp_device_torch",						Types::torch			},
	{"mp_detector_advances",				Types::det				},
	{"mp_wpn_binoc",						Types::binoc			},
	{"mp_wpn_knife",						Types::kf				}
};

std::map<Types, int> RankRestrictionsNovice =
{
	{Types::mk,1},
{Types::expl,1 },
{Types::smk,1},
{Types::advanced_ammo_p,4},
{Types::standart_ammo,5},
{Types::advanced_ammo,4},
{Types::best_ammo,3},
{Types::glgr,0}

};

/*
[rank_base]
amount_restriction = mk:0, expl:0, smk:0, advanced_ammo_p:0, standart_ammo_p:10, standart_ammo:0, advanced_ammo:0, best_ammo:0, glgr:0, wpn_auto:1, wpn_pist:1, device:1, mka:1, outfit:1, scp:1, sil:1, gl:1, torch:1, det:1, binoc:1, g_a:0, kf:1

[rank_0]
rank_name						= st_rank_novice		; звание
rank_exp						= 0, 0          ; опыт, артефакты
rank_diff_exp_bonus				= 1, 1.5, 2, 3, 4
;----------------- new items -------------------------
available_items					= mp_wpn_knife,mp_wpn_pm,mp_wpn_pb,mp_wpn_fort,mp_wpn_walther,mp_wpn_colt1911,mp_wpn_usp,mp_wpn_sig220,mp_wpn_desert_eagle,mp_wpn_bm16,mp_wpn_ak74u,mp_wpn_ak74,mp_wpn_mp5,mp_wpn_l85,mp_device_torch,mp_antirad,mp_detector_advances,mp_medkit,mp_wpn_binoc,mp_wpn_addon_scope_susat,mp_wpn_addon_scope,mp_grenade_f1,mp_grenade_rgd5,mp_grenade_gd-05,mp_wpn_addon_silencer,mp_ammo_9x18_fmj,mp_ammo_9x18_pbp,mp_ammo_9x18_pmm,mp_ammo_9x19_fmj,mp_ammo_9x19_pbp,mp_ammo_5.45x39_fmj,mp_ammo_5.45x39_ap,mp_ammo_5.56x45_ss190,mp_ammo_5.56x45_ap,mp_ammo_7.62x54_7h1,mp_ammo_7.62x54_ap,mp_ammo_9x39_pab9,mp_ammo_9x39_ap,mp_ammo_11.43x23_fmj,mp_ammo_11.43x23_hydro,mp_ammo_12x70_buck,mp_ammo_12x76_dart,mp_ammo_12x76_zhekan ;mp_wpn_toz34,mp_wpn_hpsa,mp_wpn_beretta,
amount_restriction				= mk:1, expl:1, smk:1, advanced_ammo_p:4, standart_ammo:5, advanced_ammo:4, best_ammo:3, glgr:0

[rank_1]
rank_name						= st_rank_experienced
rank_exp						= 500,0;500
rank_aquire_money				= 300
rank_diff_exp_bonus				= 0.85, 1, 1.5, 2, 3.5
;----------------- new items -------------------------
available_items					= mp_wpn_wincheaster1300,mp_wpn_lr300,mp_wpn_vintorez,mp_wpn_abakan,mp_scientific_outfit, wpn_fort_m1, wpn_ak74u_m1, wpn_walther_m1, hunters_toz, wpn_beretta_m1, wpn_hpsa
;----------------- new prices ------------------------
mp_wpn_pm_cost					= 0
mp_wpn_pb_cost					= 0
mp_wpn_fort_cost				= 0
mp_wpn_walther_cost				= 0
mp_ammo_9x18_fmj_cost			= 0
mp_ammo_9x19_fmj_cost			= 0
;----------------- new def items ---------------------
def_item_repl_mp_wpn_pm 		= mp_wpn_walther
def_item_repl_mp_wpn_pb 		= mp_wpn_fort
amount_restriction				= mk:1, expl:2, smk:1, advanced_ammo_p:4, standart_ammo:5, advanced_ammo:4, best_ammo:3

[rank_2]
rank_name						= st_rank_professional
rank_exp						= 1500,0;1500
rank_aquire_money				= 850;800
rank_diff_exp_bonus				= 0.65, 0.8, 1, 1.5, 2
;----------------- new items -------------------------
available_items					= mp_wpn_spas12,mp_wpn_groza,mp_wpn_sig550,mp_military_stalker_outfit,mp_wpn_svd,mp_wpn_svu,mp_wpn_addon_grenade_launcher,mp_wpn_addon_grenade_launcher_m203,mp_ammo_vog-25,mp_ammo_m209, wpn_abakan_m1, wpn_abakan_m2, wpn_ak74_m1, wpn_l85_m1, wpn_l85_m2, wpn_lr300_m1
;----------------- new prices ------------------------
mp_wpn_sig220_cost				= 0
mp_wpn_colt1911_cost			= 0
mp_ammo_11.43x23_fmj_cost		= 0
;----------------- new def items ---------------------
def_item_repl_mp_wpn_walther	= mp_wpn_colt1911
def_item_repl_mp_wpn_fort		= mp_wpn_sig220
amount_restriction				= mk:2, expl:3, smk:2, glgr:1, advanced_ammo_p:4, standart_ammo:5, advanced_ammo:4, best_ammo:3

[rank_3]
rank_name						= st_rank_veteran
rank_exp						= 3000,0 ; 3000;2500
rank_aquire_money				= 1500
rank_diff_exp_bonus				= 0.5, 0.7, 0.9, 1, 1.5
;----------------- new items -------------------------
available_items					= mp_wpn_g36,mp_wpn_val,mp_wpn_rpg7,mp_ammo_og-7b,mp_exo_outfit, wpn_spas12_m1, wpn_groza_coll, wpn_winchester_m1, wpn_groza_m1, wpn_eagle_m1, wpn_colt_m1, wpn_val_m1
;----------------- new prices ------------------------
mp_wpn_sig220_cost				= 0
mp_wpn_colt1911_cost			= 0
mp_ammo_11.43x23_fmj_cost		= 0
mp_wpn_usp_cost					= 0
;----------------- new def items ---------------------
def_item_repl_mp_wpn_colt1911	= mp_wpn_usp
def_item_repl_mp_wpn_sig220		= mp_wpn_usp
amount_restriction				= mk:2, expl:3, smk:3, glgr:2, advanced_ammo_p:4, standart_ammo:5, advanced_ammo:4, best_ammo:3


[rank_4]
rank_name						= st_rank_legend
rank_exp						= 6000, 1 ; 6000;4000
rank_aquire_money				= 3000
rank_diff_exp_bonus				= 0.2, 0.5, 0.7, 0.9, 1
;----------------- new items -------------------------
available_items					= mp_wpn_fn2000,mp_wpn_gauss,mp_wpn_rg-6,mp_ammo_gauss, wpn_svd_m1, wpn_svu_m1, wpn_vintorez_coll, wpn_g36_m1, wpn_mp5_m1, wpn_gauss_m1, wpn_pm_arena, wpn_mp5_arena, wpn_mp5_m2, wpn_toz34_arena, wpn_spas12_arena, wpn_ak74_arena, wpn_bm16_arena, wpn_ak74u_arena, wpn_val_arena, wpn_groza_arena, wpn_fn2000_arena, wpn_g36_arena, wpn_sig_m1, wpn_sig_m2, wpn_rg6_m1
;----------------- new prices ------------------------
mp_wpn_sig220_cost				= 0
mp_wpn_usp_cost					= 0
mp_wpn_colt1911_cost			= 0
;----------------- new def items ---------------------
def_item_repl_mp_wpn_colt1911	= mp_wpn_usp
def_item_repl_mp_wpn_sig220		= mp_wpn_usp
amount_restriction				= mk:3, expl:3, smk:3, glgr:3, advanced_ammo_p:4, standart_ammo:5, advanced_ammo:4, best_ammo:3, g_a:4


*/

static bool DisablePodstvol = false;
static bool DisableGrenades = false;
static bool DisableHardWpn = false;

int	g_sv_mp_DisablerEnabled = 0;
static std::map<std::string, bool> NotAllowedItems;

void TSMP_ShopProcessor_AddItem(std::string Args)
{
	size_t SpacePos = Args.find(' ');

	if (SpacePos == std::string::npos)
		return;

	std::string WeaponName(Args.begin(), Args.end() - Args.size() + SpacePos);
	bool DisableIt = Args[Args.size() - 1] == '1';

	auto SearchIt = NotAllowedItems.find(WeaponName);
	
	if (SearchIt == NotAllowedItems.end())
		NotAllowedItems.insert(std::pair<std::string, bool>(WeaponName, DisableIt));
	else
		SearchIt->second = DisableIt;

	if (WeaponName == "podstvol")
		DisablePodstvol = DisableIt;

	if (WeaponName == "granati")
		DisableGrenades = DisableIt;

	if (WeaponName == "hard_weapon")
		DisableHardWpn = DisableIt;
}

bool NotAllowedToBuy(std::string ItemName)
{
	auto SearchIt = NotAllowedItems.find(ItemName);

	if (SearchIt != NotAllowedItems.end())
		return SearchIt->second;

	if (DisableGrenades)
		if (ItemName == "mp_grenade_f1"
			|| ItemName == "mp_grenade_rgd5"
			|| ItemName == "mp_grenade_gd-05")
			return true;

	if (DisablePodstvol)
		if (ItemName == "mp_wpn_addon_grenade_launcher"
			|| ItemName == "mp_wpn_addon_grenade_launcher_m203"
			|| ItemName == "mp_ammo_vog-25"
			|| ItemName == "mp_ammo_vog-25p"
			|| ItemName == "mp_ammo_m209") 
			return true;
	
	if(DisableHardWpn)
		if(ItemName == "mp_ammo_og-7b"
			|| ItemName == "mp_wpn_rpg7_missile"
			|| ItemName == "mp_wpn_rpg7"
			|| ItemName == "mp_wpn_rg-6") 
			return true;

	return false;
}

u8 CorrectAddons(u8 Addons)
{
	//		0 - нет дополнений
	//		1 - прицел
	//		2 - подствол
	//		3 - прицел+подствол
	//		4 - глушитель
	//		5 - прицел+глушитель
	//		6 - подствол+глушитель
	//		7 - прицел+подствол+глушитель		

	if (DisablePodstvol)
	{
		switch (Addons)
		{
		case 2:
			Addons = 0;
			break;
		case 3:
			Addons = 1;
			break;
		case 7:
			Addons = 5;
			break;
		default:
			break;
		}
	}

	return Addons;
}

void TSMP_ShopProcessor_BuyItems(game_PlayerState* ps, u16 ActorID, game_sv_Deathmatch *game)
{	
	/*
	for (u32 i = 0; i < ps->pItemList.size(); i++)
	{		
		u16 ItemID = ps->pItemList[i];
		game->SpawnWeapon4Actor(ActorID, *game->m_strWeaponsData->GetItemName(ItemID & 0x00FF), u8((ItemID & 0xFF00) >> 0x08));

	}
	*/

	for (u16 ItemID : ps->pItemList)
	{
		shared_str Name = game->m_strWeaponsData->GetItemName(ItemID & 0x00FF);

		if (NotAllowedToBuy(Name.c_str()))
			continue;

		u8 Addons = CorrectAddons(u8((ItemID & 0xFF00) >> 0x08));
		game->SpawnWeapon4Actor(ActorID, *Name,Addons );
	}
}

/*



void	game_sv_mp::Tsmp_weapon_disabler(LPCSTR DATA)
{
	//Msg("disabler");
	std::string String1 = DATA;
	std::string String2;

	bool bIsAllOk = false;

	if (String1.find(' ') != std::string::npos)
	{
		bIsAllOk = true;

		char* s = new char[String1.size() + 1];

		strcpy(s, String1.c_str());

		char* p = strtok(s, " ");
		int iii = 0;
		while (p != NULL)
		{
			if (iii == 0) String1 = p;
			else String2 = p;
			p = strtok(NULL, " ");
			iii++;
		}

		delete[] s;
	}

	if (!(String2 == "1" || String2 == "0")) bIsAllOk = false;

	if (!bIsAllOk)
	{
		Msg("! error in weapon disabler");
		return;
	}

	int iI, iIdx = 0;
	bool bFound = false;

	for (iI = 0; (iI < m_WeaponDisablerItemsCount) && (!bFound); iI++)
	{
		if (String1 == m_WeaponDisablerItems[iI])
		{
			iIdx = iI;
			bFound = true;
		}
	}

	if (bFound)
	{
		if (String2 == "1") m_WeaponDisablerState[iIdx] = true;
		else m_WeaponDisablerState[iIdx] = false;
		return;
	}

	if (String2 == "1") m_WeaponDisablerState[m_WeaponDisablerItemsCount] = true;
	if (String2 == "0") m_WeaponDisablerState[m_WeaponDisablerItemsCount] = false;
	m_WeaponDisablerItems[m_WeaponDisablerItemsCount] = String1;
	m_WeaponDisablerItemsCount++;
};

void	game_sv_mp::SpawnWeapon4Actor(u16 actorId, LPCSTR N, u8 Addons)
{
	std::string StrIng = N;

	if (!N) return;

	if (g_sv_mp_DisablerEnabled == 1)
	{
		bool podstvol = false;

		for (int i = 0; i < m_WeaponDisablerItemsCount; i++)
		{
			if ((m_WeaponDisablerItems[i] == StrIng) && (m_WeaponDisablerState[i]))
			{
				
			//	std::string Messs = "! " + StrIng + " запрещен для покупки на данном сервере (disabled on this server)";

			//	NET_Packet			PPP;
			//	GenerateGameMessage(PPP);
			//	PPP.w_u32(GAME_EVENT_SERVER_STRING_MESSAGE);
			//	PPP.w_stringZ(Messs.c_str());
			//	m_server->SendTo(m_server->GetServerClient()->ID, PPP);			

				return;
			}

			if ((m_WeaponDisablerItems[i] == "podstvol") && (m_WeaponDisablerState[i]))
			{
				podstvol = true;
				if (StrIng == "mp_wpn_addon_grenade_launcher") return;
				if (StrIng == "mp_wpn_addon_grenade_launcher_m203") return;
				if (StrIng == "mp_ammo_vog-25") return;
				if (StrIng == "mp_ammo_vog-25p") return;
				if (StrIng == "mp_ammo_m209") return;
			}

			if ((m_WeaponDisablerItems[i] == "granati") && (m_WeaponDisablerState[i]))
			{
				if (StrIng == "mp_grenade_f1") return;
				if (StrIng == "mp_grenade_rgd5") return;
				if (StrIng == "mp_grenade_gd-05") return;
			}

			if ((m_WeaponDisablerItems[i] == "hard_weapon") && (m_WeaponDisablerState[i]))
			{
				if (StrIng == "mp_ammo_og-7b") return;
				if (StrIng == "mp_wpn_rpg7_missile") return;
				if (StrIng == "mp_wpn_rpg7") return;
				if (StrIng == "mp_wpn_rg-6") return;
			}
		}

		

	}

	CSE_Abstract* E = spawn_begin(N);
	E->ID_Parent = actorId;

	E->s_flags.assign(M_SPAWN_OBJECT_LOCAL);	// flags

													/////////////////////////////////////////////////////////////////////////////////
	//если это оружие - спавним его с полным магазином
	CSE_ALifeItemWeapon* pWeapon = smart_cast<CSE_ALifeItemWeapon*>(E);

	if (pWeapon)
	{
		pWeapon->a_elapsed = pWeapon->get_ammo_magsize();
		pWeapon->m_addon_flags.assign(Addons);
	}

	spawn_end(E, m_server->GetServerClient()->ID);
};


int			g_sv_mp_DisablerEnabled			= 0;


	bool			m_WeaponDisablerState[100];

	std::string		m_WeaponDisablerItems[100];
	int				m_WeaponDisablerItemsCount;

		m_WeaponDisablerItemsCount = 0;
	int ii;
	for (ii = 0; ii < 100; ii++)	m_WeaponDisablerState[ii] = true;
	for (ii = 0; ii < 100; ii++)	m_WeaponDisablerItems[ii] = "";


*/

