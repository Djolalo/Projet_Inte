using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum textureTypes : short
{
    CIEL = 0,
    CAVERNE = 2,
    TERRE = 1,
    PIERRE = 3,
    SABLE = -3,
    OR = -5,
    FER = -4,
    BOIS = -6,
    FEUILLE = -7
}

public class GenerationProcedurale : MonoBehaviour
{
    [SerializeField] int width, height;
    [SerializeField] float smoothness;
    [SerializeField] float seed;
    [SerializeField] TileBase groundTile, caveTile, rockTile, skyTile, treeTile, leaveTile;
    [SerializeField] Tilemap groundTilemap, caveTilemap;

    [Header("Caves")]
    [SerializeField] float modifier;


    textureTypes[,] map;
    // Start is called before the first frame update
    void Start() => Generation();

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.seed = UnityEngine.Random.Range(-10000, 10000);
            Generation();
        }
    }

    void Generation()
    {
        groundTilemap.ClearAllTiles();
        this.map = GenerateArray(this.width, this.height, true);
        this.map = TerrainGeneration(this.map);
        this.map = ajoutPierres(this.map);
        this.map = generateTree(map, 25, 3, 1, 0.7F, 3);
        RenderMap(map, groundTilemap, caveTilemap, rockTile, groundTile, caveTile, skyTile, treeTile, leaveTile);
    }

    public textureTypes[,] GenerateArray(int width, int height, bool empty)
    {
        textureTypes[,] map = new textureTypes[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y] = (empty) ? textureTypes.CIEL : textureTypes.TERRE;
            }
        }
        return map;
    }

    public textureTypes[,] TerrainGeneration(textureTypes[,] map)
    {
        int perlinHeight;
        for (int x = 0; x < width; x++)
        {
            perlinHeight = Mathf.RoundToInt(Mathf.PerlinNoise(x / smoothness, seed) * height / 2);
            perlinHeight += height / 2;
            for (int y = 0; y < perlinHeight; y++)
            {
                float caveValue = Mathf.PerlinNoise((x * modifier) + seed, (y * modifier) + seed);
                map[x, y] = (caveValue > 0.60) ? textureTypes.CAVERNE : textureTypes.TERRE;
            }
        }
        return map;
    }
    public textureTypes[,] ajoutPierres(textureTypes[,] map)
    {
        int perlinHeight; int countEnum = (textureTypes.GetValues(typeof(textureTypes)).Length / 2) + 1;
        for (int x = 0; x < width; x++)
        {
            float caveNoise = Mathf.RoundToInt(Mathf.PerlinNoise(x / (smoothness / 4), seed) * 50);
            perlinHeight = Mathf.RoundToInt(Mathf.PerlinNoise(x / smoothness, seed) * height / 2);
            perlinHeight += height / 2;
            float underGroundTH = (height / 4) + caveNoise;
            for (int y = 0; y < perlinHeight; y++)
            {
                float coefAvenir = (Mathf.PerlinNoise((x * modifier + seed) * 0.75F, (y * modifier + seed) * 0.75F));
                if (map[x, y] == textureTypes.TERRE)
                {
                    map[x, y] = (underGroundTH > y) ? (coefAvenir > 0.3) ? textureTypes.PIERRE : textureTypes.TERRE : (coefAvenir < 0.3) ? textureTypes.PIERRE : map[x, y] = textureTypes.TERRE;
                }
            }
        }
        return map;
    }

    public textureTypes[,] generateTree(textureTypes[,] map, int angle, int largeur, int longueur, float reduction, int nbRecursion)
    {
        float newAngle = (float)angle * 0.001745F;
        int milieu = this.width / 2;
        int hauteur = Mathf.RoundToInt(Mathf.PerlinNoise(milieu / smoothness, seed) * height/2);
        int x = milieu; int y = hauteur;
        int hauteurMax = hauteur + longueur; 
        while (x < (milieu + largeur))
        {
            y = hauteur;
            while(y < hauteurMax)
            {
                UnityEngine.Debug.Log("X est en position : "+x+", y : " + y);
                map[x, y] = textureTypes.BOIS;
                y++;
            }
            x++;
        }
        largeur = (int)((float)largeur * reduction);
        longueur = (int)((float)longueur * reduction);
        nbRecursion--;
        float oppositeAngle = (1.57f - (1.57f - newAngle));
        map = leftTreeRecursion(map, oppositeAngle,largeur,longueur, reduction, nbRecursion , milieu, y);
        map = rightTreeRecursion(map, newAngle, largeur, longueur , reduction, nbRecursion, milieu, y);
        return map;
    }
    public textureTypes[,] leftTreeRecursion(textureTypes[,] map, float angle, int largeur, int longueur, float reductionValue, int nbRecursion, int startingX, int startingY)
    {
        UnityEngine.Debug.Log("<color=green> On part à gauche! </color>");
        if (nbRecursion == 0)
        {
            map[startingX, startingY + 1] = textureTypes.FEUILLE;
            map[startingX + 1, startingY + 1] = textureTypes.FEUILLE;
            map[startingX + 1, startingY] = textureTypes.FEUILLE;
            map[startingX - 1, startingY + 1] = textureTypes.FEUILLE;
            map[startingX - 1, startingY] = textureTypes.FEUILLE;
            return map;
        }
        int pasX; int startY = startingY;
        int pasY= (int)(startingY * Mathf.Sin(angle));
        startingY += pasY;
        UnityEngine.Debug.Log(" X : " + startingX + "Y = " + startingY + " - " + pasY + " point de départ: "+startY);
        while (startY <startingY)
        {
            for (int z = 0; z < largeur; z++)
            {
                pasX = (int)(startingX * Mathf.Cos(angle));
                startingX += (startingX - pasX);
                map[startingX, startY] = textureTypes.BOIS;
            }
            UnityEngine.Debug.Log("X = " + startingX + " et y = " + startingY);
            startY++;
        }
        largeur = (int)(largeur * reductionValue);
        longueur = (int)(longueur * reductionValue);
        nbRecursion--;
        map = leftTreeRecursion(map, angle, largeur ,longueur , reductionValue, nbRecursion, startingX, startingY);
        map = rightTreeRecursion(map, 1.57f-(1.57f-angle), largeur , longueur , reductionValue, nbRecursion , startingX, startingY);
        return map;
    }
    public textureTypes[,] rightTreeRecursion(textureTypes[,] map, float angle, int largeur, int longueur, float reductionValue, int nbRecursion, int startingX, int startingY)
    {
        UnityEngine.Debug.Log("<color=green>On part à droite! </color>");
        if (nbRecursion == 0)
        {
            map[startingX, startingY + 1] = textureTypes.FEUILLE;
            map[startingX + 1, startingY + 1] = textureTypes.FEUILLE;
            map[startingX + 1, startingY] = textureTypes.FEUILLE;
            map[startingX - 1, startingY + 1] = textureTypes.FEUILLE;
            map[startingX - 1, startingY] = textureTypes.FEUILLE;
            return map;
        }
        int pasX; int startY = startingY;
        int pasY = (int)(startingY * Mathf.Sin(angle));
        startingY += pasY;
        UnityEngine.Debug.Log(" X : " + startingX + "Y = " + startingY + " - " + pasY + " point de départ: " + startY);
        while (startY < startingY)
        {
            for (int z = 0; z < largeur; z++)
            {
                pasX = (int)(startingX * Mathf.Cos(angle));
                startingX += (startingX - pasX);
                map[startingX, startY] = textureTypes.BOIS;
            }
            startY++;
            UnityEngine.Debug.Log("X = " + startingX + " et y = " + startingY);
        }
        largeur = (int)(largeur * reductionValue);
        longueur = (int)(longueur * reductionValue);
        nbRecursion--;
        map = leftTreeRecursion(map, 1.57f - (1.57f - angle), largeur, longueur, reductionValue, nbRecursion , startingX, startingY);
        map = rightTreeRecursion(map, -angle, largeur, longueur, reductionValue, nbRecursion , startingX, startingY);
        return map;
    }

    public void RenderMap(textureTypes[,] map, Tilemap groundTileMap, Tilemap caveTilemap, TileBase rockTilebase, TileBase groundTilebase, TileBase caveTilebase, TileBase skyTilebase, TileBase woodTile, TileBase leaveTile)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                switch (map[x, y])
                {
                    case textureTypes.CIEL:
                        caveTilemap.SetTile(new Vector3Int(x, y, 0), skyTilebase);
                        break;
                    case textureTypes.TERRE:
                        groundTileMap.SetTile(new Vector3Int(x, y, 0), groundTilebase);
                        break;
                    case textureTypes.CAVERNE:
                        caveTilemap.SetTile(new Vector3Int(x, y, 0), caveTilebase);
                        break;
                    case textureTypes.PIERRE:
                        groundTileMap.SetTile(new Vector3Int(x, y, 0), rockTilebase);
                        break;
                    case textureTypes.BOIS:
                        groundTileMap.SetTile(new Vector3Int(x,y,0), woodTile);
                        break;
                    case textureTypes.FEUILLE:
                        groundTileMap.SetTile(new Vector3Int(x,y,0), leaveTile);
                        break;
                    default: break;
                }
            }
        }
    }
    void clearMap()
    {
        groundTilemap.ClearAllTiles();
        caveTilemap.ClearAllTiles();
    }
}
