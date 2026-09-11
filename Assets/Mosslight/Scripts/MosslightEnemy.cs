using UnityEngine;

namespace Mosslight
{
    public enum EnemyKind { Walker, Charger, Spitter }

    public class MosslightEnemy : MonoBehaviour, IHurtable
    {
        public EnemyKind Kind { get; private set; }
        public int Health { get; private set; }
        public bool Alive => Health > 0;
        MosslightGame game;
        Vector2 home;
        Transform art;
        SpriteRenderer tell;
        float left, right, stateTime, stun;
        int direction = 1;
        // 0 = watching/patrolling, 1 = warning, 2 = attack, 3 = recovery.
        int state;

        public void Initialize(MosslightGame owner, EnemyKind kind, float patrolLeft, float patrolRight)
        {
            game = owner; Kind = kind; home = transform.position;
            left = patrolLeft; right = patrolRight;
            gameObject.layer = 9;
            var collider = gameObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true; collider.size = new Vector2(1.05f, .85f);
            art = MosslightArt.Enemy(transform, kind);
            tell = MosslightArt.Sprite("Attack warning", transform, Vector2.up * 1, new Vector2(.17f, .4f), MosslightArt.Gold, 18, "diamond").GetComponent<SpriteRenderer>();
            ResetEnemy();
        }

        public void ResetEnemy()
        {
            Health = Kind == EnemyKind.Charger ? 4 : 3;
            state = 0; stateTime = 1; stun = 0; direction = 1;
            transform.position = home; gameObject.SetActive(true);
        }

        void Update()
        {
            if (game == null || !game.Playing || !Alive) return;
            Vector2 delta = game.Hero.transform.position - transform.position;
            if (Mathf.Abs(delta.x) > 20) return;
            stateTime -= Time.deltaTime; stun -= Time.deltaTime;
            tell.enabled = state == 1;
            if (stun > 0) return;
            if (Kind == EnemyKind.Walker)
            {
                if (transform.position.x >= right) direction = -1;
                if (transform.position.x <= left) direction = 1;
                transform.Translate(Vector2.right * (direction * 1.35f * Time.deltaTime));
            }
            else if (state == 0 && stateTime <= 0 && Mathf.Abs(delta.x) < 9 && Mathf.Abs(delta.y) < 4)
            {
                direction = delta.x >= 0 ? 1 : -1;
                state = 1; stateTime = Kind == EnemyKind.Charger ? .8f : 1;
                game.Sound.Play(SoundCue.Warning, .4f);
            }
            else if (state == 1 && stateTime <= 0)
            {
                state = 2; stateTime = .65f;
                if (Kind == EnemyKind.Spitter)
                {
                    game.Projectile((Vector2)transform.position + new Vector2(direction * .65f, .25f), delta.normalized * 5.8f, false);
                    state = 3; stateTime = 1.8f;
                }
            }
            else if (state == 2)
            {
                float x = Mathf.Clamp(transform.position.x + direction * 8 * Time.deltaTime, left, right);
                transform.position = new Vector3(x, home.y);
                if (stateTime <= 0 || x <= left || x >= right) { state = 3; stateTime = 1.2f; }
            }
            else if (state == 3 && stateTime <= 0) { state = 0; stateTime = .6f; }

            art.localScale = new Vector3(direction, state == 1 ? .86f : 1, 1);
            if (Mathf.Abs(delta.x) < .78f && Mathf.Abs(delta.y) < .87f) game.Hero.Damage(1, transform.position);
        }

        public void Hit(int damage, Vector2 from)
        {
            if (!Alive) return;
            Health -= damage; stun = .16f;
            game.Effects.Burst(transform.position, MosslightArt.Gold, 8);
            game.Sound.Play(SoundCue.Hit);
            if (Health <= 0)
            {
                game.DefeatedEnemies++;
                gameObject.SetActive(false);
            }
        }
    }
}
