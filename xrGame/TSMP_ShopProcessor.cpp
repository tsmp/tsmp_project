#include "stdafx.h"
#include "game_base.h"
#include "game_sv_deathmatch.h"
#include "ui/UIBuyWndShared.h"
#include "../XR_IOConsole.h"

int		g_TsmpBuyVeriferEnabled = 0;

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
	kf,					// нож
	unknown
};
 
struct Itm
{
	std::string Name;
	Types Type;
	int Rank;
};

static const std::vector<Itm> Itms
{
	{"mp_medkit",							Types::mk				,0},
	{"mp_antirad",							Types::mka				,0},
	{"mp_grenade_rgd5",						Types::expl				,0},
	{"mp_grenade_f1",						Types::expl				,0},
	{"mp_grenade_gd-05",					Types::smk				,0},
	{"mp_ammo_vog-25",						Types::glgr				,2},
	{"mp_ammo_m209",						Types::glgr				,2},
	{"mp_ammo_og-7b",						Types::glgr				,3},
	{"mp_ammo_gauss",						Types::g_a				,4},
	{"mp_ammo_9x18_fmj",					Types::standart_ammo_p	,0},
	{"mp_ammo_11.43x23_fmj",				Types::standart_ammo_p	,0},
	{"mp_ammo_9x18_pmm",					Types::advanced_ammo_p	,0},
	{"mp_ammo_11.43x23_hydro",				Types::advanced_ammo_p	,0},
	{"mp_ammo_5.45x39_fmj",					Types::standart_ammo	,0},
	{"mp_ammo_9x19_fmj",					Types::standart_ammo	,0},
	{"mp_ammo_7.62x54_7h1",					Types::standart_ammo	,0},
	{"mp_ammo_5.56x45_ss190",				Types::standart_ammo	,0},
	{"mp_ammo_9x39_pab9",					Types::standart_ammo	,0},
	{"mp_ammo_12x70_buck",					Types::standart_ammo	,0},
	{"mp_ammo_5.45x39_ap",					Types::advanced_ammo	,0},
	{"mp_ammo_9x19_pbp",					Types::advanced_ammo	,0},
	{"mp_ammo_7.62x54_ap",					Types::advanced_ammo	,0},
	{"mp_ammo_5.56x45_ap",					Types::advanced_ammo	,0},
	{"mp_ammo_9x39_ap",						Types::advanced_ammo	,0},
	{"mp_ammo_12x76_zhekan" ,				Types::advanced_ammo	,0},
	{"mp_ammo_12x76_dart",					Types::best_ammo		,0},
	{"mp_wpn_bm16",							Types::wpn_auto			,0},
	{"mp_wpn_wincheaster1300",				Types::wpn_auto			,1},
	{"mp_wpn_spas12",						Types::wpn_auto			,2},
	{"mp_wpn_ak74u",						Types::wpn_auto			,0},
	{"mp_wpn_ak74",							Types::wpn_auto			,0},
	{"mp_wpn_mp5",							Types::wpn_auto			,0},
	{"mp_wpn_l85",							Types::wpn_auto			,0},
	{"mp_wpn_lr300",						Types::wpn_auto			,1},
	{"mp_wpn_abakan",						Types::wpn_auto			,1},
	{"mp_wpn_sig550",						Types::wpn_auto			,2},
	{"mp_wpn_groza",						Types::wpn_auto			,2},
	{"mp_wpn_g36",							Types::wpn_auto			,3},
	{"mp_wpn_fn2000",						Types::wpn_auto			,4},
	{"mp_wpn_val",							Types::wpn_auto			,3},
	{"mp_wpn_vintorez",						Types::wpn_auto			,1},
	{"mp_wpn_svd",							Types::wpn_auto			,2},
	{"mp_wpn_svu",							Types::wpn_auto			,2},
	{"mp_wpn_gauss",						Types::wpn_auto			,4},
	{"mp_wpn_rpg7",							Types::wpn_auto			,3},
	{"mp_wpn_rg - 6",						Types::wpn_auto			,4},
	{"mp_wpn_pm",							Types::wpn_pist			,0},
	{"mp_wpn_pb",							Types::wpn_pist			,0},
	{"mp_wpn_fort",							Types::wpn_pist			,0},
	{"mp_wpn_walther",						Types::wpn_pist			,0},
	{"mp_wpn_colt1911",						Types::wpn_pist			,0},
	{"mp_wpn_usp",							Types::wpn_pist			,0},
	{"mp_wpn_sig220",						Types::wpn_pist			,0},
	{"mp_wpn_desert_eagle",					Types::wpn_pist			,0},
	{"mp_scientific_outfit",				Types::outfit			,1},
	{"mp_military_stalker_outfit",			Types::outfit			,2},
	{"mp_exo_outfit",						Types::outfit			,3},
	{"mp_wpn_addon_scope_susat",			Types::scp				,0},
	{"mp_wpn_addon_scope",					Types::scp				,0},
	{"mp_wpn_addon_silencer",				Types::sil				,0},
	{"mp_wpn_addon_grenade_launcher",		Types::gl				,2},
	{"mp_wpn_addon_grenade_launcher_m203",	Types::gl				,2},
	{"mp_device_torch",						Types::torch			,0},
	{"mp_detector_advances",				Types::det				,0},
	{"mp_wpn_binoc",						Types::binoc			,0},
	{"mp_wpn_knife",						Types::kf				,0}
};

static const std::vector<Types> TypesVec
{
	Types::mk,Types::expl,Types::smk,Types::advanced_ammo_p,Types::standart_ammo_p,Types::standart_ammo,Types::advanced_ammo,
	Types::best_ammo,Types::glgr,Types::wpn_auto,Types::wpn_pist,Types::mka,Types::outfit,Types::scp,Types::sil,Types::gl,Types::torch,
	Types::det,Types::binoc,Types::g_a,Types::kf
};

static const std::vector<std::string> TypesRus
{
	"Аптечка", "Граната", "Дымовая граната", "Бронебойные патроны для пистолетов", "Патроны для пистолетов", "Патроны","Бронебойные патроны",
	"Дротики дробовика", "Граната гранатомета", "Автомат", "Пистолет", "Антирад", "Костюм", "Прицел", "Глушитель", "Подствол","Фонарь",
	"Детектор", "Бинокль", "Патрон Гаусс", "Нож"
};

static const std::vector<std::string> RanksRus
{
	"Новичок","Опытный","Профессионал","Ветеран","Легенда"
};

static const std::vector<std::vector<int>> RankCountRestrictions
{
	{ 1,1,1,4,10,5,4,3,0,1,1,1,1,1,1,1,1,1,1,0,1},
	{ 1,2,1,4,10,5,4,3,0,1,1,1,1,1,1,1,1,1,1,0,1},
	{ 2,3,2,4,10,5,4,3,1,1,1,1,1,1,1,1,1,1,1,0,1},
	{ 2,3,3,4,10,5,4,3,2,1,1,1,1,1,1,1,1,1,1,0,1},
	{ 3,3,3,4,10,5,4,3,3,1,1,1,1,1,1,1,1,1,1,4,1}
};

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
	if (!g_sv_mp_DisablerEnabled)
		return false;

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

int GetWeaponTypeIdx(std::string& s)
{
	Types TT = Types::unknown;

	for (u32 i = 0; i < Itms.size(); i++)
		if (Itms[i].Name == s)
			TT = Itms[i].Type;

	if (TT == Types::unknown)
	{
		Msg("! cant find type for item %s", s.c_str());
		return 0;
	}
	
	for (u32 i = 0; i < TypesVec.size(); i++)
		if (TypesVec[i] == TT)
			return i;

	Msg("! cant find type %u", (u32)TT);
	return 0;
}

void CheckItemsRank(std::string &S,u8 rank, game_PlayerState* ps)
{
	for (u32 i = 0; i < Itms.size(); i++)
		if (Itms[i].Name == S && Itms[i].Rank > rank)
		{
			std::string str = "chat_tsmp Читер? Игрок " + std::string(ps->name) + " покупает " + S + " при ранге " + RanksRus[rank] + ". Доступно для покупки с ранга " + RanksRus[Itms[i].Rank];
			Console->Execute(str.c_str());
		}
}

void CheckItemsCounts(std::vector<std::string>& ItemsVec, const std::vector<int>& RankMaxCount,u8 rank, game_PlayerState* ps)
{
	std::vector<int> Vec(21, 0);

	for (std::string& It : ItemsVec)
	{
		CheckItemsRank(It, rank,ps);
		Vec[GetWeaponTypeIdx(It)]++;
	}

	for (u32 i = 0; i < Vec.size(); i++)
		if (Vec[i] > RankMaxCount[i])
		{
			std::string str = "chat_tsmp Читер? Игрок " + std::string(ps->name) + " покупает " + std::to_string(Vec[i]) + " предметов класса " + TypesRus[i] + " при ранге " + RanksRus[rank] + " . Доступно для покупки на данном ранге - " + std::to_string(RankMaxCount[i]);
			Console->Execute(str.c_str());
		}
}

void TSMP_ShopProcessor_BuyItems(game_PlayerState* ps, u16 ActorID, game_sv_Deathmatch *game)
{	
	std::vector<std::string> ItemsToBuy;
	ItemsToBuy.reserve(ps->pItemList.size());	

	for (u16 ItemID : ps->pItemList)
	{
		bool notFoundIdx = false;
		shared_str Name = game->m_strWeaponsData->GetItemName(ItemID & 0x00FF, &notFoundIdx);

		if (notFoundIdx)
		{
			u32 idx = ItemID & 0x00FF;
			u32 maxIdx = game->m_strWeaponsData->GetItemsCount();

			std::string str;
			str += "chat_tsmp Внимание! У игрока ";
			str += std::string(ps->name);
			str += " геймдата отличается от нормальной! Он пытался купить предмет с индексом ";
			str += std::to_string(idx);
			str += ". Максимальный индекс предметов - ";
			str += std::to_string(maxIdx);
			
			Console->Execute(str.c_str());
			continue;
		}

		if (NotAllowedToBuy(Name.c_str()))
			continue;

		if (g_TsmpBuyVeriferEnabled)
			ItemsToBuy.push_back(Name.c_str());

		u8 Addons = CorrectAddons(u8((ItemID & 0xFF00) >> 0x08));
		game->SpawnWeapon4Actor(ActorID, *Name,Addons );
	}

	if (g_TsmpBuyVeriferEnabled && !game->m_bInWarmUp)
		CheckItemsCounts(ItemsToBuy,RankCountRestrictions[ps->rank],ps->rank,ps);
}
