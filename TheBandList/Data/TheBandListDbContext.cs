using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace TheBandList.Data
{
    public class TheBandListDbContext : DbContext
    {
        public TheBandListDbContext(DbContextOptions<TheBandListDbContext> options)
        : base(options)
        { }
    }
}
