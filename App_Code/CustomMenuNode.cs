using System.Collections.Generic;

public class CustomMenuNode
{
    public string Title { get; set; }
    public string Url { get; set; }
    public List<CustomMenuNode> Children { get; set; }

    public CustomMenuNode()
    {
        Children = new List<CustomMenuNode>();
    }
}

