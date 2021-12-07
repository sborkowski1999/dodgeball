using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class DodgeEnvController : MonoBehaviour
{
    [System.Serializable]
    public class PlayerInfo
    {
        public AgentDodge Agent;
        [HideInInspector]
        public Vector3 StartingPos;
        [HideInInspector]
        public Quaternion StartingRot;
        [HideInInspector]
        public Rigidbody Rb;
    }


    /// <summary>
    /// Max Academy steps before this platform resets
    /// </summary>
    /// <returns></returns>
    [Tooltip("Max Environment Steps")] public int MaxEnvironmentSteps = 25000;

    /// <summary>
    /// The area bounds.
    /// </summary>

    /// <summary>
    /// We will be changing the ground material based on success/failue
    /// </summary>

    public GameObject ball;
    //Need this to reset ball color on game reset.
    public Material neutralMaterial;
    [HideInInspector]
    public Rigidbody ballRb;
    Vector3 m_BallStartingPos;

    //List of Agents On Platform
    public List<PlayerInfo> AgentsList = new List<PlayerInfo>();
    private List<GameObject> outAgents = new List<GameObject>();

    private DodgeSettings m_DodgeSettings;


    private SimpleMultiAgentGroup m_BlueAgentGroup;
    private SimpleMultiAgentGroup m_PurpleAgentGroup;

    private int m_ResetTimer;

    private int bluescore;
    private int purplescore;

    void Start()
    {
        m_DodgeSettings = FindObjectOfType<DodgeSettings>();
        // Initialize TeamManager
        m_BlueAgentGroup = new SimpleMultiAgentGroup();
        m_PurpleAgentGroup = new SimpleMultiAgentGroup();
        ballRb = ball.GetComponent<Rigidbody>();
        m_BallStartingPos = new Vector3(ball.transform.position.x, ball.transform.position.y, ball.transform.position.z);
        bluescore = 0;
        purplescore = 0;
        foreach (var item in AgentsList)
        {
            item.StartingPos = item.Agent.transform.position;
            item.StartingRot = item.Agent.transform.rotation;
            item.Rb = item.Agent.GetComponent<Rigidbody>();
            if (item.Agent.team == DodgeballTeam.Blue)
            {
                m_BlueAgentGroup.RegisterAgent(item.Agent);
            }
            else
            {
                m_PurpleAgentGroup.RegisterAgent(item.Agent);
            }
        }
        ResetScene();
    }

    void FixedUpdate()
    {
        m_ResetTimer += 1;
        if (m_ResetTimer >= MaxEnvironmentSteps && MaxEnvironmentSteps > 0)
        {
            m_BlueAgentGroup.GroupEpisodeInterrupted();
            m_PurpleAgentGroup.GroupEpisodeInterrupted();
            ResetScene();
        }
    }


    public void ResetBall()
    {
        var randomPosX = Random.Range(-2.5f, 2.5f);
        var randomPosZ = Random.Range(-2.5f, 2.5f);

        ball.transform.position = m_BallStartingPos + new Vector3(randomPosX, 0f, randomPosZ);
        ballRb.velocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;
        ball.GetComponent<Renderer>().material = neutralMaterial;
    }

    public void PlayerHit(DodgeballTeam scoredTeam, GameObject hitplayer)
    {
        if (scoredTeam == DodgeballTeam.Blue)
        {
            m_BlueAgentGroup.AddGroupReward(1 - (float)m_ResetTimer / MaxEnvironmentSteps); // should the reward be given when hit, or when game is over?
            m_PurpleAgentGroup.AddGroupReward(-1);
            bluescore++;
        }
        else
        {
            m_PurpleAgentGroup.AddGroupReward(1 - (float)m_ResetTimer / MaxEnvironmentSteps);
            m_BlueAgentGroup.AddGroupReward(-1);
            purplescore++;
        }
        hitplayer.SetActive(false);
        outAgents.Add(hitplayer);

        if(bluescore == 2 || purplescore == 2)
        {
            bluescore = 0;
            purplescore = 0;
            m_PurpleAgentGroup.EndGroupEpisode();
            m_BlueAgentGroup.EndGroupEpisode();
            ResetScene();
        }
    }
    public void ResetScene()
    {
        m_ResetTimer = 0;

        //Reset Agents
        foreach (var item in AgentsList)
        {
            item.Agent.transform.position = item.StartingPos;
            item.Agent.transform.rotation = item.StartingRot;
            item.Rb.velocity = Vector3.zero;
            item.Rb.angularVelocity = Vector3.zero;
        }

        foreach (GameObject agent in outAgents) {
            agent.SetActive(true);
        }
        outAgents.Clear();
        //Reset Ball
        ResetBall();
    }
}
