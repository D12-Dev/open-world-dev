using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using OpenWorldRedux;
using Verse;

namespace OpenWorldRedux
{
    public class mainTabWindowChat : MainTabWindow
    {
        public static bool messageScroll;

        public override Vector2 RequestedTabSize => new Vector2(586f, 300f);

        private int startAcceptingInputAtFrame;
        private bool AcceptsInput => startAcceptingInputAtFrame <= Time.frameCount;

        private Vector2 scrollPosition = Vector2.zero;

        private float sendButtonX = 100f;
        private float sendButtonY = 30f;

        private float textFieldY = 30f;
        private int maxFieldCharacters = 256;

        private string connectionString = "";
        private string userString = "";

        public mainTabWindowChat()
        {
            layer = WindowLayer.GameUI;
            closeOnAccept = false;
            closeOnCancel = true;
        }

        public override void DoWindowContents(Rect rect)
        {
            Text.Font = GameFont.Small;

            if (BooleanCache.isConnectedToServer) connectionString = "Status: Connected [" + FocusCache.playerCount + "]";
            else connectionString = "Status: Disconnected";
            Widgets.Label(new Rect(new Vector2(rect.x, rect.y), new Vector2(Text.CalcSize(connectionString).x, Text.CalcSize(connectionString).y)), connectionString);

            userString = FocusCache.userName;
            Widgets.Label(new Rect(new Vector2(rect.xMax - Text.CalcSize(userString).x, rect.y), new Vector2(Text.CalcSize(userString).x, Text.CalcSize(userString).y)), userString);

            try { GenerateList(new Rect(new Vector2(rect.x, rect.y + 25f), new Vector2(rect.width, rect.height - 47f - 25f))); }
            catch { }

            Rect inputAreaRect = new Rect(rect.x, rect.yMax - textFieldY, rect.width - sendButtonX - StandardMargin, textFieldY);
            string inputAreaText = Widgets.TextField(inputAreaRect, MPChat.cacheInputText);
            if (AcceptsInput && inputAreaText.Length <= maxFieldCharacters) MPChat.cacheInputText = inputAreaText;

            Rect sendButtonRect = new Rect(new Vector2(rect.xMax - sendButtonX, rect.yMax - sendButtonY), new Vector2(sendButtonX, sendButtonY));
            bool keyPressed = (inputAreaText.Length > 0) && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter);
            if (Widgets.ButtonText(sendButtonRect, "Send") || keyPressed)
            {
                if (MPChat.cacheInputText.Contains("│")) MPChat.cacheInputText = MPChat.cacheInputText.Replace("│", "");
                if (MPChat.cacheInputText.Contains("»")) MPChat.cacheInputText = MPChat.cacheInputText.Replace("»", "");
                if (MPChat.cacheInputText.Contains("┼")) MPChat.cacheInputText = MPChat.cacheInputText.Replace("┼", "");
                if (string.IsNullOrWhiteSpace(MPChat.cacheInputText)) return;

                MPChat.SendMessage();
            }
        }

        
        private void GenerateList(Rect mainRect)
        {
            float height = 6f;

            foreach (string str in MPChat.cacheChatText) height += Text.CalcHeight(str, mainRect.width);

            Rect viewRect = new Rect(mainRect.x, mainRect.y, mainRect.width - 16f, height);

            Widgets.BeginScrollView(mainRect, ref scrollPosition, viewRect);

            float num = 0;
            float num2 = scrollPosition.y - 30f;
            float num3 = scrollPosition.y + mainRect.height;
            int num4 = 0;

            int index = 0;
            foreach (string str in MPChat.cacheChatText)
            {
                if (num > num2 && num < num3)
                {
                    Rect rect = new Rect(0f, mainRect.y + num, viewRect.width, Text.CalcHeight(str, mainRect.width));
                    DrawCustomRow(rect, str, index);
                }

                num += Text.CalcHeight(str, mainRect.width) + 0f;
                num4++;
                index++;
            }
            Widgets.EndScrollView();

            if (messageScroll)
            {
                scrollPosition.Set(scrollPosition.x, scrollPosition.y + 22);
                messageScroll = false;
            }

        }

        private void DrawCustomRow(Rect rect, string message, int index)
        {
            Text.Font = GameFont.Small;
            Rect fixedRect = new Rect(new Vector2(rect.x + 10f, rect.y + 5f), new Vector2(rect.width - 36f, rect.height));
            //if (index % 2 == 0) Widgets.DrawHighlight(fixedRect);
            Widgets.Label(fixedRect, message);
        }
    }





    public static class MPChat
    {
        public static string cacheInputText;

        public static List<string> cacheChatText = new List<string> { "<color=yellow>Welcome to the chat!</color>", "<color=yellow>Please, keep anything you post appropriate and respect other users.</color>", "<color=yellow>Type '/help' to see available commands.</color>" };

        public static void SendMessage()
        {
            string item = "<b>" + DateTime.Now.ToString("h:mm tt") + " | [<color=#708090>" + FocusCache.userName + "</color>]: </b>" + cacheInputText; // Formats the message that's about to send
            if (cacheInputText.StartsWith("/"))
            {
                string text = cacheInputText.Remove(0, 1);
                string text2 = "";
                switch (text)
                {
                    case "help":
                        text2 = "<color=yellow>";
                        text2 += "Available Commands:";
                        text2 += "\n- help: Shows this screen.";
                        text2 += "\n- ping: Checks connection with the server.";
                        text2 += "</color>";
                        break;
                    case "ping":
                        if (BooleanCache.isConnectedToServer) text2 = "Pong!";
                        break;
                    default:
                        text2 += "Command not found.";
                        return;
                }
                item = "<color=yellow>[SYSTEM]: </color>" + text2;

                cacheChatText.Add(item);
                cacheInputText = "";
                mainTabWindowChat.messageScroll = true;
                return;
            }
            /*string[] array = profanityFilter;
            foreach (string text4 in array)
            {
                if (MPChat.cacheInputText.Contains(text4) || MPChat.cacheInputText.Contains(text4.ToLowerInvariant()) || MPChat.cacheInputText.Contains(text4.ToUpperInvariant()))
                {
                    MPChat.cacheInputText = "";
                    return;
                }
            }*/
            cacheChatText.Add(item);

            
            //Networking.SendData("ChatMessage│" + Networking.username + "│" + cacheInputText);


            cacheInputText = "";

            mainTabWindowChat.messageScroll = true;

        }

        public static void ReceiveMessage(string data)
        {
            mainTabWindowChat.messageScroll = true;
            try
            {
                string text = data.Split('│')[1];
                string text2 = data.Split('│')[2];
                string item = DateTime.Now.ToString("h:mm tt") + " | [" + text + "]: " + text2;
                MPChat.cacheChatText.Insert(0, item);
            }
            catch
            {
            }
        }
    }
}
