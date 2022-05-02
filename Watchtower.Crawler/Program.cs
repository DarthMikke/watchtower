using System;
using Watchtower.Data;
using Watchtower.Crawler;

class Program {
    static void Main(string[] args) {
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

        // Get hosts
        crawler.LoadHosts();
        // For every host, get resources
        var hostCount = crawler.hosts.Count();
        Console.WriteLine($"Found {hostCount} {(hostCount == 1 ? "host" : "hosts")}.");
        foreach (var host in crawler.hosts)
        {
            var resources = host.Resources(crawler.client);
            var resourceCount = resources.Count();
            Console.WriteLine($"Found {resourceCount} resource{(resourceCount == 1 ? "" : "s")} @ {host.hostname} .");
            // For every resource:
            // - Find last request

            foreach (var resource in resources)
            {
                Console.Write($"{resource.method} {host.hostname}{resource.path}... ");
                bool canPoll = crawler.canCrawl(resource);

                if (!canPoll)
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
            }
        }
    }
}
