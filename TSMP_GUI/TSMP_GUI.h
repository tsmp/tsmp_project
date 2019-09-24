#pragma once

#ifdef TSMPGUI_EXPORTS
#define TSMPGUI_API __declspec(dllexport)
#else
#define TSMPGUI_API __declspec(dllimport)
#pragma comment(lib,"TSMP_GUI.lib")
#endif

TSMPGUI_API void Create_Window(void*);
