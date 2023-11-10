using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GirlfriendControllerEndo : MonoBehaviour
{
    #region Fields and Properties

    private bool _isOnStation;
    private List<Vector2> _currentRoute = new();
    private float _currentStationTimer = 0;
    private int _currentTargetIndex = 0;

    [SerializeField] private float _speed = 0.003f;
    [SerializeField] private GirlfriendStation _startStation;

    #endregion

    #region Methods

    private void Start()
    {
        _currentRoute.Add(_startStation.transform.position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out GirlfriendStation station))
        {
            _isOnStation = true;
            _currentStationTimer = station.StationStayDuration;
            _currentTargetIndex = 0;

            var newRoute = station.GetSortedRoute(transform);
            _currentRoute.Clear();

            foreach (var waypoint in newRoute)
                _currentRoute.Add(waypoint.position);
        }
    }

    private void FixedUpdate()
    {
        if (_isOnStation)
            _currentStationTimer -= Time.fixedDeltaTime;

        if (_currentStationTimer <= 0)
            _isOnStation = false;

        if (!_isOnStation && _currentTargetIndex < _currentRoute.Count)
        {
            var target = _currentRoute[_currentTargetIndex];
            var distance = Vector2.Distance(target, (Vector2)transform.position);
            var direction = target - (Vector2)transform.position;

            if (distance > 0.1f)
                transform.Translate(direction.normalized * _speed);
            else
                _currentTargetIndex++;
        }
    }

    #endregion
}
