

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 8.01.0628 */
/* at Tue Jan 19 12:14:07 2038
 */
/* Compiler settings for DroppedFolderDetectHookShellExtension.idl:
    Oicf, W1, Zp8, env=Win64 (32b run), target_arch=AMD64 8.01.0628 
    protocol : all , ms_ext, c_ext, robust
    error checks: allocation ref bounds_check enum stub_data 
    VC __declspec() decoration level: 
         __declspec(uuid()), __declspec(selectany), __declspec(novtable)
         DECLSPEC_UUID(), MIDL_INTERFACE()
*/
/* @@MIDL_FILE_HEADING(  ) */



/* verify that the <rpcndr.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCNDR_H_VERSION__
#define __REQUIRED_RPCNDR_H_VERSION__ 500
#endif

#include "rpc.h"
#include "rpcndr.h"

#ifndef __RPCNDR_H_VERSION__
#error this stub requires an updated version of <rpcndr.h>
#endif /* __RPCNDR_H_VERSION__ */


#ifndef __DroppedFolderDetectHookShellExtension_i_h__
#define __DroppedFolderDetectHookShellExtension_i_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

#ifndef DECLSPEC_XFGVIRT
#if defined(_CONTROL_FLOW_GUARD_XFG)
#define DECLSPEC_XFGVIRT(base, func) __declspec(xfg_virtual(base, func))
#else
#define DECLSPEC_XFGVIRT(base, func)
#endif
#endif

/* Forward Declarations */ 

#ifndef __DroppedFolderDetectHook_FWD_DEFINED__
#define __DroppedFolderDetectHook_FWD_DEFINED__

#ifdef __cplusplus
typedef class DroppedFolderDetectHook DroppedFolderDetectHook;
#else
typedef struct DroppedFolderDetectHook DroppedFolderDetectHook;
#endif /* __cplusplus */

#endif 	/* __DroppedFolderDetectHook_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"
#include "shobjidl.h"

#ifdef __cplusplus
extern "C"{
#endif 



#ifndef __DroppedFolderDetectHookShellExtensionLib_LIBRARY_DEFINED__
#define __DroppedFolderDetectHookShellExtensionLib_LIBRARY_DEFINED__

/* library DroppedFolderDetectHookShellExtensionLib */
/* [version][uuid] */ 


EXTERN_C const IID LIBID_DroppedFolderDetectHookShellExtensionLib;

EXTERN_C const CLSID CLSID_DroppedFolderDetectHook;

#ifdef __cplusplus

class DECLSPEC_UUID("748c00ab-b809-4dd0-a2c4-9b954e9ee6d2")
DroppedFolderDetectHook;
#endif
#endif /* __DroppedFolderDetectHookShellExtensionLib_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


