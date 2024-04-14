using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Entities
{
    public class Lecturer : Entity
    {
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public AppUser User { get; set; }

        public ICollection<LecturerGroupRelation> LecturerGroupRelations { get; set; }
    }
}
