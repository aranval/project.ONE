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
            start = (TextureButton) GetChild(1).GetChild(0).GetChild(1).GetNode("Start");
            start.GrabFocus();

        }

        public override void _PhysicsProcess(float delta) {
            cont = (TextureButton) GetChild(1).GetChild(0).GetChild(1).GetNode("Continue");
            start = (TextureButton) GetChild(1).GetChild(0).GetChild(1).GetNode("Start");
            quit = (TextureButton) GetChild(1).GetChild(0).GetChild(1).GetNode("Quit");

            // if (cont.IsHovered() && !cont.IsDisabled()) {
            //     cont.GrabFocus();
            // }

            if (start.IsHovered()) {
                start.GrabFocus();
            }

            if (quit.IsHovered()) {
                quit.GrabFocus();
            }

        }

        public void _OnContinuePressed() {
            GD.Print("Continue");
        }

        public void _OnStartPressed() {
            GetTree().ChangeScene("Scenes\\StageOne.tscn");
        }
        public void _OnQuitPressed() {
            GetTree().Quit();
        }
    }