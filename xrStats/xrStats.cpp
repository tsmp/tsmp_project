#include <iostream>
#include <fstream>
#include <vector>
#include <string>
#include <algorithm>

using namespace std;

std::vector<std::string>	StrVec;
std::vector<std::string>	StrVec2;

std::vector<std::string>	WeaponVec;
std::vector<int>			WeaponCount;

std::vector<std::string>	PlayerVec;
std::vector<int>			PlayerCount;

std::string					Path;
std::ofstream				Log;

void SortVectors(std::vector<int> &iVec, std::vector<std::string> &sVec)
{
	bool changed = true;

	while (changed)
	{
		changed = false;

		for (size_t i = 0; i < (iVec.size() - 1); i++)
		{
			if (iVec[i] < iVec[i + 1])
			{
				std::swap(iVec[i], iVec[i + 1]);
				std::swap(sVec[i], sVec[i + 1]);

				changed = true;
			}
		}
	}
}

void AddElement(std::string Name, std::vector<int> &iVec, std::vector<std::string> &sVec)
{
	bool bFound = false;

	for (size_t i = 0; i < sVec.size(); ++i)
	{
		if (sVec[i] == Name)
		{
			iVec[i]++;
			bFound = true;
		}
	}

	if (!bFound)
	{
		sVec.push_back(Name);
		iVec.push_back(1);
	}
}

void LogStatisticsFromVectors(std::vector<int> &iVec, std::vector<std::string> &sVec)
{
	for (size_t i = 0; i < sVec.size(); i++)
	{
		Log << i + 1 << ". " << iVec[i] << " " << sVec[i] << endl;
	}
}

void ReadLog()
{
	cout << "Чтение лога"<<endl;

	int same = 0;
	std::string s, s1;
	std::ifstream file(Path);

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
}

void RemoveUnused()
{
	cout << "Обработка лога" << endl;

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

	cout << "Сбор статистики оружия" << endl;

	for (long i = 0; i < StrVec2.size(); ++i)
	{		
		std::string ot = " от ";
		std::string wpn;

		char *tmp_ch=new char[32]{0};		
		const char* src = strstr(StrVec2[i].c_str(), ot.c_str());

		strcpy(tmp_ch, src+ot.size());
		wpn = tmp_ch;

		AddElement(wpn, WeaponCount, WeaponVec);		
	}

	SortVectors(WeaponCount, WeaponVec);
}

void ProcessPlayers()
{
	cout << "Сбор статистики игроков" << endl;

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

		AddElement(pl, PlayerCount, PlayerVec);
	}
	
	SortVectors(PlayerCount, PlayerVec);
}

void ShowResults()
{
	Log.open(Path + "_xrStats.txt");

	Log << endl << "		Общая статистика:" << endl;
	Log << "Строк: " << StrVec.size() << endl;
	Log << "Убийств: " << StrVec2.size() << endl;
	Log << "Игроков : " << PlayerVec.size() << endl;

	Log << endl << "		Статистика использования оружия:" << endl;
	LogStatisticsFromVectors(WeaponCount, WeaponVec);

	Log << endl << "		Статистика фрагов игроков:" << endl;
	LogStatisticsFromVectors(PlayerCount,PlayerVec);

	Log.close();

	cout << "Готово" << endl;
}

void InputPath()
{
	cout << " Введите путь к логу " << endl;
	cin >> Path;
}

int main()
{
	setlocale(LC_ALL, "rus");

	InputPath		();
	ReadLog			();
	RemoveUnused	();
	ProcessPlayers	();
	ShowResults		();

	system("pause");
	return 0;
}
