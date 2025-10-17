using Godot;
using System;

namespace ConnectMore.Scripts.MainMenu
{
    public partial class MainMenu : Control
    {
        [Export] public PackedScene GameScene { get; set; }
        
        [Export] public PackedScene LeaderboardScene { get; set; }

        private Window setupDialog;
        private SpinBox rowsInput;
        private SpinBox colsInput;
        private SpinBox playersInput;
        private SpinBox connectInput;
        private Button confirmButton;
        private Button cancelButton;

        // Reference to the game scene
        private Node2D gameReference;

        public override void _Ready()
        {
            this.GetNode<Button>("TextureRect/VBoxContainer/Button").Pressed += this.OnStartPressed;
            this.GetNode<Button>("TextureRect/VBoxContainer/Button2").Pressed += this.OnLeaderboardPressed;
            
            this.CreateSetupDialog();
        }

        private void OnStartPressed()
        {
            // Show the setup popup when Start Game is pressed
            setupDialog.PopupCentered();
        }

        private void OnLeaderboardPressed()
        {
            this.GetTree().ChangeSceneToPacked(this.LeaderboardScene);
        }

        private void CreateSetupDialog()
        {
            setupDialog = new Window
            {
                Title = "Game Setup",
                Size = new Vector2I(400, 300),
                Visible = false
            };

            VBoxContainer layout = new VBoxContainer
            {
                AnchorsPreset = (int)Control.LayoutPreset.FullRect,
                OffsetLeft = 20,
                OffsetTop = 20,
                OffsetRight = -20,
                OffsetBottom = -20
            };

            layout.AddThemeConstantOverride("separation", 12);
            setupDialog.AddChild(layout);

            // Input fields with defaults
            rowsInput = CreateLabeledSpinBox(layout, "Rows:", 6, 1, 12);
            colsInput = CreateLabeledSpinBox(layout, "Columns:", 7, 1, 12);
            playersInput = CreateLabeledSpinBox(layout, "Players:", 2, 2, 4);
            connectInput = CreateLabeledSpinBox(layout, "Connection Length:", 4, 3, 6);

            // Buttons row
            HBoxContainer buttonRow = new HBoxContainer
            {
                Alignment = BoxContainer.AlignmentMode.Center
            };
            buttonRow.AddThemeConstantOverride("separation", 16);

            confirmButton = new Button { Text = "Start Game", SizeFlagsHorizontal = Control.SizeFlags.ExpandFill };
            cancelButton = new Button { Text = "Cancel", SizeFlagsHorizontal = Control.SizeFlags.ExpandFill };

            confirmButton.Pressed += OnConfirmPressed;
            cancelButton.Pressed += () => setupDialog.Hide();

            buttonRow.AddChild(confirmButton);
            buttonRow.AddChild(cancelButton);
            layout.AddChild(buttonRow);

            this.AddChild(setupDialog);
        }

        private SpinBox CreateLabeledSpinBox(VBoxContainer layout, string labelText, double defaultValue, double min, double max)
        {
            HBoxContainer row = new HBoxContainer();
            row.AddThemeConstantOverride("separation", 10);

            Label label = new Label
            {
                Text = labelText,
                CustomMinimumSize = new Vector2(180, 30),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center
            };

            SpinBox spin = new SpinBox
            {
                MinValue = min,
                MaxValue = max,
                Value = defaultValue,
                SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
            };

            row.AddChild(label);
            row.AddChild(spin);
            layout.AddChild(row);
            return spin;
        }

        private void OnConfirmPressed()
        {
            // Read user-selected settings
            int rows = (int)rowsInput.Value;
            int cols = (int)colsInput.Value;
            int players = (int)playersInput.Value;
            int connect = (int)connectInput.Value;

            // Instantiate the game scene and store a reference
            gameReference = GameScene.Instantiate<Node2D>();

            // Reference the GameManager node
            var gameManager = gameReference.GetNodeOrNull<ConnectMore.Scripts.Game.GameManager>("GameManager");

            if (gameManager != null)
            {
                // Set the properties from the main menu
                gameManager.Players = players;

                if (gameManager.Board != null)
                {
                    gameManager.Board.Rows = rows;
                    gameManager.Board.Columns = cols;
                    gameManager.Board.ConnectLength = connect;
                }
            }

            // Add the game scene to the tree
            GetTree().Root.AddChild(gameReference);

            // Hide/remove main menu
            this.Hide();
            this.QueueFree();
        }
    }
}

