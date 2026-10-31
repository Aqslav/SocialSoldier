using UnityEngine;
using TMPro;
using System.Threading.Tasks;
public class ChatScript : MonoBehaviour
{
    TextMeshProUGUI chatText;
    GameObject LoadingIndicator;
    public float typingSpeed = 0.02f;
    private float typingTimer = 0f;
    private char[] chars;
    private int currentCharIndex = 0;
    void Start()
    {
        chatText = GetComponent<TextMeshProUGUI>();
        LoadingIndicator = transform.Find("LoadingIndicator").gameObject;
        if (LoadingIndicator == null)
            Debug.LogWarning("LoadingIndicator not found as a child of ChatScript.");
    }
    public void PutInLoad()
    {
        LoadingIndicator?.SetActive(true);
        chatText.text = "";
        chars = null;
        currentCharIndex = 0;
    }
    public void SetText(string text)
    {
        LoadingIndicator?.SetActive(false);
        chars = text.ToCharArray();
        currentCharIndex = 0;
    }
    void Update()
    {
        if (chars != null && currentCharIndex < chars.Length)
        {
            typingTimer += Time.deltaTime;
            if (typingTimer >= typingSpeed)
            {
                typingTimer = 0f;
                chatText.text += chars[currentCharIndex];
                currentCharIndex++;
            }
        }
    }
}
