﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;

public class MapGenV2 : MonoBehaviour
{
    //Tile Assets
    public Tile grassLeft;
    public Tile grassMiddle;
    public Tile grassRight;
    public Tile grassSlopeLeft;
    public Tile grassSlopeRight;
    public Tile dirtMiddleLeft;
    public Tile dirtMiddleRight;
    public Tile dirtCenter;
    public Tile dirtBottomLeft;
    public Tile dirtBottomRight;
    public Tile dirtBottomMiddle;
    public Tile dirtBottomLeftCorner;
    public Tile dirtBottomRightCorner;
    public Tile dirtTopLeftCorner;
    public Tile dirtTopRightCorner;
    public Tile error;
    public Tile testDungeon;

    // Player data
    // public GameObject player;
    //private GameObject playerFound;


    //World Seed perameters
    private System.Random psuedoRandom;

    //Map Parameters
    //Map Size and center
    public int width;
    public int height;
    private int mapCenterX;
    private int mapCenterY;
    //platform Density
    public int islandLength;
    public int islandHeight;
    public int islandSpaceingHorizontal;
    public int islandSpaceingVertical;
    public int groundGap; //used to offset platforms spawning into ground
    public int blend; //used to blur islands together and remove the square seperation between them. Makes them look more natural

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

    //Dungeon Spawn Data
    [Range(0, 100)]
    public int spawnDungeonOnePercentage;

    //Arena platform spawn rates
    [Range(0, 100)]
    public int platformDensity;
    [Range(0, 100)]
    public int spawnSmallPlatformPercentage;

    //Decoritive Items Data
    [Range(0, 100)]
    public int DecorationSpawnDensity;
    public GameObject tree;
    public GameObject bush;
    public GameObject rock;
    public GameObject box;
    public GameObject barrel;
    public GameObject well;
    public GameObject decayingWall;

    void Start()
    {
        Debug.Log("Running start method.");

        if (useRandomSeed == true) //Generate seeded hash. Either Random or preset so terain generation is repeatable
        {
            seed = DateTime.Now.ToString();
            Debug.Log(seed);
        }
        psuedoRandom = new System.Random(seed.GetHashCode());

        mapCenterX = width / 2;
        mapCenterY = height / 2;
        GenMap();
        //spawnDecorations();
        //SpawnPlayers();
    }

    void GenMap()
    {
        Debug.Log("Running GenerateMap method.");
        map = new int[width, height];
        RandomFillMap();
        for (int i = 0; i < smoothingTarget; i++) //SmoothMap exicutes based on the Smoothing target creating softer structures
        {
            SmoothMap();
        }
        //SpawnArena(width, height);
        DrawMap();
    }

    void RandomFillMap()
    {
        Debug.Log("Running RandomFillMap method.");

        //int dungeonspawn;
        int radius = (width < height) ? width/2 : height/2;
        int radiusFromCenter;

        int horizontalSpacing;
        int verticalSpacing;
        int blendValue;

        for (int x = 0; x < width; x++) //Populate map with noise
        {
            for (int y = 0; y < height; y++)
            {
                radiusFromCenter = (int)(Math.Sqrt(Math.Pow(mapCenterX - x, 2) + Math.Pow(mapCenterY - y, 2))); //calculate radius from center of x,y grid

                if (radiusFromCenter <= radius && map[x, y] != 1 && map[x, y] != 2) //get radius and compare to map width
                {
                    blendValue = psuedoRandom.Next(0, blend);
                    horizontalSpacing = islandHeight - psuedoRandom.Next(1, islandSpaceingHorizontal) + blend;
                    verticalSpacing = islandLength - psuedoRandom.Next(1, islandSpaceingVertical) + blend;
                    if (y % islandLength <= verticalSpacing && x % islandHeight <= horizontalSpacing && y > mapCenterY - (radius / 2) + groundGap)
                    {
                        map[x, y] = (psuedoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0; //RandomFillPercent controls the density of noise
                    }

                    /*if (x > mapCenterX - radiusFromCenter && x < mapCenterX + radiusFromCenter - 12 && y > mapCenterY - radiusFromCenter && y < mapCenterY + radiusFromCenter - 12) // Apply Dungeon
                    {
                        dungeonspawn = (psuedoRandom.Next(0, 10000) < spawnDungeonOnePercentage) ? 1 : 0;
                        if (dungeonspawn == 1)
                        {
                            SpawnDungeonOne(x, y);
                        }
                    }*/
                }

                if (radiusFromCenter <= radius)
                {
                    if (y < mapCenterY - (radius / 2)) // only fill lower 4th of the arena with solid ground
                    {
                        map[x, y] = 1;
                    }
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
                        adjacentTitlesCount += map[nextToX, nextToY];
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
                        if (map[x - 1, y] == 0 && map[x + 1, y] == 1 && map[x, y - 1] == 1 && map[x, y + 1] == 0 && map[x + 1, y + 1] == 1) //grass Slope Left
                        {
                            Vector3Int p = new Vector3Int(x, y, 0);
                            tilemap.SetTile(p, grassSlopeLeft);
                        }
                        else if (map[x + 1, y] == 0 && map[x - 1, y] == 1 && map[x, y - 1] == 1 && map[x, y + 1] == 0 && map[x - 1, y + 1] == 1) //grass Slope Right
                        {
                            Vector3Int p = new Vector3Int(x, y, 0);
                            tilemap.SetTile(p, grassSlopeRight);
                        }
                        else if (map[x - 1, y] == 1 && map[x, y - 1] == 1 && map[x, y + 1] == 1 && map[x + 1, y] == 1 && map[x - 1, y + 1] == 0)
                        {
                            Vector3Int p = new Vector3Int(x, y, 0);
                            tilemap.SetTile(p, dirtTopLeftCorner);
                        }
                        else if (map[x - 1, y] == 1 && map[x, y - 1] == 1 && map[x, y + 1] == 1 && map[x + 1, y] == 1 && map[x + 1, y + 1] == 0)
                        {
                            Vector3Int p = new Vector3Int(x, y, 0);
                            tilemap.SetTile(p, dirtTopRightCorner);
                        }
                        else if (map[x, y + 1] == 0 && map[x - 1, y] == 0) //Check for grass position and place left case tile
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
                        else if (map[x - 1, y] == 1 && map[x, y - 1] == 1 && map[x, y + 1] == 1 && map[x + 1, y] == 1 && map[x + 1, y - 1] == 0)
                        {
                            Vector3Int p = new Vector3Int(x, y, 0);
                            tilemap.SetTile(p, dirtBottomLeftCorner);
                        }
                        else if (map[x - 1, y] == 1 && map[x, y - 1] == 1 && map[x, y + 1] == 1 && map[x + 1, y] == 1 && map[x - 1, y - 1] == 0)
                        {
                            Vector3Int p = new Vector3Int(x, y, 0);
                            tilemap.SetTile(p, dirtBottomRightCorner);
                        }
                        else if (map[x - 1, y] == 1 && map[x, y - 1] == 1 && map[x, y + 1] == 1 && map[x + 1, y] == 1) //Find dirt Center
                        {
                            Vector3Int p = new Vector3Int(x, y, 0);
                            tilemap.SetTile(p, dirtCenter);
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

    /*void SpawnDungeonOne(int x, int y)
    {
        Debug.Log("Running SpawnDungeonOne method.");
        int[,] structTest = new int[12, 12] {
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1 },
            { 1, 2, 0, 0, 0, 0, 0, 0, 0, 0, 2, 1 },
            { 1, 2, 0, 0, 0, 0, 0, 0, 0, 0, 2, 1 },
            { 1, 2, 2, 2, 2, 0, 0, 2, 2, 2, 2, 1 },
            { 1, 2, 0, 0, 0, 0, 0, 0, 0, 0, 2, 1 },
            { 1, 2, 0, 0, 0, 0, 0, 0, 0, 0, 2, 1 },
            { 1, 2, 0, 0, 2, 2, 2, 2, 0, 0, 2, 1 },
            { 1, 2, 0, 0, 0, 0, 0, 0, 0, 0, 2, 1 },
            { 1, 2, 0, 0, 0, 0, 0, 0, 0, 0, 2, 1 },
            { 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
        };

        //rotate map 90 degrees
        int n = 12;
        int[,] ret = new int[n, n];
        for (int i = 0; i < n; ++i)
        {
            for (int j = 0; j < n; ++j)
            {
                ret[i, j] = structTest[n - j - 1, i];
            }
        }

        for (int structX = 0; structX < 12; structX++)
        {
            for (int structY = 0; structY < 12; structY++)
            {
                map[x + structX, y + structY] = ret[structX, structY];
            }
        }
    }*/

    void SpawnArena(int width, int height)
    {
        Debug.Log("Running SpawnDungeonOne method.");
        int radius;
        int spawn;

        radius = (width < height) ? width / 9 : height / 9; // find radius for Arena. 9 is arbitrary, just seems to give a good relative sized arena for now

        for (int x = mapCenterX - radius; x < mapCenterX + radius; x++) //Populate map with noise
        {
            for (int y = mapCenterY - radius; y < mapCenterY + radius; y++)
            {
                int radiusFromCenter = (int)(Math.Sqrt(Math.Pow(mapCenterX - x, 2) + Math.Pow(mapCenterY - y, 2))); //calculate radius from center of x,y grid

                if (radiusFromCenter <= radius) 
                {
                    if (y < mapCenterY - (radius/2)) // only fill lower 4th of the arena with solid ground
                    {
                        map[x, y] = 1;
                    }
                    else
                    {
                        map[x, y] = 0;
                    }
                }
            }
        }

        for (int x = mapCenterX - radius; x < mapCenterX + radius; x++) //Populate map with noise
        {
            for (int y = mapCenterY - radius; y < mapCenterY + radius; y++)
            {
                int radiusFromCenter = (int)(Math.Sqrt(Math.Pow(mapCenterX - x, 2) + Math.Pow(mapCenterY - y, 2))); //calculate radius from center of x,y grid

                if (radiusFromCenter <= radius && y > mapCenterY - (radius / 2))
                {
                    spawn = (psuedoRandom.Next(0, 100) < platformDensity) ? 1 : 0;
                    if (spawn == 1)
                    {
                        //spawnSmallPlatform(x, y);
                    }
                }
            }
        }
    }

    /*void spawnSmallPlatform(int x, int y)
    {
        Debug.Log("Running spawnSmallPlatform method.");
        int[,] SmallPlatform = new int[4, 4] {
            { 1, 1, 1, 1 },
            { 1, 1, 1, 1 },
            { 0, 1, 1, 0 },
            { 0, 0, 0, 0 }
        };

        //rotate map 90 degrees
        int n = 4;
        int[,] ret = new int[n, n];
        for (int i = 0; i < n; ++i)
        {
            for (int j = 0; j < n; ++j)
            {
                ret[i, j] = SmallPlatform[n - j - 1, i];
            }
        }

        for (int structX = 0; structX < 4; structX++)
        {
            for (int structY = 0; structY < 4; structY++)
            {
                map[x + structX, y + structY] = ret[structX, structY];
            }
        }
    }*/

    /*void spawnDecorations()
    {
        System.Random psuedoRandom = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++) // Cycle through entire tileMap
        {
            for (int y = 0; y < height; y++)
            {
                int aboveSpaceCount = CountAdjacentTiles(x, y, 1, 1, -1, 3);
                if (aboveSpaceCount == 0 && map[x, y] == 1 && map[x - 1, y] == 1 && map[x + 1, y] == 1 && map[x - 2, y] == 1 && map[x + 2, y] == 1) // if above space is empty Generate Object
                {
                    if (psuedoRandom.Next(0, 100) < DecorationSpawnDensity)
                    {
                        Vector3 p;
                        switch (psuedoRandom.Next(1, 8))
                        {
                            case 1:
                                aboveSpaceCount = CountAdjacentTiles(x, y, 2, 2, -1, 7);
                                p = new Vector3(x, y + 4.35f, 0);
                                Instantiate(tree, p, Quaternion.identity);
                                break;
                            case 2:
                                aboveSpaceCount = CountAdjacentTiles(x, y, 2, 2, -1, 1);
                                p = new Vector3(x, y + 1.5f, 0);
                                Instantiate(bush, p, Quaternion.identity);
                                break;
                            case 3:
                                aboveSpaceCount = CountAdjacentTiles(x, y, 1, 1, -1, 1);
                                p = new Vector3(x, y + 1, 0);
                                Instantiate(rock, p, Quaternion.identity);
                                break;
                            case 4:
                                aboveSpaceCount = CountAdjacentTiles(x, y, 1, 1, -1, 1);
                                p = new Vector3(x, y + 1, 0);
                                Instantiate(box, p, Quaternion.identity);
                                break;
                            case 5:
                                aboveSpaceCount = CountAdjacentTiles(x, y, 1, 1, -1, 1);
                                p = new Vector3(x, y + 1, 0);
                                Instantiate(barrel, p, Quaternion.identity);
                                break;
                            case 6:
                                aboveSpaceCount = CountAdjacentTiles(x, y, 1, 1, -1, 4);
                                p = new Vector3(x, y + 1, 0);
                                Instantiate(well, p, Quaternion.identity);
                                break;
                            case 7:
                                aboveSpaceCount = CountAdjacentTiles(x, y, 2, 2, -1, 1);
                                p = new Vector3(x, y + 1, 0);
                                Instantiate(decayingWall, p, Quaternion.identity);
                                break;
                            case 8:
                                break;
                            default:
                                Debug.Log("Default case for decoration spawn called.");
                                break;
                        }
                    }
                 
                }
            }
        }
    }*/
}

