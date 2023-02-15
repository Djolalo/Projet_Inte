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
        TerrainGeneration();
        ajoutPierres();
        generateTree(90, 3, 25, 0.9F, 4);
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

    public void TerrainGeneration()
    {
        int perlinHeight;
        for (int x = 0; x < width; x++)
        {
            perlinHeight = Mathf.RoundToInt(Mathf.PerlinNoise(x / smoothness, seed) * height / 2);
            perlinHeight += height / 2;
            for (int y = 0; y < perlinHeight; y++)
            {
                float caveValue = Mathf.PerlinNoise((x * modifier) + seed, (y * modifier) + seed);
                this.map[x, y] = (caveValue > 0.60) ? textureTypes.CAVERNE : textureTypes.TERRE;
            }
        }
    }
    public void ajoutPierres()
    {
        int perlinHeight;
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
                    map[x, y] = (underGroundTH > y) ? (coefAvenir > 0.3) ? textureTypes.PIERRE : textureTypes.TERRE : (coefAvenir < 0.3) ? textureTypes.PIERRE : textureTypes.TERRE;
                }
            }
        }
    }

    public void generateTree(int angle, int largeur, int longueur, float reduction, int nbRecursion)
    {
        float newAngle = (float)angle * 0.001745F;
        //tree spawns in the middle of the map
        int milieu = this.width / 2;

        //same calculation as above to get the surface height in the middle of the map
        int hauteur = Mathf.RoundToInt(Mathf.PerlinNoise(milieu / smoothness, seed) * height/2) + height /2;

        int x = milieu; int y = hauteur;

        //max height of the tree
        int hauteurMax = hauteur + longueur;

        //tracing the trunk
        while (x < (milieu + largeur))
        {
            y = hauteur;
            while(y < hauteurMax)
            {
                map[x, y] = textureTypes.BOIS;
                y++;
            }
            x++;
        }

        //reduction : float number that reduces at each iteration so that the tree branches get smaller
        largeur = (int)((float)largeur * reduction);
        longueur = (int)((float)longueur * reduction);

        //nbRecursion == 0 is the end of recursivity condition
        nbRecursion--;
        leftTreeRecursion(1.57f + newAngle,largeur,longueur, reduction, nbRecursion , milieu, y, newAngle);
        rightTreeRecursion(1.57f - newAngle, largeur, longueur, reduction, nbRecursion, milieu, y, newAngle);
    }
    public void leftTreeRecursion(float currentAngle, int largeur, int longueur, float reductionValue, int nbRecursion, int startingX, int startingY, float rotationAngle)
    {
        UnityEngine.Debug.Log("<color=green> On part à gauche! </color>");

        //put leaves at the end of branches
        if (nbRecursion == 0)
        {
            map[startingX, startingY] = textureTypes.BOIS;
            map[startingX, startingY + 1] = textureTypes.FEUILLE;
            map[startingX + 1, startingY + 1] = textureTypes.FEUILLE;
            map[startingX + 1, startingY] = textureTypes.FEUILLE;
            map[startingX - 1, startingY + 1] = textureTypes.FEUILLE;
            map[startingX - 1, startingY] = textureTypes.FEUILLE;
            return;
        }
        if (nbRecursion >= 1)
        {
            float sX, sY; int x, y;
            float finalX = (float)longueur * Mathf.Sin(currentAngle) + (float)startingX;
            float finalY = (float)longueur * Mathf.Cos(currentAngle) + (float)startingY;
            sX = (float)startingX;
            sY = (float)startingY;
            UnityEngine.Debug.Log("Coordonnées de départ : x" + sX + ", y " + sY + " Coordonnées d'arrivée : x " + finalX + ", y " + finalY);

            bresenhamGeneral(sX,finalX,sY,finalY);
            largeur = (int)(largeur * reductionValue);
            longueur = (int)(longueur * reductionValue);
            nbRecursion--;
            x = Mathf.FloorToInt(finalX);
            y = Mathf.FloorToInt(finalY);
            leftTreeRecursion(currentAngle + rotationAngle, largeur, longueur, reductionValue, nbRecursion, x, y, rotationAngle);
            rightTreeRecursion(currentAngle - rotationAngle, largeur, longueur, reductionValue, nbRecursion, x, y, rotationAngle);
        }
    }

    public void rightTreeRecursion(float currentAngle, int largeur, int longueur, float reductionValue, int nbRecursion, int startingX, int startingY, float rotationAngle)
    {
        UnityEngine.Debug.Log("<color=green>On part à droite! </color>");
        if (nbRecursion == 0)
        {
            map[startingX, startingY] = textureTypes.BOIS;
            map[startingX, startingY + 1] = textureTypes.FEUILLE;
            map[startingX + 1, startingY + 1] = textureTypes.FEUILLE;
            map[startingX + 1, startingY] = textureTypes.FEUILLE;
            map[startingX - 1, startingY + 1] = textureTypes.FEUILLE;
            map[startingX - 1, startingY] = textureTypes.FEUILLE;
            return;
        }
        if (nbRecursion >= 1)
        {
            float sX, sY; int x, y;
            float finalX = (float)longueur * Mathf.Sin(currentAngle) + (float)startingX;
            float finalY = (float)longueur* Mathf.Cos(currentAngle) + (float)startingY;
            sX = (float)startingX;
            sY = (float)startingY;
            UnityEngine.Debug.Log("Coordonnées de départ : x" + sX + ", y " + sY + " Coordonnées d'arrivée : x " + finalX + ", y " + finalY);

            for (int i = 0; i < finalY - startingY; i++)
            {
                bresenhamGeneral(sX + i, finalX + i, sY, finalY);
            }
            largeur = (int)(largeur * reductionValue);
            longueur = (int)(longueur * reductionValue);
            nbRecursion--;
            x = Mathf.FloorToInt(finalX);
            y = Mathf.FloorToInt(finalY);
            leftTreeRecursion(currentAngle + rotationAngle, largeur, longueur, reductionValue, nbRecursion, x, y, rotationAngle);
            rightTreeRecursion(currentAngle - rotationAngle, largeur, longueur, reductionValue, nbRecursion, x, y, rotationAngle);
        }
    }
    public void bresenham(float x1, float x2, float y1, float y2, float IncrX, float IncrY, float dx, float dy, bool inversion)
    {
        float IncreE = 2 * dy;
        float IncreNE = 2 * (dy - dx);
        float dp = 2 * (dy - dx);
        float y = y1;
        int abcisse, ordonnee;

        for (float x = x1; x != x2; x += IncrX)
        {
            abcisse = (int)(x);
            ordonnee = (int)(y);
            if (inversion) map[abcisse, ordonnee] = textureTypes.BOIS;
            else map[ordonnee, abcisse]= textureTypes.BOIS;
            if (dp <= 0)
            {
                dp += IncreE;
            }
            else
            {
                y += IncrY;
                dp += IncreNE;
            }
        }
    }


    public void bresenhamGeneral(float x1, float x2, float y1, float y2)
    {
        /*
        We only keep the integer part of the coordinates to avoid in loop issue.
        */
        x1 = Mathf.Floor(x1);
        x2 = Mathf.Floor(x2);
        y1 = Mathf.Floor(y1);
        y2 = Mathf.Floor(y2);
        float dy = y2 - y1;
        float dx = x2 - x1;
        float Incry, Incrx;
        if (dx > 0)
        {
            Incrx = 1f;
        }
        else
        {
            Incrx = -1f;
            dx = -dx;
        }
        if (dy > 0)
        {
            Incry = 1f;
        }
        else
        {
            dy = -dy;
            Incry = -1f;
        }
        if (dx >= dy)
        {
            bresenham(x1, x2, y1, y2, Incrx, Incry, dx, dy, false);
        }
        else
        {

            bresenham(y1, y2, x1, x2, Incry, Incrx, dy, dx, true);
        }
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
