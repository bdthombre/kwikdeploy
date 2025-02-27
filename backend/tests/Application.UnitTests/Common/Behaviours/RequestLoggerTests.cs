﻿using KwikDeploy.Application.Common.Behaviours;
using KwikDeploy.Application.Common.Interfaces;
using KwikDeploy.Application.Targets.Commands.TargetCreate;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace KwikDeploy.Application.UnitTests.Common.Behaviours;

public class RequestLoggerTests
{
    private Mock<ILogger<TargetCreateCommand>> _logger = null!;
    private Mock<ICurrentUserService> _currentUserService = null!;
    private Mock<IIdentityService> _identityService = null!;

    [SetUp]
    public void Setup()
    {
        _logger = new Mock<ILogger<TargetCreateCommand>>();
        _currentUserService = new Mock<ICurrentUserService>();
        _identityService = new Mock<IIdentityService>();
    }

    [Test]
    public async Task ShouldCallGetUserNameAsyncOnceIfAuthenticated()
    {
        _currentUserService.Setup(x => x.UserId).Returns(Guid.NewGuid().ToString());

        var requestLogger = new LoggingBehaviour<TargetCreateCommand>(_logger.Object, _currentUserService.Object);

        await requestLogger.Process(new TargetCreateCommand { Name = "abc" }, new CancellationToken());

        _identityService.Verify(i => i.GetUserNameAsync(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public async Task ShouldNotCallGetUserNameAsyncOnceIfUnauthenticated()
    {
        var requestLogger = new LoggingBehaviour<TargetCreateCommand>(_logger.Object, _currentUserService.Object);

        await requestLogger.Process(new TargetCreateCommand { Name = "abc" }, new CancellationToken());

        _identityService.Verify(i => i.GetUserNameAsync(It.IsAny<string>()), Times.Never);
    }
}
