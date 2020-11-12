using UnityEditor;
using UnityEngine;

namespace EnhancedHierarchy {
    /// <summary>
    /// All the styles, icons, colors and contents used in hierarchy
    /// </summary>
    public static class Styles {

        static Styles() {
            if (EditorGUIUtility.isProSkin) {
                backgroundColorEnabled = new Color32(155, 155, 155, 255);
                backgroundColorDisabled = new Color32(155, 155, 155, 100);
                normalColor = new Color32(56, 56, 56, 255);
                selectedFocusedColor = new Color32(62, 95, 150, 255);
                selectedUnfocusedColor = new Color32(72, 72, 72, 255);
            } else {
                backgroundColorEnabled = new Color32(65, 65, 65, 255);
                backgroundColorDisabled = new Color32(65, 65, 65, 120);
                normalColor = new Color32(194, 194, 194, 255);
                selectedFocusedColor = new Color32(62, 125, 231, 255);
                selectedUnfocusedColor = new Color32(143, 143, 143, 255);
            }

            childToggleColor = new Color32(30, 30, 30, 255);
            monobehaviourIconTexture = Utility.FindOrLoad(monobehaviourIcon);

            var offTexture = Utility.FindOrLoad(lockOff);
            var onTexture = Utility.FindOrLoad(lockOn);
            lockToggleStyle = Utility.CreateStyleFromTextures(onTexture, offTexture);

            offTexture = Utility.FindOrLoad(activeOff);
            onTexture = Utility.FindOrLoad(activeOn);
            activeToggleStyle = Utility.CreateStyleFromTextures(onTexture, offTexture);

            offTexture = Utility.FindOrLoad(rendererOff);
            onTexture = Utility.FindOrLoad(rendererOn);
            rendererToggleStyle = Utility.CreateStyleFromTextures(onTexture, offTexture);

            offTexture = Utility.FindOrLoad(staticOff);
            onTexture = Utility.FindOrLoad(staticOn);
            staticToggleStyle = Utility.CreateStyleFromTextures(onTexture, offTexture);

            onTexture = Utility.FindOrLoad(tag);
            tagStyle = Utility.CreateStyleFromTextures(onTexture, onTexture);
            tagStyle.padding = new RectOffset(5, 17, 0, 1);
            tagStyle.border = new RectOffset();

            onTexture = Utility.FindOrLoad(layers);
            layerStyle = Utility.CreateStyleFromTextures(onTexture, onTexture);
            layerStyle.padding = new RectOffset(5, 17, 0, 1);
            layerStyle.border = new RectOffset();

            treeLineTexture = Utility.FindOrLoad(treeLine);
            treeTeeTexture = Utility.FindOrLoad(treeTee);
            treeElbowTexture = Utility.FindOrLoad(treeElbow);

            infoIcon = Utility.FindOrLoad(info);
            warningIcon = Utility.FindOrLoad(warning);
            errorIcon = Utility.FindOrLoad(error);

            fadeTexture = Utility.FindOrLoad(fade);

            labelNormal = new GUIStyle("PR Label");
            labelDisabled = new GUIStyle("PR DisabledLabel");
            labelPrefab = new GUIStyle("PR PrefabLabel");
            labelPrefabDisabled = new GUIStyle("PR DisabledPrefabLabel");
            labelPrefabBroken = new GUIStyle("PR BrokenPrefabLabel");
            labelPrefabBrokenDisabled = new GUIStyle("PR DisabledBrokenPrefabLabel");

            miniLabelStyle = new GUIStyle("ShurikenLabel") {
                alignment = TextAnchor.MiddleRight,
                clipping = TextClipping.Overflow
            };
            miniLabelStyle.normal.textColor = Color.white;
            miniLabelStyle.hover.textColor = Color.white;
            miniLabelStyle.focused.textColor = Color.white;
            miniLabelStyle.active.textColor = Color.white;

            applyPrefabStyle = new GUIStyle("ShurikenLabel") {
                alignment = TextAnchor.MiddleCenter,
                clipping = TextClipping.Overflow
            };
            applyPrefabStyle.normal.textColor = Color.white;
            applyPrefabStyle.hover.textColor = Color.white;
            applyPrefabStyle.focused.textColor = Color.white;
            applyPrefabStyle.active.textColor = Color.white;

            EditorApplication.update += () => {
                applyPrefabStyle.fontSize = Preferences.IconsSize - 6;
            };

            onTexture = Utility.FindOrLoad(childToggleOn);
            offTexture = Utility.FindOrLoad(childToggleOff);

            iconButton = new GUIStyle("iconButton");
            iconButton.padding = new RectOffset();
            iconButton.margin = new RectOffset();

            newToggleStyle = Utility.CreateStyleFromTextures("ShurikenDropdown", onTexture, offTexture);

            newToggleStyle.fontSize = 8;
            newToggleStyle.clipping = TextClipping.Overflow;
            newToggleStyle.alignment = TextAnchor.MiddleLeft;
            newToggleStyle.imagePosition = ImagePosition.TextOnly;
            newToggleStyle.border = new RectOffset(0, 1, 0, 1);
            newToggleStyle.contentOffset = new Vector2(1f, 0f);
            newToggleStyle.padding = new RectOffset(0, 0, 0, 0);
            newToggleStyle.overflow = new RectOffset(-1, 1, -3, 0);
            newToggleStyle.fixedHeight = 0f;
            newToggleStyle.fixedWidth = 0f;
            newToggleStyle.stretchHeight = true;
            newToggleStyle.stretchWidth = true;

            newToggleStyle.normal.textColor =
                newToggleStyle.hover.textColor =
                newToggleStyle.focused.textColor =
                newToggleStyle.active.textColor =
                newToggleStyle.onNormal.textColor =
                newToggleStyle.onHover.textColor =
                newToggleStyle.onFocused.textColor =
                newToggleStyle.onActive.textColor = new Color32(230, 230, 230, 255);

            prefabApplyContent = new GUIContent("A");
            staticContent = new GUIContent();
            lockContent = new GUIContent();
            activeContent = new GUIContent();
            rendererContent = new GUIContent();
            tagContent = new GUIContent();
            layerContent = new GUIContent();

            ReloadTooltips();
        }

        public static void ReloadTooltips() {
            if (Preferences.Tooltips && !Preferences.RelevantTooltipsOnly) {
                prefabApplyContent.tooltip = "Apply Prefab Changes";
                staticContent.tooltip = "Static";
                lockContent.tooltip = "Lock/Unlock";
                activeContent.tooltip = "Enable/Disable";
                rendererContent.tooltip = "Enable/Disable renderer";
                tagContent.tooltip = "Tag";
                layerContent.tooltip = "Layer";
            } else {
                prefabApplyContent.tooltip = string.Empty;
                staticContent.tooltip = string.Empty;
                lockContent.tooltip = string.Empty;
                activeContent.tooltip = string.Empty;
                rendererContent.tooltip = string.Empty;
                tagContent.tooltip = string.Empty;
                layerContent.tooltip = string.Empty;
            }
        }

        #region Textures Sources
        private const string treeLine = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAKElEQVQ4EWP8//8/AxEApIgRmzombIKkiI0awMAwGgajYQDKMwOfDgBL0gQdcnVX0wAAAABJRU5ErkJggg==";
        private const string treeTee = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAMklEQVQ4EWP8//8/AxEApIgRmzombIKkiI0awMAw8GHAQkKUYU0wpBgwmpBwhPbApwMAL00FIa1Ycy8AAAAASUVORK5CYII=";
        private const string treeElbow = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAANUlEQVQ4EWP8//8/AxEApIgRmzombIKkiI0awMAwDMKAhYQ4x5riiDUAayoEWT4MAnHgvQAA7T4FIQ1dYzoAAAAASUVORK5CYII=";

        private const string warning = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAACh0lEQVRYCWO0sLBgoADIAfU+okA/AxMlmoF6n1Con2IH/BtoB1BqP8UhoA90AQ8lrqAkDXAALZ4AxHkD5QAXE1U2BwsN9mKgA4zJdQQLmRq5WZgZShLdeEHahc7c/tn75y+DAzlmkRsF+d5mXPYyoiwMIAxiAy2nmwOMBLiZ8sLtueEeBrGFeJlagAIkJ0hyQqA32olbnJsDoRXEjnTgtgY6gOQEiTAF7h+8DAclCRYHF0NODEUgMRUplkKghBGGJB4BUhwACvP6FE9eBiYmRgwjQWJJ7rwiQIleDEk8AqQ4IN9Gm91BW54Np3EgOZAaoAIQJgoQ6wBjdlaG3ARXcLaDG+zf8JIBhJEBSA0nG2MTUIyoBEmsA3oCrbglRAWYke3Cygap8bfksgVKEpUgiXGAgwg/k0OQNSLbYbUZSRCkVkyAKR8oZIgkjJVJjAPqQcHKzoaZ8LCaCBQEqY1z4RUDMvtwqYGJEyqKHbTkWB2ACQumHoXe2CCOwkfmgPRsO8XqcO3Rbzug+CFkOWQ2vhDgZmRkaABlO0Ygg1QA0gPRy9AE1Isz/vA5IB9YuNgrS7LitBtbLkBWDNILMgMoBkoPWAEuBxhzczDmxjoTlZOwGgwTBJkBMgvIx1plM8vIyMDUItPLYpx4dAyUscc9TGGkAw+wDsDvSA5ggmRmYuC5cPeXGlDfQpheGI0tBBykRZgdfMy5YGoopkFmgcwEGgRKkCgA3QHg8h7U0GBhJj3hoZiMxAGZBW28NAKFURIkI7BjArLpP1S9E5AOBmJQkQfCoGY3SA6EQepAcQLKuoSyL1AJTrASKLMBKssEMghmOUhsHxSD2PQA/9CjgB6Wotgx6oDREAAA8+NWHMvSwd0AAAAASUVORK5CYII=";
        private const string info = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAACBUlEQVRYCe1Xu0oDQRSdNQoKooKaNLGw9A+yX2CnP2BjLbYmv5BetEl+w85CEBZsRRQbS0kQtBB8ReM5684yu3PRzSzDNl44mcmde+8589jNJGi1WqpKm6qSnNz/AqpYgcDc9mnzC/ocnAVCYA0wjbFfCbSfEyBG2pG0+dhX+CPgHiDHOxBbXgCdy8BBGIab7XZbNRqNOND1YzAYqG63q6IoOmRd4MOsJW3BDAJqnU6nNDmJOAHWMmxs9MWngMus6vW6GVeqb9Ti8v8pQFqVUgKM5Aw5/RKZFWQUyHRxThQxgWWeAOaVEjABsQ61JicJsFTqbB+tJMBS6YNY15TeA4VXAM+2ruPcSivgXMwlURJQeAscngJLoyTACvLpkM5AYVG+zkD8KvY0a+uAS7OVfKX0DIdDnW+dL2kLrCCd7dLqn2Pk8s5g1S4koNfrqX6/f4cC+8AGwEvLpHaGBN4FMiIkAZl9SsgfkbgNXAIngKtxe1k/FSHtdzqIWY+BWyTsAFfAIjAPxJcWtLpgDX1CqkdCji0A1iWj1mw24U+NwUvAVhAE65j9Bfq7wDnAMd7teJ/7BPi0UCyh74qpePi06fE3OJ6BTEyQ+2dEklXgCFgB9oAbgIReLH8GqO4BOAaegGvA53tB5QWALyY8RTvnm5xk0qGhn/by0/j9/E2AX+akeuUCvgEs3W2OBzZAiQAAAABJRU5ErkJggg==";
        private const string error = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAACPklEQVRYCe1XQWsTQRSezXYnSdmBtD3UQ0SCB0HBSPWQi6DngkIvXpSKYtCDN/EniJceSk4K/gLFg9CbFy/+AAWlFAQJQkqyEghIMZON76lD3kxmZhtCyWUfPPbt975537fLZoYEjUaDLTIKixRH7dxA/gaWZvwIa8B/ClmELDvW/gJ8B/KLo6/BPgMhMEeEffkS56/uxHGdBwGBJ+Xv8Zi9HAzY/nC4AugtyOGka6/CarVq7zA2Jo2t81H0+q4QNZc4ckMwBiZZW8rTvTQ9AugjmWEtfQbUgiaI794XYtUnrshoos45/yFlvZum3wD/qnq2a5aB6+ei6M0DIcrHEVcCaOIi53FHyiuHaZoA/ln1zKvvZ/gcnvxtUwjmEn+cJAzTFrhmW4izF6IIP8gbNg5iPgNn4LVXXOKugRTHtfeEWAfsJsVp7TPgfHI6IKv+/wDLhKdpajeEdJKl9htehAHt4UwDmjuNeUI3vp0wU7K1tpbJySKYbyCLr/UPRyOGOU94DeDe7ooOCD/r9/8m1q7wzcA1pgGquIcHS9YAlzDi6nCCUhKe5tbcivEEVCY+JWnKvkt5DQ8Y3F5pxIUC2ygW2dVSiZ0KcZkeShxORjyWH0H+1Bn/7kwDKF6BxJMM40OWCTRiBhF/B70tyLbJUfemAcSVuOJ4TSiSuhLxF4A9hOyrnu1qM2DjHcsEEX8PQ55A9mzDKBbM+MfkNizepAMsdRewFuSBpTcFzWpgasC8wPQXNO/EGdfnBvI38Af9967LZ4JMowAAAABJRU5ErkJggg==";

        private const string monobehaviourIcon = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAARCAYAAAACCvahAAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAAsTAAALEwEAmpwYAAAAB3RJTUUH4QUWDigZn/bO1AAAAB1pVFh0Q29tbWVudAAAAAAAQ3JlYXRlZCB3aXRoIEdJTVBkLmUHAAACKUlEQVQ4y6WTTUhUURTHf+++cUaHmXGczKSNIiEzYWkGBfaxSBL62IaFBbWpIAmCFm2CFq2Cgna2EmqRUm0qhDQjEaLCIsKQjBGRwrRxmnF8Os68d26LmbFBJQgvHA4c7u/8z/9yD2zgGPncDISB0D/uTgBx4G2h4MrncCye6DZN5cZQGICRb5uylkikLEbev+NsR3s74AdeA1mVh0MZ23YvZ7NkMxmyWTsXtkOpx02pp4SGpmYe9DzuBWwgAlCAEdG5QKO18GnGYngqQfR3mqDfR9DnJbKjkfs9T14BB4vHxhENwFhskcsvxhmLWQA0bvEzcGY3wYAPgLr6MKs9IyI42uDis1ECnhL6O3axs8qHYSjQGgyDYMCHaL0Cq7/KwocfCaKxFHfa6tle6UXQXO3/ipXOcHMoirW0TKjcv1bZcYQvP5Ns9ZrUBssQ0dwanuDlt2km5+b5OJ1kaGKGgXMt6yvPzi+yyZ23IJorLbVUeaD3ZDMVLuHp6T05C6thEcFtaKylNI4IC+kMR+8NEp2e40jXILPxJLcHR4vZImVHqKsoYyqWIr6QxuNS9J1vpXXbZrpO7OVYuJprhxvQWtYfu6Wmkmqfi0uP3vD5+xyTsSSd+8OUukw6D0RW/sEauCpUjjKg+9Q+aiq83Ogb4dDd51x4OETA46KuMoCIxrZlzWtbhQbEk1xva8I0FUopTKVwJAeYyixwv4q3qhY4/h/b2A+Mb2Sd+QM5ver/WJ19nQAAAABJRU5ErkJggg==";
        private const string layers = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAABrklEQVRYCe2VvUrEQBSFzaqwglgoVjba2/iDta2tnaUI4gPYrxY+gP3C+hKKYG23Ymul2FkIihYKIvE7MgMh683OTMBtcuBjJnPvuXc2m0myPM/HRqnWKJurd7OB5g7UuQNtnqGuQ/M06RgmsISnD16aay26VrSBJlvwDGVpTbGomjHJLYp34BssKdYB5QbVDkqi2BxcQKiUK8/Q+kMTKLIK9xAreeSt7FEZxLwHH5AqeVXD7FPnGIYeu+qvXdXuXGyN8QFipb9gBcxfr1hlsGCOfQjP8c4W/GYfM1AwT7u5jtYRxBxD7zX7mAHXdJvxBY7Bn+2QF9E4+SfwCqph9jEDznTI+AXSJfizrdfujRad+oyLoHrzcAWSvKph9jEDBdMm8yeQHmEd5GtD16G51jZAOZI88mrdxAyUTAtcX4P0CftQ9h64GMNvrjzlnIHrgYUK0ySxU/DqMZlynPlFRuUoN6h2UFKp2A7X7yDdOjTXmmJRNaOSC8WXmd+Bl+Zai66XyZSoGXw9591lfEupU2cD6pe5psm/YiJl1wVPcmNf4z++hr7Xn2OzgeYO/ADhoJ0LKtfgoAAAAABJRU5ErkJggg==";
        private const string tag = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAAA9UlEQVRYCe2UMQ7CMAxFKYxcgqUSA1dgQLDQgQNyE8TEPRiiwgwTCxJq/++QhdRJBhMGf+lJbe3Ev1biquu6SUlNSxZnbTNgHbAO/H0HGlzVFnBapeCQV4N0cRIK3BDL1QML1kDa18cqJgoSg8K6N2IHcBJyhpCWAW5O8xtw4cuYNA2w5gcswZUvIWnfghmKnsE8VJzftA2wxgIc+RDSLwyE6vpv2meAhRxYgRf4kraBooeQ13ALRm8A26F1BjiI9kCcASkG7kzK1BP5OxCdgsO+kZndIN6CVDkk1sDP+thz7BBm/nx+utYZSHZiBqwD1oHiHegB4UkSNS6TXloAAAAASUVORK5CYII=";

        private const string staticOn = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAABxElEQVRYCe1WO07DQBCNkZBM7sMF0gRuEUruQMEREAUXoKQLBd+e04QWCiTznr2zmt3Mjm0UksYjjXZ+O+/t+Fs1TTM7pBwdEpzYE4FpAv81gSXurw30FlpBy8LHcMe6RL8vqMgdjApq4ngTWID2Gjov0zczp4jWKnMJ+0b5qVlihvgjlPIOnUPNE6j4GexV8K+xavmEY+43g6GYoG+hyytWjwTBOfYf6AWUfYXEN2zmTSwzqIo1iRfET1RO9go4Uq1oEleInEOldmvdChjFJEFwSk4iB++qukms4PT27y0ITXhyIfEMuw7xe6yW8HIUx45cxI2GDhZskiA45QlKEsfQB6iWweDYNBtDgLUE/QhoFolR4H8hwLESREST4OUYNHbUxYNHQwcLdg6OslaExJhesTYaaOXZJfDAId4TXg8zZwYzMh44L4fcE7xBrfeEi+EmAxF9zRGKwjjJ1VB5OvL3RG//3oIIlxoCLvv1e4IkvNe27GnXxMFGy0+hu6eAJ89rNQl+QwaRyJtYviaQnzyvJyg/XJRBJPIGu/BJgp9wCj/pbk832bfZyZPEGrpwalrs9lcp/UXZr+f9ku2FyURgmsAv12M/P0VUNfwAAAAASUVORK5CYII=";
        private const string staticOff = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAABRklEQVRYCe1WMQ7CMAykCAkexB8oGzwBZn7AwgcQAw9h5kGMXWknwrnUUZo2SU2DKqFasurYTu7ixG0TpdRkSJkOCU7YI4GxAn9ZgRSXO4NeoAnUL/QeiKhrrFVAWa4wEqgTI/YRLLHdubHlA+yzMW6aPnaCGO18V+Wf8DQlw8BZAWfAN8mKcdlf8O+rGJPIMV5Z+TXM2sCX6IgxOMKlmCSO8HjBEZ/0IWCDfygoRST4OILrf3sJqdVuUPPC8QUrYDx4EHxSGYSaIt9sNQy1PGEFy44cjakN0+mxo4ITjoRAdHAJAQKnlmoTcdmxiN64NkynZfvAg31urdXAazisCT8FJ6zyQ+FplRyxRUucWm0DvbfERK4QgbZf5mjgxFT6IooKLiVA4Fto77ITMMuMDccz/EfjmNjVLT2Crut2zhsJjBUYvAJvIjx8JkSpDJcAAAAASUVORK5CYII=";
        private const string activeOn = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAABSklEQVRYCe2VsUoDQRCGPVELRbDwDdJr4wskr5XmmiBptU3jK/gEYmWr72CqgHVAOP8vzELYmz02h7ApduDn9v75Z2Zvbm+u6brupKSdlixO7bqB2oHagbMRnyEx98KdcGvxG12/hE/h17i8C4MoExPpnoUfIWX40KDNypsjOleyhbAVcg0tMcQO1hh0KvhGeBNiexUxFa4MrOFiI5YcyTpJh4IuhQ8htrmIVBy+2MhBLjfGJU28ijPpnqck5kJYCt8G1nD4vE6Qy63lkhLPBM9oNTGPjhMOHxrPyNmr1yNM9O5lEHdt/rXjhyMfGs/I2at3tJOwTUyRB+NfHH/ggiaWtDGxu/faYlzRQ8i7Kv4ZsomigyicWMYpY7XIKA6b4Jr7M3oy7X5sct1I7B7OAfJff8djNjCwt8NdRzuIDn+UkRG1A7UDxTvwB4zvJLleQ2PrAAAAAElFTkSuQmCC";
        private const string activeOff = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAACEElEQVRYCe2Wvy5EURDGLetPVgQJT6CUEA9Agqh5Go1GsxEFjUoioVLQiJJGaHRCJJ5AQYTOnwTr9yXnbM6eO+dm7WZDsZN8Ofd8M2dmdu6cuVuoVCodfymdfxlcsdsJtCvwryuwQJMegZK6tVVSyJkDCr4ITsESeAUSVW0KjINRt39ivQHX4BPUL0oggRL8CZBo1V622yAlLyi2wBhI+a3hazbGISuJbuwOQZ58oCwD2ebGyFVyeBicAy++EkWIadDvMMt6DGI5gxgCyThJBYf06y9BLD4J6+xKbMxePvzry5zJEBh7bs9w9uY4n0QX+02wDnqAzlqV2HE677u6Vh8igwX2lixBKrgkTGKf/RqQP70OS+YgM/EyhDO6sDzADQKV893pwyQ24ORvwOniRb2UiZeahKk/Cd9ccM2DZ3fR/bDqZb/suNRSMBVWVnAqlyUqr36Fyh2Kr4R0M6EieJ7nOVOBDBEYqXFiUYPpjBpOjXcPrNdxAB/KLhszlkk649Q11FULz8nOakyfRMPXUEE0RM5ALKqEXocGkRpzEVhXdBW+4UHkf6XGaRlovNYrYU94P+ZqkkSxeH1g9KHRByclDyhunbKuJAoYm7cjhyyimwQTYAR8gUdwB65AH9CnXFc0/pRDRaIEWoCwMY/y/LciuPepJBRcY91zmbWRVxDVsLltahQ35/UXp9sJtCvwA5nkXRRXmT2oAAAAAElFTkSuQmCC";
        private const string lockOn = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgBAMAAACBVGfHAAAAJFBMVEUAAAD///////////////////////////////////////////+0CY3pAAAAC3RSTlMAG0vz4+K+tXf5hPH/P1UAAABYSURBVCjPY6AhYEo2U0ARWL179y4UBTsbJWYjK2HbwcDQnYAkwLyRgUHaAEmAewMI4xFQ3Q0GQXABb4jAFrjAbiggTaBQHE1AgBGPAEIL+dYinI7pOboBALjTVXjhsKb8AAAAAElFTkSuQmCC";
        private const string lockOff = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgBAMAAACBVGfHAAAAJFBMVEUAAAD///////////////////////////////////////////+0CY3pAAAAC3RSTlMAG0vz4+K+tXf5hPH/P1UAAABXSURBVCjPY6AhYEo2U0ARWL179y4UBTsbJWYjK2HbwcDQnYAkwLyRgUHaAM1c7g14BFR3g0EQXMAbIrAFLrAbCkgTKBRHExBgxCOA0EK+tQinY3qObgAArOBUAsnPrEAAAAAASUVORK5CYII=";
        private const string childToggleOn = "iVBORw0KGgoAAAANSUhEUgAAABIAAAASCAYAAABWzo5XAAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAAsTAAALEwEAmpwYAAAAB3RJTUUH4QULAAkpnV1J0wAAAHpJREFUOMvt07EJhFAMgOGXw0psXcrKFZzGeRzDHax0CMvvmisOEXk+LP0hRSD5SQhJ6LDJZ0OXjmB1n/XoCUgFRET855/0EK/oFT0lWgr6lrNfqzHd+LMJ9akeDcYMyYjmclZU6C8kPars5dFixv6LGW3xFTBgyK3/AgxhFqUwd4v5AAAAAElFTkSuQmCC";
        private const string childToggleOff = "iVBORw0KGgoAAAANSUhEUgAAABIAAAASCAYAAABWzo5XAAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAAsTAAALEwEAmpwYAAAAB3RJTUUH4QULAAkyFziAPwAAAG5JREFUOMvt06sNgEAQhOEdgiJoWqEHFLRBI9RDQhW0gAN5DeB+FOYMj1uB4CtgMpnsGtAAK/ctQGExYOO5ESjjoLcGIPcIAmjPHAFYmkpSyCzd5NVoN7Peo5HbRrOk+jNjd5KC+0G6vYjP0/4uHSU6sp7/OaNtAAAAAElFTkSuQmCC";
        private const string rendererOn = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgBAMAAACBVGfHAAAAFVBMVEUAAAD///////////////////////9Iz20EAAAABnRSTlMA5ImHSgYkLJCAAAAARUlEQVQoz2MYOMAilpbAwMCWlugAFWBMgwikCUAF2BJgNDkCcMCqKBSAYjNzWpoBhIWwWQCfAEILpqGUuxTD+5gBNGAAAE5iEyGKEpDSAAAAAElFTkSuQmCC";
        private const string rendererOff = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgBAMAAACBVGfHAAAAHlBMVEUAAAD///////////////////////////////////8kfJuVAAAACXRSTlMA8KodHOPitUoUm3BmAAAAT0lEQVQoz2MYOMCROXMCAwPnzGkNUAHWmRCBmQFQAc4JMJoUAWZFIQMQDQeOM2eKoNisWWw+CUVA0oFlIh4BhBYMQzGsJd/pCO9jBtCAAQBGTx9Z+1SkqgAAAABJRU5ErkJggg==";

        private const string fade = "iVBORw0KGgoAAAANSUhEUgAAALQAAAASCAYAAADyiPTBAAAABmJLR0QAAAAAAAD5Q7t/AAAACXBIWXMAAAsTAAALEwEAmpwYAAAAB3RJTUUH4gIGFTIzgL5IaQAAAjVJREFUaN7tWtGuwyAI5WD3cP//f8deusQQFbC2s7eSLFqmVPFAT60QkT+qC4xrrUPnf622CLTXOhTqnOnSXvJeT6qeiOi167b991L1VCi37Hor2MBectYW2X/fcSIbk55bPvZ8jrlO61HRU6MfVfrmfXJbUulPDdslu7mt0hj0WhIRgaktQjGRgbZGixg6ya4l0K93HBIcpzh0tbl42/Ws7fuEden1sbAx4dlASQMD7Kij36ruCQAxAPFr/77vvqbsiHh5AMhlcMaQQJ+7+e/qxBISPvnmd1ksGO8JFvilI0CWXAToJT7wl4JAGoGxgH0RoOUfOhtOXWR3BwsuK0PPDPjStpUHyJ7tKAQCbclA2R7mZE+mRaGs7eO2MjoF7vUUmrYy9I+ztZWxUQmAHnAvGRAcfHI0YRKwRuiG9UXN0ln2rvINbg7QnvFjG2AYN3GaZ36ez71WnysWbskgDo3Jo9qz0+GhGPqMQt6Gs6fb95orTwJ91oALgZIcXHzJIA6NgUDDBECugZsrYAfVDwBFqEzkZTRKnSJPHTJ2bHiyZIZRgAb5TsfhYmCSc3GslzgP+DTQWWVjna2pEhweIM2cia2xJQdOIskOR/CynUnQO9uhwxktJ1qgrYEXHf1LiYIrc7k7p8YkY5Aoh8YB3REw9zoVjTJKKaC4cquv5sdcaMdknwUmxcmfzqND899OisDIQf8zdjIiYNagSxUwc/Z4TYqCeDJ6Lx+mxssmHbAzW/YdQl8/eE1MX9J1OAoAAAAASUVORK5CYII=";
        #endregion

        public static readonly GUIContent prefabApplyContent;
        public static readonly GUIContent staticContent;
        public static readonly GUIContent lockContent;
        public static readonly GUIContent activeContent;
        public static readonly GUIContent rendererContent;
        public static readonly GUIContent tagContent;
        public static readonly GUIContent layerContent;

        public static readonly Color normalColor;
        public static readonly Color backgroundColorEnabled;
        public static readonly Color backgroundColorDisabled;
        public static readonly Color selectedFocusedColor;
        public static readonly Color selectedUnfocusedColor;
        public static readonly Color childToggleColor;

        public static readonly GUIStyle iconButton;
        public static readonly GUIStyle newToggleStyle;
        public static readonly GUIStyle staticToggleStyle;
        public static readonly GUIStyle applyPrefabStyle;
        public static readonly GUIStyle lockToggleStyle;
        public static readonly GUIStyle activeToggleStyle;
        public static readonly GUIStyle rendererToggleStyle;
        public static readonly GUIStyle miniLabelStyle;
        public static readonly GUIStyle tagStyle;
        public static readonly GUIStyle layerStyle;

        public static readonly GUIStyle labelNormal;
        public static readonly GUIStyle labelDisabled;
        public static readonly GUIStyle labelPrefab;
        public static readonly GUIStyle labelPrefabDisabled;
        public static readonly GUIStyle labelPrefabBroken;
        public static readonly GUIStyle labelPrefabBrokenDisabled;

        public static readonly Texture2D treeLineTexture;
        public static readonly Texture2D treeTeeTexture;
        public static readonly Texture2D treeElbowTexture;

        public static readonly Texture2D infoIcon;
        public static readonly Texture2D warningIcon;
        public static readonly Texture2D errorIcon;
        public static readonly Texture2D monobehaviourIconTexture;

        public static readonly Texture2D fadeTexture;
    }
}
