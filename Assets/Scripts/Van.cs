using UnityEngine;

public enum Direction
{
    Forwards,
    Backwards
}

public class Van : MonoBehaviour
{
    public Direction movingDirection;
    public VanNode currentNode;

    Quaternion targetRotation;

    float turnSpeed = 10;
    public float driveSpeed = 10;
    float minNodeDistance = 0.1f;

    float heightOffGround = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position += Vector3.up * heightOffGround;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentNode != null)
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

    void SetNodeToNext()
    {
        SetNodeTarget(currentNode.nextNode);
    }

    public void SetNodeTarget(VanNode node)
    {
        transform.position = currentNode.transform.position;
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
}
