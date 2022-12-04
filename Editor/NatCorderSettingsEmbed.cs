/* 
*   NatCorder
*   Copyright (c) 2022 NatML Inc. All rights reserved.
*/

namespace NatML.Recorders.Editor {

    using System;
    using System.IO;
    using UnityEditor;
    using UnityEditor.Build.Reporting;
    using UnityEngine;
    using Hub;
    using Hub.Editor;
    using Hub.Internal;
    using Internal;

    internal sealed class NatCorderSettingsEmbed : BuildEmbedHelper<NatCorderSettings> {

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
            var platform = ToPlatform(report.summary.platform);
            var bundle = BundleOverride?.identifier ?? NatMLHub.GetAppBundle(platform);
            var accessKey = HubSettings.Instance.AccessKey;
            var settings = NatCorderProjectSettings.CurrentSettings;
            try {
                settings = NatCorderProjectSettings.CreateSettings(platform, bundle, accessKey);
            } catch(Exception ex) {
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