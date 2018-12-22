using System;
using Godot;

public class TestSpriteMain : KinematicBody2D {
    public int Speed = 200;

    Vector2 velocity = new Vector2();

    public void GetInput() {
        velocity = new Vector2();
        if (Input.IsActionPressed("ui_right")) {
            GD.Print("Right");
            velocity.x += 1;
        }
        if (Input.IsActionPressed("ui_left")) {
            GD.Print("Left");
            velocity.x -= 1;
        }
        if (Input.IsActionPressed("ui_down")) {
            GD.Print("Down");
            velocity.y += 1;
        }
        if (Input.IsActionPressed("ui_up")) {
            GD.Print("Up");
            velocity.y -= 1;
        }
        velocity = velocity * Speed;
    }

    public override void _PhysicsProcess(float delta) {
        GetInput();

        MoveAndSlide(velocity);
    }
}