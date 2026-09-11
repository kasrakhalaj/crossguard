using System.Collections;
using UnityEngine;

namespace Crossguard.Combat
{
    /// <summary>
    /// Manages hitstop (momentary time scale freeze) to deliver visceral, kinetic game feel on weapon clashes.
    /// </summary>
    public class HitstopManager : MonoBehaviour
    {
        private static HitstopManager instance;
        private Coroutine activeHitstopCoroutine;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
        }

        /// <summary>
        /// Triggers hitstop for a given number of frames (assumes 60 FPS standard).
        /// </summary>
        /// <param name="frames">Number of frames to freeze (typically 1 to 8 frames).</param>
        public static void FreezeFrames(int frames)
        {
            if (instance == null)
            {
                // Auto-create if not present in scene
                GameObject go = new GameObject("HitstopManager");
                instance = go.AddComponent<HitstopManager>();
            }

            float duration = frames / 60f;
            instance.StartHitstop(duration);
        }

        /// <summary>
        /// Triggers hitstop for a specific duration in seconds.
        /// </summary>
        public static void Freeze(float durationSeconds)
        {
            if (instance == null)
            {
                GameObject go = new GameObject("HitstopManager");
                instance = go.AddComponent<HitstopManager>();
            }

            instance.StartHitstop(durationSeconds);
        }

        private void StartHitstop(float duration)
        {
            if (activeHitstopCoroutine != null)
            {
                StopCoroutine(activeHitstopCoroutine);
            }
            activeHitstopCoroutine = StartCoroutine(HitstopRoutine(duration));
        }

        private IEnumerator HitstopRoutine(float duration)
        {
            Time.timeScale = 0.02f; // Near dead-stop, allowing slight micro-drift
            yield return new WaitForSecondsRealtime(duration);
            Time.timeScale = 1.0f;
            activeHitstopCoroutine = null;
        }

        private void OnDestroy()
        {
            Time.timeScale = 1.0f;
        }
    }
}
