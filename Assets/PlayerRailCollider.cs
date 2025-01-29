using UnityEngine;
using UnityEngine.Events;

public class PlayerRailCollider : MonoBehaviour
{
    public UnityEvent<Rail> railHit;

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Rail")
        {
            railHit.Invoke(other.gameObject.GetComponent<Rail>());
        }
    }
}
