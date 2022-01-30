using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InClassApp.Models.Entities
{
    public class Lecturer : Entity
    {
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public AppUser User { get; set; }

        public ICollection<LecturerGroupRelation> LecturerGroupRelations { get; set; }
    }
}
