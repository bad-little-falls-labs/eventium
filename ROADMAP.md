# Eventium Development Roadmap

## Current State Summary

**What's Implemented (v0.1 Foundation):**

- ✅ `SimulationEngine` with event loop, scheduling (`Schedule`/`ScheduleIn`)
- ✅ `TimeModel` supporting Discrete and Continuous modes
- ✅ `World` with entity management
- ✅ `Entity` with component-based architecture (`IComponent`)
- ✅ `Event` and `EventQueue` (priority queue by time, then priority)
- ✅ `ISystem` interface for event handlers
- ✅ `IRandomSource` with seeded RNG for reproducibility
- ✅ Basic `MetricsRegistry` with counters
- ✅ `Scenario` class for scenario initialization
- ✅ Working discrete demo (`MovementSystem`)
- ✅ Working continuous demo (`ArrivalSystem`)

---

## Phase 1: Complete v0.1 Core (Near-term)

| Task                          | Description                                                                                                                | Priority | Status  |
| ----------------------------- | -------------------------------------------------------------------------------------------------------------------------- | -------- | ------- |
| **1.1 Add Unit Tests**        | Create `Eventium.Core.Tests` project with xUnit tests for `SimulationEngine`, `EventQueue`, `TimeModel`, `World`, `Entity` | High     | ✅ Done |
| **1.2 Metrics Integration**   | Wire `MetricsRegistry` into `SimulationEngine`; add histogram & gauge types                                                | Medium   | ✅ Done |
| **1.3 Event Tracing/Logging** | Add optional event logging with correlation IDs for debugging                                                              | Medium   |         |
| **1.4 XML Documentation**     | Add `<summary>` docs to all public APIs                                                                                    | Medium   | ✅ Done |

---

## Phase 2: Robustness & Usability (Short-term)

| Task                              | Description                                                                      | Priority |
| --------------------------------- | -------------------------------------------------------------------------------- | -------- |
| **2.1 Snapshot/Serialization**    | Implement `ISnapshot` for World + Engine state; JSON serialization for save/load | High     |
| **2.2 RNG State Capture**         | Include RNG state in snapshots for perfect reproducibility                       | High     |
| **2.3 Scenario Configuration**    | Add YAML/JSON scenario loader (entities, parameters, initial events)             | Medium   |
| **2.4 Validation**                | Scenario validation (missing entities, invalid references, constraint checks)    | Medium   |
| **2.5 Statistical Distributions** | Add `IRandomSource` extensions: Exponential, Poisson, Normal distributions       | Medium   |

---

## Phase 3: Advanced Features (Medium-term)

| Task                       | Description                                                                            | Priority |
| -------------------------- | -------------------------------------------------------------------------------------- | -------- |
| **3.1 Multi-Phase Turns**  | Support turn phases (e.g., "Upkeep → Actions → Resolution") for strategy-game patterns | Medium   |
| **3.2 System Ordering**    | Explicit system execution order / dependency graph                                     | Medium   |
| **3.3 Entity Queries**     | Add `World.Query<T>()` to find entities by component type                              | Medium   |
| **3.4 Event Cancellation** | Allow scheduled events to be cancelled before execution                                | Low      |
| **3.5 Branching/What-If**  | Fork simulation state for Monte Carlo / what-if scenarios                              | Low      |

---

## Phase 4: Observability & Analysis (Medium-term)

| Task                              | Description                                                | Priority |
| --------------------------------- | ---------------------------------------------------------- | -------- |
| **4.1 Time Series Metrics**       | Track metric values over simulation time                   | High     |
| **4.2 Export to CSV/Parquet**     | Export simulation results for analysis in Pandas/notebooks | High     |
| **4.3 Entity Lifecycle Tracking** | Track entity creation, state changes, destruction          | Medium   |
| **4.4 OpenTelemetry Integration** | Optional tracing spans for external observability          | Low      |

---

## Phase 5: Domain Libraries (Longer-term)

| Task                         | Description                                           | Priority |
| ---------------------------- | ----------------------------------------------------- | -------- |
| **5.1 Queueing Patterns**    | Reusable queue/service-station components             | Medium   |
| **5.2 Spatial/Grid Support** | Grid-based topology, pathfinding helpers              | Low      |
| **5.3 Resource Allocation**  | Resource pools, allocation/release patterns           | Low      |
| **5.4 Sports/Game Patterns** | Turn-based game framework (for baseball sim use case) | Low      |

---

## Phase 6: Packaging & DevOps

| Task                    | Description                                        | Priority |
| ----------------------- | -------------------------------------------------- | -------- |
| **6.1 NuGet Package**   | Publish `Eventium.Core` to NuGet (or private feed) | Medium   |
| **6.2 CI/CD Pipeline**  | GitHub Actions for build, test, publish            | High     |
| **6.3 Benchmarks**      | Performance benchmarks for event throughput        | Low      |
| **6.4 Sample Projects** | Complete example scenarios in separate repo/folder | Medium   |

---

## Recommended Next Steps (Immediate)

1. **Add test project** - Create `tests/Eventium.Core.Tests` with basic coverage
2. **Wire up metrics** - Add `Metrics` property usage in demos, demonstrate counter usage
3. **Add GitHub Actions CI** - Automate build & test on push
