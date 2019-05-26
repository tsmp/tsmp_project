#pragma once
#include "..\xrCore\xrCore.h"

#ifdef XRDOWNLOADER_EXPORTS
#define XRDOWNLOADER_API __declspec(dllexport)
#define CURL_STATICLIB
#include "..\components\libcurl\include\curl\curl.h"
#else
#define XRDOWNLOADER_API __declspec(dllimport)
#pragma comment(lib,"xrDownloader.lib")
#endif

struct sFile
{
	std::string Name;
	std::string Url;
	std::string Ext;
	unsigned CRC;
	std::string Location;
	bool Compressed;

	void Clear() { Name.clear(); Url.clear(); Ext.clear(); CRC = 0; Location.clear(); Compressed = 0; }
};

struct Map
{
	std::string Name;
	std::string Url;
	std::vector<std::string> Dependencies;

	void Clear() { Name.clear(); Url.clear(), Dependencies.clear(); }
};

class XRDOWNLOADER_API DownloadFiles
{
private:
	void* DFile;
	int idx;
	int totalfiles;
	std::vector<std::string> Urls;
	std::vector<std::string> Paths;
	std::vector<bool> Compressions;
public:
	void AddElement(std::string url, std::string path, bool compressed) { Urls.push_back(url); Paths.push_back(path); Compressions.push_back(compressed); }
	void StartDownload();
	int GetProgress();
};

void XRDOWNLOADER_API FillDownloadList(DownloadFiles* ldr,std::string mapname);
void XRDOWNLOADER_API RunUpdater(std::string args);

class XRDOWNLOADER_API DownloadFile
{
private:
	std::string Url,Path;
	double Downloaded, Total;
	bool Active;
public:
	DownloadFile(std::string From, std::string To);

	void StartDownload();
	void SetProgress(double progr, double from);
	int GetProgress();
	bool IsActive() { return Active; }
};

bool XRDOWNLOADER_API CheckForUpdates();
