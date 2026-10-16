## STUDENT DETAILS
Student Number -> ST10499533
Name -> Lwandle Nokubonga Makhaza

# CybersecuritChatbotGUI
## PART 2 OF THE POE

This is a GUI chatbot that greets the user with a recorded voice message
and an ASCII-art header, then has a personalised text conversation about
basic cybersecurity topics (passwords, phishing, safe browsing), with input
validation and a styled, coloured console interface. The interaction that 
happens between the user and the computer,this project has more added concepts
to it. 

## Project structure, accoring to the POE document (adding the things needed for PART 2)
# THE STRUCTURE OF PART 2 OF THE POE
CybersecurityChatbotSolution
|
+-- CybersecurityChatbotPart1
| +-- Program.cs
| +-- VoiceGreeting.cs
| +-- ...
|
+-- CybersecurityChatbotGUI
	|-- Program.cs
	|-- ChatbotForm.cs
	|-- ChatbotForm.Designer.cs
	|-- ChatBot.cs
	|-- KeywordResponder.cs
	|-- MemoryStore.cs
	|-- SentimentDetector.cs
	|-- VoiceGreeting.cs
	|-- Assets
		| -- greetings.wav | |-- Assets | -- ascii-logo.txt (optional copy of the ASCII design)
	|
	`-- README.md

## FURTHER EXPLAINATION ON THE FILES
# File and Roles: 
| `Program.cs` -> Starts the application.
| `ChatbotForm.cs` / `.Designer.cs` -> The GUI: ASCII art header, chat window, input box, voice greeting.
| `ChatBot.cs` -> Controls the conversation: name capture, flow, memory, replies.
| `KeywordResponder.cs` -> Keyword recognition, random tips, delegates.
| `MemoryStore.cs` -> Remembers the user's name, favourite topic, last topic and mood.
| `SentimentDetector.cs` -> Detects worried, curious, frustrated, etc. and returns an empathetic reply.
| `VoiceGreeting.cs` -> Plays `Assets/greetings.wav` 

## EXPLAINING EVERYTHING ABOUT THIS PROJECT.
# The features: 
1. GUI -> The dark theme that contains bubbles, spacing and the ASCII art in the header.
The WAV greeting plays in the start.
2. Keyword recognition -> 5 Topics that the user has to ask (password, phishing, scam, privacy,
malware), each of their own gave several triggers.
3. Random responses -> Each of the lists has tips. The bot picks one at random and never
repeats the same tip twice.
4. Memory -> It stores your name and favourite topic. After "I am interested in privacy"
it adds a personalized line when you ask about privacy later.
5. Sentiment -> It detects the worried, curious, frustrated, scared and overwhelmed.
It replies with empathy and gives a tip immediately, so you don't ask again.

## How to run the project
1. Open the project in Visual Studio 2022/2026 as a Console App (.NET) project.
2. Go to file "CybersecurityChatbotGUI.csproj" press F5.
