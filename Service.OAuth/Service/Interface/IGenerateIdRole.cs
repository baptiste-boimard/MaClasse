namespace Service.OAuth.Service.Interface;

public interface IGenerateIdRole
{
  Task<string> GenerateIdAsync(int length = 6);
  string Generate(int length);
  Task<bool> VerifyExistingIdRole(string idRole);
}