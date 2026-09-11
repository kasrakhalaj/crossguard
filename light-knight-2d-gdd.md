# [Working Title] — Armored Light 2D Duel Game
### Game Design Document — Prototype Phase
**Status:** New direction, superseding the 3D arena version. Reuses the resolved input spec and sword-clock mechanic; discards the 3D camera/lock-on system entirely (2D presentation removes that problem structurally).

---

## 1. Core Pitch

You play a small light/spirit sealed inside a suit of armor — helmet, sword, shield. In 2D (or 2.5D — 3D models on a locked plane, TBD via art-direction test), you fight duels using the same mouse-driven sword-clock aim from the 3D prototype, now enhanced with a **grip-tension mechanic**: how far the mouse is from center determines how extended your sword arm is, trading reach for recovery speed and risking overextension.

**Anchor:** 2D/2.5D side-on melee duel — familiar territory (Nidhogg, Hellish Quart, Dead Cells-adjacent).

**Hook:** Mouse-distance-as-grip-tension (not just angle) layered onto the existing sword-clock system, plus a distinct comedic-but-earnest tone via the character's vulnerability (visible helmet-pop, voice barks) that most duel games don't have.

**Appeal type:** Toy Appeal, same as before — mechanics and feel carry the game, not art fidelity. 2D presentation makes this *easier* to hit than the 3D version did, not harder.

---

## 2. Core Mechanics

### 2.1 Sword Control — Clock-Face Aim + Grip Tension
- Mouse XY controls sword swing direction on a 2D clock face — **identical mechanic to the 3D prototype, fully reusable, no depth-ambiguity problem in 2D by construction.**
- **New: mouse distance from center = grip tension.**
  - Near center: hand tucked in tight to the body. Lower reach, faster recovery, safer.
  - Far from center: arm extended/stretched. Longer reach, slower recovery, higher risk.
- **Overextension fatigue:** pushing to full extension repeatedly or holding it too long accumulates fatigue. Past a threshold: helmet pops open, character barks "OUCHHH," brief vulnerability window (reduced block/parry capability) before it reseals.
- **Charge attack:** a committed, high-reach lunge attack triggers an "AAAAAA" bark — high personality payoff for low implementation cost.
- Slash/Thrust trigger: reuse the resolved mechanism from the 3D spec — motion-shape-based (fast angular sweep = slash, fast radial push = thrust), not a dedicated button, avoiding the input conflicts already solved once.

### 2.2 Shield Control — unchanged from 3D spec
- Discrete keyed stances (Q/E/Z/C for KB+M, LB/LT/RB/RT for gamepad — both already conflict-free per the resolved input spec).
- Graduated durability loss based on angle match, not binary block.

### 2.3 Why 2D removes the bugs from the 3D build, structurally
- No depth ambiguity (mouse-forward vs. mouse-up) — never existed as a problem in 2D.
- No weapon occlusion/visibility issue — side-on silhouette is always fully readable.
- No camera lock-on/soft-facing/orbit logic needed — a horizontal-track follow-cam replaces all of it.
- This is a genuine, structural fix, not a workaround — worth remembering if a future difficulty tempts a return to 3D free-roam.

---

## 3. Art Direction — Locked: Pixel Art

Decision made — pixel art, learned via a purchased GameDev.tv pixel art course. This resolves the earlier open question (3D-on-plane vs. sprite) in favor of true 2D sprites.

- Every attack angle, shield stance, and the helmet-pop/overextension pose will need hand-drawn pixel frames — this is real added scope compared to the 3D-on-plane option, budget for it explicitly in the sprint plan (see Section 7).
- Pixel art's forgiving fidelity ceiling is a genuine advantage given the stated art weakness — "good enough" pixel work reads fine in a way line art often doesn't.
- **Stretch goal, not core requirement:** real-time dynamic lighting on the pixel sprites (normal-map-style lighting, inspired by Holstin's look) — a legitimate, achievable indie technique at small scale. Explicitly a post-MVP polish pass, not a Day 1-14 target.
- **Explicitly not in scope:** Holstin's multi-angle "2XD" camera system (isometric/over-the-shoulder/first-person/top-down/side-scroll switching) is a funded studio's proprietary rendering engine built over years — take the pixel-art-plus-lighting aesthetic from that reference, not the camera architecture. Your game keeps a single fixed or simple side-scrolling camera per Section 2.3.
- Practice plan: apply drawing fundamentals (shape/silhouette construction, from the earlier course recommendation) directly to the knight's actual assets — helmet, sword, shield — rather than disconnected tutorial exercises, so practice time doubles as real production progress.

---

## 4. Tone & Character
- Player character is a light/spirit inside enchanted armor — gives a built-in reason for a squeaky, vulnerable, comedic voice layer without undermining the combat's mechanical seriousness.
- Barks are functional, not just flavor: "OUCH" signals the overextension penalty just fired; the charge bark signals commitment to an attack. Sound doubles as feedback, same principle as the graduated hit-feedback requirement from the 3D doc.

---

## 5. Scope — What's IN for the first playable build

- Mouse-driven sword-clock aim + grip-tension mechanic
- Overextension fatigue + helmet-pop vulnerability window
- Discrete shield stances with graduated block
- One or a small number of arena duels (same duel-gauntlet structure as the 3D plan — courtyard/hall/throne-room style, not open exploration)
- Barks for overextension and charge attack
- Greybox art (primitives) until the core loop is validated — same Two-Week Prototype Filter discipline as before

### 5.1 Stretch Goal — Single Boss Encounter (end-of-sprint only, not core)

Added deliberately as a *small*, additive stretch goal after the Day 12-14 playtest/kill-proceed decision — not a day-1 requirement, and explicitly not the start of an open-world or multi-region game.

- **One** duel opponent with a genuinely distinct moveset — extra shield stances, a unique attack pattern or timing the mirrored basic AI doesn't have — as a culminating fight for the vertical slice.
- **One** small connective space (a short corridor or courtyard) between duel rooms with light environmental detail, giving a taste of "world" without requiring biome design, NPC systems, or content pipelines.
- Rationale (market-grounded, not just design taste): 2026 Steam data shows scope mismatch — not lack of ambition — is what kills solo/indie projects before they ship, and an unshipped game has a guaranteed 0% chance of success versus a finished small game's real, documented shot at working. A single well-crafted boss beats many half-built ones.

---

## 6. Scope — Explicitly OUT for now (do not build yet)

- **Sandbox/open map exploration** — the Section 5.1 connective space is one short corridor, not a wilderness or region system
- **Loot/gear found in the world** — if reintroduced later, follow the sidegrade philosophy already established (no green/blue/epic power tiers), not open-world loot density
- **Co-op / networking** — flagged by the framework doc itself as a Phase 0 discard trigger without prior systems in place; you also said yourself it's "not that suited" for this game — trust that
- **Allies/companions** — a full AI + relationship system, genuinely deferred, not a small add
- **Multiple bosses / a boss roster** — Section 5.1 is deliberately capped at one
- **Abilities system**
- **Any Elden Ring-scale comparison as a planning reference** — that's a different order of magnitude of scope, in team size, budget, and years of development; keep the comparison out of planning conversations entirely, including "the exploration/wilderness/boss-fight feeling" version of the comparison

---

## 7. Sprint Plan — same discipline as before, restarted clean

| Days | Goal |
|---|---|
| 1-3 | Sword-clock aim + grip-tension mechanic functional in **greybox** (primitives/placeholder rectangles), not pixel art yet. Feel test: does the reach/recovery tradeoff feel meaningful? |
| 4-6 | Overextension fatigue + helmet-pop + bark system wired in, still greybox. |
| 6-8 | Shield stances + graduated block, reused from existing spec. One AI opponent using the same systems. |
| 9-11 | First real pixel-art pass on the core knight sprite (idle + one attack angle), applying the course + fundamentals directly. Not a full asset set yet — enough to judge if the mechanic reads well once it's not a placeholder rectangle. |
| 12-14 | Blind playtest, 2-3 people, 3 minutes, zero instructions. Kill/proceed decision. |

**Note:** pixel art production (full attack/stance frame sets) is real, ongoing work that continues past Day 14 regardless of the kill/proceed outcome — don't let art time crowd out the Day 1-8 mechanic validation, which is still the actual point of the sprint.

---

## 8. Carried Over vs. Discarded from the 3D Prototype

**Carried over (reused, not rebuilt):** sword-clock mouse aim, shield stance key mapping, graduated block-by-angle-match, slash/thrust motion-based triggering, sidegrade equipment philosophy, duel-gauntlet content structure, AI-usage boundary (core mechanic hand-written, boilerplate delegated), Two-Week Prototype Filter discipline.

**Discarded (2D presentation makes these unnecessary):** first-person viewmodel system, third-person lock-on/soft-facing camera, camera orbit tuning, near-clip/weapon-clipping mitigation.
