using Matty.Framework;
using Microsoft.AspNetCore.Mvc;
using Scorpio.Api.DataAccess;
using Scorpio.Api.Models;
using System;
using System.Threading.Tasks;

namespace Scorpio.Api.Controllers
{
    public class ConfigurationController : ScorpioController
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
            var response = new ServiceResult<UiConfiguration>(User);

            try
            {
                var result = await _uiConfigurationRepository.CreateAsync(uiConfiguration);
                response.Data = result;
                response.AddSuccessMessage($"Successfully created {uiConfiguration.Name}");
                return CreatedAtAction(nameof(Add), response);
            }
            catch (Exception ex)
            {
                response.AddErrorMessage(ex.Message);
                return BadRequest(response);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(UiConfiguration uiConfiguration)
        {
            var existing = await _uiConfigurationRepository.GetByIdAsync(uiConfiguration.Id);
            if (existing is null)
            {
                return NotFound();
            }

            var response = new ServiceResult<UiConfiguration>(User);

            try
            {
                await _uiConfigurationRepository.DeleteAsync(uiConfiguration);
                response.AddSuccessMessage($"Successfully deleted {uiConfiguration.Name}");
                return CreatedAtAction(nameof(Add), response);
            }
            catch (Exception ex)
            {
                response.AddErrorMessage(ex.Message);
                return BadRequest(response);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(UiConfiguration uiConfiguration)
        {
            var existing = await _uiConfigurationRepository.GetByIdAsync(uiConfiguration.Id);
            if (existing is null)
            {
                return NotFound();
            }

            var response = new ServiceResult<UiConfiguration>(User);

            try
            {
                await _uiConfigurationRepository.UpdateAsync(uiConfiguration);
                response.AddSuccessMessage($"Successfully updated config: {uiConfiguration.Name}");
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.AddErrorMessage(ex.Message);
                return BadRequest(response);
            }
        }
    }
}