using Godot;
using System;

public class PlayerMain : KinematicBody2D
{
	//TO JEST FAKE ZEBY GODOT SIE NIE WYPIERDALAL PO USUWANIU PLIKÓW - TODO ZNALEŹĆ ODNIESIENIE DO PLAYERSPRITEMAIN I JE WYJEBAĆ
    // public int SPEED = 40; //[Export] public int SPEED = 40; [Export] allows changing variable in-editor for test puproses
    // public int GRAV = 10;
    // public int JUMP = -200;
    // Vector2 FLOOR = new Vector2(0, -1);
    // Vector2 velocity = new Vector2();
    // public AnimatedSprite playerSprite;
    // private int direction;
    // private bool onGround = true;
    // private bool jumping;
    // private Vector2 playerPosition;
    // public bool onRotationPlatform = false;
    // public bool onLadder = false;
    // public bool hasKey = false;
    // public bool isDead = false;
    // public RotatorPlatformMain rpm;
    // public override void _PhysicsProcess(float delta)
    // {
    // 	playerSprite = (AnimatedSprite)GetNode("PlayerSprite");
    //     playerPosition = playerSprite.GlobalPosition;

    //     //Checking if on Ladder
    //     TileMap tm = (TileMap) GetParent().GetNode("walls");
    //     var worldPos = tm.WorldToMap(playerSprite.GlobalPosition);
    //     worldPos.x = worldPos.x+3; // ugliest fix possible, no clue why there was some weird [3,-11] offset between grind in Godot and live
    //     worldPos.y = worldPos.y-1;
    //     int id = tm.GetCellv(worldPos);
    //     if (id > -1) { //id > -1 - theres a Tile painted
    //         if(tm.GetTileset().TileGetName(id) == "Ladder") {
    //                 onLadder = true;
    //         } else {
    //                 onLadder = false;
    //         }
    //     } else {
    //         onLadder = false;
    //     }
        
    //     //Basic controls
    //     if (Input.IsActionPressed("ui_right"))
    //     {
    //         direction = 1;
    //         velocity.x = SPEED;
    //         if (onGround) {
    //             playerSprite.Play("walk");
    //             } else {
    //                 playerSprite.Play("jump");
    //             }
    //         playerSprite.FlipH = false; 
    //     }
    //     else if (Input.IsActionPressed("ui_left"))
    //     {
    //         direction = -1;
    //         velocity.x = -SPEED;
    //         if (onGround) {
    //             playerSprite.Play("walk");
    //             } else {
    //                 playerSprite.Play("jump");
    //             }
    //         playerSprite.FlipH = true; //flips animation to be coresponding to player direction
    //     } else {
    //     	velocity.x = 0;
    //     	if(direction == -1) {
    //     		playerSprite.Play("idle");
    //     	} else {
    //     		playerSprite.Play("idle");
    //     	}
    //     }

    //     //Jump controls
    //     if (Input.IsActionPressed("ui_up") && onGround && !onLadder)
    //     {
    //         velocity.y = JUMP;
    //         if(velocity.y < 0) {
    //     		// playerSprite.FlipH;
    //             jumping = true;
    //         	playerSprite.Play("jump");
    //     	} 
    //     }

    //     if (velocity.y > 0) {
    //         jumping = false;
    //     } 

    //     //Controls on ladder
    //     if(onLadder) {
    //         if (Input.IsActionPressed("ui_up")) { 
                
    //             velocity.y = -SPEED;
    //         } else if (Input.IsActionPressed("ui_down")) {
    //             velocity.y = SPEED;
    //         } else {
    //             velocity.y = 0;
    //             velocity = MoveAndSlide(velocity);
    //         } 
    //     } else {
    //         velocity.y += GRAV;
    //         velocity = MoveAndSlide(velocity, FLOOR);
    //     }


    //     // velocity = velocity.Normalized() * SPEED;
    //     // GD.Print(velocity);
    //     // GetInput();
        
    //     onGround = IsOnFloor();
    //     // player = (Sprite)GetNode("PlayerSprite");
    //     // TileMap tm = (TileMap) GetNode("walls"); Cant see Wall from here
    //     GD.Print(jumping);
        
    }

}
