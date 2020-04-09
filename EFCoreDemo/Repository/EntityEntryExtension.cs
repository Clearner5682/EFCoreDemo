using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Repository
{
    public static class EntityEntryExtension
    {
        public static void SaveChanges(this EntityEntry entityEntry)
        {
            entityEntry.Context.SaveChanges();
        }

        public static async Task SaveChangesAsync(this EntityEntry entityEntry)
        {
            await entityEntry.Context.SaveChangesAsync();
        }
    }
}
