using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Girlfriend Route")]
public class GirlfriendRoute : ScriptableObject
{
    [field: SerializeField] public List<Vector2> Waypoints = new();
}
