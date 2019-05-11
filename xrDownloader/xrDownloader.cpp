#include "stdafx.h"
#include "xrDownloader.h"

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

int xferinfo(void* userdata, curl_off_t dltotal, curl_off_t dlnow, curl_off_t, curl_off_t) 
{
	auto downloader = static_cast<DownloadFile*>(userdata);
	downloader->SetProgress((double)dlnow, (double)dltotal);

	return 0;
}

DownloadFile::DownloadFile(std::string From, std::string To) : Url(From), Path(To) { Downloaded = 0; Total = 1000; };

void DownloadFile::StartDownload()
{
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
}

void DownloadFile::SetProgress(double progr, double from)
{
	Total = from;
	Downloaded = progr;
}

int DownloadFile::GetProgress() { return (int)(Downloaded/Total*(double)100); };
