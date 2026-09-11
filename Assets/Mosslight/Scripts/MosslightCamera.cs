using UnityEngine;

namespace Mosslight
{
    public class MosslightCamera : MonoBehaviour
    {
        MosslightGame game;
        float trauma;
        Vector3 speed;
        public void Initialize(MosslightGame owner) { game = owner; Snap(); }
        public void Shake(float amount) { if (!game.ShakeDisabled) trauma = Mathf.Max(trauma, amount * game.tuning.screenShake); }
        Vector3 Target()
        {
            var hero = game.Hero;
            float x = Mathf.Clamp(hero.transform.position.x + hero.Facing * 2, 11.5f, 129);
            float y = Mathf.Clamp(hero.transform.position.y + 3, 3, 5.5f);
            if (game.Boss != null && game.Boss.Active) { x = 113; y = 3.6f; }
            return new Vector3(x, y, -10);
        }
        public void Snap() { if (game != null) transform.position = Target(); speed = Vector3.zero; }
        void LateUpdate()
        {
            if (game == null || !game.Playing) return;
            transform.position = Vector3.SmoothDamp(transform.position, Target(), ref speed, .22f);
            if (game.ShakeDisabled) trauma = 0;
            if (trauma > 0)
            {
                transform.position += new Vector3(Mathf.Sin(Time.time * 75), Mathf.Cos(Time.time * 93), 0) * trauma;
                trauma = Mathf.Max(0, trauma - Time.deltaTime * .5f);
            }
        }
    }
}
