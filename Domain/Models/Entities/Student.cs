﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Entities
{
    public class Student : Entity
    {
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public AppUser User { get; set; }

        public string Index { get; set; }

        public ICollection<StudentGroupRelation> StudentGroupRelations { get; set; }

    }
}
