# Mosslight — your first small action platformer

This is an original, playable Unity learning prototype: one garden, one hero, three enemy types, an optional hidden heart, and one boss. It borrows the broad idea of exploring and fighting in a side-scrolling world, with its own geometric artwork and synthesized sound.

## Play it

**Without opening Unity:** run `output/Mosslight/Mosslight.exe` from the Crossguard project folder. Keep the executable and its neighboring files together.

**In Unity 6000.5.7f1:** open this existing Crossguard project, then choose **Mosslight → Open Learning Scene** in the menu bar. Press **Play**, then **Enter** or the on-screen Begin button.

The scene is `Assets/Mosslight/Scenes/Mosslight.unity`. It contains a small bootstrap object. The garden and its actors are deliberately created when you press Play. Expand the Hierarchy during Play to inspect them. Scene edits made during Play are temporary; use the saved tuning asset for changes you want to keep.

| Action | Keyboard / mouse | Gamepad |
| --- | --- | --- |
| Move | A/D or left/right arrows | Left stick or D-pad |
| Jump | Space; hold for a higher jump | Bottom face button |
| Strike | J or left mouse button | Left face button |
| Dash | Left Shift or K | Right bumper |
| Rest / heal | E near a lantern | Top face button |
| Pause | Escape | Start |

Touch a new lantern to set a checkpoint and recover health. Falling into a pit costs one heart and returns you to your checkpoint. Losing all health resets enemies and the unfinished boss encounter. Seeds and the hidden heart remain collected during the session. The pause menu can return you to your checkpoint if you become stuck.

After the boss, keep walking right to the large bell to finish. The game has a title screen, pause menu, sound/shake options, quick retries, and an ending. **There is no save between application sessions in this prototype.**

## Your first lesson: change how the jump feels

1. Open `Assets/Mosslight/Settings/LearningTuning.asset` in the Inspector.
2. Note the current **Jump Speed** of 13.5.
3. Set it to 15, press Play, and try the same jump.
4. Return it to 13.5, then change **Gravity** from 34 to 40 instead.
5. Describe the difference in your own words. Did the jump get higher, faster, or both?

Change one number at a time. Values on this asset persist after stopping Play. The provided map is tuned for the defaults; large movement changes can make gaps impossible or allow shortcuts.

## Read the code in this order

| File in `Scripts/` | What you learn |
| --- | --- |
| `MosslightInput.cs` | Turn keys and gamepad buttons into a small set of intentions. |
| `MosslightHero.cs` | Move a physics body, jump, dash, attack, take damage, and recover. |
| `MosslightEnemy.cs` | A state machine: wait, warn, attack, recover. |
| `MosslightBoss.cs` | Combine three attacks into a predictable encounter. |
| `MosslightGame.cs` | Coordinate the title, play, pause, death, checkpoints, and ending. |
| `MosslightWorld.cs` | Assemble the map from platforms, scenery, and enemy placements. |
| `MosslightHUD.cs` | Draw menus, health, instructions, and the boss health bar. |
| `MosslightArt.cs` | Build reusable shapes and layer them into original characters. |
| `MosslightCamera.cs` | Follow smoothly, frame a boss arena, and apply optional shake. |
| `MosslightEffects.cs` | Separate visual feedback from the code that deals damage. |
| `MosslightSound.cs` | Generate simple audio clips and play them when events occur. |

You do **not** need to understand every line to start. Trace one action at a time:

`Space pressed → MosslightInput.Read → hero remembers jump → FixedUpdate sets upward velocity → physics moves the body.`

`J pressed → Hero.Attack → overlap query finds enemy → Enemy.Hit subtracts health → feedback plays.`

## Five small exercises

1. **Movement:** tune jump height and dash distance. Find values you prefer without breaking the first gap.
2. **Combat:** change the attack cooldown in the tuning asset. Explain how it changes the risk of staying close to an enemy.
3. **Level design:** move one `Platform(...)` call in `MosslightWorld.cs`. Predict whether the next jump remains possible before testing it.
4. **Enemy design:** change the charger's warning time in `MosslightEnemy.cs`. Test whether you can still understand and avoid its attack.
5. **Your own contribution:** add a second small secret using an existing platform and collectible. Reuse the current systems before inventing a new one.

For each exercise, write three short notes: **what I expected / what happened / what I changed next**. Keep a copy or Git commit before a larger experiment.

## Important implementation choices

- Movement uses a `Rigidbody2D`. Device input is read in `Update`; velocity is applied in `FixedUpdate`, Unity's physics step.
- **Coyote time** allows a jump briefly after leaving an edge. **Jump buffering** remembers a jump pressed just before landing.
- A dash briefly protects the hero from contact damage. Damage also provides a short blinking grace period.
- The boss warns before each attack, then offers recovery time. At lower health, its recovery gets shorter; it keeps the same three rules.
- Layer 8 is used for this scene's ground and gates; layer 9 for hurtboxes. The builder does not rename project layers or change the global collision matrix.
- Artwork, sound, actors, and terrain are generated locally. There are no downloaded character assets, paid packages, or online services needed to play.
- This first version has simple pose animation and synthesized audio. It is a learning prototype, not a finished commercial game or a reproduction of Hollow Knight's presentation.
- Crossguard's existing arena scene and scripts are separate. The Mosslight build explicitly selects its own scene without changing the project's default build scene list.

## Build and test

Choose **Mosslight → Build Windows Learning Game** to rebuild the executable after making changes. The first build can take longer while Unity imports and compiles assets.

`Scripts/MosslightPlaytest.cs` is an opt-in integration test. It is inactive during normal play. It sends movement/attack inputs and uses explicit actor placements to isolate systems. It checks movement, jump, dash cooldown, pause, combat, the secret, checkpoint retry, pit recovery, projectile damage, the bridge, the boss's attack cycle, and the ending. It is not a substitute for someone playing the entire level and judging its difficulty.

Example in PowerShell, from the project directory:

```powershell
& '.\output\Mosslight\Mosslight.exe' -screen-width 1600 -screen-height 900 -screen-fullscreen 0 --mosslight-test --mosslight-artifacts 'C:\Unity\crossguard\output\MosslightQA'
```

Results and captured camera images are written to `output/MosslightQA`. These automated images show the world without the menu/HUD overlay. The process exits with code 0 on success and 2 if a check fails. Add `-batchmode` to run without a window.

## What counts as success for this project?

Play it, understand one system, make a change yourself, and finish another small version. Your next milestone is being able to explain why your change works.
