# Cybersecurity Awareness ChatBot

This is a console-based chatbot that greets the user with a recorded voice message
and an ASCII-art header, then has a personalised text conversation about
basic cybersecurity topics (passwords, phishing, safe browsing), with input
validation and a styled, coloured console interface.

## Project structure, accoring to the POE document.
CybersecurityAwarenessBot
|-- Program.cs
|-- Chatbot.cs
|-- VoiceGreeting.cs
|-- AsciiArt.cs
|-- ResponseHandler.cs
|-- UserProfile.cs
|
|-- Audio
| -- greetings.wav | |-- Assets | -- ascii-logo.txt (optional copy of the ASCII design)
|
`-- README.md

## How to run the project
1. Open the project in Visual Studio 2022/2026 as a Console App (.NET) project.
2. Make a minimum of six meaningful commite reflecting real progress.
	-Initial commit
	-The added ASCII art
	-Added personalised text greeting and the user interaction
USERPROFILE
-This implements the basic cybersecurity response system
RESPONSEHANDLER
-Added input validation
-Fixed build, that includes target framework, and added System.Windows.Extensions. etc.
3. The confirmation to the GitHub Actions CI workflow completes with a green check mark
after each push

## Database note
Not applicable to this part — no database is used or required.
