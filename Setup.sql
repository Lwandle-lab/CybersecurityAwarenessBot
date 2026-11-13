
-- Cybersecurity Awareness Bot (Part 3): database setup script.
-- The app creates the database and table automatically on start-up, but you can also run
-- this script yourself in MySQL Workbench or the mysql command line to create them manually.

CREATE DATABASE IF NOT EXISTS cybersecurity_chatbot;
USE cybersecurity_chatbot;

CREATE TABLE IF NOT EXISTS Tasks (
    TaskId       INT AUTO_INCREMENT PRIMARY KEY,
    Title        VARCHAR(150) NOT NULL,
    Description  VARCHAR(500) NOT NULL,
    ReminderDate DATETIME NULL,
    IsCompleted  TINYINT(1) NOT NULL DEFAULT 0,
    CreatedAt    DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Optional sample data
-- INSERT INTO Tasks (Title, Description, ReminderDate)
-- VALUES ('Enable two-factor authentication',
--         'Turn on two-factor authentication so a stolen password alone can''t unlock your account.',
--         DATE_ADD(CURDATE(), INTERVAL 3 DAY));