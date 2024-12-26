using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Entities
{
    public class Group : Entity
    {
        public string Name { get; set; }

        public string StudiesSemestr { get; set; }

        [ForeignKey(nameof(Subject))]
        public int SubjectId { get; set; }

        public Subject Subject { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public ICollection<Meeting> Meetings { get; set; }

        public ICollection<StudentGroupRelation> StudentGroupRelations { get; set; }

        public ICollection<LecturerGroupRelation> LecturerGroupRelations { get; set; }
    }
}