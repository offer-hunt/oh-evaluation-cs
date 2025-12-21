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
    },
    "ApiSettings": {
        "AiClientBase": "http://host:port",
        "CourseClientBase": "http://host:port/api/course",
        "LearningClientBase": "http://host:port",
        "PageClientBase": "http://host:port/api/pages"
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

    ApiSettings__AiClientBase="http://host:port"
    ApiSettings__CourseClientBase="http://host:port/api/course"
    ApiSettings__LearningClientBase="http://host:port"
    ApiSettings__PageClientBase="http://host:port/api/pages"
}

ASPNETCORE_ENVIRONMENT = "prod" выключит сваггер