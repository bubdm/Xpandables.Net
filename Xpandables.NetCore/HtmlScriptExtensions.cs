
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

using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Xpandables.NetCore
{
    /// <summary>
    /// Provides with helper to add script to partial views.
    /// </summary>
    public static class HtmlScriptExtensions
    {
        /// <summary>
        /// Adds a partial view script to the HTTP context to be rendered in the parent view.
        /// In the partial view, add the script like so : @Html.Script(/* your script here */)
        /// </summary>
        /// <param name="htmlHelper">The helper to act on.</param>
        /// <param name="template">The script template to be used.</param>
        public static IHtmlHelper Script(this IHtmlHelper htmlHelper, Func<object?, HelperResult> template)
        {
            htmlHelper.ViewContext.HttpContext.Items[$"_script_{Guid.NewGuid()}"] = template;
            return htmlHelper;
        }

        /// <summary>
        /// Renders any scripts used within the partial views.
        /// In the layout view, add the script like so : @Html.RenderPartialViewScript()
        /// </summary>
        /// <param name="htmlHelper">The helper to act on.</param>
        public static IHtmlHelper RenderPartialViewScript(this IHtmlHelper htmlHelper)
        {
            foreach (var key in htmlHelper.ViewContext.HttpContext.Items.Keys)
            {
                if (key.ToString()!.StartsWith("_script_")
                    && htmlHelper.ViewContext.HttpContext.Items[key] is Func<object?, HelperResult> template)
                {
                    htmlHelper.ViewContext.Writer.Write(template(null));
                }
            }

            return htmlHelper;
        }
    }
}
