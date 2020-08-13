/*
MIT License

Copyright(c) 2019 

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
using Adventure.TextProcessing.Interfaces;

namespace Adventure.TextProcessing.Synonyms
{
    /// <summary>
    /// Mapping of verb synonyms to verb enum codes. This is where the parser will reduce the different input verb
    /// synonyms to the verbs in the enumeration.
    /// </summary>
    public class VerbSynonyms : IVerbSynonyms
    {
        readonly Dictionary<string, VerbCodes> _synonymMappings = new Dictionary<string, VerbCodes>();

        /// <summary>
        /// Constructor that maps the default set of synonyms supported by the game engine.
        /// </summary>
        public VerbSynonyms()
        {
            Add("gehen", VerbCodes.Go);
            Add("fahren", VerbCodes.Go);
            Add("springen", VerbCodes.Go);
            Add("laufen", VerbCodes.Go);
            Add("schlurfen", VerbCodes.Go);
            Add("kriechen", VerbCodes.Go);
            Add("gleiten", VerbCodes.Go);
            //Add("scuffle", VerbCodes.Go);
            Add("wackeln", VerbCodes.Go);
            //Add("skip", VerbCodes.Go);
            Add("tänzeln", VerbCodes.Go);
            Add("tanzen", VerbCodes.Go);

            Add("wählen", VerbCodes.Take);
            Add("nehmen", VerbCodes.Take);
            Add("sammeln", VerbCodes.Take);
            Add("stehlen", VerbCodes.Take);
            Add("erhalten", VerbCodes.Take);
            Add("akzeptieren", VerbCodes.Take);
            Add("fangen", VerbCodes.Take);
            Add("ernten", VerbCodes.Take);
            Add("halten", VerbCodes.Take);
            Add("ereichen", VerbCodes.Take);
            Add("erwerben", VerbCodes.Take);
            Add("erlangen", VerbCodes.Take);
            Add("erwischen", VerbCodes.Take);
            //Add("clasp", VerbCodes.Take);
            Add("greifen", VerbCodes.Take);
            Add("stricken", VerbCodes.Take);
            //Add("grasp", VerbCodes.Take);
            Add("bekommen", VerbCodes.Take);
            //Add("reap", VerbCodes.Take);
            //Add("snag", VerbCodes.Take);
            Add("sichern", VerbCodes.Take);
            Add("schnappen", VerbCodes.Take);
            //Add("gain", VerbCodes.Take);
            //Add("gather", VerbCodes.Take);
            Add("übernehmen", VerbCodes.Take);
            Add("ziehen", VerbCodes.Take);

            Add("fallenlassen", VerbCodes.Drop);
            Add("aufgeben", VerbCodes.Drop);
            Add("freigeben", VerbCodes.Drop);
            Add("verwerfen", VerbCodes.Drop);
            Add("lassen", VerbCodes.Drop);
            //Add("desert", VerbCodes.Drop);
            Add("entlassen", VerbCodes.Drop);
            Add("ablehen", VerbCodes.Drop);
            Add("verstoßen", VerbCodes.Drop);
            Add("verwirken", VerbCodes.Drop);
            Add("verzichten", VerbCodes.Drop);
            Add("entsagen", VerbCodes.Drop);
            Add("ablegen", VerbCodes.Drop);
            Add("opfern", VerbCodes.Drop);
            Add("terminieren", VerbCodes.Drop);

            Add("benutzen", VerbCodes.Use);
            Add("befäftigen", VerbCodes.Use);
            Add("anwenden", VerbCodes.Use);
            Add("wirken", VerbCodes.Use);
            Add("nutzen", VerbCodes.Use);
            Add("ausüben", VerbCodes.Use);
            Add("führen", VerbCodes.Use);
            //Add("ziehen", VerbCodes.Use);
            Add("schieben", VerbCodes.Use);
            //Add("flick", VerbCodes.Use);
            //Add("flip", VerbCodes.Use);
            Add("drehen", VerbCodes.Use);
            Add("entsperren", VerbCodes.Use);
            Add("schalten", VerbCodes.Use);
            Add("klettern", VerbCodes.Use);
            Add("öffnen", VerbCodes.Use);


            Add("ansehen", VerbCodes.Look);
            Add("prüfen", VerbCodes.Look);
            Add("schauen", VerbCodes.Look);
            Add("überprüfen", VerbCodes.Look);
            Add("starren", VerbCodes.Look);
            Add("betrachten", VerbCodes.Look);
            //Add("cast", VerbCodes.Look);
            //Add("gander", VerbCodes.Look);
            Add("blicken", VerbCodes.Look);
            Add("inspiziern", VerbCodes.Look);
            Add("beobachten", VerbCodes.Look);
            Add("aufpassen", VerbCodes.Look);
            Add("erkennen", VerbCodes.Look);
            Add("erblicken", VerbCodes.Look);
            Add("erheben", VerbCodes.Look);
            Add("lesen", VerbCodes.Look);

            Add("helfen", VerbCodes.Hint);
            Add("tipp", VerbCodes.Hint);
            Add("tipps", VerbCodes.Hint);
            Add("hinweis", VerbCodes.Hint);
            Add("hinweise", VerbCodes.Hint);

            Add("angriff", VerbCodes.Attack);
            Add("treffer", VerbCodes.Attack);
            Add("rempeln", VerbCodes.Attack);
            Add("treten", VerbCodes.Attack);
            Add("schlagen", VerbCodes.Attack);
            Add("zertrümmern", VerbCodes.Attack);
            Add("stechen", VerbCodes.Attack);
            //Add("slap", VerbCodes.Attack);
            Add("boxen", VerbCodes.Attack);
            Add("schneiden", VerbCodes.Attack);

            Add("teleportieren", VerbCodes.Visit);
            Add("besuchen", VerbCodes.Visit);
            Add("aufsuchen", VerbCodes.Visit);

            Add("essen", VerbCodes.Eat);
            Add("verschlingen", VerbCodes.Eat);
            Add("verbrauchen", VerbCodes.Eat);
            Add("kauen", VerbCodes.Eat);
            Add("schlucken", VerbCodes.Eat);
            Add("herunterschlucken", VerbCodes.Eat);

            Add("walk", VerbCodes.Go);
            Add("go", VerbCodes.Go);
            Add("hop", VerbCodes.Go);
            Add("run", VerbCodes.Go);
            Add("shuffle", VerbCodes.Go);
            Add("crawl", VerbCodes.Go);
            Add("slide", VerbCodes.Go);
            Add("scuffle", VerbCodes.Go);
            Add("wiggle", VerbCodes.Go);
            Add("skip", VerbCodes.Go);
            Add("prance", VerbCodes.Go);
            Add("mince", VerbCodes.Go);

            Add("pick", VerbCodes.Take);
            Add("grab", VerbCodes.Take);
            Add("collect", VerbCodes.Take);
            Add("steal", VerbCodes.Take);
            Add("get", VerbCodes.Take);
            Add("accept", VerbCodes.Take);
            Add("capture", VerbCodes.Take);
            Add("earn", VerbCodes.Take);
            Add("hold", VerbCodes.Take);
            Add("reach", VerbCodes.Take);
            Add("acquire", VerbCodes.Take);
            Add("attain", VerbCodes.Take);
            Add("catch", VerbCodes.Take);
            Add("clasp", VerbCodes.Take);
            Add("clutch", VerbCodes.Take);
            Add("ensnare", VerbCodes.Take);
            Add("grasp", VerbCodes.Take);
            Add("obtain", VerbCodes.Take);
            Add("reap", VerbCodes.Take);
            Add("snag", VerbCodes.Take);
            Add("secure", VerbCodes.Take);
            Add("snatch", VerbCodes.Take);
            Add("gain", VerbCodes.Take);
            Add("gather", VerbCodes.Take);
            Add("take", VerbCodes.Take);
            Add("haul", VerbCodes.Take);

            Add("drop", VerbCodes.Drop);
            Add("abandon", VerbCodes.Drop);
            Add("release", VerbCodes.Drop);
            Add("discard", VerbCodes.Drop);
            Add("leave", VerbCodes.Drop);
            Add("desert", VerbCodes.Drop);
            Add("dismiss", VerbCodes.Drop);
            Add("reject", VerbCodes.Drop);
            Add("disown", VerbCodes.Drop);
            Add("forfeit", VerbCodes.Drop);
            Add("relinquish", VerbCodes.Drop);
            Add("renounce", VerbCodes.Drop);
            Add("resign", VerbCodes.Drop);
            Add("sacrifice", VerbCodes.Drop);
            Add("terminate", VerbCodes.Drop);

            Add("use", VerbCodes.Use);
            Add("employ", VerbCodes.Use);
            Add("apply", VerbCodes.Use);
            Add("work", VerbCodes.Use);
            Add("ply", VerbCodes.Use);
            Add("exert", VerbCodes.Use);
            Add("wield", VerbCodes.Use);
            Add("pull", VerbCodes.Use);
            Add("push", VerbCodes.Use);
            Add("flick", VerbCodes.Use);
            Add("flip", VerbCodes.Use);
            Add("turn", VerbCodes.Use);
            Add("unlock", VerbCodes.Use);
            Add("switch", VerbCodes.Use);
            Add("climb", VerbCodes.Use);
            Add("open", VerbCodes.Use);


            Add("look", VerbCodes.Look);
            Add("examine", VerbCodes.Look);
            Add("peek", VerbCodes.Look);
            Add("review", VerbCodes.Look);
            Add("stare", VerbCodes.Look);
            Add("view", VerbCodes.Look);
            Add("cast", VerbCodes.Look);
            Add("gander", VerbCodes.Look);
            Add("gaze", VerbCodes.Look);
            Add("inspect", VerbCodes.Look);
            Add("observe", VerbCodes.Look);
            Add("watch", VerbCodes.Look);
            Add("see", VerbCodes.Look);
            Add("glance", VerbCodes.Look);
            Add("lift", VerbCodes.Look);
            Add("read", VerbCodes.Look);

            Add("help", VerbCodes.Hint);
            Add("hint", VerbCodes.Hint);
            Add("hints", VerbCodes.Hint);
            Add("clue", VerbCodes.Hint);
            Add("clues", VerbCodes.Hint);
            Add("sos", VerbCodes.Hint);

            Add("attack", VerbCodes.Attack);
            Add("hit", VerbCodes.Attack);
            Add("barge", VerbCodes.Attack);
            Add("kick", VerbCodes.Attack);
            Add("wallop", VerbCodes.Attack);
            Add("smash", VerbCodes.Attack);
            Add("stab", VerbCodes.Attack);
            Add("slap", VerbCodes.Attack);
            Add("punch", VerbCodes.Attack);
            Add("chop", VerbCodes.Attack);

            Add("teleport", VerbCodes.Visit);
            Add("visit", VerbCodes.Visit);
            Add("goto", VerbCodes.Visit);

            Add("eat", VerbCodes.Eat);
            Add("gobble", VerbCodes.Eat);
            Add("consume", VerbCodes.Eat);
            Add("munch", VerbCodes.Eat);
            Add("gulp", VerbCodes.Eat);
            Add("swallow", VerbCodes.Eat);
        }

        /// <summary>
        /// Add a new verb synonym mapping into the dictionary.
        /// </summary>
        /// <param name="synonym">The synonym to be mapped.</param>
        /// <param name="verb">The verb that the synonym maps onto.</param>
        /// <exception cref="ArgumentNullException">If either the synonym or verb inputs are null of empty, an
        /// ArgumentNullException is thrown.</exception>
        public void Add(string synonym, VerbCodes verb)
        {
            if (string.IsNullOrEmpty(synonym))
                throw new ArgumentNullException(nameof(synonym));

            _synonymMappings.Add(synonym, verb);
        }

        /// <summary>
        /// Return the base verb by providing one of it's synonyms. This is used by the parser to reduce potential
        /// synonyms down to the base verb to add into a command
        /// </summary>
        /// <param name="synonym">The synonym to return a verb for.</param>
        /// <returns>The mapped verb</returns>
        /// <exception cref="ArgumentNullException">If the synonym is null or empty throw an ArgumentNullException.</exception>
        public VerbCodes GetVerbForSynonym(string synonym)
        {
            if (string.IsNullOrEmpty(synonym))
                throw new ArgumentNullException(nameof(synonym));

            return _synonymMappings.ContainsKey(synonym) ? _synonymMappings[synonym] : VerbCodes.NoCommand;
        }
    }
}
