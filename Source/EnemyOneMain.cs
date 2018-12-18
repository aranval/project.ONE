using Godot;
using System;

public class EnemyOneMain : KinematicBody2D
{
    public int SPEED = 10; //[Export] public int SPEED = 40; [Export] allows changing variable in-editor for test puproses
    public int GRAV = 10;
    public int JUMP = -200;
    Vector2 FLOOR = new Vector2(0, -1);
    Vector2 velocity = new Vector2();
    int direction = 1;

    // public override void _Ready()
    // {
    //     // Called every time the node is added to the scene.
    //     // Initialization here
        
    // }

   public override void _Process(float delta)
   {
       AnimatedSprite enemySprite = (AnimatedSprite) GetNode("EnemySprite");
       enemySprite.Play("walk");
       velocity.x -= GRAV;
       velocity.x = SPEED*direction;
       velocity =  MoveAndSlide(velocity, FLOOR);
       if(IsOnWall()) {
           direction = direction*(-1);
       }
       //direction == -1 ? enemySprite.FlipH = true : enemySprite.FlipH = false; WEŹ MI WYTŁUMACZ CZEMU TO KRUWA NIE DZIAŁA
       if (direction == -1) {
           enemySprite.FlipH = true;
       } else {
           enemySprite.FlipH = false;
       }

       
   }
}
