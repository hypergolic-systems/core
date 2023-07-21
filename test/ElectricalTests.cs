using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hgs.Core.System.Electrical;
using Hgs.Core.Virtual;

namespace test;

class MockPowerProducer : PowerProducer {
  public int OutputPerSecond;
  public int AvailableThisTick;
  public Voltage Voltage;

  public MockPowerProducer(int perSecond, Voltage voltage) {
    this.OutputPerSecond = perSecond;
    this.Voltage = voltage;
  }

  public ProducerKind Kind() {
    return ProducerKind.Free;
  }

  public Voltage GetVoltage() {
    return this.Voltage;
  }

  public void OnCalculateProduction(uint seconds, CompositeSpacecraft vessel) {
    this.AvailableThisTick = this.OutputPerSecond * (int) seconds;
  }

  public int TryDrawPower(int wattsNeeded) {
    var draw = Math.Min(wattsNeeded, this.AvailableThisTick);
    this.AvailableThisTick -= draw;
    return draw;
  }
}

class MockPowerConsumer : PowerConsumer {
  private int demandPerSecond;
  public int Demanded = 0;
  public int Received = 0;
  private Voltage Voltage;

  public MockPowerConsumer(int demandPerSecond, Voltage voltage) {
    this.demandPerSecond = demandPerSecond;
    this.Voltage = voltage;
  }

  public Voltage GetVoltage() {
    return this.Voltage;
  }

  public void OnCalculateDemand(uint seconds) {
    this.Received = 0;
    this.Demanded = this.demandPerSecond * (int) seconds;
  }

  public void OnPowerAvailable(Bus bus) {
    var watts = bus.TryDrawPower(this.Demanded);
    this.Demanded -= watts;
    this.Received += watts;
  }
}

[TestClass]
public class ElectricalTests {
  [TestMethod]
  public void TestSingleProducerConsumerWithEnoughPower() {
    var bus = new Bus(Voltage.Low);
    var producer = new MockPowerProducer(2, Voltage.Low);
    var consumer = new MockPowerConsumer(2, Voltage.Low);

    bus.AddProducer(producer);
    bus.AddConsumer(consumer);

    bus.PreTick(5, null);
    bus.Tick(5, null);
    Assert.AreEqual(consumer.Received, 10);
  }
  
  [TestMethod]
  public void TestSingleProducerConsumerWithShortageOfPower() {
    var bus = new Bus(Voltage.Low);
    var producer = new MockPowerProducer(2, Voltage.Low);
    var consumer = new MockPowerConsumer(3, Voltage.Low);

    bus.AddProducer(producer);
    bus.AddConsumer(consumer);

    bus.PreTick(5, null);
    bus.Tick(5, null);
    Assert.AreNotEqual(consumer.Demanded, 0);
  }
  
  [TestMethod]
  public void TestMultiConsumer() {
    var bus = new Bus(Voltage.Low);
    var producer = new MockPowerProducer(5, Voltage.Low);
    var consumerA = new MockPowerConsumer(2, Voltage.Low);
    var consumerB = new MockPowerConsumer(2, Voltage.Low);

    bus.AddProducer(producer);
    bus.AddConsumer(consumerA);
    bus.AddConsumer(consumerB);

    bus.PreTick(5, null);
    bus.Tick(5, null);
    Assert.AreEqual(consumerA.Demanded, 0);
    Assert.AreEqual(consumerB.Demanded, 0);
  }

  [TestMethod]
  public void TestBatteryDrainAndCharge() {
    var bus = new Bus(Voltage.Low);
    var producer = new MockPowerProducer(5, Voltage.Low);

    var battery = new TestBattery(0, 100);

    bus.AddProducer(producer);
    bus.AddStorage(battery);

    // 25 watts into the battery (5 watts/second * 5 seconds)
    bus.PreTick(5, null);
    bus.Tick(5, null);

    Assert.AreEqual(battery.GetWattsStored(), 25);

    // Expect to be able to draw 15 watts from the battery.
    Assert.AreEqual(bus.TryDrawPower(15), 15);

    // Battery should have 10 watts left.
    Assert.AreEqual(battery.GetWattsStored(), 10);

    // Now charge for a second without draining.
    bus.PreTick(1, null);
    bus.Tick(1, null);

    // The battery should have charged by 5 watts.
    Assert.AreEqual(battery.GetWattsStored(), 15);
  }

  class TestBattery : Battery {
    public TestBattery(int charge, int capacity) : base(0, 0) {
      Assert.IsTrue(charge >= 0);
      Assert.IsTrue(charge <= capacity);
      this.capacity = capacity;
      this.stored = charge;
    }
  }
}