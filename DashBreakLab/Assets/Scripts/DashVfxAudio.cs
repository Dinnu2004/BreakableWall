using UnityEngine;

public static class DashVfxAudio
{
    public static void Burst(Vector3 position, Color color, int count = 28)
    {
        var go = new GameObject("Break Burst"); go.transform.position = position;
        var ps = go.AddComponent<ParticleSystem>();
        var main = ps.main; main.startColor = color; main.startSize = .12f; main.startLifetime = .55f; main.startSpeed = 5f; main.maxParticles = count;
        var emission = ps.emission; emission.rateOverTime = 0; emission.SetBursts(new[] { new ParticleSystem.Burst(0, count) });
        var shape = ps.shape; shape.shapeType = ParticleSystemShapeType.Sphere; shape.radius = .15f;
        var trails = ps.trails; trails.enabled = true; trails.ratio = .4f; trails.lifetime = .2f;
        Object.Destroy(go, 1.5f); ps.Play(); PlayTone(position, 420f, .16f, .25f);
    }

    public static void Dash(Vector3 position)
    {
        var go = new GameObject("Dash Trail"); go.transform.position = position;
        var ps = go.AddComponent<ParticleSystem>(); var main = ps.main; main.startColor = new Color(.1f,.85f,1f,.8f); main.startSize = .22f; main.startLifetime = .25f; main.startSpeed = .3f; main.maxParticles = 120;
        var emission = ps.emission; emission.rateOverTime = 70; var shape = ps.shape; shape.shapeType = ParticleSystemShapeType.Cone; shape.angle = 8; shape.radius = .25f;
        var vel = ps.velocityOverLifetime; vel.enabled = true; vel.z = -2f; Object.Destroy(go, .5f); ps.Play(); PlayTone(position, 120f, .12f, .18f);
    }

    static void PlayTone(Vector3 position, float frequency, float duration, float volume)
    {
        var go = new GameObject("Gameplay SFX"); go.transform.position = position; var source = go.AddComponent<AudioSource>(); source.spatialBlend = .8f; source.volume = volume; source.clip = MakeTone(frequency, duration); source.Play(); Object.Destroy(go, duration + .1f);
    }
    static AudioClip MakeTone(float frequency, float duration)
    {
        int rate = 44100, samples = Mathf.CeilToInt(rate * duration); var clip = AudioClip.Create("Gameplay Tone", samples, 1, rate, false); var data = new float[samples];
        for (int i = 0; i < samples; i++) { float t = i / (float)rate; float envelope = 1f - i / (float)samples; data[i] = Mathf.Sin(2f * Mathf.PI * frequency * t) * envelope * .35f; }
        clip.SetData(data, 0); return clip;
    }
}