﻿using MaClasse.Shared.Models;
using MaClasse.Shared.Models.ViewDashboard;
using Microsoft.AspNetCore.Mvc;
using Service.OAuth.Interfaces;

namespace Service.OAuth.Controller;

[ApiController]
[Route("api")]
public class RattachmentController: ControllerBase
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IAuthRepository _authRepository;
    private readonly IRattachmentRepository _rattachmentRepository;

    public RattachmentController(
        ISessionRepository sessionRepository,
        IAuthRepository authRepository,
        IRattachmentRepository rattachmentRepository)
    {
        _sessionRepository = sessionRepository;
        _authRepository = authRepository;
        _rattachmentRepository = rattachmentRepository;
    }

    [HttpPost]
    [Route("add-rattachment")]
    public async Task<IActionResult> AddRattachment([FromBody] RattachmentRequest request)
    {
        var existingSession = await _sessionRepository.GetUserIdByCookies(request.IdSession);

        if (existingSession != null)
        {
            var user = await _authRepository.GetOneUserByGoogleId(existingSession.UserId);

            if (user != null)
            {
                if (!string.IsNullOrWhiteSpace(request.IdDirecteur))
                {
                    //* J'ajoute dans la table Rattachment l'idRole du user et l'idDirecteur
                    var rattachment = new Rattachment
                    {
                        IdProfesseur = user.IdRole,
                        IdDirecteur = request.IdDirecteur
                    };

                    //* Recherche si le directeur existe
                    var isexistingDirect = await _authRepository.CheckIdRole(request.IdDirecteur);

                    if (!isexistingDirect)
                    {
                        return Conflict("Ce Directeur/Directrice n'est pas inscrit sur la plateforme");
                    }

                    //* Recherche un tel rattachement existe deja
                    var existingRattachment = await _rattachmentRepository.GetRattachment(rattachment);

                    if (existingRattachment.Count > 0)
                    {
                        //* Envoi d'un message d'erreur pour dire que ce rattachement existe déjà
                        return Conflict("Vous êtes déjà rattaché à cette personne");

                    }

                    var addRattachment = await _rattachmentRepository.AddRattachment(rattachment);

                    if (addRattachment != null)
                    {
                        //* On renvoi la liste des directeur mise a jour
                        var listRattachment = await _rattachmentRepository.GetRattachmentDirect(user.IdRole);

                        return Ok(listRattachment);
                    }
                }

                if (!string.IsNullOrWhiteSpace(request.IdProfesseur))
                {

                    //* J'ajoute dans la table Rattachment l'idRole du user et l'idProfesseur
                    {
                        var rattachement = new Rattachment
                        {
                            IdProfesseur = request.IdProfesseur,
                            IdDirecteur = user.IdRole
                        };

                        //* Recherche si le professeur existe
                        var isexistingProf = await _authRepository.CheckIdRole(request.IdProfesseur);

                        if (!isexistingProf)
                        {
                            return Conflict("Ce Professeur(e) n'est pas inscrit sur la plateforme");
                        }

                        //* Recherche un tel rattachement existe deja
                        var existingRattachment = await _rattachmentRepository.GetRattachment(rattachement);

                        if (existingRattachment.Count > 0)
                        {
                            //* Envoi d'un message d'erreur pour dire que ce rattachement existe déjà
                            return Conflict("Vous êtes déjà rattaché à cette personne");

                        }

                        var addRattachment = await _rattachmentRepository.AddRattachment(rattachement);

                        if (addRattachment != null)
                        {
                            var listRattachment = await _rattachmentRepository.GetRattachmentProf(user.IdRole);

                            return Ok(listRattachment);
                        }
                    }

                    return Unauthorized();
                }

                return Unauthorized();
            }
            else
            {
                return Unauthorized();
            }
        }
        return Unauthorized();
    }

    [HttpPost]
    [Route("delete-rattachment")]
    public async Task<IActionResult> DeleteRattachment([FromBody] RattachmentRequest request)
    {
        var existingSession = await _sessionRepository.GetUserIdByCookies(request.IdSession);

        if (existingSession != null)
        {
            var user = await _authRepository.GetOneUserByGoogleId(existingSession.UserId);

            if (user != null)
            {
                if (request.IdDirecteur != null)
                {
                    var rattachment = new Rattachment
                    {
                        IdProfesseur = user.IdRole,
                        IdDirecteur = request.IdDirecteur
                    };
                    
                    //* Vérification que ce rattachement existe
                    var existingRattachment = await _rattachmentRepository.GetRattachment(rattachment);
                    
                    if (existingRattachment.Count == 0)
                    {
                        return Conflict("Ce rattachement n'existe pas");
                    }
                    
                    var deleteRattachment = await _rattachmentRepository.DeleteRattachment(rattachment);

                    if (deleteRattachment != null)
                    {
                        var listRattachment = await _rattachmentRepository.GetRattachmentProf(user.IdRole);
                        
                        return Ok(listRattachment);
                    }
                }

                if (request.IdProfesseur != null)
                {
                    var rattachement = new Rattachment
                    {
                        IdProfesseur = request.IdProfesseur,
                        IdDirecteur = user.IdRole
                    };
                    
                    //* Vérification que ce rattachement existe
                    var existingRattachment = await _rattachmentRepository.GetRattachment(rattachement);
                    
                    if (existingRattachment.Count == 0)
                    {
                        return Conflict("Ce rattachement n'existe pas");
                    }
                    
                    var deleteRattachment = await _rattachmentRepository.DeleteRattachment(existingRattachment.FirstOrDefault());

                    if (deleteRattachment != null)
                    {
                        var listRattachment = await _rattachmentRepository.GetRattachmentProf(user.IdRole);
                        
                        return Ok(listRattachment);
                    }
                }
                
                return Unauthorized();
            }
            
            return Unauthorized();
        }
        
        return Unauthorized();
    }

    [HttpPost]
    [Route("get-rattachments-infos")]
    public async Task<IActionResult> GetRattachments([FromBody] ViewDashboardRequest request)
    {
        //* Controle de l'existence de la session
        var existingSession = 
            await _sessionRepository.GetUserIdByCookies(request.IdSession);

        if (existingSession == null) return Unauthorized();

        // je recupere les infos des utilisateur grace à leur IdRole
        var rattachmentsInfos = await _authRepository.GetUsersByIdRoles(request.AsDirecteur);

        if (rattachmentsInfos == null) return BadRequest();
        
        return Ok(rattachmentsInfos);
    } 
}