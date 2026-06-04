# Release Balance Quick Spec

## Core Decisions

1. The flashlight is the only player damage source required for release.
2. Regular rounds use normal chasing monsters only. Shield, teleport, split, and blob behaviors are removed from the release runtime path.
3. Round 3 ends only when the boss is defeated. The boss uses basic chase movement, while attack patterns remain disabled until a later release.

## Skills

| Skill | Initial Value | Upgrade |
| --- | ---: | ---: |
| Light damage | 1 damage per second | +0.5 damage per second |
| Light radius | Current flashlight diameter | +10% diameter |
| Durability | 0% damage reduction | -10% incoming damage |
| Move speed | 4 world units per second | +10% move speed |

Durability upgrades stack multiplicatively:

`incoming damage multiplier = previous multiplier * 0.9`

## Levels

Maximum level: `5`

| Current Level | XP Required |
| ---: | ---: |
| 1 | 10 |
| 2 | 15 |
| 3 | 20 |
| 4 | 25 |
| 5 | 30 |

At level 5, experience is capped and no further level-up occurs.

## Rounds And Monsters

| Round | Spawn Interval | Monster Health | XP Per Kill | Move Speed |
| ---: | ---: | ---: | ---: | ---: |
| 1 | 1 second | 2 | 3 | 3.5 world units/sec |
| 2 | 2 seconds | 4 | 5 | 4 world units/sec |
| 3 | Boss only | 30 | Game clear | 4.5 world units/sec |

## Release Validation

- Flashlight deals approximately 1 damage after one second of continuous exposure.
- Each light damage upgrade adds exactly 0.5 DPS.
- Each light radius and move speed upgrade multiplies the relevant value by 1.1.
- Player base move speed is 4 world units per second.
- Each durability upgrade multiplies incoming damage by 0.9.
- XP thresholds follow 10, 15, 20, 25, and 30, and level never exceeds 5.
- Round 1 uses a 1-second spawn interval, 2 health, and 3 XP per kill.
- Round 2 uses a 2-second spawn interval, 4 health, and 5 XP per kill.
- Monster move speeds are 3.5, 4, and 4.5 world units per second for rounds 1, 2, and 3.
- Shield, teleport, split, and blob variants are never selected for regular-round spawning, and their behaviors never activate.
- Round 3 does not clear from a timer and clears when the 30-health boss dies.
- The release boss uses basic chase movement only and has no shield, dash, teleport, split, blob, or minion-spawn behavior.

## Deferred Work

- Boss movement and attack patterns will be designed and implemented after release validation.
- Removed special-variant assets may remain in the repository for future work, but they must not be reachable during the release game loop.
