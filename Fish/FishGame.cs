using System;
using System.Diagnostics;
using Urho;
using Urho.Physics;

namespace Fish
{
    public class FishGame : Application
    {
        Scene scene;

        public Dwarf dwarf { get; private set; }

        public Viewport Viewport { get; private set; }

        public FishGame() : base(new ApplicationOptions(assetsFolder: "Data") { Height = 1024, Width = 576, Orientation = ApplicationOptions.OrientationType.Portrait }) { }

        [Preserve]
        public FishGame(ApplicationOptions opts) : base(opts) { }

        static FishGame()
        {
            UnhandledException += (s, e) =>
            {
                if (Debugger.IsAttached)
                    Debugger.Break();
                e.Handled = true;
            };
        }

        protected override void Start()
        {
            base.Start();
            CreateScene();
            Input.KeyDown += (e =>
            {
                if (e.Key == Key.Esc) Exit();
            });
        }

        async void CreateScene()
        {
            scene = new Scene();
            scene.CreateComponent<Octree>();

            var physics = scene.CreateComponent<PhysicsWorld>();
            physics.SetGravity(new Vector3(0, 0, 0));

            // Camera
            var cameraNode = scene.CreateChild();
            cameraNode.Position = (new Vector3(0.0f, 0.0f, -10.0f));
            cameraNode.CreateComponent<Camera>();
            Viewport = new Viewport(Context, scene, cameraNode.GetComponent<Camera>(), null);

            Renderer.SetViewport(0, Viewport);

            var zoneNode = scene.CreateChild();
            var zone = zoneNode.CreateComponent<Zone>();
            zone.SetBoundingBox(new BoundingBox(-300.0f, 300.0f));
            zone.AmbientColor = new Color(1f, 1f, 1f);

            dwarf = new Dwarf();
            var DwarfNode = scene.CreateChild(nameof(Dwarf));
            DwarfNode.AddComponent(dwarf);
            var playersLife = dwarf.Play();
            await playersLife;
            Input.SetMouseVisible(true, false);

            // Lights:
            var lightNode = scene.CreateChild();
            lightNode.Position = new Vector3(0, -5, -40);
            lightNode.AddComponent(new Light { Range = 120, Brightness = 0.8f });
        }
    }
}