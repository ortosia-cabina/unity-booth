using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallotBox : MonoBehaviour {

    public Question pregunta;
    public Voting voting;
    public int? answer = null;

    public void Vote(int answer)
    {
        this.answer = answer;
        Debug.Log("Has votado " + answer);
    }
}
