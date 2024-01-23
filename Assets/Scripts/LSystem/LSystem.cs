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
    private Dictionary<string, List<Rule>> rulesMap;
    
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
    
    /*
    private string GetRuleName(string input, ref int index) {
        if (IsLetter(input[index])) {
            string result = input[index++].ToString();
            while (index < input.Length && Is(input[index])) {
                result += input[index];
                index++;
            }
        } else {
            return input[index].ToString();
        }

    }
    */
    
    private bool IsLetter(char c) {
        return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
    }

    private bool IsDigit(char c) {
        return c >= '0' && c <= '9';
    }

    private string GenerateIteration(string input) {
        string result = "";

        int i = 0;
        while (i < input.Length) {
            string currentRuleString = GetCurrentRuleString(input, i);
            Rule rule = GetHighestPriorityRule(input, i, currentRuleString);
            List<Rule> rules = GetAllRulesOfTheSameKind(rule, currentRuleString);
            Rule randomRule = GetRandomRule(rules);
            if (randomRule != null) {
                result += randomRule.ruleReplacement;
            } else {
                result += input[i];
            }
            
            i += currentRuleString.Length;
        }

        return result;
    
    }
    
    private string GetCurrentRuleString(string input, int index) {
        string result = "";
        
        if (IsLetter(input[index])) {
            result += input[index++].ToString();
            
            while (index < input.Length && IsDigit(input[index])) {
                result += input[index++].ToString();
            }

        } else {
            return input[index].ToString();
        }
        
        return result;
    }
    
    private Rule GetHighestPriorityRule(string input, int index, string ruleString) {
        List<Rule> rules = rulesMap.GetValueOrDefault(ruleString);

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
    
    private List<Rule> GetAllRulesOfTheSameKind(Rule rule, string ruleString) {
        List<Rule> rules = new();

        if (rule != null) {
            foreach (Rule r in rulesMap.GetValueOrDefault(ruleString)) {
                if (r.ruleString == rule.ruleString && r.prefix == rule.prefix && r.suffix == rule.suffix) {
                    rules.Add(r);
                }
            }
        }

        return rules;
    }
    
    private Rule GetRandomRule(List<Rule> rules) {
        if (rules.Count == 0) {
            return null;
        } else if (rules.Count == 1) {
            return rules[0];
        } else {
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

    private Dictionary<string, List<Rule>> InitRulesMap() {
        Dictionary<string, List<Rule>> rulesMap = new();

        foreach (Rule rule in rules) {
            if (rulesMap.ContainsKey(rule.ruleString)) {
                rulesMap[rule.ruleString].Add(rule);
            } else {
                rulesMap[rule.ruleString] = new List<Rule> { rule };
            }
        }

        return rulesMap;
    }
}
