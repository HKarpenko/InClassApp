namespace Domain.Models.Dtos;

public class PresenceRecordDto
{
    public int Id { get; set; }
    public int MeetingId { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; }
    public string StudentIndex {  get; set; }
    public bool Status { get; set; }
}
