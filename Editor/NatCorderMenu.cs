/* 
*   NatCorder
*   Copyright (c) 2022 NatML Inc. All rights reserved.
*/

namespace NatML.Recorders.Editor {

    using UnityEditor;
    using Internal;

    internal static class NatCorderMenu {

        private const int BasePriority = 0;

        [MenuItem(@"NatML/NatCorder " + NatCorderSettings.Version, false, BasePriority)]
        private static void Version () { }

        [MenuItem(@"NatML/NatCorder " + NatCorderSettings.Version, true, BasePriority)]
        private static bool DisableVersion () => false;

        [MenuItem(@"NatML/View NatCorder Docs", false, BasePriority + 1)]
        private static void OpenDocs () => Help.BrowseURL(@"https://docs.natml.ai/natcorder");

        [MenuItem(@"NatML/Open a NatCorder Issue", false, BasePriority + 2)]
        private static void OpenIssue () => Help.BrowseURL(@"https://github.com/natmlx/NatCorder");
    }
}