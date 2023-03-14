using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Windows;
using System.IO.Compression;
using System.IO;
using System.Linq;

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
    //chunkSize here concerns height; width is considered to be 4 times width.
    int chunkSize = 40;

    textureTypes[,] map;
    textureTypes[,] chunk;
    // Start is called before the first frame update
    void Start() => Generation();

    // Update is called once per frame
    void Update()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
        {
            this.seed = UnityEngine.Random.Range(-10000, 10000);
            Generation();
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.S))
        {
            SaveMap();
        }
    }

    void Generation()
    {
        //groundTilemap.ClearAllTiles();
        //this.map = GenerateArray(this.width, this.height, true);
        //TerrainGeneration();
        //ajoutPierres();
        //generateTree((int)seed, 1, 4, 1.0f, 15);
        //UnityEngine.Debug.Log(maptoString());
        stringToMap(ReadMap(), 120, 200);
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
        for (int i = 0; i < width * height; i++)
        {
            int x = i % width;
            int y = i / width;
            perlinHeight = Mathf.RoundToInt(Mathf.PerlinNoise(x / smoothness, seed) * height / 2);
            perlinHeight += height / 2;
            if (y < perlinHeight)
            {
                float caveValue = Mathf.PerlinNoise((x * modifier) + seed, (y * modifier) + seed);
                map[x, y] = (caveValue > 0.60) ? textureTypes.CAVERNE : textureTypes.TERRE;
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

    /// <summary>
    /// This function is used to generate trees following 5 parameters
    /// </summary>
    /// <param name="angle">The rotation angle each recursion</param>
    /// <param name="largeur">The width of the tree</param>
    /// <param name="longueur">The length of the branch</param>
    /// <param name="reduction">The reduction coefficient</param>
    /// <param name="nbRecursion">The number of branching wanted</param>
    public void generateTree(int angle, int largeur, int longueur, float reduction, int nbRecursion)
    {
        float newAngle = (float)angle * 0.001745F;
        //tree spawns in the middle of the map
        int milieu = this.width / 2;

        //same calculation as above to get the surface height in the middle of the map
        int hauteur = Mathf.RoundToInt(Mathf.PerlinNoise(milieu / smoothness, seed) * height/2) + height /2;

        int x = milieu; int y = hauteur;

        //max height of the tree
        int hauteurMax = hauteur + longueur*nbRecursion;

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
    /// <summary>
    /// This function performs a left angle rotation for our tree generation
    /// To get expected results, please note that on call, currentangle has to be currentAngle - rotationAngle
    /// This algorithm uses radiants notation.
    /// </summary>
    /// <param name="currentAngle">Current angle is used for trigonometric calculus</param>
    /// <param name="largeur">Width of current branch</param>
    /// <param name="longueur">Length of current branch</param>
    /// <param name="reductionValue">Lenght reduction coefficient</param>
    /// <param name="nbRecursion">Number of remaining recursions calls</param>
    /// <param name="startingX">Starting X-axis position to trace our branch</param>
    /// <param name="startingY">Starting Y-axis position to trace our branch</param>
    /// <param name="rotationAngle">Rotation angle used for next left/right rotations</param>
    public void leftTreeRecursion(float currentAngle, int largeur, int longueur, float reductionValue, int nbRecursion, int startingX, int startingY, float rotationAngle)
    {
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
            float finalY = (float)longueur * Mathf.Sin(currentAngle) + (float)startingY;
            float finalX = (float)longueur * Mathf.Cos(currentAngle) + (float)startingX;
            sX = (float)startingX;
            sY = (float)startingY;
            for (int i = 0; i < largeur; i++)
            {
                bresenhamGeneral(sX-i, finalX-i, sY, finalY);
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
    /// <summary>
    /// This function performs a right angle rotation for our tree generation
    /// To get expected results, please note that when calling this method, currentangle has to be currentAngle ° rotationAngle
    /// This algorithm uses radiants notation.
    /// </summary>
    /// <param name="currentAngle">Current angle is used for trigonometric calculus</param>
    /// <param name="largeur">Width of current branch</param>
    /// <param name="longueur">Length of current branch</param>
    /// <param name="reductionValue">Lenght reduction coefficient</param>
    /// <param name="nbRecursion">Number of remaining recursions calls</param>
    /// <param name="startingX">Starting X-axis position to trace our branch</param>
    /// <param name="startingY">Starting Y-axis position to trace our branch</param>
    /// <param name="rotationAngle">Rotation angle used for next left/right rotations</param>
    public void rightTreeRecursion(float currentAngle, int largeur, int longueur, float reductionValue, int nbRecursion, int startingX, int startingY, float rotationAngle)
    {
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
            float finalY = (float)longueur * Mathf.Sin(currentAngle) + (float)startingY;
            float finalX = (float)longueur* Mathf.Cos(currentAngle) + (float)startingX;
            sX = (float)startingX;
            sY = (float)startingY;
            for (int i = 0; i < largeur; i++)
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
    /// <summary>
    /// 
    /// </summary>
    /// <param name="x1">Point 1 x coordinate</param>
    /// <param name="x2">Point 2 x coordinate</param>
    /// <param name="y1">Point 1 y coordinate</param>
    /// <param name="y2">Point 2 y coordinate</param>
    /// <param name="IncrX">X-axis increment</param>
    /// <param name="IncrY">Y-axis increment</param>
    /// <param name="dx">X step</param>
    /// <param name="dy">Y step</param>
    /// <param name="inversion">Boolean to tell whether or not we should invert the coordinates used</param>
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
        /**
        * We only keep the integer part of the coordinates to avoid in loop issue.
        **/
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
            bresenham(x1, x2, y1, y2, Incrx, Incry, dx, dy, true);
        }
        else
        {

            bresenham(y1, y2, x1, x2, Incry, Incrx, dy, dx, false);
        }
    }


    public String maptoString()
    {
        string serializedMatrix = "";
        int rows = this.map.GetLength(0);
        int cols = this.map.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                serializedMatrix += $"{(int)this.map[i, j]}|";
            }
            serializedMatrix = serializedMatrix.Remove(serializedMatrix.Length - 1); // Retirer le dernier '|'
            serializedMatrix += ";";
        }
        serializedMatrix = serializedMatrix.Remove(serializedMatrix.Length - 1); // Retirer le dernier ';'
        return serializedMatrix;
    }
    /// <summary>
    /// Map Renderer
    /// </summary>
    /// <param name="map"></param>
    /// <param name="groundTileMap"></param>
    /// <param name="caveTilemap"></param>
    /// <param name="rockTilebase"></param>
    /// <param name="groundTilebase"></param>
    /// <param name="caveTilebase"></param>
    /// <param name="skyTilebase"></param>
    /// <param name="woodTile"></param>
    /// <param name="leaveTile"></param>
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
    public textureTypes[,] stringToMap(string serializedMatrix, int centerX, int centerY)
    {
        int chunkWidth, chunkHeight, endY, endX, startY, startX;
        chunkWidth = this.chunkSize;
        chunkHeight = chunkWidth * 4;
        startX = Mathf.Max(centerX - (chunkWidth / 2), 0);
        startY = Mathf.Max(centerY - (chunkHeight / 2), 0);
        endX = Mathf.Min(centerX + chunkWidth / 2, this.width);
        endY = Mathf.Min(centerY + chunkHeight / 2, this.height);
        string[] rows = serializedMatrix.Split(';');
        int numRows = rows.Length;
        int numCols = rows[0].Split('|').Length;
        this.chunk = new textureTypes[chunkHeight, chunkWidth];
        UnityEngine.Debug.Log(numRows);
        for (int i = startY; i < endY && i<numRows; i++)
        {
            string[] cols = rows[i].Split('|');
            for (int j = startX; j < numCols && j<endX; j++)
            {
                UnityEngine.Debug.Log(cols[j]+"\n"+i+","+j);
                chunk[(i-startX), (j-startY)] = (textureTypes)Enum.Parse(typeof(textureTypes), cols[j]);
            }
        }
        return chunk;
    }


    public String ReadMap()
    {
        byte[] saveData = UnityEngine.Windows.File.ReadAllBytes(Application.dataPath + "/data.txt");
        byte[] decompressedArray;
        using (MemoryStream memoryStream = new MemoryStream(saveData))
        {
            using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
            {
                using (MemoryStream decompressedMemoryStream = new MemoryStream())
                {
                    gzipStream.CopyTo(decompressedMemoryStream);
                    decompressedArray = decompressedMemoryStream.ToArray();
                }
            }
        }
        return Encoding.ASCII.GetString(decompressedArray);
    }
    void SaveMap()
    {
        byte[] saveInfo = Encoding.UTF8.GetBytes(maptoString().ToCharArray());
        byte[] compressedData;
        using (MemoryStream memoryStream = new MemoryStream())
        {
            using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
            {
                gzipStream.Write(saveInfo, 0, saveInfo.Length);
            }
            compressedData = memoryStream.ToArray();
        }
        UnityEngine.Windows.File.WriteAllBytes(Application.dataPath + "/data.txt", compressedData);
        UnityEngine.Debug.Log("<color=green>Saved ! </color>");
    }
    void clearMap()
    {
        groundTilemap.ClearAllTiles();
        caveTilemap.ClearAllTiles();
    }
}
