using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LSystem {
    [SerializeField]
    public string axiom;
    [SerializeField]
    public int iterations;
    [SerializeField]
    public Rule[] rules;
    private Dictionary<char, List<Rule>> rulesMap;
    
    public LSystem(string axiom, int iterations, Rule[] rules) {
        this.axiom = axiom;
        this.iterations = iterations;
        this.rules = rules;
    }

    public string Generate() {
        rulesMap ??= InitRulesMap();

        string result = axiom;

        for (int i = 0; i < iterations; i++) {
            result = GenerateIteration(result);
        }

        return result;
    }

    private string GenerateIteration(string input) {
        string result = "";

        for (int i = 0; i < input.Length; i++) {
            Rule rule = GetHighestPriorityRule(input, i);
            List<Rule> rules = GetAllRulesOfTheSameKind(rule, input, i);
            Rule randomRule = GetRandomRule(rules);
            if (randomRule != null) {
                result += randomRule.ruleReplacement;
            } else {
                result += input[i];
            }
        }

        return result;
    }

    private Rule GetHighestPriorityRule(string input, int index) {
        List<Rule> rules = rulesMap.GetValueOrDefault(input[index]);

        if (rules == null) {
            return null;
        }

        rules.Sort((rule1, rule2) => rule2.GetPriority().CompareTo(rule1.GetPriority()));

        foreach (Rule rule in rules) {
            if (rule.prefix.Length > 0 && rule.suffix.Length > 0) {
                if (IsPrefixedBy(input, index, rule.prefix) && IsSuffixedBy(input, index, rule.suffix)) {
                    return rule;
                }
            } else if (rule.prefix.Length > 0) {
                if (IsPrefixedBy(input, index, rule.prefix)) {
                    return rule;
                }
            } else if (rule.suffix.Length > 0) {
                if (IsSuffixedBy(input, index, rule.suffix)) {
                    return rule;
                }
            } else {
                return rule;
            }
        }

        return null;
    }
    
    private List<Rule> GetAllRulesOfTheSameKind(Rule rule, string input, int index) {
        List<Rule> rules = new();

        if (rule != null) {
            foreach (Rule r in rulesMap.GetValueOrDefault(input[index])) {
                if (r.ruleCharachter == rule.ruleCharachter && r.prefix == rule.prefix && r.suffix == rule.suffix) {
                    rules.Add(r);
                }
            }
        }

        return rules;
    }
    
    private Rule GetRandomRule(List<Rule> rules) {
        float totalProbability = 0f;

        foreach (Rule rule in rules) {
            totalProbability += rule.probability;
        }

        float randomValue = Random.Range(0f, totalProbability);

        foreach (Rule rule in rules) {
            if (randomValue <= rule.probability) {
                return rule;
            } else {
                randomValue -= rule.probability;
            }
        }

        return null;
    }

    public static bool IsPrefixedBy(string input, int index, string prefix) {
        if (index == 0) {
            return prefix == "";
        }

        int currentIndex = index - 1;
        int prefixIndex = prefix.Length - 1;

        while (currentIndex >= 0) {
            if (input[currentIndex] == prefix[prefixIndex]) {
                if (prefixIndex == 0) {
                    return true;
                } else {
                    currentIndex--;
                    prefixIndex--;
                }
            } else if (input[currentIndex] == ']') {
                currentIndex = SkipBracketBackward(input, currentIndex);
            } else if (input[currentIndex] == '[') {
                currentIndex--;
            } else {
                break;
            }
        }
        return false;
    }
    
    public static bool IsSuffixedBy(string input, int index, string suffix) {
        if (index == input.Length - 1) {
            return suffix == "";
        }

        int currentIndex = index + 1;
        int suffixIndex = 0;
        
        Stack<(int, int)> bracketStack = new();

        // Suffix logic is different from prefix logic, since one branch segment can branch into multiple sub-segments
        while (currentIndex < input.Length) {
            if (input[currentIndex] == suffix[suffixIndex]) {
                if (suffixIndex == suffix.Length - 1) {
                    return true;
                } else {
                    currentIndex++;
                    suffixIndex++;
                }
            } else if (input[currentIndex] == '[') {
                bracketStack.Push((currentIndex, suffixIndex));
                currentIndex++;
            } else if (input[currentIndex] == ']') {
                if (bracketStack.Count == 0) {
                    break;
                }
                (currentIndex, suffixIndex) = bracketStack.Pop();
                currentIndex = SkipBracketForward(input, currentIndex);
            } else {
                if (bracketStack.Count == 0) {
                    break;
                }
                (currentIndex, suffixIndex) = bracketStack.Pop();
                currentIndex = SkipBracketForward(input, currentIndex);
            }
        }
        return false;
    }

    public static int SkipBracketBackward(string input, int index) {
        // Nothing to skip, return the same position
        if (index == 0 || input[index] != ']') {
            return index;
        }

        int currentIndex = index;
        int bracketCount = 0;

        while (currentIndex >= 0) {
            if (input[currentIndex] == ']') {
                bracketCount++;
            } else if (input[currentIndex] == '[' && bracketCount > 0) {
                bracketCount--;
            }

            currentIndex--;

            if (bracketCount == 0) {
                break;
            }
        }
        return currentIndex;
    }

    public static int SkipBracketForward(string input, int index) {
        // Nothing to skip, return the same position
        if (index == input.Length - 1 || input[index] != '[') {
            return index;
        }

        int currentIndex = index;
        int bracketCount = 0;

        while (currentIndex < input.Length) {
            if (input[currentIndex] == '[') {
                bracketCount++;
            } else if (input[currentIndex] == ']' && bracketCount > 0) {
                bracketCount--;
            }

            currentIndex++;

            if (bracketCount == 0) {
                break;
            }
        }

        return currentIndex;
    }

    private Dictionary<char, List<Rule>> InitRulesMap() {
        Dictionary<char, List<Rule>> rulesMap = new();

        foreach (Rule rule in rules) {
            if (rulesMap.ContainsKey(rule.ruleCharachter[0])) {
                rulesMap[rule.ruleCharachter[0]].Add(rule);
            } else {
                rulesMap[rule.ruleCharachter[0]] = new List<Rule> { rule };
            }
        }

        return rulesMap;
    }
}
