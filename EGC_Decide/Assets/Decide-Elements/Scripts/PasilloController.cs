using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PasilloController : MonoBehaviour {

    [SerializeField]
    private BallotBox boxIzda;
    [SerializeField]
    private BallotBox boxDcha;

    [SerializeField]
    private TextMeshPro cartelIzda;
    [SerializeField]
    private TextMeshPro cartelDcha;
    [SerializeField]
    private GameObject puertaDcha;
    [SerializeField]
    private GameObject puertaIzda;
    [SerializeField]
    private GameObject[] mesasIzquierda;
    [SerializeField]
    private GameObject[] mesasDerecha;
    [SerializeField]
    private GameObject[] ballotsIzquierda;
    [SerializeField]
    private GameObject[] ballotsDerecha;

    public Voting votacion;

    private Question preguntaIzda_;
    private Question preguntaDcha_;

    public string textoIzda
    {
        set
        {
            cartelIzda.text = value;
        }
    }
    public string textoDcha
    {
        set
        {
            cartelDcha.text = value;
        }
    }
    public bool votableIzda
    {
        set
        {
            puertaIzda.SetActive(!value);
            boxIzda.voting = votacion;
            boxIzda.pregunta = preguntaIzda_;
        }
    }
    public bool votableDcha
    {
        set
        {
            puertaDcha.SetActive(!value);
            boxDcha.voting = votacion;
            boxDcha.pregunta = preguntaDcha_;
        }
    }

    public Question preguntaDcha
    {
        get
        {
            return preguntaDcha_;
        }
        set
        {
            if (value == null)
                votableDcha = false;
            else
            {
                preguntaDcha_ = value;
                cartelDcha.text = "Pregunta:\n"+value.desc;
                votableDcha = true;
                for (int i = 0; i < value.options.Count; i++)
                {
                    ballotsDerecha[i].GetComponent<VotingBallot>().votacion = votacion;
                    ballotsDerecha[i].GetComponent<VotingBallot>().pregunta = preguntaDcha_;
                    ballotsDerecha[i].GetComponent<VotingBallot>().number = value.options[i].number;
                    ballotsDerecha[i].transform.Find("Texto").Find("Texto proper").GetComponent<TMPro.TextMeshPro>().text = value.options[i].option;
                    ballotsDerecha[i].SetActive(true);
                }
            }
        }
    }

    public Question preguntaIzda
    {
        get
        {
            return preguntaIzda_;
        }
        set
        {
            if (value == null)
                votableIzda = false;
            else
            {
                preguntaIzda_ = value;
                cartelIzda.text = "Pregunta:\n" + value.desc;
                votableIzda = true;
                for (int i = 0; i < value.options.Count; i++)
                {
                    ballotsIzquierda[i].GetComponent<VotingBallot>().votacion = votacion;
                    ballotsIzquierda[i].GetComponent<VotingBallot>().pregunta = preguntaIzda_;
                    ballotsIzquierda[i].GetComponent<VotingBallot>().number = value.options[i].number;
                    ballotsIzquierda[i].transform.Find("Texto").Find("Texto proper").GetComponent<TMPro.TextMeshPro>().text = value.options[i].option;
                    ballotsIzquierda[i].SetActive(true);
                }
            }
        }
    }


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
