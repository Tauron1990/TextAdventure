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

using Adventure.TextProcessing.Synonyms;
using JetBrains.Annotations;

namespace Adventure.TextProcessing.Interfaces
{
    public sealed class Verb
    {
        public VerbCodes VerbCode { get; }

        public string Text { get; }

        public Verb(VerbCodes verbCode, string text)
        {
            VerbCode = verbCode;
            Text = text;
        }
    }

    public sealed class Noun
    {
        public string Replace { get; }

        public string Original { get; }

        public Noun(string replace, string original)
        {
            Replace = replace;
            Original = original;
        }
    }

    public sealed class Adjective
    {
        public string Replace { get; }

        public string Original { get; }

        public Adjective(string replace, string original)
        {
            Replace = replace;
            Original = original;
        }
    }

    [PublicAPI]
    public interface ICommand
    {
        string FullTextCommand { get; set; }
        
        Verb Verb { get; set; }

        Adjective? Adjective { get; set; }

        Noun Noun { get; set; }

        PropositionEnum Preposition { get; set; }

        Adjective? Adjective2 { get; set; }

        Noun Noun2 { get; set; }

        PropositionEnum Preposition2 { get; set; }

        Adjective? Adjective3 { get; set; }

        Noun Noun3 { get; set; }

        bool ProfanityDetected { get; set; }

        string Profanity { get; set; }
    }
}
