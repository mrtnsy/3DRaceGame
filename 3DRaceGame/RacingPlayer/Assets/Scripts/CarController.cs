using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
//Com base neste Script - vamos melhorar nosso carro e deixa-lo mais tunano 😜
//CRIE UM FREIO DE MÃO .

public class CarController : MonoBehaviour
{    
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    private float horizontalInput;
    private float verticalInput;
    private float currentSteerAngle;
    private float currentBreakForce;
    private bool isBreaking;


    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider RearLeftWheelCollider;
    [SerializeField] private WheelCollider RearRightWheelCollider;

    
    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform RearLeftWheelTransformr;
    [SerializeField] private Transform RearRightWheelTransform;

    [SerializeField] private GameObject[] HeadLight = new GameObject[1];
    private bool troca = true;


    [SerializeField] private float motorForce;
    [SerializeField] private float breakForce;
    [SerializeField] private float maxSteeringAngle;

    private void FixedUpdate() 
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
        RestartPosition();      
        TurnOnLight(); 

    }

    private void RestartPosition()
    {
       if(Input.GetKey("r"))
       {
           Debug.Log("RestartPosition");
           transform.position = new Vector3(3f, transform.position.y, transform.position.z);
           transform.rotation = Quaternion.Euler(0f, 0f, 0f);
       }
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis(HORIZONTAL);
        verticalInput = Input.GetAxis(VERTICAL);
        isBreaking = Input.GetKey(KeyCode.Space);
    }

    private void HandleMotor()
    {
        frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
        frontRightWheelCollider.motorTorque = verticalInput * motorForce;
        RearLeftWheelCollider.motorTorque = verticalInput * motorForce;
        RearRightWheelCollider.motorTorque = verticalInput * motorForce;
        currentBreakForce = isBreaking ? breakForce : 0f;
        ApplyBreaking();   
    }

    private void ApplyBreaking()
    {
        frontRightWheelCollider.brakeTorque = currentBreakForce;
        frontLeftWheelCollider.brakeTorque = currentBreakForce; 
        RearLeftWheelCollider.brakeTorque = currentBreakForce;
        RearRightWheelCollider.brakeTorque = currentBreakForce;
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
       UpdateSingleWheelCollider(RearRightWheelCollider, RearRightWheelTransform);
       UpdateSingleWheelCollider(RearLeftWheelCollider, RearLeftWheelTransformr);
       
    }

    private void UpdateSingleWheelCollider( WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos,  out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }
   
   
    private void TurnOnLight(){
      
      if(Input.GetKey("l") ){

         troca = !troca;
        HeadLight[0].SetActive(troca);
        HeadLight[1].SetActive(troca);
      }
    }

}