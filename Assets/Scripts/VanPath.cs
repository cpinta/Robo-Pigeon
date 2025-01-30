using System.Collections.Generic;
using UnityEngine;

public class VanPath : MonoBehaviour
{
    int pathLength;
    List<VanNode> pathNodes = new List<VanNode>();

    float timeBetweenVanSpawns = 4;
    float timeBetweenVanSpawnsTimer = 0;

    bool spawned = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        VanNode n = GetComponent<VanNode>();
        pathNodes.Add(n);
        while (n.transform.childCount != 0)
        {
            VanNode newn = n.transform.GetChild(0).GetComponent<VanNode>();
            n.nextNode = newn;
            newn.prevNode = n;
            n = newn;
            pathNodes.Add(n);
        }
        pathNodes[0].prevNode = pathNodes[pathNodes.Count - 1];
        pathNodes[pathNodes.Count - 1].nextNode = pathNodes[0];
    }

    // Update is called once per frame
    void Update()
    {
        if(timeBetweenVanSpawnsTimer > 0)
        {
            timeBetweenVanSpawnsTimer -= Time.deltaTime;
        }
        else
        {
            SpawnVanAtRandomNode();
        }
    }

    public bool SpawnVanAtRandomNode()
    {
        timeBetweenVanSpawnsTimer = timeBetweenVanSpawns;
        List<VanNode> notVisibleNodes = new List<VanNode>();
        for (int i = 0; i < pathNodes.Count; i++) 
        {
            if (pathNodes[i].IsVisibleToCamera())
            {
                notVisibleNodes.Add(pathNodes[i]);
            }
        }

        if(notVisibleNodes.Count == 0)
        {
            return false;
        }

        VanNode chosen = notVisibleNodes[(int)(Random.value * (notVisibleNodes.Count - 1))];

        Van van = Instantiate(GM.Instance.prefabVan, chosen.transform);
        van.currentNode = chosen.nextNode;
        van.transform.rotation = Quaternion.LookRotation((van.currentNode.transform.position - van.transform.position).normalized);
        van.SetNodeTarget(van.currentNode);

        return true;
    }
}
