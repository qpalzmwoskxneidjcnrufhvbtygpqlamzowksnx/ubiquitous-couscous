using UnityEngine;
using OpenAI;
using TMPro;
using OpenAI.Chat;
using OpenAI.Models;
using System.Collections.Generic;
using UnityEngine.UI;
public class AIController : MonoBehaviour
{
    OpenAIClient openAI;
    List<Message> messages;
    public TMP_InputField inputField;
    public TMP_Text outputText;
    [SerializeField] private TestController testController;
    void Start()
    {
        testController.OnTestOver += HandleTestOver;
        messages = new List<Message>
        {
            new Message(Role.System,
            @"You are trying to convince people to star 
            off their phones while sounding like a conversation. 
            Make sure to give short responses and summaries unless 
            the person asks further. Consider what the person says
            and if it makes sense or not. Use helpful advice
            when the user asks questions, and speak like a human while
            doing so.  Remember, you are a buddy, a being within the computer 
            that acts and looks correspondant to what the user wants in the
            test.  Be like a companion with the user, and change your personality
            and actions slowly throughout the user's interactions with you.
            You will be given a long list of test questions and answers from a user.
            Please look over the questions and answers, and change your personality based on what you think
            would be the most beneficial for this person.")
        };
        openAI = new OpenAIClient();
        inputField.onEndEdit.AddListener( (text) =>
        {
            SubmitChat(text);
        }
        );
    }
    async void SubmitChat(string input)
    {
        Debug.Log("ChatSubmitted");
        messages.Add(new Message(Role.User,inputField.text));
        var chatAttempt = new ChatRequest(messages,Model.GPT4_Turbo,maxTokens: 50);
        var response = await openAI.ChatEndpoint.GetCompletionAsync(chatAttempt);
        messages.Add(new Message(Role.Assistant,response.FirstChoice));
        outputText.text = response.FirstChoice;
    }

    async void SubmitTestResults(string input)
    {
        Debug.Log("Submitted Test Results");
        messages.Add(new Message(Role.User,input));
        var chatAttempt = new ChatRequest(messages,Model.GPT4_Turbo,maxTokens: 50);
        var response = await openAI.ChatEndpoint.GetCompletionAsync(chatAttempt);
        messages.Add(new Message(Role.Assistant,response.FirstChoice));
    }

    private void HandleTestOver(string testResults)
    {
        
        SubmitTestResults(testResults);
    }

}
