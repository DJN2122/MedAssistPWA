![MedAssist Logo](MedAssist-logo-poppins.png)

# MedAssist

## Overview

MedAssist is a personal project designed to bridge the gap between advanced artificial intelligence and practical healthcare applications. It's an AI medical assistant web application developed using Blazor WebAssembly. The application incorporates OpenAI's language models and Google Places API to provide accurate medical consultations and local healthcare information. This platform facilitates instant, AI-driven responses to medical queries and helps users locate nearby healthcare services like hospitals and gyms.

## Key Features

- **Interactive AI Chat**: Engage in conversations with MedAssist about health concerns, receiving immediate AI-driven advice.
- **Speech-to-Text and Text-to-Speech**: Utilizes Azure Speech Services for hands-free communication, enhancing accessibility for all users.
- **Generate Medical Report**: Allows users to generate a downloadable PDF report of their conversation with MedAssist for personal record or professional review.
- **Local Healthcare Provider Locator**: Using Google Places API, it provides real-time location data to help find nearby hospitals, doctors, pharmacies, and gyms.

## How to Run the Project in Visual Studio

### Prerequisites

- Visual Studio 2019 or later with the ASP.NET and web development workload installed.
- .NET 6.0 SDK (downloaded and installed during Visual Studio setup).
- An active subscription or access to OpenAI services and Google Places API for the complete functionality.

## Configuration Instructions

After forking the project, it is essential to configure the application with your own API keys to enable all features fully. Here's how you can set up each component:

- **OpenAI API Key**:
   - Open the `Program.cs` file.
   - Locate line 19 and replace `"INSERT OPENAI API KEY HERE"` with your actual OpenAI API key. The line looks like this:
     ```csharp
     builder.Services.AddSingleton(new OpenAiService("INSERT OPENAI API KEY HERE"));
     ```

- **Azure Speech Services**:
   - Open the `MedAssistantChat.razor.cs` file.
   - Navigate to line 154 and insert your Azure subscription key in place of `"INSERT OPENAI API KEY HERE"`. Here’s how it appears:
     ```csharp
     string subscriptionKey = "INSERT OPENAI API KEY HERE";
     ```
   - Additionally, update the `speech-to-text.js` file:
     - Find line 3 and replace `"INSERT OPENAI API KEY HERE"` with your subscription key. Make sure the `serviceRegion` is set correctly (e.g., `"westeurope"`):
     ```javascript
     var subscriptionKey = "INSERT OPENAI API KEY HERE", serviceRegion = "westeurope";
     ```

- **Google Places API Key**:
   - Open the `index.html` file.
   - On line 35, replace the placeholder `"INSERT OPENAI API KEY HERE"` with your Google Places API key in the script source URL:
     ```html
     <script src="https://maps.googleapis.com/maps/api/js?key=INSERT OPENAI API KEY HERE&callback=initMap" async defer></script>
     ```

Please ensure that all API keys are correctly inserted to take full advantage of MedAssist’s capabilities, from AI-powered interactions to local healthcare service mapping.


### Running the Application

1. **Open the Project**:
   - Open Visual Studio.
   - Select 'Open a project or solution'.
   - Navigate to the downloaded MedAssist solution folder and open the `.sln` file.

2. **Run the Application**:
   - Build the project by selecting 'Build Solution' from the 'Build' menu.
   - Run the application by clicking the 'Run' button or pressing F5.

## Important Notes:
- Ensure that the API keys are kept secure and not exposed publicly.
- Review the API quotas and limitations associated with your accounts to prevent service interruptions.

## Conclusion

MedAssist showcases how integrating AI with healthcare can significantly enhance patient care and operational efficiency. By leveraging cutting-edge technologies, it provides a platform that simplifies medical inquiries and facilitates access to healthcare services, ensuring that critical health information is just a few clicks away. If you have any questions please feel free to contact me! 
