﻿
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
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Xpandables.Net.CQRS;

namespace Xpandables.Net.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly IDispatcher _dispatcher;
        public ContactsController(IDispatcher dispatcher) => _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

        [Route("")]
        [HttpGet]
        public IAsyncEnumerable<Contact> SelectAllAsync([FromQuery] SelectAll selectAll, CancellationToken cancellationToken = default)
        {
            return _dispatcher.FetchAsync(selectAll, cancellationToken);
            //await foreach (var contact in _dispatcher.InvokeAsync(selectAll, cancellationToken).ConfigureAwait(false))
            //    yield return contact;
        }

        [Route("{id}", Name = "ContactLink")]
        [HttpGet]
        public async Task<IActionResult> SelectAsync([FromRoute] Select select, CancellationToken cancellationToken = default)
        {
            var contact = await _dispatcher.FetchAsync(select, cancellationToken).ConfigureAwait(false);

            return contact.IsSuccess() ? Ok(contact) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] Add add, CancellationToken cancellationToken = default)
        {
            var id = await _dispatcher.SendAsync(add, cancellationToken).ConfigureAwait(false);
            return CreatedAtRoute("ContactLink", new { id = id.Value }, id.Value);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Delete del, CancellationToken cancellationToken = default)
        {
            await _dispatcher.SendAsync(del, cancellationToken).ConfigureAwait(false);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> EditAsync([FromBody] Edit edit, CancellationToken cancellationToken = default)
        {
            var updated = await _dispatcher.SendAsync(edit, cancellationToken).ConfigureAwait(false);
            return Ok(updated.Value);
        }
    }
}
