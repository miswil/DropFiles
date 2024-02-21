// DroppedFolderDetectHook.h : CDroppedFolderDetectHook の宣言

#pragma once
#include "resource.h"       // メイン シンボル



#include "DroppedFolderDetectHookShellExtension_i.h"



#if defined(_WIN32_WCE) && !defined(_CE_DCOM) && !defined(_CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA)
#error "DCOM の完全サポートを含んでいない Windows Mobile プラットフォームのような Windows CE プラットフォームでは、単一スレッド COM オブジェクトは正しくサポートされていません。ATL が単一スレッド COM オブジェクトの作成をサポートすること、およびその単一スレッド COM オブジェクトの実装の使用を許可することを強制するには、_CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA を定義してください。ご使用の rgs ファイルのスレッド モデルは 'Free' に設定されており、DCOM Windows CE 以外のプラットフォームでサポートされる唯一のスレッド モデルと設定されていました。"
#endif

using namespace ATL;


// CDroppedFolderDetectHook

class ATL_NO_VTABLE CDroppedFolderDetectHook :
	public CComObjectRootEx<CComSingleThreadModel>,
	public CComCoClass<CDroppedFolderDetectHook, &CLSID_DroppedFolderDetectHook>,
	public ICopyHook
{
public:
	CDroppedFolderDetectHook()
	{
	}

DECLARE_REGISTRY_RESOURCEID(106)

DECLARE_NOT_AGGREGATABLE(CDroppedFolderDetectHook)

BEGIN_COM_MAP(CDroppedFolderDetectHook)
	COM_INTERFACE_ENTRY_IID(IID_IShellCopyHook, ICopyHook)
END_COM_MAP()



	DECLARE_PROTECT_FINAL_CONSTRUCT()

	HRESULT FinalConstruct()
	{
		return S_OK;
	}

	void FinalRelease()
	{
	}

public:

	UINT CopyCallback(
		HWND hwnd, UINT wFunc, UINT wFlags,
		PCWSTR pszSrcFile, DWORD dwSrcAttribs,
		PCWSTR pszDestFile, DWORD dwDestAttribs) override;
};

OBJECT_ENTRY_AUTO(__uuidof(DroppedFolderDetectHook), CDroppedFolderDetectHook)
