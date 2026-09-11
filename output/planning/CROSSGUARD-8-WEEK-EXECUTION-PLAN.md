# Crossguard — 8-Week Execution Plan
## A daily workbook for your first finished indie game

**Version:** 2 • **Prepared:** 11 September 2026  
**Nominal dates:** 11 September–5 November 2026  
**Capacity:** 28 hours/week × 8 weeks = **224 hours**  
**Schedule:** 6 workdays × 4 hours 40 minutes, followed by 1 day off  
**Production sessions:** 48  
**Purpose:** finish a small game, understand its systems, and create a credible portfolio piece.

> Use this execution workbook instead of the earlier high-level outline. It adds daily outputs and corrects the previous eight-hour workday. If you start later, shift all dates together. Your day off can fall anywhere in the week.

## 1. What success means

Finish a small, coherent Windows 2D sword-and-shield game with a beginning, encounters, a boss, an ending, clear controls and reliable retry. You should be able to explain your main contributions without relying on a tutorial or assistant to describe them.

This is a scope-controlled learning project. It does not promise commercial success, a particular review score, or Elden Ring/Shroom and Gloom production quality in two months. Those games provide specific lessons in atmosphere, identity and readability.

**Primary deliverable:** a tested downloadable build plus source backup and portfolio entry.  
**Optional distribution goal:** a Steam release if the game and platform requirements are ready.  
**Desired first-clear length:** roughly 10–20 minutes, without deliberately padding the game. A satisfying shorter game is acceptable.

### Recommended scope

| Area | Committed target | First reduction if late |
|---|---|---|
| Game format | Fully 2D side-view, Windows first | Keep one tested input scheme |
| Hero | One knight, one sword and shield | Fewer cosmetic animation frames |
| Controls | Existing movement; fixed attack and forward block by default | Remove experimental aiming and timed counter |
| Normal enemies | One base enemy and one contrasting behavior/variant | One reliable enemy type |
| Boss | One boss, two attacks | No second phase |
| Map | Three connected spaces using one material kit | Two spaces or a shorter route |
| Art | Actual 64×64 actor frames; one production palette | Simpler shapes and shared parts |
| Atmosphere | Layered scenery, contact shadows, a few focal lights | Static layered scenery and painted shading |
| Story | One short premise, one visible clue and payoff | An ending image with a clear result |
| Progress | Checkpoints/retry; persistence decision in Week 5 | Explicit short single-session game |
| Release | Packaged game, README, credits, case study | Private release while public launch waits |

**Outside this release:** multiplayer, open world, crafting, inventory economy, procedural generation, class systems, additional weapon families, an engine change, complex normal maps, and a first-person deckbuilder conversion.

A bow or mace in your reference pack is an art study; it does not add that weapon to the game.

### Decisions already made for planning

- Working title: **Crossguard**. Keep Light Knight in notes if desired; renaming is not a weekly task.
- Initial palette: **ENDESGA 64**, from your downloaded HEX file.
- Art references: original dark-fantasy armor and ruins; Shroom and Gloom's consistency/personality.
- Initial location: a ruined pilgrimage fortress with an outer approach, sheltered hall and causeway. This is a contained production setting, not a commitment to a large lore universe.
- Start from working **Mosslight 2D systems** where useful. The repository also contains older 3D Crossguard arena code; do not accidentally restart that version.
- The older mouse-directed sword-clock/grip mechanic is an experiment with a Week-1 deadline, not an assumed free reuse.

## 2. Time budget and daily routine

| Time block | Minutes | Purpose |
|---|---:|---|
| Choose one outcome | 15 | Read today's checklist and define the smallest finished result |
| Targeted learning | 30 | Learn only the technique needed today |
| Make the thing | 120 | Code, draw or assemble one outcome |
| Test and correct | 45 | Inspect the result in context, not just in the editor |
| Record and back up | 10 | Save source, note results and write the next task |
| Breaks | 20 | Two short breaks; stand up and leave the screen |
| Flex time | 40 | Debugging, extra study, exports or ending early when tired |
| **Total** | **280** | **4 hours 40 minutes** |

The 240-minute core block includes breaks. Across eight weeks:
- **176 hours** of scheduled learning/production/testing/recording.
- **16 hours** of scheduled breaks.
- **32 hours** of flex time.
- **224 hours total session time.**

Daily content below is the scope of that core block. Do not interpret every sentence as an additional homework assignment. Move flex minutes earlier in a session whenever needed.

Use six work sessions and one genuine day off each week. If you only have four hours on a day, omit flex time and move unfinished work to the next session; do not automatically double the following day. If you have a fifth hour, use it for review or cleanup, not scope growth.

### Stop rules

- After 45 minutes stuck on one issue: record expected behavior, actual behavior, exact error and what you tried. Ask for focused help or build the smallest reproduction.
- After a second session blocked on an optional feature: cut or simplify it.
- At 4 hours 40 minutes: record the next step and stop. Sustained overtime is not the recovery strategy.
- Never end a day with only watched videos if a small practical exercise was possible.
- Keep a known-good build. Make a source snapshot before substantial changes; avoid deleting working systems while learning replacements.
- Do not download another course, asset pack or engine to escape one uncomfortable lesson.

## 3. Starting resources in your existing project

| Resource | Where to start | Important limitation |
|---|---|---|
| Learning guide | [START_HERE.md](C:/Unity/crossguard/Assets/Mosslight/START_HERE.md) | Describes the current learning prototype |
| 2D scene | [Mosslight.unity](C:/Unity/crossguard/Assets/Mosslight/Scenes/Mosslight.unity) | Much of the scene is built at runtime |
| Saved tuning | [LearningTuning.asset](C:/Unity/crossguard/Assets/Mosslight/Settings/LearningTuning.asset) | Prefer saved settings over temporary Play-mode edits |
| Hero behavior | [MosslightHero.cs](C:/Unity/crossguard/Assets/Mosslight/Scripts/MosslightHero.cs) | Current attack damage occurs immediately; Week 2 aligns it with animation |
| Enemy behavior | [MosslightEnemy.cs](C:/Unity/crossguard/Assets/Mosslight/Scripts/MosslightEnemy.cs) | Reuse and simplify before extending |
| Boss behavior | [MosslightBoss.cs](C:/Unity/crossguard/Assets/Mosslight/Scripts/MosslightBoss.cs) | An existing structure, not your final encounter design |
| World layout | [MosslightWorld.cs](C:/Unity/crossguard/Assets/Mosslight/Scripts/MosslightWorld.cs) | Edit the saved level recipe, not only runtime objects |
| Pixel references | [ENDESGA exports](C:/Unity/crossguard/output/ashen-sprites-paletted/endesga-64) | Actual sizes/palette, but downsampled concept art still needs cleanup |
| Palette | [Downloaded HEX](E:/Aseprite/palettes/endesga-64.hex) | 64 colors does not require using all colors in every sprite |

**Startup trap:** edits to objects created while Unity is playing are normally temporary. Follow the guide's saved tuning/code workflow for changes that must persist. Confirm you are opening the Mosslight learning scene, not assuming the default project scene is the new game.

### Art workload rules

- Individual hero/enemy frames are **64×64**. The body should leave room for movement and equipment; the canvas size is not a demand to fill every pixel.
- Large scenery can be assembled from modules or larger backgrounds. Do not force the entire map into 64×64 assets.
- Use approximately 3–4 shades per material and only the materials a sprite needs.
- First animation set: **2 idle, 4 run, 3 attack poses, 1 block**. Hurt/defeat may reuse a pose with clear effects initially. Preserve a readable jump/fall pose if those actions remain.
- Reuse layers and forms. The generated front/side/back views are not automatically a consistent animation set.
- Check actual size every 15–20 minutes. If a detail is invisible at game scale, it has low priority.
- If one finished pose repeatedly takes over an hour, simplify before producing ten more.
- Handcrafted-looking roughness still needs consistent shapes, palette and timing. Do not substitute random texture for personality.

## 4. The eight weeks

Each week's Days 1–6 are work sessions. **Day 7 is off.** Tick a day only when its visible output exists; carry a failed output forward explicitly.

### Week 1 — A stable starting point and one rough fight
**Nominal dates:** 11 September–17 September • **Learn:** Input, movement, colliders, hitboxes, and building a Windows executable.

#### W1D1 — Baseline and scope
- [ ] **Do:** Play the current Mosslight build for 20 minutes. Write five sentences describing Crossguard, its player, action, setting and ending. Make a source snapshot/backup and export a baseline build.
- [ ] **Visible output:** A baseline ZIP, a short scope note, and three observations from play.

#### W1D2 — Understand movement
- [ ] **Do:** Trace movement in MosslightHero and input in MosslightInput. Change one tuning value, compare, then keep or undo it. Build one flat test area with safe boundaries.
- [ ] **Visible output:** A before/after movement note and a room you can traverse reliably.

#### W1D3 — Understand one hit
- [ ] **Do:** Trace Attack → overlap query → IHurtable.Hit. Display the attack area while testing. Check facing left/right, range and one-hit-per-target behavior.
- [ ] **Visible output:** A short clip showing a readable attack area and consistent damage.

#### W1D4 — Settle sword controls
- [ ] **Do:** Default to fixed attack plus shield for this release. Only if you still want mouse-directed sword control, spend at most today's four planned hours on a disposable experiment. Keep it only if it works in 2D, hits consistently and is understandable.
- [ ] **Visible output:** A written control decision. Rejected experiments stay out of the working build.

#### W1D5 — One enemy and retry
- [ ] **Do:** Use an existing enemy behavior in the test room. Adjust its health and attack interval. Test player death, enemy defeat, pit recovery if relevant and retry.
- [ ] **Visible output:** A fight that can be won and lost without a broken restart.

#### W1D6 — First outside look
- [ ] **Do:** Export the build, ask one person to try it if available, and observe. Fix the single biggest obstacle. Review hours and choose next week's first task.
- [ ] **Visible output:** Week-1 build, three observations and a passed or explicitly failed weekly gate.

**Weekly gate — check before moving on:**
- [ ] Standalone build starts and reaches a playable room.
- [ ] Movement, attack, damage, death and retry work.
- [ ] You can explain one complete hit.
- [ ] Control scope is chosen; no ongoing free-angle redesign.

**If the gate fails:** Keep the existing controller, remove the mouse-control experiment and use a flat room. Restore the baseline if the current branch cannot be built.

**Record:** hours used / biggest issue / what you can explain / what you will cut.

---

### Week 2 — Readable combat with one defense
**Nominal dates:** 18 September–24 September • **Learn:** Attack states, anticipation, recovery, shield-facing checks and separate gameplay/effects.

#### W2D1 — Enemy warning
- [ ] **Do:** Give the first enemy distinct prepare, attack and recover states. Make the preparation visible as a pose or movement; don't rely only on a color flash.
- [ ] **Visible output:** One attack you can anticipate and deliberately avoid.

#### W2D2 — Attack timing
- [ ] **Do:** The current hero damages immediately on input. Separate preparation, the active hit moment and recovery so damage can match a drawn attack. Make each swing damage a target at most once.
- [ ] **Visible output:** An attack timeline that you can explain and see; damage occurs at the intended strike.

#### W2D3 — Basic shield
- [ ] **Do:** Add one forward-facing held block. Specify which damage it stops and how it changes movement/attacking. Test front/back hits and ensure fall damage cannot be blocked.
- [ ] **Visible output:** A written three-rule block behavior and repeatable front/back tests.

#### W2D4 — A contrasting enemy
- [ ] **Do:** Adapt a second existing behavior or a simple variant: slower heavy commitment versus a faster approach. Reuse code and a common base design.
- [ ] **Visible output:** Two distinguishable attacks, or one reliable enemy if time runs out.

#### W2D5 — One satisfying combat moment
- [ ] **Do:** Improve the block impact and recovery feedback. Only if basic defense is stable, try a short timed-block counter opportunity; cap the experiment at two hours. Otherwise improve ordinary block feedback.
- [ ] **Visible output:** A useful defense with clear sound/pose feedback; no extra meter or skill tree.

#### W2D6 — Combat test and freeze
- [ ] **Do:** Ask two people to fight without coaching where possible. Observe their first successful defense and first death. Correct unfair timing and export.
- [ ] **Visible output:** Week-2 build and a settled combat loop.

**Weekly gate — check before moving on:**
- [ ] Enemy preparation is visible before damage.
- [ ] Sword artwork/hit feedback and damage timing agree.
- [ ] Shield rules work consistently.
- [ ] A new player can win one fight and explain a death.

**If the gate fails:** Keep ordinary forward block, one attack and one enemy. Cut the timed counter, guard directions and additional attack types. Trade next week's decoration time for unresolved combat.

**Record:** hours used / biggest issue / what you can explain / what you will cut.

---

### Week 3 — A reproducible 64×64 character style
**Nominal dates:** 25 September–1 October • **Learn:** Aseprite layers, silhouette, restrained color ramps, onion skin, frame alignment and animation playback.

#### W3D1 — Set the art rules
- [ ] **Do:** Open an existing ENDESGA variant. Use a 64×64 character frame. Simplify armor into large clusters, choose one silhouette feature, and draw your own standing pose.
- [ ] **Visible output:** One standing pose plus a tiny style note: outline, light direction and palette.

#### W3D2 — Idle and locomotion
- [ ] **Do:** Make two idle frames and four simple run frames using shared body parts and minimal armor motion. Inspect at 1× and 4×. Keep feet aligned.
- [ ] **Visible output:** A small readable idle/run set, even if rough.

#### W3D3 — Attack and defense
- [ ] **Do:** Draw three strong attack poses: preparation, strike, recovery. Add one block pose. Keep a clear gap between sword/shield and body.
- [ ] **Visible output:** An attack that reads by silhouette and a strong defense pose.

#### W3D4 — Hurt and integration
- [ ] **Do:** Add hurt/defeated poses or reuse a clear pose with effects. Integrate frames into the game without changing physics proportions. Match Day 8's active hit time to the strike pose.
- [ ] **Visible output:** The hero moves, strikes, blocks and takes damage with the new art.

#### W3D5 — Enemy essentials
- [ ] **Do:** Make one simplified enemy design with prepare, strike and recovery poses. Reuse/recolor its base for a variant only if the silhouettes or attack cues remain distinguishable.
- [ ] **Visible output:** One readable enemy art set inside the game.

#### W3D6 — Art-cost review
- [ ] **Do:** Record actual time per pose. Remove noise and fix pivots. Test attacks over light and dark backgrounds. Export the weekly build.
- [ ] **Visible output:** Week-3 build and an honest estimate of future animation cost.

**Weekly gate — check before moving on:**
- [ ] Individual character frames are exactly 64×64.
- [ ] You can redraw the standing pose and explain its material colors.
- [ ] Attack preparation and strike remain legible at game size.
- [ ] Imported sprites don't change collision behavior or slide unexpectedly between frames.

**If the gate fails:** Use fewer frames, simpler armor and shared layers. Keep generated sprites temporarily for secondary assets. Never spend a whole week perfecting a helmet while combat remains broken.

**Record:** hours used / biggest issue / what you can explain / what you will cut.

---

### Week 4 — One complete map with convincing 2D depth
**Nominal dates:** 2 October–8 October • **Learn:** Modular scenery, sorting layers, parallax, contact shadows and a visible environmental clue.

#### W4D1 — Map on paper, then blocks
- [ ] **Do:** Sketch three connected spaces: outer approach, sheltered hall, final arena. State what each teaches. Block out the whole route using existing solids.
- [ ] **Visible output:** A traversable route to the final arena, with no decorative detours yet.

#### W4D2 — Small environment kit
- [ ] **Do:** Draw or clean a ground module, wall module, pillar, arch and a thick doorway. Keep pixels-per-unit consistent. Reuse sections.
- [ ] **Visible output:** Five usable environment modules and a stable walkable edge.

#### W4D3 — Props with purpose
- [ ] **Do:** Add a lantern, banner and one distinctive shrine/relic. Place evidence for a simple mystery: an abandoned shrine remains warm beyond a sealed threshold, for example.
- [ ] **Visible output:** Three purposeful props and one clue the player can notice without text.

#### W4D4 — Depth layers
- [ ] **Do:** Arrange near/middle/far scenery. Add modest parallax to scenery only. Draw floor thickness and contact shadows. Keep important combat areas unobscured.
- [ ] **Visible output:** Depth is visible during a slow camera pan without moving any collider.

#### W4D5 — Assemble and stage
- [ ] **Do:** Apply the kit to all three spaces. Set restrained warm focal areas against quieter backgrounds. Try exterior/interior ambient sound. Reuse the same material set.
- [ ] **Visible output:** A complete small setting with different compositions, not three separate biomes.

#### W4D6 — Navigation test
- [ ] **Do:** Have someone find the route and identify solid ground. Inspect at common window sizes. Fix misleading ledges, camera occlusion and repetition.
- [ ] **Visible output:** Week-4 build; every required room is navigable.

**Weekly gate — check before moving on:**
- [ ] All three spaces connect.
- [ ] Walkable ground is distinct from scenery.
- [ ] Hero, weapon and attack warnings remain readable.
- [ ] One landmark or clue gives the location a specific identity.

**If the gate fails:** Use two spaces or one background if necessary. Remove weather, optional alcoves and decorative animation. Normal maps and reflective water are deferred.

**Record:** hours used / biggest issue / what you can explain / what you will cut.

---

### Week 5 — A complete beginning-to-ending game
**Nominal dates:** 9 October–15 October • **Learn:** Reusing boss states, progression flags, simple save/reset behavior and matching feedback to events.

#### W5D1 — First boss attack
- [ ] **Do:** Adapt the existing boss into the final arena. Use a larger or altered base silhouette. Start with one slow attack and an obvious recovery.
- [ ] **Visible output:** A boss encounter you can enter, fight and retry.

#### W5D2 — Second boss attack
- [ ] **Do:** Add a second attack that asks for a different response using existing controls. Test safe opportunities to retaliate. Keep visual states simple.
- [ ] **Visible output:** Two readable attacks and predictable recovery windows.

#### W5D3 — Finish the loop
- [ ] **Do:** Connect boss defeat to a short visual payoff, ending screen and restart. Complete the second enemy's rough visuals only if necessary.
- [ ] **Visible output:** A full start-to-ending run with all required encounters present.

#### W5D4 — Persistence decision
- [ ] **Do:** If the game is naturally longer than a short session, implement only checkpoint/completion persistence and New Game/reset. Otherwise explicitly choose a short single-session release with clear restart behavior. Test whichever route you choose.
- [ ] **Visible output:** One documented and working progress policy; no unfinished save system.

#### W5D5 — Essential sound and credits
- [ ] **Do:** Prioritize sword, shield, hit, death/retry, ambient space and boss warning sounds. Use original or appropriately licensed audio. Record every external asset and source.
- [ ] **Visible output:** Core actions sound distinct; a credits/source list exists.

#### W5D6 — Content-complete test
- [ ] **Do:** Play from a fresh start without editor shortcuts. Test death before and after checkpoints and the boss ending. Export and freeze the feature list.
- [ ] **Visible output:** Week-5 complete build. Missing work is bugs and polish, not new systems.

**Weekly gate — check before moving on:**
- [ ] A fresh run can reach an ending.
- [ ] All mandatory fights exist and can be won.
- [ ] Death and progress behavior are understandable.
- [ ] There are no placeholder buttons leading to unfinished features.

**If the gate fails:** Remove extra boss phases, the second normal enemy and optional side areas. Prefer a shorter complete game. A failed full-loop gate replaces planned polish with completion work.

**Record:** hours used / biggest issue / what you can explain / what you will cut.

---

### Week 6 — External testing and focused corrections
**Nominal dates:** 16 October–22 October • **Learn:** Observing confusion, writing reproducible bug reports and prioritizing fixes.

#### W6D1 — Prepare a test build
- [ ] **Do:** Write a short neutral tester note with launch instructions only. Prepare an observation sheet. Find willing testers; don't wait for a large audience.
- [ ] **Visible output:** A build someone can launch and a short feedback form.

#### W6D2 — Observe first tests
- [ ] **Do:** Watch one or two fresh players if possible. Avoid explaining during their first attempt. Record where they stop and what they think happened.
- [ ] **Visible output:** Specific observations, not just 'fun' or 'bad' ratings.

#### W6D3 — Fix blockers
- [ ] **Do:** Reproduce and fix crashes, softlocks, broken retries or unclear progression. Recheck each fix in a build.
- [ ] **Visible output:** The highest-severity problems from testing are resolved.

#### W6D4 — Fix combat communication
- [ ] **Do:** Adjust warnings, recovery, weapon readability, camera framing or sound according to observed problems. Change a small number of variables.
- [ ] **Visible output:** A focused before/after comparison for one confusing encounter.

#### W6D5 — Retest with fresh eyes
- [ ] **Do:** Get another one or two players, aiming for 3–5 total across the week. If unavailable, record and review your own cold-start run, but mark external testing incomplete.
- [ ] **Visible output:** Evidence of whether the earlier fixes helped.

#### W6D6 — Stability build
- [ ] **Do:** Run the full route. Review the remaining issues and cut any optional feature causing repeated trouble. Export.
- [ ] **Visible output:** Week-6 build and a prioritized issue list.

**Weekly gate — check before moving on:**
- [ ] At least one outside player has completed the build if testers are available.
- [ ] Earlier blockers are fixed and rechecked.
- [ ] Remaining issues are recorded by severity.
- [ ] No new mechanic or new room was added.

**If the gate fails:** Reduce one difficult encounter rather than adding more systems. If testers cannot be found, continue self-testing and keep release private until outside feedback arrives.

**Record:** hours used / biggest issue / what you can explain / what you will cut.

---

### Week 7 — A distributable release candidate
**Nominal dates:** 23 October–29 October • **Learn:** Testing outside Unity, clear menus, accurate presentation and asset attribution.

#### W7D1 — Menus and controls
- [ ] **Do:** Polish the existing start, pause, restart and quit flow. Display the actual controls. Keep gamepad claims only if tested.
- [ ] **Visible output:** No confusing menu paths or unsupported control promises.

#### W7D2 — Comfort and persistence
- [ ] **Do:** Verify volume, shake toggle, New Game/reset and any save system. Check quitting/reopening and losing application focus.
- [ ] **Visible output:** Reliable settings and progress behavior.

#### W7D3 — Clean-machine test
- [ ] **Do:** Run the exact package from a clean folder and, ideally, another machine/account. Check windowed/fullscreen modes and repeated launches.
- [ ] **Visible output:** An independently launched release candidate and recorded results.

#### W7D4 — Gameplay capture
- [ ] **Do:** Capture 4–6 screenshots and a 20–40-second gameplay clip from the current build. Include combat and an atmospheric scene.
- [ ] **Visible output:** Truthful presentation assets showing actual gameplay.

#### W7D5 — Credits and page draft
- [ ] **Do:** Finish credits/source records and a short description, controls and known limitations. Complete any applicable storefront content disclosures. Draft your portfolio contribution statement.
- [ ] **Visible output:** A complete page/README draft and credits.

#### W7D6 — Release-candidate review
- [ ] **Do:** Play through the packaged game. Check all intended buttons and the ending. Freeze the release candidate; list only must-fix issues.
- [ ] **Visible output:** Week-7 candidate ZIP and a release checklist.

**Weekly gate — check before moving on:**
- [ ] The package runs outside the editor.
- [ ] At least one full completion of that exact package succeeds.
- [ ] Screenshots and description match the game.
- [ ] Credits, supported controls and progress behavior are accurate.

**If the gate fails:** Cut optional platform features, localization, extra display options and cinematic trailers. Keep the current working menus instead of replacing them.

**Record:** hours used / biggest issue / what you can explain / what you will cut.

---

### Week 8 — Deliver, archive and make the portfolio case study
**Nominal dates:** 30 October–5 November • **Learn:** Release discipline, reproducibility and explaining your own work.

#### W8D1 — Final fresh-start check
- [ ] **Do:** Use the release checklist below on the exact candidate. Test both a fresh user state and existing progress if supported.
- [ ] **Visible output:** A signed-off checklist or a short blocker list.

#### W8D2 — Release blockers only
- [ ] **Do:** Fix any crash, broken save, missing asset or completion failure. Do not add content. Rebuild and recheck the changed path.
- [ ] **Visible output:** A corrected candidate with a clear version name.

#### W8D3 — Full retest and package
- [ ] **Do:** Run the complete game again, then package executable and required neighboring files. Include README and credits. Keep a known-good copy.
- [ ] **Visible output:** The exact deliverable ZIP and source snapshot.

#### W8D4 — Deliver through chosen route
- [ ] **Do:** Release privately or through itch.io; use Steam only if the game and platform requirements are ready. Verify the downloaded package after upload if publishing.
- [ ] **Visible output:** A working delivery link or an intentionally private portfolio build.

#### W8D5 — Portfolio case study
- [ ] **Do:** Write 400–700 words: your role, scope, controls, art process, two problems solved and what testing changed. Identify generated/third-party help honestly. Add the gameplay clip.
- [ ] **Visible output:** One credible portfolio entry focused on your contribution.

#### W8D6 — Postmortem and next decision
- [ ] **Do:** Compare planned versus actual hours. Record what you can explain independently, what took longest and three improvements for next time. Archive source/builds and take a break.
- [ ] **Visible output:** A completed project record and a short next-step list.

**Weekly gate — check before moving on:**
- [ ] Someone can obtain and finish the delivered build.
- [ ] The source and exact released package are backed up.
- [ ] Your portfolio explains your contribution accurately.
- [ ] Remaining work is explicitly deferred rather than silently unfinished.

**If the gate fails:** Move a public release date if blockers remain. Deliver a private tested build while storefront review proceeds. Do not claim a public launch or full release has happened until verified.

**Record:** hours used / biggest issue / what you can explain / what you will cut.

---

## 5. Priority rules when the plan slips

**Week 2:** if a basic fight is not understandable, use Week 3's decoration time to fix it.  
**Week 3:** if art is slow, simplify the hero and reuse enemy parts. Do not change art software or scale midweek.  
**Week 5:** if there is no complete run, stop all optional art and finish the loop.  
**Week 7:** if crashes or progression failures remain, the build is not ready for public launch.

Cut in this order:
1. Decorative effects, weather and extra props.
2. Extra idle frames and small armor ornament.
3. Optional alcove/secret.
4. Boss second phase.
5. Second regular enemy.
6. Map length, preserving a teaching encounter, a boss and an ending.

Never trade away clear controls, accurate damage, readable ground, retry or the ending to keep decorative content.

If more than one week behind, rescope the next build in writing. A shorter complete game still meets the learning objective. A larger unfinished one does not.

## 6. Playtesting protocol

Start with one tester in Week 1, two in Week 2 and aim for 3–5 fresh testers across Week 6. These are practical learning samples, not statistically reliable market forecasts.

Give only launch instructions first. Watch without explaining. Ask afterward:
1. What did you think your goal was?
2. What did you think caused your last death?
3. When did blocking help?
4. Which place or object made you curious?
5. What would you change first?

Record behavior as well as opinions. “Tried to jump onto the background ledge four times” is more actionable than “level confusing.”

### Issue priorities
- **P0:** won't start, crashes, destroys progress or cannot be completed.
- **P1:** unreliable combat, invisible hazards, broken retry, unclear required interaction.
- **P2:** confusing feedback, awkward camera or difficulty spikes.
- **P3:** cosmetic imperfections.

Fix P0/P1 before spending time on P3. Reproduce → change one thing → retest the failing case → check one related behavior. Export an executable at least weekly; an editor-only result is not the deliverable.

## 7. Release route and Steam timing

Choose the delivery route early enough that it cannot swallow the last week:
- **Week 1:** inspect Steam onboarding if a Steam launch is important to you.
- **Week 2:** decide whether this project is pursuing Steam now or after the portfolio build.
- **Weeks 4–5:** prepare page materials from the real game and submit required reviews with margin.
- **Weeks 7–8:** package, verify and release only when requirements are met.

Steam documents a **30-day wait after app-fee payment for the first few releases**, a publicly visible **Coming Soon page for at least two weeks**, and review before release. For a nominal 5 November launch, aim to have Coming Soon public before 22 October to leave margin, while meeting the fee wait separately. These are minimum requirements, not approval guarantees. Use existing presentation/flex time for this work; cut cosmetic scope if onboarding takes longer.

Sources checked 11 September 2026:
- [Steamworks onboarding](https://partner.steamgames.com/doc/gettingstarted/onboarding)
- [Steam release options](https://partner.steamgames.com/doc/store/types)

Account availability, administrative work and review can push Steam past this timetable. A private or itch.io build can still complete the two-month learning project. This document performs no purchases, uploads or publishing actions.

## 8. Final acceptance checklist

### Playability
- [ ] Fresh launch works from the deliverable package.
- [ ] Start, pause, restart and quit work.
- [ ] Controls shown match supported inputs.
- [ ] Full run reaches an ending without editor shortcuts.
- [ ] Repeated deaths and retries do not trap the player.
- [ ] Checkpoint/save/reset behavior matches the description.
- [ ] Boss defeat and subsequent restart work.
- [ ] Audio controls and shake toggle work.

### Presentation
- [ ] Actor frames intended as 64×64 really have those dimensions.
- [ ] Weapon poses and damage timing agree.
- [ ] Solid ground is distinct from scenery.
- [ ] Camera movement does not obscure fights or cause distracting pixel shimmer.
- [ ] Important information uses shape/motion/sound in addition to color where appropriate.
- [ ] Screenshots and video show the actual build.
- [ ] Credits, asset sources and applicable storefront disclosures are complete.

### Delivery and learning
- [ ] At least one outside person has tested the exact release candidate.
- [ ] The ZIP contains the executable and all required neighboring files.
- [ ] Any uploaded package has been downloaded and checked.
- [ ] A known-good build and recoverable source snapshot exist.
- [ ] You can explain movement, one hit, one enemy state, retry and animation playback.
- [ ] Your portfolio states your role and assistance/assets accurately.
- [ ] Remaining limitations are recorded.

## 9. Daily log — copy this block for each session

```text
Date / session:
Today's smallest finished outcome:
Expected behavior:
What I made:
What actually happened:
What I tested:
Minutes spent (including breaks):
One thing I can now explain:
Current blocker:
Next smallest action:
Backup/build location:
```

## 10. Weekly review — 15 minutes from your final session

```text
Week:
Build/version:
Gate: PASS / FAIL / PARTIAL
Evidence:
Planned 28 hours / actual:
Most useful player observation:
Largest remaining risk:
One thing I will cut:
One skill I learned:
Next week's first concrete task:
```

## 11. How to ask for help and keep learning

Ask one bounded question at a time:
- “In this function, when does damage occur, and how can I delay it to the strike pose?”
- “My sprite jumps sideways between these two frames. Help me inspect its pivot.”
- “Here is the enemy warning and my timing. Why does this hit feel unavoidable?”
- “I am on W3D2. I spent two hours and only have two run poses. What should I simplify?”

When receiving code or artwork help:
1. Identify what changed.
2. Predict the visible result.
3. Test it yourself.
4. Explain it in your daily log.
5. Make a small related change independently.

If you cannot explain a change, schedule a short explanation before adding another system. The portfolio value comes from decisions and understanding as well as the finished build.

## 12. Your first session, made concrete

- [ ] Open the existing 2D learning scene and play a complete run or until the current blocker.
- [ ] Write five scope sentences.
- [ ] Preserve a recoverable source snapshot and the working build.
- [ ] Change one saved tuning value and compare the result.
- [ ] Restore or keep it intentionally.
- [ ] Record what happened and prepare W1D2.

**One successful day is one small outcome you can show, test and explain.**
