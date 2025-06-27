using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Google.Apis.Auth;
using MaClasse.Shared.Models;
using MaClasse.Shared.Models.Lesson;
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

public class AuthControllerTests_GoogleLogin
{
  private readonly IUserServiceRattachment _userServiceRattachment;
  private readonly Mock<IConfiguration> _mockIConfiguration;
  private readonly Mock<IValidateGoogleTokenService> _mockValidateToken;
  private readonly Mock<IAuthRepository> _mockAuthRepo;
  private readonly Mock<ISessionRepository> _mockSessionRepo;
  private readonly Mock<IUserServiceRattachment> _mockUserServiceRattachment;
  private readonly Mock<IGenerateIdRole> _mockGenerateIdRole;
  private readonly Mock<ICreateDataService> _mockCreateDataService;
  private readonly Mock<IDeleteUserService> _mockDeleteUserService;
  
  private readonly AuthController _controller;

  public AuthControllerTests_GoogleLogin()
  {
    _mockIConfiguration = new Mock<IConfiguration>();
    _mockValidateToken = new Mock<IValidateGoogleTokenService>();
    _mockAuthRepo = new Mock<IAuthRepository>();
    _mockSessionRepo = new Mock<ISessionRepository>();
    _mockUserServiceRattachment = new Mock<IUserServiceRattachment>();
    _mockGenerateIdRole = new Mock<IGenerateIdRole>();
    _mockCreateDataService = new Mock<ICreateDataService>();
    _mockDeleteUserService = new Mock<IDeleteUserService>();
    
    _controller = new AuthController(
      _mockIConfiguration.Object,
      _mockValidateToken.Object,
      _mockAuthRepo.Object,
      _mockSessionRepo.Object,
      _mockUserServiceRattachment.Object,
      _mockGenerateIdRole.Object,
      _mockCreateDataService.Object,
      _mockDeleteUserService.Object
    );
  }
  
  [Fact]
  public async Task GoogleLogin_ShouldReturnUnauthorized_WhenTokenIsInvalid()
  {
    _mockValidateToken.Setup(s => s.ValidateGoogleToken(It.IsAny<string>())).ReturnsAsync((GoogleJsonWebSignature.Payload)null);

    var result = await _controller.GoogleLogin(new GoogleTokenRequest { Token = "invalid" });

    Assert.IsType<UnauthorizedObjectResult>(result);
  }
  
  [Fact]
  public async Task GoogleLogin_ShouldReturnOk_WhenUserExists_AndSessionCreated()
  {
    // Arrange
    
    //* Mon service avec le mock de l'appel à la BDD
    var realUserServiceRattachment = new UserServiceRattachment(
      _mockAuthRepo.Object
    );

    //* Constrcution de mon AuthController avec les mock ou non
    var _authController = new AuthController(
      _mockIConfiguration.Object,
      _mockValidateToken.Object,
      _mockAuthRepo.Object,
      _mockSessionRepo.Object,
      realUserServiceRattachment,
      _mockGenerateIdRole.Object,
      _mockCreateDataService.Object,
      _mockDeleteUserService.Object
    );
    
    //* Obtention du payload Google
    var token = "valid-token";
    var request = new GoogleTokenRequest { Token = token };
        
    var payload = new GoogleJsonWebSignature.Payload
    {
      Subject = "google-user-id",
      Email = "test@gmail.com",
      Name = "Test User",
      GivenName = "Test",
      FamilyName = "User",
      Picture = "pic-url"
    };

    _mockValidateToken.Setup(s => s.ValidateGoogleToken(token)).ReturnsAsync(payload);
    
    //* Obtention d'un utilisateur existant
    var existingUser = new UserProfile
      { 
        CreatedAt = new DateTime(2025, 06, 27, 12, 00, 00),
        Email = "test@gmail.com",
        FamilyName = "family",
        GivenName = "given",
        Id = "google-user-id",
        IdRole = "IdRole",
        Name = "given family",
        Picture = "pic-url",
        Role = "User",
        UpdatedAt = new DateTime(2025, 06, 27, 12, 00, 00),
        Zone = "A"
      };
    _mockAuthRepo.Setup(r => r.GetOneUserByGoogleId(payload.Subject)).ReturnsAsync(existingUser);
    
    //* Pas d’ancienne session
    _mockSessionRepo.Setup(r => r.GetSessionByUserId(existingUser.Id)).ReturnsAsync((SessionData)null); // Pas d’ancienne session
    
    //* Création de la nouvelle session
    var sessionSaveLogin = new SessionData
    {
      Token = "session-token",
      UserId = existingUser.Id,
      Role = existingUser.Role,
      Expiration = DateTime.UtcNow.AddHours(3)
    };
    _mockSessionRepo.Setup(r => r.SaveNewSession(It.IsAny<SessionData>())).ReturnsAsync(sessionSaveLogin);
    
    
    //* Récupération d'un nouveau calendrier
    _mockCreateDataService.Setup(s => s.GetDataScheduler(existingUser.Id)).ReturnsAsync(new Scheduler());
    
    
    //* Récupération des données de l'utilisateur avec les rattachments
    _mockAuthRepo.Setup(s => s.GetRattachmentByIdRole(existingUser.IdRole)).ReturnsAsync(new List<Rattachment>());
    
    
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
          Email = "test@gmail.com",
          FamilyName = "family",
          GivenName = "given",
          Id = "google-user-id",
          IdRole = "IdRole",
          Name = "given family",
          Picture = "pic-url",
          Role = "User",
          UpdatedAt = new DateTime(2025, 06, 27, 12, 00, 00),
          Zone = "A"
        }
      }
    };

    // Act
    var result = await _authController.GoogleLogin(request);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    okResult.Value.Should().BeEquivalentTo(responseData);
  }

  [Fact]
  public async Task GoogleLogin_ShouldReturnOk_WhenUserExists_AndSessionAlreadyexists()
  {
     // Arrange
    
    //* Mon service avec le mock de l'appel à la BDD
    var realUserServiceRattachment = new UserServiceRattachment(
      _mockAuthRepo.Object
    );

    //* Constrcution de mon AuthController avec les mock ou non
    var _authController = new AuthController(
      _mockIConfiguration.Object,
      _mockValidateToken.Object,
      _mockAuthRepo.Object,
      _mockSessionRepo.Object,
      realUserServiceRattachment,
      _mockGenerateIdRole.Object,
      _mockCreateDataService.Object,
      _mockDeleteUserService.Object
    );
    
    //* Obtention du payload Google
    var token = "valid-token";
    var request = new GoogleTokenRequest { Token = token };
        
    var payload = new GoogleJsonWebSignature.Payload
    {
      Subject = "google-user-id",
      Email = "test@gmail.com",
      Name = "Test User",
      GivenName = "Test",
      FamilyName = "User",
      Picture = "pic-url"
    };

    _mockValidateToken.Setup(s => s.ValidateGoogleToken(token)).ReturnsAsync(payload);
    
    //* Obtention d'un utilisateur existant
    var existingUser = new UserProfile
      { 
        CreatedAt = new DateTime(2025, 06, 27, 12, 00, 00),
        Email = "test@gmail.com",
        FamilyName = "family",
        GivenName = "given",
        Id = "google-user-id",
        IdRole = "IdRole",
        Name = "given family",
        Picture = "pic-url",
        Role = "User",
        UpdatedAt = new DateTime(2025, 06, 27, 12, 00, 00),
        Zone = "A"
      };
    _mockAuthRepo.Setup(r => r.GetOneUserByGoogleId(payload.Subject)).ReturnsAsync(existingUser);
    
    //* J'ai une ancienne session
    var existingSession = new SessionData
    {
      Token = "existing-session-token",
      UserId = "google-user-id",
      Role = "",
      Expiration = new DateTime(2025, 06, 27, 14, 00, 00)
    };
    _mockSessionRepo.Setup(s => s.DeleteSessionData(existingSession)).ReturnsAsync(existingSession);
    
    _mockSessionRepo.Setup(r => r.GetSessionByUserId(existingUser.Id)).ReturnsAsync(existingSession); // Pas d’ancienne session
    
    //* Création de la nouvelle session
    var sessionSaveLogin = new SessionData
    {
      Token = "new-session-token",
      UserId = existingUser.Id,
      Role = existingUser.Role,
      Expiration = new DateTime(2025, 06, 27, 15, 00, 00)
    };
    _mockSessionRepo.Setup(r => r.SaveNewSession(It.IsAny<SessionData>())).ReturnsAsync(sessionSaveLogin);
    
    
    //* Récupération d'un nouveau calendrier
    _mockCreateDataService.Setup(s => s.GetDataScheduler(existingUser.Id)).ReturnsAsync(new Scheduler());
    
    //* Récupération des données de l'utilisateur avec les rattachments
    _mockAuthRepo.Setup(s => s.GetRattachmentByIdRole(existingUser.IdRole)).ReturnsAsync(new List<Rattachment>());
        
    var responseData = new AuthReturn
    {
      IdSession = "new-session-token",
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
          Email = "test@gmail.com",
          FamilyName = "family",
          GivenName = "given",
          Id = "google-user-id",
          IdRole = "IdRole",
          Name = "given family",
          Picture = "pic-url",
          Role = "User",
          UpdatedAt = new DateTime(2025, 06, 27, 12, 00, 00),
          Zone = "A"
        }
      }
    };

    // Act
    var result = await _authController.GoogleLogin(request);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    okResult.Value.Should().BeEquivalentTo(responseData);
  }
  
  [Fact]
  public async Task GoogleLogin_ShouldReturnOk_WhenUserExists_AndSessionAlreadyexists_ErrorOnDeleteSession()
  {
     // Arrange
    
    //* Mon service avec le mock de l'appel à la BDD
    var realUserServiceRattachment = new UserServiceRattachment(
      _mockAuthRepo.Object
    );

    //* Constrcution de mon AuthController avec les mock ou non
    var _authController = new AuthController(
      _mockIConfiguration.Object,
      _mockValidateToken.Object,
      _mockAuthRepo.Object,
      _mockSessionRepo.Object,
      realUserServiceRattachment,
      _mockGenerateIdRole.Object,
      _mockCreateDataService.Object,
      _mockDeleteUserService.Object
    );
    
    //* Obtention du payload Google
    var token = "valid-token";
    var request = new GoogleTokenRequest { Token = token };
        
    var payload = new GoogleJsonWebSignature.Payload
    {
      Subject = "google-user-id",
      Email = "test@gmail.com",
      Name = "Test User",
      GivenName = "Test",
      FamilyName = "User",
      Picture = "pic-url"
    };

    _mockValidateToken.Setup(s => s.ValidateGoogleToken(token)).ReturnsAsync(payload);
    
    //* Obtention d'un utilisateur existant
    var existingUser = new UserProfile
      { 
        CreatedAt = new DateTime(2025, 06, 27, 12, 00, 00),
        Email = "test@gmail.com",
        FamilyName = "family",
        GivenName = "given",
        Id = "google-user-id",
        IdRole = "IdRole",
        Name = "given family",
        Picture = "pic-url",
        Role = "User",
        UpdatedAt = new DateTime(2025, 06, 27, 12, 00, 00),
        Zone = "A"
      };
    _mockAuthRepo.Setup(r => r.GetOneUserByGoogleId(payload.Subject)).ReturnsAsync(existingUser);
    
    //* J'ai une ancienne session
    var existingSession = new SessionData
    {
      Token = "existing-session-token",
      UserId = "google-user-id",
      Role = "",
      Expiration = new DateTime(2025, 06, 27, 14, 00, 00)
    };
    _mockSessionRepo.Setup(s => s.DeleteSessionData(existingSession)).ReturnsAsync((SessionData)null);
    
    // Act
    var result = await _authController.GoogleLogin(request);

    // Assert
    Assert.IsType<UnauthorizedResult>(result);

  }
  
  [Fact]
  public async Task GoogleLogin_ShouldReturnOk_WhenUserExists_ErrorOnSessionCreating()
  {
    // Arrange
    
    //* Mon service avec le mock de l'appel à la BDD
    var realUserServiceRattachment = new UserServiceRattachment(
      _mockAuthRepo.Object
    );

    //* Constrcution de mon AuthController avec les mock ou non
    var _authController = new AuthController(
      _mockIConfiguration.Object,
      _mockValidateToken.Object,
      _mockAuthRepo.Object,
      _mockSessionRepo.Object,
      realUserServiceRattachment,
      _mockGenerateIdRole.Object,
      _mockCreateDataService.Object,
      _mockDeleteUserService.Object
    );
    
    //* Obtention du payload Google
    var token = "valid-token";
    var request = new GoogleTokenRequest { Token = token };
        
    var payload = new GoogleJsonWebSignature.Payload
    {
      Subject = "google-user-id",
      Email = "test@gmail.com",
      Name = "Test User",
      GivenName = "Test",
      FamilyName = "User",
      Picture = "pic-url"
    };

    _mockValidateToken.Setup(s => s.ValidateGoogleToken(token)).ReturnsAsync(payload);
    
    //* Obtention d'un utilisateur existant
    var existingUser = new UserProfile
      { 
        CreatedAt = new DateTime(2025, 06, 27, 12, 00, 00),
        Email = "test@gmail.com",
        FamilyName = "family",
        GivenName = "given",
        Id = "google-user-id",
        IdRole = "IdRole",
        Name = "given family",
        Picture = "pic-url",
        Role = "User",
        UpdatedAt = new DateTime(2025, 06, 27, 12, 00, 00),
        Zone = "A"
      };
    _mockAuthRepo.Setup(r => r.GetOneUserByGoogleId(payload.Subject)).ReturnsAsync(existingUser);
    
    //* Pas d’ancienne session
    _mockSessionRepo.Setup(r => r.GetSessionByUserId(existingUser.Id)).ReturnsAsync((SessionData)null); // Pas d’ancienne session
    
    //* Création de la nouvelle session
    var sessionSaveLogin = new SessionData
    {
      Token = "session-token",
      UserId = existingUser.Id,
      Role = existingUser.Role,
      Expiration = DateTime.UtcNow.AddHours(3)
    };
    _mockSessionRepo.Setup(r => r.SaveNewSession(It.IsAny<SessionData>())).ReturnsAsync((SessionData)null);
    
    // Act
    var result = await _authController.GoogleLogin(request);

    // Assert
    Assert.IsType<UnauthorizedResult>(result);
  }
  
  [Fact]
  public async Task GoogleLogin_ShouldReturnOk_WhenUserNotExists_AndSessionCreated()
  {
    // Arrange
    
    //* Mon service avec le mock de l'appel à la BDD
    var realUserServiceRattachment = new UserServiceRattachment(
      _mockAuthRepo.Object
    );

    //* Constrcution de mon AuthController avec les mock ou non
    var _authController = new AuthController(
      _mockIConfiguration.Object,
      _mockValidateToken.Object,
      _mockAuthRepo.Object,
      _mockSessionRepo.Object,
      realUserServiceRattachment,
      _mockGenerateIdRole.Object,
      _mockCreateDataService.Object,
      _mockDeleteUserService.Object
    );
    
    //* Obtention du payload Google
    var token = "valid-token";
    var request = new GoogleTokenRequest { Token = token };
        
    var payload = new GoogleJsonWebSignature.Payload
    {
      Subject = "google-user-id",
      Email = "test@gmail.com",
      Name = "Test User",
      GivenName = "Test",
      FamilyName = "User",
      Picture = "pic-url"
    };

    _mockValidateToken.Setup(s => s.ValidateGoogleToken(token)).ReturnsAsync(payload);
    
    //* L'utilisateur n'existe pas
    _mockAuthRepo.Setup(r => r.GetOneUserByGoogleId(payload.Subject)).ReturnsAsync((UserProfile)null);
    
    //* Enregistrement du nouvel utilisateur

    _mockGenerateIdRole.Setup(r => r.GenerateIdAsync(6)).ReturnsAsync("ABDCEF");
    
    UserProfile? capturedUser = null;
    
    _mockAuthRepo
      .Setup(r => r.AddUser(It.IsAny<UserProfile>()))
      .Callback<UserProfile>(u => capturedUser = u)
      .ReturnsAsync((UserProfile u) => u);
    
    //* Création du scheduler et du lessonBook
    _mockCreateDataService.Setup(s => s.CreateDataScheduler(It.IsAny<string>())).ReturnsAsync(new Scheduler());
    _mockCreateDataService.Setup(r => r.CreateDateLessonBook(It.IsAny<string>())).ReturnsAsync(new LessonBook());
    
    //* Création de la nouvelle session
    var sessionSaveLogin = new SessionData
    {
      Token = "session-token",
      UserId = payload.Subject,
      Role = "",
      Expiration = new DateTime(2025, 06, 27, 15, 00, 00)
    };
    _mockSessionRepo.Setup(r => r.SaveNewSession(It.IsAny<SessionData>())).ReturnsAsync(sessionSaveLogin);
    
    //* Récupération des données de l'utilisateur avec les rattachments
    _mockAuthRepo.Setup(s => s.GetRattachmentByIdRole(It.IsAny<string>())).ReturnsAsync(new List<Rattachment>());
    
    // Act
    var result = await _authController.GoogleLogin(request);
    
    // Assert
    
    var responseData = new AuthReturn
    {
      IdSession = "session-token",
      IsNewUser = true,
      Scheduler = new Scheduler(),
      UserWithRattachment = new UserWithRattachment
      {
        AccessToken = "valid-token",
        AsDirecteur = new List<Rattachment>(),
        AsProfesseur = new List<Rattachment>(),
        UserProfile = new UserProfile
        {
          CreatedAt = null,
          Email = "test@gmail.com",
          FamilyName = "User",
          GivenName = "Test",
          Id = "google-user-id",
          IdRole = "ABDCEF",
          Name = "Test User",
          Picture = "pic-url",
          Role = "",
          UpdatedAt = null,
          Zone = ""
        }
      }
    };
    
    
    var okResult = Assert.IsType<OkObjectResult>(result);
    okResult.Value.Should().BeEquivalentTo(responseData);
  }

  [Fact]
  public async Task GoogleLogin_ShouldReturnOk_WhenUserNotExists_SessionSaveSignUpIsNull()
  {
    // Arrange
    
    //* Mon service avec le mock de l'appel à la BDD
    var realUserServiceRattachment = new UserServiceRattachment(
      _mockAuthRepo.Object
    );

    //* Constrcution de mon AuthController avec les mock ou non
    var _authController = new AuthController(
      _mockIConfiguration.Object,
      _mockValidateToken.Object,
      _mockAuthRepo.Object,
      _mockSessionRepo.Object,
      realUserServiceRattachment,
      _mockGenerateIdRole.Object,
      _mockCreateDataService.Object,
      _mockDeleteUserService.Object
    );
    
    //* Obtention du payload Google
    var token = "valid-token";
    var request = new GoogleTokenRequest { Token = token };
        
    var payload = new GoogleJsonWebSignature.Payload
    {
      Subject = "google-user-id",
      Email = "test@gmail.com",
      Name = "Test User",
      GivenName = "Test",
      FamilyName = "User",
      Picture = "pic-url"
    };

    _mockValidateToken.Setup(s => s.ValidateGoogleToken(token)).ReturnsAsync(payload);
    
    //* L'utilisateur n'existe pas
    _mockAuthRepo.Setup(r => r.GetOneUserByGoogleId(payload.Subject)).ReturnsAsync((UserProfile)null);
    
    //* Enregistrement du nouvel utilisateur

    _mockGenerateIdRole.Setup(r => r.GenerateIdAsync(6)).ReturnsAsync("ABDCEF");
    
    UserProfile? capturedUser = null;
    
    _mockAuthRepo
      .Setup(r => r.AddUser(It.IsAny<UserProfile>()))
      .Callback<UserProfile>(u => capturedUser = u)
      .ReturnsAsync((UserProfile u) => u);
    
    //* Création du scheduler et du lessonBook
    _mockCreateDataService.Setup(s => s.CreateDataScheduler(It.IsAny<string>())).ReturnsAsync(new Scheduler());
    _mockCreateDataService.Setup(r => r.CreateDateLessonBook(It.IsAny<string>())).ReturnsAsync(new LessonBook());
    
    //* Création de la nouvelle session
    var sessionSaveLogin = new SessionData
    {
      Token = "session-token",
      UserId = payload.Subject,
      Role = "",
      Expiration = new DateTime(2025, 06, 27, 15, 00, 00)
    };
    _mockSessionRepo.Setup(r => r.SaveNewSession(It.IsAny<SessionData>())).ReturnsAsync((SessionData)null);
    
    // Act
    var result = await _authController.GoogleLogin(request);
    
    // Assert
    Assert.IsType<UnauthorizedResult>(result);

  }

}