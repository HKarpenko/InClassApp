using System.Collections.Generic;

namespace InClassApp.Models.Entities
{
    public class Subject : Entity
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public ICollection<Group> Groups { get; set; }
    }
}