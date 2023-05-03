using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using OpenWorldRedux;
using HarmonyLib;
using Verse;
using Verse.Sound;

namespace OpenWorldRedux
{
    [StaticConstructorOnStartup]
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

        public static Texture2D icon;
        public static Texture2D icon2;

        [StaticConstructorOnStartup]
        static class iconHelper
        {
            static iconHelper()
            {
                icon = ContentFinder<Texture2D>.Get("Icons/chatHighlight");
                icon2 = ContentFinder<Texture2D>.Get("Icons/chat");
            }
        }

        public mainTabWindowChat()
        {
            layer = WindowLayer.GameUI;
            closeOnAccept = false;
            closeOnCancel = true;
        }


        MainButtonDef chatDef = DefDatabase<MainButtonDef>.GetNamed("Chat");


        public override void DoWindowContents(Rect rect)
        {
            if(chatDef.Icon == icon)
            {
                AccessTools.Field(typeof(MainButtonDef), "icon").SetValue(chatDef, icon2);
            }


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




    [StaticConstructorOnStartup]
    public static class MPChat
    {
        public static string cacheInputText;

        public static List<string> cacheChatText = new List<string> { };

        public static Texture2D icon;
        public static Texture2D icon2;

        [StaticConstructorOnStartup]
        static class fileHelper
        {
            static fileHelper()
            {
                icon = ContentFinder<Texture2D>.Get("Icons/chatHighlight");
                icon2 = ContentFinder<Texture2D>.Get("Icons/chat");


            }
        }

        public static void SendMessage()
        {
            if (cacheInputText.StartsWith("/"))
            {
                string text = cacheInputText.Remove(0, 1);
                string text2 = "";
                string item = "";


                string command = text.Split(' ')[0];

                switch (command)
                {
                    case "help":
                        if(!BooleanCache.isAdmin)
                        {
                            List<string> cmdList = new List<string>();
                            cmdList.Add("<color=yellow>[SYSTEM]: Local Commands:</color>");
                            cmdList.Add("<color=yellow><b>- help</b>: Shows a list of available commands.</color>");
                            cmdList.Add("<color=yellow><b>- ping</b>: Checks connection with the server.</color>");
                            cmdList.Add("<color=yellow><b>- pm</b>: Sends a private message to player [1].</color>");
                            foreach (string cmd in cmdList)
                            {
                                cacheChatText.Add(cmd);
                                cacheInputText = "";
                                mainTabWindowChat.messageScroll = true;
                            }
                        }
                        else
                        {
                            Packet RequestHelp = new Packet("RequestAdminHelp");
                            Network.SendData(RequestHelp);
                            cacheInputText = "";
                        }
                        break;
                    case "ping":
                        if (BooleanCache.isConnectedToServer) 
                        {
                            text2 = "Pong!";
                            item= "<color=yellow>[SYSTEM]: </color>" + text2;
                        }
                        break;
                    case "pm":
                        SendMessage(cacheInputText, true);
                        cacheInputText = "";
                        return;
                    default:
                        if (BooleanCache.isAdmin)
                        {
                            SendCommand(cacheInputText.Remove(0, 1));
                            cacheInputText = "";
                            break;
                        }
                        else
                        {
                            text2 = "Permission Denied. Either you are not an admin or this command does not exist.</color>";
                            item = "<color=red>[ERROR]: " + text2;
                            break;
                        }
                }

                if (item != "")
                {
                    cacheChatText.Add(item);
                    cacheInputText = "";
                    mainTabWindowChat.messageScroll = true;
                }
                return;
            }
            

            SendMessage(cacheInputText, false);
            cacheInputText = "";

        }

        public static void SendMessage(string data, bool isPrivate)
        {
            string packetType = "SendMessage";
            if(isPrivate) { packetType = "SendPrivateMessage"; };

            string[] contents = new string[] { cacheInputText };
            Packet NewMsgPacket = new Packet(packetType, contents);
            Network.SendData(NewMsgPacket);

        }

        public static void SendCommand(string data)
        {
            string[] contents = new string[] { cacheInputText };
            Packet NewCommandPacket = new Packet("SendCommand", contents);
            Network.SendData(NewCommandPacket);
        }



        // public static SoundDef chatSound = SoundDef.Named("OW_chatSound");


        public static void ReceiveMessage(string data)
        {
            try
            {
                mainTabWindowChat.messageScroll = true;
                if (cacheChatText.Count >= 100)
                {
                    cacheChatText.RemoveAt(0);
                }
                cacheChatText.Add(data);

                //Log.Message(chatSound.defName + " is trying to play! maxSimultaneous is " + chatSound.maxSimultaneous);
                //chatSound.PlayOneShot(SoundInfo.OnCamera());


                MainButtonDef chatDef = DefDatabase<MainButtonDef>.GetNamed("Chat");
                AccessTools.Field(typeof(MainButtonDef), "icon").SetValue(chatDef, icon);

            }
            catch(Exception ex)
            {
                Log.Message(ex.ToString());
            }
        }

        public static void ReceiveCache(List<string> data)
        {
            try
            {
                mainTabWindowChat.messageScroll = true;
                if (cacheChatText.Count >= 100)
                {
                    cacheChatText.RemoveAt(0);
                }
                cacheChatText.AddRange(data);
            }
            catch
            {
            }
        }

        public static void ReceiveAdminHelp(List<string> data)
        {

            List<string> cmdList = new List<string>();
            cmdList.Add("<color=yellow><b>[SYSTEM]</b>: Local Commands:</color>");
            cmdList.Add("<color=yellow><b>- help</b>: Shows a list of available commands.</color>");
            cmdList.Add("<color=yellow><b>- ping</b>: Checks connection with the server.</color>");
            cmdList.Add("<color=yellow><b>- pm</b>: Sends a private message to a player.</color>");

            cmdList.Add("\n<color=yellow><b>[SYSTEM]</b>: Serverside Commands:</color>");
            foreach (string cmd in data)
            {
                cmdList.Add("<color=yellow>- " + cmd + "</color>");
            }
            
            foreach(string item in cmdList) 
            {
                cacheChatText.Add(item);
                cacheInputText = "";
                mainTabWindowChat.messageScroll = true;
            }
        }
    }
}
