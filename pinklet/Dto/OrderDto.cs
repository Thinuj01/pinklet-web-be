using pinklet.Controllers;

namespace pinklet.Dto
{
    public class OrderDetailsDto
    {
        public int Id { get; set; }
        public string OrderId { get; set; }
        public int UserId { get; set; }
        public int CartId { get; set; }
        public double TotalAmount { get; set; }
        public string Address { get; set; }
        public string RecipientName { get; set; }
        public string ClientFullName { get; set; }
        public string ClientPhoneNumber { get; set; }
        public string RecipientPhoneNumber { get; set; }
        public string Progress { get; set; }
        public DateTime OrderedDate { get; set; }
        public string District { get; set; }
        public string PostalCode { get; set; }
        public string? DeliveryNote { get; set; }
        public string? OrderNote { get; set; }
        public string ClientEmailAddress { get; set; }

        public CartDetailsDto Cart { get; set; }

    }

    public class OrderDto
    {
        public int UserId { get; set; }
        public int CartId { get; set; }
        public double TotalAmount { get; set; }
        public string Address { get; set; }
        public string RecipientName { get; set; }
        public string ClientPhoneNumber { get; set; }
        public string RecipientPhoneNumber { get; set; }
        public string Progress { get; set; }
        public DateTime OrderedDate { get; set; }
        public string District { get; set; }
        public string PostalCode { get; set; }
        public string? DeliveryNote { get; set; }
        public string? OrderNote { get; set; }
        public string OrderId { get; set; }
        public string ClientEmailAddress { get; set; }
        public string ClientFullName { get; set; }
    }

    public class CartDto
    {
        public int UserId { get; set; }
        public int PackageId { get; set; }
    }
    public class CartDetailsDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public PackageDto Package { get; set; }
        public Boolean IsCheckedOut { get; set; }
    }

    public class PackageDto
    {
        public int Id { get; set; }
        public string PackageName { get; set; }
        public List<ItemPackageDto> ItemPackages { get; set; }
        public double TotalAmount { get; set; }
        public int TotalItems { get; set; }
        public int TotalCategories { get; set; }
        public CakeDto? Cake { get; set; }

    }

    public class CakeDto
    {
        public int Id { get; set; }
        public string CakeCode { get; set; }
        public string CakeName { get; set; }
        public string CakeCategory { get; set; }
        public string CakeTags { get; set; }
        public double CakePrice { get; set; }
        public int CakeRating { get; set; }
        public string CakeDescription { get; set; }
        public string? CakeImageLink1 { get; set; }
        public string? CakeImageLink2 { get; set; }
        public string? CakeImageLink3 { get; set; }
        public string? CakeImageLink4 { get; set; }
    }

    public class ItemPackageDto
    {
        public ItemDto Item { get; set; }
        public int Quantity { get; set; }
        public int? Variant { get; set; }
    }

    public class ItemDto
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public string? ItemVariant { get; set; }
        public string ItemCategory { get; set; }
        public string ItemSubCategory { get; set; }
        public int? ItemStock { get; set; }
        public double? ItemPrice { get; set; }
        public int ItemRating { get; set; }
        public string ItemDescription { get; set; }
        public string? ItemImageLink1 { get; set; }
        public string? ItemImageLink2 { get; set; }
        public string? ItemImageLink3 { get; set; }
        public string? ItemImageLink4 { get; set; }
        public string? ItemImageLink5 { get; set; }

    }
}
