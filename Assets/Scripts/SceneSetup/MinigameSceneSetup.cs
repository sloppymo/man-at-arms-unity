using UnityEngine;
using UnityEngine.UI;
using ManAtArms.UI;
using ManAtArms.Overworld;
using ManAtArms.Core;

namespace ManAtArms.SceneSetup
{
    /// <summary>
    /// Scene setup for minigame overlay with map.png
    /// Automatically configures the scene when loaded
    /// </summary>
    public class MinigameSceneSetup : MonoBehaviour
    {
        [Header("Scene Configuration")]
        [SerializeField] private bool autoSetupOnStart = true;
        [SerializeField] private string mapPath = "Art/Overworld/map";
        
        [Header("Camera Settings")]
        [SerializeField] private Color backgroundColor = new Color(0.1f, 0.1f, 0.2f, 1f);
        [SerializeField] private float cameraSize = 10f;
        
        private void Start()
        {
            if (autoSetupOnStart)
            {
                SetupScene();
            }
        }
        
        [ContextMenu("Setup Scene")]
        public void SetupScene()
        {
            Debug.Log("🎮 Setting up Minigame Scene...");
            
            // Setup camera
            SetupCamera();
            
            // Setup lighting
            SetupLighting();
            
            // Create minigame overlay
            CreateMinigameOverlay();
            
            // Setup audio
            SetupAudio();
            
            // Initialize services
            InitializeServices();
            
            Debug.Log("✅ Minigame Scene setup complete!");
        }
        
        private void SetupCamera()
        {
            // Find or create main camera
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                GameObject cameraObj = new GameObject("Main Camera");
                mainCamera = cameraObj.AddComponent<Camera>();
                cameraObj.tag = "MainCamera";
            }
            
            // Configure camera
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            mainCamera.backgroundColor = backgroundColor;
            mainCamera.orthographic = true;
            mainCamera.orthographicSize = cameraSize;
            
            // Add audio listener
            if (mainCamera.GetComponent<AudioListener>() == null)
            {
                mainCamera.gameObject.AddComponent<AudioListener>();
            }
            
            Debug.Log("📷 Camera setup complete");
        }
        
        private void SetupLighting()
        {
            // Create directional light
            GameObject lightObj = new GameObject("Directional Light");
            Light directionalLight = lightObj.AddComponent<Light>();
            directionalLight.type = LightType.Directional;
            directionalLight.intensity = 1f;
            directionalLight.color = Color.white;
            
            // Create ambient light
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.4f, 0.4f, 0.5f, 1f);
            
            Debug.Log("💡 Lighting setup complete");
        }
        
        private void CreateMinigameOverlay()
        {
            // Check if overlay already exists
            MinigameOverlay existingOverlay = FindObjectOfType<MinigameOverlay>();
            if (existingOverlay != null)
            {
                Debug.Log("⚠️ MinigameOverlay already exists, skipping creation");
                return;
            }
            
            // Create overlay GameObject
            GameObject overlayObj = new GameObject("MinigameOverlay");
            overlayObj.transform.position = Vector3.zero;
            
            // Add MinigameOverlay component
            MinigameOverlay overlay = overlayObj.AddComponent<MinigameOverlay>();
            
            // Create UI structure
            CreateUIStructure(overlayObj);
            
            Debug.Log("🎮 MinigameOverlay created");
        }
        
        private void CreateUIStructure(GameObject overlayObj)
        {
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
            mapImageRect.anchorMin = new Vector2(0.05f, 0.05f);
            mapImageRect.anchorMax = new Vector2(0.95f, 0.85f);
            mapImageRect.offsetMin = Vector2.zero;
            mapImageRect.offsetMax = Vector2.zero;
            
            RawImage mapImage = mapImageObj.AddComponent<RawImage>();
            mapImage.color = Color.white;
            
            // Load map sprite
            LoadMapSprite(mapImage);
            
            // Add Event Trigger for map clicks
            UnityEngine.EventSystems.EventTrigger eventTrigger = mapImageObj.AddComponent<UnityEngine.EventSystems.EventTrigger>();
            
            var clickEntry = new UnityEngine.EventSystems.EventTrigger.Entry();
            clickEntry.eventID = UnityEngine.EventSystems.EventTriggerType.PointerClick;
            clickEntry.callback.AddListener((data) => { OnMapClicked(); });
            eventTrigger.triggers.Add(clickEntry);
            
            // Create Player Marker
            CreatePlayerMarker(mapContainerObj);
            
            // Create Control Panel
            CreateControlPanel(canvasObj);
            
            // Configure MinigameOverlay component
            MinigameOverlay overlay = overlayObj.GetComponent<MinigameOverlay>();
            overlay.mapImage = mapImage;
            overlay.mapContainer = mapContainer;
            overlay.overlayCanvas = canvas;
            
            Debug.Log("🎨 UI structure created");
        }
        
        private void LoadMapSprite(RawImage mapImage)
        {
            // Try to load map sprite
            Sprite mapSprite = Resources.Load<Sprite>(mapPath);
            
            if (mapSprite != null)
            {
                // Convert sprite to texture
                Texture2D texture = mapSprite.texture;
                mapImage.texture = texture;
                mapImage.color = Color.white;
                
                Debug.Log($"🗺️ Map loaded from Resources/{mapPath}");
            }
            else
            {
                // Create procedural map
                CreateProceduralMap(mapImage);
                Debug.LogWarning($"⚠️ Map not found at Resources/{mapPath}, using procedural map");
            }
        }
        
        private void CreateProceduralMap(RawImage mapImage)
        {
            // Create a procedural map texture
            int width = 1024;
            int height = 1024;
            Texture2D texture = new Texture2D(width, height);
            
            // Generate terrain-like pattern
            Color[] pixels = new Color[width * height];
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Create noise-based terrain
                    float noise = Mathf.PerlinNoise(x * 0.01f, y * 0.01f);
                    
                    if (noise < 0.3f)
                    {
                        pixels[y * width + x] = new Color(0.2f, 0.4f, 0.2f, 1f); // Grass
                    }
                    else if (noise < 0.6f)
                    {
                        pixels[y * width + x] = new Color(0.5f, 0.4f, 0.3f, 1f); // Dirt
                    }
                    else
                    {
                        pixels[y * width + x] = new Color(0.3f, 0.3f, 0.4f, 1f); // Stone
                    }
                }
            }
            
            texture.SetPixels(pixels);
            texture.Apply();
            
            mapImage.texture = texture;
            mapImage.color = Color.white;
        }
        
        private void CreatePlayerMarker(GameObject mapContainer)
        {
            GameObject playerMarkerObj = new GameObject("PlayerMarker");
            playerMarkerObj.transform.SetParent(mapContainer.transform);
            playerMarkerObj.tag = "Player";
            
            RectTransform playerMarkerRect = playerMarkerObj.AddComponent<RectTransform>();
            playerMarkerRect.sizeDelta = new Vector2(30, 30);
            playerMarkerRect.anchoredPosition = Vector2.zero;
            
            Image playerImage = playerMarkerObj.AddComponent<Image>();
            playerImage.color = Color.yellow;
            
            // Add glow effect
            GameObject glowObj = new GameObject("Glow");
            glowObj.transform.SetParent(playerMarkerObj.transform);
            
            RectTransform glowRect = glowObj.AddComponent<RectTransform>();
            glowRect.sizeDelta = new Vector2(40, 40);
            glowRect.anchoredPosition = Vector2.zero;
            
            Image glowImage = glowObj.AddComponent<Image>();
            glowImage.color = new Color(1f, 1f, 0f, 0.3f);
            
            // Find MinigameOverlay and assign player marker
            MinigameOverlay overlay = FindObjectOfType<MinigameOverlay>();
            if (overlay != null)
            {
                overlay.playerMarker = playerMarkerObj;
            }
        }
        
        private void CreateControlPanel(GameObject canvas)
        {
            GameObject controlPanelObj = new GameObject("ControlPanel");
            controlPanelObj.transform.SetParent(canvas.transform);
            
            RectTransform controlPanelRect = controlPanelObj.AddComponent<RectTransform>();
            controlPanelRect.anchorMin = new Vector2(0, 0.85f);
            controlPanelRect.anchorMax = new Vector2(1, 1);
            controlPanelRect.offsetMin = new Vector2(10, 0);
            controlPanelRect.offsetMax = new Vector2(-10, -10);
            
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
            statusText.text = "Click on the map to move!";
            statusText.color = Color.white;
            statusText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            statusText.fontSize = 16;
            statusText.alignment = TextAnchor.MiddleLeft;
            
            // Create Test Button
            GameObject testButtonObj = new GameObject("TestButton");
            testButtonObj.transform.SetParent(controlPanelObj.transform);
            
            RectTransform testButtonRect = testButtonObj.AddComponent<RectTransform>();
            testButtonRect.anchorMin = new Vector2(0.75f, 0.2f);
            testButtonRect.anchorMax = new Vector2(0.95f, 0.8f);
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
            
            // Find MinigameOverlay and assign UI elements
            MinigameOverlay overlay = FindObjectOfType<MinigameOverlay>();
            if (overlay != null)
            {
                overlay.controlPanel = controlPanelObj;
                overlay.statusText = statusText;
                overlay.testButton = testButton;
            }
        }
        
        private void SetupAudio()
        {
            // Create Audio Manager if it doesn't exist
            if (FindObjectOfType<AudioManager>() == null)
            {
                GameObject audioManagerObj = new GameObject("AudioManager");
                audioManagerObj.AddComponent<AudioManager>();
            }
            
            Debug.Log("🎵 Audio setup complete");
        }
        
        private void InitializeServices()
        {
            // Initialize GameBootstrap if it doesn't exist
            if (FindObjectOfType<GameBootstrap>() == null)
            {
                GameObject bootstrapObj = new GameObject("GameBootstrap");
                bootstrapObj.AddComponent<GameBootstrap>();
            }
            
            Debug.Log("🔧 Services initialized");
        }
        
        private void OnMapClicked()
        {
            // This will be handled by the MinigameOverlay component
            Debug.Log("🖱️ Map clicked");
        }
        
        [ContextMenu("Create Test Scene")]
        public void CreateTestScene()
        {
            // Create a new scene with the minigame setup
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
            );
        }
    }
}
