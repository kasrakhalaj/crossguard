using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mosslight
{
    public enum GamePhase { Title, Playing, Paused, Fallen, Complete }

    public class MosslightGame : MonoBehaviour
    {
        [Header("Open Assets/Mosslight/Settings/LearningTuning to experiment")]
        public MosslightTuning tuning;
        public Material spriteMaterial;
        public GamePhase Phase { get; private set; } = GamePhase.Title;
        public bool Playing => Phase == GamePhase.Playing;
        public MosslightHero Hero { get; private set; }
        public MosslightBoss Boss;
        public MosslightCamera CameraRig { get; private set; }
        public MosslightSound Sound { get; private set; }
        public MosslightEffects Effects { get; private set; }
        public readonly List<MosslightEnemy> Enemies = new List<MosslightEnemy>();
        public Vector2[] Lanterns;
        public Vector2 Checkpoint { get; private set; } = new Vector2(8, -.3f);
        public GameObject EntranceGate, ExitGate, Shortcut;
        public bool SecretFound, ShortcutOpened;
        public int Seeds, DefeatedEnemies, Falls;
        public float PlaySeconds { get; private set; }
        public string Announcement { get; private set; } = "";
        public string AnnouncementDetail { get; private set; } = "";
        public float AnnouncementLeft { get; private set; }
        public bool Automated { get; set; }
        [System.NonSerialized] public HeroInput TestInput;
        public bool Muted { get; private set; }
        float fallTime;
        int currentArea = -1;
        int checkpointIndex;

        public string AreaName
        {
            get
            {
                float x = Hero == null ? 0 : Hero.transform.position.x;
                return x < 22 ? "THE WAKING GARDEN" : x < 43 ? "ROOTBOUND WALK" : x < 64 ? "THE GLASS WELL" : x < 87 ? "THORN GALLERY" : x < 102 ? "THE LAST LANTERN" : x < 124 ? "BELFRY OF THE LOST" : "A LITTLE DAWN";
            }
        }

        void Start()
        {
            Time.timeScale = 1;
            Application.targetFrameRate = 120;
            if (tuning == null) tuning = ScriptableObject.CreateInstance<MosslightTuning>();
            MosslightArt.Material = spriteMaterial;
            Sound = gameObject.AddComponent<MosslightSound>(); Sound.Initialize(tuning.soundVolume);
            Effects = new GameObject("Transient effects").AddComponent<MosslightEffects>();
            var heroObject = new GameObject("HERO — select me during Play to inspect movement");
            heroObject.transform.position = new Vector2(5, 0);
            Hero = heroObject.AddComponent<MosslightHero>(); Hero.Initialize(this);
            MosslightWorld.Build(this);
            var cameraObject = new GameObject("Mosslight Camera");
            var camera = cameraObject.AddComponent<Camera>(); camera.tag = "MainCamera";
            camera.orthographic = true; camera.orthographicSize = 6.7f;
            camera.backgroundColor = MosslightArt.Ink; camera.clearFlags = CameraClearFlags.SolidColor;
            camera.nearClipPlane = .1f; camera.farClipPlane = 100;
            cameraObject.AddComponent<AudioListener>();
            CameraRig = cameraObject.AddComponent<MosslightCamera>(); CameraRig.Initialize(this);
            gameObject.AddComponent<MosslightHUD>().Initialize(this);
            Hero.Body.simulated = false;
            var args = System.Environment.GetCommandLineArgs();
            if (System.Array.IndexOf(args, "--mosslight-test") >= 0)
                gameObject.AddComponent<MosslightPlaytest>().Initialize(this);
        }

        void Update()
        {
            if (Phase == GamePhase.Title)
            {
                if (MosslightInput.Confirm) Begin();
                return;
            }
            if (MosslightInput.Pause && (Playing || Phase == GamePhase.Paused)) TogglePause();
            if (Phase == GamePhase.Complete) { if (MosslightInput.Restart) NewJourney(); return; }
            if (Phase == GamePhase.Fallen)
            {
                fallTime -= Time.unscaledDeltaTime;
                if (fallTime <= 0) Respawn();
                return;
            }
            if (!Playing) return;
            PlaySeconds += Time.deltaTime;
            AnnouncementLeft -= Time.deltaTime;
            int area = Hero.transform.position.x < 22 ? 0 : Hero.transform.position.x < 43 ? 1 : Hero.transform.position.x < 64 ? 2 : Hero.transform.position.x < 87 ? 3 : Hero.transform.position.x < 102 ? 4 : Hero.transform.position.x < 124 ? 5 : 6;
            if (area != currentArea) { currentArea = area; if (area != 5) Announce(AreaName, ""); }
            for (int i = 0; i < Lanterns.Length; i++)
            {
                if (Vector2.Distance(Hero.transform.position, Lanterns[i]) < 1.5f && checkpointIndex != i)
                {
                    checkpointIndex = i; Checkpoint = Lanterns[i]; Hero.Heal();
                    Announce("LANTERN KINDLED", "Your journey continues from here."); Sound.Play(SoundCue.Bell, .7f);
                }
            }
            if (!ShortcutOpened && Hero.transform.position.x > 93)
            {
                ShortcutOpened = true; Shortcut.SetActive(true);
                Announce("A WAY BACK", "The old bridge settles into place."); Sound.Play(SoundCue.Bell, .5f);
            }
            if (Boss.Defeated && Hero.transform.position.x > 130) Complete();
        }

        public void Begin()
        {
            Phase = GamePhase.Playing; Hero.Body.simulated = true;
            Announce("THE WAKING GARDEN", "A small light. One last bell.");
        }
        public void TogglePause()
        {
            if (Phase != GamePhase.Playing && Phase != GamePhase.Paused) return;
            Phase = Playing ? GamePhase.Paused : GamePhase.Playing;
            Time.timeScale = Phase == GamePhase.Paused ? 0 : 1;
        }
        public void ToggleMute() { Muted = !Muted; Sound.SetVolume(Muted ? 0 : tuning.soundVolume); }
        public void ToggleShake() { CameraRig.enabled = true; ShakeDisabled = !ShakeDisabled; }
        public bool ShakeDisabled { get; private set; }
        public void HeroDied()
        {
            Phase = GamePhase.Fallen; fallTime = 1.3f; Falls++;
            Hero.Body.linearVelocity = Vector2.zero; Hero.Body.simulated = false;
        }
        public void Respawn()
        {
            foreach (var projectile in FindObjectsByType<MosslightProjectile>()) Destroy(projectile.gameObject);
            foreach (var enemy in Enemies) enemy.ResetEnemy();
            if (!Boss.Defeated) Boss.ResetBoss();
            SetArenaClosed(false);
            Hero.Revive(Checkpoint); Hero.Body.simulated = true; Phase = GamePhase.Playing;
            CameraRig.Snap();
        }
        public bool NearLantern()
        {
            foreach (var point in Lanterns) if (Vector2.Distance(Hero.transform.position, point) < 1.7f) return true;
            return false;
        }
        public void TryRest()
        {
            if (!NearLantern()) return;
            Hero.Heal(); Sound.Play(SoundCue.Bell, .6f);
            Announce("A MOMENT OF WARMTH", "Health restored.");
        }
        public void SetArenaClosed(bool closed)
        {
            EntranceGate.SetActive(closed);
            ExitGate.SetActive(!Boss.Defeated);
        }
        public void Announce(string title, string detail)
        {
            Announcement = title; AnnouncementDetail = detail; AnnouncementLeft = 4;
        }
        public void Projectile(Vector2 position, Vector2 velocity, bool wave)
        {
            var projectile = new GameObject(wave ? "Bell wave — jump over it" : "Spore projectile");
            projectile.transform.position = position;
            projectile.AddComponent<MosslightProjectile>().Initialize(this, velocity, wave);
        }
        public void Complete()
        {
            Phase = GamePhase.Complete; Hero.Body.linearVelocity = Vector2.zero; Hero.Body.simulated = false;
            Sound.Play(SoundCue.Secret);
        }
        public void NewJourney()
        {
            Time.timeScale = 1;
#if UNITY_EDITOR
            // The learning scene need not replace Crossguard in Build Settings.
            UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(SceneManager.GetActiveScene().path, new LoadSceneParameters(LoadSceneMode.Single));
#else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
#endif
        }
        public void Quit()
        {
            Time.timeScale = 1;
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        void OnDestroy() { Time.timeScale = 1; }
    }
}
