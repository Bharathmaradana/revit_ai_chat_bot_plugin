//using Newtonsoft.Json;
//using System;
//using System.Net.Http;
//using System.Text;
//using System.Threading.Tasks;

//namespace ClassLibrary3
//{
//    public static class ChatService
//    {
//        private static string apiKey = "*";

//        public static async Task<string> Ask(string prompt)
//        {
//            Logger.Log("Calling OpenAI...");

//            using (var client = new HttpClient())
//            {
//                client.Timeout = TimeSpan.FromSeconds(30);

//                client.DefaultRequestHeaders.Clear();
//                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

//                var body = new
//                {
//                    model = "gpt-4.1-mini",
//                    input = prompt
//                };

//                var response = await client.PostAsync(
//                    "https://api.openai.com/v1/responses",
//                    new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json")
//                );

//                string result = await response.Content.ReadAsStringAsync();

//                Logger.Log("API Response: " + result);

//                if (!response.IsSuccessStatusCode)
//                    return "API Error: " + result;

//                dynamic json = JsonConvert.DeserializeObject(result);

//                try
//                {
//                    return json.output[0].content[0].text;
//                }
//                catch
//                {
//                    return result;
//                }
//            }
//        }
//    }
//}
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary3
{
    public static class ChatService
    {
        private static string apiKey = "*";

        public static async Task<string> Ask(string prompt)
        {
            Logger.Log("Calling Gemini (HTTP)...");

            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(30);

                    //  SAME endpoint as your JS
                    string url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-3-flash-preview:generateContent";

                    //  SAME payload structure
                    var payload = new
                    {
                        contents = new[]
                        {
                            new
                            {
                                parts = new[]
                                {
                                    new { text = prompt }
                                }
                            }
                        },
                        generationConfig = new
                        {
                            thinkingConfig = new
                            {
                                thinkingLevel = "low"
                            }
                        }
                    };

                    string json = JsonConvert.SerializeObject(payload);

                    Logger.Log("Request: " + json);

                    //  Header instead of query param (like your JS)
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("x-goog-api-key", apiKey);

                    var response = await client.PostAsync(
                        url,
                        new StringContent(json, Encoding.UTF8, "application/json")
                    );

                    string result = await response.Content.ReadAsStringAsync();

                    Logger.Log("Response: " + result);

                    if (!response.IsSuccessStatusCode)
                        return "Gemini API Error: " + result;

                    dynamic data = JsonConvert.DeserializeObject(result);

                    try
                    {
                        return data.candidates[0].content.parts[0].text;
                    }
                    catch
                    {
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Exception: " + ex.ToString());
                return "Error: " + ex.Message;
            }
        }
    }
}
