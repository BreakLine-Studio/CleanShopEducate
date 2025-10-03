using System;

namespace CleanShop.Domain.Entities;

public abstract class BaseEntity
{

      public DateOnly CreatedAt { get; set; } 
      public DateOnly UpdatedAt { get; set; } 
    
}
