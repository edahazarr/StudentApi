using Microsoft.AspNetCore.Mvc;
using StudentApi.Models1;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System; // TryParse için

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
                return BadRequest("Email zaten kayıtlı.");

            var student = new Student
            {
                FirstName = request.FirstName,
                LastName  = request.LastName,
                Email     = request.Email,
                Role      = request.Role
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

            var verify = _passwordHasher.VerifyHashedPassword(student, student.PasswordHash, request.Password);
            if (verify == PasswordVerificationResult.Failed)
                return Unauthorized("Email veya şifre yanlış");

            // --- JWT ayarları (null ve parse korumalı) ---
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secret   = jwtSettings["SecretKey"];
            var issuer   = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expiryStr = jwtSettings["ExpiryMinutes"];

            if (string.IsNullOrWhiteSpace(secret) ||
                string.IsNullOrWhiteSpace(issuer) ||
                string.IsNullOrWhiteSpace(audience) ||
                string.IsNullOrWhiteSpace(expiryStr))
            {
                _logger.LogError("JwtSettings eksik. Lütfen appsettings.json içindeki JwtSettings bölümünü kontrol edin.");
                return StatusCode(500, "Sunucu yapılandırma hatası (JwtSettings).");
            }

            if (!int.TryParse(expiryStr, out var expiryMinutes))
            {
                _logger.LogError("JwtSettings:ExpiryMinutes sayıya çevrilemedi: {Value}", expiryStr);
                return StatusCode(500, "Sunucu yapılandırma hatası (ExpiryMinutes).");
            }

            // --- Role enum -> string (CS1503 fix) ---
            var token = JwtTokenGenerator.GenerateToken(
                student.Email,
                student.Role.ToString(),   // <- ÖNEMLİ: enum'u string'e çevir
                secret,
                issuer,
                audience,
                expiryMinutes
            );

            _logger.LogInformation("Öğrenci giriş yaptı: {Email}", student.Email);
            return Ok(new { Token = token });
        }
    }
}
