using cloudscribe.Pagination.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pokedex.Common.Interfaces;
using Pokedex.Logging.Interfaces;
using PokedexAPI.Controllers;
using PokedexAPI.Interfaces;
using PokedexAPI.Models.Output;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pokedex.Tests.Controllers
{
    [TestClass]
    public class AbilitiesAPIControllerFixture
    {
        private Mock<IPokedexAPILogic> _pokedexAPILogicMock;

        private Mock<IPaginationHelper> _paginationHelperMock;
        private Mock<ILoggerAdapter<AbilitiesController>> _loggerMock;

        private AbilitiesController _abilitiesController;

        [TestInitialize]
        public void Intitialize()
        {
            _pokedexAPILogicMock = new Mock<IPokedexAPILogic>();
            _pokedexAPILogicMock.Setup(plm => plm.GetAllAbilities()).ReturnsAsync(It.IsAny<List<GenericLookupResult>>());
            _pokedexAPILogicMock.Setup(plm => plm.GetAbilityById(1)).ReturnsAsync(new GenericLookupResult { Id = 1 });

            _loggerMock = new Mock<ILoggerAdapter<AbilitiesController>>();

            _paginationHelperMock = new Mock<IPaginationHelper>();

            _paginationHelperMock.Setup(phm => phm.GetPagedResults(It.IsAny<IEnumerable<GenericLookupResult>>(),
                It.IsAny<int>(), It.IsAny<int>())).Returns(new PagedResult<GenericLookupResult>());

            _abilitiesController = new AbilitiesController(_pokedexAPILogicMock.Object,
                _paginationHelperMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public async Task GetAbilitiesIsSuccessfulAndCallsRepository()
        {
            await _abilitiesController.GetAbilities(3, 33);

            _pokedexAPILogicMock.Verify(plm => plm.GetAllAbilities(), Times.Once);

            _paginationHelperMock.Verify(plm => plm.GetPagedResults<GenericLookupResult>(null, 3, 33),
                Times.Once);
        }

        [TestMethod]
        public async Task GetAbilityByIdIsSuccessfulAndCallsRepository()
        {
            await _abilitiesController.GetAbilityById(1);

            _pokedexAPILogicMock.Verify(plm => plm.GetAbilityById(1), Times.Once);

            _loggerMock.VerifyNoOtherCalls();
        }


        [TestMethod]
        public async Task GetAbilityByInvalidIdCallsRepositoryAndLogsInformation()
        {
            await _abilitiesController.GetAbilityById(2);

            _pokedexAPILogicMock.Verify(prm => prm.GetAbilityById(2), Times.Once);

            _loggerMock.Verify(lm => lm.LogInformation("Invalid Request for Ability with Id: 2"), Times.Once);
        }
    }
}