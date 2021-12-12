using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using System.Linq;
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


    //List of Agents On Platform
    public List<PlayerInfo> AgentsList = new List<PlayerInfo>();
    private List<GameObject> outAgents = new List<GameObject>(); //Keep track of which agents are out to add them again after reset.
    public List<GameObject> balls = new List<GameObject>();
    public StageColor stageColorer;

    private SimpleMultiAgentGroup m_BlueAgentGroup;
    private SimpleMultiAgentGroup m_PurpleAgentGroup;

    public int m_ResetTimer;

    private int bluescore = 0;
    private int purplescore = 0;

    void Start()
    {
        // Initialize TeamManager
        m_BlueAgentGroup = new SimpleMultiAgentGroup();
        m_PurpleAgentGroup = new SimpleMultiAgentGroup();

        //Save agents starting position and rotation for resetting later. Also set their team.
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

    public void ResetBalls()
    {
        foreach (GameObject ball in balls) {
            ball.SetActive(true);
            ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
            ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            ball.GetComponent<Dodgeball>().SetState(Dodgeball.BallState.neutral);
            ball.GetComponent<Dodgeball>().canpickup = 1;
            ball.transform.localPosition = ball.GetComponent<Dodgeball>().startingPosition;
        }
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

        if(bluescore >=2 || purplescore >= 2)
        {
            stageColorer.SetWinner(bluescore >= 2 ? StageColor.DodgeballWinner.blue : StageColor.DodgeballWinner.purple);
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
            item.Agent.resetInventory();
        }
        foreach (GameObject agent in outAgents) {
            agent.SetActive(true);
        }
        outAgents.Clear();
        /*When last agent gets hit and dies, it still imparts force on ball. Adding 0.1 seconds of delay 
        here means we wait for the force to be over and then stop momentum in ResetBalls method. 
        Otherwise ResetBalls wouldn't have any affect.*/
        Invoke("ResetBalls",0.1f);
    }

}
