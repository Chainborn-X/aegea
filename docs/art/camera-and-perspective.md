# Camera and Perspective

## Perspective

Aegea should use a top-down action RPG perspective with slight side visibility.

Approximate feeling:

- 70% top-down
- 30% side visibility

This is closer to Secret of Mana, Chrono Trigger, CrossCode, and Eastward than to true isometric games.

## Not Isometric

Aegea should not use a strict diamond-grid isometric perspective. The camera does not rotate. The world scrolls as the player walks.

## Camera Movement

The camera follows the player smoothly through a static 2D world. Depth is created through layers, not camera rotation.

Recommended layers:

- Ground tile layer
- Detail/decal layer
- Object/prop layer
- Character and creature layer
- Foreground occlusion layer
- Lighting/fog layer
- Distant parallax layer where appropriate

## Tile-Based Suitability

The vision art is not directly tile-ready. It is mood and direction. Production art should translate the look into reusable terrain tiles, props, cliff modules, water edges, vegetation sets, temple pieces, and lighting rules.
