namespace CleanArchitecture.Domain.Entities
{
    public class ItemMaster
    {
        //Just a portion of a whole 185 fields for now
        public string? ItemNumber { get; set; }
        public string? ItemTermType { get; set; }
        public string? ItemType { get; set; }
        public string? ItemDescription { get; set; }
        public int ItemProdLine { get; set; }
        public decimal ItemWeight { get; set; }
    }
}
