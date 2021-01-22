using Microsoft.AspNetCore.Mvc;
using Pokedex.Common;
using Pokedex.Logging.Interfaces;
using PokedexAPI.Interfaces;
using PokedexAPI.Models.Output;
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
        public async Task<IActionResult> GetCategories()
        {
            List<GenericLookupResult> categories = await _pokedexAPILogic.GetAllCategories();

            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            GenericLookupResult category = await _pokedexAPILogic.GetCategoryById(id);

            if (category == null)
            {
                _logger.LogInformation(Constants.InvalidRequest + " for " + Constants.Category + " with Id: " + id);

                return BadRequest();
            }

            return Ok(category);
        }
    }
}