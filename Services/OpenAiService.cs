using System.Collections.Generic;
using System.Threading.Tasks;
using OpenAI_API;
using OpenAI_API.Chat;

namespace MedAssistPWA.Services
{
    public class OpenAiService
    {
        private readonly OpenAIAPI _openAiApi;
        private List<ChatMessage> _chatHistory = new List<ChatMessage>(); // Store chat history

        public OpenAiService(string apiKey)
        {
            _openAiApi = new OpenAIAPI(apiKey); // Initialize the OpenAI API client with your API key
        }

        public async Task<string> GenerateResponseAsync(string inputText)
        {
            // Append the new user input to the chat history
            _chatHistory.Add(new ChatMessage
            {
                Role = ChatMessageRole.User,
                TextContent = inputText
            });

            var chatRequest = new ChatRequest
            {
                Model = "gpt-3.5-turbo", // Use the appropriate model for chat
                Messages = _chatHistory, // Use the accumulated chat history for context
                Temperature = 0.9, // Adjust based on desired creativity
                MaxTokens = 1000, // Adjust based on the maximum length of the response you want
                TopP = 1.0,
                FrequencyPenalty = 0,
                PresencePenalty = 0,
            };

            // Send the chat completion request and await the response
            var response = await _openAiApi.Chat.CreateChatCompletionAsync(chatRequest);

            // Update the chat history with the AI's response
            if (response.Choices.Count > 0)
            {
                _chatHistory.Add(new ChatMessage
                {
                    Role = ChatMessageRole.Assistant,
                    TextContent = response.Choices[0].Message.TextContent.Trim()
                });
            }

            // Return the AI's response text
            return response.Choices.Count > 0 ? response.Choices[0].Message.TextContent.Trim() : string.Empty;
        }

        // Optionally, provide a method to reset the chat history for a new story or game session
        public void ResetChatHistory()
        {
            _chatHistory.Clear();
        }
    }
}
