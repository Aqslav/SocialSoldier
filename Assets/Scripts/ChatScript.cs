using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using System.Text;

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
        text = SafeForTMP(chatText, text);
        LoadingIndicator?.SetActive(false);
        chars = text.ToCharArray();
        currentCharIndex = 0;
    }



    private string SafeForTMP(TextMeshProUGUI textBox, string text)
    {
        if (string.IsNullOrEmpty(text)) 
            return string.Empty;
        TMP_FontAsset font = textBox.font;
        var sb = new StringBuilder(text.Length);
        foreach (char c in text)
        {
            if (c == '\n' || c == '\r' || c == '\t')
            {
                sb.Append(c);
                continue;
            }
            if (char.IsSurrogate(c)) 
                continue;
            if (c == '\uFE0F' || c == '\uFE0E' || c == '\u200D' || c == '\u200B') 
                continue;
            if (font != null && font.HasCharacter(c, true))
                sb.Append(c);
        }

        return sb.ToString().Trim();
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
