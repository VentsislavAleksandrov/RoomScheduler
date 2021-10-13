using System;
using System.Collections.Generic;

namespace RoomSceduler.Data.Models
{
    public class Room
    {
        public int Id { get; set; }

        public string RoomName { get; set; }

        public int Capacity { get; set; }

        public DateTime AvailableFrom { get; set; }

        public DateTime AvailableTo { get; set; }

        public List<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();

        public List<RoomTimeslots> RoomTimeslots { get; set; } = new List<RoomTimeslots>();
    }
}
