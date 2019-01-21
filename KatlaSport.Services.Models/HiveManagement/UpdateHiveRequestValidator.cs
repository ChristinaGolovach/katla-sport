using FluentValidation;

namespace KatlaSport.Services.HiveManagement
{
    /// <summary>
    /// Represents a validator for <see cref="UpdateHiveRequestValidator"/>
    /// </summary>
    public class UpdateHiveRequestValidator : AbstractValidator<UpdateHiveRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateHiveRequestValidator"/> class.
        /// </summary>
        public UpdateHiveRequestValidator()
        {
            RuleFor(r => r.Name).NotNull().Length(4, 60);
            RuleFor(r => r.Code).NotNull().Length(5);
            RuleFor(r => r.Address).NotNull().Length(0, 300);
        }
    }
}
