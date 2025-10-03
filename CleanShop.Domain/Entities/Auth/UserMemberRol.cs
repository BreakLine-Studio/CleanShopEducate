using System;

namespace CleanShop.Domain.Entities.Auth;

public class UserMemberRol : BaseEntity
{
        public int UserMemberId { get; set; }
        public UserMember UserMembers { get; set; } = null!;
        public int RolId { get; set; }
        public Rol Rol { get; set; }  = null!;
}
