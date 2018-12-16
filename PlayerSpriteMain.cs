using Godot;
using System;

public class PlayerSpriteMain : KinematicBody2D
{
    [Export] public int SPEED = 200; //[Export] pozawala na wyświetlenie parametru w edytorze
    [Export] public int GRAV = 20;
    [Export] public int JUMP = -700;
    Vector2 FLOOR = new Vector2(0, -1); 
    Vector2 velocity = new Vector2();
    public AnimatedSprite playerSprite;
    private int direction = 0;

    public override void _PhysicsProcess(float delta)
    {
        
        playerSprite = (AnimatedSprite)GetNode("PlayerSprite");
        if (Input.IsActionPressed("ui_right"))
        {
            direction = 1;
            velocity.x = SPEED;
        }
        else if (Input.IsActionPressed("ui_left"))
        {
            direction = -1;
            velocity.x = -SPEED;
        } else {
        	velocity.x = 0;
        }
        // if (Input.IsActionPressed("ui_down"))
        // {
        //     velocity.y += 1;
        // }
        // if (IsOnFloor()) {
        //     GD.Print("GO UP YOU MOFO");
        // }
        if (Input.IsActionPressed("ui_up") && IsOnFloor())
        {
            velocity.y = JUMP;
            // GD.Print(JUMP);
        }
        // velocity = velocity.Normalized() * SPEED;
        GD.Print(playerSprite.GlobalPosition);
        // GetInput();
        velocity.y += GRAV; 
        velocity = MoveAndSlide(velocity, FLOOR);
        if ((direction == 1 || direction == 0) && velocity.x == 0 && IsOnFloor()) {
        	playerSprite.Play("idleRight");
        } else if (direction == -1 && velocity.x == 0 && IsOnFloor()) {
        	playerSprite.Play("idleLeft"); 
        } else if (direction == 1 && velocity.x != 0 && IsOnFloor()) {
        	playerSprite.Play("walkRight"); 
        } else if (direction == -1 && velocity.x != 0 && IsOnFloor()) {
        	playerSprite.Play("walkLeft"); 
        }
    }
}