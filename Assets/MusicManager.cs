using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip _victoryMusic;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    public void PlayVictoryMusic()
    {
        _audioSource.Stop();
        _audioSource.clip = _victoryMusic;
        _audioSource.Play();
    }
}
