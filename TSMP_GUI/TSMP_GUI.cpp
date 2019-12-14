#include "TSMP_GUI.h"
#include "resource.h"
#include <Windows.h>
#include <string>
#include "..\xrCore\xrCore.h"
#include "..\xrCore\log.h"

#pragma comment(lib,"xrCore.lib")

static HWND hWindow = 0;
static HWND hText = 0;
static HWND hButton = 0;
static HWND hLog = 0;

typedef void (*void_f)(LPCSTR);

void* ConsoleExecPointer;

void ExecConsole(LPCSTR Command)
{
	((void_f)ConsoleExecPointer)(Command);
}

void OnMyButtonClick()
{
	int potential_length = GetWindowTextLength(hText);
	std::string text;
	text.resize(potential_length + 1);

	int final_length = GetWindowText(hText, &text[0], potential_length + 1);
	text.resize(final_length);

	ExecConsole(text.c_str());
}

void ScaleInterface(long Left, long Top, long Right, long Bottom)
{
	static const long ButtonHeight = 50;
	static const long ButtonWidth = 70;
	static const long BottomOut = 50;
	static const long RightOut = 30;

	static const long ElementInterval = 20;
	
	long ButX = Right - RightOut - ButtonWidth;
	long ButY = Bottom - BottomOut - ButtonHeight;
	long ButXX = ButtonWidth;
	long ButYY = ButtonHeight;

	long TexX = ElementInterval;
	long TexY = ButY;
	long TexXX = ButX-ElementInterval-TexX;
	long TexYY = Bottom-BottomOut-TexY;

	long LogX = ElementInterval;
	long LogY = ElementInterval;
	long LogXX = Right - RightOut - LogX;
	long LogYY = ButY-ElementInterval - LogY;
	
	SetWindowPos(hButton, HWND_TOP, ButX, ButY, ButXX, ButYY, 0);
	SetWindowPos(hText, HWND_TOP, TexX, TexY, TexXX, TexYY, 0);
	SetWindowPos(hLog, HWND_TOP, LogX, LogY, LogXX, LogYY, 0);

	Msg("Upd");
}

static INT_PTR CALLBACK WndProc(HWND hw, UINT msg, WPARAM wp, LPARAM lp)
{
	switch (msg)
	{
	case WM_DESTROY:
		break;

	case WM_CLOSE:
	{
		if (MessageBox(0, "Завершить работу сервера?", "Выход", MB_OKCANCEL) == 1)
			ExitProcess(0);
	}
	break;
	case WM_SIZE:
		//if (!(LOWORD(wp) == SIZE_MAXHIDE || LOWORD(wp) == SIZE_MINIMIZED))
		{
			RECT rct;
			GetWindowRect(hWindow, &rct);
			ScaleInterface(rct.left, rct.top, rct.right, rct.bottom);
		}			
		break;
	case WM_COMMAND:
		if (LOWORD(wp) == IDCANCEL)		
			ExitProcess(0);
		
		if (LOWORD(wp) == IDC_BUTTON1)
			OnMyButtonClick();
		
		break;
	default:
		return FALSE;
	}
	return TRUE;
}

u32 LogSize;

void AddLogLine(const char* Line)
{
	DWORD StartPos, EndPos;
	SendMessage(hLog, EM_GETSEL, reinterpret_cast<WPARAM>(&StartPos), reinterpret_cast<WPARAM>(&EndPos));

	int outLength = GetWindowTextLength(hLog);
	SendMessage(hLog, EM_SETSEL, outLength, outLength);

	SendMessage(hLog, EM_REPLACESEL, TRUE, reinterpret_cast<LPARAM>(Line));
	SendMessage(hLog, EM_REPLACESEL, TRUE, reinterpret_cast<LPARAM>("\r\n"));
	SendMessage(hLog, EM_SETSEL, StartPos, EndPos);
}

static void _process_messages(void)
{
	MSG msg;

	if (PeekMessage(&msg, NULL, 0, 0, PM_REMOVE))
	{
		TranslateMessage(&msg);

		if (msg.message == WM_KEYDOWN && msg.wParam == VK_RETURN)
			OnMyButtonClick();

		DispatchMessage(&msg);
	}

	if (LogSize != LogFile->size())
	{
		for (; LogSize < LogFile->size(); LogSize++)
		{
			const char* S = *(*LogFile)[LogSize];

			if (0 == S)	
				S = "";	
			
			AddLogLine(S);
		}
	}

	Sleep(10);
}

HMODULE GetCurrentModuleHandle() 
{
	HMODULE hModule = nullptr;

	GetModuleHandleEx(GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS
		, (LPCTSTR)GetCurrentModuleHandle,
		&hModule);

	return hModule;
}

void InitWindow()
{
	hWindow = CreateDialog(GetCurrentModuleHandle(), MAKEINTRESOURCE(IDD_TSMPGUI), 0, WndProc);

	if (!hWindow)
	{
		MessageBox(0, "Cant create server window", "Error!", MB_OK | MB_ICONERROR);
		return;
	}

	hText = GetDlgItem(hWindow, IDC_EDIT1);
	hLog = GetDlgItem(hWindow, IDC_LOG);
	hButton = GetDlgItem(hWindow, IDC_BUTTON1);

	SendMessage(hLog, EM_SETLIMITTEXT, -1, 0);

	HANDLE hicon = LoadImage(GetCurrentModuleHandle(), MAKEINTRESOURCE(IDI_ICON1), IMAGE_ICON, 0, 0, LR_DEFAULTCOLOR | LR_DEFAULTSIZE);
	SendMessageW(hWindow, WM_SETICON, ICON_BIG, (LPARAM)hicon);

	SetWindowPos(hWindow, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);

	RECT rct;
	GetWindowRect(hWindow, &rct);
	ScaleInterface(rct.left, rct.top, rct.right, rct.bottom);

	while (true)
		_process_messages();
}

void Create_Window(void *v)
{
	LogSize = 0;
	ConsoleExecPointer = v;
	InitWindow();
}
