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

                var listUsersResponse = await CognitoService.ListUsersByEmailOrCpfAsync(
                    _cognito,
                    _settings,
                    user.Email!.Value,
                    user.Cpf!.Value
                );
                if (listUsersResponse.Users.Count > 0)
                    throw new BusinessException("Usuário com email ou CPF já existe.");

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

                return new StatusCodeResult(500);
            }
        }
    }
}