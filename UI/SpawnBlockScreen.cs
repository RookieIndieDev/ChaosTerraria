using ChaosTerraria.Tile_Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChaosTerraria.UI
{
    class SpawnBlockScreen : UIState
    {
        UIPanel mainPanel;
        UIText spawnPointId;
        UIText spawnCount;
        ModTextBox spawnPointTextbox;
        ModTextBox spawnCountTextbox;
        UIImageButton closeButton;
        SpawnBlockTileEntity spawnBlockTileEntity;

        public override void OnInitialize()
        {
            mainPanel = new UIPanel();
            spawnPointId = new UIText("Spawn Point ID: ");
            spawnCount = new UIText("Spawn Count: ");
            spawnPointTextbox = new ModTextBox("");
            spawnCountTextbox = new ModTextBox("");
            closeButton = new UIImageButton(ModContent.GetTexture("ChaosTerraria/UI/CloseButton"));

            mainPanel.Width.Set(Main.screenWidth * 0.25f, 0f);
            mainPanel.Height.Set(Main.screenHeight * 0.45f, 0f);
            mainPanel.Top.Set(Main.screenHeight * 0.50f, 0f);
            mainPanel.Left.Set(Main.screenWidth * 0.50f, 0f);
            spawnPointId.Left.Set(spawnPointTextbox.Left.Pixels - 3f, 0f);
            spawnPointId.Top.Set(mainPanel.Height.Pixels * 0.40f, 0f);
            spawnCount.Left.Set(spawnCountTextbox.Left.Pixels - 3f, 0f);
            spawnCount.Top.Set(mainPanel.Height.Pixels * 0.55f, 0f);
            spawnPointTextbox.Left.Set(mainPanel.Width.Pixels / 2 - 20f, 0f);
            spawnPointTextbox.Top.Set(mainPanel.Height.Pixels * 0.40f, 0f);
            spawnCountTextbox.Left.Set(mainPanel.Width.Pixels / 2 - 20f, 0f);
            spawnCountTextbox.Top.Set(mainPanel.Height.Pixels * 0.55f, 0f);
            closeButton.Left.Set(mainPanel.Width.Pixels / 2 - closeButton.Width.Pixels / 2, 0f);
            closeButton.Top.Set(mainPanel.Height.Pixels * 0.65f, 0f);
            closeButton.OnClick += this.OnClose;
            mainPanel.Append(spawnPointId);
            mainPanel.Append(spawnCount);
            mainPanel.Append(spawnPointTextbox);
            mainPanel.Append(spawnCountTextbox);
            mainPanel.Append(closeButton);
            Append(mainPanel);
        }

        private void OnClose(UIMouseEvent evt, UIElement listeningElement)
        {
            if (spawnBlockTileEntity != null)
            {
                spawnBlockTileEntity.roleNamespace = spawnPointTextbox.Text;
                spawnBlockTileEntity.spawnCount = int.Parse(spawnCountTextbox.Text);
            }
            UIHandler.isSpawnBlockScreenVisible = false;
        }

        public void GetValues(int i, int j)
        {
            int index = ModContent.GetInstance<SpawnBlockTileEntity>().Find(i, j);
            if (index != -1)
            {
                spawnBlockTileEntity = (SpawnBlockTileEntity)TileEntity.ByID[index];
                spawnPointTextbox.SetText(spawnBlockTileEntity.roleNamespace);
                spawnCountTextbox.SetText(spawnBlockTileEntity.spawnCount.ToString());
                Main.blockInput = false;
            }
            else
            {
                Main.NewText("Couldn't get the tile entity at X: " + i + " Y: " + j + "Tile Type" + Main.tile[i, j].type);
            }
        }
    }
}
