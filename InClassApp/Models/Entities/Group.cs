using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace InClassApp.Models.Entities
{
    public class Group : Entity
    {
        public string Name { get; set; }

        public string StudiesSemestr { get; set; }

        [ForeignKey(nameof(Subject))]
        public int SubjectId { get; set; }

        public Subject Subject { get; set; }

        [ForeignKey(nameof(Lecturer))]
        public int LecturerId { get; set; }

        public Lecturer Lecturer { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public ICollection<Meeting> Meetings { get; set; }

        public ICollection<StudentGroupRelation> StudentGroupRelations { get; set; }
    }
}