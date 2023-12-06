using UnityEngine;

[System.Serializable]
public class Rule {
    public string name = "";
    public string prefix = "";
    public string suffix = "";
    public string ruleCharachter = "";
    public string ruleReplacement;
    public float probability = 1f;
    
    public Rule(string ruleCharachter, string ruleReplacement) {
        this.ruleCharachter = ruleCharachter;
        this.ruleReplacement = ruleReplacement;
    }
    
    public Rule(string prefix, string ruleCharachter, string suffix, string ruleReplacement) {
        this.prefix = prefix;
        this.suffix = suffix;
        this.ruleCharachter = ruleCharachter;
        this.ruleReplacement = ruleReplacement;
    }

    public int GetPriority() {
        return prefix.Length + suffix.Length;
    }
}
