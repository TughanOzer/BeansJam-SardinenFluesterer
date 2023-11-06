using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds multiple viable paths for the girlfriend takes, one of which is chosen randomly to follow.
/// Needs a trigger collider and girlfriend needs to get her path from this on collision
/// Last waypoints in a route must end within the collider of a station!
/// </summary>
public class GirlfriendStation : MonoBehaviour
{
    #region Fields and Properties

    private List<List<Transform>> ViableRoutes = new();
    [SerializeField] private List<Transform> route1 = new();
    [SerializeField] private List<Transform> route2 = new();
    [SerializeField] private List<Transform> route3 = new();
    [SerializeField] private List<Transform> route4 = new();
    [SerializeField] private List<Transform> route5 = new();
    [field: SerializeField] public float StationStayDuration { get; private set; } = 30f;

    #endregion

    #region Methods

    private void Start()
    {
        if (route1.Count > 0)
            ViableRoutes.Add(route1);
        if (route2.Count > 0)
            ViableRoutes.Add(route2);
        if (route3.Count > 0)
            ViableRoutes.Add(route3);
        if (route4.Count > 0)
            ViableRoutes.Add(route4);
        if (route5.Count > 0)
            ViableRoutes.Add(route5);
        
    }

    public List<Transform> GetSortedRoute(Transform girlfriendTransform)
    {
        //List<Transform> chosenPath = new();

        int randomIndex = UnityEngine.Random.Range(0, ViableRoutes.Count);
        var unsortedRoute = ViableRoutes[randomIndex];

        #region Removed Sort
        /*
        foreach (var waypoint in unsortedRoute)
        {
            var distance = Vector2.Distance(waypoint.position, girlfriendTransform.position);

            if (chosenPath.Count == 0)
            {
                chosenPath.Add(waypoint);
            }
            else
            {
                var index = 0;
                foreach (var point in chosenPath)
                {
                    var oldDistance = Vector2.Distance(point.position, girlfriendTransform.position);

                    if (index == chosenPath.Count - 1) //furthest element
                    {
                        chosenPath.Add(waypoint);
                        break;
                    }
                    else if (distance < oldDistance) //there is a farther element already
                    {
                        chosenPath.Insert(index, waypoint);
                        break;
                    }
                    else
                    {
                        index++;
                    }                  
                }
            }      
        }
        */

        #endregion

        return unsortedRoute;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<GirlfriendControllerEndo>())
            collision.GetComponent<AudioSource>().Play();
    }

    #endregion
}
