using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SoundManager.Instance.PlaySound("nextLevel", transform);
            GameManager.Instance.LoadNextLevel();
            Destroy(other.gameObject);
        }
    }
}
