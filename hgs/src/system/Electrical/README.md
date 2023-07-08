# `Hgs.System.Electrical`

The electrical subsystem, which simulates the generation, storage, and consumption of electricity as a resource.

## Gameplay mechanics

HGS divides electricity as a resource into low-voltage and high-voltage power. All spacecraft have a low-voltage electrical system (a low voltage bus) while larger spacecraft will additionally have a high-voltage bus.

Low voltage power is used for most normal vessel operations, such as navigation, reaction wheels, communications, engine ignition, smaller rover wheel motors, and other operational aspects of small spacecraft. High voltage power is used for electric or ionic engines, mining drills, resource processing and conversion.

A key gameplay difference is that only low-voltage power can be stored in batteries. High-voltage power is typically produced and consumed instantaneously, although more exotic storage of high-voltage power (thermal storage in molten salt batteries, for example) could make sense.

The high voltage bus will also supply the low voltage bus if needed.

## Simulation

Each electrical subsystem (LV and HV) are simulated as instances of a `Bus`, which consists of registered `PowerProducer`s, `PowerConsumer`s, and `PowerStorage`s. `Bus` implements `SimulatedSystem` and power is processed on each simulation tick.

At the beginning of a simulation tick, each `PowerProducer` calculates how much power it will produce during the tick, while each `PowerConsumer` calculates how much power it will attempt to draw.

Power is then allocated to each consumer in turn (so the ordering of consumers determines the priority of the power distribution) until either every consumer's demands are met or the production runs out. If `PowerStorage` is available, shortfalls in production are taken up by draining all available `PowerStorage` equally, while excess power production is distributed equally to `PowerStorage` that can accept it.

### `ProducerKind`s

Not all power production has the same mechanics. Each `PowerProducer` has a `ProducerKind`: `Free`, `Fueled`, `Storage`, or `HighVoltageBridge`.

`Free` producers are parts like solar panels or RTGs, which are always producing power regardless of demand. Engine alternators also fall into this category - although the power comes from burning fuel, its production is a side effect of the engine running.

`Fueled` producers are parts like fuel cells or generators, which consume another resource in order to produce power. Particularly, this resource should not be consumed if power is not demanded from `Fueled` producers.

`Storage` producers are `PowerStorage`s, and if a HV `Bus` is present then a `HighVoltageBridge` producer on the LV bus will convert as much power from the HV bus as needed to cover excess demand.