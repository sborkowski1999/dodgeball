using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageColor : MonoBehaviour
{
    public GameObject purpleSide;
    public GameObject blueSide;
    public Material winMaterial;
    public Material loseMaterial;

    public enum DodgeballWinner{blue,purple}
    public void SetWinner(DodgeballWinner winner) {
        purpleSide.GetComponent<Renderer>().enabled = true;
        blueSide.GetComponent<Renderer>().enabled = true;
        if (winner == DodgeballWinner.blue) {
            blueSide.GetComponent<Renderer>().material = winMaterial;
            purpleSide.GetComponent<Renderer>().material = loseMaterial;
        }
        else {
            purpleSide.GetComponent<Renderer>().material = winMaterial;
            blueSide.GetComponent<Renderer>().material = loseMaterial;
        }
        Invoke("TurnOffColour", 1f);
    }

    private void TurnOffColour() {
        purpleSide.GetComponent<Renderer>().enabled = false;
        blueSide.GetComponent<Renderer>().enabled = false;
    }
}
