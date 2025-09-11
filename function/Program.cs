using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Foodcore.Auth.Utils;
using Foodcore.Auth.Model;
using Amazon.CognitoIdentityProvider;
using Amazon;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

# region AWS Credentials
var rawCreds = Environment.GetEnvironmentVariable("AWS_CREDENTIALS");
if (string.IsNullOrEmpty(rawCreds))
    throw new InvalidOperationException("AWS_CREDENTIALS environment variable is not set.");

var credsDict = AwsCredentialsUtils.GetAwsCredentialsDict(rawCreds);

var accessKey = credsDict.GetValueOrDefault("aws_access_key_id");
var secretKey = credsDict.GetValueOrDefault("aws_secret_access_key");
var sessionToken = credsDict.GetValueOrDefault("aws_session_token");

var region = Environment.GetEnvironmentVariable("AWS_REGION");
var userPoolId = Environment.GetEnvironmentVariable("COGNITO_USER_POOL_ID");

if (string.IsNullOrEmpty(region) || string.IsNullOrEmpty(userPoolId))
    throw new InvalidOperationException("Region or UserPoolId environment variable is not set.");


#endregion

builder.Services.AddSingleton<IAmazonCognitoIdentityProvider>(sp =>
{
    return new AmazonCognitoIdentityProviderClient(
        accessKey,
        secretKey,
        sessionToken,
        RegionEndpoint.GetBySystemName(region)
    );
});
builder.Services.AddSingleton(new CognitoSettings
{
    UserPoolId = userPoolId
});

builder.Build().Run();
