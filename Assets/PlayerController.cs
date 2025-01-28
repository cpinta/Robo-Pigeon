using UnityEngine;
using UnityEngine.InputSystem;


public enum PigeonState
{
    Walk = 0,
    Fly = 1
}
public class PlayerController : MonoBehaviour
{
    public Camera cam;
    public GameObject model;
    public Transform trFlyRotation;
    public Rigidbody rb;
    public Animator anim;
    public InputAction move;

    public string strIsGrinding = "isGrinding";
    public string strIsGliding = "isGliding";
    public string strFlap = "Flap";


    public PigeonState state = PigeonState.Fly;
    public Vector2 inputMove = Vector2.zero;
    public Vector2 inputLook = Vector2.zero;
    public Vector3 moveDirection = Vector3.zero;
    public bool isGrinding = false;
    public bool isGliding = false;

    public float flyTurnSpeed = 20;
    public float flyZRotationMax = 25;
    public Vector3 flyRotation = Vector3.zero;
    public float flyRotationZ = 0;
    public float flyRotZSpeed = 25;
    public Vector3 velocity = Vector3.zero;
    public float flySpeed = 10;
    public float flyAcceleration = 40;

    public float CAM_DISTANCE = 0.4f;

    public float moveSpeed = 10f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isGrinding = false;
        if (anim == null)
        {
            anim = GetComponentInChildren<Animator>();
        }
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        anim.SetBool(strIsGrinding, false);
        anim.SetBool(strIsGrinding, false);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Vector2 moveVector = Vector2.zero;
        switch (state)
        {
            case PigeonState.Walk:
                moveVector = moveSpeed * inputMove * Time.fixedDeltaTime;

                moveDirection += moveVector.y * new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z).normalized;
                moveDirection += moveVector.x * new Vector3(cam.transform.right.x, 0, cam.transform.right.z).normalized;

                if (moveVector != Vector2.zero)
                {
                    model.transform.eulerAngles = new Vector3(model.transform.eulerAngles.x, cam.transform.eulerAngles.y, model.transform.eulerAngles.z);
                }

                transform.position += moveDirection;

                anim.SetBool(strIsGliding, false);

                break;
            case PigeonState.Fly:
                moveVector = moveSpeed * inputMove * Time.fixedDeltaTime;
;

                if (moveVector.x != 0)
                {
                    flyRotationZ = Mathf.Clamp(flyRotationZ + (moveSpeed * -inputMove.x * Time.fixedDeltaTime), -flyZRotationMax, flyZRotationMax);
                }
                else
                {
                    if(Mathf.Abs(flyRotationZ) > 0.1f) 
                    {
                        flyRotationZ += -Mathf.Sign(flyRotationZ) * flyRotZSpeed * Time.fixedDeltaTime;
                    }
                    else
                    {
                        flyRotationZ = 0;
                    }
                }

                flyRotation += new Vector3(inputMove.y, inputMove.x) * flyTurnSpeed * Time.fixedDeltaTime;
                if (flyRotation.x > 90f)
                {
                    flyRotation.x = 90;
                }
                if (flyRotation.x < -90)
                {
                    flyRotation.x = -90;
                }

                trFlyRotation.localEulerAngles = flyRotation;
                model.transform.localEulerAngles = new Vector3(flyRotation.x, flyRotation.y, flyRotationZ);

                velocity += trFlyRotation.forward * flySpeed * Time.fixedDeltaTime;
                if(rb.linearVelocity.magnitude < flySpeed)
                {
                    rb.linearVelocity += trFlyRotation.forward * flyAcceleration * Time.fixedDeltaTime;
                }
                else
                {
                    rb.linearVelocity = trFlyRotation.forward * flySpeed;
                }

                //transform.position += trFlyRotation.forward * flySpeed * Time.fixedDeltaTime;

                anim.SetBool(strIsGliding, true);
                break;
        }

        moveDirection = Vector3.zero;
    }

    public void Move(InputAction.CallbackContext context)
    {
        inputMove = context.ReadValue<Vector2>();
        //Debug.Log("dirVector:" +directionVector);
    }
    public void Look(InputAction.CallbackContext context)
    {
        inputLook = context.ReadValue<Vector2>();
        //Debug.Log("dirVector:" +directionVector);
    }
}
