using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage.Queue;
using Stadion.DataProviders.Opta.Football.WebJobs;

namespace FootballDataProcessorWebJob
{
    public class Program 
    {
      
        static void Main(string[] args)
        {

            JobHost host = new JobHost();

            host.RunAndBlock();
        }

        public static void ProcessQueueMessage([QueueTrigger("footballdata")] CloudQueueMessage binaryFile, TextWriter logger)
        {

            WebjobDataProcessor dataProcessor = new WebjobDataProcessor();
            dataProcessor.ProcessMessage(binaryFile);
        }


    }
}
