using UnityEngine;

namespace Mosslight
{
    public class MosslightBoss : MonoBehaviour, IHurtable
    {
        public int Health { get; private set; }
        public bool Active { get; private set; }
        public bool Defeated => Health <= 0;
        public string Telegraph { get; private set; }
        public string LastAttack { get; private set; }
        MosslightGame game;
        Transform art, warning;
        float timer, jumpStartX, jumpTargetX;
        int moveIndex, facing = -1;
        enum Phase { Rest, Tell, Attack }
        Phase phase;
        const float FloorY = .3f;

        public void Initialize(MosslightGame owner)
        {
            game = owner;
            gameObject.layer = 9;
            var c = gameObject.AddComponent<BoxCollider2D>(); c.isTrigger = true; c.size = new Vector2(1.8f, 2.8f); c.offset = new Vector2(0, .2f);
            art = MosslightArt.Boss(transform);
            warning = MosslightArt.Sprite("Landing warning", null, new Vector2(110, -.93f), new Vector2(3.8f, .1f), MosslightArt.Red, 19);
            ResetBoss();
        }

        public void ResetBoss()
        {
            Active = false; Health = game.tuning.bossHealth; phase = Phase.Rest;
            timer = 1.6f; moveIndex = 0; Telegraph = ""; LastAttack = "";
            transform.position = new Vector2(118, FloorY);
            warning.gameObject.SetActive(false); gameObject.SetActive(true);
        }

        public void Begin()
        {
            if (Active || Defeated) return;
            Active = true; game.SetArenaClosed(true);
            game.Announce("THE BELLKEEPER", "The last watch has not ended.");
            game.Sound.Play(SoundCue.Bell);
        }

        void Update()
        {
            if (game == null || !game.Playing || Defeated) return;
            if (!Active)
            {
                if (game.Hero.transform.position.x > 103) Begin();
                return;
            }
            timer -= Time.deltaTime;
            float dx = game.Hero.transform.position.x - transform.position.x;
            if (phase == Phase.Rest)
            {
                facing = dx >= 0 ? 1 : -1;
                if (timer <= 0)
                {
                    phase = Phase.Tell; timer = 1.05f;
                    jumpStartX = transform.position.x;
                    jumpTargetX = Mathf.Clamp(game.Hero.transform.position.x, 105, 120);
                    Telegraph = moveIndex == 0 ? "DASH — jump over the charge" : moveIndex == 1 ? "LEAP — leave the marked ground" : "RING — jump over the light";
                    warning.position = new Vector2(jumpTargetX, -.93f);
                    warning.gameObject.SetActive(moveIndex == 1);
                    game.Sound.Play(SoundCue.Warning, .7f);
                }
            }
            else if (phase == Phase.Tell && timer <= 0)
            {
                phase = Phase.Attack;
                timer = moveIndex == 0 ? .78f : moveIndex == 1 ? .95f : .3f;
                LastAttack = moveIndex == 0 ? "charge" : moveIndex == 1 ? "leap" : "ring";
                if (moveIndex == 2)
                {
                    game.Projectile(new Vector2(transform.position.x - 1, -.55f), Vector2.left * 6.5f, true);
                    game.Projectile(new Vector2(transform.position.x + 1, -.55f), Vector2.right * 6.5f, true);
                    game.Sound.Play(SoundCue.Bell, .7f);
                    game.CameraRig.Shake(.15f);
                }
            }
            else if (phase == Phase.Attack)
            {
                if (moveIndex == 0)
                {
                    float x = Mathf.Clamp(transform.position.x + facing * 13 * Time.deltaTime, 104.5f, 121);
                    transform.position = new Vector2(x, FloorY);
                }
                if (moveIndex == 1)
                {
                    float t = Mathf.Clamp01(1 - timer / .95f);
                    transform.position = new Vector2(Mathf.Lerp(jumpStartX, jumpTargetX, t), FloorY + Mathf.Sin(t * Mathf.PI) * 5);
                }
                if (timer <= 0)
                {
                    transform.position = new Vector2(transform.position.x, FloorY);
                    if (moveIndex == 1) { game.Effects.Burst(transform.position, MosslightArt.Gold, 20); game.CameraRig.Shake(.25f); }
                    warning.gameObject.SetActive(false);
                    phase = Phase.Rest;
                    timer = Health < game.tuning.bossHealth / 2 ? .95f : 1.35f;
                    moveIndex = (moveIndex + 1) % 3;
                    Telegraph = "An opening. Strike, then make space.";
                }
            }
            art.localScale = new Vector3(facing, phase == Phase.Tell ? .94f : 1, 1);
            if (Mathf.Abs(dx) < 1.25f && Mathf.Abs(game.Hero.transform.position.y - transform.position.y) < 1.6f)
                game.Hero.Damage(1, transform.position);
        }

        public void Hit(int damage, Vector2 from)
        {
            if (!Active || Defeated) return;
            Health = Mathf.Max(0, Health - damage);
            game.Effects.Burst((Vector2)transform.position + Vector2.up * .5f, MosslightArt.Gold, 10);
            game.Sound.Play(SoundCue.Hit);
            if (Defeated)
            {
                Active = false; warning.gameObject.SetActive(false); game.SetArenaClosed(false);
                game.Effects.Burst(transform.position, MosslightArt.Teal, 35);
                game.Announce("THE WATCH IS OVER", "Carry the light to the bell beyond.");
                game.Sound.Play(SoundCue.Secret);
                gameObject.SetActive(false);
            }
        }
    }
}
