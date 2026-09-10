using AuctionService.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Data;

public static class DbInitializer
{
    public static void InitDb(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AuctionDbContext>();

        // Automatically apply any pending migrations
        context.Database.Migrate();

        // Guard: do not re-seed if auctions already exist
        if (context.Auctions.Any())
        {
            return;
        }

        var auctions = new List<Auction>
        {
            // 1. Ford GT - Bob (Live)
            new()
            {
                Id = "afbee524-5972-4075-8800-7d1f9d7b0a0c",
                Status = Status.Live,
                ReservePrice = 20000,
                Seller = "bob",
                AuctionEnd = DateTime.UtcNow.AddDays(10),
                Item = new Item
                {
                    Make = "Ford",
                    Model = "GT",
                    Color = "White",
                    Mileage = 50000,
                    Year = 2020,
                    ImageUrl = "https://cdn.pixabay.com/photo/2016/05/06/16/32/car-1376190_960_720.jpg",
                    Description = "Supercar with twin-turbocharged 3.5L EcoBoost V6 engine."
                }
            },
            // 2. Bugatti Veyron - Alice (Live)
            new()
            {
                Id = "c8c3ec17-01bf-49db-82aa-1ef80b833a9f",
                Status = Status.Live,
                ReservePrice = 90000,
                Seller = "alice",
                AuctionEnd = DateTime.UtcNow.AddDays(60),
                Item = new Item
                {
                    Make = "Bugatti",
                    Model = "Veyron",
                    Color = "Black",
                    Mileage = 15000,
                    Year = 2018,
                    ImageUrl = "https://cdn.pixabay.com/photo/2012/05/29/00/43/car-49278_960_720.jpg",
                    Description = "Iconic hypercar capable of reaching over 250 mph."
                }
            },
            // 3. Ford Mustang - Bob (Live)
            new()
            {
                Id = "bbab4d54-142b-4188-8775-6493a0f3cc93",
                Status = Status.Live,
                Seller = "bob",
                AuctionEnd = DateTime.UtcNow.AddDays(4),
                ReservePrice = 0,
                Item = new Item
                {
                    Make = "Ford",
                    Model = "Mustang",
                    Color = "Red",
                    Mileage = 65000,
                    Year = 2023,
                    ImageUrl = "https://cdn.pixabay.com/photo/2012/11/02/13/08/car-63930_960_720.jpg",
                    Description = "V8 5.0L GT fastback in Race Red with premium package."
                }
            },
            // 4. Mercedes SLK - Tom (Live)
            new()
            {
                Id = "155225c1-4448-4066-9886-6786536e05ea",
                Status = Status.Live,
                ReservePrice = 50000,
                Seller = "tom",
                AuctionEnd = DateTime.UtcNow.AddDays(13),
                Item = new Item
                {
                    Make = "Mercedes",
                    Model = "SLK",
                    Color = "Silver",
                    Mileage = 15001,
                    Year = 2020,
                    ImageUrl = "https://cdn.pixabay.com/photo/2016/04/17/22/10/mercedes-benz-1335674_960_720.png",
                    Description = "Luxury convertible roadster in pristine condition."
                }
            },
            // 5. BMW X1 - Alice (Live)
            new()
            {
                Id = "466e4744-4eb5-4266-a46f-dd2a5d1a39fb",
                Status = Status.Live,
                ReservePrice = 20000,
                Seller = "alice",
                AuctionEnd = DateTime.UtcNow.AddDays(30),
                Item = new Item
                {
                    Make = "BMW",
                    Model = "X1",
                    Color = "White",
                    Mileage = 90000,
                    Year = 2017,
                    ImageUrl = "https://cdn.pixabay.com/photo/2017/08/31/05/47/bmw-2699538_960_720.jpg",
                    Description = "Compact luxury crossover SUV, single-owner maintenance."
                }
            },
            // 6. Ferrari F40 - Bob (Live)
            new()
            {
                Id = "dc1e4071-d35f-4224-b089-b3378313e735",
                Status = Status.Live,
                ReservePrice = 200000,
                Seller = "bob",
                AuctionEnd = DateTime.UtcNow.AddDays(45),
                Item = new Item
                {
                    Make = "Ferrari",
                    Model = "F40",
                    Color = "Red",
                    Mileage = 1450,
                    Year = 1990,
                    ImageUrl = "https://cdn.pixabay.com/photo/2019/11/08/10/48/car-4610993_960_720.jpg",
                    Description = "Collector legendary twin-turbo V8 supercar in Rosso Corsa."
                }
            },
            // 7. Audi R8 - Tom (Finished)
            new()
            {
                Id = "47111973-d160-4620-ac35-4e7a60b9f7ac",
                Status = Status.Finished,
                ReservePrice = 150000,
                Seller = "tom",
                Winner = "alice",
                SoldAmount = 160000,
                CurrentHighBid = 160000,
                AuctionEnd = DateTime.UtcNow.AddDays(-10),
                Item = new Item
                {
                    Make = "Audi",
                    Model = "R8",
                    Color = "White",
                    Mileage = 10050,
                    Year = 2021,
                    ImageUrl = "https://cdn.pixabay.com/photo/2019/12/04/17/12/audi-4672970_960_720.jpg",
                    Description = "V10 Performance Coupe with Quattro all-wheel drive."
                }
            },
            // 8. Porsche 911 - Bob (ReserveNotMet)
            new()
            {
                Id = "6a5011a1-fc1f-4795-a578-0f49f493466f",
                Status = Status.ReserveNotMet,
                ReservePrice = 120000,
                Seller = "bob",
                CurrentHighBid = 105000,
                AuctionEnd = DateTime.UtcNow.AddDays(-5),
                Item = new Item
                {
                    Make = "Porsche",
                    Model = "911",
                    Color = "Yellow",
                    Mileage = 25000,
                    Year = 2020,
                    ImageUrl = "https://cdn.pixabay.com/photo/2015/05/15/14/46/porsche-768602_960_720.jpg",
                    Description = "Porsche 911 Carrera S in Racing Yellow with Sport Chrono package."
                }
            },
            // 9. Dodge Challenger - Alice (Live)
            new()
            {
                Id = "4049006d-55e9-4916-b80a-426db4d0c142",
                Status = Status.Live,
                ReservePrice = 35000,
                Seller = "alice",
                AuctionEnd = DateTime.UtcNow.AddDays(20),
                Item = new Item
                {
                    Make = "Dodge",
                    Model = "Challenger",
                    Color = "Black",
                    Mileage = 40000,
                    Year = 2020,
                    ImageUrl = "https://cdn.pixabay.com/photo/2020/03/12/13/19/dodge-4925055_960_720.jpg",
                    Description = "SRT Hellcat 6.2L Supercharged HEMI V8 American muscle."
                }
            },
            // 10. Ford Focus - Tom (Live)
            new()
            {
                Id = "3659ac24-29ab-40c4-83eb-4422136a6fbc",
                Status = Status.Live,
                ReservePrice = 20000,
                Seller = "tom",
                AuctionEnd = DateTime.UtcNow.AddDays(18),
                Item = new Item
                {
                    Make = "Ford",
                    Model = "Focus",
                    Color = "Blue",
                    Mileage = 90000,
                    Year = 2018,
                    ImageUrl = "https://cdn.pixabay.com/photo/2017/08/31/05/47/car-2699539_960_720.jpg",
                    Description = "Reliable daily commuter with excellent fuel efficiency."
                }
            }
        };

        context.AddRange(auctions);
        context.SaveChanges();
    }
}
