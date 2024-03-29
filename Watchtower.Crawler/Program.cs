﻿using System;
using Watchtower.Data;
using Watchtower.Crawler;

class Program
{
    static void Main(string[] args)
    {
        // @TODO: Check for the -h / --help command line flag
        if (args.Contains("-h") || args.Contains("--help")) {
            Console.WriteLine("This is a crawler.");
            Console.WriteLine("Usage: Watchtower.Crawler.exe [options]");
            Console.WriteLine("Options:");
            Console.WriteLine("-f, --force\t\tForce the crawler to run even if the last run was less than the threshold (Default 1 hour).");
            Console.WriteLine("-h, --help\t\tDisplay this help message.");
            Console.WriteLine("-n, --no-upload\tDo not upload the results to the database.");
            return;
        }

        if (args.Contains("-h") || args.Contains("--help")) {
            Console.WriteLine("This is a crawler.");
            return;
        }

        // Fetch credentials
        // Get current directory
        var currentDirectory = System.IO.Directory.GetCurrentDirectory();
        var filename = $"{currentDirectory}/mongodb.txt";

        Console.WriteLine($"Looking for credentials in {filename}...");

        if (!System.IO.File.Exists(filename))
        {
            Console.WriteLine("No credentials file was found. Exiting.");
            System.Environment.Exit(-1);
        }

        string connectionString = System.IO.File.ReadAllText(filename);

        var crawler = new Crawler(connectionString);

        // Get hosts
        crawler.LoadHosts();
        // For every host, get resources
        var filteredHosts = crawler.Hosts.Where(x => !x.Ignore);
        var hostCount = filteredHosts.Count();
        Console.WriteLine($"Found {hostCount} {(hostCount == 1 ? "host" : "hosts")}. {crawler.Hosts.Count() - hostCount} hosts were ignored.");
        foreach (var host in filteredHosts)
        {
            host.FetchResources();
            var resources = host.Resources.Where(x => !x.Ignore);
            var resourceCount = resources.Count();
            var ignoredCount = host.Resources.Count() - resourceCount;
            Console.Write($"Found {resourceCount} resource{(resourceCount == 1 ? "" : "s")} @ {host.hostname} .");
            Console.WriteLine($"{ignoredCount} resource{(ignoredCount == 1 ? "" : "s")} were ignored.");
            // For every resource:
            // - Find last request

            foreach (var resource in resources)
            {
                Console.Write($"{resource.method} {host.hostname}{resource.path}... ");
                bool canPoll = crawler.canCrawl(resource);
                
                // TODO: Check for command line flags.
                if (!canPoll && (!args.Contains("-f") && !args.Contains("--force")))
                {
                    // - Return if too short time has passed since last execution
                    Console.WriteLine("Polled recently.");
                    continue;
                }
                // - ping it
                Console.WriteLine("Polling.");
                WatchtowerRequest response;
                response = crawler.Poll(host, resource).Result;

                Console.WriteLine($"Got {response.Status} ({response.ResponseTime} ms), expected {resource.expectedStatus} ({resource.expectedResponseTime} ms).");

                // - upload the result
                if (args.Contains("-n") || args.Contains("--no-upload"))
                {
                    Console.WriteLine("Skipping upload.");
                }
                else
                {
                    Console.Write("Uploading the result...");
                    try
                    {
                        crawler.uploadResponse(response);
                        Console.WriteLine("OK.");
                    }
                    catch
                    {
                        Console.WriteLine("Fail.");
                    }
                }
            }
        }
    }
}
