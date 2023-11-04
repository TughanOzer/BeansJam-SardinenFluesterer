using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostManager : MonoBehaviour
{
    #region Fields and Properties

    public static GhostManager Instance { get; private set; }

    [SerializeField] private ObjectsFoundVisuals _ghostObjectUI;

    [SerializeField] private List<Ghost> _ghostPrefabs;
    [SerializeField] private float _startSpawnDelay = 10;
    [SerializeField] private float _delayReductionStep = 0.1f;
    [SerializeField] private float _minimumDelay = 3f;

    private float _spawnTimer;
    private float _currentSpawnDelay;

    #endregion

    #region Methods

    private void Awake()
    {
        if (Instance == null)
            Instance = this;     
        else
            Destroy(gameObject);  
    }

    private void Start()
    {
        _spawnTimer = _startSpawnDelay;
        _currentSpawnDelay = _startSpawnDelay;
    }

    private void Update()
    {
        _spawnTimer -= Time.deltaTime;

        if (_spawnTimer <= 0)
            SpawnGhost();
    }

    private void SpawnGhost()
    {
        var ghostSpawnCount = _ghostObjectUI.GhostImages.Count;

        if (_currentSpawnDelay > _minimumDelay)
            _currentSpawnDelay -= _delayReductionStep * ghostSpawnCount;
        else
            _currentSpawnDelay = _minimumDelay;

        _spawnTimer = _currentSpawnDelay;
     
        int ghostIndex = UnityEngine.Random.Range(0, ghostSpawnCount);

        float xSpawnPosition = UnityEngine.Random.Range(10, 50);
        float ySpawnPosition = UnityEngine.Random.Range(5, 25);

        float xInverted = UnityEngine.Random.Range(0, 1f);
        float yInverted = UnityEngine.Random.Range(0, 1f);

        xSpawnPosition = xInverted < 0.5f ? xSpawnPosition : -xSpawnPosition;
        ySpawnPosition = yInverted < 0.5f ? ySpawnPosition : -ySpawnPosition;

        var ghost = Instantiate(_ghostPrefabs[0], new Vector2(xSpawnPosition, ySpawnPosition), Quaternion.identity);
        ghost.SetUp(ghostIndex);
    }

    #endregion
}
