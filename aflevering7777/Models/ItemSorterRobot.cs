using System.Globalization;

namespace aflevering7777
{
    /// <summary>
    /// Henter fra a/b/c (1..3) og lægger i S.
    /// x = 0.1 * id  (meter). Y/Z er faste for enkelhed som i opgaven.
    /// </summary>
    public class ItemSorterRobot : Robot
    {
        public const string UrscriptTemplate = @"
def move_item_to_shipment_box():
  SBOX_X = 0.30
  SBOX_Y = 0.30
  ITEM_X = {0}
  ITEM_Y = 0.10
  DOWN_Z = 0.10

  def moveto(x, y, z = 0.0):
    movel(p[x, y, z, 0, 0, 0], a=0.5, v=0.2)
  end

  # over emne -> ned (samle) -> op -> over S -> ned (aflægn)
  moveto(ITEM_X, ITEM_Y, 0.30)
  moveto(ITEM_X, ITEM_Y, DOWN_Z)
  moveto(ITEM_X, ITEM_Y, 0.30)
  moveto(SBOX_X, SBOX_Y, 0.30)
  moveto(SBOX_X, SBOX_Y, DOWN_Z)
end

move_item_to_shipment_box()
";

        public void PickUp(uint itemId)
        {
            var x = 0.1 * itemId; // 1,2,3 -> 0.1,0.2,0.3
            var xt = x.ToString(CultureInfo.InvariantCulture);
            var prog = string.Format(UrscriptTemplate, xt) + "\n";
            SendUrscript(prog);
        }
    }
}