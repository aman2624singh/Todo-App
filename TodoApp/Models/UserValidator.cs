using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Models
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email address.");

            RuleFor(user => user.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

            RuleFor(user => user.UserName)
               .NotEmpty().WithMessage("Username is required.")
               .MinimumLength(6).WithMessage("Username must be Unique.");

            RuleFor(user => user.Gender)
            .NotEmpty().WithMessage("Gender is required.")
            .Must(gender => new[] { "Male", "Female", "Other" }.Contains(gender))
            .WithMessage("Invalid gender.");

            RuleFor(user => user.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.");

        }
    }
}
