using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Votador : MonoBehaviour {

    public Image reticle;
    public bool isPaused
    {
        get
        {
            return Time.timeScale != 1;
        }
    }
    public Sprite[] reticleImages;
    public Voting votacion = null;
    public Question pregunta = null;
    public int answerIndex;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                GameObject objectHit = hit.transform.gameObject;
                if (objectHit.CompareTag("VotingBallot"))
                {
                    VotingBallot temp = objectHit.GetComponent<VotingBallot>();
                    this.votacion = temp.votacion;
                    this.answerIndex = temp.number;
                    this.pregunta = temp.pregunta;
                    Debug.Log("Has cogido la papeleta " + pregunta.options.Find(i => i.number == answerIndex).option + " para la pregunta " + pregunta.desc+ " de la votación "+votacion.desc);
                    transform.Find("VoteSource").GetComponent<AudioSource>().Play();
                }
                else if (objectHit.CompareTag("BallotBox"))
                {
                    BallotBox b = objectHit.GetComponent<BallotBox>();
                    if (b.pregunta.Equals(pregunta) && b.voting.id == votacion.id)
                    {
                        transform.Find("VoteSource").GetComponent<AudioSource>().Play();
                        Debug.Log("Vas a votar");
                        b.Vote(answerIndex);
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (isPaused)
        {
            reticle.enabled = false;
        }
        else
        {
            reticle.enabled = true;
        }
    }
}
