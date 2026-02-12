using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using ManAtArms.UI;

namespace ManAtArms.Editor
{
    /// <summary>
    /// Editor script to create minigame overlay prefab
    /// </summary>
    public class MinigameOverlayCreator
    {
        [MenuItem("Man-at-Arms/Create Minigame Overlay")]
        public static void CreateMinigameOverlay()
        {
            // Create main GameObject
            GameObject overlayObj = new GameObject("MinigameOverlay");
            overlayObj.AddComponent<MinigameOverlay>();
            
            // Create Canvas
            GameObject canvasObj = new GameObject("Canvas");
            canvasObj.transform.SetParent(overlayObj.transform);
            
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // Create Map Container
            GameObject mapContainerObj = new GameObject("MapContainer");
            mapContainerObj.transform.SetParent(canvasObj.transform);
            
            RectTransform mapContainer = mapContainerObj.AddComponent<RectTransform>();
            mapContainer.anchorMin = Vector2.zero;
            mapContainer.anchorMax = Vector2.one;
            mapContainer.offsetMin = Vector2.zero;
            mapContainer.offsetMax = Vector2.zero;
            
            // Create Map Image
            GameObject mapImageObj = new GameObject("MapImage");
            mapImageObj.transform.SetParent(mapContainerObj.transform);
            
            RectTransform mapImageRect = mapImageObj.AddComponent<RectTransform>();
            mapImageRect.anchorMin = new Vector2(0.1f, 0.1f);
            mapImageRect.anchorMax = new Vector2(0.9f, 0.8f);
            mapImageRect.offsetMin = Vector2.zero;
            mapImageRect.offsetMax = Vector2.zero;
            
            RawImage mapImage = mapImageObj.AddComponent<RawImage>();
            mapImage.color = Color.white;
            
            // Add Event Trigger for map clicks
            UnityEngine.EventSystems.EventTrigger eventTrigger = mapImageObj.AddComponent<UnityEngine.EventSystems.EventTrigger>();
            
            // Create Player Marker
            GameObject playerMarkerObj = new GameObject("PlayerMarker");
            playerMarkerObj.transform.SetParent(mapContainerObj.transform);
            playerMarkerObj.tag = "Player";
            
            RectTransform playerMarkerRect = playerMarkerObj.AddComponent<RectTransform>();
            playerMarkerRect.sizeDelta = new Vector2(20, 20);
            playerMarkerRect.anchoredPosition = Vector2.zero;
            
            Image playerImage = playerMarkerObj.AddComponent<Image>();
            playerImage.color = Color.yellow;
            
            // Create Path Marker Prefab
            GameObject pathMarkerPrefab = new GameObject("PathMarker");
            pathMarkerPrefab.tag = "PathMarker";
            
            RectTransform pathMarkerRect = pathMarkerPrefab.AddComponent<RectTransform>();
            pathMarkerRect.sizeDelta = new Vector2(10, 10);
            
            Image pathMarkerImage = pathMarkerPrefab.AddComponent<Image>();
            pathMarkerImage.color = new Color(0, 1, 0, 0.5f);
            
            // Create Obstacle Marker Prefab
            GameObject obstacleMarkerPrefab = new GameObject("ObstacleMarker");
            obstacleMarkerPrefab.tag = "Obstacle";
            
            RectTransform obstacleMarkerRect = obstacleMarkerPrefab.AddComponent<RectTransform>();
            obstacleMarkerRect.sizeDelta = new Vector2(30, 30);
            
            Image obstacleMarkerImage = obstacleMarkerPrefab.AddComponent<Image>();
            obstacleMarkerImage.color = new Color(1, 0, 0, 0.7f);
            
            // Create Control Panel
            GameObject controlPanelObj = new GameObject("ControlPanel");
            controlPanelObj.transform.SetParent(canvasObj.transform);
            
            RectTransform controlPanelRect = controlPanelObj.AddComponent<RectTransform>();
            controlPanelRect.anchorMin = new Vector2(0, 0.8f);
            controlPanelRect.anchorMax = new Vector2(1, 1);
            controlPanelRect.offsetMin = new Vector2(20, -20);
            controlPanelRect.offsetMax = new Vector2(-20, 20);
            
            Image controlPanelBg = controlPanelObj.AddComponent<Image>();
            controlPanelBg.color = new Color(0, 0, 0, 0.7f);
            
            // Create Status Text
            GameObject statusTextObj = new GameObject("StatusText");
            statusTextObj.transform.SetParent(controlPanelObj.transform);
            
            RectTransform statusTextRect = statusTextObj.AddComponent<RectTransform>();
            statusTextRect.anchorMin = Vector2.zero;
            statusTextRect.anchorMax = new Vector2(0.7f, 1);
            statusTextRect.offsetMin = new Vector2(10, 5);
            statusTextRect.offsetMax = new Vector2(-5, -5);
            
            Text statusText = statusTextObj.AddComponent<Text>();
            statusText.text = "Ready to play!";
            statusText.color = Color.white;
            statusText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            statusText.fontSize = 16;
            statusText.alignment = TextAnchor.MiddleLeft;
            
            // Create Test Button
            GameObject testButtonObj = new GameObject("TestButton");
            testButtonObj.transform.SetParent(controlPanelObj.transform);
            
            RectTransform testButtonRect = testButtonObj.AddComponent<RectTransform>();
            testButtonRect.anchorMin = new Vector2(0.7f, 0.3f);
            testButtonRect.anchorMax = new Vector2(0.9f, 0.7f);
            testButtonRect.offsetMin = Vector2.zero;
            testButtonRect.offsetMax = Vector2.zero;
            
            Image testButtonBg = testButtonObj.AddComponent<Image>();
            testButtonBg.color = new Color(0.2f, 0.6f, 1f, 0.8f);
            
            Button testButton = testButtonObj.AddComponent<Button>();
            testButton.targetGraphic = testButtonBg;
            
            GameObject testButtonTextObj = new GameObject("Text");
            testButtonTextObj.transform.SetParent(testButtonObj.transform);
            
            RectTransform testButtonTextRect = testButtonTextObj.AddComponent<RectTransform>();
            testButtonTextRect.anchorMin = Vector2.zero;
            testButtonTextRect.anchorMax = Vector2.one;
            testButtonTextRect.offsetMin = Vector2.zero;
            testButtonTextRect.offsetMax = Vector2.zero;
            
            Text testButtonText = testButtonTextObj.AddComponent<Text>();
            testButtonText.text = "Test";
            testButtonText.color = Color.white;
            testButtonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            testButtonText.fontSize = 14;
            testButtonText.alignment = TextAnchor.MiddleCenter;
            
            // Configure MinigameOverlay component
            MinigameOverlay overlay = overlayObj.GetComponent<MinigameOverlay>();
            overlay.mapImage = mapImage;
            overlay.mapContainer = mapContainer;
            overlay.overlayCanvas = canvas;
            overlay.playerMarker = playerMarkerObj;
            overlay.pathMarkerPrefab = pathMarkerPrefab;
            overlay.obstacleMarkerPrefab = obstacleMarkerPrefab;
            overlay.controlPanel = controlPanelObj;
            overlay.statusText = statusText;
            overlay.testButton = testButton;
            
            // Create prefabs
            SavePrefab("PathMarker", pathMarkerPrefab);
            SavePrefab("ObstacleMarker", obstacleMarkerPrefab);
            
            // Save main overlay prefab
            string overlayPath = "Assets/Prefabs/MinigameOverlay.prefab";
            PrefabUtility.SaveAsPrefabAsset(overlayObj, overlayPath);
            
            Debug.Log($"✅ Minigame Overlay created at {overlayPath}");
            Debug.Log($"✅ Path Marker prefab created");
            Debug.Log($"✅ Obstacle Marker prefab created");
            
            // Clean up scene objects
            Object.DestroyImmediate(overlayObj);
        }
        
        private static void SavePrefab(string name, GameObject obj)
        {
            string path = $"Assets/Prefabs/{name}.prefab";
            
            // Ensure Prefabs folder exists
            if (!System.IO.Directory.Exists("Assets/Prefabs"))
            {
                System.IO.Directory.CreateDirectory("Assets/Prefabs");
            }
            
            PrefabUtility.SaveAsPrefabAsset(obj, path);
            Object.DestroyImmediate(obj);
        }
    }
}
