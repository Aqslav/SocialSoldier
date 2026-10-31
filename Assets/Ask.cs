using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class Ask : Interactable
{
    static AIClient AI = new OpenRouterClient();
    public static string BuildPrompt(string preprompt, string question, string context)
    {
        return $"{preprompt}\n\nContext: {context}\n\nQuestion: {question}\n\nAnswer:";
    }
    public async static Task<string> SendRequest(string request)
    {
        return await AI.SendRequest(request);
    }
    public async static Task<string> AskQuestion(string preprompt, string question, string context)
    {
        string prompt = BuildPrompt(preprompt, question, context);
        return await SendRequest(prompt);
    }
}

public interface AIClient
{
    Task<string> SendRequest(string request);
}

public class OpenRouterClient : AIClient
{
    const string Url = "https://openrouter.ai/api/v1/chat/completions";

    public static string ApiKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY");
    public static string[] Models = new string[] 
    { 
        "cohere/north-mini-code:free",
        "nvidia/nemotron-3-ultra-550b-a55b:free",
        "nvidia/nemotron-3.5-lightning:free",
        "liquid/lfm-2.5-2.6b:free",
        "openrouter/free",
    };

    static readonly HttpClient http = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };

    [Serializable] class Message      { public string role; public string content; }
    [Serializable] class RequestBody  { public string model; public Message[] messages; }
    [Serializable] class Choice       { public Message message; }
    [Serializable] class ResponseBody { public Choice[] choices; }

    public async Task<string> SendRequest(string request)
    {
        foreach (var Model in Models)
        {
            try
            {
                string response = await TryRequest(request, Model);
                return response;
            }
            catch (Exception e)
            {
                Debug.LogError($"Request failed in {Model}: {e.Message}.");
            }
        }
        Debug.LogError("All models failed to respond.");
        return "Sorry, lets talk later";
    }
    public async Task<string> TryRequest(string request, string model)
    {
        if (string.IsNullOrEmpty(ApiKey))
            throw new Exception("OpenRouter API key is not set. Please set the OPENROUTER_API_KEY environment variable.");

        try
        {
            var body = new RequestBody
            {
                model = model,
                messages = new[] { new Message { role = "user", content = request } }
            };

            using var msg = new HttpRequestMessage(HttpMethod.Post, Url);
            msg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);
            msg.Content = new StringContent(JsonUtility.ToJson(body), Encoding.UTF8, "application/json");

            using var response = await http.SendAsync(msg).ConfigureAwait(false);
            string json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error: {(int)response.StatusCode}: {json}");
            }

            var parsed = JsonUtility.FromJson<ResponseBody>(json);
            if (parsed?.choices == null || parsed.choices.Length == 0)
                throw new Exception("Error: empty response.");

            return parsed.choices[0].message.content.Trim();
        }
        catch (Exception e)
        {
            throw new Exception($"OpenRouter request failed.{e.Message}");
        }
    }
}