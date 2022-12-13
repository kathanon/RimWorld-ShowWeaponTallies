using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace ShowWeaponTallies {
    [HarmonyPatch]
    public static class Patches {
        private static Equality equality = new Equality();

        public const float MarginX = 4f;
        public const float MarginY = 3f;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Command), "GizmoOnGUIInt")]
        public static void OnGui(Rect butRect, Command __instance) {
            if (__instance is Command_VerbTarget verb && !ForColonist(verb)) {
                var list = Traverse.Create(verb).Field<List<Verb>>("groupedVerbs").Value;
                int n = 1 + list?.Distinct(equality).Count() ?? 0;
                if (n > 1) {
                    var label = $"x{n}";
                    Text.Font = GameFont.Tiny;
                    var rect = CalcRect(butRect, label);
                    Widgets.Label(rect, label);
                    Text.Font = GameFont.Small;
                }
            }
        }

        private static bool ForColonist(Command_VerbTarget verb) 
            => (verb.verb.caster as Pawn)?.IsColonist ?? false;

        private static Rect CalcRect(Rect butRect, string label) {
            var size = Text.CalcSize(label);
            var pos = butRect.min;
            pos.x += butRect.width - size.x - MarginX;
            pos.y += MarginY;
            return new Rect(pos, size);
        }

        private class Equality : IEqualityComparer<Verb> {
            public bool Equals(Verb x, Verb y) => x.caster == y.caster;

            public int GetHashCode(Verb obj) => obj.caster.GetHashCode();
        }
    }
}
