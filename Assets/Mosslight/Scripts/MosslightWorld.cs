using UnityEngine;

namespace Mosslight
{
    // This is the level recipe. Positions are in Unity units. Ground top is y = -1.
    // Keeping the map in one file makes it easy to move an obstacle and play again.
    public static class MosslightWorld
    {
        public static void Build(MosslightGame game)
        {
            Random.InitState(716); // A fixed seed makes decoration identical on every run.
            var world = MosslightArt.Group("WORLD — platforms, scenery and encounters", game.transform, Vector2.zero);
            Background(world);
            Ground(world, 0, 31); Ground(world, 35, 63); Ground(world, 68, 87); Ground(world, 91, 140);
            Platform(world, new Vector2(23, 1), 3.8f);
            Platform(world, new Vector2(30.8f, 1.8f), 2.8f);
            Platform(world, new Vector2(38, 1), 3);
            Platform(world, new Vector2(45, .8f), 3.2f);
            Platform(world, new Vector2(49.5f, 2.5f), 3);
            Platform(world, new Vector2(54.5f, 4.3f), 5);
            Platform(world, new Vector2(60, 2.5f), 2.7f);
            Platform(world, new Vector2(67, 1.6f), 2.3f);
            Platform(world, new Vector2(77, .8f), 3.8f);
            Platform(world, new Vector2(82, 2.6f), 3.6f);
            game.Shortcut = Platform(world, new Vector2(89, -1), 4.2f);
            game.Shortcut.name = "Shortcut bridge — opens from east side";
            game.Shortcut.SetActive(false);
            // The outer walls keep even repeated dashes inside the playable map.
            Solid(world, "West wall", new Vector2(-1, 3), new Vector2(2, 15), MosslightArt.Ink);
            Solid(world, "East wall", new Vector2(141, 3), new Vector2(2, 15), MosslightArt.Ink);
            game.EntranceGate = Gate(world, 102);
            game.EntranceGate.SetActive(false);
            game.ExitGate = Gate(world, 124);
            Lantern(world, 8); Lantern(world, 59); Lantern(world, 97);
            game.Lanterns = new[] { new Vector2(8, -.35f), new Vector2(59, -.35f), new Vector2(97, -.35f) };
            foreach (Vector2 pos in new[] { new Vector2(17, 0), new Vector2(30.8f, 2.5f), new Vector2(41, 0), new Vector2(67, 2.3f), new Vector2(82, 3.3f), new Vector2(95, 0) })
                AddMote(game, world, pos, false);
            AddMote(game, world, new Vector2(54.5f, 5.05f), true);
            AddEnemy(game, world, EnemyKind.Walker, 25, 22, 29);
            AddEnemy(game, world, EnemyKind.Walker, 39, 36, 42);
            AddEnemy(game, world, EnemyKind.Spitter, 47, 44, 51);
            AddEnemy(game, world, EnemyKind.Charger, 57, 52, 62);
            AddEnemy(game, world, EnemyKind.Charger, 75, 70, 84);
            AddEnemy(game, world, EnemyKind.Spitter, 85, 79, 86);
            var boss = new GameObject("BOSS — The Bellkeeper"); boss.transform.SetParent(world);
            game.Boss = boss.AddComponent<MosslightBoss>(); game.Boss.Initialize(game);
            FinalBell(world, 132);
        }

        static void Background(Transform world)
        {
            MosslightArt.Sprite("Night sky", world, new Vector2(70, 4), new Vector2(170, 30), MosslightArt.Ink, -100);
            for (int i = 0; i < 24; i++)
            {
                float x = i * 6.5f;
                float y = Random.Range(3, 9f);
                MosslightArt.Sprite("Distant vault", world, new Vector2(x, y), new Vector2(Random.Range(3, 6), 19), MosslightArt.Deep, -90, "leaf");
            }
            for (int i = 0; i < 13; i++)
            {
                float x = 1 + i * 11;
                Color arch = MosslightArt.Hex("243b49");
                MosslightArt.Sprite("Ancient arch", world, new Vector2(x, 5.5f), new Vector2(8.8f, 10.5f), arch, -80, "ring");
                MosslightArt.Sprite("Arch pillar", world, new Vector2(x - 3.85f, .9f), new Vector2(.65f, 8), arch, -80);
                MosslightArt.Sprite("Arch pillar", world, new Vector2(x + 3.85f, .9f), new Vector2(.65f, 8), arch, -80);
                MosslightArt.Sprite("High window", world, new Vector2(x, 5.9f), new Vector2(2.5f, 5), new Color(.32f, .52f, .56f, .12f), -78, "leaf");
                MosslightArt.Sprite("Shaft of light", world, new Vector2(x + 1, 2), new Vector2(2.3f, 16), new Color(.6f, .83f, .74f, .035f), -76, "box", -22);
                MosslightArt.Line(world, new Vector2(x - .65f, 4), new Vector2(x - .65f, 7.6f), .035f, arch, -77);
                MosslightArt.Line(world, new Vector2(x + .65f, 4), new Vector2(x + .65f, 7.6f), .035f, arch, -77);
            }
            for (int i = 0; i < 130; i++)
            {
                float x = Random.Range(0, 140f), y = Random.Range(0, 12f);
                MosslightArt.Sprite("Drifting dust", world, new Vector2(x, y), Vector2.one * Random.Range(.025f, .065f), new Color(.75f, .9f, .78f, Random.Range(.15f, .5f)), -40, "circle");
            }
            for (int i = 0; i < 36; i++)
            {
                float x = i * 4;
                MosslightArt.Sprite("Fallen monument", world, new Vector2(x, -.1f), new Vector2(Random.Range(.5f, 1.6f), Random.Range(1.3f, 3.7f)), MosslightArt.Hex("1e3440"), -35, "diamond", Random.Range(-12, 12));
            }
            // The arena has its own circular silhouette, visible before the fight.
            MosslightArt.Sprite("Arena halo", world, new Vector2(113, 4), new Vector2(12, 12), MosslightArt.Hex("3c4d55"), -60, "ring");
            MosslightArt.Sprite("Arena inner halo", world, new Vector2(113, 4), new Vector2(10, 10), MosslightArt.Hex("2d424d"), -59, "ring");
            for (int i = 0; i < 12; i++)
            {
                float a = i * Mathf.PI / 6;
                MosslightArt.Sprite("Halo rune", world, new Vector2(113 + Mathf.Cos(a) * 4.9f, 4 + Mathf.Sin(a) * 4.9f), new Vector2(.16f, .46f), MosslightArt.Gold * .6f, -58, "diamond", a * Mathf.Rad2Deg - 90);
            }
        }

        static GameObject Solid(Transform parent, string name, Vector2 center, Vector2 size, Color color)
        {
            var t = MosslightArt.Sprite(name, parent, center, size, color, 0);
            t.gameObject.layer = 8;
            t.gameObject.AddComponent<BoxCollider2D>();
            return t.gameObject;
        }

        static void Ground(Transform world, float start, float end)
        {
            float width = end - start;
            Solid(world, "Ground " + start + "–" + end, new Vector2((start + end) / 2, -3), new Vector2(width, 4), MosslightArt.Deep);
            MosslightArt.Sprite("Stone lip", world, new Vector2((start + end) / 2, -1.1f), new Vector2(width, .24f), MosslightArt.Stone, 2);
            MosslightArt.Sprite("Moss edge", world, new Vector2((start + end) / 2, -.97f), new Vector2(width, .06f), MosslightArt.Moss, 3);
            for (float x = start + .2f; x < end; x += .47f)
            {
                float h = Random.Range(.08f, .35f);
                MosslightArt.Sprite("Grass", world, new Vector2(x, -.98f + h / 2), new Vector2(.09f, h), MosslightArt.Moss * Random.Range(.7f, 1.1f), 4, "leaf", Random.Range(-22, 22));
            }
            for (float x = start + 1; x < end; x += 2)
            {
                MosslightArt.Line(world, new Vector2(x, -1.35f), new Vector2(x - .3f, -2.1f), .025f, MosslightArt.Stone, 2);
                if (Random.value > .6f) Plant(world, new Vector2(x, -.95f));
            }
        }

        static void Plant(Transform parent, Vector2 pos)
        {
            var t = MosslightArt.Group("Fern", parent, pos);
            MosslightArt.Line(t, Vector2.zero, new Vector2(.08f, .8f), .025f, MosslightArt.Moss, 5);
            for (int i = 0; i < 4; i++)
            {
                MosslightArt.Sprite("Fern leaf", t, new Vector2(-.13f, .17f + i * .16f), new Vector2(.15f, .43f - i * .045f), MosslightArt.Moss * .78f, 5, "leaf", 55);
                MosslightArt.Sprite("Fern leaf", t, new Vector2(.16f, .21f + i * .16f), new Vector2(.15f, .43f - i * .045f), MosslightArt.Moss, 5, "leaf", -55);
            }
        }

        static GameObject Platform(Transform parent, Vector2 top, float width)
        {
            var root = MosslightArt.Group("Platform — top " + top, parent, Vector2.zero);
            Solid(root, "Collidable stone", top + Vector2.down * .2f, new Vector2(width, .4f), MosslightArt.Stone);
            MosslightArt.Sprite("Gold moss lip", root, top, new Vector2(width, .06f), MosslightArt.Moss, 3);
            MosslightArt.Sprite("Hanging roots", root, top + Vector2.down * .65f, new Vector2(width * .65f, 1.1f), MosslightArt.Deep, -1, "leaf");
            return root.gameObject;
        }

        static GameObject Gate(Transform world, float x)
        {
            var root = MosslightArt.Group("Seal gate", world, Vector2.zero);
            var hit = Solid(root, "Seal collider", new Vector2(x, 3.5f), new Vector2(.35f, 9), new Color(.45f, .76f, .69f, .18f));
            for (int i = 0; i < 8; i++) MosslightArt.Sprite("Seal diamond", root, new Vector2(x, i * 1.15f - .6f), new Vector2(.28f, .55f), MosslightArt.Teal, 8, "diamond");
            return root.gameObject;
        }

        static void Lantern(Transform parent, float x)
        {
            var t = MosslightArt.Group("CHECKPOINT — rest lantern", parent, new Vector2(x, -1));
            MosslightArt.Sprite("Lantern plinth", t, new Vector2(0, .14f), new Vector2(1.4f, .28f), MosslightArt.Stone, 5);
            MosslightArt.Sprite("Lantern stem", t, new Vector2(0, .72f), new Vector2(.09f, 1.3f), MosslightArt.Moss, 6);
            MosslightArt.Sprite("Warm pool", t, new Vector2(0, 1.5f), Vector2.one * 5, new Color(1, .75f, .4f, .3f), 6, "glow");
            MosslightArt.Sprite("Lantern frame", t, new Vector2(0, 1.4f), new Vector2(.66f, .85f), MosslightArt.Gold, 7, "diamond");
            MosslightArt.Sprite("Lantern center", t, new Vector2(0, 1.4f), new Vector2(.3f, .45f), MosslightArt.Cream, 8, "diamond");
        }

        static void FinalBell(Transform parent, float x)
        {
            var t = MosslightArt.Group("EXIT — the dawn bell", parent, new Vector2(x, 0));
            MosslightArt.Sprite("Dawn glow", t, new Vector2(0, 3.4f), Vector2.one * 14, new Color(.7f, .84f, .58f, .4f), -20, "glow");
            MosslightArt.Line(t, new Vector2(0, 3.7f), new Vector2(0, 10), .08f, MosslightArt.Gold, 3);
            MosslightArt.Sprite("Bronze bell", t, new Vector2(0, 2.9f), new Vector2(2.4f, 2.8f), MosslightArt.Gold, 4, "leaf");
            MosslightArt.Sprite("Bell lip", t, new Vector2(0, 1.8f), new Vector2(2.5f, .22f), MosslightArt.Cream, 5);
            MosslightArt.Sprite("Bell clapper", t, new Vector2(0, 1.5f), new Vector2(.26f, .55f), MosslightArt.Gold, 4, "circle");
        }

        static void AddEnemy(MosslightGame game, Transform world, EnemyKind kind, float x, float left, float right)
        {
            var t = MosslightArt.Group(kind.ToString(), world, new Vector2(x, -.53f));
            var enemy = t.gameObject.AddComponent<MosslightEnemy>(); enemy.Initialize(game, kind, left, right);
            game.Enemies.Add(enemy);
        }

        static void AddMote(MosslightGame game, Transform parent, Vector2 position, bool secret)
        {
            var t = MosslightArt.Group(secret ? "SECRET — heart seed" : "COLLECTIBLE — light seed", parent, position);
            MosslightArt.Sprite("Seed glow", t, Vector2.zero, Vector2.one * 1.9f, new Color(.6f, 1, .8f, .45f), 6, "glow");
            MosslightArt.Sprite("Seed", t, Vector2.zero, new Vector2(secret ? .42f : .24f, secret ? .56f : .36f), secret ? MosslightArt.Gold : MosslightArt.Teal, 7, "diamond");
            var mote = t.gameObject.AddComponent<MosslightMote>(); mote.Initialize(game, secret);
        }
    }

    public class MosslightMote : MonoBehaviour
    {
        MosslightGame game;
        bool secret;
        Vector3 home;
        public void Initialize(MosslightGame owner, bool isSecret) { game = owner; secret = isSecret; home = transform.position; }
        void Update()
        {
            transform.position = home + Vector3.up * (Mathf.Sin(Time.time * 2.5f + home.x) * .1f);
            if (!game.Playing || Vector2.Distance(game.Hero.transform.position, transform.position) > .9f) return;
            if (secret) { game.SecretFound = true; game.Hero.Heal(); game.Announce("A HEART REMEMBERED", "Maximum health increased. The garden keeps its gifts."); }
            else game.Seeds++;
            game.Sound.Play(secret ? SoundCue.Secret : SoundCue.Seed);
            game.Effects.Burst(transform.position, MosslightArt.Teal, 12);
            Destroy(gameObject);
        }
    }
}
