/* 
*   NatCorder
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Recorders.Internal {

    using UnityEngine;

    internal sealed class NatCorderSettings : ScriptableObject {

        #region --Data--
        [SerializeField, HideInInspector]
        internal string token = string.Empty;
        #endregion


        #region --Client API--
        /// <summary>
        /// Get or set the NatCorder app token.
        /// </summary>
        internal string Token => !string.IsNullOrEmpty(token) ? token : null;

        /// <summary>
        /// NatCorder settings for this project.
        /// </summary>
        public static NatCorderSettings Instance { get; internal set; }
        #endregion


        #region --Operations--
        public const string API = @"ai.natml.natcorder";
        public const string Version = @"1.9.2";

        void OnEnable () {
            if (!Application.isEditor)
                Instance = this;
        }
        #endregion
    }
}