using Microsoft.AspNetCore.Mvc;
using StudentApi.Models1;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;  // ILogger için

namespace StudentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<StudentsController> _logger; // ILogger eklendi

        public StudentsController(AppDbContext context, IMapper mapper, ILogger<StudentsController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;  // DI ile alındı
        }

        [HttpGet]
        public ActionResult<IEnumerable<StudentDto>> GetStudents()
        {
            var students = _context.Students.ToList();
            var studentDtos = _mapper.Map<List<StudentDto>>(students);

            _logger.LogInformation("Students listed from database. Count: {Count}", studentDtos.Count);

            return Ok(studentDtos);
        }

        [HttpPost]
        public ActionResult<Student> CreateStudent(Student student)
        {
            _context.Students.Add(student);
            _context.SaveChanges();

            _logger.LogInformation("Student created with ID: {Id}, Name: {FirstName} {LastName}", student.Id, student.FirstName, student.LastName);

            return Ok(student);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateStudent(int id, Student updatedStudent)
        {
            var existingStudent = _context.Students.FirstOrDefault(s => s.Id == id);

            if (existingStudent == null)
            {
                _logger.LogWarning("Update failed. Student not found with ID: {Id}", id);
                return NotFound($"Öğrenci bulunamadı: ID = {id}");
            }

            existingStudent.FirstName = updatedStudent.FirstName;
            existingStudent.LastName = updatedStudent.LastName;
            existingStudent.BirthDate = updatedStudent.BirthDate;
            existingStudent.Email = updatedStudent.Email;

            _context.SaveChanges();

            _logger.LogInformation("Student updated with ID: {Id}, Name: {FirstName} {LastName}", existingStudent.Id, existingStudent.FirstName, existingStudent.LastName);

            return Ok(existingStudent);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            var student = _context.Students.FirstOrDefault(s => s.Id == id);

            if (student == null)
            {
                _logger.LogWarning("Delete failed. Student not found with ID: {Id}", id);
                return NotFound($"Silinecek öğrenci bulunamadı: ID = {id}");
            }

            _context.Students.Remove(student);
            _context.SaveChanges();

            _logger.LogInformation("Student deleted with ID: {Id}", id);

            return Ok($"Öğrenci başarıyla silindi: ID = {id}");
        }

        [HttpGet("search")]
        public ActionResult<IEnumerable<StudentDto>> SearchStudents(string? firstName, string? lastName, string? email)
        {
            var query = _context.Students.AsQueryable();

            if (!string.IsNullOrEmpty(firstName))
            {
                query = query.Where(s => s.FirstName.Contains(firstName));
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                query = query.Where(s => s.LastName.Contains(lastName));
            }

            if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(s => s.Email.Contains(email));
            }

            var filteredStudents = query.ToList();
            var result = _mapper.Map<List<StudentDto>>(filteredStudents);

            _logger.LogInformation("Students searched. Filter count: {Count}", result.Count);

            return Ok(result);
        }
    }
}
