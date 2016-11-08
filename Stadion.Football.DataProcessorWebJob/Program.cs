using System.IO;
using Common.Logging;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage.Queue;
using Stadion.Core;
using Stadion.Core.Caching;
using Stadion.Core.Contracts;
using Stadion.Core.Contracts.Sports;
using Stadion.Core.Events;
using Stadion.Core.Football;
using Stadion.Core.Football.Contracts;
using Stadion.Core.Football.Handlers;
using Stadion.Core.HandlebarsHelpers;
using Stadion.Core.Lively;
using Stadion.Core.Repositories;
using Stadion.Core.Services;
using Stadion.Core.Settings;
using Stadion.DataProviders.Opta.Football;
using Stadion.DataProviders.Opta.Football.Domain;
using Stadion.DataProviders.Opta.Football.WebJobs;
using StructureMap;

namespace FootballDataProcessorWebJob
{
    public class Program
    {
        public static WebjobDataProcessor dataProcessor;
        public static void Main(string[] args)
        {

            JobHostConfiguration config = new JobHostConfiguration();

            config.Queues.BatchSize = 2;
            config.Queues.MaxDequeueCount = 2;
            JobHost host = new JobHost(config);

            dataProcessor = new WebjobDataProcessor(GetContainer());
            host.RunAndBlock();
        }

        public static void ProcessQueueMessage([QueueTrigger("footballdata")] CloudQueueMessage binaryFile, TextWriter logger)
        {
            dataProcessor.ProcessMessage(binaryFile);
        }

        public static Container GetContainer()
        {

            return new Container(c => {
                c.For<IGeneralSettings>().Use(StadionSettings.Settings.GeneralSettings);
                c.For<IApiSettings>().Use(StadionSettings.Settings.ApiSettings);
                c.For<IReadOnlyFootballDataRepository>().Use<FootballDataRepository>();
                c.For<IClubContentService>().Use<ClubContentServiceAPiClient>();
                c.For<IUpdateableFootballDataRepository>().Use<UpdateableFootballDataRepository>();
                c.AddRegistry<OptaFootballRegistry>();
                c.For<ILog>().Use(LogManager.GetLogger<WebjobDataProcessor>());
                c.For<ISerializer>().Use<XmlSerialize>();
                c.For<IClubContentService>().Use<ClubContentServiceAPiClient>();
                c.For<IMediaStorageManager>().Use<S3MediaStorageManager>();
                c.For<IEventNotifier>().Use<EventNotifier>();
                c.For<ICachingProvider>().Use<RedisCacheProvider>();
                c.For<ICdnInvalidator>().Use<CloudfrontInvalidator>();
                c.For<ICloudflareSettings>().Use(StadionSettings.Settings.CdnSettings);
                c.For<ICdnSettings>().Use(StadionSettings.Settings.CdnSettings);
                c.For<IMatchDataCdnClearer>().Use<MatchDataCdnClearer>();
                c.For<IFixtureService>().Use<FootballFixtureServiceAPIClient>();
                c.For<IHandle<AutomatedCommentaryUpdated>>().Use<SendAutomatedCommentaryMessagesToLivelyEventHandler>();
                c.For<ILivelyApiClient>().Singleton().Use<LivelyApiClient>();
                c.For<ILivelySettings>().Use(StadionSettings.Settings.LivelySettings);
                c.For<IImageRepository>().Use<ImageRepository>();
                c.For<IApiSettings>().Use(StadionSettings.Settings.ApiSettings);
                c.For<IFileService>().Use<FileService>();
                c.AddRegistry<HandlebarsRegistry>();

            });
        }
    }
}
