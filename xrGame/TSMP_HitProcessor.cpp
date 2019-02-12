#include "StdAfx.h"
#include "TSMP_HitProcessor.h"

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
