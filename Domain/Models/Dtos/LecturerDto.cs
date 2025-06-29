namespace Domain.Models.Dtos;

public class LecturerDto
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string FullName { get {  return $"{FirstName} {LastName}"; } }
}
