using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage.Queue;
using Stadion.DataProviders.Opta.Football.WebJobs;

namespace FootballDataProcessorWebJob
{
    public class Program 
    {
      
        public static void Main(string[] args)
        {

            JobHostConfiguration config = new JobHostConfiguration();

            config.Queues.BatchSize = 1;
            config.Queues.MaxDequeueCount = 1;
            JobHost host = new JobHost(config);

            host.RunAndBlock();
        }

        public static void ProcessQueueMessage([QueueTrigger("footballdata")] CloudQueueMessage binaryFile, TextWriter logger)
        {

            var dataProcessor = new WebjobDataProcessor();
            dataProcessor.ProcessMessage(binaryFile);
        }


    }
}
