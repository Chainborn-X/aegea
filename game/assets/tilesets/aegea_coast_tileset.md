# Aegea Coast Tileset

Source vision: `assets/vision-art/aegea-vision-coast-cave-collage.png`

Runtime texture: `res://assets/tilesets/aegea_coast_tileset.png`

Concept sheet: `res://assets/tilesets/aegea_coast_tileset_concept.png`

Grid: 8 columns by 8 rows, 32px square tiles.

## Rows

- Row 0: sand base variations.
- Row 1: grass/sand blends and worn path surfaces.
- Row 2: shallow and deep water.
- Row 3: shoreline transitions with surf foam.
- Row 4: salt-worn stone, broken ruin floor, and overgrown paving.
- Row 5: cliff, rock boundary, and boulder tiles.
- Row 6: Mediterranean vegetation and flower clusters.
- Row 7: beach props and discovery markers.

The MVP scene builds a Godot 4 `TileSetAtlasSource` from this atlas in `OutdoorArea` and paints separate `TileMapLayer`s for ground and details.
