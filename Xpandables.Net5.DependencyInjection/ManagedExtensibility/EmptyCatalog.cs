using System;
using System.ComponentModel.Composition.Primitives;
using System.Linq;

namespace Xpandables.Standard.ManagedExtensibility
{
    /// <summary>
    /// Defines a <see cref="ComposablePartCatalog"/> implementation that always returns an empty collection of parts.
    /// </summary>
    public sealed class EmptyCatalog : ComposablePartCatalog
    {
        /// <summary>
        /// Gets the part definitions that are contained in the catalog.
        /// </summary>
        /// <returns>The <see cref="ComposablePartDefinition" /> contained in the <see cref="ComposablePartCatalog" />.</returns>
        /// <exception cref="ObjectDisposedException">The <see cref="ComposablePartCatalog" /> object has been disposed of.</exception>
        public override IQueryable<ComposablePartDefinition> Parts => Enumerable.Empty<ComposablePartDefinition>().AsQueryable();
    }
}
