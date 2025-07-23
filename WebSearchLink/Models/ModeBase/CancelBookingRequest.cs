namespace WebSearchLink.Models.ModeBase
{
    public class CancelBookingRequest
    {
        public int UserId { get; set; }     
        public int TimeSlotId { get; set; }   
    }
    public class CheckUserTimeSlotRequest
    {
        public int UserId { get; set; }       // ID của người dùng
        public int TimeSlotId { get; set; }   // ID của thời gian slot
    }


}
