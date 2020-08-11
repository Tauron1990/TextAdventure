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

using System.Text;
using Adventure.TextProcessing.Interfaces;
using Adventure.TextProcessing.Synonyms;
using JetBrains.Annotations;

namespace Adventure.TextProcessing
{ 
    [PublicAPI]
    public abstract class ParserBase
    {
        protected ParserBase()
        {
            Verbs = new VerbSynonyms();
            Nouns = new NounSynonyms();
            Prepositions = new PrepositionMapping();
            Adjectives = new AdjectiveMapping();
            EnableProfanityFilter = true;
        }

        /// <summary>
        /// Constructor that allows you to custom set the verb, noun and preposition synonyms used by the parser. This
        /// constructor is mostly used by the unit tests.
        /// </summary>
        /// <param name="verbSynonyms">Verb synonyms to be used by the parser.</param>
        /// <param name="nounSynonyms">Noun synonyms to be used by the parser.</param>
        /// <param name="prepositionMapping">Prepositions being used by the parser.</param>
        protected ParserBase(IVerbSynonyms verbSynonyms, INounSynonyms nounSynonyms, IPrepositionMapping prepositionMapping)
        {
            Verbs = verbSynonyms;
            Nouns = nounSynonyms;
            Prepositions = prepositionMapping;
            Adjectives = new AdjectiveMapping();
            EnableProfanityFilter = true;
        }

        /// <summary>
        /// Retrieve the verb synonyms being used by the parser.
        /// </summary>
        public IVerbSynonyms Verbs { get; }

        /// <summary>
        /// Retrieve the noun synonyms being used by the parser.
        /// </summary>
        public INounSynonyms Nouns { get; }

        /// <summary>
        /// If this flag is set to True, then the users input passed into the parser will also be scanned for profanity.
        /// It is not the intention of this engine to perform any censorship, but it can be useful to know if the player
        /// ius a potty mouthed little so and so. You can even use this fact as part of the narrative.
        /// </summary>
        public bool EnableProfanityFilter { get; set; }

        /// <summary>
        /// Retrieve the prepositions being used by the parser.
        /// </summary>
        public IPrepositionMapping Prepositions { get; }

        /// <summary>
        /// Gets or sets the adjectives.
        /// </summary>
        /// <value>The adjectives.</value>
        public IAdjectiveMapping Adjectives { get; }

        protected ParserStatesEnum ParserStates = ParserStatesEnum.Verb;
        protected ICommand Command;
        protected readonly IProfanityFilter ProfanityFilter = new ProfanityFilter();

        protected string RemovePunctuation(string s)
        {
            var result = new StringBuilder();

            // ReSharper disable once ForCanBeConvertedToForeach
            for (int i = 0; i < s.Length; i++)
            {
                if (char.IsWhiteSpace(s[i]))
                {
                    result.Append(" ");
                }
                else if (!char.IsLetter(s[i]) && !char.IsNumber(s[i])) { }
                else
                {
                    result.Append(s[i]);
                }
            }

            return result.ToString();
        }

        protected VerbCodes ProcessVerbs(string word, ParserStatesEnum nextState)
        {
            var verb = Verbs.GetVerbForSynonym(word);

            if (verb != VerbCodes.NoCommand) 
                ParserStates = nextState;

            return verb != VerbCodes.NoCommand ? verb : VerbCodes.NoCommand;
        }

        protected string ProcessNoun(string word, ParserStatesEnum nextState)
        {
            var noun = Nouns.GetNounForSynonym(word);
            if (string.IsNullOrEmpty(noun)) return string.Empty;
            ParserStates = nextState;
            return noun;

        }

        protected PropositionEnum ProcessPreposition(string word, ParserStatesEnum nextState)
        {
            var preposition = Prepositions.GetPreposition(word);

            if (preposition == PropositionEnum.NotRecognised) return PropositionEnum.NotRecognised;
            ParserStates = nextState;
            return preposition;

        }

        protected void CheckForProfanity(string lowerCase)
        {
            if (!EnableProfanityFilter) return;
            
            var profanity = ProfanityFilter.StringContainsFirstProfanity(lowerCase);
            if (string.IsNullOrEmpty(profanity)) return;
            
            Command.ProfanityDetected = true;
            Command.Profanity = profanity;
        }

        protected void SanitizeInput(string command, out string lowerCase, out string[] wordList)
        {
            lowerCase = command.ToLower();
            lowerCase = RemovePunctuation(lowerCase);

            wordList = lowerCase.Split(' ');
            Command = new Command();
        }
    }
}
