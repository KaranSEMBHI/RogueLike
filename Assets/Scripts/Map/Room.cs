using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int X, Y;
    public int Width, Height;

    public Room(int x, int y, int width, int height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public Vector2Int Center()
        => new Vector2Int(X + Width / 2, Y + Height / 2);
    public Bounds GetBounds()
        => new Bounds(new Vector3(X, Y, 0), new Vector3(Width, Height, 0));
    public BoundsInt GetBoundsInt()
        => new BoundsInt(new Vector3Int(X, Y, 0), new Vector3Int(Width, Height, 0));

    public bool Overlaps(List<Room> otherRooms)
    {
        foreach (var room in otherRooms)
        {
            if (GetBounds().Intersects(room.GetBounds()))
            {
                return true;
            }
        }
        return false;
    }
}