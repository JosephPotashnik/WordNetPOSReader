
using System.Collections.Generic;
using System.IO;
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
                "because of",  //complex prepositions (prepositions of more than one word)
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



            dict["ADV"].Add("as");  //as soft as silk
            dict["ADV"].Add("by"); // without object 'by' can act as adverb
            dict["ADV"].Add("against");  //without object 'against' can act as adverb
            dict["ADV"].Add("since");  //without object 'since' can act as adverb
            dict["ADV"].Add("with");  //without object 'with' can act as adverb
            dict["ADV"].Add("without"); /// as above...
            dict["ADV"].Add("aboard");  
            dict["ADV"].Add("about");  
            dict["ADV"].Add("above"); 
            dict["ADV"].Add("across");
            dict["ADV"].Add("after");
            dict["ADV"].Add("all");
            dict["ADV"].Add("along");
            dict["ADV"].Add("alongside");
            dict["ADV"].Add("any");
            dict["ADV"].Add("anything");
            dict["ADV"].Add("around");
            dict["ADV"].Add("astride");
            dict["ADV"].Add("before");
            dict["ADV"].Add("behind");
            dict["ADV"].Add("below");
            dict["ADV"].Add("beneath");
            dict["ADV"].Add("besides");
            dict["ADV"].Add("between");
            dict["ADV"].Add("beyond");
            dict["ADV"].Add("but");
            dict["ADV"].Add("down");
            dict["ADV"].Add("each");    //cost a dollar each
            dict["ADV"].Add("either");
            dict["ADV"].Add("in");
            dict["ADV"].Add("inside");
            dict["ADV"].Add("more");
            dict["ADV"].Add("most");
            dict["ADV"].Add("near");
            dict["ADV"].Add("neither");
            dict["ADV"].Add("notwithstanding");
            dict["ADV"].Add("off");
            dict["ADV"].Add("on");
            dict["ADV"].Add("opposite");
            dict["ADV"].Add("otherwise");
            dict["ADV"].Add("outside");
            dict["ADV"].Add("over");
            dict["ADV"].Add("past");
            dict["ADV"].Add("round");
            dict["ADV"].Add("so");
            dict["ADV"].Add("some");
            dict["ADV"].Add("something");
            dict["ADV"].Add("somewhat");
            dict["ADV"].Add("such");
            dict["ADV"].Add("there");
            dict["ADV"].Add("though");
            dict["ADV"].Add("through");
            dict["ADV"].Add("throughout");
            dict["ADV"].Add("under");
            dict["ADV"].Add("underneath");
            dict["ADV"].Add("up");
            dict["ADV"].Add("vis-a-vis");
            dict["ADV"].Add("whatever");
            dict["ADV"].Add("whatsoever");
            dict["ADV"].Add("when");
            dict["ADV"].Add("wherewith");
            dict["ADV"].Add("within");
            dict["ADV"].Add("yet");
            dict["ADV"].Add("yon");
            dict["ADV"].Add("yonder");

            dict["ADV"].Add("this");
            dict["ADV"].Add("that");
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

            return dict;
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

            //https://www.ucl.ac.uk/internet-grammar/preps/complex.htm
            var l = GetPossiblePOS("much", dict);
            var l2 = GetPossiblePOS("because of", dict);
            var l3 = GetPossiblePOS("in spite of", dict);
            var l4 = GetPossiblePOS("according", dict);
            var l5 = GetPossiblePOS("according to", dict);
            var l6 = GetPossiblePOS("instead of", dict);
            var l7 = GetPossiblePOS("in lieu of", dict);

            File.WriteAllText(@"UniversalVocabulary.json", JsonConvert.SerializeObject(dict, Formatting.Indented));

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
