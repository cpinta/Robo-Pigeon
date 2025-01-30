using UnityEngine;
using UnityEngine.Events;

public class VanNode : MonoBehaviour
{
    public VanNode nextNode;
    public VanNode prevNode;

    Renderer renderer;

    public int vansInSphere = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool IsVisibleToCamera()
    {
        return renderer.isVisible;
    }

    public float DistanceToNode(Vector3 position)
    {
        return (transform.position - position).magnitude;
    }

    public bool CanBeSpawnedAt()
    {
        return vansInSphere == 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        vansInSphere++;
    }

    private void OnTriggerExit(Collider other)
    {
        vansInSphere--;
        if(vansInSphere < 0)
        {
            Debug.Log("van node at "+transform.position+" had -1 vans");
            vansInSphere = 0;
        }
    }
}
