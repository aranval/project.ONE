using System;
using Godot;

public class EnemyOneMain : KinematicBody2D {
    public int SPEED = 10; //[Export] public int SPEED = 40; [Export] allows changing variable in-editor for test puproses
    public int GRAV = 10;
    public int JUMP = -200;
    Vector2 FLOOR = new Vector2(0, -1);
    Vector2 velocity = new Vector2();
    int direction = 1;
    int targetAquired = 1;

    // public override void _Ready()
    // {
    //     // Called every time the node is added to the scene.
    //     // Initialization here

    // }

    public override void _Process(float delta) {
        AnimatedSprite enemySprite = GetNode<AnimatedSprite>("EnemySprite");
        AnimatedSprite reactionSprite = GetNode<AnimatedSprite>("ReactionSprite");
        Node player = GetParent().GetNode("Player");
        Area2D hitbox = GetNode<Area2D>("Hitbox");
        Area2D aggro = GetNode<Area2D>("Aggro");
        CollisionShape2D aggroRange = aggro.GetNode<CollisionShape2D>("AggroRange");

        enemySprite.Play("walk");
        // velocity.y += GRAV; but lets make him float
        velocity.x = SPEED * direction * targetAquired;
        velocity = MoveAndSlide(velocity, FLOOR);
        if (IsOnWall()) {
            direction *= -1;
            aggroRange.SetPosition(aggroRange.GetPosition()*(-1));
        }
        //direction == -1 ? enemySprite.FlipH = true : enemySprite.FlipH = false; WEŹ MI WYTŁUMACZ CZEMU TO KRUWA NIE DZIAŁA
        if (direction == -1) {
            enemySprite.FlipH = true;
            aggroRange.SetRotationDegrees(270);
        } else {
            enemySprite.FlipH = false;
            aggroRange.SetRotationDegrees(90);
        }

        if(aggro.OverlapsBody(player)) {
            reactionSprite.Play("aggroON");
            GD.Print("Player in aggro range.");
            targetAquired = 2;
        } else if(targetAquired == 2 && !aggro.OverlapsBody(player)) {
                reactionSprite.Play("aggroOFF");
                GD.Print("Target lost.");
                targetAquired = 1;
        }
        if (hitbox.OverlapsBody(player)) {
            player.Set("hasFatallyCollided", true);
        }

    }
}