using System;
using System.Collections.Generic;
using System.Text;
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
