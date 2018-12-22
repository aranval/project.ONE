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
        Position2D spawn = (Position2D) GetNode("Spawn");

        if (this.OverlapsBody(player)) {
            
            player.Set("currentCheckpoint", spawn.GetGlobalPosition());
        }

        if(spawn.GetGlobalPosition() == (Vector2) player.Get("currentCheckpoint")) {
            GetNode<Sprite>("Active").SetVisible(true);
        } else {
            GetNode<Sprite>("Active").SetVisible(false);
        }
    }
}