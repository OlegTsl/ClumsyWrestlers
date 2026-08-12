# ClumsyWrestlers
Unity 3D game
# Unity Project: Arena Brawler (Steam/Mobile)

## Mandatory Architecture Rules (ALWAYS FOLLOW)

### SOLID Principles
- Single Responsibility: One class = one job. One System = one responsibility.
- Open/Closed: New features = new systems. NEVER modify existing systems.
- Liskov Substitution: Use interfaces (IMovable, IAttacker, IPushable).
- Interface Segregation: Small, focused interfaces. No god-interfaces.
- Dependency Inversion: Systems depend on abstractions (interfaces), not concretions.

### System Independence (CRITICAL)
- Systems NEVER call each other directly.
- ALL communication goes through EventBus ONLY.
- Systems don't know about other systems' existence.
- Events are immutable structs.

### Encapsulation
- View-Model pattern for UI (View knows nothing about logic).
- Public fields = properties (never public fields).
- Systems implement IDisposable and unsubscribe from events in Dispose().

### Project-Specific Rules
- Unity Version: 2022.3.52f1
- Architecture: ECS-like (Systems + Context)
- DI Framework: Zenject (constructor injection only, [Inject] only for MonoBehaviours)
- Async: UniTask (NO coroutines, NO Task)
- Asset Management: Addressables + custom AssetManager with reference counting
- Event System: EventBus (publish/subscribe pattern)
- Platforms: Steam (PC) + Mobile (iOS/Android)
- Genre: 3D Arena Brawler (top-down view)
- Core Mechanic: Push opponents off the arena (normal attack + charged attack)
- Input: Keyboard (WASD + attack) / Touch (mobile)

### Performance Rules (NO EXCEPTIONS)
- NO allocations in Update/LateUpdate/FixedUpdate.
- Components = structs (value types, zero GC).
- Events = structs (zero GC).
- Object pooling for spawned objects (projectiles, effects).
- Cache GetComponent, Camera.main, GameObject.Find in Awake/Start.
- NO FindObjectOfType or GameObject.Find in Update.
- NO Resources.Load in production (use Addressables only).
