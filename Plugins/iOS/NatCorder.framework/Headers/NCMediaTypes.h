//
//  NCMediaTypes.h
//  NatCorder
//
//  Created by Yusuf Olokoba on 10/25/2022.
//  Copyright © 2022 NatML Inc. All Rights Reserved.
//

#pragma once

#ifdef __cplusplus
    #define NML_BRIDGE extern "C"
#else
    #define NML_BRIDGE extern
#endif

#ifdef _WIN64
    #define NML_EXPORT __declspec(dllexport)
#else
    #define NML_EXPORT
#endif

#ifdef __EMSCRIPTEN__
    #define NML_API EMSCRIPTEN_KEEPALIVE
#elif defined(_WIN64)
    #define NML_API APIENTRY
#else
    #define NML_API
#endif