using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Linq;
using BadgeReader;

namespace Encoder
{
    public class E2HttpProtocol
    {
        public BadgeElement[] BadgeElements { get; set; } = new BadgeElement[54];

        public List<Position> PanelPositions { get; set; } = new List<Position>();

        public E2HttpProtocol()
        {
            InitializePanelPositions();
            InitializeBadgeElements();
        }

        public static BigInteger DecodeBadges(List<Badge> badges)
        {
            BigInteger finalInt = 0;
            var resultInt = new List<int>();
            var e2HttpProtocol = new E2HttpProtocol();
            for (int i = 0; i < 10; ++i)
            {
                var panelPos = e2HttpProtocol.PanelPositions[i];

                var selectedBadges = new List<Badge>();
                foreach (var badge in badges)
                {
                    if (badge.Position.X >= panelPos.X && badge.Position.X <= panelPos.X + 4 &&
                        badge.Position.Y >= panelPos.Y && badge.Position.Y <= panelPos.Y + 10)
                    {
                        var x = badge.Position.X - panelPos.X;
                        var y = badge.Position.Y - panelPos.Y;
                        var selectedBadge = new Badge();
                        selectedBadge.Position = new Position(x, y);
                        selectedBadge.BadgeType = badge.BadgeType;
                        selectedBadges.Add(selectedBadge);
                    }
                }


                BadgeElement selectedBadgeElement = null;
                foreach (var badgeElement in e2HttpProtocol.BadgeElements)
                {
                    if (badgeElement.AllocatedBadges.Count != selectedBadges.Count)
                        continue;

                    bool found = true;
                    foreach (var selectedBadge in selectedBadges)
                    {
                        if (!badgeElement.AllocatedBadges.Any(s =>
                            s.Position.X == selectedBadge.Position.X && s.Position.Y == selectedBadge.Position.Y &&
                            s.BadgeType == selectedBadge.BadgeType))
                        {
                            found = false;
                            break;
                        }
                    }

                    if (found)
                    {
                        selectedBadgeElement = badgeElement;
                        break;
                    }
                }

                var index = e2HttpProtocol.BadgeElements.ToList().IndexOf(selectedBadgeElement);
                resultInt.Add(index);
            }


            for (int i = 0; i < 10; ++i)
            {
                finalInt += resultInt[i] * BigInteger.Pow(54, 10 - i - 1);
            }

            return finalInt;
        }


        public static List<Badge> GenerateBadges(BigInteger transactionInt)
        {
            var results = transactionInt.ToBaseX(54);
            while (results.Count < 10)
            {
                results.Insert(0, 0);
            }


            var badges = new List<Badge>();
            var e2HttpProtocol = new E2HttpProtocol();
            for (int i = 0; i < 10; ++i)
            {
                var panelPos = e2HttpProtocol.PanelPositions[i];
                foreach (var badgeElement in e2HttpProtocol.BadgeElements[results[i]].AllocatedBadges)
                {
                    var badge = new Badge();
                    badge.Position = new Position(panelPos.X + badgeElement.Position.X, panelPos.Y + badgeElement.Position.Y);
                    badge.BadgeType = badgeElement.BadgeType;
                    badges.Add(badge);
                }
            }

            return badges;
        }


        public void InitializePanelPositions()
        {
            PanelPositions.Add(new Position(0, 0));
            PanelPositions.Add(new Position(4, 12));
            PanelPositions.Add(new Position(8, 0));
            PanelPositions.Add(new Position(12, 12));
            PanelPositions.Add(new Position(16, 0));
            PanelPositions.Add(new Position(20, 12));
            PanelPositions.Add(new Position(24, 0));
            PanelPositions.Add(new Position(28, 12));
            PanelPositions.Add(new Position(32, 0));
            PanelPositions.Add(new Position(36, 12));
        }

        public void InitializeBadgeElements()
        {
            for (int i = 0; i < 54; ++i)
            {
                BadgeElements[i] = new BadgeElement();
            }

            Badge badge = null;

            // 0 - 7
            BadgeElements[0] = new BadgeElement();

            badge = new Badge {Position = new Position(2, 2), BadgeType = BadgeType.Small };
            BadgeElements[1].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(1, 3), BadgeType = BadgeType.Small };
            BadgeElements[2].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(0, 4), BadgeType = BadgeType.Small };
            BadgeElements[3].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(0, 6), BadgeType = BadgeType.Small };
            BadgeElements[4].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(0, 8), BadgeType = BadgeType.Small };
            BadgeElements[5].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(1, 9), BadgeType = BadgeType.Small };
            BadgeElements[6].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(2, 10), BadgeType = BadgeType.Small };
            BadgeElements[7].AllocatedBadges.Add(badge);

            // 8 - 19
            badge = new Badge { Position = new Position(3, 9), BadgeType = BadgeType.Small };
            BadgeElements[8].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(4, 8), BadgeType = BadgeType.Small };
            BadgeElements[9].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(4, 6), BadgeType = BadgeType.Small };
            BadgeElements[10].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(4, 4), BadgeType = BadgeType.Small };
            BadgeElements[11].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(3, 3), BadgeType = BadgeType.Small };
            BadgeElements[12].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(2, 4), BadgeType = BadgeType.Small };
            BadgeElements[13].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(1, 5), BadgeType = BadgeType.Small };
            BadgeElements[14].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(1, 7), BadgeType = BadgeType.Small };
            BadgeElements[15].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(2, 8), BadgeType = BadgeType.Small };
            BadgeElements[16].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(3, 7), BadgeType = BadgeType.Small };
            BadgeElements[17].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(3, 5), BadgeType = BadgeType.Small };
            BadgeElements[18].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(2, 6), BadgeType = BadgeType.Small };
            BadgeElements[19].AllocatedBadges.Add(badge);

            // median

            badge = new Badge { Position = new Position(1, 3), BadgeType = BadgeType.Median };
            BadgeElements[20].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(0, 4), BadgeType = BadgeType.Median };
            BadgeElements[21].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(0, 6), BadgeType = BadgeType.Median };
            BadgeElements[22].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(1, 7), BadgeType = BadgeType.Median };
            BadgeElements[23].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(2, 6), BadgeType = BadgeType.Median };
            BadgeElements[24].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(2, 4), BadgeType = BadgeType.Median };
            BadgeElements[25].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(1, 5), BadgeType = BadgeType.Median };
            BadgeElements[26].AllocatedBadges.Add(badge);

            // large
            badge = new Badge { Position = new Position(0, 4), BadgeType = BadgeType.Large };
            BadgeElements[27].AllocatedBadges.Add(badge);

            // 28 - 33
            badge = new Badge { Position = new Position(2, 2), BadgeType = BadgeType.Small };
            BadgeElements[28].AllocatedBadges.Add(badge);
            badge = new Badge { Position = new Position(2, 10), BadgeType = BadgeType.Small };
            BadgeElements[28].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(0, 4), BadgeType = BadgeType.Small };
            BadgeElements[29].AllocatedBadges.Add(badge);
            badge = new Badge { Position = new Position(4, 8), BadgeType = BadgeType.Small };
            BadgeElements[29].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(0, 8), BadgeType = BadgeType.Small };
            BadgeElements[30].AllocatedBadges.Add(badge);
            badge = new Badge { Position = new Position(4, 4), BadgeType = BadgeType.Small };
            BadgeElements[30].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(2, 2), BadgeType = BadgeType.Small };
            BadgeElements[31].AllocatedBadges.Add(badge);
            badge = new Badge { Position = new Position(0, 8), BadgeType = BadgeType.Small };
            BadgeElements[31].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(3, 3), BadgeType = BadgeType.Small };
            BadgeElements[32].AllocatedBadges.Add(badge);
            badge = new Badge { Position = new Position(1, 9), BadgeType = BadgeType.Small };
            BadgeElements[32].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(4, 4), BadgeType = BadgeType.Small };
            BadgeElements[33].AllocatedBadges.Add(badge);
            badge = new Badge { Position = new Position(2, 10), BadgeType = BadgeType.Small };
            BadgeElements[33].AllocatedBadges.Add(badge);

            // 34 - 39
            badge = new Badge { Position = new Position(2, 2), BadgeType = BadgeType.Small };
            BadgeElements[34].AllocatedBadges.Add(badge);
            badge = new Badge { Position = new Position(4, 8), BadgeType = BadgeType.Small };
            BadgeElements[34].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(1, 3), BadgeType = BadgeType.Small };
            BadgeElements[35].AllocatedBadges.Add(badge);
            badge = new Badge { Position = new Position(3, 9), BadgeType = BadgeType.Small };
            BadgeElements[35].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(0, 4), BadgeType = BadgeType.Small };
            BadgeElements[36].AllocatedBadges.Add(badge);
            badge = new Badge { Position = new Position(2, 10), BadgeType = BadgeType.Small };
            BadgeElements[36].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(0, 4), BadgeType = BadgeType.Small };
            BadgeElements[37].AllocatedBadges.Add(badge);
            badge = new Badge { Position = new Position(4, 4), BadgeType = BadgeType.Small };
            BadgeElements[37].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(0, 6), BadgeType = BadgeType.Small };
            BadgeElements[38].AllocatedBadges.Add(badge);
            badge = new Badge { Position = new Position(4, 6), BadgeType = BadgeType.Small };
            BadgeElements[38].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(0, 8), BadgeType = BadgeType.Small };
            BadgeElements[39].AllocatedBadges.Add(badge);
            badge = new Badge { Position = new Position(4, 8), BadgeType = BadgeType.Small };
            BadgeElements[39].AllocatedBadges.Add(badge);

            // 40 - 45
            badge = new Badge { Position = new Position(2, 2), BadgeType = BadgeType.Small };
            BadgeElements[40].AllocatedBadges.Add(badge);
            badge = new Badge { Position = new Position(1, 9), BadgeType = BadgeType.Small };
            BadgeElements[40].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(2, 2), BadgeType = BadgeType.Small };
            BadgeElements[41].AllocatedBadges.Add(badge);
            badge = new Badge { Position = new Position(3, 9), BadgeType = BadgeType.Small };
            BadgeElements[41].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(0, 4), BadgeType = BadgeType.Small };
            BadgeElements[42].AllocatedBadges.Add(badge);
            badge = new Badge { Position = new Position(3, 9), BadgeType = BadgeType.Small };
            BadgeElements[42].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(0, 4), BadgeType = BadgeType.Small };
            BadgeElements[43].AllocatedBadges.Add(badge);
            badge = new Badge { Position = new Position(4, 6), BadgeType = BadgeType.Small };
            BadgeElements[43].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(0, 8), BadgeType = BadgeType.Small };
            BadgeElements[44].AllocatedBadges.Add(badge);
            badge = new Badge { Position = new Position(4, 6), BadgeType = BadgeType.Small };
            BadgeElements[44].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(0, 8), BadgeType = BadgeType.Small };
            BadgeElements[45].AllocatedBadges.Add(badge);
            badge = new Badge { Position = new Position(3, 3), BadgeType = BadgeType.Small };
            BadgeElements[45].AllocatedBadges.Add(badge);

            // 40 - 45
            badge = new Badge { Position = new Position(3, 3), BadgeType = BadgeType.Small };
            BadgeElements[46].AllocatedBadges.Add(badge);
            badge = new Badge { Position = new Position(2, 10), BadgeType = BadgeType.Small };
            BadgeElements[46].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(1, 3), BadgeType = BadgeType.Small };
            BadgeElements[47].AllocatedBadges.Add(badge);
            badge = new Badge { Position = new Position(2, 10), BadgeType = BadgeType.Small };
            BadgeElements[47].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(1, 3), BadgeType = BadgeType.Small };
            BadgeElements[48].AllocatedBadges.Add(badge);
            badge = new Badge { Position = new Position(4, 8), BadgeType = BadgeType.Small };
            BadgeElements[48].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(0, 6), BadgeType = BadgeType.Small };
            BadgeElements[49].AllocatedBadges.Add(badge);
            badge = new Badge { Position = new Position(4, 8), BadgeType = BadgeType.Small };
            BadgeElements[49].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(0, 6), BadgeType = BadgeType.Small };
            BadgeElements[50].AllocatedBadges.Add(badge);
            badge = new Badge { Position = new Position(4, 4), BadgeType = BadgeType.Small };
            BadgeElements[50].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(2, 10), BadgeType = BadgeType.Small };
            BadgeElements[51].AllocatedBadges.Add(badge);
            badge = new Badge { Position = new Position(4, 4), BadgeType = BadgeType.Small };
            BadgeElements[51].AllocatedBadges.Add(badge);

            // 52, 53
            badge = new Badge { Position = new Position(2, 2), BadgeType = BadgeType.Small };
            BadgeElements[52].AllocatedBadges.Add(badge);
            badge = new Badge { Position = new Position(0, 8), BadgeType = BadgeType.Small };
            BadgeElements[52].AllocatedBadges.Add(badge);
            badge = new Badge { Position = new Position(4, 8), BadgeType = BadgeType.Small };
            BadgeElements[52].AllocatedBadges.Add(badge);

            badge = new Badge { Position = new Position(0, 4), BadgeType = BadgeType.Small };
            BadgeElements[53].AllocatedBadges.Add(badge);
            badge = new Badge { Position = new Position(4, 4), BadgeType = BadgeType.Small };
            BadgeElements[53].AllocatedBadges.Add(badge);
            badge = new Badge { Position = new Position(2, 10), BadgeType = BadgeType.Small };
            BadgeElements[53].AllocatedBadges.Add(badge);
        }
    }

    public class BadgeElement
    {
        public List<Badge> AllocatedBadges { get; set; } = new List<Badge>();
    }
}
