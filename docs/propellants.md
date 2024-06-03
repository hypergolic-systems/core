# Propellant

This doc explains the choices around propellant systems used in HGS, which are a bit different from those used in KSP itself.

HGS attempts to use propellant systems which are much more similar to their real analogues than KSP does. However, rather than use exact figures from literature, HGS opts to simplify where possible, and offer more round numbers.

In KSP, the bipropellant combination of Liquid Fuel & Oxidizer is interesting as both compounds are declared with the same density of 5 kg/U. This differs greatly from most real life analogues. Furthermore, the propellant ratio is given in terms of volume, not mass (but in the case of LF & Ox they're equivalent), and is 0.9 LF to 1.1 Ox, or approximately 1:1.22.

## Volume as the primary measurement

HGS does not use different parts for tanks containing different resources. Instead, it models the concept of a tank part with a given available volume, which can be subdivided into one or more internal tanks. Each internal tank can hold a specific resource (assuming compatibility with that part's design - not all tanks can hold cryogenic fuels for example).

To make it easier for players to work with tanks of different resources, HGS always reports resource quantities in terms of _volume_, not mass. Tank parts are subdivided into tanks by volume, and each individual tank is reported in terms of the volume of a given resource it contains.

To make it easier to work with bipropellant systems, mixing ratios are also specified by volume.

## Liquid Fuel & Liquid Oxygen

In HGS, Liquid Fuel is roughly analogous to RP-1, with a density of 0.9 kg/L. Its bipropellant oxidizer is Liquid Oxygen, with a density of 1.1 kg/L.

In real life, RP-1 and LOX mix at a mass ratio of ~2.54:1. HGS uses a volumetric ratio of 3:1 for easier math, which corresponds to a mass ratio of 2.45:1.

As an example, the Rockomax Jumbo-64 tank in KSP contains 2,880 U () of LF and 3,520 U of oxidizer, for a total fuel weight of 32 metric tons. In HGS, this tank would still contain ~32 tons of propellants, which means the delta-v for any given engine running on this tank would be approximately the same. However, the difference in propellant specifications means that the tank would have a volume of ~32,000 L. For the sake of round numbers, we'll specify a tank of 25,500 L LF and 8,500 L LOX, for a total tank volume of 34,000 L and a combined propellant mass of 32.3t (thus, this tank is slightly buffered in HGS).

## Liquid Hydrogen

Liquid Hydrogen is another propellant in HGS, which is not especially dense at 0.07 kg/L. In real life, the Vulcain 2 uses a 6.1:1 LH2:LOx mass ratio, which is approximately a 96:1 volumetric ratio. For simplicity, HGS uses a 99:1 volumetric ratio. With this ratio, the Rockomax Jumbo-64 would have 340 L of LOX and 33,660 L of LH2 (assuming it could store cryogenic LH2).

## Hydrazine

HGS uses Hydrazine as a more accurate analogue to KSP's Monopropellant, with a density of 1 kg/L. 100 KSP units of Monopropellant correspond to 400 L of hydrazine.

## Xenon

Xenon is an interesting one. In KSP, the volumetric density of Xenon is approximately 1 kg/L, based on the relative tank size to other fuel tanks. However, the real-life Dawn mission used supercritical Xenon at a density of roughly 2 kg/L. HGS uses the more realistic figure, meaning that Xenon tanks get a significant buff in HGS, holding approximately twice the mass of Xenon as they did in vanilla KSP.