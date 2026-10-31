using UnityEngine;
using TMPro;
using System.Threading.Tasks;
public class ChatScript : MonoBehaviour
{
    TextMeshProUGUI chatText;
    GameObject LoadingIndicator;
    bool textGenerated = false;
    void Start()
    {
        chatText = GetComponent<TextMeshProUGUI>();
        LoadingIndicator = transform.Find("LoadingIndicator").gameObject;
        if (LoadingIndicator == null)
            Debug.LogWarning("LoadingIndicator not found as a child of ChatScript.");
    }
    async void UpdateChatText(string preprompt, string question, string context)
    {
        chatText.text = await Ask.AskQuestion(preprompt, question, context);
        LoadingIndicator?.SetActive(false);
    }
    public void Generate()
    {
        textGenerated = true;
        LoadingIndicator?.SetActive(true);
        UpdateChatText("You are really bitchy woman who hates the user.", "Hi, how is it going?", "You walk around the park and you meet your school friend");
    }
    public void Clear()
    {
        textGenerated = false;
        if(chatText != null)
            chatText.text = "";
        LoadingIndicator?.SetActive(false);
    }
    void Update()
    {
        if(!textGenerated)
        {
            Generate();
        }
    }
}
