using CollabApp.mvc;
using CollabApp.mvc.Context;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabApp.IntegrationTests.Controllers
{
    public class CollabAppWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly ApplicationDbContext _context;
        public CollabAppWebApplicationFactory()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "IntegrationTestDb")
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _context = new ApplicationDbContext(options);
        }

    }
}
