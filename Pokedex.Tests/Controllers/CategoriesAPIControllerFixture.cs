using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pokedex.Logging.Interfaces;
using PokedexAPI.Controllers;
using PokedexAPI.Interfaces;
using PokedexAPI.Models.Output;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pokedex.Tests.Controllers
{
    [TestClass]
    public class CategoriesAPIControllerFixture
    {
        private Mock<IPokedexAPILogic> _pokedexAPILogicMock;
        private Mock<ILoggerAdapter<CategoriesController>> _loggerMock;

        private CategoriesController _categoriesController;

        [TestInitialize]
        public void Intitialize()
        {
            _pokedexAPILogicMock = new Mock<IPokedexAPILogic>();
            _pokedexAPILogicMock.Setup(plm => plm.GetAllCategories()).ReturnsAsync(It.IsAny<List<GenericLookupResult>>());
            _pokedexAPILogicMock.Setup(plm => plm.GetCategoryById(1)).ReturnsAsync(new GenericLookupResult { Id = 1 });

            _loggerMock = new Mock<ILoggerAdapter<CategoriesController>>();

            _categoriesController = new CategoriesController(_pokedexAPILogicMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public async Task GetCategoriesIsSuccessfulAndCallsRepository()
        {
            await _categoriesController.GetCategories();

            _pokedexAPILogicMock.Verify(plm => plm.GetAllCategories(), Times.Once);
        }

        [TestMethod]
        public async Task GetCategoryByIdIsSuccessfulAndCallsRepository()
        {
            await _categoriesController.GetCategoryById(1);

            _pokedexAPILogicMock.Verify(plm => plm.GetCategoryById(1), Times.Once);

            _loggerMock.VerifyNoOtherCalls();
        }


        [TestMethod]
        public async Task GetCategoryByInvalidIdCallsRepositoryAndLogsInformation()
        {
            await _categoriesController.GetCategoryById(2);

            _pokedexAPILogicMock.Verify(prm => prm.GetCategoryById(2), Times.Once);

            _loggerMock.Verify(lm => lm.LogInformation("Invalid Request for Category with Id: 2"), Times.Once);
        }
    }
}
