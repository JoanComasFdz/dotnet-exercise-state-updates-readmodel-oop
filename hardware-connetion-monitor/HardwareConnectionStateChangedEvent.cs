﻿namespace hardware_connetion_monitor;

public record HardwareConnectionStateChangedEvent(string HardwareUnitId, HardwareConnectionState State, DateTime OccurredAt);