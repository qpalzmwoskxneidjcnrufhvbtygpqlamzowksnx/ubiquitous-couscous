
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices;
using System;



public class TestController : MonoBehaviour
{
    private Dictionary<int, QuestionAnswer> questionsAndAnswers = new();
    private List<string> questions;

    [SerializeField] private TMP_Text questionDisplay;
    [SerializeField] private TMP_InputField inputField;


    private int currentQuestionNumber;
    public event Action<string> OnTestOver;
    private void Awake()
    {
        currentQuestionNumber = 0;

        var file = Resources.Load<TextAsset>("Questions/questions");

        if (file != null)
        {
            // Splits the text by any common line-ending format and removes empty lines
            questions = new List<string>(file.text.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.RemoveEmptyEntries));
            
            Debug.Log($"Loaded {questions.Count} questions.");
        }
        else
        {
            Debug.LogError("Could not find questions.txt in the Resources/Questions folder!");
        }

        LoadFirstQuestion();
    }

    public void OnNextQuestionButtonPressed()
    {
        if(currentQuestionNumber >= questions.Count)
        {
            return; //TEST IS OVER
        }

        CleanupCurrentQuestion();
        LoadNextQuestion();
    
    }

    private void CleanupCurrentQuestion()
    {
        questionsAndAnswers[currentQuestionNumber] = new QuestionAnswer
        {
            question = questions[currentQuestionNumber],
            answer = inputField.text,
        };
        inputField.text = "";
        currentQuestionNumber++;
    }

    private void LoadNextQuestion()
    {
        if(currentQuestionNumber >= questions.Count)
        {
            QuestionsOver();
        }
        else
        {
            questionDisplay.text = questions[currentQuestionNumber];
        }
        
    }

    private void LoadFirstQuestion()
    {
        inputField.text = "";
        questionDisplay.text = questions[currentQuestionNumber];
    }

    private void QuestionsOver()
    {
        string completeString = FormCompleteString();
        OnTestOver?.Invoke(completeString);
    }

    private string FormCompleteString()
    {
        string doneString = "";
        foreach(var key in questionsAndAnswers.Keys)
        {
            doneString += $"{key}:: Q:{questionsAndAnswers[key].question} | A:{questionsAndAnswers[key].answer}\n";
        }
        Debug.Log(doneString);
        return doneString;
    }



    
}

public struct QuestionAnswer
{
    public string question;
    public string answer;
}
