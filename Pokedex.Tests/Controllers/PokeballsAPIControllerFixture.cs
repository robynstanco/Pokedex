﻿using cloudscribe.Pagination.Models;
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
    public class PokeballsAPIControllerFixture
    {
        private Mock<IPokedexAPILogic> _pokedexAPILogicMock;
        private Mock<IPaginationHelper> _paginationHelperMock;
        private Mock<ILoggerAdapter<PokeballsController>> _loggerMock;

        private PokeballsController _pokeballsController;

        [TestInitialize]
        public void Intitialize()
        {
            _pokedexAPILogicMock = new Mock<IPokedexAPILogic>();
            _pokedexAPILogicMock.Setup(plm => plm.GetAllPokeballs()).ReturnsAsync(It.IsAny<List<LookupResult>>());
            _pokedexAPILogicMock.Setup(plm => plm.GetPokeballById(1)).ReturnsAsync(new LookupResult { Id = 1 });

            _loggerMock = new Mock<ILoggerAdapter<PokeballsController>>();

            _paginationHelperMock = new Mock<IPaginationHelper>();
            _paginationHelperMock.Setup(phm => phm.GetPagedResults(It.IsAny<IEnumerable<LookupResult>>(),
                It.IsAny<int>(), It.IsAny<int>())).Returns(new PagedResult<LookupResult>());

            _pokeballsController = new PokeballsController(_pokedexAPILogicMock.Object,
                _paginationHelperMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public async Task GetPokeballsIsSuccessfulAndCallsRepository()
        {
            await _pokeballsController.GetPokeballs(3, 33);

            _pokedexAPILogicMock.Verify(plm => plm.GetAllPokeballs(), Times.Once);

            _paginationHelperMock.Verify(plm => plm.GetPagedResults<LookupResult>(null, 3, 33),
                Times.Once);
        }

        [TestMethod]
        public async Task GetPokeballByIdIsSuccessfulAndCallsRepository()
        {
            await _pokeballsController.GetPokeballById(1);

            _pokedexAPILogicMock.Verify(plm => plm.GetPokeballById(1), Times.Once);

            _loggerMock.VerifyNoOtherCalls();
        }


        [TestMethod]
        public async Task GetPokeballByInvalidIdCallsRepositoryAndLogsInformation()
        {
            await _pokeballsController.GetPokeballById(2);

            _pokedexAPILogicMock.Verify(prm => prm.GetPokeballById(2), Times.Once);

            _loggerMock.Verify(lm => lm.LogInformation("Invalid Request for Pokéball with Id: 2"), Times.Once);
        }
    }
}