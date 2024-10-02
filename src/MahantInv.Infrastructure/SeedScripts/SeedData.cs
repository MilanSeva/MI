using MahantInv.Infrastructure.Data;
using MahantInv.Infrastructure.Entities;
using MahantInv.Infrastructure.Identity;
using MahantInv.Infrastructure.Utility;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MahantInv.Infrastructure.SeedScripts
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, IMediator mediator, UserManager<MIIdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            await SeedRoles(roleManager);
            await SeedUsers(userManager);
            var context = new MIDbContext(serviceProvider.GetRequiredService<DbContextOptions<MIDbContext>>(), mediator);
            await SeedOrderStatuses(context);
            await SeedUnitTypes(context);
            await SeedPaymentTypes(context);
            await SeedPartyCategories(context);
        }

        private static async Task SeedPaymentTypes(MIDbContext _context)
        {
            List<PaymentType> paymentTypes = [
            new PaymentType{ Id = "Seva", Title= "Seva" },
            new PaymentType{ Id = "Cash", Title = "Cash" },
            new PaymentType{ Id = "Check", Title = "Check" },
            new PaymentType{ Id = "Online", Title = "Online" }
            ];
            var existingPaymentTypes = await _context.PaymentTypes.ToListAsync();
            var newPaymentTypes = paymentTypes.Where(os => !existingPaymentTypes.Any(eos => eos.Id == os.Id)).ToList();
            if (newPaymentTypes.Any())
            {
                await _context.PaymentTypes.AddRangeAsync(newPaymentTypes);
                await _context.SaveChangesAsync();
            }
        }

        private static async Task SeedPartyCategories(MIDbContext _context)
        {
            List<PartyCategory> partyCategories = [
            new PartyCategory{ Id = 1, Name= "Mandir" },
            new PartyCategory{ Id = 2, Name = "Merchant" },
            new PartyCategory{ Id = 3, Name = "HariBhakt" },
            new PartyCategory{ Id = 4, Name = "Trust" },
            new PartyCategory{ Id = 5, Name = "Saint" },
            new PartyCategory{ Id = 6, Name = "Other" }
            ];
            var existingPartyCategories = await _context.PartyCategories.ToListAsync();
            var newPartyCategories = partyCategories.Where(os => !existingPartyCategories.Any(eos => eos.Id == os.Id)).ToList();
            if (newPartyCategories.Any())
            {
                await _context.PartyCategories.AddRangeAsync(newPartyCategories);
                await _context.SaveChangesAsync();
            }
        }

        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync(Meta.Roles.Admin))
            {
                await roleManager.CreateAsync(new IdentityRole(Meta.Roles.Admin));
            }
            if (!await roleManager.RoleExistsAsync(Meta.Roles.User))
            {
                await roleManager.CreateAsync(new IdentityRole(Meta.Roles.User));
            }
        }

        private static async Task SeedUsers(UserManager<MIIdentityUser> userManager)
        {
            if (userManager.Users.All(u => u.UserName != "system"))
            {
                var user = new MIIdentityUser
                {
                    UserName = "system",
                    Email = "admin@system.com",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(user, "M@hant@1309");
                await userManager.AddToRoleAsync(user, Meta.Roles.Admin);
            }
        }
        private static async Task SeedOrderStatuses(MIDbContext _context)
        {
            List<OrderStatusType> orderStatuses = [
            new OrderStatusType{ Id = "Ordered", Title = "Ordered" },
            new OrderStatusType{ Id = "Received", Title = "Received" },
            new OrderStatusType{ Id = "Cancelled", Title = "Cancelled" }
            ];
            var existingOrderStatuses = await _context.OrderStatusTypes.ToListAsync();
            var newOrderStatuses = orderStatuses.Where(os => !existingOrderStatuses.Any(eos => eos.Id == os.Id)).ToList();
            if (newOrderStatuses.Any())
            {
                await _context.OrderStatusTypes.AddRangeAsync(newOrderStatuses);
                await _context.SaveChangesAsync();
            }
        }
        private static async Task SeedUnitTypes(MIDbContext _context)
        {
            List<UnitType> unitTYpes = [
            new UnitType{ Code = "kg", Name = "Kilogram" },
            new UnitType{ Code = "g", Name = "Gram" },
            new UnitType{ Code = "L", Name = "Liter" },
            new UnitType{ Code = "mL", Name = "Milliliter" },
            new UnitType{ Code = "pcs", Name = "Pieces" },
            new UnitType{ Code = "doz", Name = "Dozon" },
            new UnitType{ Code = "pkg", Name = "Package" },
            new UnitType{ Code = "box", Name = "Box" },
            new UnitType{ Code = "btl", Name = "Bottle" },
            new UnitType{ Code = "ctn", Name = "Carton" },
            new UnitType{ Code = "mg", Name = "Milligram" }
            ];
            var existingUnitTypes = await _context.UnitTypes.ToListAsync();
            var newUnitTypes = unitTYpes.Where(os => !existingUnitTypes.Any(eos => eos.Code == os.Code)).ToList();
            if (newUnitTypes.Any())
            {
                await _context.UnitTypes.AddRangeAsync(newUnitTypes);
                await _context.SaveChangesAsync();
            }
        }
    }
}
