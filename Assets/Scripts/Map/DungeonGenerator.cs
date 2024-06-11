using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    private int width, height;
    private int maxRoomSize, minRoomSize;
    private int maxRooms;
    private int maxEnemies;
    private int maxItems;
    private int currentFloor; // Nieuwe variabele voor de huidige verdieping

    List<Room> rooms = new List<Room>();

    // Lijst van vijanden in volgorde van sterkte
    private readonly List<string> enemyNames = new List<string>
    {
        "Ant",
        "Bomboclat",
        "Ding",
        "Dragpn",
        "Hond",
        "Ja",
        "Tijger",
        "WOlf"
    };

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

    public void SetMaxItems(int max)
    {
        maxItems = max;
    }

    public void SetCurrentFloor(int floor)
    {
        this.currentFloor = floor;
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

            // Als de kamer overlapt met een andere kamer, negeer het
            if (room.Overlaps(rooms))
            {
                continue;
            }

            // Voeg tegels toe om de kamer zichtbaar te maken op de tilemap
            for (int x = roomX; x < roomX + roomWidth; x++)
            {
                for (int y = roomY; y < roomY + roomHeight; y++)
                {
                    if (x == roomX
                        || x == roomX + roomWidth - 1
                        || y == roomY
                        || y == roomY + roomHeight - 1)
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

            // Maak een gang tussen kamers
            if (rooms.Count != 0)
            {
                TunnelBetween(rooms[rooms.Count - 1], room);
            }

            PlaceEnemies(room, maxEnemies);
            PlaceItems(room, maxItems);
            rooms.Add(room);
        }

        // Plaats de speler en ladders op basis van de huidige verdieping
        if (rooms.Count > 0)
        {
            Vector3Int startPos = new Vector3Int(rooms[0].Center().x, rooms[0].Center().y, 0);
            Vector3Int endPos = new Vector3Int(rooms[rooms.Count - 1].Center().x, rooms[rooms.Count - 1].Center().y, 0);

            // Plaats een ladder naar beneden in de laatste kamer
            PlaceLadderDown(endPos);

            // Plaats de speler in de eerste kamer
            var player = GameObject.Find("Player");
            if (player != null)
            {
                player.transform.position = new Vector3(startPos.x, startPos.y, 0);
            }
            else
            {
                GameManager.Get.CreateGameObject("Player", new Vector2(startPos.x, startPos.y));
            }

            // Als de huidige verdieping groter is dan 0, plaats een ladder naar boven in de eerste kamer
            if (currentFloor > 0)
            {
                PlaceLadderUp(startPos);
            }
        }
    }

    private bool TrySetWallTile(Vector3Int pos)
    {
        // Als dit een vloer is, mag het geen muur zijn
        if (MapManager.Get.FloorMap.GetTile(pos))
        {
            return false;
        }
        else
        {
            // Anders kan het een muur zijn
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
        // Zet de vloertegel
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

        // Genereer de coördinaten voor deze tunnel
        List<Vector2Int> tunnelCoords = new List<Vector2Int>();
        BresenhamLine.Compute(oldRoomCenter, tunnelCorner, tunnelCoords);
        BresenhamLine.Compute(tunnelCorner, newRoomCenter, tunnelCoords);

        // Zet de tegels voor deze tunnel
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

    private void PlaceEnemies(Room room, int maxEnemies)
    {
        // Het aantal vijanden dat we willen
        int num = Random.Range(0, maxEnemies + 1);

        for (int counter = 0; counter < num; counter++)
        {
            // De grenzen van de kamer zijn muren, dus tel er 1 bij op en trek er 1 vanaf
            int x = Random.Range(room.X + 1, room.X + room.Width - 1);
            int y = Random.Range(room.Y + 1, room.Y + room.Height - 1);

            // De kans op sterkere vijanden neemt toe met de verdieping
            float difficultyModifier = Mathf.Clamp01(currentFloor / 10f);
            int enemyIndex = Mathf.FloorToInt(difficultyModifier * (enemyNames.Count - 1));

            // Kies een willekeurige vijand uit de lijst op basis van de huidige verdieping
            string enemyName = enemyNames[Random.Range(0, enemyIndex + 1)];

            GameManager.Get.CreateGameObject(enemyName, new Vector2(x, y));
        }
    }

    private void PlaceItems(Room room, int maxItems)
    {
        // Het aantal items dat we willen
        int num = Random.Range(0, maxItems + 1);

        for (int counter = 0; counter < num; counter++)
        {
            // De grenzen van de kamer zijn muren, dus tel er 1 bij op en trek er 1 vanaf
            int x = Random.Range(room.X + 1, room.X + room.Width - 1);
            int y = Random.Range(room.Y + 1, room.Y + room.Height - 1);

            // Maak verschillende items
            float value = Random.value;
            if (value > 0.8f)
            {
                GameManager.Get.CreateGameObject("Scroll", new Vector2(x, y));
            }
            else if (value > 0.5f)
            {
                GameManager.Get.CreateGameObject("Fireball", new Vector2(x, y));
            }
            else
            {
                GameManager.Get.CreateGameObject("HealthPotion", new Vector2(x, y));
            }
        }
    }

    private void PlaceLadderDown(Vector3Int position)
    {
        // Plaats een ladder naar beneden op de gegeven positie
        GameManager.Get.CreateGameObject("PijlOmlaag", new Vector2(position.x, position.y));
    }

    private void PlaceLadderUp(Vector3Int position)
    {
        // Plaats een ladder naar boven op de gegeven positie
        GameManager.Get.CreateGameObject("PijlOmhoog", new Vector2(position.x, position.y));
    }
}
