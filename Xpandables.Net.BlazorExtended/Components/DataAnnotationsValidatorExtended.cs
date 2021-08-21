
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
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Xpandables.Net.Components;

/// <summary>
/// Adds <see cref="IOperationResult"/> Data Annotations validation support to an <see cref="EditContext"/>.
/// </summary>
public class DataAnnotationsValidatorExtended : DataAnnotationsValidator
{
    private ValidationMessageStore _validationMessageStore = default!;

    [CascadingParameter]
    private EditContext AnnotationEditContext { get; set; } = default!;

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        _validationMessageStore = new(AnnotationEditContext);
        AnnotationEditContext.OnValidationRequested += (s, e) => _validationMessageStore.Clear();
        AnnotationEditContext.OnFieldChanged += (s, e) => _validationMessageStore.Clear(e.FieldIdentifier);
    }

    /// <summary>
    /// Applies <see cref="IOperationResult"/> errors to the context.
    /// </summary>
    /// <param name="result">The operation result to act with.</param>
    /// <remarks>Only available for failed result.</remarks>
    /// <exception cref="ArgumentNullException">The <paramref name="result"/> is null.</exception>
    public virtual void ValidateModel(IOperationResult result)
    {
        _ = result ?? throw new ArgumentNullException(nameof(result));

        if (result.IsSucceeded)
            return;

        foreach (var error in result.Errors)
        {
            _validationMessageStore.Add(AnnotationEditContext.Field(error.Key), error.ErrorMessages);
        }

        AnnotationEditContext.NotifyValidationStateChanged();
    }

    /// <summary>
    /// Removes the notifications from the context and notify changes.
    /// </summary>
    public virtual void ClearModel()
    {
        _validationMessageStore.Clear();
        AnnotationEditContext.NotifyValidationStateChanged();
    }
}
