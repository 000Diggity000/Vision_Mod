using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BoplFixedMath;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace Teleport_Thingy
{
    [BepInPlugin("com.000diggity000.vision_mod", "Vision Mod", "1.0.0")]
    public class Main : BaseUnityPlugin
    {
        public static Stream GetResourceStream(string namespaceName, string path) => Assembly.GetExecutingAssembly().GetManifestResourceStream($"{namespaceName}.{path}");
        public static AssetBundle bundle;
        public static GameObject vision;
        public static Image visionImage;
        public static bool inMatch = false;
        private void Awake()
        {
            Harmony harmony = new Harmony("com.000diggity000.vision_mod");
            harmony.PatchAll(typeof(Patches));
            Logger.LogInfo("Vision Mod Loaded");
        }
        private void Start()
        {
            //thx Almafa to some of the code here
            string modPath = Paths.PluginPath;
            string namespaceName = "Teleport_Thingy"; // The name of the namespace of the dll is located, generally this is the same as PLUGIN_NAME

            string fileName = "test.bundle"; // embedded folders use '.' as separator not '/', so if file is inside Assets then Assets.test.bundle

            bundle = AssetBundle.LoadFromStream(GetResourceStream(namespaceName, fileName));

            GameObject testPrefab = (GameObject)bundle.LoadAsset("Vision_Canvas");
            vision = GameObject.Instantiate(testPrefab, new Vector2(0, 0), Quaternion.identity);
            DontDestroyOnLoad(vision);
            visionImage = vision.GetComponentInChildren<Image>();
            visionImage.enabled = false;
        }
        private void FixedUpdate()
        {
            if(inMatch)
            {
                visionImage.rectTransform.localScale = new Vector2(visionImage.rectTransform.localScale.x + 0.0008f, visionImage.rectTransform.localScale.y + 0.0008f);
            }
        }
        void OnApplicationQuit()
        {
            
        }
    }
    public class Patches
    {
        [HarmonyPatch(typeof(GameSessionHandler), "LeaveGame")]
        [HarmonyPrefix]
        public static void LeavePatch()
        {
            Main.inMatch = false;
            Main.visionImage.enabled = false;
        }
        [HarmonyPatch(typeof(GameSessionHandler), "LoadNextLevelScene")]
        [HarmonyPrefix]
        public static void NextLevelPatch()
        {
            Main.visionImage.enabled = true;
            Main.visionImage.rectTransform.localScale = new Vector3(1, 1, 1);
            Main.inMatch = true;
        }
        [HarmonyPatch(typeof(GameSessionHandler), "LoadAbilitySelectScene")]
        [HarmonyPrefix]
        public static void AbilitySelectPatch()
        {
            Main.inMatch = false;
            Main.visionImage.enabled = false;
        }
        [HarmonyPatch(typeof(ExitGameMenu), "LeaveGame")]
        [HarmonyPostfix]
        public static void LeaveGamePatch()
        {
            Main.inMatch = false;
            Main.visionImage.enabled = false;
        }
        [HarmonyPatch(typeof(GameSessionHandler), "DeclareGameUndecided")]
        [HarmonyPrefix]
        public static void UndecidedPatch()
        {
            Main.inMatch = false;
            Main.visionImage.enabled = false;
        }
        [HarmonyPatch(typeof(GameSessionHandler), "WinGame")]
        [HarmonyPrefix]
        public static void WinGamePatch()
        {
            Main.inMatch = false;
            Main.visionImage.enabled = false;
        }
        [HarmonyPatch(typeof(CharacterSelectHandler), "TryStartGame")]
        [HarmonyPrefix]
        public static void StartPatch(CharacterSelectHandler __instance)
        {
            Main.visionImage.enabled = true;
            Main.visionImage.rectTransform.localScale = new Vector3(1, 1, 1);
            Main.inMatch = true;
        }
        [HarmonyPatch(typeof(CharacterSelectHandler_online), "TryStartGame")]
        [HarmonyPrefix]
        public static void StartOnlinePatch(CharacterSelectHandler_online __instance)
        {
            Main.visionImage.enabled = true;
            Main.visionImage.rectTransform.localScale = new Vector3(1, 1, 1);
            Main.inMatch = true;
        }
    }
}
