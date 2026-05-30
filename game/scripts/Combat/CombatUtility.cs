using Godot;

namespace Aegea;

public static class CombatUtility
{
    public static Damageable? FindDamageable(Node node)
    {
        if (node is Damageable damageable)
        {
            return damageable;
        }

        foreach (Node child in node.GetChildren())
        {
            if (child is Damageable childDamageable)
            {
                return childDamageable;
            }
        }

        Node? parent = node.GetParent();
        if (parent is not null)
        {
            foreach (Node sibling in parent.GetChildren())
            {
                if (sibling is Damageable siblingDamageable)
                {
                    return siblingDamageable;
                }
            }
        }

        return null;
    }
}
