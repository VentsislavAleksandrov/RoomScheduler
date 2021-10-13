namespace RoomSceduler.Data.Models
{
    public class RoomTimeslots
    {
        public int RoomId { get; set; }

        public Room Room { get; set; }

        public int TimeSlotId { get; set; }

        public TimeSlot TimeSlot { get; set; }
    }
}
