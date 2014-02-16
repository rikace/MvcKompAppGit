using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class OrderDetail : IEntity
    {
        [ForeignKey("President")]
        public int OrderId { get; set; }

        [ForeignKey("president")]
        public int ProductId { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public virtual Product Product { get; set; }
        public virtual President President { get; set; }
        public int Id { get; set; }
    }
}