using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using SimpleJSON;

public class CohereAPI : MonoBehaviour
{
    private string apiKey = "your-api-key-here";
    private string url = "https://api.cohere.ai/v1/generate";
    private string model = "command-nightly";
    private int maxTokens = 200;
    private float temperature = 0.750f;

    // Assign these in the inspector
    public InputField inputPrompt;
    public Button sendButton;
    public Text answerText;

    void Start()
    {
        // Add a listener to the Button component to trigger the PostRequest function
        sendButton.onClick.AddListener(() => StartCoroutine(PostRequest(url, inputPrompt.text)));
    }

    IEnumerator PostRequest(string url, string prompt)
    {
        var request = new UnityWebRequest(url, "POST");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);
        request.SetRequestHeader("Content-Type", "application/json");

        GenerateRequestBody body = new GenerateRequestBody();
        body.model = model;
        body.prompt = prompt;
        body.max_tokens = maxTokens;
        body.temperature = temperature;

        string bodyJson = JsonUtility.ToJson(body);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(bodyJson);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
        }
        else
        {
            var jsonResponse = JSON.Parse(request.downloadHandler.text);
            string responseText = jsonResponse["generations"][0]["text"].Value;

            // Display the response in the AnswerText field
            answerText.text = responseText;
        }
    }
}

[System.Serializable]
public class GenerateRequestBody
{
    public string model;
    public string prompt;
    public int max_tokens;
    public float temperature;
}
