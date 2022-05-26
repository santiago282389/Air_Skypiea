using Air_Skypiea.Data.Entities;
using Air_Skypiea.Enums;
using Air_Skypiea.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Air_Skypiea.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IBlobHelper _blobHelper;

        public SeedDb(DataContext context, IUserHelper userHelper, IBlobHelper blobHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _blobHelper = blobHelper;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckCategoriesAsync();
            await CheckCountriesAsync();
            await CheckRolesAsync();
            await CheckUserAsync("0520", "Alejandro", "Gómez", "alego@yopmail.com", "305 383 8383", "Calle Jardín", "cantinflas.png", UserType.Admin);
            await CheckUserAsync("2020", "Catalina", "Rojas", "catarojas@yopmail.com", "301 636 6366", "Calle Luna Calle Sol", "Mr_bean.png", UserType.User);
            await CheckProductsAsync();
        }

        private async Task CheckProductsAsync()
        {
            if (!_context.Products.Any())
            {
                await AddProductAsync("Destino cosmopolita por excelencia", "Bogotá", 850000M, 12F, new List<string>() { "Económica", "Ejecutiva/Business", "Premium" }, new List<string>() { "Bogotá.jpg" });
                await AddProductAsync("La sultana del valle", "Cali", 650000M, 12F, new List<string>() { "Económica", "Ejecutiva/Business", "Premium" }, new List<string>() { "Cali.jpg" });
                await AddProductAsync("La puerta de oro", "Cartagena", 1300000M, 12F, new List<string>() { "Económica", "Ejecutiva/Business", "Premium" }, new List<string>() { "Cartagena.jpg" });
                await AddProductAsync("Ciudad innovadora", "Medellin", 870000M, 12F, new List<string>() { "Económica", "Ejecutiva/Business", "Premium" }, new List<string>() { "Medellin.jpg" });
                await AddProductAsync("el mar es de siete colores", "San Andrés", 1200000M, 6F, new List<string>() { "Económica", "Ejecutiva/Business", "Premium" }, new List<string>() { "San_Andrés.jpg" });
                await AddProductAsync("Tesoro del Caribe", "Santa Marta", 990000M, 24F, new List<string>() { "Económica", "Ejecutiva/Business", "Premium" }, new List<string>() { "Santa_Marta.jpg" });
                await AddProductAsync("Ciudad de cantos y joropo", "Sincelejo", 820000M, 12F, new List<string>() { "Económica", "Ejecutiva/Business", "Premium" }, new List<string>() { "Sincelejo.jpg" });
                await AddProductAsync("La puerta del llano", "Villavicencio", 700000M, 6F, new List<string>() { "Económica", "Ejecutiva/Business", "Premium" }, new List<string>() { "Villavicencio.jpg" });
                await _context.SaveChangesAsync();
            }

        }

        private async Task AddProductAsync(string name1, string name, decimal price, float stock, List<string> categories, List<string> images)
        {
            Product prodcut = new()
            {
                Description = name1,
                Name = name,
                Price = price,
                Stock = stock,
                ProductCategories = new List<ProductCategory>(),
                ProductImages = new List<ProductImage>()
            };

            foreach (string? category in categories)
            {
                prodcut.ProductCategories.Add(new ProductCategory { Category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == category) });
            }


            foreach (string? image in images)
            {
                Guid imageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\products\\{image}", "products");
                prodcut.ProductImages.Add(new ProductImage { ImageId = imageId });
            }

            _context.Products.Add(prodcut);
        }


        private async Task<User> CheckUserAsync(
            string document,
            string firstName,
            string lastName,
            string email,
            string phone,
            string address,
            string image,
            UserType userType)
        {
            User user = await _userHelper.GetUserAsync(email);
            if (user == null)
            {
                Guid imageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\users\\{image}", "users");
                user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phone,
                    Address = address,
                    Document = document,
                    City = _context.Cities.FirstOrDefault(),
                    UserType = userType,
                    ImageId = imageId
                };

                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRoleAsync(user, userType.ToString());

                string token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                await _userHelper.ConfirmEmailAsync(user, token);
            }

            return user;
        }

        private async Task CheckRolesAsync()
        {
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.User.ToString());
        }

        private async Task CheckCountriesAsync()
        {
            if (!_context.Countries.Any())
            {
                _context.Countries.Add(new Country
                {
                    Name = "Colombia",
                    States = new List<State>()
                    {
                        new State()
                        {
                            Name = "Antioquia",
                            Cities = new List<City>() {
                                new City() { Name = "Medellín" },
                                new City() { Name = "Itagüí" },
                                new City() { Name = "Envigado" },
                                new City() { Name = "Bello" },
                                new City() { Name = "Rionegro" },
                            }
                        },
                        new State()
                        {
                            Name = "Bogotá",
                            Cities = new List<City>() {
                                new City() { Name = "Usaquen" },
                                new City() { Name = "Champinero" },
                                new City() { Name = "Santa fe" },
                                new City() { Name = "Useme" },
                                new City() { Name = "Bosa" },
                            }
                        },
                    }
                });
                _context.Countries.Add(new Country
                {
                    Name = "Estados Unidos",
                    States = new List<State>()
                    {
                        new State()
                        {
                            Name = "Florida",
                            Cities = new List<City>() {
                                new City() { Name = "Orlando" },
                                new City() { Name = "Miami" },
                                new City() { Name = "Tampa" },
                                new City() { Name = "Fort Lauderdale" },
                                new City() { Name = "Key West" },
                            }
                        },
                        new State()
                        {
                            Name = "Texas",
                            Cities = new List<City>() {
                                new City() { Name = "Houston" },
                                new City() { Name = "San Antonio" },
                                new City() { Name = "Dallas" },
                                new City() { Name = "Austin" },
                                new City() { Name = "El Paso" },
                            }
                        },
                    }
                });
            }

            await _context.SaveChangesAsync();
        }

        private async Task CheckCategoriesAsync()
        {
            if (!_context.Categories.Any())
            {
                _context.Categories.Add(new Category { Name = "Económica" });
                _context.Categories.Add(new Category { Name = "Ejecutiva/Business" });
                _context.Categories.Add(new Category { Name = "Premium" });

                await _context.SaveChangesAsync();
            }
        }
    }
}
