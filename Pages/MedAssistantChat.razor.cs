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
            if (query == "")
            {
                Query = query +
                $"\n\nYou are a medical assistant of 'MedAssist' website. Act as Professional Medical assistant and advise user regarding their problems and only write 7-8 words per line. Reply with 'Welcome to MedAssist, How may I help you?'." +
                "Note: And if user ask anything else other than Medical Advise or Medical Help like 'asking weather, talking about something whihc is not assosiated with Medical help' then respond with 'Sorry, I can't answer that. I can only help you with Medical problems.' or something better in professional manner.";
            }
            else
            {
                Query = query +
                $"\n\nYou are a medical assistant of 'MedAssist' website. Act as Professional Medical assistant and advise user regarding their problems and only write 7-8 words per line. Now Respond to user in professional and appropriate manner." +
                "Note: And if user ask anything else other than Medical Advise or Medical Help like 'asking weather, talking about something whihc is not assosiated with Medical help' then respond with 'Sorry, I can't answer that. I can only help you with Medical problems.' or something better in professional manner."; ;
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

