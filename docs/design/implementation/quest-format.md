# Scenario and Quest Format

Echoes of Olympus should use scenarios rather than checklist quests whenever possible. A scenario is a small playable story unit built around place, discovery, consequence, and reward.

## Scenario Template

```markdown
# Scenario: [Name]

## Summary

[One paragraph describing what the player discovers or changes.]

## Location

- Region:
- Primary location:
- Connected locations:

## Narrative Purpose

[Why this scenario exists in the story.]

## Gameplay Purpose

[What mechanic, route, enemy, item, or behavior this teaches or reinforces.]

## Trigger Conditions

- Required flags:
- Required items/relics:
- Required time/world state:

## Characters

- [NPC]: role in scenario

## Player Objective

[What the player is trying to do, stated diegetically.]

## Flow

1. [Discovery or hook]
2. [Investigation or obstacle]
3. [Complication]
4. [Resolution]
5. [Return or consequence]

## Dialogue Beats

- [NPC or inscription]: [beat, not final prose]

## Encounters

- Enemies:
- Mini boss:
- Environmental hazards:

## Rewards

- Items:
- Flags:
- Map changes:
- NPC changes:
- Knowledge gained:

## Secrets

- [Optional discovery]

## Failure or Deferral

[What happens if the player leaves, ignores, or returns later.]
```

## Example Scenario: The Widow's Bell

## Summary

A widow in Elia remembers being married, but no one else remembers her husband. The player finds his bell in a root-sealed cellar after gaining the Ember of Hestia.

## Location

- Region: Helikon Vale.
- Primary location: Elia.
- Connected locations: Hearthwood Shrine, village cellar.

## Narrative Purpose

Introduces the idea that the Sundering erased ordinary lives, not only gods and kingdoms.

## Gameplay Purpose

Teaches post-relic return exploration and persistent NPC state.

## Trigger Conditions

- Required flags: `helikon.elia.star_fallen`.
- Required items/relics: Ember of Hestia for completion.
- Required time/world state: after Hearthwood Shrine opens.

## Characters

- Widow of Elia: remembers grief without a name.
- Demeas: avoids the topic until completion.

## Player Objective

Find what the widow has lost and restore enough memory for her to mourn honestly.

## Flow

1. Speak to the widow before the shrine.
2. Notice an unlit hearth mark in her home.
3. Return with the Ember of Hestia.
4. Open the root-sealed cellar.
5. Defeat cracked urn spirits.
6. Recover the bell.
7. Return it to the widow.

## Dialogue Beats

- Widow: she remembers setting two bowls at dinner but cannot remember why.
- Demeas: says some names went quiet after the Sundering.
- Echo memory: a man ringing the bell during the first village evacuation.

## Encounters

- Cracked Urn Spirits.
- Optional Thorn Wight if the player breaks the wrong root cluster.

## Rewards

- Item: Widow's Bell returned.
- Flag: `helikon.elia.widow_bell_returned`.
- Knowledge: ordinary memory can be erased.
- Future consequence: husband can be found in the Underworld if the player learns River Cant.

## Secrets

- If the player lights all cellar lamps, a hidden inscription gives a Hearth-script fragment.

## Failure or Deferral

The scenario remains open. The widow's dialogue changes after each major story beat to show memory fading further.

