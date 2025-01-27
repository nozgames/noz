/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/


using System.Numerics;

namespace NoZ.Graphics
{
    public class SpriteAtlas : Resource<SpriteAtlas>
    {
        private SpriteAtlasSerializer.Sprite[]? _sprites;
        private Dictionary<string, int>? _spriteNameMap;
        private Texture? _texture;
        
        public Texture Texture => _texture!;
        
        internal SpriteAtlas(SpriteAtlasSerializer.SpriteAtlas json, Texture texture)
        {
            _sprites = json.Sprites;
            _spriteNameMap = new Dictionary<string, int>(_sprites.Length, StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < _sprites.Length; i++)
                _spriteNameMap[_sprites[i].Name] = i;
            
            _texture = texture;
        }

        public Sprite LoadSprite(string name)
        {
            var spritePath = Path + "/" + name;
            if (Resource<Sprite>.TryGet(spritePath, out var sprite))
                return sprite!;

            if (!_spriteNameMap!.TryGetValue(name, out var spriteIndex))
                throw new System.IO.FileNotFoundException(name);

            ref var spriteDef = ref _sprites![spriteIndex];

            sprite = new Sprite(
                Texture,
                new Rect(spriteDef.X, spriteDef.Y, spriteDef.Width, spriteDef.Height),
                new Vector2(spriteDef.PivotX, spriteDef.PivotY));

            return Resource<Sprite>.Register(spritePath, sprite);
        }

        protected internal override void Unload()
        {
            _texture?.Dispose();
            _texture = null;
            _sprites = null;
            _spriteNameMap = null;
        }
    }
}
