using System.Collections.Generic;
using UnityEngine;

namespace Mosslight
{
    // All artwork is made from our own geometric shapes. No downloaded game assets.
    // The same tiny textures are reused by hundreds of sprites (one shared material).
    public static class MosslightArt
    {
        public static readonly Color Ink = Hex("101c26"), Deep = Hex("192b38"),
            Stone = Hex("294451"), Moss = Hex("70a08a"), Gold = Hex("edc58a"),
            Cream = Hex("f5e9cb"), Red = Hex("ee937e"), Teal = Hex("88dac8");
        public static Material Material;
        static readonly Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();

        public static Color Hex(string value)
        {
            ColorUtility.TryParseHtmlString("#" + value, out Color c);
            return c;
        }

        static Sprite Shape(string kind)
        {
            if (sprites.TryGetValue(kind, out var cached) && cached != null) return cached;
            const int n = 96;
            var tex = new Texture2D(n, n, TextureFormat.RGBA32, false) { name = "Mosslight " + kind, filterMode = FilterMode.Bilinear, wrapMode = TextureWrapMode.Clamp };
            var pixels = new Color[n * n];
            for (int y = 0; y < n; y++) for (int x = 0; x < n; x++)
            {
                float u = (x + .5f) / n * 2 - 1, v = (y + .5f) / n * 2 - 1;
                float d = Mathf.Sqrt(u * u + v * v);
                float a = 1;
                if (kind == "circle") a = Mathf.Clamp01((1 - d) * 48);
                if (kind == "glow") a = Mathf.Pow(Mathf.Clamp01(1 - d), 2.5f);
                if (kind == "diamond") a = Mathf.Clamp01((1 - Mathf.Abs(u) - Mathf.Abs(v)) * 48);
                if (kind == "leaf") a = Mathf.Clamp01((1 - Mathf.Abs(u) - v * v) * 48);
                if (kind == "ring") a = Mathf.Clamp01((.075f - Mathf.Abs(d - .88f)) * 48);
                pixels[y * n + x] = new Color(1, 1, 1, a);
            }
            tex.SetPixels(pixels); tex.Apply();
            var sprite = UnityEngine.Sprite.Create(tex, new Rect(0, 0, n, n), new Vector2(.5f, .5f), n);
            sprites[kind] = sprite;
            return sprite;
        }

        public static Transform Sprite(string name, Transform parent, Vector2 position, Vector2 size, Color color, int order = 0, string shape = "box", float angle = 0)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = position;
            go.transform.localScale = new Vector3(size.x, size.y, 1);
            go.transform.localRotation = Quaternion.Euler(0, 0, angle);
            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = Shape(shape); renderer.color = color; renderer.sortingOrder = order;
            if (Material != null) renderer.sharedMaterial = Material;
            return go.transform;
        }

        public static Transform Group(string name, Transform parent, Vector2 pos)
        {
            var t = new GameObject(name).transform;
            t.SetParent(parent, false); t.localPosition = pos;
            return t;
        }

        public static void Line(Transform parent, Vector2 a, Vector2 b, float width, Color color, int order)
        {
            Vector2 d = b - a;
            Sprite("Carved line", parent, (a + b) / 2, new Vector2(d.magnitude, width), color, order, "box", Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg);
        }

        public static Transform Hero(Transform parent)
        {
            var t = Group("Hero artwork", parent, Vector2.zero);
            Sprite("Cloak shadow", t, new Vector2(0, -.18f), new Vector2(.75f, .85f), Ink, 10, "leaf");
            Sprite("Sage cloak", t, new Vector2(0, -.1f), new Vector2(.63f, .86f), Moss, 11, "leaf");
            Sprite("Cloak seam", t, new Vector2(.06f, -.17f), new Vector2(.035f, .45f), Teal, 12);
            Sprite("Left boot", t, new Vector2(-.17f, -.49f), new Vector2(.16f, .22f), Ink, 12, "circle");
            Sprite("Right boot", t, new Vector2(.17f, -.49f), new Vector2(.16f, .22f), Ink, 12, "circle");
            Sprite("Porcelain helmet", t, new Vector2(0, .39f), new Vector2(.63f, .63f), Cream, 13, "circle");
            Sprite("Helmet crest", t, new Vector2(-.1f, .77f), new Vector2(.22f, .49f), Gold, 12, "leaf", 22);
            Sprite("Visor", t, new Vector2(.13f, .38f), new Vector2(.34f, .1f), Ink, 14);
            Sprite("Eye", t, new Vector2(.24f, .4f), new Vector2(.045f, .045f), Teal, 15, "circle");
            Sprite("Scarf", t, new Vector2(-.38f, .08f), new Vector2(.6f, .19f), Gold, 12, "leaf", -12);
            Sprite("Needle blade", t, new Vector2(.45f, -.17f), new Vector2(.08f, .8f), Cream, 14, "diamond", -22);
            return t;
        }

        public static Transform Enemy(Transform parent, EnemyKind kind)
        {
            var t = Group(kind + " artwork", parent, Vector2.zero);
            Color shell = kind == EnemyKind.Charger ? Hex("936b76") : kind == EnemyKind.Spitter ? Hex("75869f") : Hex("6c8171");
            for (int i = -1; i <= 1; i++)
                Sprite("Leg", t, new Vector2(i * .28f, -.33f), new Vector2(.1f, .48f), Ink, 9, "box", i * -28);
            Sprite("Shell", t, new Vector2(0, .02f), new Vector2(1.05f, .85f), shell, 10, "circle");
            Sprite("Shell ridge", t, new Vector2(-.15f, .16f), new Vector2(.7f, .09f), shell * 1.3f, 11, "leaf", -25);
            Sprite("Face", t, new Vector2(.4f, -.07f), new Vector2(.4f, .36f), Ink, 11, "circle");
            Sprite("Eye", t, new Vector2(.49f, .02f), new Vector2(.12f, .08f), Gold, 12, "circle");
            if (kind == EnemyKind.Charger)
                Sprite("Ram horn", t, new Vector2(.67f, .18f), new Vector2(.7f, .23f), Cream, 10, "diamond", 22);
            if (kind == EnemyKind.Spitter)
                for (int i = 0; i < 3; i++) Sprite("Spore cap", t, new Vector2((i - 1) * .28f, .49f), new Vector2(.4f, .35f), Teal, 12, "circle");
            return t;
        }

        public static Transform Boss(Transform parent)
        {
            var t = Group("The Bellkeeper artwork", parent, Vector2.zero);
            Sprite("Torn mantle", t, new Vector2(0, -.08f), new Vector2(1.7f, 2.5f), Hex("70576b"), 10, "leaf");
            Sprite("Breastplate", t, new Vector2(.1f, .21f), new Vector2(1.25f, 1.55f), Stone, 11, "diamond");
            Sprite("Chest lantern glow", t, new Vector2(.1f, .24f), new Vector2(2, 2), new Color(1, .65f, .3f, .6f), 12, "glow");
            Sprite("Chest lantern", t, new Vector2(.1f, .24f), new Vector2(.42f, .55f), Gold, 13, "diamond");
            Sprite("Crowned helm", t, new Vector2(0, 1.12f), new Vector2(.95f, .78f), Cream, 13, "diamond");
            Sprite("Helm slit", t, new Vector2(.12f, 1.17f), new Vector2(.64f, .085f), Ink, 14);
            for (int i = -1; i <= 1; i++) Sprite("Crown", t, new Vector2(i * .3f, 1.67f), new Vector2(.13f, .65f - Mathf.Abs(i) * .2f), Gold, 12, "diamond", -i * 20);
            Sprite("Hammer haft", t, new Vector2(1, .1f), new Vector2(.13f, 2.7f), Moss, 13, "box", -14);
            Sprite("Bell hammer", t, new Vector2(1.2f, .95f), new Vector2(.98f, .86f), Gold, 14, "diamond");
            return t;
        }
    }
}
