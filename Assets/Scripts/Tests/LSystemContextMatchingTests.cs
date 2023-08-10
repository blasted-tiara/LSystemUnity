using System.Collections.Generic;
using NUnit.Framework;

public class LSystemContextMatchingTests {
    // Test backwards bracket skipping
    [Test]
    public void TestNoBracketBackward() {
        string str = "ABCDEFG";
        int index = 3;
        Assert.AreEqual(3, LSystem.SkipBracketBackward(str, index));
    }

    [Test]
    public void TestSimpleBracketBackward() {
        string str = "AB[CD]EFG";
        int index = 5;
        Assert.AreEqual(1, LSystem.SkipBracketBackward(str, index));
    }

    [Test]
    public void TestNestedBracketBackward() {
        string str = "AB[CD[EF]GH]IJK";
        int index = 11;
        Assert.AreEqual(1, LSystem.SkipBracketBackward(str, index));
    }

    [Test]
    public void TestMultipleNestedBracketBackward() {
        string str = "A[B[C[D[E]F]G]H]I";
        int index = 15;
        Assert.AreEqual(0, LSystem.SkipBracketBackward(str, index));
    }

    [Test]
    public void TestUnmatchedBracketBackward() {
        string str = "AB[CD[EF]GH]IJK";
        int index = 2;
        Assert.AreEqual(2, LSystem.SkipBracketBackward(str, index));
    }

    [Test]
    public void TestZeroIndex() {
        string str = "A[BC]D";
        int index = 0;
        Assert.AreEqual(0, LSystem.SkipBracketBackward(str, index));
    }

    // Test forward bracket skipping
    [Test]
    public void TestNoBracketForward() {
        string str = "ABCDEFG";
        int index = 3;
        Assert.AreEqual(3, LSystem.SkipBracketForward(str, index));
    }

    [Test]
    public void TestSimpleBracketForward() {
        string str = "AB[CD]EFG";
        int index = 2;
        Assert.AreEqual(6, LSystem.SkipBracketForward(str, index));
    }

    [Test]
    public void TestNestedBracketForward() {
        string str = "AB[CD[EF]GH]IJK";
        int index = 2;
        Assert.AreEqual(12, LSystem.SkipBracketForward(str, index));
    }

    [Test]
    public void TestMultipleNestedBracketForward() {
        string str = "A[B[C[D[E]F]G]H]I";
        int index = 1;
        Assert.AreEqual(16, LSystem.SkipBracketForward(str, index));
    }

    [Test]
    public void TestUnmatchedBracketForward() {
        string str = "AB[CD[EF]GH]IJK";
        int index = 11;
        Assert.AreEqual(11, LSystem.SkipBracketForward(str, index));
    }

    [Test]
    public void TestLastIndex() {
        string str = "A[BC]D";
        int index = 5;
        Assert.AreEqual(5, LSystem.SkipBracketForward(str, index));
    }

    // Prefix matching tests
    [Test]
    public void TestPrefixNoBrackets() {
        string input = "ABCD";
        string pattern = "AB";
        Assert.IsTrue(LSystem.IsPrefixedBy(input, 2, pattern));
    }

    [Test]
    public void TestNonMatchingPrefix() {
        string input = "ABCD";
        string pattern = "CB";
        Assert.IsFalse(LSystem.IsPrefixedBy(input, 2, pattern));
    }

    [Test]
    public void TestPrefixWithSimpleBrackets() {
        string input = "AB[CD]EFG";
        string pattern = "AB";
        Assert.IsTrue(LSystem.IsPrefixedBy(input, 6, pattern));
    }

    [Test]
    public void TestPrefixFromInsideBracket() {
        string input = "AB[CD]EFG";
        string pattern = "AB";
        Assert.IsTrue(LSystem.IsPrefixedBy(input, 3, pattern));
    }

    [Test]
    public void TestPrefixWithNestedBrackets() {
        string input = "AB[CD[EF]GH]IJK";
        string pattern = "AB";
        Assert.IsTrue(LSystem.IsPrefixedBy(input, 12, pattern));
    }

    [Test]
    public void TestPrefixFromNestedBrackets() {
        string input = "AB[[G]CD[EF]GH]IJK";
        string pattern = "ABCD";
        Assert.IsTrue(LSystem.IsPrefixedBy(input, 9, pattern));
    }

    [Test]
    public void TestPrefixFromInsideBracket2() {
        string input = "ABC[DE][SG[HI[JK]L]MNO]";
        string pattern = "BC";
        Assert.IsTrue(LSystem.IsPrefixedBy(input, 8, pattern));
    }

    // Suffix matching tests
    [Test]
    public void TestSuffixNoBrackets() {
        string input = "ABCD";
        string pattern = "CD";
        Assert.IsTrue(LSystem.IsSuffixedBy(input, 1, pattern));
    }

    [Test]
    public void TestNonMatchingSuffix() {
        string input = "ABCD";
        string pattern = "CB";
        Assert.IsFalse(LSystem.IsSuffixedBy(input, 1, pattern));
    }

    [Test]
    public void TestSuffixWithSimpleBrackets() {
        string input = "AB[CD]EFG";
        string pattern = "EFG";
        Assert.IsTrue(LSystem.IsSuffixedBy(input, 1, pattern));
    }

    [Test]
    public void TestSuffixFromInsideBracket() {
        string input = "AB[CD]EFG";
        string pattern = "EFG";
        Assert.IsFalse(LSystem.IsSuffixedBy(input, 4, pattern));
    }

    [Test]
    public void TestSuffixWithNestedBrackets() {
        string input = "AB[CD[EF]GH]IJK";
        string pattern = "IJK";
        Assert.IsTrue(LSystem.IsSuffixedBy(input, 1, pattern));
    }

    [Test]
    public void TestSuffixFromNestedBrackets() {
        string input = "ABC[DE][SG[HI[JK]L]MNO";
        string pattern = "GHIL";
        Assert.IsTrue(LSystem.IsSuffixedBy(input, 8, pattern));
    }

    // LSystem tests
    [Test]
    public void TestSimpleLSystem() {
        string axiom = "A";
        Rule[] rules = { new Rule("A", "AB"), new Rule("B", "A") };
        LSystem lsys = new(axiom, 1, rules);
        Assert.AreEqual("AB", lsys.Generate());
    }

    [Test]
    public void TestSimpleLSystem2() {
        string axiom = "A";
        Rule[] rules = { new Rule("A", "AB"), new Rule("B", "A") };
        LSystem lsys = new(axiom, 2, rules);
        Assert.AreEqual("ABA", lsys.Generate());
    }

    [Test]
    public void TestSimpleLSystem3() {
        string axiom = "A";
        Rule[] rules = { new Rule("A", "AB"), new Rule("B", "A") };
        LSystem lsys = new(axiom, 3, rules);
        Assert.AreEqual("ABAAB", lsys.Generate());
    }

    [Test]
    public void TestContextSensitiveLSystem1() {
        string axiom = "AC";
        Rule[] rules = { new Rule("A", "AB"), new Rule("B", "A"), new Rule("B", "C", "", "D"), new Rule("D", "X") };
        LSystem lsys = new(axiom, 2, rules);
        Assert.AreEqual("ABAD", lsys.Generate());
    }

    [Test]
    public void TestContextSensitiveLSystem2() {
        string axiom = "AC";
        Rule[] rules = { new Rule("A", "AB"), new Rule("B", "A"), new Rule("B", "C", "", "D"), new Rule("D", "X"), new Rule("", "A", "D", "F") };
        LSystem lsys = new(axiom, 3, rules);
        Assert.AreEqual("ABAFX", lsys.Generate());
    }
    
    [Test]
    public void TestContextSensitiveLSystem3() {
        string axiom = "BAAAAAAAA";
        Rule[] rules = { new Rule("B", "A", "", "B"), new Rule("B", "A")};
        LSystem lsys = new(axiom, 4, rules);
        Assert.AreEqual("AAAABAAAA", lsys.Generate());
    }

    [Test]
    public void TestContextSensitiveBracketedLSystem() {
        string axiom = "A[B]C";
        Rule[] rules = {
            new Rule("A", "AB"),
            new Rule("B", "A[C]B"),
            new Rule("C", "B"),
            new Rule("", "B", "B", "X"),
        };
        LSystem lsys = new(axiom, 2, rules);
        Assert.AreEqual("ABX[AB[B]A[C]B]A[C]B", lsys.Generate());
    }
}
