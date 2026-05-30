# Scene Format

Scenes are authored narrative moments: dialogue, memory visions, cutscenes, short interactions, boss intros, boss defeats, companion conversations, or ending beats.

The goal is to keep scenes buildable, testable, and easy to revise before writing final prose.

## Scene Template

```markdown
# Scene: [Name]

## ID

`region.location.scene_name`

## Type

[Dialogue | Echo memory | Cutscene | Boss intro | Boss defeat | Companion beat | Ending beat]

## Trigger

- Required flags:
- Forbidden flags:
- Required item/relic:
- Location trigger:
- Interaction trigger:

## Characters

- [Character]: [role in scene]

## Player Control

[Full control | limited movement | locked | dialogue choice only]

## Camera and Staging

[Important camera movement, blocking, animation, environmental focus.]

## Purpose

- Narrative:
- Gameplay:
- Emotional:

## Information Revealed

- [Fact, contradiction, clue, or question]

## Beat Outline

1. [Beat]
2. [Beat]
3. [Beat]

## Player Choice

[Choices if any, and whether they affect flags, tone, or ending alignment.]

## Flags Set

- `flag.name`

## Rewards or Unlocks

- [Item, map marker, route, lore entry, NPC state]

## Variants

- [Variant condition]: [change]

## Re-entry Behavior

[Does the scene replay, become a memory entry, change to short dialogue, or disappear?]
```

## Example Scene: Hestia's First Echo

## ID

`helikon.hearthwood.hestia_first_echo`

## Type

Echo memory.

## Trigger

- Required flags: `helikon.hearthwood.central_hearth_lit`.
- Forbidden flags: `helikon.hearthwood.hestia_first_echo_seen`.
- Required item/relic: Ember of Hestia.
- Location trigger: central hearth chamber.
- Interaction trigger: light the final brazier.

## Characters

- Hestia Echo: first divine presence.
- Unknown child: memory figure from the Sundering.

## Player Control

Limited movement. Player can walk inside the memory circle but cannot leave until the Echo fades.

## Camera and Staging

Camera centers on the hearth. The ruined chamber briefly appears whole. Figures sit around a table where the boss arena will later form.

## Purpose

- Narrative: reveal that gods persist as broken memories.
- Gameplay: teach that lighting sacred hearths can reveal past room states.
- Emotional: make the first divine contact quiet and intimate rather than grandiose.

## Information Revealed

- Hestia remembers the village before the Sundering.
- The shrine was meant to shelter people, not receive worship.
- Something went wrong above Olympus.

## Beat Outline

1. The chamber repairs itself in firelight.
2. Hestia speaks as if to someone long gone.
3. A child asks whether the gods can forget people.
4. Hestia says a hearth remembers even when kings do not.
5. The vision collapses, revealing a new root path.

## Player Choice

None.

## Flags Set

- `helikon.hearthwood.hestia_first_echo_seen`
- `global.echoes.hestia_contacted`

## Rewards or Unlocks

- new root path opens
- memory entry added
- Hestia blessing progress increments

## Variants

- If `helikon.elia.widow_bell_started` is true, the child briefly looks toward Elia before the vision fades.

## Re-entry Behavior

The full scene does not replay. The hearth can be inspected later for a short memory line and lore recap.

