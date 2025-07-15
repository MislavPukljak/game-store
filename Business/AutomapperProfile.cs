using AutoMapper;
using Business.DTO;
using Data.MongoDb.Entities;
using Data.SQL.Entities;

namespace Business;

public class AutomapperProfile : Profile
{
    public AutomapperProfile()
    {
        CreateMap<Game, GameDto>()
            .ForMember(dto => dto.Id, o => o.MapFrom(src => src.Id))
            .ForMember(dto => dto.Key, o => o.MapFrom(src => src.Alias))
            .ReverseMap();

        CreateMap<Game, CreateGameDto>()
            .ForPath(dto => dto.Game, o => o.MapFrom(i => new Game { Id = i.Id }));

        CreateMap<Genre, CreateGameDto>()
            .ForMember(dto => dto.Genres, o => o.MapFrom(src => src.Id));

        CreateMap<CreateGameDto, Genre>()
            .ForMember(dto => dto.Id, o => o.MapFrom(src => src.Genres));

        CreateMap<CreateGameDto, Publisher>()
            .ForMember(dto => dto.Id, o => o.MapFrom(src => src.Publishers))
            .ReverseMap();

        CreateMap<CreateModelGameDto, Game>()
            .ForMember(dto => dto.Discontinued, o => o.MapFrom(src => src.Discontinued))
            .ForMember(dto => dto.Alias, o => o.MapFrom(src => src.Key))
            .ForMember(dto => dto.Id, o => o.MapFrom(src => src.Id));

        CreateMap<Game, CreateModelGameDto>()
            .ForMember(dto => dto.Key, o => o.MapFrom(src => src.Alias))
            .ForMember(dto => dto.Id, o => o.MapFrom(src => src.Id));

        CreateMap<Genre, GenreDto>()
            .ForMember(dto => dto.Id, o => o.MapFrom(src => src.Id))
            .ForMember(dto => dto.Subcategory, o => o.MapFrom(src => src.Subcategory))
            .ReverseMap();

        CreateMap<PlatformDto, Data.SQL.Entities.Platform>()
            .ForMember(dto => dto.Id, o => o.MapFrom(src => src.Id))
            .ForMember(dto => dto.Type, o => o.MapFrom(src => src.Type));

        CreateMap<Data.SQL.Entities.Platform, PlatformDto>()
            .ForMember(dto => dto.Id, o => o.MapFrom(src => src.Id))
            .ForMember(dto => dto.Type, o => o.MapFrom(src => src.Type));

        CreateMap<PublisherDto, Publisher>()
            .ForMember(dto => dto.CompanyName, o => o.MapFrom(src => src.CompanyName))
            .ForMember(dto => dto.Id, o => o.MapFrom(src => src.Id))
            .ReverseMap();

        CreateMap<Data.SQL.Entities.Order, OrderDto>()
            .ForMember(dto => dto.OrderDetails, o => o.MapFrom(src => src.OrderDetails))
            .ForMember(dto => dto.Id, o => o.MapFrom(src => src.Id))
            .ReverseMap();

        CreateMap<CartDto, Cart>()
            .ForMember(dto => dto.CartItems, o => o.MapFrom(src => src.CartItems))
            .ForMember(dto => dto.Id, o => o.MapFrom(src => src.Id))
            .ReverseMap();

        CreateMap<Data.SQL.Entities.OrderDetail, OrderDetailDto>()
            .ForMember(dto => dto.ProductId, o => o.MapFrom(src => src.ProductId))
            .ForMember(dto => dto.ProductName, o => o.MapFrom(src => src.Product))
            .ReverseMap();

        CreateMap<CartItemsDto, CartItem>()
            .ForMember(dto => dto.ProductId, o => o.MapFrom(src => src.ProductId))
            .ForMember(dto => dto.Quantity, o => o.MapFrom(src => src.Quantity))
            .ForMember(dto => dto.Products, o => o.MapFrom(src => src.Products))
            .ReverseMap();

        CreateMap<PaymentOption, PaymentOptionDto>().ReverseMap();

        CreateMap<PlatformIBox, PlatformIBoxDto>().ReverseMap();

        CreateMap<Visa, VisaDto>().ReverseMap();

        CreateMap<Comment, CommentDto>()
            .ForMember(dto => dto.ChildComments, o => o.MapFrom(src => src.ChildComments))
            .ForMember(dto => dto.Id, o => o.MapFrom(src => src.Id))
            .ReverseMap();

        CreateMap<CommentDto, Comment>()
            .ForMember(dto => dto.ChildComments, o => o.MapFrom(src => src.ChildComments))
            .ForMember(dto => dto.Id, o => o.MapFrom(src => src.Id))
            .ReverseMap();

        CreateMap<Data.SQL.Entities.Customer, CustomerDto>()
            .ForMember(dto => dto.Id, o => o.MapFrom(src => src.Id))
            .ReverseMap();

        CreateMap<GamePageInfo, GamePageInfoDto>()
            .ForMember(dto => dto.Games, o => o.MapFrom(src => src.Games))
            .ReverseMap();

        CreateMap<ProductPageInfo, GamePageInfoDto>()
            .ForMember(dto => dto.Games, o => o.MapFrom(src => src.Products))
            .ReverseMap();

        CreateMap<GameFilter, GameFilterDto>().ReverseMap();

        CreateMap<ProductFilter, GameFilterDto>()
            .ForMember(dto => dto.Publishers, o => o.MapFrom(src => src.Supplier))
            .ForMember(dto => dto.Genres, o => o.MapFrom(src => src.Category))
            .ReverseMap();

        CreateMap<Data.SQL.Entities.Order, OrderHistoryDto>()
            .ForMember(dto => dto.Id, o => o.MapFrom(src => src.Id))
            .ForMember(dto => dto.CustomerId, o => o.MapFrom(src => src.CustomerId))
            .ForMember(dto => dto.OrderDate, o => o.MapFrom(src => src.OrderDate))
            .ReverseMap();

        CreateMap<Data.MongoDb.Entities.Order, OrderHistoryDto>()
            .ForMember(dto => dto.Id, o => o.MapFrom(src => src.Id))
            .ForMember(dto => dto.CustomerId, o => o.MapFrom(src => src.CustomerID))
            .ForMember(dto => dto.OrderDate, o => o.MapFrom(src => src.OrderDate))
            .ReverseMap();

        CreateMap<Supplier, PublisherDto>()
            .ForMember(dto => dto.CompanyName, o => o.MapFrom(src => src.CompanyName))
            .ForMember(dto => dto.Id, o => o.MapFrom(src => src.SupplierID))
            .ForMember(dto => dto.HomePage, o => o.MapFrom(src => src.HomePage))
            .ReverseMap();

        CreateMap<Category, GenreDto>()
            .ForMember(dto => dto.Id, o => o.MapFrom(src => src.CategoryId))
            .ForMember(dto => dto.Name, o => o.MapFrom(src => src.CategoryName))
            .ReverseMap();

        CreateMap<Product, GameDto>()
            .ForMember(dto => dto.Id, o => o.MapFrom(src => src.ProductID))
            .ForMember(dto => dto.Name, o => o.MapFrom(src => src.ProductName))
            .ForMember(dto => dto.Price, o => o.MapFrom(src => src.UnitPrice))
            .ForMember(dto => dto.Discontinued, o => o.MapFrom(src => src.Discontinued))
            .ForMember(dto => dto.UnitInStock, o => o.MapFrom(src => src.UnitsInStock))
            .ForMember(dto => dto.Key, o => o.MapFrom(src => src.Alias))
            .ForMember(dto => dto.GenreId, o => o.MapFrom(src => src.CategoryID))
            .ForMember(dto => dto.PublisherId, o => o.MapFrom(src => src.SupplierID))
            .ForMember(dto => dto.Description, o => o.MapFrom(src => src.QuantityPerUnit))
            .ReverseMap();

        CreateMap<Shipper, ShipperDto>().ReverseMap();

        CreateMap<Data.MongoDb.Entities.OrderDetail, OrderDetailDto>()
            .ForMember(dto => dto.Id, o => o.MapFrom(src => src.OrderID))
            .ForMember(dto => dto.ProductId, o => o.MapFrom(src => src.ProductID))
            .ForMember(dto => dto.Price, o => o.MapFrom(src => src.UnitPrice))
            .ForMember(dto => dto.Quantity, o => o.MapFrom(src => src.Quantity))
            .ForMember(dto => dto.Discount, o => o.MapFrom(src => src.Discount))
            .ReverseMap();

        CreateMap<Data.MongoDb.Entities.Customer, CustomerDto>()
            .ForMember(dto => dto.Id, o => o.MapFrom(src => src.CustomerID))
            .ForMember(dto => dto.Name, o => o.MapFrom(src => src.ContactName))
            .ReverseMap();

        CreateMap<ImagesData, ImageDataDto>()
            .ForMember(dto => dto.Id, o => o.MapFrom(src => src.Id))
            .ForMember(dto => dto.Name, o => o.MapFrom(src => src.Name))
            .ForMember(dto => dto.Container, o => o.MapFrom(src => src.Container))
            .ForMember(dto => dto.ContentType, o => o.MapFrom(src => src.ContentType))
            .ReverseMap();

        CreateMap<Notification, NotificationDto>()
            .ForMember(dto => dto.Id, o => o.MapFrom(src => src.Id))
            .ForMember(dto => dto.Queue, o => o.MapFrom(src => src.Queue))
            .ForMember(dto => dto.IsTurnedOff, o => o.MapFrom(src => src.IsTurnedOff))
            .ReverseMap();
    }
}
