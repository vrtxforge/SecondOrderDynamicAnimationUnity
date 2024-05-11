using UnityEngine;

public class ProceduralAnimationController : MonoBehaviour
{
    public Transform target;
    private Transform targetTransform;

    public bool animatePosition;

    public Vector3 positionConstants;

    [HideInInspector]
    public bool disableSimulation;

    private SecondOrderDynamics movementDynamics;


    private void Start()
    {
        // Initialize SecondOrderDynamics with appropriate values
        Vector3 initialPosition = transform.position;
        movementDynamics = new(positionConstants.x, positionConstants.y, positionConstants.z, initialPosition);

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
    }
}