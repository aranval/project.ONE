    using System;
    using Godot;

    public class InGameMenuMain : Control {
        // Member variables here, example:
        // private int a = 2;
        // private string b = "textvar";
        TextureButton start;
        TextureButton cont;
        TextureButton quit;

        public override void _Ready() {
        }

        public override void _PhysicsProcess(float delta) {
            if (Input.IsActionJustPressed("ui_pause")) {
                this.Hide();
                GetTree().Paused = false;
            }
            cont = (TextureButton) GetChild(0).GetChild(0).GetNode("Continue");
            start = (TextureButton) GetChild(0).GetChild(0).GetNode("Start");
            quit = (TextureButton) GetChild(0).GetChild(0).GetNode("Quit");

            if (cont.IsHovered()) {
                cont.GrabFocus();
            }

            if (start.IsHovered()) {
                start.GrabFocus();
            }

            if (quit.IsHovered()) {
                quit.GrabFocus();
            }

        }

        public void _OnContinuePressed() {
            this.Hide();
            GetTree().Paused = false;
        }

        public void _OnStartPressed() {
            GetTree().Paused = false;
            GetTree().ReloadCurrentScene();
        }
        public void _OnQuitPressed() {
            GetTree().Quit();
        }
    }