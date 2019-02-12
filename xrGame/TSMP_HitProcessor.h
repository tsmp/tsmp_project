#pragma once
#include "StdAfx.h"

class HitProcessor
{
public:
	struct HitInfo
	{
		float		fImpulse;
		float		fPower;
		float		fAP;
		string32	StrPlayerName;
		string32	StrWeaponName;
		int			iBoneID;
		int			iHitType;
		int			iPlayerID;
		u32			uTime;

		bool operator==(const HitInfo &HI)
		{
			return HI.iPlayerID == iPlayerID
				&& HI.uTime == uTime;
		}
	};	

	HitProcessor();
	~HitProcessor();

	void AddHit(HitInfo	&NewHit);

	volatile bool UseBuffer1;
	volatile bool ThreadWorking;

	std::vector<HitInfo> Buffer1;
	std::vector<HitInfo> Buffer2;

	void AddHitPrivate(HitInfo &HitToAdd, std::vector<HitInfo> &Buf);

	xrCriticalSection HitLogger_CS;
};
