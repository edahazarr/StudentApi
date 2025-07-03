using Microsoft.AspNetCore.Mvc;
using StudentApi.Models1;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;

namespace StudentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public StudentsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Tüm öğrencileri listeleyen metot.
        /// </summary>
        /// <returns>Öğrenci listesini döner.</returns>
        [HttpGet]
        public ActionResult<IEnumerable<StudentDto>> GetStudents()
        {
            var students = _context.Students.ToList();
            var studentDtos = _mapper.Map<List<StudentDto>>(students);
            return Ok(studentDtos);
        }

        /// <summary>
        /// Yeni öğrenci oluşturur.
        /// </summary>
        /// <param name="student">Yeni öğrenci bilgileri.</param>
        /// <returns>Oluşturulan öğrenci nesnesini döner.</returns>
        [HttpPost]
        public ActionResult<Student> CreateStudent(Student student)
        {
            _context.Students.Add(student);
            _context.SaveChanges();
            return Ok(student);
        }

        /// <summary>
        /// Varolan öğrenciyi günceller.
        /// </summary>
        /// <param name="id">Güncellenecek öğrencinin Id'si.</param>
        /// <param name="updatedStudent">Yeni öğrenci bilgileri.</param>
        /// <returns>Güncellenmiş öğrenci nesnesi.</returns>
        [HttpPut("{id}")]
        public IActionResult UpdateStudent(int id, Student updatedStudent)
        {
            var existingStudent = _context.Students.FirstOrDefault(s => s.Id == id);

            if (existingStudent == null)
                return NotFound($"Öğrenci bulunamadı: ID = {id}");

            existingStudent.FirstName = updatedStudent.FirstName;
            existingStudent.LastName = updatedStudent.LastName;
            existingStudent.BirthDate = updatedStudent.BirthDate;
            existingStudent.Email = updatedStudent.Email;

            _context.SaveChanges();

            return Ok(existingStudent);
        }

        /// <summary>
        /// Belirtilen Id'ye sahip öğrenciyi siler.
        /// </summary>
        /// <param name="id">Silinecek öğrencinin Id'si.</param>
        /// <returns>Başarı mesajı döner.</returns>
        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            var student = _context.Students.FirstOrDefault(s => s.Id == id);

            if (student == null)
                return NotFound($"Silinecek öğrenci bulunamadı: ID = {id}");

            _context.Students.Remove(student);
            _context.SaveChanges();

            return Ok($"Öğrenci başarıyla silindi: ID = {id}");
        }
    }
}
