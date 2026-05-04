using System.Threading.Tasks;

namespace ClassLibrary3
{
    public static class ChatProcessor
    {
        public static async Task<string> ProcessAsync(string input, string data)
        {
            Logger.Log("Processing with LLM");

            string prompt = $@"
You are a Revit assistant.

User question:
{input}

Revit data:
{data}

Answer clearly. No JSON.
";

            return await ChatService.Ask(prompt);
        }
    }
}