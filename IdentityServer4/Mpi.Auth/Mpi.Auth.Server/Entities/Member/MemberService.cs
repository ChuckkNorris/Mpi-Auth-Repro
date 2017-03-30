using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mpi.Auth.Server.Entities.Member
{
    public class MemberService
    {
        public bool ValidateCredentials(string username, string password) {
            // Get Username and password from DB
            // Lookup this call IdentityServer4.Test.TestUserStore.ValidateCredentials()
            return true;
        }

        public Member FindByUsername(string username) {
            // var matchingMember = _context.Members.FirstOrDefault(member => member.Username == username);
            Member toReturn = new Member {
                FirstName = "Tom",
                LastName = "Hanks",
                MembershipLevel = MembershipLevel.LEADER
            };
            return toReturn;
        }

        public void CreateNewMember(Member member) {

        }

    }
}
