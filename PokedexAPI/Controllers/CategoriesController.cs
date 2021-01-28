using cloudscribe.Pagination.Models;
using Microsoft.AspNetCore.Mvc;
using Pokedex.Common;
using Pokedex.Common.Interfaces;
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
        private IPaginationHelper _paginationHelper;
        private ILoggerAdapter<CategoriesController> _logger;
        public CategoriesController(IPokedexAPILogic pokedexAPILogic, IPaginationHelper paginationHelper,
            ILoggerAdapter<CategoriesController> logger)
        {
            _pokedexAPILogic = pokedexAPILogic;
            _paginationHelper = paginationHelper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories([FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = Constants.PageSize)
        {
            List<GenericLookupResult> categories = await _pokedexAPILogic.GetAllCategories();

            PagedResult<GenericLookupResult> pagedCategories = 
                _paginationHelper.GetPagedResults(categories, pageNumber, pageSize);

            return Ok(pagedCategories.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            GenericLookupResult category = await _pokedexAPILogic.GetCategoryById(id);

            if (category == null)
            {
                _logger.LogInformation(Constants.InvalidRequest + " for " + Constants.Category + Constants.WithId + id);

                return BadRequest();
            }

            return Ok(category);
        }
    }
}