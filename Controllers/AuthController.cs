using Microsoft.AspNetCore.Mvc;
using StudentApi.Models1;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace StudentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AuthController> _logger;
        private readonly PasswordHasher<Student> _passwordHasher = new PasswordHasher<Student>();
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, ILogger<AuthController> logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterRequest request)
        {
            if (_context.Students.Any(s => s.Email == request.Email))
            {
                return BadRequest("Email zaten kayıtlı.");
            }

            var student = new Student
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
            };

            student.PasswordHash = _passwordHasher.HashPassword(student, request.Password);

            _context.Students.Add(student);
            _context.SaveChanges();

            _logger.LogInformation("Yeni öğrenci kayıt oldu: {Email}", student.Email);

            return Ok("Kayıt başarılı");
        }

        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            var student = _context.Students.SingleOrDefault(s => s.Email == request.Email);

            if (student == null)
                return Unauthorized("Email veya şifre yanlış");

            var result = _passwordHasher.VerifyHashedPassword(student, student.PasswordHash, request.Password);
            if (result == PasswordVerificationResult.Failed)
                return Unauthorized("Email veya şifre yanlış");

            // Token üret
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var token = JwtTokenGenerator.GenerateToken(
                student.Email,
                jwtSettings["SecretKey"],
                jwtSettings["Issuer"],
                jwtSettings["Audience"],
                int.Parse(jwtSettings["ExpiryMinutes"])
            );

            _logger.LogInformation("Öğrenci giriş yaptı: {Email}", student.Email);

            return Ok(new { Token = token });
        }
    }
}
