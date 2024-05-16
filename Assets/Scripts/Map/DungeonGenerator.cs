using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    private int width, height;
    private int maxRoomSize, minRoomSize;
    private int maxRooms;
    public int maxEnemies; // Nieuw attribuut voor het maximum aantal vijanden
    List<Room> rooms = new List<Room>();

    public void SetSize(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    public void SetRoomSize(int min, int max)
    {
        minRoomSize = min;
        maxRoomSize = max;
    }

    public void SetMaxRooms(int max)
    {
        maxRooms = max;
    }

    public void SetMaxEnemies(int max)
    {
        maxEnemies = max;
    }

    // Functie om vijanden in een kamer te plaatsen
    private void PlaceEnemies(Room room, int maxEnemies)
    {
        // Het aantal vijanden dat we willen
        int num = Random.Range(0, maxEnemies + 1);

        for (int counter = 0; counter < num; counter++)
        {
            // De grenzen van de kamer zijn muren, dus voeg en trek 1 af
            int x = Random.Range(room.X + 1, room.X + room.Width - 1);
            int y = Random.Range(room.Y + 1, room.Y + room.Height - 1);

            // Creëer verschillende vijanden
            if (Random.value < 0.5f)
            {
                GameManager.Get.CreateActor("Tiger", new Vector2(x, y));
            }
            else
            {
                GameManager.Get.CreateActor("Bear", new Vector2(x, y));
            }
        }
    }

    public void Generate()
    {
        rooms.Clear();

        for (int roomNum = 0; roomNum < maxRooms; roomNum++)
        {
            int roomWidth = Random.Range(minRoomSize, maxRoomSize);
            int roomHeight = Random.Range(minRoomSize, maxRoomSize);

            int roomX = Random.Range(0, width - roomWidth - 1);
            int roomY = Random.Range(0, height - roomHeight - 1);

            var room = new Room(roomX, roomY, roomWidth, roomHeight);

            // Als de kamer overlapt met een andere kamer, verwijder deze dan
            if (room.Overlaps(rooms))
            {
                continue;
            }

            // Voeg tegels toe om de kamer zichtbaar te maken op de tilemap
            for (int x = roomX; x < roomX + roomWidth; x++)
            {
                for (int y = roomY; y < roomY + roomHeight; y++)
                {
                    if (x == roomX || x == roomX + roomWidth - 1 || y == roomY || y == roomY + roomHeight - 1)
                    {
                        if (!TrySetWallTile(new Vector3Int(x, y)))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        SetFloorTile(new Vector3Int(x, y, 0));
                    }
                }
            }

            // Creëer een gang tussen kamers
            if (rooms.Count != 0)
            {
                TunnelBetween(rooms[rooms.Count - 1], room);
            }

            rooms.Add(room);

            // Plaats vijanden in de kamer
            PlaceEnemies(room, maxEnemies);
        }

        var player = GameManager.Get.CreateActor("Player", rooms[0].Center());
    }

    private bool TrySetWallTile(Vector3Int pos)
    {
        // Als dit een vloertegel is, mag het geen muur zijn
        if (MapManager.Get.FloorMap.GetTile(pos))
        {
            return false;
        }
        else
        {
            // Zo niet, dan kan het een muur zijn
            MapManager.Get.ObstacleMap.SetTile(pos, MapManager.Get.WallTile);
            return true;
        }
    }

    private void SetFloorTile(Vector3Int pos)
    {
        // Deze tegel moet begaanbaar zijn, dus verwijder elk obstakel
        if (MapManager.Get.ObstacleMap.GetTile(pos))
        {
            MapManager.Get.ObstacleMap.SetTile(pos, null);
        }
        // Stel de vloertegel in
        MapManager.Get.FloorMap.SetTile(pos, MapManager.Get.FloorTile);
    }

    private void TunnelBetween(Room oldRoom, Room newRoom)
    {
        Vector2Int oldRoomCenter = oldRoom.Center();
        Vector2Int newRoomCenter = newRoom.Center();
        Vector2Int tunnelCorner;

        if (Random.value < 0.5f)
        {
            // Beweeg horizontaal, dan verticaal
            tunnelCorner = new Vector2Int(newRoomCenter.x, oldRoomCenter.y);
        }
        else
        {
            // Beweeg verticaal, dan horizontaal
            tunnelCorner = new Vector2Int(oldRoomCenter.x, newRoomCenter.y);
        }

        // Genereer de coördinaten voor deze gang
        List<Vector2Int> tunnelCoords = new List<Vector2Int>();
        BresenhamLine.Compute(oldRoomCenter, tunnelCorner, tunnelCoords);
        BresenhamLine.Compute(tunnelCorner, newRoomCenter, tunnelCoords);

        // Stel de tegels voor deze gang in
        for (int i = 0; i < tunnelCoords.Count; i++)
        {
            SetFloorTile(new Vector3Int(tunnelCoords[i].x, tunnelCoords[i].y));

            for (int x = tunnelCoords[i].x - 1; x <= tunnelCoords[i].x + 1; x++)
            {
                for (int y = tunnelCoords[i].y - 1; y <= tunnelCoords[i].y + 1; y++)
                {
                    if (!TrySetWallTile(new Vector3Int(x, y, 0)))
                    {
                        continue;
                    }
                }
            }
        }
    }
}
