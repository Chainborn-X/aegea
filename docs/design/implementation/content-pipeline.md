# Content Pipeline

This file defines how narrative and level content should move from design to implementation.

## Content Unit Hierarchy

Use this hierarchy for planning:

1. **Region**: broad geographic and mythic area.
2. **Location**: town, dungeon, cave, shrine, road, island, or landmark.
3. **Scenario**: playable content unit with an objective, state changes, and rewards.
4. **Scene**: authored narrative moment, dialogue, memory, or cutscene.
5. **Interaction**: object, inscription, door, chest, NPC line, switch, secret, or encounter.

## Recommended Data Categories

Content should eventually be represented in structured data or strongly typed resources:

- regions
- locations
- NPCs
- dialogue
- scenes
- items
- relics
- enemies
- bosses
- quests or side stories
- flags
- map markers
- inscriptions
- myth fragments

## Flag Naming

Flags should be readable, stable, and scoped.

Pattern:

```text
region.location.subject.state
```

Examples:

```text
helikon.elia.star_fallen
helikon.hearthwood.ember_acquired
helikon.elia.widow_bell_returned
thalassion.drowned_trident.ketos_defeated
nysa.white_stag.trial_completed
global.language.river_cant_learned
ending.primordial_sanctums_all_found
```

## Content Authoring Flow

1. Write or update the region brief.
2. Create scenario briefs for the region.
3. Create scene briefs for required narrative beats.
4. Define flags, items, and rewards.
5. Greybox the location.
6. Implement interactions and encounters.
7. Add placeholder dialogue.
8. Playtest for route clarity.
9. Add final writing pass.
10. QA persistence, flags, and re-entry states.

## Region Readiness Checklist

A region is production-ready when it has:

- a mood target
- a story purpose
- a traversal identity
- a primary dungeon
- 2-4 mini dungeons or optional spaces
- a town or settlement unless intentionally wild
- core NPC population
- regional enemies
- wildlife
- one major side story
- at least three secrets
- one myth set
- one late-game return reason

## Dungeon Readiness Checklist

A dungeon is production-ready when it has:

- theme
- backstory
- floor plan or room graph
- relic or reward
- core mechanic
- escalation mechanic
- signature puzzle
- mini boss or pressure event
- final boss
- narrative reveal
- post-dungeon world change

## Scene Readiness Checklist

A scene is production-ready when it has:

- trigger condition
- location
- characters
- player control rules
- emotional purpose
- information revealed
- flags set
- optional variants
- skip/replay policy

## Writing Pass Order

Use staged writing to avoid polishing scenes that may change.

1. **Beat pass**: what happens and why.
2. **Functional pass**: player-facing objectives, reveals, flags.
3. **Voice pass**: character language and tone.
4. **Economy pass**: remove exposition and repeated information.
5. **Continuity pass**: check against world history, region docs, and ending logic.

