using System;
using Godot;

public class PlayerMain : KinematicBody2D {
    [Export] public int SPEED = 40; //
    [Export] public int CLIMB_SPEED = 10;
    [Export] public int GRAV = 10;
    [Export] public int JUMP = -210;
    [Export] public int ROTATOR_SPEED = -20;
    public int PLAYER_FALL_DEATH = 2000;
    [Export] public int lifeCount;
    Vector2 FLOOR = new Vector2(0, -1);
    Vector2 NOFLOOR = new Vector2(0, 0);
    Vector2 velocity = new Vector2();
    public AnimatedSprite playerSprite;
    public AudioStreamPlayer2D playerSounds;
    private int direction;
    private bool onGround = true;
    private float prevFrameVelocityY = 0;
    private bool justJumped;
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
            // worldPos.x = worldPos.x + 3; // ? VERIFY IF THE "FIX" IS REQUIRED FOR ALL THE TILESETS
            // worldPos.y = worldPos.y - 1;
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
        if ((GetPlayerPositionTileType(playerPosition, (TileMap) GetParent().GetNode("walls")) == "NewLadder") ||
            (GetPlayerPositionTileType(playerPosition, (TileMap) GetParent().GetNode("walls"), 0, 1) == "NewLadder")) {
            return true;
        }
        return false;
    }

    public Boolean CheckLadder() {
        return CheckLadder(playerPosition);
    }

    public Boolean CheckLadderUnder(Vector2 playerPosition) {
        if ((GetPlayerPositionTileType(playerPosition, (TileMap) GetParent().GetNode("walls"), 0, 1) == "NewLadder")) {
            return true;
        }
        return false;
    }    

    /** CheckLadder
     * @param playerPosition        Global position of specific character
     * @return Boolean              Returns true if character enters tile type Ladder            
     */
    public Boolean CheckSpikes(Vector2 playerPosition) {
        if ((GetPlayerPositionTileType(playerPosition, (TileMap) GetParent().GetNode("walls")) == "Spikes")) {
            return true;
        }
        return false;
    }

    /** CheckRotator
     * @param playerPosition
     * @return Boolean
     */
    public Boolean CheckRotator(Vector2 playerPosition) {
        TileMap tm = (TileMap) GetParent().GetNode("walls");
        if (GetPlayerPositionTileType(playerPosition, tm, 0, 1) == "Rotator00" ||
            GetPlayerPositionTileType(playerPosition, tm, 0, 1) == "Rotator01" ||
            GetPlayerPositionTileType(playerPosition, tm, 0, 1) == "Rotator02" ||
            GetPlayerPositionTileType(playerPosition, tm, 0, 1) == "Rotator03") {
            return true;
        } else if (GetPlayerPositionTileType(playerPosition, tm, 1, 1) == "Rotator00" ||
            GetPlayerPositionTileType(playerPosition, tm, -1, 1) == "Rotator03") { //QUICKFIX for edge of hitbox bug
            return true;
        }
        return false;
    }

    /** PlayerDies
     *  @param reason
     */
    public void PlayerDies(String reason) {
        Timer deathTimer = GetNode<Timer>("DeathTimer");
        deathTimer.Start();
        CollisionShape2D collisionShape2D = (CollisionShape2D) GetNode("CollisionShape2D");
        isDead = true;
        lifeCount -= 1;
        collisionShape2D.Disabled = true;
        velocity = new Vector2(0, 0);
        if (reason == "hit") {
            playerSprite.Play("dead");
        } else if (reason == "fall") {
            playerSprite.Play("splash");
        }
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
    private void _OnDeathTimerTimeout() {
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

    private void _OnJumpTimerTimeout() {
        justJumped = false;
    }

    public override void _PhysicsProcess(float delta) {
        playerSprite = (AnimatedSprite) GetNode("PlayerSprite");
        playerSounds = (AudioStreamPlayer2D) GetNode("jump");
        playerPosition = playerSprite.GlobalPosition;
        playerHitbox = GetNode<CollisionShape2D>("CollisionShape2D");

        if (!isDead) {

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

            //Basic controls + LadderControls
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
                if(CheckLadder()) MoveAndSlide(velocity);
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
                if(CheckLadder()) MoveAndSlide(velocity);
            } else {
                velocity.x = 0 - rotator;
                if (direction == -1) {
                    playerSprite.Play("idle"); //Co tu miało być ????
                } else {
                    playerSprite.Play("idle");
                }
            }
            //Ladder up controls
            if (Input.IsActionPressed("ui_up") && CheckLadder()) {
                if(velocity.y > 0) { velocity.y = 0; }
                velocity.y -= CLIMB_SPEED;
                MoveAndSlide(velocity);
            }
            //Ladder down controls
            if (Input.IsActionPressed("ui_down") && CheckLadder()) {
                GD.Print(velocity);
                if(velocity.y < 0) { velocity.y = 0; }
                velocity.y += CLIMB_SPEED;
                MoveAndSlide(velocity);
                GD.Print(velocity);
            }
            //Allows to drop from one-side collision ledges
            if (Input.IsActionJustPressed("ui_down") && onGround) {
                Vector2 drop = new Vector2(playerPosition.x, playerPosition.y + 1);
                this.SetPosition(drop);
            }

            //Jump controls
            if (Input.IsActionPressed("ui_up") && onGround && !justJumped && !CheckLadder()) {
                velocity.y = JUMP;
                playerSounds.Play();
            } 
            //Ladder up controls

            if (velocity.y < 0) {
                // jumping = true; ? not needed so far, maybe later
                playerSprite.Play("jump");
            }
            if (velocity.y > 0) {
                // jumping = false; translation - falling
            }

            CheckFallDeath();
            if (velocity.y == 0 && prevFrameVelocityY >= 150) {
                if (!hasFallen) {
                    Timer timer = GetNode<Timer>("JumpTimer");
                    playerSprite.Play("recover");
                    timer.Start();
                } else {
                    PlayerDies("fall");
                }
            }
            //Death Handing
            if (hasFatallyCollided || (CheckSpikes(playerPosition) && onGround)) {
                PlayerDies();
            }
            prevFrameVelocityY = velocity.y;
            if (CheckLadder() && !onGround) {
                onLadder = true;
            } else {
                onLadder = false;
            }
            onGround = IsOnFloor();
            // GD.Print(CheckLadder());
            //STANDARD GRAV HANDLING
            if (!onLadder) {
                velocity.y += GRAV;
                velocity = MoveAndSlide(velocity, FLOOR);
            }
            TileMap tm = GetParent().GetNode<TileMap>("walls"); 
            GD.Print(playerPosition + "     " + tm.WorldToMap(playerSprite.GlobalPosition));
        } //*  END OF IF ISDEAD LOOP
    }
}