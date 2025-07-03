using FluentValidation;
using StudentApi.Models1;

public class StudentValidator : AbstractValidator<Student>
{
    public StudentValidator()
    {
        RuleFor(s => s.FirstName)
            .NotEmpty().WithMessage("Ad boş olamaz")
            .MinimumLength(2).WithMessage("Ad en az 2 karakter olmalı")
            .Matches(@"^[^\d]+$").WithMessage("Ad rakam içeremez");

        RuleFor(s => s.LastName)
            .NotEmpty().WithMessage("Soyad boş olamaz")
            .MinimumLength(2).WithMessage("Soyad en az 2 karakter olmalı")
            .Matches(@"^[^\d]+$").WithMessage("Soyad rakam içeremez");

        RuleFor(s => s.Email)
            .NotEmpty().WithMessage("Email boş olamaz")
            .EmailAddress().WithMessage("Geçerli bir email giriniz");

        RuleFor(s => s.BirthDate)
            .LessThan(DateTime.Now).WithMessage("Doğum tarihi gelecekte olamaz");
    }
}
