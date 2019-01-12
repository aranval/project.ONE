using Godot;
using System;

public class DiamondMain : AnimatedSprite
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
        int diaxCount = (int) player.Get("diaxCount");

        if (this.GetNode<Area2D>("Area2D").OverlapsBody(player)) {
            // this.GetChildren().Remove();
            diaxCount++;
            player.Set("diaxCount", diaxCount);
            this.QueueFree(); 
        }
    }
}
