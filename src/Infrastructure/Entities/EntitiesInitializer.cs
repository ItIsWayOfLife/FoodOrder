using Core.Entities;
using System;
using System.Linq;

namespace Infrastructure.Entities
{
    public static class EntitiesInitializer
    {
        public static void Initialize(ApplicationContext context)
        {
            if (!context.Providers.Any())
            {
                context.Providers.AddRange(
                    new Provider
                    {
                        Email = "holavkysno@gmail.com",
                        Info = "Европейская кухня",
                        IsActive = true,
                        IsFavorite = true,
                        Name = "HOLA",
                        Path = "hola.jpeg",
                        TimeWorkTo = new DateTime(2020, 03, 19, 23, 00, 00),
                        TimeWorkWith = new DateTime(2020, 03, 19, 08, 00, 00),
                        WorkingDays = "Понедельник - Суббота"
                    },
                    new Provider
                    {
                        Email = "shawarma777@gmail.com",
                        Info = "Шаурма",
                        IsActive = true,
                        IsFavorite = true,
                        Name = "777",
                        Path = "777.jpeg",
                        TimeWorkTo = new DateTime(2020, 03, 19, 22, 30, 00),
                        TimeWorkWith = new DateTime(2020, 03, 19, 08, 00, 00),
                        WorkingDays = "Понедельник - Воскресенье"
                    },
                    new Provider
                    {
                        Email = "sushi@gmail.com",
                        Info = "Японская, Азиатская, Суши",
                        IsActive = true,
                        IsFavorite = false,
                        Name = "Суши ВЕСЛА",
                        Path = "TAKE_AWAY.jpeg",
                        TimeWorkTo = new DateTime(2020, 03, 19, 22, 00, 00),
                        TimeWorkWith = new DateTime(2020, 03, 19, 10, 00, 00),
                        WorkingDays = "Понедельник - Суббота"
                    }
                );
                context.SaveChanges();

                context.Catalogs.AddRange(
                  new Catalog
                  {
                      ProviderId = context.Providers.Where(p => p.Email == "holavkysno@gmail.com").FirstOrDefault().Id,
                      Name = "Сэндвичи",
                      Info = "Только лучшие"
                  },
                  new Catalog
                  {
                      ProviderId = context.Providers.Where(p => p.Email == "holavkysno@gmail.com").FirstOrDefault().Id,
                      Name = "Блинчики",
                      Info = "Разные виды и начинки"
                  },
                   new Catalog
                   {
                       ProviderId = context.Providers.Where(p => p.Email == "holavkysno@gmail.com").FirstOrDefault().Id,
                       Name = "Салаты",
                       Info = "Различные рецепты"
                   },
                     new Catalog
                     {
                         ProviderId = context.Providers.Where(p => p.Email == "shawarma777@gmail.com").FirstOrDefault().Id,
                         Name = "Шаурма",
                         Info = "Различные рецепты, ингредиенты и размеры"
                     },
                       new Catalog
                       {
                           ProviderId = context.Providers.Where(p => p.Email == "shawarma777@gmail.com").FirstOrDefault().Id,
                           Name = "Напитки",
                           Info = "Соки и газировка"
                       },
                         new Catalog
                         {
                             ProviderId = context.Providers.Where(p => p.Email == "sushi@gmail.com").FirstOrDefault().Id,
                             Name = "Супы",
                             Info = "Лучшие рецепты"
                         }
            );
                context.SaveChanges();

                context.Dishes.AddRange(
             new Dish
             {
                 CatalogId = context.Catalogs.Where(p => p.Name == "Сэндвичи").FirstOrDefault().Id,
                 Info = "Сэндвич с ветчиной, сыром и овощами",
                 Name = "Cheese friend",
                 Weight = 200,
                 Price = 3.10M,
                 Path = "send1.jpeg",
             },
             new Dish
             {
                 CatalogId = context.Catalogs.Where(p => p.Name == "Сэндвичи").FirstOrDefault().Id,
                 Info = "Сэндвич с ветчиной, сыром и овощами",
                 Name = "Vegetarian",
                 Weight = 200,
                 Price = 2.90M,
                 Path = "send2.jpeg",
             },
               new Dish
               {
                   CatalogId = context.Catalogs.Where(p => p.Name == "Блинчики").FirstOrDefault().Id,
                   Info = "Ветчина, сыр, маринованные огурцы, зелень",
                   Name = "Блинчики Фирменные",
                   Weight = 150,
                   Price = 4.90M,
                   Path = "firm.jpeg",
               },
                 new Dish
                 {
                     CatalogId = context.Catalogs.Where(p => p.Name == "Блинчики").FirstOrDefault().Id,
                     Info = "Ветчина, сыр, маринованные огурцы, зелень, майонез",
                     Name = "Блинчики Друзья",
                     Weight = 150,
                     Price = 4.50M,
                     Path = "fr.jpeg",
                 },
                   new Dish
                   {
                       CatalogId = context.Catalogs.Where(p => p.Name == "Блинчики").FirstOrDefault().Id,
                       Info = "Ветчина, маринованные огурцы, зелень",
                       Name = "Блинчики Вкусно",
                       Weight = 150,
                       Price = 4.20M,
                       Path = "fis.jpeg",
                   },
                    new Dish
                    {
                        CatalogId = context.Catalogs.Where(p => p.Name == "Блинчики").FirstOrDefault().Id,
                        Info = "Ветчина, сыр, маринованные огурцы, зелень, красная рыба",
                        Name = "Блинчики Рыбные",
                        Weight = 150,
                        Price = 5.90M,
                        Path = "3.jpeg",
                    },
                     new Dish
                     {
                         CatalogId = context.Catalogs.Where(p => p.Name == "Салаты").FirstOrDefault().Id,
                         Info = "Картофель, яйца, морковка, колбаса вареная, огурцы, майонец",
                         Name = "Салат Оливье",
                         Weight = 220,
                         Price = 5.90M,
                         Path = "ol.jpeg",
                     },
                      new Dish
                      {
                          CatalogId = context.Catalogs.Where(p => p.Name == "Салаты").FirstOrDefault().Id,
                          Info = "Филе, сыр, пимодор, чеснок, майонез, хлеб белый, салат",
                          Name = "Салат Цезарь",
                          Weight = 220,
                          Price = 7.60M,
                          Path = "ce.jpeg",
                      },
                        new Dish
                        {
                            CatalogId = context.Catalogs.Where(p => p.Name == "Шаурма").FirstOrDefault().Id,
                            Info = "Лаваш, мясо птицы, огурец, сыр, зелень, помидор",
                            Name = "Шаурма маленькая",
                            Weight = 400,
                            Price = 5.50M,
                            Path = "sm.jpeg",
                        },
                         new Dish
                         {
                             CatalogId = context.Catalogs.Where(p => p.Name == "Шаурма").FirstOrDefault().Id,
                             Info = "Лаваш, мясо птицы, огурец, сыр, зелень, помидор",
                             Name = "Шаурма средняя",
                             Weight = 550,
                             Price = 7.50M,
                             Path = "sm.jpeg",
                         },
                          new Dish
                          {
                              CatalogId = context.Catalogs.Where(p => p.Name == "Шаурма").FirstOrDefault().Id,
                              Info = "Лаваш, мясо птицы, огурец, сыр, зелень, помидор",
                              Name = "Шаурма большая",
                              Weight = 700,
                              Price = 10.00M,
                              Path = "sm.jpeg",
                          },
                           new Dish
                           {
                               CatalogId = context.Catalogs.Where(p => p.Name == "Шаурма").FirstOrDefault().Id,
                               Info = "Эксклюзивная булочка, чеснок, капуста, мясо птицы, огурец, сыр, зелень",
                               Name = "Шаурма в пите",
                               Weight = 5500,
                               Price = 5.50M,
                               Path = "sbv.jpeg",
                           },
                           new Dish
                           {
                               CatalogId = context.Catalogs.Where(p => p.Name == "Напитки").FirstOrDefault().Id,
                               Info = "Газировка",
                               Name = "Кока Кола",
                               Weight = 0.5,
                               Price = 2.00M,
                               Path = "cola.jpeg",
                           },
                             new Dish
                             {
                                 CatalogId = context.Catalogs.Where(p => p.Name == "Напитки").FirstOrDefault().Id,
                                 Info = "Газировка",
                                 Name = "Фанта",
                                 Weight = 0.5,
                                 Price = 2.00M,
                                 Path = "fant.jpeg",
                             },
                              new Dish
                              {
                                  CatalogId = context.Catalogs.Where(p => p.Name == "Супы").FirstOrDefault().Id,
                                  Info = "Грибы намеко, сыр Тофу, водоросли Вакаме",
                                  Name = "Мисо суп",
                                  Weight = 200,
                                  Price = 3.20M,
                                  Path = "ms.jpeg",
                              },
                               new Dish
                               {
                                   CatalogId = context.Catalogs.Where(p => p.Name == "Супы").FirstOrDefault().Id,
                                   Info = "Жареный лосось, яичная лапша, зелёный лук, острый соус",
                                   Name = "Отстрый суп с жареным лососем",
                                   Weight = 200,
                                   Price = 5.90M,
                                   Path = "ms.jpeg",
                               },
                                new Dish
                                {
                                    CatalogId = context.Catalogs.Where(p => p.Name == "Супы").FirstOrDefault().Id,
                                    Info = "Филе цыплёнка в соусе Терияки, водоросли Вакаме, яичная ламша, зелёный лук",
                                    Name = "Мисо с курицей",
                                    Weight = 200,
                                    Price = 4.20M,
                                    Path = "sk.jpeg"
                                });

                context.SaveChanges();

                context.Menus.AddRange(
            new Menu
            {
                ProviderId = context.Providers.Where(p => p.Email == "holavkysno@gmail.com").FirstOrDefault().Id,
                Date = DateTime.Now,
                Info = "Вкусные блинчики и не только"
            },
             new Menu
             {
                 ProviderId = context.Providers.Where(p => p.Email == "shawarma777@gmail.com").FirstOrDefault().Id,
                 Date = DateTime.Now,
                 Info = "Шаурма и напитки"
             },
                new Menu
                {
                    ProviderId = context.Providers.Where(p => p.Email == "holavkysno@gmail.com").FirstOrDefault().Id,
                    Date = DateTime.Now.AddDays(1),
                    Info = "Сэндвичи и не только",
                });

                context.SaveChanges();

                context.MenuDishes.AddRange(
                    new MenuDishes
                    {
                        MenuId = context.Menus.Where(p => p.Info == "Вкусные блинчики и не только").FirstOrDefault().Id,
                        DishId = context.Dishes.Where(p => p.Name == "Блинчики Фирменные").FirstOrDefault().Id
                    },
                     new MenuDishes
                     {
                         MenuId = context.Menus.Where(p => p.Info == "Вкусные блинчики и не только").FirstOrDefault().Id,
                         DishId = context.Dishes.Where(p => p.Name == "Блинчики Рыбные").FirstOrDefault().Id
                     },
                      new MenuDishes
                      {
                          MenuId = context.Menus.Where(p => p.Info == "Вкусные блинчики и не только").FirstOrDefault().Id,
                          DishId = context.Dishes.Where(p => p.Name == "Блинчики Друзья").FirstOrDefault().Id
                      },
                       new MenuDishes
                       {
                           MenuId = context.Menus.Where(p => p.Info == "Вкусные блинчики и не только").FirstOrDefault().Id,
                           DishId = context.Dishes.Where(p => p.Name == "Салат Оливье").FirstOrDefault().Id
                       },
                       new MenuDishes
                       {
                           MenuId = context.Menus.Where(p => p.Info == "Сэндвичи и не только").FirstOrDefault().Id,
                           DishId = context.Dishes.Where(p => p.Name == "Салат Оливье").FirstOrDefault().Id
                       },
                        new MenuDishes
                        {
                            MenuId = context.Menus.Where(p => p.Info == "Сэндвичи и не только").FirstOrDefault().Id,
                            DishId = context.Dishes.Where(p => p.Name == "Vegetarian").FirstOrDefault().Id
                        },
                         new MenuDishes
                         {
                             MenuId = context.Menus.Where(p => p.Info == "Сэндвичи и не только").FirstOrDefault().Id,
                             DishId = context.Dishes.Where(p => p.Name == "Cheese friend").FirstOrDefault().Id
                         },
                          new MenuDishes
                          {
                              MenuId = context.Menus.Where(p => p.Info == "Шаурма и напитки").FirstOrDefault().Id,
                              DishId = context.Dishes.Where(p => p.Name == "Кока Кола").FirstOrDefault().Id
                          },
                           new MenuDishes
                           {
                               MenuId = context.Menus.Where(p => p.Info == "Шаурма и напитки").FirstOrDefault().Id,
                               DishId = context.Dishes.Where(p => p.Name == "Фанта").FirstOrDefault().Id
                           },
                            new MenuDishes
                            {
                                MenuId = context.Menus.Where(p => p.Info == "Шаурма и напитки").FirstOrDefault().Id,
                                DishId = context.Dishes.Where(p => p.Name == "Шаурма большая").FirstOrDefault().Id
                            },
                             new MenuDishes
                             {
                                 MenuId = context.Menus.Where(p => p.Info == "Шаурма и напитки").FirstOrDefault().Id,
                                 DishId = context.Dishes.Where(p => p.Name == "Шаурма в пите").FirstOrDefault().Id
                             }
                    );

                context.SaveChanges();

            }
        }
    }
}
