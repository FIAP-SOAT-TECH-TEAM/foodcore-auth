using Foodcore.Auth.DTO;
using Foodcore.Auth.Model;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json.Serialization;

namespace Foodcore.Auth.Config
{
  public class AuthResponseDTOExample : OpenApiExample<AuthResponseDTO>
  {
    public override IOpenApiExample<AuthResponseDTO> Build(NamingStrategy namingStrategy)
    {
      Examples.Add(
          OpenApiExampleResolver.Resolve(
              "AuthResponseDTOExample",
              new AuthResponseDTO
              {
                IdToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
                AccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
                RefreshToken = "dGhpc2lzYXJlZnJlc2h0b2tlbg==",
                ExpiresIn = 3600,
                TokenType = "Bearer"
              },
              namingStrategy
          )
      );

      return this;
    }
  }

  public class CustomerAuthDTOExample : OpenApiExample<CustomerAuthDTO>
  {
    public override IOpenApiExample<CustomerAuthDTO> Build(NamingStrategy namingStrategy)
    {
      Examples.Add(
          OpenApiExampleResolver.Resolve(
              "CustomerAuthDTOExample",
              new CustomerAuthDTO
              {
                Email = "cliente@exemplo.com",
                Cpf = "12345678901"
              },
              namingStrategy
          )
      );

      return this;
    }
  }

  public class ErrorDTOExample : OpenApiExample<ErrorDTO>
  {
    public override IOpenApiExample<ErrorDTO> Build(NamingStrategy namingStrategy)
    {
      Examples.Add(
          OpenApiExampleResolver.Resolve(
              "ErrorDTOExample",
              new ErrorDTO
              {
                Timestamp = DateTime.UtcNow,
                Status = 500,
                Message = "Ocorreu um erro inesperado.",
                Path = "/auth/login"
              },
              namingStrategy
          )
      );

      return this;
    }
  }

  public class UserCreatedResponseDTOExample : OpenApiExample<UserCreatedResponseDTO>
  {
    public override IOpenApiExample<UserCreatedResponseDTO> Build(NamingStrategy namingStrategy)
    {
      Examples.Add(
          OpenApiExampleResolver.Resolve(
              "UserCreatedResponseDTOExample",
              new UserCreatedResponseDTO
              {
                Message = "Usuário criado com sucesso.",
                UserSub = "c1a2b3c4-d5e6-7890-abcd-ef1234567890"
              },
              namingStrategy
          )
      );

      return this;
    }
  }

  public class UserCreateDTOExample : OpenApiExample<UserCreateDTO>
  {
    public override IOpenApiExample<UserCreateDTO> Build(NamingStrategy namingStrategy)
    {
      Examples.Add(
          OpenApiExampleResolver.Resolve(
              "UserCreateDTOExample",
              new UserCreateDTO
              {
                Name = "João da Silva",
                Email = "joao.silva@exemplo.com",
                Password = "SenhaForte123!",
                Cpf = "12345678901"
              },
              namingStrategy
          )
      );

      return this;
    }
  }

  public class UserDetailsDTOExample : OpenApiExample<UserDetailsDTO>
  {
    public override IOpenApiExample<UserDetailsDTO> Build(NamingStrategy namingStrategy)
    {
      Examples.Add(
          OpenApiExampleResolver.Resolve(
              "UserDetailsDTOExample",
              new UserDetailsDTO
              {
                Subject = "c1a2b3c4-d5e6-7890-abcd-ef1234567890",
                Name = "João da Silva",
                Email = "joao.silva@exemplo.com",
                Cpf = "12345678901",
                Role = Role.CUSTOMER.ToString(),
                CreatedAt = DateTime.UtcNow
              },
              namingStrategy
          )
      );

      return this;
    }
  }
}
