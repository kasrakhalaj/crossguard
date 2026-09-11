using UnityEngine;

namespace Mosslight
{
    public interface IHurtable { void Hit(int damage, Vector2 from); }

    public class MosslightHero : MonoBehaviour
    {
        public const int GroundMask = 1 << 8, EnemyMask = 1 << 9;
        public int Health { get; private set; }
        public int MaxHealth => game.tuning.playerHealth + (game.SecretFound ? 1 : 0);
        public bool Grounded { get; private set; }
        public bool Dashing => dashLeft > 0;
        public float DashReady => 1 - Mathf.Clamp01(dashWait / game.tuning.dashCooldown);
        public int Facing { get; private set; } = 1;
        public Rigidbody2D Body { get; private set; }
        public Transform Artwork { get; private set; }
        MosslightGame game;
        HeroInput input;
        float dashLeft, dashWait, attackWait, invulnerability, coyote, bufferedJump, knockback;
        readonly Collider2D[] hits = new Collider2D[12];
        ContactFilter2D attackFilter;

        public void Initialize(MosslightGame owner)
        {
            game = owner;
            Body = gameObject.AddComponent<Rigidbody2D>();
            Body.gravityScale = 0;
            Body.constraints = RigidbodyConstraints2D.FreezeRotation;
            Body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            Body.interpolation = RigidbodyInterpolation2D.Interpolate;
            var shape = gameObject.AddComponent<CapsuleCollider2D>();
            shape.size = new Vector2(.6f, 1.2f);
            var material = new PhysicsMaterial2D("Frictionless hero") { friction = 0, bounciness = 0 };
            shape.sharedMaterial = material;
            Artwork = MosslightArt.Hero(transform);
            attackFilter = new ContactFilter2D { useLayerMask = true, layerMask = EnemyMask, useTriggers = true };
            Health = MaxHealth;
        }

        void Update()
        {
            if (game == null || !game.Playing) return;
            float dt = Time.deltaTime;
            input = game.Automated ? game.TestInput : MosslightInput.Read();
            dashWait -= dt; attackWait -= dt; invulnerability -= dt; knockback -= dt;
            if (input.jumpPressed) bufferedJump = game.tuning.jumpBuffer;
            else bufferedJump -= dt;
            if (Mathf.Abs(input.move) > .1f && !Dashing) Facing = input.move > 0 ? 1 : -1;
            if (input.dash && dashWait <= 0)
            {
                dashLeft = game.tuning.dashDuration;
                dashWait = game.tuning.dashCooldown;
                game.Sound.Play(SoundCue.Dash);
                game.Effects.Burst(transform.position, MosslightArt.Teal, 10);
            }
            if (input.attack && attackWait <= 0 && !Dashing) Attack();
            if (input.interact) game.TryRest();
            if (transform.position.y < -5) Damage(1, transform.position, true);

            float bob = Grounded ? Mathf.Sin(Time.time * 14) * Mathf.Abs(input.move) * .035f : .015f;
            Artwork.localPosition = new Vector3(0, bob);
            Artwork.localScale = new Vector3(Facing * (Dashing ? 1.2f : 1), Dashing ? .82f : 1, 1);
            // Brief blinking communicates the grace period after a hit.
            Artwork.gameObject.SetActive(invulnerability <= 0 || Mathf.FloorToInt(Time.time * 15) % 2 == 0);
        }

        void FixedUpdate()
        {
            if (game == null || !game.Playing) return;
            float dt = Time.fixedDeltaTime;
            Grounded = Physics2D.OverlapBox((Vector2)transform.position + Vector2.down * .63f, new Vector2(.44f, .11f), 0, GroundMask) != null;
            coyote = Grounded ? game.tuning.coyoteTime : coyote - dt;
            Vector2 velocity = Body.linearVelocity;
            if (dashLeft > 0)
            {
                dashLeft -= dt;
                velocity = new Vector2(Facing * game.tuning.dashSpeed, 0);
            }
            else
            {
                if (knockback <= 0) velocity.x = Mathf.MoveTowards(velocity.x, input.move * game.tuning.runSpeed, (Grounded ? 65 : 40) * dt);
                // Short button presses give small hops; holding gives a full jump.
                float gravityMultiplier = velocity.y > 0 && !input.jumpHeld ? 2.3f : 1;
                velocity.y = Mathf.Max(velocity.y - game.tuning.gravity * gravityMultiplier * dt, -24);
                if (bufferedJump > 0 && coyote > 0)
                {
                    velocity.y = game.tuning.jumpSpeed;
                    coyote = 0; bufferedJump = 0;
                    game.Sound.Play(SoundCue.Jump);
                    game.Effects.Burst((Vector2)transform.position + Vector2.down * .6f, MosslightArt.Moss, 5);
                }
            }
            Body.linearVelocity = velocity;
        }

        void Attack()
        {
            attackWait = game.tuning.attackCooldown;
            Vector2 center = (Vector2)transform.position + new Vector2(Facing * .88f, .08f);
            int count = Physics2D.OverlapBox(center, new Vector2(game.tuning.attackReach, 1.4f), 0, attackFilter, hits);
            for (int i = 0; i < count; i++) hits[i].GetComponent<IHurtable>()?.Hit(1, transform.position);
            game.Effects.Slash(transform.position, Facing);
            game.Sound.Play(SoundCue.Swing);
        }

        public void Damage(int amount, Vector2 source, bool falling = false)
        {
            if (!game.Playing || (!falling && (invulnerability > 0 || Dashing))) return;
            Health = Mathf.Max(0, Health - amount);
            invulnerability = game.tuning.damageInvulnerability;
            knockback = .2f;
            Body.linearVelocity = new Vector2((transform.position.x >= source.x ? 1 : -1) * 7, 6);
            game.Effects.Burst(transform.position, MosslightArt.Red, 14);
            game.CameraRig.Shake(.22f);
            game.Sound.Play(SoundCue.Hurt);
            if (Health <= 0) game.HeroDied();
            else if (falling) Teleport(game.Checkpoint);
        }

        public void Heal() { Health = MaxHealth; }
        public void Revive(Vector2 point)
        {
            Teleport(point); Health = MaxHealth; invulnerability = 1;
            dashLeft = dashWait = knockback = attackWait = bufferedJump = coyote = 0;
            input = default; Artwork.gameObject.SetActive(true);
        }
        public void Teleport(Vector2 point)
        {
            Body.position = point; transform.position = point; Body.linearVelocity = Vector2.zero;
        }
    }
}
