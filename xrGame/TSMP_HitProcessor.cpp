#include "StdAfx.h"
#include "TSMP_HitProcessor.h"

volatile int g_sv_mp_CheckHitsEnabled = 0;

#define MinHitsToProcess 100

HitProcessor::~HitProcessor() 
{
	Msg("- Destroying hit processor");
	ThreadWorking = false;

	Buffer1.clear();
	Buffer2.clear();
}

void HitProcessor::AddHit(HitInfo &NewHit)
{
	if (UseBuffer1)
		AddHitPrivate(NewHit, Buffer1);
	else
		AddHitPrivate(NewHit, Buffer2);
}

void HitProcessor::AddHitPrivate(HitInfo &HitToAdd, std::vector<HitInfo> &Buf)
{
	HitLogger_CS.Enter();
	Buf.push_back(HitToAdd);
	HitLogger_CS.Leave();
}

void ProcessHits(HitProcessor *P, std::vector<HitProcessor::HitInfo> Buffer)
{
	if (Buffer.size() < MinHitsToProcess)
		return;

	P->UseBuffer1 = (P->UseBuffer1) ? false : true;

	HitProcessor::HitInfo LastHit		= { 0 };
	HitProcessor::HitInfo CurrentHit	= { 0 };

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
			Msg("! Parted hit detected. Cheater?");
			CurrentHit = LastHit;
			i--;
		}
		else
			CurrentHit = Buffer[i];

		string128 Message;

		sprintf(Message, "# %s [%s] HT %i HP %i+%0.2f - | BT %i | HIM %0.2f | K_AP %0.2f"
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

		if (g_sv_mp_CheckHitsEnabled)
			P->CheckForCheats(CurrentHit, iBulletCount);

		iBulletCount = 1;	
		LastHit = CurrentHit;
	}

	Buffer.clear();
	Buffer.reserve(MinHitsToProcess);
}

void HitProcessingThread(void *P)
{
	HitProcessor *HP = (HitProcessor*)P;

	HP->ThreadWorking = true;
	SetThreadPriority(GetCurrentThread(), THREAD_PRIORITY_BELOW_NORMAL);

	while (true)
	{
		if (!HP->ThreadWorking)
		{
			Msg("- Stopping hitprocessing thread");
			break;
		}

		if (HP->UseBuffer1)
			ProcessHits(HP, HP->Buffer1);
		else 
			ProcessHits(HP, HP->Buffer2);		
		
		Sleep(60000);
	}
}

HitProcessor::HitProcessor()
{
	Msg("- Starting hit processor");

	UseBuffer1		= true;
	ThreadWorking	= false;

	Buffer1.reserve(MinHitsToProcess);
	Buffer2.reserve(MinHitsToProcess);

	thread_spawn(HitProcessingThread, "hits-processing thread", 0, this);
}

struct Weapon
{
	string32 WeaponName;
	float HitPower;
	float HitImpulse;
	float bHitPower;
	float bHitImpulse;
	bool Drob;
};

static std::vector<Weapon> WPN = {
{ "mp_wpn_knife"			, 1.80 , 120.00		, 1.80, 120.00	, false },
{ "mp_wpn_pm"				, 0.55 , 70.00		, 0.47, 91.00	, false },
{ "mp_wpn_fort"				, 0.55 , 77.00		, 0.47, 100.10	, false },
{ "mp_wpn_walther"			, 0.65 , 87.50		, 0.55, 131.25	, false },
{ "mp_wpn_sig220"			, 0.74 , 105.50		, 0.63, 126.60	, false },
{ "mp_wpn_usp"				, 0.78 , 105.50		, 0.66, 126.60	, false },
{ "mp_wpn_desert_eagle"		, 1.50 , 120.00		, 1.28, 144.00	, false },
{ "mp_wpn_pb"				, 0.50 , 56.00		, 0.43, 72.80	, false },
{ "mp_wpn_colt1911"			, 0.84 , 105.00		, 0.71, 126.00	, false },
{ "mp_wpn_bm16"				, 1.80 , 315.00		, 1.26, 273.00	, true },
{ "mp_wpn_wincheaster1300"	, 1.80 , 315.00		, 1.26, 273.00	, true },
{ "mp_wpn_spas12"			, 2.00 , 315.00		, 1.40, 273.00	, true },
{ "mp_wpn_ak74u"			, 0.70 , 140.00		, 0.60, 133.00	, false },
{ "mp_wpn_ak74"				, 0.81 , 140.00		, 0.69, 133.00	, false },
{ "mp_wpn_abakan"			, 0.90 , 140.00		, 0.77, 133.00	, false },
{ "mp_wpn_groza"			, 0.95 , 184.80		, 0.81, 146.30	, false },
{ "mp_wpn_val"				, 1.00 , 114.00		, 0.85, 90.25	, false },
{ "mp_wpn_fn2000"			, 0.86 , 105.00		, 0.73, 105.00	, false },
{ "mp_wpn_mp5"				, 0.58 , 140.00		, 0.49, 210.00	, false },
{ "mp_wpn_l85"				, 0.75 , 140.00		, 0.64, 140.00	, false },
{ "mp_wpn_lr300"			, 0.84 , 140.00		, 0.71, 140.00	, false },
{ "mp_wpn_sig550"			, 0.88 , 140.00		, 0.75, 140.00	, false },
{ "mp_wpn_g36"				, 0.80 , 105.00		, 0.68, 105.00	, false },
{ "mp_wpn_vintorez"			, 1.00 , 104.40		, 0.85, 82.65	, false },
{ "mp_wpn_svu"				, 2.60 , 224.30		, 2.34, 212.90	, false },
{ "mp_wpn_svd"				, 2.60 , 245.30		, 2.34, 232.90	, false },
{ "mp_wpn_gauss"			, 5.00 , 3000.8		, 5.00, 3000.8	, false }
};


void HitProcessor::CheckForCheats(HitInfo &Hit, int Bullets)
{
	int WpnIdx = -1;

	for (int i = 0; i < WPN.size(); i++)
	{
		if (strcmp(Hit.StrWeaponName, WPN[i].WeaponName) == 0)
			WpnIdx = i;
	}

	if (WpnIdx == -1)
	{
		Msg("! Cant find weapon in list: %s", Hit.StrWeaponName);
		return;
	}

	if (!WPN[WpnIdx].Drob && (Bullets > 4))
	{
		Msg("! Cheater detected. Player: %s had %i bullets from %s"
			, Hit.StrPlayerName
			, Bullets
			, Hit.StrWeaponName);
	}

	if (Hit.fAP < 0.001)
	{
		if (Hit.fImpulse-0.1 > WPN[WpnIdx].HitImpulse)
			Msg("! Cheater detected. Player: %s had %0.2f impulse from %s (max: %0.2f)"
				, Hit.StrPlayerName
				, Hit.fImpulse
				, Hit.StrWeaponName
				, WPN[WpnIdx].HitImpulse
			);

		if (Hit.fPower-0.1 > WPN[WpnIdx].HitPower)
			Msg("! Cheater detected. Player: %s had %0.2f power from %s (max: %0.2f)"
				, Hit.StrPlayerName
				, Hit.fPower
				, Hit.StrWeaponName
				, WPN[WpnIdx].HitPower
			);
	}
	else
	{
		if (Hit.fImpulse-0.1 > WPN[WpnIdx].bHitImpulse)
			Msg("! Cheater detected. Player: %s had %0.2f impulse from %s b (max: %0.2f)"
				, Hit.StrPlayerName
				, Hit.fImpulse
				, Hit.StrWeaponName
				, WPN[WpnIdx].bHitImpulse
			);

		if (Hit.fPower-0.1 > WPN[WpnIdx].bHitPower)
			Msg("! Cheater detected. Player: %s had %0.2f power from %s b (max: %0.2f)"
				, Hit.StrPlayerName
				, Hit.fPower
				, Hit.StrWeaponName
				, WPN[WpnIdx].bHitPower
			);
	}
}
