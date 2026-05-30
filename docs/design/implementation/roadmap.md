# Implementation Roadmap

This roadmap translates the Echoes of Olympus narrative foundation into production milestones. It is intentionally ordered around playable proof, reusable systems, and a strong vertical slice before full content expansion.

## Development Strategy

Build the game through a small, polished slice first:

1. prove the feel of top-down movement and combat
2. prove exploration and secrets
3. prove relic-based traversal
4. prove Echo memory scenes
5. prove region-to-dungeon-to-return-loop structure
6. prove one side story with persistent consequences

Do not begin full world production until the vertical slice demonstrates the emotional and mechanical promise.

## Phase 0 - Foundation Audit

Goal: Align repository docs, Godot prototype, and production assumptions.

Deliverables:

- Confirm engine version and project structure.
- Confirm target screen resolution, tile size, and camera behavior.
- Confirm save/state model requirements.
- Confirm content authoring format for dialogue, scenes, items, NPCs, and flags.
- Confirm whether narrative content lives in data files, Godot resources, or C# definitions.

Exit criteria:

- A developer can explain where to add a room, NPC, item, scene trigger, and save flag.

## Phase 1 - Core Playable Feel

Goal: Make basic traversal and combat feel good in an empty test region.

Systems:

- player movement
- collision
- camera follow
- basic sword combat
- enemy hurt/hit states
- health and damage
- interact prompts
- room or scene transitions
- basic inventory state

Exit criteria:

- Player can move, fight, interact, take damage, defeat enemies, and transition between test rooms.

## Phase 2 - Exploration and State

Goal: Make discovery persistent and readable.

Systems:

- save/load
- world flags
- opened chests
- defeated mini bosses
- hidden passage reveals
- map markers or sketches
- simple NPC state changes
- basic journal or memory log

Exit criteria:

- A discovered secret remains discovered.
- An NPC can react to a completed local event.
- A map clue can lead to a hidden route without explicit objective arrows.

## Phase 3 - Relic Framework

Goal: Build a reusable ability framework before authoring many relics.

Systems:

- equip or quick-use relics
- relic energy if needed
- environment target detection
- ability gates
- puzzle interactables
- combat effects

First relic implementation:

- **Ember of Hestia**
  - light braziers
  - reveal memory overlays
  - burn root barriers
  - damage or weaken thorn enemies

Exit criteria:

- A single relic affects combat, traversal, puzzle solving, and narrative discovery.

## Phase 4 - Dialogue and Echo Scenes

Goal: Make memory discovery a core system, not a cutscene exception.

Systems:

- dialogue lines
- speaker portraits or names
- branching choices if needed
- trigger conditions
- Echo memory overlays
- inscription reads
- rumor text
- flag changes after viewing memories

Exit criteria:

- A player can discover an inscription, trigger an Echo scene, gain a memory flag, and unlock an NPC response.

## Phase 5 - Vertical Slice

Goal: Ship the first production-quality slice.

Scope:

- Elia village
- Helikon Vale opening route
- Hearthwood Shrine dungeon
- Ember of Hestia relic
- Ashen Keeper boss
- one mini side story
- one hidden passage chain
- one myth fragment chain
- one post-dungeon return moment

Exit criteria:

- The slice can be played from new game through first dungeon completion.
- The player experiences home, mystery, combat, relic discovery, memory reveal, boss victory, and world recontextualization.
- The slice can be handed to testers with no designer explanation.

## Phase 6 - Content Production Pipeline

Goal: Turn the slice process into repeatable region production.

Deliverables:

- region template
- dungeon template
- scenario template
- scene template
- NPC template
- item template
- enemy template
- QA checklist

Exit criteria:

- A new region can be planned and implemented using established patterns.

## Phase 7 - First World Expansion

Goal: Expand from vertical slice to first act.

Content:

- full Helikon Vale
- Pyrgos Harbor
- first sailing prototype
- Temple of the Drowned Trident
- Phaon companion arc start
- Lysandra first encounter

Exit criteria:

- Act I is playable in rough form.
- The player can see the wider world promise.

## Phase 8 - Midgame Systems

Goal: Support multi-region progression and nonlinear exploration.

Systems:

- sailing map
- region map annotations
- language progression
- myth fragment sets
- companion reactions
- side story consequences
- multi-relic puzzle combinations

Exit criteria:

- At least two midgame regions can be completed in flexible order without breaking narrative continuity.

## Phase 9 - Late Game and Ending Framework

Goal: Support final state, world changes, and ending paths.

Systems:

- world transformation after Gate of Echoes
- ending alignment flags
- Primordial sanctum tracking
- companion arc resolution
- final dungeon state checks
- epilogue variation

Exit criteria:

- The ending framework can represent restore old order, create new order, end age of gods, and secret Eighth Echo.

## First Production Backlog

Recommended first tickets:

- Define `GameFlag` naming conventions.
- Implement interactable base component.
- Implement dialogue data schema.
- Implement Echo memory trigger.
- Implement Hestia brazier interactable.
- Implement root barrier interactable.
- Implement basic enemy AI for Thorn Wight.
- Implement Elia test map.
- Implement Hearthwood Shrine greybox.
- Implement Ashen Keeper prototype.
- Implement save/load for flags and inventory.

