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

public class AuthControllerTests_FinishedSignup
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

  public AuthControllerTests_FinishedSignup()
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
  public async Task FinishedSignUp_WithUpdateSession_ReturnResponse()
  {

    var result = new SignupDialogResult
    {
      Role = "User",
      Zone = "A",
      AccessToken = "valid-token",
      IdSession = "session-token"
    };
    
    //* Récupération de la UserSession
    var userSession = new SessionData
    {
      Token = "session-token",
      UserId = "user-id",
      Role = "",
      Expiration = new DateTime(2025, 06, 27, 12, 00, 00)
    };

    _mockSessionRepo.Setup(s => s.GetUserIdByCookies(result.IdSession)).ReturnsAsync(userSession);
    
    //* Mise à jour de la session
    SessionData? capturedSession = null;
    _mockSessionRepo
      .Setup(s => s.UpdateSession(It.IsAny<SessionData>()))
      .Callback<SessionData>(s => capturedSession = s)
      .ReturnsAsync((SessionData s) => s);
    
    //* Récupération du User
    var updatedUser = new UserProfile
    {
      Id = "user-id",
      IdRole = "user-idRole",
      Email = "user-email",
      Name = "user name",
      Role = "User",
      Zone = "A",
      GivenName = "user",
      FamilyName = "name",
      Picture = "user-picture",
      CreatedAt = new DateTime(2025, 06, 27, 12, 00, 00),
      UpdatedAt = new DateTime(2025, 06, 27, 12, 00, 00)
    };
    _mockAuthRepo.Setup(d => d.GetOneUserByGoogleId(updatedUser.Id)).ReturnsAsync(updatedUser);
    
    //* Mise à jour de l'utilisateur
    UserProfile? capturedUserProfile = null;
    
    _mockAuthRepo
      .Setup(a => a.UpdateUser(updatedUser))
      .Callback<UserProfile>(a => capturedUserProfile = a)
      .ReturnsAsync(updatedUser);
    
    //* Création du scheduler avec les vacances
    _mockCreateDataService.Setup(s => s.AddHolidayToScheduler(updatedUser)).ReturnsAsync(new Scheduler());
    
    //* Récupération des données de l'utilisateur avec les rattachments
    _mockAuthRepo.Setup(s => s.GetRattachmentByIdRole(It.IsAny<string>())).ReturnsAsync(new List<Rattachment>());
    
    var responseData = new AuthReturn
    {
      IdSession = "session-token",
      IsNewUser = false,
      Scheduler = new Scheduler(),
      UserWithRattachment = new UserWithRattachment
      {
        AccessToken = "valid-token",
        AsDirecteur = new List<Rattachment>(),
        AsProfesseur = new List<Rattachment>(),
        UserProfile = new UserProfile
        {
          CreatedAt = new DateTime(2025, 06, 27, 12, 00, 00),
          Email = "user-email",
          FamilyName = "name",
          GivenName = "user",
          Id = "user-id",
          IdRole = "user-idRole",
          Name = "user name",
          Picture = "user-picture",
          Role = "User",
          Zone = "A"
        }
      }
    };
    
    // Act
    var response = await _controller.FinishedSignUp(result);
    
    //Assert
    capturedSession.Should().NotBeNull();
    capturedSession.Role.Should().Be(result.Role);
    capturedUserProfile.Should().NotBeNull();
    capturedUserProfile.Role.Should().Be("User");
    capturedUserProfile.Zone.Should().Be("A");
    capturedUserProfile.UpdatedAt.Should().NotBeNull();
    
    var okResult = Assert.IsType<OkObjectResult>(response);
    okResult.Value.Should().BeEquivalentTo(responseData, options => 
      options.Excluding(r => r.UserWithRattachment.UserProfile.UpdatedAt));
    
    // Vérifie que UpdatedAt n'est pas null
    var returned = Assert.IsType<AuthReturn>(okResult.Value);
    returned.UserWithRattachment.UserProfile.UpdatedAt.Should().NotBeNull();
  }
  
  [Fact]
  public async Task FinishedSignUp_NoUpdateSession()
  {

    var result = new SignupDialogResult
    {
      Role = "User",
      Zone = "A",
      AccessToken = "valid-token",
      IdSession = "session-token"
    };
    
    //* Récupération de la UserSession
    var userSession = new SessionData
    {
      Token = "session-token",
      UserId = "user-id",
      Role = "",
      Expiration = new DateTime(2025, 06, 27, 12, 00, 00)
    };

    _mockSessionRepo.Setup(s => s.GetUserIdByCookies(result.IdSession)).ReturnsAsync(userSession);
    
    //* Mise à jour de la session
    SessionData? capturedSession = null;
    _mockSessionRepo
      .Setup(s => s.UpdateSession(It.IsAny<SessionData>())).ReturnsAsync((SessionData) null);
    
    //Act
    var response = await _controller.FinishedSignUp(result);
    
    //Assert
    Assert.IsType<UnauthorizedResult>(response);
  }
  
  [Fact]
  public async Task FinishedSignUp_WithUpdateSession_UpdatedUserForRattachmentIsNull()
  {

    var result = new SignupDialogResult
    {
      Role = "User",
      Zone = "A",
      AccessToken = "valid-token",
      IdSession = "session-token"
    };
    
    //* Récupération de la UserSession
    var userSession = new SessionData
    {
      Token = "session-token",
      UserId = "user-id",
      Role = "",
      Expiration = new DateTime(2025, 06, 27, 12, 00, 00)
    };

    _mockSessionRepo.Setup(s => s.GetUserIdByCookies(result.IdSession)).ReturnsAsync(userSession);
    
    //* Mise à jour de la session
    SessionData? capturedSession = null;
    _mockSessionRepo
      .Setup(s => s.UpdateSession(It.IsAny<SessionData>()))
      .Callback<SessionData>(s => capturedSession = s)
      .ReturnsAsync((SessionData s) => s);
    
    //* Récupération du User
    var updatedUser = new UserProfile
    {
      Id = "user-id",
      IdRole = "user-idRole",
      Email = "user-email",
      Name = "user name",
      Role = "User",
      Zone = "A",
      GivenName = "user",
      FamilyName = "name",
      Picture = "user-picture",
      CreatedAt = new DateTime(2025, 06, 27, 12, 00, 00),
      UpdatedAt = new DateTime(2025, 06, 27, 12, 00, 00)
    };
    _mockAuthRepo.Setup(d => d.GetOneUserByGoogleId(updatedUser.Id)).ReturnsAsync(updatedUser);
    
    //* Mise à jour de l'utilisateur
    UserProfile? capturedUserProfile = null;
    
    _mockAuthRepo
      .Setup(a => a.UpdateUser(updatedUser)).ReturnsAsync((UserProfile) null);
    
    //* Création du scheduler avec les vacances
    _mockCreateDataService.Setup(s => s.AddHolidayToScheduler(updatedUser)).ReturnsAsync(new Scheduler());
    
    // //* Récupération des données de l'utilisateur avec les rattachments
    // _mockAuthRepo.Setup(s => s.GetRattachmentByIdRole(It.IsAny<string>())).ReturnsAsync(new List<Rattachment>());
    
    // var responseData = new AuthReturn
    // {
    //   IdSession = "session-token",
    //   IsNewUser = false,
    //   Scheduler = new Scheduler(),
    //   UserWithRattachment = new UserWithRattachment
    //   {
    //     AccessToken = "valid-token",
    //     AsDirecteur = new List<Rattachment>(),
    //     AsProfesseur = new List<Rattachment>(),
    //     UserProfile = new UserProfile
    //     {
    //       CreatedAt = new DateTime(2025, 06, 27, 12, 00, 00),
    //       Email = "user-email",
    //       FamilyName = "name",
    //       GivenName = "user",
    //       Id = "user-id",
    //       IdRole = "user-idRole",
    //       Name = "user name",
    //       Picture = "user-picture",
    //       Role = "User",
    //       Zone = "A"
    //     }
    //   }
    // };
    //
    // Act
    var response = await _controller.FinishedSignUp(result);
    
    //Assert
    Assert.IsType<UnauthorizedResult>(response);
    
  }
  
 

}