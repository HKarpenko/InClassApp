using System.ComponentModel.DataAnnotations;

namespace Domain.Models.Dtos
{
    public class GroupDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string StudiesSemestr { get; set; }
        public string SubjectName { get; set; }
        public List<string> Lecturers { get; set; }
 
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }
    }
}
