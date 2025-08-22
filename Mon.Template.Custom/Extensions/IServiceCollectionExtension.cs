#if Utiliser2fa
using Services.DeuxFa;
#endif
#if UtiliserJWT
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Services.Jwts;
#endif
using Services.Mdp;
#if UtiliserQrCode
using Services.QrCodes;
#endif
using System.Reflection;
using System.Security.Cryptography;

namespace Mon.Template.Custom.Extensions;

public static class IServiceCollectionExtension
{
    public static IServiceCollection AjouterService(this IServiceCollection _service, RSA _rsa = null)
    {
        _service
#if UtiliserJWT
            .AddSingleton<IJwtService>(new JwtService(_rsa, ""))
#endif
            .AddSingleton<IMdpService, MdpService>();

#if Utiliser2fa
        _service.AddTransient<IDeuxFaService, DeuxFaService>();
#endif
#if UtiliserQrCode
        _service.AddTransient<IQrCodeService, QrCodeService>();
#endif

        // donne acces a httpContext dans les validators
        _service.AddHttpContextAccessor();

        return _service;
    }
#if UtiliserCache
    public static IServiceCollection AjouterOutputCache(this IServiceCollection _service)
    {
        // cache actif avec le authorize
        _service.AddOutputCache(x =>
        {
            
        });

        return _service;
    }
#endif

#if UtiliserJWT
    public static IServiceCollection AjouterSecuriteJwt(this IServiceCollection _service, RSA _rsa)
    {
        _service.AddAuthorizationBuilder();
        _service.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, option =>
            {
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    // se qu'on veut valider ou non
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

                // permet de valider le chiffrement du JWT en definissant la clé utilisée
                option.Configuration = new OpenIdConnectConfiguration
                {
                    SigningKeys = { new RsaSecurityKey(_rsa) }
                };

                // pour avoir les cl� valeur normal comme dans les claims
                // par defaut ajouter des Uri pour certain truc comme le "sub"
                option.MapInboundClaims = false;
            });

        return _service;
    }
#endif

    public static IServiceCollection AjouterSwagger(this IServiceCollection _service)
    {
        _service.AddSwaggerGen(swagger =>
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

        return _service;
    }
}
