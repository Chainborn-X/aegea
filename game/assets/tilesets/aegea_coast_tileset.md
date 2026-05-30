# Aegea Coast Tileset

Source vision: `assets/vision-art/aegea-vision-coast-cave-collage.png`

Runtime texture: `res://assets/tilesets/aegea_coast_tileset.png`

Concept sheet: `res://assets/tilesets/aegea_coast_tileset_concept.png`

Grid: 8 columns by 8 rows, 32px square tiles.

## MVP Coast Chunk Tileset

Runtime texture: `res://assets/tilesets/aegea_coast_chunk_tileset.png`

Visual target: `res://assets/tilesets/aegea_coast_scene_target.png`

Sample scene: `res://assets/tilesets/aegea_coast_chunk_sample_scene.png`

Grid: 12 columns by 8 rows, 128px square chunks.

This is the active MVP scene tileset. It intentionally uses larger painterly chunks instead of tiny repeated stamps, because the coast vision depends on organic shoreline silhouettes, continuous light/shadow, dense vegetation, and broken ruin shapes that do not survive a 32px-only pass.

## Rows

- Row 0: sand base variations.
- Row 1: grass/sand blends and worn path surfaces.
- Row 2: shallow and deep water.
- Row 3: shoreline transitions with surf foam.
- Row 4: salt-worn stone, broken ruin floor, and overgrown paving.
- Row 5: cliff, rock boundary, and boulder tiles.
- Row 6: Mediterranean vegetation and flower clusters.
- Row 7: beach props and discovery markers.

The older 32px atlas remains as an experimental micro-tile pass. The MVP scene builds a Godot 4 `TileSetAtlasSource` from the 128px chunk atlas in `OutdoorArea`.
