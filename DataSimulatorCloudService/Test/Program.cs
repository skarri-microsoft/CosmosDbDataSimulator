using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    using Common;
    using Common.SdkExtensions;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Newtonsoft.Json;
    using SimulationDataModels;

    class Program
    {
        private static readonly ConnectionPolicy connectionPolicy = new ConnectionPolicy
        {
            ConnectionMode = ConnectionMode.Direct,
            ConnectionProtocol = Protocol.Tcp
        };
        static void Main(string[] args)
        {
            string eventId1 = System.Guid.NewGuid().ToString();
            string eventId2 = System.Guid.NewGuid().ToString();

            
            //Duplicates
            AirplaneEvent a1=new AirplaneEvent(){AirId = "a1",DayId = "22", MonthId = "10", EvCount = 1,EvType = "EvType1", EventId = eventId1,YearId = "2018" };
            AirplaneEvent a2 = new AirplaneEvent() { AirId = "a1", DayId = "22", MonthId = "10", EvCount = 1, EvType = "EvType1" , EventId = eventId1, YearId = "2018" };
            AirplaneEvent a3 = new AirplaneEvent() { AirId = "a1", DayId = "22", MonthId = "10", EvCount = 1, EvType = "EvType1", EventId = eventId1, YearId = "2018" };
            AirplaneEvent a4 = new AirplaneEvent() { AirId = "a1", DayId = "22", MonthId = "10", EvCount = 1, EvType = "EvType1" , EventId = eventId1, YearId = "2018" };

            AirplaneEvent a5 = new AirplaneEvent() { AirId = "a2", DayId = "24", MonthId = "10", EvCount = 1, EvType = "EvType2", EventId = eventId2, YearId = "2018" };
            AirplaneEvent a6 = new AirplaneEvent() { AirId = "a2", DayId = "24", MonthId = "10", EvCount = 1, EvType = "EvType2", EventId = eventId2, YearId = "2018" };
            AirplaneEvent a7 = new AirplaneEvent() { AirId = "a2", DayId = "24", MonthId = "10", EvCount = 1, EvType = "EvType2", EventId = eventId2, YearId = "2018" };
            AirplaneEvent a8 = new AirplaneEvent() { AirId = "a2", DayId = "24", MonthId = "10", EvCount = 1, EvType = "EvType2", EventId = eventId2, YearId = "2018" };

            // No duplicates for a1 but event ids are different
            string eventId3 = System.Guid.NewGuid().ToString();
            string eventId4 = System.Guid.NewGuid().ToString();
            AirplaneEvent a9 = new AirplaneEvent() { AirId = "a1", DayId = "22", MonthId = "10", EvCount = 1, EvType = "EvType1", EventId = eventId3, YearId = "2018" };

            AirplaneEvent a10 = new AirplaneEvent() { AirId = "a1", DayId = "22", MonthId = "10", EvCount = 1, EvType = "EvType1", EventId = eventId4, YearId = "2018" };


            ServiceBusClientExtension.InitClient(ServiceBusConfig.GetServiceBusConfig());

            ServiceBusClientExtension.SendMessagesAsync(JsonConvert.SerializeObject(a1)).Wait();
            ServiceBusClientExtension.SendMessagesAsync(JsonConvert.SerializeObject(a2)).Wait();
            ServiceBusClientExtension.SendMessagesAsync(JsonConvert.SerializeObject(a3)).Wait();
            ServiceBusClientExtension.SendMessagesAsync(JsonConvert.SerializeObject(a4)).Wait();

            ServiceBusClientExtension.SendMessagesAsync(JsonConvert.SerializeObject(a5)).Wait();
            ServiceBusClientExtension.SendMessagesAsync(JsonConvert.SerializeObject(a6)).Wait();
            ServiceBusClientExtension.SendMessagesAsync(JsonConvert.SerializeObject(a7)).Wait();
            ServiceBusClientExtension.SendMessagesAsync(JsonConvert.SerializeObject(a8)).Wait();

            ServiceBusClientExtension.SendMessagesAsync(JsonConvert.SerializeObject(a9)).Wait();
            ServiceBusClientExtension.SendMessagesAsync(JsonConvert.SerializeObject(a10)).Wait();
        }

        static void QueryDocs()
        {
            CosmosDbConfig config = CosmosDbConfig.GetCosmosDbConfig();
            SqlClientExtension clientExtension = new SqlClientExtension(
                config,
                ConsistencyLevel.Session,
                connectionPolicy);

            var result = clientExtension.queryDocs(
                "select * from c where c.virtualMachine.uuid  = 'b0ae252e-4ee5-4aed-9080-faf494ce8fb0'");
            result.Wait();
        }
    }
}
