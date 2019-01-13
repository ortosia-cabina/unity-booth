using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CI.HttpClient;
using System.Net;
using Newtonsoft.Json;
using System;
using UnityEngine.SceneManagement;
using System.Linq;

public class LoginManagerAndTeleporter : MonoBehaviour {

    [SerializeField]
    public string serverURL;

    [SerializeField]
    private Transform destination;
    public HttpClient client;
    public string token
    {
        get
        {
            return token_;
        }
        set
        {
            token_ = value;
            if (value != null && value != "")
                getOwnUserId();
            else
                thisActorId = -1;
        }
    }
    private string token_;
    [SerializeField]
    private Transform errorPanel;
    [SerializeField]
    private GameObject setPasilloNv2;
    [SerializeField]
    private GameObject pasilloNv1;

    private List<Voting> votacionesPuedeVotar = null;
    public int thisActorId
    {
        get { return thisActorId_; }
        set { thisActorId_ = value;
            if (value == -1)
            {
                votacionesPuedeVotar = null;
            }
            else
            {
                getVotingsCanVote();
            }
        }
    }
    private int thisActorId_;

    // Use this for initialization
    void Start () {
		client = new HttpClient();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public List<Voting> votaciones;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            commenceProcess(other.gameObject);
        }
    }

    public void receiveLogin(HttpResponseMessage response)
    {
        try
        {
            Debug.Log(response.ReadAsString());
            var values = JsonUtility.FromJson<DecideResponse>(response.ReadAsString());
            Debug.Log(values.token);
            this.token = values.token;
            if (token == "" || token == null)
                indicateError("Estas credenciales no son válidas");
            Debug.Log(values.non_field_errors.Count>0?values.non_field_errors[0]:"nada de nada");

        }
        catch (System.ArgumentNullException)
        {
            indicateError("El servidor no está activo en este momento");
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void indicateError(string error)
    {
        StartCoroutine(indicateErrorCoroutine(error, 2));
    }

    private void indicateError(string error, float time)
    {
        StartCoroutine(indicateErrorCoroutine(error, time));
    }

    private IEnumerator indicateErrorCoroutine(string error, float time)
    {
        FindObjectOfType<FocusTogglerAndPauser>().canAlternateNormally = false;
        FindObjectOfType<FocusTogglerAndPauser>().hasFocus = false;
        errorPanel.gameObject.SetActive(true);
        errorPanel.Find("Text").GetComponent<UnityEngine.UI.Text>().text = error;
        yield return new WaitForSecondsRealtime(time);
        errorPanel.gameObject.SetActive(false);
        FindObjectOfType<FocusTogglerAndPauser>().canAlternateNormally = true;
        FindObjectOfType<FocusTogglerAndPauser>().hasFocus = true;
    }

    public void makeLogin(string username, string password)
    {
        client.Post(new System.Uri(serverURL+"/authentication/login/"), new StringContent("{\"username\":\""+username+"\", \"password\":\""+password+"\"}", System.Text.Encoding.UTF8, "application/json"), HttpCompletionOption.AllResponseContent, receiveLogin);
    }

    private void refreshRooms(Voting[] votings)
    {
        votaciones = new List<Voting>(votings);
        var etsiiInterior = GameObject.Find("ETSII_Interior");
        Vector3 rightSideLv2Position = new Vector3(-0.23579f, 0.00732f, -0.201f);
        Vector3 leftSideLv2Position = new Vector3(0.24123f, 0.00732f, -0.201f);
        Vector3 leftSideLv1Position = new Vector3(0.1093345f, 0f, 0.04105011f);
        Vector3 rightSideLv1Position = new Vector3(-0.1106655f, 0f, 0.04105011f);
        float horizontalDistance = 0.26259f;
        float longitudinalDistance = 0.2751f;
        bool isRightOrLeft = false;
        int distanceFromCenter = 0;
        int distanceFromBottom = 0;
        foreach (var item in new List<Voting>(votings).OrderBy(o => o.id).ToList())
        {
            var pasillo = Instantiate(pasilloNv1, etsiiInterior.transform);
            pasillo.transform.localScale = new Vector3(isRightOrLeft ? 1 : -1, 1, 1);
            if (isRightOrLeft)
            {
                pasillo.transform.localPosition = new Vector3(rightSideLv1Position.x - horizontalDistance * distanceFromCenter, rightSideLv1Position.y, rightSideLv1Position.z);
                distanceFromCenter += 1;
            }
            else
            {
                pasillo.transform.localPosition = new Vector3(leftSideLv1Position.x + horizontalDistance * distanceFromCenter, leftSideLv1Position.y, leftSideLv1Position.z);
                foreach(var texto in pasillo.transform.GetComponentsInChildren<TMPro.TextMeshPro>())
                {
                    texto.transform.localScale = new Vector3(-texto.transform.localScale.x, texto.transform.localScale.y, texto.transform.localScale.z);
                }
            }
            pasillo.transform.Find("GeneralSign").Find("TextMeshPro").GetComponent<TMPro.TextMeshPro>().text = "Votación:\n" + item.name +"\nID: "+item.id;
            bool hasUnlockables = false;
            foreach (var item_ in item.questions)
                foreach(var item__ in item_.options)
                    if (item__.unlockquestion.Count > 0)
                        hasUnlockables = true;
            if (hasUnlockables)
            {
                pasillo.transform.Find("fermat").Find("Cube").Find("TextMeshPro").GetComponent<TMPro.TextMeshPro>().text = "Tipo de votación no soportado";
            }else if (checkCanVote(item.id))
            {
                pasillo.transform.Find("fermat").gameObject.SetActive(false);
                bool isRightSide=false;
                PasilloController current = null;
                GameObject pasillo2 = null;
                pasillo.GetComponent<VotacionController>().votacionId = item.id;
                foreach (var i in item.questions)
                {
                    if (!isRightSide)
                    {
                        pasillo2 = Instantiate(setPasilloNv2, etsiiInterior.transform);
                        pasillo2.transform.localScale = new Vector3(1, 1, 1);
                        pasillo2.transform.localPosition = new Vector3(rightSideLv2Position.x - horizontalDistance * (distanceFromCenter-1), rightSideLv2Position.y, rightSideLv2Position.z-(distanceFromBottom)*longitudinalDistance);
                        pasillo.GetComponent<VotacionController>().ballotBoxes.Add(pasillo2.transform.Find("aula").Find("TABLE_Folding").Find("Ballot box N070817").GetComponent<BallotBox>());
                        current = pasillo2.GetComponent<PasilloController>();
                        current.votacion = item;
                        current.preguntaIzda = i;
                    }
                    else
                    {
                        pasillo.GetComponent<VotacionController>().ballotBoxes.Add(pasillo2.transform.Find("aula (1)").Find("TABLE_Folding (1)").Find("Ballot box N070817").GetComponent<BallotBox>());
                        current.preguntaDcha = i;
                        Debug.Log("Derecha: " + i.desc);
                        distanceFromBottom++;
                    }
                    isRightSide = !isRightSide;
                }
            }
            isRightOrLeft = !isRightOrLeft;
        }
    }

    private bool checkCanVote(int id)
    {
        foreach(var vot in votacionesPuedeVotar)
        {
            if (vot.id == id)
                return true;
        }
        return false;
    }

    public void getVotings()
    {
        client.Get(new Uri(serverURL+"/voting/"), HttpCompletionOption.AllResponseContent, r => {
            string s = "";
            try
            {
                s = "{\"Items\":" + r.ReadAsString() + "}";
            }
            catch (System.ArgumentNullException)
            {
                indicateError("El servidor no está activo en este momento");
                return;
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
                return;
            }
            refreshRooms(JsonHelper.FromJson<Voting>(s));
        });
    }

    public void getOwnUserId()
    {
        client.Headers.Add("Authorization", "Token "+token);
        client.Post(new Uri(serverURL + "/authentication/getuser/"), new StringContent("{\"token\":\""+token+"\"}", System.Text.Encoding.UTF8, "application/json"), HttpCompletionOption.AllResponseContent, r => {
            string s = "";
            try
            {
                User u = JsonUtility.FromJson<User>(r.ReadAsString());
                thisActorId = u.id;
            }
            catch (System.ArgumentNullException)
            {
                indicateError("El servidor no está activo en este momento");
                return;
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
                return;
            }
        });
    }

    public void getVotingsCanVote()
    {
        client.Get(new Uri(serverURL + "/census/voter/"+thisActorId+"/"), HttpCompletionOption.AllResponseContent, r => {
            string s = "";
            try
            {
                s = "{\"Items\":" + r.ReadAsString() + "}";
                Debug.Log(s);
            }
            catch (System.ArgumentNullException)
            {
                indicateError("El servidor no está activo en este momento");
                return;
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
                return;
            }
            votacionesPuedeVotar = new List<Voting>(JsonHelper.FromJson<Voting>(s));
        });
    }

    public void makeLogout()
    {
        client.Post(new System.Uri(serverURL+"/authentication/logout/"), new StringContent("", System.Text.Encoding.UTF8, "application/json"), HttpCompletionOption.AllResponseContent, null);
        token = null;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void commenceProcess(GameObject player)
    {
        //FindObjectOfType<FocusTogglerAndPauser>().canAlternateNormally = false;
        player.transform.position = destination.position;
    }
}

[System.Serializable]
public struct DecideResponse
{
    public string token;
    public List<string> non_field_errors;
}

[System.Serializable]
public class Voting
{
    public int id;
    public string name;
    public string desc;
    public List<Question> questions;
    public string start_date;
    public string end_date;
    public DateTime startTime
    {
        get
        {
            return DateTime.Parse(start_date);
        }
    }
    public DateTime endTime
    {
        get
        {
            return DateTime.Parse(end_date);
        }
    }
}

[System.Serializable]
public class Question
{
    public string desc;
    public List<Option> options;
    public override bool Equals(object obj)
    {
        var item = obj as Question;

        if (item == null)
        {
            return false;
        }

        return options.Count.Equals(item.options.Count) && desc.Equals(item.desc);
    }

    public override int GetHashCode()
    {
        return desc.GetHashCode();
    }
}

[System.Serializable]
public struct Option
{
    public int number;
    public string option;
    public List<Question> unlockquestion;
    public override bool Equals(object obj)
    {
        if (!(obj is Option))
            return false;

        Option mys = (Option)obj;
        return mys.number.Equals(number) && mys.option.Equals(option);
    }

    public override int GetHashCode()
    {
        return number.GetHashCode();
    }
}

[System.Serializable]
public struct User
{
    public int id;
    public string username;
    public string first_name;
    public string last_name;
    public string email;
    public bool is_staff;
}

public static class JsonHelper
{

    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = UnityEngine.JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return UnityEngine.JsonUtility.ToJson(wrapper);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}