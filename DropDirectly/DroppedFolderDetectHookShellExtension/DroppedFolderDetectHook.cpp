// DroppedFolderDetectHook.cpp : CDroppedFolderDetectHook の実装

#include "pch.h"
#include "DroppedFolderDetectHook.h"

UINT CDroppedFolderDetectHook::CopyCallback(
	HWND hwnd, UINT wFunc, UINT wFlags,
	PCWSTR pszSrcFile, DWORD dwSrcAttribs,
	PCWSTR pszDestFile, DWORD dwDestAttribs)
{
	TCHAR targetPath[MAX_PATH + 1];
	TCHAR longTargetPath[MAX_PATH + 1];
	TCHAR longSrcPath[MAX_PATH + 1];
	GetTempPath2(MAX_PATH + 1, targetPath);
	GetLongPathName(targetPath, longTargetPath, MAX_PATH + 1);
	GetLongPathName(pszSrcFile, longSrcPath, MAX_PATH + 1);
	StrNCat(longTargetPath, TEXT("DropSource"), MAX_PATH + 1);
	if (StrNCmp(longSrcPath, longTargetPath, _tcslen(longTargetPath)) != 0) {
		return IDYES;
	}

	LPTSTR lpszPipename = TEXT("\\\\.\\pipe\\com.example.dropdirectly");
	DWORD nwrite;
	HANDLE hPipe = CreateFile(
		lpszPipename,
		GENERIC_WRITE,
		0,
		NULL,
		OPEN_EXISTING,
		0,
		NULL);
	WriteFile(hPipe,
		pszDestFile,
		(_tcslen(pszDestFile) + 1) * sizeof(TCHAR),
		&nwrite, NULL);
	FlushFileBuffers(hPipe);
	CloseHandle(hPipe);
	return IDNO;
}

// CDroppedFolderDetectHook

