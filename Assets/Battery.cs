using UnityEngine;

public class Battery : MonoBehaviour
{
    [SerializeField] GameObject model;
    Collider col;

    [SerializeField] float batteryRespawnTime = 10;
    float batteryRespawnTimer = 0;

    bool batteryRespawning = false;

    [SerializeField] float spinRate = 10;
    [SerializeField] float zRotation = 45;

    [SerializeField] float bobSpeed = 5f;
    [SerializeField] float bobHeight = 0.5f;

    [SerializeField] float yOffset = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (batteryRespawning)
        {
            if (batteryRespawnTimer > 0)
            {
                batteryRespawnTimer -= Time.deltaTime;
            }
            else
            {
                Respawn();
            }
        }
        else
        {
            model.transform.eulerAngles = new Vector3 (0, model.transform.eulerAngles.y, zRotation);
            model.transform.Rotate(new Vector3(0, spinRate * Time.deltaTime, 0));

            Vector3 pos = model.transform.localPosition;
            float newY = yOffset + Mathf.Sin(bobSpeed * Time.time);
            model.transform.localPosition = new Vector3(pos.x, newY, pos.z) * bobHeight;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (batteryRespawning)
        {
            return;
        }
        if (other.gameObject.tag == "Player")
        {
            PlayerController player = other.GetComponent<PlayerController>();
            player.PickUpBattery();
            PickedUp();
        }
    }

    void Respawn()
    {
        model.SetActive(true);
        batteryRespawning = false;
    }


    void PickedUp()
    {
        batteryRespawning = true;
        batteryRespawnTimer = batteryRespawnTime;
        model.SetActive(false);
    }
}
