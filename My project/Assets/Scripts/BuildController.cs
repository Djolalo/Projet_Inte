using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildController : MonoBehaviour
{
    public RuleTile grassTile;
    public Tilemap groundTileMap;

    public float castDistance = 1.0f;
    public Transform raycastPoint;
    public LayerMask layer;

    float blockDestroyTime = 0.2f;

    Vector3 direction;
    RaycastHit2D hit;

    bool destroyingBlock = false;
    bool placingBlock = false;

    public void FixedUpdate(){

        //que ce soit pour construire ou detruire un bloc on recupere la direction du raycast
        if(Input.GetKey(KeyCode.F) || Input.GetKey(KeyCode.C)){
            RaycastDirection();
        }

    }
    
    //fonction qui permet de recuperer la direction ou on veut que l'action soit effectuee
    public void RaycastDirection(){
        if(Input.GetAxis("Horizontal")!=0 ||Input.GetAxis("Vertical")!=0 ){
            direction.x = Input.GetAxis("Horizontal");
            direction.y = Input.GetAxis("Vertical");
        }

        hit = Physics2D.Raycast(raycastPoint.position, direction, castDistance, layer.value);
        Vector2 endpos = raycastPoint.position + direction;

        Debug.DrawLine(raycastPoint.position, endpos, Color.red);

        //si on appuie sur la touche F on detruit le bloc pointe par le raycast
        if(Input.GetKey(KeyCode.F)){
            if(hit.collider && !destroyingBlock){
                destroyingBlock = true;
                StartCoroutine(DestroyBlock(hit.collider.gameObject.GetComponent<Tilemap>(), endpos));
            }
        }

        //si on appuie sur la touche C on construit un bloc la ou pointe par le raycast
        if(Input.GetKey(KeyCode.C)){
            if(!hit.collider && !placingBlock){
                placingBlock = true;
                StartCoroutine(PlaceBlock(groundTileMap, endpos));
            }
        }
    }

    
    IEnumerator DestroyBlock(Tilemap map, Vector2 pos){
        yield return new WaitForSeconds(blockDestroyTime);

        pos.y = Mathf.Floor(pos.y);
        pos.x = Mathf.Floor(pos.x);

        map.SetTile(new Vector3Int((int)pos.x,(int)pos.y, 0), null);


        destroyingBlock = false;
    }

    IEnumerator PlaceBlock(Tilemap map, Vector2 pos){
        yield return new WaitForSeconds(0f);

        pos.y = Mathf.Floor(pos.y);
        pos.x = Mathf.Floor(pos.x);

        map.SetTile(new Vector3Int((int)pos.x,(int)pos.y, 0), grassTile);



        placingBlock = false;
    }
}
