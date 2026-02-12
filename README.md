# Unity Man-at-Arms

## 🎮 WebGL Demo - Play Now!

**Live Demo:** https://sloppymo.github.io/man-at-arms-unity/

### 🚀 About This Project
Unity Man-at-Arms is a fully refactored RPG showcasing advanced Unity development techniques and modern architecture patterns.

### ✨ Key Features
- **🗺️ Advanced Pathfinding**: A* algorithm implementation on hexagonal grids
- **🎵 Audio System**: Complete audio management with Yarn Spinner integration
- **💾 Save/Load System**: Robust game state persistence using Newtonsoft.Json
- **🔧 Service Locator**: Modern dependency injection pattern replacing singletons
- **📊 Performance Monitoring**: Real-time performance tracking and optimization
- **🧪 Testing Suite**: Comprehensive unit and integration tests

### 🎯 Interactive Demo
The WebGL demo demonstrates:
- **Click-to-move** hex grid navigation
- **A* pathfinding** with obstacle avoidance
- **Smooth animations** and visual feedback
- **Interactive controls** and testing features

### 🎮 Controls
- **Mouse Click** - Move player to clicked position
- **T** - Run complete test suite
- **G** - Generate random obstacles
- **R** - Reset player to center
- **C** - Clear current movement
- **Space** - Show debug information

### 🛠️ Technical Architecture
- **Unity 2022.3** with Universal Render Pipeline (URP)
- **Service Locator Pattern** for dependency management
- **Event-Driven System** with memory optimization
- **Newtonsoft.Json** for complex serialization
- **A* Pathfinding** optimized for hexagonal grids
- **AudioMixer Integration** for professional sound management

### 📁 Project Structure
```
Assets/Scripts/
├── Audio/          # AudioManager, SoundDatabase
├── Core/            # GameBootstrap, ServiceLocator, GameLogger
├── Narrative/       # Yarn integration, dialogue systems
├── Overworld/       # HexGrid, Pathfinding, Player movement
├── Systems/         # Save/Load, Performance profiler
├── Tests/           # Unit and integration tests
└── Tools/           # Debug and testing utilities
```

### 🚀 Deployment
This game is deployed to GitHub Pages using WebGL build technology, providing instant browser-based gameplay without downloads.

### 📱 Browser Compatibility
- ✅ Chrome 80+
- ✅ Firefox 75+
- ✅ Safari 13+
- ✅ Edge 80+

### 🎯 Development Highlights
- **Complete refactor** from singleton to Service Locator architecture
- **Memory optimization** with weak references and event cleanup
- **Performance profiling** with real-time monitoring
- **Comprehensive testing** with automated test suites
- **Professional audio system** with mixer integration
- **Advanced pathfinding** with caching and optimization

---

**Made with ❤️ using Unity | Deployed on GitHub Pages** 🚀

### 🔗 Links
- **Play Now**: https://sloppymo.github.io/man-at-arms-unity/
- **Source Code**: https://github.com/sloppymo/man-at-arms-unity/
- **Documentation**: Complete setup guides and API documentation
