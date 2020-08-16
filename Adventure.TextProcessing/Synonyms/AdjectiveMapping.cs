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
    public class AdjectiveMapping : IAdjectiveMapping
    {
        readonly Dictionary<string, string> _adjectiveMapping = new Dictionary<string, string>();

        public AdjectiveMapping()
        {
            // Appearance
            Add("bald");
            Add("beautiful");
            Add("chubby");
            Add("clean");
            Add("dazzling");
            Add("drab");
            Add("elegant");
            Add("fancy");
            Add("fit");
            Add("flabby");
            Add("glamorous");
            Add("gorgeous");
            Add("handsome");
            Add("magnificent");
            Add("muscular");
            Add("plain");
            Add("plump");
            Add("scruffy");
            Add("shapely");
            Add("skinny");
            Add("stock");
            Add("unkempt");
            Add("unsightly");

            // Positive Personality
            Add("agreeable");
            Add("ambitious");
            Add("brave");
            Add("calm");
            Add("delightful");
            Add("eager");
            Add("faithful");
            Add("gentle");
            Add("happy");
            Add("jolly");
            Add("kind");
            Add("lively");
            Add("nice");
            Add("obedient");
            Add("polite");
            Add("proud");
            Add("silly");
            Add("thankful");
            Add("victorious");
            Add("witty");
            Add("wonderful");
            Add("zealous");

            // Negative Personality
            Add("angry");
            Add("bewildered");
            Add("clumsy");
            Add("defeated");
            Add("embarrassed");
            Add("fierce");
            Add("grumpy");
            Add("helpless");
            Add("itchy");
            Add("jealous");
            Add("lazy");
            Add("mysterious");
            Add("nervous");
            Add("obnoxious");
            Add("panicky");
            Add("pitiful");
            Add("repulsive");
            Add("scary");
            Add("thoughtless");
            Add("uptight");
            Add("worried");

            // Size
            Add("big");
            Add("colossal");
            Add("fat");
            Add("gigantic");
            Add("great");
            Add("huge");
            Add("immense");
            Add("large");
            Add("little");
            Add("mammoth");
            Add("massive");
            Add("microscopic");
            Add("miniature");
            Add("petite");
            Add("puny");
            Add("scrawny");
            Add("short");
            Add("small");
            Add("tall");
            Add("teeny");
            Add("tiny");

            var german = new[]
                         {
                             "abstinent", "achtsam", "afrikanisch", "akkurat", "alkoholisch", "alphabetisch", "ängstlich", "ärgerlich", "automatisch", "baff", "bairisch", "bang", "bankrott",
                             "bedrohlich", "begünstigt", "behaglich", "beharrlich", "blind", "brillant", "bunt", "charmant", "chemisch", "chorisch", "christlich", "chronisch", "chronologisch", 
                             "cremefarben", "dämlich", "dämmrig", "dankbar", "darstellbar", "dazugehörig", "deckend", "demokratisch", "depressiv", "derb", "dialogisch", "diebisch", "dumm",
                             "dringend", "dünn", "eckig", "edel", "effizient", "egoistisch", "ehebrecherisch", "ehrerbietig", "ehrfürchtig", "ehrgeizig", "ehrlos", "eigenständig", "einladend", 
                             "elektrisch", "evangelisch", "fabelhaft", "fachkundig", "fad", "fade", "fadenscheinig", "fahrlässig", "faktisch", "fantasielos", "fantastisch", "fein", "fest", 
                             "fettig", "fit", "flach", "flauschig", "flott", "frei", "gängig", "garstig", "gastfreundlich", "gebogen", "gedrückt", "geeignet", "gefährlich", "gefangen", 
                             "geisterhaft", "gelb", "gereizt", "glatt", "gleichberechtigt", "glücklich", "grafisch", "groß", "gründlich", "haarig", "halb", "halsbrecherisch", "hämisch", 
                             "handlungsfähig", "heiß", "hell", "herzoglich", "hinfällig", "hoch", "hoffnungsvoll", "hündisch", "hyperaktiv", "ideal", "identisch", "idyllisch", "illegal", 
                             "illusorisch", "imaginär", "imponierend", "individuell", "inhaltlich", "inklusiv", "integriert", "international", "isoliert", "jährlich", "jetzig", "jodhaltig", 
                             "jordanisch", "jüdisch", "jugendlich", "jung", "jungfräulich", "juristisch", "kahl", "kalorisch", "kämpferisch", "katholisch", "käuflich", "keusch", "kirchlich", 
                             "klangvoll", "knackig", "kokett", "kontrovers", "korrekt", "krank", "krumm", "künstlich", "kurz", "labberig", "labbrig", "labil", "lahm", "ländlich", "laut", 
                             "lebensgroß", "legitim", "leicht", "lieb", "lockig", "lokal", "löslich", "luftig", "luxuriös", "mächtig", "männlich", "maßvoll", "materiell", "mehrsprachig", 
                             "meisterlich", "mental", "mickerig", "mickrig", "mitleidig", "monatlich", "motorisch", "musikalisch", "mutig", "mütterlich", "mystisch", "nächtlich", "nah", 
                             "närrisch", "nass", "negativ", "neidisch", "neu", "niedrig", "niveaulos", "nördlich", "normal", "notdürftig", "nützlich", "oberschlau", "obsolet", "obszön", 
                             "ockerfarben", "ockerfarbig", "öde", "öd", "offen", "öffentlich", "ökologisch", "ölig", "olympiareif", "operativ", "oral", "örtlich", "österlich", "ozonhaltig", 
                             "pädagogisch", "paradiesisch", "parkartig", "parlamentarisch", "passiv", "peinlich", "pensioniert", "persönlich", "perspektivlos", "pflichtbewusst", "phantastisch", 
                             "physisch", "politisch", "poetisch", "praktisch", "provokant", "psychisch", "qualitativ", "qualvoll", "quecksilberhaltig", "quengelig", "quergestreift", 
                             "quicklebendig", "quietschfidel", "quirlig", "rabenschwarz", "radikal", "raffiniert", "rankenartig", "rasch", "ratlos", "rauchfrei", "recyclebar", "reformbedürftig", 
                             "regnerisch", "reich", "rein", "relativ", "respektvoll", "rhythmisch", "riesig", "roh", "rostig", "rückläufig", "rund", "sachkundig", "sachlich", "saisonal", 
                             "salzig", "sauer", "scharf", "schattig", "schleimig", "schreckenerregend", "schusselig", "seidenweich", "selbstständig", "sesshaft", "sicher", "soft", "sperrig",
                             "spitz", "steil", "stramm", "sympathisch", "tailliert", "taktvoll", "technisch", "temperamentvoll", "theoretisch", "topografisch", "tot", "trächtig", "traditionell",
                             "treu", "trocken", "trotzig", "tüchtig", "typisch", "übel", "übertrieben", "überparteilich", "ultimativ", "ultrakurz", "umkehrbar", "umständlich", "unbehaglich", 
                             "unerlässlich", "uralt", "utopisch", "variabel", "väterlich", "verabscheuungswürdig", "verantwortungslos", "verblüfft", "verdaulich", "verklemmt", "versichert", 
                             "viertürig", "viral", "voll", "vulgär", "wach", "weise", "wahlberechtigt", "warm", "wässrig", "weich", "weihnachtlich", "weit", "weise", "weiß", "weitreichend", 
                             "wertvoll", "widerlegbar", "wirtschaftlich", "wohnlich", "würdig", "x-beliebig", "x-fach", "x-mal", "y-förmig", "youtubebegeistert", "zackig", "zahlenmäßig", 
                             "zappelig", "zapplig", "zart", "zehnjährig", "zeitlich", "zentral", "zickig", "zinslos", "zivil", "zornig", "zuckerfrei", "zuvorkommend", "zweckgebunden", "zweifach", 
                             "zynisch"
                         };

            foreach (var adj in german) _adjectiveMapping[adj] = adj;
        }

        public void Add(string adjective)
        {
            if (string.IsNullOrEmpty(adjective))
                throw new ArgumentNullException(nameof(adjective));

            _adjectiveMapping.Add(adjective, adjective);
        }

        public bool CheckAdjectiveExists(string adjective) 
            => !string.IsNullOrEmpty(adjective) && _adjectiveMapping.ContainsKey(adjective);
    }
}
