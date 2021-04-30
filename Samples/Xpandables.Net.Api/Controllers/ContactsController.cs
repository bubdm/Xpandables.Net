
/************************************************************************************************************
 * Copyright (C) 2020 Francis-Black EWANE
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
************************************************************************************************************/
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Api.Database;
using Xpandables.Net.Api.Handlers;
using Xpandables.Net.Correlations;
using Xpandables.Net.Database;
using Xpandables.Net.Dispatchers;
using Xpandables.Net.Http;

namespace Xpandables.Net.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ContactsController : ControllerBase
    {
        private readonly IDispatcher _dispatcher;

        public ContactsController(IDispatcher dispatcher, IDataContextTenantAccessor correlationCollection)
        {
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            correlationCollection.SetTenantName<ContactContext>();
        }

        [Route("")]
        [HttpGet]
        public IAsyncEnumerable<Contact> SelectAllAsync([FromQuery] SelectAllQuery selectAll, CancellationToken cancellationToken = default)
        {
            return _dispatcher.FetchAsync(selectAll, cancellationToken);
        }

        [Route("{id}", Name = "ContactLink")]
        [HttpGet]
        public async Task<IActionResult> SelectAsync([FromRoute] SelectQuery select, CancellationToken cancellationToken = default)
            => Ok(await _dispatcher.FetchAsync(select, cancellationToken).ConfigureAwait(false));

        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] AddCommand add, CancellationToken cancellationToken = default)
        {
            var createdResult = await _dispatcher.SendAsync(add, cancellationToken).ConfigureAwait(false);
            if (createdResult.Failed) return Ok(createdResult);

            return CreatedAtRoute("ContactLink", new { id = createdResult.Value }, createdResult.Value);
        }

        [HttpDelete]
        [Route("{id:string}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] DeleteCommand del, CancellationToken cancellationToken = default)
            => Ok(await _dispatcher.SendAsync(del, cancellationToken).ConfigureAwait(false));

        [HttpPut]
        [Route("{id:string}")]
        public async Task<IActionResult> EditAsync([FromRoute] string id, [FromBody] EditCommand edit, CancellationToken cancellationToken = default)
        {
            edit.Id = id;
            return Ok(await _dispatcher.SendAsync(edit, cancellationToken).ConfigureAwait(false));
        }

        [HttpGet]
        [Route("ip/{id:string}")]
        public async Task<IActionResult> GetLocationAsync([FromRoute] GetIpQuery ipAddress,
            [FromServices] IHttpIPAddressLocationAccessor httpIpAddressLocationAccessor,
            [FromServices] IConfiguration configuration,
            CancellationToken cancellationToken = default)
        {
            var key = configuration["IPAddressStackKey"]!;
            var request = new IPAddressLocationRequest(ipAddress.Id, key);
            var location = await httpIpAddressLocationAccessor.ReadLocationAsync(request, cancellationToken).ConfigureAwait(false);

            return Ok(location.Result);
        }

        protected IOperationResult ApplyJsonPatch<TModel>(TModel model, JsonPatchDocument<TModel> jsonPatch)
                   where TModel : class
        {
            if (jsonPatch.Operations.Any(op => op.OperationType != OperationType.Replace))
            {
                return new FailureOperationResult(
                    HttpStatusCode.Unauthorized,
                    jsonPatch.Operations.Where(op => op.OperationType != OperationType.Replace).Select(op => new OperationError($"op {op.OperationType}", "Unauthorized")).ToList());
            }

            jsonPatch.ApplyTo(model, ModelState);
            return !ModelState.IsValid || !TryValidateModel(model) ? GetFailedOperationResult(ModelState) : new SuccessOperationResult();
        }

        protected IOperationResult GetFailedOperationResult(ModelStateDictionary modelState)
        {
            _ = modelState ?? throw new ArgumentNullException(nameof(modelState));
            return new FailureOperationResult(HttpStatusCode.BadRequest, modelState
                .Keys
                .Where(key => modelState[key].Errors.Count > 0)
                .Select(key => new OperationError(key, modelState[key].Errors.Select(error => error.ErrorMessage).ToArray()))
                .ToList());
        }
    }
}
