using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    North, East, South, West
}

public enum DirectionChange
{
    None, TurnRight, TurnLeft, TurnAround
}

public static class DirectionExtensions
{
    static Quaternion[] rotations = { Quaternion.identity, Quaternion.Euler(0f, 90f, 0f), Quaternion.Euler(0f, 180f, 0f), Quaternion.Euler(0f, 270f, 0f) };

    public static Quaternion GetRotation(this Direction direction)
    {
        return rotations[(int)direction];
    }

    public static DirectionChange GetDirectionChangeTo(this Direction current, Direction next)
    {
        if (current == next)
        {
            return DirectionChange.None;
        }
        else if (current + 1 == next || current - 3 == next)
        {
            return DirectionChange.TurnRight;
        }
        else if (current - 1 == next || current + 3 == next)
        {
            return DirectionChange.TurnLeft;
        }
        return DirectionChange.TurnAround;
    }

    public static float GetAngle(this Direction direction)
    {
        return (float)direction * 90f;
    }

    static Vector3[] halfVectors = {
        Vector3.forward * 0.5f,
        Vector3.right * 0.5f,
        Vector3.back * 0.5f,
        Vector3.left * 0.5f
    };
    public static Vector3 GetHalfVector(this Direction direction)
    {
        return halfVectors[(int)direction];
    }
}

public class GameTile : MonoBehaviour
{
    [SerializeField] 
    Transform arrow = default;
    private GameTile north, south, east, west, nextOnPath;
    public GameTile NextTileOnPath => nextOnPath;
    private int distance;
    public bool IsAlternative { get; set; }
    public bool HasPath => distance != int.MaxValue;
    public Vector3 ExitPoint { get; private set; }
    private GameTileContent tileContent;
    public GameTileContent TileContent
    {
        get => tileContent;
        set
        {
            Debug.Assert(value != null, "Null assigned to content!");
            if (tileContent != null)
            {
                tileContent.Recycle();
            }
            tileContent = value;
            tileContent.transform.localPosition = transform.localPosition;
        }
    }

    public Direction directionPath { get; private set; }
    public static void MakeEastWestRelationship(GameTile east, GameTile west)
    {
        Debug.Assert(east.west == null && west.east == null, "Redefined Neighbour");
        east.west = west;
        west.east = east;
    }
    public static void MakeNorthSouthRelationship(GameTile north, GameTile south)
    {
        Debug.Assert(north.south == null &&     south.north == null, "Redefined Neighbour");
        north.south = south;
        south.north = north;
    }

    public void ClearPath()
    {
        distance = int.MaxValue;
        nextOnPath = null;
    }

    public void BecomeDestination()
    {
        distance = 0;
        nextOnPath = null;
        ExitPoint = transform.localPosition;
    }

    public GameTile GrowPathTo(GameTile tile, Direction direction)
    {
        if (tile == null || tile.HasPath  || !HasPath) 
        { 
            return null; 
        }
        tile.nextOnPath = this;
        tile.distance = distance + 1;
        tile.directionPath = direction;
        tile.ExitPoint = tile.transform.localPosition + direction.GetHalfVector();
        return tile.TileContent.BlocksPath ?  null :  tile;
       
    }

    public GameTile GrowPathNorth() => GrowPathTo(north, Direction.South);
    public GameTile GrowPathSouth() => GrowPathTo(south, Direction.North);
    public GameTile GrowPathEast() => GrowPathTo(east, Direction.West);
    public GameTile GrowPathWest() => GrowPathTo(west, Direction.East);

    static Quaternion
        northRotation = Quaternion.Euler(90f, 0f, 0f),
        eastRotation = Quaternion.Euler(90f, 90f, 0f),
        southRotation = Quaternion.Euler(90f, 180f, 0f),
        westRotation = Quaternion.Euler(90f, 270f, 0f);

    public void ShowPath()
    {
        if (distance == 0)
        {
            arrow.gameObject.SetActive(false);
            return;
        }
        arrow.gameObject.SetActive(true);
        arrow.localRotation =
            nextOnPath == north ? northRotation :
            nextOnPath == south ? southRotation :
            nextOnPath == east ? eastRotation :
            westRotation;
    }

    public void HidePath()
    {
        arrow.gameObject.SetActive(false);
    }
}       
