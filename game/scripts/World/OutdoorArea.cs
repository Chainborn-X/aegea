using Godot;
using System.Collections.Generic;

namespace Aegea;

public partial class OutdoorArea : Node2D
{
    private const int TileSourceId = 0;
    private const int TileSize = 32;
    private const string CoastTilesetPath = "res://assets/tilesets/aegea_coast_tileset.png";

    private static readonly Vector2I[] SandTiles =
    [
        new(0, 0), new(1, 0), new(2, 0), new(3, 0),
        new(4, 0), new(5, 0), new(6, 0), new(7, 0)
    ];

    private static readonly Vector2I[] GrassBlendTiles =
    [
        new(0, 1), new(1, 1), new(2, 1), new(3, 1),
        new(4, 1), new(5, 1), new(6, 1), new(7, 1)
    ];

    private static readonly Vector2I[] ShallowWaterTiles = [new(0, 2), new(1, 2), new(2, 2), new(3, 2)];
    private static readonly Vector2I[] DeepWaterTiles = [new(4, 2), new(5, 2), new(6, 2), new(7, 2)];
    private static readonly Vector2I[] StoneTiles =
    [
        new(0, 4), new(1, 4), new(2, 4), new(3, 4),
        new(4, 4), new(5, 4), new(6, 4), new(7, 4)
    ];

    private static readonly Vector2I[] RockTiles =
    [
        new(0, 5), new(1, 5), new(2, 5), new(3, 5),
        new(4, 5), new(5, 5), new(6, 5), new(7, 5)
    ];

    private static readonly Vector2I[] VegetationTiles =
    [
        new(0, 6), new(1, 6), new(2, 6), new(3, 6),
        new(4, 6), new(5, 6), new(6, 6), new(7, 6)
    ];

    private static readonly Vector2I[] PropTiles =
    [
        new(0, 7), new(1, 7), new(2, 7), new(3, 7),
        new(4, 7), new(5, 7), new(6, 7), new(7, 7)
    ];

    private readonly List<(Vector2 Position, Vector2 Size)> _walls = new()
    {
        (new Vector2(512, -56), new Vector2(1024, 80)),
        (new Vector2(512, 776), new Vector2(1024, 80)),
        (new Vector2(-56, 360), new Vector2(80, 720)),
        (new Vector2(1080, 360), new Vector2(80, 720)),
        (new Vector2(770, 120), new Vector2(260, 44)),
        (new Vector2(880, 230), new Vector2(44, 220)),
        (new Vector2(194, 505), new Vector2(230, 32)),
        (new Vector2(372, 563), new Vector2(32, 144)),
        (new Vector2(620, 595), new Vector2(310, 36))
    };

    private readonly List<(Vector2 Position, float Radius)> _rocks = new()
    {
        (new Vector2(280, 190), 22),
        (new Vector2(320, 214), 16),
        (new Vector2(724, 472), 20),
        (new Vector2(760, 494), 15),
        (new Vector2(912, 570), 23)
    };

    public override void _Ready()
    {
        BuildCoastalTileMaps();

        foreach ((Vector2 position, Vector2 size) in _walls)
        {
            AddStaticRectangle(position, size);
        }

        foreach ((Vector2 position, float radius) in _rocks)
        {
            var body = new StaticBody2D { Position = position };
            body.AddChild(new CollisionShape2D { Shape = new CircleShape2D { Radius = radius } });
            AddChild(body);
        }
    }

    public override void _Draw()
    {
        DrawTrees();
    }

    private void BuildCoastalTileMaps()
    {
        TileSet tileSet = CreateCoastTileSet();

        var ground = new TileMapLayer
        {
            Name = "CoastalGround",
            TileSet = tileSet,
            ZIndex = -20
        };
        AddChild(ground);
        PaintGround(ground);

        var details = new TileMapLayer
        {
            Name = "CoastalDetails",
            TileSet = tileSet,
            ZIndex = -10
        };
        AddChild(details);
        PaintDetails(details);
    }

    private static TileSet CreateCoastTileSet()
    {
        var tileSet = new TileSet { TileSize = new Vector2I(TileSize, TileSize) };
        Texture2D texture = ResourceLoader.Exists(CoastTilesetPath)
            ? GD.Load<Texture2D>(CoastTilesetPath)
            : ImageTexture.CreateFromImage(Image.LoadFromFile(CoastTilesetPath));
        var atlas = new TileSetAtlasSource
        {
            Texture = texture,
            TextureRegionSize = new Vector2I(TileSize, TileSize)
        };

        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                atlas.CreateTile(new Vector2I(x, y));
            }
        }

        tileSet.AddSource(atlas, TileSourceId);
        return tileSet;
    }

    private static void PaintGround(TileMapLayer layer)
    {
        for (int y = -4; y <= 27; y++)
        {
            for (int x = -4; x <= 35; x++)
            {
                Vector2I cell = new(x, y);
                layer.SetCell(cell, TileSourceId, PickBaseTile(x, y));
            }
        }

        PaintStonePatch(layer, new Rect2I(22, 3, 8, 4));
        PaintStonePatch(layer, new Rect2I(23, 1, 6, 2));
        PaintStonePath(layer, new Vector2I(25, 5), new Vector2I(16, 11), new Vector2I(10, 18));
    }

    private static Vector2I PickBaseTile(int x, int y)
    {
        if (y >= 22)
        {
            return Pick(DeepWaterTiles, x, y);
        }

        if (y == 21)
        {
            return Pick(ShallowWaterTiles, x, y);
        }

        if (y == 20)
        {
            return new Vector2I(1, 3);
        }

        if (y >= 15)
        {
            return Pick(SandTiles, x, y);
        }

        if (y >= 10)
        {
            return Pick(GrassBlendTiles, x, y);
        }

        return Pick(GrassBlendTiles, x + 13, y + 7);
    }

    private static void PaintDetails(TileMapLayer layer)
    {
        PaintRockBoundaries(layer);
        PaintRockClusters(layer);
        PaintBeachProps(layer);
        PaintVegetation(layer);
    }

    private static void PaintRockBoundaries(TileMapLayer layer)
    {
        PaintTileRect(layer, new Rect2I(-4, -3, 40, 3), RockTiles);
        PaintTileRect(layer, new Rect2I(-4, 24, 40, 4), RockTiles);
        PaintTileRect(layer, new Rect2I(-4, -1, 3, 25), RockTiles);
        PaintTileRect(layer, new Rect2I(32, -1, 4, 25), RockTiles);
        PaintTileRect(layer, new Rect2I(23, 3, 7, 2), RockTiles);
        PaintTileRect(layer, new Rect2I(27, 6, 2, 8), RockTiles);
        PaintTileRect(layer, new Rect2I(3, 15, 7, 2), RockTiles);
        PaintTileRect(layer, new Rect2I(10, 17, 2, 5), RockTiles);
        PaintTileRect(layer, new Rect2I(17, 18, 10, 2), RockTiles);
    }

    private static void PaintRockClusters(TileMapLayer layer)
    {
        Vector2I[] clusters =
        [
            new(8, 6), new(9, 6), new(22, 14), new(23, 15), new(28, 17),
            new(5, 19), new(6, 20), new(30, 20)
        ];

        foreach (Vector2I cell in clusters)
        {
            layer.SetCell(cell, TileSourceId, Pick(RockTiles, cell.X, cell.Y));
        }
    }

    private static void PaintBeachProps(TileMapLayer layer)
    {
        (Vector2I Cell, Vector2I Tile)[] props =
        [
            (new Vector2I(4, 18), PropTiles[0]),
            (new Vector2I(7, 20), PropTiles[1]),
            (new Vector2I(13, 16), PropTiles[2]),
            (new Vector2I(26, 5), PropTiles[3]),
            (new Vector2I(26, 2), PropTiles[4]),
            (new Vector2I(3, 19), PropTiles[5]),
            (new Vector2I(19, 17), PropTiles[6]),
            (new Vector2I(15, 20), PropTiles[7])
        ];

        foreach ((Vector2I cell, Vector2I tile) in props)
        {
            layer.SetCell(cell, TileSourceId, tile);
        }
    }

    private static void PaintVegetation(TileMapLayer layer)
    {
        Vector2I[] vegetation =
        [
            new(3, 3), new(5, 4), new(7, 3), new(4, 12), new(5, 13),
            new(17, 7), new(18, 8), new(20, 7), new(30, 9), new(31, 12),
            new(24, 7), new(22, 8), new(29, 13), new(12, 12), new(13, 13),
            new(18, 14), new(20, 15), new(27, 15)
        ];

        foreach (Vector2I cell in vegetation)
        {
            layer.SetCell(cell, TileSourceId, Pick(VegetationTiles, cell.X, cell.Y));
        }
    }

    private static void PaintStonePatch(TileMapLayer layer, Rect2I rect)
    {
        PaintTileRect(layer, rect, StoneTiles);
    }

    private static void PaintStonePath(TileMapLayer layer, params Vector2I[] points)
    {
        for (int i = 0; i < points.Length - 1; i++)
        {
            PaintStoneLine(layer, points[i], points[i + 1]);
        }
    }

    private static void PaintStoneLine(TileMapLayer layer, Vector2I start, Vector2I end)
    {
        int steps = Mathf.Max(Mathf.Abs(end.X - start.X), Mathf.Abs(end.Y - start.Y));
        for (int i = 0; i <= steps; i++)
        {
            float t = steps == 0 ? 0f : i / (float)steps;
            int x = Mathf.RoundToInt(Mathf.Lerp(start.X, end.X, t));
            int y = Mathf.RoundToInt(Mathf.Lerp(start.Y, end.Y, t));

            for (int offset = -1; offset <= 1; offset++)
            {
                Vector2I cell = new(x + offset, y);
                layer.SetCell(cell, TileSourceId, Pick(StoneTiles, cell.X, cell.Y));
            }
        }
    }

    private static void PaintTileRect(TileMapLayer layer, Rect2I rect, Vector2I[] tiles)
    {
        for (int y = rect.Position.Y; y < rect.End.Y; y++)
        {
            for (int x = rect.Position.X; x < rect.End.X; x++)
            {
                layer.SetCell(new Vector2I(x, y), TileSourceId, Pick(tiles, x, y));
            }
        }
    }

    private static Vector2I Pick(Vector2I[] tiles, int x, int y)
    {
        int hash = Mathf.Abs((x * 73856093) ^ (y * 19349663));
        return tiles[hash % tiles.Length];
    }

    private void AddStaticRectangle(Vector2 position, Vector2 size)
    {
        var body = new StaticBody2D { Position = position };
        body.AddChild(new CollisionShape2D { Shape = new RectangleShape2D { Size = size } });
        AddChild(body);
    }

    private void DrawTrees()
    {
        Vector2[] trees =
        [
            new Vector2(100, 95),
            new Vector2(170, 70),
            new Vector2(250, 82),
            new Vector2(120, 420),
            new Vector2(945, 340),
            new Vector2(988, 415)
        ];

        foreach (Vector2 tree in trees)
        {
            DrawLine(tree + new Vector2(0, 18), tree + new Vector2(0, -8), new Color(0.33f, 0.22f, 0.13f), 6f);
            DrawCircle(tree + new Vector2(-7, -14), 18, new Color(0.22f, 0.44f, 0.25f));
            DrawCircle(tree + new Vector2(10, -17), 17, new Color(0.28f, 0.5f, 0.29f));
            DrawCircle(tree + new Vector2(0, -30), 16, new Color(0.39f, 0.58f, 0.32f));
        }
    }
}
