using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Dal.Identity
{
    public class RoleSeeder
    {
        public static async Task SeedAsync(RoleManager<IdentityRole> roleMgr, UserManager<AppUser> userMgr)
        {
            string[] roles = new[] { "USER","PrimaryUser", "InventoryEmployee", "InventoryOrganizer", "ReceivingClerk", "Admin","Supplier","Company" };
            foreach (var r in roles)
                if (!await roleMgr.RoleExistsAsync(r))
                    await roleMgr.CreateAsync(new IdentityRole(r));

            var admin = await userMgr.FindByNameAsync("admin");
            if (admin == null)
            {
                admin = new AppUser { UserName = "admin", Email = "admin@example.com", FullName = "System Admin" };
                await userMgr.CreateAsync(admin, "Admin@123");
                await userMgr.AddToRoleAsync(admin, "PrimaryUser");
            }
        }
    }
}
