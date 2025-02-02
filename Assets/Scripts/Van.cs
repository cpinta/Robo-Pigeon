using UnityEngine;

public enum Direction
{
    Forwards,
    Backwards
}

public class Van : MonoBehaviour
{
    public Direction movingDirection;
    public VanNode previousNode;
    public VanNode currentNode;
    [SerializeField] Transform trModel;

    public bool wasHit;
    [SerializeField] float hitSpin = 200;
    [SerializeField] float hitTime = 3;
    float hitTimer = 0;

    Quaternion targetRotation;

    float turnSpeed = 10;
    public float driveSpeed = 10;
    float minNodeDistance = 0.1f;

    float heightOffGround = 0.5f;

    string strBlindedMailTruck = "Blinded Mail Truck";
    int blindedMailTruckScoreAmount = 2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position += Vector3.up * heightOffGround;
    }

    // Update is called once per frame
    void Update()
    {
        if (!wasHit)
        {
            if (currentNode != null)
            {
                float distance = Vector3.Distance(transform.position, currentNode.transform.position);
                if (distance > minNodeDistance)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                    transform.position = Vector3.MoveTowards(transform.position, currentNode.transform.position, Time.deltaTime * driveSpeed);
                }
                else
                {
                    SetNodeToNext();
                }
            }
        }
        else
        {
            if(hitTimer > 0)
            {
                hitTimer -= Time.deltaTime;
            }
            else
            {
                Destroy(gameObject);
            }
            transform.Rotate(new Vector3(0, hitSpin * Time.deltaTime, 0));
        }
    }

    void SetNodeToNext()
    {
        if (previousNode == null)
        {
            previousNode = currentNode;
            SetNodeTarget(currentNode.GetRandomNextNode());
        }
        else
        {
            SetNodeTarget(currentNode.GetRandomNextNode(previousNode));
        }
    }

    public void SetNodeTarget(VanNode node)
    {
        transform.position = currentNode.transform.position;
        previousNode = currentNode;
        currentNode = node;
        targetRotation = Quaternion.LookRotation((currentNode.transform.position - transform.position).normalized);
    }

    public Newspaper ThrowNewspaper(Transform destination)
    {
        Newspaper newspaper = Instantiate(GM.Instance.prefabNewspaper, destination);
        newspaper.transform.position = transform.position;
        newspaper.transform.eulerAngles = new Vector3(0, -90, 0);

        newspaper.Setup(destination.position);

        return newspaper;
    }

    public void Hit()
    {
        wasHit = true;
        hitTimer = hitTime;
        GM.Instance.ChangeScore(blindedMailTruckScoreAmount, strBlindedMailTruck);
    }
}
