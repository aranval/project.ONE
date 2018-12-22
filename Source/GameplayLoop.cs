using System;
using Godot;

public class GameplayLoop : Node {
  TextureButton cont;
  // Member variables here, example:
  // private int a = 2;
  // private string b = "textvar";

  // public override void _Ready() { }

  public override void _PhysicsProcess(float delta) {
    if (Input.IsActionPressed("ui_pause")) {
      GetTree().Paused = true;
      GetNode<Control>("InGameMenu").Show();
      cont = (TextureButton) GetNode<Control>("InGameMenu").GetChild(0).GetChild(0).GetNode("Continue");
      cont.GrabFocus();
      GetNode<AnimatedSprite>("EnemyTwo2").FlipH = true;; 
    }
  }
}