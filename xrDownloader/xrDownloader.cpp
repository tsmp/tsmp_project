#include "stdafx.h"
#include "xrDownloader.h"
#include <fstream>

std::string WithoutSlashAndText(std::string str)
{
	int lastId = 0;

	for (int i = 0; i < (int)str.size(); i++)
		if (str[i] == '\\')
			lastId = i;

	std::string str2 = str;
	str2.resize(lastId);

	return str2;
}

std::string CorrectFilename(std::string ssss)
{
	std::string DPath = ssss;

	for (int i = 0; ; i++)
	{
		if (DPath[i] == '/') DPath[i] = '\\';
		if (DPath[i] == '\\')
		{
			DPath += " ";

			for (int j = DPath.size() - 1; j > i; j--)
			{
				DPath[j] = DPath[j - 1];
			}

			DPath[i + 1] = '\\';
			i++;
		}

		if (i == (DPath.size() - 1)) break;

	}
	Msg("was %s become %s", ssss.c_str(), DPath.c_str());

	return DPath;
}

std::string GetProgrammPath()
{
	char ch[_MAX_PATH + 1]{ 0 };
	GetModuleFileName(NULL, ch, _MAX_PATH);
	std::string str = ch;

	return str;
}

bool FileExists(std::string Fle)
{
	return GetFileAttributes(CorrectFilename(Fle).c_str()) != DWORD(-1);
}

void FileDelete(std::string file)
{
	remove(CorrectFilename(file).c_str());
}

int xferinfo(void* userdata, curl_off_t dltotal, curl_off_t dlnow, curl_off_t, curl_off_t) 
{
	auto downloader = static_cast<DownloadFile*>(userdata);
	downloader->SetProgress((double)dlnow, (double)dltotal);

	return 0;
}

DownloadFile::DownloadFile(std::string From, std::string To) : Url(From), Path(To) { Downloaded = 0; Total = 1000; Active = false; };

void DownloadFile::StartDownload()
{
	Active = true;
	CURL *curl = nullptr;
	curl = curl_easy_init();

	if (!curl)
	{
		Msg("! Cant init curl");
		return;
	}

	FILE *fp = nullptr;
	CURLcode res;

	fp = fopen(CorrectFilename(Path).c_str(), "wb");

	curl_easy_setopt(curl, CURLOPT_URL,Url.c_str());
	curl_easy_setopt(curl, CURLOPT_WRITEFUNCTION, NULL);
	curl_easy_setopt(curl, CURLOPT_NOPROGRESS, 0);
	curl_easy_setopt(curl, CURLOPT_XFERINFOFUNCTION, xferinfo);
	curl_easy_setopt(curl, CURLOPT_XFERINFODATA, this);
	curl_easy_setopt(curl, CURLOPT_WRITEDATA, fp);

	Msg("Downloading file %s", Path.c_str());
	Msg("From %s", Url.c_str());

	res = curl_easy_perform(curl);
	fclose(fp);

	Msg("File %s downloaded", Path.c_str());
	curl_easy_cleanup(curl);

	Active = false;
}

void DownloadFile::SetProgress(double progr, double from)
{
	Total = from;
	Downloaded = progr;
}



int DownloadFile::GetProgress() { return (int)(Downloaded/Total*(double)100); };

void ReadFile(std::string Path, std::vector<std::string>& St)
{
	std::string s, s1;
	std::ifstream file(Path);

	while (true)
	{
		std::getline(file, s);
		if (s == s1) break;
		St.push_back(s);
		s1 = s;
	}

	file.close();
}

bool MapListParsed = false;

struct MapServer
{
	std::string Name;
	std::string File;
	std::string Url;
	u32 Crc;
	u32 Compression;

	void Clear() { Name.clear(); File.clear(); Url.clear(); Crc = 0; Compression = 0; }
};

std::vector<MapServer> maps;

void ParseMapListServer(std::vector<std::string>& all )
{
	MapServer map;

	for (int i = 0; i < all.size(); i++)
	{
		std::string str = all[i];

		if (strchr(str.c_str(), '[') && strchr(str.c_str(), ']'))
		{
			map.Clear();

			std::string mapname(str.begin() + 1, str.end() - 1);
			map.Name = mapname;
		}

		if (strstr(str.c_str(), "name="))
		{
			std::string mapfl(str.begin() + 5, str.end());
			map.File = mapfl;
		}

		if (strstr(str.c_str(), "url="))
		{
			std::string maplink(str.begin() + 4, str.end());
			map.Url = maplink;
		}

		if (strstr(str.c_str(), "crc="))
		{
			std::string crc_str(str.begin() + 4, str.end());
			map.Crc = stoul(crc_str);
		}

		if (strstr(str.c_str(), "compr="))
		{
			std::string crc_str(str.begin() + 6, str.end());
			map.Compression = stoul(crc_str);
			maps.push_back(map);
		}
	}
}

void FillMapParams(std::string& FileName, std::string& Url, unsigned& Compression, unsigned& CRC, std::string current_map)
{
	if (!MapListParsed)
	{
		string_path p;
		FS.update_path(p, "$app_data_root$", "");
		std::string Root = p;
		std::string List = Root + "\\tsmp_maplist.txt";

		if (FileExists(List))
		{
			std::vector<std::string> Vec;
			ReadFile(List, Vec);
			ParseMapListServer(Vec);
			MapListParsed = true;
			Msg("maplist loaded");
		}
		else
		{
			Msg("! no maplist"); 
			FileName = "military_kuznya_1.0.xdb.map";
			Url = "http://82.202.249.152/compressed_maps_shoc/military_kuznya.cab";
			Compression = 2;
			CRC = 936722695;
		}
	}
	
	std::string str = current_map;

	for (int i = 0; i < maps.size(); i++)
	{
		if (maps[i].Name == str)
		{
			FileName = maps[i].File;
			Url = maps[i].Url;
			Compression = maps[i].Compression;
			CRC= maps[i].Crc;
			Msg("found in map list");
		}
	}		
}

void ParseMaplist(std::vector<std::string>& all, std::vector<Map>& maps)
{
	Map map;

	for (int i = 0; i < all.size(); i++)
	{
		std::string str = all[i];

		if (strchr(str.c_str(), '[') && strchr(str.c_str(), ']'))
		{
			map.Clear();

			std::string mapname(str.begin() + 1, str.end() - 1);
			map.Name = mapname;
		}

		if (strstr(str.c_str(), "url="))
		{
			std::string maplink(str.begin() + 4, str.end());
			map.Url = maplink;
		}

		if (strstr(str.c_str(), "dependencies="))
		{
			std::string s2(str.begin() + 13, str.end());
			size_t idx = s2.find(',');

			if (idx == std::string::npos)
			{
				map.Dependencies.push_back(s2);
				
			}
			else
			{

				while (true)
				{
					std::string name(s2.begin(), s2.begin() + idx);
					std::string temp(s2.begin() + idx + 1, s2.end());
					s2 = temp;

					map.Dependencies.push_back(name);
					idx = s2.find(',');

					if (idx == std::string::npos)
					{
						map.Dependencies.push_back(s2);
						break;
					}
				}
			}

			maps.push_back(map);
		}
	}

	Msg("parsing maplist finished");
	Msg("%u lines, %u maps",all.size(), maps.size());
}

void ParseFilelist(std::vector<std::string> & Vec, std::vector<sFile>& FilesVec)
{
	sFile file;

	for (int i = 0; i < Vec.size(); i++)
	{
		std::string str = Vec[i];

		if (strchr(str.c_str(), '[') && strchr(str.c_str(), ']'))
		{
			file.Clear();

			std::string name(str.begin() + 1, str.end() - 1);
			file.Name = name;
		}

		if (strstr(str.c_str(), "url="))
		{
			std::string link(str.begin() + 4, str.end());
			file.Url = link;
		}

		if (strstr(str.c_str(), "ext="))
		{
			std::string ext(str.begin() + 4, str.end());
			file.Ext = ext;
		}

		if (strstr(str.c_str(), "crc="))
		{
			std::string crc_str(str.begin() + 4, str.end());
			file.CRC = stoul(crc_str);
		}

		if (strstr(str.c_str(), "loc="))
		{
			std::string loc(str.begin() + 4, str.end());
			file.Location = loc;
		}

		if (strstr(str.c_str(), "unp="))
		{
			if (strstr(str.c_str(), "=1"))
				file.Compressed = true;
			else
				file.Compressed = false;

			FilesVec.push_back(file);
		}
	}

	Msg("parsing finished");
	Msg("%u lines, %u files",Vec.size(), FilesVec.size());
}



void ScanFiles(std::vector<std::string> &Vec)
{	
	std::string ModDir = WithoutSlashAndText(WithoutSlashAndText(GetProgrammPath()));

	WIN32_FIND_DATA FindFileData1;
	HANDLE hf1;
	hf1 = FindFirstFile((ModDir+"\\BINS\\*").c_str(), &FindFileData1);

	if (hf1 != INVALID_HANDLE_VALUE) 
	{
		do 
		{
			std::string s = FindFileData1.cFileName;

			if (s[0] != '.')
				Vec.push_back(ModDir + "\\bins\\" + s);
		} while (FindNextFile(hf1, &FindFileData1) != 0);

		FindClose(hf1);
	}

	WIN32_FIND_DATA FindFileData;
	HANDLE hf;
	hf = FindFirstFile((ModDir + "\\RESOURCES\\*").c_str(), &FindFileData);

	if (hf != INVALID_HANDLE_VALUE)
	{
		do 
		{
			std::string s = FindFileData.cFileName;

			if(s[0]!='.')
				Vec.push_back(ModDir+"\\resources\\"+s);
		} while (FindNextFile(hf, &FindFileData) != 0);

		FindClose(hf);
	}	
	
	Msg("FilesScan found %u files", Vec.size());
}

void WhoNeedUpdate(std::vector<std::string> &Filess, std::vector<std::string>& Urlss, std::vector<bool> &Compr)
{
	Msg("WhoNeedUpdate?");

	std::string URL = "http://dark-stalker.clan.su/tsmp/filelist.txt";
	string_path p;
	FS.update_path(p, "$fs_root$", "");
	std::string Root = p;

	std::string List = Root + "\\appdata\\FileList.txt";

	if (FileExists(List))
		FileDelete(List);

	Msg("downloading filelist to %s", List.c_str());

	DownloadFile loader(URL, List);
	loader.StartDownload();

	Msg("reading filelist");

	std::vector<std::string> Lines;
	std::vector<sFile> Fileslist;
	ReadFile(List, Lines);

	Msg("parsing filelist");
	ParseFilelist(Lines, Fileslist);

	Msg("checking resources");

	std::string ModDir = WithoutSlashAndText(WithoutSlashAndText(GetProgrammPath()));
		
	std::vector<std::string> Files2;
	ScanFiles(Files2);

	Msg("i to %u ; j to %u", Files2.size(), Fileslist.size());

	for (int i = 0; i < Files2.size(); i++)
	{
		for (int j = 0; j < Fileslist.size(); j++)
		{
			std::string str, full, str2;
			str = "\\" + Fileslist[j].Location + "\\" + Fileslist[j].Name + '.' + Fileslist[j].Ext;
			full = ModDir + str;
			str2 = Files2[i];

			//Msg("comparing %s %s", full.c_str(), str2.c_str());

			u32 size1, size2;
			std::transform(str2.begin(), str2.end(), str2.begin(), ::tolower);
			std::transform(full.begin(), full.end(), full.begin(), ::tolower);
			size1 = full.size();
			size2 = str2.size();

			//Msg("%u %u", size1, size2);

			if ((size1 == size2) && (!str2.compare(full)))
			{
				std::ifstream ifs(full);
				ifs.seekg(0, std::ios::end);
				std::ifstream::pos_type pos = ifs.tellg();

				std::vector<char> result(pos);

				ifs.seekg(0, std::ios::beg);
				ifs.read(&result[0], pos);

				u32 m_crc32 = 0;

				m_crc32 = crc32(result.data(), result.size());

				if (m_crc32 != Fileslist[j].CRC)
				{
					Msg("file %s has not normal crc %u", full.c_str(), m_crc32);
					
					Filess.push_back(full);
					Urlss.push_back(Fileslist[j].Url);
					Compr.push_back(Fileslist[j].Compressed);
				}
				else
					Msg("file %s has normal crc %u", full.c_str(), m_crc32);
			}
		}
	}
}


bool NeedToUpdate(std::vector<sFile>& FilesList)
{
	std::string ModDir = WithoutSlashAndText(WithoutSlashAndText(GetProgrammPath()));
	std::vector<std::string> Files;
	ScanFiles(Files);

	Msg("i to %u ; j to %u",Files.size(), FilesList.size());

	for (int i=0; i <Files.size();i++)
	{
		for (int j = 0; j < FilesList.size(); j++)
		{
			std::string str,full,str2;
			str = "\\" + FilesList[j].Location + "\\" + FilesList[j].Name +'.' +FilesList[j].Ext;
			full = ModDir + str;
			str2 = Files[i];

		//	Msg("comparing %s %s",full.c_str(), str2.c_str());

			u32 size1, size2;
			std::transform(str2.begin(), str2.end(), str2.begin(), ::tolower);
			std::transform(full.begin(), full.end(), full.begin(), ::tolower);
			size1 = full.size();
			size2 = str2.size();

			//Msg("%u %u",size1,size2);

			if ((size1==size2) && (!str2.compare(full)))
			{
				std::ifstream ifs(full);
				ifs.seekg(0, std::ios::end);
				std::ifstream::pos_type pos = ifs.tellg();

				std::vector<char> result(pos);

				ifs.seekg(0, std::ios::beg);
				ifs.read(&result[0], pos);

				u32 m_crc32 = 0;

				m_crc32 = crc32(result.data(), result.size());

				if (m_crc32 != FilesList[j].CRC)
				{
					Msg("file %s has not normal crc %u", full.c_str(), m_crc32);
					return true;
				}
				else
					Msg("file %s has normal crc %u",full.c_str(), m_crc32);
			}
		}
	}

	return false;
}

void CreateFolder(std::string Path)
{
	CreateDirectory(CorrectFilename(Path).c_str(), NULL);
}

bool XRDOWNLOADER_API CheckForUpdates()
{
	std::string URL = "http://dark-stalker.clan.su/tsmp/filelist.txt";
	string_path p;
	FS.update_path(p, "$fs_root$", "");
	std::string Root = p;

	std::string List = Root + "\\appdata\\FileList.txt";

	if (FileExists(List))
		FileDelete(List);

	Msg("downloading filelist to %s",List.c_str());

	DownloadFile loader(URL, List);
	loader.StartDownload();

	Msg("reading filelist");

	std::vector<std::string> Lines;
	std::vector<sFile> Files;
	ReadFile(List, Lines);

	Msg("parsing filelist");
	ParseFilelist(Lines, Files);

	Msg("checking resources");

	return NeedToUpdate(Files);
}

void RunProgramm(std::string Path, std::string Arg, bool hidden = false)
{
	STARTUPINFO si;
	PROCESS_INFORMATION pi;

	ZeroMemory(&si, sizeof(si));
	si.cb = sizeof(si);
	ZeroMemory(&pi, sizeof(pi));


	if (hidden)
	{
		si.dwFlags = STARTF_USESHOWWINDOW;
		si.wShowWindow = SW_HIDE;
	}

	CreateProcess
	(CorrectFilename(Path).c_str(),   // No module name (use command line)
		const_cast<LPSTR>(Arg.c_str()),        // Command line
		NULL,           // Process handle not inheritable
		NULL,           // Thread handle not inheritable
		FALSE,          // Set handle inheritance to FALSE
		0,              // No creation flags
		NULL,           // Use parent's environment block
		NULL,           // Use parent's starting directory 
		&si,            // Pointer to STARTUPINFO structure
		&pi);           // Pointer to PROCESS_INFORMATION structure


	// Wait until child process exits.
	//WaitForSingleObject(pi.hProcess, INFINITE);

	// Close process and thread handles. 
	//CloseHandle(pi.hProcess);
	//CloseHandle(pi.hThread);
}

std::string GetFileNameFromPath(std::string Path)
{
	std::string tmp = WithoutSlashAndText(Path);
	std::string result(Path.begin() + tmp.size() + 1, Path.end());
	return result;
}

void DownloadFiles::StartDownload()
{
	all_done = false;

	std::string ModDir = WithoutSlashAndText(WithoutSlashAndText(GetProgrammPath()));
	std::string Downloads = ModDir + "\\downloads\\";

	CreateFolder(Downloads);

	std::ofstream Out;
	Out.open(Downloads + "\\tasklist.txt");

	totalfiles = Urls.size();

	for (int i = 0; i < totalfiles; i++)
	{
		idx = i;
		std::string DownloadHere = Downloads + GetFileNameFromPath(Paths[i]);

		

		if (Compressions[i])
		{
			DownloadHere += ".7z";
			Out << "decompress copy" << std::endl;
		}
		else
		{
			Out << "copy" << std::endl;
		}
		
		Out << DownloadHere<< std::endl;
		Out << Paths[i]<< std::endl;
		
		Msg("loading to %s",DownloadHere.c_str());

		
		DownloadFile *dl = new DownloadFile(Urls[i], DownloadHere);
		DFile = (void*)dl;
		isinprogress = true;
		dl->StartDownload();
		isinprogress = false;
		dl->~DownloadFile();

		Msg("loaded");
	}
	all_done = true;
	
	Out.close();
}

int DownloadFiles::GetProgress()
{
	/*
	
	2 

	0.5 

	*/

	float f = ((float)idx / (float)totalfiles)*(float)100;

	if (isinprogress && DFile)
	{
		DownloadFile *ldr = (DownloadFile*)DFile;
		f+=  ((float)ldr->GetProgress())/(float)totalfiles;
	}

	if (all_done)
		return 100;
	else
		return f;
}

void AddMapDependencies(std::vector<std::string>& Filess, std::vector<std::string>& Urlss, std::vector<bool> &Compr, std::string mapname)
{
	std::string ModDir = WithoutSlashAndText(WithoutSlashAndText(GetProgrammPath()));


	std::string URL = "http://dark-stalker.clan.su/tsmp/filelist.txt";
	string_path p;
	FS.update_path(p, "$fs_root$", "");
	std::string Root = p;

	std::string List = Root + "\\appdata\\FileList.txt";

	if (FileExists(List))
		FileDelete(List);

	Msg("downloading filelist to %s", List.c_str());

	DownloadFile loader(URL, List);
	loader.StartDownload();

	Msg("reading filelist");

	std::vector<std::string> Lines;
	std::vector<sFile> Files;
	ReadFile(List, Lines);

	Msg("parsing filelist");
	ParseFilelist(Lines, Files);



	std::string URL2 = "http://dark-stalker.clan.su/tsmp/mapslist.txt";
	string_path p2;
	FS.update_path(p2, "$fs_root$", "");
	std::string Root2 = p2;

	std::string List2 = Root2 + "\\appdata\\maplist.txt";

	if (FileExists(List2))
		FileDelete(List2);

	Msg("downloading maplist to %s", List.c_str());

	DownloadFile loader2(URL2, List2);
	loader2.StartDownload();

	Msg("reading maplist");

	std::vector<std::string> Lines2;
	std::vector<Map> Maps;
	ReadFile(List2, Lines2);

	Msg("parsing maplist");
	ParseMaplist(Lines2, Maps);

	int idx = -1;

	for (int i = 0; i < Maps.size(); i++)
	{
		Msg("%s %s",Maps[i].Name.c_str(),mapname.c_str());

		if (mapname == Maps[i].Name)
		{
			idx = i;
			Msg("equal");
		}
		else
			Msg("not equal");
	}

	if (idx == -1)
		return;

	for (int i = 0; i < Maps[idx].Dependencies.size(); i++)
	{
		Msg("depend from %s", Maps[idx].Dependencies[i].c_str());

		for (int j=0; j < Files.size(); j++)
		{
			std::string path = ModDir + "\\" + Files[j].Location + "\\" + Files[j].Name + '.' + Files[j].Ext;

			if (Files[j].Name == Maps[idx].Dependencies[i] && !FileExists(path))
			{
				Urlss.push_back(Files[j].Url);
				Filess.push_back(path);
				Compr.push_back(Files[j].Compressed);
			}
		}
	}
}

void FillDownloadList(DownloadFiles* ldr, std::string MapName)
{
	std::vector<std::string> FilesToUpdate;
	std::vector<std::string> UrlsToFiles;
	std::vector<bool> Compr;

	WhoNeedUpdate(FilesToUpdate, UrlsToFiles,Compr);

	if (MapName != "none")
	{
		AddMapDependencies(FilesToUpdate, UrlsToFiles,Compr, MapName);
	}

	Msg("");
	Msg("FilesToProcess:");

	for (int i = 0; i < FilesToUpdate.size(); i++)
	{
		Msg("%s",FilesToUpdate[i].c_str() );
		Msg("%s", UrlsToFiles[i].c_str());

		ldr->AddElement(UrlsToFiles[i], FilesToUpdate[i],Compr[i]);
	}
}

void XRDOWNLOADER_API RunUpdater(std::string args)
{
	std::string ModDir = WithoutSlashAndText(WithoutSlashAndText(GetProgrammPath()));
	std::string path = ModDir + "\\downloads\\updater.exe";

	DownloadFile f("http://dark-stalker.clan.su/tsmp/updater.ex", path);
	f.StartDownload();


	std::ofstream Out;
	Out.open(ModDir + "\\downloads\\arglist.txt");
	Out << args << std::endl;

	RunProgramm(path, args);
}