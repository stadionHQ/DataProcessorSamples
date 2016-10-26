using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage.Queue;
using Stadion.DataProviders.Opta.Football.WebJobs;

namespace Stadion.Football.DataPusherWebJob
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
   public class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            try
            {
                

                var host = new JobHost();
                // The following code ensures that the WebJob will be running continuously
                host.RunAndBlock();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                Console.Write(ex.StackTrace);
            }
            
        }

        public static void ProcessQueueMessage([QueueTrigger("footballPushData")] CloudQueueMessage message, TextWriter log)
        {
            Functions.ProcessMessage(message, log);
        }
    }
}
