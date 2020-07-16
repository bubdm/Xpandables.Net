using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

using Xpandables.Net5.Expressions;
using Xpandables.Net5.HttpRestClient;
using Xpandables.Net5.Queries;
using Xpandables.Net5.ValidatorRules;
using Xpandables.Samples.Domain.Models;

namespace Xpandables.Samples.Business.Contracts
{
    [HttpRestClient(Path = "api/user/signin", IsSecured = false, BodyFormat = BodyFormat.String, ContentType = ContentType.Json)]
    public class SignInRequest : QueryExpression<User>, IQuery<SignInResponse>, IBehaviorValidation
    {
        public SignInRequest() { }
        public SignInRequest(string email, string password) => (Email, Password) = (email, password);

        protected override Expression<Func<User, bool>> BuildExpression() => user => user.Email == Email;

        [Required, EmailAddress, StringLength(byte.MaxValue, MinimumLength = 3)]
        [Display(Name = nameof(Email), Description = nameof(Email))]
        public string Email { get; set; }

        [Required, StringLength(byte.MaxValue, MinimumLength = 3)]
        [Display(Name = nameof(Password), Description = nameof(Password))]
        public string Password { get; set; }

        public object GetStreamContent() => this;
    }
}
