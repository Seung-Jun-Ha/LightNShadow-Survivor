# Level: [Level Name]

## Quick Reference

- **Area/Region**: [Where in the game world]
- **Type**: [Combat / Exploration / Puzzle / Hub / Boss / Mixed]
- **Estimated Play Time**: [X-Y minutes]
- **Difficulty**: [1-10 relative scale]
- **Prerequisite**: [What the player must have done to reach this level]
- **Status**: [Concept | Layout | Graybox | Art Pass | Polish | Final]

## Narrative Context

- **Story Moment**: [Where in the narrative arc does this level occur]
- **Narrative Purpose**: [What story beat this level delivers]
- **Emotional Target**: [What the player should feel during this level]
- **Lore Discoveries**: [What world-building the player can find here]

## Layout

### Overview Map

```
[ASCII diagram of the level layout. Use these symbols:]
[S] = Start point
[E] = Exit/end point
[C] = Combat encounter
[P] = Puzzle
[R] = Reward/loot
[!] = Story beat
[?] = Secret/optional
[>] = One-way passage
[=] = Two-way passage
[@] = NPC
[B] = Boss encounter
```

### Critical Path

[The mandatory route through the level, step by step.]

1. Player enters at [S]
2. [Description of what happens along the path]
3. Player exits at [E]

### Optional Paths

| Path | Access Requirement | Reward | Discovery Hint |
|------|-------------------|--------|---------------|

### Points of Interest

| Location | Type | Description | Purpose |
|----------|------|-------------|---------|

## Encounters

### Combat Encounters

| ID | Position | Enemy Composition | Difficulty | Arena Notes |
|----|----------|------------------|-----------|-------------|
| E-01 | [Map ref] | [2x Grunt, 1x Ranged] | 3/10 | Open area, cover on flanks |
| E-02 | [Map ref] | [1x Elite, 3x Grunt] | 5/10 | Narrow corridor, no retreat |

### Non-Combat Encounters

| ID | Position | Type | Description | Solution Hint |
|----|----------|------|-------------|---------------|

## Pacing Chart

```
Intensity
10 |                              *
 8 |                         *   * *
 6 |            *  *        * * *   *
 4 |     *  *  * ** *   *  *
 2 | * ** ** *        * * *          *
 0 |S-----------------------------------------E
     [Start]    [Mid]              [Climax] [Exit]
```

[Describe the intended rhythm: where are the peaks, valleys, rest points?]

## Audio Direction

| Zone/Moment | Music Track | Ambience | Key SFX |
|-------------|------------|----------|---------|
| [Entry] | [Track] | [Ambient sounds] | [Door opening] |
| [Combat] | [Combat music] | [Muted ambience] | [Combat SFX] |
| [Post-combat] | [Calm transition] | [Return to ambience] | |

## Visual Direction

- **Lighting**: [Key, fill, ambient description]
- **Color Palette**: [Dominant colors and why]
- **Mood Board References**: [Description of visual references]
- **Landmarks**: [Visible navigation aids and their locations]
- **Sight Lines**: [What the player should see from key positions]

## Collectibles and Secrets

| Item | Location | Visibility | Hint | Required For |
|------|----------|-----------|------|-------------|

## Technical Notes

- **Estimated Object Count**: [N]
- **Streaming Zones**: [Where to break the level for streaming]
- **Performance Concerns**: [Any known heavy areas]
- **Required Systems**: [What game systems are active in this level]
---

## 한국어 설명

이 파일은 Codex Game Studio Framework에서 '.\codex\docs\templates\level-design-document.md' 경로가 담당하는 원문 지침과 참조 정보를 보존합니다. Codex 환경에서는 AGENTS.md, codex/ 지식 베이스, Unity 우선 엔진 참조를 기준으로 읽으면 됩니다. 핵심 결정은 원문을 삭제하지 않고, Codex 기준 경로와 Unity 제작 흐름을 함께 이해하도록 보조 설명을 추가하는 것입니다.


