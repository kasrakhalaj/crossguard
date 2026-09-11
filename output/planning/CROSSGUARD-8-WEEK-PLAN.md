# Crossguard — eight-week learning and release plan
11 September–5 November 2026 • 28 hours/week • 224 hours total

## The intended result
A small, complete Windows 2D sword-and-shield game: one compact map, three adjoining spaces, one hero, two regular enemy types, one boss, checkpoints and an ending. Aim for a natural 10–20-minute first clear; a shorter satisfying game is acceptable. This is a scope target, not a guarantee of production speed or sales.

The plan assumes continuing Crossguard's fully 2D pixel direction. Shroom and Gloom is an art-direction reference, not a decision to switch to a first-person deckbuilder. Use the existing Mosslight systems where useful; the older 3D Crossguard arena is not the foundation for this eight-week sprite build.

## Boundaries
- Working title: Crossguard. Do not spend these weeks on repeated renaming.
- One chosen palette: ENDESGA 64 initially; change at the week-three art review only if necessary.
- Actual character/enemy frames: 64x64. That does not mean every environmental object must occupy 64x64; build large architecture from modules and use larger composed backgrounds where needed.
- One weapon loadout, forward shield block, readable attack/recovery. Timebox the older mouse-driven sword/grip idea in week one and keep a simpler fallback.
- No multiplayer, open world, inventory economy, crafting, procedural generation, class system or new engine.
- No new combat systems after week two; no new features after week five.
- Existing generated art is placeholder/reference until you have simplified and inspected it. Palette conversion alone is not final art.
- Commercial launch and good reviews are not guaranteed. A finished learning/portfolio game is the primary outcome.

## Existing assets to reuse
- [Learning prototype guide](../../Assets/Mosslight/START_HERE.md).
- [2D learning scene](../../Assets/Mosslight/Scenes/Mosslight.unity).
- [ENDESGA sprite variants](../ashen-sprites-paletted/endesga-64).
- [Original design brief](../../light-knight-2d-gdd.md).
- [Art-direction review](../art-direction-review/ART-DIRECTION-REVIEW.md).

## Weekly working pattern
Five four-hour work sessions plus one eight-hour day split into two blocks gives 28 hours, with one full day off. Move days around your actual commitments.
A typical four-hour session: 15 minutes choose one concrete outcome; 45 minutes targeted learning; 2 hours implementation/drawing; 45 minutes play, compare and correct; 15 minutes save and record the next step. When you already know the technique, use the learning time for production.
The eight-hour day should be two four-hour blocks with a substantial break between them. Use one for finishing the weekly milestone and one for playtesting, packaging and buffer work.
Each week allocates 24 planned hours plus 4 hours of buffer. Total buffer: 32 hours. Unused buffer improves the weakest existing feature; it does not authorize new features.
Short study is included in each week's hours. Do not add a second curriculum on top of the plan.

## Weekly milestones
| Week | Dates | Milestone |
|---|---|---|
| 1 | 11–17 September | Choose the game and make one rough fight |
| 2 | 18–24 September | Make combat readable and satisfying |
| 3 | 25 September–1 October | Establish a small art style you can reproduce |
| 4 | 2–8 October | Build the complete small map and its atmosphere |
| 5 | 9–15 October | Finish the full game loop and boss |
| 6 | 16–22 October | Playtest and fix what people actually struggle with |
| 7 | 23–29 October | Make a release candidate and presentation package |
| 8 | 30 October–5 November | Ship the small game and document what you learned |

## Week 1 — Choose the game and make one rough fight
11–17 September • 28 hours

### Work budget
| Activity | Hours |
|---|---:|
| Scope, baseline build and backup | 4 |
| Learn the existing controller and hit detection | 4 |
| Tune movement and camera framing | 5 |
| Build one greybox fight; timebox sword-control experiment within this block | 7 |
| Playtest, fix the biggest issue and export a build | 4 |
| Buffer | 4 |
| Total | 28 |

### Learn
Read one action from input to movement or damage. Learn transforms, Rigidbody2D, colliders, input and the difference between sprite visuals and hitboxes.

### Build
Write a one-page brief. Working title Crossguard. Keep side-view XY movement. Play the existing Mosslight build and list what can be reused. Make a copy/branch for the new prototype; keep the original playable. Use one small room and one enemy. Spend at most four of the seven combat hours testing mouse-driven sword aim with a clear temporary visual. If it needs major re-engineering or remains hard to understand, use a fixed attack with a readable arc for this eight-week release. Record that decision; don't revisit it mid-production.

### End-of-week check
A standalone build opens, lets you move, attack, take damage, die and retry. You can explain the code path for one hit. Sword-control scope is settled.

### If behind
Use the existing movement and camera. Cut free-angle aiming and grip-tension first. Do not start final animation until the attack behavior is settled.

## Week 2 — Make combat readable and satisfying
18–24 September • 28 hours

### Work budget
| Activity | Hours |
|---|---:|
| Learn state machines and attack timing | 3 |
| Implement a simple forward shield block and optional timed counter | 7 |
| Adapt two regular enemy behaviors | 6 |
| Add attack warnings and impact feedback | 4 |
| Observe two playtests and fix issues | 4 |
| Buffer | 4 |
| Total | 28 |

### Learn
Learn enemy states: wait, prepare, attack, recover. Understand facing checks, cooldowns, invulnerability windows and keeping damage separate from effects.

### Build
Use one slow committed attacker and one faster approach attacker. Reuse existing behavior where practical. The shield protects the front; keep directional stances out of this version. If basic blocking works early, test one signature moment: a well-timed block creates a brief counter opportunity. No additional skill tree or meter is required. Make attacks readable using pose, movement and sound rather than color alone. Keep the existing dash only if it serves the chosen combat; do not expand it.

### End-of-week check
A new player can understand an enemy warning, use the shield, win one fight and identify why they died. The core fight is enjoyable with placeholder art.

### If behind
If adding a timed counter makes combat unstable, keep basic block. If both enemies are late, make one enemy reliable and reserve the second for spare time. No new combat systems after this week.

## Week 3 — Establish a small art style you can reproduce
25 September–1 October • 28 hours

### Work budget
| Activity | Hours |
|---|---:|
| Learn Aseprite frames, onion skin and consistent pivots | 3 |
| Choose silhouette and material rules | 3 |
| Clean and animate the hero | 8 |
| Make first enemy's essential visual states | 4 |
| Import art and build one visual test room | 4 |
| Review at actual size and simplify | 2 |
| Buffer | 4 |
| Total | 28 |

### Learn
Practice silhouette, shadow clusters, small color ramps, aligned frames and readable poses. Use the downloaded ENDESGA 64 as the initial palette; Resurrect 64 remains a comparison, not a second simultaneous production palette.

### Build
Keep character and enemy frames exactly 64x64. Use the existing generated sprites as references/placeholders; they are not polished animation sheets. Redraw/simplify the hero so you can reproduce it. Start with about 3 idle frames, 6 run frames, 4 attack poses, one block pose, one hurt pose and one defeated pose; reuse jump/fall poses if platforming remains. These are caps/starting suggestions, not a demand for 16 highly detailed drawings. Keep feet and hands aligned across frames. Put sword/shield on separate layers while drawing. Frame-by-frame weapon movement follows the combat scope chosen in week one. Give the character one distinctive silhouette feature and one recognizable gesture.

### End-of-week check
One original or substantially redrawn character can move and fight in the build. You can redraw its standing pose without tracing. A still frame has a clear actor and floor.

### If behind
Reduce armor detail, idle frames and cosmetic motion. Preserve preparation, strike and recovery poses. If a polished frame takes over an hour, simplify before multiplying it into a full set.

## Week 4 — Build the complete small map and its atmosphere
2–8 October • 28 hours

### Work budget
| Activity | Hours |
|---|---:|
| Learn sorting layers, parallax and tile repetition | 2 |
| Create a reusable environment kit | 6 |
| Stage depth and lighting | 5 |
| Assemble three connected spaces | 6 |
| Test navigation and foreground occlusion | 3 |
| Choose and test the sound atmosphere | 2 |
| Buffer | 4 |
| Total | 28 |

### Learn
Understand overlap, consistent pixel scale, contact shadows, distant contrast and moving background layers at different speeds.

### Build
Use one ruined pilgrimage fortress as the proposed location: an outer approach, a sheltered hall and a final causeway arena. Reuse stone, pillar, arch, lantern, banner and shrine modules. Keep world lore to a short premise and one visible mystery with an eventual payoff. Give scenery depth with near/middle/far layers and painted thickness. Keep the camera side-on. Begin with painted shadows and a few light sources; normal maps, reflective water and elaborate weather are optional later. Create one small optional side alcove only if the main path is complete. Record material-appropriate sound references and use original or appropriately licensed audio.

### End-of-week check
The complete map can be traversed from entrance to the final arena. Players can distinguish solid floor from background architecture. One landmark makes each area recognizable.

### If behind
Remove the optional alcove, moving weather and decorative props. Reuse one background. Never hide a collision problem with scenery.

## Week 5 — Finish the full game loop and boss
9–15 October • 28 hours

### Work budget
| Activity | Hours |
|---|---:|
| Learn boss state sequencing and simple persistence | 2 |
| Adapt one boss with two clear attacks | 8 |
| Implement ending, checkpoint progression and minimal save/reset | 5 |
| Run the entire game and tune difficulty | 4 |
| Replace the most distracting placeholder sounds | 3 |
| Fix completion blockers | 2 |
| Buffer | 4 |
| Total | 28 |

### Learn
Reuse the existing boss architecture. Learn predictable attack selection and saving only necessary progress with a clear new-game/reset path.

### Build
Make one boss that tests the same attack/block lessons as ordinary fights. Two attacks and clear recovery windows are enough; a faster second phase is optional only if the base fight works. Connect entrance, encounters, checkpoint, boss, visual mystery payoff, ending and restart. Save only checkpoint/boss completion if using persistent progress; avoid inventory serialization. Complete or reuse the second enemy's essential visual states within the boss/art work. Add a small number of readable hits, shield impacts and environment sounds. Credits begin here with a running list of asset sources.

### End-of-week check
Content complete: someone can launch the game, reach and defeat the boss, see an ending, quit and restart without becoming stuck. All required encounters exist.

### If behind
Cut a boss phase, the second enemy if still unfinished, and optional secrets. If a 10–20-minute first clear is not supported naturally, release a shorter complete game; don't pad it.

## Week 6 — Playtest and fix what people actually struggle with
16–22 October • 28 hours

### Work budget
| Activity | Hours |
|---|---:|
| Observe 3–5 external playtests | 5 |
| Fix crashes, progression failures and confusing interactions | 8 |
| Tune combat and camera feel | 5 |
| Improve art/UI readability | 4 |
| Export and check the new build | 2 |
| Buffer | 4 |
| Total | 28 |

### Learn
Learn to distinguish a bug, a communication failure and an intentional challenge. Observe before explaining controls.

### Build
Use people who have not watched you build it, if available. Let them begin without a spoken tutorial. Record where they stop, which attacks seem unfair, first successful block, accidental damage, completion and comments. Ask what they believed caused a death and what they wanted to explore. Prioritize blocker bugs, then readability, then tuning. Check the game at normal display size rather than only in the editor. Do not treat a tiny sample as market proof.

### End-of-week check
The latest build is completable by outside players. Major failures from the first tests are fixed and rechecked. No new features enter the schedule.

### If behind
Remove an encounter or visual effect that causes disproportionate confusion. Fix the underlying combat feedback before adding more tutorials.

## Week 7 — Make a release candidate and presentation package
23–29 October • 28 hours

### Work budget
| Activity | Hours |
|---|---:|
| Polish existing menus, controls and options | 5 |
| Verify save/reset and settings behavior | 4 |
| Test clean builds on another machine or account | 5 |
| Prepare store or portfolio page materials | 4 |
| Finish credits and asset records | 3 |
| Capture real gameplay and screenshots | 3 |
| Buffer | 4 |
| Total | 28 |

### Learn
Learn the difference between an editor session and a distributable build. Practice describing a finished game's actual behavior.

### Build
Verify start, pause, restart, quit, audio levels and screen-shake toggle. Keep existing gamepad support only if you can test it; don't promise unsupported inputs. Check focus loss, repeated deaths, checkpoint reload, boss completion, windowed/fullscreen display and fresh launch. Prepare 4–6 gameplay screenshots and a short 20–40-second clip from the actual build. Describe your contribution clearly in portfolio notes, including generated asset assistance and subsequent edits. If using a storefront, complete its applicable content and asset disclosures accurately. Don't present concept boards as gameplay screenshots.

### End-of-week check
A release candidate ZIP runs outside your development setup. Screenshots and description match the build. Credits and sources are recorded.

### If behind
Cut achievements, localization, extra graphics options, a cinematic trailer and nonessential platform integrations.

## Week 8 — Ship the small game and document what you learned
30 October–5 November • 28 hours

### Work budget
| Activity | Hours |
|---|---:|
| Final cold-start and completion checks | 5 |
| Fix release-blocking defects | 7 |
| Finish portfolio case study | 4 |
| Package and publish through the chosen route | 4 |
| Write postmortem and archive the project | 4 |
| Buffer | 4 |
| Total | 28 |

### Learn
Learn release discipline: finish, verify, package, describe and maintain one known-good build.

### Build
Test the exact package that will be delivered. Include clear controls, version, credits and contact/feedback route. Use a private downloadable build or itch.io if Steam is not ready; choose free versus paid based on the actual result. Keep a known-good build and a reproducible source snapshot. Write a short case study explaining your role, design decisions, problems solved, playtest changes and remaining limitations. Reserve the defect block for fresh launch, save and completion problems rather than adding content.

### End-of-week check
There is a playable, finishable downloadable game, a portfolio entry and a source backup. A Steam release is included only if the game and account/page/build requirements are ready.

### If behind
Delay the storefront launch rather than shipping a broken build to meet a calendar date. Deliver the finished private portfolio build while platform review proceeds.

## How to learn without handing the project over
For each change, write: what I expect, what happened, what I changed next.
Ask for explanations of one file or one behavior, then implement a small part yourself. If using assistant-written code, identify inputs, outputs and the place where the behavior changes before accepting it.
By the end, be able to explain input → movement, attack → hit detection → damage, enemy warning → attack → recovery, sprite frames → animation, checkpoint → retry, and build → distribution.
Use tutorial lessons to answer a current obstacle, then close the lesson and reproduce the result in your own project.

## Scope-reduction order
1. Decorative effects and extra props.
2. Extra idle frames and armor ornament.
3. Optional alcove/secret.
4. Extra boss phase.
5. Second normal enemy type.
6. Map length, preserving a beginning, at least one teaching encounter, a boss and an ending.
Preserve clear controls, accurate collision, readable damage, retry and the ending.

If more than one week behind, spend the next session reducing scope. Do not solve overruns by removing every day off or starting a new art pipeline.
At week two, a bad core fight gets tuning time at the expense of week-three art detail.
At week five, an incomplete full loop gets time at the expense of optional content, not external testing.
At week seven, persistent crashes or progression blockers mean private testing continues; the public release date moves.

## Steam as an optional parallel route
If Steam release within these eight weeks matters, inspect onboarding requirements in week one, decide whether to proceed by week two, and start preparing page materials from actual work around weeks four and five. Use the allocated scope/presentation time and reduce cosmetic tasks if platform setup takes longer.
Valve documents a 30-day wait after app-fee payment for the first few releases, a publicly visible Coming Soon page for at least two weeks, and review before release. Treat these as minimum conditions, not an approval deadline.
- [Steamworks onboarding](https://partner.steamgames.com/doc/gettingstarted/onboarding)
- [Release options](https://partner.steamgames.com/doc/store/types)
If aiming for the 5 November end date, get Coming Soon live earlier than 22 October to leave margin, and satisfy the fee wait separately. Submit page/build reviews early enough to fix feedback. Actual approval and account status can move release beyond this plan.
No accounts, fees, public posts or uploads are being performed by this planning document.

## Release checklist
- [ ] Fresh launch reaches an understandable start screen.
- [ ] Controls are visible and match the actual supported inputs.
- [ ] A full run reaches an ending.
- [ ] Repeated deaths and checkpoints do not trap the player.
- [ ] Save, reset and new-game behavior are understandable.
- [ ] Audio can be adjusted and shake disabled.
- [ ] Pixel scaling and combat readability are checked at normal display sizes.
- [ ] Another person has tested the packaged build.
- [ ] Credits, asset provenance and applicable storefront disclosures are complete.
- [ ] Actual gameplay screenshots match the delivered build.
- [ ] A known-good package and source snapshot are backed up.
- [ ] Portfolio description identifies what you made and what help/assets you used.

## First four-hour session
1. 30 minutes: play the current Mosslight build and record what works.
2. 30 minutes: write the scope in five sentences.
3. 30 minutes: make a recoverable source snapshot and confirm a baseline build.
4. 90 minutes: change one movement or combat setting yourself and compare.
5. 30 minutes: record a short before/after clip.
6. 30 minutes: write the next three tasks and save your work.

## Weekly review template
- Build/version:
- One completed player-visible improvement:
- One behavior I can now explain:
- Hours actually spent:
- Biggest remaining problem:
- What the playtest showed:
- What I am cutting:
- Next week's single milestone:
