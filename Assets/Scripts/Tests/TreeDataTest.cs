using UnityEngine;
using NUnit.Framework;

public class TreeDataTest {
    // Test empty string input
    [Test]
    public void TestEmptyString() {
        string str = "";
        TreeData treeData = new(1, 90);
        treeData.CreateTreeDataFromString(str);
        // There should be one node at (0, 0, 0) coordinates
        Assert.IsTrue(treeData.root.position == Vector3.zero);
    }

    // Test simplest string input
    [Test]
    public void TestSimpleString() {
        string str = "F";
        TreeData treeData = new(1, 90);
        treeData.CreateTreeDataFromString(str);
        // There should be two nodes at (0, 0, 0) and (0, 1, 0) coordinates
        Assert.IsTrue(treeData.root.position == Vector3.zero);
        Assert.IsTrue(treeData.root.children[0].position == new Vector3(0, 1, 0));
    }

    // Test "FF" string input
    [Test]
    public void TestSimpleString2() {
        string str = "FF";
        TreeData treeData = new(1, 90);
        treeData.CreateTreeDataFromString(str);
        // There should be three nodes at (0, 0, 0), (0, 1, 0) and (0, 2, 0) coordinates
        Assert.IsTrue(treeData.root.position == Vector3.zero);
        Assert.IsTrue(treeData.root.children[0].position == new Vector3(0, 1, 0));
        Assert.IsTrue(treeData.root.children[0].children[0].position == new Vector3(0, 2, 0));
    }

    // Test "F+F" string input
    [Test]
    public void TestAngleIncrement() {
        string str = "F+F";
        TreeData treeData = new(1, 90);
        treeData.CreateTreeDataFromString(str);
        // There should be three nodes at (0, 0, 0), (0, 1, 0) and (-1, 1, 0) coordinates
        Assert.IsTrue(treeData.root.position == Vector3.zero);
        Assert.IsTrue(treeData.root.children[0].position == new Vector3(0, 1, 0));
        Assert.IsTrue(treeData.root.children[0].children[0].position == new Vector3(-1, 1, 0));
    }
    
    // Test "F-F" string input
    [Test]
    public void TestAngleDecrement() {
        string str = "F-F";
        TreeData treeData = new(1, 90);
        treeData.CreateTreeDataFromString(str);
        // There should be three nodes at (0, 0, 0), (0, 1, 0) and (1, 1, 0) coordinates
        Assert.IsTrue(treeData.root.position == Vector3.zero);
        Assert.IsTrue(treeData.root.children[0].position == new Vector3(0, 1, 0));
        Assert.IsTrue(treeData.root.children[0].children[0].position == new Vector3(1, 1, 0));
    }
    
    // Test brackets
    [Test]
    public void TestBrackets() {
        string str = "F[+F][-F]";
        TreeData treeData = new(1, 90);
        treeData.CreateTreeDataFromString(str);
        // There should be three nodes at (0, 0, 0), (0, 1, 0), (1, 1, 0) and (-1, 1, 0) coordinates 
        Assert.IsTrue(treeData.root.position == Vector3.zero);
        Assert.IsTrue(treeData.root.children[0].position == new Vector3(0, 1, 0));
        Assert.IsTrue(treeData.root.children[0].children[0].position == new Vector3(-1, 1, 0));
        Assert.IsTrue(treeData.root.children[0].children[1].position == new Vector3(1, 1, 0));
    }
}
