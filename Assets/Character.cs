using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public struct Message
{
    public string role;
    public string content;

    public Message(string role, string content)
    {
        this.role = role;
        this.content = content;
    }
    public override string ToString()
    {
        return $"{role}: {content}";
    }
}
public class Character : MonoBehaviour
{
    private ChatScript chatScript;
    public string preprompt;
    public string postprompt;
    public string characterName = "You";
    public List<Message> chatHistory = new List<Message>();
    public int maxHistoryLength = 25;
    const string userRole = "User";

    void Start()
    {
        chatScript = GetComponentInChildren<ChatScript>(true);
        if (chatScript == null)
        {
            Debug.LogError($"ChatScript component not found on {gameObject.name}, it is required for Character to function properly.");
        }
    }
    async void SetResponce(string text)
    {
        string response = await Ask.SendRequest(text);
        chatScript.SetText(response);
        chatHistory.Add(new Message(characterName, response));
    }
    public void Say(string text)
    {
        int count = Mathf.Min(25, chatHistory.Count);
        List<Message> lastMessages = chatHistory.GetRange(chatHistory.Count - count, count);
        string formattedText = $"{preprompt}\n\nYour recent dialogue:\n{string.Join("\n", lastMessages)}\n\n{userRole} said: {text}\n\n{postprompt}";
        chatHistory.Add(new Message(userRole, text));
        chatScript.PutInLoad();
        SetResponce(formattedText);
    }
    void Update()
    {
        
    }
}
