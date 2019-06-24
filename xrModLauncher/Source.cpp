#include <Windows.h>
#include <tchar.h>
#include <string>
#include <iostream>
#include <fstream>
#include <experimental/filesystem>

using namespace std;

ofstream logger;
bool opened = false;

void Msg(std::string mes)
{
	if (!opened)
	{
		logger.open("log.txt");
		opened = true;
	}

	logger << mes << endl;
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

	return DPath;
}

void RunProgramm(std::string Path, std::string Arg, bool hidden=false)
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
	WaitForSingleObject(pi.hProcess, INFINITE);

	// Close process and thread handles. 
	CloseHandle(pi.hProcess);
	CloseHandle(pi.hThread);
}

std::string GetProgrammPath()
{
	char ch[_MAX_PATH + 1]{ 0 };
	GetModuleFileName(NULL, ch, _MAX_PATH);	
	std::string str = ch;

	return str;
}

bool FileExist(std::string File)
{
	bool b=GetFileAttributes(CorrectFilename(File).c_str()) != DWORD(-1);

	if (b)
		Msg("File exists: " + File);
	else
		Msg("File not exists: " + File);
	
	return b;
}

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

void FileCopy(std::string From, std::string To)
{
	Msg("Copying "+From+" to "+To);
	CopyFile(CorrectFilename(From).c_str(), CorrectFilename(To).c_str(), true);
}

void CreateFolder(std::string Path)
{
	Msg("Creating folder: "+Path);
	CreateDirectory(CorrectFilename(Path).c_str(), NULL);
}

int APIENTRY WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, char* lpCmdLine, int nCmdShow)
{
	Msg("Starting launcher");
	Msg("Init data");

	std::string CMD = lpCmdLine;
	std::string CurDir = WithoutSlashAndText(GetProgrammPath()); // appdata\svn\tsmp\bin
	std::string PatchDir = WithoutSlashAndText(CurDir); // appdata\svn\tsmp
	
	Msg("Command line: "+CMD);
	Msg("Current directory: " + CurDir);
	Msg("Mod directory: "+PatchDir);

	bool exist = FileExist(CurDir + "\\7z.exe")
		&& FileExist(CurDir + "\\7z.dll")
		&& FileExist(CurDir + "\\fsgame.ltx")
		&& FileExist(CurDir + "\\res_base_bins.7z")
		&& FileExist(CurDir + "\\res_base_snd.7z")
		&& FileExist(CurDir + "\\res_base_stuff.7z")
		&& FileExist(CurDir + "\\res_base_tex.7z");


	if (!exist)
	{
		Msg("Some resources not exist, turning off");
		return 0;
	}

	std::string xr_3da = PatchDir + "\\userdata\\bins\\xr_3da.exe";
	std::string marker = PatchDir + "\\userdata\\bins\\123.txt";

	if (!FileExist(marker))
	{
		std::string s1= PatchDir + "\\userdata\\bins";
		std::string s2 = PatchDir + "\\userdata\\resources";

		
		std::experimental::filesystem::remove_all(s1);
		std::experimental::filesystem::remove_all(s2);

	}
	

	if (!FileExist(xr_3da))
	{
		Msg("Xr_3da not exists, working");

		CreateFolder(PatchDir + "\\userdata");
		CreateFolder(PatchDir + "\\userdata\\appdata");
		CreateFolder(PatchDir + "\\userdata\\bins");
		CreateFolder(PatchDir + "\\userdata\\resources");

		std::string bins, tex, snd, stuff, res,bin;
		bin= PatchDir + "\\userdata\\bins";
		res = PatchDir + "\\userdata\\resources";
		bins = bin+"\\res_base_bins.7z";
		tex =  res+"\\res_base_tex.7z";
		snd = res+"\\res_base_snd.7z";
		stuff = res+"\\res_base_stuff.7z";

		FileCopy(CurDir + "\\7z.exe", bin+"\\7z.exe");
		FileCopy(CurDir + "\\7z.dll", bin + "\\7z.dll");

		FileCopy(CurDir+"\\fsgame.ltx",PatchDir+"\\userdata\\fsgame.ltx");
		FileCopy(CurDir + "\\fsgame.ltx", PatchDir + "\\userdata\\fsgame_.ltx");
		FileCopy(CurDir + "\\res_base_bins.7z",bins);
		FileCopy(CurDir + "\\res_base_snd.7z",snd );
		FileCopy(CurDir + "\\res_base_stuff.7z", stuff);
		FileCopy(CurDir + "\\res_base_tex.7z", tex);


		if(FileExist(PatchDir + "\\userdata\\user.ltx"))
			FileCopy(PatchDir + "\\userdata\\user.ltx", PatchDir + "\\userdata\\appdata\\user.ltx");
	
		Msg("Running unpacker");

		SetCurrentDirectory(CorrectFilename(bin).c_str());

		RunProgramm(bin+ "\\7z.exe", "7z e \""+bins+"\" -aoa", true);

		SetCurrentDirectory(CorrectFilename(res).c_str());
		RunProgramm(bin + "\\7z.exe", "7z e \"" + stuff + "\" -aoa", true);
		RunProgramm(bin + "\\7z.exe", "7z e \"" + tex + "\" -aoa", true);
		RunProgramm(bin + "\\7z.exe", "7z e \"" + snd + "\" -aoa", true);
	}
		
	ofstream of;
	of.open(marker);
	of << "321";
	of.close();

	Msg("Launching xr_3da");
	logger.close();
	RunProgramm(xr_3da, CMD);	
	
	return 0;
}