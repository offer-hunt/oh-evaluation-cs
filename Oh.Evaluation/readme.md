appsettings.json:
{

    "AuthSettings": {
        "AuthIssuer": --,
        "AuthJwksUrl": --,
        "AuthAudience": --,
        "ServerPort": --
    },
    "DatabaseSettings": {
        "Host": --,
        "Port": --,
        "User": --,
        "Password": --,
        "Name": --,
        "Schema": --
    }
}

env:
{
 
    AuthSettings__AuthIssuer=--
    AuthSettings__AuthJwksUrl=--
    AuthSettings__AuthAudience=--
    AuthSettings__ServerPort=--

    DatabaseSettings__Host=--
    DatabaseSettings__Port=--
    DatabaseSettings__User=--
    DatabaseSettings__Password=--
    DatabaseSettings__Name=--
    DatabaseSettings__Schema=--
}

ASPNETCORE_ENVIRONMENT = "prod" выключит сваггер