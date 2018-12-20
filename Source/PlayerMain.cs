using Godot;
using System;

public class PlayerMain : KinematicBody2D
{
    [Export] public int SPEED = 40; //[Export] public int SPEED = 40;
    [Export] public int GRAV = 10;
    [Export] public int JUMP = -200;
    [Export] public int ROTATOR_SPEED = 20;
    Vector2 FLOOR = new Vector2(0, -1);
    Vector2 velocity = new Vector2();
    public AnimatedSprite playerSprite;
    private int direction;
    private bool onGround = true;
    private bool jumping;
    private Vector2 playerPosition;
    public bool onRotationPlatform = false;
    public bool onLadder = false;
    [Export] public bool hasKey = false;
    [Export] public bool isDead = false;
    public RotatorPlatformMain rpm;

    public String _GetPlayerPositionTileType(Vector2 playerPosition, TileMap tm, int x, int y) {
        var worldPos = tm.WorldToMap(playerSprite.GlobalPosition);
            worldPos.x += x;
            worldPos.y += y;
        if(tm.GetName() == "walls") {
            worldPos.x = worldPos.x+3; //VERIFY IF THE "FIX" IS REQUIRED FOR ALL THE TILESETS
            worldPos.y = worldPos.y-1;
        }
        
        int id = tm.GetCellv(worldPos);
        if(id > -1) {
        return tm.GetTileset().TileGetName(id);
        } else {
            return "Empty";
        }
    }

    public String _GetPlayerPositionTileType(Vector2 playerPosition, TileMap tm) {
        return _GetPlayerPositionTileType(playerPosition, tm, 0, 0);
    }

    public Boolean _CheckLadder(Vector2 playerPosition) {
        if(_GetPlayerPositionTileType(playerPosition, (TileMap) GetParent().GetNode("walls")) == "Ladder") {
            return true;
        }
        return false;
    }

    public Boolean _CheckRotator(Vector2 playerPosition) {
        TileMap tm = (TileMap) GetParent().GetNode("walls"); // UGLY AF, make containter and a foreach
        GD.Print(_GetPlayerPositionTileType(playerPosition, tm, 0, 1));
        if(_GetPlayerPositionTileType(playerPosition, tm, 0, 1) == "Rotator00" || _GetPlayerPositionTileType(playerPosition, tm, 0, 1) == "Rotator01" || _GetPlayerPositionTileType(playerPosition, tm, 0, 1) == "Rotator02" || _GetPlayerPositionTileType(playerPosition, tm, 0, 1) == "Rotator03") { 
            return true;
        }
 
        return false;
    }

    
    public override void _PhysicsProcess(float delta)
    {
    	playerSprite = (AnimatedSprite)GetNode("PlayerSprite");
        playerPosition = playerSprite.GlobalPosition;
        Area2D door = (Area2D) GetParent().GetNode("Door");
        Area2D door2 = (Area2D) GetParent().GetNode("Door2");
        Area2D key = (Area2D) GetParent().GetNode("Key00");

        //Checking if on Ladder
        TileMap tm = (TileMap) GetParent().GetNode("background");
        var worldPos = tm.WorldToMap(playerSprite.GlobalPosition);
        worldPos.x = worldPos.x+3; // ugliest fix possible, no clue why there was some weird [3,-11] offset between grind in Godot and live
        worldPos.y = worldPos.y-1;
        int id = tm.GetCellv(worldPos);
        if (id > -1) { //id > -1 - theres a Tile painted
            if(tm.GetTileset().TileGetName(id) == "Ladder") {
                    onLadder = true;
            } else {
                    onLadder = false;
            }
        } else {
            onLadder = false;
        }
        
        //Basic controls
        int rotator = 0; //pretty sure this can be done way clearer UGLY AF CLEAN THIS SHIT UP
        if(_CheckRotator(playerPosition)) {
            rotator = ROTATOR_SPEED;
        }
        if (Input.IsActionPressed("ui_right"))
        {
            direction = 1;
            velocity.x = SPEED-rotator;
            if (onGround) {
                playerSprite.Play("walk");
                } else {
                    playerSprite.Play("jump");
                }
            playerSprite.FlipH = false; 
        }
        else if (Input.IsActionPressed("ui_left"))
        {
            direction = -1;
            velocity.x = -(SPEED+rotator);
            if (onGround) {
                playerSprite.Play("walk");
                } else {
                    playerSprite.Play("jump");
                }
            playerSprite.FlipH = true; //flips animation to be coresponding to player direction
        } else {
        	velocity.x = 0-rotator;
        	if(direction == -1) {
        		playerSprite.Play("idle");
        	} else {
        		playerSprite.Play("idle");
        	}
        }

        //Jump controls
        if (Input.IsActionPressed("ui_up") && onGround)
        {
            if(onLadder) {
                velocity.y = JUMP*2;
            } else {  
                velocity.y = JUMP;
            }
            if(velocity.y < 0) {
        		// playerSprite.FlipH;
                jumping = true;
            	playerSprite.Play("jump");
        	} 
        } 
        if (velocity.y >= 0) {
            jumping = false;
        } 

        //Controls on ladder REMOVE PLACEHOLDER IN JUMP!!!
        // if(onLadder) {
        //     if (Input.IsActionPressed("ui_up")) { 
                
        //         velocity.y = -SPEED;
        //     } else if (Input.IsActionPressed("ui_down")) {
        //         velocity.y = SPEED;
        //     } else {
        //         velocity.y = 0;
        //         velocity = MoveAndSlide(velocity);
        //     } 
        // } else {
        //     //Standard grav
        //     velocity.y += GRAV;
        //     velocity = MoveAndSlide(velocity, FLOOR);
        // }

        //Basic door controls TODO: develop further, add Door despawn / opening anim EXPORT TO DoorMain.cs
        if (door.OverlapsBody(GetParent().GetNode("Player")) && hasKey == true) {
            hasKey = false;
            door.Hide(); //crash at despawn
            door.RemoveChild(door.GetNode("Locked"));
        } else if (door2.OverlapsBody(GetParent().GetNode("Player")) && hasKey == true) {
            hasKey = false;
            door2.Hide();
            door2.RemoveChild(door2.GetNode("Locked"));
        }
        //Basic key controls EXPORT TO KeyMain.cs
        if (key.OverlapsBody(GetParent().GetNode("Player"))) {
            key.Hide();
            hasKey = true;
        }


        //STANDARD GRAV HANDLING
        velocity.y += GRAV; 
        velocity = MoveAndSlide(velocity, FLOOR);


        // velocity = velocity.Normalized() * SPEED;
        // GD.Print(velocity);
        // GetInput();
        
        onGround = IsOnFloor();
        // player = (Sprite)GetNode("PlayerSprite");
        // TileMap tm = (TileMap) GetNode("walls"); Cant see Wall from here
        // GD.Print(onLadder + " " + _CheckLadder(playerPosition));
        // GD.Print(_CheckRotator(playerPosition) + " " + _CheckLadder(playerPosition));
        GD.Print(hasKey);
        
    }

}
