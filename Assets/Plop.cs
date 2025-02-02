using UnityEngine;

public class Plop : MonoBehaviour
{
    Vector3 direction;
    float speed;


    Vector3 startPosition = Vector3.zero;
    float maxDistance = 1000;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        transform.position += direction * speed * Time.fixedDeltaTime;
        if(Vector3.Distance(startPosition, transform.position) > maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision) 
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Van")
        {
            Van van = other.GetComponent<Van>();
            van.Hit();

            Destroy(gameObject);
        }
    }

    public void Setup(Vector3 startPosition, Vector3 direction, float speed)
    {
        this.startPosition = startPosition;
        this.direction = direction;
        this.speed = speed;
    }
}
