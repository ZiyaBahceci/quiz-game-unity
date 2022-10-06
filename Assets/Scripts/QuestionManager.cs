using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

public class QuestionManager : MonoBehaviour
{
    public static int questionsNumber;
    public static bool answerPending;

    public TMP_Text questionsTitleText;
    public TMP_Text questionsText;
    public TMP_Text displayButtonText;
    public TMP_Text timeText;

    public List<AnswerButton> answerButtons;

    private int remainingTime = 60;

    private Coroutine timeCoroutine;

    public UnityEvent onTimeEnded, onQuestionsEnded;

    public void Start()
    {
        StartCoroutine(GetToken());
    }

    public void Init()
    {
        if (questionsNumber == 10)
        {
            onQuestionsEnded?.Invoke();
            return;
        }
        StartCoroutine(GetQuestion());
        
        // if(timeCoroutine != null) StopCoroutine(timeCoroutine);
        if(timeCoroutine == null) timeCoroutine = StartCoroutine(ResetTimer());
        
        displayButtonText.text = "Display Answer";
    }

    public void ClearTimerRoutine()
    {
        if(timeCoroutine != null) StopCoroutine(timeCoroutine);
        timeCoroutine = null;
    }

    IEnumerator ResetTimer()
    {
        remainingTime = 60;
        timeText.text = "60";
        while (remainingTime > 0)
        {
            yield return new WaitForSeconds(1);
            remainingTime--;
            timeText.text = remainingTime.ToString();
        }
        
        onTimeEnded?.Invoke();
    }

    IEnumerator GetToken()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get("https://opentdb.com/api_token.php?command=request"))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if(webRequest.result == UnityWebRequest.Result.Success)
            {
                var json = webRequest.downloadHandler.text;
                var token = JObject.Parse(json).SelectToken("token");

                if (token != null)
                {
                    PlayerPrefs.SetString("SessionToken", token.ToString());
                }
                else
                {
                    PlayerPrefs.DeleteKey("SessionToken");
                }
            }
        }
    }

    IEnumerator GetQuestion()
    {
        var selectedCategory = PlayerPrefs.GetString("category", "all");
        var uri = "https://opentdb.com/api.php?amount=1";
        if (selectedCategory != "all" || !string.IsNullOrEmpty(selectedCategory))
        {
            uri += "&category=" + selectedCategory;
        }

        var token = PlayerPrefs.GetString("SessionToken", "");
        if (token != "")
        {
            uri += $"&token={token}";
        }
        
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if(webRequest.result == UnityWebRequest.Result.Success)
            {
                var json = webRequest.downloadHandler.text;
                var des = JObject.Parse(json).SelectToken("results")?.First;

                var res = des?.SelectToken("question");
                var correct = des?.SelectToken("correct_answer");
                var wrongs = des?.SelectToken("incorrect_answers");
                var wrong1 = wrongs?.First;
                var wrong2 = wrong1?.Next;
                var wrong3 = wrong2?.Next;

                questionsTitleText.text = $"Question #{questionsNumber + 1}";
                
                answerButtons = answerButtons.OrderBy((i) => Random.value).ToList();

                answerButtons[0].Init(true, correct?.ToString());
                answerButtons[1].Init(false, wrong1?.ToString());
                answerButtons[2].Init(false, wrong2?.ToString());
                answerButtons[3].Init(false, wrong3?.ToString());
                
                questionsText.text = res?.ToString();
            }
        }
    }

    public void ResetNumber()
    {
        QuestionManager.questionsNumber = 0;
    }
    
    public void ShowCorrect()
    {
        answerButtons.Find((i) => i.isCorrect).button.onClick?.Invoke();
    }

    public async Task PendAnswer(bool init)
    {
        answerPending = true;
        await Task.Delay(3000);
        answerPending = false;
        if(init) Init();
    }
}
