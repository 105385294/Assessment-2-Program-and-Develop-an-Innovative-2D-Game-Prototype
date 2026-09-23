// -----------------------------------------------------------------------------
// AI assistance acknowledgement (COS30031 Unit Outline requirement)
// Tool: Claude (Anthropic)
// Prompt: "Write the player-side half of a Unity 2D surface system: a component
//          that collects modifiers from overlapping trigger zones and exposes the
//          active values, so the movement script only has to read two properties."
// Output: initial structure of this file. Reviewed, tested and modified by the
//         author (Nho Anh Khoa Nguyen, 105312661).
// -----------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sits on any object that should be affected by ground surfaces - the player
/// now, and potentially NPCs or vehicles later.
///
/// This component is the seam between the surface system and movement. Zones
/// write into it, movement scripts read from it, and neither knows about the
/// other. That means the surface system can be tested with a stand-in player and
/// later connected to the real movement script without either side being rewritten.
/// </summary>
[DisallowMultipleComponent]
public class SurfaceReceiver : MonoBehaviour
{
    [Header("Impulse handling")]
    [Tooltip("How quickly a push-back impulse fades, in units per second. " +
             "Higher values make the shove shorter and snappier.")]
    [SerializeField] private float impulseDecay = 14f;

    // Zones the object is currently standing in. Stored as a list rather than a
    // single value because tilemap edges overlap - the player is often inside two
    // zones for a frame or two. The most recently entered zone wins, which matches
    // what the player sees: the surface they just stepped onto.
    private readonly List<SurfaceZone> activeZones = new List<SurfaceZone>();

    private Vector2 externalVelocity;

    /// <summary>Settings from the surface currently underfoot, or neutral if none.</summary>
    public SurfaceSettings CurrentSurface =>
        activeZones.Count > 0 ? activeZones[activeZones.Count - 1].Settings : SurfaceSettings.Default;

    /// <summary>Velocity from push-backs and other one-off forces, added on top of input.</summary>
    public Vector2 ExternalVelocity => externalVelocity;

    /// <summary>The surface type underfoot. Useful for footstep sounds or particles.</summary>
    public SurfaceType CurrentSurfaceType =>
        activeZones.Count > 0 ? activeZones[activeZones.Count - 1].Type : SurfaceType.Normal;

    public void EnterZone(SurfaceZone zone)
    {
        if (zone == null || activeZones.Contains(zone)) return;
        activeZones.Add(zone);
    }

    public void ExitZone(SurfaceZone zone)
    {
        activeZones.Remove(zone);
    }

    /// <summary>
    /// Adds a one-off velocity that fades out over the next moment. Used for the
    /// broken-road push-back, and reusable for anything else that should knock the
    /// player around - explosions when a building is demolished, for example.
    /// </summary>
    public void ApplyImpulse(Vector2 impulse)
    {
        externalVelocity += impulse;
    }

    private void FixedUpdate()
    {
        // Clear out any zones destroyed while we were standing in them. Without
        // this, demolishing a plot under the player leaves a null entry behind and
        // CurrentSurface throws.
        activeZones.RemoveAll(zone => zone == null);

        if (externalVelocity.sqrMagnitude > 0.0001f)
        {
            externalVelocity = Vector2.MoveTowards(
                externalVelocity, Vector2.zero, impulseDecay * Time.fixedDeltaTime);
        }
        else
        {
            externalVelocity = Vector2.zero;
        }
    }
}
