# Crossguard / Light Knight: Hearthsteel 24
Original proposed starter palette. This is our design proposal, not an existing game's palette.

## Files
- crossguard-hearthsteel-24.gpl: editable/importable palette.
- crossguard-hearthsteel-24.png: exact 24-color swatch image, four columns by six rows. Each swatch is 32x32 pixels. No additional colors.
- reference-board.png (if generation succeeds): AI-generated visual inspiration, not a verified production atlas. Use the GPL/PNG swatches for exact colors rather than sampling the generated board.
- generation-prompt.txt: complete prompt used with the built-in image-generation tool.

## Palette
Each row progresses darkest to lightest. A ramp is this ordered group of related colors.
| Material | Deep shadow | Shadow/base | Base/light | Highlight |
|---|---|---|---|---|
| Steel | #171C2C | #39465E | #718DA5 | #C8DCE5 |
| Stone | #292936 | #494653 | #77717A | #AAA1A0 |
| Wood | #302229 | #654033 | #A36843 | #D8A16A |
| Gold-Light | #69402C | #B87832 | #EABB58 | #FFF0B0 |
| Moss | #243A32 | #41604A | #78935A | #B2BB78 |
| Cloth | #402638 | #78384B | #B95C65 | #ECA08D |

Use three colors per material initially. Not every object needs all four.
Steel's #171C2C can also serve as the shared outline and deepest seam color.
Gold's #FFF0B0 is reserved for tiny glints, torch cores and the spirit's brightest pixels.
Transparency is separate from these 24 opaque colors.

## Drawing setup
Use your existing pixel editor if comfortable. Aseprite is suitable; Piskel is a free browser alternative.
Use a one-pixel hard pencil, full opacity, no anti-aliasing, no blur and no soft brushes.
Draw at native resolution, inspect frequently at 100%, and enlarge only by integer multiples using nearest-neighbor.
Keep a top-left light direction across all assets.
Suggested shared scale: 16x16 tile grid; knight fits a 32x32 frame and is roughly 24-28 pixels tall.
These are practice targets, not engine requirements or specifications from another game.
Do not shrink a high-resolution AI board to make finished sprites automatically; redraw chosen shapes on your native grid.

## Material recipes
### Steel armor, helmets, shields
Draw silhouette, then broad steel base, bottom-right shadow, and a narrow broken upper-left highlight.
Use dark gaps between plates. Keep broad plates mostly flat.
For the knight's visor, add two or three gold/light pixels surrounded by dark steel; avoid glow blur.
Steel: three or four steel colors. Rust, if desired: sparing wood colors.
Gold trim: a few gold pixels rather than coloring the whole armor yellow.

### Sword (16x32 practice canvas)
Draw a straight blade with a clearly readable tip; give it a dark edge, a midtone face and a thin bright edge.
Add a guard wider than the grip. Wood colors for grip, steel for blade, optional gold on the guard.
Start with 5-7 colors total. One very bright glint is enough.
If the held sword is separate from the character, keep the grip/pivot location consistent between frames.

### Bow and arrow (16x32 practice canvas)
Draw a simple curved wood silhouette 2-3 pixels thick in its wider areas.
Connect the tips with a straight one-pixel string in a muted pale color, such as #AAA1A0.
Leave visible space between string and wood; check readability against the map.
Use wood shadow on the inner/lower edge and a few warm highlights on the upper-left.
Arrow: one-pixel shaft, clearly shaped steel head, tiny fletching. Avoid detail that vanishes at 100%.

### Wooden shield, crate, barrel
Use wood colors and shared dark seams. Three tones are sufficient.
Crate: 16x16 with a strong border, two or three planks and one diagonal brace.
Barrel: 16x24 or 16x32; narrow top/bottom, broad middle, two steel bands.
Wooden shield: 16x16 or 16x24; readable outline, one central boss and minimal plank marks.
Grain should be a handful of short strokes, not noise on every surface.

### Rock and stone floor
Use stone colors, broad angular clusters, a darker underside and one lighter top face.
Start with one 16x16 rock and one 16x16 floor tile.
For repeatable tiles, test a 3x3 arrangement to spot seams.
Keep floor detail quieter than the character. Use moss colors sparingly on upper edges.

### Plants and grass
Use two or three moss colors. Draw distinct leaf/blade clusters and leave gaps.
Grass tuft: 16x16, three or five blade groups, dark base and a few light tips.
Keep decorative grass from obscuring combat silhouettes.

### Cloth and banners
Use three cloth colors and one large fold shadow.
Banner: 16x32, simple border and one emblem at most.
Avoid using the lightest cloth everywhere; reserve it for small lit edges.
Reuse the cloth ramp on the knight to tie character and environment together.

### Torches and spirit light
Use gold colors: a dark outer shape, golden body, tiny pale core.
Torch body uses wood or steel. Start with a still sprite; animate 3-4 flame variations afterward.
These are painted pixels, not a requirement for real-time lighting or bloom.
Do not make every prop glow. Reserve the brightest colors for character, interactables and effects.
Use shape, placement or motion in addition to color to signal important interactions.

## Repeatable process
1. Draw the silhouette in one dark color.
2. Fill each material with its base color.
3. Add one shadow mass per major form.
4. Add a few highlights at the upper-left.
5. Remove stray pixels and unnecessary texture.
6. Inspect at actual game size against both a light and dark background.
7. Put the object beside existing assets to check scale and palette.
8. Save your editable source, then export a transparent PNG.

## First practice session
Make one sword, one wooden shield and one crate. Finish those before attempting a full character.
Then draw a helmet and a single knight standing pose.
Only after the standing pose reads clearly, try a restrained 3-4-frame idle.
Keep the feet anchored in the same place; move visor light or cloth sparingly.
Create one small playable screen with these assets before adding another palette.

## Sources for editor capabilities
- Piskel: https://www.piskelapp.com/
- Aseprite color bar/palette guide: https://www.aseprite.org/docs/tutorial/color-bar-tutorial/
- Aseprite palettes: https://www.aseprite.org/docs/color-bar/
