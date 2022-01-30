using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }

        public ICollection<Meeting> Meetings { get; set; }

        public ICollection<StudentGroupRelation> StudentGroupRelations { get; set; }

        public ICollection<LecturerGroupRelation> LecturerGroupRelations { get; set; }
    }
}