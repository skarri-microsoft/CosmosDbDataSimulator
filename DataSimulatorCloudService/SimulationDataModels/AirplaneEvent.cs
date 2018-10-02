using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulationDataModels
{
    using Newtonsoft.Json;

    public class AirplaneEvent
    {
        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("airid")]
        public string AirId { get; set; }

        [JsonProperty("evcount")]
        public int EvCount { get; set; }

        [JsonProperty("evtype")]
        public string EvType { get; set; }

        [JsonProperty("thresh")]
        public int Thresh { get; set; }

        [JsonProperty("dayid")]
        public string DayId { get; set; }

        [JsonProperty("monthid")]
        public string MonthId { get; set; }

        [JsonProperty("yearid")]
        public string YearId { get; set;}

        [JsonProperty("eventid")]
        public string EventId { get; set;}
    }

    public class AirplaneEventAgg
    {
        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("airid")]
        public string AirId { get; set; }

        [JsonProperty("evcount")]
        public int EvCount { get; set; }

        [JsonProperty("evtype")]
        public string EvType { get; set; }

        [JsonProperty("thresh")]
        public int Thresh { get; set; }

        [JsonProperty("dayid")]
        public string DayId { get; set; }

        [JsonProperty("monthid")]
        public string MonthId { get; set; }

        [JsonProperty("yearid")]
        public string YearId { get; set; }

        [JsonProperty("eventid")]
        public string EventId { get; set; }

        [JsonProperty("ts")]
        public Int32 UnixTimestamp { get; set; }

        [JsonProperty("updatedWorkers")]
        public List<Worker> UpdatedWorkers { get; set; }

        public static bool IsDuplicate(AirplaneEventAgg model, AirplaneEvent planeEvent)
        {
            foreach (var worker in model.UpdatedWorkers)
            {
                if (worker.LastUpdatedEventId == planeEvent.EventId)
                {
                    return true;
                }
            }
            return false;
        }

        public static AirplaneEventAgg GetNewDoc(AirplaneEvent airplaneEvent)
        {
            AirplaneEventAgg model = new AirplaneEventAgg();
            model.AirId = airplaneEvent.AirId;
            model.EvCount= airplaneEvent.EvCount;
            model.EventId= airplaneEvent.EventId;
            model.EvType = airplaneEvent.EvType;
            model.UnixTimestamp = DateTimeUtil.GetUnixTimeStamp();
            model.DayId = airplaneEvent.DayId;
            model.MonthId = airplaneEvent.MonthId;
            model.YearId = airplaneEvent.YearId;
            model.id = airplaneEvent.id;
            model.UpdatedWorkers = new List<Worker>();
            string machineName = Environment.MachineName;
            model.UpdatedWorkers.Add(
                new Worker()
                {
                    LastUpdatedEventId = airplaneEvent.EventId,
                    LastUpdatedUnixTimestamp = DateTimeUtil.GetUnixTimeStamp(),
                    Name = machineName
                });
            return model;
        }

        public static AirplaneEventAgg GetIncrementedCountDoc(AirplaneEventAgg model, AirplaneEvent airplaneEvent)
        {
            model.EvCount = model.EvCount+ airplaneEvent.EvCount;
            bool isNew = true;
            string machineName = Environment.MachineName;
            foreach (var worker in model.UpdatedWorkers)
            {
                if (worker.Name.ToLower() == machineName.ToLower())
                {
                    worker.LastUpdatedEventId = airplaneEvent.EventId;
                    worker.LastUpdatedUnixTimestamp = DateTimeUtil.GetUnixTimeStamp();
                    isNew = false;
                    break;
                }
            }
            if (isNew)
            {
                model.UpdatedWorkers.Add(
                    new Worker()
                    {
                        LastUpdatedEventId = airplaneEvent.EventId,
                        LastUpdatedUnixTimestamp = DateTimeUtil.GetUnixTimeStamp(),
                        Name = machineName
                    });
            }

            return model;
        }
    }
}
