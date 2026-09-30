# Physics and Collisions

Author: Nho Anh Khoa Nguyen (105312661)
Branches: `feature/khoa/physics`, `feature/khoa/debris` — merged into `main` via pull request #4.

This document covers the physics side of the prototype: surface materials, collision
filtering, map boundaries and the demolition debris system.

---

## 1. Physics materials

Four `PhysicsMaterial2D` assets live in `Assets/Physics Materials`. They are not
decoration: each one is attached to debris produced when a building is demolished,
so the difference between them is visible during play.

| Material | Friction | Bounciness | Behaviour on landing |
|---|---|---|---|
| Concrete | _(fill in)_ | _(fill in)_ | Drops and stops almost immediately |
| Glass | _(fill in)_ | _(fill in)_ | Slides the furthest across the ground |
| Wood | _(fill in)_ | _(fill in)_ | Bounces a few times, then rolls |
| Metal | _(fill in)_ | _(fill in)_ | Bounces highest and settles last |

> Fill the two numeric columns from the Inspector values of each asset before submitting.

Every debris piece is launched with the same speed range regardless of which material
it uses, because the launch impulse is multiplied by the piece's mass. Differences seen
on landing therefore come from the material, not from the mass — a controlled comparison
rather than four things changing at once.

## 2. Collision layers

Layers used: `Player`, `Building`, `Debris`, `Surface`, `Boundary`, `DebrisFloor`.

The Layer Collision Matrix is configured deliberately rather than left at the default
"everything hits everything". Notable choices:

- `Surface` only interacts with `Player`. Surface zones are triggers that read the
  player's position; they must not block debris or buildings.
- `Debris` collides with `DebrisFloor` and `Boundary`, but not with `Surface`. Debris
  should land and stay inside the map without being slowed down by water zones.
- `DebrisFloor` collides with `Debris` alone. It is an internal helper and must never
  block the player.
- `Debris` does not collide with other debris, which keeps a burst cheap and stops
  pieces from jamming against each other.

Turning pairs off is as much a design decision as turning them on: each disabled pair
is a collision test the engine no longer runs every frame.

## 3. Surface zones

`SurfaceZone` sits on each tile group (`Ocean`, `Land`, `Dirt`, `Shore`, `Roads`,
`Broken Roads`). `SurfaceReceiver` on the player reads the zone it is standing in and
applies a speed multiplier and a control value used by `PlayerMovement`.

Surface types in use:

| Zone | Type | Intent |
|---|---|---|
| Ocean | Slow | Wading through water |
| Shore | Normal (overridden) | Slightly slower than land, a transition |
| Land / Dirt | Normal | Baseline speed |
| Roads | Fast | Roads are the quick way across the city |
| Broken Roads (level 3) | Slippery | Cracked surface, the player keeps sliding |

Each group is built the same way: a static `Rigidbody2D` on the parent, a
`CompositeCollider2D` set to **Is Trigger** with geometry type **Polygons**, and a
`PolygonCollider2D` on every child tile set to **Merge**. The composite fuses dozens of
tile colliders into one shape, so the engine tests one region instead of one collider
per tile.

Geometry type matters here. `Outlines` produces edge colliders, which only register at
the border of the region; `Polygons` produces filled shapes, which is what a trigger
covering an area needs.

## 4. Map boundaries

Each level has a `MapBounds` object on the `Boundary` layer carrying a closed
`EdgeCollider2D` traced around the island. An edge collider is a better fit than a
polygon collider because the boundary is a line the player must not cross, not an area
to fill, and each island has a different outline.

## 5. Debris system

`DebrisSpawner.ExplodeAt(position, buildingType)` is the public entry point.
`Building.Demolish()` calls it in one line before destroying the building, so the
demolition flow does not need to know anything about how debris works.

What happens on an explosion:

1. A temporary floor is created on the `DebrisFloor` layer, below the building.
2. 8–12 pieces are spawned, cycling through the materials that suit the building type.
3. Each piece is launched with a random direction, speed and spin, given local gravity,
   and fades out and destroys itself after a few seconds.
4. A player standing close to the blast is pushed away through the same
   `SurfaceReceiver.ApplyImpulse` used by the surface system.

**Why a temporary floor.** The game is top-down: global gravity is unused and the player
never falls. Without something to land on, friction and bounciness would never be
exercised and the four materials would look identical. Giving debris its own gravity and
its own private floor produces a visible arc and landing while leaving the rest of the
game's physics untouched. The floor lives on its own layer so it cannot block anything
else, and it is destroyed with the debris.

**Materials per building.** A warehouse is a metal shed and a park is timber and stone,
so each building type breaks into materials that suit it:

| Building | Debris |
|---|---|
| Apartment | Concrete, Glass, Wood |
| Office | Concrete, Glass, Metal |
| Warehouse | Metal, Concrete |
| Cafe | Wood, Glass, Metal |
| Library | Concrete, Wood, Glass |
| Park | Wood, Concrete |

An unknown building type falls back to all four materials, so the system cannot break
if a new building is added later.

## 6. Problems solved along the way

| Problem | Cause | Fix |
|---|---|---|
| Tiles flew apart on Play | Every child tile had its own `Rigidbody2D`, so each was an independent body pushing against its overlapping neighbours | Removed child rigidbodies; only the parent group has one, set to Static |
| Composite collider covered nothing (`Shape Count = 0`) | A child collider only feeds a composite if it has no rigidbody of its own | Same fix — the parent's rigidbody is the one that matters |
| Invisible wall near the centre plots | A stray `PolygonCollider2D` had been added to the `Shore` parent, which has no sprite, so Unity generated a small default shape in the middle of the map | Removed the collider from the parent; only children carry polygon colliders |
| Water only slowed the player at the edge | Composite geometry type was `Outlines` | Changed to `Polygons` |
| Debris invisible although the objects existed | Colours picked in the Inspector kept the default alpha of 0 | Alpha is now forced to 1 in `Debris.Init`, so a mis-set colour cannot hide a piece |
| Debris fell through its floor | The floor was 0.2 units thick and the pieces were fast | Floor is now a thick box whose top face is the landing surface |

## 7. Files

| File | Role |
|---|---|
| `Assets/Scripts/SurfaceZone.cs` | Marks a tile group as a surface with a speed profile |
| `Assets/Scripts/SurfaceReceiver.cs` | On the player; applies the current surface and external impulses |
| `Assets/Scripts/Debris.cs` | One debris piece: setup, fade, cleanup |
| `Assets/Scripts/DebrisSpawner.cs` | Spawns bursts, builds the temporary floor, pushes the player |
| `Assets/Physics Materials/*.physicsMaterial2D` | The four materials |

## 8. AI assistance

`Debris.cs` and `DebrisSpawner.cs` were drafted with Claude (Anthropic) and carry a
declaration comment at the top of each file recording the prompt and the date. All
tuning values, the layer design and the integration with the demolition flow are my own,
and every generated file was reviewed and tested before being committed.
One line was added to `Building.cs` (originally written by Ben) to trigger debris on
demolition; nothing else in that file was changed.
