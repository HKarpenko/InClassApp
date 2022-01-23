using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace InClassApp.Models.Entities
{
    public class Meeting : Entity
    {
        public DateTime? MeetingStartDate { get; set; }

        public DateTime? MeetingEndDate { get; set; }

        [ForeignKey(nameof(Group))]
        public int GroupId { get; set; }

        public Group Group { get; set; }

        public bool IsAttendanceCheckLaunched { get; set; }

        public string LastlyGeneratedCheckCode { get; set; }

        public string LastlyGeneratedCodeIV { get; set; }

        public ICollection<PresenceRecord> PresenceRecords { get; set; }
    }
}