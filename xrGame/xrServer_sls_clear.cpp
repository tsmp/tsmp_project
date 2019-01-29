#include "stdafx.h"
#include "game_sv_single.h"
#include "alife_simulator.h"
#include "xrServer_Objects.h"
#include "xrServer.h"
#include "xrmessages.h"
#include "ai_space.h"

void xrServer::Perform_destroy(CSE_Abstract* object, u32 mode)
{
	R_ASSERT(object);
	R_ASSERT(object->ID_Parent == 0xffff);

#ifdef DEBUG
#ifdef SLOW_VERIFY_ENTITIES
	verify_entities();
#endif
#endif

	while (!object->children.empty())
	{
		CSE_Abstract *child = game->get_entity_from_eid(object->children.back());

		R_ASSERT2(child, make_string("child registered but not found [%d]", object->children.back()));

		Perform_reject(child, object, 2 * NET_Latency);

#ifdef DEBUG
#ifdef SLOW_VERIFY_ENTITIES
		verify_entities();
#endif
#endif

		Perform_destroy(child, mode);
	}

	u16	object_id = object->ID;
	entity_Destroy(object);

#ifdef DEBUG
#ifdef SLOW_VERIFY_ENTITIES
	verify_entities();
#endif
#endif

	NET_Packet P;

	P.w_begin(M_EVENT);
	P.w_u32(Device.dwTimeGlobal - 2 * NET_Latency);
	P.w_u16(GE_DESTROY);
	P.w_u16(object_id);

	SendBroadcast(BroadcastCID, P, mode);
}

void xrServer::SLS_Clear()
{
	Msg("sls_clear called");

	u32 mode = net_flags(TRUE, TRUE);

	Msg("sls_clear mode %i",mode);

	while (!entities.empty())
	{
		Msg("sls_clear mode !entities_empty");

		bool found = false;

		xrS_entities::const_iterator I = entities.begin();
		xrS_entities::const_iterator E = entities.end();

		for (; I != E; ++I)
		{
			if ((*I).second->ID_Parent != 0xffff)
			{
				Msg("sls_clear mode continue");

				continue;
			}

			found = true;
			Msg("sls_clear mode found =  true");

			Perform_destroy((*I).second, mode);
			break;
		}

		//	R_ASSERT(found);
		if (!found)
		{
			Msg("! error xrServer_sls_clear.cpp:xrServer::SLS_Clear found");
			break;
		}
	}
}
