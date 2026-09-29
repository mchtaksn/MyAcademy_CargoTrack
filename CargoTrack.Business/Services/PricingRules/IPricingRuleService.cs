using CargoTrack.Dto.Dtos.PricingRuleDtos;

namespace CargoTrack.Business.Services.PricingRules
{
    public interface IPricingRuleService
    {
        Task<List<ResultPricingRuleDto>> GetAllAsync();
        Task<UpdatePricingRuleDto> GetByIdAsync(Guid id);
        Task UpdateAsync(UpdatePricingRuleDto updatePricingRuleDto);
    }
}