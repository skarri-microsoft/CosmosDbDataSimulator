namespace AfServiceBusQueueTrigger
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Common;
    using Common.SdkExtensions;
    using Newtonsoft.Json;
    using SimulationDataModels;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Host;
    using Microsoft.ServiceBus.Messaging;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;

    public static class QueueReceiverFunction
    {
        private static readonly ConnectionPolicy connectionPolicy = new ConnectionPolicy
        {
            ConnectionMode = ConnectionMode.Direct,
            ConnectionProtocol = Protocol.Https
        };
        private static SqlClientExtension sqlClientExtension = null;
        [FunctionName("QueueReceiverFunction")]
        public static void Run(
            [ServiceBusTrigger("test", AccessRights.Manage, Connection = "AzServiceBusConn")]string myQueueItem,
            TraceWriter log)
        {
            log.Info($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
            ProcessMessageRun(myQueueItem, log);
        }

        public static void ProcessMessageRun(string mySbMsg, TraceWriter log)
        {
            if (mySbMsg != "" || mySbMsg != null)
            {
                AirplaneEvent airplaneEvent = JsonConvert.DeserializeObject<AirplaneEvent>(mySbMsg);

                string lookUpId = string.Format(
                    "{0}_{1}_{2}_{3}_{4}",
                    airplaneEvent.AirId,
                    airplaneEvent.EvType,
                    airplaneEvent.DayId,
                    airplaneEvent.MonthId,
                    airplaneEvent.YearId);

                airplaneEvent.id = lookUpId;
                CosmosDbConfig config = CosmosDbConfig.GetCosmosDbConfig();

                if (sqlClientExtension == null)
                {
                    sqlClientExtension = new SqlClientExtension(
                        config,
                        ConsistencyLevel.Session,
                        connectionPolicy
                    );
                }

                while (true)
                {
                    try
                    {


                        Task<List<object>> items =
                            sqlClientExtension.queryDocs("select * from c where c.id='" + lookUpId + "'");
                        items.Wait();

                        if (items.Result.Any())
                        {
                            // update the count
                            AirplaneEventAgg airplaneEventAgg =
                                JsonConvert.DeserializeObject<AirplaneEventAgg>(items.Result[0].ToString());
                            if (!AirplaneEventAgg.IsDuplicate(airplaneEventAgg, airplaneEvent))
                            {
                                AirplaneEventAgg updated =
                                    AirplaneEventAgg.GetIncrementedCountDoc(airplaneEventAgg, airplaneEvent);
                                sqlClientExtension.UpdateItem((Document)items.Result[0], updated).Wait();
                            }
                            break;
                        }
                        else
                        {
                            AirplaneEventAgg siteIdAggModel = AirplaneEventAgg.GetNewDoc(airplaneEvent);
                            sqlClientExtension.CreateDocument(siteIdAggModel, false).Wait();
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
        }

    }
}
