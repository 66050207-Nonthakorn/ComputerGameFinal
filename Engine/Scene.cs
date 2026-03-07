using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ComputerGameFinal.Engine.Components;

namespace ComputerGameFinal.Engine;

public abstract class Scene
{
    protected Dictionary<string, GameObject> GameObjects { get; } = [];
    private readonly List<string> _deadObjects = [];
    
    public Camera2D Camera { get; set; }

    // For add game objects
    public abstract void Setup();
    
    public void Load()
    {
        this.Setup();

        foreach (var gameObject in GameObjects.Values)
        {
            gameObject.InitializeComponents();
        }

        foreach (var gameObject in GameObjects.Values)
        {
            gameObject.Initialize();
        }
    }

    public void Unload()
    {
        GameObjects.Clear();
    }

    public void Update(GameTime gameTime)
    {
        foreach (var gameObject in GameObjects.Values)
        {
            gameObject.UpdateComponents(gameTime);
        }

        foreach (var gameObject in GameObjects.Values)
        {
            gameObject.Update(gameTime);
        }

        foreach (var name in _deadObjects)
        {
            GameObjects.Remove(name);
        }

        // Physics and collision handling update

        _deadObjects.Clear();
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var gameObject in GameObjects.Values)
        {
            gameObject.DrawComponents(spriteBatch);
        }
    }
    
    public Matrix GetCameraTransform()
    {
        return Camera?.TransformMatrix ?? Matrix.Identity;
    }

    public T AddGameObject<T>(string name) where T : GameObject, new() 
    {
        T gameObject = new T();
        GameObjects.Add(name, gameObject);
        
        return gameObject;
    }

    public void RemoveGameObject(string name)
    {
        if (GameObjects.ContainsKey(name))
        {
            _deadObjects.Add(name);
        }
    }
}