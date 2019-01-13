using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CI.HttpClient;
using System;

public class VotacionController : MonoBehaviour {

    public int votacionId;
    public List<BallotBox> ballotBoxes;
    private bool hasVoted = false;
    [SerializeField]
    private GameObject fermat;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasVoted)
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
                Vote();                                                                                 //VOTAR
            }
        }
    }

    public void Vote()
    {
        HttpClient client = FindObjectOfType<LoginManagerAndTeleporter>().client;
        string serverURL = FindObjectOfType<LoginManagerAndTeleporter>().serverURL;
        string token = FindObjectOfType<LoginManagerAndTeleporter>().token;
        int thisActorId = FindObjectOfType<LoginManagerAndTeleporter>().thisActorId;
        Debug.Log("{\"voting_id\":" + votacionId + ", \"voter_id\":" + thisActorId + ", \"a\":" + votacionId + ", \"b\":" + votacionId + "}");
        client.Post(new Uri(serverURL + "/store/"), new StringContent("{\"voting_id\":" + votacionId + ", \"voter_id\":" + thisActorId + ", \"vote\":[\"a\":" + votacionId + ", \"b\":" + votacionId + "]}", System.Text.Encoding.UTF8, "application/json"), HttpCompletionOption.AllResponseContent, r => {
            try
            {
                r = Validate(r);
                if (r.IsSuccessStatusCode)
                {
                    hasVoted = true;
                    fermat.SetActive(true);
                    Debug.Log("Votado con éxito");
                }
                else
                {
                throw new InvalidOperationException("Fallo al votar. Código de error: " + r.StatusCode + ", mensaje: " + r.ReadAsString());
                }
            }
            catch (System.ArgumentNullException)
            {
                FindObjectOfType<LoginManagerAndTeleporter>().indicateError("El servidor no está activo en este momento");
                return;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return;
            }
        });
    }

    private HttpResponseMessage Validate(HttpResponseMessage r)
    {
        r.StatusCode = System.Net.HttpStatusCode.Accepted;
        return r;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
