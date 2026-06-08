using UnityEngine;

public class CarController : MonoBehaviour
{
    public float motorForce = 100f; 
    public float brakeForce = 1000f;
    public float maxSteerAngle = 30f;

    [Header("Input Settings")]
    public CarInputHandler inputHandler;

    [Header("Physics: 4 Wheel Colliders")]
    public WheelCollider frontLeftWheelCollider;
    public WheelCollider frontRightWheelCollider;
    public WheelCollider rearLeftWheelCollider;
    public WheelCollider rearRightWheelCollider;

    [Header("Visuals: 3 Wheel Meshes")]
    public Transform frontLeftWheelTransform;
    public Transform frontRightWheelTransform;
    public Transform rearWheelTransform;

    [Header("Stability Settings")]
    public Vector3 centerOfMassOffset = new Vector3(0, -1.0f, 0);
    public float antiRollForce = 5000f; // Force to keep the car from flipping

    private float horizontalInput;
    private float verticalInput;
    private float currentSteerAngle;
    private float currentBrakeForce;
    private bool isBraking;
    
    private Rigidbody carRigidbody;

    void Start()
    {
        if (inputHandler == null)
        {
            inputHandler = GetComponent<CarInputHandler>();
        }
        
        carRigidbody = GetComponent<Rigidbody>();
        // Lowering the center of mass keeps the car from flipping over easily
        carRigidbody.centerOfMass = centerOfMassOffset;
    }

    private void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        ApplyBraking();
        ApplyAntiRoll();
    }

    void ApplyAntiRoll()
    {
        ApplyAntiRollToAxle(frontLeftWheelCollider, frontRightWheelCollider);
        ApplyAntiRollToAxle(rearLeftWheelCollider, rearRightWheelCollider);
    }

    void ApplyAntiRollToAxle(WheelCollider leftWheel, WheelCollider rightWheel)
    {
        WheelHit hit;
        float travelL = 1.0f;
        float travelR = 1.0f;

        bool groundedL = leftWheel.GetGroundHit(out hit);
        if (groundedL)
            travelL = (-leftWheel.transform.InverseTransformPoint(hit.point).y - leftWheel.radius) / leftWheel.suspensionDistance;

        bool groundedR = rightWheel.GetGroundHit(out hit);
        if (groundedR)
            travelR = (-rightWheel.transform.InverseTransformPoint(hit.point).y - rightWheel.radius) / rightWheel.suspensionDistance;

        float antiRollVal = (travelL - travelR) * antiRollForce;

        if (groundedL)
            carRigidbody.AddForceAtPosition(leftWheel.transform.up * -antiRollVal, leftWheel.transform.position);
        if (groundedR)
            carRigidbody.AddForceAtPosition(rightWheel.transform.up * antiRollVal, rightWheel.transform.position);
    }

    private void LateUpdate()
    {
        UpdateWheels();
    }

    void GetInput()
    {
        if (inputHandler != null)
        {
            horizontalInput = inputHandler.HorizontalInput;
            verticalInput = inputHandler.VerticalInput;
            isBraking = inputHandler.IsBraking;
        }
        else
        {
            horizontalInput = 0f;
            verticalInput = 0f;
            isBraking = false;
        }
    }

    void HandleMotor()
    {
        // Currently Front-Wheel Drive. You can apply this to the rear wheels for Rear-Wheel Drive!
        frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
        frontRightWheelCollider.motorTorque = verticalInput * motorForce;

        currentBrakeForce = isBraking ? brakeForce : 0f;
    }

    void ApplyBraking()
    {
        frontLeftWheelCollider.brakeTorque = currentBrakeForce;
        frontRightWheelCollider.brakeTorque = currentBrakeForce;
        rearLeftWheelCollider.brakeTorque = currentBrakeForce;
        rearRightWheelCollider.brakeTorque = currentBrakeForce;
    }

    void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        if (wheelTransform == null) return;

        Vector3 position;
        Quaternion rotation;
        wheelCollider.GetWorldPose(out position, out rotation);
        wheelTransform.position = position;
        wheelTransform.rotation = rotation;
    }

    void UpdateWheels()
    {
        // Update the front wheels normally
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);

        // Special logic for the single rear visual wheel
        if (rearWheelTransform != null && rearLeftWheelCollider != null && rearRightWheelCollider != null)
        {
            Vector3 leftPos, rightPos;
            Quaternion leftRot, rightRot;

            rearLeftWheelCollider.GetWorldPose(out leftPos, out leftRot);
            rearRightWheelCollider.GetWorldPose(out rightPos, out rightRot);

            // Average the positions to keep the visual wheel perfectly centered
            rearWheelTransform.position = (leftPos + rightPos) / 2f;

            // Apply the rotation from one of the wheels (they spin the same way)
            rearWheelTransform.rotation = leftRot;
        }
    }
}