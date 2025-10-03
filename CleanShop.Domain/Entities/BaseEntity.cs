using System;

namespace CleanShop.Domain.Entities;

public abstract class BaseEntity
{

      public DateTime CreatedAt { get; set; }
      public DateTime UpdatedAt { get; set; }
    
}
