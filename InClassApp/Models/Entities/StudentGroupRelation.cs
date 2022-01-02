using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InClassApp.Models.Entities
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
