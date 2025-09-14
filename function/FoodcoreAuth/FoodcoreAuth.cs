using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Foodcore.Auth.DTO;
using Foodcore.Auth.Model;
using Amazon.CognitoIdentityProvider;
using Foodcore.Auth.Exceptions;
using Foodcore.Auth.Mapper;
using Foodcore.Auth.Services;
using Foodcore.Auth.Presenter;
using MsAzureFuncWorker = Microsoft.Azure.Functions.Worker.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Azure.Functions.Worker.Http;
using Amazon.CognitoIdentityProvider.Model;

namespace Foodcore.Auth
{
    public class FoodcoreAuth(
        ILogger<FoodcoreAuth> logger,
        IAmazonCognitoIdentityProvider cognito,
        CognitoSettings settings)
    {
        private readonly ILogger<FoodcoreAuth> _logger = logger;
        private readonly IAmazonCognitoIdentityProvider _cognito = cognito;
        private readonly CognitoSettings _settings = settings;

        [Function("CreateUser")]
        public async Task<IActionResult> CreateUser(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "users")]
        [MsAzureFuncWorker.FromBody] UserCreateDTO userCreateDTO)
        {
            try
            {
                var user = UserMapper.ToModel(userCreateDTO);

                var existingUser = await CognitoService.GetUserByEmailOrCpfAsync(
                    _cognito,
                    _settings,
                    user.Email!.Value,
                    user.Cpf!.Value
                );
                if (existingUser != null)
                    throw new BusinessException("Usuário com este email ou CPF já existe.");

                var password = UsuarioService.GetUserPassword(user);

                var createdUser = await CognitoService.CreateUser(
                    _cognito,
                    _settings,
                    user
                );

                await CognitoService.UpdateUserPassword(
                    _cognito,
                    _settings,
                    createdUser.User.Username,
                    password
                );

                return new OkObjectResult(UserPresenter.ToUserCreatedResponse(createdUser));
            }
            catch (BusinessException ex)
            {
                _logger.LogDebug(ex, "Erro de negócio ao criar usuário.");

                var errorDto = CommonPresenter.ToErrorDTO(ex);

                return new BadRequestObjectResult(errorDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar usuário.");
                var errorDto = CommonPresenter.ToErrorDTO(ex);

                return new ObjectResult(errorDto) { StatusCode = 500 };
            }
        }

        [Function("ValidateToken")]
        public async Task<IActionResult> ValidateToken(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "auth/validate")]
        HttpRequestData httpRequestData)
        {
            try
            {
                var accessToken = httpRequestData.Query["access_token"];

                if (string.IsNullOrWhiteSpace(accessToken))
                    throw new SecurityTokenException("Access token não fornecido.");

                var jwtToken = await CognitoService.ValidateToken(_settings, accessToken);

                var jwtTokenSubject = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

                if (string.IsNullOrWhiteSpace(jwtTokenSubject))
                    throw new SecurityTokenException("Token inválido: 'sub' claim não encontrado.");

                var user = await CognitoService.GetUserBySubAsync(_cognito, _settings, jwtTokenSubject) ?? throw new NotAuthorizedException("Usuário não encontrado.");

                var response = UserPresenter.ToUserDetailsDTO(user, jwtToken.Claims);

                return new OkObjectResult(response);
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogWarning(ex, "Token inválido.");
                var errorDto = CommonPresenter.ToErrorDTO(ex);

                return new UnauthorizedObjectResult(errorDto);
            }
            catch (NotAuthorizedException ex)
            {
                _logger.LogWarning(ex, "Usuário não autorizado.");
                var errorDto = CommonPresenter.ToErrorDTO(ex);

                return new UnauthorizedObjectResult(errorDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar token.");
                var errorDto = CommonPresenter.ToErrorDTO(ex);

                return new ObjectResult(errorDto) { StatusCode = 500 };
            }
        }

        [Function("AuthCustomer")]
        public async Task<IActionResult> AuthCustomer(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth/customer/login")]
        [MsAzureFuncWorker.FromBody] CustomerAuthDTO customerAuthDTO)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(customerAuthDTO.Email) && string.IsNullOrWhiteSpace(customerAuthDTO.Cpf))
                    throw new BusinessException("Email ou CPF devem ser fornecidos.");

                var existingUser = await CognitoService.GetUserByEmailOrCpfAsync(
                    _cognito,
                    _settings,
                    customerAuthDTO.Email,
                    customerAuthDTO.Cpf
                ) ?? throw new BusinessException("Usuário com este email ou CPF não existe.");

                var userIsCustomer = existingUser.Attributes.Any(attr => attr.Name == "custom:role" && attr.Value == "CUSTOMER");

                if (!userIsCustomer)
                    throw new BusinessException("Usuário não é um cliente.");

                var password = Environment.GetEnvironmentVariable("DEFAULT_CUSTOMER_PASSWORD") ?? throw new InvalidOperationException("Default customer password not set.");

                var token = await CognitoService.AuthenticateUserAsync(
                    _cognito,
                    _settings,
                    existingUser.Username,
                    password
                );

                return new OkObjectResult(AuthPresenter.ToAuthResponseDTO(token));
            }
            catch (BusinessException ex)
            {
                _logger.LogDebug(ex, "Erro de negócio ao autenticar cliente.");

                var errorDto = CommonPresenter.ToErrorDTO(ex);

                return new BadRequestObjectResult(errorDto);
            }
            catch (NotAuthorizedException ex)
            {
                _logger.LogWarning(ex, "Usuário não autorizado.");
                var errorDto = CommonPresenter.ToErrorDTO(ex);

                return new UnauthorizedObjectResult(errorDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao autenticar cliente.");
                var errorDto = CommonPresenter.ToErrorDTO(ex);

                return new ObjectResult(errorDto) { StatusCode = 500 };
            }
        }

    }
}