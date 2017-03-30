using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mpi.Auth.Server.Entities.Member
{
    public class Member {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MembershipLevel { get; set; }
    }

    public class MembershipLevel {
        public const string NON_MEMBER = "non_member";
        public const string BASIC = "basic";
        public const string LEADER = "leader";
        public const string CHAPTER_OWNER = "chapter_owner";
        public const string ADMIN = "admin";
    }
}
