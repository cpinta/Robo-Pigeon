using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;


public enum PigeonState
{
    Walk = 0,
    Grind = 1,
    Fly = 2,
    Hover = 3,
    Hurt = 4
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

    string strIsGrinding = "isGrinding";
    string strIsGliding = "isGliding";
    string strIsHovering = "isHovering";
    string strFlap = "Flap";


    //fly vars
    public PigeonState state = PigeonState.Fly;
    public Vector2 inputMove = Vector2.zero;
    public Vector2 inputLook = Vector2.zero;
    public bool inputJump = false;
    public bool inputAimFly = false;
    public Vector3 moveDirection = Vector3.zero;

    //fly vars
    public float flyTurnSpeed = 100;
    public float flyZRotationMax = 25;
    public Vector3 flyRotation = Vector3.zero;
    public float flyRotationZ = 0;
    public float flyRotZSpeed = 25;
    public float flyMaxSpeed = 10;
    public float flyAboveMaxDecel = 2.5f;
    public float flyAcceleration = 5;
    public float gravityMagnitude = 5;

    //flap vars
    public float flapForce = 5;
    public float flapPowerDrain = 10;

    //hover vars
    public float hoverDecelRate = 0.025f;
    public float hoverMinSpeed = 0.5f;

    //power vars
    public float powerMax = 100;
    public float powerMin = 0;
    public float powerMinFullSpeedCharge = 75;
    public float powerDrain = 10;
    public float powerChargeRateRail = 5;
    public float currentPower = 100;

    //grind vars
    bool isSwitchingRails = true;
    [SerializeField] float grindSpeed = 10;
    [SerializeField] float heightOffset;
    float timeForFullSpline;
    float elapsedTime;
    [SerializeField] float lerpSpeed = 10f;
    [SerializeField] Rail currentRail;

    public float grindSwitchRailTime = 0.5f;
    public float grindSwitchRailTimer = 0;

    public float CAM_DISTANCE = 0.4f;

    public float moveSpeed = 10f;

    //hurt vars
    Vector3 hurtDirection = Vector3.zero;
    float isHurtTimer = 0;
    float isHurtTime = 2;
    float hurtPowerLoss = 10;
    float hurtBounceSpeed = 2;

    //score vars
    string strGrindScoreDesc = "Grinding";
    int grindScoreAmount = 1;
    float addGrindScoreEvery = 0.5f;
    float addGrindScoreEveryTimer = 0;

    string strHitMaxSpeedDesc = "Hit Max Speed";
    int maxSpeedScoreAmount = 1;
    float maxSpeedScoreEvery = 0.5f;
    float maxSpeedScoreEveryTimer = 0;

    string strFlapScoreDesc = "Flap Wings";
    int flapScoreAmount = 2;

    string strStoleMailDesc = "Stole Mail";
    int stealMailScoreAmount = 4;
    string strBlindedMailTruck = "Blinded Mail Truck";
    int poopVanScoreAmount = 8;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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

    float GetYAngleEuler()
    {
        Debug.DrawLine(transform.position, transform.position + new Vector3(transform.forward.x, trFlyRotation.forward.y, transform.forward.z) * 1, Color.red);
        float angle = Vector3.Angle(transform.forward, new Vector3(transform.forward.x, trFlyRotation.forward.y, transform.forward.z));
        angle = trFlyRotation.localEulerAngles.y;
        if(angle > 180)
        {
            angle = angle - 360;
        }
        return angle;
    }

    float GetXAngleEuler()
    {
        //Debug.DrawLine(transform.position, transform.position + new Vector3(trFlyRotation.forward.x, transform.forward.y, transform.forward.z) * 1, Color.blue);
        Debug.DrawLine(transform.position, transform.position + new Vector3(trFlyRotation.forward.x, transform.forward.y, transform.forward.z) * 1, Color.blue);
        
        float angle =  Vector3.SignedAngle(transform.forward, new Vector3(trFlyRotation.forward.x, transform.forward.y, transform.forward.z), transform.forward);

        angle = trFlyRotation.localEulerAngles.x;

        //if (angle < -180)
        //{
        //    angle = angle + 360;
        //}

        angle = (transform.rotation * trFlyRotation.rotation).eulerAngles.x;

        return angle;
    }
    float GetZAngleEuler()
    {
        Debug.DrawLine(transform.position, transform.position + new Vector3(transform.forward.x, transform.forward.y, trFlyRotation.forward.z) * 1, Color.green);
        float angle = Vector3.Angle(transform.forward, new Vector3(transform.forward.x, transform.forward.y, trFlyRotation.forward.z));

        angle = trFlyRotation.localEulerAngles.z;

        return angle;
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
                Vector3 prepos = transform.position;
                rb.linearVelocity = Vector3.zero;
                MoveAlongRail();
                Debug.Log(((transform.position - prepos)/Time.fixedDeltaTime).magnitude);
                GainPower(powerChargeRateRail * Time.fixedDeltaTime);
                if(grindSwitchRailTimer > 0)
                {
                    isSwitchingRails = true;
                    grindSwitchRailTimer -= Time.fixedDeltaTime;
                }
                else
                {

                    isSwitchingRails = false;
                }

                if(addGrindScoreEveryTimer > 0)
                {
                    addGrindScoreEveryTimer -= Time.fixedDeltaTime;
                }
                else
                {
                    GM.Instance.ChangeScore(grindScoreAmount, strGrindScoreDesc);
                    addGrindScoreEveryTimer = addGrindScoreEvery;
                }
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

                trFlyRotation.RotateAround(transform.position, Vector3.up, inputMove.x * flyTurnSpeed * Time.fixedDeltaTime);
                trFlyRotation.Rotate(new Vector3(inputMove.y * flyTurnSpeed * Time.fixedDeltaTime, 0, 0));

                float angle = Vector3.Angle(trFlyRotation.forward, transform.up);
                if(angle < 10)
                {
                    trFlyRotation.Rotate(new Vector3(-inputMove.y * (10 - angle), 0, 0));
                }


                model.transform.localEulerAngles = trFlyRotation.localEulerAngles;

                float maxSpeed = flyMaxSpeed * (currentPower < powerMinFullSpeedCharge ? (currentPower / powerMinFullSpeedCharge) : 1);

                if (rb.linearVelocity.magnitude < maxSpeed)
                {
                    rb.linearVelocity = trFlyRotation.forward * (rb.linearVelocity.magnitude + (flyAcceleration * Time.fixedDeltaTime));
                }
                else
                {
                    if ((trFlyRotation.forward * maxSpeed).magnitude != 0)
                    {
                        rb.linearVelocity = trFlyRotation.forward * maxSpeed;
                    }

                    if(rb.linearVelocity.magnitude > flyMaxSpeed)
                    {
                        if (maxSpeedScoreEveryTimer > 0)
                        {
                            maxSpeedScoreEveryTimer -= Time.fixedDeltaTime;
                        }
                        else
                        {
                            ChangeScore(maxSpeedScoreAmount, strHitMaxSpeedDesc);
                            maxSpeedScoreEveryTimer = maxSpeedScoreEvery;
                        }
                    }
                }
                //rb.linearVelocity += transform.up * -gravityMagnitude * Time.fixedDeltaTime;


                LosePower(powerDrain * Time.fixedDeltaTime);

                anim.SetBool(strIsGliding, true);
                break;
            case PigeonState.Hover:
                //model.transform.eulerAngles = new Vector3(model.transform.eulerAngles.x, cam.transform.eulerAngles.y, model.transform.eulerAngles.z);

                model.transform.rotation = cam.transform.rotation;

                rb.linearVelocity = (trFlyRotation.forward * (rb.linearVelocity.magnitude - hoverDecelRate * Time.fixedDeltaTime));

                if(rb.linearVelocity.magnitude < hoverMinSpeed)
                {
                    EndHover();
                }

                anim.SetBool(strIsHovering, true);
                break;
            case PigeonState.Hurt:
                if(isHurtTimer > 0)
                {
                    isHurtTimer -= Time.fixedDeltaTime;
                }
                else
                {
                    SetState(PigeonState.Fly);
                }
                trFlyRotation.RotateAround(transform.position, Vector3.up, inputMove.x * flyTurnSpeed * Time.fixedDeltaTime);
                trFlyRotation.Rotate(new Vector3(inputMove.y * flyTurnSpeed * Time.fixedDeltaTime, 0, 0));
                model.transform.localEulerAngles = trFlyRotation.localEulerAngles;

                transform.position += hurtDirection * Time.fixedDeltaTime;
                break;
        }

        moveDirection = Vector3.zero;

    }

    void EndHover()
    {
        SetState(PigeonState.Fly);
        Vector3 moveVector = moveSpeed * inputMove * Time.fixedDeltaTime;
        moveDirection += moveVector.y * new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z).normalized;
        moveDirection += moveVector.x * new Vector3(cam.transform.right.x, 0, cam.transform.right.z).normalized;

        trFlyRotation.rotation = model.transform.rotation;

        rb.linearVelocity = trFlyRotation.forward * rb.linearVelocity.magnitude;
    }

    void LosePower(float usedPower)
    {
        currentPower -= usedPower;
        if (currentPower < powerMin)
        {
            currentPower = 0;
        }
    }

    void GainPower(float givePower)
    {
        currentPower += givePower;
        if (currentPower > powerMax)
        {
            currentPower = powerMax;
        }
    }

    void ChangeScore(int giveScore, string description)
    {
        GM.Instance.ChangeScore(giveScore, description);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(currentPower == 0)
        {
            //if(rb.linearVelocity.normalized == Vector3.down)
            //{

            //}

            Died();
        }
        else
        {
            Collision(collision);
        }
    }

    void Collision(Collision collision)
    {
        hurtDirection = collision.contacts[0].normal * hurtBounceSpeed;

        LosePower(hurtPowerLoss);
        //hurtDirection = -rb.linearVelocity;
        SetState(PigeonState.Hurt);
        GM.Instance.EndStreak();
    }

    void Died()
    {
        GM.Instance.EndStreak();
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

        switch (state)
        {
            case PigeonState.Grind:
                if (!isSwitchingRails)
                {
                    //Rail newRail = null;
                    //if (inputMove.x > 0)
                    //{
                    //    newRail = currentRail.GetRailRelativeToVector(trFlyRotation.right);
                    //}
                    //else if(inputMove.x < 0)
                    //{
                    //    newRail = currentRail.GetRailRelativeToVector(-trFlyRotation.right);
                    //}
                    //if (newRail != null)
                    //{
                    //    //rb.linearVelocity = trFlyRotation.forward * (currentRail.totalSplineLength / timeForFullSpline);
                    //    ThrowOffRail();
                    //    GetOnRail(newRail);
                    //    isSwitchingRails = true;
                    //    grindSwitchRailTimer = grindSwitchRailTime;
                    //}
                }
                break;
        }


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
        if (inputJump)
        {
            Flap();
        }
        //Debug.Log("dirVector:" +directionVector);
    }
    public void AimFly(InputAction.CallbackContext context)
    {
        inputAimFly = context.performed;
        switch (state)
        {
            case PigeonState.Walk:
                break;
            case PigeonState.Grind:
                break;
            case PigeonState.Fly:
                if (inputAimFly)
                {
                    state = PigeonState.Hover;
                }
                if (!inputAimFly)
                {
                    EndHover();
                }
                break;
            case PigeonState.Hover:
                if (!inputAimFly)
                {
                    EndHover();
                }
                break;
        }
    }

    public void Flap()
    {
        if(state != PigeonState.Hover)
        {
            anim.SetTrigger(strFlap);
            SetState(PigeonState.Fly);
            rb.AddForce(trFlyRotation.forward * flapForce);
            LosePower(flapPowerDrain);
        }
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
                anim.SetBool(strIsHovering, false);
                isSwitchingRails = false;
                col.enabled = true;
                rb.useGravity = true;

                break;
            case PigeonState.Grind:
                anim.SetBool(strIsGliding, false);
                anim.SetBool(strIsGrinding, true);
                anim.SetBool(strIsHovering, false);
                isSwitchingRails = false;
                col.enabled = false;
                rb.useGravity = false;

                addGrindScoreEveryTimer = 0;
                break;
            case PigeonState.Hover:
                anim.SetBool(strIsGliding, false);
                anim.SetBool(strIsGrinding, false);
                anim.SetBool(strIsHovering, true);
                isSwitchingRails = false;
                col.enabled = false;
                rb.useGravity = false;

                break;
            case PigeonState.Fly:
                anim.SetBool(strIsGliding, true);
                anim.SetBool(strIsGrinding, false);
                anim.SetBool(strIsHovering, false);
                isSwitchingRails = false;
                col.enabled = true;
                rb.useGravity = true;

                maxSpeedScoreEveryTimer = 0;
                break;
            case PigeonState.Hurt:
                anim.SetBool(strIsGliding, true);
                anim.SetBool(strIsGrinding, false);
                anim.SetBool(strIsHovering, false);
                isSwitchingRails = false;
                col.enabled = false;
                rb.useGravity = false;

                isHurtTimer = isHurtTime;
                break;
        }
    }

    void MoveAlongRail()
    {
        if (currentRail != null && state == PigeonState.Grind) //This is just some additional error checking.
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
                nextTimeNormalised = (elapsedTime + Time.fixedDeltaTime) / timeForFullSpline;
            }
            else
            {
                nextTimeNormalised = (elapsedTime - Time.fixedDeltaTime) / timeForFullSpline;
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
            trFlyRotation.rotation = Quaternion.Lerp(trFlyRotation.rotation, Quaternion.LookRotation(nextPos - worldPos), lerpSpeed * Time.fixedDeltaTime);
            trFlyRotation.rotation = Quaternion.Lerp(trFlyRotation.rotation, Quaternion.FromToRotation(trFlyRotation.up, up) * trFlyRotation.rotation, lerpSpeed * Time.fixedDeltaTime);
            model.transform.localRotation = trFlyRotation.localRotation;

            if (currentRail.normalDir)
            {
                elapsedTime += Time.fixedDeltaTime;
            }
            else
            {
                elapsedTime -= Time.fixedDeltaTime;
            }

        }
    }

    void CalculateAndSetRailPosition()
    {
        timeForFullSpline = currentRail.totalSplineLength / rb.linearVelocity.magnitude;
        Debug.Log("Pre:"+rb.linearVelocity.magnitude);

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
        Debug.Log("Post:"+currentRail.totalSplineLength / timeForFullSpline);
        rb.linearVelocity = trFlyRotation.forward * (currentRail.totalSplineLength / timeForFullSpline);
        currentRail = null;
        SetTransformRotationToFlyRotation();
        //trFlyRotation.position += trFlyRotation.forward * 1;
    }
}
