using System;
using System.ComponentModel.DataAnnotations;

namespace StudentApi.Models1
{
    public class Student
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        public DateTime BirthDate { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
public Department Department { get; set; }

    }
}
