using Godot;
using System;

public class GameplayLoop : Node
{
    // Member variables here, example:
    // private int a = 2;
    // private string b = "textvar";

    // public override void _Ready()
    // {
    //     // Called every time the node is added to the scene.
    //     // Initialization here
        
    // }
    public bool playerOnLadder;
    PlayerMain pm = new PlayerMain();


   public override void _Process(float delta)
   {
     //Checking if there's a Ladder at playerPos TODO export to method
     // Vector2 playerPosition = 
     // TileMap tm = (TileMap) GetNode("walls");
     // Node player = GetNode("Player");
     // AnimatedSprite playerSprite = (AnimatedSprite) player.GetNode("PlayerSprite");
     // Vector2 playerPos = playerSprite.GetGlobalPosition();
     // Vector2 mapPos = tm.WorldToMap(playerPos);
     // int tilePos = tm.GetCellv(mapPos);
     // GD.Print(playerPos + "  " + mapPos + "  " + tilePos);
     // if (tilePos > -1) {
     //      if (tm.GetTileset().TileGetName(tilePos) == "Ladder") {
     //           playerOnLadder = true;
     //      } else {
     //                 playerOnLadder = false;
     //      }
     // } else {
     //      playerOnLadder = false;
     // }
   }
}
