using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] TileBase groundTile, caveTile, rockTile;
    [SerializeField] Tilemap groundTilemap,caveTilemap;

    [Header("Caves")]
    [SerializeField] float modifier;

    
    textureTypes [,] map;
    // Start is called before the first frame update
    void Start() => Generation();

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            Generation();
    }

    void Generation(){
        groundTilemap.ClearAllTiles();
        this.map=GenerateArray(this.width,this.height,true);
        this.map=TerrainGeneration(this.map);
        RenderMap(map,groundTilemap,caveTilemap ,rockTile,groundTile, caveTile);
    }

    public textureTypes[,] GenerateArray(int width, int height, bool empty){
        textureTypes [,] map = new textureTypes[width, height];
        for(int x=0; x<width; x++){
            for(int y= 0;y<height; y++){
                map[x,y]= (empty) ?textureTypes.CIEL:textureTypes.TERRE;
            }
        }
        return map;
    }

    public textureTypes[,] TerrainGeneration(textureTypes[,] map){
        int perlinHeight; int countEnum=(textureTypes.GetValues(typeof(textureTypes)).Length/2)+1;
        for(int x=0; x<width;x++){
            perlinHeight=Mathf.RoundToInt(Mathf.PerlinNoise(x/smoothness,seed)*height/2);
            perlinHeight +=height/2;
            for(int y = 0; y<perlinHeight; y++){
                int caveValue = Mathf.RoundToInt(countEnum*Mathf.PerlinNoise((x * modifier) + seed, (y * modifier) + seed));
                map[x,y] = (caveValue==1)?textureTypes.PIERRE :textureTypes.TERRE ;
            }
        }
        return map;
    }
    public void RenderMap(textureTypes [,] map, Tilemap groundTileMap, Tilemap caveTilemap, TileBase rockTilebase,TileBase groundTilebase, TileBase caveTilebase){
        for(int x=0; x<width; x++){
            for (int y=0; y<height; y++){
                switch (map[x, y])
                {
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
