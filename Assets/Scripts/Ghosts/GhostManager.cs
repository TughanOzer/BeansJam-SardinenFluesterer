using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostManager : MonoBehaviour
{
    #region Fields and Properties

    public static GhostManager Instance { get; private set; }

    [SerializeField] private List<Ghost> _ghostPrefabs;

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
        var ghost = Instantiate(_ghostPrefabs[0], transform.position, Quaternion.identity);
        ghost.SetUp(0);
    }


    #endregion
}
