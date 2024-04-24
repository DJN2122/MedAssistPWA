using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MedAssistPWA.Services;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using PdfSharpCore.Fonts;
using MedAssistPWA.Models;
namespace MedAssistPWA.Pages
{
    public partial class MedAssistantChat
    {
        [Inject]
        public IJSRuntime JS { get; set; }

        [Inject]
        public OpenAiService OpenAiService { get; set; }

        [Inject]
        public PdfService pdfService { get; set; }

        [Inject]
        public FontServices FontService { get; set; }

        public string? query { get; set; }
        public List<string> UserMessages { get; set; } = new List<string>();
        public List<string> AiMessages { get; set; } = new List<string>();
        public string? newAiResponse { get; set; }


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var dotNetRef = DotNetObjectReference.Create(this);
                // Initialize the speech to text functionality
                await JS.InvokeVoidAsync("initializeSpeechToText", dotNetRef);

                // Load fonts and set the global font resolver
                Fonts font = await FontService.LoadFonts();
                try
                {
                    GlobalFontSettings.FontResolver = new CustomFontResolver(font);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message.ToString());
                }

                // Execute the typewriter effect on the specified text element
                try
                {
                    var text = "MedAssist Chat: Instant Health Insights!";
                    var typingSpeed = 50;
                    var deletionSpeed = 100;
                    await JS.InvokeVoidAsync("typeWriter", "typewriterText", text, typingSpeed, deletionSpeed);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while initializing the typewriter effect: {ex.Message}");
                    // Optionally, handle the error more specifically if needed
                }
            }
        }

        public async Task TriggerCallSpeechToText()
        {
            query = "Recording..."; // Set the placeholder text
            StateHasChanged(); // Immediately trigger UI update to show the placeholder text

            var result = await CallSpeechToText();
            if (!string.IsNullOrEmpty(result))
            {
                await UpdateChatWithResult(result);
            }
            else
            {
                query = ""; // Optionally clear the placeholder if no result was obtained
                StateHasChanged(); // Update the UI if needed
            }
        }


        // Adjusting the Blazor call to handle promise properly.
        public async Task<string?> CallSpeechToText()
        {
            try
            {
                var result = await JS.InvokeAsync<string>("SpeechToTextFromMic");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }



        public async Task UpdateChatWithResult(string text)
        {
            UserMessages.Add(text);
            query = text; // Bind the recognized text to the query string.
            StateHasChanged(); // Notify Blazor that the component state has changed to re-render the UI.
        }

        private async Task<string> HandleSendButtonClick()
        {
            var aiResponse = string.Empty;
            var Query = string.Empty;

            //model training 
            if (string.IsNullOrEmpty(query))
            {
                Query = query +
                "\n\nNote: You are a virtual medical assistant of 'MedAssist' website. Act as Professional Medical assistant and advise user regarding their health concerns and while keeping replies short and simple with only write 5-6 words per line." +
                "\n\nYou must take on the role similar to that of a registered nurse, doctor, or medical assistant. Initiate the dialogue with a warm welcome, inviting users to share their health concerns today. Use this introduction: \"Welcome to MedAssist. How may I assist you with your health concerns today?\" As you proceed, collect comprehensive information about the user's symptoms. Inquire about the duration, severity, accompanying symptoms, and any self-treatment attempts. It is crucial to refrain from providing direct diagnoses; your primary goal is to thoroughly understand the context of the user's health issues before proceeding." +
                "\n\nNote: If the user's inquiry is unrelated to health and medical advice, such as asking about the weather or mathematics questions or any other unrelated topics, politely redirect them: 'I'm here to help with health-related questions. Can I assist you with another health issue?'";
            }
            else
            {
                Query = query +
                "\n\nNote: You are a virtual medical assistant of 'MedAssist' website. Act as Professional Medical assistant and advise user regarding their health concerns and while keeping replies short and simple with only write 5-6 words per line, have a normal human like convesation with the user keep in mind the users previous prompts" +
                "\n\nYou must take on the role similar to that of a registered nurse, doctor, or medical assistant. Initiate the dialogue with a warm welcome, inviting users to share their health concerns today. As you proceed, collect comprehensive information about the user's symptoms. Inquire about the duration, severity, accompanying symptoms, and any self-treatment attempts. It is crucial to refrain from providing direct diagnoses; your primary goal is to thoroughly understand the context of the user's health issues before proceeding." +
                "\n\nAlways clarify symptoms in detail. For example, if a user mentions a headache, ask: 'Can you tell me more about your headache? How long has it lasted, and have you noticed any other symptoms like vision problems, nausea or vomiting?'" +
                "\n\nRemember to tailor your responses to provide supportive and professional advice, and if and only if health issues seem non-urgent but unclear, suggest they consult a healthcare provider for a detailed evaluation." +
                "\n\nRemember to collect key metrics like age, weight, and any known allergies or chronic conditions that could be relevant to the user's symptoms. For instance, if a user reports fatigue and dizziness, ask: 'Could you share your age and whether you have any chronic conditions? Also, do you have any known allergies?'" +
                "\n\nIn addition to offering general health advice, if it seems that the user may benefit from local healthcare services, guide them to the 'Find Local Care & Fitness' feature on the 'info' page. For example, if a user is seeking a gym for physical therapy or a pharmacy to refill a prescription, direct them by saying: 'For finding the nearest hospital, gym, doctor's office or pharmacy, you can use our 'Find Local Care & Fitness' provider on the 'info' page of the MedAssist website for convenient options around you.'" +
                "\n\nAlways ensure that the advice you provide is in line with the best practices of patient care and triage. Encourage users to keep track of their symptoms and, when in doubt, to consult with a healthcare provider directly for personalized medical advice." +
                "\n\nFor serious symptoms such as uncontrolled bleeding, severe chest pain, or difficulty breathing, immediately advise the user to seek emergency medical help. Suggest: 'Your symptoms sound serious and require immediate attention. Please call emergency services (112 or 999) or visit the nearest emergency room right away.'" +
                "\n\nNote: If the conversation drifts to non-medical topics, gently guide it back: 'I'm here to focus on your health. Let’s talk about any other health concerns you might have.'";
            }

            //getting ai response from open_ai
            aiResponse = await OpenAiService.GenerateResponseAsync(Query);
            
            //for showing user Responses in page
            UserMessages.Add(query);

            //for showing Ai Responses in page
            AiMessages.Add(aiResponse);

            //for converting text-to-speech
            newAiResponse = aiResponse;

            query = "";
            StateHasChanged();

            return aiResponse;
        }

        [JSInvokable]
        public async Task<string> ReturnPromptAsync(string prompt) { 
            query = prompt;
            var response = await HandleSendButtonClick();
            return response;
        }

        // for text-to-speech part
        string subscriptionKey = "INSERT AZURE API KEY HERE"; 
        string region = "westeurope"; 
        private string _base64EncodedAudio;

        private async Task TextToSpeech()
        {
            _base64EncodedAudio = "";
            StateHasChanged();
            // Endpoint URL
            string endpoint = $"https://{region}.tts.speech.microsoft.com/cognitiveservices/v1";

            // SSML content
            string ssml = $"<speak version='1.0' xml:lang='en-US'><voice xml:lang='en-US' xml:gender='Female' name='en-US-AvaMultilingualNeural'>{newAiResponse}</voice></speak>";

            // Create HttpClient
            using HttpClient client = new HttpClient();

            // Add headers

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            client.DefaultRequestHeaders.Add("Accept", "application/ssml+xml");
            client.DefaultRequestHeaders.Add("X-Microsoft-OutputFormat", "audio-16khz-128kbitrate-mono-mp3");
            client.DefaultRequestHeaders.Add("User-Agent", "curl");

            // Create HttpContent for the SSML data
            var content = new StringContent(ssml, Encoding.UTF8);

            // Send the POST request
            var response = await client.PostAsync(endpoint, content);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                // Read the response content as a byte array
                var audioBytes = await response.Content.ReadAsByteArrayAsync();
                _base64EncodedAudio = Convert.ToBase64String(audioBytes);
                Console.WriteLine($"Status code: {response.StatusCode} :" + audioBytes.Length);
            }
            else
            {
                // Handle the error appropriately
                throw new Exception($"Failed to convert text to speech. Status code: {response.StatusCode}");
            }
        }

        private async Task GeneratePdfReport()
        {
            var memoryStream = pdfService.GeneratePdf(UserMessages, AiMessages);



            using (var streamRef = new DotNetStreamReference(stream: memoryStream))
            {
                await JS.InvokeVoidAsync("saveAsFile", "ChatReport.pdf", streamRef);
            }
        }
    }

}

