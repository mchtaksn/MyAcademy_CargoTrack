using CargoTrack.Dto.Dtos.CargoDtos;
using FluentValidation;

namespace CargoTrack.Business.Validators.Cargos
{
    public class CreateCargoDtoValidator : AbstractValidator<CreateCargoDto>
    {
        public CreateCargoDtoValidator()
        {
            RuleFor(x => x.SenderId)
                .NotEmpty().WithMessage("Gönderici seçilmelidir.");

            RuleFor(x => x.ReceiverId)
                .NotEmpty().WithMessage("Alıcı seçilmelidir.");

            RuleFor(x => x)
                .Must(x => x.SenderId != x.ReceiverId)
                .WithMessage("Gönderici ve alıcı aynı kişi olamaz.");

            RuleFor(x => x.OriginBranchId)
                .NotEmpty().WithMessage("Çıkış şubesi seçilmelidir.");

            RuleFor(x => x.DestinationBranchId)
                .NotEmpty().WithMessage("Varış şubesi seçilmelidir.");

            RuleFor(x => x)
                .Must(x => x.OriginBranchId != x.DestinationBranchId)
                .WithMessage("Çıkış ve varış şubesi aynı olamaz.");

            RuleFor(x => x.Weight)
                .GreaterThan(0).WithMessage("Ağırlık 0'dan büyük olmalıdır.");

            RuleFor(x => x.Width)
                .GreaterThan(0).WithMessage("En 0'dan büyük olmalıdır.");

            RuleFor(x => x.Length)
                .GreaterThan(0).WithMessage("Boy 0'dan büyük olmalıdır.");

            RuleFor(x => x.Height)
                .GreaterThan(0).WithMessage("Yükseklik 0'dan büyük olmalıdır.");

            RuleFor(x => x.CargoType)
                .IsInEnum().WithMessage("Geçersiz gönderi tipi.");
        }
    }
}