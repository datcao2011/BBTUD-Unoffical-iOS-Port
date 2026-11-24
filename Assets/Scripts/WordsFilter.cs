using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000002 RID: 2
public class WordsFilter
{
    public WordsFilter()
    {
        FilteredWords = ((TextAsset)Resources.Load("FilteredWords")).text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList();
    }

    // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
    public string Filter(string input)
    {
        string result = "";
        string[] words = input.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string word in words)
        {
            string _ = word;
            foreach (string text in FilteredWords)
            {
                string censored = "";
                for (int j = 0; j < _.Length; j++)
                {
                    censored += "*";
                }
                if (_.ToLower().Contains(text))
                {
                    _ = censored;
                }
            }
            result += _ + " ";
        }
        return result.TrimEnd();
    }

    // Token: 0x04000001 RID: 1
    public List<string> FilteredWords = new List<string>();
}