using Godot;
using System;

public class KeyMain : Area2D
{
    // Member variables here, example:
    // private int a = 2;
    // private string b = "textvar";

    public override void _Ready()
    {
        // Called every time the node is added to the scene.
        // Initialization here
        
    }

    public override void _Process(float delta) {
        Node player = GetParent().GetNode("Player");
        bool hasKey = (bool) player.Get("hasKey");

        if (this.OverlapsBody(player) && !hasKey) {
            hasKey = true;
            player.Set("hasKey", hasKey);
            this.Hide(); 
        }
    }
}
