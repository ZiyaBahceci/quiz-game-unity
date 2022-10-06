using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class CategoryLister : MonoBehaviour
{
    public CategoryButton categoryButtonPrefab;

    public GameObject categoryView;
    public QuestionManager questionManager;
    
    void Start()
    {
        StartCoroutine(GetRequest("https://opentdb.com/api_category.php"));
    }

    IEnumerator GetRequest(string uri)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if(webRequest.result == UnityWebRequest.Result.Success)
            {
                var json = webRequest.downloadHandler.text;
                var categories = JsonConvert.DeserializeObject<Dictionary<string, 
                    List<Dictionary<string,string>>>>(json)["trivia_categories"];
                foreach (var cat in categories)
                {
                    var inst = Instantiate(categoryButtonPrefab, transform);
                    inst.Setup(cat["name"], cat["id"], OnClick);
                }
            }
            
        }
    }

    void OnClick()
    {
        categoryView.SetActive(false);
        questionManager.gameObject.SetActive(true);
        questionManager.Init();
    }
}
