# Aegea Playable MVP

This folder contains the first playable Godot 4 C# vertical slice for Aegea.

The slice is a small outdoor shrine-coast area that proves the core loop: explore, run, cut overgrowth, collect items, talk to a mysterious object, find the Old Shrine Key, fight corrupted guardians, and survive long enough to inspect the sealed path north.

## Run

1. Open `game/project.godot` in Godot 4.4 Mono or another Godot 4.x Mono build.
2. Press Play.

Command line:

```sh
cd game
/Applications/Godot_mono.app/Contents/MacOS/Godot --path .
```

## Controls

- WASD or arrow keys: move
- Shift: run
- Space or left mouse: sword attack
- E: interact / advance dialogue
- I: inventory
- Esc: pause
- F5: save
- F9: load

The inventory panel includes a `Use Sunleaf Herb` button for the MVP healing item.

## Project Structure

- `scenes/Main.tscn`: playable entry scene.
- `scripts/GameManager.cs`: world bootstrap, input registration, pause/save/load routing.
- `scripts/Entities`: player, enemies, and destructible plants.
- `scripts/Combat`: reusable health/damage and attack hitbox logic.
- `scripts/Interaction`: interactable interface, dialogue, chests, and shrine gate.
- `scripts/Inventory`: item definitions, inventory state, equipment slot stub, pickups.
- `scripts/Quests`: objective tracker.
- `scripts/Save`: JSON save/load stub for player state, inventory, and objectives.
- `scripts/UI`: health/stamina, inventory, notifications, prompts, dialogue, pause/settings stub.
- `scripts/World`: procedural placeholder outdoor area and collision geometry.

## Architecture Notes

Gameplay state is split into small Godot nodes rather than a single player script. `Damageable` is shared by the player, enemies, and cuttable plants. `AttackHitbox` scans for `Damageable` nodes and applies damage/knockback based on team ownership. `InventoryComponent`, `QuestTracker`, and `SaveGameService` keep player/world state explicit so the same data can later be persisted or synchronized outside the client.

The current art is procedural placeholder drawing in `_Draw()` methods. This keeps the prototype version-control friendly and easy to run while leaving clear replacement points for sprite scenes, animations, tilesets, particles, and audio.

## Complete

- Playable Godot 4 C# project and first scene.
- 8-direction player movement with walking, running, acceleration, stamina, facing, and camera follow.
- Sword attack with cooldown, active hitbox timing, enemy damage, plant cutting, and feedback.
- Player health, damage reactions, knockback, temporary invulnerability, death, and respawn.
- Enemy health, knockback, simple patrol/chase/contact attack/reset behavior.
- Cuttable grass and bushes with pickup drops.
- Pickups, basic inventory data model, equipped weapon slot stub, key item, material items, and healing item.
- Interaction prompt and reusable `IInteractable` pattern for dialogue objects, signs, chests, and gates.
- Original objective: find the Old Shrine Key.
- Dialogue/text panel with multiple lines.
- Health/stamina HUD, pickup notifications, inventory panel, pause menu, and settings stub.
- Save/load stub for position, health, inventory, and objective state.

## Known Limitations

- Placeholder visuals are procedural shapes, not final sprites or animation assets.
- No audio, music, authored tilemap asset, or final collision tileset yet.
- Save data does not persist destroyed plants, opened chests, defeated enemies, or gate-open state.
- Inventory item use is UI-button based; no hotbar or controller support yet.
- Enemy AI is intentionally simple and local-only.
- No networking, account, blockchain, or server-authoritative state.

## Suggested Next Milestones

1. Replace procedural placeholders with real sprite scenes, animation players, hit sparks, and cut-grass particles.
2. Move the outdoor area to an authored TileMap/TileSet with collision layers and navigation regions.
3. Persist world-state changes such as opened chests, cut plants, defeated enemies, and gate state.
4. Add tuned combat timings, enemy telegraphs, audio feedback, and richer weapon data resources.
5. Add item hotkeys, equipment UI, and data-driven item/resource definitions.
6. Add a second connected area behind the gate and a small shrine interior objective.
