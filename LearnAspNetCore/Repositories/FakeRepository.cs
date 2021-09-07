using System;
using System.Threading;
using System.Threading.Tasks;
using LearnAspNetCore.Data;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LearnAspNetCore.Repositories
{
	public class FakeRepository : IFakeRepository
	{
		private readonly RepoDbContext _dbContext;
		private readonly ILogger<FakeRepository> _logger;

		public FakeRepository(RepoDbContext dbContext, ILogger<FakeRepository> logger)
		{
			_dbContext = dbContext;
			_logger = logger;
		}

		public async Task<int> GetSomeNumberAsync(CancellationToken token)
		{
			_logger.LogInformation("Calling db to get information. Might take a while");
			var cmnd = @"WITH RECURSIVE r(i) AS (VALUES(0) UNION ALL SELECT i FROM r LIMIT 30000000) SELECT i FROM r where i == 1;";
			var command = _dbContext.Database.GetDbConnection().CreateCommand();
			command.CommandText = cmnd;
			command.ConfigureAwait(false);

			await command.Connection.OpenAsync(token);
			await command.ExecuteScalarAsync(token);
			_logger.LogInformation("First result returned");

			if (token.IsCancellationRequested)
			{
				await command.Connection.CloseAsync();
				return 0;
			}

			await command.ExecuteScalarAsync(token);
			_logger.LogInformation("Second result returned");
			await command.Connection.CloseAsync();
			return 0;
		}
	}

	public interface IFakeRepository
	{
		Task<int> GetSomeNumberAsync(CancellationToken token);
	}
}
