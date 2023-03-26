using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hgs.Part;
using Hgs.System.Electrical;
using UnityEngine;

namespace test;

[TestClass]
public class ElectricalTests {
  // [TestMethod]
  // public void TestSingleProducerConsumerWithEnoughPower() {
  //   var bus = new Bus(Voltage.Low);
  //   var solar = new HgPartSolarPanel();
  //   solar.MaxOutput = 2;
  //   var user = new HgPartTmpPower();
  //   user.PowerUsed = 1;

  //   bus.AddProducer(solar);
  //   bus.AddConsumer(user);

  //   bus.PreTick(5);
  //   bus.Tick(5);
  //   Assert.IsTrue(user.Powered);
  // }
  
  // [TestMethod]
  // public void TestSingleProducerConsumerWithShortageOfPower() {
  //   var bus = new Bus(Voltage.Low);
  //   var solar = new HgPartSolarPanel();
  //   solar.MaxOutput = 2;
  //   var user = new HgPartTmpPower();
  //   user.PowerUsed = 3;

  //   bus.AddProducer(solar);
  //   bus.AddConsumer(user);

  //   bus.PreTick(5);
  //   bus.Tick(5);
  //   Assert.IsFalse(user.Powered);
  // }
  
  // [TestMethod]
  // public void TestMultiConsumer() {
  //   var bus = new Bus(Voltage.Low);
  //   var solar = new HgPartSolarPanel();
  //   solar.MaxOutput = 5;
  //   var userA = new HgPartTmpPower();
  //   var userB = new HgPartTmpPower();
  //   userA.PowerUsed = 2;
  //   userB.PowerUsed = 2;

  //   bus.AddProducer(solar);
  //   bus.AddConsumer(userA);
  //   bus.AddConsumer(userB);

  //   bus.PreTick(5);
  //   bus.Tick(5);
  //   Assert.IsTrue(userA.Powered);
  //   Assert.IsTrue(userB.Powered);
  // }

  // [TestMethod]
  // public void TestBatteryDrainAndCharge() {
  //   var bus = new Bus(Voltage.Low);
  //   var solar = new HgPartSolarPanel();
  //   solar.MaxOutput = 5;

  //   var battery = new FakeBattery(Voltage.Low, 20, 20);

  //   bus.AddProducer(solar);
  //   bus.AddStorage(battery);

  //   bus.PreTick(1); // max production: 5
  //   bus.Tick(1);

  //   // Expect to be able to draw 5 power from solar + 10 power from batteries.
  //   Assert.Equals(bus.TryDrawPower(15), 15);

  //   // Battery should have 10 watts left.
  //   Assert.Equals(battery.WattsStored, 10);

  //   // Now charge for a second without draining.
  //   bus.PreTick(1);
  //   bus.Tick(1);

  //   // The battery should have charged by 5 watts.
  //   Assert.Equals(battery.WattsStored, 15);
  // }
  
  // class FakeBattery : IStorage {

  //   public Voltage Voltage;

  //   public Voltage GetVoltage() {
  //     return Voltage;
  //   }

  //   public int WattsStored;
  //   public int WattsCapacity;

  //   public int GetWattsStored() {
  //     return this.WattsStored;
  //   }

  //   public int GetWattsCapacity() {
  //     return this.WattsCapacity;
  //   }
    
  //   public FakeBattery(Voltage voltage, int charge, int capacity) {
  //     this.Voltage = voltage;
  //     this.WattsStored = charge;
  //     this.WattsCapacity = capacity;
  //   }

  //   public int TryDrawPower(int wattsRequested) {
  //     int draw = Math.Min(WattsStored, wattsRequested);
  //     this.WattsStored -= draw;
  //     return draw;
  //   }

  //   public void OnCalculateProduction(int seconds) {}

  //   public void OnRecharge(int chargeWatts) {
  //     this.WattsStored += chargeWatts;
  //   }
  // }
}