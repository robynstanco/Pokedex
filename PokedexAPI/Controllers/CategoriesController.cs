using Microsoft.AspNetCore.Mvc;
using Pokedex.Common;
using Pokedex.Logging.Interfaces;
using PokedexAPI.Interfaces;
using System;
using System.Threading.Tasks;

namespace PokedexAPI.Controllers
{
    /// <summary>
    /// The categories controller, readonly operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IPokedexApiLogic _pokedexApiLogic;
        private readonly ILoggerAdapter<CategoriesController> _logger;
        public CategoriesController(IPokedexApiLogic pokedexApiLogic, ILoggerAdapter<CategoriesController> logger)
        {
            _pokedexApiLogic = pokedexApiLogic ?? throw new ArgumentNullException(nameof(pokedexApiLogic));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get paginated categories.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size</param>
        /// <returns>The paginated lookup results.</returns>
        [HttpGet]
        public async Task<IActionResult> GetCategories([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = Constants.PageSize)
        {
            try
            {
                var paginatedCategories = await _pokedexApiLogic.GetCategories(pageNumber, pageSize);

                return Ok(paginatedCategories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Constants.UnexpectedError);

                return StatusCode(500);
            }
        }

        /// <summary>
        /// Get the category by id.
        /// </summary>
        /// <param name="id">The category id.</param>
        /// <returns>The lookup result.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
                var category = await _pokedexApiLogic.GetCategoryById(id);

                if (category != null)
                {
                    return Ok(category);
                }

                _logger.LogInformation($"{Constants.InvalidRequest} for {Constants.Category}{Constants.WithId}{id}");

                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Constants.UnexpectedError);

                return StatusCode(500);
            }
        }
    }
}