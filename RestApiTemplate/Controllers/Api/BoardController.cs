using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RepoDb;
using RepoDb.Enumerations;
using RestApiTemplate.Configurations;
using RestApiTemplate.Models.Board;
using System.Data.SqlClient;

namespace RestApiTemplate.Controllers.Api
{
    [ApiController]
    [Produces("application/json")]
    [Route("[controller]")]
    public class BoardController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContext;

        public BoardController(IHttpContextAccessor context)
        {
            _httpContext = context;
        }

        #region GET

        #region BOARD > 게시글 가져오기
        /// <summary>
        /// 게시글 가져오기 API
        /// </summary>
        [HttpGet]
        [Route("/api/board/list")]
        [Authorize]
        public IActionResult GetKboSchedule(int page = 1, int size = 10)
        {
            JObject response = new JObject();

            //유효성 검사
            if (page == 0)
            {
                page = 1;
            }
            if (size == 0)
            {
                size = 10;
            }

            try
            {
                IEnumerable<OrderField> orderBy = OrderField.Parse(new { boardId = Order.Descending });

                //MS-SQL Server ORM Init
                using SqlConnection connection = new SqlConnection(AppSettings.MSSQL_DB_CONNECTION);

                // SELECT 전체 가져오기
                // isActive = 1 : 정상 상태
                List<BoardEntity> boardList = connection.Query<BoardEntity>(e => e.isActive == 1, orderBy: orderBy).ToList();

                // https://repodb.net/operation/batchquery
                // Pagination, Skip
                List<BoardEntity> boardList2 = connection.BatchQuery<BoardEntity>(page: page, rowsPerBatch: size, orderBy: orderBy, where: e => e.isActive == 1).ToList();

                response.Add("boardList", JArray.FromObject(boardList));

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
