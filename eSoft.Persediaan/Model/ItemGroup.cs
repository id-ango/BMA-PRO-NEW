using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSoft.Persediaan.Model
{
    [Table("ItemGroup")]
    public class ItemGroup
    {
        [Key]
        public int ItemGroupId { get; set; }

        [Required]
        [MaxLength(20)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public int? ParentId { get; set; }

        [ForeignKey(nameof(ParentId))]
        public ItemGroup? Parent { get; set; }

        public ICollection<ItemGroup> Children { get; set; } = new List<ItemGroup>();

        public ICollection<IcItem> Items { get; set; } = new List<IcItem>();
    }
}
