using System;
using Watchtower.Data;
using Watchtower.Crawler;

// Fetch credentials
// Get current directory
var currentDirectory = System.IO.Directory.GetCurrentDirectory();
var filename = $"{currentDirectory}/mongodb.txt";

Console.WriteLine($"Looking for credentials in {filename}...");

if (!System.IO.File.Exists(filename)) {
    Console.WriteLine("No credentials file was found. Exiting.");
    System.Environment.Exit(-1);
}

string connectionString = System.IO.File.ReadAllText(filename);

var crawler = new Crawler(connectionString);

// Find last request
// Return if too short time has passed since last execution
// Get hosts
// For every host, get resources
// For every resource:
// - ping it
// - upload the result
