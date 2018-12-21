using System;
using Godot;

public class EnemyFourMain : KinematicBody2D {
    public int SPEED = 10; //[Export] public int SPEED = 40; [Export] allows changing variable in-editor for test puproses
    public int GRAV = 10;
    public int JUMP = -200;
    Vector2 FLOOR = new Vector2(0, -1);
    Vector2 velocity = new Vector2();
    int directionx = 1;
    int directiony = 0;
    int targetAquired = 1;
    AnimatedSprite enemySprite;
    Vector2 enemyPosition;
    bool canMoveUp;
    bool canMoveDown;
    bool movingHorizontally = true;
    bool movingVertically = false;

    /** GetPlayerPositionTileType
     * @param playerPosition        Global position of specific character
     * @param tm                    Specific tileset
     * @param x                     x variable of Vector2(x, y) allows returning different tiles than the one character is currently at
     * @param y                     y variable of Vector2(x, y) allows returning different tiles than the one character is currently at
     * @return String          Returns either name of a currently chosen tile or keyword Empty
     */
    public String GetPlayerPositionTileType(Vector2 playerPosition, TileMap tm, int x, int y) {
        var worldPos = tm.WorldToMap(enemySprite.GlobalPosition);
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
        if ((GetPlayerPositionTileType(playerPosition, (TileMap) GetParent().GetNode("walls")) == "Ladder") ||
            (GetPlayerPositionTileType(playerPosition, (TileMap) GetParent().GetNode("walls"), 0, 1) == "Ladder")) {
            return true;
        }
        return false;
    }

    public Boolean CheckLadderAt(Vector2 playerPosition) {
        if ((GetPlayerPositionTileType(playerPosition, (TileMap) GetParent().GetNode("walls")) == "Ladder")) {
            return true;
        }
        return false;
    }
    public Boolean CheckLadderUnder(Vector2 playerPosition) {
        if ((GetPlayerPositionTileType(playerPosition, (TileMap) GetParent().GetNode("walls"), 0, 1) == "Ladder")) {
            return true;
        }
        return false;
    }

    private void ChangeDir(params string[] args) {
        Random r = new Random();
        int rInt = r.Next(0, args.Length);
        if ((String) args.GetValue(rInt) == "up") {
            directionx *= 0;
            directiony = -1;
        } else if ((String) args.GetValue(rInt) == "down") {
            directionx *= 0;
            directiony = 1;
        } else if ((String) args.GetValue(rInt) == "left") {
            directionx *= -1;
            directiony = 0;
        } else if ((String) args.GetValue(rInt) == "right") {
            directionx *= 1;
            directiony = 0;
        }
    }

    public override void _PhysicsProcess(float delta) {
        AnimatedSprite enemySprite = GetNode<AnimatedSprite>("EnemySprite");
        AnimatedSprite reactionSprite = GetNode<AnimatedSprite>("ReactionSprite");
        Node player = GetParent().GetNode("Player");
        Area2D hitbox = GetNode<Area2D>("Hitbox");
        Area2D aggro = GetNode<Area2D>("Aggro");
        enemyPosition = enemySprite.GlobalPosition;
        CollisionShape2D aggroRange = aggro.GetNode<CollisionShape2D>("AggroRange");

        enemySprite.Play("walk");
        // velocity.y += GRAV; but lets make him float
        velocity.x = SPEED * directionx * targetAquired;
        velocity.y = SPEED * directiony * targetAquired;
        velocity = MoveAndSlide(velocity, FLOOR);
        if (IsOnWall() && !CheckLadder(enemyPosition)) { //CheckLadder = true and IsOnWall() = true most likely cant ever happen
            directionx *= -1;
            directiony = 0;
            movingHorizontally = true;
            aggroRange.SetPosition(aggroRange.GetPosition() * (-1));
        }
        if (movingHorizontally && CheckLadder(enemyPosition) && !IsOnWall()) {
            ChangeDir("down", "left", "right", "up");
        } else if (movingVertically && CheckLadderUnder(enemyPosition) && !CheckLadderAt(enemyPosition)) {
            ChangeDir("down", "left", "right");
        } else if (IsOnFloor() && CheckLadderAt(enemyPosition)) {
            ChangeDir("up", "left", "right");
        }

        if (directionx == -1) {
            enemySprite.FlipH = true;
            aggroRange.SetRotationDegrees(270);
        } else if (directionx == 1) {
            enemySprite.FlipH = false;
            aggroRange.SetRotationDegrees(90);
        } else if (directiony == -1) {
            enemySprite.FlipH = false;
            aggroRange.SetRotationDegrees(180);
        } else if (directiony == 1) {
            enemySprite.FlipH = false;
            aggroRange.SetRotationDegrees(0);
        }

        // NO AGGRO FOR NOW
        // if (aggro.OverlapsBody(player)) {
        //     reactionSprite.Play("aggroON");
        //     GD.Print("Player in aggro range.");
        //     targetAquired = 2;
        // } else if (targetAquired == 2 && !aggro.OverlapsBody(player)) {
        //     reactionSprite.Play("aggroOFF");
        //     GD.Print("Target lost.");
        //     targetAquired = 1;
        // }
        if (hitbox.OverlapsBody(player)) {
            player.Set("hasFatallyCollided", true);
        }

    }
}