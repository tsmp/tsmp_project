#include <iostream>
#include <fstream>
#include <vector>
#include <string>
#include <algorithm>
#include <filesystem>

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
		sVec.push_back(std::move(Name));
		iVec.push_back(1);
	}
}

void LogStatisticsFromVectors(std::vector<int> &iVec, std::vector<std::string> &sVec)
{
	for (size_t i = 0; i < sVec.size(); i++)
		Log << i + 1 << ". \t " << iVec[i] << " \t " << sVec[i] << endl;	
}

void ReadLog(std::string FileName)
{
	cout << "Чтение лога "<< FileName <<endl;
	
	std::ifstream ifs(FileName);
	
	if (!ifs)
		return;
	
	for (std::string line; std::getline(ifs, line); ) 
		StrVec.push_back(std::move(line));
}

void SuperLog()
{
	std::ofstream				Logg;
	Logg.open(Path + "\\SuperLog.txt");

	for (std::string& SS : StrVec)
		Logg << SS << endl;

	Logg.close();
}

void ReadLogs()
{
	for (const auto& entry : std::filesystem::directory_iterator(Path))
		ReadLog(entry.path().string());

	//SuperLog();
}

void RemoveUnused()
{
	cout << "Обработка лога" << endl;

	for (size_t i = 0; i < StrVec.size(); ++i)
	{
		if (strstr(StrVec[i].c_str(), " died from bleeding, thanks to "))
		{
			std::string Bleed = " died from bleeding, thanks to ";
			size_t Pos = StrVec[i].find(Bleed);

			std::string Killer(StrVec[i].begin() + Pos + Bleed.size(), StrVec[i].end()-1);
			std::string Killed(StrVec[i].begin(), StrVec[i].end() - StrVec[i].size() + Pos);

			StrVec[i] = Killer + " killed " + Killed + " от Кровотечение";
		}

		if (strstr(StrVec[i].c_str(), "killed") && strstr(StrVec[i].c_str(), " от "))
		{
			if (strstr(StrVec[i].c_str(), "в голову!!!"))
			{
				// убираем надпись "в голову!!!" из строк где она есть

				std::string head = "в голову!!!";
				StrVec[i].resize(StrVec[i].size() - head.size());
			}
			
			StrVec2.push_back(std::move(StrVec[i]));
		}
	}

	cout << "Сбор статистики оружия" << endl;

	for (size_t i = 0; i < StrVec2.size(); ++i)
	{		
		std::string ot = " от ";

		std::string wpn(StrVec2[i].begin() + StrVec2[i].find(ot)+ot.size()
			, StrVec2[i].end());
		
		AddElement(wpn, WeaponCount, WeaponVec);		
	}

	SortVectors(WeaponCount, WeaponVec);
}

void ProcessPlayers()
{
	cout << "Сбор статистики игроков" << endl;

	for (size_t i = 0; i < StrVec2.size(); ++i)
	{
		std::string killed = " killed ";
		
		std::string Player(StrVec2[i].begin(), StrVec2[i].end() - StrVec2[i].size() + StrVec2[i].find(killed));
		
		AddElement(Player, PlayerCount, PlayerVec);
	}
	
	SortVectors(PlayerCount, PlayerVec);
}

void ShowResults()
{
	Log.open(Path + "_xrStats.txt");

	Log << endl << "		Общая статистика:" << endl;
	Log << "Строк: \t" << StrVec.size() << endl;
	Log << "Убийств: \t" << StrVec2.size() << endl;
	Log << "Игроков : \t" << PlayerVec.size() << endl;

	Log << endl << "		Статистика использования оружия:" << endl;
	LogStatisticsFromVectors(WeaponCount, WeaponVec);

	Log << endl << "		Статистика фрагов игроков:" << endl;
	LogStatisticsFromVectors(PlayerCount,PlayerVec);

	Log.close();

	cout << "Готово" << endl;
}

void InputPath()
{
	//cout << " Введите путь к папке с логами " << endl;
	//cin >> Path;

	Path = "C:\\logs\\";
}

int main()
{
	setlocale(LC_ALL, "rus");

	InputPath		();
	ReadLogs		();
	RemoveUnused	();
	ProcessPlayers	();
	ShowResults		();

	system("pause");
	return 0;
}
