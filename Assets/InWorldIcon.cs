using UnityEngine;

public class InWorldIcon : MonoBehaviour
{
    [SerializeField] float sizeMultiplier;

    void Update()
    {
        float size = (Camera.main.transform.position - transform.position).magnitude * (sizeMultiplier/10);
        transform.localScale = new Vector3(size, size, size);
    }
}
