using UnityEngine;
using UnityEngine.Events;

public class PlayerRailCollider : MonoBehaviour
{
    public UnityEvent<Rail> railHit;
    public UnityEvent<Rail> railLeft;

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

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Rail")
        {
            railLeft.Invoke(other.gameObject.GetComponent<Rail>());
        }
    }
}
