# Traveling Cart Scanning

## Classes
* [`TravelingCart`](./TravelingCart.cs) - base traveling cart
    * `TravelingCart.GetStock(int seed)` - returns the stock for the given `seed`
    * `TravelingCart.GetStock(int gameSeed, int day)` - returns the stock for the given `gameSeed` and `day`
    * the stock is a `HashSet<CartItem>`
    * `CartItem` contains fields `Id`, `Cost`, `Quantity` for the specific item
* [`CompressedTravelingCart`](./CompressedTravelingCart.cs) - Compressed form of the traveling cart, runs slightly faster/allows dumping a much smaller amount of data to disk as each item in the stock can be represented with only a `uint16`
    * `CompressedTravelingCart.GetStock(int seed)` - returns the stock for the given `seed`
    * `CompressedTravelingCart.GetStock(int gameSeed, int day)` - returns the stock for the given `gameSeed` and `day`
    * the stock is a `HashSet<CompressedCartItem>`
    * `CompressedCartItem` is a wrapper around a `uint16` with the same `Id`, `Cost`, `Quantity` fields as `CartItem`

## Example Usage

This is an example from [VaultCartSeeding.cs](../VaultCartSeeding.cs) where we want to find a seed where the traveling cart has the green bean, solar essence, and void essence, and at least one of the crab pot items not findable on the beach.

We define a function `ValidCart(seed)` to implement our check, and with this we can scan over the range of seeds. An example parallel implementation is available in [VaultCartSeeding.cs](../VaultCartSeeding.cs). 

```c#
using System.Linq;
...

static HashSet<int> NeededItems = new HashSet<int>()
{
    188, 768, 769
    // green bean / solar essence / void essence
};
static HashSet<int> OneOfTheseItems = new HashSet<int>()
{
    715, 716, 717, 720, 721, 722
    // lobster / crayfish / crab / shrimp / snail / periwinkle
};
static bool ValidCart(int seed)
{
    var stock = CompressedTravelingCart.GetStock(seed)
        .Select(o => o.Id).ToHashSet();
    return (
        stock.IsSupersetOf(NeededItems) &&
        OneOfTheseItems.Any((o) => stock.Contains(o))
    );
}
```