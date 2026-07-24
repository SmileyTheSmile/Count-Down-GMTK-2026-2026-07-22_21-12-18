using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [System.Serializable]
    public class SoundEffect
    {
        public string name;
        public AudioClip clip;
        public float minPitch = 1f;
        public float maxPitch = 1f;
        public float volume = 1f;
    }

    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioSource audioSourcePrefab;
    [SerializeField] private List<SoundEffect> _sounds = new List<SoundEffect>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    public void PlaySound(string sound, Transform transform)
    {
        SoundEffect sfx = GetClipByName(sound);
        if (sfx == null)
        {
            Debug.LogWarning($"Sound '{sound}' not found!");
            return;
        }

        AudioSource audioSource = Instantiate(audioSourcePrefab, transform.position, Quaternion.identity);

        audioSource.clip = sfx.clip;
        audioSource.volume = sfx.volume;

        if (sfx.minPitch != sfx.maxPitch)
            audioSource.pitch = Random.Range(sfx.minPitch, sfx.maxPitch);

        audioSource.Play();

        float clipLength = audioSource.clip.length / audioSource.pitch;

        Destroy(audioSource.gameObject, clipLength);
    }

    private SoundEffect GetClipByName(string name)
    {
        return _sounds.Find(s => s.name == name);
    }
}
