#include <iostream>
#include <fstream>
#include <vector>
#include <string>

using namespace std;

std::vector<std::string> StrVec;
std::vector<std::string> StrVec2;

std::vector<std::string>	WeaponVec;
std::vector<int>			WeaponCount;

std::vector<std::string>	PlayerVec;
std::vector<int>			PlayerCount;

void SortWeapons()
{
	bool changed = true;

	while (changed)
	{
		for (size_t i = 0; i < (WeaponCount.size() - 1); i++)
		{
			changed = false;

			if (WeaponCount[i] < WeaponCount[i + 1])
			{
				int temp = WeaponCount[i];
				std::string tempstr= WeaponVec[i];

				WeaponCount[i] = WeaponCount[i + 1];
				WeaponCount[i + 1] = temp;

				WeaponVec[i] = WeaponVec[i + 1];
				WeaponVec[i + 1] = temp;

				changed = true;
			}
		}
	}
}

void AddWeapon(std::string str)
{
	bool bFound = false;
	
	for (size_t i = 0; i < WeaponVec.size(); ++i)
	{
		if (WeaponVec[i] == str)
		{
			WeaponCount[i]++;
			bFound = true;
		}	
	}

	if(!bFound)
	{	
		WeaponVec.push_back(str);
		WeaponCount.push_back(1);
	}
};

void ShowWeaponStats()
{
	for (size_t i = 0; i < WeaponVec.size(); i++)
	{
		cout << WeaponCount[i] <<" "<< WeaponVec[i] << endl;
	}
};

void Read()
{
	cout << "reading log"<<endl;

	int same = 0;
	std::string s, s1;
	std::ifstream file("C:\\logs\\log1.txt");

	while (true)
	{
		std::getline(file, s);

		if (s == s1) 
			same++;
		else 
			same = 0;

		if (same > 100) 
			break;

		StrVec.push_back(s);

		if(!strstr(s.c_str(), "ownership"))
			s1 = s;
	}

	file.close();

	cout << "reading finished" << endl;	
}

void Process()
{
	cout << "processing log" << endl;

	for (long i = 0; i < StrVec.size(); ++i)
	{
		if (strstr(StrVec[i].c_str(), "killed") && strstr(StrVec[i].c_str(), " от "))
		{
			if (strstr(StrVec[i].c_str(), "в голову!!!"))
			{
				// убираем надпись "в голову!!!" из строк где она есть

				std::string head = "в голову!!!";
				char *buf = new char[100]{0};

				size_t str_size = StrVec[i].size() - head.size();

				strncpy(buf, StrVec[i].c_str(), str_size);

				std::string tmp;
				tmp= buf;
				StrVec2.push_back(tmp);
			}
			else 
				StrVec2.push_back(StrVec[i]);
		}
	}

	cout << "detaching weapon names" << endl;

	for (long i = 0; i < StrVec2.size(); ++i)
	{		
		std::string ot = " от ";
		std::string wpn;

		char *tmp_ch=new char[32]{0};		
		const char* src = strstr(StrVec2[i].c_str(), ot.c_str());

		strcpy(tmp_ch, src+ot.size());
		wpn = tmp_ch;

		AddWeapon(wpn);		
	}
	
	cout <<"Строк: "<< StrVec.size() << endl;
	cout <<"Убийств: "<< StrVec2.size() << endl;

	SortWeapons();
	ShowWeaponStats();
}

void AddPlayer(std::string str)
{
	bool bFound = false;

	for (size_t i = 0; i < PlayerVec.size(); ++i)
	{
		if (PlayerVec[i] == str)
		{
			PlayerCount[i]++;
			bFound = true;
		}
	}

	if (!bFound)
	{
		PlayerVec.push_back(str);
		PlayerCount.push_back(1);
	}
};

void ShowPlayersStats()
{
	for (size_t i = 0; i < PlayerVec.size(); i++)
	{
		cout << PlayerCount[i] << " " << PlayerVec[i] << endl;
	}
};

void ProcessPlayers()
{
//	killed himself
//		killed by anomaly
//	админ killed Maksim от ИЛ86в голову!!!

	std::ofstream output;
	output.open("C:\\logs\\players_stats.txt");

	for (long i = 0; i < StrVec2.size(); ++i)
	{
		if (strstr(StrVec2[i].c_str(), "killed himself") || strstr(StrVec2[i].c_str(), "killed by anomaly"))
			continue;

		std::string ot = " killed ";
		std::string pl;

		char *tmp_ch = new char[32]{ 0 };
		int Move = 0;

		for (int ii = 0; ii < (StrVec2[i].size() - 7); ii++)
		{
			if (StrVec2[i][ii] == ' '
				&&	StrVec2[i][ii + 1] == 'k'
				&&	StrVec2[i][ii + 2] == 'i'
				&&	StrVec2[i][ii + 3] == 'l'
				&&	StrVec2[i][ii + 4] == 'l'
				&&	StrVec2[i][ii + 5] == 'e'
				&&	StrVec2[i][ii + 6] == 'd'
				&&	StrVec2[i][ii + 7] == ' ')
			{
				Move = ii;
				break;
			}
		}

		strncpy(tmp_ch, StrVec2[i].c_str(), Move);
		pl = tmp_ch;

		//cout << pl << endl;
		AddPlayer(pl);
	}

	ShowPlayersStats();

	
		for (size_t i = 0; i < PlayerVec.size(); i++)
		{
			output << PlayerCount[i] << " " << PlayerVec[i] << endl;
		}
	

	cout <<"Игроков : "<< PlayerVec.size() << endl;

	output.close();

}

int main()
{
	setlocale(LC_ALL, "rus");

	Read();
	Process();
//	ProcessPlayers();
	
	system("pause");
	return 0;
}
