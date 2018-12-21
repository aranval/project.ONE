using System;
using Godot;

public class PlayerMain : KinematicBody2D {
    [Export] public int SPEED = 40; //[Export] public int SPEED = 40;
    [Export] public int GRAV = 10;
    [Export] public int JUMP = -210;
    [Export] public int ROTATOR_SPEED = -20;
    [Export] public int PLAYER_FALL_DEATH = 220;
    [Export] public int lifeCount;
    Vector2 FLOOR = new Vector2(0, -1);
    Vector2 velocity = new Vector2();
    public AnimatedSprite playerSprite;
    public AudioStreamPlayer2D playerSounds;
    private int direction;
    private bool onGround = true;
    private float prevFrameVelocityY = 0;
    private bool jumping; // ? useful later
    private Vector2 playerPosition;
    public CollisionShape2D playerHitbox;
    public bool onLadder = false;
    public bool hasFatallyCollided = false;
    [Export] public bool hasKey = false;
    public bool hasFallen = false;
    public bool isDead = false;
    public Vector2 currentCheckpoint;

    public override void _Ready() {
        Position2D pos = GetParent().GetNode<Position2D>("Spawn");
        currentCheckpoint = pos.GetGlobalPosition();
        lifeCount = 4;
    }

    /** GetPlayerPositionTileType
     * @param playerPosition        Global position of specific character
     * @param tm                    Specific tileset
     * @param x                     x variable of Vector2(x, y) allows returning different tiles than the one character is currently at
     * @param y                     y variable of Vector2(x, y) allows returning different tiles than the one character is currently at
     * @return String          Returns either name of a currently chosen tile or keyword Empty
     */
    public String GetPlayerPositionTileType(Vector2 playerPosition, TileMap tm, int x, int y) {
        var worldPos = tm.WorldToMap(playerSprite.GlobalPosition);
        worldPos.x += x;
        worldPos.y += y;
        if (tm.GetName() == "walls") {
            worldPos.x = worldPos.x + 3; // ? VERIFY IF THE "FIX" IS REQUIRED FOR ALL THE TILESETS
            worldPos.y = worldPos.y - 1;
        }

        int id = tm.GetCellv(worldPos);
        if (id > -1) {
            return tm.GetTileset().TileGetName(id);
        } else {
            return "Empty";
        }
    }

    /** GetPlayerPositionTileType
     * @param playerPosition        Global position of specific character
     * @param tm                    Specific tileset
     * @return Method               Returns most common case - at player position
     */
    public String GetPlayerPositionTileType(Vector2 playerPosition, TileMap tm) {
        return GetPlayerPositionTileType(playerPosition, tm, 0, 0);
    }

    /** CheckLadder
     * @param playerPosition        Global position of specific character
     * @return Boolean              Returns true if character enters tile type Ladder            
     */
    public Boolean CheckLadder(Vector2 playerPosition) {
        if (GetPlayerPositionTileType(playerPosition, (TileMap) GetParent().GetNode("walls")) == "Ladder") {
            return true;
        }
        return false;
    }

    /** CheckRotator
     * @param playerPosition
     * @return Boolean
     */
    public Boolean CheckRotator(Vector2 playerPosition) {
        TileMap tm = (TileMap) GetParent().GetNode("walls"); // TODO: UGLY AF, make containter and a foreach
        if (GetPlayerPositionTileType(playerPosition, tm, 0, 1) == "Rotator00" || GetPlayerPositionTileType(playerPosition, tm, 0, 1) == "Rotator01" || GetPlayerPositionTileType(playerPosition, tm, 0, 1) == "Rotator02" || GetPlayerPositionTileType(playerPosition, tm, 0, 1) == "Rotator03") {
            return true;
        } else if (GetPlayerPositionTileType(playerPosition, tm, 1, 1) == "Rotator00" || GetPlayerPositionTileType(playerPosition, tm, -1, 1) == "Rotator03") { //QUICKFIX for abusing edge of hitbox
            return true;
        }
        return false;
    }

    /** PlayerDies
     *  @param reason
     */
    public void PlayerDies(String reason) {
        CollisionShape2D collisionShape2D = (CollisionShape2D) GetNode("CollisionShape2D");
        Timer timer = GetNode<Timer>("Timer");
        isDead = true;
        lifeCount--;
        velocity = new Vector2(0, 0);
        if (reason == "hit") {
            playerSprite.Play("dead");
        } else if (reason == "fall") {
            playerSprite.Play("splash");
        }
        collisionShape2D.Disabled = true;
        timer.Start();
    }

    /** PlayerDies
     *  @return Method PlayerDies()
     */
    public void PlayerDies() {
        PlayerDies("hit");
    }

    /** CheckFallDeath
     *  @return Boolean 
     */
    private void CheckFallDeath() {
        if (velocity.y > PLAYER_FALL_DEATH) {
            hasFallen = true;
        }
    }

    /** GetPlayerPositionTileType
     *  @param 
     *  @return
     * TODO: Add Animation, make timer longer
     */
    private void _OnTimerTimeout() {

        isDead = false;
        hasFatallyCollided = false;
        playerHitbox.Disabled = false;
        // this.collisionShape2D.Disabled = false; wonder why godot implementation doesnt allow this to work.
        if (lifeCount > 0) {
            this.SetPosition(currentCheckpoint);
        } else {
            GetTree().ChangeScene("Scenes\\MainMenu.tscn");
        }
    }

    public override void _PhysicsProcess(float delta) {
        // TODO: Getting scene elements !MIGHT GET LONG AF THINK OF WORKAROUND!
        playerSprite = (AnimatedSprite) GetNode("PlayerSprite");
        playerSounds = (AudioStreamPlayer2D) GetNode("jump");
        playerPosition = playerSprite.GlobalPosition;
        playerHitbox = GetNode<CollisionShape2D>("CollisionShape2D");
        // Area2D door = (Area2D) GetParent().GetNode("Door");
        // Area2D door2 = (Area2D) GetParent().GetNode("Door2");
        // Area2D key = (Area2D) GetParent().GetNode("Key00");
        // Area2D enemyOneHitBox = (Area2D) GetParent().GetNode("EnemyOne").GetNode("Area2D");
        // Area2D checkpoint = (Area2D) GetParent().GetNode("Checkpoint00");

        if (!isDead) { // TODO: Verify use of Pause (and PausePopup)

            // // // Checking if on Ladder
            // // TileMap tm = (TileMap) GetParent().GetNode("background");
            // // var worldPos = tm.WorldToMap(playerSprite.GlobalPosition);
            // // worldPos.x = worldPos.x+3; // * ugliest fix possible, no clue why there was some weird [3,-1] offset between grind in Godot and live
            // // worldPos.y = worldPos.y-1;
            // // int id = tm.GetCellv(worldPos);
            // // if (id > -1) {             // id > -1 - theres a Tile painted
            // //     if(tm.GetTileset().TileGetName(id) == "Ladder") {
            // //             onLadder = true;
            // //     } else {
            // //             onLadder = false;
            // //     }
            // // } else {
            // //     onLadder = false;
            // // }

            //Basic controls
            int rotator = 0; //pretty sure this can be done way clearer UGLY AF CLEAN THIS SHIT UP
            if (CheckRotator(playerPosition)) {
                rotator = ROTATOR_SPEED;
            }
            if (Input.IsActionPressed("ui_right")) {
                direction = 1;
                velocity.x = SPEED - rotator;
                if (onGround) {
                    playerSprite.Play("walk");
                } else {
                    playerSprite.Play("jump");
                }
                playerSprite.FlipH = false;
            } else if (Input.IsActionPressed("ui_left")) {
                direction = -1;
                velocity.x = -(SPEED + rotator);
                if (onGround) {
                    playerSprite.Play("walk");
                } else {
                    playerSprite.Play("jump");
                }
                playerSprite.FlipH = true; //flips animation to be coresponding to player direction
            } else {
                velocity.x = 0 - rotator;
                if (direction == -1) {
                    playerSprite.Play("idle"); //Co tu miało być ????
                } else {
                    playerSprite.Play("idle");
                }
            }
            if(Input.IsActionJustPressed("ui_down")) {
                Vector2 drop = new Vector2(playerPosition.x, playerPosition.y+1);
                this.SetPosition(drop);
            }

            //Jump controls
            if (Input.IsActionPressed("ui_up") && onGround) {
                if (onLadder) {
                    velocity.y = JUMP * 2;
                } else {
                    velocity.y = JUMP;
                }
                if (velocity.y < 0) {
                    jumping = true;
                    playerSprite.Play("jump");
                    playerSounds.Play();
                }
            }
            if (velocity.y > 0) {
                jumping = false;
            }

            CheckFallDeath();
            if (velocity.y == 0 && prevFrameVelocityY >= 150) {
                if(!hasFallen) {
                    GD.Print("Recovering...");
                    playerSprite.Play("recover");
                } else {
                    GD.Print("Well, fuck me.");
                    PlayerDies("fall");
                }
                
            }

            if (hasFatallyCollided) {
                PlayerDies();
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
            // if (door.OverlapsBody(GetParent().GetNode("Player")) && hasKey == true) {
            //     hasKey = false;
            //     door.Hide(); //crash at despawn
            //     door.RemoveChild(door.GetNode("Locked"));
            // } else if (door2.OverlapsBody(GetParent().GetNode("Player")) && hasKey == true) {
            //     hasKey = false;
            //     door2.Hide();
            //     door2.RemoveChild(door2.GetNode("Locked"));
            // }
            //Basic key controls EXPORT TO KeyMain.cs
            // if (key.OverlapsBody(GetParent().GetNode("Player"))) {
            //     key.Hide();
            //     hasKey = true;
            // }
            //Basic checkpoint handling, same as above
            // if (checkpoint.OverlapsBody(GetParent().GetNode("Player"))) {
            //     currentCheckpoint = checkpoint;
            // }

            prevFrameVelocityY = velocity.y;
            onGround = IsOnFloor();
            
            //STANDARD GRAV HANDLING
            velocity.y += GRAV;
            velocity = MoveAndSlide(velocity, FLOOR);
            
            //COLLISIONS - TODO GET THIS SHIT DONE IN EnemyOne.cs
            // if (enemyOneHitBox.OverlapsBody(GetParent().GetNode("Player"))) {
            //     PlayerDies();
            // }

            //Blink test
            // if (Input.IsActionJustPressed("ui_test")) {
            //     // Vector2 pos = this.GetGlobalPosition();
            //     Position2D spawn = (Position2D) GetParent().GetNode("Spawn00");
            //     Vector2 pos = spawn.GetGlobalPosition();
            //     this.SetGlobalPosition(pos);
            //     GD.Print("CAST SPELL TELEPORT");
            // }

        } //*  END OF IF ISDEAD LOOP

        // velocity = velocity.Normalized() * SPEED;

        
        
        // GD.Print(velocity.y + " " + prevFrameVelocityY);
        // GD.Print(currentCheckpoint);
        // player = (Sprite)GetNode("PlayerSprite");
        // TileMap tm = (TileMap) GetNode("walls"); Cant see Wall from here
        // GD.Print(onLadder + " " + CheckLadder(playerPosition));
        // GD.Print(CheckRotator(playerPosition) + " " + CheckLadder(playerPosition));
        // GD.Print(GetSlideCount());
        // GD.Print(GetPlayerPositionTileType(playerPosition, (TileMap) GetParent().GetNode("walls"), -1, 1) + " " + GetPlayerPositionTileType(playerPosition, (TileMap) GetParent().GetNode("walls"), 0, 1) + GetPlayerPositionTileType(playerPosition, (TileMap) GetParent().GetNode("walls"), 1, 1));
    }

}