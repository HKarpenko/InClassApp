namespace Domain.Models.Entities
{
    public class Subject : Entity
    {
        public required string Name { get; set; }

        public required string Code { get; set; }

        public ICollection<Group>? Groups { get; set; }
    }
}