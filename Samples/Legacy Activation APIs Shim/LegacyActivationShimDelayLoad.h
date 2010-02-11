// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==
//
// LegacyActivationShimDelayLoad.h
//
// This file offers the same functionality as LegacyActivationShim.h
// in a delay-loaded fashion.

#ifndef __LEGACYACTIVATIONSHIMDELAYLOAD_H__
#define __LEGACYACTIVATIONSHIMDELAYLOAD_H__

#define LEGACY_ACTIVATION_SHIM_DELAY_LOAD
#include ".\LegacyActivationShim.h"
#undef LEGACY_ACTIVATION_SHIM_DELAY_LOAD

#endif //__LEGACYACTIVATIONSHIMDELAYLOAD_H__
