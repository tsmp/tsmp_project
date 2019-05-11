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

class XRDOWNLOADER_API DownloadFile
{
private:
	std::string Url,Path;
	double Downloaded, Total;
public:
	DownloadFile(std::string From, std::string To);

	void StartDownload();
	void SetProgress(double progr, double from);
	int GetProgress();
};
