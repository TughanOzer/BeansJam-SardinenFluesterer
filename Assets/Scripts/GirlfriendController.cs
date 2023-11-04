using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GirlfriendController : MonoBehaviour
{
    [SerializeField] float speed;
    
    //#region waypointsystem
    //[SerializeField] GameObject[] waypoints;
    //int nextWaypoint = 0;
    
    //float waypointRadius = 1;

    //void Update()
    //{
    //    if (Vector2.Distance(waypoints[nextWaypoint].transform.position, transform.position) < waypointRadius)
    //    {
    //        nextWaypoint++;
    //        if(nextWaypoint >= waypoints.Length) nextWaypoint = 0;
    //    }
    //    transform.position = Vector2.MoveTowards(transform.position, waypoints[nextWaypoint].transform.position, Time.deltaTime *speed);
    //}
    //#endregion


    #region pathfinding with A* //https://www.youtube.com/watch?v=1bO1FdEThnU


    public int2 gridSize;
    [SerializeField] Tilemap grid;
    int2 currentPosition;
    public Transform nextTarget;
    [SerializeField]  List<int2> unalkablePathNodes = new List<int2>();
    const int moveStraightCost = 10;
    const int moveDiagonalCost = 10;

    private void Start()
    {
        gridSize = new int2(grid.size.x,grid.size.y);
        NextTask();
    }
   
    public void NextTask() {
        currentPosition = new int2((int)transform.position.x, (int)transform.position.y);
        int2 targetPosition = new((int)nextTarget.position.x, (int)nextTarget.position.y);
        Debug.Log($"currentPosition: {currentPosition}, targetPosition: {targetPosition}");
        FindPath(currentPosition,targetPosition);
    }
    public void FindPath(int2 startPosition, int2 endPosition)
    {
        NativeArray<PathNode> pathNodeArray = new NativeArray<PathNode>(gridSize.x * gridSize.y, Allocator.Temp);
        for (int x = 0; x < gridSize.x; x++) {
            for (int y = 0; y < gridSize.y; y++)
            {
                PathNode pathNode = new PathNode();
                pathNode.x = x;
                pathNode.y = y;
                pathNode.index = CalculateIndex(x, y, gridSize.x);
                pathNode.gCost = int.MaxValue;
                pathNode.hCost = CalculateDistanceCost(new int2(x, y), endPosition);
                pathNode.CalculateFCost();

                pathNode.isWalkable = true;
                pathNode.cameFromNodeIndex = -1;

                pathNodeArray[pathNode.index] = pathNode;

            }
        }
        {
            foreach (var obstacle in unalkablePathNodes)
            {
                PathNode pathNode; 
                for (int i = 0; i < pathNodeArray.Length; i++)
                {
                    if (obstacle.x == pathNodeArray[i].x && obstacle.y == pathNodeArray[i].y) {
                        
                        pathNode = pathNodeArray[CalculateIndex(obstacle.x, obstacle.y, gridSize.x)];
                        pathNode.SetIsWalkable(false);
                        pathNodeArray[CalculateIndex(obstacle.x, obstacle.y, gridSize.x)] = pathNode;
                    }
                }

            }
                
        }

        NativeArray<int2> neighbourOffsetArray = new NativeArray<int2>(new int2[] {
        new int2(-1,0), // left
        new int2(+1, 0), //right
        new int2(0, +1), // up
        new int2(0, -1), // down
        new int2(-1, +1), // left up
        new int2(+1, +1), // right up
        new int2(-1, -1), //left down
        new int2(+1, -1), // right down
        }, Allocator.Temp);

        int endNodeIndex = CalculateIndex(endPosition.x, endPosition.y, gridSize.x);

        PathNode startNode = pathNodeArray[CalculateIndex(startPosition.x,startPosition.y,gridSize.x)];
        startNode.gCost = 0;
        startNode.CalculateFCost();
        pathNodeArray[startNode.index] = startNode;

        NativeList<int> openList = new NativeList<int>(Allocator.Temp);
        NativeList<int> closedList = new NativeList<int>(Allocator.Temp);

        openList.Add(startNode.index);
        while (openList.Length > 0) {
            int currentNodeIndex = GetLowestCostFNodeIndex(openList, pathNodeArray);
            PathNode currentNode = pathNodeArray[currentNodeIndex];
            if (currentNodeIndex == endNodeIndex)
            {
                // reached the de
                break;
            }
            for(int i = 0; i<openList.Length; i++)
            {
                if (openList[i] == currentNodeIndex)
                {
                    openList.RemoveAtSwapBack(i); break;
                }
                
            }
            closedList.Add(currentNodeIndex);

            for (int i = 0; i < neighbourOffsetArray.Length; i++)
            {
                int2 neighbourOffset = neighbourOffsetArray[i];
                int2 neighbourPosition = new int2(currentNode.x + neighbourOffset.x, currentNode.y + neighbourOffset.y);

                if (!IsPositionInsideGrid(neighbourPosition, gridSize))
                {
                    // no valid position
                    continue;
                }
                int neighbourNodeIndex = CalculateIndex(neighbourPosition.x, neighbourPosition.y, gridSize.x);
                if (closedList.Contains(neighbourNodeIndex))
                {
                    // already checked
                    continue;
                }
                PathNode neighbourNode = pathNodeArray[neighbourNodeIndex];
                if (!neighbourNode.isWalkable) { continue; }

                int2 currentNodePosition = new int2(currentNode.x, currentNode.y);
                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNodePosition, neighbourPosition);
                if (tentativeGCost < neighbourNode.gCost) {
                    neighbourNode.cameFromNodeIndex = currentNodeIndex;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.CalculateFCost();
                    pathNodeArray[neighbourNodeIndex] = neighbourNode;

                    if(!openList.Contains(neighbourNode.index))
                    {
                        openList.Add(neighbourNode.index);
                    }
                }

            }
        }
        PathNode endNode = pathNodeArray[endNodeIndex];
        if (endNode.cameFromNodeIndex == -1) {
            Debug.Log("Didn't find a path");//didn't find a path
        }
        else
        {
            NativeList<int2> path = CalculatePath(pathNodeArray, endNode);
            foreach(int2 pathPosition in path)
            {
                Debug.Log(pathPosition);
                Vector2 nextPosition = new Vector2(pathPosition.x, pathPosition.y);
                transform.position = Vector2.MoveTowards(transform.position, nextPosition, Time.deltaTime * speed);
            }
            path.Dispose();
        }

        pathNodeArray.Dispose();
        neighbourOffsetArray.Dispose();
         openList.Dispose();
         closedList.Dispose();
    }

    private NativeList<int2> CalculatePath(NativeArray<PathNode> pathNodeArray,PathNode endNode)
    {
        if(endNode.cameFromNodeIndex == -1) {
            
            // couldn't find a path
            return new NativeList<int2>(Allocator.Temp);
        } else{
            NativeList<int2> path = new NativeList<int2>(Allocator.Temp);
            path.Add(new int2(endNode.x,endNode.y));
            PathNode currentNode = endNode;
            while(currentNode.cameFromNodeIndex!=-1)
            {
                PathNode cameFromNode = pathNodeArray[currentNode.cameFromNodeIndex];
                path.Add(new int2(cameFromNode.x, cameFromNode.y));
                currentNode = cameFromNode;
            }
            return path;
        }
    }
    bool IsPositionInsideGrid(int2 gridPosition, int2 gridSize) {
        return 
            gridPosition.x >= 0 && gridPosition.y >= 0 &&
            gridPosition.x < gridSize.x && gridPosition.y < gridSize.y;           ;
    }
    int CalculateIndex(int x, int y, int gridWith) {
        return x + y * gridWith; 
    }
    int CalculateDistanceCost(int2 aPosition, int2 bPosition) { 
        int xDistance = math.abs(aPosition.x-bPosition.x);
        int yDistance = math.abs(aPosition.y-bPosition.y);
        int remaining = math.abs(xDistance-yDistance);
        return moveDiagonalCost * math.min(xDistance, yDistance) + moveStraightCost*remaining;
    }

    int GetLowestCostFNodeIndex(NativeList<int> openList, NativeArray<PathNode> pathNodeArray)
    {
        PathNode lowestCostPathNode = pathNodeArray[openList[0]];
        for(int i= 1; i<openList.Length;i++)
        {
            PathNode testPathNode = pathNodeArray[openList[i]];
            if (testPathNode.fCost < lowestCostPathNode.fCost)
            {
                lowestCostPathNode = testPathNode;
            }
        }
        return lowestCostPathNode.index;
    }
    public struct PathNode
    {
        public int x;
        public int y;

        public int index;

        public int gCost;
        public int hCost;
        public int fCost;

        public bool isWalkable;
        public int cameFromNodeIndex;

        public void CalculateFCost()
        {
            fCost = gCost + hCost;
        }
        public void SetIsWalkable(bool isWalkable)
        {
            this.isWalkable = isWalkable;
        }
    }
    #endregion
}
