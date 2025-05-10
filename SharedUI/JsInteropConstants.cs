// <copyright file="JsInteropConstants.cs" company="Joorak Rezapour">
// Copyright (c) Joorak Rezapour. All rights reserved.
// </copyright>

using Domain.Entities;

namespace SharedUI
{
    /// <summary>
    /// A runtime JS constants.
    /// </summary>
    public static class JsInteropConstants
    {
        /// <summary>
        /// Gets the session storage.
        /// </summary>
        public const string GetSessionStorage = $"{FuncPrefix}.getSessionStorage";

        /// <summary>
        /// Sets the session storage.
        /// </summary>
        public const string SetSessionStorage = $"{FuncPrefix}.setSessionStorage";

        /// <summary>
        /// The modal name.
        /// </summary>
        public const string HideModal = $"{FuncPrefix}.hideModal";

        /// <summary>
        /// The name of the JS runtimes.
        /// </summary>
        private const string FuncPrefix = "Leasik";
    }
    public class SellableContainer
    {
        public RenderFragment<ContainerContext> ContainerTemplate { get; set; }
        public RenderFragment<SellableItem> ItemTemplate { get; set; }
        public List<SellableItem> Items { get; set; }
    }

    public class ContainerContext
    {
        public List<SellableItem> Items { get; set; }
        public RenderFragment RenderItems { get; set; }
    }
}
