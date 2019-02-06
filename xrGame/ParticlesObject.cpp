//----------------------------------------------------
// file: PSObject.cpp
//----------------------------------------------------
#include "stdafx.h"
#pragma hdrstop

#include "ParticlesObject.h"
#include "../defines.h"
#include "../fbasicvisual.h"
#include "../ParticleCustom.h"
#include "../render.h"
#include "../IGame_Persistent.h"

extern bool bIsDedicatedServer;

const Fvector zero_vel		= {0.f,0.f,0.f};

CParticlesObject::CParticlesObject	(LPCSTR p_name, BOOL bAutoRemove, bool destroy_on_game_load) :
	inherited				(destroy_on_game_load)
{
	Init					(p_name,0,bAutoRemove);
}

void CParticlesObject::Init	(LPCSTR p_name, IRender_Sector* S, BOOL bAutoRemove)
{
	m_bLooped				= false;
	m_bStopping				= false;
	m_bAutoRemove			= bAutoRemove;
	float time_limit		= 0.0f;

	if(!bIsDedicatedServer)
	{
		// create visual
		renderable.visual		= Render->model_CreateParticles(p_name);
		VERIFY					(renderable.visual);
		IParticleCustom* V		= smart_cast<IParticleCustom*>(renderable.visual);  VERIFY(V);
		time_limit				= V->GetTimeLimit();
	}else
	{
		time_limit					= 1.0f;
	}

	if(time_limit > 0.f)
	{
		m_iLifeTime			= iFloor(time_limit*1000.f);
	}
	else
	{
		if(bAutoRemove)
		{
			R_ASSERT3			(!m_bAutoRemove,"Can't set auto-remove flag for looped particle system.",p_name);
		}
		else
		{
			m_iLifeTime = 0;
			m_bLooped = true;
		}
	}


	// spatial
	spatial.type			= 0;
	spatial.sector			= S;
	
	// sheduled
	shedule.t_min			= 20;
	shedule.t_max			= 50;
	shedule_register		();

	dwLastTime				= Device.dwTimeGlobal;
	mt_dt					= 0;
}

//----------------------------------------------------
CParticlesObject::~CParticlesObject()
{
	VERIFY					(0==mt_dt);

//	we do not need this since CPS_Instance does it
//	shedule_unregister		();
}

void CParticlesObject::UpdateSpatial()
{
	if(bIsDedicatedServer)		return;

	// spatial	(+ workaround occasional bug inside particle-system)
	if (_valid(renderable.visual->vis.sphere))
	{
		Fvector	P;	float	R;
		renderable.xform.transform_tiny	(P,renderable.visual->vis.sphere.P);
		R								= renderable.visual->vis.sphere.R;
		if (0==spatial.type)	{
			// First 'valid' update - register
			spatial.type			= STYPE_RENDERABLE;
			spatial.sphere.set		(P,R);
			spatial_register		();
		} else {
			BOOL	bMove			= FALSE;
			if		(!P.similar(spatial.sphere.P,EPS_L*10.f))		bMove	= TRUE;
			if		(!fsimilar(R,spatial.sphere.R,0.15f))			bMove	= TRUE;
			if		(bMove)			{
				spatial.sphere.set	(P, R);
				spatial_move		();
			}
		}
	}
}

const shared_str CParticlesObject::Name()
{
	if(bIsDedicatedServer)	return "";

	IParticleCustom* V	= smart_cast<IParticleCustom*>(renderable.visual); VERIFY(V);
	return (V) ? V->Name() : "";
}

//----------------------------------------------------
void CParticlesObject::Play		()
{
	if(bIsDedicatedServer)		return;

	IParticleCustom* V			= smart_cast<IParticleCustom*>(renderable.visual); VERIFY(V);
	V->Play						();
	dwLastTime					= Device.dwTimeGlobal-33ul;
	mt_dt						= 0;
	PerformAllTheWork			(0);
	m_bStopping					= false;
}

void CParticlesObject::play_at_pos(const Fvector& pos, BOOL xform)
{
	if(bIsDedicatedServer)		return;

	IParticleCustom* V			= smart_cast<IParticleCustom*>(renderable.visual); VERIFY(V);
	Fmatrix m; m.translate		(pos); 
	V->UpdateParent				(m,zero_vel,xform);
	V->Play						();
	dwLastTime					= Device.dwTimeGlobal-33ul;
	mt_dt						= 0;
	PerformAllTheWork			(0);
	m_bStopping					= false;
}

void CParticlesObject::Stop		(BOOL bDefferedStop)
{
	if(bIsDedicatedServer)		return;

	IParticleCustom* V			= smart_cast<IParticleCustom*>(renderable.visual); VERIFY(V);
	V->Stop						(bDefferedStop);
	m_bStopping					= true;
}

void CParticlesObject::shedule_Update	(u32 _dt)
{
	inherited::shedule_Update		(_dt);

	if(bIsDedicatedServer)		return;

	// Update
	if (m_bDead)					return;
	u32 dt							= Device.dwTimeGlobal - dwLastTime;
	if (dt)							{
		if (0){//.psDeviceFlags.test(mtParticles))	{    //. AlexMX comment this line// NO UNCOMMENT - DON'T WORK PROPERLY
			mt_dt					= dt;
			fastdelegate::FastDelegate0<>		delegate	(this,&CParticlesObject::PerformAllTheWork_mt);
			Device.seqParallel.push_back		(delegate);
		} else {
			mt_dt					= 0;
			IParticleCustom* V		= smart_cast<IParticleCustom*>(renderable.visual); VERIFY(V);
			V->OnFrame				(dt);
		}
		dwLastTime					= Device.dwTimeGlobal;
	}
	UpdateSpatial					();
}

void CParticlesObject::PerformAllTheWork(u32 _dt)
{
	if(bIsDedicatedServer)		return;

	// Update
	u32 dt							= Device.dwTimeGlobal - dwLastTime;
	if (dt)							{
		IParticleCustom* V		= smart_cast<IParticleCustom*>(renderable.visual); VERIFY(V);
		V->OnFrame				(dt);
		dwLastTime				= Device.dwTimeGlobal;
	}
	UpdateSpatial					();
}

void CParticlesObject::PerformAllTheWork_mt()
{
	if(bIsDedicatedServer)		return;

	if (0==mt_dt)			return;	//???
	IParticleCustom* V		= smart_cast<IParticleCustom*>(renderable.visual); VERIFY(V);
	V->OnFrame				(mt_dt);
	mt_dt					= 0;
}

void CParticlesObject::SetXFORM			(const Fmatrix& m)
{
	if(bIsDedicatedServer)		return;

	IParticleCustom* V	= smart_cast<IParticleCustom*>(renderable.visual); VERIFY(V);
	V->UpdateParent		(m,zero_vel,TRUE);
	renderable.xform.set(m);
	UpdateSpatial		();
}

void CParticlesObject::UpdateParent		(const Fmatrix& m, const Fvector& vel)
{
	if(bIsDedicatedServer)		return;

	IParticleCustom* V	= smart_cast<IParticleCustom*>(renderable.visual); VERIFY(V);
	V->UpdateParent		(m,vel,FALSE);
	UpdateSpatial		();
}

Fvector& CParticlesObject::Position		()
{
	if(bIsDedicatedServer) 
	{
		static Fvector _pos = Fvector().set(0,0,0);
		return _pos;
	}
	return renderable.visual->vis.sphere.P;
}

float CParticlesObject::shedule_Scale		()	
{ 
	if(bIsDedicatedServer)		return 5.0f;

	return Device.vCameraPosition.distance_to(Position())/200.f; 
}

void CParticlesObject::renderable_Render	()
{
	VERIFY					(renderable.visual);
	u32 dt					= Device.dwTimeGlobal - dwLastTime;
	if (dt){
		IParticleCustom* V	= smart_cast<IParticleCustom*>(renderable.visual); VERIFY(V);
		V->OnFrame			(dt);
		dwLastTime			= Device.dwTimeGlobal;
	}
	::Render->set_Transform	(&renderable.xform);
	::Render->add_Visual	(renderable.visual);
}
bool CParticlesObject::IsAutoRemove			()
{
	if(m_bAutoRemove) return true;
	else return false;
}
void CParticlesObject::SetAutoRemove		(bool auto_remove)
{
	VERIFY(m_bStopping || !IsLooped());
	m_bAutoRemove = auto_remove;
}

//�������� �� ��������, ���������� �� PSI_Alive, ��� ��� �����
//��������� Stop �������� ����� ��� ���������� �������� IsPlaying = true
bool CParticlesObject::IsPlaying()
{
	if(bIsDedicatedServer)		return false;

	IParticleCustom* V	= smart_cast<IParticleCustom*>(renderable.visual); 
	VERIFY(V);
	return !!V->IsPlaying();
}
