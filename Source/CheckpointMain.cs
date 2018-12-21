using System;
using Godot;

public class CheckpointMain : Area2D {
    // Member variables here, example:
    // private int a = 2;
    // private string b = "textvar";

    public override void _Ready() {
        // Called every time the node is added to the scene.
        // Initialization here

    }

    public override void _Process(float delta) {
        Node player = GetParent().GetNode("Player");

        if (this.OverlapsBody(player)) {
            Position2D spawn = (Position2D) GetNode("Spawn");
            player.Set("currentCheckpoint", spawn.GetGlobalPosition());
        }
    }
}