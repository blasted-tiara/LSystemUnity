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
    
    public int GetPriority() {
        return prefix.Length + suffix.Length;
    }
}
