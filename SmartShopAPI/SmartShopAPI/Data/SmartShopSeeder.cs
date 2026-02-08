using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using SmartShopAPI.Entities;
using SmartShopAPI.Models;

using static System.Net.Mime.MediaTypeNames;

namespace SmartShopAPI.Data
{
    public class SmartShopSeeder
    {
        private readonly SmartShopDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public SmartShopSeeder(SmartShopDbContext dbContext, IPasswordHasher<User> passwordHasher)
        {
            _context = dbContext;
            _passwordHasher = passwordHasher;
        }

        public void Seed()
        {
            if (_context.Database.IsRelational() && _context.Database.GetPendingMigrations().Any())
            {
                _context.Database.Migrate();
            }
            SeedCategories();
            SeedProducts();
            SeedRoles();
            SeedUsers();
        }

        private void SeedCategories()
        {
            if (!_context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "PS4" },
                    new Category { Name = "PS5" }
                };
                _context.Categories.AddRange(categories);
                _context.SaveChanges();
            }
        }

        private void SeedProducts()
        {
            if (!_context.Products.Any())
            {
                var products = new List<Product>
                {
                    new Product
                    {
                        Name = "The Last of Us Part II Remastered",
                        Description = "The highly anticipated sequel to the critically acclaimed action-adventure game, following Ellie’s journey through a post-apocalyptic world as she faces challenges and uncovers secrets in a richly detailed and immersive environment.",
                        Price = 199.99M,
                        StockQuantity = 12,
                        CategoryId = 2,
                        ImagePath = "images/products/product1.jpg"
                    },
                    new Product
                    {
                        Name = "God of War",
                        Description = "An epic action-adventure game where Kratos, the former Greek god, and his son embark on a perilous journey through the Nordic realms, battling gods and mythical creatures in a quest for survival and redemption.",
                        Price = 149.99M,
                        StockQuantity = 15,
                        CategoryId = 1,
                        ImagePath = "images/products/product2.jpg"
                    },
                    new Product
                    {
                        Name = "Ghost of Tsushima",
                        Description = "An open-world action game set in feudal Japan, where players step into the shoes of Jin Sakai, a samurai warrior fighting to protect his homeland from the Mongol invasion using both traditional samurai skills and stealth tactics.",
                        Price = 179.99M,
                        StockQuantity = 10,
                        CategoryId = 2,
                        ImagePath = "images/products/product3.jpg"
                    },
                    new Product
                    {
                        Name = "Spider-Man: Miles Morales",
                        Description = "Join Miles Morales as he takes up the mantle of Spider-Man in an action-packed adventure set in New York City, facing new villains and exploring the challenges of becoming a hero in a vibrant and dynamic open-world setting.",
                        Price = 249.99M,
                        StockQuantity = 8,
                        CategoryId = 2,
                        ImagePath = "images/products/product4.jpg"
                    },
                    new Product
                    {
                        Name = "Demon's Souls",
                        Description = "A remake of the classic RPG action game renowned for its challenging difficulty and deep, atmospheric world. Players venture through a dark fantasy realm, battling nightmarish creatures and facing intense boss fights.",
                        Price = 299.99M,
                        StockQuantity = 6,
                        CategoryId = 2,
                        ImagePath = "images/products/product5.jpg"
                    },
                    new Product
                    {
                        Name = "Ratchet & Clank: Rift Apart",
                        Description = "An action-platformer with fast-paced gameplay and stunning visuals. Follow Ratchet and Clank as they traverse through different dimensions to stop an evil emperor from conquering the multiverse.",
                        Price = 269.99M,
                        StockQuantity = 10,
                        CategoryId = 2,
                        ImagePath = "images/products/product6.jpg"
                    },
                    new Product
                    {
                        Name = "Gran Turismo 7",
                        Description = "A captivating racing simulation game offering realistic graphics and a wide selection of vehicles. Experience a detailed and immersive driving experience with various tracks and challenging races.",
                        Price = 280.90M,
                        StockQuantity = 10,
                        CategoryId = 1,
                        ImagePath = "images/products/product7.jpg"
                    },
                    new Product
                    {
                        Name = "Grand Theft Auto V",
                        Description = "An expansive open-world action game featuring a gripping narrative, dynamic missions, and a richly detailed city to explore. Engage in a variety of criminal activities and experience the thrilling world of Los Santos.",
                        Price = 76.99M,
                        StockQuantity = 10,
                        CategoryId = 1,
                        ImagePath = "images/products/product8.jpg"
                    },
                    new Product
                    {
                        Name = "EA SPORTS FC 24",
                        Description = "The latest installment in the football simulation series, offering realistic gameplay, immersive match experiences, and advanced online features that bring the excitement of football to life.",
                        Price = 76.99M,
                        StockQuantity = 25,
                        CategoryId = 1,
                        ImagePath = "images/products/product9.jpg"
                    },
                    new Product
                    {
                        Name = "EA SPORTS FC 25",
                        Description = "The newest chapter in the popular football franchise, introducing new game modes, enhanced graphics, and more realistic animations, delivering an even more engaging and authentic football experience.",
                        Price = 299.99M,
                        StockQuantity = 18,
                        CategoryId = 2,
                        ImagePath = "images/products/product10.jpg"
                    },
                    new Product
                    {
                        Name = "Marvel's Spider-Man 2",
                        Description = "The thrilling continuation of Spider-Man's adventures, where players once again swing through the streets of New York, confront new adversaries, and uncover more secrets in the Marvel Universe with enhanced gameplay and storytelling.",
                        Price = 76.99M,
                        StockQuantity = 7,
                        CategoryId = 1,
                        ImagePath = "images/products/product11.jpg"
                    }
                };
                _context.Products.AddRange(products);
                _context.SaveChanges();
            }
        }


        private void SeedRoles()
        {
            if (!_context.Roles.Any())
            {
                var roles = new List<Role>
                {
                    new Role { Name = "Admin" },
                    new Role { Name = "User" }
                };
                _context.Roles.AddRange(roles);
                _context.SaveChanges();
            }
        }

        private void SeedUsers()
        {
            if (!_context.Users.Any())
            {
                var admin = new User
                {
                    Email = "admin@admin.com",
                    FirstName = "Admin",
                    LastName = "Admin",
                    DateOfBirth = new DateTime(1980, 1, 1),
                    RoleId = 1
                };
                admin.PasswordHash = _passwordHasher.HashPassword(admin, "admin123");
                admin.Addresses.Add(new Address
                {
                    City = "Warsaw",
                    Street = "Admin St",
                    PostalCode = "00-111",
                    IsDefault = true
                });

                var user = new User
                {
                    Email = "user@user.com",
                    FirstName = "User",
                    LastName = "User",
                    DateOfBirth = new DateTime(1990, 5, 15),
                    RoleId = 2
                };
                user.PasswordHash = _passwordHasher.HashPassword(user, "user1234");
                user.Addresses.Add(new Address
                {
                    City = "Krakow",
                    Street = "User St",
                    PostalCode = "30-222",
                    IsDefault = true
                });

                _context.Users.AddRange(admin, user);
                _context.SaveChanges();
            }
        }
    }
}
