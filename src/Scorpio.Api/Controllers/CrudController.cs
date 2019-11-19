using Matty.Framework;
using Microsoft.AspNetCore.Mvc;
using Scorpio.Api.DataAccess;
using Scorpio.Api.Models;
using System;
using System.Threading.Tasks;

namespace Scorpio.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public abstract class CrudController<TRepository, TEntity> : ControllerBase
        where TRepository : IGenericRepository<TEntity, string>
        where TEntity : EntityBase
    {
        protected readonly TRepository Repository;

        protected CrudController(TRepository repository)
        {
            Repository = repository;
        }

        [HttpGet]
        public virtual async Task<IActionResult> GetAll()
        {
            var result = await Repository.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> GetById(string id)
        {
            var result = await Repository.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Add(TEntity entity)
        {
            var response = new ServiceResult<TEntity>(User);

            try
            {
                response.Data = await Repository.CreateAsync(entity);
                response.AddSuccessMessage($"Successfully created {entity}");
                return CreatedAtAction(nameof(Add), response);
            }
            catch (FormatException ex)
            {
                var msg = $"Supplied invalid id: {ex.Message} Please don't use any ID here - it will be created automatically.";
                response.AddErrorMessage(msg);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.AddErrorMessage(ex.Message);
                return BadRequest(response);
            }
        }

        [HttpDelete]
        public virtual async Task<IActionResult> Delete(TEntity entity)
        {
            var existing = await Repository.GetByIdAsync(entity.Id);
            if (existing is null)
            {
                return NotFound();
            }

            var response = new ServiceResult<TEntity>(User);

            try
            {
                await Repository.DeleteAsync(entity);
                response.AddSuccessMessage($"Successfully deleted {entity}");
                return CreatedAtAction(nameof(Add), response);
            }
            catch (FormatException ex)
            {
                response.AddErrorMessage($"Supplied invalid id: {ex.Message}");
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.AddErrorMessage(ex.Message);
                return BadRequest(response);
            }
        }

        [HttpPut]
        public virtual async Task<IActionResult> Update(TEntity entity)
        {
            var existing = await Repository.GetByIdAsync(entity.Id);
            if (existing is null)
            {
                return NotFound();
            }

            var response = new ServiceResult<TEntity>(User);

            try
            {
                response.Data = await Repository.UpdateAsync(entity);
                response.AddSuccessMessage($"Successfully updated config: {entity}");
                return Ok(response);
            }
            catch (FormatException ex)
            {
                response.AddErrorMessage($"Supplied invalid id: {ex.Message}");
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.AddErrorMessage(ex.Message);
                return BadRequest(response);
            }
        }
    }
}