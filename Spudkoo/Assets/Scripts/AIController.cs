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
    void Start()
    {
        messages = new List<Message>
        {
            new Message(Role.System,
            @"You are trying to convince people to star 
            off their phones while sounding like a conversation. 
            Make sure to give short responses and summaries unless 
            the person asks further. Consider what the person says
            and if it makes sense or not. Use helpful advice
            when the user asks questions, and speak like a human while
            doing so")
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

}
