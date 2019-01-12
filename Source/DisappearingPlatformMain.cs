using System;
using Godot;

public class DisappearingPlatformMain : AnimatedSprite {
    private bool visible = true;

    public override void _Ready() {
        // Called every time the node is added to the scene.
        // Initialization here

    }

    private void _OnTimerTimeout() {
        CollisionShape2D platColl = (this.GetNode("StaticBody2D")).GetNode<CollisionShape2D>("CollisionShape2D");
        if (visible) {
            this.Play("disappear");
            visible = false;
            platColl.Disabled = !visible;
        } else {
            this.Play("appear");
            visible = true;
            platColl.Disabled = !visible;
        }
    }

    //    public override void _Process(float delta)
    //    {
    //        // Called every frame. Delta is time since last frame.
    //        // Update game logic here.
    //        
    //    }
}