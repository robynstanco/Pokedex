Logger Adapter allows us to better unit test the logging in the app. 
See https://ardalis.com/testing-logging-in-aspnet-core for details.

Allows moq testing as follows:
	_loggerMock.Verify(lm => lm.LogInformation("Some Log"), Times.Once);
	_loggerMock.Verify(lm => lm.LogError("some error"), Times.Exactly(3));