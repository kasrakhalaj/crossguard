using UnityEngine;

namespace Mosslight
{
    // Immediate-mode UI keeps the sample easy to read and needs no extra UI package.
    // All coordinates use a 1600 x 900 design canvas and scale to the game window.
    public class MosslightHUD : MonoBehaviour
    {
        MosslightGame game;
        GUIStyle small, body, heading, title, button, center, subtitle;
        bool styled;
        readonly Color muted = MosslightArt.Hex("9bacac");
        public void Initialize(MosslightGame owner) { game = owner; }

        GUIStyle Text(int size, Color color, FontStyle weight = FontStyle.Normal, TextAnchor align = TextAnchor.UpperLeft)
        {
            return new GUIStyle(GUI.skin.label) { fontSize = size, fontStyle = weight, alignment = align, wordWrap = true, normal = { textColor = color }, padding = new RectOffset(0, 0, 0, 0) };
        }
        void Styles()
        {
            if (styled) return;
            small = Text(15, muted); body = Text(20, MosslightArt.Cream);
            heading = Text(32, MosslightArt.Cream); title = Text(100, MosslightArt.Cream);
            center = Text(26, MosslightArt.Cream, FontStyle.Normal, TextAnchor.MiddleCenter);
            subtitle = Text(17, MosslightArt.Gold);
            button = new GUIStyle(GUI.skin.button) { fontSize = 21, alignment = TextAnchor.MiddleLeft, padding = new RectOffset(24, 15, 0, 0), normal = { textColor = MosslightArt.Cream }, hover = { textColor = Color.white } };
            styled = true;
        }
        void Panel(Rect rect, Color color) { Color old = GUI.color; GUI.color = color; GUI.DrawTexture(rect, Texture2D.whiteTexture); GUI.color = old; }
        void Label(float x, float y, float w, float h, string text, GUIStyle style) { GUI.Label(new Rect(x, y, w, h), text, style); }
        void Rule(float x, float y, float width) { Panel(new Rect(x, y, width, 1), new Color(.7f, .78f, .71f, .25f)); }
        void Diamond(float x, float y, float size, Color color)
        {
            var matrix = GUI.matrix; GUIUtility.RotateAroundPivot(45, new Vector2(x + size / 2, y + size / 2));
            Panel(new Rect(x, y, size, size), color); GUI.matrix = matrix;
        }

        void OnGUI()
        {
            if (game == null || game.Hero == null) return;
            Styles();
            Matrix4x4 original = GUI.matrix;
            // Letterbox the UI on narrow windows instead of stretching text.
            float scale = Mathf.Min(Screen.width / 1600f, Screen.height / 900f);
            Vector2 offset = new Vector2((Screen.width - 1600 * scale) / 2, (Screen.height - 900 * scale) / 2);
            GUI.matrix = Matrix4x4.TRS(offset, Quaternion.identity, Vector3.one * scale);
            if (game.Phase == GamePhase.Title) Title();
            else
            {
                HUD();
                if (game.Phase == GamePhase.Paused) Pause();
                if (game.Phase == GamePhase.Fallen) Fallen();
                if (game.Phase == GamePhase.Complete) Ending();
            }
            GUI.matrix = original;
        }

        void Title()
        {
            Panel(new Rect(0, 0, 1600, 900), new Color(.035f, .07f, .10f, .42f));
            Panel(new Rect(0, 0, 735, 900), new Color(.035f, .07f, .10f, .84f));
            Label(88, 88, 550, 30, "A SMALL ADVENTURE IN THE FORGOTTEN GARDEN", subtitle);
            Label(80, 186, 620, 135, "MOSSLIGHT", title);
            Rule(88, 338, 470);
            Label(88, 374, 470, 90, "Carry a little light through the ruins.\nWake the bell. Find your way home.", body);
            if (GUI.Button(new Rect(88, 512, 360, 64), "Begin the journey     →", button)) game.Begin();
            Label(88, 591, 450, 30, "ENTER  /  GAMEPAD A TO BEGIN", small);
            Label(88, 710, 545, 70, "MOVE  A / D      JUMP  SPACE\nSTRIKE  J / CLICK      DASH  SHIFT      REST  E", small);
            Rule(88, 807, 470);
            Label(88, 831, 560, 30, "LEARNING EDITION  /  01                         ORIGINAL UNITY PROTOTYPE", small);
            Label(1170, 799, 340, 36, "ONE GARDEN. ONE LAST WATCH.", subtitle);
        }

        void HUD()
        {
            Panel(new Rect(36, 28, 370, 115), new Color(.035f, .07f, .10f, .85f));
            Label(57, 44, 200, 24, "MOSSLIGHT", subtitle);
            for (int i = 0; i < game.Hero.MaxHealth; i++) Diamond(62 + i * 33, 88, 16, i < game.Hero.Health ? MosslightArt.Cream : MosslightArt.Stone);
            Label(281, 79, 102, 24, "DASH", small);
            Panel(new Rect(281, 111, 94, 3), MosslightArt.Stone);
            Panel(new Rect(281, 111, 94 * game.Hero.DashReady, 3), MosslightArt.Teal);
            Label(1160, 44, 375, 30, game.AreaName, new GUIStyle(small) { alignment = TextAnchor.UpperRight });
            Label(1230, 78, 305, 30, "LIGHT SEEDS  " + game.Seeds + " / 6", new GUIStyle(subtitle) { alignment = TextAnchor.UpperRight });
            if (game.AnnouncementLeft > 0 && game.Playing)
            {
                float alpha = Mathf.Clamp01(game.AnnouncementLeft);
                Color old = GUI.color; GUI.color = new Color(1, 1, 1, alpha);
                Label(410, 145, 780, 45, game.Announcement, new GUIStyle(heading) { alignment = TextAnchor.MiddleCenter });
                Label(390, 194, 820, 35, game.AnnouncementDetail, new GUIStyle(small) { alignment = TextAnchor.MiddleCenter });
                GUI.color = old;
            }
            string hint;
            float x = game.Hero.transform.position.x;
            if (game.NearLantern()) hint = "E / Y   REST AT THE LANTERN  ·  restore health";
            else if (x < 22) hint = "A D / ← →   move     SPACE   jump     J / CLICK   strike     SHIFT   dash";
            else if (x < 43) hint = "Hold jump to go higher. Dash across the gaps. Strike when close.";
            else if (x < 64) hint = "Something glows above the well. The high path is optional.";
            else if (x < 87) hint = "Watch the warning. Jump the charge. Strike during recovery.";
            else if (x < 102) hint = "Rest here. The last watch waits beyond the seal.";
            else if (!game.Boss.Defeated) hint = game.Boss.Telegraph;
            else hint = "Walk east. The dawn bell is waiting.";
            Panel(new Rect(260, 814, 1080, 49), new Color(.035f, .07f, .10f, .88f));
            Label(275, 825, 1050, 30, hint, new GUIStyle(small) { alignment = TextAnchor.MiddleCenter });
            Label(1410, 839, 150, 30, "ESC  PAUSE", small);
            if (game.Boss.Active)
            {
                Label(480, 731, 640, 30, "THE BELLKEEPER", new GUIStyle(subtitle) { alignment = TextAnchor.MiddleCenter });
                Panel(new Rect(480, 774, 640, 7), MosslightArt.Stone);
                Panel(new Rect(480, 774, 640f * game.Boss.Health / game.tuning.bossHealth, 7), MosslightArt.Gold);
            }
        }

        void Pause()
        {
            Panel(new Rect(0, 0, 1600, 900), new Color(.035f, .065f, .09f, .92f));
            Label(180, 160, 700, 75, "A MOMENT OF STILLNESS", heading);
            Label(180, 225, 650, 50, "Your light will wait.", body);
            if (GUI.Button(new Rect(180, 335, 400, 60), "Continue journey", button)) game.TogglePause();
            if (GUI.Button(new Rect(180, 415, 400, 60), game.Muted ? "Sound: off" : "Sound: on", button)) game.ToggleMute();
            if (GUI.Button(new Rect(180, 495, 400, 60), game.ShakeDisabled ? "Camera shake: off" : "Camera shake: on", button)) game.ToggleShake();
            if (GUI.Button(new Rect(180, 575, 400, 60), "Return to checkpoint", button)) { game.TogglePause(); game.Respawn(); }
            if (GUI.Button(new Rect(180, 655, 400, 60), "Quit", button)) game.Quit();
            Label(860, 340, 530, 45, "YOUR CONTROLS", subtitle);
            Label(860, 400, 590, 275, "Move         A / D or arrows · left stick\nJump         Space · A / Cross\nStrike        J or left click · X / Square\nDash         Shift or K · right bumper\nRest          E · Y / Triangle\nPause        Escape · Start", body);
            Label(860, 735, 520, 55, "Progress lasts for this session. Quitting starts a new journey next time.", small);
        }
        void Fallen()
        {
            Panel(new Rect(0, 0, 1600, 900), new Color(.035f, .07f, .10f, .72f));
            Label(400, 365, 800, 70, "THE LIGHT RETURNS", new GUIStyle(heading) { alignment = TextAnchor.MiddleCenter });
            Label(400, 435, 800, 40, "Breathe. The lantern remembers.", center);
        }
        void Ending()
        {
            Panel(new Rect(0, 0, 1600, 900), new Color(.035f, .07f, .10f, .89f));
            Label(280, 143, 1040, 30, "JOURNEY COMPLETE", new GUIStyle(subtitle) { alignment = TextAnchor.MiddleCenter });
            Label(230, 225, 1140, 120, "A LITTLE DAWN", new GUIStyle(title) { fontSize = 78, alignment = TextAnchor.MiddleCenter });
            Label(400, 375, 800, 90, "The bell rings again.\nSomewhere in the garden, another light wakes.", center);
            Rule(525, 507, 550);
            string time = System.TimeSpan.FromSeconds(game.PlaySeconds).ToString(@"mm\:ss");
            Label(460, 550, 680, 85, time + "  JOURNEY     /     " + game.Seeds + " OF 6 SEEDS     /     " + game.Falls + " RETURNS\n" + (game.SecretFound ? "THE HIDDEN HEART WAS FOUND" : "A HIDDEN HEART STILL WAITS IN THE WELL"), new GUIStyle(small) { alignment = TextAnchor.MiddleCenter });
            if (GUI.Button(new Rect(600, 684, 400, 60), "Another journey     [ R ]", button)) game.NewJourney();
            Label(440, 809, 720, 35, "MADE TO BE PLAYED, TAKEN APART, AND LEARNED FROM.", new GUIStyle(subtitle) { alignment = TextAnchor.MiddleCenter });
        }
    }
}
