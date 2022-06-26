using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using UnityEngine;
using System.Threading;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using HarmonyLib;
using DG.Tweening;

namespace ADOFAIModLoader
{
    public class WaitForAnyKeyInput : CustomYieldInstruction
    {
        public override bool keepWaiting
        {
            get
            {
                return !Input.anyKeyDown;
            }
        }
        
    }
    public static class ModLoaderMain
    {
        private class AMLMonoBehaviour : MonoBehaviour
        {
            private void Update()
            {
                if (ADOBase.sceneName != "MODSettings" &&  Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F10))
                {
                    LoadSceneFromAssetBundle("MODSettings", false);
                }
            }
        }

        private class LoadingMonoBehaviour :  MonoBehaviour
        {
            private void Start()
            {
                StartCoroutine(LoadModCoroutine());
                GameObject go = new GameObject("AMLGameObject");
                DontDestroyOnLoad(go);
                AMLGameObject = go;
                go.AddComponent<AMLMonoBehaviour>();
            }
        }
        public static Dictionary<Type, Mod> mods = new Dictionary<Type, Mod>();
        public static Dictionary<Type, Mod> plugins = new Dictionary<Type, Mod>();
        private static AssetBundle amlScenes;
        private static AssetBundle amlAssets;
        internal static GameObject AMLGameObject;
        public static void LoadMods()
        {
            amlScenes = AssetBundle.LoadFromFile(Path.Combine("AMLAssets", "amlscenes"));
            amlAssets = AssetBundle.LoadFromFile(Path.Combine("AMLAssets", "amlassets"));
            LoadSceneFromAssetBundle("AMLIntro", false);
            SceneManager.activeSceneChanged += OnSceneChange;
            Harmony harmony = new Harmony("HARMONYFORAML");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
        private static void OnSceneChange(Scene b, Scene a)
        {
            if (a.name == "AMLIntro")
            {
                GameObject.Find("txt").AddComponent<LoadingMonoBehaviour>();
            }
            if (a.name == "MODSettings")
            {
                InitializeModSettings();
            }
        }
        static void InitializeModSettings()
        {
            GameObject controller = GameObject.Find("controller");
            controller.AddComponent<scrController>();
            GameObject.Find("Exit").GetComponent<Button>().onClick.AddListener(() =>
            {
                SceneManager.LoadScene(GCNS.sceneLevelSelect);
            });

            foreach (KeyValuePair<Type, Mod> kp in plugins)
            {
                var btno = UnityEngine.Object.Instantiate(LoadPrefabFromAssetBundle("ModButton"));
                var btn = btno.GetComponent<Button>();
                btn.onClick.AddListener(() =>
                {
                    GameObject.Find("name").GetComponent<Text>().text = "Mod / Plugin Name: " + kp.Value.ModName;
                    GameObject.Find("author").GetComponent<Text>().text = "Author: " + kp.Value.Author;
                    foreach (GameObject go in kp.Value.OnRenderSettings())
                    {
                        Transform mscont = GameObject.Find("ModSetting").transform.Find("Viewport").Find("Content");
                        go.transform.parent = mscont;
                    }
                });
            }
            foreach (KeyValuePair<Type, Mod> kp in mods)
            {
                var btno = UnityEngine.Object.Instantiate(LoadPrefabFromAssetBundle("ModButton"));
                var btn = btno.GetComponent<Button>();
                btn.onClick.AddListener(() =>
                {
                    GameObject.Find("name").GetComponent<Text>().text = "Mod / Plugin Name: " + kp.Value.ModName;
                    GameObject.Find("author").GetComponent<Text>().text = "Author: " + kp.Value.Author;
                    foreach (GameObject go in kp.Value.OnRenderSettings())
                    {
                        Transform mscont = GameObject.Find("ModSetting").transform.Find("Viewport").Find("Content");
                        go.transform.parent = mscont;
                    }
                });
            }
        }

        static IEnumerator LoadModCoroutine()
        {
            yield return null;
            var stopwatch = Stopwatch.StartNew();
            var pluginDir = new DirectoryInfo("Plugins");
            var modDir = new DirectoryInfo("Mods");
            if (!pluginDir.Exists)
            {
                pluginDir.Create();
            }
            if (!modDir.Exists)
            {
                modDir.Create();
            }
            GameObject.Find("txt").GetComponent<Text>().text = "Loading Plugins...";
            yield return null;
            foreach (DirectoryInfo d in pluginDir.GetDirectories())
            {
                DirectoryInfo libfolder = new DirectoryInfo(d.FullName + "Libs");
                if (libfolder.Exists)
                {
                    foreach (FileInfo f in libfolder.GetFiles("*.dll"))
                    {
                        Assembly.LoadFile(f.FullName);
                    }
                }
                var fa = d.GetFiles("*.dll")[0];
                var asm = Assembly.LoadFile(fa.FullName);
                foreach (Type t in asm.GetTypes())
                {
                    if (t.BaseType.Equals(typeof(Mod)))
                    {
                        var go = new GameObject("Loading Plugin...");
                        UnityEngine.Object.DontDestroyOnLoad(go);
                        var ic = go.AddComponent(t);
                        var i = (Mod)ic;
                        var pluginName = i.ModName;
                        var author = i.Author;
                        go.name = pluginName;
                        plugins.Add(t, i);
                        i.Startup();
                        ModLogger.InternalLog("Loaded Plugin " + pluginName + " by " + author);
                    }
                }
            }
            GameObject.Find("txt").GetComponent<Text>().text = "Loading Mods...";
            yield return null;
            foreach (DirectoryInfo d in modDir.GetDirectories())
            {
                DirectoryInfo libfolder = new DirectoryInfo(d.FullName + "Libs");
                if (libfolder.Exists)
                {
                    foreach (FileInfo f in libfolder.GetFiles("*.dll"))
                    {
                        Assembly.LoadFile(f.FullName);
                    }
                }

                var fa = d.GetFiles("*.dll")[0];
                var asm = Assembly.LoadFile(fa.FullName);
                foreach (Type t in asm.GetTypes())
                {
                    if (t.BaseType.Equals(typeof(Mod)))
                    {
                        var go = new GameObject("Loading Mod...");
                        UnityEngine.Object.DontDestroyOnLoad(go);
                        var ic = go.AddComponent(t);
                        var i = (Mod)ic;
                        var modName = i.ModName;
                        var author = i.Author;
                        mods.Add(t, i);
                        i.Startup();
                        ModLogger.InternalLog("Loaded Mod " + modName + "  by " + author);
                    }

                }
                
            }
            stopwatch.Stop();
            ModLogger.InternalLog("Loaded " + mods.Count + " mods and " + plugins.Count + " plugins in " + stopwatch.ElapsedMilliseconds + "ms.");
            GameObject.Find("txt").GetComponent<Text>().text = "Done!\n Loaded " + mods.Count + " mods and " + plugins.Count + " plugins in " + stopwatch.ElapsedMilliseconds + "ms.\nPress Any Key To Continue...";
            yield return new WaitForAnyKeyInput();
            SceneManager.LoadScene("scnSplash");
        }

        private static void LoadSceneFromAssetBundle(string sceneName, bool isAdditive)
        {
            AssetBundle assetBundle = amlScenes;
            string[] scenes = assetBundle.GetAllScenePaths();
            string loadScenePath = null;
            foreach (string sname in scenes)
            {
                if (sname.Contains(sceneName)){
                    loadScenePath = sname;
                }}if (loadScenePath == null) return;
            LoadSceneMode loadMode;
            if (isAdditive) loadMode = LoadSceneMode.Additive;
            else loadMode = LoadSceneMode.Single;
            SceneManager.LoadScene(loadScenePath, loadMode);
            scrController.deaths = 0;
        }
        private static GameObject LoadPrefabFromAssetBundle(string prefabname)
        {
            return amlAssets.LoadAsset<GameObject>(prefabname);
        }
    }
}
