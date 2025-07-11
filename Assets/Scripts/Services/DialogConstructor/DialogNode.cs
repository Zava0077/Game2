using System;
using System.Collections.Generic;
using System.Linq;
public abstract class DialogNode //хуёвый пока конструктор
{
    private static readonly Random rnd = new Random();
    public bool Last { get; set; }
    public DialogNode Parent { get; private set; }
    public List<DialogNode> Childrens { get; private set; } = new List<DialogNode>();
    public Action NodeReaction { get; private set; }
    protected DialogNode() { }
    public static HangingNode CreateHanging() => new HangingNode();
    public DialogNode ToFirstParentOfType<T>(int depth = 1)
    {
        DialogNode dynamicNode = this;
        while (depth > 0)
        {
            dynamicNode = dynamicNode.GetToParentOrDefault();
            if (dynamicNode is T) depth--;
        }
        return dynamicNode;
    }
    /// <summary>
    /// Создает вилку из 2 нодов.
    /// Лучше использовать когда у этих двух нодов не планируется добавление нодов - детей
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="forkAction"></param>
    /// <param name="firstAction"></param>
    /// <param name="secondAction"></param>
    /// <returns></returns>
    public ForkNode CreateFork<T, R>(Action forkAction = null, Action firstAction = null, Action secondAction = null) where T : DialogNode, new() where R : DialogNode, new()
    {
        var node = CreateChildren<ForkNode>(forkAction);
        node.CreateChildren<T>(firstAction);
        node.CreateChildren<R>(secondAction);
        return node;
    }
    public T CreateChildren<T>(Action reaction = null) where T : DialogNode, new()
    {
        T node = new T() { Parent = this, NodeReaction = reaction };
        Childrens.Add(node);
        return node;
    }
    public DialogNode GetToParentOrDefault() => Parent ?? this;
    public DialogNode GetToChildrenOrDefault<T>() where T : DialogNode
    {
        if (Last) return this;
        var childrens = Childrens.OfType<T>().ToList();
        return !childrens.Any() ? this : childrens.ElementAt(rnd.Next(childrens.Count()));
    }
    public virtual void React()
    {
        //if (NodeReaction == null)
        //{
        //    Last = true;
        //    return;
        //}
        NodeReaction.Invoke();
        //NodeReaction = null;
    }
}
public sealed class HangingNode : DialogNode
{
    public override void React() { }
}
public sealed class FirstNode : DialogNode { }
public sealed class SecondNode : DialogNode { }
public sealed class ThirdNode : DialogNode { }
public sealed class FourthNode : DialogNode { }
public sealed class FifthNode : DialogNode { }
public sealed class ForkNode : DialogNode { }
