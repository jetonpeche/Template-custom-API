#if UtiliserJWT
using System.Security.Cryptography;
#endif
using Mon.Template.Custom.Extensions;
#if UtiliserMail
using Services.Mail; 
#endif
#if UtiliserFluentValidator
using FluentValidation;
#endif

var builder = WebApplication.CreateBuilder(args);

#if UtiliserJWT
string cheminCleRsa = builder.Configuration.GetValue<string>("cheminCleRsa")!;

RSA rsa = RSA.Create();

if(!Directory.Exists("Rsa"))
    Directory.CreateDirectory("Rsa");

// creer la cle une seule fois
if (!File.Exists(cheminCleRsa))
{
    // cree un fichier bin pour signer le JWT
    var clePriver = rsa.ExportRSAPrivateKey();
    File.WriteAllBytes(cheminCleRsa, clePriver);
}

// recupere la cl�
rsa.ImportRSAPrivateKey(File.ReadAllBytes(cheminCleRsa), out _);
builder.Services.AjouterSecuriteJwt(rsa);
#endif

builder.Services.AddEndpointsApiExplorer();
#if UtiliserJWT
   builder.Services.AjouterSwagger();
#endif
#if UtiliserHttpClient
builder.Services.AddHttpClient("", x => 
{
    x.BaseAddress = new Uri("{BaseAdresseUri}");
});
#endif

#if UtiliserCache
builder.Services.AjouterOutputCache();
#endif

#if UtiliserFluentValidator
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
#endif
builder.Services.AddCors(x => x.AddDefaultPolicy(y => y.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
#if UtiliserMail
builder.Services.AddSingleton<IMailService>(new MailService(new MailOptions
{
    Expediteur = "",
    Mdp = "",
    NomSmtp = "smtp.gmail.com",
    NumeroPortSmtp = 587
}));
#endif
#if UtiliserJWT
builder.Services.AjouterService(rsa);
#else
builder.Services.AjouterService();
#endif

var app = builder.Build();

app.UseCors();
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

app.AjouterRouteAPI();

app.Run();
