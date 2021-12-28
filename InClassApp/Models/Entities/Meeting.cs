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

        public ICollection<PresenceRecord> PresenceRecords { get; set; }
    }
}