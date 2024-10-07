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
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one number.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

            RuleFor(user => user.UserName)
             .NotEmpty().WithMessage("Username is required.")
              .MinimumLength(6).WithMessage("Username must be at least 6 characters long.")
             .Matches(@"^\S*$").WithMessage("Username should not contain spaces.");


            RuleFor(user => user.Gender)
            .NotEmpty().WithMessage("Gender is required.")
            .Must(gender => new[] { "Male", "Female", "Other" }.Contains(gender))
            .WithMessage("Invalid gender.");

            RuleFor(user => user.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?[1-9]\d{9,14}$").WithMessage("Phone number must be at least 10 digits long and in a valid format.");

        }
    }
}
