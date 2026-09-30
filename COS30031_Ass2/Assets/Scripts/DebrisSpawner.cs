using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// ---------------------------------------------------------------------------
// AI-ASSISTANCE DECLARATION (COS30031 Assessment 2)
// Tool: Claude (Anthropic), 20/09/2026.
// Prompt (summary): "Write a reusable DebrisSpawner for a Unity 2D isometric
//   game. Explode(origin) spawns 8-12 debris pieces spread evenly across four
//   PhysicsMaterial2D types, creates a temporary invisible floor on a
//   DebrisFloor layer so bounce and friction are visible without global
//   gravity, and pushes the player away through SurfaceReceiver.ApplyImpulse.
//   Include a temporary test key. Comments in English."
// Output: this file. Reviewed, tested and integrated by Nho Anh Khoa Nguyen
//   (105312661). Layer design and tuning values are my own.
// ---------------------------------------------------------------------------

/// <summary>
/// Spawns physics-driven debris when a building is demolished.
/// Public entry point: Explode(Vector2 origin). The demolish system calls this;
/// nothing else in the game needs to know how debris works.
/// </summary>
public class DebrisSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct DebrisType
    {
        [Tooltip("Label for the Inspector only, e.g. Concrete.")]
        public string label;
        public PhysicsMaterial2D material;
        public Color color;
        [Min(0.01f)] public float mass;
    }

    [Header("Debris")]
    [SerializeField] private Debris debrisPrefab;
    [Tooltip("One entry per physics material. Pieces are spread evenly across these.")]
    [SerializeField] private DebrisType[] debrisTypes;
    [SerializeField, Min(1)] private int minPieces = 8;
    [SerializeField, Min(1)] private int maxPieces = 12;
    [Tooltip("Random offset from the origin so pieces do not all start on the same point.")]
    [SerializeField] private float spawnJitter = 0.2f;

    [Header("Launch")]
    [SerializeField] private float minLaunchSpeed = 3f;
    [SerializeField] private float maxLaunchSpeed = 6f;
    [Tooltip("Minimum upward component of the launch direction (0 = any angle, 1 = straight up).")]
    [SerializeField, Range(0f, 1f)] private float upwardBias = 0.4f;
    [SerializeField] private float maxSpin = 1.5f;
    [Tooltip("Local gravity for debris only. The rest of the game stays gravity-free.")]
    [SerializeField] private float gravityScale = 1.5f;

    [Header("Temporary floor")]
    [Tooltip("Must exist in Tags and Layers and collide with Debris only.")]
    [SerializeField] private string floorLayerName = "DebrisFloor";
    [Tooltip("Distance below the origin where debris lands (the base of the building).")]
    [SerializeField] private float floorOffset = 0.5f;
    [SerializeField] private float floorWidth = 6f;
    [Tooltip("A thin floor can be tunnelled through by fast pieces, so keep this well above the debris size.")]
    [SerializeField] private float floorThickness = 4f;

    [Header("Player push")]
    [SerializeField] private float blastRadius = 2.5f;
    [SerializeField] private float blastForce = 6f;
    [SerializeField] private LayerMask playerMask;

    [Header("Testing (turn off before release)")]
    [SerializeField] private bool enableTestKey = true;

    private void Update()
    {
        if (!enableTestKey) return;

#if ENABLE_INPUT_SYSTEM
        bool pressed = Keyboard.current != null && Keyboard.current.bKey.wasPressedThisFrame;
#else
        bool pressed = Input.GetKeyDown(KeyCode.B);
#endif
        if (pressed)
        {
            Explode(transform.position);
        }
    }

    /// <summary>
    /// Spawns a burst of debris at the given world position.
    /// </summary>
    public void Explode(Vector2 origin)
    {
        if (debrisPrefab == null || debrisTypes == null || debrisTypes.Length == 0)
        {
            Debug.LogWarning($"{name}: DebrisSpawner is missing a prefab or debris types.", this);
            return;
        }

        float debrisLifetime = debrisPrefab.Lifetime;
        CreateFloor(origin, debrisLifetime + 0.5f);

        int count = Random.Range(minPieces, maxPieces + 1);
        for (int i = 0; i < count; i++)
        {
            // Round-robin so every material appears in every explosion.
            DebrisType type = debrisTypes[i % debrisTypes.Length];

            Vector2 pos = origin + Random.insideUnitCircle * spawnJitter;
            Quaternion rot = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
            Debris piece = Instantiate(debrisPrefab, pos, rot);

            Vector2 dir = new Vector2(Random.Range(-1f, 1f), Random.Range(upwardBias, 1f)).normalized;
            float speed = Random.Range(minLaunchSpeed, maxLaunchSpeed);

            // Impulse scaled by mass gives every piece the same launch speed range,
            // so differences on landing come from the physics material, not from mass.
            Vector2 impulse = dir * speed * type.mass;
            float spin = Random.Range(-maxSpin, maxSpin) * type.mass;

            piece.Init(type.material, type.color, type.mass, gravityScale, impulse, spin);
        }

        PushPlayer(origin);
    }

    /// <summary>
    /// Creates an invisible, short-lived floor that only debris can hit.
    /// </summary>
    private void CreateFloor(Vector2 origin, float lifetime)
    {
        int layer = LayerMask.NameToLayer(floorLayerName);
        if (layer < 0)
        {
            Debug.LogWarning($"{name}: layer '{floorLayerName}' does not exist. Debris will fall forever.", this);
            return;
        }

        GameObject floor = new GameObject("DebrisFloor (temp)");
        floor.layer = layer;

        // The landing surface is the TOP face of the box, so the box centre sits
        // half a thickness lower. A thick box cannot be tunnelled through.
        float surfaceY = origin.y - floorOffset;
        floor.transform.position = new Vector2(origin.x, surfaceY - floorThickness * 0.5f);

        BoxCollider2D box = floor.AddComponent<BoxCollider2D>();
        box.size = new Vector2(floorWidth, floorThickness);

        Debug.Log($"Debris floor created at y={surfaceY:F2}, debris spawned at y={origin.y:F2}");

        Destroy(floor, lifetime);
    }

    /// <summary>
    /// Pushes the player away from the blast, reusing the SurfaceReceiver impulse API.
    /// </summary>
    private void PushPlayer(Vector2 origin)
    {
        Collider2D hit = Physics2D.OverlapCircle(origin, blastRadius, playerMask);
        if (hit == null) return;
        if (!hit.TryGetComponent(out SurfaceReceiver receiver)) return;

        Vector2 away = (Vector2)hit.transform.position - origin;
        float distance = away.magnitude;
        if (distance < 0.01f) away = Vector2.up;

        // Stronger push the closer the player is to the centre.
        float falloff = 1f - Mathf.Clamp01(distance / blastRadius);
        receiver.ApplyImpulse(away.normalized * blastForce * falloff);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 origin = transform.position;

        Gizmos.color = new Color(1f, 0.5f, 0f, 0.6f);
        Gizmos.DrawWireSphere(origin, blastRadius);

        Gizmos.color = Color.cyan;
        Vector3 floorCentre = origin + Vector3.down * floorOffset;
        Vector3 half = Vector3.right * floorWidth * 0.5f;
        Gizmos.DrawLine(floorCentre - half, floorCentre + half);
    }
}
