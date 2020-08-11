/*
MIT License

Copyright (c) 2019 

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections.Generic;
using Adventure.Utilities.Interfaces;
using JetBrains.Annotations;

namespace Adventure.Utilities
{
    /// <summary>
    /// The ContentManagement class offers a way to abstract all the game text away from the game and keeps it in one
    /// place. 
    /// </summary>
    [PublicAPI]
    public class ContentManagement : IContentManagement
    {
        private readonly Dictionary<string, object> _content = new Dictionary<string, object>();


        /// <summary>
        /// Add a content item to the content management system. The text that you add has to be provided with an
        /// identifier which is used to call back the text with in the game.
        /// </summary>
        /// <param name="identifier">The identifier used to recall text from the content management system.
        /// This identifier needs to be unique.</param>
        /// <param name="content">The text to be stored in the content management system.</param>
        /// <exception cref="ArgumentNullException">If either the identifier or the text is null or empty, then an
        /// ArgumentNullException is thrown. </exception>
        public void AddContentItem(string identifier, object content)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                throw new ArgumentNullException(nameof(identifier));
            }

            if (content == null)
                throw new ArgumentNullException(nameof(content));

            _content.Add(identifier, content);
        }

        /// <summary>
        /// Retrieve a content item by specifying the unique identifier for the content.
        /// </summary>
        /// <param name="identifier">The identifier of the content to retrieve.</param>
        /// <param name="def"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">If the identifier is null or empty, then throw an
        /// ArgumentNullException.</exception>
        public TType RetrieveContentItem<TType>(string identifier, TType def)
        {
            if (string.IsNullOrEmpty(identifier))
                throw new ArgumentNullException(nameof(identifier));

            if (_content.TryGetValue(identifier, out var obj))
            {
                switch (obj)
                {
                    case TType ele:
                        return ele;
                    case Func<TType> fac:
                        return fac();
                    case Lazy<TType> laz:
                        return laz.Value;
                    default:
                        return def;
                }
            }
  
            return def;            
        }

        /// <summary>
        /// Return the number of items in the content management system.
        /// </summary>
        public int CountContentItems => _content.Count;

        /// <summary>
        /// Check if an identifier exists in the content management system.
        /// </summary>
        /// <param name="identifier">The identifier to search for.</param>
        /// <returns>True if the identifier exists in the content management system; false otherwise.</returns>
        /// <exception cref="ArgumentNullException">If the identifier is null or empty, then throw an
        /// ArgumentNullException.</exception>
        public bool Exists(string identifier)
        {
            if (string.IsNullOrEmpty(identifier))
                throw new ArgumentNullException(nameof(identifier));

            return _content.ContainsKey(identifier);
        }        
    }
}
