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
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using System.Net;

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
        [OpenApiOperation(operationId: "CreateUser", tags: ["Users"])]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(UserCreateDTO), Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(UserCreatedResponseDTO), Description = "Usuário criado com sucesso")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(ErrorDTO), Description = "Erro de negócio")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(ErrorDTO), Description = "Erro interno")]
        public async Task<IActionResult> CreateUser(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "users")]
        HttpRequestData httpRequestData, [MsAzureFuncWorker.FromBody] UserCreateDTO userCreateDTO)
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

                var responseStatusCode = (int)HttpStatusCode.BadRequest;
                var errorDto = CommonPresenter.ToErrorDTO(ex, responseStatusCode, httpRequestData.Url.AbsolutePath);

                return new BadRequestObjectResult(errorDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar usuário.");

                var responseStatusCode = (int)HttpStatusCode.InternalServerError;
                var errorDto = CommonPresenter.ToErrorDTO(ex, responseStatusCode, httpRequestData.Url.AbsolutePath);

                return new ObjectResult(errorDto) { StatusCode = responseStatusCode };
            }
        }

        [Function("ValidateToken")]
        [OpenApiIgnore]
        [OpenApiOperation(operationId: "ValidateToken", tags: ["Auth"])]
        [OpenApiParameter(name: "access_token", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "Token JWT de acesso")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(UserDetailsDTO), Description = "Token validado com sucesso")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Unauthorized, contentType: "application/json", bodyType: typeof(ErrorDTO), Description = "Token inválido ou usuário não autorizado")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(ErrorDTO), Description = "Erro interno")]
        public async Task<IActionResult> ValidateToken(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "validate")]
        HttpRequestData httpRequestData)
        {
            try
            {
                var accessToken = httpRequestData.Query["access_token"];

                var url = httpRequestData.Query["url"];
                if (string.IsNullOrWhiteSpace(url))
                    throw new NotAuthorizedException("URL não fornecida.");

                var httpMethod = httpRequestData.Query["http_method"];
                if (string.IsNullOrWhiteSpace(httpMethod))
                    throw new NotAuthorizedException("HTTP method não fornecido.");
                httpMethod = httpMethod.ToUpperInvariant();

                if (string.IsNullOrWhiteSpace(accessToken))
                {
                    UsuarioService.UserCanAccessUrl(url, httpMethod);
                    
                    var emptyResponse = UserPresenter.ToUserDetailsDTO();
                    return new OkObjectResult(emptyResponse);
                }
                    
                var jwtToken = await CognitoService.ValidateToken(_settings, accessToken);
                var jwtTokenSubject = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
                if (string.IsNullOrWhiteSpace(jwtTokenSubject))
                    throw new SecurityTokenException("Token inválido: 'sub' claim não encontrado.");

                var user = await CognitoService.GetUserBySubAsync(_cognito, _settings, jwtTokenSubject) ?? throw new NotAuthorizedException("Usuário não encontrado.");
                var userRole = user.Attributes.Find(attr => attr.Name == "custom:role")!.Value!;
                if (string.IsNullOrWhiteSpace(userRole) || !Enum.TryParse<Role>(userRole, out var role))
                    throw new NotAuthorizedException("Role não encontrada ou inválida.");
                UsuarioService.UserCanAccessUrl(url, httpMethod, role);

                var response = UserPresenter.ToUserDetailsDTO(user, jwtToken.Claims);
                return new OkObjectResult(response);
            }
            catch (AccessDeniedException ex)
            {
                _logger.LogWarning(ex, "Acesso negado ao recurso.");

                var responseStatusCode = (int)HttpStatusCode.Forbidden;
                var url = httpRequestData.Query["url"]!;
                var errorDto = CommonPresenter.ToErrorDTO(ex, responseStatusCode, url);

                return new ObjectResult(errorDto) { StatusCode = responseStatusCode };
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogWarning(ex, "Token inválido.");

                var responseStatusCode = (int)HttpStatusCode.Unauthorized;
                var url = httpRequestData.Query["url"]!;
                var errorDto = CommonPresenter.ToErrorDTO(ex, responseStatusCode, url);

                return new UnauthorizedObjectResult(errorDto);
            }
            catch (NotAuthorizedException ex)
            {
                _logger.LogWarning(ex, "Usuário não autorizado.");

                var responseStatusCode = (int)HttpStatusCode.Unauthorized;
                var url = httpRequestData.Query["url"]!;
                var errorDto = CommonPresenter.ToErrorDTO(ex, responseStatusCode, url);

                return new UnauthorizedObjectResult(errorDto);
            }
            catch (BusinessException ex)
            {
                _logger.LogDebug(ex, "Erro de negócio ao validar token ou URL.");

                var responseStatusCode = (int)HttpStatusCode.BadRequest;
                var url = httpRequestData.Query["url"]!;
                var errorDto = CommonPresenter.ToErrorDTO(ex, responseStatusCode, url);

                return new BadRequestObjectResult(errorDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar token.");

                var responseStatusCode = (int)HttpStatusCode.InternalServerError;
                var url = httpRequestData.Query["url"]!;
                var errorDto = CommonPresenter.ToErrorDTO(ex, responseStatusCode, url);

                return new ObjectResult(errorDto) { StatusCode = responseStatusCode };
            }
        }

        [Function("AuthCustomer")]
        [OpenApiOperation(operationId: "AuthCustomer", tags: ["Auth"])]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(CustomerAuthDTO), Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(AuthResponseDTO), Description = "Autenticação de cliente realizada com sucesso")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(ErrorDTO), Description = "Erro de negócio")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Unauthorized, contentType: "application/json", bodyType: typeof(ErrorDTO), Description = "Usuário não autorizado")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(ErrorDTO), Description = "Erro interno")]
        public async Task<IActionResult> AuthCustomer(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "customer/login")]
        HttpRequestData httpRequestData, [MsAzureFuncWorker.FromBody] CustomerAuthDTO customerAuthDTO)
        {
            try
            {
                UserType? existingUser = null;

                var isGuestAuthentication = string.IsNullOrWhiteSpace(customerAuthDTO.Email) && string.IsNullOrWhiteSpace(customerAuthDTO.Cpf);

                if (isGuestAuthentication)
                {
                    existingUser = await CognitoService.GetUserByEmailOrCpfAsync(
                        _cognito,
                        _settings,
                        _settings.GuestUserEmail
                    ) ?? throw new BusinessException("Usuário convidado não encontrado.");
                }
                else
                {
                    existingUser = await CognitoService.GetUserByEmailOrCpfAsync(
                        _cognito,
                        _settings,
                        customerAuthDTO.Email,
                        customerAuthDTO.Cpf
                    ) ?? throw new BusinessException("Usuário com este email ou CPF não existe.");
                }

                var userIsCustomer = existingUser.Attributes.Any(attr => attr.Name == "custom:role" && attr.Value == Role.CUSTOMER.ToString());
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

                var responseStatusCode = (int)HttpStatusCode.BadRequest;
                var errorDto = CommonPresenter.ToErrorDTO(ex, responseStatusCode, httpRequestData.Url.AbsolutePath);

                return new BadRequestObjectResult(errorDto);
            }
            catch (NotAuthorizedException ex)
            {
                _logger.LogWarning(ex, "Usuário não autorizado.");

                var responseStatusCode = (int)HttpStatusCode.Unauthorized;
                var errorDto = CommonPresenter.ToErrorDTO(ex, responseStatusCode, httpRequestData.Url.AbsolutePath);

                return new UnauthorizedObjectResult(errorDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao autenticar cliente.");

                var responseStatusCode = (int)HttpStatusCode.InternalServerError;
                var errorDto = CommonPresenter.ToErrorDTO(ex, responseStatusCode, httpRequestData.Url.AbsolutePath);

                return new ObjectResult(errorDto) { StatusCode = responseStatusCode };
            }
        }

    }
}