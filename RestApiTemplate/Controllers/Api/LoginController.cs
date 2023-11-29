using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Newtonsoft.Json.Linq;
using RepoDb;
using RestApiTemplate.Configurations;
using RestApiTemplate.Models.Auth;
using RestApiTemplate.Service;

namespace RestApiTemplate.Controllers.Api
{
    [ApiController]
    [Produces("application/json")]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContext;

        public LoginController(IHttpContextAccessor context)
        {
            _httpContext = context;
        }

        #region POST

        #region Login
        /// <summary>
        /// JWT 로그인
        /// </summary>
        /// <remarks>
        /// sample JWT 발급
        /// 
        /// Sample request:
        ///
        ///     POST /api/auth/login
        ///     {
        ///        "username": "test",
        ///        "password": "1234"
        ///     }
        ///
        /// </remarks>
        [HttpPost]
        [Route("/api/auth/login")]
        public IActionResult PostLogin([FromBody] LoginInfoDto loginInfo)
        {
            JObject response = new JObject();

            try
            {
                // 유효성 검사
                if (string.IsNullOrEmpty(loginInfo.username) || string.IsNullOrEmpty(loginInfo.password))
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                //MariaDB ORM Init
                using MySqlConnection connection = new MySqlConnection(AppSettings.MYSQL_DB_CONNECTION);
                connection.Open();

                // SELECT 
                TestUserEntity user = connection.Query<TestUserEntity>(e => e.username == loginInfo.username && e.password == loginInfo.password).FirstOrDefault();

                if (user == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                // 토큰 발급
                string token = AuthService.GenerateJSONWebToken(user.username);

                response.Add("username", user.username);
                response.Add("token", $"Bearer {token}");

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response.Add("error", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
        #endregion

        #endregion
    }
}
