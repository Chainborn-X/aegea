using Godot;
using System.Collections.Generic;

namespace Aegea;

public partial class OutdoorArea : Node2D
{
    private const int TileSourceId = 0;
    private const int ChunkSize = 128;
    private const int ChunkColumns = 12;
    private const int ChunkRows = 8;
    private const string CoastChunkTilesetPath = "res://assets/tilesets/aegea_coast_chunk_tileset.png";

    private readonly List<(Vector2 Position, Vector2 Size)> _walls = new()
    {
        (new Vector2(768, -56), new Vector2(1536, 96)),
        (new Vector2(768, 1080), new Vector2(1536, 96)),
        (new Vector2(-56, 512), new Vector2(96, 1024)),
        (new Vector2(1592, 512), new Vector2(96, 1024)),
        (new Vector2(585, 130), new Vector2(350, 95)),
        (new Vector2(900, 218), new Vector2(155, 245)),
        (new Vector2(1190, 185), new Vector2(420, 135)),
        (new Vector2(1285, 485), new Vector2(150, 405)),
        (new Vector2(410, 674), new Vector2(105, 190)),
        (new Vector2(735, 780), new Vector2(280, 105))
    };

    private readonly List<(Vector2 Position, float Radius)> _rocks = new()
    {
        (new Vector2(315, 300), 32),
        (new Vector2(390, 402), 28),
        (new Vector2(492, 590), 22),
        (new Vector2(760, 514), 24),
        (new Vector2(1060, 650), 28),
        (new Vector2(1185, 715), 24)
    };

    public override void _Ready()
    {
        if (GetNodeOrNull<TileMapLayer>("CoastalChunkTileMap") is null)
        {
            BuildCoastalChunkTileMap();
        }

        if (GetNodeOrNull<Node2D>("Collision") is null)
        {
            AddCollisionGeometry();
        }
    }

    private void BuildCoastalChunkTileMap()
    {
        var layer = new TileMapLayer
        {
            Name = "CoastalChunkTileMap",
            TileSet = CreateCoastChunkTileSet(),
            ZIndex = -20
        };
        AddChild(layer);

        for (int y = 0; y < ChunkRows; y++)
        {
            for (int x = 0; x < ChunkColumns; x++)
            {
                layer.SetCell(new Vector2I(x, y), TileSourceId, new Vector2I(x, y));
            }
        }
    }

    private static TileSet CreateCoastChunkTileSet()
    {
        var tileSet = new TileSet { TileSize = new Vector2I(ChunkSize, ChunkSize) };
        Texture2D texture = ResourceLoader.Exists(CoastChunkTilesetPath)
            ? GD.Load<Texture2D>(CoastChunkTilesetPath)
            : ImageTexture.CreateFromImage(Image.LoadFromFile(CoastChunkTilesetPath));

        var atlas = new TileSetAtlasSource
        {
            Texture = texture,
            TextureRegionSize = new Vector2I(ChunkSize, ChunkSize)
        };

        for (int y = 0; y < ChunkRows; y++)
        {
            for (int x = 0; x < ChunkColumns; x++)
            {
                atlas.CreateTile(new Vector2I(x, y));
            }
        }

        tileSet.AddSource(atlas, TileSourceId);
        return tileSet;
    }

    private void AddCollisionGeometry()
    {
        var collision = new Node2D { Name = "Collision" };
        AddChild(collision);

        foreach ((Vector2 position, Vector2 size) in _walls)
        {
            AddStaticRectangle(collision, position, size);
        }

        foreach ((Vector2 position, float radius) in _rocks)
        {
            var body = new StaticBody2D { Position = position };
            body.AddChild(new CollisionShape2D { Shape = new CircleShape2D { Radius = radius } });
            collision.AddChild(body);
        }
    }

    private static void AddStaticRectangle(Node parent, Vector2 position, Vector2 size)
    {
        var body = new StaticBody2D { Position = position };
        body.AddChild(new CollisionShape2D { Shape = new RectangleShape2D { Size = size } });
        parent.AddChild(body);
    }
}
