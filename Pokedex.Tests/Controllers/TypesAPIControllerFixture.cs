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
    public class TypesAPIControllerFixture
    {
        private Mock<IPokedexAPILogic> _pokedexAPILogicMock;
        private Mock<IPaginationHelper> _paginationHelperMock;
        private Mock<ILoggerAdapter<TypesController>> _loggerMock;

        private TypesController _typesController;

        [TestInitialize]
        public void Intitialize()
        {
            _pokedexAPILogicMock = new Mock<IPokedexAPILogic>();
            _pokedexAPILogicMock.Setup(plm => plm.GetAllTypes()).ReturnsAsync(It.IsAny<List<LookupResult>>());
            _pokedexAPILogicMock.Setup(plm => plm.GetTypeById(1)).ReturnsAsync(new LookupResult { Id = 1 });

            _loggerMock = new Mock<ILoggerAdapter<TypesController>>();

            _paginationHelperMock = new Mock<IPaginationHelper>();
            _paginationHelperMock.Setup(phm => phm.GetPagedResults(It.IsAny<IEnumerable<LookupResult>>(),
                It.IsAny<int>(), It.IsAny<int>())).Returns(new PagedResult<LookupResult>());

            _typesController = new TypesController(_pokedexAPILogicMock.Object,
                _paginationHelperMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public async Task GetTypesIsSuccessfulAndCallsRepository()
        {
            await _typesController.GetTypes(3, 33);

            _pokedexAPILogicMock.Verify(plm => plm.GetAllTypes(), Times.Once);

            _paginationHelperMock.Verify(plm => plm.GetPagedResults<LookupResult>(null, 3, 33),
                Times.Once);
        }

        [TestMethod]
        public async Task GetTypeByIdIsSuccessfulAndCallsRepository()
        {
            await _typesController.GetTypeById(1);

            _pokedexAPILogicMock.Verify(plm => plm.GetTypeById(1), Times.Once);

            _loggerMock.VerifyNoOtherCalls();
        }


        [TestMethod]
        public async Task GetTypeByInvalidIdCallsRepositoryAndLogsInformation()
        {
            await _typesController.GetTypeById(2);

            _pokedexAPILogicMock.Verify(prm => prm.GetTypeById(2), Times.Once);

            _loggerMock.Verify(lm => lm.LogInformation("Invalid Request for Type with Id: 2"), Times.Once);
        }
    }
}