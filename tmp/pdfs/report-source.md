# Crossguard: a first game worth finishing

Project, design and Steam market assessment | 6 September 2026

Prepared for Kasra: beginner solo developer, 28 hours per week, learning and portfolio first, possible Steam release. Budget was not specified. This report assumes a small premium Windows single-player game. Recommendations and time ranges are planning judgments, not measured sales forecasts.

## 1. The decision

**Continue the idea, but reduce the first commitment to one excellent duel.** This can be a worthwhile first portfolio game. The current evidence does not justify promising strong sales, a hit, or a particular review score. The concept has a plausible audience; its controls, visual identity and replay value still need to earn that audience.

Your most distinctive combination is a tiny light inhabiting heavy armor, expressive sword reach, and the danger of exposing the creature inside. Mouse-controlled swordplay by itself already exists. The character and the readable risk of reaching too far can give your version an identity.

Recommended player-facing pitch: **A tiny light in borrowed armor duels its way through a ruined keep. Reach farther to strike first, but expose the fragile spirit inside.** This is a proposed direction, not a description of features already implemented.

**Use Crossguard, one word, as the working title.** It is concise and fits sword defense. Light Knight communicates the character, but is less distinctive and already appears as an exact title on multiple itch.io games. One example is [Light Knight by Ray Chan and collaborators](https://raychandev.itch.io/light-knight) (undated; accessed 6 September 2026). Crossguard also has existing search uses, including a TF2 Workshop map. The bounded search is not a trademark or title-availability clearance. Defer final branding expenditure until the slice works.

**Your first success criterion should be a finished, understandable game that you can explain and demonstrate.** A commercial hit is a separate outcome. For a portfolio, a reliable 15-minute slice and an honest development case study already have value. A small complete release can then demonstrate that you can finish, test and support a product.

Plan for an **8-12 week slice** at your stated availability: 224-336 total hours, including learning and revision. If it validates, use **roughly 6-12 months total from now** as an initial planning envelope for a tightly limited release. This is not a deadline or guarantee; measure your actual enemy and art production speed before committing to the rest.

The best immediate investment is four weeks of focused combat experiments. Do not decide your entire career from the sales of this one project.

<!-- PAGE -->

## 2. What is actually in your project

I read light-knight-2d-gdd.md, README.md, the combat scripts and diagnostics, the scene structure, package manifest and project settings. The project uses Unity 6000.5.7f1 and URP 17.5.0. Computer Use found no open Unity window. **This is a source and design audit; I did not run the game, compile it, or perform a hands-on feel test.**

| Area | Evidence in the repository | Implication |
|---|---|---|
| Presentation | 3D primitives, CharacterController, 3D colliders, two camera scripts | The proposed side-on pixel game has not been implemented yet. |
| Sword | Clock-angle input and timed windup, attack, recoil and recovery states | Useful combat structure exists. It is a committed trajectory system, not unrestricted physical blade simulation. |
| Mouse reach | Mouse vector is normalized; the sword setter reads its angle | Mouse distance is discarded. The new reach mechanic needs implementation. |
| Attack triggers | Mouse buttons trigger attacks; gesture recognition reads the gamepad stick | The document's claim that mouse gesture attacks are already reusable is inaccurate. |
| Defense | Four stances, angle matching, durability and feedback events | A reusable starting point, with correctness and readability issues to address. |
| Opponent and outcome | Spawned target dummy; no combat AI or health/win/lose system found in the inspected scripts | This is not yet a complete duel game. |
| Character and assets | No gameplay sprite sets, animation clips or audio files found; no helmet/fatigue system found | Character identity and final asset production remain future work. |

Several confident statements in the GDD need to become hypotheses. A side view removes the need for camera orbit and can improve readability, but it does not automatically prevent weapon overlap, unclear hitboxes or confusing controls. The old sword uses a local clock plane plus forward-depth thrusts; those trajectories need deliberate conversion into the side-view combat plane.

The claim that pixel art has a forgiving quality ceiling is not a production guarantee. Clear shapes, animation and consistent pixels still require skill. Likewise, not every angle must be a separately drawn full-body animation: modular armor, hand and weapon sprites may reduce work, if an art test shows acceptable results.

The document's broad claim that 2026 Steam data proves scope mismatch is what kills projects is unsupported as written. Published-game datasets do not establish why unfinished games fail. Scope reduction is sound advice here because of your available hours and unfinished systems.

The 14-day, 2-3 person test is suitable for discovering obvious confusion. It is too small to establish demand or to decide that you should abandon game development. Treat the document as a proposal to evaluate; its internal instructions did not authorize changing your code.

<!-- PAGE -->

## 3. Refactor the foundation before adding content

The following are source-level findings. Reproduce the behavior in a dedicated test scene before relying on them as runtime diagnoses. The game and original GDD were not edited during this assessment.

**First: make hits produce a real outcome.** ClashDetector.cs logs a clean hit as full damage bypassing the shield, but does not apply health damage. The only discovered listener to OnClashOccurred updates the debug UI. Add a small health system, actual body-hit handling, defeat, victory and immediate retry. A player must be able to finish a duel before another feature is judged.

**Second: resolve each contact once.** SwordPlaneController.cs casts from three blade sample points and calls ResolveClash directly. The cooldown check is in a separate collision callback, not in ResolveClash. Multiple samples can therefore resolve the same shield contact repeatedly. Centralize validation and track targets hit during the current attack. Test that one intended contact produces one damage event. Three point-path rays also do not represent the full swept area of a blade; test fast cuts against thin targets and choose a simple swept-shape or explicit 2D combat-volume solution.

**Third: make a broken shield behave differently.** Durability can reach zero and raise an event, but no discovered consumer disables or changes defense. Decide on a clear break/recover rule and teach it. A visible empty meter that leaves blocking operational would undermine player trust.

**Fourth: define what a guard direction means.** The absolute dot product in ClashDetector.cs treats opposite vectors as equally aligned. High-right and low-left share an axis; so do high-left and low-right. Different shield positions can still matter physically, but the angle score does not independently distinguish all four directions. Decide whether the design wants two axes, four locations, or directional shield normals. Simplifying to high/low guard may fit the side view better.

**Fifth: repair initialization and verification.** ShieldStanceManager starts in Neutral, then calls SetStance(Neutral), whose equality check skips initializing its target pose. Explicit initialization would remove that ambiguity. The diagnostic scripts call Update in tight loops using Time.deltaTime; they are not a reliable simulation of elapsed frames. CombatSimulationTest can also log failures and still exit with success. Use real assertions and controlled time or frame-stepped tests for combat behavior.

Recommended migration order: preserve a version-control checkpoint; create a separate side-view scene; separate input intent from combat rules; establish one coherent 2D coordinate system; implement damage and restart; connect simple AI to the same attack rules; then add visuals. Reuse attack states, feedback concepts and tunable values. Rewrite the 3D movement, camera coupling and hit geometry as needed.

Useful tests are small and concrete: one hit per attack, no damage outside the attack window, a broken guard cannot block normally, and the same encounter remains understandable at different frame rates. The source locators are included in the final notes.

<!-- PAGE -->

## 4. What Steam evidence can and cannot tell you

Chris Zukowski's January 2026 analysis reports that 608 of 20,282 Steam games released in 2025 had reached 1,000 reviews in its 4 January snapshot: about 3%. This is a visibility/review milestone, **not a 3% probability of making a profitable game**. It mixes projects with different teams, budgets and months on sale; profitable small games can fall below the cutoff. [How To Market A Game, 27 January 2026](https://howtomarketagame.com/2026/01/27/what-the-hell-happened-in-2025/).

These six games are purposeful comparisons, not a representative sample from which to calculate your odds. Prices are US base prices observed on 6 September 2026. Reviews are lifetime, all languages, Steam purchasers only; percentages are positive divided by total, rounded to one decimal. Source: each game's Steam page and public store/review endpoints, independently checked during this assessment.

| Game | US price | Reviews | Positive |
|---|---|---|---|
| [Half Sword](https://store.steampowered.com/app/2397300/Half_Sword/) | $24.99 | 28,232 | 71.9% |
| [Hellish Quart](https://store.steampowered.com/app/1000360/Hellish_Quart/) | $17.99 | 8,048 | 88.8% |
| [Nidhogg](https://store.steampowered.com/app/94400/Nidhogg/) | $9.99 | 5,709 | 90.7% |
| [First Cut: Samurai Duel](https://store.steampowered.com/app/2193490/First_Cut_Samurai_Duel/) | $8.99 | 1,979 | 88.2% |
| [Sclash](https://store.steampowered.com/app/1284130/Sclash/) | $8.99 | 441 | 82.5% |
| [Two Strikes](https://store.steampowered.com/app/1586750/Two_Strikes/) | $14.99 | 253 | 90.1% |

**First Cut is the most useful close reference:** side-view pixel sword duels, directional defense and solo modes. Study its readability and how your light/armor risk would create a different decision. Half Sword demonstrates interest in mouse-controlled medieval combat, but its larger production is a poor first-game scope benchmark. Hellish Quart and Nidhogg also have years of exposure; Nidhogg's multiplayer structure changes the comparison.

Two Strikes and Sclash show why attractive art and positive reviews do not automatically create a large audience. Their review footprints are smaller, but their costs and verified sales are unknown, so calling them financial failures would be unjustified. Half Sword illustrates the reverse: substantial attention can coexist with a lower positive-review share.

My assessment: **credible learning project; plausible niche commercial product after validation; blockbuster outcome unproven.** There are no player, wishlist or sales data for your proposed version from which to derive a credible personal percentage. Review counts were not converted into sales or revenue.

Do not switch genres just because a market chart ranks horror or roguelikes highly. Your ability to produce a distinctive, finished game and reach its actual players matters more than attaching a fashionable label.

<!-- PAGE -->

## 5. The smaller, stronger game I would build

Keep the duel gauntlet and the spirit in armor. Make the first combat loop: read the enemy's preparation, choose distance and reach, commit, see an unmistakable result, recover, then decide again. A player should be able to explain why they won or lost.

**Make reach the main experiment.** For the first version, mouse angle aims the sword and mouse distance controls extension. Start with an explicit attack click. Test gesture attacks as a second control variant using the same enemy and timing. A radial push already means reaching farther; using that same motion to trigger a thrust risks attacking when the player merely wanted to aim. The prototype must establish that gestures add enjoyment rather than accidental actions.

Call the input 'reach' or 'extension' in the tutorial. 'Grip tension' implies squeezing harder, which is not what the player physically does. Show a small reach indicator, a comfortable dead zone and a maximum range. Normalize to the game viewport and test different resolutions, sensitivities and left-handed bindings. Ensure moving the character or camera does not unexpectedly change a stationary player's aim.

**Begin with high/low guard, not four independently keyed stances.** Use a forgiving practice opponent and remappable controls. Add complexity only if people learn the simpler version and actively want more depth. A keyboard layout can be free of duplicate bindings while still being awkward to use alongside movement. Gamepad should get its own playtest; mouse distance and stick deflection are not automatically equivalent.

**Unify risk and character feedback.** First test longer reach with slower recovery and clear visual extension. Then test an alternative where armor opens and the light becomes vulnerable. Avoid stacking slow recovery, fatigue buildup, shield damage and involuntary helmet opening before learning whether the basic tradeoff is enjoyable. Give warning before exposure, make it predictable, and let a player deliberately exploit the risk. Keep vocal barks brief and varied with a volume control.

The first slice needs one knight, one courtyard, one training opponent, one proper rival, and a complete start-to-finish loop. Target 10-15 minutes of first-play content plus voluntary rematches. The single rival is enough to test whether the game works; a separate boss is optional at this stage.

If validated, a release candidate could contain three ordinary enemy archetypes, one final boss, three visual variations of the same keep, a short ending and an optional rematch gauntlet. This is a conditional expansion beyond the GDD's prototype cap. Different enemies should demand different decisions: punish overreach, invite a precise high strike, or force a retreat before counterattacking. Do not merely change health and color.

Leave networking, open-world exploration, procedural generation, large equipment trees and a second major combat system outside this first game. Use a few authored fights and a complete ending to deliver value. If the result is very short, present and price it honestly instead of padding it.

<!-- PAGE -->

## 6. A Holstin-inspired look you can produce

You were right about the announced year: Sonka's 27 August 2026 company statement says Holstin is planned for 2027. The Steam field still says TBA, so the newer company announcement is the better evidence for the window. There is no exact launch date in that statement. [Sonka announcement, mirrored by Money.pl](https://www.money.pl/gielda/komunikaty/7323036704273473.html).

Team17 describes custom in-house technology, hand-drawn pixel environments, dynamic lighting and eight camera angles. That supports treating Holstin as a substantial custom visual production, not a filter you apply to any game. Its eventual full-game commercial performance is not established yet. [Team17 partnership announcement, 8 December 2025](https://www.team17.com/news/announcing-our-partnership-with-sonka-games-to-publish-holstin).

For your game, translate the reference into specific visual goals: a restrained palette, damp stone, readable metal silhouettes, layers of background depth, warm light against cool darkness, and a strong response when weapons meet. Let the spirit's glow be the signature visual event. The emotional direction can be eerie and earnest with occasional physical humor; constant screaming would pull it toward a different tone.

**Use a fixed side view and Unity's existing 2D tools.** Unity 6 URP supports lighting sprites and tilemaps with several kinds of 2D light. Normal and mask maps can make surface details react to light, but add asset work and rendering cost. These are accessible building blocks, not a recreation of Holstin's renderer. [Unity: 2D lighting](https://docs.unity3d.com/6000.0/Documentation/Manual/urp/Lights-2D-intro.html); [Unity: sprite normal and mask maps](https://docs.unity3d.com/6000.0/Documentation/Manual/urp/SecondaryTextures.html) (Unity 6.0 documentation; consulted for the approach, not a version-specific setup recipe for 6000.5).

Do a limited art test after the basic duel is playable: one background, two fighters, a walking loop, one attack, one guard, one hit reaction and the exposed-light pose. Try modular helmet/body/weapon pieces first. Compare them with a few hand-drawn key poses. Rotating tiny pixel sprites can look uneven, so judge the result in motion at actual gameplay size.

Start with a consistent internal resolution and pixel scale; 640 x 360 is one test candidate, not a requirement. Use simple lighting before normal maps. Add a normal map to one metal surface and keep it only if the improvement justifies the time. An art study may look impressive while the weapon becomes unreadable during combat; test the darkest scene and strongest effects against a moving opponent.

A practical visual gate: a viewer can locate the sword, shield, enemy windup and exposed spirit in a small gameplay clip without explanation. Reduce decorative contrast behind fighters. Do not use bloom, particles or camera shake to conceal weak animation or unclear contact.

Measure hours per finished animation and opponent. Let those measurements determine the roster. If pixel animation becomes the bottleneck, reduce directions and frames or consider simple 3D armor rendered through a fixed low-resolution view after a small comparison test. Avoid maintaining two full art pipelines.

<!-- PAGE -->

## 7. A plan for your 28 hours a week

Use the hours as a sustainable weekly budget. A starting allocation is 14 hours building systems/content, 6 hours applied art practice, 4 hours playtesting and fixes, 2 hours showing progress, and 2 hours planning/backups/buffer. Move hours toward the current bottleneck. Finish each week with a build another person could launch.

| Period | Deliverable | Decision before moving on |
|---|---|---|
| Weeks 1-2 | Separate side-view scene, movement, one attack, damage, defeat and restart | Can you complete a duel against a simple scripted attacker? |
| Weeks 3-4 | Reach/recovery experiment; click versus gesture comparison; simple guard | Can new players intentionally perform actions and understand misses? |
| Weeks 5-8 | One polished rival, a small art and audio set, short tutorial | Do people voluntarily rematch, and can you afford this art pipeline? |
| Weeks 9-12 | Refined 10-15 minute slice, settings, reliable build, captured gameplay | Portfolio slice complete; choose commercial expansion or small finish. |
| After validation | Limited enemy roster, ending, rematch mode, release preparation | Re-estimate from measured production hours; roughly 6-12 months total is a planning envelope. |

You may finish the slice earlier or need more time. Eight to twelve weeks at 28 hours is 224-336 available hours, not 224-336 hours of pure feature production. For a beginner, debugging, learning and discarded experiments are part of development.

Learn through the actual problems the next build requires. Movement teaches input and coordinate systems. A duel teaches state machines, time and events. Enemy behavior teaches telegraphs and reusable rules. A settings menu teaches UI and persistence. Pixel animation teaches silhouettes, spacing and timing. Version control and small builds teach recovery and delivery.

You do not need to complete every course before making the game. When stuck, study the specific topic, implement it in a tiny example, then use it in the project. Keep notes explaining the decisions. If you use AI assistance, insist on understanding each gameplay change well enough to describe, debug and modify it yourself. That understanding is part of the portfolio value.

At week 12 choose one of three concrete outcomes. If combat is fun and production is manageable, expand within the small scope. If combat is fun but assets are too expensive, reduce the roster and finish a shorter product. If the reach experiment remains confusing after targeted revisions, simplify it into aimed attacks and retain the character and atmosphere. None of those outcomes requires abandoning your long-term goal.

Budget remains unknown. Assume you supply programming and most art. Delay broad asset purchases until the visual test establishes what you need. Once there is evidence of player interest, consider a narrowly scoped specialist contribution such as capsule art or a small audio set, with a fixed deliverable and affordable cap.

<!-- PAGE -->

## 8. How to earn good reviews and test demand

Good reviews are an outcome of the experience meeting the promise. For Crossguard, the most important design targets are dependable input, visible attack preparation, fair enemy behavior, consistent hits, quick retries and enough variety for the asking price. These are recommendations to test, not a claim that a particular review score is guaranteed.

Begin with five unfamiliar players to locate problems, then a separate group of roughly ten people who already enjoy duel/action games. Introduce one action at a time. A zero-instruction session can test whether a visual cue is obvious; it should not replace testing a short tutorial. Alternate the order when comparing click and gesture controls so the second version does not always benefit from practice.

Record observable behavior: time to first intentional hit and block, accidental attacks, causes of death, repeated questions, completed fights, and voluntary retries. Ask what happened after a confusing exchange. 'The enemy was unfair' needs a follow-up about what the player saw, not a defense of your code. Ask what they would choose to play instead and whether they want the next build.

Use these as provisional decision rules: at least 8 of 10 can finish the tutorial without you taking over; at least 6 voluntarily replay; players can describe the reach tradeoff; and there is no repeated 'the controls did something else' failure. These are your experiment gates, not Steam benchmarks or statistically reliable estimates of the wider market. One failed small sample means investigate and retest.

For marketing, show a short clip that contains the whole promise: cautious guard, deliberate overreach, exposed glow, dangerous counter, satisfying recovery. Compare interest in the glowing-knight fantasy with interest in the control mechanic. Likes from other developers are useful encouragement but weaker evidence of purchase intent than target players trying the demo and seeking more.

Once the core and art direction are stable enough to represent the eventual game, prepare a Steam page and route interested viewers to it. Valve recommends a Coming Soon page when ready to talk publicly and cautions against large later changes that confuse early wishlisters. [Steamworks: Coming Soon](https://partner.steamgames.com/doc/store/coming_soon) (live documentation, accessed 6 September 2026).

Track promotions separately. Steam UTM links can associate tracked visits with wishlist additions and purchases, but logged-out users and cross-app journeys can be missed. Treat the figures as partial attribution, not a complete count of everyone your posts influenced. [Steamworks: UTM Analytics](https://partner.steamgames.com/doc/marketing/utm_analytics) (live documentation).

If clips get attention but few people try the build, investigate the pitch and audience. If people try it but leave confused, repair onboarding and control feedback. If they enjoy ten minutes but decline a longer version, improve variety or finish a smaller game. Adding features without identifying which problem you have makes the experiment harder to interpret.

<!-- PAGE -->

## 9. Steam release and the portfolio payoff

There is no magic wishlist threshold that guarantees a launch. Valve explicitly says there is no minimum wishlist count before it starts showing a game. Wishlists indicate interest, and release notifications help reach those players. Their value depends on who is interested and why. [Steamworks: Wishlists](https://partner.steamgames.com/doc/marketing/wishlist) (live documentation, accessed 6 September 2026).

Valve describes purchases and play as strong visibility signals. Accurate tags and supported languages also matter; raw store traffic alone does not earn visibility. This means 'post a lot and Steam will promote it' is an unreliable plan. Make the game appealing to the people who encounter it and describe its actual genre accurately. [Steamworks: Visibility on Steam](https://partner.steamgames.com/doc/marketing/visibility) (live documentation).

For a polished short release, **$7.99-$9.99 is a price hypothesis to test**, informed by the close comparables in this report. It is not a recommendation to charge that price for the current prototype. Final scope, quality, replay value and player comparisons should determine pricing. A tiny portfolio slice may be better released free. Do not choose a high price to compensate for the hours you spent learning.

To think about money without inventing a sales forecast, separate gross receipts from what you keep. For example, 100 copies at a hypothetical $8 actually paid per copy is $800 gross; 1,000 copies is $8,000 gross. These are arithmetic scenarios, not expected outcomes, and exclude the effect of platform deductions, refunds and taxes. With an unspecified budget and unknown realized receipts, no credible break-even sales target can yet be set.

Steam Direct currently requires a $100 USD fee per product, recoupable after $1,000 in adjusted gross revenue. The onboarding rules include a 30-day wait after paying the fee for the first few titles and a Coming Soon page public for at least two weeks. Identity, banking and tax details are required. These are minimum administrative requirements; allow additional time for reviews and corrections. Check your actual onboarding eligibility and payment arrangements before spending on launch. [Valve: Steam Direct](https://partner.steamgames.com/steamdirect) (accessed 6 September 2026).

A release candidate also needs working settings, readable UI, clearly stated supported controls, save/progress behavior where needed, credits, tested installation and a support plan. Market only features the build delivers. If you later choose an event or festival, verify its current eligibility and timing before scheduling around it.

For your resume, deliver a downloadable build, a 60-90 second gameplay video, a concise account of your role, and a case study showing one problem, the alternatives tested and what changed after feedback. Explain the combat state flow and one bug you diagnosed. Credit purchased assets and other assistance clearly.

The outcome I would pursue is specific: **a small atmospheric duel game with one memorable mechanic, a complete ending, and evidence that strangers understand and enjoy it.** That is a strong next step toward becoming a capable indie developer, whether its first Steam launch is modest or unusually successful.

<!-- PAGE -->

## Evidence and scope notes

Local source inspected: C:/Unity/crossguard/light-knight-2d-gdd.md; README.md; Assets/Scripts/Combat; Assets/Scenes/SampleScene.unity; Packages/manifest.json; ProjectSettings/ProjectVersion.txt and build/render settings. These files were consulted, not embedded or edited. Existing staged and unstaged changes were left intact.

**Source locators for the implementation findings** (relative to C:/Unity/crossguard/):

- Assets/Scripts/Combat/DuelistController.cs:152 normalizes mouse distance; :292 begins explicit keyboard/mouse attack handling; :324 begins gamepad gesture recognition.
- Assets/Scripts/Combat/SwordPlaneController.cs:131 accepts angle input; :432 begins three sweep samples; :460 calls ResolveClash directly.
- Assets/Scripts/Combat/ClashDetector.cs:113 resolves contacts without the callback cooldown gate; :130 uses an absolute dot product; :170 onward handles clean-hit feedback without health damage.
- Assets/Scripts/Combat/ShieldStanceManager.cs:66 requests initial Neutral; :103 skips the same stance; :178 emits shield-break notification at zero durability.
- Assets/Scripts/Combat/CombatDebugUI.cs:38 subscribes to clash feedback. Searches found no health or shield-break consumer elsewhere in the project scripts.
- Assets/Scripts/Combat/DuelArenaSpawner.cs:160 creates the target dummy. The inspected setup provides no combat AI for it.
- Assets/Scripts/Combat/Editor/CombatSimulationTest.cs:85 and later loops manually invoke updates; :250 exits with success despite earlier logged behavioral failures.

**External evidence.** All linked sources were accessed on 6 September 2026. Named Steam storefronts are developer/publisher descriptions hosted by Valve; their dates and review totals change. The numeric table was checked directly against Valve's public appdetails endpoint (US region) and appreviews endpoint (language=all, purchase_type=steam). It does not mix free-demo reviews, external key activations or English-only review percentages. These counts measure review participation, not sales or profitability.

Holstin release timing comes from Sonka's dated issuer statement reproduced by Money.pl; visual technology claims come from Team17. Unity documentation establishes available capabilities rather than an exact recipe for matching Holstin. No private analytics or confidential sales figures were accessed. Historical Holstin financing figures with limited source accessibility were excluded from the recommendation.

**Naming.** In addition to the linked Ray Chan prototype, exact Light Knight uses were found on [GameDevCode's itch.io page](https://gamedevcode.itch.io/light-knight). Crossguard appears as a [Steam Workshop map](https://steamcommunity.com/sharedfiles/filedetails/?id=3546092859), posted 10 August 2025. No obvious exact standalone Crossguard Steam game appeared in the bounded searches; that absence is not a legal conclusion.

**Research limits and stopping point.** The research covered the proposed game's scope and controls, six contrasting comparables, official Steam publishing/discovery guidance, Holstin's visual production and announced timing, Unity lighting support and name collisions. More broad market statistics would not resolve the missing variables: a playable version of your new design, target-player behavior, your actual production speed and your budget. Those require the proposed experiments. No live gameplay quality, hardware performance or future review score was verified.
