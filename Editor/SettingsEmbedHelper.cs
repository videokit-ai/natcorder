/* 
*   NatCorder
*   Copyright (c) 2022 NatML Inc. All rights reserved.
*/

namespace NatML.Recorders.Editor {

    using System;
    using System.IO;
    using System.Threading.Tasks;
    using UnityEditor;
    using UnityEditor.Build.Reporting;
    using UnityEngine;
    using Hub;
    using Hub.Editor;
    using Hub.Internal;
    using Hub.Requests;
    using Internal;

    internal sealed class SettingsEmbedHelper : BuildEmbedHelper<NatCorderSettings> {

        protected override BuildTarget[] SupportedTargets => new [] {
            BuildTarget.Android,
            BuildTarget.iOS,
            BuildTarget.StandaloneOSX,
            BuildTarget.StandaloneWindows,
            BuildTarget.StandaloneWindows64,
            BuildTarget.WebGL,
        };
        private const string CachePath = @"Assets/NMLBuildCache";

        protected override NatCorderSettings[] CreateEmbeds (BuildReport report) {
            var webIdentifier = $"{Application.productName.Replace(' ', '-')}-{Application.version.Replace(' ', '-')}";
            var identifier = report.summary.platform == BuildTarget.WebGL ? webIdentifier : Application.identifier;
            var input = new CreateAppTokenRequest.Input {
                api = NatCorderSettings.API,
                version = NatCorderSettings.Version,
                platform = ToPlatform(report.summary.platform),
                bundle = BundleOverride?.identifier ?? identifier
            };
            var accessKey = HubSettings.Instance.AccessKey;
            var settings = NatCorderProjectSettings.CreateSettings();
            settings.token = null;
            try {
                settings.token = Task.Run(() => NatMLHub.CreateAppToken(input, accessKey)).Result;
            } catch (Exception ex) {
                Debug.LogWarning($"NatCorder Error: {ex.InnerException.Message}");
            }
            Directory.CreateDirectory(CachePath);
            AssetDatabase.CreateAsset(settings, $"{CachePath}/NatCorder.asset");
            return new [] { settings };
        }

        protected override void ClearEmbeds (BuildReport report) {
            base.ClearEmbeds(report);
            AssetDatabase.DeleteAsset(CachePath);
        }
    }
}