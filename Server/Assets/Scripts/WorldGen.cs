using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;


public class WorldGen : MonoBehaviour
{
    //Tile Assets
    public Tile grassLeft;
    public Tile grassMiddle;
    public Tile grassRight;
    public Tile dirtMiddleLeft;
    public Tile dirtMiddleRight;
    public Tile dirtCenter;
    public Tile dirtBottomLeft;
    public Tile dirtBottomRight;
    public Tile dirtBottomMiddle;
    public Tile error;

    //Map Parameters
    //Map Size and center
    public int width;
    public int height;
    private int mapCenterX;
    private int mapCenterY;

    //2d map array and Tilemap Refference
    int[,] map;
    public Tilemap tilemap;

    //Map seeding
    public string seed;
    public bool useRandomSeed;

    //Density of map Gen(effects smoothing)
    [Range(0, 100)]
    public int randomFillPercent;
    public int smoothingTarget;
    public int wallCountModifier;

    void Start()
    {
        mapCenterX = width / 2;
        mapCenterY = height / 2;
        generateWorld();
    }

    void generateWorld()
    {
        map = new int[width, height];
        RandomFillMap();
        for (int i = 0; i < smoothingTarget; i++) //SmoothMap exicutes based on the Smoothing target creating softer structures
        {
            SmoothMap();
        }
        DrawMap();
    }

    void RandomFillMap()
    {
        if (useRandomSeed == true) //Generate seeded hash. Either Random or preset so terain generation is repeatable
        {
            seed = DateTime.Now.ToString();
            Debug.Log(seed);
        }

        System.Random psuedoRandom = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++) //Populate map with noise
        {
            for (int y = 0; y < height; y++)
            {
                int radiusFromCenter = (int)(Math.Sqrt(Math.Pow(mapCenterX - x, 2) + Math.Pow(mapCenterY - y, 2))); //calculate radius from center of x,y grid

                if (radiusFromCenter <= mapCenterX && map[x, y] != 1 && map[x, y] != 2) //get radius and compare to map width
                {
                    map[x, y] = (psuedoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0; //RandomFillPercent controls the density of noise
                }
            }
        }
    }

    void SmoothMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == 1 || map[x, y] == 0)
                {
                    int adjacentTitlesCount = CountAdjacentTiles(x, y, 1, 1, 1, 1);

                    if (adjacentTitlesCount > wallCountModifier)
                    {
                        map[x, y] = 1;
                    }
                    else if (adjacentTitlesCount < wallCountModifier)
                    {
                        map[x, y] = 0;
                    }
                }
            }
        }
    }

    int CountAdjacentTiles(int mapX, int mapY, int startX, int endX, int startY, int endY) //returns count of nearby tiles to be used for smoothing and item spawning
    {
        int adjacentTitlesCount = 0;

        for (int nextToX = mapX - startX; nextToX <= mapX + endX; nextToX++)
        {
            for (int nextToY = mapY - startY; nextToY <= mapY + endY; nextToY++)
            {
                if (nextToX >= 0 && nextToX < width && nextToY >= 0 && nextToY < height) //check for out of bounds
                {
                    if (nextToX != mapX || nextToY != mapY) //Do not count position being measured
                    {
                        if (map[nextToX, nextToY] == 1 || map[nextToX, nextToY] == 0) // only smooth dirt Islands, not dungeons
                        {
                            adjacentTitlesCount += map[nextToX, nextToY];
                        }
                    }
                    else
                    {
                        adjacentTitlesCount++;
                    }
                }
            }
        }
        return adjacentTitlesCount;
    }

    void DrawMap()
    {
        Debug.Log("Running DrawMap method.");
        if (map != null)
        {
            for (int x = width - 1; x >= 0; x--)
            {
                for (int y = height - 1; y >= 0; y--)
                {
                    if (x == 0 || x == width - 1 || y == 0 || y == height - 1) //handle edge cases
                    {
                        //Do Nothing
                    }
                    else if (map[x, y] == 0) //Empty Space
                    {
                        Vector3Int p = new Vector3Int(x, y, 0);
                        tilemap.SetTile(p, null);
                    }
                    else if (map[x, y] == 1) // Place Dirt **********************************************************************************
                    {
                        if (map[x, y + 1] == 0 && map[x - 1, y] == 0) //Check for grass position and place left case tile
                        {
                            Vector3Int p = new Vector3Int(x, y, 0);
                            tilemap.SetTile(p, grassLeft);
                        }
                        else if (map[x, y + 1] == 0 && map[x + 1, y] == 0) //Check for grass position and place right case tile
                        {
                            Vector3Int p = new Vector3Int(x, y, 0);
                            tilemap.SetTile(p, grassRight);
                        }
                        else if (map[x, y + 1] == 0 && map[x - 1, y] == 1 && map[x + 1, y] == 1) //Check for grass position and place middle case tile
                        {
                            Vector3Int p = new Vector3Int(x, y, 0);
                            tilemap.SetTile(p, grassMiddle);
                        }
                        else if (map[x - 1, y] == 0 && map[x, y - 1] == 0 && map[x, y + 1] == 1 && map[x + 1, y] == 1 && map[x + 1, y + 1] == 1) // Find dirt bottom left
                        {
                            Vector3Int p = new Vector3Int(x, y, 0);
                            tilemap.SetTile(p, dirtBottomLeft);
                        }
                        else if (map[x - 1, y] == 1 && map[x + 1, y] == 1 && map[x, y + 1] == 1) // Find dirt bottom middle
                        {
                            Vector3Int p = new Vector3Int(x, y, 0);
                            tilemap.SetTile(p, dirtBottomMiddle);
                        }
                        else if (map[x - 1, y] == 1 && map[x, y - 1] == 0 && map[x, y + 1] == 1 && map[x + 1, y] == 0 && map[x - 1, y + 1] == 1) // Find dirt bottom right
                        {
                            Vector3Int p = new Vector3Int(x, y, 0);
                            tilemap.SetTile(p, dirtBottomRight);
                        }
                        else if (map[x - 1, y] == 0 && map[x, y - 1] == 1 && map[x, y + 1] == 1 && map[x + 1, y] == 1) // Find dirt Middle Left
                        {
                            Vector3Int p = new Vector3Int(x, y, 0);
                            tilemap.SetTile(p, dirtMiddleLeft);
                        }
                        else if (map[x - 1, y] == 1 && map[x, y - 1] == 1 && map[x, y + 1] == 1 && map[x + 1, y] == 0) // Find dirt Middle Right
                        {
                            Vector3Int p = new Vector3Int(x, y, 0);
                            tilemap.SetTile(p, dirtMiddleRight);
                        }
                        else
                        {
                            Vector3Int p = new Vector3Int(x, y, 0);
                            tilemap.SetTile(p, error);
                        }
                    }
                }
            }
        }
    }
}
