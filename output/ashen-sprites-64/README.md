# Crossguard: actual 64x64 sprite exports
Every individually named asset PNG is exactly 64x64 pixels, with an alpha channel.
16 assets: knight front/side/back, helmet, breastplate, sword, bow, arrow, shield, mace, lantern, urn, shrine, broken column, banner and grass.
atlas-256.png is a 256x256 sheet: 4 columns by 4 rows, 64x64 cells, same order as above.
preview-4x.png is only an enlarged viewing preview, not a native sprite.

Created with built-in image generation followed by cell extraction, rough background matting and nearest-neighbor export. The generator produced an opaque checkerboard despite the transparency request; connected exterior checkerboard and the enclosed bow/ring spaces were removed for these exports.
These are downsampled concept sprites for prototyping and drawing reference, not manually polished pixel art or animation-ready matched frames. Inspect contours and simplify noisy clusters before final production. Equipment studies use their own framing; they are not all at matching world scale.
The exported colors have NOT been mapped to any Lospec palette. The earlier Ashen/Hearthsteel palettes were custom proposals, not downloaded palettes.

Palette links:
- Resurrect 64 by Kerrie Lake: https://lospec.com/palette-list/resurrect-64
- ENDESGA 64: https://lospec.com/palette-list/endesga-64
- AAP-64: https://lospec.com/palette-list/aap-64
The number 64 in those names refers to colors, not image dimensions.
Download PNG or GPL from the palette page's Downloads section.
For a restrained dark-fantasy study, use only selected neutral, brown, muted green and cloth colors; reserve vivid accents for lights.

Full generation prompt: ../art-direction-review/native-64-export-prompt.txt
Unity import starting point: Sprite (2D and UI), Point filtering, no compression; use consistent pixels per unit. atlas-256.png can be sliced into a 64x64 grid. Keep editable source work separate.
