# Stardew Seed Scanning Tools in C\#

## Why C\# when we have python scripts?
Speed. For doing max int32 scans, this will be significantly faster than python (and also easier to parallelize).

On my machine scanning the Traveling Cart stock:
* python (single core): 21,000 seeds per second (28 hours for 2^31-1)
* C# (single core): 280,000 seeds per second (~13x faster, 2-2.5 hours for 2^31-1)
* C# (parallel): 1,800,000 seeds per second (~85x faster, 20 minutes for 2^31-1)

Remix bundle scanning:
* python (single core): 16,000 seeds per second (37 hours for 2^31-1)
* c# (single core): 315,000 seeds per second (~20x faster, 2 hours for 2^31-1)
* c# (parallel): 1,300,000 seeds per second (~80x faster, 28 minutes for 2^31-1)

When you need to do more complicated searches, that speed difference will be even more pronounced.

## Functions
* Remixed Bundle scanning (for details see [Remixed Bundles](Bundles/README.md))
* Traveling Cart scanning (for details see [Traveling Cart](TravelingCart/README.md))
* Location forage (docs in progress)

## Setup (in progress)

NOTE: this uses .NET 5 to be in line with Stardew Valley. .NET 5 is not supported anymore/you'll get compilation warnings but it'll still work assuming you have the .NET 5 SDK installed.

The SMAPI docs have some great instructions for setting up a C# development environment [here](https://stardewvalleywiki.com/Modding:Modder_Guide/Get_Started#Get_started). Assuming you have things set up, you should be able to clone this repo and start setting up your searches.

** NOTE:** I can't seem to get the post-build command to correctly copy the `data` folder into the `bin` folder cross-platform. After you build, you'll need to manually copy the `data` folder into the `bin` folder (under either the `bin/Debug/net5.0/` or `bin/Release/net5.0/` folder that is generated).

## TODO:
* [ ] Add docs for location forage