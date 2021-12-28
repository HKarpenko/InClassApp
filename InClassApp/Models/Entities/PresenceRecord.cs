using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace InClassApp.Models.Entities
{
    public class PresenceRecord : Entity
    {
        [ForeignKey(nameof(Meeting))]
        public int MeetingId { get; set; }

        public Meeting Meeting { get; set; }

        public bool Status { get; set; }
    }
}