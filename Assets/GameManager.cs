using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private List<GameObject> _levels;

    private GameObject _currentLevel;
    private int _currentLevelNum = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

        if (_levels == null || _levels.Count == 0)
        {
            Debug.LogError("No levels assigned in the GameManager.");
            return;
        }

        _currentLevel = Instantiate(_levels[_currentLevelNum], Vector3.zero, Quaternion.identity);
    }

    public void LoadNextLevel()
    {
        Destroy(_currentLevel);
        _currentLevelNum++;
        if (_currentLevelNum == 8)
        {
            CastleManager.Instance.SetStage2();
        }
        else if (_currentLevelNum == 14)
        {
            CastleManager.Instance.SetStage3();
        }

        _currentLevel = Instantiate(_levels[_currentLevelNum], Vector3.zero, Quaternion.identity);
        if (_currentLevelNum == _levels.Count - 1)
        {
            MusicManager.Instance.PlayVictoryMusic();
        }
    }

    public void Restart()
    {
        Destroy(_currentLevel);
        _currentLevelNum = 0;
        _currentLevel = Instantiate(_levels[_currentLevelNum], Vector3.zero, Quaternion.identity);
        CastleManager.Instance.SetStage1();
    }
}
