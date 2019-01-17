using System.Collections.Generic;
using System.IO;
using LinearIndexedGrammarParser;
using Newtonsoft.Json;

namespace WordNetPOSReader
{
    //helper 1:
    //check parts of speech of desired word in merriam webster online

    //helper 2:
    //you can check the POS tag of a word in NLTK brown corpus as well.
    //use interactive python window and write (replace your-word with what you'd like to check)
    //import nltk
    //words = nltk.corpus.brown.tagged_words(tagset='universal')
    //x = set([y for (x,y) in words if x == desired word])
    //x

    class Program
    {
        public static Dictionary<string, HashSet<string>> CreateDictionary()
        {
            //generating universal vocabulary with 12 universal parts of speech:
            // https://github.com/slavpetrov/universal-pos-tags
            // https://www.nltk.org/_modules/nltk/tag/mapping.html

            var dict = new Dictionary<string, HashSet<string>>();

            //read from wordnet files (version 3.1)
            //these files include only open-ended word categories (content words)
            dict["ADV"] = GetWords("ADV", "index.adv"); //adverbs
            dict["N"] = GetWords("N", "index.noun");    //nouns
            dict["V"] = GetWords("V", "index.verb");    //verbs
            dict["ADJ"] = GetWords("ADJ", "index.adj");  //adjectives

            //add functional categories, a closed set.
            //those are the stop words for wordnet, appearing in:
            // http://www.d.umn.edu/~tpederse/Group01/WordNet/words.txt

            dict["PRON"] = new HashSet<string>() //pronouns
            {
                "i",
                "he",
                "me",
                "thou",
                "us",
                "his", //my eyes are brown and his are green (possessive pronoun)
                "who",
                "anybody",
                "anyone",
                "everybody",
                "everyone",
                "her",
                "hers", //possessive pronoun
                "herself",
                "him",
                "himself",
                "hisself",
                "idem",
                "it",
                "itself",
                "myself",
                "oneself",
                "ours",   //missing from the stop list of wordnet. 
                "ourself",
                "ourselves",
                "she",
                "that",
                "this",
                "these",
                "those",
                "such",
                "thee",
                "theirs",
                "them",
                "themselves",
                "they",
                "thyself",
                "thine",
                "tother",
                "we",
                "what",
                "whatall",
                "which",
                "whichever",
                "whichsoever",
                "whoever",
                "whom",
                "whomever",
                "whoso",
                "whomso",
                "whomsoever",
                "ye",
                "you",
                "you-all",
                "yours",
                "yourself",
                "yourselves",
                "all",
                "another",
                "any",
                "anything",
                "both",
                "either",
                "few",
                "fewer",
                "ilk",
                "many",
                "mine",
                "more",
                "most",
                "naught",
                "neither",
                "none",
                "nothing",
                "other",
                "otherwise",
                "own",
                "self",
                "several",
                "so",
                "some",
                "somebody",
                "someone",
                "something",
                "somewhat",
                "such",
                "suchlike",
                "sundry",
                "there",
                "twain",
                "various",
                "whatever",
                "whatsoever",
                "when",
                "wherewith",
                "wherewithal",
                "yon",
                "yonder",
                "whether",
            };

            dict["D"] = new HashSet<string> //determiners
            {
                "what",
                "which",
                "whichever",
                "whichsoever",
                "whatever",
                "whatsoever",
                "whose",

                "a",
                "an",
                "the",

                "my",
                "mine", //="my" before vowel-first/h, e.g. mine arms
                "your",
                "his",
                "her",
                "its",
                "our",
                "their",
                "whose",

                "that",
                "this",
                "these",
                "those",
                
                //"them", //them cats - nonstandard / humorous.
                "thine",

                //quantifiers
                "all",
                "every",
                "any",
                "both",
                "each",
                "either",
                "enough",
                "less",
                "no",
                "few",
                "fewer",
                "little",
                "many",
                "more",
                "most",
                "much",
                "neither",
                "none",
                "several",
                "some",
                "sufficient"
            };

            dict["P"] = new HashSet<string>()  //preprositions/postpositions/inpositions
            {
                "as",       //his face was as a mask
                "at",
                "by",
                "against",
                "amid",
                "amidst",
                "among",        //discontent among the poor
                "amongst",
                "because",    //-rather new use: 'because NOUN', e.g: I was late because heavy traffic.
                "beside",
                "circa",
                "despite",
                "during",
                "for",
                "from",
                "into",
                "of",
                "onto",
                "per",
                "since",
                "than",
                "to",
                "toward",
                "towards",
                "until",
                "upon",
                "versus",
                "via",
                "with",
                "without",
                "aboard",
                "about",
                "above",
                "across",
                "after",
                "along",
                "alongside",
                "anti",
                "around",
                "astride",
                "bar",      //rare: city centre is open bar buses and taxis
                "barring",
                "before",
                "below",
                "beneath",
                "besides",
                "between",
                "beyond",
                "but",
                "concerning",
                "considering",
                "down",
                "except",
                "excepting",
                "following",
                "in",
                "including",
                "inside",
                "like",
                "minus",
                "near",
                "notwithstanding",
                "off",
                "on",
                "opposite",
                "outside",
                "over",
                "past",
                "pending",
                "plus",
                "regarding",
                "round",
                "save",
                "through",
                "throughout",
                "till",
                "under",
                "underneath",
                "unlike",
                "up",
                "vis-a-vis",
                "while",
                "within",
                "worth",

                //compounded (complex) prepositions
                // should we treat them idiomatically as a single POS, or
                //let the syntactic rules find a corresponding strucure?
                //(say, ZP -> P PP -> P P NP ?
                //null hypothesis: let the syntax try to converget to a structure.
                //think about compositional semantics for compounded P's? 
                //"because of",
                //"according to",
                //"along with",
                //"apart from",
                //"contrary to",
                //"due to",
                //"except for",
                //"instead of",
                //"prior to",
                //"regardless of",
                //"ahead of",
                //"as for",
                //"as well as",
                //"aside from",
                //"but for",
                //"in between",
                //"inside of",
                //"in spite of",
                //"near to",
                //"next to",
                //"out of",
                //"outside of",
                //"owing to",
                //"subsequent to",
                //"such as",
                //"together with",
                //"up against",
                //"up to",
                //"up until",
            };

            dict["CONJ"] = new HashSet<string>() //conjunctions
            {
                "or",
                "and",
                "because",
                "for",
                "if",
                "nor",
                "since",
                "that",
                "unless", //subordinate can be modified to superficially look like a preposition: 'unless disciplined'
                "until",
                "whereas",
                "after",
                "although" ,//subordinate can be modified to superficially look like a preposition: 'although very angry'
                "before",
                "behind",
                "but",
                "considering",
                "either",
                "except",
                "like",
                "neither",
                "notwithstanding",
                "otherwise", //conjuncting adverb
                "plus",
                "save", //=except, but
                "so",
                "though",
                "till",
                "unlike",
                "when",
                "wherewith",
                "wherewithal",
                "while",
                "yet",
                "how",
                "where",
                "whether",

            };


            dict["N"].Add("without");  //outer place: came from without.
            dict["N"].Add("above");
            dict["N"].Add("bar");
            dict["N"].Add("behind");
            dict["N"].Add("below");
            dict["N"].Add("beyond");
            dict["N"].Add("down");
            dict["N"].Add("following");
            dict["N"].Add("inside");
            dict["N"].Add("like"); // have not seen the like
            dict["N"].Add("many"); // the many
            dict["N"].Add("mine");
            dict["N"].Add("minus");
            dict["N"].Add("more"); //the more
            dict["N"].Add("most"); //the most
            dict["N"].Add("naught");
            dict["N"].Add("nothing");
            dict["N"].Add("opposite");
            dict["N"].Add("other");
            dict["N"].Add("outside");
            dict["N"].Add("past");
            dict["N"].Add("plus");
            dict["N"].Add("round");
            dict["N"].Add("save");
            dict["N"].Add("self");
            dict["N"].Add("somebody");
            dict["N"].Add("there");
            dict["N"].Add("till");
            dict["N"].Add("twain");
            dict["N"].Add("vis-a-vis");
            dict["N"].Add("wherewithal");
            dict["N"].Add("while");
            dict["N"].Add("within");
            dict["N"].Add("worth");
            dict["N"].Add("where");

            dict["V"].Add("would");
            dict["V"].Add("shall");
            dict["V"].Add("should");
            dict["V"].Add("ought");

            dict["V"].Add("bar");
            dict["V"].Add("barring");
            dict["V"].Add("concerning");
            dict["V"].Add("considering");
            dict["V"].Add("down");
            dict["V"].Add("except");
            dict["V"].Add("exclude");
            dict["V"].Add("follow");
            dict["V"].Add("include");
            dict["V"].Add("like");
            dict["V"].Add("mine");
            dict["V"].Add("near");
            dict["V"].Add("off");
            dict["V"].Add("own");
            dict["V"].Add("regard");
            dict["V"].Add("round");
            dict["V"].Add("save");
            dict["V"].Add("till");
            dict["V"].Add("up");
            dict["V"].Add("while");
            dict["V"].Add("worth");


            dict["ADV"].Add("as");   
            dict["ADV"].Add("all");
            dict["ADV"].Add("any");
            dict["ADV"].Add("anything");
            dict["ADV"].Add("but");
            dict["ADV"].Add("each");    //cost a dollar each
            dict["ADV"].Add("either");
            dict["ADV"].Add("more");
            dict["ADV"].Add("most");
            dict["ADV"].Add("neither");
            dict["ADV"].Add("notwithstanding");
            dict["ADV"].Add("otherwise");
            dict["ADV"].Add("so");
            dict["ADV"].Add("some");
            dict["ADV"].Add("something");
            dict["ADV"].Add("somewhat");
            dict["ADV"].Add("such");
            dict["ADV"].Add("there");
            dict["ADV"].Add("though");
            dict["ADV"].Add("whatever");
            dict["ADV"].Add("whatsoever");
            dict["ADV"].Add("when");
            dict["ADV"].Add("wherewith");
            dict["ADV"].Add("within");
            dict["ADV"].Add("yet");
            dict["ADV"].Add("yon");
            dict["ADV"].Add("yonder");
            dict["ADV"].Add("how");
            dict["ADV"].Add("where");
            dict["ADV"].Add("else");

            dict["ADV"].Add("this");
            dict["ADV"].Add("that");

            //spatial or temporal prepositions that can act as
            //adverbs without the complementing noun
            //dict["ADV"].Add("by");
            //dict["ADV"].Add("against");
            //dict["ADV"].Add("since");
            //dict["ADV"].Add("with");
            //dict["ADV"].Add("without");
            //dict["ADV"].Add("aboard");
            //dict["ADV"].Add("about");
            //dict["ADV"].Add("above");
            //dict["ADV"].Add("across");
            //dict["ADV"].Add("after");
            //dict["ADV"].Add("along");
            //dict["ADV"].Add("alongside");
            //dict["ADV"].Add("around");
            //dict["ADV"].Add("astride");
            //dict["ADV"].Add("before");
            //dict["ADV"].Add("behind");
            //dict["ADV"].Add("below");
            //dict["ADV"].Add("beneath");
            //dict["ADV"].Add("besides");
            //dict["ADV"].Add("between");
            //dict["ADV"].Add("beyond");
            //dict["ADV"].Add("down");
            //dict["ADV"].Add("in");
            //dict["ADV"].Add("inside");
            //dict["ADV"].Add("near");
            //dict["ADV"].Add("off");
            //dict["ADV"].Add("on");
            //dict["ADV"].Add("opposite");
            //dict["ADV"].Add("outside");
            //dict["ADV"].Add("over");
            //dict["ADV"].Add("past");
            //dict["ADV"].Add("round");
            //dict["ADV"].Add("through");
            //dict["ADV"].Add("throughout");
            //dict["ADV"].Add("under");
            //dict["ADV"].Add("underneath");
            //dict["ADV"].Add("up");
            //dict["ADV"].Add("vis-a-vis");


            //dict["ADV"].Add("to"); //rare? he brought her to (=woke her up),  we came to (=close to the wind)
            //dict["ADV"].Add("upon"); //obsolete
            //dict["ADJ"].Add("about"); //rare? there is a scarcity of jobs about

            dict["ADJ"].Add("above");
            dict["ADJ"].Add("another"); //another better classified as an adjective than determiner, see https://en.wikipedia.org/wiki/English_determiners
            dict["ADJ"].Add("before");
            dict["ADJ"].Add("behind");
            dict["ADJ"].Add("below");//the below list
            dict["ADJ"].Add("concerning");
            dict["ADJ"].Add("down");
            dict["ADJ"].Add("following");
            dict["ADJ"].Add("in");
            dict["ADJ"].Add("inside");
            dict["ADJ"].Add("like");
            dict["ADJ"].Add("minus");
            dict["ADJ"].Add("near");
            dict["ADJ"].Add("nothing");
            dict["ADJ"].Add("off");
            dict["ADJ"].Add("on");
            dict["ADJ"].Add("opposite");
            dict["ADJ"].Add("other");
            dict["ADJ"].Add("otherwise");
            dict["ADJ"].Add("outside");
            dict["ADJ"].Add("over");
            dict["ADJ"].Add("own");
            dict["ADJ"].Add("past");
            dict["ADJ"].Add("pending");
            dict["ADJ"].Add("plus");
            dict["ADJ"].Add("round");
            dict["ADJ"].Add("self");
            dict["ADJ"].Add("so");
            dict["ADJ"].Add("such");
            dict["ADJ"].Add("suchlike");
            dict["ADJ"].Add("sundry");
            dict["ADJ"].Add("through");
            dict["ADJ"].Add("under");
            dict["ADJ"].Add("unlike");
            dict["ADJ"].Add("up");
            dict["ADJ"].Add("various");
            dict["ADJ"].Add("within");
            dict["ADJ"].Add("worth");
            dict["ADJ"].Add("yon");
            dict["ADJ"].Add("yonder");
            dict["ADJ"].Add("else");


            AddIrregularVerbConjugations(dict);
            AddIrregularPlurals(dict);
            return dict;
        }

        private static void AddIrregularVerbConjugations(Dictionary<string, HashSet<string>> dict)
        {
            //if the verb is identical in past participle and past simple -
            //add it just to the past participle list (enough)
            // http://conjugator.reverso.net/conjugation-irregular-verbs-english.html

            List<string> irregularPastParticipleVerbs = new List<string>();
            List<string> irregularPastSimpleVerbs = new List<string>();

            irregularPastParticipleVerbs.Add("abode");
            irregularPastParticipleVerbs.Add("arisen");
            irregularPastParticipleVerbs.Add("awoken");
            irregularPastParticipleVerbs.Add("been");
            irregularPastParticipleVerbs.Add("borne");
            irregularPastParticipleVerbs.Add("beaten");
            irregularPastParticipleVerbs.Add("become");
            irregularPastParticipleVerbs.Add("begotten");
            irregularPastParticipleVerbs.Add("begun");
            irregularPastParticipleVerbs.Add("bent");
            irregularPastParticipleVerbs.Add("besought");
            irregularPastParticipleVerbs.Add("beseeched");
            irregularPastParticipleVerbs.Add("bet");
            irregularPastParticipleVerbs.Add("bid");
            irregularPastParticipleVerbs.Add("bidden");
            irregularPastParticipleVerbs.Add("bound");
            irregularPastParticipleVerbs.Add("bitten");
            irregularPastParticipleVerbs.Add("bled");
            irregularPastParticipleVerbs.Add("blown");
            irregularPastParticipleVerbs.Add("broken");
            irregularPastParticipleVerbs.Add("bred");
            irregularPastParticipleVerbs.Add("brought");
            irregularPastParticipleVerbs.Add("built");
            irregularPastParticipleVerbs.Add("burnt");
            irregularPastParticipleVerbs.Add("burst");

            irregularPastParticipleVerbs.Add("bought");
            irregularPastParticipleVerbs.Add("cast");
            irregularPastParticipleVerbs.Add("caught");
            irregularPastParticipleVerbs.Add("chidden");
            irregularPastParticipleVerbs.Add("chosen");
            irregularPastParticipleVerbs.Add("cloven");
            irregularPastParticipleVerbs.Add("cleft");
            irregularPastParticipleVerbs.Add("clung");
            irregularPastParticipleVerbs.Add("come");
            irregularPastParticipleVerbs.Add("cost");
            irregularPastParticipleVerbs.Add("crept");
            irregularPastParticipleVerbs.Add("cut");
            irregularPastParticipleVerbs.Add("dealt");
            irregularPastParticipleVerbs.Add("dug");
            irregularPastParticipleVerbs.Add("done");
            irregularPastParticipleVerbs.Add("drawn");
            irregularPastParticipleVerbs.Add("dreamt");
            irregularPastParticipleVerbs.Add("drunk");
            irregularPastParticipleVerbs.Add("driven");
            irregularPastParticipleVerbs.Add("dwelt");
            irregularPastParticipleVerbs.Add("eaten");
            irregularPastParticipleVerbs.Add("fallen");
            irregularPastParticipleVerbs.Add("fed");
            irregularPastParticipleVerbs.Add("felt");
            irregularPastParticipleVerbs.Add("fought");
            irregularPastParticipleVerbs.Add("found");
            irregularPastParticipleVerbs.Add("fled");
            irregularPastParticipleVerbs.Add("flung");
            irregularPastParticipleVerbs.Add("flown");
            irregularPastParticipleVerbs.Add("forbidden");
            irregularPastParticipleVerbs.Add("forgotten");
            irregularPastParticipleVerbs.Add("forsaken");
            irregularPastParticipleVerbs.Add("frozen");
            irregularPastParticipleVerbs.Add("gotten");
            irregularPastParticipleVerbs.Add("got");
            irregularPastParticipleVerbs.Add("gilt");
            irregularPastParticipleVerbs.Add("girt");
            irregularPastParticipleVerbs.Add("given");
            irregularPastParticipleVerbs.Add("gone");
            irregularPastParticipleVerbs.Add("ground");
            irregularPastParticipleVerbs.Add("grown");
            irregularPastParticipleVerbs.Add("hung");
            irregularPastParticipleVerbs.Add("had");
            irregularPastParticipleVerbs.Add("heard");
            irregularPastParticipleVerbs.Add("hewn");
            irregularPastParticipleVerbs.Add("hid");
            irregularPastParticipleVerbs.Add("hidden");
            irregularPastParticipleVerbs.Add("hit");
            irregularPastParticipleVerbs.Add("held");
            irregularPastParticipleVerbs.Add("hurt");
            irregularPastParticipleVerbs.Add("kept");
            irregularPastParticipleVerbs.Add("knelt");
            irregularPastParticipleVerbs.Add("known");
            irregularPastParticipleVerbs.Add("laden");
            irregularPastParticipleVerbs.Add("laid");
            irregularPastParticipleVerbs.Add("led");
            irregularPastParticipleVerbs.Add("leant");
            irregularPastParticipleVerbs.Add("leapt");
            irregularPastParticipleVerbs.Add("learnt");
            irregularPastParticipleVerbs.Add("left");
            irregularPastParticipleVerbs.Add("lent");
            irregularPastParticipleVerbs.Add("let");
            irregularPastParticipleVerbs.Add("lain");
            irregularPastParticipleVerbs.Add("lit");
            irregularPastParticipleVerbs.Add("lost");
            irregularPastParticipleVerbs.Add("made");
            irregularPastParticipleVerbs.Add("meant");
            irregularPastParticipleVerbs.Add("met");
            irregularPastParticipleVerbs.Add("mown");
            irregularPastParticipleVerbs.Add("paid");
            irregularPastParticipleVerbs.Add("put");
            irregularPastParticipleVerbs.Add("quit");
            irregularPastParticipleVerbs.Add("read");
            irregularPastParticipleVerbs.Add("rid");
            irregularPastParticipleVerbs.Add("rent");
            irregularPastParticipleVerbs.Add("ridden");
            irregularPastParticipleVerbs.Add("rung");
            irregularPastParticipleVerbs.Add("risen");
            irregularPastParticipleVerbs.Add("run");
            irregularPastParticipleVerbs.Add("sawn");
            irregularPastParticipleVerbs.Add("said");
            irregularPastParticipleVerbs.Add("seen");
            irregularPastParticipleVerbs.Add("sought");
            irregularPastParticipleVerbs.Add("sold");
            irregularPastParticipleVerbs.Add("sent");
            irregularPastParticipleVerbs.Add("set");
            irregularPastParticipleVerbs.Add("sewn");
            irregularPastParticipleVerbs.Add("shaken");
            irregularPastParticipleVerbs.Add("shaven");
            irregularPastParticipleVerbs.Add("shorn");
            irregularPastParticipleVerbs.Add("shone");
            irregularPastParticipleVerbs.Add("shod");
            irregularPastParticipleVerbs.Add("shed");
            irregularPastParticipleVerbs.Add("shot");
            irregularPastParticipleVerbs.Add("shown");
            irregularPastParticipleVerbs.Add("shrunk");
            irregularPastParticipleVerbs.Add("shut");
            irregularPastParticipleVerbs.Add("sung");
            irregularPastParticipleVerbs.Add("sat");
            irregularPastParticipleVerbs.Add("slain");
            irregularPastParticipleVerbs.Add("slept");
            irregularPastParticipleVerbs.Add("slid");
            irregularPastParticipleVerbs.Add("slung");
            irregularPastParticipleVerbs.Add("slunk");
            irregularPastParticipleVerbs.Add("slit");
            irregularPastParticipleVerbs.Add("smelt");
            irregularPastParticipleVerbs.Add("smitten");
            irregularPastParticipleVerbs.Add("sown");
            irregularPastParticipleVerbs.Add("spoken");
            irregularPastParticipleVerbs.Add("sped");
            irregularPastParticipleVerbs.Add("spelt");
            irregularPastParticipleVerbs.Add("spent");
            irregularPastParticipleVerbs.Add("spilt");
            irregularPastParticipleVerbs.Add("spun");
            irregularPastParticipleVerbs.Add("spat");
            irregularPastParticipleVerbs.Add("split");
            irregularPastParticipleVerbs.Add("spoilt");
            irregularPastParticipleVerbs.Add("spread");
            irregularPastParticipleVerbs.Add("sprung");
            irregularPastParticipleVerbs.Add("stood");
            irregularPastParticipleVerbs.Add("stove");
            irregularPastParticipleVerbs.Add("stolen");
            irregularPastParticipleVerbs.Add("stuck");
            irregularPastParticipleVerbs.Add("stung");
            irregularPastParticipleVerbs.Add("stunk");
            irregularPastParticipleVerbs.Add("strewn");
            irregularPastParticipleVerbs.Add("stridden");
            irregularPastParticipleVerbs.Add("struck");
            irregularPastParticipleVerbs.Add("stricken");
            irregularPastParticipleVerbs.Add("strung");
            irregularPastParticipleVerbs.Add("striven");
            irregularPastParticipleVerbs.Add("sworn");
            irregularPastParticipleVerbs.Add("swept");
            irregularPastParticipleVerbs.Add("swollen");
            irregularPastParticipleVerbs.Add("swum");
            irregularPastParticipleVerbs.Add("swung");
            irregularPastParticipleVerbs.Add("taken");
            irregularPastParticipleVerbs.Add("taught");
            irregularPastParticipleVerbs.Add("torn");
            irregularPastParticipleVerbs.Add("told");
            irregularPastParticipleVerbs.Add("thought");
            irregularPastParticipleVerbs.Add("thriven");
            irregularPastParticipleVerbs.Add("thrown");
            irregularPastParticipleVerbs.Add("thrust");
            irregularPastParticipleVerbs.Add("trodden");
            irregularPastParticipleVerbs.Add("woken");
            irregularPastParticipleVerbs.Add("worn");
            irregularPastParticipleVerbs.Add("woven");
            irregularPastParticipleVerbs.Add("wept");
            irregularPastParticipleVerbs.Add("won");
            irregularPastParticipleVerbs.Add("wound");
            irregularPastParticipleVerbs.Add("wrung");
            irregularPastParticipleVerbs.Add("written");
            irregularPastParticipleVerbs.Add("withdrawn");
            irregularPastParticipleVerbs.Add("undone");
            irregularPastParticipleVerbs.Add("undergone");
            irregularPastParticipleVerbs.Add("forgiven");
            irregularPastParticipleVerbs.Add("sunk");
            irregularPastParticipleVerbs.Add("beheld");


            irregularPastSimpleVerbs.Add("arose");
            irregularPastSimpleVerbs.Add("awoke");
            irregularPastSimpleVerbs.Add("was");
            irregularPastSimpleVerbs.Add("were");
            irregularPastSimpleVerbs.Add("bear");
            irregularPastSimpleVerbs.Add("beat");
            irregularPastSimpleVerbs.Add("became");
            irregularPastSimpleVerbs.Add("begat");
            irregularPastSimpleVerbs.Add("begot");
            irregularPastSimpleVerbs.Add("began");
            irregularPastSimpleVerbs.Add("bade");
            irregularPastSimpleVerbs.Add("bit");
            irregularPastSimpleVerbs.Add("blew");
            irregularPastSimpleVerbs.Add("broke");
            irregularPastSimpleVerbs.Add("could");
            irregularPastSimpleVerbs.Add("chid");
            irregularPastSimpleVerbs.Add("chose");
            irregularPastSimpleVerbs.Add("clove");
            irregularPastSimpleVerbs.Add("came");
            irregularPastSimpleVerbs.Add("dove");
            irregularPastSimpleVerbs.Add("did");
            irregularPastSimpleVerbs.Add("drew");
            irregularPastSimpleVerbs.Add("drank");
            irregularPastSimpleVerbs.Add("drove");
            irregularPastSimpleVerbs.Add("ate");
            irregularPastSimpleVerbs.Add("fell");
            irregularPastSimpleVerbs.Add("flew");
            irregularPastSimpleVerbs.Add("forbade");
            irregularPastSimpleVerbs.Add("forgot");
            irregularPastSimpleVerbs.Add("forsook");
            irregularPastSimpleVerbs.Add("froze");
            irregularPastSimpleVerbs.Add("gave");
            irregularPastSimpleVerbs.Add("went");
            irregularPastSimpleVerbs.Add("grew");
            irregularPastSimpleVerbs.Add("knew");
            irregularPastSimpleVerbs.Add("lay");
            irregularPastSimpleVerbs.Add("might");
            irregularPastSimpleVerbs.Add("rode");
            irregularPastSimpleVerbs.Add("rang");
            irregularPastSimpleVerbs.Add("rose");
            irregularPastSimpleVerbs.Add("ran");
            irregularPastSimpleVerbs.Add("saw");
            irregularPastSimpleVerbs.Add("shook");
            irregularPastSimpleVerbs.Add("shrank");
            irregularPastSimpleVerbs.Add("sang");
            irregularPastSimpleVerbs.Add("slew");
            irregularPastSimpleVerbs.Add("smote");
            irregularPastSimpleVerbs.Add("spoke");
            irregularPastSimpleVerbs.Add("span");
            irregularPastSimpleVerbs.Add("sprang");
            irregularPastSimpleVerbs.Add("stole");
            irregularPastSimpleVerbs.Add("stank");
            irregularPastSimpleVerbs.Add("strode");
            irregularPastSimpleVerbs.Add("strove");
            irregularPastSimpleVerbs.Add("swore");
            irregularPastSimpleVerbs.Add("swam");
            irregularPastSimpleVerbs.Add("took");
            irregularPastSimpleVerbs.Add("tore");
            irregularPastSimpleVerbs.Add("throve");
            irregularPastSimpleVerbs.Add("threw");
            irregularPastSimpleVerbs.Add("woke");
            irregularPastSimpleVerbs.Add("wore");
            irregularPastSimpleVerbs.Add("wove");
            irregularPastSimpleVerbs.Add("wrote");
            irregularPastSimpleVerbs.Add("withdrew");
            irregularPastSimpleVerbs.Add("undid");
            irregularPastSimpleVerbs.Add("underwent");
            irregularPastSimpleVerbs.Add("forgave");
            irregularPastSimpleVerbs.Add("sank");

            foreach (var verb in irregularPastSimpleVerbs)
                dict["V"].Add(verb);

            foreach (var verb in irregularPastParticipleVerbs)
            {
                dict["V"].Add(verb);
                dict["ADJ"].Add(verb);
            }
        }

        private static void AddIrregularPlurals(Dictionary<string, HashSet<string>> dict)
        {
            dict["N"].Add("addenda");
            dict["N"].Add("alumnae");
            dict["N"].Add("alumni");
            dict["N"].Add("analyses");
            dict["N"].Add("antennae");
            dict["N"].Add("antennas");
            dict["N"].Add("antitheses");
            dict["N"].Add("apexes");
            dict["N"].Add("apices");
            dict["N"].Add("appendices");
            dict["N"].Add("appendixes");
            dict["N"].Add("axes");
            dict["N"].Add("bacilli");
            dict["N"].Add("bacteria");
            dict["N"].Add("bases");
            dict["N"].Add("cacti");
            dict["N"].Add("children");
            dict["N"].Add("concerti");
            dict["N"].Add("corpora");
            dict["N"].Add("codices");
            dict["N"].Add("crises");
            dict["N"].Add("criteria");
            dict["N"].Add("curricula");
            dict["N"].Add("data");
            dict["N"].Add("diagnoses");
            dict["N"].Add("dice");
            dict["N"].Add("dwarves");
            dict["N"].Add("ellipses");
            dict["N"].Add("errata");
            dict["N"].Add("fezzes");
            dict["N"].Add("foci");
            dict["N"].Add("feet");
            dict["N"].Add("formulae");
            dict["N"].Add("fungi");
            dict["N"].Add("genera");
            dict["N"].Add("geese");
            dict["N"].Add("graffiti");
            dict["N"].Add("halves");
            dict["N"].Add("hooves");
            dict["N"].Add("hypotheses");
            dict["N"].Add("indices");
            dict["N"].Add("larvae");
            dict["N"].Add("libretti");
            dict["N"].Add("loaves");
            dict["N"].Add("loci");
            dict["N"].Add("lice");
            dict["N"].Add("men");
            dict["N"].Add("matrices");
            dict["N"].Add("media");
            dict["N"].Add("memoranda");
            dict["N"].Add("minutiae");
            dict["N"].Add("mice");
            dict["N"].Add("nebulae");
            dict["N"].Add("nuclei");
            dict["N"].Add("oases");
            dict["N"].Add("opera");
            dict["N"].Add("ova");
            dict["N"].Add("oxen");
            dict["N"].Add("parentheses");
            dict["N"].Add("phenomena");
            dict["N"].Add("phyla");
            dict["N"].Add("prognoses");
            dict["N"].Add("quizzes");
            dict["N"].Add("radii");
            dict["N"].Add("referenda");
            dict["N"].Add("scarves");
            dict["N"].Add("selves");
            dict["N"].Add("stimuli");
            dict["N"].Add("strata");
            dict["N"].Add("syllabi");
            dict["N"].Add("symposia");
            dict["N"].Add("synopses");
            dict["N"].Add("theses");
            dict["N"].Add("thieves");
            dict["N"].Add("teeth");
            dict["N"].Add("vertebrae");
            dict["N"].Add("vertices");
            dict["N"].Add("vitae");
            dict["N"].Add("vortices");
            dict["N"].Add("wharves");
            dict["N"].Add("wives");
            dict["N"].Add("wolves");
            dict["N"].Add("women");
        }

        public static List<string> GetPossiblePOS(string word, Dictionary<string, HashSet<string>> dict)
        {
            var l = new List<string>();

            foreach (var pos in dict.Keys)
            {
                if (dict[pos].Contains(word))
                    l.Add(pos);
            }

            return l;
        }
        static void Main()
        {

            var dict = CreateDictionary();

            //1. decision: remove all ambiguous P/ADV in WordNet, leaving them assigned P category.
            //motivation: I think prepositional phrases can function as adverbial phrases, regardless of complement.
            //sometimes, P does not take a complement (it can be salient, or some deictic center/time, etc)
            //hence, ADJP/ADVP -> P NP, or ADJP/ADVP -> P , the adverbial function or the adjectival function is identical.
            //for instance:  I saw the girl [outside]
            //               I saw the girl [outside my house]

            //note: there is ambiguity between adjectival phrase and adverbial phrase:
            //outside (my house) = where the girl is (adjectival)
            //outside (my house) = where the seeing took place (adverbial), the girl could be elsewhere.
            //the ambiguity is present regardless of the complement "my house".
            dict["ADV"].ExceptWith(dict["P"]);

            //remove adjectives from adjectives list that are regarded as determiners
            //eg. "no"
            dict["ADJ"].ExceptWith(dict["D"]);

            Vocabulary v = new Vocabulary();
            v.POSWithPossibleWords = dict;
            File.WriteAllText(@"UniversalVocabulary.json", JsonConvert.SerializeObject(v, Formatting.Indented));

        }

        public static HashSet<string> GetWords(string category, string filename)
        {
            var l = new HashSet<string>();
            string line;

            var file = new StreamReader(filename);
            while ((line = file.ReadLine()) != null)
            {
                if (line[0] == ' ') continue; 
                //copyright notice of the file opens with blank spaces

                var s = line.Split()[0];
                //get the first word, rest of items are WordNET internal data.

                if (s.Contains('_'))
                    s = string.Join(' ', s.Split('_'));
                //word could be separated with underscore, and then the value
                //is multiple words, corresponding to a single entry in the lexicon
                //for instance a_la_mode = a la mode

                l.Add(s);

            }
            file.Close();

            return l;
        }
    }
}
