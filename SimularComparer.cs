using FFmpeg.AutoGen;
using System;
using System.Collections.Generic;
using System.Text;

public class SimularComparer
{
    public bool CaseSensitive { get; set; } = false;
    public bool IgnoreNonAlphaNumeric { get; set; } = true;
    public bool IgnoreUmlauts { get; set; } = true;

    public bool UseStartWith { get; set; } = true;
    public bool UseEndWith { get; set; } = true;
    public bool UseContains { get; set; } = true;
    public bool ContainsReverse { get; set; } = true;

    public bool AllowFuzzy { get; set; } = true;
    public int MaxEditDistance { get; set; } = 0; // 0 = relative Toleranz
    public double RelativeTolerance { get; set; } = 0.25;
    public int MinLengthForRelativeTolerance { get; set; } = 4;

    public SimularComparer(bool caseSensitive = false, bool ignoreNonAlphaNumeric= true, bool ignoreUmlauts=true, bool useStartWith=true, bool useEndWith=true, bool useContains=true, bool containsReverse=true, bool allowFuzzy=true, int maxEditDistance=0, double relativeTolerance=0.25, int minLengthForRelativeTolerance=4)
    {
        CaseSensitive = caseSensitive;
        IgnoreNonAlphaNumeric = ignoreNonAlphaNumeric;
        IgnoreUmlauts = ignoreUmlauts;
        UseStartWith = useStartWith;
        UseEndWith = useEndWith;
        UseContains = useContains;
        ContainsReverse = containsReverse;
        AllowFuzzy = allowFuzzy;
        MaxEditDistance = maxEditDistance;
        RelativeTolerance = relativeTolerance;
        MinLengthForRelativeTolerance = minLengthForRelativeTolerance;
    }

    static readonly Dictionary<char, string> UmlautToDigraph = new()
    {
        { 'ä', "ae" }, { 'Ä', "Ae" },
        { 'ö', "oe" }, { 'Ö', "Oe" },
        { 'ü', "ue" }, { 'Ü', "Ue" },
        { 'ß', "ss" }, { 'ẞ', "SS" }
    };

    public bool Matches(string a, string b)
    {
        bool result = false;
        if (a is null || b is null)
        {
            a = "";
            b = "";
        }

        string ca = Canonical(a);
        string cb = Canonical(b);

        if (!CaseSensitive)
        {
            ca = ca.ToLowerInvariant();
            cb = cb.ToLowerInvariant();
        }
        if (ca == cb) return true;
        if (UseStartWith)
        {
            result |= ca.StartsWith(cb) || (AllowFuzzy && FuzzyPrefix(ca, cb));
            if (ContainsReverse && !result)
                result |= cb.StartsWith(ca) || (AllowFuzzy && FuzzyPrefix(cb, ca));
        }
        if (UseEndWith && !result)
        {
            result |= ca.StartsWith(cb) || (AllowFuzzy && FuzzyPrefix(ca, cb));
            if (ContainsReverse && !result)
                result |= cb.EndsWith(ca) || (AllowFuzzy && FuzzySuffix(cb, ca));
        }
        if (UseContains && !result)
        {
            result |= ca.Contains(cb) || (AllowFuzzy && FuzzyContains(ca, cb));
            if (ContainsReverse && !result)
                result |= cb.Contains(ca) || (AllowFuzzy && FuzzyContains(cb, ca));
        }
        if (result)
            return result;
        if (AllowFuzzy)
            return IsWithinTolerance(ca, cb);
        return false;
    }

    public string Canonical(string s)
    {
        String result = s;
        if (IgnoreUmlauts)
        {
            foreach(char c in UmlautToDigraph.Keys)
                result = result.Replace(c.ToString(), UmlautToDigraph[c]);
        }
        if (IgnoreNonAlphaNumeric)
        {
            foreach (char c in s)
                if (!char.IsLetterOrDigit(c))
                    result = result.Replace(c.ToString(), "");
        }
        return result;
    }

    bool IsWithinTolerance(string a, string b)
    {
        int dist = DamerauLevenshteinDistance(a, b);
        if (MaxEditDistance > 0)
            return dist <= MaxEditDistance;

        int len = Math.Max(a.Length, b.Length);
        if (len < MinLengthForRelativeTolerance) return dist <= 1;
        int allowed = Math.Max(1, (int)Math.Floor(len * RelativeTolerance));
        return dist <= allowed;
    }

    bool FuzzyPrefix(string a, string b)
    {
        int take = Math.Min(a.Length, b.Length + 1);
        return IsWithinTolerance(a[..take], b);
    }

    bool FuzzySuffix(string a, string b)
    {
        int take = Math.Min(a.Length, b.Length + 1);
        return IsWithinTolerance(a[^take..], b);
    }

    bool FuzzyContains(string a, string b)
    {
        int win = Math.Min(a.Length, b.Length + 1);
        for (int i = 0; i + win <= a.Length; i++)
        {
            if (IsWithinTolerance(a.Substring(i, win), b))
                return true;
        }
        return false;
    }

    int DamerauLevenshteinDistance(string s, string t)
    {
        if (s == t) return 0;
        if (s.Length == 0) return t.Length;
        if (t.Length == 0) return s.Length;

        int n = s.Length, m = t.Length;
        var prev = new int[m + 1];
        var curr = new int[m + 1];
        for (int j = 0; j <= m; j++) prev[j] = j;

        for (int i = 1; i <= n; i++)
        {
            curr[0] = i;
            for (int j = 1; j <= m; j++)
            {
                int cost = s[i - 1] == t[j - 1] ? 0 : 1;
                curr[j] = Math.Min(Math.Min(curr[j - 1] + 1, prev[j] + 1), prev[j - 1] + cost);
                if (i > 1 && j > 1 && s[i - 1] == t[j - 2] && s[i - 2] == t[j - 1])
                    curr[j] = Math.Min(curr[j], prev[j - 2] + 1);
            }
            (prev, curr) = (curr, prev);
        }
        return prev[m];
    }
}

/* Beispiel:
var cmp = new SimularComparer();
var settings = new SimularComparer.Settings
{
    CaseSensitive = false,
    AllowFuzzy = true,
    RelativeTolerance = 0.25
};

bool m1 = cmp.Matches("Äpfel", "Aepfel", settings);     // true
bool m2 = cmp.Matches("Kartoffeln", "Katoffeln", settings); // true
*/
