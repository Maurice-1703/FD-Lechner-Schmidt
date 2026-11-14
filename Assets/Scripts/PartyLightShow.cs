using UnityEngine;

public class PartyLightShow : MonoBehaviour
{
    [SerializeField] private Light[] lights = System.Array.Empty<Light>();
    [SerializeField, Min(0.1f)] private float cycleSeconds = 2.4f;

    private float[] baseIntensities;

    private void Awake()
    {
        baseIntensities = new float[lights.Length];

        for (int i = 0; i < lights.Length; i++)
        {
            if (lights[i] != null)
                baseIntensities[i] = lights[i].intensity;
        }
    }

    private void Update()
    {
        for (int i = 0; i < lights.Length; i++)
        {
            if (lights[i] == null)
                continue;

            float phase = Time.time / cycleSeconds + (float)i / lights.Length;
            float pulse = 0.5f + 0.5f * Mathf.Sin(phase * 2f * Mathf.PI);
            lights[i].intensity = baseIntensities[i] * Mathf.Lerp(0.3f, 1f, pulse);
        }
    }
}