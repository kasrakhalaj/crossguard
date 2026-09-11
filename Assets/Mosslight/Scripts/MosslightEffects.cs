using System.Collections.Generic;
using UnityEngine;

namespace Mosslight
{
    public class MosslightEffects : MonoBehaviour
    {
        class Fleck { public Transform t; public SpriteRenderer r; public Vector2 velocity; public float life, total; }
        readonly List<Fleck> flecks = new List<Fleck>();

        public void Burst(Vector2 point, Color color, int count)
        {
            for (int i = 0; i < count; i++)
            {
                float life = Random.Range(.22f, .6f);
                var t = MosslightArt.Sprite("Fleck", transform, point, Vector2.one * Random.Range(.04f, .12f), color, 30, "diamond", Random.Range(0, 360));
                flecks.Add(new Fleck { t = t, r = t.GetComponent<SpriteRenderer>(), velocity = Random.insideUnitCircle * 5, life = life, total = life });
            }
        }

        public void Slash(Vector2 point, int facing)
        {
            // A curved sweep, drawn as small strokes; the damage is in Hero.Attack.
            for (int i = 0; i < 11; i++)
            {
                float angle = Mathf.Lerp(-65, 65, i / 10f) * Mathf.Deg2Rad;
                var p = point + new Vector2(Mathf.Cos(angle) * facing * 1.35f, Mathf.Sin(angle) * .85f);
                var t = MosslightArt.Sprite("Sword arc", transform, p, new Vector2(.09f, .34f), MosslightArt.Cream, 32, "leaf", -angle * Mathf.Rad2Deg * facing);
                flecks.Add(new Fleck { t = t, r = t.GetComponent<SpriteRenderer>(), life = .13f, total = .13f });
            }
        }

        void Update()
        {
            for (int i = flecks.Count - 1; i >= 0; i--)
            {
                var f = flecks[i]; f.life -= Time.deltaTime;
                if (f.life <= 0) { Destroy(f.t.gameObject); flecks.RemoveAt(i); continue; }
                f.t.Translate(f.velocity * Time.deltaTime, Space.World);
                f.velocity += Vector2.down * (5 * Time.deltaTime);
                Color c = f.r.color; c.a = f.life / f.total; f.r.color = c;
            }
        }
    }

    public class MosslightProjectile : MonoBehaviour
    {
        MosslightGame game;
        Vector2 velocity;
        float life = 5;
        bool wave;
        public void Initialize(MosslightGame owner, Vector2 speed, bool groundWave)
        {
            game = owner; velocity = speed; wave = groundWave;
            MosslightArt.Sprite("Projectile aura", transform, Vector2.zero, Vector2.one * 1.2f, new Color(1, .55f, .4f, .5f), 15, "glow");
            MosslightArt.Sprite("Projectile", transform, Vector2.zero, wave ? new Vector2(.3f, .7f) : Vector2.one * .24f, MosslightArt.Red, 16, "diamond");
        }
        void Update()
        {
            if (!game.Playing) return;
            Vector2 previous = transform.position;
            Vector2 next = previous + velocity * Time.deltaTime;
            // A segment check avoids fast projectiles skipping over thin walls.
            if (!wave && Physics2D.Linecast(previous, next, MosslightHero.GroundMask).collider != null) { Destroy(gameObject); return; }
            transform.position = next;
            Vector2 delta = (Vector2)game.Hero.transform.position - next;
            if (Mathf.Abs(delta.x) < .5f && Mathf.Abs(delta.y) < (wave ? .75f : .65f))
            {
                game.Hero.Damage(1, next); Destroy(gameObject); return;
            }
            life -= Time.deltaTime;
            if (life <= 0) Destroy(gameObject);
        }
    }
}
