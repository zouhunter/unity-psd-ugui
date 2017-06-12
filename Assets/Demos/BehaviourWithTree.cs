using System.Collections.Generic;
using System;
using UnityEngine;

public class BehaviourWithTree : MonoBehaviour, ISerializationCallbackReceiver
{
    // Node class that is used at runtime.
    // This is internal to the BehaviourWithTree class and is not serialized.
    public class Node
    {
        public string interestingValue = "value";
        public List<Node> children = new List<Node>();
    }
    // Node class that we will use for serialization.
    [Serializable]
    public struct SerializableNode
    {
        public string interestingValue;
        public int childCount;
        public int indexOfFirstChild;
    }
    // The root node used for runtime tree representation. Not serialized.
    Node root = new Node();
    // This is the field we give Unity to serialize.
    public List<SerializableNode> serializedNodes;
    public void OnBeforeSerialize()
    {
        // Unity is about to read the serializedNodes field's contents.
        // The correct data must now be written into that field "just in time".
        if (serializedNodes == null) serializedNodes = new List<SerializableNode>();
        if (root == null) root = new Node();
        serializedNodes.Clear();
        AddNodeToSerializedNodes(root);
        // Now Unity is free to serialize this field, and we should get back the expected 
        // data when it is deserialized later.
    }
    void AddNodeToSerializedNodes(Node n)
    {
        var serializedNode = new SerializableNode()
        {
            interestingValue = n.interestingValue,
            childCount = n.children.Count,
            indexOfFirstChild = serializedNodes.Count + 1
        }
        ;
        serializedNodes.Add(serializedNode);
        foreach (var child in n.children)
            AddNodeToSerializedNodes(child);
    }
    public void OnAfterDeserialize()
    {
        //Unity has just written new data into the serializedNodes field.
        //let's populate our actual runtime data with those new values.
        if (serializedNodes.Count > 0)
        {
            ReadNodeFromSerializedNodes(0, out root);
        }
        else
            root = new Node();
    }
    int ReadNodeFromSerializedNodes(int index, out Node node)
    {
        var serializedNode = serializedNodes[index];
        // Transfer the deserialized data into the internal Node class
        Node newNode = new Node()
        {
            interestingValue = serializedNode.interestingValue,
            children = new List<Node>()
        }
        ;
        // The tree needs to be read in depth-first, since that's how we wrote it out.
        for (int i = 0; i != serializedNode.childCount; i++)
        {
            Node childNode;
            index = ReadNodeFromSerializedNodes(++index, out childNode);
            newNode.children.Add(childNode);
        }
        node = newNode;
        return index;
    }
    // This OnGUI draws out the node tree in the Game View, with buttons to add new nodes as children.
    void OnGUI()
    {
        if (root != null)
            Display(root);
    }
    void Display(Node node)
    {
        GUILayout.Label("Value: ");
        // Allow modification of the node's "interesting value".
        node.interestingValue = GUILayout.TextField(node.interestingValue, GUILayout.Width(200));
        GUILayout.BeginHorizontal();
        GUILayout.Space(20);
        GUILayout.BeginVertical();
        foreach (var child in node.children)
            Display(child);
        if (GUILayout.Button("Add child"))
            node.children.Add(new Node());
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }
}

