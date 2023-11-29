using RepoDb.Attributes.Parameter;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestApiTemplate.Models.Board
{
    [Table("board")]
    public class BoardEntity
    {

        [Name("board_id")]
        public int boardId { get; set; }
        [Name("board_tt")]
        public string boardTitle { get; set; }
        [Name("board_ct")]
        public string boardContent { get; set; }
        [Name("condition_cd")]
        public int isActive { get; set; }

        [Name("input_dt")]
        public DateTime inputDate { get; set; }
    }
}
