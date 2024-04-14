using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Entities
{
    public class LecturerGroupRelation : Entity
    {
        [ForeignKey(nameof(Group))]
        public int GroupId { get; set; }

        public Group Group { get; set; }

        [ForeignKey(nameof(Lecturer))]
        public int LecturerId { get; set; }

        public Lecturer Lecturer { get; set; }
    }
}
