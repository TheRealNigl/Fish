using System;
using System.Threading.Tasks;
using Urho;
using Urho.Actions;
using Urho.Gui;
using Urho.Shapes;

namespace Fish
{
    public class StartMenu : Component
    {
        TaskCompletionSource<bool> menuTaskSource;
        Node Dwarf;
        Node rotor;
        Text textBlock;
        Node menuLight;
        bool finished = true;

        public StartMenu()
        {
            ReceiveSceneUpdates = true;
        }

        public async Task ShowStartMenu(bool gameOver)
        {
            var cache = Application.ResourceCache;
            Dwarf = Node.CreateChild();
            var model = Dwarf.CreateComponent<StaticModel>();

            if (gameOver)
            {
                model.Model = cache.GetModel(Assets.Models.Dwarf);
                model.SetMaterial(cache.GetMaterial(Assets.Models.Dwarf).Clone(""));
                Dwarf.SetScale(0.3f);
                Dwarf.Rotate(new Quaternion(180, 90, 20), TransformSpace.Local);
            }
            else
            {
                model.Model = cache.GetModel(Assets.Models.Dwarf);
                model.SetMaterial(cache.GetMaterial(Assets.Models.Dwarf).Clone(""));
                Dwarf.SetScale(1f);
                Dwarf.Rotate(new Quaternion(0, 40, -50), TransformSpace.Local);
            }

            Dwarf.Position = new Vector3(10, 2, 10);
            Dwarf.RunActions(new RepeatForever(new Sequence(new RotateBy(1f, 0f, 0f, 5f), new RotateBy(1f, 0f, 0f, -5f))));

            //TODO: rotor should be defined in the model + animation
            rotor = Dwarf.CreateChild();
            var rotorModel = rotor.CreateComponent<Box>();
            rotorModel.Color = Color.White;
            rotor.Scale = new Vector3(0.1f, 1.5f, 0.1f);
            rotor.Position = new Vector3(0, 0, -1.3f);
            var rotorAction = new RepeatForever(new RotateBy(1f, 0, 0, 360f * 6)); //RPM
            rotor.RunActions(rotorAction);

            menuLight = Dwarf.CreateChild();
            menuLight.Position = new Vector3(-3, 6, 2);
            menuLight.AddComponent(new Light { LightType = LightType.Point, Brightness = 0.3f });

            await Dwarf.RunActionsAsync(new EaseIn(new MoveBy(1f, new Vector3(-10, -2, -10)), 2));

            textBlock = new Text();
            textBlock.HorizontalAlignment = HorizontalAlignment.Center;
            textBlock.VerticalAlignment = VerticalAlignment.Bottom;
            textBlock.Value = gameOver ? "GAME OVER" : "TAP TO START";
            textBlock.SetFont(cache.GetFont(Assets.Fonts.Font), Application.Graphics.Width / 15);
            Application.UI.Root.AddChild(textBlock);

            menuTaskSource = new TaskCompletionSource<bool>();
            finished = false;
            await menuTaskSource.Task;
        }

        protected override async void OnUpdate(float timeStep)
        {
            if (finished)
                return;

            var input = Application.Input;
            if (input.GetMouseButtonDown(MouseButton.Left) || input.NumTouches > 0)
            {
                finished = true;
                Application.UI.Root.RemoveChild(textBlock, 0);
                await Dwarf.RunActionsAsync(new EaseIn(new MoveBy(1f, new Vector3(-10, -2, -10)), 3));
                rotor.RemoveAllActions();
                menuTaskSource.TrySetResult(true);
            }
        }
    }
}