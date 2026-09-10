using AuctionService.DTOs;
using AuctionService.Entities;
using Mapster;

namespace AuctionService.Mapping;

public class MappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Auction -> AuctionDto (flattening vehicle specifications from Item)
        config.NewConfig<Auction, AuctionDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Status, src => src.Status.ToString())
            .Map(dest => dest.Make, src => src.Item.Make)
            .Map(dest => dest.Model, src => src.Item.Model)
            .Map(dest => dest.Year, src => src.Item.Year)
            .Map(dest => dest.Color, src => src.Item.Color)
            .Map(dest => dest.Mileage, src => src.Item.Mileage)
            .Map(dest => dest.Description, src => src.Item.Description)
            .Map(dest => dest.ImageUrl, src => src.Item.ImageUrl);

        // CreateAuctionDto -> Auction (constructs and populates child Item)
        config.NewConfig<CreateAuctionDto, Auction>()
            .Map(dest => dest.Item, src => src.Adapt<Item>());

        // UpdateAuctionDto -> Item (partial update: ignore null fields)
        config.NewConfig<UpdateAuctionDto, Item>()
            .IgnoreNullValues(true);
    }
}
