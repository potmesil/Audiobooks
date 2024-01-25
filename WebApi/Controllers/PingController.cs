using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;
using System.Threading.Tasks;

namespace Audiobooks.WebApi.Controllers
{
	[Route("")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public class PingController : ControllerBase
	{
		private readonly DbConnection _dbConnection;

		public PingController(DbConnection dbConnection)
		{
			_dbConnection = dbConnection;
		}

		[HttpGet]
		public async Task<IActionResult> GetAsync()
		{
			await _dbConnection.QueryAsync("SELECT TOP 10 * FROM [Audiobook] --ping query");
			return NoContent();
		}
	}
}