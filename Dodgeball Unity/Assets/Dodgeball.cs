using UnityEngine;

public class Dodgeball : MonoBehaviour
{
    public GameObject area;
    [HideInInspector]
    public DodgeEnvController envController;
    [HideInInspector]
    public enum BallState{
        neutral = 0,
        purple = 1,
        blue = 2
    }
    private BallState curr_state;
    public Material purpleMaterial;
    public Material blueMaterial;
    public Material neutralMat;
    public string purpleAgent; //will be used to check if collided with purple goal ///// TO FIX
    public string blueAgent; //will be used to check if collided with blue goal
    public string wall;
    public string ball;
    [HideInInspector]
    public Vector3 startingPosition; //Start position is saved for when game is reset.
    public int canpickup = 1;

    public GameObject owner;

    void Start()
    {
        envController = area.GetComponent<DodgeEnvController>();
        // curr_state = BallState.neutral;
        startingPosition = this.transform.localPosition;
    }

    void Update() { 
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("purpleAgent")) //ball touched purple agent.
        {
            if(curr_state == BallState.blue) {
                envController.PlayerHit(DodgeballTeam.Blue, col.gameObject);
                owner.GetComponent<AgentDodge>().AddReward(1);
            }
            else if(curr_state == BallState.neutral) {
                SetState(BallState.purple);
                owner = col.gameObject; // associate the ball with the agent that touched the ball
            }
        }

        else if (col.gameObject.CompareTag("blueAgent")) //ball touched blue agent.
        {

            if(curr_state == BallState.purple){
                owner.GetComponent<AgentDodge>().AddReward(1);
                envController.PlayerHit(DodgeballTeam.Purple, col.gameObject);
            }
            else if(curr_state == BallState.neutral) {
                SetState(BallState.blue);
                owner = col.gameObject; // associate the ball with the agent that touched the ball
            }
        }
        else if (col.gameObject.CompareTag(wall)) //ball touched wall.
        {
            SetState(BallState.neutral);
        }
    }

    public void SetState(BallState newState) {
        curr_state = newState;
        switch(newState) {
            case BallState.blue:
                curr_state = BallState.blue;
                GetComponent<Renderer>().material = blueMaterial;
                canpickup = 0;
                break;
            case BallState.purple:
                curr_state = BallState.purple;
                GetComponent<Renderer>().material = purpleMaterial;
                canpickup = 0;
                break;
            case BallState.neutral:
                curr_state = BallState.neutral;
                GetComponent<Renderer>().material = neutralMat;
                canpickup = 1;
                break;
        }
    }
}
