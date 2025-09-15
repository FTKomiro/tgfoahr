using System.Collections;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;

// HUDRemover for Tainted Grail: Fall of Avalon by FTKomiro
namespace HUDRemover
{
    [BepInPlugin(PluginConsts.PLUGIN_GUID, PluginConsts.PLUGIN_NAME, PluginConsts.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource Log;

        private GameObject hudObject;
        private GameObject compassObject;
        private bool isHudVisible = true;
        private bool objectsFound = false;

        private void Awake()
        {
            Log = Logger;
            SceneManager.sceneLoaded += OnSceneLoaded;
            StartCoroutine(FindHudAfterDelay());
            Log.LogInfo($"Plugin {PluginConsts.PLUGIN_GUID} is loaded!");
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!objectsFound)
            {
                StartCoroutine(FindHudAfterDelay());
            }
        }

        private System.Collections.IEnumerator FindHudAfterDelay()
        {
            yield return new WaitForSeconds(2f); 

            hudObject = GameObject.Find("Root/UI/Canvases/HUDCanvas/HUD");
            compassObject = GameObject.Find("Root/UI/Canvases/CompassCanvas");

            if (hudObject == null || compassObject == null)
            {
                if (!objectsFound)
                {
                    StartCoroutine(PollForHud()); 
                }
            }
            else
            {
                Log.LogInfo($"HUD and Compass found");
                objectsFound = true;
                hudObject.SetActive(true);
                compassObject.SetActive(true);
            }
        }

        private System.Collections.IEnumerator PollForHud()
        {
            int a = 0;
            while (!objectsFound)
            {
                yield return new WaitForSeconds(2f);
                hudObject = GameObject.Find("Root/UI/Canvases/HUDCanvas/HUD");
                compassObject = GameObject.Find("Root/UI/Canvases/CompassCanvas");

                if (hudObject != null && compassObject != null)
                {
                    objectsFound = true;
                    hudObject.SetActive(true);
                    compassObject.SetActive(true);
                    yield break; 
                }
                else
                {
                    a++;
                }
                if (a >= 500)
                {
                    Log.LogWarning("Failed to find HUD & Compass");
                    Destroy(this);
                    yield break;
                }
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F11))
            {
                Log.LogInfo("toggling HUD");
                if (!objectsFound)
                {
                    Log.LogWarning("Objects not found");
                    StartCoroutine(FindHudAfterDelay());
                    return;
                }

                isHudVisible = !isHudVisible;
                if (hudObject != null)
                {
                    hudObject.SetActive(isHudVisible);
                    Log.LogInfo($"HUD toggled to: {(isHudVisible ? "Visible" : "Hidden")}");
                }
                if (compassObject != null)
                {
                    compassObject.SetActive(isHudVisible);
                    Log.LogInfo($"Compass toggled to: {(isHudVisible ? "Visible" : "Hidden")}");
                }
            }
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded; 
            Log.LogInfo($"Plugin {PluginConsts.PLUGIN_GUID} is unloaded");
        }
    }
}
