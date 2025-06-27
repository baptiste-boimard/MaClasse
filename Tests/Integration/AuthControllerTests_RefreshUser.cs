using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using MaClasse.Shared.Models;
using MaClasse.Shared.Models.Scheduler;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Service.OAuth.Controller;
using Service.OAuth.Interfaces;
using Service.OAuth.Service;
using Service.OAuth.Service.Interface;
using Xunit;

namespace Tests.Integration;

public class AuthControllerTests_RefreshUser
{
  private readonly IUserServiceRattachment _userServiceRattachment;
  private readonly Mock<IConfiguration> _mockIConfiguration;
  private readonly Mock<IValidateGoogleTokenService> _mockValidateToken;
  private readonly Mock<IAuthRepository> _mockAuthRepo;
  private readonly Mock<ISessionRepository> _mockSessionRepo;
  private readonly Mock<IGenerateIdRole> _mockGenerateIdRole;
  private readonly Mock<ICreateDataService> _mockCreateDataService;
  private readonly Mock<IDeleteUserService> _mockDeleteUserService;
  
  private readonly IUserServiceRattachment _realUserServiceRattachment;
  
  private readonly AuthController _controller;

  public AuthControllerTests_RefreshUser()
  {
    _mockIConfiguration = new Mock<IConfiguration>();
    _mockValidateToken = new Mock<IValidateGoogleTokenService>();
    _mockAuthRepo = new Mock<IAuthRepository>();
    _mockSessionRepo = new Mock<ISessionRepository>();
    _mockGenerateIdRole = new Mock<IGenerateIdRole>();
    _mockCreateDataService = new Mock<ICreateDataService>();
    _mockDeleteUserService = new Mock<IDeleteUserService>();
    
    _realUserServiceRattachment = new UserServiceRattachment(_mockAuthRepo.Object);
    
    _controller = new AuthController(
      _mockIConfiguration.Object,
      _mockValidateToken.Object,
      _mockAuthRepo.Object,
      _mockSessionRepo.Object,
      _realUserServiceRattachment,
      _mockGenerateIdRole.Object,
      _mockCreateDataService.Object,
      _mockDeleteUserService.Object
    );
  }

  [Fact]
  public async Task GetUser_ShouldReturnOk_WhenSessionIsValidAndUserExists()
  {
    var request = new GoogleTokenRequest { Token = "session-token" };

    var userSession = new SessionData
    {
      Token = "session-token",
      UserId = "user-id",
      Expiration = DateTime.UtcNow.AddMinutes(10)
    };

    _mockSessionRepo.Setup(x => x.GetUserIdByCookies(request.Token)).ReturnsAsync(userSession);
    
    var user = new UserProfile
    {
      Id = "user-id",
      IdRole = "role-id",
      Email = "test@gmail.com",
      Name = "Test User",
      Role = "User",
      Zone = "A",
      GivenName = "Test",
      FamilyName = "User",
      Picture = "pic-url"
    };

    _mockAuthRepo.Setup(x => x.GetOneUserByGoogleId(userSession.UserId)).ReturnsAsync(user);
    
    var scheduler = new Scheduler();

    _mockCreateDataService.Setup(x => x.GetDataScheduler(user.Id)).ReturnsAsync(scheduler);
    
    //* Récupération des données de l'utilisateur avec les rattachments
    _mockAuthRepo.Setup(s => s.GetRattachmentByIdRole(user.IdRole)).ReturnsAsync(new List<Rattachment>());
    
    var responseData = new AuthReturn
    {
      IdSession = "session-token",
      IsNewUser = false,
      Scheduler = new Scheduler(),
      UserWithRattachment = new UserWithRattachment
      {
        AccessToken = "session-token",
        AsDirecteur = new List<Rattachment>(),
        AsProfesseur = new List<Rattachment>(),
        UserProfile = new UserProfile
        {
          CreatedAt = null,
          Email = "test@gmail.com",
          FamilyName = "User",
          GivenName = "Test",
          Id = "user-id",
          IdRole = "role-id",
          Name = "Test User",
          Picture = "pic-url",
          Role = "User",
          UpdatedAt = null,
          Zone = "A"
        }
      }
    };

    // Act
    var response = await _controller.GetUser(request);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(response);
    var returned = Assert.IsType<AuthReturn>(okResult.Value);

    returned.IdSession.Should().Be("session-token");
    returned.UserWithRattachment.UserProfile.Email.Should().Be("test@gmail.com");
    
    okResult.Value.Should().BeEquivalentTo(responseData);
  }

  [Fact]
  public async Task GetUser_ShouldReturnUnauthorized_WhenSessionNotFound()
  {
    var request = new GoogleTokenRequest { Token = "invalid-token" };

    _mockSessionRepo.Setup(x => x.GetUserIdByCookies(request.Token)).ReturnsAsync((SessionData?)null);

    var response = await _controller.GetUser(request);

    Assert.IsType<UnauthorizedResult>(response);
  }
  
  [Fact]
  public async Task GetUser_ShouldReturnUnauthorized_WhenSessionExpired()
  {
    var request = new GoogleTokenRequest { Token = "expired-token" };

    var session = new SessionData
    {
      Token = "expired-token",
      UserId = "user-id",
      Expiration = DateTime.UtcNow.AddMinutes(-5)
    };

    _mockSessionRepo.Setup(x => x.GetUserIdByCookies(request.Token)).ReturnsAsync(session);

    var response = await _controller.GetUser(request);

    Assert.IsType<UnauthorizedResult>(response);
  }
  
  [Fact]
  public async Task GetUser_ShouldReturnUnauthorized_WhenUserNotFound()
  {
    var request = new GoogleTokenRequest { Token = "session-token" };

    var session = new SessionData
    {
      Token = "session-token",
      UserId = "user-id",
      Expiration = DateTime.UtcNow.AddMinutes(5)
    };

    _mockSessionRepo.Setup(x => x.GetUserIdByCookies(request.Token)).ReturnsAsync(session);
    _mockAuthRepo.Setup(x => x.GetOneUserByGoogleId(session.UserId)).ReturnsAsync((UserProfile?)null);
    _mockCreateDataService.Setup(x => x.GetDataScheduler(It.IsAny<string>())).ReturnsAsync(new Scheduler());

    var response = await _controller.GetUser(request);

    Assert.IsType<UnauthorizedResult>(response);
  }

}