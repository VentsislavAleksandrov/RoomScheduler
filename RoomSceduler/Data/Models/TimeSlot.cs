using System;
using System.Collections.Generic;

namespace RoomSceduler.Data.Models
{
    public class TimeSlot
    {
        public int Id { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public List<RoomTimeslots> RoomTimeslots { get; set; }
    }
}
