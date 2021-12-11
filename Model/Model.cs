using System;

namespace ApiClientApp.Model
{
    public class Model
    {
    }
    public class StockModel
    {
        public string? VariantCode { get; set; }
        public string? ProductCode { get; set; }
        public DateTime CreateTime { get; set; }
        public int Quantity { get; set; } = 1;
        public int Id { get; set; }
        public DateTime UpdateTime { get; set; }
    }

}
