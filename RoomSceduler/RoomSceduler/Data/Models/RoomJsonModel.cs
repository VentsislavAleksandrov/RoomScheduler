using System;
using System.Collections.Generic;

namespace RoomSceduler.Data.Models
{
    public class RoomJsonModel
    {
        public string RoomName { get; set; }

        public int Capacity { get; set; }

        public string AvailableFrom { get; set; }

        public string AvailableTo { get; set; }

        public List<TimeSlotJsonModel> Schedule { get; set; }
    }
}
