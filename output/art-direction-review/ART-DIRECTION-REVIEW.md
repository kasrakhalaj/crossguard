# Crossguard / Light Knight — art direction review
Reviewed 9 September 2026. Proposed refinements, not approved lore or an implemented game change.

## Conclusion
Keep fully 2D side-view gameplay. Pursue convincing depth through drawn volume, staged layers, consistent lighting and environmental storytelling. The current palette is serviceable; the larger weaknesses are a generic setting, inconsistent visual targets and missing depth behavior in the learning prototype.

## Scope and evidence
Inspected the current GDD, project README, Mosslight guide, runtime art/world/camera/shader/effects/audio code, package manifest, build-scene settings, the starter palette and generated reference board, and saved garden/boss QA screenshots. The screenshots are previously captured views, not a fresh live playtest. No gameplay or rendering files were changed during this review.

There are three distinct things in this folder:
1. Legacy Crossguard arena: 3D primitive actors and perspective-camera code remain in Assets/Scripts/Combat. SampleScene is still listed as the default build scene. This is repository history, not the requested future direction.
2. Mosslight learning prototype: fully 2D runtime sprites and physics, a garden and a Bellkeeper boss.
3. Crossguard pixel starter board: a proposed medieval palette and reference image, not integrated sprite assets.

The GDD's 2D pixel direction matches the latest user request. Its older scope and schedule statements were treated as reference material, not instructions to restart or implement systems.

## What works
- Steel versus amber gives the hero a useful color relationship.
- A small light inhabiting closed armor is readable with very few pixels.
- Simple weapons and a limited material vocabulary are realistic practice subjects.
- Mosslight already has readable ground edges, an orthographic camera, checkpoints, and a recognizable arena landmark.
- One compact location permits repeated modules without needing many biomes.

## What needs refinement
### Starter board
The helmet, sword, bow, crates and banners communicate medieval fantasy, but do not identify a particular society or place. Decorative crosses introduce a heraldic/religious association without any established narrative reason. Replace them with a simple shutter or divided-disc maker's mark if using the proposed world.
The generated character has more detail and apparent source pixels than the earlier 24–28-pixel-tall practice target. The board's glow, gradients and inconsistent pixel scale make it a poor exact production specification. Keep it as material reference.
Most objects are presented as isolated inventory icons. A side-view game also needs grounded props, wall attachments, top surfaces, shadow footprints and consistent character-relative size.

### Existing 2D presentation
MosslightArt.cs:25–26 creates 96px procedural textures with bilinear filtering. These are soft geometric components, not the new pixel sprites.
Paper.shader:12–17 is unlit. Glow sprites brighten an overlay visually; they do not make nearby steel/stone respond to a lamp.
MosslightWorld.cs:50 onward draws scenery at different sorting orders, but does not implement parallax. Draw order creates overlap, not a difference in apparent distance during camera movement.
Repeated huge ring arches, identical light shafts and recurring ferns provide rhythm but little local history.
The floor is a narrow top line above a largely flat fill; little bevel, front-face variation or contact shadow establishes thickness.
MosslightCamera.cs:15–24 uses immediate facing-based look-ahead and continuous smoothing. For close duels, facing flips could move the framing unnecessarily; pixel art also needs an intentional camera/pixel-grid policy.
The saved garden image devotes much of the frame to repeated architecture. Close weapon combat may benefit from a larger hero on screen, subject to reach/telegraph testing.
The procedural sound system currently supplies simple synthesized cues and a repeating tonal ambience. A specific place would benefit from material impacts and distinct rain/interior room tone.

## Proposed world: The Last Sunworks
A coastal fortress once collected daylight in brass reflectors and stored it in amber glass. Its workshops equipped the guards and supplied lamps to nearby settlements. Now the collectors are damaged, the service bridges are broken and the workshops are abandoned. A small armored light arrives at a sealed room that is still warm.

This proposal gives the world a shared craft and a visible past. It does not require light-switch puzzles, survival timers or a new resource mechanic.

### Three visual rules
1. Function leaves a shape: circular glass vessels, hinged reflector petals, thick door shutters and brass conduits recur at several scales.
2. Wear follows conditions: rain streaks outside, soot above vents, rust around fasteners, dry shelves under cover, glass fragments beneath damaged storage.
3. Light has a source and purpose: cold overcast daylight fills the exterior; isolated amber sources identify surviving equipment. Enemies remain readable outside those pools.

### Curiosity expressed through evidence
- Most vessels are empty, but a conduit still leads toward a lit door.
- A reflector points at a collector whose surface is broken.
- An empty armor rack shares the hero's maker's mark.
- A floor stain ends at a sealed workshop.
These are optional authored clues, not a promise of a larger quest system. Give at least one visible clue a payoff inside the small map.

### One map, three adjoining spaces
- Service causeway: weather, sea depth and a distant broken collector establish location.
- Repair workshop: shelter, glass storage and a damaged reflector explain the craft.
- Shutter chamber: the same modules arranged into one clear duel arena; a larger enemy carries equipment derived from the workshop.
A vista is background art, not a promise that its distant city can be explored.

## A completely 2D depth recipe
Keep XY movement/collisions and a fixed side-on orthographic view. All scenery can remain sprites; no orbit camera, 3D models or perspective gameplay are required.

| Layer | Content | Example horizontal screen motion when camera travels 100px |
|---|---|---|
| Far sky | Overcast sky and ocean haze | 0–5px opposite camera travel |
| Far landmark | Dead collector and distant fortress | 10–20px |
| Middle distance | Service bridge, cliffs and towers | 30–50px |
| Rear wall | Workshop supports and niches | 75–90px |
| Gameplay | Hero, enemies, floor and interactables | World objects: 100px |
| Near frame | Cropped post, cable or arch at screen edges | 110–120px |

Values are suggested starting points, not engine settings or a universal formula. For camera-only translation, a layer with target screen-motion ratio s can use world offset (1-s) times camera displacement from a captured origin. Anchor by origin rather than cumulatively adding offsets. Do not apply this to colliders or actors.

Distance should also reduce texture, edge contrast and saturation; exterior distant colors converge toward the sky/haze color. Do not merely make every distant layer black. Keep the strongest functional contrast around actors and the walkable surface.

Draw visible thickness into door frames, floor lips, shelf brackets and bridge fronts. Choose one consistent drawing perspective; receding lines converge consistently if present. Background floors must not look like alternate walkable lanes.

Anchor objects with contact shadows. Begin with hand-drawn shadow shapes beneath props and a restrained shadow beneath grounded actors. Decorative cast shadows point away from the local lamp. Shadows are not a substitute for collision readability.

## Palette refinement
Keep Hearthsteel 24 as the base asset palette. No replacement palette is necessary yet.
- Steel: actor armor and sharp blade highlights.
- Stone: broad cool structures with muted light faces.
- Wood: tool handles and supports; its darks also serve worn bronze shadows.
- Gold/light: tarnished brass, stored daylight and a few functional highlights.
- Moss: sparse damp seams, rather than a uniform garden carpet.
- Cloth: one shared muted burgundy accent, faded further in scenery.
Use roughly three quarters subdued cool/neutral scenery, a smaller warm material area and a few bright focal accents as a composition exercise, not a rigid measured quota.
The rendered scene can contain additional colors from fog, transparency and lighting. Do not claim a strict 24-color final framebuffer while using smooth lighting effects.

The earlier top-left-light instruction remains useful for flat drawing practice. For a dynamically lit scene, paint restrained form shading, then let local lights provide the strongest directional highlights. Strong baked top-left highlights plus a lamp on the opposite side can contradict each other.

## Pixel scale and camera
Two valid scopes:
- Easiest first test: retain 16px tiles and a roughly 28px-tall knight; use closer framing.
- More armor articulation: test a 32x48 frame with a roughly 36–40px-tall character. This replaces, rather than silently mixes with, the earlier 32x32 frame convention and increases animation effort.

Do a single-pose comparison before choosing. A suggested display target is 384x216 with integer enlargement; it divides 1920x1080 exactly by five. This is a proposal, not the current project setting. Choose common pixels-per-unit and preserve it across gameplay sprites. Point filtering, intentional scaling and a pixel-consistent render path matter more than a high-resolution source drawing.

Keep physics motion smooth. Apply pixel alignment to the rendering/camera strategy, not by rounding Rigidbody2D positions. Test slow camera travel for shimmer. Avoid zoom changes during the first visual test. Frame both fighters with stable look-ahead/dead zones rather than turning the camera whenever the hero flips direction.

## Lighting implementation options
First pass: drawn surface volume, contact shadows, parallax and restrained emissive-looking sprites. These can establish most of the composition without a normal-map pipeline.

Later pass: a scene-specific URP 2D Renderer, compatible sprite-lit materials, weak ambient fill and a few local 2D lights. Current Paper.shader does not respond to Light2D, so adding a light alone will not provide the intended result.
Optional normal maps encode which direction a sprite's surface appears to face, allowing local light to suggest curved steel or bevelled stone. They do not create new silhouettes, real geometry or automatically correct cast shadows. Start with one static helmet or door; animated normal maps add frame-maintenance work.
Keep fog behind the gameplay lane and bloom limited. A broad bright halo around every object weakens the focal hierarchy.

Technical references:
- https://docs.unity3d.com/6000.0/Documentation/Manual/urp/Lights-2D-intro.html
- https://docs.unity3d.com/6000.0/Documentation/Manual/urp/SecondaryTextures.html

## Armor, weapons, enemies and sound
Hero: retain closed helmet, one amber slit, muted scarf, clear sword/shield separation. Add a single maker's mark and one patch repair, not many glowing seams.
Weapons: make sword guard and bow fittings resemble workshop hardware. Preserve silhouette and attack readability before decoration. A bow drawing does not commit the prototype to a ranged-combat system.
Props: replace several generic crates with one recognizable light vessel, one broken reflector and a service shutter. Reuse brackets, rivets and stone modules.
Enemies: if returning to the duel GDD, build opponents from the same armored craft tradition with different silhouettes. Do not automatically add Mosslight's insect roster to this direction.
Sound: broad outdoor wind/rain, quieter sheltered dripping, occasional glass/metal ticks and strong differentiated blade/wood/armor impacts. Do not add a new sound system before the visual-room test; these are atmosphere targets.

## First production test and acceptance criteria
Build a separate visual test room, then decide whether to adopt the direction.
Minimum art: one hero pose, sword/shield, ground strip, wall module, pillar, shutter door, lantern, amber vessel, reflector, one distant silhouette and one foreground fragment.
Reuse materials and avoid detailed background animation initially.

Suggested order:
1. Half-day composition thumbnails: test two value arrangements.
2. Several drawing sessions for the minimum kit; measure actual time per asset.
3. Assemble one room with depth layers and painted shadows.
4. Add one controlled light experiment only after the unlit composition reads.
5. Move through the room, check foreground occlusion and test a short duel.
These are task order suggestions, not a promise of completion time for a beginner.

Acceptance:
- A still frame distinguishes near framing, gameplay plane and distant scenery.
- Hero, weapon tip and enemy warning remain visible in grayscale and without glow.
- Ground contacts and collision surfaces are unambiguous.
- Walking the camera produces depth without pixel shimmer or slipping colliders.
- A viewer can infer that this was a workshop for storing light.
- A viewer notices the lit sealed door and wants to investigate.
- One clue receives a small in-map payoff.
- Record asset time before committing to more rooms.

## Concept image limits
sunworks-concept.png was generated using the built-in image-generation tool. Complete prompt: sunworks-generation-prompt.txt.
It demonstrates atmosphere, shape relationships and layered composition. It has substantially more texture and color variation than the proposed beginner production kit, and is not a pixel-perfect 384x216 screenshot or game-ready asset pack.
Use the room's light/dark organization and material story; simplify its masonry, machinery and background detail.
