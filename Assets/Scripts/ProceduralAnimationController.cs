using UnityEngine;

public class ProceduralAnimationController : MonoBehaviour
{
    public Transform target;
    private Transform targetTransform;

    public bool animatePosition = true;
    public bool animateRotation;
    public bool animateScale;

    public Vector3 positionConstants = new(1,1,1);
    public Vector3 rotationConstants = new(1,1,1);
    public Vector3 scaleConstants = new(1,1,1);

    [HideInInspector]
    public bool disableSimulation;

    private SecondOrderDynamics movementDynamics;
    private SecondOrderDynamics rotationDynamics;
    private SecondOrderDynamics scaleDynamics;

    private Transform defaultTransform;

    private void Start()
    {
        defaultTransform = transform;
        // Initialize SecondOrderDynamics with appropriate values
        Quaternion initialRotation = transform.rotation;
        Vector3 initialScale = transform.localScale;
        Vector3 initialPosition = transform.position;
        Vector4 initialRotationV4 = new(initialRotation.x, initialRotation.y, initialRotation.z, initialRotation.w);
        movementDynamics = new(positionConstants.x, positionConstants.y, positionConstants.z, initialPosition);
        rotationDynamics = new(rotationConstants.x, rotationConstants.y, rotationConstants.z, initialRotationV4);
        scaleDynamics = new(scaleConstants.x, scaleConstants.y, scaleConstants.z, initialScale);

        // Set the initial target position
        targetTransform = target;
    }

    private void FixedUpdate()
    {
        // Update the target position based on your logic
        targetTransform = target;

        if (animatePosition)
        {
            //POSITION
            // Update the dynamics and get the new position
            Vector3 newPosition = movementDynamics.Update(Time.deltaTime, targetTransform.position, disableSimulation);
            transform.position = newPosition;
        }
        else
        {
            transform.position = targetTransform.position;
        }

        if (animateRotation)
        {
            //ROTATION
            //update the target rotation and convert it into a vector4
            Quaternion targetRotation = targetTransform.rotation;
            Vector4 targetRotationV4 = new(targetRotation.x, targetRotation.y, targetRotation.z, targetRotation.w);

            Vector4 newRotation = rotationDynamics.Update(Time.deltaTime, targetRotationV4, disableSimulation);

            //reconvert rotation into the quaternion
            Quaternion finalRotation = new(newRotation.x, newRotation.y, newRotation.z, newRotation.w);

            transform.rotation = finalRotation;
        }
        else
        {
            transform.rotation = targetTransform.rotation;
        }

        if(animateScale)
        {
            //SCALE
            Vector3 newScale = scaleDynamics.Update(Time.deltaTime, targetTransform.localScale, disableSimulation);
            transform.localScale = newScale;
        }
        else
        {
            transform.localScale = defaultTransform.localScale;
        }
    }
}