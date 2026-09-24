using UnityEngine;

// ---------------------------------------------------------------------------
// AI-ASSISTANCE DECLARATION (COS30031 Assessment 2)
// Tool: Claude (Anthropic), 20/09/2026.
// Prompt (summary): "Write a Debris component for a Unity 2D isometric game.
//   Each piece receives one of four PhysicsMaterial2D assets, a colour and a
//   mass from a spawner, is launched with an impulse, then fades out and
//   destroys itself. Comments in English."
// Output: this file. Reviewed, tested and integrated by Nho Anh Khoa Nguyen
//   (105312661). Physics material values and tuning are my own.
// ---------------------------------------------------------------------------

/// <summary>
/// A single piece of building debris. The physical behaviour (bounce, slide)
/// comes entirely from the PhysicsMaterial2D assigned by DebrisSpawner.
/// This script only handles setup, fade-out and cleanup.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(SpriteRenderer))]
public class Debris : MonoBehaviour
{
    [Tooltip("Total seconds before the piece is destroyed.")]
    [SerializeField] private float lifetime = 4f;

    [Tooltip("Seconds at the end of the lifetime spent fading out.")]
    [SerializeField] private float fadeDuration = 1f;

    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer sr;
    private Color baseColor;
    private float spawnTime;

    /// <summary>Lifetime is exposed so the spawner can keep the floor alive long enough.</summary>
    public float Lifetime => lifetime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();

        // Small, fast pieces can tunnel through a thin floor without these.
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        baseColor = sr.color;
        spawnTime = Time.time;
    }

    /// <summary>
    /// Configures and launches the piece. Called by DebrisSpawner right after Instantiate.
    /// </summary>
    public void Init(PhysicsMaterial2D material, Color color, float mass,
                     float gravityScale, Vector2 impulse, float spin)
    {
        // The collider's material is what the physics engine uses on contact.
        col.sharedMaterial = material;

        rb.mass = mass;
        rb.gravityScale = gravityScale;

        baseColor = color;
        sr.color = color;

        rb.AddForce(impulse, ForceMode2D.Impulse);
        rb.AddTorque(spin, ForceMode2D.Impulse);

        spawnTime = Time.time;
    }

    private void Update()
    {
        float age = Time.time - spawnTime;
        float fadeStart = lifetime - fadeDuration;

        if (age >= fadeStart)
        {
            // Fade alpha from 1 to 0 over the last fadeDuration seconds.
            float t = Mathf.Clamp01((age - fadeStart) / fadeDuration);
            Color c = baseColor;
            c.a = baseColor.a * (1f - t);
            sr.color = c;
        }

        if (age >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}
