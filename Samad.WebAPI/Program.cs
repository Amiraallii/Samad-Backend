using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Samad.Application.Dtos;
using Samad.Application.Files;
using Samad.Application.IServices;
using Samad.Application.Services;
using Samad.Infrastructure.Context;
using Samad.Infrastructure.External.Services;
using Samad.Infrastructure.IRepositories;
using Samad.Infrastructure.Repositories;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Samad API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Enter JWT token"
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(
                "Bearer",
                document)] = []
        });
});
builder.Services.AddDbContext<SamadDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
var fileUploadOptions =
    builder.Configuration
        .GetSection("FileUpload")
        .Get<FileUploadOptions>()
    ?? new FileUploadOptions();

builder.Services.AddSingleton(fileUploadOptions);

var settings =
    builder.Configuration
        .GetSection("Settings")
        .Get<AppSettings>()
    ?? throw new InvalidOperationException(
        "Settings configuration not found.");

builder.Services.AddSingleton(settings);


builder.Services.AddSingleton<IAmazonS3>(_ =>
{
    var s3Settings = settings.S3Storage;

    var config = new AmazonS3Config
    {
        ServiceURL = s3Settings.ServiceUrl,

        ForcePathStyle = true
    };

    return new AmazonS3Client(
        s3Settings.AccessKey,
        s3Settings.SecretKey,
        config);
});



builder.Services
    .AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = settings.Jwt.Issuer,
                ValidAudience = settings.Jwt.Audience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            settings.Jwt.Key))
            };
    });

builder.Services.AddAuthorization();




builder.Services.AddScoped<IRequestService, RequestService>();

builder.Services.AddScoped<IFileUploadService, FileUploadService>();




builder.Services.AddSingleton<FilePolicyProvider>();

builder.Services.AddScoped<IFileProcessor, ImageSharpWebpProcessor>();




builder.Services.AddScoped<IFileStorage, S3FileStorage>();

builder.Services.AddScoped<
    ISecretaryRequestService,
    SecretaryRequestService>();

builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
builder.Services.AddScoped<
    ICouncilReviewService,
    CouncilReviewService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<ICouncilSignatureService, CouncilSignatureService>();
builder.Services.AddScoped<
    IRequestWorkflowService,
    RequestWorkflowService>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();