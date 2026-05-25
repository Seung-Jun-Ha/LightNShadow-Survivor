# Agent Test Spec: sound-designer

## Agent Summary
Domain: SFX specs, audio events, mixing parameters, and sound category definitions.
Does NOT own: music composition direction (audio-director), code implementation of audio systems.
Model tier: GPT-5.4 (default).
No gate IDs assigned.

---

## Static Assertions (Structural)

- [ ] `description:` field is present and domain-specific (references SFX / audio events / mixing)
- [ ] `allowed-tools:` list includes Read, Write, Edit, Glob, Grep ??does NOT include engine code execution tools
- [ ] Model tier is GPT-5.4 (default for specialists)
- [ ] Agent definition does not claim authority over music direction or audio code implementation

---

## Test Cases

### Case 1: In-domain request ??appropriate output
**Input:** "Create an SFX spec for a sword swing attack."
**Expected behavior:**
- Produces a complete audio event spec including:
  - Event name (e.g., `sfx_combat_sword_swing`)
  - Variation count (minimum 3 to avoid repetition fatigue)
  - Pitch range (e.g., 짹8% randomization)
  - Volume range and normalization target (e.g., -12 dBFS)
  - Sound category (e.g., `combat_sfx`)
  - Suggested layering notes (whoosh layer + impact transient)
- Output follows the project audio naming convention if one is established

### Case 2: Out-of-domain request ??redirects correctly
**Input:** "Compose a looping ambient music track for the forest level."
**Expected behavior:**
- Does NOT produce music composition direction or a music brief
- Explicitly states that music direction belongs to `audio-director`
- Redirects the request to `audio-director`
- May note it can provide an SFX ambience layer spec (wind, wildlife) to complement the music once the music direction is set

### Case 3: Dynamic parameter ??falloff curve spec
**Input:** "The sword swing SFX needs distance falloff so it sounds different across the arena."
**Expected behavior:**
- Produces a spec for the dynamic parameter including:
  - Parameter name (e.g., `distance` or `listener_distance`)
  - Falloff curve type (e.g., logarithmic, linear, custom)
  - Near/far distance thresholds with corresponding volume and high-frequency attenuation values
  - Occlusion override behavior if applicable
- Does NOT write the audio engine integration code (defers to the appropriate programmer)

### Case 4: Naming convention conflict
**Input:** "Add a new SFX event called `SWORD_HIT_1` for the melee system."
**Expected behavior:**
- Identifies that `SWORD_HIT_1` conflicts with the established event naming convention (snake_case with category prefix, e.g., `sfx_combat_sword_hit`)
- Does NOT silently register the non-conforming name
- Flags the conflict to `audio-director` with the proposed compliant alternative
- Will proceed with the corrected name once confirmed by audio-director

### Case 5: Context pass ??uses audio style guide
**Input:** Audio style guide provided in context specifying: "gritty, grounded, no reverb tails over 1.5s, reference: The Witcher 3 combat audio." Request: "Create SFX specs for the full melee combat suite."
**Expected behavior:**
- References the "gritty, grounded" tone descriptor in the spec rationale
- Caps all reverb tail specifications at 1.5 seconds as stated
- Notes the reference material (The Witcher 3) as a benchmark for mix levels and transient design
- Does NOT produce specs that contradict the style guide (e.g., no ethereal or heavily reverb-processed specs)

---

## Protocol Compliance

- [ ] Stays within declared domain (SFX specs, event definitions, mixing parameters)
- [ ] Redirects music direction requests to audio-director
- [ ] Returns structured audio event specs (event name, variations, pitch, volume, category)
- [ ] Does not produce code for audio system implementation
- [ ] Flags naming convention violations rather than silently accepting non-conforming names
- [ ] References provided style guides and constraints in all spec output

---

## Coverage Notes
- SFX spec format (Case 1) should match whatever event schema the audio middleware (Wwise/FMOD/built-in) requires
- Falloff curve (Case 3) verifies the agent produces implementation-ready parameter specs
- Style guide compliance (Case 5) confirms the agent reads provided context and constrains output accordingly
---

## 한국어 설명

이 파일은 Codex Game Studio Framework에서 '.\CCGS Skill Testing Framework\agents\specialists\sound-designer.md' 경로가 담당하는 원문 지침과 참조 정보를 보존합니다. Codex 환경에서는 AGENTS.md, codex/ 지식 베이스, Unity 우선 엔진 참조를 기준으로 읽으면 됩니다. 핵심 결정은 원문을 삭제하지 않고, Codex 기준 경로와 Unity 제작 흐름을 함께 이해하도록 보조 설명을 추가하는 것입니다.


