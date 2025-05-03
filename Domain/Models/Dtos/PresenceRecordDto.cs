namespace Domain.Models.Dtos;

public class PresenceRecordDto
{
    public int Id { get; set; }
    public int MeetingId { get; set; }
    public int StudentId { get; set; }
    public bool Status { get; set; }
}
