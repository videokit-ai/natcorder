/* 
*   NatCorder
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Recorders.Internal {

    using System;
    using System.IO;
    using System.Threading.Tasks;
    using UnityEngine;
    using Hub;
    using Hub.Internal;
    using Hub.Requests;

    #if UNITY_EDITOR
    using UnityEditor;
    #endif

    #if UNITY_EDITOR
    [InitializeOnLoad]
    #endif
    internal sealed class NatCorderSettings : ScriptableObject {

        #region --Data--
        [SerializeField, HideInInspector]
        private string token = string.Empty;
        #endregion


        #region --Client API--
        /// <summary>
        /// Get or set the NatCorder app token.
        /// </summary>
        internal string Token {
            get => !string.IsNullOrEmpty(token) ? token : null;
            set => token = value;
        }

        /// <summary>
        /// NatCorder settings for this project.
        /// </summary>
        public static NatCorderSettings Instance {
            get {
                #if UNITY_EDITOR
                // Check
                if (settings)
                    return settings;
                // Check
                if (EditorBuildSettings.TryGetConfigObject<NatCorderSettings>(SettingsIdentifier, out settings))
                    return settings;
                // Create
                Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath));
                settings = ScriptableObject.CreateInstance<NatCorderSettings>();
                AssetDatabase.CreateAsset(settings, SettingsPath);
                EditorBuildSettings.AddConfigObject(SettingsIdentifier, settings, true);
                return settings;
                #else
                return settings;
                #endif
            }
        }
        #endregion


        #region --Operations--
        public const string API = @"ai.natml.natcorder";
        public const string Version = @"1.8.3";
        private static NatCorderSettings settings;
        internal static string SettingsIdentifier => $"{API}.settings";
        private const string SettingsPath = @"Assets/NatML/Settings/NatCorder.asset";

        static NatCorderSettings () => HubSettings.OnUpdateSettings += UpdateToken;

        void OnEnable () => settings = this;

        internal static void UpdateToken (HubSettings hubSettings) {
            var input = new CreateAppTokenRequest.Input {
                api = API,
                version = Version,
                platform = NatMLHub.CurrentPlatform,
                bundle = HubSettings.EditorBundle
            };
            try {
                Instance.token = Task.Run(() => NatMLHub.CreateAppToken(input, hubSettings.AccessKey)).Result;
            } catch (Exception ex) {
                Debug.LogWarning($"NatCorder Error: {ex.InnerException.Message}");
            }
        }
        #endregion
    }
}