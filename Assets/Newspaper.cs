using UnityEngine;
using UnityEngine.Events;

public class Newspaper : MonoBehaviour
{
    Vector3 destination;
    float travelVelocity = 4;
    float minDistanceToDestination = 0.1f;

    bool isTraveling = false;

    public UnityEvent snatchedNewspaper = new UnityEvent();

    public void Setup(Vector3 destination)
    {
        isTraveling = true;
        this.destination = destination;
    }

    void Update()
    {
        if (isTraveling)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, travelVelocity * Time.deltaTime);
            float distance = Vector3.Distance(transform.position, destination);
            if(distance < minDistanceToDestination)
            {
                isTraveling = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GM.Instance.PlayerGrabbedNewspaper();
            snatchedNewspaper.Invoke();
            Destroy(gameObject);
        }
    }
}
