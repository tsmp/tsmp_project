#pragma once

class ClientID
{
public:
	ClientID() :id(0) {};
	ClientID(u32 val) :id(val) {};

	u32	value()const { return id; };
	void set(u32 v) { id = v; };

	bool compare(u32 v) const { return id == v; };

	bool operator == (const ClientID &other)const { return value() == other.value(); };
	bool operator != (const ClientID &other)const { return value() != other.value(); };
	bool operator <	 (const ClientID &other)const { return value() <  other.value(); };
	
private:
	u32 id;
};

