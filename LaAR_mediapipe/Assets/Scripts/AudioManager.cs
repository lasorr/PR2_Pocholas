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
    }

    public PoseAudio[] poseAudios;
    public AudioSource audioSource;
    public PoseDetector poseDetector;

    private PoseDetector.HandPose lastPose = PoseDetector.HandPose.None;
    private float lastPlayTime = 0f;
    public float minDelay = 0.3f;

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (poseDetector == null) return;

        PoseDetector.HandPose currentPose = poseDetector.currentPose;

        if (currentPose != lastPose && currentPose != PoseDetector.HandPose.None)
        {
            if (Time.time - lastPlayTime >= minDelay)
            {
                PlaySoundForPose(currentPose);
                lastPlayTime = Time.time;
            }
        }
        lastPose = currentPose;
    }

    void PlaySoundForPose(PoseDetector.HandPose pose)
    {
        foreach (var pa in poseAudios)
        {
            if (pa.pose == pose && pa.clip != null)
            {
                audioSource.PlayOneShot(pa.clip, pa.volume);
                Debug.Log("Sonido: " + pose);
                break;
            }
        }
    }
}