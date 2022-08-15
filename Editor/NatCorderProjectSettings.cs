/* 
*   NatCorder
*   Copyright (c) 2022 NatML Inc. All rights reserved.
*/

namespace NatML.Recorders.Editor {

    using System;
    using System.Threading.Tasks;
    using UnityEngine;
    using UnityEditor;
    using Hub;
    using Hub.Internal;
    using Hub.Requests;
    using Internal;

    [FilePath(@"ProjectSettings/NatCorder.asset", FilePathAttribute.Location.ProjectFolder)]
    internal sealed class NatCorderProjectSettings : ScriptableSingleton<NatCorderProjectSettings> {

        #region --Client API--
        /// <summary>
        /// Create NatCorder settings from the current project settings.
        /// </summary>
        internal static NatCorderSettings CreateSettings () {
            var settings = ScriptableObject.CreateInstance<NatCorderSettings>();
            settings.token = SessionState.GetString(tokenKey, string.Empty);
            return settings;
        }
        #endregion

        #region --Operations--
        private static string tokenKey => $"{NatCorderSettings.API}.token";

        [InitializeOnLoadMethod]
        private static void OnLoad () {
            NatCorderSettings.Instance = CreateSettings();
            HubSettings.OnUpdateSettings += OnUpdateHubSettings;
        }

        private static void OnUpdateHubSettings (HubSettings settings) {
            var input = new CreateAppTokenRequest.Input {
                api = NatCorderSettings.API,
                version = NatCorderSettings.Version,
                platform = NatMLHub.CurrentPlatform,
                bundle = HubSettings.EditorBundle
            };
            try {
                var token = Task.Run(() => NatMLHub.CreateAppToken(input, settings.AccessKey)).Result;
                SessionState.SetString(tokenKey, token);
            } catch (Exception ex) {
                SessionState.EraseString(tokenKey);
                Debug.LogWarning($"NatCorder Error: {ex.InnerException.Message}");
            }
            NatCorderSettings.Instance = CreateSettings();
        }
        #endregion
    }
}