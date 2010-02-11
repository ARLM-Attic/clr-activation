LegacyActivationShim.h provides the main functionality of this project. 

 This file allows simple migration from .NET Runtime v2 Host Activation APIs
 to the .NET Runtime v4 Host Activation APIs through simple shim functions.
 To use, just include this header file after the header file that declares the
 deprecated APIs you are using, and append the "LegacyActivationShim::" namespace
 in front of all deprecated API calls.

 For example,
 		#include "mscoree.h"
      ...
      CorBindToRuntimeEx(
			NULL, NULL, 0, CLSID_CLRRuntimeHost, IID_ICLRRuntimeHost, (LPVOID *)&pRH));
 becomes
 		#include "mscoree.h"
      #include "LegacyActivationShim.h"
      ...
      LegacyActivationShim::CorBindToRuntimeEx(
			NULL, NULL, 0, CLSID_CLRRuntimeHost, IID_ICLRRuntimeHost, (LPVOID *)&pRH));

 To enable fallback to the legacy APIs when v4.0 is not installed on the machine,
 define LEGACY_ACTIVATION_SHIM_ALLOW_LEGACY_API_FALLBACK before including this
 header file.

 To use the legacy API fallback in a delay-loaded fashion, include LegacyActivationShimDelayLoad.h
 instead.



To see an example use of this please see the LegacyActivationShimWrapper.cpp file included under the source files folder. 