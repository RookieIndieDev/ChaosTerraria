using ChaosTerraria.ChaosUtils;
using ChaosTerraria.Network;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChaosTerraria.UI
{
    class LoginScreen : UIState
    {
        UIPanel mainPanel;
        UIText usernameText;
        UIText passwordText;
        UIText trainingRoomText;
        UIText trainingRoomOwnerText;
        UIText heading;
        ModTextBox username;
        ModTextBox password;
        ModTextBox trainingroomOwnerUserName;
        ModTextBox trainingroom;
        UIImageButton loginButton;

        public override void OnInitialize()
        {
            mainPanel = new UIPanel();
            usernameText = new UIText("Username: ");
            passwordText = new UIText("Password: ");
            trainingRoomText = new UIText("Training Room Name: ");
            heading = new UIText("ChaosNet Login");
            trainingRoomOwnerText = new UIText("Training Room Owner Name: ");
            loginButton = new UIImageButton(ModContent.GetTexture("ChaosTerraria/UI/LoginButton"));
            loginButton.OnClick += new MouseEvent(DoAuth);
            username = new ModTextBox("");
            password = new ModTextBox("");
            trainingroomOwnerUserName = new ModTextBox("");
            trainingroom = new ModTextBox("");

            mainPanel.Width.Set(Main.screenWidth * 0.50f, 0f);
            mainPanel.Height.Set(Main.screenHeight * 0.65f, 0f);
            mainPanel.Top.Set(Main.screenHeight * 0.25f, 0f);
            mainPanel.Left.Set(Main.screenWidth * 0.25f, 0f);
            loginButton.Left.Set(mainPanel.Width.Pixels / 2 - loginButton.Width.Pixels / 2, 0f);
            loginButton.Top.Set(mainPanel.Height.Pixels * 0.70f, 0f);
            username.Top.Set(mainPanel.Height.Pixels * 0.25f, 0f);
            username.Left.Set((mainPanel.Width.Pixels / 2) - (username.Width.Pixels / 2), 0f);
            password.Top.Set(mainPanel.Height.Pixels * 0.35f, 0f);
            password.Left.Set(mainPanel.Width.Pixels / 2, 0f);
            trainingroom.Top.Set(mainPanel.Height.Pixels * 0.45f, 0f);
            trainingroom.Left.Set(mainPanel.Width.Pixels / 2, 0f);
            trainingroomOwnerUserName.Top.Set(mainPanel.Height.Pixels * 0.55f, 0f);
            trainingroomOwnerUserName.Left.Set(mainPanel.Width.Pixels / 2, 0f);
            usernameText.Top.Set(username.Top.Pixels + 5f, 0f);
            passwordText.Top.Set(password.Top.Pixels + 5f, 0f);
            trainingRoomText.Top.Set(trainingroom.Top.Pixels + 5f, 0f);
            trainingRoomOwnerText.Top.Set(trainingroomOwnerUserName.Top.Pixels + 5f, 0f);
            mainPanel.Append(username);
            mainPanel.Append(password);
            mainPanel.Append(trainingroom);
            mainPanel.Append(trainingroomOwnerUserName);
            mainPanel.Append(usernameText);
            mainPanel.Append(passwordText);
            mainPanel.Append(trainingRoomText);
            mainPanel.Append(trainingRoomOwnerText);
            mainPanel.Append(heading);
            mainPanel.Append(loginButton);
            Append(mainPanel);
        }

        private void DoAuth(UIMouseEvent evt, UIElement listeningElement)
        {
            ChaosNetworkHelper networkHelper = new ChaosNetworkHelper();
            ChaosNetConfig.data.trainingRoomNamespace = trainingroom.Text;
            ChaosNetConfig.data.username = username.Text.ToLower();
            ChaosNetConfig.data.trainingRoomUsernameNamespace = trainingroomOwnerUserName.Text.ToLower();
            networkHelper.Auth(username.Text, password.Text, trainingroomOwnerUserName.Text.ToLower());
        }
    }
}
