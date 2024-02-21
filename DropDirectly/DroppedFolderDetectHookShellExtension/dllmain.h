// dllmain.h : モジュール クラスの宣言です。

class CDroppedFolderDetectHookShellExtensionModule : public ATL::CAtlDllModuleT< CDroppedFolderDetectHookShellExtensionModule >
{
public :
	DECLARE_LIBID(LIBID_DroppedFolderDetectHookShellExtensionLib)
	DECLARE_REGISTRY_APPID_RESOURCEID(IDR_DROPPEDFOLDERDETECTHOOKSHELLEXTENSION, "{300d1fde-9a07-468c-bbc5-37cd2466adc0}")
};

extern class CDroppedFolderDetectHookShellExtensionModule _AtlModule;
