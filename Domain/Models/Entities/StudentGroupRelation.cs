using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Entities
{
    public class StudentGroupRelation : Entity
    {
        [ForeignKey(nameof(Group))]
        public int GroupId { get; set; }

        public Group Group { get; set; }

        [ForeignKey(nameof(Student))]
        public int StudentId { get; set; }

        public Student Student { get; set; }
    }
}
