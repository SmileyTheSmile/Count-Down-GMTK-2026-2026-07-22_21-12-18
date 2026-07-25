using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] private GameObject _playerPrefab;

    private GameObject _player;

    private void Awake()
    {
        GameObject[] existingPlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in existingPlayers)
        {
            Destroy(p);
        }

        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        if (_playerPrefab != null)
        {
            BoxCollider2D playerCollider = _playerPrefab.GetComponent<BoxCollider2D>();
            Vector2 spawnPosition = new Vector2(transform.position.x, transform.position.y + playerCollider.size.y);
            _player = Instantiate(_playerPrefab, transform.position, Quaternion.identity);
            Player playerComponent = _player.GetComponent<Player>();
            playerComponent.SpawnPoint = this;
        }
        else
        {
            Debug.LogError("Player prefab is not assigned in the PlayerSpawn script.");
        }
    }

    public void Respawn()
    {
        _player.transform.position = transform.position;
    }
}
