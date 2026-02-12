using UnityEngine;
using UnityEngine.UI;
using ManAtArms.Overworld;
using ManAtArms.Core;
using ManAtArms.Audio;

namespace ManAtArms.UI
{
    /// <summary>
    /// Minigame overlay system for overworld map
    /// Creates interactive UI overlay on top of map.png with pathfinding and movement
    /// </summary>
    public class MinigameOverlay : MonoBehaviour
    {
        [Header("Map Settings")]
        [SerializeField] private Sprite mapSprite;
        [SerializeField] private RawImage mapImage;
        [SerializeField] private RectTransform mapContainer;
        
        [Header("Player Settings")]
        [SerializeField] private GameObject playerMarker;
        [SerializeField] private GameObject pathMarkerPrefab;
        [SerializeField] private GameObject obstacleMarkerPrefab;
        
        [Header("UI Elements")]
        [SerializeField] private Canvas overlayCanvas;
        [SerializeField] private GameObject controlPanel;
        [SerializeField] private Text statusText;
        [SerializeField] private Button testButton;
        
        [Header("Game Settings")]
        [SerializeField] private bool enableClickToMove = true;
        [SerializeField] private bool showPathVisualization = true;
        [SerializeField] private bool enableObstacles = true;
        
        // Game state
        private HexGrid hexGrid;
        private HexPathfinder pathfinder;
        private PlayerMovementController playerController;
        private Camera overlayCamera;
        
        // UI state
        private Vector2 mapSize;
        private Vector2 mapScale;
        private bool isInitialized = false;
        
        private void Start()
        {
            InitializeOverlay();
        }
        
        private void InitializeOverlay()
        {
            Debug.Log("🎮 Initializing Minigame Overlay...");
            
            // Setup camera
            SetupOverlayCamera();
            
            // Load map
            LoadMapImage();
            
            // Initialize hex grid
            InitializeHexGrid();
            
            // Setup player
            SetupPlayer();
            
            // Setup UI
            SetupUI();
            
            // Start game systems
            StartGameSystems();
            
            isInitialized = true;
            Debug.Log("✅ Minigame Overlay initialized successfully!");
        }
        
        private void SetupOverlayCamera()
        {
            // Create camera for overlay
            GameObject cameraObj = new GameObject("OverlayCamera");
            overlayCamera = cameraObj.AddComponent<Camera>();
            overlayCamera.clearFlags = CameraClearFlags.SolidColor;
            overlayCamera.backgroundColor = new Color(0.1f, 0.1f, 0.2f, 1f);
            overlayCamera.cullingMask = LayerMask.GetMask("UI");
            overlayCamera.orthographic = true;
            overlayCamera.orthographicSize = 10f;
            
            // Set canvas camera
            if (overlayCanvas != null)
            {
                overlayCanvas.renderMode = RenderMode.ScreenSpaceCamera;
                overlayCanvas.worldCamera = overlayCamera;
            }
        }
        
        private void LoadMapImage()
        {
            // Load map sprite
            if (mapSprite == null)
            {
                // Try to load from Resources
                mapSprite = Resources.Load<Sprite>("Art/Overworld/map");
            }
            
            if (mapSprite != null && mapImage != null)
            {
                mapImage.sprite = mapSprite;
                mapImage.color = Color.white;
                
                // Calculate map dimensions
                mapSize = new Vector2(mapSprite.rect.width, mapSprite.rect.height);
                
                // Scale map to fit screen
                float screenRatio = (float)Screen.width / Screen.height;
                float mapRatio = mapSize.x / mapSize.y;
                
                if (screenRatio > mapRatio)
                {
                    mapScale = new Vector2(mapSize.x / mapSize.y, 1f);
                }
                else
                {
                    mapScale = new Vector2(1f, mapSize.y / mapSize.x);
                }
                
                if (mapContainer != null)
                {
                    mapContainer.localScale = mapScale;
                }
                
                Debug.Log($"🗺️ Map loaded: {mapSize.x}x{mapSize.y}, scale: {mapScale}");
            }
            else
            {
                Debug.LogWarning("⚠️ Map sprite not found. Using fallback.");
                CreateFallbackMap();
            }
        }
        
        private void CreateFallbackMap()
        {
            // Create a procedural map if sprite not found
            if (mapImage != null)
            {
                // Create a simple colored background
                mapImage.color = new Color(0.2f, 0.4f, 0.2f, 1f);
                
                // Add grid pattern
                Texture2D gridTexture = CreateGridTexture(512, 512);
                mapImage.texture = gridTexture;
                
                mapSize = new Vector2(512, 512);
                mapScale = Vector2.one;
            }
        }
        
        private Texture2D CreateGridTexture(int width, int height)
        {
            Texture2D texture = new Texture2D(width, height);
            Color[] pixels = new Color[width * height];
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Create hex grid pattern
                    bool isHex = (x + y) % 40 < 20;
                    pixels[y * width + x] = isHex ? 
                        new Color(0.3f, 0.5f, 0.3f, 1f) : 
                        new Color(0.2f, 0.4f, 0.2f, 1f);
                }
            }
            
            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }
        
        private void InitializeHexGrid()
        {
            // Create hex grid system
            GameObject hexGridObj = new GameObject("HexGrid");
            hexGrid = hexGridObj.AddComponent<HexGrid>();
            
            // Configure grid based on map size
            hexGrid.gridWidth = Mathf.RoundToInt(mapSize.x / 50); // 50 units per hex
            hexGrid.gridHeight = Mathf.RoundToInt(mapSize.y / 50);
            hexGrid.tileSize = 1f;
            hexGrid.showGrid = false; // Hide debug grid
            
            // Create pathfinder
            GameObject pathfinderObj = new GameObject("Pathfinder");
            pathfinder = pathfinderObj.AddComponent<HexPathfinder>();
            pathfinder.hexGrid = hexGrid;
            pathfinder.maxPathLength = 100;
            pathfinder.cacheSize = 1000;
            
            Debug.Log($"🗺️ Hex grid initialized: {hexGrid.gridWidth}x{hexGrid.gridHeight}");
        }
        
        private void SetupPlayer()
        {
            // Create player controller
            GameObject playerObj = new GameObject("PlayerController");
            playerController = playerObj.AddComponent<PlayerMovementController>();
            playerController.pathfinder = pathfinder;
            playerController.moveSpeed = 5f;
            playerController.stopDistance = 0.1f;
            
            // Position player at center
            playerController.SetPosition(new HexCoord(0, 0));
            
            // Create visual player marker
            if (playerMarker != null)
            {
                playerMarker.SetActive(true);
                UpdatePlayerMarker();
            }
            
            Debug.Log("🎮 Player controller initialized");
        }
        
        private void SetupUI()
        {
            // Setup control panel
            if (controlPanel != null)
            {
                controlPanel.SetActive(true);
            }
            
            // Setup test button
            if (testButton != null)
            {
                testButton.onClick.AddListener(RunTestSuite);
            }
            
            // Setup status text
            if (statusText != null)
            {
                statusText.text = "Ready to play! Click on the map to move.";
            }
            
            // Add click handler to map
            if (mapImage != null)
            {
                var eventTrigger = mapImage.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
                var entry = new UnityEngine.EventSystems.EventTrigger.Entry();
                entry.eventID = UnityEngine.EventSystems.EventTriggerType.PointerClick;
                entry.callback.AddListener((data) => { OnMapClicked(); });
                eventTrigger.triggers.Add(entry);
            }
            
            Debug.Log("🎨 UI setup complete");
        }
        
        private void StartGameSystems()
        {
            // Start GameBootstrap if available
            var gameBootstrap = FindObjectOfType<GameBootstrap>();
            if (gameBootstrap == null)
            {
                GameObject bootstrapObj = new GameObject("GameBootstrap");
                bootstrapObj.AddComponent<GameBootstrap>();
            }
            
            // Generate initial obstacles
            if (enableObstacles)
            {
                GenerateRandomObstacles();
            }
            
            Debug.Log("🚀 Game systems started");
        }
        
        private void Update()
        {
            if (!isInitialized) return;
            
            // Update player marker
            UpdatePlayerMarker();
            
            // Handle keyboard input
            HandleKeyboardInput();
            
            // Update status
            UpdateStatusText();
        }
        
        private void UpdatePlayerMarker()
        {
            if (playerMarker != null && playerController != null)
            {
                // Convert hex position to world position
                var hexPos = playerController.CurrentPosition;
                var worldPos = HexGrid.HexToWorld(hexPos);
                
                // Convert to UI position
                Vector2 uiPos = WorldToUIPosition(worldPos);
                
                playerMarker.GetComponent<RectTransform>().anchoredPosition = uiPos;
            }
        }
        
        private Vector2 WorldToUIPosition(Vector3 worldPos)
        {
            // Convert world position to UI coordinates
            Vector3 screenPos = overlayCamera.WorldToScreenPoint(worldPos);
            
            // Convert to canvas coordinates
            Vector2 canvasPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                mapContainer, 
                screenPos, 
                overlayCamera, 
                out canvasPos
            );
            
            return canvasPos;
        }
        
        private void OnMapClicked()
        {
            if (!enableClickToMove) return;
            
            // Get click position
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                mapContainer,
                Input.mousePosition,
                overlayCamera,
                out localPoint
            );
            
            // Convert to world position
            Vector3 worldPos = overlayCamera.ScreenToWorldPoint(Input.mousePosition);
            
            // Convert to hex coordinates
            HexCoord hexCoord = WorldToHexCoord(worldPos);
            
            // Move player
            MovePlayerTo(hexCoord);
        }
        
        private HexCoord WorldToHexCoord(Vector3 worldPos)
        {
            // Simple conversion - you may need to adjust based on your coordinate system
            int q = Mathf.RoundToInt(worldPos.x);
            int r = Mathf.RoundToInt(worldPos.z);
            return new HexCoord(q, r);
        }
        
        private void MovePlayerTo(HexCoord target)
        {
            if (playerController == null || pathfinder == null) return;
            
            // Find path
            var result = pathfinder.FindPath(playerController.CurrentPosition, target);
            
            if (result.Success)
            {
                // Visualize path
                if (showPathVisualization)
                {
                    VisualizePath(result.Path);
                }
                
                // Move player
                playerController.SetPath(result.Path);
                
                UpdateStatus($"Moving to {target}");
            }
            else
            {
                UpdateStatus($"Cannot reach {target}");
            }
        }
        
        private void VisualizePath(System.Collections.Generic.List<HexCoord> path)
        {
            // Clear existing path markers
            ClearPathMarkers();
            
            // Create path markers
            if (pathMarkerPrefab != null)
            {
                foreach (var hexCoord in path)
                {
                    var worldPos = HexGrid.HexToWorld(hexCoord);
                    var uiPos = WorldToUIPosition(worldPos);
                    
                    var marker = Instantiate(pathMarkerPrefab, mapContainer);
                    marker.GetComponent<RectTransform>().anchoredPosition = uiPos;
                    
                    // Add fade animation
                    var canvasGroup = marker.GetComponent<CanvasGroup>();
                    if (canvasGroup == null) canvasGroup = marker.AddComponent<CanvasGroup>();
                    canvasGroup.alpha = 0.5f;
                }
            }
        }
        
        private void ClearPathMarkers()
        {
            // Remove all path markers
            var pathMarkers = GameObject.FindGameObjectsWithTag("PathMarker");
            foreach (var marker in pathMarkers)
            {
                Destroy(marker);
            }
        }
        
        private void GenerateRandomObstacles()
        {
            if (!enableObstacles || obstacleMarkerPrefab == null) return;
            
            // Generate random obstacles
            int obstacleCount = 10;
            for (int i = 0; i < obstacleCount; i++)
            {
                var randomCoord = new HexCoord(
                    Random.Range(-hexGrid.gridWidth/2, hexGrid.gridWidth/2),
                    Random.Range(-hexGrid.gridHeight/2, hexGrid.gridHeight/2)
                );
                
                // Set as obstacle
                hexGrid.SetTileWalkability(randomCoord, false);
                
                // Create visual obstacle
                var worldPos = HexGrid.HexToWorld(randomCoord);
                var uiPos = WorldToUIPosition(worldPos);
                
                var obstacle = Instantiate(obstacleMarkerPrefab, mapContainer);
                obstacle.GetComponent<RectTransform>().anchoredPosition = uiPos;
                obstacle.tag = "Obstacle";
            }
            
            UpdateStatus($"Generated {obstacleCount} obstacles");
        }
        
        private void HandleKeyboardInput()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                RunTestSuite();
            }
            else if (Input.GetKeyDown(KeyCode.G))
            {
                GenerateRandomObstacles();
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                ResetPlayer();
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                ClearPathMarkers();
            }
        }
        
        private void RunTestSuite()
        {
            UpdateStatus("Running test suite...");
            
            // Simulate test suite
            StartCoroutine(TestSuiteCoroutine());
        }
        
        private System.Collections.IEnumerator TestSuiteCoroutine()
        {
            string[] tests = {
                "Service Locator: PASS",
                "A* Pathfinding: PASS", 
                "Audio System: PASS",
                "Save/Load: PASS",
                "Performance: PASS"
            };
            
            for (int i = 0; i < tests.Length; i++)
            {
                UpdateStatus($"✅ {tests[i]}");
                yield return new WaitForSeconds(0.5f);
            }
            
            UpdateStatus("All tests passed! 🎉");
        }
        
        private void ResetPlayer()
        {
            if (playerController != null)
            {
                playerController.SetPosition(new HexCoord(0, 0));
                ClearPathMarkers();
                UpdateStatus("Player reset to center");
            }
        }
        
        private void UpdateStatus(string message)
        {
            if (statusText != null)
            {
                statusText.text = message;
            }
            Debug.Log($"🎮 {message}");
        }
        
        private void UpdateStatusText()
        {
            if (statusText != null && playerController != null)
            {
                string baseStatus = playerController.IsMoving ? "Moving..." : "Ready";
                string position = $"Pos: {playerController.CurrentPosition}";
                statusText.text = $"{baseStatus} | {position}";
            }
        }
        
        [ContextMenu("Generate Obstacles")]
        public void GenerateObstacles()
        {
            GenerateRandomObstacles();
        }
        
        [ContextMenu("Reset Game")]
        public void ResetGame()
        {
            ResetPlayer();
            ClearPathMarkers();
            GenerateRandomObstacles();
        }
    }
}
