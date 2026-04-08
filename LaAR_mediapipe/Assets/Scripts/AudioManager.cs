using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public class PoseAudio
    {
        public PoseDetector.HandPose pose;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(0.1f, 2f)]
        public float crossfadeTime = 0.3f; // Tiempo de transición por pose
    }

    public PoseAudio[] poseAudios;
    public PoseDetector poseDetector;

    // Configuración global
    [Header("Configuración de Audio")]
    public float defaultCrossfadeTime = 0.3f;
    public float minDelayBetweenPoses = 0.2f;

    [Header("Debug")]
    public bool showDebugLogs = true;

    // AudioSources para crossfade
    private AudioSource audioSourceA;
    private AudioSource audioSourceB;
    private AudioSource activeSource;

    private PoseDetector.HandPose lastPose = PoseDetector.HandPose.None;
    private PoseDetector.HandPose currentPlayingPose = PoseDetector.HandPose.None;
    private float lastPlayTime = 0f;

    // Diccionario para búsqueda rápida
    private Dictionary<PoseDetector.HandPose, PoseAudio> audioDict;

    // Corutina para crossfade
    private Coroutine currentCrossfade;

    void Start()
    {
        // Crear dos AudioSources para crossfade
        audioSourceA = gameObject.AddComponent<AudioSource>();
        audioSourceB = gameObject.AddComponent<AudioSource>();

        // Configurar ambos
        ConfigureAudioSource(audioSourceA);
        ConfigureAudioSource(audioSourceB);

        activeSource = audioSourceA;

        // Construir diccionario
        audioDict = new Dictionary<PoseDetector.HandPose, PoseAudio>();
        foreach (var pa in poseAudios)
        {
            if (!audioDict.ContainsKey(pa.pose))
                audioDict.Add(pa.pose, pa);
        }

        if (showDebugLogs)
            Debug.Log($"🎵 AudioManager iniciado. {audioDict.Count} sonidos cargados.");
    }

    void ConfigureAudioSource(AudioSource source)
    {
        source.loop = false;
        source.playOnAwake = false;
        source.spatialBlend = 0f; // Sonido 2D
        source.volume = 0f; // Comienza en 0 para crossfade
    }

    void Update()
    {
        if (poseDetector == null) return;

        PoseDetector.HandPose currentPose = poseDetector.currentPose;

        // Reproducir cuando cambia la pose y no es None
        if (currentPose != lastPose && currentPose != PoseDetector.HandPose.None)
        {
            if (Time.time - lastPlayTime >= minDelayBetweenPoses)
            {
                PlaySoundSmooth(currentPose);
                lastPlayTime = Time.time;
            }
        }
        lastPose = currentPose;
    }

    void PlaySoundSmooth(PoseDetector.HandPose newPose)
    {
        if (!audioDict.TryGetValue(newPose, out PoseAudio poseAudio))
        {
            if (showDebugLogs)
                Debug.Log($"🔇 Sin sonido para: {newPose}");
            return;
        }

        if (poseAudio.clip == null)
        {
            Debug.LogWarning($"⚠️ Clip nulo para pose: {newPose}");
            return;
        }

        // Si es la misma pose que ya está sonando, no hacer nada
        if (currentPlayingPose == newPose && activeSource.isPlaying)
            return;

        // Detener cualquier crossfade en progreso
        if (currentCrossfade != null)
            StopCoroutine(currentCrossfade);

        // Iniciar nuevo crossfade
        currentCrossfade = StartCoroutine(CrossfadeToNewClip(poseAudio, newPose));
    }

    System.Collections.IEnumerator CrossfadeToNewClip(PoseAudio newAudio, PoseDetector.HandPose newPose)
    {
        float crossfadeTime = newAudio.crossfadeTime;

        // Obtener el source inactivo para reproducir el nuevo clip
        AudioSource oldSource = activeSource;
        AudioSource newSource = (activeSource == audioSourceA) ? audioSourceB : audioSourceA;

        // Configurar y reproducir el nuevo clip
        newSource.clip = newAudio.clip;
        newSource.volume = 0f;
        newSource.Play();

        // Hacer crossfade
        float elapsed = 0f;

        while (elapsed < crossfadeTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / crossfadeTime;

            // Curva suave (ease in-out)
            float smoothT = Mathf.SmoothStep(0, 1, t);

            // Bajar volumen del source viejo, subir el nuevo
            oldSource.volume = Mathf.Lerp(oldSource.volume, 0f, smoothT);
            newSource.volume = Mathf.Lerp(0f, newAudio.volume, smoothT);

            yield return null;
        }

        // Finalizar
        oldSource.Stop();
        oldSource.volume = 0f;
        newSource.volume = newAudio.volume;

        // El nuevo source es ahora el activo
        activeSource = newSource;
        currentPlayingPose = newPose;

        if (showDebugLogs)
            Debug.Log($"🎵 Crossfade completado: {lastPose} → {newPose}");

        currentCrossfade = null;
    }

    // Método para detener todo el audio inmediatamente
    public void StopAllAudio(float fadeTime = 0.1f)
    {
        if (currentCrossfade != null)
            StopCoroutine(currentCrossfade);

        StartCoroutine(FadeOutBoth(fadeTime));
    }

    System.Collections.IEnumerator FadeOutBoth(float fadeTime)
    {
        float elapsed = 0f;
        float startVolumeA = audioSourceA.volume;
        float startVolumeB = audioSourceB.volume;

        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeTime;

            audioSourceA.volume = Mathf.Lerp(startVolumeA, 0, t);
            audioSourceB.volume = Mathf.Lerp(startVolumeB, 0, t);

            yield return null;
        }

        audioSourceA.Stop();
        audioSourceB.Stop();
        audioSourceA.volume = 0;
        audioSourceB.volume = 0;
        currentPlayingPose = PoseDetector.HandPose.None;
    }
}