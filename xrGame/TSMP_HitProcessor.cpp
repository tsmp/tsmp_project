#include "StdAfx.h"
#include "TSMP_HitProcessor.h"
#include "../XR_IOConsole.h"

volatile int g_sv_mp_CheckHitsEnabled = 0;
volatile int g_sv_mp_AutoBanHitCheaters = 0;
volatile int g_sv_mp_ShowHits = 0;

constexpr auto MinHitsToProcess = 50;;

HitProcessor::~HitProcessor()
{
	Msg("- Destroying hit processor");

	if (hThread)
	{
		Msg("- Destroying hit processing thread");
		TerminateThread(hThread, 0);
	}
	else
		Msg("! hit processing thread handle = 0");

	Buffer1.clear();
	Buffer2.clear();
}

void HitProcessor::AddHit(HitInfo& NewHit)
{
	if (UseBuffer1)
		AddHitPrivate(NewHit, Buffer1);
	else
		AddHitPrivate(NewHit, Buffer2);
}

void HitProcessor::AddHitPrivate(HitInfo& HitToAdd, std::vector<HitInfo>& Buf)
{
	HitLogger_CS.Enter();
	Buf.push_back(std::move(HitToAdd));
	HitLogger_CS.Leave();
}

void ProcessHits(HitProcessor* P, std::vector<HitProcessor::HitInfo> Buffer)
{
	if (Buffer.size() <= MinHitsToProcess)
		return;

	if (P->UseBuffer1)
		P->UseBuffer1 = false;
	else
		P->UseBuffer1 = true;

	HitProcessor::HitInfo LastHit = { 0 };
	HitProcessor::HitInfo CurrentHit = { 0 };

	int iBulletCount = 1;

	for (int i = 0; i < (int)Buffer.size(); i++)
	{
		if (Buffer[i] == LastHit)
		{
			iBulletCount++;
			continue;
		}

		if (iBulletCount > 1)
		{
			CurrentHit = LastHit;
			i--;
		}
		else
			CurrentHit = Buffer[i];

		if (g_sv_mp_ShowHits)
		{
			string128 Message;

			sprintf_s(
				Message
				,sizeof(Message)
				, "# %s [%s] HT %i HP %i+%0.2f - | BT %i | HIM %0.2f | K_AP %0.2f"
				, CurrentHit.StrPlayerName
				, CurrentHit.StrWeaponName
				, CurrentHit.iHitType
				, iBulletCount
				, CurrentHit.fPower
				, CurrentHit.iBoneID
				, CurrentHit.fImpulse
				, CurrentHit.fAP
			);

			Msg(Message);
		}

		if (g_sv_mp_CheckHitsEnabled)
			P->CheckForCheats(CurrentHit, iBulletCount);

		iBulletCount = 1;
		LastHit = CurrentHit;
	}

	Buffer.erase(Buffer.begin(), Buffer.end());
	Buffer.reserve(MinHitsToProcess);
}

DWORD WINAPI  HitProcessingThread(void* P)
{
	HitProcessor* HP = (HitProcessor*)P;

	while (true)
	{
		if (HP)
		{
			if (HP->UseBuffer1)
				ProcessHits(HP, HP->Buffer1);
			else
				ProcessHits(HP, HP->Buffer2);
		}

		Sleep(20000);
	}
}

HitProcessor::HitProcessor()
{
	Msg("- Starting hit processor");

	UseBuffer1 = true;
	hThread = nullptr;

	Buffer1.reserve(MinHitsToProcess);
	Buffer2.reserve(MinHitsToProcess);

	Msg("- Starting hit processing thread");
	hThread = CreateThread(0,0,&HitProcessingThread,this,0,0);
	SetThreadPriority(hThread, THREAD_PRIORITY_LOWEST);
}

struct Weapon
{
	float HitPower;
	float HitImpulse;
	float bHitPower;
	float bHitImpulse;
	bool Drob;
};

static const std::map<std::string, Weapon> WPN =
{
	{"mp_wpn_knife"	,{ 9999999.F , 120.00, 9999999.F, 120.00	, false }},
	{"mp_wpn_pm"				,{ 0.55 , 70.00		, 0.47, 91.00	, false }},
	{"mp_wpn_fort"				,{ 0.55 , 77.00		, 0.47, 100.10	, false }},
	{"mp_wpn_walther"			,{ 0.65 , 87.50		, 0.55, 131.25	, false }},
	{"mp_wpn_sig220"			,{ 0.74 , 105.50	, 0.63, 126.60	, false }},
	{"mp_wpn_usp"				,{ 0.78 , 105.50	, 0.66, 126.60	, false }},
	{"mp_wpn_desert_eagle"		,{ 1.50 , 120.00	, 1.28, 144.00	, false }},
	{"mp_wpn_pb"				,{ 0.50 , 56.00		, 0.43, 72.80	, false }},
	{"mp_wpn_colt1911"			,{ 0.84 , 105.00	, 0.71, 126.00	, false }},
	{"mp_wpn_bm16"				,{ 1.80 , 315.00	, 1.26, 273.00	, true }},
	{"mp_wpn_wincheaster1300"	,{ 1.80 , 315.00	, 1.26, 273.00	, true }},
	{"mp_wpn_spas12"			,{ 2.00 , 315.00	, 1.40, 273.00	, true }},
	{"mp_wpn_ak74u"				,{ 0.70 , 140.00	, 0.60, 140.00	, false }},
	{"mp_wpn_ak74"				,{ 0.81 , 140.00	, 0.69, 140.00	, false }},
	{"mp_wpn_abakan"			,{ 0.90 , 140.00	, 0.77, 140.00	, false }},
	{"mp_wpn_groza"				,{ 0.95 , 184.80	, 0.81, 146.30	, false }},
	{"mp_wpn_val"				,{ 1.00 , 114.00	, 0.85, 90.25	, false }},
	{"mp_wpn_fn2000"			,{ 0.86 , 105.00	, 0.73, 105.00	, false }},
	{"mp_wpn_mp5"				,{ 0.58 , 140.00	, 0.49, 210.00	, false }},
	{"mp_wpn_l85"				,{ 0.75 , 140.00	, 0.64, 140.00	, false }},
	{"mp_wpn_lr300"				,{ 0.84 , 140.00	, 0.71, 140.00	, false }},
	{"mp_wpn_sig550"			,{ 0.88 , 140.00	, 0.75, 140.00	, false }},
	{"mp_wpn_g36"				,{ 0.80 , 105.00	, 0.68, 105.00	, false }},
	{"mp_wpn_vintorez"			,{ 1.00 , 104.40	, 0.85, 82.65	, false }},
	{"mp_wpn_svu"				,{ 2.60 , 224.30	, 2.34, 212.90	, false }},
	{"mp_wpn_svd"				,{ 2.60 , 245.30	, 2.34, 232.90	, false }},
	{"mp_wpn_gauss"				,{ 5.00 , 3000.8	, 5.00, 3000.8	, false }}
};

void BanCheater(u32 id, string128 &Message)
{
	static std::vector<u32> Cheaters;
	int already_detected = 0;

	for (u32 uu : Cheaters)
		if (uu == id)
			already_detected++;

	Cheaters.push_back(id);

	if (already_detected <= 3)
	{
		string128 Mes;
		sprintf_s(Mes, sizeof(Mes), "chat_tsmp %s", Message);
		Console->Execute(Mes);
	}

	if (g_sv_mp_AutoBanHitCheaters && already_detected == 3)
	{
		string128 Arg;
		sprintf_s(Arg,sizeof(Arg), "sv_banplayer_id %u 99999999", id);
		Console->Execute(Arg);
	}	
}

void HitProcessor::CheckForCheats(HitInfo& Hit, int Bullets)
{
	static const int Fault = 2;

	auto a = WPN.find(Hit.StrWeaponName);
	string128 message{ 0 };

	if (a == WPN.end())
		return;

	if (!a->second.Drob && (Bullets > 4))
	{
		sprintf_s(message,sizeof(message), "Cheater detected. Player: %s had %i bullets from %s"
			, Hit.StrPlayerName
			, Bullets
			, Hit.StrWeaponName);

		BanCheater(Hit.iPlayerID, message);
	}

	if (Hit.fAP < 0.001)
	{
		if (Hit.fImpulse - Fault > a->second.HitImpulse)
		{
			sprintf_s(message,sizeof(message), "Cheater detected. Player: %s had %0.2f impulse from %s (max: %0.2f)"
				, Hit.StrPlayerName
				, Hit.fImpulse
				, Hit.StrWeaponName
				, a->second.HitImpulse
			);

			BanCheater(Hit.iPlayerID, message);
		}

		if (Hit.fPower - 1 > a->second.HitPower)
		{
			sprintf_s(message, sizeof(message),"Cheater detected. Player: %s had %0.2f power from %s (max: %0.2f)"
				, Hit.StrPlayerName
				, Hit.fPower
				, Hit.StrWeaponName
				, a->second.HitPower
			);

			BanCheater(Hit.iPlayerID, message);
		}
	}
	else
	{
		if (Hit.fImpulse - Fault > a->second.bHitImpulse)
		{
			sprintf_s(message,sizeof(message), "Cheater detected. Player: %s had %0.2f impulse from %s b (max: %0.2f)"
				, Hit.StrPlayerName
				, Hit.fImpulse
				, Hit.StrWeaponName
				, a->second.bHitImpulse
			);

			BanCheater(Hit.iPlayerID, message);
		}

		if (Hit.fPower - 1 > a->second.bHitPower)
		{
			sprintf_s(message,sizeof(message), "Cheater detected. Player: %s had %0.2f power from %s b (max: %0.2f)"
				, Hit.StrPlayerName
				, Hit.fPower
				, Hit.StrWeaponName
				, a->second.bHitPower
			);

			BanCheater(Hit.iPlayerID, message);
		}
	}
}
