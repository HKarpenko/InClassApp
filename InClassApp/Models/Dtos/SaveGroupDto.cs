using System;
using System.Collections.Generic;

namespace InClassApp.Models.Dtos
{
    public class SaveGroupDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string StudiesSemestr { get; set; }

        public int SubjectId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public ICollection<int> LecturersIds { get; set; }
    }
}
