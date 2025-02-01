using UnityEngine;

public enum BillboardFacing
{
    front = 0,
    right = 1,
    back = 2,
    left = 3
}

public class char1 : MonoBehaviour
{
    /*
     * facing:
     * 0 = front
     * 1 = right
     * 2 = back
     * 3 = left
     */

    Animator animator;

    string strFacing = "facing";
    string strIsWalking = "isWalking";
    string strIsPickingUp = "isPickingUp";
    string strIsYelling = "isYelling";

    BillboardFacing facing;

    [SerializeField] int backAngle = 65;
    [SerializeField] int sideAngle = 115;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        float signedAngle = Vector3.SignedAngle(transform.forward, Camera.main.transform.forward, Vector3.up);
        float angle = Mathf.Abs(signedAngle);
        print(angle);
        if(angle < backAngle)
        {
            facing = BillboardFacing.back;
        }
        else if (angle < sideAngle)
        {
            if(signedAngle > 0)
            {
                facing = BillboardFacing.left;
            }
            else
            {
                facing = BillboardFacing.right;

            }
        }
        else
        {
            facing = BillboardFacing.front;
        }

        animator.SetInteger(strFacing, (int)facing);
    }
}
