# Template custom API

Template custom pour minimal API .NET 8  
Génération via le CLI dotnet `Template-custom-API`  
`dotnet new Template-custom-API --help`

## Installation
1. Ouvrir une invite de commande (cmd)
2. Se rendre dans le projet jusqu'au `.sln` (cd)
3. Exécuter la commande `dotnet new install .`

## Désinstallation
1. Ouvrir une invite de commande (cmd)
2. Se rendre dans le projet jusqu'au `.sln` (cd)
3. Exécuter la commande `dotnet new uninstall .`

## Options Services:
- Generateur de QR code
- 2fa
- Generateur de Jwt
- Envoie de Mail (pas de pieces jointes)

## Options dispo (hors services)
- OutputCache (avec policy custom pour Authorize)
- HttpClient
- Bdd MySql (Pomelo) / SqlServer
- CORS
- Fluent Validator
- Jwt (config et config swagger)

## Options appSetting.json
- Connexion string MySql / SqlServer
- Url de base pour HttpClient
