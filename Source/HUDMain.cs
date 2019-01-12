using System;
using Godot;

public class HUDMain : Node {
    public int gui_health = 4;

    public override void _Ready() {
        drawHealthBar();

    }

    void drawTile(TileMap tm, TileSet ts, Vector2 vector2, String tileName, bool flipy, bool fliph) {
        tm.SetCellv(vector2, ts.FindTileByName(tileName), flipy, fliph);
    }
    void drawHealthBar() {
        Vector2 vec;
        TileMap tm = GetNode<TileMap>("TileMap");
        TileSet ts = tm.GetTileset();
        vec = new Vector2(1, 1);
        drawTile(tm, ts, vec, "Life", false, false);
        vec = new Vector2(0, 1);
        drawTile(tm, ts, vec, "Life", true, false);
        vec = new Vector2(1, 0);
        drawTile(tm, ts, vec, "Life", false, true);
        vec = new Vector2(0, 0);
        drawTile(tm, ts, vec, "Life", true, true);
    }

    void updateHealth() {
        Vector2 vec;
        TileMap tm = GetNode<TileMap>("TileMap");
        TileSet ts = tm.GetTileset();
        if (gui_health == 3) {
            vec = new Vector2(1, 1);
            drawTile(tm, ts, vec, "LifeLost", false, false);
        } else if (gui_health == 2) {
            vec = new Vector2(0, 1);
            drawTile(tm, ts, vec, "LifeLost", true, false);
        } else if (gui_health == 1) {
            vec = new Vector2(1, 0);
            drawTile(tm, ts, vec, "LifeLost", false, true);
        } else if (gui_health == 0) {
            vec = new Vector2(0, 0);
            drawTile(tm, ts, vec, "LifeLost", true, true);
        }
    }

    public override void _Process(float delta) {
        updateHealth();
    }
}