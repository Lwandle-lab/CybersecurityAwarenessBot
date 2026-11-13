## STUDENT DETAILS
Student Number -> ST10499533
Name -> Lwandle Nokubonga Makhaza

## CYBERSECURITY AWARENESS BOT (PART 3 - FINAL POE)

# Project Overview
The Cybersecurity Awareness Bot is a Window Forms desktop application developed in C# using .NET.
The chatbot educates users about cybersecurity while provideing task management, reminders, quiz, 
fuctionality, activity tracking, and natural language interaction.

Part 3 expands the functionality from Parts 1 and 2 by introducing:
- MySQL database integration
- Task management
- Quiz functionality
- Activity logging
- Natuaral Language Processing (NLP)
- Improved object-oriented architecture

## Features
# Cybersecurity Awareness

The chatbot educates users on important cybersecurity topics: 
- Password security
- Phishing attacks 
- Social engineering 
- Online scams
- Privacy protection
- Safe browsing
- Two-factor authentication (2FA)

Examples Commands
	- What is phishing
	- Tell me about password safety
	- How can I browse safely?

## Quiz Functionality
The chatbot also includes a security quiz system that allows users to test their knowledge.

--- Features ---
- Multiple-choice questions
- True/False questions
- Randomized questions
- Score tracking 
- Feedback after completion

Example Command
"start quiz"

The system keeps track of:
- Number of the questions answered
- User score
- Final performance feedback

## Activity Log
The chatbot records user interactions during a session.

Example Commands
- what have you done for me
- activity log
- show log 
- recent actions

## Natural Language Processing (NLP)
The chatbot uses NLP techniques to understand different command variations.

Examples:

- Quiz
- start quiz
- quiz
- test my knowledge
- Activity Log
- activity log
- what have you done for me
- recent actions

--- The NLP Engine uses:---
	- Keyword detection
	- Regular expressions
	- String manipulation
	- Time phrase recognition

## Time Phrase Recognition
The chatbot can identify and process natural date expressions such as:

today
tomorrow
next week
next month
Friday
in 3 days

## Task Management
# Task management functionality is implemented through a dedicated MySQL repository.

--- Features include ---
	- Creating tasks
	- Viewing tasks
	- Completing tasks
	- Deleting tasks
	- Managing reminders
	- Task data is stored persistently in a MySQL database.

## Database Integration
The application uses MySQL and MySqlConnector.

The database is automatically created if it does not exist.

--- Database Name ---
cybersecurity_chatbot

--- Tasks Table ---
Tasks

--- Database Fields ---
Field			Description
TaskId			Unique task identifier
Title			Task title
Description		Task description
ReminderDate	Reminder date and time
IsCompleted		Completion status
CreatedAt		Date created

## Object-Oriented Design
The application follows object-oriented design principles and separation of concerns.

# ChatBot
--- Responsible for ----
- Processing user input
- Generating chatbot responses
- Managing quiz interactions

# ChatbotForm
--- Responsible for---
- Graphical User Interface (GUI)
- Displaying messages
- User interaction

# QuizService
--- Responsible for ---

- Managing quiz sessions
- Question selection
- Score tracking
- Final feedback generation

# QuizQuestion
Represents an individual quiz question including:
- Question text
- Available answers
- Correct answer
- Explanation

# ActivityLog
Responsible for:
- Recording chatbot activities
- Storing timestamps
- Displaying recent actions

# NlpEngine
Responsible for:
- Intent detection
- Command recognition
- Time phrase identification
- Title extraction

# TaskRepository
Responsible for:
- All MySQL database operations
- CRUD functionality
- Task persistence

# DatabaseSettings
Responsible for:
- Database configuration
- Connection string management
- TaskItem
- Represents a task object within the application.

## Technologies Used
	- C#
	-.NET Windows Forms
	- MySQL
	- MySqlConnector
	- Object-Oriented Programming (OOP)
	- Natural Language Processing (NLP)
	- Visual Studio 2022

## Running the Application
	- Open the solution in Visual Studio.
	- Restore NuGet packages.
	- Ensure MySQL Server is running.
	- Configure database settings in:
appsettings.local.json

	- Build the solution.
	- Run the application.

## Example Commands
# Cybersecurity Questions

- What is phishing?
- Tell me about password safety

--- Quiz ---
start quiz

--- Activity Log ---
what have you done for me

--- Help ---
help

## GitHub Repository
The project is maintained using GitHub and includes:
	- Source code
	- Commit history
	- Project documentation
	- Final Part 3 submission

## How to Run the Project
1. Open the Project
- Open the solution file (.sln) in Visual Studio 2022.
- To run the project press F5 or Ctrl + F5
