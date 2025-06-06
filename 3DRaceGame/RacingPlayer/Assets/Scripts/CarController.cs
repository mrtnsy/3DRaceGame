using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    private float horizontalInput;
    private float verticalInput;
    private float currentSteerAngle;
    private float currentBreakForce;
    private bool isBreaking;
    private bool isHandBraking;

    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;

    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;

    [SerializeField] private GameObject[] headLight = new GameObject[2];
    private bool troca = true;

    [SerializeField] private float motorForce = 3500f;
    [SerializeField] private float breakForce = 3000f;
    [SerializeField] private float handBrakeForce = 3000f;
    [SerializeField] private float maxSteeringAngle = 50f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0); // Baixando centro de massa
    }

    private void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
        RestartPosition();
        TurnOnLight();
        HandleDrift();

        rb.AddForce(-transform.up * rb.linearVelocity.magnitude * 10f); // Downforce
    }

    private void RestartPosition()
    {
        if (Input.GetKey("r"))
        {
            Debug.Log("RestartPosition");
            transform.position = new Vector3(1f, transform.position.y, transform.position.z);
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis(HORIZONTAL);
        verticalInput = Input.GetAxis(VERTICAL);
        isBreaking = Input.GetKey(KeyCode.Space);
        isHandBraking = Input.GetKey(KeyCode.LeftShift);
    }

    private void HandleMotor()
    {
        float appliedMotorForce = verticalInput * motorForce;

        frontLeftWheelCollider.motorTorque = appliedMotorForce;
        frontRightWheelCollider.motorTorque = appliedMotorForce;
        rearLeftWheelCollider.motorTorque = appliedMotorForce;
        rearRightWheelCollider.motorTorque = appliedMotorForce;

        currentBreakForce = isBreaking ? breakForce : 0f;

        ApplyBreaking();
    }

    private void ApplyBreaking()
    {
        // Aplica o freio padrão
        frontLeftWheelCollider.brakeTorque = currentBreakForce;
        frontRightWheelCollider.brakeTorque = currentBreakForce;
        rearLeftWheelCollider.brakeTorque = currentBreakForce;
        rearRightWheelCollider.brakeTorque = currentBreakForce;

        // Freio de mão: força extra nas rodas traseiras
        if (isHandBraking)
        {
            rearLeftWheelCollider.brakeTorque += handBrakeForce;
            rearRightWheelCollider.brakeTorque += handBrakeForce;
        }
    }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteeringAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheelCollider(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheelCollider(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheelCollider(rearLeftWheelCollider, rearLeftWheelTransform);
        UpdateSingleWheelCollider(rearRightWheelCollider, rearRightWheelTransform);
    }

    private void UpdateSingleWheelCollider(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.position = pos;
        wheelTransform.rotation = rot;
    }

    private void HandleDrift()
{
    WheelFrictionCurve rearFriction = rearLeftWheelCollider.sidewaysFriction;

    if (isHandBraking)
    {
        rearFriction.extremumSlip = 0.4f;    // ponto de início do derrape
        rearFriction.extremumValue = 0.5f;   // tração inicial
        rearFriction.asymptoteSlip = 0.8f;   // ponto onde começa a perder controle
        rearFriction.asymptoteValue = 0.4f;  // tração perdida
        rearFriction.stiffness = 0.3f;       // quanto o pneu adere lateralmente
    }
    else
    {
        rearFriction.extremumSlip = 0.2f;
        rearFriction.extremumValue = 1f;
        rearFriction.asymptoteSlip = 0.5f;
        rearFriction.asymptoteValue = 0.75f;
        rearFriction.stiffness = 2f;
    }

    rearLeftWheelCollider.sidewaysFriction = rearFriction;
    rearRightWheelCollider.sidewaysFriction = rearFriction;
}

    private void TurnOnLight()
    {
        if (Input.GetKeyDown("l"))
        {
            troca = !troca;
            if (headLight.Length >= 2)
            {
                headLight[0].SetActive(troca);
                headLight[1].SetActive(troca);
            }
        }
    }
}
