using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GenerationProcedurale : MonoBehaviour
{
    [SerializeField] int width, height;
    [SerializeField] float smoothness;
    [SerializeField] float seed;
    [SerializeField] TileBase groundTile, caveTile;
    [SerializeField] Tilemap groundTilemap,caveTilemap;

    [Header("Caves")]
    [SerializeField] float modifier;

    int [,] map;
    // Start is called before the first frame update
    void Start()
    {
       Generation(); 
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            Generation();
    }

    void Generation(){
        seed = Random.Range(-10000, 10000);
        groundTilemap.ClearAllTiles();
        this.map=GenerateArray(this.width,this.height,true);
        this.map=TerrainGeneration(this.map);
        RenderMap(map,groundTilemap,caveTilemap ,groundTile, caveTile);
    }

    public int[,] GenerateArray(int width, int height, bool empty){
        int [,] map = new int[width, height];
        for(int x=0; x<width; x++){
            for(int y= 0;y<height; y++){
                map[x,y]= (empty) ?0:1;
            }
        }
        return map;
    }

    public int[,] TerrainGeneration(int[,] map){
        int perlinHeight;
        for(int x=0; x<width;x++){
            perlinHeight=Mathf.RoundToInt(Mathf.PerlinNoise(x/smoothness,seed)*height/2);
            perlinHeight +=height/2;
            for(int y = 0; y<perlinHeight; y++){
                //map[x,y]=1;
                int caveValue = Mathf.RoundToInt(Mathf.PerlinNoise((x * modifier) + seed, (y * modifier) + seed));
                map[x,y] = (caveValue==1)?2:1 ;
            }
        }
        return map;
    }
    public void RenderMap(int [,] map, Tilemap groundTileMap, Tilemap caveTilemap, TileBase groundTilebase, TileBase caveTilebase){
        for(int x=0; x<width; x++){
            for (int y=0; y<height; y++){
                if(map[x,y]==1){
                    groundTileMap.SetTile(new Vector3Int(x,y,0),groundTilebase);
                }
                else if (map[x, y] == 2)
                {
                    caveTilemap.SetTile(new Vector3Int(x, y, 0),caveTilebase);
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
