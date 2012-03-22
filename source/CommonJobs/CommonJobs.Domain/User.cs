using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace CommonJobs.Domain
{
    public class User
    {
        public User(string userName, string password)
        {
            UserName = userName;
            Id = string.Format("users/{0}", userName); //TODO: mejorar esto un poco
            SetPassword(password);
        }

        public string Id { get; set; }
        const string ConstantSalt = "oi83baqd15w2#";
        public string UserName { get; set; }
        protected string HashedPassword { get; private set; }
        private Guid passwordSalt;
        private Guid PasswordSalt
        {
            get
            {
                if (passwordSalt == Guid.Empty)
                    passwordSalt = Guid.NewGuid();
                return passwordSalt;
            }
            set { passwordSalt = value; }
        }
        public void SetPassword(string pwd)
        {
            HashedPassword = GetHashedPassword(pwd);
        }
        private string GetHashedPassword(string pwd)
        {
            string hashedPassword;
            using (var sha = SHA256.Create())
            {
                var saltPerUser = Id;
                var computedHash = sha.ComputeHash(
                    PasswordSalt.ToByteArray().Concat(
                        Encoding.Unicode.GetBytes(saltPerUser + pwd + ConstantSalt)
                        ).ToArray()
                    );

                hashedPassword = Convert.ToBase64String(computedHash);
            }
            return hashedPassword;
        }
        public bool ValidatePassword(string maybePwd)
        {
            return HashedPassword == GetHashedPassword(maybePwd);
        }
    }
}
