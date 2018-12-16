using Godot;
using System;

public class PlayerMain : KinematicBody2D
{
    public int SPEED = 40; //[Export] pozawala na wyświetlenie parametru w edytorze
    public int GRAV = 10;
    public int JUMP = -200;
    Vector2 FLOOR = new Vector2(0, -1);
    Vector2 velocity = new Vector2();
    public AnimatedSprite playerSprite;
    private int direction;
    private bool onGround = false;
    public bool onRotationPlatform = false;
    public RotatorPlatformMain rpm;

    public override void _PhysicsProcess(float delta)
    {
    	playerSprite = (AnimatedSprite)GetNode("PlayerSprite");
    	if (Input.IsActionPressed("ui_right"))
        {
            direction = 1;
            velocity.x = SPEED;
            playerSprite.Play("walk");
            playerSprite.FlipH = false;
        }
        else if (Input.IsActionPressed("ui_left"))
        {
            direction = -1;
            velocity.x = -SPEED;
            playerSprite.Play("walk");
            playerSprite.FlipH = true;
        } else {
        	velocity.x = 0;
        	if(direction == -1) {
        		// playerSprite.FlipH;
            	playerSprite.Play("idle");
        	} else {
        		playerSprite.Play("idle");
        	}
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
            if(velocity.y < 0) {
        		// playerSprite.FlipH;
            	playerSprite.Play("jump");
        	} else {
        		playerSprite.Play("fall");
        	}
        }
        // velocity = velocity.Normalized() * SPEED;
        // GD.Print(velocity);
        // GetInput();
        velocity.y += GRAV;
        velocity = MoveAndSlide(velocity, FLOOR);
        // player = (Sprite)GetNode("PlayerSprite");
        GD.Print("Player Position: " + onRotationPlatform);
    }
}
