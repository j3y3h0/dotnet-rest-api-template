# RestApiTemplate

이 프로젝트는 C# ASP.NET 8.0 프레임워크를 사용하여 개발된 REST API 템플릿 웹 애플리케이션입니다.

![image](https://github.com/j3y3h0/dotnet-rest-api-template/assets/18677603/c884494d-f1bc-41b4-a3bf-7d9ca8a5eca8)

## 구현된 부분

1. ORM 라이브러리를 사용하여 데이터베이스와 통신

   - 데이터베이스 액세스를 위해 `RepoDB` 라이브러리를 활용하였습니다.
   - 구현된 연결 가능 DB는 `MS-SQL`, `MySQL`, `MariaDB`입니다.

2. 로그인 및 JWT 인증 방식 적용

   - 사용자 인증을 위한 로그인 엔드포인트를 구현하였으며, `JWT`를 사용하여 API에 대한 인증을 수행합니다.

## 사용된 라이브러리

1. **RepoDb**

   - SQL코드를 사용하지 않고 C#코드만으로 간단하게 데이터베이스 액세스를 위한 라이브러리입니다.
   - [RepoDb GitHub](https://github.com/mikependon/RepoDb)

2. **Swagger UI**

   - API 문서를 자동으로 생성하고 시각적으로 확인할 수 있는 도구입니다.
   - [Swagger GitHub](https://github.com/swagger-api/swagger-ui)

3. **Newtonsoft Json**

   - JSON 데이터를 처리하기 위한 유연한 라이브러리입니다.
   - [Newtonsoft.Json GitHub](https://github.com/JamesNK/Newtonsoft.Json)

4. **Microsoft.AspNetCore.Authentication.JwtBearer**
   - JWT(JSON Web Token) 기반의 인증을 구현하기 위한 ASP.NET 미들웨어입니다.
   - [Microsoft Docs](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.jwtbearer)

## 프로젝트 설정 및 실행

1. 프로젝트를 클론합니다.
   ```bash
   git clone https://github.com/j3y3h0/dotnet-rest-api-template.git
   ```

## API

| METHOD | URL             | 내용                                 |
| ------ | --------------- | ------------------------------------ |
| GET    | /api/board/list | 게시판 리스트                        |
| POST   | /api/auth/login | 로그인 및 유저정보 가져와서 JWT 발급 |

## 인증 설정

JWT 토큰을 사용하여 API에 대한 인증을 수행합니다. 인증을 위한 설정은 appsettings.json 파일에서 찾을 수 있습니다.

```json
{
  "Jwt": {
    "secretKey": "256_BIT_KEY",
    "expirationMins": 60
  }
}
```
