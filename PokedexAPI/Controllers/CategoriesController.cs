using Microsoft.AspNetCore.Mvc;
using Pokedex.Common;
using Pokedex.Logging.Interfaces;
using PokedexAPI.Interfaces;
using PokedexAPI.Models.Output;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PokedexAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private IPokedexAPILogic _pokedexAPILogic;
        private ILoggerAdapter<CategoriesController> _logger;
        public CategoriesController(IPokedexAPILogic pokedexAPILogic, ILoggerAdapter<CategoriesController> logger)
        {
            _pokedexAPILogic = pokedexAPILogic;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = Constants.PageSize)
        {
            try
            {
                List<LookupResult> paginatedCategories = await _pokedexAPILogic.GetAllCategories(pageNumber, pageSize);

                return Ok(paginatedCategories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Constants.UnexpectedError);

                return StatusCode(500);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
                LookupResult category = await _pokedexAPILogic.GetCategoryById(id);

                if (category == null)
                {
                    _logger.LogInformation($"{Constants.InvalidRequest} for {Constants.Category}{Constants.WithId}{id}");

                    return NotFound();
                }

                return Ok(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Constants.UnexpectedError);

                return StatusCode(500);
            }
        }
    }
}