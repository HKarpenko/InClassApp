namespace Domain.Models.Dtos
{
    public class MeetingDto
    {
        public int Id { get; set; }
        public DateTime? MeetingStartDate { get; set; }
        public DateTime? MeetingEndDate { get; set; }
        public string? GroupName { get; set; }
        public int GroupId { get; set; }
    }
}
