using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Accenture.eviola
{
    public class Text 
    {
        /// <summary>
        /// returns the indices of all occurences of theSubstring in theString
        /// </summary>
        static public List<int> FindAllOccurencesOfSubstringInString(string theString, string theSubString) {
            List<int> foundIdx = new List<int>();
            for (int i = 0; i < theString.Length; i += theSubString.Length) {
                i = theString.IndexOf(theSubString, i);
                if (i == -1) { 
                    return foundIdx;
                }
                foundIdx.Add(i);
            }
            return foundIdx;
        }

        static public string ApplyTokensToTemplate(string template, Dictionary<string, string> tokens, string tokenStart="[[", string tokenStop="]]") {
            string res = template;
            foreach (KeyValuePair<string, string> tokenValuePair in tokens) { 
                string tbf = tokenStart + tokenValuePair.Key + tokenStop;
                res = res.Replace(tbf, tokenValuePair.Value);
            }
            return res;
        }
    }
}