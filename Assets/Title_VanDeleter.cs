using UnityEngine;

public class Title_VanDeleter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Van")
        {
            Destroy(other.gameObject);
        }
    }
}
