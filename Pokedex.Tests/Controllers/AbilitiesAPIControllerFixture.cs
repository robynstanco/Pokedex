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
    public class AbilitiesAPIControllerFixture
    {
        private Mock<IPokedexAPILogic> _pokedexAPILogicMock;
        private Mock<ILoggerAdapter<AbilitiesController>> _loggerMock;

        private AbilitiesController _abilitiesController;

        [TestInitialize]
        public void Intitialize()
        {
            _pokedexAPILogicMock = new Mock<IPokedexAPILogic>();
            _pokedexAPILogicMock.Setup(plm => plm.GetAllAbilities()).ReturnsAsync(It.IsAny<List<GenericLookupResult>>());
            _pokedexAPILogicMock.Setup(plm => plm.GetAbilityById(1)).ReturnsAsync(new GenericLookupResult { Id = 1 });

            _loggerMock = new Mock<ILoggerAdapter<AbilitiesController>>();

            _abilitiesController = new AbilitiesController(_pokedexAPILogicMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public async Task GetAbilitiesIsSuccessfulAndCallsRepository()
        {
            await _abilitiesController.GetAbilities();

            _pokedexAPILogicMock.Verify(plm => plm.GetAllAbilities(), Times.Once);
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
