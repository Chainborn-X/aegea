# Player Traveler 4-Direction Sheet

Runtime sheet: `res://assets/characters/player_traveler_4dir_sheet.png`

Concept/source sheet: `res://assets/characters/player_traveler_4dir_concept.png`

SpriteFrames resource: `res://assets/characters/player_traveler_4dir_frames.tres`

Grid: 8 columns by 4 rows, 192px by 256px frames.

## Rows

- Row 0: down-facing frames.
- Row 1: right-facing frames.
- Row 2: left-facing frames.
- Row 3: up-facing frames.

## Columns

- Columns 0-3: idle/walk/run frames.
- Columns 4-5: sword attack frames.
- Column 6: hurt frame.
- Column 7: death frame.

`PlayerSprite` in `Main.tscn` uses the SpriteFrames resource directly, so placement, scale, and animation frames can be adjusted in the editor. `PlayerController` only swaps animations at runtime and keeps sheet slicing as a fallback if the resource is missing.
