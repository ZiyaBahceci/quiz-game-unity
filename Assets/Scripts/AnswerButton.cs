using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AnswerButton : MonoBehaviour
{
    public bool isCorrect;

    public Image image;
    public QuestionManager questionManager;
    
    public Button button;
    public TMP_Text buttonText;
    public TMP_Text displayButtonText;
    public UnityEvent onCorrect, onWrong;

    public Color correctColor, neutralColor, wrongColor;

    public void Init(bool isCorrect, string text)
    {
        gameObject.SetActive(!string.IsNullOrEmpty(text));

        image.color = neutralColor;
        
        this.isCorrect = isCorrect;
        buttonText.text = text;
        
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        if (QuestionManager.answerPending) return;
        
        
        if (isCorrect)
        {
            onCorrect?.Invoke();
            image.color = correctColor;
            displayButtonText.text = "Correct";
                QuestionManager.questionsNumber++;

        }
        else
        {
            onWrong?.Invoke();
            image.color = wrongColor;
            questionManager.ShowCorrect();
            displayButtonText.text = "Wrong";
        
        }

        questionManager.PendAnswer(isCorrect);
    }
}