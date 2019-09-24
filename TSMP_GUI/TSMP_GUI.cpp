#include "TSMP_GUI.h"
#include "resource.h"
#include <Windows.h>
#include <string>

static HWND hWindow = 0;
static HWND hText = 0;

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

static INT_PTR CALLBACK WndProc(HWND hw, UINT msg, WPARAM wp, LPARAM lp)
{
	switch (msg)
	{
	case WM_DESTROY:
		break;
	case WM_CLOSE:
		ExitProcess(0);
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

static void _process_messages(void)
{
	MSG msg;

	if (PeekMessage(&msg, NULL, 0, 0, PM_REMOVE))
	{
		TranslateMessage(&msg);
		DispatchMessage(&msg);
	}
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

	SetWindowPos(hWindow, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);

	while (true)
		_process_messages();
}

void Create_Window(void *v)
{
	ConsoleExecPointer = v;
	InitWindow();
}
