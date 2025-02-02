using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Title_VanSpawner : MonoBehaviour
{
    VanNode startNode;
    [SerializeField] Van prefabVan;
    [SerializeField] float timeBtSpawns = 5;
    float timeBtSpawnsTimer = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startNode = GetComponent<VanNode>();
    }

    // Update is called once per frame
    void Update()
    {
        if(timeBtSpawnsTimer > 0)
        {
            timeBtSpawnsTimer -= Time.deltaTime;
        }
        else
        {
            SpawnAtSpawnTile();
        }
    }

    public bool SpawnAtSpawnTile()
    {
        timeBtSpawnsTimer = timeBtSpawns;
        VanNode chosen = startNode;

        Van van = Instantiate(prefabVan, chosen.transform);
        van.currentNode = startNode;
        //van.transform.rotation = Quaternion.LookRotation((van.currentNode.transform.position - van.transform.position).normalized);
        van.SetNodeTarget(van.currentNode);
        return true;
    }
}
