using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using YourCarBud.Common;

namespace YourCarBud.WebApi.Modules.DbContextModule
{
    public class YourCarBudDbContext : DbContext
    {
        private readonly IEnumerable<IDependencyInjectionModule> _modules;


        public YourCarBudDbContext(DbContextOptions options, IEnumerable<IDependencyInjectionModule> modules)
            : base(options)
        {
            _modules = modules;
        }

        // All Tables are created dynamically, inside their own modules, based on the module's "IDependencyInjectionModule" configuration
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var module in _modules)
            {
                module.OnModelCreate(modelBuilder);
            }
        }
    }
}