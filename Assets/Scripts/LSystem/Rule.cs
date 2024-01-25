using System.Xml;
using UnityEngine;

[System.Serializable]
public class Rule {
    public string name = "";
    public string prefix = "";
    public string suffix = "";
    public string ruleString = "";
    public string ruleReplacement;
    public float probability = 1f;
    
    public Rule(string ruleString, string ruleReplacement) {
        this.ruleString = ruleString;
        this.ruleReplacement = ruleReplacement;
    }
    
    public Rule(string prefix, string ruleString, string suffix, string ruleReplacement) {
        this.prefix = prefix;
        this.suffix = suffix;
        this.ruleString = ruleString;
        this.ruleReplacement = ruleReplacement;
    }
    
    
    public void ExpandStrings() {
        ruleReplacement = ExpandString(ruleReplacement);
        prefix = ExpandString(prefix);
        suffix = ExpandString(suffix);
    }
        
    private string ExpandString(string ruleReplacement) {
        if (ruleReplacement == null || ruleReplacement.Length == 0) {
            return "";
        }

        string expandedRuleReplacement = "";
        for (int i = 0; i < ruleReplacement.Length; i++) {
            if (ruleReplacement[i] == '{') {
                char c = ruleReplacement[i - 1];
                string numberString = "";
                i++;
                while (ruleReplacement[i] != '}') {
                    if (!IsDigit(ruleReplacement[i])) {
                        throw new System.FormatException("Multiplicator braces can only contain digits: " + ruleReplacement);
                    }

                    numberString += ruleReplacement[i];
                    i++;
                }
                
                int repeatNum;
                // Error handling
                try {
                    repeatNum = int.Parse(numberString);
                } catch (System.FormatException) {
                    throw new System.FormatException("Invalid number in rule replacement: " + ruleReplacement);
                }
                if (repeatNum <= 0) {
                    throw new System.FormatException("Multiplication number must be non-positive integer: " + ruleReplacement);
                }

                expandedRuleReplacement += RepeatCharachter(c, repeatNum - 1);
            } else {
                expandedRuleReplacement += ruleReplacement[i];
            }
        };
        
        return expandedRuleReplacement;
    }

    private string RepeatCharachter(char c, int n) {
        string s = "";

        for (int i = 0; i < n; i++) {
            s += c;
        }

        return s;
    }
    
    public int GetPriority() {
        return prefix.Length + suffix.Length;
    }
    
    private bool IsDigit(char c) {
        return c >= '0' && c <= '9';
    }
}
