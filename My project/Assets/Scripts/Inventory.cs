using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int flowersCount;
    public int stoneCount;

    public static Inventory instance;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogWarning("il y a plus d'un inventaire dans la scene");
            return;
        }

        instance = this;
    }

    public void AddFlowers(int count)
    {
        flowersCount += count;
    }

    public void AddStone(int count)
    {
        stoneCount += count;
    }
}
