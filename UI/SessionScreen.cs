using ChaosTerraria.ChaosUtils;
using ChaosTerraria.Network;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChaosTerraria.UI
{
    class SessionScreen : UIState
    {
        UIPanel mainPanel;
        UIText sessionText;
        UIImageButton startSession;

        public override void OnInitialize()
        {
            mainPanel = new UIPanel();
            sessionText = new UIText("Press the button to start the Session!");
            startSession = new UIImageButton(ModContent.GetTexture("ChaosTerraria/UI/StartButton"));
            startSession.OnClick += new MouseEvent(StartSession);

            mainPanel.Width.Set(Main.screenWidth * 0.50f, 0f);
            mainPanel.Height.Set(Main.screenHeight * 0.65f, 0f);
            mainPanel.Top.Set(Main.screenHeight * 0.25f, 0f);
            mainPanel.Left.Set(Main.screenWidth * 0.25f, 0f);

            sessionText.Left.Set(mainPanel.Width.Pixels * 0.35f, 0f);
            sessionText.Top.Set(mainPanel.Height.Pixels * 0.45f, 0f);

            startSession.Left.Set(mainPanel.Width.Pixels / 2 - startSession.Width.Pixels / 2, 0f);
            startSession.Top.Set(mainPanel.Height.Pixels * 0.50f, 0f);

            mainPanel.Append(sessionText);
            mainPanel.Append(startSession);
            Append(mainPanel);
        }

        private void StartSession(UIMouseEvent evt, UIElement listeningElement)
        {
            ChaosNetworkHelper networkHelper = new ChaosNetworkHelper();
            networkHelper.StartSession();
        }
    }
}
