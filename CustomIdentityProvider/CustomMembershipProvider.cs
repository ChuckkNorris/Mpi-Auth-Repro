using SitefinityWebApp.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Telerik.Sitefinity.Security.Data;
using Telerik.Sitefinity.Security.Model;
using System.Web.Security;
using Telerik.Sitefinity.Model;
using System.Collections.Specialized;
using Telerik.Sitefinity.Data;

namespace SitefinityWebApp.Providers {
    public class CustomMembershipProvider : MembershipDataProvider {

        #region Privates variables
        private ManagerInfo managerInfo;
        private string providerName = "CustomMembershipProvider";
        private DateTime minDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        #endregion

        #region Public Properties
        public CustomMembershipProviderEntities _ctx { get; set; }

        public string ProviderName {
            get { return this.providerName; }
            set { this.providerName = value; }
        }

        public override string ApplicationName {
            get {
                return "/CustomMembershipProvider";
            }
        }
        private ManagerInfo ManagerInfo {
            get {
                return this.managerInfo ?? (this.managerInfo = new ManagerInfo() {
                    ProviderName = this.ProviderName,
                    ManagerType = "Telerik.Sitefinity.Security.UserManager",
                    ApplicationName = this.ApplicationName
                });
            }
        }

        /// <summary>
        /// Gets the provider abilities for the current principal. E.g. which operations are supported and allowed
        /// </summary>
        /// <value>The provider abilities.</value>
        public override ProviderAbilities Abilities {
            get {
                var abilities = new ProviderAbilities { ProviderName = Name, ProviderType = GetType().FullName };
                abilities.AddAbility("GetUser", true, true);
                abilities.AddAbility("AddUser", true, true);
                abilities.AddAbility("DeleteUser", true, true);
                abilities.AddAbility("UpdateUser", true, true);
                abilities.AddAbility("ValidateUser", true, true);
                abilities.AddAbility("ResetPassword", true, true);
                abilities.AddAbility("GetPassword", true, true);
                return abilities;
            }
        }
        #endregion

        public CustomMembershipProvider() {
            Debug.WriteLine("At CTOR");
            
        }


        protected override void Initialize(string providerName, NameValueCollection config, Type managerType) {
            Debug.WriteLine($"At Initialize Method w/ provider: {providerName}");
            _ctx = new CustomMembershipProviderEntities();
            base.Initialize(providerName, config, managerType);
        }

        // * * * USER VALIDATION * * * //

        public override bool ValidateUser(string userName, string password) {
            Debug.WriteLine("At ValidateUser(string userName, string password)");
            //return base.ValidateUser(userName, password);
            Customer matchingUser = _ctx.Customers.FirstOrDefault(x => x.Username == userName);
            if (matchingUser is Customer)
                return matchingUser.Password == password;
            return false;
        }

        public override bool ValidateUser(User user, string password) {
            Debug.WriteLine("At ValidateUser(User user, string password)");
            Customer matchingUser = _ctx.Customers.FirstOrDefault(x => x.Email == user.Email || x.Username == user.UserName);
            if (matchingUser is Customer)
                return matchingUser.Password == password;
            return base.ValidateUser(user, password);
        }

        // * * * GET USERS * * * //

        public override User GetUser(Guid id) {
            Debug.WriteLine($"At GetUser(Guid id): {id}");
            Customer matchingCustomer = _ctx.Customers.FirstOrDefault(customer => customer.CustomerId == id);
            if (matchingCustomer is Customer) {
                return GetSitefinityUser(matchingCustomer);
            }
            return null;
        }

        public override User GetUserByEmail(string email) {
            Debug.WriteLine("At GetUserByEmail(string email)");
            Customer matchingCustomer = _ctx.Customers.FirstOrDefault(customer => customer.Email == email);
            if (matchingCustomer is Customer) {
                return GetSitefinityUser(matchingCustomer);
            }
            return null;
            //return base.GetUserByEmail(email);
        }

        public override User GetUser(string userName) {
            Debug.WriteLine("At GetUserByEmail(string userName)");
            Customer matchingCustomer = _ctx.Customers.FirstOrDefault(customer => customer.Username == userName);
            if (matchingCustomer is Customer) {
                return GetSitefinityUser(matchingCustomer);
            }
            return null;
            //return base.GetUser(userName);
        }

        public override IQueryable<User> GetUsers() {
            Debug.WriteLine("At GetUsers()");
            return _ctx.Customers.ToList().Select(customer => GetSitefinityUser(customer)).AsQueryable(); // as IQueryable<User>;
        }

        // * * * CREATE AND DELETE USERS * * * //

        public override User CreateUser(Guid id, string email) {
            Debug.WriteLine("At CreateUser(Guid id, string email)");
            var user = new User { ApplicationName = this.ApplicationName, Id = id };
            user.SetUserName(email);
            ((IDataItem)user).Provider = this;
            user.ManagerInfo = ManagerInfo;

            return user;
        }

        public override User CreateUser(string email) {
            Debug.WriteLine("At CreateUser(string email)");
            Customer toAdd = new Customer {
                Email = email
            };
            _ctx.Customers.Add(toAdd);
            _ctx.SaveChanges();
            return GetSitefinityUser(toAdd);
        }
        //public override User CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status) {
        //    return base.CreateUser(username, password, email, passwordQuestion, passwordAnswer, isApproved, providerUserKey, out status);
        //}
        public override User CreateUser(string email, string password, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status) {
            Debug.WriteLine("At CreateUser(blah blah blah)");
            var salt = this.GenerateSalt();

            // Get a UTC DateTime
            var utcNow = DateTime.UtcNow;

            // Create a new user with the parameters entered from the Sitefinity backend
            var empty = this.CreateUser(Guid.NewGuid(), email);
            empty.Password = this.EncodePassword(password, salt, this.PasswordFormat);
            empty.PasswordAnswer = this.EncodePassword(passwordAnswer.ToUpperInvariant(), null, this.PasswordFormat);
            empty.Salt = salt;
            empty.Email = email;
            empty.Comment = string.Empty;
            empty.IsApproved = isApproved;
            empty.FailedPasswordAttemptCount = 0;
            empty.FailedPasswordAttemptWindowStart = utcNow;
            empty.FailedPasswordAnswerAttemptCount = 0;
            empty.FailedPasswordAnswerAttemptWindowStart = utcNow;
            empty.PasswordFormat = (int)PasswordFormat;
            empty.PasswordAnswer = passwordAnswer;
            empty.SetPasswordQuestion(passwordQuestion);
            empty.SetCreationDate(DateTime.Now);
            empty.ExternalProviderName = ProviderName;

            Customer toAdd = new Customer {
                CustomerId = empty.Id,
                Username = empty.UserName,
                Firstname = "Levi",
                Lastname = "Fuller",
                Email = empty.Email,
                Password = empty.Password,
                Salt = empty.Salt,
                IsApproved = empty.IsApproved
            };
            Debug.WriteLine(providerUserKey);
            _ctx.Customers.Add(toAdd);
            _ctx.SaveChanges();
            status = MembershipCreateStatus.Success;
            return empty;
            //return base.CreateUser(email, password, passwordQuestion, passwordAnswer, isApproved, providerUserKey, out status);
        }

        public override void Delete(User item) {
            Debug.WriteLine("At Delete(User item)");
            Customer matchingCustomer = _ctx.Customers.FirstOrDefault(customer => customer.CustomerId == item.Id);
            if (matchingCustomer is Customer) {
                Debug.WriteLine("User to delete found!");
                _ctx.Customers.Remove(matchingCustomer);
                _ctx.SaveChanges();
            }
        }

        //public User ConvertToUser(Customer customer) {
        //    var user = new User {
        //        ApplicationName = this.ApplicationName,
        //        IsBackendUser = false,
        //        Id = customer.CustomerId,
        //        Email = customer.Email,
        //        Comment = string.Empty,
        //        LastActivityDate = DateTime.UtcNow.AddDays(-1),
        //        LastModified = minDate,
        //        LastLoginDate = minDate,
        //        FailedPasswordAnswerAttemptWindowStart = minDate,
        //        FailedPasswordAttemptWindowStart = minDate,
        //        Password = customer.Password,
        //        Salt = customer.Salt,
        //        ManagerInfo = ManagerInfo,
        //        IsApproved = customer.IsApproved,
        //        PasswordFormat = 2
        //    };
        //    User toReturn = new User {
        //        Id = customer.CustomerId,
        //        Email = customer.Email,
        //        Salt = customer.Salt,
        //        Password = customer.Password
        //    };
        //    return toReturn;
        //}

        private User GetSitefinityUser(Customer customer) {
            var user = new User {
                ApplicationName = this.ApplicationName,
                IsBackendUser = false,
                Id = customer.CustomerId,
                FirstName = customer.Firstname,
                LastName = customer.Lastname,
                Email = customer.Email,
                Comment = string.Empty,
                LastActivityDate = DateTime.UtcNow.AddDays(-1),
                LastModified = minDate,
                LastLoginDate = minDate,
                FailedPasswordAnswerAttemptWindowStart = minDate,
                FailedPasswordAttemptWindowStart = minDate,
                Password = customer.Password,
                Salt = customer.Salt,
                ManagerInfo = managerInfo,
                IsApproved = customer.IsApproved,
                PasswordFormat = 2,
                ExternalProviderName = ProviderName
            };

            user.SetUserName(customer.Username);
            user.SetCreationDate(DateTime.Now);
            user.SetIsLockedOut(false);
            user.SetLastLockoutDate(DateTime.Now);
            user.SetLastPasswordChangedDate(DateTime.Now);
            user.SetPasswordQuestion("question");

            return user;
        }

    }

    public static class CustomerExtensions {
        public static User ToUser(this Customer customer) {
            User toReturn = new User {
                Id = customer.CustomerId,
                Email = customer.Email,
            };
            return toReturn;
        }
    }
}