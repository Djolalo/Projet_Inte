using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum textureTypes :short
{ 
    CIEL = 0, 
    CAVERNE =2,
    TERRE = 1,
    PIERRE = 3,
    SABLE = -3,
    OR = -5,
    FER = -4 
}

public class GenerationProcedurale : MonoBehaviour
{
    [SerializeField] int width, height;
    [SerializeField] float smoothness;
    [SerializeField] float seed;
    [SerializeField] TileBase groundTile, caveTile, rockTile, skyTile;
    [SerializeField] Tilemap groundTilemap,caveTilemap;

    [Header("Caves")]
    [SerializeField] float modifier;

    
    textureTypes [,] map;
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

    void Generation(){
        groundTilemap.ClearAllTiles();
        this.map=GenerateArray(this.width,this.height,true);
        this.map=TerrainGeneration(this.map);
        this.map=ajoutPierres(this.map);
        RenderMap(map,groundTilemap,caveTilemap ,rockTile,groundTile, caveTile,skyTile);
    }

    public textureTypes[,] GenerateArray(int width, int height, bool empty){
        textureTypes [,] map = new textureTypes[width, height];
        for(int x=0; x<width; x++){
            for(int y= 0;y<height; y++) {
                map[x,y]= (empty) ?textureTypes.CIEL:textureTypes.TERRE;
            }
        }
        return map;
    }

    public textureTypes[,] TerrainGeneration(textureTypes[,] map){
        int perlinHeight; 
        for(int x=0; x<width;x++){
            perlinHeight=Mathf.RoundToInt(Mathf.PerlinNoise(x/smoothness,seed)*height/2);
            perlinHeight +=height/2;
            for(int y = 0; y<perlinHeight; y++){
                float caveValue = Mathf.PerlinNoise((x * modifier) + seed, (y * modifier) + seed);
                map[x,y] = (caveValue>0.60)?textureTypes.CAVERNE :textureTypes.TERRE ;
            }
        }
        return map;
    }
    public textureTypes[,] ajoutPierres(textureTypes[,] map)
    {
        int perlinHeight; int countEnum = (textureTypes.GetValues(typeof(textureTypes)).Length / 2) + 1; float decalage = 0.3F;
        float binoM;
        for (int x = 0; x < width; x++)
        {
            float caveNoise = Mathf.RoundToInt(Mathf.PerlinNoise(x / (smoothness / 4), seed) * 50);
            perlinHeight = Mathf.RoundToInt(Mathf.PerlinNoise(x / smoothness, seed) * height / 2);
            perlinHeight += height / 2;
            float underGroundTH = (height / 4) + caveNoise;
            for (int y = 0; y < perlinHeight; y++)
            {
                float coefAvenir = (Mathf.PerlinNoise((x*modifier+seed)*0.75F, (y*modifier+seed) * 0.75F));
                UnityEngine.Debug.Log(coefAvenir);
                if (map[x, y] == textureTypes.TERRE)
                {
                    if (underGroundTH > y)
                    {
                        if (coefAvenir > 0.3)
                        {
                            map[x, y] = textureTypes.PIERRE;
                        }
                        else
                        {
                            map[x, y] = textureTypes.TERRE;
                        }
                    }
                    else
                    {
                        if ((coefAvenir<0.3))
                            map[x, y] = textureTypes.PIERRE;
                        else
                        {
                            map[x, y] = textureTypes.TERRE;
                        }
                    }            
                }
            }
        }
        return map;
    }
    public void RenderMap(textureTypes [,] map, Tilemap groundTileMap, Tilemap caveTilemap, TileBase rockTilebase,TileBase groundTilebase, TileBase caveTilebase, TileBase skyTilebase){

        for(int x=0; x<width; x++){
            for (int y=0; y<height; y++){
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
                        groundTileMap.SetTile(new Vector3Int(x,y,0), rockTilebase);
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
