using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mpi.Auth.Server.Models
{
    public class AptifyMember
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public MembershipLevel MembershipLevel { get; set; }
    }

    public class MembershipLevel {
        public const string NON_MEMBER = "non_member";
        public const string BASIC = "essential";
        public const string LEADER = "preferred";
        public const string CHAPTER_OWNER = "premium";
        public const string ADMIN = "admin";
    }

}
