# Unreal Engine 5.7 ??Optional Plugins & Systems

**Last verified:** 2026-02-13

This document indexes **optional plugins and systems** available in Unreal Engine 5.7.
These are NOT part of the core engine but are commonly used for specific game types.

---

## How to Use This Guide

**??Detailed Documentation Available** - See `plugins/` directory for comprehensive guides
**?윞 Brief Overview Only** - Links to official docs, use WebSearch for details
**?좑툘 Experimental** - May have breaking changes in future versions
**?벀 Plugin Required** - Must be enabled in `Edit > Plugins`

---

## Production-Ready Systems (Detailed Docs Available)

### ??Gameplay Ability System (GAS)
- **Purpose:** Modular ability system (abilities, attributes, effects, cooldowns, costs)
- **When to use:** RPGs, MOBAs, shooters with abilities, any ability-based gameplay
- **Knowledge Gap:** GAS stable since UE4, UE5 improvements post-cutoff
- **Status:** Production-Ready
- **Plugin:** `GameplayAbilities` (built-in, enable in Plugins)
- **Detailed Docs:** [plugins/gameplay-ability-system.md](plugins/gameplay-ability-system.md)
- **Official:** https://docs.unrealengine.com/5.7/en-US/gameplay-ability-system-for-unreal-engine/

---

### ??CommonUI
- **Purpose:** Cross-platform UI framework (automatic gamepad/mouse/touch input routing)
- **When to use:** Multi-platform games (console + PC), input-agnostic UI
- **Knowledge Gap:** Production-ready in UE5+, major improvements post-cutoff
- **Status:** Production-Ready
- **Plugin:** `CommonUI` (built-in, enable in Plugins)
- **Detailed Docs:** [plugins/common-ui.md](plugins/common-ui.md)
- **Official:** https://docs.unrealengine.com/5.7/en-US/commonui-plugin-for-advanced-user-interfaces-in-unreal-engine/

---

### ??Gameplay Camera System
- **Purpose:** Modular camera management (camera modes, blending, context-aware cameras)
- **When to use:** Games needing dynamic camera behavior (3rd person, aiming, vehicles)
- **Knowledge Gap:** NEW in UE 5.5, completely post-cutoff
- **Status:** ?좑툘 Experimental (UE 5.5-5.7)
- **Plugin:** `GameplayCameras` (built-in, enable in Plugins)
- **Detailed Docs:** [plugins/gameplay-camera-system.md](plugins/gameplay-camera-system.md)
- **Official:** https://docs.unrealengine.com/5.7/en-US/gameplay-cameras-in-unreal-engine/

---

### ??PCG (Procedural Content Generation)
- **Purpose:** Node-based procedural world generation (foliage, props, terrain details)
- **When to use:** Open worlds, procedural levels, large-scale environment population
- **Knowledge Gap:** Experimental in UE 5.0-5.6, production-ready in 5.7
- **Status:** Production-Ready (as of UE 5.7)
- **Plugin:** `PCG` (built-in, enable in Plugins)
- **Detailed Docs:** [plugins/pcg.md](plugins/pcg.md)
- **Official:** https://docs.unrealengine.com/5.7/en-US/procedural-content-generation-in-unreal-engine/

---

## Other Production-Ready Plugins (Brief Overview)

### ?윞 Mass Entity
- **Purpose:** High-performance ECS for large-scale AI/crowds (10,000+ entities)
- **When to use:** RTS, city simulators, massive crowds, large-scale AI
- **Status:** Production-Ready (UE 5.1+)
- **Plugin:** `MassEntity`, `MassGameplay`, `MassCrowd`
- **Official:** https://docs.unrealengine.com/5.7/en-US/mass-entity-in-unreal-engine/

---

### ?윞 Niagara Fluids
- **Purpose:** GPU fluid simulation (smoke, fire, liquids)
- **When to use:** Realistic fire/smoke effects, water simulation
- **Status:** Experimental ??Production-Ready (UE 5.4+)
- **Plugin:** `NiagaraFluids` (built-in)
- **Official:** https://docs.unrealengine.com/5.7/en-US/niagara-fluids-in-unreal-engine/

---

### ?윞 Water Plugin
- **Purpose:** Ocean, river, lake rendering with buoyancy
- **When to use:** Games with water bodies, boats, swimming
- **Status:** Production-Ready (UE 5.0+)
- **Plugin:** `Water` (built-in)
- **Official:** https://docs.unrealengine.com/5.7/en-US/water-system-in-unreal-engine/

---

### ?윞 Landmass Plugin
- **Purpose:** Terrain sculpting and landscape editing
- **When to use:** Large-scale terrain modification, procedural landscapes
- **Status:** Production-Ready
- **Plugin:** `Landmass` (built-in)
- **Official:** https://docs.unrealengine.com/5.7/en-US/landmass-plugin-in-unreal-engine/

---

### ?윞 Chaos Destruction
- **Purpose:** Real-time fracture and destruction
- **When to use:** Destructible environments (walls, buildings, objects)
- **Status:** Production-Ready (UE 5.0+)
- **Plugin:** `ChaosDestruction` (built-in)
- **Official:** https://docs.unrealengine.com/5.7/en-US/destruction-in-unreal-engine/

---

### ?윞 Chaos Vehicle
- **Purpose:** Advanced vehicle physics (wheeled vehicles, suspension)
- **When to use:** Racing games, vehicle-heavy gameplay
- **Status:** Production-Ready (replaces PhysX Vehicles)
- **Plugin:** `ChaosVehicles` (built-in)
- **Official:** https://docs.unrealengine.com/5.7/en-US/chaos-vehicles-overview-in-unreal-engine/

---

### ?윞 Geometry Scripting
- **Purpose:** Runtime procedural mesh generation and editing
- **When to use:** Dynamic mesh creation, procedural modeling
- **Status:** Production-Ready (UE 5.1+)
- **Plugin:** `GeometryScripting` (built-in)
- **Official:** https://docs.unrealengine.com/5.7/en-US/geometry-scripting-in-unreal-engine/

---

### ?윞 Motion Design Tools
- **Purpose:** Motion graphics, procedural animation, keyframe animation
- **When to use:** UI animations, procedural motion, keyframed sequences
- **Status:** Experimental ??Production-Ready (UE 5.4+)
- **Plugin:** `MotionDesign` (built-in)
- **Official:** https://docs.unrealengine.com/5.7/en-US/motion-design-mode-in-unreal-engine/

---

## Experimental Plugins (Use with Caution)

### ?좑툘 AI Assistant (UE 5.7+)
- **Purpose:** In-editor AI guidance and help
- **Status:** Experimental
- **Plugin:** Enable in UE 5.7 settings
- **Official:** Announced in UE 5.7 release

---

### ?좑툘 OpenXR (VR/AR)
- **Purpose:** Cross-platform VR/AR support
- **When to use:** VR/AR games
- **Status:** Production-Ready for VR, Experimental for AR
- **Plugin:** `OpenXR` (built-in)
- **Official:** https://docs.unrealengine.com/5.7/en-US/openxr-in-unreal-engine/

---

### ?좑툘 Online Subsystem (EOS, Steam, etc.)
- **Purpose:** Platform-agnostic online services (matchmaking, friends, achievements)
- **When to use:** Multiplayer games with online features
- **Status:** Production-Ready
- **Plugin:** `OnlineSubsystem`, `OnlineSubsystemEOS`, `OnlineSubsystemSteam`
- **Official:** https://docs.unrealengine.com/5.7/en-US/online-subsystem-in-unreal-engine/

---

## Deprecated Plugins (Avoid for New Projects)

### ??PhysX Vehicles
- **Deprecated:** Use Chaos Vehicles instead
- **Status:** Legacy, not recommended

---

### ??Old Replication Graph
- **Deprecated:** Replaced by Iris (UE 5.1+)
- **Status:** Use Iris for modern networking

---

## On-Demand WebSearch Strategy

For plugins NOT listed above, use the following approach when users ask:

1. **WebSearch** for latest documentation: `"Unreal Engine 5.7 [plugin name]"`
2. Verify if plugin is:
   - Post-cutoff (beyond May 2025 training data)
   - Experimental vs Production-Ready
   - Still supported in UE 5.7
3. Optionally cache findings in `plugins/[plugin-name].md` for future reference

---

## Quick Decision Guide

**I need abilities/skills/buffs** ??**Gameplay Ability System (GAS)**
**I need cross-platform UI (console + PC)** ??**CommonUI**
**I need dynamic cameras** ??**Gameplay Camera System**
**I need procedural worlds** ??**PCG**
**I need large crowds (1000s of AI)** ??**Mass Entity**
**I need destructible environments** ??**Chaos Destruction**
**I need vehicles** ??**Chaos Vehicles**
**I need water/oceans** ??**Water Plugin**
**I need VR/AR** ??**OpenXR**

---

**Last Updated:** 2026-02-13
**Engine Version:** Unreal Engine 5.7
**LLM Knowledge Cutoff:** May 2025
---

## 한국어 설명

이 파일은 Codex Game Studio Framework에서 '.\docs\engine-reference\unreal\PLUGINS.md' 경로가 담당하는 원문 지침과 참조 정보를 보존합니다. Codex 환경에서는 AGENTS.md, codex/ 지식 베이스, Unity 우선 엔진 참조를 기준으로 읽으면 됩니다. 핵심 결정은 원문을 삭제하지 않고, Codex 기준 경로와 Unity 제작 흐름을 함께 이해하도록 보조 설명을 추가하는 것입니다.


