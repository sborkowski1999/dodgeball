using UnityEngine;

public class DodgeballController : MonoBehaviour
{
    public GameObject area;
    [HideInInspector]
    public DodgeEnvController envController;
    [HideInInspector]
    public enum ball_state
    {
        neutral = 0,
        purple = 1,
        blue = 2
    }
    private ball_state curr_state;
    public Material purpleMaterial;
    public Material blueMaterial;
    public Material neutralMat;
    public string purpleAgent; //will be used to check if collided with purple goal ///// TO FIX
    public string blueAgent; //will be used to check if collided with blue goal
    public string wall;
    public string ball;

    void Start()
    {
        envController = area.GetComponent<DodgeEnvController>();
        curr_state = ball_state.neutral;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag(purpleAgent)) //ball touched purple agent
        {
            if(curr_state == ball_state.blue)
            {
                envController.PlayerHit(DodgeballTeam.Blue, col.gameObject); ////////// change GoalTouched to player hit or something
            }else if(curr_state == ball_state.neutral)
            {
                curr_state = ball_state.purple;
                GetComponent<Renderer>().material = purpleMaterial;
            }
        }
        else if (col.gameObject.CompareTag(blueAgent)) //ball touched blue agent
        {
            if(curr_state == ball_state.purple)
            {
                envController.PlayerHit(DodgeballTeam.Purple, col.gameObject);
            }else if(curr_state == ball_state.neutral)
            {
                curr_state = ball_state.blue;
                GetComponent<Renderer>().material = blueMaterial;
            }
        }
        else if (col.gameObject.CompareTag(wall) || col.gameObject.CompareTag(ball))
        {
            curr_state = ball_state.neutral;
            GetComponent<Renderer>().material = neutralMat;
        }
    }

    
}
