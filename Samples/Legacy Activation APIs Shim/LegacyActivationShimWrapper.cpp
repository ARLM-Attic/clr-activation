// LegacyActivationShimWrapper.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "cor.h"
#include "mscoree.h"
#include "strongname.h"
//#define LEGACY_ACTIVATION_SHIM_ALLOW_LEGACY_API_FALLBACK
#include "LegacyActivationShimDelayLoad.h"

int _tmain(int argc, _TCHAR* argv[])
{
	ICLRRuntimeHost *pRH = NULL;
	HRESULT hr = LegacyActivationShim::CorBindToRuntime(NULL, NULL, CLSID_CLRRuntimeHost, IID_ICLRRuntimeHost, reinterpret_cast<LPVOID*>(&pRH));
	hr = pRH->Start();
	LegacyActivationShim::CorExitProcess(0);
}

