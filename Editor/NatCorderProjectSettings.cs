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
        /// NatCorder settings from the current project settings.
        /// </summary>
        internal static NatCorderSettings CurrentSettings {
            get {
                var settings = ScriptableObject.CreateInstance<NatCorderSettings>();
                settings.token = SessionState.GetString(tokenKey, string.Empty);
                return settings;
            }
        }

        /// <summary>
        /// Create NatCorder settings.
        /// </summary>
        /// <param name="platform">NatML platform identifier.</param>
        /// <param name="bundle">NatML app bundle.</param>
        /// <param name="accessKey">NatML access key.</param>
        internal static NatCorderSettings CreateSettings (string platform, string bundle, string accessKey) {
            var input = new CreateMediaSessionRequest.Input {
                api = NatCorderSettings.API,
                version = NatCorderSettings.Version,
                platform = platform,
                bundle = bundle,
            };
            var session = Task.Run(() => NatMLHub.CreateMediaSession(input, accessKey)).Result;
            var settings = ScriptableObject.CreateInstance<NatCorderSettings>();
            settings.token = session.token;
            return settings;
        }
        #endregion


        #region --Operations--
        private static string tokenKey => $"{NatCorderSettings.API}.token";

        [InitializeOnLoadMethod]
        private static void OnLoad () {
            NatCorderSettings.Instance = CurrentSettings;
            HubSettings.OnUpdateSettings += OnUpdateHubSettings;
        }

        private static void OnUpdateHubSettings (HubSettings hubSettings) {
            try {
                var settings = CreateSettings(NatMLHub.CurrentPlatform, NatMLHub.GetEditorBundle(), hubSettings.AccessKey);
                SessionState.SetString(tokenKey, settings.token);
            } catch (Exception ex) {
                SessionState.EraseString(tokenKey);
                Debug.LogWarning($"NatCorder Error: {ex.InnerException.Message}");
            }
            NatCorderSettings.Instance = CurrentSettings;
        }
        #endregion
    }
}