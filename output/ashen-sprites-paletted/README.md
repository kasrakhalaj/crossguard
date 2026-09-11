# Crossguard — downloaded palette variants

Both variants use the exact HEX files supplied in E:/Aseprite/palettes. Originals were preserved.

- resurrect-64/: 16 separate 64x64 PNGs using Resurrect 64.
- endesga-64/: 16 separate 64x64 PNGs using ENDESGA 64.
- Each folder includes a 256x256 atlas (4x4 cells of 64x64), a 4x enlarged preview, the supplied HEX palette, a converted GPL palette and a swatch PNG.
- Each visible sprite pixel was mapped to the nearest palette color using Euclidean CIELAB distance. No dithering or new colors were added. Original alpha values were preserved.
- validation.txt records checks for the individual PNGs: 64x64 dimensions, exact palette membership and unchanged alpha.
- preview-4x.png is for viewing only; its solid background is not part of the sprite palette. Use the individual sprites or transparent atlas in your project.

These remain prototype sprites derived from generated concept art. Palette conversion does not repair noisy pixel clusters or turn the three standing views into an animation.

Original palette pages:
https://lospec.com/palette-list/resurrect-64
https://lospec.com/palette-list/endesga-64
