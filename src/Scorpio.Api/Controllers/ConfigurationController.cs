using Microsoft.AspNetCore.Mvc;
using Scorpio.Api.DataAccess;
using Scorpio.Api.Models;
using System.Threading.Tasks;

namespace Scorpio.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly IUiConfigurationRepository _uiConfigurationRepository;

        public ConfigurationController(IUiConfigurationRepository uiConfigurationRepository)
        {
            _uiConfigurationRepository = uiConfigurationRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _uiConfigurationRepository.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _uiConfigurationRepository.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add(UiConfiguration uiConfiguration)
        {
            var result = await _uiConfigurationRepository.CreateAsync(uiConfiguration);
            return CreatedAtAction(nameof(Add), result);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(UiConfiguration uiConfiguration)
        {
            var existing = await _uiConfigurationRepository.GetByIdAsync(uiConfiguration.Id);
            if (existing is null)
            {
                return NotFound();
            }

            await _uiConfigurationRepository.DeleteAsync(uiConfiguration);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(UiConfiguration uiConfiguration)
        {
            var existing = await _uiConfigurationRepository.GetByIdAsync(uiConfiguration.Id);
            if (existing is null)
            {
                return NotFound();
            }

            await _uiConfigurationRepository.UpdateAsync(uiConfiguration);
            return Ok();
        }
    }
}