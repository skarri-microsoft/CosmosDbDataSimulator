namespace DataSimulatorWorkerRole
{
    using System;
    using System.Threading.Tasks;
    using Common;
    using Common.SdkExtensions;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using SimulationDataModels.TimeSeries;

    public class CosmosDbSqlWriter
    {
        private static readonly ConnectionPolicy connectionPolicy = new ConnectionPolicy
        {
            ConnectionMode = ConnectionMode.Direct,
            ConnectionProtocol = Protocol.Tcp
        };
        public async Task GenerateTimeSeriesData()
        {
            CosmosDbConfig config = CosmosDbConfig.GetCosmosDbConfig();

            SqlClientExtension clientExtension=new SqlClientExtension(
                config,
                ConsistencyLevel.Session,
                connectionPolicy);
            await clientExtension.CreateCollectionIfNotExistsAsync();
            while (true)
            {
                SensorReading reading = SensorReading.GetSampleReading();

                await clientExtension.DocumentClient.UpsertDocumentAsync(
                    UriFactory.CreateDocumentCollectionUri(clientExtension.DatabaseName, clientExtension.CollectionName), 
                    reading);
                System.Threading.Thread.Sleep(new TimeSpan(0,0,0,1));
            }
        }
    }
}
