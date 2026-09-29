using CargoTrack.DataAccess.Repositories.PricingRules;
using CargoTrack.Dto.Dtos.PricingRuleDtos;
using Mapster;
using System.ComponentModel.DataAnnotations;

namespace CargoTrack.Business.Services.PricingRules
{
    public class PricingRuleService(IPricingRuleRepository _pricingRuleRepository) : IPricingRuleService
    {
        public async Task<List<ResultPricingRuleDto>> GetAllAsync()
        {
            var rules = await _pricingRuleRepository.GetAllAsync();
            return rules.Adapt<List<ResultPricingRuleDto>>();
        }

        public async Task<UpdatePricingRuleDto> GetByIdAsync(Guid id)
        {
            var rule = await _pricingRuleRepository.GetByIdAsync(id);
            if (rule is null)
            {
                throw new ValidationException("Fiyat kuralı bulunamadı.");
            }
            return rule.Adapt<UpdatePricingRuleDto>();
        }

        public async Task UpdateAsync(UpdatePricingRuleDto updatePricingRuleDto)
        {
            var rule = await _pricingRuleRepository.GetByIdAsync(updatePricingRuleDto.Id);
            if (rule is null)
            {
                throw new ValidationException("Fiyat kuralı bulunamadı.");
            }

            rule.PricePerUnit = updatePricingRuleDto.PricePerUnit;
            rule.SameCityFee = updatePricingRuleDto.SameCityFee;
            rule.DifferentCityFee = updatePricingRuleDto.DifferentCityFee;

            await _pricingRuleRepository.UpdateAsync(rule);
        }
    }
}