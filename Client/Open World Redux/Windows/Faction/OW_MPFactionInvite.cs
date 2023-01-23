using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace OpenWorldRedux
{
    public class OW_MPFactionInvite : Window
    {
        public override Vector2 InitialSize => new Vector2(350f, 200f);

        private string windowTitle = "Info";
        private string windowDescription = "You have been invited to:";
        private string factionName = "";

        private float buttonX = 137f;
        private float buttonY = 38f;

        public OW_MPFactionInvite(Packet receivedPacket)
        {
            soundAppear = SoundDefOf.CommsWindow_Open;
            soundClose = SoundDefOf.CommsWindow_Close;
            absorbInputAroundWindow = true;
            closeOnAccept = false;
            closeOnCancel = true;

            factionName = receivedPacket.contents[0];
        }

        public override void DoWindowContents(Rect rect)
        {
            float centeredX = rect.width / 2;

            float windowDescriptionDif = Text.CalcSize(windowDescription).y + StandardMargin;

            float windowSubDescriptionDif = Text.CalcSize(factionName).y + Text.CalcSize(factionName).y + StandardMargin * 2;

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(windowTitle).x / 2, rect.y, Text.CalcSize(windowTitle).x, Text.CalcSize(windowTitle).y), windowTitle);

            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(windowDescription).x / 2, windowDescriptionDif, Text.CalcSize(windowDescription).x, Text.CalcSize(windowDescription).y), windowDescription);

            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(factionName).x / 2, windowSubDescriptionDif, Text.CalcSize(factionName).x, Text.CalcSize(factionName).y), factionName);

            if (Widgets.ButtonText(new Rect(new Vector2(rect.x, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Accept"))
            {
                OnAccept();
            }

            if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - buttonX, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Deny"))
            {
                OnReject();
            }
        }

        private void OnAccept()
        {
            string[] contents = new string[] { factionName };
            Packet AcceptFactionInvitationPacket = new Packet("AcceptFactionInvitationPacket", contents);
            Network.SendData(AcceptFactionInvitationPacket);
            Close();
        }

        private void OnReject()
        {
            Close();
        }
    }
}
