//
//  NCMediaSession.h
//  NatCorder
//
//  Created by Yusuf Olokoba on 10/25/2022.
//  Copyright © 2022 NatML Inc. All Rights Reserved.
//

#pragma once

#include "NCMediaTypes.h"

#pragma region --Enumerations--
/*!
 @enum NCMediaStatus

 @abstract NatCorder status codes.

 @constant NC_OK
 Successful operation.

 @constant NC_ERROR_INVALID_ARGUMENT
 Provided argument is invalid.

 @constant NC_ERROR_INVALID_OPERATION
 Operation is invalid in current state.

 @constant NC_ERROR_NOT_IMPLEMENTED
 Operation has not been implemented.

 @constant NC_ERROR_INVALID_SESSION
 NatCorder session has not been set or is invalid.

 @constant NC_ERROR_MISSING_NATML_HUB
 NatMLHub dependency library is missing.

 @constant NC_ERROR_INVALID_NATML_HUB
 NatMLHub dependency library is invalid.
 
 @constant NC_ERROR_INVALID_PLAN
 Current NatML plan does not allow the operation.

 @constant NC_WARNING_LIMITED_PLAN
 Current NatML plan only allows for limited functionality.
 */
enum NCMediaStatus {
    NC_OK                       = 0,
    NC_ERROR_INVALID_ARGUMENT   = 1,
    NC_ERROR_INVALID_OPERATION  = 2,
    NC_ERROR_NOT_IMPLEMENTED    = 3,
    NC_ERROR_INVALID_SESSION    = 101,
    NC_ERROR_MISSING_NATML_HUB  = 102,
    NC_ERROR_INVALID_NATML_HUB  = 103,
    NC_ERROR_INVALID_PLAN       = 104,
    NC_WARNING_LIMITED_PLAN     = 105,
};
typedef enum NCMediaStatus NCMediaStatus;
#pragma endregion


#pragma region --Client API--
/*!
 @function NCSetSessionToken
 
 @abstract Set the NatCorder session token.
 
 @discussion Set the NatCorder session token.
 
 @param token
 NatCorder session token.

 @returns Session status.
*/
NML_BRIDGE NML_EXPORT NCMediaStatus NML_API NCSetSessionToken (const char* token);
#pragma endregion