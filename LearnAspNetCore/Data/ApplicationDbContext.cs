using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LearnAspNetCore.Data
{
	public class ApplicationDbContext : IdentityDbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}
	}

	public class RepoDbContext : DbContext
	{
		public RepoDbContext(DbContextOptions<RepoDbContext> options)
			: base(options)
		{
		}
	}
}
