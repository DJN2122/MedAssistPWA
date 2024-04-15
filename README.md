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

After forking the project, you will need to configure the application with your own API keys to fully enable all its features:

- **OpenAI API Key**:
   - Navigate to `Program.cs`.
   - Replace `"INSERT OPENAI API KEY HERE"` with your actual OpenAI API key in the OpenAiService registration line.

- **Azure Speech Services**:
   - For speech-to-text and text-to-speech functionality, update the Azure subscription key in `MedAssistantChat.razor.cs` and the `speech-to-text.js` file with your key.

- **Google Places API Key**:
   - For the local healthcare provider locator feature, replace the placeholder in the `index.html` with your Google Places API key.

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
