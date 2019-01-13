using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VotacionController : MonoBehaviour {

    public int votacionId;
    public List<BallotBox> ballotBoxes;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bool weGood = true;
            foreach (var item in ballotBoxes)
            {
                if (item.answer == null)
                    weGood = false;
            }
            if (weGood)
            {
                Debug.Log("Aquí se vota del tó");
                                                                                                //VOTAR
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
