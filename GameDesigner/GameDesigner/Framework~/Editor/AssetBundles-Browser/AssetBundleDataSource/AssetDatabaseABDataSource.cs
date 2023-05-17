using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.Assertions;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using AssetBundleBrowser.AssetBundleModel;

namespace AssetBundleBrowser.AssetBundleDataSource
{
    internal class AssetDatabaseABDataSource : ABDataSource
    {
        public static List<ABDataSource> CreateDataSources()
        {
            var op = new AssetDatabaseABDataSource();
            var retList = new List<ABDataSource>();
            retList.Add(op);
            return retList;
        }

        public string Name {
            get {
                return "Default";
            }
        }

        public string ProviderName {
            get {
                return "Built-in";
            }
        }

        public string[] GetAssetPathsFromAssetBundle (string assetBundleName) {
            return AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName);
        }

        public string GetAssetBundleName(string assetPath) {
            var importer = AssetImporter.GetAtPath(assetPath);
            if (importer == null) {
                return string.Empty;
            }
            var bundleName = importer.assetBundleName;
            if (importer.assetBundleVariant.Length > 0) {
                bundleName = bundleName + "." + importer.assetBundleVariant;
            }
            return bundleName;
        }

        public string GetImplicitAssetBundleName(string assetPath) {
            return AssetDatabase.GetImplicitAssetBundleName (assetPath);
        }

        public string[] GetAllAssetBundleNames() {
            return AssetDatabase.GetAllAssetBundleNames ();
        }

        public bool IsReadOnly() {
            return false;
        }

        public void SetAssetBundleNameAndVariant (string assetPath, string bundleName, string variantName) {
            AssetImporter.GetAtPath(assetPath).SetAssetBundleNameAndVariant(bundleName, variantName);
        }

        public void RemoveUnusedAssetBundleNames() {
            AssetDatabase.RemoveUnusedAssetBundleNames ();
        }

        public bool CanSpecifyBuildTarget { 
            get { return true; } 
        }
        public bool CanSpecifyBuildOutputDirectory { 
            get { return true; } 
        }

        public bool CanSpecifyBuildOptions { 
            get { return true; } 
        }

        public bool BuildAssetBundles (ABBuildInfo info, AssetBundleManageTab m_ManageTab)
        {
            if(info == null)
            {
                Debug.Log("Error in build");
                return false;
            }

            var builds = new List<AssetBundleBuild>();
            var rows = m_ManageTab.m_BundleTree.m_Controller.m_BundleTree.GetRows();
            foreach (BundleTreeItem row in rows)
            {
                if (!row.m_Enable)
                    continue;
                var bundleInfo = row.bundle as BundleDataInfo;
                var build = new AssetBundleBuild();
                build.assetBundleName = bundleInfo.displayName;
                var assetNames = new List<string>();
                foreach (var concrete in bundleInfo.m_ConcreteAssets)
                {
                    if (concrete.isFolder)
                        continue;
                    assetNames.Add(concrete.fullAssetName);
                }
                foreach (var concrete in bundleInfo.m_DependentAssets)
                {
                    if (concrete.isFolder)
                        continue;
                    assetNames.Add(concrete.fullAssetName);
                }
                build.assetNames = assetNames.ToArray();
                builds.Add(build);
            }

            for (int i = 0; i < builds.Count; i++)
            {
                var build = builds[i];
                var buildManifest = BuildPipeline.BuildAssetBundles(info.outputDirectory, new AssetBundleBuild[] { build }, info.options, info.buildTarget);
                if (buildManifest == null)
                {
                    Debug.Log("打包:" + build.assetBundleName + "失败！！！！");
                    return false;
                }
                foreach (var assetBundleName in buildManifest.GetAllAssetBundles())
                {
                    info.onBuild?.Invoke(assetBundleName);
                }
                Debug.Log("打包:" + build.assetBundleName + "成功！");
            }

            return true;
        }
    }
}
