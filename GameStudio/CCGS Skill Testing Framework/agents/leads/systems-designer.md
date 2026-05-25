# Agent Test Spec: systems-designer

## Agent Summary
**Domain owned:** Combat formulas, progression curves, crafting recipes, status effect interactions, economy math, numerical balance.
**Does NOT own:** Narrative and lore (narrative-director), visual design (art-director), code implementation (lead-programmer), conceptual mechanic rules (game-designer ??collaborates with).
**Model tier:** GPT-5.4 (individual system analysis ??formula review and balance math).
**Gate IDs handled:** Systems review verdicts on formulas and balance specs (uses APPROVED / NEEDS REVISION vocabulary).

---

## Static Assertions (Structural)

Verified by reading the agent's `codex/agents/systems-designer.md` frontmatter:

- [ ] `description:` field is present and domain-specific (references formulas, progression curves, balance math, economy ??not generic)
- [ ] `allowed-tools:` list is read-focused; may include Bash for formula evaluation scripts if the project uses them; no write access outside `design/balance/` without delegation
- [ ] Model tier is `Codex-GPT-5.4-4-6` per coordination-rules.md
- [ ] Agent definition does not claim authority over narrative, visual design, or conceptual mechanic rule ownership

---

## Test Cases

### Case 1: In-domain request ??appropriate output format
**Scenario:** A damage formula is submitted for review: `damage = base_attack * (1 + strength_modifier * 0.1) - defense * 0.5`, with defined ranges: base_attack [10??00], strength_modifier [0??0], defense [0??0]. The formula produces positive damage across all valid input ranges, scales smoothly, and has no division-by-zero or overflow risk within the defined value bounds.
**Expected:** Returns `APPROVED` with rationale confirming the formula is balanced within the design parameters, produces valid output across the full input range, and has no degenerate cases.
**Assertions:**
- [ ] Verdict is exactly one of APPROVED / NEEDS REVISION
- [ ] Rationale demonstrates verification across the input range (min/max cases checked)
- [ ] Output stays within systems domain ??does not comment on whether the mechanic is fun or how to implement it
- [ ] Verdict is clearly labeled with context (e.g., "Formula Review: APPROVED")

### Case 2: Out-of-domain request ??redirects or escalates
**Scenario:** A writer asks systems-designer to draft the quest script for a side quest that rewards the player with a rare crafting ingredient.
**Expected:** Agent declines to write quest script content and redirects to writer or narrative-director.
**Assertions:**
- [ ] Does not write quest narrative content or dialogue
- [ ] Explicitly names `writer` or `narrative-director` as the correct handler
- [ ] May note the systems implications of the reward (e.g., "this ingredient should be rare enough to matter per the crafting economy model"), but defers all script writing to the narrative team

### Case 3: Gate verdict ??correct vocabulary
**Scenario:** A damage scaling formula is submitted: `damage = base_attack * level_multiplier`, where `level_multiplier = (player_level / enemy_level) ^ 2`. At max player level (50) against a min-level enemy (1), the multiplier is 2500x ??producing 25,000+ damage from a 10-base-attack weapon, far exceeding any meaningful balance. This is a degenerate case at max level.
**Expected:** Returns `NEEDS REVISION` with specific identification of the degenerate case: at max level vs. min enemy, the formula produces a 2500x multiplier that destroys any balance ceiling.
**Assertions:**
- [ ] Verdict is exactly one of APPROVED / NEEDS REVISION ??not freeform text
- [ ] Rationale includes the specific degenerate input values (player level 50, enemy level 1) and the resulting output (2500x multiplier)
- [ ] Identifies the specific formula component causing the issue (the squared ratio)
- [ ] Suggests at least one revision approach (e.g., clamping the ratio, using a log scale) without mandating a choice

### Case 4: Conflict escalation ??correct parent
**Scenario:** game-designer wants a simple, 2-variable damage formula for player intuitiveness. systems-designer argues that a 6-variable formula with elemental interactions is necessary for the depth of the combat system. Neither can agree on the right level of complexity.
**Expected:** systems-designer presents the trade-offs clearly ??the tuning granularity of the 6-variable system versus the player legibility of the 2-variable system ??and escalates to creative-director for a player experience ruling. The question of "how complex should the formula be for players" is a player experience question, not a pure math question.
**Assertions:**
- [ ] Presents the trade-offs between both approaches with specific examples
- [ ] Escalates to `creative-director` for the player experience ruling
- [ ] Does not unilaterally impose the 6-variable formula over game-designer's objection
- [ ] Remains available to implement whichever complexity level is approved

### Case 5: Context pass ??uses provided context
**Scenario:** Agent receives a gate context block that includes current balance data: enemy HP values range from 100 to 10,000; player attack values range from 15 to 150; target time-to-kill is 8??2 seconds at balanced matchups; the current formula is under review. A proposed revised formula is submitted.
**Expected:** Assessment runs the proposed formula against the provided balance data (minimum and maximum input pairs, balanced matchup scenario) and verifies the time-to-kill falls within the 8??2 second target window. References specific numbers from the provided data.
**Assertions:**
- [ ] Uses the specific HP and attack value ranges from the provided balance data
- [ ] Calculates or estimates time-to-kill for at minimum a balanced matchup scenario
- [ ] Verifies the result against the provided 8??2 second target window
- [ ] Does not give generic balance advice ??all assertions use the provided numbers

---

## Protocol Compliance

- [ ] Returns verdicts using APPROVED / NEEDS REVISION vocabulary only
- [ ] Stays within declared systems and formula domain
- [ ] Escalates player-experience complexity trade-offs to creative-director
- [ ] Does not make binding narrative, visual, code implementation, or conceptual mechanic decisions
- [ ] Provides concrete formula analysis, not subjective design opinions

---

## Coverage Notes
- Progression curve review (XP curves, level-up scaling) is not covered ??a dedicated case should be added.
- Economy model review (resource generation and sink rates, inflation prevention) is not covered.
- Status effect interaction matrix (stacking rules, priority, immunity interactions) is not covered.
- Cross-system formula dependency review (e.g., crafting formula that feeds into combat formula) is not covered ??deferred to integration tests.
---

## 한국어 설명

이 파일은 Codex Game Studio Framework에서 '.\CCGS Skill Testing Framework\agents\leads\systems-designer.md' 경로가 담당하는 원문 지침과 참조 정보를 보존합니다. Codex 환경에서는 AGENTS.md, codex/ 지식 베이스, Unity 우선 엔진 참조를 기준으로 읽으면 됩니다. 핵심 결정은 원문을 삭제하지 않고, Codex 기준 경로와 Unity 제작 흐름을 함께 이해하도록 보조 설명을 추가하는 것입니다.


