// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using JetBrains.Annotations;
using TextAdventure.Editor.ViewModels;

namespace TextAdventure.Editor
{
    [PublicAPI]
    public class ViewLocator : IDataTemplate
    {
        public bool SupportsRecycling => false;

        public IControl Build(object data)
        {
            var name = data.GetType().FullName?.Replace("ViewModel", "View");
            if (!string.IsNullOrEmpty(name))
            {
                var type = Type.GetType(name);

                if (type != null)
                {
                    if (Activator.CreateInstance(type) is Control c)
                        return c;
                }
            }

            return new TextBlock { Text = "Not Found: " + name };
        }

        public bool Match(object data) => data is ViewModelBase;
    }
}