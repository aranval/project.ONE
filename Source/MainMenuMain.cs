    using System;
    using Godot;

    public class MainMenuMain : Node {
        // Member variables here, example:
        // private int a = 2;
        // private string b = "textvar";
        TextureButton start;
        TextureButton cont;
        TextureButton quit;

        public override void _Ready() {
            start = (TextureButton) GetChild(0).GetChild(1).GetChild(0).GetChild(1).GetNode("Start");
            start.GrabFocus();
        }

        public void GUIControls() {
            cont = (TextureButton) GetChild(0).GetChild(1).GetChild(0).GetChild(1).GetNode("Continue");
            start = (TextureButton) GetChild(0).GetChild(1).GetChild(0).GetChild(1).GetNode("Start");
            quit = (TextureButton) GetChild(0).GetChild(1).GetChild(0).GetChild(1).GetNode("Quit");

            if (start.IsHovered()) {
                start.GrabFocus();
            }

            if (quit.IsHovered()) {
                quit.GrabFocus();
            }
        }

        public void HUDUpdate() {
            if (this.GetChildCount() > 1) {
                Node player = this.GetChild(1).GetNode("Player");
                int lifeCount = (int) player.Get("lifeCount");
                int keyCount = (int) player.Get("keyCount");
                int diaxCount = (int) player.Get("diaxCount");
                Node gui = this.GetChild(0).GetNode("GUI");
                gui.Set("gui_health", lifeCount);
            }
        }

        public override void _PhysicsProcess(float delta) {
            GUIControls();
            HUDUpdate();
        }

        public void _OnContinuePressed() {
            GD.Print("Continue");
        }

        public void _OnStartPressed() {
            this.GetNode("HUD").GetNode<CanvasItem>("MarginContainer").Visible = false;
            GD.Print("Start Pressed!");
            var gameScene = (PackedScene)ResourceLoader.Load("res://Scenes/StageOne.tscn");
            var game = gameScene.Instance();
            this.AddChild(game);
            // GetTree().ChangeScene("Scenes\\StageOne.tscn");
        }
        public void _OnQuitPressed() {
            GetTree().Quit();
        }
    }