// -----------------------------------------------------------------------------
// AI assistance acknowledgement (COS30031 Unit Outline requirement)
// Tool: Claude (Anthropic)
// Prompt: "Write a reusable Unity 2D surface zone system for an isometric city
//          game. One script should handle five surface behaviours - normal, fast,
//          slow, slippery and unstable push-back - configured from the Inspector,
//          without the zone needing to know which movement script the player uses."
// Output: initial structure of this file. Reviewed, tested and modified by the
//         author (Nho Anh Khoa Nguyen, 105312661).
// -----------------------------------------------------------------------------

using UnityEngine;

/// <summary>
/// The ground behaviours available in the game. Each maps to a preset in
/// <see cref="SurfaceZone.PresetFor"/>.
/// </summary>
public enum SurfaceType
{
    Normal,     // Grass - baseline movement, nothing modified.
    Fast,       // Road - player moves noticeably quicker.
    Slow,       // Ocean / deep water - player is held back.
    Slippery,   // Wet road - player keeps sliding after releasing input.
    Unstable    // Broken road - shoves the player back when they step on it.
}

/// <summary>
/// The movement modifiers a surface applies. Kept as a plain serializable struct
/// so designers can override a preset per zone straight from the Inspector.
/// </summary>
[System.Serializable]
public struct SurfaceSettings
{
    [Tooltip("Multiplies the player's base move speed. 1 = unchanged.")]
    public float speedMultiplier;

    [Tooltip("How quickly the player reaches the speed their input asks for. " +
             "1 = instant response, lower values feel slippery.")]
    [Range(0.02f, 1f)]
    public float control;

    /// <summary>Neutral settings, used when the player is not inside any zone.</summary>
    public static SurfaceSettings Default =>
        new SurfaceSettings { speedMultiplier = 1f, control = 1f };
}

/// <summary>
/// Marks a trigger collider as a piece of ground with a particular behaviour.
/// Attach to a Tilemap (with a TilemapCollider2D set to Is Trigger) or to an
/// individual ground object.
///
/// The zone never touches the movement script directly. It pushes its settings
/// into the <see cref="SurfaceReceiver"/> on whatever entered it, which means the
/// same zone works with any movement implementation that reads a SurfaceReceiver.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class SurfaceZone : MonoBehaviour
{
    [Header("Behaviour")]
    [SerializeField] private SurfaceType surfaceType = SurfaceType.Normal;

    [Tooltip("Tick to ignore the preset and use the values below instead. " +
             "Useful for tuning one specific area without changing every zone.")]
    [SerializeField] private bool overridePreset = false;
    [SerializeField] private SurfaceSettings customSettings = new SurfaceSettings
    {
        speedMultiplier = 1f,
        control = 1f
    };

    [Header("Unstable surfaces only")]
    [Tooltip("Impulse applied once when the player steps onto an Unstable surface.")]
    [SerializeField] private float pushBackForce = 7f;

    /// <summary>The settings this zone applies while the player is inside it.</summary>
    public SurfaceSettings Settings => overridePreset ? customSettings : PresetFor(surfaceType);

    public SurfaceType Type => surfaceType;

    /// <summary>
    /// The tuned values behind each surface type. Kept in one place so the whole
    /// game stays consistent - changing "road" here changes every road in every level.
    /// </summary>
    private static SurfaceSettings PresetFor(SurfaceType type)
    {
        switch (type)
        {
            // Road: a clear speed boost, still fully responsive.
            case SurfaceType.Fast:
                return new SurfaceSettings { speedMultiplier = 1.45f, control = 1f };

            // Water: heavy resistance. Slow but the player keeps full control,
            // so wading through never feels like the controls broke.
            case SurfaceType.Slow:
                return new SurfaceSettings { speedMultiplier = 0.5f, control = 0.8f };

            // Wet road: normal top speed, but low control means the player
            // accelerates and stops gradually - they slide past their target.
            case SurfaceType.Slippery:
                return new SurfaceSettings { speedMultiplier = 1.1f, control = 0.08f };

            // Broken road: slightly slower, plus a one-off shove handled on entry.
            case SurfaceType.Unstable:
                return new SurfaceSettings { speedMultiplier = 0.85f, control = 0.7f };

            // Grass and anything unhandled.
            case SurfaceType.Normal:
            default:
                return SurfaceSettings.Default;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        SurfaceReceiver receiver = other.GetComponent<SurfaceReceiver>();
        if (receiver == null) return;

        receiver.EnterZone(this);

        if (surfaceType == SurfaceType.Unstable)
        {
            receiver.ApplyImpulse(PushBackDirection(other) * pushBackForce);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        SurfaceReceiver receiver = other.GetComponent<SurfaceReceiver>();
        if (receiver == null) return;

        receiver.ExitZone(this);
    }

    /// <summary>
    /// Works out which way to shove the player. Pushing back along their own
    /// direction of travel reads as "that surface rejected me". If they are not
    /// moving, fall back to pushing them away from the centre of the zone so the
    /// impulse is never zero.
    /// </summary>
    private Vector2 PushBackDirection(Collider2D other)
    {
        Rigidbody2D body = other.attachedRigidbody;

        if (body != null && body.linearVelocity.sqrMagnitude > 0.01f)
        {
            return -body.linearVelocity.normalized;
        }

        Vector2 awayFromCentre = (Vector2)other.transform.position - (Vector2)transform.position;
        return awayFromCentre.sqrMagnitude > 0.01f ? awayFromCentre.normalized : Vector2.up;
    }

    /// <summary>
    /// Editor-only guard. A surface that is not a trigger would physically block
    /// the player instead of modifying them, which is a confusing failure to debug.
    /// </summary>
    private void OnValidate()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning(
                $"SurfaceZone on '{name}' has a collider that is not set to Is Trigger. " +
                "Surface zones must be triggers or they will block movement.", this);
        }
    }
}
