using System.Collections.Generic;
using UnityEngine;

namespace Mosslight
{
    public enum SoundCue { Jump, Dash, Swing, Hit, Hurt, Seed, Bell, Warning, Secret }

    // Small synthesized sounds make this project completely self-contained.
    // Later, replace these clips with your own recordings without changing gameplay.
    public class MosslightSound : MonoBehaviour
    {
        AudioSource source, ambience;
        readonly Dictionary<SoundCue, AudioClip> clips = new Dictionary<SoundCue, AudioClip>();
        public void Initialize(float volume)
        {
            source = gameObject.AddComponent<AudioSource>(); source.volume = volume;
            source.spatialBlend = 0;
            foreach (SoundCue cue in System.Enum.GetValues(typeof(SoundCue))) clips[cue] = Make(cue);
            ambience = gameObject.AddComponent<AudioSource>(); ambience.loop = true; ambience.volume = volume * .12f;
            const int sampleRate = 22050, seconds = 8;
            var data = new float[sampleRate * seconds];
            for (int i = 0; i < data.Length; i++)
            {
                float t = i / (float)sampleRate;
                data[i] = (Mathf.Sin(t * 2 * Mathf.PI * 110) + .5f * Mathf.Sin(t * 2 * Mathf.PI * 165) + .25f * Mathf.Sin(t * 2 * Mathf.PI * 220)) * .1f;
            }
            ambience.clip = AudioClip.Create("Garden drone (original synthesis)", data.Length, 1, sampleRate, false);
            ambience.clip.SetData(data, 0); ambience.Play();
        }
        public void Play(SoundCue cue, float volume = 1) { source.PlayOneShot(clips[cue], volume); }
        public void SetVolume(float volume) { source.volume = volume; ambience.volume = volume * .12f; }
        AudioClip Make(SoundCue cue)
        {
            float duration = cue == SoundCue.Bell || cue == SoundCue.Secret ? 1.4f : .2f;
            float frequency = cue == SoundCue.Seed ? 880 : cue == SoundCue.Secret ? 660 : cue == SoundCue.Bell ? 220 : cue == SoundCue.Jump ? 400 : cue == SoundCue.Warning ? 165 : 120;
            const int rate = 22050;
            var data = new float[Mathf.CeilToInt(rate * duration)];
            var random = new System.Random(7);
            float phase = 0;
            for (int i = 0; i < data.Length; i++)
            {
                float t = i / (float)rate, normalized = t / duration;
                float f = frequency * (cue == SoundCue.Jump ? 1 + normalized : 1 - .2f * normalized);
                phase += 2 * Mathf.PI * f / rate;
                float note = Mathf.Sin(phase) * .55f + Mathf.Sin(phase * 2.01f) * .2f;
                if (cue == SoundCue.Swing || cue == SoundCue.Dash || cue == SoundCue.Hit || cue == SoundCue.Hurt)
                    note = note * .3f + ((float)random.NextDouble() * 2 - 1) * .5f;
                data[i] = note * Mathf.Exp(-normalized * 5) * Mathf.Min(1, t * 150) * .45f;
            }
            var clip = AudioClip.Create(cue.ToString(), data.Length, 1, rate, false); clip.SetData(data, 0); return clip;
        }
    }
}
