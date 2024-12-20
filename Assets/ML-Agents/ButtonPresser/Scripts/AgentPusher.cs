using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;

public class AgentPusher : Agent
{
    [SerializeField]
    Transform targetTransform;

    float m_LateralSpeed = 0.15f;
    float m_ForwardSpeed = 0.5f;

    float lastDistance = 0f;
    float newDistance = 0f;

    [HideInInspector]
    public Rigidbody agentRb;

    public GameEnvController envController;

    public override void Initialize()
    {
        agentRb = GetComponent<Rigidbody>();
        agentRb.maxAngularVelocity = 500;
    }

    public void MoveAgent(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;


        var forwardAxis = act[0];
        var rightAxis = act[1];
        var rotateAxis = act[2];

        switch (forwardAxis)
        {
            case 1:
                dirToGo = transform.forward * m_ForwardSpeed;
                break;
            case 2:
                dirToGo = transform.forward * -m_ForwardSpeed;
                break;
        }

        switch (rightAxis)
        {
            case 1:
                dirToGo = transform.right * m_LateralSpeed;
                break;
            case 2:
                dirToGo = transform.right * -m_LateralSpeed;
                break;
        }

        switch (rotateAxis)
        {
            case 1:
                rotateDir = transform.up * -1f;
                break;
            case 2:
                rotateDir = transform.up * 1f;
                break;
        }

        transform.Rotate(rotateDir, Time.deltaTime * 100f);
        agentRb.AddForce(dirToGo, ForceMode.VelocityChange);
    }

    public void SetTarget(Transform transform)
    {
        targetTransform = transform;
        lastDistance = DistanceToTarget();
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        MoveAgent(actionBuffers.DiscreteActions);
        newDistance = DistanceToTarget();

        if (IsMovingTowardsButton()) {
            AddReward(0.1f);
        } else if (IsOnButton()) {
            AddReward(1.0f);
            EndEpisode();
        } if (IsIdle()) {
            AddReward(-0.01f);
        }

        AddReward(-0.001f);
        lastDistance = newDistance;
    }

    private float DistanceToTarget()
    {
        return Vector3.Distance(transform.position, targetTransform.position);
    }

    private bool IsMovingTowardsButton()
    {
        return newDistance < lastDistance;
    }

    private bool IsOnButton()
    {
        return newDistance < 0.7f;
    }

    private bool IsIdle()
    {
        return GetComponent<Rigidbody>().velocity.magnitude < 0.1f;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        //forward
        if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2;
        }
        //rotate
        if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[2] = 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[2] = 2;
        }
        //right
        if (Input.GetKey(KeyCode.E))
        {
            discreteActionsOut[1] = 1;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            discreteActionsOut[1] = 2;
        }
    }

    public override void OnEpisodeBegin()
    {
        envController.ResetScene();
    }

}
