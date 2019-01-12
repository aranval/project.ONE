using System;
using Godot;

public class DoorMain : Area2D {
    // Member variables here, example:
    // private int a = 2;
    // private string b = "textvar";

    public override void _Ready() {
        // Called every time the node is added to the scene.
        // Initialization here

    }

    public override void _Process(float delta) {
        Node player = GetParent().GetNode("Player");
        int keyCount = (int) player.Get("keyCount");

        if (this.OverlapsBody(player) && keyCount > 0) {
            keyCount--;
            player.Set("keyCount", keyCount);
            this.Hide(); 
            this.QueueFree();
        }
    }
}