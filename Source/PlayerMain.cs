using System;
using Godot;

public class PlayerMain : KinematicBody2D {
    [Export] public int SPEED = 40; //
    [Export] public int CLIMB_SPEED = 40;
    public int GRAV = 10;
    [Export] public int JUMP = -180;
    public int ROTATOR_SPEED = -20;
    [Export] public int PLAYER_FALL_DEATH = 2000;
    public int lifeCount;
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
    // [Export] public bool hasKey = false;
    public int keyCount = 0;
    public int diaxCount = 0;
    public bool hasFallen = false;
    public bool isDead = false;
    public Vector2 currentCheckpoint;
    private int tmpKey;
    private int tmpDiax;

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
            worldPos.x = worldPos.x + 3; // ! ONLY FOR STAGE ONE
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
        if ((GetPlayerPositionTileType(playerPosition, (TileMap) GetParent().GetNode("walls")) == "NewLadder") ||
            (GetPlayerPositionTileType(playerPosition, (TileMap) GetParent().GetNode("walls"), 0, 1) == "NewLadder") ||
            (GetPlayerPositionTileType(playerPosition, (TileMap) GetParent().GetNode("walls")) == "LadderTop")) {
            return true;
        }
        return false;
    }

    public Boolean CheckLadder() {
        return CheckLadder(playerPosition);
    }

    public Boolean CheckHook() => CheckHook(playerPosition);

    private bool CheckHook(Vector2 playerPosition) {
        if ((GetPlayerPositionTileType(playerPosition, (TileMap) GetParent().GetNode("walls")) == "HookActive") ||
            (GetPlayerPositionTileType(playerPosition, (TileMap) GetParent().GetNode("walls")) == "ThinChainActive")) {
            return true;
        }
        return false;
    }

    public Boolean CheckLadderUnder(Vector2 playerPosition) {
        if ((GetPlayerPositionTileType(playerPosition, (TileMap) GetParent().GetNode("walls"), 0, 1) == "LadderTop")) {
            return true;
        }
        return false;
    }

    public Boolean CheckSlider(Vector2 playerPosition) {
        if ((GetPlayerPositionTileType(playerPosition, (TileMap) GetParent().GetNode("walls")) == "NewSlider") ||
            (GetPlayerPositionTileType(playerPosition, (TileMap) GetParent().GetNode("walls")) == "SliderTop01") ||
            (GetPlayerPositionTileType(playerPosition, (TileMap) GetParent().GetNode("walls")) == "SliderTop02") ||
            (GetPlayerPositionTileType(playerPosition, (TileMap) GetParent().GetNode("walls")) == "SliderBot01")) {
            return true;
        }
        return false;
    }

    public Boolean CheckSlider() {
        return CheckSlider(playerPosition);
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

    public void CheckPickUp() {
        if(tmpKey != keyCount || tmpDiax != diaxCount) {
            AudioStreamPlayer2D asp2d = this.GetNode<AudioStreamPlayer2D>("pickup");
            asp2d.Play();
        }
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
    private Boolean CheckFallDeath() {
        if (velocity.y > PLAYER_FALL_DEATH && !onLadder) {
            hasFallen = true;
        }
        return hasFallen;
    }

    /** GetPlayerPositionTileType
     *  @param 
     *  @return
     * TODO: Add Animation, make timer longer
     */
    private void _OnDeathTimerTimeout() {
        isDead = false;
        hasFatallyCollided = false;
        hasFallen = false;
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

            //Basic controls + LadderControls
            int rotator = 0; //pretty sure this can be done way clearer UGLY AF CLEAN THIS SHIT UP
            if (CheckRotator(playerPosition) && onGround) {
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
                if ((CheckHook() || CheckLadder()) && !onGround) {
                    if(!(Input.IsActionPressed("ui_up") || Input.IsActionPressed("ui_down"))) velocity.y = 0;
                    MoveAndSlide(velocity);
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
                if ((CheckHook() || CheckLadder()) && !onGround) {
                    if(!(Input.IsActionPressed("ui_up") || Input.IsActionPressed("ui_down"))) velocity.y = 0;
                    MoveAndSlide(velocity);
                }
            } else {
                velocity.x = 0 - rotator;
                if (direction == -1) {
                    playerSprite.Play("idle"); //Co tu miało być ????
                } else {
                    playerSprite.Play("idle");
                }
            }
            //Ladder up controls
            if (Input.IsActionPressed("ui_up") && ((CheckLadder() || CheckHook()) || (!onGround && CheckLadderUnder(playerPosition)))) {
                if (velocity.y > 0) { velocity.y = 0; }
                velocity.y = -CLIMB_SPEED;
                MoveAndSlide(velocity);
                playerSprite.Play("climb");
            }
            // TODO PARTIAL FIX, BREAKS MUCH MORE THAN IT SOLVES FML - PREFER TO HAVE A BUG THAN THIS FIX
            // if (!Input.IsActionPressed("ui_up") && CheckLadder() && 
            //     (!((GetPlayerPositionTileType(playerPosition, (TileMap) GetParent().GetNode("walls"), 0, 1) == "NewLadder"))) {
            //     velocity.y += GRAV;
            //     velocity = MoveAndSlide(velocity, FLOOR);
            // } 

            //Ladder down controls
            if (Input.IsActionPressed("ui_down") && (CheckLadder() || CheckHook()) && !(onGround && CheckRotator(playerPosition))) {
                if (velocity.y < 0) { velocity.y = 0; }
                velocity.y = CLIMB_SPEED;
                MoveAndSlide(velocity);
                playerSprite.Play("climb");
            }

            // ? Ladder jumpbug fix 
            // if(velocity.y < 0 && !(GetPlayerPositionTileType(playerPosition, (TileMap) GetParent().GetNode("walls")) == "NewLadder") &&
            // (GetPlayerPositionTileType(playerPosition, (TileMap) GetParent().GetNode("walls"), 0, 1) == "NewLadder")) {
            //     velocity.y = 0;
            // }
            // if(CheckHook()){
            //     velocity.y = -5;
            // }

            if (CheckSlider() && !onGround) {
                velocity.y = -5;
                velocity.y += CLIMB_SPEED;
                playerSprite.Play("slide");
                MoveAndSlide(velocity);
            }
            //Allows to drop from one-side collision ledges
            if (Input.IsActionJustPressed("ui_down") && onGround) {
                Vector2 drop = new Vector2(playerPosition.x, playerPosition.y + 1);
                this.SetPosition(drop);
            }

            //Jump controls
            if (Input.IsActionPressed("ui_jump") && onGround && !justJumped) {
                velocity.y = JUMP;
                playerSounds.Play();
            }

            if (velocity.y < 0) {
                // jumping = true; ? not needed so far, maybe later
                playerSprite.Play("jump");
            }
            if (velocity.y > 0) {
                // jumping = false; translation - falling
            }

            
            //Death Handing
            if (hasFatallyCollided || (CheckSpikes(playerPosition) && onGround) || CheckFallDeath()) {
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
            if (!onLadder && !CheckSlider() && !CheckHook()) {
                velocity.y += GRAV;
                velocity = MoveAndSlide(velocity, FLOOR);
            }
            CheckPickUp();
            tmpKey = keyCount;
            tmpDiax = diaxCount;
            // GD.Print(diaxCount);
        } //*  END OF IF ISDEAD LOOP
    }
}