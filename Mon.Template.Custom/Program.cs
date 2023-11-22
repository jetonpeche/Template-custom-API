#if UtiliserJWT
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Services.Jwts;
using Services.Mdp;
using System.Security.Cryptography;
#endif
#if UtiliserFluentValidator
using FluentValidation;
#endif
using System.Reflection;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

#if UtiliserJWT
string cheminCleRsa = builder.Configuration.GetValue<string>("cheminCleRsa")!;

RSA rsa = RSA.Create();

// creer la cle une seule fois
if (!File.Exists(cheminCleRsa))
{
    // cree un fichier bin pour signer le JWT
    var clePriver = rsa.ExportRSAPrivateKey();
    File.WriteAllBytes(cheminCleRsa, clePriver);
}

// recupere la cl�
rsa.ImportRSAPrivateKey(File.ReadAllBytes(cheminCleRsa), out _);

// permet de savoir si on a le bon role pour pouvoir y acceder*
// .AddPolicy("nom", policy => policy.RequireRole("admin"));
// nom => a donner dans .RequireAuthorization("nom")
builder.Services.AddAuthorizationBuilder();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, option =>
                {
                    option.TokenValidationParameters = new TokenValidationParameters
                    {
                        // se qu'on veut valider ou non
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };

                    // permet de valider le chiffrement du JWT en definissant la cle utilis�
                    option.Configuration = new OpenIdConnectConfiguration
                    {
                        SigningKeys = { new RsaSecurityKey(rsa) }
                    };

                    // pour avoir les cl� valeur normal comme dans les claims
                    // par defaut ajouter des Uri pour certain truc comme le "sub"
                    option.MapInboundClaims = false;
                });
#endif

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swagger =>
{
    // genere un XML et permet de voir la doc dans swagger pour chaque Routes API
    string xmlNomFichier = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    swagger.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlNomFichier));

#if UtiliserJWT
    // ajout d'une option pour mettre le token en mode Bearer
    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        // ou le trouver
        In = ParameterLocation.Header,

        // description
        Description = "Token",

        // nom dans le header
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",

        // JWT de type Bearer
        Scheme = "Bearer"
    });

    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },

            new string[]{}
        }
    });
#endif
});

#if UtiliserHttpClient
builder.Services.AddHttpClient("", x => 
{
    x.BaseAddress = new Uri("{BaseAdresseUri}");
});
#endif
builder.Services.ConfigureHttpJsonOptions(x =>
{
    x.SerializerOptions.PropertyNameCaseInsensitive = true;
    x.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

#if UtiliserCache
// config cache de sortie
builder.Services.AddOutputCache(x =>
{
    
});
#endif

#if UtiliserFluentValidator
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
#endif

#if UtiliserCors
builder.Services.AddCors(x => x.AddDefaultPolicy(y => y.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
#endif

builder.Services.AddSingleton<IMdpService, MdpService>();
#if UtiliserJWT
builder.Services.AddSingleton<IJwtService>(new JwtService(rsa, ""));
#endif
#if UtiliserMail
builder.Services.AddSingleton<IMailService>(new MailService(new back.Options.MailOptions
{
    Expediteur = "",
    Mdp = "",
    NomSmtp = "smtp.gmail.com",
    NumeroPortSmtp = 587
}));
#endif
#if Utiliser2fa
builder.Services.AddTransient<IDeuxFaService, DeuxFaService>();
#endif
#if UtiliserQrCode
builder.Services.AddTransient<IQrCodeService, QrCodeService>();
#endif

var app = builder.Build();

#if UtiliserCors
app.UseCors();
#endif
#if UtiliserJWT
// l'ordre est important
app.UseAuthentication();
app.UseAuthorization();
#endif
#if UtiliserCache
app.UseOutputCache();
#endif

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    // cacher la liste des models import / export dans swagger
    app.UseSwaggerUI(x => x.DefaultModelsExpandDepth(-1));
}

app.Run();
