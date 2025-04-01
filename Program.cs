using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Mail;
using System.Diagnostics; // Add this for SMTP debugging

namespace AzureCostTracker
{
    class Program
    {
        // Azure AD application credentials
        private static string clientId = "clientd id";
        private static string tenantId = "input your tenant id";
        private static string subscriptionId = "also input your subscription id";

        /// Main method - entry point of the application.It retrieves an access token, fetches daily Azure cost data, and sends an email.
        static async Task Main(string[] args)
        {
            try
            {
                string token = await GetAccessTokenAsync();// Get authentication token

                Console.WriteLine("\nAccess Token:\n");// Display the access token
                Console.WriteLine(token);
                Console.WriteLine("\n---------------------\n");

                // Fetch daily cost details from Azure
                string costData = await GetDailyCostAsync(token);

                // Send email with the cost data
                await SendEmailAsync(
                    toEmail: "gabrielfrancisa@gmail.com",
                    ccEmails: new[] { "gabrielfrancisa@gmail.com", "gabrielfrancisa@gmail.com" }, // Team emails
                    subject: "Azure Cost Report",
                    body: costData
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        /// Retrieves an OAuth 2.0 access token for authenticating requests to Azure API.
        /// Uses interactive login to prompt the user for authentication.
        /// <returns>Access token as a string</returns>
        private static async Task<string> GetAccessTokenAsync()
        {
            // Create a public client application for authentication
            IPublicClientApplication app = PublicClientApplicationBuilder.Create(clientId)
                .WithAuthority(new Uri($"https://login.microsoftonline.com/{tenantId}"))
                .WithRedirectUri("http://localhost") // Redirect URI for interactive login
                .Build();

            // Define the scope required for API access
            string[] scopes = { "https://management.azure.com/user_impersonation" }; // Delegated permission

            // Prompt user for authentication and obtain token
            AuthenticationResult result = await app.AcquireTokenInteractive(scopes).ExecuteAsync();

            return result.AccessToken;// Return the access token
        }

        /// <summary>
        /// Calls the Azure Consumption API to fetch daily cost details.
        /// <param name="token">Access token for authentication</param>
        /// <returns>Formatted cost data as a string</returns>
        private static async Task<string> GetDailyCostAsync(string token)
        {
            using (HttpClient client = new HttpClient())
            {
                // Set Authorization header with Bearer token
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Azure API endpoint for fetching usage details
                string url = $"https://management.azure.com/subscriptions/{subscriptionId}/providers/Microsoft.Consumption/usageDetails?api-version=2021-10-01";

                HttpResponseMessage response = await client.GetAsync(url);// Send GET request to Azure API

                if (response.IsSuccessStatusCode)
                { 
                    string jsonResponse = await response.Content.ReadAsStringAsync();// Read and parse JSON response
                    JObject parsedResponse = JObject.Parse(jsonResponse);

                    string costData = FormatCostData(parsedResponse);// Format cost data for email
 
                    DisplayCosts(parsedResponse);// Display cost details
 
                    return costData;// Return formatted cost data
                }
                else
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();// Print error details if request fails
                    Console.WriteLine($"Error: {response.StatusCode}");
                    Console.WriteLine($"Details: {errorResponse}");
                    return "Error fetching cost data.";
                }
            }
        }

        /// Parses and displays the daily cost data retrieved from the Azure API.
        /// <param name="parsedResponse">JSON object containing the cost details</param>
        private static void DisplayCosts(JObject parsedResponse)
        {
            Console.WriteLine("Date\t\tCost");
            Console.WriteLine("----------|------------");

            if (parsedResponse["value"] == null || !parsedResponse["value"].HasValues)// Check if the response contains cost data
            {
                Console.WriteLine("No cost data found.");
                return;
            }

            foreach (var item in parsedResponse["value"]!)// Iterate through cost data entries
            {

                string? date = item["properties"]?["usageStart"]?.ToString()?.Split('T')[0]; // Extract date and cost from response
                string? cost = item["properties"]?["pretaxCost"]?.ToString();

                if (date != null && cost != null)// Print valid cost data or show error if missing
                {
                    Console.WriteLine($"{date}\t{cost}");
                }
                else
                {
                    Console.WriteLine("Invalid or missing data in response.");
                }
            }
        }

        /// Formats the cost data into a readable string for the email body.
        /// <param name="parsedResponse">JSON object containing the cost details</param>
        /// <returns>Formatted cost data as a string</returns>
        private static string FormatCostData(JObject parsedResponse)
        {
            var formattedData = new System.Text.StringBuilder();
           
            if (parsedResponse["value"] == null || !parsedResponse["value"].HasValues) // Check if the response contains cost data
            {
                return "No cost data found.";
            }

            foreach (var item in parsedResponse["value"]!)// Iterate through cost data entries
            {
                string? date = item["properties"]?["usageStart"]?.ToString()?.Split('T')[0];// Extract date and cost from response
                string? cost = item["properties"]?["pretaxCost"]?.ToString();

                if (date != null && cost != null)// Append valid cost data to the formatted string
                {
                    formattedData.AppendLine($"{date}\t{cost}");
                }
            }

            return formattedData.ToString();
        }

        /// Sends an email with the specified subject and body to the recipient and CC emails.
        private static async Task SendEmailAsync(string toEmail, string[] ccEmails, string subject, string body)
        {
            try
            { 
                var fromAddress = new MailAddress("gabrielfrancisa@gmail.com", "Azure Cost Tracker");// Sender's email address and name

                var toAddress = new MailAddress(toEmail);// Recipient's email address

                // Gmail SMTP settings
                const string fromPassword = "nnnn 3333 tmkt 0101"; // Gmail APP password

                SmtpClient smtpClient = new SmtpClient    // Create SMTP client
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };

                smtpClient.SendCompleted += (sender, e) => // Enable SMTP debugging
                {
                    if (e.Error != null)
                    {
                        Console.WriteLine($"SMTP Error: {e.Error.Message}");
                    }
                    else if (e.Cancelled)
                    {
                        Console.WriteLine("SMTP Send Cancelled.");
                    }
                    else
                    {
                        Console.WriteLine("SMTP Send Completed Successfully.");
                    }
                };

                TextWriterTraceListener traceListener = new TextWriterTraceListener("smtp.log");// Log SMTP communication to a file (optional)
                Trace.Listeners.Add(traceListener);
                Trace.AutoFlush = true;

                using (var message = new MailMessage(fromAddress, toAddress)// Create the email message
                {
                    Subject = subject,
                    Body = body
                })
                {
                    foreach (var ccEmail in ccEmails)
                    {
                        message.CC.Add(new MailAddress(ccEmail));
                    }

                    await smtpClient.SendMailAsync(message);
                }

                Console.WriteLine("Email sent successfully!");
            }
            catch (SmtpException ex)
            {
                Console.WriteLine($"SMTP Error: {ex.StatusCode} - {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
            }
        }
    }
}