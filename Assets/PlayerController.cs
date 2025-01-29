using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;


public enum PigeonState
{
    Walk = 0,
    Grind = 1,
    Fly = 2
}
public class PlayerController : MonoBehaviour
{
    public Camera cam;
    public GameObject model;
    public Transform trFlyRotation;
    public Rigidbody rb;
    public Animator anim;
    public Collider col;
    public InputAction move;

    public string strIsGrinding = "isGrinding";
    public string strIsGliding = "isGliding";
    public string strFlap = "Flap";

    //fly vars
    public PigeonState state = PigeonState.Fly;
    public Vector2 inputMove = Vector2.zero;
    public Vector2 inputLook = Vector2.zero;
    public bool inputJump = false;
    public Vector3 moveDirection = Vector3.zero;

    //fly vars
    public bool isGliding = false;
    public float flyTurnSpeed = 20;
    public float flyZRotationMax = 25;
    public Vector3 flyRotation = Vector3.zero;
    public float flyRotationZ = 0;
    public float flyRotZSpeed = 25;
    public Vector3 velocity = Vector3.zero;
    public float flySpeed = 10;
    public float flyAcceleration = 40;

    //grind vars
    public bool isGrinding = false;
    [SerializeField] float grindSpeed = 10;
    [SerializeField] float heightOffset;
    float timeForFullSpline;
    float elapsedTime;
    [SerializeField] float lerpSpeed = 10f;
    [SerializeField] Rail currentRail;

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

        PlayerRailCollider railCol = GetComponentInChildren<PlayerRailCollider>();
        railCol.railHit.AddListener(GetOnRail);

        anim.SetBool(strIsGrinding, false);
        anim.SetBool(strIsGrinding, false);

        SetState(PigeonState.Fly);

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
            case PigeonState.Grind:
                MoveAlongRail();
                rb.linearVelocity = Vector3.zero;
                isGrinding = true;
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

                SetTransformRotationToFlyRotation();

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

    void SetTransformRotationToFlyRotation(bool useFlyRotZ = true)
    {
        trFlyRotation.localEulerAngles = flyRotation;
        model.transform.localEulerAngles = new Vector3(flyRotation.x, flyRotation.y, useFlyRotZ? flyRotationZ : 0);
    }

    void SetFlyRotation(Vector3 rotation, bool useFlyRotZ = false)
    {
        flyRotation = rotation;
        SetTransformRotationToFlyRotation(useFlyRotZ);
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
    public void Jump(InputAction.CallbackContext context)
    {
        inputJump = context.performed;
        //Debug.Log("dirVector:" +directionVector);
    }

    void GetOnRail(Rail rail)
    {
        SetState(PigeonState.Grind);
        currentRail = rail;
        CalculateAndSetRailPosition();
    }

    public void SetState(PigeonState newState)
    {
        state = newState;
        switch (newState)
        {
            case PigeonState.Walk:
                anim.SetBool(strIsGliding, false);
                anim.SetBool(strIsGrinding, false);
                isGrinding = false;
                isGliding = false;
                col.enabled = true;
                break;
            case PigeonState.Grind:
                anim.SetBool(strIsGliding, false);
                anim.SetBool(strIsGrinding, true);
                isGrinding = true;
                isGliding = false;
                col.enabled = false;
                break;
            case PigeonState.Fly:
                anim.SetBool(strIsGliding, true);
                anim.SetBool(strIsGrinding, false);
                isGrinding = false;
                isGliding = true;
                col.enabled = true;
                break;
        }
    }

    void MoveAlongRail()
    {
        if (currentRail != null && isGrinding) //This is just some additional error checking.
        {

            float progress = elapsedTime / timeForFullSpline;


            if (progress < 0 || progress > 1)
            {
                ThrowOffRail();
                return;
            }

            float nextTimeNormalised;
            if (currentRail.normalDir)
            {
                nextTimeNormalised = (elapsedTime + Time.deltaTime) / timeForFullSpline;
            }
            else
            {
                nextTimeNormalised = (elapsedTime - Time.deltaTime) / timeForFullSpline;
            }


            float3 pos, tangent, up;
            float3 nextPosfloat, nextTan, nextUp; 
            Vector3 worldPos;
            Vector3 nextPos;

            currentRail.GetNextRailPosition(progress, nextTimeNormalised, out pos, out tangent, out up, out nextPosfloat, out nextTan, out nextUp, out worldPos, out nextPos);
            //SplineUtility.Evaluate(currentRail.railSpline.Spline, progress, out pos, out tangent, out up);
            //SplineUtility.Evaluate(currentRail.railSpline.Spline, nextTimeNormalised, out nextPosfloat, out nextTan, out nextUp);

            //transform.position = worldPos + (transform.up * heightOffset);
            transform.position = worldPos;
            trFlyRotation.rotation = Quaternion.Lerp(trFlyRotation.rotation, Quaternion.LookRotation(nextPos - worldPos), lerpSpeed * Time.deltaTime);
            trFlyRotation.rotation = Quaternion.Lerp(trFlyRotation.rotation, Quaternion.FromToRotation(trFlyRotation.up, up) * trFlyRotation.rotation, lerpSpeed * Time.deltaTime);
            model.transform.localRotation = trFlyRotation.localRotation;

            if (currentRail.normalDir)
            {
                elapsedTime += Time.deltaTime;
            }
            else
            {
                elapsedTime -= Time.deltaTime;
            }

            Debug.Log(worldPos);
        }
    }

    void CalculateAndSetRailPosition()
    {
        timeForFullSpline = currentRail.totalSplineLength / grindSpeed;

        Vector3 splinePoint;

        float normalisedTime = currentRail.CalculateTargetRailPoint(transform.position, out splinePoint);
        elapsedTime = timeForFullSpline * normalisedTime;

        float3 pos, forward, up;
        SplineUtility.Evaluate(currentRail.railSpline.Spline, normalisedTime, out pos, out forward, out up);
        currentRail.CalculateDirection(forward, trFlyRotation.forward);
        transform.position = splinePoint + (transform.up * heightOffset);
        trFlyRotation.transform.rotation = transform.rotation;
    }

    void ThrowOffRail()
    {
        SetState(PigeonState.Fly);
        Vector3 otherAngle = new Vector3(transform.forward.x, trFlyRotation.forward.y, transform.forward.z);
        flyRotation = trFlyRotation.eulerAngles;
        //flyRotation.y = Vector3.Angle(transform.forward, otherAngle);
        currentRail = null;
        SetTransformRotationToFlyRotation();
        //trFlyRotation.position += trFlyRotation.forward * 1;
    }
}
