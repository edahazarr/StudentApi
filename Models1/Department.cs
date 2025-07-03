using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace StudentApi.Models1
{
    public class Department
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }


    }
}
