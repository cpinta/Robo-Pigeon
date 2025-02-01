using UnityEngine;

public class DebugRenderer : MonoBehaviour
{
    void Start()
    {
        if (!GM.Instance.isInDebugMode)
        {
            //Debug.Log(this.gameObject.name);
            Renderer renderer = GetComponent<Renderer>();
            Destroy(renderer);
        }
    }
}
