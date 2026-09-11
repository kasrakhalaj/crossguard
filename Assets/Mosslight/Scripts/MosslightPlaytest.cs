using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

namespace Mosslight
{
    // Opt-in automated integration test. Inactive during ordinary play.
    // Run the built game with --mosslight-test --mosslight-artifacts <directory>.
    // It uses real inputs/physics, with explicit placements to isolate each feature.
    public class MosslightPlaytest : MonoBehaviour
    {
        MosslightGame game;
        string directory;
        readonly List<string> results = new List<string>();
        int failures;

        public void Initialize(MosslightGame owner)
        {
            game = owner; game.Automated = true;
            Application.runInBackground = true;
            var args = Environment.GetCommandLineArgs();
            int i = Array.IndexOf(args, "--mosslight-artifacts");
            directory = i >= 0 && i + 1 < args.Length ? args[i + 1] : Path.Combine(Application.dataPath, "../QA");
            Directory.CreateDirectory(directory);
            Application.logMessageReceived += Log;
            StartCoroutine(Run());
        }
        void Log(string message, string stack, LogType type)
        {
            if (type != LogType.Exception && type != LogType.Error && type != LogType.Assert) return;
            failures++; results.Add("RUNTIME ERROR: " + message + "\n" + stack);
        }
        void Check(bool condition, string description)
        {
            results.Add((condition ? "PASS: " : "FAIL: ") + description);
            if (!condition) failures++;
        }
        IEnumerator Capture(string name)
        {
            // Render a camera image explicitly so this works even with a hidden
            // window. These QA images show the world; IMGUI menus are separate.
            yield return new WaitForSecondsRealtime(.2f);
            var camera = game.CameraRig.GetComponent<Camera>();
            var target = RenderTexture.GetTemporary(1600, 900, 24, RenderTextureFormat.ARGB32);
            if (GraphicsSettings.currentRenderPipeline != null)
                RenderPipeline.SubmitRenderRequest(camera, new RenderPipeline.StandardRequest { destination = target });
            else
            {
                camera.targetTexture = target; camera.Render(); camera.targetTexture = null;
            }
            var previous = RenderTexture.active;
            RenderTexture.active = target;
            var texture = new Texture2D(1600, 900, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, 1600, 900), 0, 0); texture.Apply();
            File.WriteAllBytes(Path.Combine(directory, name + ".png"), texture.EncodeToPNG());
            RenderTexture.active = previous; RenderTexture.ReleaseTemporary(target); Destroy(texture);
            yield return null;
        }
        IEnumerator Run()
        {
            yield return null;
            yield return Capture("01-title");
            game.Begin();
            yield return new WaitForSeconds(.4f);
            Check(game.Hero.Grounded, "Hero lands on the starting floor");
            float start = game.Hero.transform.position.x;
            game.TestInput = new HeroInput { move = 1 };
            yield return new WaitForSeconds(.4f);
            game.TestInput = default;
            Check(game.Hero.transform.position.x > start + 1.5f, "Horizontal input moves the hero using physics");
            float height = game.Hero.transform.position.y;
            game.TestInput = new HeroInput { jumpPressed = true, jumpHeld = true };
            yield return null;
            game.TestInput = new HeroInput { jumpHeld = true };
            yield return new WaitForSeconds(.24f);
            Check(game.Hero.transform.position.y > height + 1.4f, "Jump input lifts the hero clear of the ground");
            game.TestInput = default;
            yield return new WaitForSeconds(.65f);
            start = game.Hero.transform.position.x;
            game.TestInput = new HeroInput { move = 1, dash = true };
            yield return null;
            game.TestInput = new HeroInput { move = 1 };
            yield return new WaitForSeconds(.2f);
            game.TestInput = default;
            Check(game.Hero.transform.position.x > start + 2.5f, "Dash covers extra distance");
            Check(game.Hero.DashReady < 1, "Dash enters cooldown");
            yield return Capture("02-garden");
            game.Hero.Teleport(new Vector2(29.5f, -.35f));
            yield return new WaitForSeconds(.8f);
            game.TestInput = new HeroInput { move = 1, jumpPressed = true, jumpHeld = true };
            yield return null;
            game.TestInput = new HeroInput { move = 1, jumpHeld = true };
            yield return new WaitForSeconds(.22f);
            game.TestInput = new HeroInput { move = 1, jumpHeld = true, dash = true };
            yield return null;
            game.TestInput = new HeroInput { move = 1, jumpHeld = true };
            yield return new WaitForSeconds(.65f);
            game.TestInput = default;
            Check(game.Hero.transform.position.x > 35 && game.Hero.transform.position.y > -2, "Jump and dash traverse the first real map gap");
            game.TogglePause();
            Vector3 paused = game.Hero.transform.position;
            yield return new WaitForSecondsRealtime(.2f);
            Check(Vector3.Distance(paused, game.Hero.transform.position) < .01f && Time.timeScale == 0, "Pause freezes simulation");
            game.TogglePause();

            var walker = game.Enemies[0];
            walker.ResetEnemy();
            for (int n = 0; n < 3; n++)
            {
                game.Hero.Teleport((Vector2)walker.transform.position + new Vector2(-1.3f, .15f));
                game.TestInput = new HeroInput { move = 1 };
                yield return new WaitForFixedUpdate();
                game.TestInput = new HeroInput { attack = true };
                yield return null;
                game.TestInput = default;
                yield return new WaitForSeconds(.33f);
            }
            Check(!walker.Alive, "Three sword inputs defeat a walker through overlap hit detection");
            game.Hero.Teleport(new Vector2(54.5f, 5.1f));
            yield return new WaitForSeconds(.1f);
            Check(game.SecretFound && game.Hero.MaxHealth == game.tuning.playerHealth + 1, "Hidden heart is collected and increases maximum health");
            game.CameraRig.Snap();
            yield return Capture("03-secret");
            game.Hero.Teleport(new Vector2(59, -.35f));
            yield return new WaitForSeconds(.15f);
            Check(Mathf.Abs(game.Checkpoint.x - 59) < .1f, "Touching the second lantern updates the checkpoint");
            game.Hero.Damage(100, game.Hero.transform.position, true);
            Check(game.Phase == GamePhase.Fallen, "Lethal damage enters the fallen state");
            yield return new WaitForSeconds(1.5f);
            Check(game.Playing && Mathf.Abs(game.Hero.transform.position.x - 59) < .3f && game.Hero.Health == game.Hero.MaxHealth, "Death returns the hero to the checkpoint with full health");
            Check(walker.Alive, "Ordinary enemies reset on checkpoint retry");
            game.Hero.Teleport(new Vector2(65, -6));
            yield return new WaitForSeconds(.1f);
            Check(game.Hero.transform.position.y > -2 && game.Hero.Health == game.Hero.MaxHealth - 1, "A pit costs one heart and recovers to safe ground");
            game.TryRest();
            Check(game.Hero.Health == game.Hero.MaxHealth, "Resting at a lantern restores health");

            game.Hero.Teleport(new Vector2(12, -.35f));
            yield return new WaitForSeconds(1.3f);
            int hp = game.Hero.Health;
            game.Projectile(new Vector2(13.2f, -.3f), Vector2.left * 5, false);
            yield return new WaitForSeconds(.3f);
            Check(game.Hero.Health == hp - 1, "Spore projectile contact damages the hero");

            game.Hero.Heal(); game.Hero.Teleport(new Vector2(97, -.35f));
            yield return new WaitForSeconds(.2f);
            Check(game.ShortcutOpened && game.Shortcut.activeSelf, "Reaching the east side opens the return bridge");
            game.Hero.Teleport(new Vector2(107, -.35f));
            yield return new WaitForSeconds(.25f);
            Check(game.Boss.Active && game.EntranceGate.activeSelf, "Entering the belfry starts the boss and closes the entrance");
            game.Hero.Damage(100, game.Hero.transform.position, true);
            yield return new WaitForSeconds(1.5f);
            Check(game.Playing && !game.Boss.Active && !game.EntranceGate.activeSelf && game.Boss.Health == game.tuning.bossHealth && Mathf.Abs(game.Hero.transform.position.x - 97) < .3f,
                "Boss retry resets health and gates and returns to the last lantern");
            game.Hero.Teleport(new Vector2(107, -.35f));
            yield return new WaitForSeconds(.2f);
            game.CameraRig.Snap();
            yield return Capture("04-boss");
            // Watch the full attack cycle from a safe position, then test sword damage.
            game.Hero.Body.simulated = false; game.Hero.Teleport(new Vector2(104, 8));
            var attacks = new HashSet<string>();
            for (int n = 0; n < 135; n++)
            {
                attacks.Add(game.Boss.LastAttack);
                yield return new WaitForSeconds(.1f);
            }
            Check(attacks.Contains("charge") && attacks.Contains("leap") && attacks.Contains("ring"), "Boss executes all three telegraphed attacks");
            game.Hero.Body.simulated = true;
            for (int n = 0; n < 85 && !game.Boss.Defeated; n++)
            {
                game.Hero.Heal();
                game.Hero.Teleport((Vector2)game.Boss.transform.position + new Vector2(-1.7f, 0));
                game.TestInput = new HeroInput { move = 1 };
                yield return new WaitForFixedUpdate();
                game.TestInput = new HeroInput { attack = true };
                yield return null;
                game.TestInput = default;
                yield return new WaitForSeconds(.32f);
            }
            Check(game.Boss.Defeated && !game.ExitGate.activeSelf, "Sword hits defeat the boss and open the exit");
            game.Hero.Teleport(new Vector2(132, -.35f)); game.CameraRig.Snap();
            yield return new WaitForSeconds(.2f);
            Check(game.Phase == GamePhase.Complete, "Reaching the dawn bell after the boss completes the game");
            yield return Capture("05-ending");
            Application.logMessageReceived -= Log;
            results.Add("Failures: " + failures);
            File.WriteAllLines(Path.Combine(directory, "playtest-results.txt"), results);
            Application.Quit(failures == 0 ? 0 : 2);
        }
        void OnDestroy() { Application.logMessageReceived -= Log; }
    }
}
