using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using System.Collections.Generic;

public enum DodgeballTeam
{
    Blue = 0,
    Purple = 1
}

public class AgentDodge : Agent
{
    // Note that that the detectable tags are different for the blue and purple teams. The order is
    // * ball
    // * own goal
    // * opposing goal
    // * wall
    // * own teammate
    // * opposing player

    [HideInInspector]
    public DodgeballTeam team;
    float m_KickPower;
    // The coefficient for the reward for colliding with a ball. Set using curriculum.
    float m_BallTouch;
    //public Position position;

    const float k_Power = 2000f;
    float m_LateralSpeed;
    float m_ForwardSpeed;


    [HideInInspector]
    public Rigidbody agentRb;
    DodgeSettings m_DodgeSettings;
    BehaviorParameters m_BehaviorParameters;
    public Vector3 initialPos;
    public float rotSign;

    EnvironmentParameters m_ResetParams;

    public GameObject grabbedObject;
    private Stack<GameObject> inventory = new Stack<GameObject>();

    public float launchVelocity = 10f;
    public DodgeEnvController envController;

    public override void Initialize()
    {
        DodgeEnvController envController = GetComponentInParent<DodgeEnvController>();

        m_BehaviorParameters = gameObject.GetComponent<BehaviorParameters>();
        if (m_BehaviorParameters.TeamId == (int)DodgeballTeam.Blue)
        {
            team = DodgeballTeam.Blue;
            initialPos = new Vector3(transform.position.x - 5f, .5f, transform.position.z);
            rotSign = 1f;
        }
        else
        {
            team = DodgeballTeam.Purple;
            initialPos = new Vector3(transform.position.x + 5f, .5f, transform.position.z);
            rotSign = -1f;
        }
        m_LateralSpeed = 0.2f;
        m_ForwardSpeed = 0.2f;
        m_DodgeSettings = FindObjectOfType<DodgeSettings>();
        agentRb = GetComponent<Rigidbody>();
        agentRb.maxAngularVelocity = 500;

        m_ResetParams = Academy.Instance.EnvironmentParameters;
        inventory.Clear();
    }


    public void MoveAgent(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        m_KickPower = 0f;

        var forwardAxis = act[0];
        var rightAxis = act[1];
        var rotateAxis = act[2];
        var shoot = act[3];

        switch (forwardAxis)
        {
            case 1:
                dirToGo = transform.forward * m_ForwardSpeed;
                m_KickPower = 1f;
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

        switch (shoot)
        {
            case 1:
                shootball();
                break;
        }

        transform.Rotate(rotateDir, Time.deltaTime * 100f);
        agentRb.AddForce(dirToGo * m_DodgeSettings.agentRunSpeed, ForceMode.VelocityChange);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        MoveAgent(actionBuffers.DiscreteActions);
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
        if (Input.GetKey(KeyCode.Space))
        {
            discreteActionsOut[3] =1;
        }
    }

    void OnCollisionEnter(Collision c)
    {
        var force = k_Power * m_KickPower;
        if (c.gameObject.CompareTag("ball"))
        {
            AddReward(0.0625f);
            if (c.gameObject.GetComponent<Dodgeball>().canpickup == 1)
            {
                pickupball(c.gameObject);
            }
        }
    }

    public override void OnEpisodeBegin()
    {
        m_BallTouch = m_ResetParams.GetWithDefault("ball_touch", 0);
    }

    public void pickupball(GameObject grabbedObject)
    {
        if (grabbedObject != null) {
            grabbedObject.SetActive(false);
            inventory.Push(grabbedObject);
        }
    }

    public void shootball()
    {
        if (inventory.Count == 0) {
            return;
        }
        var ball = inventory.Pop();
        ball.SetActive(true);
        ball.transform.position = this.transform.position+(transform.forward*2);
        ball.transform.rotation = this.transform.rotation;

        AddReward(0.0625f);
        
        ball.GetComponent<Rigidbody>().AddRelativeForce(new Vector3 (0, 0, 4000f));
        ball.GetComponent<Dodgeball>().area = (GameObject.Find("Game Environment"));
        if (gameObject.tag == "blueAgent")
        {
            ball.GetComponent<Dodgeball>().SetState(Dodgeball.BallState.blue);
        }
        else if (gameObject.tag == "purpleAgent")
        {
            ball.GetComponent<Dodgeball>().SetState(Dodgeball.BallState.purple);
        }
    }

    public void resetInventory()
    {
        inventory.Clear();
    }
}
